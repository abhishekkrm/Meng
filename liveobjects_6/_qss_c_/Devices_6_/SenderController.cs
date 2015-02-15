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

namespace QS._qss_c_.Devices_6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class SenderController : QS.Fx.Inspection.Inspectable, ISenderController
    {
        private const int DefaultMaximumSends = 1000;
        private const int DefaultMinimumBytes = 10000;
        private const int DefaultMaximumBytes = 1000000;

        public SenderController() 
            : this(QS._core_c_.Base2.PreciseClock.Clock, DefaultMaximumSends, DefaultMinimumBytes, DefaultMaximumBytes)
        {
        }

        public SenderController(QS.Fx.Clock.IClock clock, int maximumSends, int minimumBytes, int maximumBytes)
        {
            this.maximumSends = maximumSends;
            this.minimumBytes = minimumBytes;
            this.maximumBytes = maximumBytes;
            this.clock = clock;
        }

        private int maximumSends, minimumBytes, maximumBytes;
        private int sends, bytes;
        private Queue<IControlledSender> senderQueue = new Queue<IControlledSender>();
        private QS.Fx.Clock.IClock clock;

#if DEBUG_AllowCollectingOfStatistics
        [QS.CMS.Diagnostics.Component("Concurrent Sends (X = time, Y = count)")]
        private QS.CMS.Statistics.SamplesXY timeSeries_concurrentSends = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Megabytes in Transit (X = time, Y = count)")]
        private QS.CMS.Statistics.SamplesXY timeSeries_megabytesInTransit = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Adjusting Configuration

        public int MaximumSends
        {
            get { return maximumSends; }
            set { maximumSends = value; }
        }

        public int MaximumBytes
        {
            get { return maximumBytes; }
            set { maximumBytes = value; }
        }

        #endregion

        #region Internal Processing

        private void ProcessQueue()
        {
            while (senderQueue.Count > 0)
            {
                int availableSends = maximumSends - sends;
                int availableBytes = maximumBytes - bytes;

                if (availableSends > 0 && availableBytes > minimumBytes)
                {
                    IControlledSender sender = senderQueue.Dequeue();

                    int consumedBytes, consumedSends;
                    bool moreToGo;

                    Monitor.Exit(this);
                    try
                    {
                        sender.Consume(availableSends, availableBytes, out consumedSends, out consumedBytes, out moreToGo);
                    }
                    finally
                    {
                        Monitor.Enter(this);
                    }

                    sends += consumedSends;
                    bytes += consumedBytes;

                    if (moreToGo)
                        senderQueue.Enqueue(sender);
                }
                else
                    break;
            }

#if DEBUG_AllowCollectingOfStatistics
            if (timeSeries_concurrentSends.Enabled || timeSeries_megabytesInTransit.Enabled)
            {
                double time = clock.Time;
                if (timeSeries_concurrentSends.Enabled)
                    timeSeries_concurrentSends.addSample(time, sends);
                if (timeSeries_megabytesInTransit.Enabled)
                    timeSeries_megabytesInTransit.addSample(time, ((double)bytes) / ((double)QS.Constants.MEGA));
            }
#endif
        }

        #endregion

        #region ISenderController Members

        void ISenderController.Register(IControlledSender sender)
        {
            lock (this)
            {
                senderQueue.Enqueue(sender);
                ProcessQueue();
            }
        }

        void ISenderController.Release(int completedBytes, int completedSends)
        {
            lock (this)
            {
                bytes -= completedBytes;
                sends -= completedSends;

                ProcessQueue();
            }
        }

        #endregion
    }
}
