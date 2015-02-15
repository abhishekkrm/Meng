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

// ﻿#define DEBUG_LogEstimates

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class SmoothingEstimator : IEstimator, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		private const double default_smoothingFactor = 0.9;

        public SmoothingEstimator() : this(default_smoothingFactor)
        {
        }

		public SmoothingEstimator(double smoothingFactor)
		{
			this.smoothingFactor = smoothingFactor;
		}

		public SmoothingEstimator(double smoothingFactor, double initialValue) : this(smoothingFactor)
		{
			((IEstimator)this).Estimate = initialValue;
		}

		private double estimate, smoothingFactor;
		private bool initialized = false;

#if DEBUG_LogEstimates
        [QS.CMS.Diagnostics.Component("Logged Estimates")]
		private Statistics.Samples logged_estimates = new QS.CMS.Statistics.Samples();
#endif

		#region IEstimator Members

		void IEstimator.AddSample(double x)
		{
			lock (this)
			{
				if (initialized)
					estimate = estimate + (1 - smoothingFactor) * (x - estimate);
				else
				{
					estimate = x;
					initialized = true;
				}

#if DEBUG_LogEstimates
				logged_estimates.addSample(estimate);
#endif
			}
		}

		double IEstimator.Estimate
		{
			set 
			{
				lock (this)
				{
					estimate = value;
					initialized = true;

#if DEBUG_LogEstimates
					logged_estimates.addSample(estimate);
#endif
				}
			}

			get
			{
				if (!initialized)
					throw new Exception("Not initialized!");
				return estimate;
			}
		}

		#endregion

		public override string ToString()
		{
			return estimate.ToString();
		}

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
#if DEBUG_LogEstimates
				s.Add(new Components.Attribute("Estimates", logged_estimates.DataSet));
#endif
				return s;
			}
		}

		#endregion
	}
}
