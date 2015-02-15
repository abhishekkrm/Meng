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

// #define DEBUG_CollectStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.FlowControl3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class RateCalculator3 : QS.Fx.Inspection.Inspectable, IRateCalculator
    {
        public static readonly TimeSpan DefaultMinimumWindow = TimeSpan.FromSeconds(0.5);

        public RateCalculator3(QS.Fx.Clock.IClock clock) : this(clock, DefaultMinimumWindow)
		{
		}

		public RateCalculator3(QS.Fx.Clock.IClock clock, TimeSpan minimumWindow)
		{
			this.clock = clock;
            this.minimumWindow = minimumWindow.TotalSeconds;
		}

		private QS.Fx.Clock.IClock clock;
        private double minimumWindow, timeCalculated = 0, valueCalculated = 0;
        private int nreceived = 0;

        #region IRateCalculator Members

        void IRateCalculator.sample()
        {
            lock (this)
            {
                nreceived++;
            }
        }

        double IRateCalculator.Rate
        {
            get 
            {
                lock (this)
                {
                    double time = clock.Time;
                    double interval = time - timeCalculated;
                    if (interval >= minimumWindow)
                    {
                        double newrate = ((double)nreceived) / interval;
                        timeCalculated = time;
                        nreceived = 0;
                        valueCalculated = newrate;
                    }

                    return valueCalculated;
                }
            }
        }

        #endregion
}
}
