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

// #define STATISTICS_TrackNumberOfEntries

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl2
{
	public class OutgoingController<C> : QS.Fx.Inspection.Inspectable, IOutgoingController<C> where C : class
	{
		public OutgoingController(int initialCapacity, int windowSize, ReadyCallback<C> readyCallback, QS.Fx.Clock.IClock clock)
		{
            this.clock = clock;
			buffer = new C[this.bufferSize = initialCapacity];
			for (int ind = 0; ind < initialCapacity; ind++)
				buffer[ind] = default(C);
			this.windowSize = windowSize;
			this.readyCallback = readyCallback;
			pending = new Queue<C>();

			firstusedSeqNo = availableSeqNo = 1;
			nslotsOccupied = 0;

			inspectableBuffer = new QS._qss_e_.Inspection_.Array("Buffer", buffer);
		}

		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)] private int bufferSize;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)] private int windowSize;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)] private int firstusedSeqNo;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)] private int availableSeqNo;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)] private int nslotsOccupied;

		[QS.Fx.Base.Inspectable("Buffer_AsInspectableArray", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.Array inspectableBuffer;

        private QS.Fx.Clock.IClock clock;
#if STATISTICS_TrackNumberOfEntries
        [TMS.Inspection.Inspectable]
        private QS.CMS.Statistics.SamplesXY timeSeries_numberOfSlotsOccupied = new QS.CMS.Statistics.SamplesXY();
#endif

		[QS.Fx.Base.Inspectable]
		public string Buffer_AsString
		{
			get
			{
				StringBuilder s = new StringBuilder();
				for (int ind = 0; ind < bufferSize; ind++)
				{
					s.Append((buffer[ind] == default(C)) ? "." : "*");
					if ((ind % 100) == 0)
						s.AppendLine();
				}
				return s.ToString();
			}
		}

		[QS.Fx.Base.Inspectable]
		public C EarliestPending
		{
			get { return (nslotsOccupied > 0) ? buffer[firstusedSeqNo % bufferSize] : null; }
		}

		private C[] buffer;
		private ReadyCallback<C> readyCallback;
		private Queue<C> pending;

		private System.Collections.Generic.IEnumerable<Base3_.Seq<C>> CheckQueue
		{
			get
			{
				System.Collections.Generic.List<Base3_.Seq<C>> ready_list = new List<QS._qss_c_.Base3_.Seq<C>>();
				while (pending.Count > 0 && nslotsOccupied < windowSize && (availableSeqNo - firstusedSeqNo) < bufferSize)
				{
					C element = pending.Dequeue();
					int seqno = availableSeqNo++;
					buffer[seqno % bufferSize] = element;
					nslotsOccupied++;
					ready_list.Add(new Base3_.Seq<C>(element, seqno));
				}

#if STATISTICS_TrackNumberOfEntries
                timeSeries_numberOfSlotsOccupied.addSample(clock.Time, nslotsOccupied);
#endif

				return ready_list;
			}
		}

		#region IOutgoingController<C> Members

		/// <summary>
		/// Concurrency level, i.e. the number of requests currently being processed, normally smaller than window size (unless window was recently shrinked).
		/// </summary>
		/// <value></value>
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		public int Concurrency
		{
			get { return nslotsOccupied; }
		}

		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		public int WindowSize
		{
			get { return windowSize; }
			set 
			{
				lock (this)
				{
					if (value > bufferSize)
					{
						// throw new ArgumentException("Window size is larger than buffer size.");
						windowSize = bufferSize;
					}
					else
						windowSize = (value > 0) ? value : 1;

					readyCallback(this.CheckQueue);
				}
			}
		}

		public ReadyCallback<C> ReadyCallback
		{
			set { readyCallback = value; }
		}

		public void schedule(C element)
		{
			System.Nullable<Base3_.Seq<C>> ready;
			schedule(element, out ready);
			if (ready.HasValue)
				readyCallback(new Base3_.Seq<C>[] { ready.Value });
		}

		public void schedule(C element, out System.Nullable<Base3_.Seq<C>> ready)
		{
			lock (this)
			{
				if (nslotsOccupied < windowSize && (availableSeqNo - firstusedSeqNo) < bufferSize)
				{
					int seqno = availableSeqNo++;
					buffer[seqno % bufferSize] = element;
					nslotsOccupied++;

#if STATISTICS_TrackNumberOfEntries
                    timeSeries_numberOfSlotsOccupied.addSample(clock.Time, nslotsOccupied);
#endif

					ready = new Base3_.Seq<C>(element, seqno);
				}
				else
				{
					pending.Enqueue(element);
					ready = null;
				}
			}
		}

		public C removeCompleted(int seqno, out System.Collections.Generic.IEnumerable<Base3_.Seq<C>> ready)
		{
			C result = default(C);

			lock (this)
			{
				if (seqno < firstusedSeqNo || seqno >= availableSeqNo)
					throw new ArgumentException("Wrong seqno: accepted range is " + firstusedSeqNo.ToString() + "-" + (availableSeqNo - 1).ToString());

				int slotno = seqno % bufferSize;
				result = buffer[slotno];
				buffer[slotno] = default(C);

				nslotsOccupied--;
				while (firstusedSeqNo < availableSeqNo && buffer[firstusedSeqNo % bufferSize] == default(C))
					firstusedSeqNo++;

				ready = this.CheckQueue;
			}

			return result;
		}

		public C removeCompleted(int seqno)
		{
			System.Collections.Generic.IEnumerable<Base3_.Seq<C>> ready;
			C result = removeCompleted(seqno, out ready);
			readyCallback(ready);
			return result;
		}

		#endregion
	}
}
