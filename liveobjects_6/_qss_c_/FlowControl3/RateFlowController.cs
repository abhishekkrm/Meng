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
	public class RateFlowController<C> : FlowController<C>, QS._core_c_.FlowControl3.IRateControlled where C : class
	{
		public RateFlowController(QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock,  
			double sending_rate, double upper_threshold, ReadyCallback<C> readyCallback) : base(readyCallback)
		{
			this.alarmClock = alarmClock;
			this.clock = clock;
			this.sending_rate = sending_rate;
			this.upper_threshold = upper_threshold;

			current_credit = upper_threshold;
			last_checked = clock.Time;
			alarmCallback = new QS.Fx.Clock.AlarmCallback(recheckingCallback);
		}

		private QS.Fx.Clock.IAlarmClock alarmClock;
		private QS.Fx.Clock.IClock clock;
		private double sending_rate, upper_threshold, current_credit, last_checked;
		private System.Collections.Generic.Queue<C> outgoingQueue = new Queue<C>();
		private QS.Fx.Clock.IAlarm alarmRef;
		private QS.Fx.Clock.AlarmCallback alarmCallback;

		#region IRateControlled Members

		double QS._core_c_.FlowControl3.IRateControlled.MaximumRate
		{
			get { return sending_rate; }
			set
			{
				IEnumerable<C> toSend;
				lock (this)
				{
					sending_rate = value;
					toSend = internal_processing(null);
				}
				if (toSend != null)
					readyCallback(toSend);
			}
		}

		#endregion

		#region Internal Processing

		private void recheckingCallback(QS.Fx.Clock.IAlarm alarmRef)
		{
			List<C> toSend;
			lock (this)
			{
				toSend = check();

				if (outgoingQueue.Count > 0)
					alarmRef.Reschedule(WaitingTime);
				else
					alarmRef = null;
			}
			if (toSend.Count > 0)
				readyCallback(toSend);
		}

		private double WaitingTime
		{
			get { return (outgoingQueue.Count < upper_threshold ? outgoingQueue.Count : upper_threshold) / sending_rate; }
		}

		private List<C> check()
		{
			double time_now = clock.Time;
			current_credit += (time_now - last_checked) * sending_rate;
			if (current_credit > upper_threshold)
				current_credit = upper_threshold;
			last_checked = time_now;

			List<C> toSend = new List<C>();
			while (current_credit >= 1 && outgoingQueue.Count > 0)
			{
				current_credit -= 1;
				toSend.Add(outgoingQueue.Dequeue());
			}
			return toSend;
		}

		private IEnumerable<C> internal_processing(C element)
		{
			List<C> toSend;
			lock (this)
			{
				if (element != null)
					outgoingQueue.Enqueue(element);

				toSend = check();

				if (outgoingQueue.Count > 0)
				{
					if (alarmRef == null)
						alarmRef = alarmClock.Schedule(WaitingTime, alarmCallback, null);
				}
				else
				{
					if (alarmRef != null)
					{
						alarmRef.Cancel();
						alarmRef = null;
					}
				}
			}
			return toSend.Count > 0 ? toSend : null;
		}

		#endregion

		#region IFlowController<C> Members

		public override void submit(C element, out IEnumerable<C> ready)
		{
			ready = internal_processing(element);
		}

		public override void release(out IEnumerable<C> ready)
		{
			ready = internal_processing(null);
		}

		public override void resubmit(C element, out IEnumerable<C> ready)
		{
			ready = internal_processing(element);
		}

		#endregion
	}
}
