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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl3
{
	public class RateCalculator2 : QS.Fx.Inspection.Inspectable, IRateCalculator, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		private const double default_smoothing_factor = 0.99;

		public RateCalculator2()
		{
		}

		private double last_time = -1, smoothed_interval, smoothing_factor = default_smoothing_factor;
		private static QS.Fx.Clock.IClock clock = QS._core_c_.Base2.PreciseClock.Clock;

		#region IRateCalculator Members

		void IRateCalculator.sample()
		{
			lock (this)
			{
				double now = clock.Time;
				if (last_time >= 0)
				{
					double new_interval = now - last_time;
					smoothed_interval = smoothing_factor * smoothed_interval + (1 - smoothing_factor) * new_interval;
				}
				last_time = now;
			}
		}

		double IRateCalculator.Rate
		{
			get { return (smoothed_interval > 0) ? (1 / smoothed_interval) : 0; }
		}

		#endregion

		#region IStatisticsCollector Members
		
		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get { return new List<QS._core_c_.Components.Attribute>(); }
		}

		#endregion
	}
}
