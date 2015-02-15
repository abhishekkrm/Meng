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

// ----- THESE PRODUCE HUGE DATA DUMPS -----

// #define DEBUG_LogCredits

// ------------------------------------------------------------------

// #define DEBUG_RegisterAlarmFiringDelays

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._core_c_.RateControl
{
    [QS.Fx.Base.Inspectable]
    [QS._core_c_.Diagnostics.ComponentContainer]
    public sealed class RateController1 : QS.Fx.Inspection.Inspectable, IRateController, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Class Class

        public class Class : IRateControllerClass
        {
            public Class(double maximumCredits)
            {
                this.maximumCredits = maximumCredits;
            }

            private double maximumCredits;

            public double MaximumCredits
            {
                get { return maximumCredits; }
                set { maximumCredits = value; }
            }

            #region IRateControllerClass Members

            IRateController IRateControllerClass.Create(
                QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, double maximumRate,
                Statistics.IStatisticsController statisticsController)
            {
                return new RateController1(clock, alarmClock, maximumRate, maximumCredits, statisticsController);
            }

            #endregion
        }

        #endregion

        public RateController1(QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, double maximumRate, double maximumCredits,
            Statistics.IStatisticsController statisticsController)
        {
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.maximumRate = maximumRate;
            this.credits = this.maximumCredits = maximumCredits;
            this.lowCredits = 0; 
            this.highCredits = maximumCredits / 2;
            this.recoveryMaxCount = 50;
            this.statisticsController = statisticsController;

#if DEBUG_RegisterAlarmFiringDelays
            ts_AlarmFiringDelays = statisticsController.Allocate2D("alarm firing delays", "times and delays in firing alarms for recreating credits",
                "time", "s", "time when the alarm fired", "firing delay", "s", "how much firing of the alarm was delayed with respect to the orginal schedule");
#endif

            lastChecked = clock.Time;
            waitingForCredits = false;
			rateAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.RateCallback);

#if DEBUG_LogCredits
            ts_Credits = statisticsController.Allocate2D("credits", "number of credits at different times",
                "time", "s", "time when sample was taken", "number of credits", "", "how many credits were left at the given time");
			ts_Credits.Add(clock.Time, credits);
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private double credits, lastChecked, lowCredits, highCredits, maximumCredits, maximumRate, recoveryMaxCount;
        private bool waitingForCredits;
        private QS.Fx.Clock.IAlarm rateAlarm;
        private QS.Fx.Clock.AlarmCallback rateAlarmCallback;
        private event QS.Fx.Base.Callback readyCallback;
        private Statistics.IStatisticsController statisticsController;

#if DEBUG_RegisterAlarmFiringDelays
        [QS._core_c_.Diagnostics2.Property("AlarmFiringDelays")]
        private Statistics.ISamples2D ts_AlarmFiringDelays;
#endif

#if DEBUG_LogCredits
        // [QS.CMS.Diagnostics.Component("Credits")]
        [QS._core_c_.Diagnostics2.Property("Credits")]
        private Statistics.ISamples2D ts_Credits;
#endif

        public double MaximumCredits
        {
            get { return maximumCredits; }
            set { maximumCredits = value; }
        }

        public double LowCredits
        {
            get { return lowCredits; }
            set { lowCredits = value; }
        }

        public double HighCredits
        {
            get { return highCredits; }
            set { highCredits = value; }
        }

        public double RecoveryMaxCount
        {
            get { return recoveryMaxCount; }
            set { recoveryMaxCount = value; }
        }

        private void regenerateCredits()
        {
			double now = clock.Time;
            if (now > lastChecked)
            {
                if (double.IsPositiveInfinity(maximumRate))
                    credits = maximumCredits;
                else
                {
                    credits += (now - lastChecked) * maximumRate;
                    if (credits > maximumCredits)
                        credits = maximumCredits;
                }
                lastChecked = now;

#if DEBUG_LogCredits
                if (ts_Credits.Enabled)
                    ts_Credits.Add(now, credits);
#endif
            }
        }

        #region RateCallback

        private void RateCallback(QS.Fx.Clock.IAlarm alarm)
        {
#if DEBUG_RegisterAlarmFiringDelays
            double now = clock.Time;
            double delay = now - alarm.Time;
            ts_AlarmFiringDelays.Add(now, delay);
#endif

			regenerateCredits();

            double recoveryCount = Math.Min(highCredits - credits, recoveryMaxCount);
            if (recoveryCount > 0)
                alarm.Reschedule(recoveryCount / maximumRate);
            else
            {
                rateAlarm = null;
				waitingForCredits = false;
            }

            if (credits > 0)
            {
                //readyCallback();

            }
        }

        #endregion

        #region IRateController Members

        double IRateController.MaximumRate
        {
            get { return maximumRate; }
            set { maximumRate = value; }
        }

        bool IRateController.Ready
        {
            get 
            {
                if (credits > 0)
                    return true;
                else
                {
                    regenerateCredits();
                    return credits > 0;
                }
            }
        }

        event QS.Fx.Base.Callback IRateController.OnReady
        {
            add { readyCallback += value; }
            remove { readyCallback -= value; }
        }

        void IRateController.Consume()
        {
			if (!double.IsPositiveInfinity(maximumRate))
			{
				credits--;

#if DEBUG_LogCredits
                if (ts_Credits.Enabled)
                    ts_Credits.Add(clock.Time, credits);
#endif

				if (credits < lowCredits && !waitingForCredits)
				{
					waitingForCredits = true;
					rateAlarm = alarmClock.Schedule(
                        Math.Min(highCredits - credits, recoveryMaxCount) / maximumRate, rateAlarmCallback, null); 
				}
			}
        }

        #endregion
    }
}
