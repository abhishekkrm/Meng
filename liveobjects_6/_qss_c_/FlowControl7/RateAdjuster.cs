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

#define DEBUG_AllowCollectingStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.FlowControl7
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class RateAdjuster : QS.Fx.Inspection.Inspectable, IRateAdjuster, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public RateAdjuster(QS.Fx.Clock.IClock clock, double inertia, double growth, double maxadd, double maxzoom)
        {
            this.maxadd = maxadd;
            this.clock = clock;
            this.inertia = inertia;
            this.growth = growth;
            this.maxzoom = maxzoom;

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.FlowControl3.IRateControlled controlled;
        private QS.Fx.Clock.IClock clock;
        private double measuredRate, requestedRate, calculatedRate, inertia, growth, lastChanged, maxadd, maxzoom;

        public double MaxAdd
        {
            get { return maxadd; }
            set { maxadd = value; }
        }

        public double MaxZoom
        {
            get { return maxzoom; }
            set { maxzoom = value; }
        }

        #region Statistics

#if DEBUG_AllowCollectingStatistics
        [QS._core_c_.Diagnostics2.Property("MeasuredRates")]
        private QS._qss_c_.Statistics_.Samples2D timeSeries_measuredRates = new QS._qss_c_.Statistics_.Samples2D(
            "measured rates", "actual sending rates as measured by the controlled element", "time", "s", "", "rate", "packets/s", "");
        [QS._core_c_.Diagnostics2.Property("RequestedRates")]
        private QS._qss_c_.Statistics_.Samples2D timeSeries_requestedRates = new QS._qss_c_.Statistics_.Samples2D(
            "requested rates", "desired target rates as set by the rate controller", "time", "s", "", "rate", "packets/s", "");
        [QS._core_c_.Diagnostics2.Property("CalculatedRates")]
        private QS._qss_c_.Statistics_.Samples2D timeSeries_calculatedRates = new QS._qss_c_.Statistics_.Samples2D(
            "calculated rates", "parameter to configure the controlled component with", "time", "s", "", "rate", "packets/s", "");

        [QS._core_c_.Diagnostics.Component]
        private QS._core_e_.Data.DataCo CombinedStatistics
        {
            get
            {
                QS._core_e_.Data.DataCo data = new QS._core_e_.Data.DataCo("Combined Statistics", "");
                data.Add(new QS._core_e_.Data.Data2D("measured", timeSeries_measuredRates.Samples));
                data.Add(new QS._core_e_.Data.Data2D("requested", timeSeries_requestedRates.Samples));
                data.Add(new QS._core_e_.Data.Data2D("calculated", timeSeries_calculatedRates.Samples)); 
                return data;
            }
        }
#endif

        #endregion

        #region Adjusting rates

        private void Adjust()
        {
            if (controlled != null)
            {
                double time_now = clock.Time;
                if (calculatedRate < requestedRate || measuredRate == 0)
                    calculatedRate = requestedRate;
                else
                {
                    if (time_now > lastChanged + inertia)
                        calculatedRate = calculatedRate * (1 + growth * (Math.Min(Math.Min(requestedRate, measuredRate + maxadd) / measuredRate, maxzoom) - 1));
                }
                lastChanged = time_now;
#if DEBUG_AllowCollectingStatistics
                timeSeries_calculatedRates.Add(clock.Time, calculatedRate);
#endif
                controlled.MaximumRate = calculatedRate;
            }
        }

        #endregion

        #region IRateControlled Members

        double QS._core_c_.FlowControl3.IRateControlled.MaximumRate
        {
            get { return requestedRate; }
            set 
            {
                lock (this)
                {
                    requestedRate = value;
#if DEBUG_AllowCollectingStatistics
                    timeSeries_requestedRates.Add(clock.Time, requestedRate);
#endif
                    Adjust();
                }
            }
        }

        #endregion
    
        #region IRateAdjuster Members

        QS._core_c_.FlowControl3.IRateControlled IRateAdjuster.ControlledComponent
        {
            get { return controlled; }
            set
            {
                lock (this)
                {
                    controlled = value;
                    calculatedRate = requestedRate;
#if DEBUG_AllowCollectingStatistics
                    timeSeries_calculatedRates.Add(clock.Time, calculatedRate);
#endif
                    controlled.MaximumRate = calculatedRate;
                }
            }
        }

        double  IRateAdjuster.MeasuredRate
        {
            get { return measuredRate; }
            set 
	        {
                lock (this)
                {
                    measuredRate = value;
#if DEBUG_AllowCollectingStatistics
                    timeSeries_measuredRates.Add(clock.Time, measuredRate);
#endif
                    Adjust();
                }
            }
        }

        #endregion
    }
}
