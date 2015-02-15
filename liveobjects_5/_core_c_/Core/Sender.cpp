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
#include "Sender.h"
// #include "Sockets.h"
#include "Overlapped.h"

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
			#pragma region Constructor

			Sender::Sender(ISenderController^ senderController, IIOController^ ioController, ICore^ core, QS::Fx::Clock::IClock^ clock,
				ErrorCallback^ errorCallback, Core::Address^ address, IntPtr completionPort, int mtu, int maximumConcurrency, 
				QS::_core_c_::RateControl::IRateController^ rateController, 
				QS::Fx::Logging::ILogger^ logger, QS::Fx::Logging::IEventLogger^ eventLogger, int streamid,
				QS::_core_c_::Statistics::IStatisticsController^ statisticsController, int sizeOfTheAdfBuffer, bool high_priority)
			{
				this->parameters = gcnew QS::_core_x_::Base::Parameters();

				diagnosticsContainer = gcnew QS::_core_c_::Diagnostics2::Container();

				this->high_priority = high_priority;
				this->sizeOfTheAdfBuffer = sizeOfTheAdfBuffer;
				this->streamid = streamid;
				this->logger = logger;
				this->eventLogger = eventLogger;
				this->core = core;
				this->clock = clock;
				this->senderController = senderController;
				this->ioController = ioController;
				this->address = address;
				this->completionPort = completionPort;
				this->mtu = mtu;
				this->maximumConcurrency = maximumConcurrency;
				this->errorCallback = errorCallback;
				this->rateController = rateController;
				this->statisticsController = statisticsController;

				concurrency = 0;
				transmitters = gcnew Queue<Transmitter^>();
				sources = gcnew Queue<QS::Fx::Network::ISource^>();
				requests = gcnew Queue<QS::Fx::Network::AsynchronousSend>();
				registeredWithCore = false;
				dataWaiting = false;
				rateController->OnReady += gcnew QS::Fx::Base::Callback(this, &QS::_core_c_::Core::Sender::RateCallback);

#if !defined(OPTION_TransmittersUseSeparateSockets)
				shared_socket = this->CreateSocket();
#endif

				QS::_core_c_::Diagnostics2::IModule^ module = dynamic_cast<QS::_core_c_::Diagnostics2::IModule^>(rateController);
				if (module) 
					((QS::_core_c_::Diagnostics2::IContainer^) diagnosticsContainer)->Register("RateController", module->Component);

				#pragma region Initializing Tracing Structures

#if defined(DEBUG_LogSenderCreation)
				logger->Log(this, "Created sender for address { " + address->ToString() + 
					" } with maximum rate { " + rateController->MaximumRate.ToString() + " }, concurrency { " + maximumConcurrency.ToString() + 
					" } and with adbf buffers of { " + sizeOfTheAdfBuffer.ToString() + " } bytes.");
#endif

#if defined(DEBUG_LogScheduleWithCoreTimes)
				ts_ScheduleWithCoreTimes = statisticsController->Allocate2D("schedule with core times", "", "time", "s", "", "reason", "", "");
#endif

#if defined(DEBUG_LogCoreTransmissionCallbacks)
				ts_CoreTransmissionCallbackAllowedPackets = statisticsController->Allocate2D("tx callback allowed packets", "", "time", "s", "", "packets", "", "");
				ts_CoreTransmissionCallbackAllowedBytes = statisticsController->Allocate2D("tx callback allowed bytes", "", "time", "s", "", "bytes", "", "");
				ts_CoreTransmissionCallbackReturnedPackets = statisticsController->Allocate2D("tx callback returned packets", "", "time", "s", "", "packets", "", "");
				ts_CoreTransmissionCallbackReturnedBytes = statisticsController->Allocate2D("tx callback returned bytes", "", "time", "s", "", "bytes", "", "");
#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheadsDetailedProfiling)

				ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead1 = statisticsController->Allocate2D("tx_overhead1", "", "", "", "", "", "", "");
				ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead2 = statisticsController->Allocate2D("tx_overhead2", "", "", "", "", "", "", "");
				ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead3 = statisticsController->Allocate2D("tx_overhead3", "", "", "", "", "", "", "");
				ts_TransmitterTransmitOverheadsDetailedProfiling_Overhead4 = statisticsController->Allocate2D("tx_overhead4", "", "", "", "", "", "", "");

#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitCompletionOverheadsDetailedProfiling)

				ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead1 = statisticsController->Allocate2D("tx_completion_overhead1", "", "", "", "", "", "", "");
				ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead2 = statisticsController->Allocate2D("tx_completion_overhead2", "", "", "", "", "", "", "");
				ts_TransmitterTransmitCompletionOverheadsDetailedProfiling_Overhead3 = statisticsController->Allocate2D("tx_completion_overhead3", "", "", "", "", "", "", "");

#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads)

				ts_SourceGetMinimumOverheads = statisticsController->Allocate2D("min source get overheads", "", "time", "s", "", "overhead", "s", "");
				ts_SourceGetAverageOverheads = statisticsController->Allocate2D("avg source get overheads", "", "time", "s", "", "overhead", "s", "");
				ts_SourceGetMaximumOverheads = statisticsController->Allocate2D("max source get overheads", "", "time", "s", "", "overhead", "s", "");

#endif

#if defined(DEBUG_LogIndividualTransmitTimes)
				ts_TransmitTimes = statisticsController->Allocate1D("transmit times", "", "packet number", "", "", "time", "s", ""); 
				// timeSeries_individualTransmitTimes = gcnew List<double>();
#endif 

#if defined(DEBUG_LogTransmitRates)
				lastlogged = clock->Time;
				lastntransmitted = 0;
				timeSeries_transmitRates = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(DEBUG_LogNumberOfTransmittersAndConcurrency)
				ts_NumberOfTransmitters = statisticsController->Allocate2D("number of transmitters", "", "time", "s", "", "num transmitters", "", "");
				ts_Concurrency= statisticsController->Allocate2D("concurrency", "", "time", "s", "", "concurrency", "", "");
#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics)

				pstat_lastmeasured = clock->Time;

#if defined (DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)				
				ts_TransmitterTransmitOverheads = statisticsController->Allocate2D(
					"transmit overheads", "", "time", "s", "beginning of the time interval", "average transmitter overhead", "s", "");
#endif

#endif

				#pragma endregion

				this->parameters->RegisterLocal(this);
				QS::_core_c_::Diagnostics2::Helper::RegisterLocal(diagnosticsContainer, this);
			}

//			void Sender::AdjustParameters()
//			{
//				lowCredits = 0; // maximumCredits / 3;
//				highCredits = maximumCredits / 2; // 2 * maximumCredits / 3;
//			}

			#pragma endregion

			#pragma region Signaling

			void Sender::Signal()
			{
				if (!dataWaiting)
				{
					dataWaiting = true;

					if (!registeredWithCore && rateController->Ready && // (credits > 0) && 
						(transmitters->Count > 0 || concurrency < maximumConcurrency))
					{					
						registeredWithCore = true;
						senderController->ScheduleSender(this);

#if defined(DEBUG_LogScheduleWithCoreTimes)
						ts_ScheduleWithCoreTimes->Add(clock->Time, (double)(int) ScheduleCoreReason::Signaling);
#endif
					}
				}
			}

			#pragma endregion

			#pragma region GetSocket

			Socket^ Sender::CreateSocket()
			{
				Socket^ socket = QS::_core_c_::Core::Sockets::CreateSendSocket(address, sizeOfTheAdfBuffer);
				if (CreateIoCompletionPort(
					(HANDLE) socket->Handle, (HANDLE) completionPort, 0, 0) != (HANDLE) completionPort)
					throw gcnew Exception("Could not associate completion port with the socket.");
				return socket;
			}

			#pragma endregion

			#pragma region Managing Transmitters

			void Sender::AllocateTransmitter()
			{
				Socket^ socket;
#if defined(OPTION_TransmittersUseSeparateSockets)
				socket = this->CreateSocket();
#else
				socket = this->shared_socket;
#endif

				__int32 reference = ioController->AllocateReference;
				MyOverlapped* overlapped = new MyOverlapped(MyOverlapped::Send, reference, -1);		

				Transmitter^ transmitter = gcnew Transmitter(this, logger, clock, socket, (void *) overlapped, errorCallback, this->streamid, this->high_priority);

				ioController->RegisterReference(reference, transmitter);
				transmitters->Enqueue(transmitter);
				concurrency++;

#if defined(DEBUG_LogNumberOfTransmittersAndConcurrency)
				double time = clock->Time;
				ts_NumberOfTransmitters->Add(time, transmitters->Count);
				ts_Concurrency->Add(time, concurrency); 
#endif

			}

			void Sender::RecycleTransmitter(ITransmitter^ toRecycle, int numberOfBytes)
			{
				Transmitter^ transmitter = dynamic_cast<Transmitter^>(toRecycle);
				if (transmitter == nullptr)
					throw gcnew Exception("Internal error: The recycled object is not a transmitter.");

				transmitters->Enqueue(transmitter);
				senderController->ReleaseResources(1, numberOfBytes);

				if (dataWaiting && !registeredWithCore && rateController->Ready && // (credits > 0) && 
					(transmitters->Count > 0 || concurrency < maximumConcurrency))
				{
					registeredWithCore = true;
					senderController->ScheduleSender(this);

#if defined(DEBUG_LogScheduleWithCoreTimes)
					ts_ScheduleWithCoreTimes->Add(clock->Time, (double)(int) ScheduleCoreReason::Recycling);
#endif
				}

#if defined(DEBUG_LogNumberOfTransmittersAndConcurrency)
				double time = clock->Time;
				ts_NumberOfTransmitters->Add(time, transmitters->Count);
				ts_Concurrency->Add(time, concurrency); 
#endif
			}

			#pragma endregion

			#pragma region Rate Alarm Callback

//			void Sender::RateAlarmCallback(IAlarm^ alarm)
//			{
//				RegenerateCredits();
//				if (credits > 0)
//				{
//					rateAlarm = nullptr;
//					waitingForCredits = false;
//
//					if (dataWaiting && !registeredWithCore && (transmitters->Count > 0 || concurrency < maximumConcurrency))
//					{
//						registeredWithCore = true;
//						senderController->ScheduleSender(this);
//					}
//				}
//				else
//					alarm->Reschedule((highCredits - credits) / maximumRate);
//			}
//
//			#pragma endregion

			void Sender::RateCallback()
			{
				if (dataWaiting && !registeredWithCore && (transmitters->Count > 0 || concurrency < maximumConcurrency))
				{
					registeredWithCore = true;
					senderController->ScheduleSender(this);

#if defined(DEBUG_LogScheduleWithCoreTimes)
					ts_ScheduleWithCoreTimes->Add(clock->Time, (double)(int) ScheduleCoreReason::RateCallback);
#endif
				}
			}

//			#pragma region Managing flow control credits
//
//			void Sender::ConsumeCredit()
//			{
//				if (!double::IsPositiveInfinity(maximumRate))
//				{
//					credits--;
//
// #if defined(DEBUG_LogCredits)
//					if (timeseries_credits->Enabled)
//						timeseries_credits->addSample(core->Time, credits);
// #endif
//
//					if (credits < lowCredits && !waitingForCredits)
//					{
//						waitingForCredits = true;
//						rateAlarm = core->Schedule((highCredits - credits) / maximumRate, rateAlarmCallback, nullptr); 
//					}
//				}
//			}
//
//			void Sender::RegenerateCredits()
//			{
//				double now = core->Time;
//
//				if (double::IsPositiveInfinity(maximumRate))
//					credits = maximumCredits;
//				else
//				{
//					credits += (now - lastChecked) * maximumRate;
//					if (credits > maximumCredits)
//						credits = maximumCredits;
//				}
//				lastChecked = now;
//
// #if defined(DEBUG_LogCredits)
//				if (timeseries_credits->Enabled)
//					timeseries_credits->addSample(now, credits);
// #endif
//			}

			#pragma endregion

			#pragma region Upcall from controller: AllowTransmission

			void Sender::AllowTransmission(int maximumNumberOfPackets, int maximumNumberOfBytes, 
				int& numberOfPacketsConsumed, int& numberOfBytesConsumed, bool& moreAvailable)
			{
//				RegenerateCredits();

				numberOfPacketsConsumed = 0;
				numberOfBytesConsumed = 0;
				moreAvailable = true;

				while (numberOfPacketsConsumed < maximumNumberOfPackets && 
					numberOfBytesConsumed < maximumNumberOfBytes)
				{					
					int maximumBytes = maximumNumberOfBytes - numberOfBytesConsumed;
					if (mtu < maximumBytes)
						maximumBytes = mtu;

					dataWaiting = requests->Count > 0 || sources->Count > 0;

					if (rateController->Ready && (transmitters->Count > 0 || concurrency < maximumConcurrency) && dataWaiting)
					{
						if (transmitters->Count == 0)
							AllocateTransmitter();

						if (requests->Count > 0)
						{
							if (requests->Peek().Data.Size <= maximumBytes)
							{
								QS::Fx::Network::AsynchronousSend request = requests->Dequeue();
								Transmitter^ transmitter = transmitters->Dequeue();
								
								numberOfPacketsConsumed++;
								numberOfBytesConsumed += request.Data.Size;

#if defined(DEBUG_LogIndividualTransmitTimes)
								ts_TransmitTimes->Add(clock->Time);
								// timeSeries_individualTransmitTimes->Add(clock->Time);
#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics) && defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)
								double t1 = clock->Time;
#endif

								transmitter->Transmit(request, ntransmitted++);

#if defined(DEBUG_MeasurePeriodicSenderStatistics) && defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)
								double t2 = clock->Time;
								pstat_tx_overheads_cumulated += t2 - t1;
								pstat_tx_overheads_nsamples++;
								_MeasurePeriodicStatistics(t2);
#endif

#if defined(DEBUG_LogTransmitRates)
								_MayLogTxRate();
#endif

//								ConsumeCredit();
								rateController->Consume();
							}
							else
								break;
						}
						else
						{
							QS::Fx::Network::ISource^ source = sources->Dequeue();

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads)
							double tm1 = clock->Time;
#endif

							QS::Fx::Network::Data data;
							QS::Fx::Base::ContextCallback^ callback;
							Object^ context;
							bool more;
							bool returned = source->Get(maximumBytes, data, callback, context, more);

#if defined(DEBUG_MeasurePeriodicSenderStatistics_LogSourceGetOverheads)
							double tm2 = clock->Time;
							double source_get_overhead = tm2 - tm1;
							
							if (source_get_overhead < min_source_get_overhead)
								min_source_get_overhead = source_get_overhead;
							if (source_get_overhead > max_source_get_overhead)
								max_source_get_overhead = source_get_overhead;
							sum_source_get_overheads += source_get_overhead;
							num_source_get_overheads++;
#endif

							if (more)
							{
								sources->Enqueue(source);
								if (!returned)
									break;
							}

							if (returned)
							{
								Transmitter^ transmitter = transmitters->Dequeue();

								numberOfPacketsConsumed++;
								numberOfBytesConsumed += data.Size;

#if defined(DEBUG_LogIndividualTransmitTimes)
								ts_TransmitTimes->Add(clock->Time);
								// timeSeries_individualTransmitTimes->Add(clock->Time);
#endif

#if defined(DEBUG_MeasurePeriodicSenderStatistics) && defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)
								double t1 = clock->Time;
#endif

								transmitter->Transmit(QS::Fx::Network::AsynchronousSend(data, callback, context), ntransmitted++);

#if defined(DEBUG_MeasurePeriodicSenderStatistics) && defined(DEBUG_MeasurePeriodicSenderStatistics_LogTransmitterTransmitOverheads)
								double t2 = clock->Time;
								pstat_tx_overheads_cumulated += t2 - t1;
								pstat_tx_overheads_nsamples++;
								_MeasurePeriodicStatistics(t2);
#endif

#if defined(DEBUG_LogTransmitRates)
								_MayLogTxRate();
#endif

								// ConsumeCredit();
								rateController->Consume();
							}
						}
					}
					else
					{
						moreAvailable = false;
						registeredWithCore = false;
						break;
					}
				}

#if defined(DEBUG_LogCoreTransmissionCallbacks)
				double time1 = clock->Time;
				ts_CoreTransmissionCallbackAllowedPackets->Add(time1, maximumNumberOfPackets);
				ts_CoreTransmissionCallbackAllowedBytes->Add(time1, maximumNumberOfBytes);
				ts_CoreTransmissionCallbackReturnedPackets->Add(time1, numberOfPacketsConsumed);
				ts_CoreTransmissionCallbackReturnedBytes->Add(time1, numberOfBytesConsumed);
#endif

#if defined(DEBUG_LogNumberOfTransmittersAndConcurrency)
				double time2 = clock->Time;
				ts_NumberOfTransmitters->Add(time2, transmitters->Count);
				ts_Concurrency->Add(time2, concurrency); 
#endif
			}

			#pragma endregion
		}
	}
}
