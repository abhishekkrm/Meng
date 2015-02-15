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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Threading;

namespace QS._qss_c_.Monitoring_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class Agent : Components_1_.ClockedObject, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		public Agent(string name) : base()
		{
            this.name = name;
            clock = QS._core_c_.Base2.PreciseClock.Clock;
		}

		[QS.Fx.Base.Inspectable("Currently Consumed", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private double currentConsumption = 0;
#if DEBUG_CollectStatistics
        [QS.CMS.Diagnostics.Component("Samples")]
		private Statistics.SamplesXY memoryConsumption = new QS.CMS.Statistics.SamplesXY();
#endif
        private QS.Fx.Clock.IClock clock;
        private string name;

		protected override void PeriodicWork()
		{
			currentConsumption = (double) GC.GetTotalMemory(true);
#if DEBUG_CollectStatistics
			memoryConsumption.addSample(clock.Time, currentConsumption);
#endif
		}

		#region TMS.Base.IStatisticsCollector Members

		System.Collections.Generic.IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get
			{
#if DEBUG_CollectStatistics
				return Helpers.ListOf<QS.CMS.Components.Attribute>.Singleton(
					new QS.CMS.Components.Attribute("Memory Consumed", memoryConsumption.DataSet));
#else
                return Helpers_.ListOf<QS._core_c_.Components.Attribute>.Nothing;
#endif
			}
		}

		#endregion
	}
}
