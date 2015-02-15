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

#include "Core.h"
#include "Listener.h"
// #include "Sockets.h"
// #include "AsynchronousCall.h"
#include "File.h"
#include "ReceiveBuffer.h"
#include "Buffer.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

// #define DEBUG_DisplayIncomingPackets
// #define DEBUG_DisplayOutgoingMessagesForUnmanagedApplication
// #define DEBUG_DisplayIncomingMessagesForUnmanagedApplication

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			#pragma region IChannelController Members

			IChannel^ Core::OpenChannel(String^ id, String ^name, IChannel ^channelclient)
			{
				IntPtr unmanagedid;
				HANDLE incomingpipe, outgoingpipe, incomingmemory, outgoingmemory;				
				Buffer *incomingbuffer, *outgoingbuffer;
				int errorno;
				unmanagedid = Marshal::StringToHGlobalUni("\\\\.\\pipe\\" + id + "1");
				if (this->channelcontroller)
				{
					incomingpipe = 
						CreateNamedPipe(
							(wchar_t *) unmanagedid.ToPointer(), 
							PIPE_ACCESS_INBOUND | FILE_FLAG_FIRST_PIPE_INSTANCE | FILE_FLAG_OVERLAPPED,
							PIPE_TYPE_BYTE | PIPE_READMODE_BYTE, 1, 0, this->channelcapacityincomingcontrol, INFINITE, NULL);
				}
				else
				{
					outgoingpipe = 
						CreateFile(
							(wchar_t *) unmanagedid.ToPointer(), 
							GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
				}
				Marshal::FreeHGlobal(unmanagedid);
				unmanagedid = Marshal::StringToHGlobalUni("\\\\.\\pipe\\" + id + "2");
				if (this->channelcontroller)
				{
					outgoingpipe = 
						CreateNamedPipe(
							(wchar_t *) unmanagedid.ToPointer(), 
							PIPE_ACCESS_OUTBOUND | FILE_FLAG_FIRST_PIPE_INSTANCE | FILE_FLAG_OVERLAPPED,
							PIPE_TYPE_BYTE | PIPE_READMODE_BYTE, 1, this->channelcapacityoutgoingcontrol, 0, INFINITE, NULL);
				}
				else
				{
					incomingpipe = 
						CreateFile(
							(wchar_t *) unmanagedid.ToPointer(), 
							GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
				}
				Marshal::FreeHGlobal(unmanagedid);
				unmanagedid = Marshal::StringToHGlobalUni("\\Global\\liveobjects." + id + "1");
				if (this->channelcontroller)
					incomingmemory = CreateFileMapping(
						INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, this->channelcapacityincoming + sizeof(Buffer), (wchar_t *) unmanagedid.ToPointer());
				else
					incomingmemory = OpenFileMapping(FILE_MAP_READ | FILE_MAP_WRITE, FALSE, (wchar_t *) unmanagedid.ToPointer());
				Marshal::FreeHGlobal(unmanagedid);
				if (!incomingmemory)
					throw gcnew Exception("Cannot create file mapping for the incoming buffer.");
				unmanagedid = Marshal::StringToHGlobalUni("\\Global\\liveobjects." + id + "2");
				if (this->channelcontroller)
					outgoingmemory = CreateFileMapping(
						INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, this->channelcapacityoutgoing + sizeof(Buffer), (wchar_t *) unmanagedid.ToPointer());
				else
					outgoingmemory = OpenFileMapping(FILE_MAP_READ | FILE_MAP_WRITE, FALSE, (wchar_t *) unmanagedid.ToPointer());
				Marshal::FreeHGlobal(unmanagedid);
				if (!outgoingmemory)
					throw gcnew Exception("Cannot create file mapping for the outgoing buffer.");
				if (!(incomingbuffer = (Buffer *) MapViewOfFile(incomingmemory, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 0)))
					throw gcnew Exception("Can't map the incoming buffer to process memory.");
				if (!(outgoingbuffer = (Buffer *) MapViewOfFile(outgoingmemory, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 0)))
					throw gcnew Exception("Can't map the outgoing buffer to process memory.");
				if (this->channelcontroller)
				{
					incomingbuffer->initialize(this->channelcapacityincoming);					
					outgoingbuffer->initialize(this->channelcapacityoutgoing);
				}
				else
				{
				}
				if (incomingpipe == INVALID_HANDLE_VALUE)
					throw gcnew Exception("Cannot create incoming pipe.");
				if (outgoingpipe == INVALID_HANDLE_VALUE)
					throw gcnew Exception("Cannot create outgoing pipe.");
				if (CreateIoCompletionPort(incomingpipe, (HANDLE) this->completionPort, 0, 0) != ((HANDLE) this->completionPort))					
					throw gcnew Exception("Cannot associate incoming pipe with the completion port (" + GetLastError().ToString() + ").");
				if (CreateIoCompletionPort(outgoingpipe, (HANDLE) this->completionPort, 0, 0) != ((HANDLE) this->completionPort))					
					throw gcnew Exception("Cannot associate outgoing pipe with the completion port (" + GetLastError().ToString() + ").");
				__int32 reference = Interlocked::Increment(this->last_reference);
				Channel^ channel = gcnew Channel(this, id, name, reference, IntPtr(incomingpipe), IntPtr(outgoingpipe), 
					IntPtr(incomingmemory), IntPtr(outgoingmemory), IntPtr((void*) incomingbuffer), IntPtr((void *) outgoingbuffer), 
					channelclient, !this->channelcontroller, !this->channelcontroller);
				this->mychannels->Add(id, channel);
				this->references->Add(reference, channel);
				if (this->channelcontroller)
				{
					MyOverlapped * overlapped;
					overlapped = new MyOverlapped(MyOverlapped::PipeConnect, reference, 1);
					errorno = 0;
					if (!ConnectNamedPipe(incomingpipe, &overlapped->overlapped) && ((errorno = GetLastError()) != ERROR_IO_PENDING))
						throw gcnew Exception("Cannot initiate connection on the incoming pipe (" + errorno.ToString() + ").");
					overlapped = new MyOverlapped(MyOverlapped::PipeConnect, reference, 2);
					errorno = 0;
					if (!ConnectNamedPipe(outgoingpipe, &overlapped->overlapped) && ((errorno = GetLastError()) != ERROR_IO_PENDING))
						throw gcnew Exception("Cannot initiate connection on the outgoing pipe (" + errorno.ToString() + ").");
				}
				else
				{
					channel->Incoming();
				}
				return channel;
			}

			void Core::CloseChannel(String^ id)
			{
				mychannels->Remove(id);
			}

			#pragma endregion

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

			#pragma region Constructor, destructor, logging errors

			void Core::UnhandledExceptionHandler(Object^ sender, UnhandledExceptionEventArgs^ args) 
			{
				Error("Unhandled Exception:\n" + QS::_core_c_::Helpers::ToString::Exception(dynamic_cast<Exception^> (args->ExceptionObject)));

				DumpLogs();
			}

			void Core::DumpLogs()
			{
				// ..............
			}

			Core::Core(String^ workingroot) 
			{
				this->Constructor(workingroot, nullptr, false);
			}

			Core::Core(String^ workingroot, QS::Fx::Scheduling::IQueue^ downcalls, bool pinthreads)
			{
				this->Constructor(workingroot, downcalls, pinthreads);
			}

			void Core::Constructor(String^ workingroot, QS::Fx::Scheduling::IQueue^ downcalls, bool pinthreads)
			{
				this->pinthreads = pinthreads;

				this->does_dropping = false;

				mychannels = gcnew System::Collections::Generic::Dictionary<String^, QS::_core_c_::Core::Channel^>();
				
				if (downcalls == nullptr)
					downcalls = gcnew QS::Fx::Scheduling::SinglethreadedQueue();
				this->downcalls = downcalls;

				if (!System::IO::Directory::Exists(workingroot))
					System::IO::Directory::CreateDirectory(workingroot);

				this->workingroot = workingroot;
				this->statisticsController = gcnew QS::_core_c_::Statistics::FileController(this, this, workingroot);

				if (Transmitter::TaggingOutgoingStreams != Core::UntaggingIncomingStreams)
					throw gcnew Exception("Configurations for tagging data streams at the senders and at the receivers mismatch.");

//				QS::Win32::SetEnvironmentVariable("COMPLUS_DbgJitDebugLaunchSetting", "1");

				AppDomain::CurrentDomain->UnhandledException += 
					gcnew UnhandledExceptionEventHandler(this, &Core::UnhandledExceptionHandler);

				diagnosticsContainer = gcnew QS::_core_c_::Diagnostics2::Container();
				diagnosticsContainerForSenders = gcnew QS::_core_c_::Diagnostics2::Container();
				diagnosticsContainerForListeners = gcnew QS::_core_c_::Diagnostics2::Container();

				((QS::_core_c_::Diagnostics2::IContainer ^) diagnosticsContainer)->Register("Senders", diagnosticsContainerForSenders);
				((QS::_core_c_::Diagnostics2::IContainer ^) diagnosticsContainer)->Register("Listeners", diagnosticsContainerForListeners);

				this->maximumConcurrency = Core::default_MaximumConcurrency;
				this->minimumTransmitted = Core::default_MinimumTransmitted;
				this->maximumTransmitted = Core::default_MaximumTransmitted;

				this->defaultMTU = Core::default_SenderMTU;
				this->defaultMaximumSenderConcurrency = Core::default_SenderMaximumConcurrency;

				this->defaultMaximumSenderUnicastRate = Core::default_SenderMaximumUnicastRate;
				this->defaultMaximumSenderUnicastCredits = Core::default_SenderMaximumUnicastCredits;
				this->defaultMaximumSenderMulticastRate = Core::default_SenderMaximumMulticastRate;
				this->defaultMaximumSenderMulticastCredits = Core::default_SenderMaximumMulticastCredits;

				maximum_alarms_quantum = Core::default_maximumQuantumForAlarms;
				maximum_io_quantum = Core::default_maximumQuantumForCompletionPort;

				drain_completion_ports = true;

				clock = Clock::SharedClock;
				alarmQueue = gcnew SplayTree<double, Alarm^>();
				expiredAlarms = gcnew Queue<Alarm^>();

				concurrency = 0;
				transmitted = 0;
				registeredSenders = gcnew Queue<Sender^>();
				sendersToKeep = gcnew Queue<Sender^>();

				completionPort = (IntPtr) CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0); 
				if ((HANDLE) completionPort == NULL)
					throw gcnew Exception("Could not create the completion port.");

				listeners = gcnew Dictionary<Address^, QS::_core_c_::Core::Listener^>();
				senders = gcnew Dictionary<Address^, Sender^>();
				
				last_reference = 0;
				references = gcnew Dictionary<__int32, Object^>();

				finished = false;
				thread = gcnew Thread(gcnew ThreadStart(this, &QS::_core_c_::Core::Core::MainLoop));
				thread->Name = "Quicksilver(0)";
				thread->Priority = ThreadPriority::Normal;							

#if defined(DEBUG_CollectCoreStatistics)
				ts_QueryCompletionStatusTimeouts = statisticsController->Allocate2D(
					"query completion status timeouts", "", "time", "s", "time when io completion port was queried", "timeout", "s", "the timeout we used");
				ts_QueryCompletionStatusMeasuredDelays = statisticsController->Allocate2D(
					"query completion status measured delays", "", "time", "s", "time port was queried", "delay", "s", "time it took to return from the call");
				ts_QueryCompletionStatusTimeoutsVsMeasuredDelays = statisticsController->Allocate2D(
					"query completion status timeouts vs measured delays", "", "timeout", "s", "timeout used", "delay", "s", "actual delay that occurred");
#endif

#if defined(DEBUG_MeasureCoreConcurrencyStatistics)

				ts_CoreConcurrency = statisticsController->Allocate2D(
					"core concurrency", "", "time", "s", "the time this sample was taken", "concurrency", "", "the number of active send operations");

				ts_CoreBytesBeingTransmitted = statisticsController->Allocate2D(
					"core bytes being transmitted", "", "time", "s", "the time this sample was taken", "bytes being transmitted", "bytes", 
					"the total number of bytes across all active send operations");

#endif

				runningAllowed = gcnew System::Threading::ManualResetEvent(true);

#if defined(DEBUG_RegisterCoreActions)
				ts_CoreActions = statisticsController->Allocate2D("core actions", "", "time", "s", "", "action type id", "", ""); 
#endif

#if defined(DEBUG_RegisterSchedulingHistory)
				timeseries_schedulingHistory = gcnew List<QS::_core_e_::Data::XY>();
#endif				

#if defined(DEBUG_RememberMostRecentAlarms)
				historyOfAlarmsRecentlyFired = gcnew Queue<AlarmInfo>();
#endif

#if defined(DEBUG_CollectStatisticsForAlarms)
				timeseries_alarmOverheads = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(DEBUG_CollectStatisticsForIO)
				ts_SendCompletionTotalProcessingOverheads = statisticsController->Allocate2D(
					"send completion processing overheads", "", "time", "s", "", "overhead", "s", "");
				ts_ReceiveCompletionTotalProcessingOverheads = statisticsController->Allocate2D(
					"receive completion processing overheads", "", "time", "s", "", "overhead", "s", "");
#endif

#if defined(DEBUG_UntagIncomingStreams)
				receiveStatuses = gcnew Dictionary<IPAddress^, ReceiveStatus^>();

				diagnosticsContainerForReceiveStatuses = gcnew QS::_core_c_::Diagnostics2::Container();
				((QS::_core_c_::Diagnostics2::IContainer ^) diagnosticsContainer)->Register("Sources", diagnosticsContainerForReceiveStatuses);
#endif

#if defined(DEBUG_CollectPeriodicStatistics)
				ts_averagePendingAlarmQueueLengths = gcnew List<QS::_core_e_::Data::XY>();
				ts_maximumPendingAlarmQueueLengths = gcnew List<QS::_core_e_::Data::XY>();
				ts_averageExpiredAlarmQueueLengths = gcnew List<QS::_core_e_::Data::XY>();
				ts_maximumExpiredAlarmQueueLengths = gcnew List<QS::_core_e_::Data::XY>();
				ts_averageAlarmFiringDelays = gcnew List<QS::_core_e_::Data::XY>();
				ts_maximumAlarmFiringDelays = gcnew List<QS::_core_e_::Data::XY>();
				ts_alarmSchedulingRate = gcnew List<QS::_core_e_::Data::XY>();
				ts_alarmCancelingRate = gcnew List<QS::_core_e_::Data::XY>();
				ts_alarmFiringRate = gcnew List<QS::_core_e_::Data::XY>();
				ts_alarmRemovingRate = gcnew List<QS::_core_e_::Data::XY>();
				ts_totalLocalProcessorUsage = gcnew List<QS::_core_e_::Data::XY>();
				ts_userLocalProcessorUsage = gcnew List<QS::_core_e_::Data::XY>();
				ts_rateAtWhichActiveSendersReturnNothing = gcnew List<QS::_core_e_::Data::XY>();

#if defined(DEBUG_LogWriteCompletionRates)
				ts_writeCompletionRate = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)
				ts_ProcessSendCompletionDetailedProfilingInformation_WSAGetOverlappedResult =
					statisticsController->Allocate2D("Overheads of WSAGetOverlappedResult", "", "time", "s", "", "overhead", "s", "");
				ts_ProcessSendCompletionDetailedProfilingInformation_TransmitterCompleted =
					statisticsController->Allocate2D("Overheads of TransmitterCompleted", "", "time", "s", "", "overhead", "s", "");
#endif
				
				ts_Overhead_ProcessSenders = statisticsController->Allocate2D("process senders overhead", "", "time", "s", "", "overhead", "s", "");
				ts_Overhead_AllowTransmission = statisticsController->Allocate2D("allow transmission overhead", "", "time", "s", "", "overhead", "s", "");

				ts_memoryAllocated = gcnew List<QS::_core_e_::Data::XY>();
				myprocess = System::Diagnostics::Process::GetCurrentProcess();
#endif

				unicastControllers = gcnew QS::_core_c_::RateControl::RateController1::Class(defaultMaximumSenderUnicastCredits);
				multicastControllers = gcnew QS::_core_c_::RateControl::RateController1::Class(defaultMaximumSenderMulticastCredits);

				inspectable_receiveStatuses = gcnew QS::Fx::Inspection::AttributeCollection("Sources");

				timeWarpThresholdForAlarms = default_timeWarpThresholdForAlarms;
				timeWarpThresholdForIO = default_timeWarpThresholdForIO;

#if defined(DEBUG_LogTimeWarps)				
				ts_timeWarps = gcnew List<QS::_core_e_::Data::XY>();
				ts_timeWarpContexts = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)
				completedTransmitters = gcnew Queue<Transmitter^>();
				completedPriorityTransmitters = gcnew Queue<Transmitter^>();
				receiveCompletions = gcnew Queue<DeferredProcessReceive>();
				priorityReceiveCompletions = gcnew Queue<DeferredProcessReceive>();
				completedFileIOs = gcnew Queue<DeferredCompletedFileIO>();
#endif

				QS::_core_c_::Diagnostics2::Helper::RegisterLocal(diagnosticsContainer, this);

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
				incomingchannel = QS::_core_x_::Unmanaged::MemoryChannel::Create(DefaultSizeOfTheMemoryChannel, (HANDLE) completionPort);
				outgoingchannel = QS::_core_x_::Unmanaged::MemoryChannel::Create(DefaultSizeOfTheMemoryChannel, CreateEvent(NULL, FALSE, FALSE, NULL));
				pendingincoming = new QS::_core_x_::Unmanaged::UnprotectedBuffer();
				pendingoutgoing = new QS::_core_x_::Unmanaged::UnprotectedBuffer();
#endif

#if defined(OPTION_USE_BUFFER_MANAGER)
				
				// buffercontroller = gcnew ReceiveBuffer::Controller(20000);
				
#endif

				//int fuck = sizeof(QS::_core_c_::Core::MyOverlapped);
				//LogMessage(fuck.ToString());
			}

			#pragma endregion

			#pragma region Start and stop

			void Core::Start()
			{				
				System::Threading::Monitor::Enter(this);
				try
				{
					if (running)
						throw gcnew Exception("Already running.");

					if (thread->ThreadState == System::Threading::ThreadState::Unstarted)
						thread->Start();
					else
						runningAllowed->Set();

					running = true;

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)
//				LogMessage("Core: Using deferred priority-based I/O processing.");
#else
//				LogMessage("Core: Priority-based I/O processing is DISABLED!!");
#endif
				}
				finally
				{
					System::Threading::Monitor::Exit(this);
				}
			}

			void Core::Stop()
			{
				System::Threading::Monitor::Enter(this);
				try
				{
					if (thread->ThreadState == System::Threading::ThreadState::Unstarted)
						throw gcnew Exception("Not yet started.");

					//if (!running)
					//	throw gcnew Exception("Not running.");

					runningAllowed->Reset();
					running = false;
				}
				finally
				{
					System::Threading::Monitor::Exit(this);
				}
			}

			Core::~Core()
			{
				finished = true;
				runningAllowed->Set();
				if (!PostQueuedCompletionStatus((HANDLE) completionPort, 0, (ULONG_PTR) NULL, NULL)) 
					throw gcnew Exception("Could not post finalization command.");	

				thread->Join();
			}

			void Core::Error(String^ description)
			{
				errorCallback(description);
			}

			#pragma endregion

			#pragma region IConsole Members

			void Core::writeLine(String^ s)
			{
				QS::Fx::Logging::ILogger^ logger = this->logger;
				if (logger != nullptr)
					logger->Log(s);
				else
					Error("Could not log a message (no logger defined).\n" + s);
			}

			#pragma endregion

			#pragma region ILogger Members

			void Core::clear()
			{
				throw gcnew NotSupportedException();
			}

			void Core::logMessage(Object^ source, String^ message)
			{
				QS::Fx::Logging::ILogger^ logger = this->logger;
				if (logger != nullptr)
					logger->Log(source, message);
				else
					Error("Could not log a message (no logger defined).\n" + message);
			}

			#pragma endregion

			#pragma region IEventLogger Members

			void Core::Log(QS::Fx::Logging::IEvent^ eventToLog)
			{
				QS::Fx::Logging::IEventLogger^ eventLogger = this->eventLogger;
				if (eventLogger != nullptr)
					eventLogger->Log(eventToLog);
				else
					Error("Could not log an event (no event logger defined).\n" + eventToLog->ToString());
			}

			#pragma endregion

			#pragma region Invocation

			void Core::Schedule(QS::Fx::Base::IEvent^ e)
			{
				downcalls->Enqueue(e);
				if (downcalls->Blocked && !Interlocked::Exchange(will_process_downcalls, 1))
				{
					if (!PostQueuedCompletionStatus((HANDLE) completionPort, 0, 0, NULL))
						throw gcnew Exception("Could not schedule a call.");	
				}
			}

			IAsyncResult^ Core::BeginExecute(AsyncCallback^ asynchronousCallback, Object^ asynchronousState)
			{
				AsynchronousCall^ call = gcnew AsynchronousCall(asynchronousCallback, asynchronousState);
				call->GCHandle = GCHandle::Alloc(call);
				
				if (!PostQueuedCompletionStatus((HANDLE) completionPort, 0, 
					(ULONG_PTR) (GCHandle::ToIntPtr(call->GCHandle).ToPointer()), NULL))
				{
					call->GCHandle.Free();
					throw gcnew Exception("Could not schedule a call.");	
				}

				return call;
			}

			#pragma endregion

			#pragma region Alarm processing

			QS::Fx::Clock::IAlarm^ Core::Schedule(double timeout, QS::Fx::Clock::AlarmCallback^ asynchronousCallback, Object^ asynchronousState)
			{
				VerifyWhetherThisIsCoreThread();

				this->time = clock->Time;

				Alarm^ alarm = gcnew Alarm(this, this->time + timeout, timeout, asynchronousCallback, asynchronousState);
				alarmQueue->Enqueue(alarm);
				alarm->Registered = true;

#if defined(DEBUG_CollectPeriodicStatistics)
				pstat_alarmsScheduledSinceLastCheck++;
				UpdateAlarmQueueSizes();
				CheckUpdatePeriodicStatistics();
#endif

				return alarm;
			}

/*
			QS::Fx::Clock::IAlarm^ Core::Schedule(
					double timeSpanInSeconds, QS::_core_c_::Base::AlarmCallback^ alarmCallback, Object^ argument)
			{
				VerifyWhetherThisIsCoreThread();

				this->time = clock->Time;

				Alarm^ alarm = gcnew Alarm(this, this->time + timeSpanInSeconds, timeSpanInSeconds, alarmCallback, argument);
				alarmQueue->Enqueue(alarm);
				alarm->Registered = true;

#if defined(DEBUG_CollectPeriodicStatistics)
				pstat_alarmsScheduledSinceLastCheck++;
				UpdateAlarmQueueSizes();
				CheckUpdatePeriodicStatistics();
#endif

				return alarm;
			}
*/

			void Core::FireAlarm(Alarm^ alarm)
			{
				alarm->Fire();
			}

			void Core::Register(QS::Fx::Clock::IAlarm^ toRegister)
			{
				VerifyWhetherThisIsCoreThread();

				Alarm^ alarm = dynamic_cast<Alarm^>(toRegister);
				if (alarm != nullptr)
				{
					alarm->Time = time + alarm->Timeout;
					alarmQueue->Enqueue(alarm);
					alarm->Registered = true;

#if defined(DEBUG_CollectPeriodicStatistics)
					pstat_alarmsScheduledSinceLastCheck++;
					UpdateAlarmQueueSizes();
					CheckUpdatePeriodicStatistics();
#endif
				}
				else
					throw gcnew Exception("Internal error: Cannot insert an object of this type into the alarm queue.");
			}

			void Core::Unregister(QS::Fx::Clock::IAlarm^ toUnregister)
			{
				VerifyWhetherThisIsCoreThread();

				Alarm^ alarm = dynamic_cast<Alarm^>(toUnregister);
				if (alarm != nullptr)
				{
					if (!alarm->Registered)
						throw gcnew Exception("Internal error: Unregistering an alarm that has not been registered.");
					alarm->Registered = false;
					alarmQueue->Remove(alarm);

#if defined(DEBUG_CollectPeriodicStatistics)
					pstat_alarmsRemovedSinceLastCheck++;
					UpdateAlarmQueueSizes();
					CheckUpdatePeriodicStatistics();
#endif
				}
				else
					throw gcnew Exception("Internal error: Cannot remove an object of this type from the alarm queue.");
			}

			#pragma endregion

			#pragma region Managing listeners

			IListener^ Core::Listen(Address^ address, QS::Fx::Network::ReceiveCallback^ callback, Object^ context, 
				int maximumSize, int numberOfBuffers, bool drainSynchronously, int sizeOfTheAdfBuffer, bool high_priority)
			{				
				VerifyWhetherThisIsCoreThread();

				if (listeners->ContainsKey(address))
					throw gcnew Exception("Already listening at " + address->ToString());

				Socket^ socket = Sockets::CreateReceiveSocket(address, true, sizeOfTheAdfBuffer);
				if (CreateIoCompletionPort(
					(HANDLE) socket->Handle, (HANDLE) completionPort, 0, 0) != (HANDLE) completionPort)
					throw gcnew Exception("Could not associate completion port with the socket.");
				
				__int32 reference = Interlocked::Increment(last_reference);

				QS::_core_c_::Core::Listener^ listener = gcnew QS::_core_c_::Core::Listener(this, reference, address, socket, 
					callback, context, maximumSize, numberOfBuffers, drainSynchronously, statisticsController, high_priority, sizeOfTheAdfBuffer);

				if (listener == nullptr)
					throw gcnew Exception("Could not create listener.");

#if defined(LogCreationOfListeners)
				LogMessage("Listener { " + address->ToString() + " } : nbuffers = " + numberOfBuffers.ToString() + ", buffersize = " + 
					maximumSize.ToString() + ", drain = " + drainSynchronously.ToString() + ", adf_buffersize = " + sizeOfTheAdfBuffer.ToString());
#endif

				listeners->Add(address, listener);

				((QS::_core_c_::Diagnostics2::IContainer ^) diagnosticsContainerForListeners)->Register(
					address->ToString(), ((QS::_core_c_::Diagnostics2::IModule ^) listener)->Component, QS::_core_c_::Diagnostics2::RegisteringMode::Force);

				references->Add(reference, listener);

				for (int ind = 0; ind < numberOfBuffers; ind++)
				{
					MyOverlapped* overlapped = new MyOverlapped(MyOverlapped::Receive, reference, ind);		
					overlapped->wsabuf.len = listener->BufferSize;
					overlapped->remote_address_length = sizeof(struct sockaddr_in);

#if defined(OPTION_USE_BUFFER_MANAGER)

					ReceiveBuffer^ buffer = listener->BufferController->Allocate();
					overlapped->bufferno = buffer->ID;
					overlapped->wsabuf.buf = (char*) ((LPVOID) buffer->Address);

#else

					overlapped->wsabuf.buf = (char*) ((LPVOID) listener->BufferAddresses[ind]);

#endif

					ProcessReceive(overlapped, false, false, listener, socket);
				}

				return listener;
			}

			void Core::Unregister(IListener^ toUnregister)
			{
				VerifyWhetherThisIsCoreThread();

				QS::_core_c_::Core::Listener^ listener = dynamic_cast<QS::_core_c_::Core::Listener^>(toUnregister);
				
				listeners->Remove(listener->CoreAddress);
			}

			#pragma endregion

			#pragma region Managing senders

			QS::_core_c_::Core::ISender^ Core::GetSender(Address^ address, int sizeOfTheAdfBuffer, int numberOfTransmitters, bool high_priority)
			{
				VerifyWhetherThisIsCoreThread();

				Sender^ sender;
				if (!senders->TryGetValue(address, sender))
				{
					QS::_core_c_::RateControl::IRateController^ rateController = 
						((address->IsMulticast) ? multicastControllers : unicastControllers)->Create(clock, this, this, 
						((address->IsMulticast) ? defaultMaximumSenderMulticastRate : defaultMaximumSenderUnicastRate), statisticsController);

					int streamid = nstreams++;

#if defined(LogStreamAssignments)
					LogMessage("Stream { " + streamid.ToString("000") + " } : Address " + address->ToString());
#endif

					sender = gcnew Sender(this, this, this, clock, gcnew ErrorCallback(this, &QS::_core_c_::Core::Core::Error), address, completionPort, 
						defaultMTU, ((numberOfTransmitters > 0) ? numberOfTransmitters : this->defaultMaximumSenderConcurrency), 
						rateController, this, this, streamid, statisticsController, sizeOfTheAdfBuffer, high_priority);

					if (sender == nullptr)
						throw gcnew Exception("Could not create sender.");
					senders->Add(address, sender);

					((QS::_core_c_::Diagnostics2::IContainer ^) diagnosticsContainerForSenders)->Register(
						"[" + streamid.ToString("0000") + "] " + address->ToString(), ((QS::_core_c_::Diagnostics2::IModule ^) sender)->Component);
				}
				return sender;
			}

			void Core::ProcessSenders()
			{
#if defined (DEBUG_CollectPeriodicStatistics)
				time = clock->Time;
				double tt1 = time;
#endif

				while (concurrency < maximumConcurrency && 
					transmitted <= maximumTransmitted - minimumTransmitted && registeredSenders->Count > 0)
				{
					Sender^ sender = registeredSenders->Dequeue();

					int numberOfPackets;
					int numberOfBytes;
					bool moreAvailable;

#if defined (DEBUG_CollectPeriodicStatistics)
					time = clock->Time;
					double tt3 = time;
#endif

					sender->AllowTransmission(maximumConcurrency - concurrency, maximumTransmitted - transmitted, 
						numberOfPackets, numberOfBytes, moreAvailable);
					
#if defined (DEBUG_CollectPeriodicStatistics)
					time = clock->Time;
					double tt4 = time;

					pstat_allowTransmissionCumulated += tt4 - tt3;
					pstat_allowTransmissionNumOfSamples++;
#endif

					concurrency += numberOfPackets;
					transmitted += numberOfBytes;

					if (moreAvailable)
					{
						sendersToKeep->Enqueue(sender);

#if defined (DEBUG_CollectPeriodicStatistics)
						if (numberOfPackets == 0)
							pstat_numberOfTimesAnActiveSenderReturnedNothing++;
#endif
					}
				}

				while (sendersToKeep->Count > 0)
					registeredSenders->Enqueue(sendersToKeep->Dequeue());

#if defined(DEBUG_MeasureCoreConcurrencyStatistics)
				ts_CoreConcurrency->Add(time, concurrency);
				ts_CoreBytesBeingTransmitted->Add(time, transmitted);
#endif

#if defined (DEBUG_CollectPeriodicStatistics)
				time = clock->Time;
				double tt2 = time;
		
				pstat_processSendersCumulated += tt2 - tt1;
				pstat_processSendersNumOfSamples++;
#endif
			}

			void Core::ScheduleSender(ISender^ toSchedule)
			{
				Sender^ sender = dynamic_cast<Sender^>(toSchedule);
				if (sender == nullptr)
					throw gcnew Exception("Internal error: sender is NULL");

				registeredSenders->Enqueue(sender);
				ProcessSenders();
			}

			void Core::ReleaseResources(int numberOfMessages, int numberOfBytes)
			{
				concurrency -= numberOfMessages;
				transmitted -= numberOfBytes;

				ProcessSenders();
			}

			#pragma endregion

			#pragma region Main loop

			void Core::MainLoop()
			{
				if (this->pinthreads)
				{
					int mythreadid = ::GetCurrentThreadId();
					Process^ process = Process::GetCurrentProcess();
					for each (ProcessThread^ processthread in process->Threads) 
					{
						if (processthread->Id == mythreadid)
						{
							processthread->IdealProcessor = 0;
							processthread->ProcessorAffinity = System::IntPtr(1);
						}
					}
				}

				try
				{
					// Error("QuickSilver Core Thread Starting");

					time = clock->Time;
					initializationCallback();				

					time = clock->Time;
					timewarps_initialize();

					while (!finished)
					{				
						// resynchronize with the wall clock
						clock->Correct();

						#pragma region Move all expired alarms to the expired alarm queue
						
						time = clock->Time;

#if defined(DEBUG_RegisterSchedulingHistory)
						RegisterSchedulerAction(ProcessingMode::Alarms);
#endif

						while (!alarmQueue->IsEmpty && alarmQueue->Head->Time <= time)
						{
							Alarm^ alarm = alarmQueue->Dequeue();
							if (!alarm->Registered)
								throw gcnew Exception("Internal error: Unregistering an alarm that has not been registered.");
							alarm->Registered = false;
							if (!alarm->Cancelled)
								expiredAlarms->Enqueue(alarm);
						}

						#pragma endregion

						#pragma region Process expired alarms in the expired alarm queue

						double waiting;
						time = clock->Time;

						if (timewarps_recheck(ProcessingContext::Alarms1, timeWarpThresholdForAlarms))
						{
							waiting = 0;
						}
						else
						{
#if defined(DEBUG_CollectPeriodicStatistics)
							UpdateAlarmQueueSizes();
							CheckUpdatePeriodicStatistics();
#endif

							double completion_port_check_deadline;
							completion_port_check_deadline = time + maximum_alarms_quantum;

							while (true)
							{
								while (!running && !finished)
								{
									runningAllowed->WaitOne(System::Threading::Timeout::Infinite, false);

									time = clock->Time;
									timewarps_initialize();
								}

								if (finished)
									break;

								time = clock->Time;
								if (timewarps_recheck(ProcessingContext::Alarms2, timeWarpThresholdForAlarms))
								{
									waiting = 0;
									break;
								}

								Alarm^ alarm = nullptr;
								if (expiredAlarms->Count == 0)
								{
									if (alarmQueue->IsEmpty)
									{
										waiting = double::PositiveInfinity;
										break;
									}
									else
									{
										waiting = alarmQueue->Head->Time - time;
										if (waiting > 0)
											break;
										else
										{
											alarm = alarmQueue->Dequeue();
											if (!alarm->Registered)
												throw gcnew Exception("Internal error: Unregistering an alarm that has not been registered.");
											alarm->Registered = false;
										}
									}
								}

								if (time > completion_port_check_deadline)
								{
									if (alarm != nullptr)
										expiredAlarms->Enqueue(alarm);

									waiting = 0;
									break;
								}

								if (alarm == nullptr)
									alarm = expiredAlarms->Dequeue();

#if defined(DEBUG_RegisterCoreActions)
								RegisterAction(ActionType::Alarm);
#endif

#if defined(DEBUG_RememberMostRecentAlarms)
								LogAlarm(time, alarm);
#endif

#if defined(DEBUG_CollectStatisticsForAlarms)
								double time1 = clock->Time;
								time = time1;
#endif

#if defined(DEBUG_CollectPeriodicStatistics)
								UpdateAlarmQueueSizes();
								pstat_alarmsFiredSinceLastCheck++;
								double firing_delay = time - alarm->Time;
								if (firing_delay > pstat_maximumAlarmFiringDelay)
									pstat_maximumAlarmFiringDelay = firing_delay;
								pstat_cumulatedAlarmFiringDelay += firing_delay;
								pstat_cumulatedAlarmFiringDelaySamples++;
								CheckUpdatePeriodicStatistics();
#endif

								time = clock->Time;
								timewarps_recheck(ProcessingContext::Alarms3, timeWarpThresholdForAlarms);						

								FireAlarm(alarm);							

								time = clock->Time;
								timewarps_recheck(ProcessingContext::Alarms4, timeWarpThresholdForAlarms);						

#if defined(DEBUG_CollectStatisticsForAlarms)
								double time2 = clock->Time;
								time = time2;
								AddAlarmStatistic(time1, time2 - time1);							
#endif
							}
						}

						#pragma endregion

						#pragma region Process I/O

						time = clock->Time;
						timewarps_recheck(ProcessingContext::Alarms5, timeWarpThresholdForAlarms);						

#if defined(DEBUG_RegisterSchedulingHistory)
						RegisterSchedulerAction(ProcessingMode::IO);
#endif
						
						do
						{
							if (finished)
								break;

							double alarm_check_deadline;
							alarm_check_deadline = time + maximum_io_quantum;											

							if (CheckCompletionPort(waiting))
							{
								if (drain_completion_ports)
								{
									while (time < alarm_check_deadline && CheckCompletionPort(0))
									{
#if defined(DEBUG_CollectPeriodicStatistics)
										CheckUpdatePeriodicStatistics();
#endif
									}
								}

								// now we should process all the accumulated events in the proper sequence

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)
								
								if (priorityReceiveCompletions->Count > 0)
								{
									for each (DeferredProcessReceive completion in priorityReceiveCompletions)
										ProcessReceive(
											completion.overlapped, completion.completed, completion.succeeded, completion.listener, completion.socket);
									priorityReceiveCompletions->Clear();
								}

								if (completedPriorityTransmitters->Count > 0)
								{
									for each (Transmitter^ transmitter in completedPriorityTransmitters)
										transmitter->Completed();
									completedPriorityTransmitters->Clear();
								}

								if (receiveCompletions->Count > 0)
								{
									for each (DeferredProcessReceive completion in receiveCompletions)
										ProcessReceive(
											completion.overlapped, completion.completed, completion.succeeded, completion.listener, completion.socket);
									receiveCompletions->Clear();
								}

								if (completedTransmitters->Count > 0)
								{
									for each (Transmitter^ transmitter in completedTransmitters)
										transmitter->Completed();
									completedTransmitters->Clear();
								}

								if (completedFileIOs->Count > 0)
								{
									for each (DeferredCompletedFileIO completion in completedFileIOs)
										completion.file->CompletedFileIO(completion.overlapped, completion.ntransmitted, completion.succeeded);
									completedFileIOs->Clear();
								}

#endif
							}
						}
						while (timewarps_recheck(ProcessingContext::IO4, timeWarpThresholdForIO) && timewarps_continueio);

						if (finished)
							break;

						#pragma endregion

						#pragma region Processing downcalls

						ProcessDowncalls();

						#pragma endregion

						#pragma region Communicating with native application

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
						ProcessMessagesFromUnmanagedApplication();
						_CommunicateToUnmanagedApplication();
#endif

						#pragma endregion
					}

					time = clock->Time;
					cleanupCallback();

					Error("QuickSilver Core Thread Exiting Gracefully");
				}
				catch (Exception^ exc)
				{
					Error("QuickSilver Core Thread Crashed: " + exc->ToString());
				}
			}

			#pragma endregion

			#pragma region ProcessDowncalls

			void Core::ProcessDowncalls()
			{
				while (downcalls->Blocked)
				{
					QS::Fx::Base::IEvent^ e = downcalls->Dequeue();
					if (e != nullptr)
						e->Handle();
				}
			}

			#pragma endregion

			#pragma region CheckCompletionPort

			bool Core::CheckCompletionPort(double waiting)
			{
				DWORD timeout = double::IsPositiveInfinity(waiting) ? (1000 /* INFINITE */) : (DWORD) ((int) Math::Ceiling(1000 * waiting));

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
					downcalls->Blocked 
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

				case MyOverlapped::PipeConnect:
					{
						ProcessPipeConnect(overlapped, succeeded);
					}
					break;

				case MyOverlapped::PipeRead:
					{
						ProcessPipeRead(overlapped, succeeded);
					}
					break;

				case MyOverlapped::PipeWrite:
					{
						ProcessPipeWrite(overlapped, succeeded);
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

			#pragma region ProcessPipeConnect

			void Core::ProcessPipeConnect(MyOverlapped* overlapped, bool succeeded)
			{
				if (!succeeded)
					throw gcnew NotImplementedException("Don't know how to handle failure while connecting a pipe.");

				__int32 reference = overlapped->reference;				
				Object^ channelobj;
				if (references->TryGetValue(reference, channelobj))
				{
					Channel^ channel = dynamic_cast<Channel^>(channelobj);
					if (channel == nullptr)
						throw gcnew Exception("Internal error while processing pipe connect completion, reference (" + 
							reference.ToString() + ") does not point to a channel.");

					switch (overlapped->bufferno)
					{
					case 1:
						channel->Incoming();
						break;

					case 2:
						channel->Outgoing();
						break;

					default:
						throw gcnew NotImplementedException();
					}
				}
				else
				{
					Error("Could not locate the channel corresponding to reference (" + reference.ToString() + ")");
					delete overlapped;
				}
			}

			#pragma endregion

			#pragma region ProcessPipeRead

			void Core::ProcessPipeRead(MyOverlapped* overlapped, bool succeeded)
			{
				if (!succeeded)
					throw gcnew NotImplementedException("Don't know how to handle failure while reading from a pipe.");

				__int32 reference = overlapped->reference;				
				Object^ channelobj;
				if (references->TryGetValue(reference, channelobj))
				{
					Channel^ channel = dynamic_cast<Channel^>(channelobj);
					if (channel == nullptr)
						throw gcnew Exception("Internal error while processing pipe read completion, reference (" + 
							reference.ToString() + ") does not point to a channel.");
					channel->Incoming();
				}
				else
				{
					Error("Could not locate the channel corresponding to reference (" + reference.ToString() + ")");
					delete overlapped;
				}
			}

			#pragma endregion

			#pragma region ProcessPipeWrite

			void Core::ProcessPipeWrite(MyOverlapped* overlapped, bool succeeded)
			{
				if (!succeeded)
					throw gcnew NotImplementedException("Don't know how to handle failure while writing to a pipe.");

				__int32 reference = overlapped->reference;				
				Object^ channelobj;
				if (references->TryGetValue(reference, channelobj))
				{
					Channel^ channel = dynamic_cast<Channel^>(channelobj);
					if (channel == nullptr)
						throw gcnew Exception("Internal error while processing pipe write completion, reference (" + 
							reference.ToString() + ") does not point to a channel.");
					channel->Outgoing();
				}
				else
				{
					Error("Could not locate the channel corresponding to reference (" + reference.ToString() + ")");
					delete overlapped;
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
					completedFileIOs->Enqueue(DeferredCompletedFileIO(file, num_bytes_transmitted, overlapped, succeeded));
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

			#pragma region QS.TMS.Inspection.Inspectable

			QS::Fx::Inspection::IAttributeCollection^ Core::Senders::get()
			{
				QS::Fx::Inspection::AttributeCollection^ attributes = gcnew QS::Fx::Inspection::AttributeCollection("Senders");
				for each (KeyValuePair<Address^,Sender^> element in senders)
					attributes->Add(gcnew QS::Fx::Inspection::ScalarAttribute(
						element.Key->IPAddress->ToString() + ":" + element.Key->PortNumber.ToString(), element.Value));				
				return attributes;
			}

			QS::Fx::Inspection::IAttributeCollection^ Core::Listeners::get()
			{
				QS::Fx::Inspection::AttributeCollection^ attributes = gcnew QS::Fx::Inspection::AttributeCollection("Listeners");
				for each (KeyValuePair<Address^,Listener^> element in listeners)
					attributes->Add(gcnew QS::Fx::Inspection::ScalarAttribute(
						element.Key->IPAddress->ToString() + ":" + element.Key->PortNumber.ToString(), element.Value));				
				return attributes;
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

			#pragma region Managing Files

			IFile^ Core::OpenFile(String^ filename)
			{
				__int32 reference = Interlocked::Increment(last_reference);
				File^ file = gcnew File(completionPort, reference, filename);

				references->Add(reference, file);

				return file;
			}

			IFile^ Core::OpenFile(String^ filename, FileMode mode, FileAccess access, FileShare share, FileFlagsAndAttributes flagsAndAttributes)
			{
				__int32 reference = Interlocked::Increment(last_reference);
				File^ file = gcnew File(completionPort, reference, filename, mode, access, share, flagsAndAttributes);

				references->Add(reference, file);

				return file;
			}

			#pragma endregion
		}
	}
}
