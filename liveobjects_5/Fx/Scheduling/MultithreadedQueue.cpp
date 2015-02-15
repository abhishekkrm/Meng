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

//#define PROFILE_CountEnqueuesAndDequeues
//#define PROFILE_SavePerformanceStatistics
// #define PROFILE_SavePerformanceStatistics_Detailed


#include "stdafx.h"
#include "MultithreadedQueue.h"
#include "..\..\_core_c_\Core\Clock.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;
using namespace System::Threading;
using namespace System::Diagnostics;

namespace QS
{
	namespace Fx
	{
		namespace Scheduling
		{
			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Constructor

			MultithreadedQueue::MultithreadedQueue(int concurrency, Policy policy, bool pinthreads)
			{
				this->clock = QS::_core_c_::Core::Clock::SharedClock;
				this->concurrency = concurrency;
				this->policy = policy;
				this->queues = gcnew cli::array<Queue^>(concurrency);
				this->blocked = false;
				this->finishing = false;
				this->finished = 0;
				this->selected = 0;
				this->pinthreads = false;
				this->random = gcnew System::Random();
				for (int i = 0; i < concurrency; i++)
				{
					this->queues[i] = gcnew Queue();
					this->queues[i]->id = i;
#if defined(PROFILE_SavePerformanceStatistics_Detailed)
					this->queues[i]->id2 = ((double) 1) + 0.01 * ((double) i);
#endif
					this->queues[i]->automatic = i > 0;
					this->queues[i]->finished = false;
					this->queues[i]->root = nullptr;
					this->queues[i]->from = nullptr;
					this->queues[i]->to = nullptr;
					this->queues[i]->deposited = false;
					this->queues[i]->count = 0;
					this->queues[i]->operating = 0;
#if defined(PROFILE_SavePerformanceStatistics)
					this->queues[i]->statistics_dequeued = gcnew List<QS::_core_e_::Data::XY>();
#if defined(PROFILE_SavePerformanceStatistics_Detailed)
					this->queues[i]->statistics_busy = gcnew List<QS::_core_e_::Data::XY>();					
#endif
#endif
					#if defined(PROFILE_DIFFS)
					this->queues[i]->_last_handle = -1;
					this->queues[i]->statistics_diffs = gcnew List<QS::_core_e_::Data::XY>();					
#endif
					if (i > 0)
					{
						this->queues[i]->check = gcnew AutoResetEvent(false);
						this->queues[i]->thread = gcnew Thread(gcnew ParameterizedThreadStart(this, &QS::Fx::Scheduling::MultithreadedQueue::ThreadCallback));
						this->queues[i]->thread->Name = "Quicksilver(" + i.ToString() + ")";
						this->queues[i]->thread->Priority = ThreadPriority::Normal;
						this->queues[i]->thread->Start(this->queues[i]);
					}
				}
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Destructor

			MultithreadedQueue::~MultithreadedQueue()
			{
				this->finishing = true;
				for (int i = 1; i < this->concurrency; i++)
					this->queues[i]->check->Set();
				for (int i = 0; ((i < 20) && (this->finished < (this->concurrency - 1))); i++)
					Thread::Sleep(50);
				for (int i = 1; i < this->concurrency; i++)
				{
					if (this->queues[i]->finished)
						this->queues[i]->thread->Join();
					else
						this->queues[i]->thread->Abort();
				}
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Enqueue

			void MultithreadedQueue::Enqueue(QS::Fx::Base::IEvent^ e)
			{
				Queue^ queue;
				if (((e->SynchronizationOption & QS::Fx::Base::SynchronizationOption::Singlethreaded) == QS::Fx::Base::SynchronizationOption::None) && this->concurrency > 1)
					queue = this->queues[this->Balance()];
				else
					queue = this->queues[0];
				QS::Fx::Base::IEvent^ myroot;
				do
				{
					myroot = queue->root;
					e->Next = myroot;
				}
				while (Interlocked::CompareExchange<QS::Fx::Base::IEvent^>(queue->root, e, e->Next) != myroot);
#if defined(PROFILE_CountEnqueuesAndDequeues)
				Interlocked::Increment(queue->enqueued);
#endif
				queue->deposited = true;
				if (queue->automatic)
				{
					if (!queue->operating && !Interlocked::Exchange(queue->operating, 1))
						queue->check->Set();
				}
				else
					this->blocked = true;
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Dequeue

			QS::Fx::Base::IEvent^ MultithreadedQueue::Dequeue(Queue^ queue)
			{
				do
				{
					while (queue->deposited)
					{
						queue->deposited = false;
						QS::Fx::Base::IEvent^ e = Interlocked::Exchange<QS::Fx::Base::IEvent^>(queue->root, nullptr);
						QS::Fx::Base::IEvent^ s1 = nullptr;
						QS::Fx::Base::IEvent^ s2 = nullptr;
						while (e)
						{
#if defined(PROFILE_CountEnqueuesAndDequeues)
							queue->dequeued++;
#endif
							queue->count++;
							QS::Fx::Base::IEvent^ n = e->Next;
							if (s1 != nullptr)
								e->Next = s1;
							else
							{
								e->Next = nullptr;
								s2 = e;
							}
							s1 = e;
							e = n;
						}
						if (s1 != nullptr)
						{
							if (queue->to != nullptr)
								queue->to->Next = s1;
							else
								queue->from = s1;
							queue->to = s2;
						}
					}
					queue->operating = 0;
				}
				while (queue->deposited && !Interlocked::CompareExchange(queue->operating, 1, 0));
#if defined(PROFILE_SavePerformanceStatistics)
				queue->statistics_dequeued->Add(QS::_core_e_::Data::XY(this->clock->Time, (double) queue->dequeued));
#endif
				if (queue->from)
				{
					queue->count--;
					QS::Fx::Base::IEvent^ e = queue->from;
					queue->from = e->Next;
					if (!queue->from)
						queue->to = nullptr;
					e->Next = nullptr;
					return e;
				}
				else
					return nullptr;
			}

			QS::Fx::Base::IEvent^ MultithreadedQueue::Dequeue()
			{
				this->blocked = false;
				QS::Fx::Base::IEvent^ e = this->Dequeue(this->queues[0]);
				if (e != nullptr)
					this->blocked = true;
				return e;
			}       

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Balance

			int MultithreadedQueue::Balance()
			{
				switch (this->policy)
				{
					case Policy::RoundRobin:
					{
						int i, j;
						do
						{
							i = this->selected;
							j = (i + 1) % this->concurrency;
						}
						while (Interlocked::CompareExchange(this->selected, j, i) != i);
						return i;
					}
					break;

					case Policy::BalancedRoundRobin:
					{
						int i, j;
						do
						{
							i = this->selected;
							int j1 = (i + 1) % this->concurrency;
							int j2 = (i + 2) % this->concurrency;
							//j = (this->queues[j2]->count < this->queues[j1]->count) ? j2 : j1;
							j = (this->queues[j1]->count < this->queues[j2]->count) ? j1 : j2;
						}
						while (Interlocked::CompareExchange(this->selected, j, i) != i);
						return i;
					}
					break;

					case Policy::Random:
					{
						int i = this->random->Next(this->concurrency);
						this->selected = i;
						return i;
					}
					break;

					default:
						throw gcnew NotImplementedException();
				}
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region ThreadCallback

			void MultithreadedQueue::ThreadCallback(Object^ o)
			{
				Queue^ queue = dynamic_cast<Queue^>(o);

				if (this->pinthreads)
				{
					int mythreadid = ::GetCurrentThreadId();
					Process^ process = Process::GetCurrentProcess();
					for each (ProcessThread^ processthread in process->Threads) 
					{
						if (processthread->Id == mythreadid)
						{
							processthread->IdealProcessor = 0;
							processthread->ProcessorAffinity = System::IntPtr(1 << queue->id);
						}
					}
				}

				try
				{
					while (!this->finishing)
					{
						queue->check->WaitOne();
#if defined(PROFILE_SavePerformanceStatistics_Detailed)
						double t1 = this->clock->Time;
						queue->statistics_busy->Add(QS::_core_e_::Data::XY(t1 - 0.000000001, (double) 0));
						queue->statistics_busy->Add(QS::_core_e_::Data::XY(t1, queue->id2));
#endif
						QS::Fx::Base::IEvent^ e;
						while ((e = this->Dequeue(queue)) != nullptr) {
#if defined(PROFILE_DIFFS)
							if(queue->_last_handle!=-1) {
queue->statistics_diffs->Add(QS::_core_e_::Data::XY(queue->_last_handle,this->clock->Time - queue->_last_handle));
							}
#endif
							e->Handle();
#if defined(PROFILE_DIFFS)
		queue->_last_handle = this->clock->Time;					
							
#endif
						}
#if defined(PROFILE_SavePerformanceStatistics_Detailed)
						double t2 = this->clock->Time;
						queue->statistics_busy->Add(QS::_core_e_::Data::XY(t2 - 0.000000001, queue->id2));
						queue->statistics_busy->Add(QS::_core_e_::Data::XY(t2, (double) 0));
#endif
					}
				}
				catch (System::Exception^ exception)
				{
					System::Windows::Forms::MessageBox::Show(exception->ToString(), "Exception in thread ( " + queue->id.ToString() + " ).", 
						System::Windows::Forms::MessageBoxButtons::OK, System::Windows::Forms::MessageBoxIcon::Error);				
				}
				catch (...)
				{
					System::Windows::Forms::MessageBox::Show("Unknown exception.", "Exception in thread ( " + queue->id.ToString() + " ).", 
						System::Windows::Forms::MessageBoxButtons::OK, System::Windows::Forms::MessageBoxIcon::Error);				
				}
				finally
				{
					queue->finished = true;
					Interlocked::Increment(this->finished);
				}
			}       

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
		}
	}
}
