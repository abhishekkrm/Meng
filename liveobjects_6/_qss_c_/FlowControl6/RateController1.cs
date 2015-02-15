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

// #define STATISTICS_MeasureCredits

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.FlowControl6
{
    [QS.Fx.Base.Inspectable]
    [Logging_1_.IgnoreCallbacks]
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class RateController1 : QS.Fx.Inspection.Inspectable, IFlowController, QS._core_c_.FlowControl3.IRateControlled
    {
        public RateController1(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock,
            double sending_rate, double upper_threshold, QS._qss_c_.Base3_.NoArgumentCallback readyCallback)
        {
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.sending_rate = sending_rate;
            this.upper_threshold = upper_threshold;
            this.readyCallback = readyCallback;

            current_credit = upper_threshold;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private double sending_rate, upper_threshold, current_credit, timeStarted;
        private QS._qss_c_.Base3_.NoArgumentCallback readyCallback;
        private QS.Fx.Clock.IAlarm recheckingAlarmRef;
        // private bool waiting = false;

#if STATISTICS_MeasureCredits
        [QS.CMS.Diagnostics.Component("Credit Samples")]
        private QS.CMS.Statistics.SamplesXY creditSamples = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Internal Processing

        private void RecheckingCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            try
            {
                // bool was_waiting;

                lock (this)
                {
                    double time_now = clock.Time;
                    current_credit = current_credit + (time_now - timeStarted) * sending_rate;
                    if (current_credit < upper_threshold)
                    {
                        timeStarted = time_now;

                        if (recheckingAlarmRef != null)
                            recheckingAlarmRef.Reschedule(upper_threshold / sending_rate);
                        else
                            recheckingAlarmRef = alarmClock.Schedule(
                                upper_threshold / sending_rate, new QS.Fx.Clock.AlarmCallback(this.RecheckingCallback), null);
                    }
                    else
                    {
                        current_credit = upper_threshold;
                        recheckingAlarmRef = null;
                    }

                    // was_waiting = waiting;
                    // waiting = false;

#if STATISTICS_MeasureCredits
                    creditSamples.addSample(time_now, current_credit);
#endif
                }

                //            if (was_waiting)

                readyCallback();
            }
            catch (Exception exc)
            {
                logger.Log(this, exc.ToString());
            }
        }

        private void ConsumeOne()
        {
            current_credit = current_credit - 1;
            if (current_credit < upper_threshold && recheckingAlarmRef == null)
            {
                timeStarted = clock.Time;
                recheckingAlarmRef = alarmClock.Schedule(
                    upper_threshold / sending_rate, new QS.Fx.Clock.AlarmCallback(this.RecheckingCallback), null);
            }
            
#if STATISTICS_MeasureCredits
            creditSamples.addSample(clock.Time, current_credit);
#endif
        }

        #endregion

        #region IFlowController Members

        bool IFlowController.Ready
        {
            get { return current_credit > 0; }
        }

        void IFlowController.Wait()
        {
            // lock (this)
            // {
            //    waiting = true;
            //    // may need to reschedule alarm...
            // }
        }

        void IFlowController.Consume()
        {
            lock (this)
            {
                ConsumeOne();
            }
        }

        bool IFlowController.TryConsume()
        {
            lock (this)
            {
                bool consume_now = ((IFlowController)this).Ready;
                if (consume_now)
                {
                    ConsumeOne();
                }
                else
                {
                    // waiting = true;
                }
                return consume_now;
            }
        }

        QS._qss_c_.Base3_.NoArgumentCallback IFlowController.ReadyCallback
        {
            set { this.readyCallback = value; }
        }

        #endregion

        #region IRateControlled Members

        double QS._core_c_.FlowControl3.IRateControlled.MaximumRate
        {
            get { return sending_rate; }
            set
            {
                lock (this)
                {
                    double time_now = clock.Time;
                    current_credit = current_credit + (time_now - timeStarted) * sending_rate;
                    timeStarted = time_now;
                    sending_rate = value;

#if STATISTICS_MeasureCredits
                    creditSamples.addSample(time_now, current_credit);
#endif
                }
            }
        }

        #endregion
    }
}
