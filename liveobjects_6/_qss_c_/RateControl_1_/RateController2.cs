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

// #define DEBUG_LogAlarms

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.RateControl_1_
{
    [QS.Fx.Base.Inspectable]
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class RateController2 : QS.Fx.Inspection.Inspectable, QS._core_c_.RateControl.IRateController, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Class Class

        public class Class : QS._core_c_.RateControl.IRateControllerClass
        {
            public Class()
            {
            }

            #region IRateControllerClass Members

            QS._core_c_.RateControl.IRateController QS._core_c_.RateControl.IRateControllerClass.Create(
                QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, double maximumRate,
                QS._core_c_.Statistics.IStatisticsController statisticsController)
            {
                return new RateController2(clock, alarmClock, logger, maximumRate, statisticsController);
            }

            #endregion
        }

        #endregion

        public RateController2(QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, double rate, QS._core_c_.Statistics.IStatisticsController statisticsController)
        {
            this.logger = logger;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.rate = rate;

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private int currentPosition, elementCount, windowSize;
        private double rate;
        private double[] consumptionTimes;
        private bool waiting;
        private QS.Fx.Clock.IAlarm alarm;
        private event QS.Fx.Base.Callback onReady;

#if DEBUG_LogAlarms
        [QS.CMS.Diagnostics.Component("Alarms")]
        QS.CMS.Statistics.SamplesXY timeseries_alarms = new QS.CMS.Statistics.SamplesXY();
#endif

        public int WindowSize
        {
            get { return windowSize; }
            set 
            {                       
                double[] new_consumptionTimes = new double[value];
                
                if (value < elementCount)
                    elementCount = value;

                if (windowSize > 0)
                {
                    for (int ind = 0; ind < elementCount; ind++)
                        new_consumptionTimes[ind] = consumptionTimes[(currentPosition + windowSize - elementCount + 1) % windowSize];
                }

                currentPosition = 0;
                windowSize = value;
                consumptionTimes = new_consumptionTimes;
            }
        }

        private void RecheckingCallback(QS.Fx.Clock.IAlarm alarm)
        {
            try
            {
                bool ready_now = elementCount < windowSize;
                if (!ready_now)
                {
                    double now = clock.Time;
                    double time_ready = consumptionTimes[currentPosition] + windowSize / rate;
                    ready_now = now >= time_ready;
                    if (!ready_now)
                    {
                        alarm.Reschedule(time_ready - now);

#if DEBUG_LogAlarms
                        timeseries_alarms.addSample(now, time_ready - now);
#endif
                    }
                }

                if (ready_now) 
                {
                    waiting = false;
                    if (onReady != null)
                        onReady();
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, exc.ToString());
            }
        }

        #region IRateController Members

        bool QS._core_c_.RateControl.IRateController.Ready
        {
            get 
            {
                if (windowSize == 0)
                    return false;

                if (elementCount < windowSize)
                    return true;
                else
                {
                    double now = clock.Time;
                    double time_ready = consumptionTimes[currentPosition] + windowSize / rate;
                    if (now >= time_ready)
                        return true;
                    else
                    {
                        if (!waiting)
                        {
                            waiting = true;

                            if (alarm == null)
                                alarm = alarmClock.Schedule(
                                    time_ready - now, new QS.Fx.Clock.AlarmCallback(this.RecheckingCallback), null);
                            else
                                alarm.Reschedule(time_ready - now);

#if DEBUG_LogAlarms
                            timeseries_alarms.addSample(now, time_ready - now);
#endif
                        }

                        return false;
                    }
                }
            }
        }

        void QS._core_c_.RateControl.IRateController.Consume()
        {
            double now = clock.Time;
            if (elementCount < windowSize)
                elementCount++;
            else
            {
//                if (now < consumptionTimes[currentPosition] + windowSize * rate)
//                    throw new Exception("Cannot consume right now, not ready yet.");
            }

            consumptionTimes[currentPosition] = now;
            currentPosition = (currentPosition + 1) % windowSize;
        }

        event QS.Fx.Base.Callback QS._core_c_.RateControl.IRateController.OnReady
        {
            add { onReady += value; }
            remove { onReady -= value; }
        }

        double QS._core_c_.RateControl.IRateController.MaximumRate
        {
            get { return rate; }
            set { rate = value; }
        }

        #endregion
    }
}
