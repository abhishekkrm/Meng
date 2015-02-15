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

// #include "ISender.h"
// #include "ISenderController.h"
// #include "AsynchronousSend.h"
// #include "IIOController.h"
#include "Transmitter.h"
// #include "ITransmitterController.h"
// #include "ICore.h"
// #include "ErrorCallback.h"

// #define DEBUG_LogCredits

// ----- THESE PRODUCE HUGE DATA DUMPS -----

// #define DEBUG_LogScheduleWithCoreTimes
// #define DEBUG_LogCoreTransmissionCallbacks
// #define DEBUG_LogIndividualTransmitTimes
// #define DEBUG_LogNumberOfTransmittersAndConcurrency

// ------------------------------------------------------------------

#define DEBUG_LogTransmitRates

// #define DEBUG_LogCredits
#define DEBUG_LogSenderCreation

#define DEBUG_MeasurePeriodicSenderStatistics
#define DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads
#define DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheadsDetailedProfiling
#define DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitCompletionOverheadsDetailedProfiling
#define DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads

// #define OPTION_TransmittersUseSeparateSockets

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			[QS::_core_c_::Diagnostics::ComponentContainerAttribute]
			[QS::Fx::Base::Inspectable]
			public ref class Sender sealed : public QS::Fx::Inspection::Inspectable, public ISender, public ITransmitterController,
				public QS::_core_c_::Diagnostics2::IModule
			{
			public:

				Sender(ISenderController^ senderController, IIOController^ ioController, ICore^ core, QS::Fx::Clock::IClock^ clock, 
					ErrorCallback^ errorCallback, Core::Address^ address, IntPtr completionPort, int mtu, int maximumConcurrency, 
					QS::_core_c_::RateControl::IRateController^ rateController, 
					QS::Fx::Logging::ILogger^ logger, QS::Fx::Logging::IEventLogger^ eventLogger, int streamid,
					QS::_core_c_::Statistics::IStatisticsController^ statisticsController, int sizeOfTheAdfBuffer, bool high_priority);

				~Sender()
				{
				}

				#pragma region Accessors

				virtual property QS::_core_c_::Core::Address^ CombinedAddress 
				{
					QS::_core_c_::Core::Address^ get() = 
						QS::_core_c_::Core::ITransmitterController::CombinedAddress::get,
						QS::_core_c_::Core::ISender::CombinedAddress::get
					{ 
						return address; 
					}
				}

				virtual property System::Net::IPAddress^ InterfaceAddress
				{
					System::Net::IPAddress^ get() = QS::Fx::Network::ISender::InterfaceAddress::get
					{
						return address->NIC;
					}
				}

				virtual property QS::Fx::Network::NetworkAddress^ NetworkAddress
				{
					QS::Fx::Network::NetworkAddress^ get() = QS::Fx::Network::ISender::Address::get
					{
						return gcnew QS::Fx::Network::NetworkAddress(address->IPAddress, address->PortNumber);
					}
				}

				virtual property int MTU
				{
					int get() { return mtu; }
					void set(int value) { this->mtu = value; }
				}

				virtual property int MaximumConcurrency
				{
					int get() { return maximumConcurrency; }
					void set(int value) { this->maximumConcurrency = value; }
				}

//				virtual property double MaximumRate
//				{
//					double get() { return flowController->maximumRate; }
//					void set(double value) { flowController->maximumRate = value; }
//				}

//				virtual property double MaximumCredits
//				{
//					double get() { return maximumCredits; }
//					void set(double value) 
//					{ 
//						this->maximumCredits = value; 
//						AdjustParameters();
//					}
//				}

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::SenderInfo::Parameters::RateController)]
				virtual property QS::_core_c_::RateControl::IRateController^ RateController
				{
					QS::_core_c_::RateControl::IRateController^ get()
					{
						return rateController;
					}

					void set(QS::_core_c_::RateControl::IRateController^ rateController)
					{
						this->rateController = rateController;

						rateController->OnReady += gcnew QS::Fx::Base::Callback(this, &QS::_core_c_::Core::Sender::RateCallback);
						if (rateController->Ready)
							RateCallback();

						QS::_core_c_::Diagnostics2::IModule^ module = dynamic_cast<QS::_core_c_::Diagnostics2::IModule^>(rateController);
						if (module) 
							((QS::_core_c_::Diagnostics2::IContainer^) diagnosticsContainer)->Register("RateController", module->Component, 
								QS::_core_c_::Diagnostics2::RegisteringMode::Override | QS::_core_c_::Diagnostics2::RegisteringMode::Reregister);
					}
				}

				#pragma endregion

				#pragma region QS.Fx.Base.IParametrized Members

				virtual property QS::Fx::Base::IParameters^ Parameters
				{
					QS::Fx::Base::IParameters^ get() = QS::Fx::Base::IParametrized::Parameters::get
					{
						return this->parameters;
					}
				}
				
				#pragma endregion

				#pragma region Sending

				virtual void Send(QS::Fx::Network::Data data)
				{
					requests->Enqueue(QS::Fx::Network::AsynchronousSend(data, nullptr, nullptr));
					Signal();
				}

				virtual void Send(QS::Fx::Network::Data data, QS::Fx::Base::ContextCallback^ completionCallback, Object^ context)
				{
					requests->Enqueue(QS::Fx::Network::AsynchronousSend(data, completionCallback, context));
					Signal();
				}

				virtual void Send(QS::Fx::Network::ISource^ source)
				{
					sources->Enqueue(source);
					Signal();
				}

				#pragma endregion

				void AllowTransmission(int maximumNumberOfPackets, int maximumNumberOfBytes, 
					int& numberOfPacketsConsumed, int& numberOfBytesConsumed, bool& moreAvailable);

				virtual void RecycleTransmitter(ITransmitter^ transmitter, int numberOfBytes) = 
					QS::_core_c_::Core::ITransmitterController::RecycleTransmitter;

				#pragma region Logging Overheads from Transmitters

				virtual void LogTransmitOverheads(ITransmitter^ transmitter, double time, double overhead1, double overhead2, double overhead3, 
					double overhead4) = QS::_core_c_::Core::ITransmitterController::LogTransmitOverheads
				{
#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheadsDetailedProfiling)
					ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead1->Add(time, overhead1);
					ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead2->Add(time, overhead2);
					ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead3->Add(time, overhead3);
					ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead4->Add(time, overhead4);
#endif
				}

				virtual void LogTransmitCompletionOverheads(ITransmitter^ transmitter, double time, double overhead1, double overhead2, 
					double overhead3) = QS::_core_c_::Core::ITransmitterController::LogTransmitCompletionOverheads
				{
#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitCompletionOverheadsDetailedProfiling)
					ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead1->Add(time, overhead1);
					ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead2->Add(time, overhead2);
					ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead3->Add(time, overhead3);
#endif
				}

				#pragma endregion

				#pragma region FlowControl3.IRateControlled Members

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::SenderInfo::Parameters::MaximumRate)]
				virtual property double MaximumRate // = QS::_core_c_::FlowControl3::IRateControlled::MaximumRate
				{
					double get() { return rateController->MaximumRate; }
					void set(double rate) { rateController->MaximumRate = rate; }
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

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::SenderInfo::Parameters::AdfBufferSize)]
				property int AdfBufferSize
				{
					int get() { return sizeOfTheAdfBuffer; }

					void set(int n)
					{
						if (n > 0)
						{
							shared_socket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::SendBuffer, n);
							sizeOfTheAdfBuffer = n;
						}
					}
				}

			private:
				
				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::SenderInfo::Parameters::HighPriority)]
				bool high_priority;

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::SenderInfo::Parameters::MaximumConcurrency)]
				int maximumConcurrency;

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::SenderInfo::Parameters::MTU)]
				int mtu;

#if !defined(OPTION_TransmittersUseSeparateSockets)
				Socket^ shared_socket;
#endif

				int streamid, ntransmitted, sizeOfTheAdfBuffer;
				QS::_core_c_::Statistics::IStatisticsController^ statisticsController;
				QS::_core_x_::Base::Parameters^ parameters;

				#pragma region DEBUG_LogScheduleWithCoreTimes

#if defined(DEBUG_LogScheduleWithCoreTimes)
				enum class ScheduleCoreReason : int
				{
					Signaling, RateCallback, Recycling
				};

				[QS::_core_c_::Diagnostics2::Property("ScheduleWithCoreTimes")]
				QS::_core_c_::Statistics::ISamples2D^ ts_ScheduleWithCoreTimes;

//				[QS::_core_c_::Diagnostics::Component("Schedule With Core Times")]
//				[QS::_core_c_::Diagnostics2::Property("ScheduleWithCoreTimes")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_ScheduleWithCoreTimes
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_scheduleWithCoreTimes->ToArray());
//					}
//				}
#endif

				#pragma endregion

				#pragma region DEBUG_LogCoreTransmissionCallbacks

#if defined(DEBUG_LogCoreTransmissionCallbacks)

				[QS::_core_c_::Diagnostics2::Property("CoreTransmissionCallbackAllowedPackets")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreTransmissionCallbackAllowedPackets;

				[QS::_core_c_::Diagnostics2::Property("CoreTransmissionCallbackReturnedPackets")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreTransmissionCallbackReturnedPackets;

//				[QS::_core_c_::Diagnostics::Component("Core Transmission Callback Packets")]
//				[QS::_core_c_::Diagnostics2::Property("CoreTransmissionCallbackPackets")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_CoreTransmissionCallbackPackets
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_coreTransmissionCallbackPackets->ToArray());
//					}
//				}

				[QS::_core_c_::Diagnostics2::Property("CoreTransmissionCallbackAllowedBytes")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreTransmissionCallbackAllowedBytes;

				[QS::_core_c_::Diagnostics2::Property("CoreTransmissionCallbackReturnedBytes")]
				QS::_core_c_::Statistics::ISamples2D^ ts_CoreTransmissionCallbackReturnedBytes;

//				[QS::_core_c_::Diagnostics::Component("Core Transmission Callback Bytes")]
//				[QS::_core_c_::Diagnostics2::Property("CoreTransmissionCallbackBytes")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_CoreTransmissionCallbackBytes
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_coreTransmissionCallbackBytes->ToArray());
//					}
//				}
#endif

				#pragma endregion

				#pragma region DEBUG_LogIndividualTransmitTimes

#if defined(DEBUG_LogIndividualTransmitTimes)
				[QS::_core_c_::Diagnostics2::Property("TransmitTimes")]
				QS::_core_c_::Statistics::ISamples1D^ ts_TransmitTimes;

//				[QS::_core_c_::Diagnostics::Component("Transmit Times")]
//				[QS::_core_c_::Diagnostics2::Property("TransmitTimes")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_IndividualTransmitTimes
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::DataSeries(timeSeries_individualTransmitTimes->ToArray());
//					}
//				}
#endif

				#pragma endregion

				#pragma region DEBUG_LogTransmitRates

#if defined(DEBUG_LogTransmitRates)
				double lastlogged;
				int lastntransmitted;
				List<QS::_core_e_::Data::XY>^ timeSeries_transmitRates;

				[QS::_core_c_::Diagnostics::Component("Transmit Rates")]
				[QS::_core_c_::Diagnostics2::Property("TransmitRates")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_TransmitRates
				{
					QS::_core_e_::Data::IDataSet^ get()
					{
						return gcnew QS::_core_e_::Data::XYSeries(timeSeries_transmitRates->ToArray());
					}
				}

				__inline void _MayLogTxRate()
				{
					double now = clock->Time;
					if (now > lastlogged + 1)
					{
						timeSeries_transmitRates->Add(QS::_core_e_::Data::XY(now, ((double)(ntransmitted - lastntransmitted)) / (now - lastlogged)));
						lastlogged = now;
						lastntransmitted = ntransmitted;
					}
				}

#endif

				#pragma endregion

				#pragma region DEBUG_LogNumberOfTransmittersAndConcurrency

#if defined(DEBUG_LogNumberOfTransmittersAndConcurrency)
				[QS::_core_c_::Diagnostics2::Property("NumberOfTransmitters")]
				QS::_core_c_::Statistics::ISamples2D^ ts_NumberOfTransmitters;

				[QS::_core_c_::Diagnostics2::Property("Concurrency")]
				QS::_core_c_::Statistics::ISamples2D^ ts_Concurrency;
#endif

				#pragma endregion

				#pragma region DEBUG_MeasurePeriodicSenderStatistics

#if defined(DEBUG_MeasurePeriodicSenderStatistics)

				double pstat_lastmeasured;

				#pragma region DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads

#if defined (DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)
				double pstat_tx_overheads_cumulated;
				int pstat_tx_overheads_nsamples;
				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitOverheads;
#endif

				#pragma endregion

				#pragma region DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheadsDetailedProfiling

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheadsDetailedProfiling)

				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitOverheadsDetailedProfiling_Overhead1")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead1;
				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitOverheadsDetailedProfiling_Overhead2")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead2;
				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitOverheadsDetailedProfiling_Overhead3")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead3;
				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitOverheadsDetailedProfiling_Overhead4")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead4;

#endif

				#pragma endregion

				#pragma region DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitCompletionOverheadsDetailedProfiling

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitCompletionOverheadsDetailedProfiling)

				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead1")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead1;
				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead2")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead2;
				[QS::_core_c_::Diagnostics2::Property("TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead3")]
				QS::_core_c_::Statistics::ISamples2D^ ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead3;

#endif

				#pragma endregion

				#pragma region DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads)

				double min_source_get_overhead, max_source_get_overhead, sum_source_get_overheads;
				int num_source_get_overheads;

				[QS::_core_c_::Diagnostics2::Property("SourceGetMinimumOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_SourceGetMinimumOverheads;
				[QS::_core_c_::Diagnostics2::Property("SourceGetAverageOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_SourceGetAverageOverheads;
				[QS::_core_c_::Diagnostics2::Property("SourceGetMaximumOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_SourceGetMaximumOverheads;

#endif

				#pragma endregion

				__inline void _MeasurePeriodicStatistics(double time_now)
				{
					double elapsed_time = time_now - pstat_lastmeasured;

					if (elapsed_time > 1)
					{
						double sample_time = pstat_lastmeasured;

#if defined (DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)
						ts_TransmitterTransmitOverheads->Add(sample_time, pstat_tx_overheads_cumulated / pstat_tx_overheads_nsamples);
						pstat_tx_overheads_cumulated = 0;
						pstat_tx_overheads_nsamples = 0;
#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads)
						ts_SourceGetMinimumOverheads->Add(sample_time, min_source_get_overhead);
						ts_SourceGetAverageOverheads->Add(sample_time, sum_source_get_overheads / ((double) num_source_get_overheads));
						ts_SourceGetMaximumOverheads->Add(sample_time, max_source_get_overhead);
						min_source_get_overhead = max_source_get_overhead = sum_source_get_overheads = 0;
						num_source_get_overheads = 0;
#endif

						pstat_lastmeasured = time_now;
					}
				}

				__inline void _MeasurePeriodicStatistics()
				{
					_MeasurePeriodicStatistics(clock->Time);
				}

#endif

				#pragma endregion

				QS::_core_c_::Diagnostics2::Container^ diagnosticsContainer;
				QS::Fx::Logging::ILogger^ logger;
				QS::Fx::Logging::IEventLogger^ eventLogger;
				IIOController^ ioController;
				ICore^ core;
				QS::Fx::Clock::IClock^ clock;
				ISenderController^ senderController;
				QS::_core_c_::Core::Address^ address;
				IntPtr completionPort;
				int concurrency;
				
				[QS::Fx::Base::Inspectable]
				Queue<Transmitter^>^ transmitters;
				[QS::Fx::Base::Inspectable]
				Queue<QS::Fx::Network::AsynchronousSend>^ requests;			
				[QS::Fx::Base::Inspectable]
				Queue<QS::Fx::Network::ISource^>^ sources;			

				[QS::Fx::Base::Inspectable]
				property int NumberOfQueuedRequests
				{
					int get() { return requests->Count; }
				}

				[QS::Fx::Base::Inspectable]
				property int NumberOfQueuedSources
				{
					int get() { return sources->Count; }
				}

				bool dataWaiting, registeredWithCore;
				ErrorCallback^ errorCallback;

				[QS::_core_c_::Diagnostics::Component("Rate Controller")]
				QS::_core_c_::RateControl::IRateController^ rateController;

				Socket^ CreateSocket();

				void Signal();
				void AllocateTransmitter();
				void RateCallback();

//				This has been moved to rate controller...
//				
//				double credits, lastChecked, lowCredits, highCredits, maximumCredits, maximumRate;
//				bool waitingForCredits;
//				IAlarm^ rateAlarm;
//				AlarmCallback^ rateAlarmCallback;
//				void RateAlarmCallback(IAlarm^ alarm);
//				void ConsumeCredit();
//				void RegenerateCredits();
//				void AdjustParameters();
// #if defined(DEBUG_LogCredits)
//				[QS::Fx::Base::Inspectable]
//				QS::_core_c_::Statistics::SamplesXY^ timeseries_credits;
// #endif
			};
		}
	}
}
