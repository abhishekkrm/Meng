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

#define OPTION_SUPPORTING_UNMANAGED_APPLICATIONS



// #include "ICore.h"
// #include "IListenerController.h"
#include "Listener.h"
#include "Overlapped.h"
// #include "IAlarmController.h"
// #include "Alarm.h"
// #include "ISenderController.h"
#include "Sender.h"
// #include "IIOController.h"
// #include "IPriorityQueue.h"
// #include "SplayTree.h"
#include "Clock.h"
// #include "CompatibilityACW.h"
#include "InterlockedQueue.h"
#include "File.h"
#include "../../_core_x_/Unmanaged/MemoryChannel.h"
#include "../../_core_x_/Unmanaged/UnprotectedBuffer.h"
#include "ReceiveBuffer.h"
#include "Channel.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

// ----- THESE PRODUCE HUGE DATA DUMPS -----

// #define DEBUG_CollectCoreStatistics
// #define DEBUG_CollectStatisticsForIO
// #define DEBUG_MeasureCoreConcurrencyStatistics
// #define DEBUG_RegisterCoreActions

// ------------------------------------------------------------------

#define OPTION_UseDeferredPriorityBasedIOProcessing

#define DEBUG_UntagIncomingStreams
#define CheckingThreadIDForNonReentrantCalls
#define AllowSynchronousReceive
#define CatchAddressErrorsOnDispatch

#define DEBUG_CollectPeriodicStatistics
// #define DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation
#define DEBUG_LogReceiveRates
#define DEBUG_LogWriteCompletionRates

// #define DEBUG_RegisterSchedulingHistory
// #define DEBUG_CollectStatisticsForAlarms
// #define DEBUG_CollectSocketWaitingStatistics
// #define DEBUG_CollectReceiveTypes
// #define DEBUG_RememberMostRecentAlarms
// #define DEBUG_PeriodicallyMeasureLocalStatistics
// #define DEBUG_LogTimeWarps
// #define DEBUG_LogIndividualReceiveTimes
// #define DEBUG_LogOnReceive
// #define LogStreamAssignments
// #define LogCreationOfListeners

#define OPTION_AllowPeriodicDroppingOfMessages
// #define OPTION_AllowPeriodicDroppingOfMessages_BySeqno

#define OPTION_USE_BUFFER_MANAGER

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			[QS::Fx::Base::Inspectable]
			[QS::_core_c_::Diagnostics::ComponentContainerAttribute]
			public ref class Core sealed : public QS::Fx::Inspection::Inspectable, public ICore, public IListenerController, 
				public IAlarmController, public ISenderController, public IIOController, public QS::Fx::Clock::IClock,
				public QS::Fx::Clock::IAlarmClock, public QS::Fx::Logging::ILogger, public QS::Fx::Logging::IEventLogger,
				public QS::_core_c_::Diagnostics2::IModule, public QS::_core_c_::Core::IChannelController
			{
			public:

				#pragma region Public Stuff

				#pragma region IChannelController Members

				virtual IChannel^ OpenChannel(String ^id, String^ name, IChannel^ channelclient) = QS::_core_c_::Core::IChannelController::Open;
				virtual void CloseChannel(String ^id) = QS::_core_c_::Core::IChannelController::Close;

				property bool ChannelController
				{
					bool get() { return this->channelcontroller; }
					void set(bool value) { this->channelcontroller = value; }
				}

				property int ChannelCapacityOutgoing
				{
					int get() { return this->channelcapacityoutgoing; }
					void set(int value) { this->channelcapacityoutgoing = value; }
				}

				property int ChannelCapacityIncoming
				{
					int get() { return this->channelcapacityincoming; }
					void set(int value) { this->channelcapacityincoming = value; }
				}

				property int ChannelCapacityOutgoingControl
				{
					int get() { return this->channelcapacityoutgoingcontrol; }
					void set(int value) { this->channelcapacityoutgoingcontrol = value; }
				}

				property int ChannelCapacityIncomingControl
				{
					int get() { return this->channelcapacityincomingcontrol; }
					void set(int value) { this->channelcapacityincomingcontrol = value; }
				}

				#pragma endregion

				#pragma region UntaggingIncomingStreams

				const static bool UntaggingIncomingStreams = 
#if defined(DEBUG_UntagIncomingStreams)
					true
#else
					false
#endif
					;

				#pragma endregion

				#pragma region Defaults

				static const int default_MaximumConcurrency = 10000;
				static const int default_MinimumTransmitted = 1000;
				static const int default_MaximumTransmitted = 10000000;

				static const int default_SenderMTU = 65535;
				static const int default_SenderMaximumConcurrency = 100;				

				static const double default_SenderMaximumUnicastRate = 300; // double::PositiveInfinity;
				static const double default_SenderMaximumMulticastRate = 1000; // double::PositiveInfinity;
				static const double default_SenderMaximumUnicastCredits = 100;
				static const double default_SenderMaximumMulticastCredits = 100;

				static const double default_maximumQuantumForAlarms = 0.005;
				static const double default_maximumQuantumForCompletionPort = 0.05;

				static const double default_timeWarpThresholdForAlarms = 0.05;
				static const double default_timeWarpThresholdForIO = 0.05;

				static const int DefaultSizeOfTheMemoryChannel = 1000;

				#pragma endregion

				#pragma region COnstructor and destructor

				Core(String^ workingroot);
				Core(String^ workingroot, QS::Fx::Scheduling::IQueue^ downcalls, bool pinthreads);

				~Core();

				#pragma endregion

				#pragma region Processing Nonblocking

//				virtual void ProcessNonblocking(QS::_core_c_::Core::IRequest^ request) = QS::_core_c_::Synchronization::INonblockingWorker<QS::_core_c_::Core::IRequest^>::Process
//				{
//					ScheduleCall(request);
//				}

				#pragma endregion

				#pragma region ICore Members

				virtual void Schedule(QS::Fx::Base::IEvent^ e) = QS::Fx::Scheduling::IScheduler::Schedule;

				// virtual void ScheduleCall(IRequest^ downcall);
				
				virtual IAsyncResult^ BeginExecute(AsyncCallback^ callback, Object^ context) = QS::Fx::Scheduling::IScheduler::BeginExecute;
				
				__inline virtual void EndExecute(IAsyncResult^ result) = QS::Fx::Scheduling::IScheduler::EndExecute
				{
				}

				__inline virtual void Execute(AsyncCallback^ callback, Object^ context) = QS::Fx::Scheduling::IScheduler::Execute
				{
					BeginExecute(callback, context);
				}

				virtual QS::Fx::Clock::IAlarm^ Schedule(double timeout, QS::Fx::Clock::AlarmCallback^ callback, Object^ context);
				virtual IListener^ Listen(Address^ address, QS::Fx::Network::ReceiveCallback^ callback, Object^ context, 
					int maximumSize, int numberOfBuffers, bool drainSynchronously, int sizeOfTheAdfBuffer, bool high_priority);

				virtual void Start();
				virtual void Stop();

				virtual property bool Running
				{
					bool get() { return running; }
				}

				virtual property double Time
				{
					double get() 
					{ 
						time = clock->Time;
						return time; 
					}
				}

				virtual event QS::Fx::Base::Callback^ OnInitialization
				{
					void add(QS::Fx::Base::Callback^ callback) { initializationCallback += callback; }
					void remove(QS::Fx::Base::Callback^ callback) { initializationCallback -= callback; }
				}

				virtual event QS::Fx::Base::Callback^ OnCleanup
				{
					void add(QS::Fx::Base::Callback^ callback) { cleanupCallback += callback; }
					void remove(QS::Fx::Base::Callback^ callback) { cleanupCallback -= callback; }
				}

				virtual event ErrorCallback^ OnError
				{
					void add(ErrorCallback^ callback) { errorCallback += callback; }
					void remove(ErrorCallback^ callback) { errorCallback -= callback; }
				}

				virtual property QS::Fx::Logging::ILogger^ Logger
				{
					void set(QS::Fx::Logging::ILogger^ logger) { this->logger = logger; }
				}

				virtual property QS::Fx::Logging::IEventLogger^ EventLogger
				{
					void set(QS::Fx::Logging::IEventLogger^ eventLogger) { this->eventLogger = eventLogger; }
				}

				virtual ISender^ GetSender(Address^, int sizeOfTheAdfBuffer, int numberOfTransmitters, bool high_priority) = ICore::GetSender;

				virtual IFile^ OpenFile(String^ filename) = QS::_core_c_::Core::IFileController::OpenFile;

				virtual IFile^ OpenFile(String^ filename, FileMode mode, FileAccess access, FileShare share, FileFlagsAndAttributes flagsAndAttributes) = QS::_core_c_::Core::IFileController::OpenFile;

				virtual property QS::_core_c_::Statistics::IStatisticsController^ StatisticsController
				{
					QS::_core_c_::Statistics::IStatisticsController^ get() = QS::_core_c_::Core::ICore::StatisticsController::get 
					{ 
						return statisticsController; 
					}
				}

				#pragma endregion

				#pragma region IConsole Members

				virtual void writeLine(String^ s) = QS::Fx::Logging::IConsole::Log;

				#pragma endregion

				#pragma region ILogger Members

				virtual void clear() = QS::Fx::Logging::ILogger::Clear;
				virtual void logMessage(Object^ source, String^ message) = QS::Fx::Logging::ILogger::Log;

				#pragma endregion

				#pragma region IEventLogger Members

				virtual void Log(QS::Fx::Logging::IEvent^ eventToLog) = QS::Fx::Logging::IEventLogger::Log;				

				#pragma endregion

				#pragma region IDiagnosticsComponent Members

				virtual property QS::Fx::Diagnostics::ComponentClass Class_AsDiagnosticsComponent
				{
					QS::Fx::Diagnostics::ComponentClass get() = QS::Fx::Diagnostics::IDiagnosticsComponent::Class::get
					{ 
						return QS::Fx::Diagnostics::ComponentClass::Other; 
					}
				}

				virtual property bool Enabled_AsDiagnosticsComponent
				{
					bool get() = QS::Fx::Diagnostics::IDiagnosticsComponent::Enabled::get
					{ 
						return true; 
					}

					void set(bool value) = QS::Fx::Diagnostics::IDiagnosticsComponent::Enabled::set
					{ 
						throw gcnew NotSupportedException(); 
					}
				}

				virtual void ResetComponent() = QS::Fx::Diagnostics::IDiagnosticsComponent::ResetComponent
				{
					throw gcnew NotSupportedException();
				}

				#pragma endregion

				#pragma region IListenerController Members

				virtual void Unregister(IListener^ listener) = QS::_core_c_::Core::IListenerController::Unregister;

				#pragma endregion

				#pragma region IAlarmController Members

				virtual void Register(QS::Fx::Clock::IAlarm^ alarm);
				virtual void Unregister(QS::Fx::Clock::IAlarm^ alarm) = QS::_core_c_::Core::IAlarmController::Unregister;

				#pragma endregion

				#pragma region ISenderController Members

				virtual void ScheduleSender(ISender^ toSchedule) = QS::_core_c_::Core::ISenderController::ScheduleSender;
				virtual void ReleaseResources(int numberOfMessages, int numberOfBytes);

				#pragma endregion

				#pragma region IIOController Members

				virtual property __int32 AllocateReference //  = QS::_core_c_::Core::IIOController::AllocateReference
				{
					__int32 get() { return Interlocked::Increment(last_reference); }
				}

				virtual void RegisterReference(__int32 reference, Object^ target) // = IIOController::RegisterReference
				{
					references->Add(reference, target);
				}

				#pragma endregion

				#pragma region Accessors used to edit configuration

				property QS::_core_c_::RateControl::IRateControllerClass ^ DefaultUnicastRateControllerClass
				{
					QS::_core_c_::RateControl::IRateControllerClass^ get() { return unicastControllers; }
					void set(QS::_core_c_::RateControl::IRateControllerClass^ controllers) { unicastControllers = controllers; }
				}
				
				property QS::_core_c_::RateControl::IRateControllerClass ^ DefaultMulticastRateControllerClass
				{
					QS::_core_c_::RateControl::IRateControllerClass^ get() { return multicastControllers; }
					void set(QS::_core_c_::RateControl::IRateControllerClass^ controllers) { multicastControllers = controllers; }
				}

				property double MaximumQuantumForAlarms
				{
					double get() { return maximum_alarms_quantum; }
					void set(double value) { maximum_alarms_quantum = value; }
				}

				property double MaximumQuantumForCompletionPorts
				{
					double get() { return maximum_io_quantum; }
					void set(double value) { maximum_io_quantum = value; }
				}

				property int MaximumConcurrency
				{
					int get() { return maximumConcurrency; }
					void set(int value) { maximumConcurrency = value; }
				}

				property int MinimumTransmitted
				{
					int get() { return minimumTransmitted; }
					void set(int value) { minimumTransmitted = value; }
				}

				property int MaximumTransmitted
				{
					int get() { return maximumTransmitted; }
					void set(int value) { maximumTransmitted = value; }
				}

				property int DefaultMTU
				{
					int get() { return defaultMTU; }
					void set(int value) { defaultMTU = value; }
				}

				property int DefaultMaximumSenderConcurrency
				{
					int get() { return defaultMaximumSenderConcurrency; }
					void set(int value) { defaultMaximumSenderConcurrency = value; }
				}

				property double DefaultMaximumSenderUnicastRate
				{
					double get() { return defaultMaximumSenderUnicastRate; }
					void set(double value) { defaultMaximumSenderUnicastRate = value; }
				}

				property double DefaultMaximumSenderMulticastRate
				{
					double get() { return defaultMaximumSenderMulticastRate; }
					void set(double value) { defaultMaximumSenderMulticastRate = value; }
				}

				property double DefaultMaximumSenderUnicastCredits
				{
					double get() { return defaultMaximumSenderUnicastCredits; }
					void set(double value) { defaultMaximumSenderUnicastCredits = value; }
				}

				property double DefaultMaximumSenderMulticastCredits
				{
					double get() { return defaultMaximumSenderMulticastCredits; }
					void set(double value) { defaultMaximumSenderMulticastCredits = value; }
				}

				property bool ContinueIOOnTimeWarps
				{
					bool get() { return timewarps_continueio; }
					void set(bool value) { timewarps_continueio = value; }
				}

				#pragma endregion

/*
				#pragma region QS.CMS.Base.IAlarmClock Members

				virtual QS::Fx::Clock::IAlarm^ Schedule(
					double timeSpanInSeconds, QS::Fx::Clock::AlarmCallback^ alarmCallback, Object^ argument);

				#pragma endregion

				#pragma region QS.CMS.Base2.IClock Members

				virtual void Adjust(double adjustment)
				{
					throw gcnew NotSupportedException("This clock cannot be adjusted.");
				}

				#pragma endregion
*/

				#pragma region PhysicalClock

				virtual property QS::Fx::Clock::IClock^ PhysicalClock
				{
					QS::Fx::Clock::IClock^ get() { return clock; }
				}

				#pragma endregion

				#pragma region Diagnostics.IModule Members

				virtual property QS::_core_c_::Diagnostics2::IComponent^ DiagnosticsComponent
				{
					QS::_core_c_::Diagnostics2::IComponent^ get() = QS::_core_c_::Diagnostics2::IModule::Component::get
					{
						return diagnosticsContainer; 
					}
				}

				#pragma endregion

				#pragma region Other accessors

				property bool DiscardIncomingIO
				{
					bool get() { return discard_incoming_io; }
					void set(bool value) { discard_incoming_io = value; }
				}

				#pragma endregion

				#pragma region Controlling Dropping

				void Drop(double timeout, double interval, bool repeat)
				{
#if defined(OPTION_AllowPeriodicDroppingOfMessages)
					repeat_dropping_interval = timeout;
					dropping_interval = interval;
					repeat_dropping = repeat;					
					next_start_dropping = clock->Time + timeout;
#else
					throw gcnew NotSupportedException();
#endif

				}

				void Drop(double interval_between_dropping, int how_many_to_drop, bool repeat_dropping)
				{
#if defined(OPTION_AllowPeriodicDroppingOfMessages_BySeqno)
					this->interval_between_dropping = interval_between_dropping;
					this->how_many_to_drop = how_many_to_drop;
					this->repeat_dropping = repeat_dropping;					
					next_dropping_time = clock->Time + interval_between_dropping;
#else
					throw gcnew NotSupportedException();
#endif
				}

				#pragma endregion

				#pragma region Channels

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
				
				property IntPtr IncomingChannel
				{
					IntPtr get() { return IntPtr((void *) incomingchannel); }
				}
				
				property IntPtr OutgoingChannel
				{
					IntPtr get() { return IntPtr((void *) outgoingchannel); }
				}

#endif

				#pragma endregion

				#pragma region OPTION_SUPPORTING_UNMANAGED_APPLICATIONS

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
				
				property QS::_core_x_::Unmanaged::OutgoingMsgCallback^ OutgoingMsgCallback
				{
					QS::_core_x_::Unmanaged::OutgoingMsgCallback^ get() { return outgoingmsgcallback; }
					void set(QS::_core_x_::Unmanaged::OutgoingMsgCallback^ value) { outgoingmsgcallback = value; }
				}

				void AcknowledgeOutgoingMsg(QS::_core_x_::Unmanaged::OutgoingMsg^ message);
				void UnmanagedLoopback(QS::_core_x_::Unmanaged::OutgoingMsg^ message);
				void ProcessIncomingMsg(QS::_core_x_::Unmanaged::IncomingMsg^ message);

#endif

				#pragma endregion

				#pragma endregion

			private:

				bool pinthreads;

				#pragma region Private Stuff

				void Constructor(String^ workingroot, QS::Fx::Scheduling::IQueue^ downcalls, bool pinthreads);

				#pragma region OPTION_USE_BUFFER_MANAGER

#if defined(OPTION_USE_BUFFER_MANAGER)
				
				// ReceiveBuffer::Controller^ buffercontroller;
				
#endif

				#pragma endregion

				#pragma region OPTION_AllowPeriodicDroppingOfMessages

#if defined(OPTION_AllowPeriodicDroppingOfMessages)

				bool does_dropping, dropping_now, repeat_dropping;
				double next_start_dropping, next_stop_dropping, dropping_interval, repeat_dropping_interval;

				__inline void _update_dropping()
				{							
					if (does_dropping)
					{
						if (dropping_now)
						{
							if (time > next_stop_dropping)
							{
								dropping_now = false;
								next_start_dropping = repeat_dropping ? (time + repeat_dropping_interval) : double::PositiveInfinity;
							}
						}
						else
						{
							if (time > next_start_dropping)
							{
								dropping_now = true;
								next_stop_dropping = time + dropping_interval;
							}
						}
					}
					else
						dropping_now = false;
				}

#endif

				#pragma endregion

				#pragma region OPTION_AllowPeriodicDroppingOfMessages_BySeqno

#if defined(OPTION_AllowPeriodicDroppingOfMessages_BySeqno)

				bool dropping_now, repeat_dropping;
				int how_many_to_drop, ndropped;
				double interval_between_dropping, next_dropping_time;
					 
				__inline bool check_dropping()
				{					
					if (dropping_now)
					{
						ndropped++;
						if (ndropped >= how_many_to_drop)
							dropping_now = false;
						return true;
					}
					else
					{
						if (time > next_dropping_time)
						{
							dropping_now = true;
							ndropped = 1;
							next_dropping_time = repeat_dropping ? (time + interval_between_dropping) : double::PositiveInfinity;
							return true;
						}
						else
							return false;
					}
				}

#endif

				#pragma endregion

				#pragma region Private Fields

				bool channelcontroller;
				int channelcapacityoutgoing, channelcapacityincoming, channelcapacityoutgoingcontrol, channelcapacityincomingcontrol;
				QS::_core_c_::Statistics::IStatisticsController^ statisticsController;
				String^ workingroot;
				QS::_core_c_::RateControl::IRateControllerClass ^ unicastControllers, ^ multicastControllers;
				void DumpLogs();
				void UnhandledExceptionHandler(Object^ sender, UnhandledExceptionEventArgs^ args);
				QS::_core_c_::Diagnostics2::Container ^ diagnosticsContainer, ^ diagnosticsContainerForSenders, ^ diagnosticsContainerForListeners;
				QS::Fx::Logging::ILogger^ logger;
				QS::Fx::Logging::IEventLogger^ eventLogger;
				Clock^ clock;
				double time, timeWarpThresholdForAlarms, timeWarpThresholdForIO, defaultMaximumSenderUnicastRate, 
					defaultMaximumSenderMulticastRate, defaultMaximumSenderUnicastCredits, defaultMaximumSenderMulticastCredits, 
					maximum_alarms_quantum, maximum_io_quantum;
				IntPtr completionPort;
				bool running, finished, drain_completion_ports;
				System::Threading::ManualResetEvent^ runningAllowed; 
				Thread^ thread;
				event ErrorCallback^ errorCallback;
				event QS::Fx::Base::Callback^ initializationCallback;
				event QS::Fx::Base::Callback^ cleanupCallback;
				__int32 last_reference;
				IDictionary<__int32, Object^>^ references;
				[QS::_core_c_::Diagnostics::ComponentCollection]
				IDictionary<Address^, QS::_core_c_::Core::Listener^>^ listeners;	
				[QS::_core_c_::Diagnostics::ComponentCollection]
				IDictionary<Address^, Sender^>^ senders;
				int nstreams;
				int concurrency, maximumConcurrency, transmitted, minimumTransmitted, maximumTransmitted, defaultMTU,
					defaultMaximumSenderConcurrency;
				Queue<Sender^> ^ registeredSenders, ^ sendersToKeep;
				IPriorityQueue<Alarm^>^ alarmQueue;
				Queue<Alarm^>^ expiredAlarms;
				double timewarps_lasttime;
				bool timewarps_continueio;
				bool discard_incoming_io;
				[QS::Fx::Base::Inspectable]
				QS::Fx::Scheduling::IQueue^ downcalls;
				[QS::Fx::Base::Inspectable]
				int will_process_downcalls;
				System::Collections::Generic::IDictionary<String^, QS::_core_c_::Core::Channel^>^ mychannels;

				#pragma endregion

				void ProcessPipeConnect(MyOverlapped* overlapped, bool succeeded);
				void ProcessPipeRead(MyOverlapped* overlapped, bool succeeded);
				void ProcessPipeWrite(MyOverlapped* overlapped, bool succeeded);

				#pragma region OPTION_UseDeferredPriorityBasedIOProcessing

#if defined(OPTION_UseDeferredPriorityBasedIOProcessing)

				value class DeferredProcessReceive
				{
				public:

					DeferredProcessReceive(MyOverlapped* overlapped, bool completed, bool succeeded, Listener^ listener, Socket^ socket)
					{
						this->overlapped = overlapped;
						this->completed = completed;
						this->succeeded = succeeded;
						this->listener = listener;
						this->socket = socket;
					}

					MyOverlapped* overlapped;
					bool completed, succeeded;
					Listener^ listener;
					Socket^ socket;
				};

				value class DeferredCompletedFileIO
				{
				public:

					DeferredCompletedFileIO(File^ file, unsigned int ntransmitted, MyOverlapped* overlapped, bool succeeded)
					{
						this->ntransmitted = ntransmitted;
						this->file = file;
						this->overlapped = overlapped;
						this->succeeded = succeeded;
					}

					unsigned int ntransmitted;
					File^ file;
					MyOverlapped* overlapped;
					bool succeeded;
				};

				Queue<Transmitter^> ^ completedTransmitters, ^ completedPriorityTransmitters;
				Queue<DeferredProcessReceive> ^ receiveCompletions, ^ priorityReceiveCompletions;
				Queue<DeferredCompletedFileIO> ^ completedFileIOs;
#endif

				#pragma endregion

				#pragma region OPTION_SUPPORTING_UNMANAGED_APPLICATIONS

#if defined(OPTION_SUPPORTING_UNMANAGED_APPLICATIONS)
				
				QS::_core_x_::Unmanaged::MemoryChannel *incomingchannel, *outgoingchannel;
				QS::_core_x_::Unmanaged::UnprotectedBuffer *pendingincoming, *pendingoutgoing;
				QS::_core_x_::Unmanaged::OutgoingMsgCallback^ outgoingmsgcallback;

				void _CommunicateToUnmanagedApplication();
				void _CommunicateToUnmanagedApplication(QS::Fx::Unmanaged::Message request);
				void ProcessMessagesFromUnmanagedApplication();

#endif

				#pragma endregion

				#pragma region Various Private Methods

				enum class ProcessingContext : int
				{
					Alarms1 = 1, Alarms2, Alarms3, Alarms4, Alarms5, IO1, IO2, IO3, IO4, IO5
				};

				__inline void timewarps_initialize()
				{
					timewarps_lasttime = time;
				}

				__inline bool timewarps_recheck(ProcessingContext context, double timewarp_threshold)
				{
					return timewarps_recheck(context, timewarp_threshold, 0);
				}

				__inline bool timewarps_recheck(ProcessingContext context, double timewarp_threshold, double duration_correction)
				{
					bool warp_occurred = time > timewarps_lasttime + timewarp_threshold;
#if defined(DEBUG_LogTimeWarps)
					if (warp_occurred)
					{
						double sample_time = timewarps_lasttime, warp_duration = time - timewarps_lasttime - duration_correction;
						ts_timeWarps->Add(QS::_core_e_::Data::XY(sample_time, warp_duration));
						ts_timeWarpContexts->Add(QS::_core_e_::Data::XY(sample_time, (int) context));
					}
#endif
					timewarps_lasttime = time;
					return warp_occurred;
				}

				void MainLoop();
				bool CheckCompletionPort(double waiting);
				void FireAlarm(Alarm^ alarm);
				void ProcessIO(MyOverlapped* overlapped, unsigned int numberOfBytes, bool succeeded);
				void ProcessSendCompletion(MyOverlapped* overlapped, bool succeeded);
				void ProcessReadCompletion(MyOverlapped* overlapped, bool succeeded);
				void ProcessFileIOCompletion(MyOverlapped* overlapped, unsigned int ntransmitted, bool succeeded);

				__inline void LogMessage(String^ message)
				{
					if (logger != nullptr)
						logger->Log(this, message);
					else
						Error("Could not log.\n" + message);
				}

				void Error(String^ message);
				void ProcessReceive(MyOverlapped* overlapped, bool completed, bool succeeded, Listener^ listener, Socket^ socket);
				void DispatchReceived(MyOverlapped* overlapped, int numberOfBytesReceiver, Listener^ listener, bool asynchronous);
				// Sender^ GetSender(Address^ address);
				void ProcessSenders();

				[QS::Fx::Base::Inspectable]
				property QS::Fx::Inspection::IAttributeCollection^ Senders
				{
					QS::Fx::Inspection::IAttributeCollection^ get();
				}

				[QS::Fx::Base::Inspectable]
				property QS::Fx::Inspection::IAttributeCollection^ Listeners
				{
					QS::Fx::Inspection::IAttributeCollection^ get();
				}

				void ProcessDowncalls();

				#pragma endregion

				#pragma region VerifyWhetherThisIsCoreThread

				__inline void VerifyWhetherThisIsCoreThread()
				{
#if defined(CheckingThreadIDForNonReentrantCalls)
					if (thread->ThreadState != System::Threading::ThreadState::Unstarted && 
						!ReferenceEquals(System::Threading::Thread::CurrentThread, thread))
							throw gcnew Exception("This functionality is only available from within the core thread.");
#endif
				}

				#pragma endregion

				#pragma region DEBUG_LogTimeWarps

#if defined(DEBUG_LogTimeWarps)
				
				List<QS::_core_e_::Data::XY> ^ ts_timeWarps, ^ ts_timeWarpContexts;

				[QS::_core_c_::Diagnostics::Component("Time Warp (X = time, Y = duration)")]
				[QS::_core_c_::Diagnostics2::Property("TimeWarps")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_TimeWarps
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_timeWarps->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Time Warp Contexts (X = time, Y = context)")]
				[QS::_core_c_::Diagnostics2::Property("TimeWarpContexts")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_TimeWarpContexts
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_timeWarpContexts->ToArray()); }
				}

#endif				
				
				#pragma endregion

				#pragma region DEBUG_PeriodicallyMeasureLocalStatistics

#if defined(DEBUG_PeriodicallyMeasureLocalStatistics)
				// ..................
#endif

				#pragma endregion

				#pragma region DEBUG_CollectPeriodicStatistics

#if defined(DEBUG_CollectPeriodicStatistics)

#if defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)
				double pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_WSAGetOverlappedResult,
					pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_TransmitterCompleted;
				int pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_WSAGetOverlappedResult,
					pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_TransmitterCompleted;
#endif

				System::Diagnostics::Process ^myprocess;

				double pstat_lastchecked, pstat_nextcheck, pstat_cumulatedPendingAlarmQueueLength, pstat_cumulatedPendingAlarmQueueTime, 
					pstat_cumulatedExpiredAlarmQueueTime, pstat_cumulatedExpiredAlarmQueueLength, pstat_maximumAlarmFiringDelay, 
					pstat_cumulatedAlarmFiringDelay, pstat_alarmsFiredSinceLastCheck, pstat_alarmsScheduledSinceLastCheck, 
					pstat_alarmsCanceledSinceLastCheck, pstat_alarmsRemovedSinceLastCheck, pstat_lastTotalProcessorTime, 
					pstat_lastUserProcessorTime, 
					pstat_processSendersCumulated, pstat_allowTransmissionCumulated;

				int pstat_maximumPendingAlarmQueueLength, pstat_maximumExpiredAlarmQueueLength, pstat_cumulatedAlarmFiringDelaySamples,
					pstat_numberOfTimesAnActiveSenderReturnedNothing, 
					pstat_processSendersNumOfSamples, pstat_allowTransmissionNumOfSamples;

				List<QS::_core_e_::Data::XY> ^ ts_averagePendingAlarmQueueLengths, ^ ts_maximumPendingAlarmQueueLengths,
					^ ts_averageExpiredAlarmQueueLengths, ^ ts_maximumExpiredAlarmQueueLengths, ^ ts_averageAlarmFiringDelays, 
					^ ts_maximumAlarmFiringDelays, ^ ts_alarmFiringRate, ^ ts_alarmSchedulingRate, ^ ts_alarmCancelingRate, ^ ts_alarmRemovingRate,
					^ ts_totalLocalProcessorUsage, ^ ts_userLocalProcessorUsage, ^ ts_rateAtWhichActiveSendersReturnNothing,
					^ ts_memoryAllocated;

#if defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)

				[QS::_core_c_::Diagnostics2::Property("ProcessSendCompletionDetailedProfilingInformation_WSAGetOverlappedResult")]
				QS::_core_c_::Statistics::ISamples2D^ ts_ProcessSendCompletionDetailedProfilingInformation_WSAGetOverlappedResult;

				[QS::_core_c_::Diagnostics2::Property("ProcessSendCompletionDetailedProfilingInformation_TransmitterCompleted")]
				QS::_core_c_::Statistics::ISamples2D^ ts_ProcessSendCompletionDetailedProfilingInformation_TransmitterCompleted;

#endif

				[QS::_core_c_::Diagnostics2::Property("Overhead_ProcessSenders")]
				QS::_core_c_::Statistics::ISamples2D^ ts_Overhead_ProcessSenders;

				[QS::_core_c_::Diagnostics2::Property("Overhead_AllowTransmission")]
				QS::_core_c_::Statistics::ISamples2D^ ts_Overhead_AllowTransmission;

				static const double PeriodicStatistics_PeriodLengthIsSeconds = 1;

				__inline void CheckUpdatePeriodicStatistics()
				{
					if (time > pstat_nextcheck)
						UpdatePeriodicStatistics();
				}

				void UpdatePeriodicStatistics()
				{
					if (true) // time > pstat_nextcheck)
					{
						if (time > pstat_lastchecked)
						{
							double sampletime = time;
							double time_elapsed = time - pstat_lastchecked;
							
							ts_Overhead_ProcessSenders->Add(
								sampletime, pstat_processSendersCumulated / ((double) pstat_processSendersNumOfSamples));
							ts_Overhead_AllowTransmission->Add(
								sampletime, pstat_allowTransmissionCumulated / ((double) pstat_allowTransmissionNumOfSamples));
							pstat_processSendersCumulated = pstat_allowTransmissionCumulated = 0;
							pstat_processSendersNumOfSamples = pstat_allowTransmissionNumOfSamples = 0;

#if defined(DEBUG_CollectPeriodicStatistics_ProcessSendCompletionDetailedProfilingInformation)

							ts_ProcessSendCompletionDetailedProfilingInformation_WSAGetOverlappedResult->Add(sampletime, 
								pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_WSAGetOverlappedResult /
								((double) pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_WSAGetOverlappedResult));

							ts_ProcessSendCompletionDetailedProfilingInformation_TransmitterCompleted->Add(sampletime, 
								pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_TransmitterCompleted /
								((double) pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_TransmitterCompleted));

							pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_WSAGetOverlappedResult = 0;
							pstat_ProcessSendCompletionDetailedProfilingInformation_Accumulated_TransmitterCompleted = 0;
							pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_WSAGetOverlappedResult = 0;
							pstat_ProcessSendCompletionDetailedProfilingInformation_NumberOfSamples_TransmitterCompleted = 0;
#endif

							ts_maximumPendingAlarmQueueLengths->Add(QS::_core_e_::Data::XY(sampletime, pstat_maximumPendingAlarmQueueLength));
							
							double cumulatedPendingAlarmQueueTimeDelta = pstat_cumulatedPendingAlarmQueueTime - pstat_lastchecked;
							if (cumulatedPendingAlarmQueueTimeDelta > 0)
								ts_averagePendingAlarmQueueLengths->Add(QS::_core_e_::Data::XY(sampletime, 
									pstat_cumulatedPendingAlarmQueueLength / cumulatedPendingAlarmQueueTimeDelta));

							pstat_maximumPendingAlarmQueueLength = alarmQueue->Count;
							pstat_cumulatedPendingAlarmQueueTime = time;
							pstat_cumulatedPendingAlarmQueueLength = 0;

							ts_maximumExpiredAlarmQueueLengths->Add(QS::_core_e_::Data::XY(sampletime, pstat_maximumExpiredAlarmQueueLength));
							
							double cumulatedExpiredAlarmQueueTimeDelta = pstat_cumulatedExpiredAlarmQueueTime - pstat_lastchecked;
							if (cumulatedExpiredAlarmQueueTimeDelta > 0)
								ts_averageExpiredAlarmQueueLengths->Add(QS::_core_e_::Data::XY(sampletime, 
									pstat_cumulatedExpiredAlarmQueueLength / cumulatedExpiredAlarmQueueTimeDelta));

							pstat_maximumExpiredAlarmQueueLength = expiredAlarms->Count;
							pstat_cumulatedExpiredAlarmQueueTime = time;
							pstat_cumulatedExpiredAlarmQueueLength = 0;

							ts_maximumAlarmFiringDelays->Add(QS::_core_e_::Data::XY(sampletime, pstat_maximumAlarmFiringDelay));
							
							if (pstat_cumulatedAlarmFiringDelaySamples > 0)
								ts_averageAlarmFiringDelays->Add(QS::_core_e_::Data::XY(sampletime, 
									pstat_cumulatedAlarmFiringDelay / pstat_cumulatedAlarmFiringDelaySamples));

							pstat_maximumAlarmFiringDelay = 0;
							pstat_cumulatedAlarmFiringDelaySamples = 0;
							pstat_cumulatedAlarmFiringDelay = 0;

							ts_alarmSchedulingRate->Add(QS::_core_e_::Data::XY(sampletime, pstat_alarmsScheduledSinceLastCheck / time_elapsed));
							ts_alarmFiringRate->Add(QS::_core_e_::Data::XY(sampletime, pstat_alarmsFiredSinceLastCheck / time_elapsed));
							ts_alarmCancelingRate->Add(QS::_core_e_::Data::XY(sampletime, pstat_alarmsCanceledSinceLastCheck / time_elapsed));
							ts_alarmRemovingRate->Add(QS::_core_e_::Data::XY(sampletime, pstat_alarmsRemovedSinceLastCheck / time_elapsed));

							pstat_alarmsScheduledSinceLastCheck = 0;
							pstat_alarmsFiredSinceLastCheck = 0;
							pstat_alarmsCanceledSinceLastCheck = 0;
							pstat_alarmsRemovedSinceLastCheck = 0;
						
							double totalcputime = myprocess->TotalProcessorTime.TotalSeconds, usercputime = myprocess->UserProcessorTime.TotalSeconds;
							ts_totalLocalProcessorUsage->Add(QS::_core_e_::Data::XY(sampletime, (totalcputime - pstat_lastTotalProcessorTime) / time_elapsed));
							ts_userLocalProcessorUsage->Add(QS::_core_e_::Data::XY(sampletime, (usercputime - pstat_lastUserProcessorTime) / time_elapsed));
							pstat_lastTotalProcessorTime = totalcputime;
							pstat_lastUserProcessorTime = usercputime;

							ts_rateAtWhichActiveSendersReturnNothing->Add(QS::_core_e_::Data::XY(sampletime, 
								((double) pstat_numberOfTimesAnActiveSenderReturnedNothing) / time_elapsed));

							pstat_numberOfTimesAnActiveSenderReturnedNothing = 0;

#if defined(DEBUG_LogWriteCompletionRates)
							ts_writeCompletionRate->Add(QS::_core_e_::Data::XY(sampletime, ((double) numWriteCompletions) / time_elapsed));
							numWriteCompletions = 0;
#endif

							ts_memoryAllocated->Add(QS::_core_e_::Data::XY(sampletime, ((double) GC::GetTotalMemory(false))));
						}

						pstat_lastchecked = time;
						pstat_nextcheck = time + PeriodicStatistics_PeriodLengthIsSeconds;
					}
				}

				__inline void UpdateAlarmQueueSizes()
				{
					int npending = alarmQueue->Count;
					int nexpired = expiredAlarms->Count;

					if (npending > pstat_maximumPendingAlarmQueueLength)
						pstat_maximumPendingAlarmQueueLength = npending;
					pstat_cumulatedPendingAlarmQueueLength += (time - pstat_cumulatedPendingAlarmQueueTime) * ((double) npending);
					pstat_cumulatedPendingAlarmQueueTime = time;

					if (nexpired > pstat_maximumExpiredAlarmQueueLength)
						pstat_maximumExpiredAlarmQueueLength = nexpired;
					pstat_cumulatedExpiredAlarmQueueLength += (time - pstat_cumulatedExpiredAlarmQueueTime) * ((double) nexpired);
					pstat_cumulatedExpiredAlarmQueueTime = time;
				}

#if defined(DEBUG_LogWriteCompletionRates)
				List<QS::_core_e_::Data::XY> ^ ts_writeCompletionRate;
				int numWriteCompletions;

				[QS::_core_c_::Diagnostics::Component("Write Completion Rate (X = time, Y = rate)")]
				[QS::_core_c_::Diagnostics2::Property("WriteCompletionRate")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_WriteCompletionRate
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_writeCompletionRate->ToArray()); }
				}
#endif

				[QS::_core_c_::Diagnostics::Component("Total Local Processor Usage (X = time, Y = usage)")]
				[QS::_core_c_::Diagnostics2::Property("TotalLocalProcessorUsage")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_TotalLocalProcessorUsage
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_totalLocalProcessorUsage->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("User Local Processor Usage (X = time, Y = usage)")]
				[QS::_core_c_::Diagnostics2::Property("UserLocalProcessorUsage")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_UserLocalProcessorUsage
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_userLocalProcessorUsage->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Alarm Scheduling Rate (X = time, Y = rate)")]
				[QS::_core_c_::Diagnostics2::Property("AlarmSchedulingRate")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AlarmSchedulingRate
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_alarmSchedulingRate->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Alarm Firing Rate (X = time, Y = rate)")]
				[QS::_core_c_::Diagnostics2::Property("AlarmFiringRate")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AlarmFiringRate
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_alarmFiringRate->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Alarm Canceling Rate (X = time, Y = rate)")]
				[QS::_core_c_::Diagnostics2::Property("AlarmCancelingRate")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AlarmCancelingRate
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_alarmCancelingRate->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Average Alarm Firing Delays (X = time, Y = delay)")]
				[QS::_core_c_::Diagnostics2::Property("AverageAlarmFiringDelays")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AverageAlarmFiringDelays
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_averageAlarmFiringDelays->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Maximum Alarm Firing Delays (X = time, Y = delay)")]
				[QS::_core_c_::Diagnostics2::Property("MaximumAlarmFiringDelays")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_MaximumAlarmFiringDelays
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_maximumAlarmFiringDelays->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Average Pending Alarm Queue Lengths (X = time, Y = length)")]
				[QS::_core_c_::Diagnostics2::Property("AveragePendingAlarmQueueLengths")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AveragePendingAlarmQueueLengths
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_averagePendingAlarmQueueLengths->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Maximum Pending Alarm Queue Lengths (X = time, Y = length)")]
				[QS::_core_c_::Diagnostics2::Property("MaximumPendingAlarmQueueLengths")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_MaximumPendingAlarmQueueLengths
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_maximumPendingAlarmQueueLengths->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Average Expired Alarm Queue Lengths (X = time, Y = length)")]
				[QS::_core_c_::Diagnostics2::Property("AverageExpiredAlarmQueueLengths")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AverageExpiredAlarmQueueLengths
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_averageExpiredAlarmQueueLengths->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Maximum Expired Alarm Queue Lengths (X = time, Y = length)")]
				[QS::_core_c_::Diagnostics2::Property("MaximumExpiredAlarmQueueLengths")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_MaximumExpiredAlarmQueueLengths
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_maximumExpiredAlarmQueueLengths->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Rate At Which Active Senders Return Nothing (X = time, Y = length)")]
				[QS::_core_c_::Diagnostics2::Property("RateAtWhichActiveSendersReturnNothing")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_RateAtWhichActiveSendersReturnNothing
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_rateAtWhichActiveSendersReturnNothing->ToArray()); }
				}

				[QS::_core_c_::Diagnostics::Component("Memory Allocated (X = time, Y = bytes)")]
				[QS::_core_c_::Diagnostics2::Property("MemoryAllocated")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_MemoryAllocated
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(ts_memoryAllocated->ToArray()); }
				}

#endif
				#pragma endregion

				#pragma region DEBUG_UntagIncomingStreams

#if defined(DEBUG_UntagIncomingStreams)

				#pragma region Class ReceiveStatus

				[QS::_core_c_::Diagnostics::ComponentContainerAttribute]
				ref class ReceiveStatus sealed : public QS::Fx::Inspection::Inspectable, public QS::_core_c_::Diagnostics2::IModule
				{
				public:

					ReceiveStatus(IPAddress^ sourceAddress);

					#pragma region Diagnostics.IModule Members

					virtual property QS::_core_c_::Diagnostics2::IComponent^ DiagnosticsComponent
					{
						QS::_core_c_::Diagnostics2::IComponent^ get() = QS::_core_c_::Diagnostics2::IModule::Component::get
						{
							return diagnosticsContainer; 
						}
					}

					#pragma endregion

					#pragma region Received

					__inline void Received(int streamid, int seqno, double time)
					{
						Stream^ incomingStream;
						if (!incomingStreams->TryGetValue(streamid, incomingStream))
						{
							incomingStream = gcnew Stream(streamid);
							incomingStreams->Add(streamid, incomingStream);
							(static_cast<QS::_core_c_::Diagnostics2::IContainer^>(diagnosticsContainerForIncomingStreams))->Register(
								"[" + streamid.ToString("0000") + "]", (static_cast<QS::_core_c_::Diagnostics2::IModule^>(incomingStream))->Component);
						}

						incomingStream->Received(seqno, time);
					}

					#pragma endregion

				private:

					#pragma region Class Stream

					[QS::_core_c_::Diagnostics::ComponentContainerAttribute]
					ref class Stream sealed : public QS::Fx::Inspection::Inspectable, public QS::_core_c_::Diagnostics2::IModule
					{
					public:

						Stream(int streamid);

						#pragma region Diagnostics.IModule Members

						virtual property QS::_core_c_::Diagnostics2::IComponent^ DiagnosticsComponent
						{
							QS::_core_c_::Diagnostics2::IComponent^ get() = QS::_core_c_::Diagnostics2::IModule::Component::get
							{
								return diagnosticsContainer; 
							}
						}

						#pragma endregion

						#pragma region Received

						__inline void Received(int seqno, double time)
						{
							nreceived++;

#if defined(DEBUG_LogIndividualReceiveTimes)
							receiveTimes->Add(QS::_core_e_::Data::XY(time, (double) seqno));
#endif

#if defined(DEBUG_LogReceiveRates)
							if (time > lastlogged + 1)
							{
								receiveRates->Add(QS::_core_e_::Data::XY(time, (((double)(nreceived - lastnreceived)) / (time - lastlogged))));
								lastlogged = time;
								lastnreceived = nreceived;
							}
#endif
						}

						#pragma endregion

					private:

						int streamid, nreceived;
						QS::_core_c_::Diagnostics2::Container ^diagnosticsContainer;

#if defined(DEBUG_LogIndividualReceiveTimes)
						List<QS::_core_e_::Data::XY>^ receiveTimes;

						[QS::_core_c_::Diagnostics::Component("Receive Times")]
						[QS::_core_c_::Diagnostics2::Property("ReceiveTimes")]
						property QS::_core_e_::Data::IDataSet^ TimeSeries_ReceiveTimes
						{
							QS::_core_e_::Data::IDataSet^ get()
							{
								return gcnew QS::_core_e_::Data::XYSeries(receiveTimes->ToArray());
							}
						}
#endif

#if defined(DEBUG_LogReceiveRates)
						List<QS::_core_e_::Data::XY>^ receiveRates;
						double lastlogged;
						int lastnreceived;

						[QS::_core_c_::Diagnostics::Component("Receive Rates")]
						[QS::_core_c_::Diagnostics2::Property("ReceiveRates")]
						property QS::_core_e_::Data::IDataSet^ TimeSeries_ReceiveRates
						{
							QS::_core_e_::Data::IDataSet^ get()
							{
								return gcnew QS::_core_e_::Data::XYSeries(receiveRates->ToArray());
							}
						}
#endif
					};

					#pragma endregion

					IPAddress^ sourceAddress;
					QS::_core_c_::Diagnostics2::Container ^diagnosticsContainer, ^diagnosticsContainerForIncomingStreams;

					[QS::_core_c_::Diagnostics::ComponentCollection("Streams")]
					IDictionary<int, Stream^>^ incomingStreams;
				};

				#pragma endregion

				QS::_core_c_::Diagnostics2::Container ^diagnosticsContainerForReceiveStatuses;				

				[QS::_core_c_::Diagnostics::ComponentCollection]
				IDictionary<IPAddress^, ReceiveStatus^>^ receiveStatuses;

				[QS::Fx::Base::Inspectable("Sources")]
				QS::Fx::Inspection::AttributeCollection^ inspectable_receiveStatuses;

#endif

				#pragma endregion

				#pragma region ScheduledAlarmTimes

				[QS::_core_c_::Diagnostics::Component("Schedule Alarm Times")]
				// [QS::_core_c_::Diagnostics2::Property("ScheduledAlarmTimes")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_ScheduledAlarmTimes
				{
					QS::_core_e_::Data::IDataSet^ get()
					{
						System::Collections::Generic::List<double>^ alarmTimes = gcnew System::Collections::Generic::List<double>();
						for each (Alarm^ alarm in alarmQueue)
							alarmTimes->Add(alarm->Time);
						return gcnew QS::_core_e_::Data::DataSeries(alarmTimes->ToArray());
					}
				}

				#pragma endregion

				#pragma region DEBUG_RememberMostRecentAlarms

#if defined(DEBUG_RememberMostRecentAlarms)
				value struct AlarmInfo
				{
				public:

					AlarmInfo(double time, Alarm^ alarm)
					{
						this->Time = time;
						this->Timeout = alarm->Timeout;
						this->Delay = time - alarm->Time;
						this->Callback = alarm->Callback1;
						if (this->Callback == nullptr)
							this->Callback = alarm->Callback2;
						this->Context = alarm->Context;
					}
					
					double Time, Timeout, Delay;
					Delegate^ Callback;
					Object^ Context;

					virtual String^ ToString() override
					{
						return "Time\t=\t" + Time.ToString() + "\nTimeout\t=\t" + Timeout.ToString() + 
							"\nDelay\t=\t" + Delay.ToString() + "\nTarget\t=\t" + 
							Callback->Target->ToString() + " : " + Callback->Target->GetType()->ToString() + "\nMethod\t=\t" + 
							Callback->Method->Name + "\nContext\t=\t" + 
							((Context != nullptr) ? (Context->ToString() + " : " + Context->GetType()->ToString()) : "null");
					}
				};

				[QS::Fx::Base::Inspectable("History of Alarms Recently Fired (last 100 values)")]
				Queue<AlarmInfo>^ historyOfAlarmsRecentlyFired;

				__inline void LogAlarm(double time, Alarm^ alarm)
				{
					historyOfAlarmsRecentlyFired->Enqueue(AlarmInfo(time, alarm));
					while (historyOfAlarmsRecentlyFired->Count > 100)
						historyOfAlarmsRecentlyFired->Dequeue();
				}
#endif

#pragma endregion

				#pragma region DEBUG_CollectStatisticsForAlarms

#if defined(DEBUG_CollectStatisticsForAlarms)

				List<QS::_core_e_::Data::XY>^ timeseries_alarmOverheads;

				[QS::_core_c_::Diagnostics::Component("Alarm Overheads")]
				[QS::_core_c_::Diagnostics2::Property("AlarmOverheads")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_AlarmOverheads
				{
					QS::_core_e_::Data::IDataSet^ get()
					{
						return gcnew QS::_core_e_::Data::XYSeries(timeseries_alarmOverheads->ToArray());
					}
				}

				__inline void AddAlarmStatistic(double time, double overhead)
				{
					timeseries_alarmOverheads->Add(QS::_core_e_::Data::XY(time, overhead));
				}

#endif

				#pragma endregion

				#pragma region DEBUG_RegisterSchedulingHistory

#if defined(DEBUG_RegisterSchedulingHistory)

				enum class ProcessingMode : int
				{
					Alarms, IO
				};

				List<QS::_core_e_::Data::XY>^ timeseries_schedulingHistory;

				[QS::_core_c_::Diagnostics::Component("Scheduling History")]
				[QS::_core_c_::Diagnostics2::Property("SchedulingHistory")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_SchedulingHistory
				{
					QS::_core_e_::Data::IDataSet^ get()
					{
						return gcnew QS::_core_e_::Data::XYSeries(timeseries_schedulingHistory->ToArray());
					}
				}

				__inline void RegisterSchedulerAction(ProcessingMode actionType)
				{
					timeseries_schedulingHistory->Add(QS::_core_e_::Data::XY(time, (double)((int) actionType)));
				}

				[QS::Fx::Base::Inspectable("Core Processing Mode Codes")]
				property QS::Fx::Inspection::AttributeCollection^ CoreProcessingModeCodes
				{
					QS::Fx::Inspection::AttributeCollection^ get() 
					{
						QS::Fx::Inspection::AttributeCollection^ collection = gcnew QS::Fx::Inspection::AttributeCollection("Codes");
						for each (Object^ value in System::Enum::GetValues(ProcessingMode::typeid))
							collection->Add(
								gcnew QS::Fx::Inspection::ScalarAttribute(
									((int) ((ProcessingMode) value)).ToString(), System::Enum::GetName(ProcessingMode::typeid, value)));
						return collection;
					}
				}

#endif

				#pragma endregion

				#pragma region DEBUG_RegisterCoreActions

#if defined(DEBUG_RegisterCoreActions)
				enum class ActionType : int
				{
					Alarm, Callback, Received, SendCompletion, UnknownIO
				};

				[QS::_core_c_::Diagnostics2::Property("CoreActions")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreActions;

//				[QS::_core_c_::Diagnostics::Component("History of Core Actions")]
//				[QS::_core_c_::Diagnostics2::Property("CoreActionHistory")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_HistoryOfCoreActions
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_registerCoreActions->ToArray());
//					}
//				}

				__inline void RegisterAction(ActionType actionType)
				{
					ts_CoreActions->Add(time, (double)((int) actionType));
				}

				[QS::Fx::Base::Inspectable("Core Action Type Codes")]
				property QS::Fx::Inspection::AttributeCollection^ CoreActionTypeCodes
				{
					QS::Fx::Inspection::AttributeCollection^ get() 
					{
						QS::Fx::Inspection::AttributeCollection^ collection = gcnew QS::Fx::Inspection::AttributeCollection("Codes");
						for each (Object^ value in System::Enum::GetValues(ActionType::typeid))
							collection->Add(
								gcnew QS::Fx::Inspection::ScalarAttribute(
									((int) ((ActionType) value)).ToString(), System::Enum::GetName(ActionType::typeid, value)));
						return collection;
					}
				}
#endif

				#pragma endregion

				#pragma region DEBUG_CollectStatisticsForIO

#if defined(DEBUG_CollectStatisticsForIO)
				[QS::_core_c_::Diagnostics2::Property("SendCompletionProcessingOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_SendCompletionTotalProcessingOverheads;

				[QS::_core_c_::Diagnostics2::Property("ReceiveCompletionProcessingOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_ReceiveCompletionTotalProcessingOverheads;

//				[QS::_core_c_::Diagnostics::Component("Send Completion Total Processing Overheads")]
//				[QS::_core_c_::Diagnostics2::Property("SendCompletionProcessingOverheads")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_SendCompletionTotalProcessingOverheads
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_sendCompletionTotalProcessingOverheads->ToArray());
//					}
//				}

//				[QS::_core_c_::Diagnostics::Component("Receive Completion Total Processing Overheads")]
//				[QS::_core_c_::Diagnostics2::Property("ReceiveCompletionProcessingOverheads")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_ReceiveCompletionTotalProcessingOverheads
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_receiveCompletionTotalProcessingOverheads->ToArray());
//					}
//				}
#endif

				#pragma endregion

				#pragma region DEBUG_MeasureCoreConcurrencyStatistics

#if defined(DEBUG_MeasureCoreConcurrencyStatistics)

				[QS::_core_c_::Diagnostics2::Property("CoreConcurrency")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreConcurrency;

				[QS::_core_c_::Diagnostics2::Property("CoreBytesBeingTransmitted")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreBytesBeingTransmitted;

#endif

				#pragma endregion

				#pragma region DEBUG_CollectCoreStatistics

#if defined(DEBUG_CollectCoreStatistics)
				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusTimeouts")]
				QS::_core_c_::Statistics::ISamples2D^ ts_QueryCompletionStatusTimeouts;

				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusMeasuredDelays")]
				QS::_core_c_::Statistics::ISamples2D^ ts_QueryCompletionStatusMeasuredDelays;

				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusTimeoutsVsMeasuredDelays")]
				QS::_core_c_::Statistics::ISamples2D^ ts_QueryCompletionStatusTimeoutsVsMeasuredDelays;

//				[QS::_core_c_::Diagnostics::Component("Query Completion Status Timeouts")]
//				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusTimeouts")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_QueryCompletionStatusTimeouts
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_queryCompletionStatusTimeouts->ToArray());
//					}
//				}

//				[QS::_core_c_::Diagnostics::Component("Query Completion Status Measured Delays")]
//				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusMeasuredDelays")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_QueryCompletionStatusMeasuredDelays
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_queryCompletionStatusMeasuredDelays->ToArray());
//					}
//				}

//				[QS::_core_c_::Diagnostics::Component("Query Completion Status Timeouts vs. Measured Delays")]
//				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusTimeoutsVsMeasuredDelays")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_QueryCompletionStatusTimeoutsVsMeasuredDelays
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_queryCompletionStatusTimeoutsVsMeasuredDelays->ToArray());
//					}
//				}

//				[QS::_core_c_::Diagnostics::Component("Query Completion Status Timeouts and Measured Delays")]
//				[QS::_core_c_::Diagnostics2::Property("QueryCompletionStatusTimeoutsAndMeasuredDelays")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_QueryCompletionStatus
//				{
//					QS::_core_e_::Data::IDataSet^ get() 
//					{
//						QS::_core_e_::Data::DataCo^ multiseries = gcnew QS::_core_e_::Data::DataCo();
//						multiseries->Add(gcnew QS::_core_e_::Data::Data2D(
//							"timeouts", timeseries_queryCompletionStatusTimeouts->ToArray()));
//						multiseries->Add(gcnew QS::_core_e_::Data::Data2D(
//							"measured delays", timeseries_queryCompletionStatusMeasuredDelays->ToArray()));
//						return multiseries;
//					}
//				}
#endif

				#pragma endregion

				#pragma endregion
			};			
		}
	}
}
