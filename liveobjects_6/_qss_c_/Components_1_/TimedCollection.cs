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
	public class TimedCollection<C> : QS.Fx.Inspection.Inspectable, ISeqCollection<C> where C : class
	{
		public class Factory
		{
            public Factory(QS.Fx.Clock.IAlarmClock alarmClock, TimeSpan recyclingTimeout)
			{
				this.alarmClock = alarmClock;
				this.recyclingTimeout = recyclingTimeout;
			}

            private QS.Fx.Clock.IAlarmClock alarmClock;
			private TimeSpan recyclingTimeout;

			public TimeSpan RecyclingTimeout
			{
				get { return recyclingTimeout; }
				set { recyclingTimeout = value; }
			}

			public Base3_.Constructor<ISeqCollection<C>> Constructor
			{
				get { return new QS._qss_c_.Base3_.Constructor<ISeqCollection<C>>(createCollection); }
			}

			private ISeqCollection<C> createCollection()
			{
				return new TimedCollection<C>(alarmClock, null, recyclingTimeout);
			}
		}

        public TimedCollection(QS.Fx.Clock.IAlarmClock alarmClock, 
			SeqCollection<C>.Constructor constructorCallback, TimeSpan recyclingTimeout)
		{
			this.recyclingTimeout = recyclingTimeout;
			this.alarmClock = alarmClock;
			this.constructorCallback = constructorCallback;

			inspectableElementsProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<int, C>("Elements", elements,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<int, C>.ConversionCallback(Helpers_.FromString.Int32));
		}

        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IAlarm recyclingAlarm;
		private TimeSpan recyclingTimeout;
		private SeqCollection<C>.Constructor constructorCallback;
		private System.Collections.Generic.IDictionary<int, C> elements = new  System.Collections.Generic.Dictionary<int, C>();
		private System.Collections.Generic.List<int> inserted = new System.Collections.Generic.List<int>();
		private System.Collections.Generic.List<int> todelete = new System.Collections.Generic.List<int>();

		[QS.Fx.Base.Inspectable("Elements", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper1<int, C> inspectableElementsProxy;

        private void recyclingCallback(QS.Fx.Clock.IAlarm alarmRef)
		{
			lock (this)
			{
				foreach (int seqno in todelete)
					elements.Remove(seqno);
				todelete.Clear();

				if (inserted.Count > 0)
				{
					System.Collections.Generic.List<int> temp = todelete;
					todelete = inserted;
					inserted = temp;

					alarmRef.Reschedule();
				}
				else
					alarmRef = null;
			}
		}

		#region ISeqCollection<C> Members

		SeqCollection<C>.Constructor ISeqCollection<C>.ConstructorCallback
		{
			set { constructorCallback = value; }
		}

		C ISeqCollection<C>.lookup(int seqno)
		{
			lock (this)
			{
				if (elements.ContainsKey(seqno))
					return elements[seqno];
				else
				{
					C element = constructorCallback(seqno);
					elements[seqno] = element;

					inserted.Add(seqno);

					if (recyclingAlarm == null)
						recyclingAlarm = alarmClock.Schedule(
							recyclingTimeout.TotalSeconds, new QS.Fx.Clock.AlarmCallback(recyclingCallback), null);

					return element;
				}
			}
		}

		C ISeqCollection<C>.remove(int seqno)
		{
			lock (this)
			{
				if (elements.ContainsKey(seqno))
				{
					C element = elements[seqno];
					elements.Remove(seqno);
					return element;
				}
				else
					return null;
			}
		}

		#endregion
	}
}
