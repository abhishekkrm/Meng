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
using System.Net;

namespace QS._qss_c_.Devices_6_
{
    public class ReceiverController : IReceiverController
    {
        public ReceiverController(QS.Fx.Clock.IClock clock, int mtu, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.clock = clock;
            this.mtu = mtu;
            this.eventLogger = eventLogger;
        }

        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private int mtu, numberOfBuffersAllocated, numberOfBuffersCurrentlyInUse;
        private Queue<ReceiveBuffer> pendingQueue = new Queue<ReceiveBuffer>();
        private Queue<ReceiveBuffer> processingQueue = new Queue<ReceiveBuffer>();
        private Queue<ReceiveBuffer> freeQueue = new Queue<ReceiveBuffer>();
        private bool processing;
        private ReceiveCallback receiveCallback;

#if DEBUG_AllowCollectingOfStatistics
        [Diagnostics.Component("Number of Buffers Allocated")]
        private Statistics.SamplesXY timeSeries_numberOfBuffersAllocated = new QS.CMS.Statistics.SamplesXY();
        [Diagnostics.Component("Number of Buffers Currently In Use")]
        private Statistics.SamplesXY timeSeries_numberOfBuffersCurrentlyInUse = new QS.CMS.Statistics.SamplesXY();
        [Diagnostics.Component("Receiving Times and Sizes")]
        private Statistics.SamplesXY timeSeries_receiveTimes = new QS.CMS.Statistics.SamplesXY();
        [Diagnostics.Component("Processing Times and Counts")]
        private Statistics.SamplesXY timeSeries_processingTimesAndCounts = new QS.CMS.Statistics.SamplesXY();
        [Diagnostics.Component("Processing Times and Overheads")]
        private Statistics.SamplesXY timeSeries_processingTimesAndOverheads = new QS.CMS.Statistics.SamplesXY();
#endif

        public ReceiveCallback Callback
        {
            get { return receiveCallback; }
            set { receiveCallback = value; }
        }

        #region Internal Processing

        private void Process(ReceiveBuffer buffer)
        {
            try
            {
                receiveCallback(buffer);
            }
            catch (Exception exc)
            {
                if (eventLogger.Enabled)
                    eventLogger.Log(new Logging_1_.Events.ExceptionCaught(clock.Time, this, exc));
            }
        }

        #endregion

        #region IReceiverController Members

        void IReceiverController.Enqueue(ref ReceiveBuffer buffer)
        {
            lock (this)
            {
                double now = clock.Time;

                if (buffer != null)
                {
                    pendingQueue.Enqueue(buffer);
                    numberOfBuffersCurrentlyInUse--;

#if DEBUG_AllowCollectingOfStatistics
                    if (timeSeries_receiveTimes.Enabled)
                        timeSeries_receiveTimes.addSample(now, buffer.Buffer.Count);
#endif
                }

                if (freeQueue.Count > 0)
                    buffer = freeQueue.Dequeue();
                else
                {
                    buffer = new ReceiveBuffer(mtu);
                    numberOfBuffersAllocated++;
                }

                numberOfBuffersCurrentlyInUse++;

#if DEBUG_AllowCollectingOfStatistics
                if (timeSeries_numberOfBuffersAllocated.Enabled)
                    timeSeries_numberOfBuffersAllocated.addSample(now, numberOfBuffersAllocated);
                if (timeSeries_numberOfBuffersCurrentlyInUse.Enabled)
                    timeSeries_numberOfBuffersCurrentlyInUse.addSample(now, numberOfBuffersCurrentlyInUse);
#endif
            }            
        }

        void IReceiverController.Process()
        {
            bool process_now;
            lock (this)
            {
                process_now = !processing && pendingQueue.Count > 0;                
                if (process_now)
                {
                    processing = true;
                    Queue<ReceiveBuffer> temp = pendingQueue;
                    pendingQueue = processingQueue;
                    processingQueue = temp;

#if DEBUG_AllowCollectingOfStatistics
                    if (timeSeries_processingTimesAndCounts.Enabled)
                        timeSeries_processingTimesAndCounts.addSample(clock.Time, processingQueue.Count);
#endif
                }
            }

            if (process_now)
            {
                while (true)
                {
                    foreach (ReceiveBuffer buffer in processingQueue)
                    {
#if DEBUG_AllowCollectingOfStatistics
                        double timestamp = clock.Time;
#endif

                        this.Process(buffer);

#if DEBUG_AllowCollectingOfStatistics
                        if (timeSeries_processingTimesAndOverheads.Enabled)
                        {
                            lock (timeSeries_processingTimesAndOverheads)
                            {
                                if (timeSeries_processingTimesAndOverheads.Enabled)
                                    timeSeries_processingTimesAndOverheads.addSample(timestamp, clock.Time - timestamp);
                            }
                        }
#endif
                    }

                    lock (this)
                    {
                        foreach (ReceiveBuffer buffer in processingQueue)
                            freeQueue.Enqueue(buffer);
                        processingQueue.Clear();

                        if (pendingQueue.Count > 0)
                        {
                            Queue<ReceiveBuffer> temp = pendingQueue;
                            pendingQueue = processingQueue;
                            processingQueue = temp;

#if DEBUG_AllowCollectingOfStatistics
                            if (timeSeries_processingTimesAndCounts.Enabled)
                                timeSeries_processingTimesAndCounts.addSample(clock.Time, processingQueue.Count);
#endif
                        }
                        else
                        {
                            processing = false;
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
