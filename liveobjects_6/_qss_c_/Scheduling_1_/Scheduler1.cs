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

// #define DEBUG_AllowCollectingOfStatistics

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#pragma warning disable 0420

namespace QS._qss_c_.Scheduling_1_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Scheduler1 : QS.Fx.Inspection.Inspectable, IScheduler
    {
        public Scheduler1(QS.Fx.Clock.IClock clock, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.clock = clock;
            this.eventLogger = eventLogger;
        }

        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS.Fx.Clock.IClock clock;
        private Synchronization_1_.IAccumulatingStack<IEvent> eventStack = new Synchronization_1_.NonblockingStack<IEvent>();
        private Stack<IEvent> workingStack = new Stack<IEvent>();
        private volatile int working;
        private volatile bool clean = true;

#if DEBUG_AllowCollectingOfStatistics
        [Diagnostics.Component("Dequeue Times and Counts")]
        private Statistics.SamplesXY timeSeries_dequeueTimesAndCounts = new QS.CMS.Statistics.SamplesXY();
        [Diagnostics.Component("Processing Times")]
        private Statistics.SamplesXY timeSeries_processingTimes = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Internal Processing

        private void Process(IEvent element)
        {
#if DEBUG_AllowCollectingOfStatistics
            double t1 = clock.Time;
#endif

            try
            {
                element.Process();
            }
            catch (Exception exc)
            {
                if (eventLogger.Enabled)
                    eventLogger.Log(new Logging_1_.Events.ExceptionCaught(clock.Time, null, exc));
            }

#if DEBUG_AllowCollectingOfStatistics
            double t2 = clock.Time;
            if (timeSeries_processingTimes.Enabled)
                timeSeries_processingTimes.addSample(t1, t2 - t1);
#endif
        }

        #endregion

        #region IScheduler Members

        void IScheduler.Schedule(IEvent e)
        {
            eventStack.Add(e);
            clean = false;
        }

        void IScheduler.Schedule(Synchronization_1_.ChainOf<IEvent> c)
        {
            eventStack.Add(c);
            clean = false;
        }

        void IScheduler.Work()
        {
            while (!clean && Interlocked.Exchange(ref working, 1) == 0)
            {
                do
                {
                    clean = true;
                    IEvent element = eventStack.AccumulatedChain;

                    if (element != null)
                    {
                        do
                        {
                            workingStack.Push(element);
                            element = ((QS._core_c_.Synchronization.ILinkable<IEvent>) element).Next;
                        }
                        while (element != null);

#if DEBUG_AllowCollectingOfStatistics
                        if (timeSeries_dequeueTimesAndCounts.Enabled)
                            timeSeries_dequeueTimesAndCounts.addSample(clock.Time, workingStack.Count);
#endif

                        foreach (IEvent e in workingStack)
                            Process(e);
                        
                        workingStack.Clear();
                    }
                }
                while (!clean);

                working = 0;
            }
        }

        #endregion
    }
}

#pragma warning restore 0420
