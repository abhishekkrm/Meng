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

// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Base6_
{
    public class Generator1<C> : Source<C>, ISource<C>
    {
        public Generator1(GetCallback<C> getCallback, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, double rate, int burstiness)
        {
            this.getCallback = getCallback;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.rate = rate;
            this.burstiness = burstiness;            
        }

        private GetCallback<C> getCallback;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private double rate, consumed, lastChecked;
        private int burstiness;
        private QS.Fx.Clock.IAlarm signalingAlarm;

        protected override void ChannelChanged()
        {
            if (channel != null)
            {
                Monitor.Exit(this);
                try
                {
                    channel.Signal();
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
        }

        private void SignalingCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            bool signal_now = false;
            lock (this)
            {
                Adjust();

                if (consumed <= (burstiness - 1))
                {
                    signalingAlarm = null;
                    signal_now = true;
                }
                else
                {
                    signalingAlarm = alarmClock.Schedule(consumed / rate,
                        new QS.Fx.Clock.AlarmCallback(this.SignalingCallback), null);
                }
            }

            if (signal_now)
                channel.Signal();
        }

        private void Adjust()
        {
            double now = clock.Time;
            consumed -= ((now - lastChecked) / rate);
            if (consumed < 0)
                consumed = 0;
            lastChecked = now;
        }

        #region ISource<C> Members

        void ISource<C>.GetObjects(Queue<C> objectQueue,
                int maximumNumberOfObjects,
#if UseEnhancedRateControl
                int maximumNumberOfBytes, 
#endif
                out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
                out int numberOfBytesReturned,
#endif
                out bool moreObjectsAvailable)
        {
            // TODO: Implement enhanced rate control

            lock (this)
            {
                Adjust();

                int maxToConsume = burstiness - (int) Math.Ceiling(consumed);
                if (maximumNumberOfObjects < maxToConsume)
                    maxToConsume = maximumNumberOfObjects;

                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                numberOfBytesReturned = 0;
#endif
                while (numberOfObjectsReturned < maxToConsume)
                {
                    objectQueue.Enqueue(getCallback());
                    numberOfObjectsReturned++;
                    // numberOfBytesReturned += ...............................................................................HERE
                }

                consumed += numberOfObjectsReturned;

                moreObjectsAvailable = consumed <= (burstiness - 1);

                if (!moreObjectsAvailable && signalingAlarm == null)
                    signalingAlarm = alarmClock.Schedule(consumed / rate,
                        new QS.Fx.Clock.AlarmCallback(this.SignalingCallback), null);
            }
        }

        #endregion
    }
}
