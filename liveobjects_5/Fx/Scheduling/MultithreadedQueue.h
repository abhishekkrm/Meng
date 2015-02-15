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

#define PROFILE_CountEnqueuesAndDequeues
#define PROFILE_SavePerformanceStatistics
#define PROFILE_SavePerformanceStatistics_Detailed
#define PROFILE_DIFFS
#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace System::Threading;

namespace QS
{
	namespace Fx
	{
		namespace Scheduling
		{
			public ref class MultithreadedQueue sealed : public QS::Fx::Inspection::Inspectable, QS::Fx::Scheduling::IQueue
			{
			public:

				MultithreadedQueue(int concurrency, Policy policy, bool pinthreads);
				~MultithreadedQueue();

				virtual void Enqueue(QS::Fx::Base::IEvent^ e) = QS::Fx::Scheduling::IQueue::Enqueue;
				virtual QS::Fx::Base::IEvent^ Dequeue() = QS::Fx::Scheduling::IQueue::Dequeue;				
				virtual property bool Blocked { bool get() = QS::Fx::Scheduling::IQueue::Blocked::get { return this->blocked; } }

			private:

				ref class Queue sealed : public QS::Fx::Inspection::Inspectable
				{
				public:
					
					[QS::Fx::Base::Inspectable] int id;
					[QS::Fx::Base::Inspectable] int count;
					[QS::Fx::Base::Inspectable] int operating;
					[QS::Fx::Base::Inspectable] bool automatic;
					[QS::Fx::Base::Inspectable] bool finished;
					[QS::Fx::Base::Inspectable] bool deposited;
					[QS::Fx::Base::Inspectable] Thread^ thread;
					[QS::Fx::Base::Inspectable] AutoResetEvent^ check;
					[QS::Fx::Base::Inspectable] QS::Fx::Base::IEvent ^root;
					[QS::Fx::Base::Inspectable] QS::Fx::Base::IEvent ^from;
					[QS::Fx::Base::Inspectable] QS::Fx::Base::IEvent ^to;

#if defined(PROFILE_DIFFS)
					[QS::Fx::Base::Inspectable] double _last_handle;
					List<QS::_core_e_::Data::XY>^ statistics_diffs;
					[QS::Fx::Base::Inspectable] 
					property QS::_core_e_::Data::IData^ Statistics_Diffs
					{
						QS::_core_e_::Data::IData^ get() 
						{ 
							return gcnew QS::_core_e_::Data::Data2D("diffs on core " + id.ToString(), statistics_diffs->ToArray()); 
						}
					}
#endif

#if defined(PROFILE_CountEnqueuesAndDequeues)
					[QS::Fx::Base::Inspectable] int enqueued;
					[QS::Fx::Base::Inspectable] int dequeued;
#if defined(PROFILE_SavePerformanceStatistics)
					List<QS::_core_e_::Data::XY>^ statistics_dequeued;
					[QS::Fx::Base::Inspectable] 
					property QS::_core_e_::Data::IData^ Statistics_Dequeued
					{
						QS::_core_e_::Data::IData^ get() 
						{ 
							return gcnew QS::_core_e_::Data::Data2D("dequeued on core " + id.ToString(), statistics_dequeued->ToArray()); 
						}
					}
#if defined(PROFILE_SavePerformanceStatistics_Detailed)
					double id2;
					List<QS::_core_e_::Data::XY>^ statistics_busy;
					[QS::Fx::Base::Inspectable] 
					property QS::_core_e_::Data::IData^ Statistics_Busy
					{
						QS::_core_e_::Data::IData^ get() 
						{ 
							return gcnew QS::_core_e_::Data::Data2D("busy on core " + id.ToString(), statistics_busy->ToArray()); 
						}
					}
#endif
#endif
#endif
				};

				[QS::Fx::Base::Inspectable] QS::Fx::Clock::IClock ^clock;
				[QS::Fx::Base::Inspectable] int concurrency;
				[QS::Fx::Base::Inspectable] Policy policy;
				[QS::Fx::Base::Inspectable] int finished;
				[QS::Fx::Base::Inspectable] int selected;
				[QS::Fx::Base::Inspectable] cli::array<Queue^>^ queues;
				[QS::Fx::Base::Inspectable] bool blocked;
				[QS::Fx::Base::Inspectable] bool finishing;
				[QS::Fx::Base::Inspectable] bool pinthreads;

#if defined(PROFILE_SavePerformanceStatistics)
				[QS::Fx::Base::Inspectable] 
				property QS::_core_e_::Data::IData^ Statistics_Dequeued
				{
					QS::_core_e_::Data::IData^ get() 
					{ 
						QS::_core_e_::Data::DataCo^ statistics = 
							gcnew QS::_core_e_::Data::DataCo(
								"dequeued across cores", "dequeue progress across cores", "time", "s", "time in seconds", "dequeued", "events", "events dequeued");
						for each (Queue^ queue in this->queues)
							statistics->Add(queue->Statistics_Dequeued);
						return statistics;
					}
				}
#if defined(PROFILE_SavePerformanceStatistics_Detailed)
				[QS::Fx::Base::Inspectable] 
				property QS::_core_e_::Data::IData^ Statistics_Busy
				{
					QS::_core_e_::Data::IData^ get() 
					{ 
						QS::_core_e_::Data::DataCo^ statistics = 
							gcnew QS::_core_e_::Data::DataCo(
								"busy across cores", "busy across cores", "time", "s", "time in seconds", "busy", ">0?", "0 = free, >0 = busy");
						for each (Queue^ queue in this->queues)
							statistics->Add(queue->Statistics_Busy);
						return statistics;
					}
				}		
#endif
#endif

				System::Random^ random;

				int Balance();
				void ThreadCallback(Object^ o);
				QS::Fx::Base::IEvent^ Dequeue(Queue^ queue);
			};
		}
	}
}
