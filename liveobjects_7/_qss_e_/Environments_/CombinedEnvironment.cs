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

namespace QS._qss_e_.Environments_
{
	/// <summary>
	/// Summary description for CombinedEnvironment.
	/// </summary>
	public class CombinedEnvironment : QS.Fx.Inspection.Inspectable, Runtime_.IEnvironment
	{
		public CombinedEnvironment(Runtime_.IEnvironment[] subenvironments, QS.Fx.Logging.ILogger logger) 
			: this(subenvironments, new QS._qss_c_.Base1_.PQAlarmClock(new QS._qss_c_.Collections_1_.BHeap(100, 2), logger), 
			QS._core_c_.Base2.PreciseClock.Clock)
		{
		}

		public CombinedEnvironment(Runtime_.IEnvironment[] subenvironments, 
			QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock)
		{
			this.clock = clock;
			this.alarmClock = alarmClock;

			subcomponents = new Management_.IManagedComponent[subenvironments.Length];
			subenvironments.CopyTo(subcomponents, 0);
		
			uint nnodes = 0;
			for (uint ind = 0; ind < subenvironments.Length; ind++)
				nnodes += (uint) subenvironments[ind].Nodes.Length;
			nodes = new Runtime_.INodeRef[nnodes];
			nnodes = 0;
			for (uint ind = 0; ind < subenvironments.Length; ind++)
			{
				for (uint nno = 0; nno < subenvironments[ind].Nodes.Length; nno++)
					nodes[nnodes++] = subenvironments[ind].Nodes[nno];
			}
		}

		private Runtime_.INodeRef[] nodes;
		private Management_.IManagedComponent[] subcomponents;
		private QS.Fx.Clock.IClock clock;
		private QS.Fx.Clock.IAlarmClock alarmClock;

		private bool disposeSubenvironments = false;

		public bool DisposeSubenvironments
		{
			set { disposeSubenvironments = value; }
		}

		#region IEnvironment Members

		public QS._qss_e_.Runtime_.INodeRef[] Nodes
		{
			get
			{
				return nodes;
			}
		}

		public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get
			{
				return alarmClock;
			}
		}

		public QS.Fx.Clock.IClock Clock
		{
			get
			{
				return clock;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (disposeSubenvironments)
				foreach (Management_.IManagedComponent subcomponent in subcomponents)
					((Runtime_.IEnvironment)subcomponent.Component).Dispose();
		}

		#endregion

		#region IManagedComponent Members

		public string Name
		{
			get
			{
				return "combined environment";
			}
		}

		public QS._qss_e_.Management_.IManagedComponent[] Subcomponents
		{
			get
			{
				return subcomponents;
			}
		}

		public QS._core_c_.Base.IOutputReader Log
		{
			get
			{
				return null;
			}
		}

		public object Component
		{
			get
			{
				return this;
			}
		}

		#endregion
	}
}
