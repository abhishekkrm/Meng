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

#include "stdafx.h"
#include <winsock2.h>

#include "Transmitter.h"
#include "Overlapped.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

// #define DEBUG_DisplayOutgoingPackets 

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			#pragma region Transmit

			void Transmitter::Transmit(QS::Fx::Network::AsynchronousSend request, int seqno)
			{
#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt1 = clock->Time;
#endif

				MyOverlapped* overlapped = (MyOverlapped *) poverlapped;
				ZeroMemory(&overlapped->overlapped, sizeof(WSAOVERLAPPED));

				int nbuffers = request.Data.Segments->Count;

#if defined(DEBUG_TagOutgoingStreams)
				nbuffers++;
#endif

				if (overlapped->nsenderbuffers < nbuffers)
				{
					delete overlapped->senderbuffers;

					while (overlapped->nsenderbuffers < nbuffers)
						overlapped->nsenderbuffers = 2 * overlapped->nsenderbuffers;
					overlapped->senderbuffers = new WSABUF[overlapped->nsenderbuffers];
				}

				int bufno= 0;

#if defined(DEBUG_TagOutgoingStreams)
				bufno++;
				overlapped->senderbuffers[0].buf = (char *) miniheader;
				overlapped->senderbuffers[0].len = 8;
				((int*) miniheader)[1] = seqno;
#endif

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt2 = clock->Time;
#endif

				for each (QS::Fx::Base::Block segment in request.Data.Segments)
				{
					switch (segment.type)
					{
					case QS::Fx::Base::Block::Type::Managed:
						{
							GCHandle bufferHandle = GCHandle::Alloc(segment.buffer, GCHandleType::Pinned);
							overlapped->senderbuffers[bufno].buf = 
								((char*) ((LPVOID) ((IntPtr) bufferHandle.AddrOfPinnedObject()))) + segment.offset;
							overlapped->senderbuffers[bufno].len = segment.size;
							bufno++;

							bufferHandles->Enqueue(bufferHandle);
						}
						break;

					case QS::Fx::Base::Block::Type::Unmanaged:
						{
							overlapped->senderbuffers[bufno].buf = ((char*) ((LPVOID) segment.address)) + segment.offset;
							overlapped->senderbuffers[bufno].len = segment.size;
							bufno++;
						}
						break;

					default:
						throw gcnew Exception("Unsupported type of memory block encountered.");
					}
				}

				data = request.Data;
				completionCallback = request.Callback;
				context = request.Context;

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt3 = clock->Time;
#endif

#if defined(DEBUG_DisplayOutgoingPackets)
				unsigned char _checksum = 0;
				System::Text::StringBuilder s;
				unsigned int _ppos = 0;
				for (unsigned int _ibuf = 0; _ibuf < nbuffers; _ibuf++)
				{
					WSABUF _buf = overlapped->senderbuffers[_ibuf];
					for (unsigned int _ichr = 0; _ichr < _buf.len; _ichr++)
					{
						unsigned char _c = _buf.buf[_ichr];
						_checksum ^= _c;
						if ((_ppos % 8) == 0)
							s.Append("\n" + _ppos.ToString("000000") + "-" + (_ppos + 7).ToString("000000"));
						s.Append(" ");
						s.Append(((unsigned int) _c).ToString("x2"));
						_ppos++;
					}
				}
				s.Append("\n\n");
				logger->Log("OUTGOING PACKET ( size = " + _ppos.ToString() + ", checksum = " + _checksum.ToString() + " ) : \n" + s.ToString());
#endif

				bool succeeded = false;
				DWORD numberOfBytes = 0;
				if (WSASend((SOCKET)((HANDLE) socket->Handle), overlapped->senderbuffers, nbuffers, 
					&numberOfBytes, 0,  (LPWSAOVERLAPPED) (&overlapped->overlapped), NULL) == SOCKET_ERROR)
				{
					int errorCode = WSAGetLastError();
					if (errorCode != WSA_IO_PENDING)
					{
						errorCallback("Could not initiate send operation, error: " + errorCode.ToString());
					}
					else
						succeeded = true;
				}
				else
				{
					succeeded = true;
					// errorCallback("Asynchronous send operation completed immediately, but it should not.");
				}

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt4 = clock->Time;
#endif

				if (!succeeded)
					Completed();

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt5 = clock->Time;

				cumulated_d1 += tt2 - tt1;
				cumulated_d2 += tt3 - tt2;
				cumulated_d3 += tt4 - tt3;
				cumulated_d4 += tt5 - tt4;
				nsamples++;

				if (tt5 > last_checked + 1)
				{
					owner->LogTransmitOverheads(this, tt1, cumulated_d1 / ((double) nsamples), cumulated_d2 / ((double) nsamples), 
						cumulated_d3 / ((double) nsamples), cumulated_d4 / ((double) nsamples));
					
					nsamples = 0;
					cumulated_d1 = cumulated_d2 = cumulated_d3 = cumulated_d4 = 0;
					last_checked = tt5;
				}
#endif
			}

			#pragma endregion

			#pragma region Completed

			void Transmitter::Completed()
			{
#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt1 = clock->Time;
#endif

				for each (GCHandle bufferHandle in bufferHandles)
					bufferHandle.Free();
				bufferHandles->Clear();

				int numberOfBytes = data.Size;
				QS::Fx::Base::ContextCallback^ callback = this->completionCallback;
				Object^ context = this->context;

				data.Clear();
				this->completionCallback = nullptr;
				this->context = nullptr;

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt2 = clock->Time;
#endif

				owner->RecycleTransmitter(this, numberOfBytes);

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				double tt3 = clock->Time;
#endif

				if (callback != nullptr)
					callback(context);

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionCompletionOverheads)
				double tt4 = clock->Time;

				completion_cumulated_d1 += tt2 - tt1;
				completion_cumulated_d2 += tt3 - tt2;
				completion_cumulated_d3 += tt4 - tt3;
				completion_nsamples++;

				if (tt4 > completion_last_checked + 1)
				{
					owner->LogTransmitCompletionOverheads(this, tt1, 
						completion_cumulated_d1 / ((double) completion_nsamples), 
						completion_cumulated_d2 / ((double) completion_nsamples), 
						completion_cumulated_d3 / ((double) completion_nsamples));
					
					completion_nsamples = 0;
					completion_cumulated_d1 = completion_cumulated_d2 = completion_cumulated_d3 = 0;
					completion_last_checked = tt4;
				}
#endif
			}

			#pragma endregion
		}
	}
}
