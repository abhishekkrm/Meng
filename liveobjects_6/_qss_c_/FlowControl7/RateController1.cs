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

namespace QS._qss_c_.FlowControl7
{
    public class RateController1 : Base1_.IFactory<IRateController>
    {
        public RateController1(QS.Fx.Clock.IClock clock, double growthCoefficient)
        {
            this.clock = clock;
            this.growthCoefficient = growthCoefficient;
        }

        private QS.Fx.Clock.IClock clock;
        private double growthCoefficient;

        #region Class Controller

        [QS._core_c_.Diagnostics.ComponentContainer]
        [QS.Fx.Base.Inspectable]
        private class Controller : QS.Fx.Inspection.Inspectable, IRateController, QS._core_c_.FlowControl3.IRateControlled, QS._core_c_.Diagnostics2.IModule
        {
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            #region IModule Members

            QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
            {
                get { return diagnosticsContainer; }
            }

            #endregion

            public Controller(RateController1 owner)
            {
                this.owner = owner;
                rateAdjuster = new RateAdjuster(owner.clock, 1, 0.5, 200, 1.5);
                rateAdjuster.ControlledComponent = this;

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
            }

            private RateController1 owner;
            private double minimumRate = 10, maximumRate;
            [QS._core_c_.Diagnostics.Component]
            [QS._core_c_.Diagnostics2.Module("RateAdjuster")]
            private IRateAdjuster rateAdjuster;

            #region IRateController Members

            double IRateController.Calculate(double send_rate, double min_receive_rate, double avg_receive_rate, double max_receive_rate)
            {
                rateAdjuster.MaximumRate = min_receive_rate * (1 + owner.growthCoefficient);
                rateAdjuster.MeasuredRate = send_rate;
                return Math.Max(minimumRate, maximumRate);
            }

            #endregion

            #region IRateControlled Members

            double QS._core_c_.FlowControl3.IRateControlled.MaximumRate
            {
                get { return maximumRate; }
                set { maximumRate = value; }
            }

            #endregion
        }

        #endregion

        #region IFactory<IRateController> Members

        IRateController QS._qss_c_.Base1_.IFactory<IRateController>.Create()
        {
            return new Controller(this);
        }

        #endregion
    }
}
