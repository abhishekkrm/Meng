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
    [QS.CMS.Diagnostics.ComponentContainer]
    public class RateController4 : TMS.Inspection.Inspectable, IRateController
    {
        public RateController4(Core.IClock clock, Core.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, double rate, int burstiness)
        {
            this.logger = logger;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.rate = rate;
            this.burstiness = burstiness;
        }

        private QS.Fx.Logging.ILogger logger;
        private Core.IAlarmClock alarmClock;
        private Core.IClock clock;
        private double rate, lastChecked, resume;
        private int burstiness;
        private Core.IAlarm alarm;
        private event Core.Callback onReady;
        private bool waiting;

/-*
        private int currentPosition, elementCount, windowSize, burstSize;        
        private double[] consumptionTimes;        
*-/

        #region IRateController Members

        bool IRateController.Ready
        {
            get 
            { 
                double time_now = clock.Time;
/-*
                if (time_now < nextWakeup)
                {
                    // ...........
                }
                else
                {
                }
*-/
                throw new NotImplementedException();
            }
        }

        void IRateController.Consume()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        event QS.CMS.Core.Callback IRateController.OnReady
        {
            add { onReady += value; }
            remove { onReady -= value; }
        }

        double IRateController.MaximumRate
        {
            get { return rate; }
            set { rate = value; }
        }

        #endregion
    }
*/
}
