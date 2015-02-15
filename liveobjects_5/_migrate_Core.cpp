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
#include <tchar.h>

#include "_migrate_Core.h"
#include "Listener.h"
// #include "Sockets.h"
// #include "AsynchronousCall.h"
#include "File.h"
#include "ReceiveBuffer.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

// #define DEBUG_DisplayIncomingPackets
// #define DEBUG_DisplayOutgoingMessagesForUnmanagedApplication
// #define DEBUG_DisplayIncomingMessagesForUnmanagedApplication

namespace _migrate_QS
{
	namespace _migrate_CMS
	{
		namespace _migrate_Core
		{
			#pragma region Submitting back to the channel

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)

			void Core::_CommunicateToUnmanagedApplication()
			{
				bool communicated_something = false;
				while (!pendingoutgoing->IsEmpty() && !outgoingchannel->IsFull())
				{
					QS::Fx::Unmanaged::Message another;
					pendingoutgoing->Dequeue(&another);
					outgoingchannel->Enqueue(another);
					communicated_something = true;
				}

				if (communicated_something && outgoingchannel->blocked)
				{
					outgoingchannel->blocked = false;
					SetEvent((HANDLE) outgoingchannel->handle);
				}
			}

			void Core::_CommunicateToUnmanagedApplication(QS::Fx::Unmanaged::Message request)
			{
				pendingoutgoing->Enqueue(request);
				_CommunicateToUnmanagedApplication();
			}

#endif

			#pragma endregion

			#pragma region Processing messages from native application

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)

			void Core::ProcessMessagesFromUnmanagedApplication()
			{
				QS::Fx::Unmanaged::Message request;

				while (incomingchannel->Dequeue(&request))
					pendingincoming->Enqueue(request);

				while (pendingincoming->Dequeue(&request))
				{
					switch (request.type)
					{
					case QS::Fx::Unmanaged::Message::Data:
						{
							if (request.isresponse)
							{
								unsigned __int32 reference = ((unsigned __int32 *) &request.cookie)[0];
								Object^ listenerObj;
								if (references->TryGetValue(reference, listenerObj))
								{
									QS::_core_c_::Core::Listener^ listener = dynamic_cast<QS::_core_c_::Core::Listener^>(listenerObj);
									if (listener == nullptr)
										throw gcnew Exception("Received a malformed cookie; reference (" + reference.ToString() + ") does not point to a listener.");

									unsigned __int32 id = ((unsigned __int32 *) &request.cookie)[1];

									ReceiveBuffer^ buffer = listener->BufferController->Lookup(id);
									if (buffer == nullptr)
										throw gcnew Exception("Received a malformed cookie; no buffer with id (" + id.ToString() + ") is registered with this listener.");

									buffer->NumReferences--;
									listener->BufferController->Release(buffer);
								}
								else
									throw gcnew Exception("Received a malformed cookie; no listener with reference (" + reference.ToString() + ") could be found.");
							}
							else
							{
								if (outgoingmsgcallback != nullptr)
								{
									QS::_core_x_::Unmanaged::OutgoingMsg^ message = 
										gcnew QS::_core_x_::Unmanaged::OutgoingMsg(
											request.channel, System::IntPtr(request.data), request.size, request.respond, request.cookie);

#if defined(DEBUG_DisplayOutgoingMessagesForUnmanagedApplication)
									unsigned char _checksum = 0;
									System::Text::StringBuilder s;
									for (unsigned int _ppos = 0; _ppos < message->size; _ppos++)
									{
										unsigned char _c = ((unsigned char *) message->data.ToPointer())[_ppos];
										_checksum ^= _c;
										if ((_ppos % 8) == 0)
											s.Append("\n" + _ppos.ToString("000000") + "-" + (_ppos + 7).ToString("000000"));
										s.Append(" ");
										s.Append(((unsigned int) _c).ToString("x2"));
									}
									s.Append("\n\n");
									logger->Log("OUTGOING UNMANAGED ( size = " + message->size.ToString() + ", checksum = " + _checksum.ToString() + " ) : \n" + s.ToString());
#endif

									outgoingmsgcallback(message);
								}
							}
						}
						break;

					case QS::Fx::Unmanaged::Message::Log:
						{							
							System::Diagnostics::Debug::Assert(!request.isresponse);

							logger->Log(nullptr, gcnew String((wchar_t *) request.data));

							if (request.respond)
								_CommunicateToUnmanagedApplication(QS::Fx::Unmanaged::Message::ResponseTo(request));
						}
						break;
					}
				}
			}

#endif

			#pragma endregion

			#pragma region AcknowledgeOutgoingMsg

			void Core::AcknowledgeOutgoingMsg(QS::_core_x_::Unmanaged::OutgoingMsg^ message)
			{
				_CommunicateToUnmanagedApplication(
					QS::Fx::Unmanaged::Message::Response(
						QS::Fx::Unmanaged::Message::Data, message->data.ToPointer(), message->size, message->channel, message->cookie));
			}

			#pragma endregion

			#pragma region UnmanagedLoopback

			void Core::UnmanagedLoopback(QS::_core_x_::Unmanaged::OutgoingMsg^ message)
			{
				_CommunicateToUnmanagedApplication(
					QS::Fx::Unmanaged::Message::Message(
						QS::Fx::Unmanaged::Message::Data, false, message->data.ToPointer(), message->size, message->channel, message->cookie));
			}

			#pragma endregion

			#pragma region ProcessIncomingMsg

			void Core::ProcessIncomingMsg(QS::_core_x_::Unmanaged::IncomingMsg^ message)
			{
#if defined(DEBUG_DisplayIncomingMessagesForUnmanagedApplication)
				unsigned char _checksum = 0;
				System::Text::StringBuilder s;
				for (unsigned int _ppos = 0; _ppos < message->size; _ppos++)
				{
					unsigned char _c = (((unsigned char *) message->data.ToPointer()) + message->offset)[_ppos];
					_checksum ^= _c;
					if ((_ppos % 8) == 0)
						s.Append("\n" + _ppos.ToString("000000") + "-" + (_ppos + 7).ToString("000000"));
					s.Append(" ");
					s.Append(((unsigned int) _c).ToString("x2"));
				}
				s.Append("\n\n");
				logger->Log("INCOMING UNMANAGED ( size = " + message->size.ToString() + ", checksum = " + _checksum.ToString() + " ) : \n" + s.ToString());
#endif

				_CommunicateToUnmanagedApplication(
					QS::Fx::Unmanaged::Message::Message(
						QS::Fx::Unmanaged::Message::Data, true, 
						(void *)(((unsigned char *) message->data.ToPointer()) + message->offset), 
						message->size, message->channel, message->cookie));
			}

			#pragma endregion

			#pragma region Alarm processing

			void Core::FireAlarm(Alarm^ alarm)
			{
				alarm->Fire();
			}

			#pragma endregion

			#pragma region ProcessDowncalls

			void Core::ProcessDowncalls()
			{
				while (may_have_downcalls)
				{
					may_have_downcalls = false;					
					
					QS::_core_c_::Core::IRequest^ request = downcalls->Dequeue();
					
					while (request != nullptr)
					{
						QS::_core_c_::Core::IRequest^ nextrequest = request->Next;
						request->Next = nullptr;
						
						request->Process();
						request = nextrequest;
					}
				}
			}

			#pragma endregion

			#pragma region CheckCompletionPort

			bool Core::CheckCompletionPort(double waiting)
			{
				DWORD timeout = double::IsPositiveInfinity(waiting) ? INFINITE : (DWORD) ((int) Math::Ceiling(1000 * waiting));

				time = clock->Time;

#if defined(DEBUG_CollectCoreStatistics)
				double time1_GetQueuedCompletionStatus = time;
				if (timeout != INFINITE)
					ts_QueryCompletionStatusTimeouts->Add(time1_GetQueuedCompletionStatus, ((double) timeout) / 1000);
#endif

				if (timeout > 10)
				{
					will_process_downcalls = 0;			

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
					incomingchannel->blocked = true;
#endif
				}

				if (
					may_have_downcalls 
#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
					|| !incomingchannel->IsEmpty() || !pendingoutgoing->IsEmpty()
#endif
					)
				{
					timeout = 0;
					will_process_downcalls = 1;

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
					incomingchannel->blocked = false;
#endif
				}

				LPOVERLAPPED lpOverlapped;
				DWORD numberOfBytes;
				ULONG_PTR completionKey;
				bool completionOK = (GetQueuedCompletionStatus((HANDLE) completionPort, 
					&numberOfBytes, (PULONG_PTR) &completionKey, &lpOverlapped, timeout) != FALSE);

				will_process_downcalls = 1;

				time = clock->Time;
				if (timeout == INFINITE)
					timewarps_initialize();
				else
					timewarps_recheck(ProcessingContext::IO1, timeout + timeWarpThresholdForIO, timeout);

#if defined(DEBUG_CollectCoreStatistics)
				double time2_GetQueuedCompletionStatus = time;
				ts_QueryCompletionStatusMeasuredDelays->Add(
					time1_GetQueuedCompletionStatus, time2_GetQueuedCompletionStatus - time1_GetQueuedCompletionStatus);
				if (timeout != INFINITE)
					ts_QueryCompletionStatusTimeoutsVsMeasuredDelays->Add(
						((double) timeout) / 1000, time2_GetQueuedCompletionStatus - time1_GetQueuedCompletionStatus);
#endif

				bool something_got_processed = true;

				if (completionOK)
				{
					if (lpOverlapped != NULL)
					{
						MyOverlapped* overlapped = CONTAINING_RECORD(lpOverlapped, MyOverlapped, overlapped);						
						ProcessIO(overlapped, numberOfBytes, true);
					}
					else
					{
#if defined(DEBUG_RegisterCoreActions)
						RegisterAction(ActionType::Callback);
#endif

						void *pkey = (void*) completionKey;
						if (pkey != NULL)
						{
							GCHandle gcHandle = GCHandle::FromIntPtr((IntPtr) pkey);
							AsynchronousCall^ call = dynamic_cast<AsynchronousCall^>(gcHandle.Target);
							gcHandle.Free();							

							if (call != nullptr)
								call->Invoke();
						}
						else
						{
							ProcessDowncalls();
						}
					}
				}
				else
				{
					if (lpOverlapped != NULL)
					{
						MyOverlapped* overlapped = CONTAINING_RECORD(lpOverlapped, MyOverlapped, overlapped);						
						ProcessIO(overlapped, numberOfBytes, false);
					}
					else
					{
						if (GetLastError() != WAIT_TIMEOUT)
							Error("Could not dequeue a completion packet for reason other than timeout, error " + 
								WSAGetLastError().ToString());

						something_got_processed = false;
					}
				}

				return something_got_processed;
			}

			#pragma endregion

			#pragma region Processing I/O callbacks

			void Core::ProcessIO(MyOverlapped* overlapped, unsigned int num_bytes_transmitted, bool succeeded)
			{
				MyOverlapped::Operation operation = overlapped->operation;
				if (!succeeded)
					Error("An I/O operation of type (" + ((int) operation).ToString() + ") has failed, error " + WSAGetLastError().ToString());

				switch (operation)
				{
				case MyOverlapped::Send:
					{
						ProcessSendCompletion(overlapped, succeeded);
					}
					break;

				case MyOverlapped::Receive:
					{
						ProcessReadCompletion(overlapped, succeeded);
					}
					break;

				case MyOverlapped::FileIO:
					{
						ProcessFileIOCompletion(overlapped, num_bytes_transmitted, succeeded);
					}
					break;

				default:
					{
#if defined(DEBUG_RegisterCoreActions)
						RegisterAction(ActionType::UnknownIO);
#endif
					}
					break;
				}
			}

			#pragma endregion

			#pragma region Processing Send Completions

			void Core::ProcessSendCompletion(MyOverlapped* overlapped, bool succeeded)
			{
#if defined(DEBUG_CollectStatisticsForIO)
				double time1 = clock->Time;
				time = time1;
#endif

				__int32 reference = overlapped->reference;
				
				Object^ transmitterObj;
				if (references->TryGetValue(reference, transmitterObj))
				{
					Transmitter^ transmitter = dynamic_cast<Transmitter^>(transmitterObj);
					if (transmitter == nullptr)
						throw gcnew Exception("Internal error while processing send completion, reference (" + 
							reference.ToString() + ") does not point to a transmitter.");

					Socket^ socket = transmitter->Socket;

#if defined(DEBUG_CollectPeriodicStatistics) && defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)
					double tt1 = clock->Time;
					time = tt1;
#endif

					bool completionOK;
					DWORD numberOfBytesSent = 0;
					DWORD completionFlags = 0;
					completionOK = (WSAGetOverlappedResult(
						(SOCKET)((HANDLE) socket->Handle), (LPWSAOVERLAPPED) &overlapped->overlapped, 
						&numberOfBytesSent, FALSE, &completionFlags) != FALSE);

#if defined(DEBUG_CollectPeriodicStatistics) && defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)
					double tt2 = clock->Time;
					time = tt2;
				
					pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_WSAGetOverlappedResult += tt2 - tt1;
					pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_WSAGetOverlappedResult++;
#endif

					if (completionOK)
					{
#if defined(DEBUG_CollectPeriodicStatistics) && defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)
						double tt3 = clock->Time;
						time = tt3;
#endif

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)
						(transmitter->HighPriority ? completedPriorityTransmitters : completedTransmitters)->Enqueue(transmitter);
#else
						transmitter->Completed();
#endif

#if defined(DEBUG_CollectPeriodicStatistics) && defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)
						double tt4 = clock->Time;
						time = tt4;

						pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_TransmitterCompleted += tt4 - tt3;
						pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_TransmitterCompleted++;
#endif
					}
					else
					{
						int errorcode = WSAGetLastError();
						// System::Text::StringBuilder^ builder = gcnew System::Text::StringBuilder(1024);
						// int retval = QS::Win32::FormatMessage(
						//	FORMAT_MESSAGE_FROM_SYSTEM, IntPtr::Zero, errorcode, 0, builder, builder->Capacity, IntPtr::Zero);
						Error("Could not get overlapped \"send\" result for sender { " + transmitter->Controller->CombinedAddress->ToString() + 
							" }, error code (" + errorcode.ToString() + ") = \"" + "?" /* + builder->ToString() */ + "\".");
					}

#if defined(DEBUG_RegisterCoreActions)
					RegisterAction(ActionType::SendCompletion);
#endif
				}
				else
				{
					Error("Could not locate the transmitter corresponding to reference (" + reference.ToString() + ")");

					// TODO: Somehow cleanup.................

					delete overlapped;
				}

#if defined(DEBUG_CollectStatisticsForIO)
				double time2 = clock->Time;
				time = time2;
				ts_SendCompletionTotalProcessingOverheads->Add(time1, time2 - time1);
#endif

				timewarps_recheck(ProcessingContext::IO5, timeWarpThresholdForIO);
			}

			#pragma endregion

			#pragma region Processing Read Completions

			void Core::ProcessReadCompletion(MyOverlapped* overlapped, bool succeeded)
			{
#if defined(DEBUG_CollectStatisticsForIO)
				double time1 = clock->Time;
				time = time1;
#endif

				__int32 reference = overlapped->reference;
				
				Object^ listenerObj;
				if (references->TryGetValue(reference, listenerObj))
				{
					QS::_core_c_::Core::Listener^ listener = dynamic_cast<QS::_core_c_::Core::Listener^>(listenerObj);
					if (listener == nullptr)
						throw gcnew Exception("Internal error while processing received packet, reference (" + 
							reference.ToString() + ") does not point to a listener.");

					if (listener->Disposed)
					{
						Error("Not processing or reinitating I/O for listener " + listener->CoreAddress->ToString() + ", the listener has been disposed of.");
					}
					else
					{						
						Socket^ socket = listener->Socket;

#if defined(DEBUG_RegisterCoreActions)
						RegisterAction(ActionType::Received);
#endif

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)
						(listener->HighPriority ? priorityReceiveCompletions : receiveCompletions)->Enqueue(
							DeferredProcessReceive(overlapped, true, succeeded, listener, socket));
#else
						ProcessReceive(overlapped, true, succeeded, listener, socket);
#endif
					}
				}
				else
				{
					Error("Could not locate the listener corresponding to reference (" + reference.ToString() + ")");

					// TODO: Somehow cleanup.................

					delete overlapped;
				}

#if defined(DEBUG_CollectStatisticsForIO)
				double time2 = clock->Time;
				time = time2;
				ts_ReceiveCompletionTotalProcessingOverheads->Add(time1, time2 - time1);
#endif
			}

			#pragma endregion

			#pragma region Processing File I/O Completions

			void Core::ProcessFileIOCompletion(MyOverlapped* overlapped, unsigned int num_bytes_transmitted, bool succeeded)
			{
#if defined(DEBUG_LogWriteCompletionRates)
				numWriteCompletions++;
#endif

				__int32 reference = overlapped->reference;
				
				Object^ fileObj;
				if (references->TryGetValue(reference, fileObj))
				{
					File^ file = dynamic_cast<File^>(fileObj);
					if (file == nullptr)
						throw gcnew Exception("Internal error while processing file I/O completion, reference (" + 
							reference.ToString() + ") does not point to a file.");

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)
					completedFileIOs->Enqueue(DeferredCompletedFileIO(file, num_bytes_transmitted, overlapped));
#else
					file->CompletedWrite(overlapped);
#endif
				}
				else
				{
					Error("Could not locate the file corresponding to reference (" + reference.ToString() + ")");
					delete overlapped;
				}
			}

			#pragma endregion

			#pragma region ProcessReceive

			void Core::ProcessReceive(
				MyOverlapped* overlapped, bool completed, bool succeeded, Listener^ listener, Socket^ socket)
			{
#if defined(DEBUG_LogOnReceive)
				Error("__________ProcessReceive(completed = " + completed.ToString() + 
					", succeeded = " + succeeded.ToString() + ", address = " + listener->Address->ToString() + ")");
#endif

				if (completed)
				{
#if defined(DEBUG_CollectSocketWaitingStatistics)
					int nwaiting = 0;
					if (::ioctlsocket((SOCKET)((HANDLE) socket->Handle), FIONREAD, (u_long*) &nwaiting) != SOCKET_ERROR) 
					{
						listener->LogWaiting(clock->Time, nwaiting);
					}
#endif

					DWORD numberOfBytesReceived = 0;
					DWORD completionFlags = 0;
					if (WSAGetOverlappedResult((SOCKET)((HANDLE) socket->Handle), 
						(LPWSAOVERLAPPED) &overlapped->overlapped, &numberOfBytesReceived, FALSE, &completionFlags))
					{
						if (numberOfBytesReceived > 0)
						{
#if defined(OPTION_AllowPeriodicDroppingOfMessages)
							_update_dropping();
							if (!dropping_now)
								DispatchReceived(overlapped, numberOfBytesReceived, listener, true);
#else 
#if defined(OPTION_AllowPeriodicDroppingOfMessages_BySeqno)
							if (!check_dropping())
								DispatchReceived(overlapped, numberOfBytesReceived, listener, true);
#else
							DispatchReceived(overlapped, numberOfBytesReceived, listener, true);
#endif
#endif
						}
					}
					else
						Error("Could not get overlapped result, error " + WSAGetLastError().ToString());

					ZeroMemory(&overlapped->overlapped, sizeof(WSAOVERLAPPED));
					overlapped->remote_address_length = sizeof(struct sockaddr_in);
				}

#if defined(AllowSynchronousReceive)
				if (listener->DrainSynchronously)
				{
					while (true)
					{
						time = clock->Time;
						timewarps_recheck(ProcessingContext::IO2, timeWarpThresholdForIO);

						int nwaiting = 0;
						int errorCode = ::ioctlsocket((SOCKET)((HANDLE) socket->Handle), FIONREAD, (u_long*) &nwaiting);
						if (errorCode == SOCKET_ERROR) 
						{
							Error("Could not retrieve the number of available bytes on a receive socket.");
						}
						else
						{
#if defined(DEBUG_CollectSocketWaitingStatistics)
							listener->LogWaiting(clock->Time, nwaiting);
#endif

							if (nwaiting > 0) // TODO: Determine what value is good
							{
								int nreceived = ::recvfrom(
									(SOCKET)((HANDLE) socket->Handle), overlapped->wsabuf.buf, overlapped->wsabuf.len, 0, 
									(struct sockaddr*) &overlapped->remote_address, &overlapped->remote_address_length);

								if (nreceived > 0)
								{
#if defined(OPTION_AllowPeriodicDroppingOfMessages)
									_update_dropping();
									if (!dropping_now)
										DispatchReceived(overlapped, nreceived, listener, false);
#else
									DispatchReceived(overlapped, nreceived, listener, false);
#endif
								}
								else
								{
									if (!nreceived)
										Error("Collection has been closed.");
									else
										Error("Cannot receive, error " + WSAGetLastError().ToString());
									
									// TODO: Should act somehow intelligently.....
									break;
								}
							}
							else
								break;
						}
					}
				}
#endif

				DWORD numberOfBytes = 0;
				DWORD flags = 0;
				if (WSARecvFrom(
					(SOCKET)((HANDLE) socket->Handle), &overlapped->wsabuf, 1, &numberOfBytes, &flags, 
					(struct sockaddr*) &overlapped->remote_address, &overlapped->remote_address_length, 
					(LPWSAOVERLAPPED) (&overlapped->overlapped), NULL) != SOCKET_ERROR)
				{
#if defined(DEBUG_LogOnReceive)
					Error("WSARecvFrom completed synchronously");
#endif
				}
				else
				{
					int errorCode = WSAGetLastError();
					if (errorCode != WSA_IO_PENDING)
						Error("Could not initiate or reinitiate listening on the socket at the address " + 
							listener->CombinedAddress->ToString() + ", error code " + errorCode.ToString());
					else
					{
#if defined(DEBUG_LogOnReceive)
						Error("Good: WSA_IP_PENDING");
#endif
					}
				}

				time = clock->Time;
				timewarps_recheck(ProcessingContext::IO3, timeWarpThresholdForIO);
			}

			#pragma endregion

			#pragma region DispatchReceived

			void Core::DispatchReceived(MyOverlapped* overlapped, int numberOfBytesReceived, Listener^ listener, bool asynchronous)
			{
#if defined(DEBUG_DisplayIncomingPackets)
				unsigned char _checksum = 0;
				System::Text::StringBuilder s;
				unsigned int _ppos = 0;
				for (unsigned int _ichr = 0; _ichr < numberOfBytesReceived; _ichr++)
				{
					unsigned char _c = overlapped->wsabuf.buf[_ichr];
					_checksum ^= _c;
					if ((_ppos % 8) == 0)
						s.Append("\n" + _ppos.ToString("000000") + "-" + (_ppos + 7).ToString("000000"));
					s.Append(" ");
					s.Append(((unsigned int) _c).ToString("x2"));
					_ppos++;
				}
				s.Append("\n\n");
				logger->Log("INCOMING PACKET ( size = " + _ppos.ToString() + ", checksum = " + _checksum.ToString() + " ) : \n" + s.ToString());
#endif

#if defined(DEBUG_CollectReceiveTypes) || defined(DEBUG_CollectStatisticsForIO)
				double time1 = clock->Time;
				time = time1;
#endif

#if defined(DEBUG_CollectReceiveTypes)
				listener->LogReceive(time, asynchronous ? Listener::ReceiveType::Asynchronous : Listener::ReceiveType::Synchronous);
#endif

				__int64 encoded_as_int64 = (__int64)(*((unsigned __int32*) &overlapped->remote_address.sin_addr));

				IPAddress^ address;
#if defined(CatchAddressErrorsOnDispatch)
// #pragma warning (disable : 4700)
				try
				{
#endif
					address = gcnew IPAddress(encoded_as_int64);
#if defined(CatchAddressErrorsOnDispatch)
				}
				catch (Exception^ exception)
				{
					throw gcnew Exception("Cannot decode address { " + encoded_as_int64.ToString("x") + " }", exception);
				}
// #pragma warning (restore : 4700)
#endif

				int port = ntohs(overlapped->remote_address.sin_port);

				try
				{						
#if defined(DEBUG_UntagIncomingStreams)
					
					ReceiveStatus^ receiveStatus;
					if (!receiveStatuses->TryGetValue(address, receiveStatus))
					{
						receiveStatus = gcnew ReceiveStatus(address);
						receiveStatuses->Add(address, receiveStatus);
						inspectable_receiveStatuses->Add(gcnew QS::Fx::Inspection::ScalarAttribute(address->ToString(), receiveStatus));
						(static_cast<QS::_core_c_::Diagnostics2::IContainer^>(diagnosticsContainerForReceiveStatuses))->Register(
							address->ToString(), (static_cast<QS::_core_c_::Diagnostics2::IModule^>(receiveStatus))->Component);
					}

					int receivebufoffset, receivebufsize;

#if defined(DEBUG_UntagIncomingStreams)
					receivebufoffset = 8;
					receivebufsize = numberOfBytesReceived - 8;
#else
					receivebufoffset = 0;
					receivebufsize = numberOfBytesReceived;
#endif					

					int *pminiheader;
					QS::Fx::Base::Block receivedblock;

#if defined(OPTION_USE_BUFFER_MANAGER)

					ReceiveBuffer^ receivebuf = listener->BufferController->Lookup(overlapped->bufferno);
					pminiheader = (int*)((LPVOID) receivebuf->Address);
					receivedblock = QS::Fx::Base::Block::Block(QS::Fx::Base::Block::Type::Pinned, receivebuf->Buffer,
						receivebuf->Address, receivebufoffset, receivebufsize);
					receivedblock.control = static_cast<QS::Fx::Base::IBlockControl^>(receivebuf);

#else

					pminiheader = (int*)((LPVOID) listener->BufferAddresses[overlapped->bufferno]);
					receivedblock = QS::Fx::Base::Block::Block(QS::Fx::Base::Block::Type::Managed, listener->Buffers[overlapped->bufferno],
						IntPtr::Zero, receivebufoffset, receivebufsize);

#endif
					
					int streamid = pminiheader[0];
					int seqno = pminiheader[1];					

					time = clock->Time;
					receiveStatus->Received(streamid, seqno, time);

#endif

					listener->Callback(address, port, receivedblock, listener->Context);

#if defined(OPTION_USE_BUFFER_MANAGER)
		
					if (receivebuf->NumReferences > 0)
					{
						ReceiveBuffer^ anotherbuffer = listener->BufferController->Allocate();
						overlapped->bufferno = anotherbuffer->ID;
						overlapped->wsabuf.buf = (char*) ((LPVOID) anotherbuffer->Address);

						// We might want to register the fact that the original buffer is supposed to be eventually returned...
					}

#endif
				}
				catch (Exception^ exc)
				{
					Error("Exception caught while processing received data.\n" + exc->ToString());
				}

#if defined(DEBUG_CollectStatisticsForIO)
				double time2 = clock->Time;
				time = time2;
				listener->LogOverhead(time1, time2 - time1);
#endif
			}

			#pragma endregion

			#pragma region Class ReceiveStatus

			Core::ReceiveStatus::ReceiveStatus(IPAddress^ sourceAddress)
			{
				this->sourceAddress = sourceAddress;

				diagnosticsContainer = gcnew QS::_core_c_::Diagnostics2::Container();
				diagnosticsContainerForIncomingStreams = gcnew QS::_core_c_::Diagnostics2::Container();

				incomingStreams = gcnew Dictionary<int, Stream^>();

				((QS::_core_c_::Diagnostics2::IContainer ^) diagnosticsContainer)->Register("Streams", diagnosticsContainerForIncomingStreams);
			}

			Core::ReceiveStatus::Stream::Stream(int streamid)
			{
				this->streamid = streamid;
				diagnosticsContainer = gcnew QS::_core_c_::Diagnostics2::Container();				

#if defined(DEBUG_LogIndividualReceiveTimes)
				receiveTimes = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(DEBUG_LogReceiveRates)
				receiveRates = gcnew List<QS::_core_e_::Data::XY>();
#endif

				QS::_core_c_::Diagnostics2::Helper::RegisterLocal(diagnosticsContainer, this);			
			}

			#pragma endregion
		}
	}
}
