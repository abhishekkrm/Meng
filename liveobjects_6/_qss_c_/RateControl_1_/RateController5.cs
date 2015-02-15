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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.RateControl_1_
{
/*
    [TMS.Inspection.Inspectable]
    [QS.CMS.Diagnostics.ComponentContainer]
    public sealed class RateController5 : TMS.Inspection.Inspectable, IRateController, QS.CMS.QS._core_c_.Diagnostics2.IModule
    {
        #region Class Configuration

        public class Configuration
        {
            public Configuration(int minimumBurstSize, int maximumBurstSize, double minimumAlarmInterval)
            {
                this.minimumBurstSize = minimumBurstSize;
                this.maximumBurstSize = maximumBurstSize;
                this.minimumAlarmInterval = minimumAlarmInterval;
            }

            public Configuration(Configuration other) 
                : this(other.minimumBurstSize, other.maximumBurstSize, other.minimumAlarmInterval)
            {
            }

            private int minimumBurstSize, maximumBurstSize;
            private double minimumAlarmInterval;

            #region Accessors

            public int MinimumBurstSize
            {
                get { return minimumBurstSize; }
                set { minimumBurstSize = value; }
            }

            public int MaximumBurstSize
            {
                get { return maximumBurstSize; }
                set { maximumBurstSize = value; }
            }

            public double MinimumAlarmInterval
            {
                get { return minimumAlarmInterval; }
                set { minimumAlarmInterval = value; }
            }

            #endregion
        }

        #endregion

        #region Class Class

        public class Class : IRateControllerClass
        {
            public Class(Configuration configuration)
            {
                this.configuration = configuration;
            }

            private Configuration configuration;

            #region IRateControllerClass Members

            IRateController IRateControllerClass.Create(
                QS.CMS.Core.IClock clock, QS.CMS.Core.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, 
                double maximumRate, QS.CMS.Statistics.IStatisticsController statisticsController)
            {
                return new RateController5(new Configuration(configuration), clock, alarmClock, statisticsController, maximumRate); 
            }

            #endregion
        }

        #endregion

        public RateController5(Configuration configuration, 
            Core.IClock clock, Core.IAlarmClock alarmClock, Statistics.IStatisticsController statisticsController, double maximumRate)
        {
            this.configuration = configuration;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.statisticsController = statisticsController;            
            this.maximumRate = maximumRate;
            
            rateAlarmCallback = new Core.AlarmCallback(this.RateCallback);

            Adjust();
            RegenerateCredits();

/-*
            this.lowCredits = 0;
            this.highCredits = maximumCredits / 2;
            this.recoveryMaxCount = 50;
*-/

            QS.CMS.QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private Configuration configuration;
        private Core.IAlarmClock alarmClock;
        private Core.IClock clock;
        private Statistics.IStatisticsController statisticsController;
        private double maximumRate;
        private QS.CMS.QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS.CMS.QS._core_c_.Diagnostics2.Container();
        private event Core.Callback readyCallback;
//        private Core.IAlarm rateAlarm;
        private Core.AlarmCallback rateAlarmCallback;

        private double credits, minimumCredits, schedulingPenalty, maximumCredits, lastChecked = -1000;
        private bool waiting;

/-*
        private double , lowCredits, highCredits, recoveryMaxCount;
*-/

        #region Regenerate Credits

        private void RegenerateCredits()
        {
            double now = clock.Time;            
            if (now > lastChecked)
            {
                credits = Math.Min(credits + (now - lastChecked) * maximumRate, maximumCredits);
                lastChecked = now;
            }
        }

        #endregion

        #region Adjust

        private void Adjust()
        {
            double optimal_interval = Math.Max(configuration.MinimumAlarmInterval, 1 / maximumRate);
            maximumCredits = Math.Max(configuration.MinimumBurstSize, 
                Math.Min(optimal_interval * schedulingPenalty, configuration.MaximumBurstSize));
            minimumCredits = Math.Max(1, optimal_interval * (schedulingPenalty - 1) * maximumRate);
        }

        #endregion

        #region RateCallback

        private void RateCallback(Core.IAlarm alarm)
        {
/-*
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
                readyCallback();
*-/
        }

        #endregion

        #region IRateController Members

        void IRateController.Consume()
        {
            credits--;
            if (credits < minimumCredits && !waiting)
            {
                waiting = true;
                double interval = Math.Max(configuration.MinimumAlarmInterval, (maximumCredits - credits) / (maximumRate * schedulingPenalty));

//                if (rateAlarm == null)
//                    rateAlarm = alarmClock.Schedule(

// ........................................................................................................................
/-*

                if (credits < lowCredits && !waitingForCredits)
                {
                    waitingForCredits = true;
                    rateAlarm = alarmClock.Schedule(
                        Math.Min(highCredits - credits, recoveryMaxCount) / maximumRate, rateAlarmCallback, null);
                }
            }
*-/
            }
        }

        bool IRateController.Ready
        {
            get { return (credits > 0); }
        }

        double IRateController.MaximumRate
        {
            set
            {
                maximumRate = value;
                Adjust();
            }

            get { return maximumRate; }
        }

        event Core.Callback IRateController.OnReady
        {
            add { readyCallback += value; }
            remove { readyCallback -= value; }
        }

        #endregion

        #region IModule Members

        QS.CMS.QS._core_c_.Diagnostics2.IComponent QS.CMS.QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion
    }
*/
}
