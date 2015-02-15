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

#define Using_QuickSilver_C
// #define DEBUG_LogAppDomains

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

/*
namespace QS.CMS.Base3
{
#if Using_QuickSilver_C

    [TMS.Inspection.Inspectable]
	public class Clock : TMS.Inspection.Inspectable, QS.Fx.QS.Fx.Clock.IClock
	{
#if DEBUG_LogAppDomains
        static Clock()
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter("C:\\.QuickSilver\\appdomains.log"))
                {
                    writer.WriteLine(DateTime.Now.ToString() + "\t" + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + ":" +
                        System.Diagnostics.Process.GetCurrentProcess().ProcessName + "\t" + System.AppDomain.CurrentDomain.Id.ToString() + ":" +
                            System.AppDomain.CurrentDomain.FriendlyName);
                }
            }
            catch (Exception)
            {
            }
        }
#endif

        public static readonly QS.Fx.QS.Fx.Clock.IClock SharedClock = Base2.PreciseClock.Clock; // new Clock(new CPUClock());

        private Clock(CPUClock cpuClock)
		{
            this.cpuClock = cpuClock;
		}

        private CPUClock cpuClock;

		#region IClock Members

		[TMS.Inspection.Inspectable]
		public double Time
		{
			get { return cpuClock.Time; }
		}

		#endregion
	}

#else

	public static class Clock // : TMS.Inspection.Inspectable, QS.Fx.QS.Fx.Clock.IClock
	{
		public static double Time
		{
			get { return SystemClock.Time; }
		}

		public static readonly QS.Fx.QS.Fx.Clock.IClock SystemClock =
			QS.CMS.Base2.PreciseClock.Clock;
	}

#endif
}
*/
