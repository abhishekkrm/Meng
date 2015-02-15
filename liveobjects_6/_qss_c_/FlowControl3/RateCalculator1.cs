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

// #define DEBUG_LogSamples
// ﻿#define DEBUG_LogRates

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class RateCalculator1 : QS.Fx.Inspection.Inspectable, IRateCalculator, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		private const int default_windowSize = 100;

		public RateCalculator1() : this(QS._core_c_.Base2.PreciseClock.Clock, default_windowSize)
		{
		}

		public RateCalculator1(QS.Fx.Clock.IClock clock, int windowSize)
		{
			this.clock = clock;
			window = new double[this.windowSize = windowSize];
		}

		private QS.Fx.Clock.IClock clock;

		private double[] window;
		private int windowSize;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		private int nsamples;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		private double last_estimate;

#if DEBUG_LogRates
        [QS.CMS.Diagnostics.Component("Calculated Values")]
		private Statistics.Samples calculated_values = new QS.CMS.Statistics.Samples();
#endif
#if DEBUG_LogSamples
        [QS.CMS.Diagnostics.Component("Received Samples")]
		private Statistics.Samples received_samples = new QS.CMS.Statistics.Samples();
#endif

		#region IRateCalculator Members

		void IRateCalculator.sample()
		{
			lock (this)
			{
				double now = clock.Time;
				window[nsamples % windowSize] = now;
				nsamples++;

#if DEBUG_LogSamples
				received_samples.addSample(now);
#endif
			}
		}

		[QS.Fx.Base.Inspectable]
		double IRateCalculator.Rate
		{
			get 
			{ 
				lock (this)
				{
					double now = clock.Time;
					double oldest_sample = window[(nsamples < windowSize) ? 0 : (nsamples % windowSize)];
					int samples_used = nsamples < windowSize ? nsamples : windowSize;
					double current_estimate = (now - oldest_sample) / ((double) samples_used);

#if DEBUG_LogRates
					calculated_values.addSample(current_estimate);
#endif

					last_estimate = current_estimate;
					return current_estimate;
				}
			}
		}

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
				System.Collections.Generic.List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();  			
#if DEBUG_LogRates
				statistics.Add(new QS.CMS.Components.Attribute("Calculated Rates", calculated_values.DataSet));
#endif
#if DEBUG_LogSamples
				statistics.Add(new QS.CMS.Components.Attribute("Samples", received_samples.DataSet));
#endif				
				return statistics;
			}
		}

		#endregion
	}
}
