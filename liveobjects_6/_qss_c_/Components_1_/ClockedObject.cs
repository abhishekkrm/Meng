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

namespace QS._qss_c_.Components_1_
{
	public interface IClockedObject : IThreadedObject
	{
		double Interval
		{
			get;
			set;
		}
	}

	public abstract class ClockedObject : ThreadedObject, IClockedObject
	{
		public const double DefaultInterval = 30.0;

		public ClockedObject() : this(DefaultInterval)
		{
		}

		public ClockedObject(double interval) : this(interval, true)
		{
		}

		public ClockedObject(double interval, bool startup) 
			: this(interval, startup, new Base3_.Logger(QS._core_c_.Base2.PreciseClock.Clock))
		{
		}

		public ClockedObject(double interval, bool startup, QS.Fx.Logging.ILogger logger)
		{
			this.interval = interval;
			this.logger = logger;
			if (startup)
				((IThreadedObject)this).Start();
		}

		[QS.Fx.Base.Inspectable("Interval", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private double interval;
		[QS.Fx.Base.Inspectable("Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS.Fx.Logging.ILogger logger;

		#region IClockedObject Members

		double IClockedObject.Interval
		{
			get { return interval; }
			set { interval = value; }
		}

		#endregion

		protected abstract void PeriodicWork();

		protected override void Work()
		{
			while (!((Components_1_.IThreadedObject)this).IsCompleted)
			{
				try
				{
					PeriodicWork();
				}
				catch (Exception exc)
				{
					if (logger != null)
						logger.Log(this, exc.ToString());
				}
				((Components_1_.IThreadedObject)this).Completed.WaitOne(TimeSpan.FromSeconds(interval), false);
			}
		}
	}
}
