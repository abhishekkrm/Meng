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
using namespace System::Collections;
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

			class Overlapped2
			{
			public:

				Overlapped2()
				{
					ZeroMemory(this, sizeof(Overlapped2));
					wsabufs = new WSABUF[1];
					wsabufs->buf = (char*) &beacon;
					wsabufs->len = sizeof(struct Beacon);
				}

				~Overlapped2()
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
				Beacon beacon;
			};

			#pragma endregion

			#pragma region Class Coordinator

			public ref class Coordinator
			{
			public:

				static void Synchronize(IPAddress^ interfaceAddress, IPAddress^ multicastAddress, int portno, int ntransmissions, int batchsize)
				{
					Socket^ socket = gcnew Socket(AddressFamily::InterNetwork, SocketType::Dgram, ProtocolType::Udp);				
					socket->Bind(gcnew IPEndPoint(interfaceAddress, 0));
					socket->SetSocketOption(SocketOptionLevel::IP, SocketOptionName::MulticastTimeToLive, 1);
					socket->SetSocketOption(SocketOptionLevel::IP, SocketOptionName::MulticastLoopback, false);            
					socket->Connect(multicastAddress, portno);

					IntPtr completion_port = (IntPtr) CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0); 
					if ((HANDLE) completion_port == NULL)
						throw gcnew Exception("Could not create the completion port.");
					if (CreateIoCompletionPort(
						(HANDLE) socket->Handle, (HANDLE) completion_port, 0, 0) != (HANDLE) completion_port)
						throw gcnew Exception("Could not associate socket with the completion port.");

					Overlapped2 **overlapped = new Overlapped2*[ntransmissions];
					for (int ind = 0; ind < ntransmissions; ind++)
						overlapped[ind] = new Overlapped2();
					
					Core::Clock^ clock = Core::Clock::SharedClock;

					int ntransmitted = 0;
					for (int ind = 0; ind < ntransmissions; ind++)
					{
						overlapped[ind]->beacon.seqno = ind + 1;
						overlapped[ind]->beacon.time = clock->Time;
						int nbytes, errorcode;
						if (WSASend((SOCKET)((HANDLE) socket->Handle), overlapped[ind]->wsabufs, 1, 
							(LPDWORD) &nbytes, 0,  (LPWSAOVERLAPPED) (&overlapped[ind]->overlapped), NULL) == SOCKET_ERROR)
						{
							if ((errorcode = WSAGetLastError()) != WSA_IO_PENDING)
								throw gcnew Exception("Error " + errorcode.ToString());
						}
						else
							ntransmitted++;

						if (((ind % batchsize) == (batchsize - 1)) || (ind == (ntransmissions - 1)))
						{
							while (ntransmitted < (ind + 1))
							{
								LPOVERLAPPED poverlapped;
								DWORD nbytes;
								ULONG_PTR completion_key;
								if (GetQueuedCompletionStatus((HANDLE) completion_port, &nbytes, (PULONG_PTR) &completion_key, 
									&poverlapped, INFINITE) != FALSE && poverlapped != NULL)
								{
									ntransmitted++;
								}
								else
									throw gcnew Exception("Error");
							}			

							Thread::Sleep(1);
						}
					}

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
