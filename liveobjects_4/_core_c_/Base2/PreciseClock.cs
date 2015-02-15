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
using System.Runtime.InteropServices;

namespace QS._core_c_.Base2
{
	/// <summary>
	/// Summary description for PreciseClock.
	/// </summary>
	public class PreciseClock : QS.Fx.Inspection.Inspectable, QS.Fx.Clock.IClock	
	{
/*
		[DllImport("Kernel32.dll")]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);
*/

        public static QS.Fx.Clock.IClock Clock
		{
			get
			{
				return preciseClock;
			}
		}

        private readonly static QS.Fx.Clock.IClock preciseClock = new PreciseClock(); // QS.CMS.Base3.Clock.SystemClock;

		private PreciseClock()
		{
/*
			if (!QueryPerformanceFrequency(out frequency))
				throw new Exception("no supported");

			QueryPerformanceCounter(out initial_timestamp);
*/ 
		}

/*
		private long frequency, initial_timestamp;
        private double adjustment = 0;
*/
        private int startticks = System.Environment.TickCount;        

		#region IClock Members

		[QS.Fx.Base.Inspectable]
		public double Time
		{
			get
			{
                return ((double)(System.Environment.TickCount - startticks)) * 0.001;
                
/*
				long current_timestamp;
				QueryPerformanceCounter(out current_timestamp);

				return ((double) (current_timestamp - initial_timestamp) / (double) frequency) + adjustment;
*/ 
			}
		}

		#endregion
	}
}
