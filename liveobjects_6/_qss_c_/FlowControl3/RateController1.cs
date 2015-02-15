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

// ﻿#define DEBUG_LogReceiveRates

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl3
{
	public class RateController1 : QS.Fx.Inspection.Inspectable, IRateController, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		public RateController1(QS._core_c_.FlowControl3.IRateControlled controlledObject, FlowControl3.IEstimator estimator, double expansion_factor)
		{
			this.controlledObject = controlledObject;
			this.receiveRateEstimator = estimator;
			this.expansion_factor = expansion_factor;
		}

		private QS._core_c_.FlowControl3.IRateControlled controlledObject;
		private double expansion_factor;

#if DEBUG_LogReceiveRates
            [QS.CMS.Diagnostics.Component("Logged Receive Rates")]
            private QS.CMS.Statistics.Samples loggedReceiveRates = new QS.CMS.Statistics.Samples();
#endif

        [QS._core_c_.Diagnostics.Component("Receive Rate Estimate")]
		private FlowControl3.IEstimator receiveRateEstimator;

		#region IRateController Members

		QS._core_c_.FlowControl3.IRateControlled IRateController.ControlledObject
		{
			set { controlledObject = value; }
		}

		double IRateController.Rate
		{
			set 
			{
				double estimated_rate;
				lock (receiveRateEstimator)
				{
					receiveRateEstimator.AddSample(value);
					estimated_rate = receiveRateEstimator.Estimate;

#if DEBUG_LogReceiveRates
					loggedReceiveRates.addSample(value);
#endif
				}

				controlledObject.MaximumRate = estimated_rate * expansion_factor;
			}
		}

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
#if DEBUG_LogReceiveRates
				s.Add(new Components.Attribute("Receive Rates", loggedReceiveRates.DataSet));
#endif
				return s; 
			}
		}

		#endregion
	}
}
