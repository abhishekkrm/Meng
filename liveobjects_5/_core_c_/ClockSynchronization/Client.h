/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

#pragma once

// #include "..\\..\\..\\stdafx.h"
#include <winsock2.h>
#pragma comment(lib,"ws2_32.lib")

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Threading;

namespace QS
{
	namespace _core_c_
	{
		namespace ClockSynchronization
		{
			#pragma region Class Overlapped

			class Overlapped3
			{
			public:

				Overlapped3()
				{
					ZeroMemory(this, sizeof(Overlapped3));
					wsabufs = new WSABUF[1];
					wsabufs->buf = (char*) &beacon;
					wsabufs->len = sizeof(struct Beacon);
					localtime = double::NaN;
					remote_address_length = sizeof(struct sockaddr_in);
				}

				~Overlapped3()
				{
					delete wsabufs;
				}

				struct Beacon
				{
				public:

					int seqno;
					double time;
				};

				WSAOVERLAPPED overlapped;
				WSABUF* wsabufs;
				struct sockaddr_in remote_address;
				int remote_address_length;
				Beacon beacon;
				double localtime;
			};

			#pragma endregion

			#pragma region Class Client

			public ref class Client
			{
			public:

				static void Accept(QS::Fx::Logging::ILogger^ logger, 
					IPAddress^ interfaceAddress, IPAddress^ multicastAddress, int portno, int ntransmissions, int aggregationsize)
				{
					Socket^ socket = gcnew Socket(AddressFamily::InterNetwork, SocketType::Dgram, ProtocolType::Udp);				
					socket->Bind(gcnew IPEndPoint(interfaceAddress, portno));
					socket->SetSocketOption(SocketOptionLevel::IP, SocketOptionName::AddMembership, 
						gcnew MulticastOption(multicastAddress, interfaceAddress));

					IntPtr completion_port = (IntPtr) CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0); 
					if ((HANDLE) completion_port == NULL)
						throw gcnew Exception("Could not create the completion port.");
					if (CreateIoCompletionPort(
						(HANDLE) socket->Handle, (HANDLE) completion_port, 0, 0) != (HANDLE) completion_port)
						throw gcnew Exception("Could not associate socket with the completion port.");

					Overlapped3 **overlapped = new Overlapped3*[ntransmissions];
					for (int ind = 0; ind < ntransmissions; ind++)
						overlapped[ind] = new Overlapped3();

					Core::Clock^ clock = Core::Clock::SharedClock;

					for (int ind = 0; ind < ntransmissions; ind++)
					{
						DWORD nbytes = 0, flags = 0;
						if (WSARecvFrom((SOCKET)((HANDLE) socket->Handle), overlapped[ind]->wsabufs, 1, &nbytes, &flags, 
							(struct sockaddr *) &overlapped[ind]->remote_address, &overlapped[ind]->remote_address_length, 
							(LPWSAOVERLAPPED) &overlapped[ind]->overlapped, NULL) != SOCKET_ERROR)
							throw gcnew Exception("Error: receive completed synchronously.");
						else
						{
							int errorcode;
							if ((errorcode = WSAGetLastError()) != WSA_IO_PENDING)
								throw gcnew Exception("Error " + errorcode.ToString());
						}
					}

					int nreceived = 0;
					while (nreceived < ntransmissions)
					{
						LPOVERLAPPED poverlapped;
						DWORD nbytes;
						ULONG_PTR completion_key;
						if (GetQueuedCompletionStatus((HANDLE) completion_port, &nbytes, (PULONG_PTR) &completion_key, 
							&poverlapped, 1000) != FALSE && poverlapped != NULL)
						{
							nreceived++;
							Overlapped3* thisoverlapped = CONTAINING_RECORD(poverlapped, Overlapped3, overlapped);						
							thisoverlapped->localtime = clock->Time;												
						}
						else
						{
							if (nreceived > ntransmissions / 2)
								break;
						}
					}

					List<QS::_core_e_::Data::XY>^ datapoints = gcnew List<QS::_core_e_::Data::XY>();

					double sumtimes = 0, minvalue = double::PositiveInfinity;
					int nsamples = 0;
					for (int ind = 0; ind < ntransmissions; ind++)
					{
						if (!double::IsNaN(overlapped[ind]->localtime))
						{
							double delta = overlapped[ind]->localtime - overlapped[ind]->beacon.time;
							if (delta < minvalue)
								minvalue = delta;
							sumtimes += overlapped[ind]->localtime;
							nsamples++;
						}

						if (((ind % aggregationsize) == (aggregationsize - 1)) || (ind == (ntransmissions - 1)))
						{
							if (nsamples > 0)
								datapoints->Add(QS::_core_e_::Data::XY(sumtimes / nsamples, minvalue));
							sumtimes = 0;
							minvalue = double::PositiveInfinity;
							nsamples = 0;
						}
					}

					array<QS::_core_e_::Data::XY>^ datapoints_array = datapoints->ToArray();

					double a, b;
					QS::_core_e_::MyMath::LeastSquares::Fit(datapoints_array, a, b);

					logger->Log(nullptr, 
						"Synchronizing Clock\na = " + a.ToString("000000.000000000000") + "\nb = " +b.ToString("000000.000000000000"));

					clock->Correct((1 - a), (- b));

					socket->Close();
					CloseHandle((HANDLE) completion_port);
					for (int ind = 0; ind < ntransmissions; ind++)
						delete overlapped[ind];
					delete overlapped;
				}
			};

			#pragma endregion
		}
	}
}
