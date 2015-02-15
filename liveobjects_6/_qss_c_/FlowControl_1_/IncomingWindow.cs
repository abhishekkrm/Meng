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

namespace QS._qss_c_.FlowControl_1_
{
	/// <summary>
	/// Summary description for IncomingWindow.
	/// </summary>
    [QS.Fx.Base.Inspectable]
	public class IncomingWindow : QS.Fx.Inspection.Inspectable, IIncomingWindow
	{
		public IncomingWindow(uint capacity)
		{
			this.capacity = capacity;
			this.slots = new object[capacity];
			for (int ind = 0; ind < capacity; ind++)
				slots[ind] = null;
			firstOccupiedSlot = 0;
			numberConsumed = numberConsumable = 0;
			firstOccupiedSeqNo = 1;
		}

		protected object[] slots;

		protected uint firstOccupiedSlot, numberConsumed, numberConsumable;
		protected uint firstOccupiedSeqNo;
		protected uint capacity; 

		public string AllToString
		{
			get
			{
				return makeString(true);
			}
		}

		public override string ToString()
		{
			return makeString(false);
		}

		private string makeString(bool objectDetails)
		{
			string result = "[ ";
			for (uint ind = 0; ind < capacity; ind++)
			{
				string slotstr = ((ind == firstOccupiedSlot) ? firstOccupiedSeqNo.ToString() : "") + 
					(((((ind + capacity) - firstOccupiedSlot) % capacity) < numberConsumed) ? "C" : "") +
					((slots[ind] != null) ? (objectDetails ? (slots[ind].ToString()) : "#") : "_") + " ";
				result = result + slotstr;
			}
			result = result + "]";
			return result;
		}

		public uint lastConsumedSeqNo()
		{
			return firstOccupiedSeqNo + numberConsumed - 1;
		}

		public uint lastConsumableSeqNo()
		{
			return firstOccupiedSeqNo + numberConsumable - 1;
		}

		public bool consumed(uint seqno)
		{
			return seqno < (firstOccupiedSeqNo + numberConsumed);
		}

		public bool accepts(uint seqno)
		{
			return seqno >= firstOccupiedSeqNo && 
				seqno < firstOccupiedSeqNo + capacity;
		}

		private uint locateSlot(uint seqno)
		{
			if (!accepts(seqno))
				throw new Exception("not accepted in this window");

			return (firstOccupiedSlot + seqno - firstOccupiedSeqNo) % capacity;
		}

		public object lookup(uint seqno)
		{
			return slots[locateSlot(seqno)];
		}

		public void insert(uint seqno, object item)
		{
            if (slots[locateSlot(seqno)] != null)
                throw new Exception("Cannot insert, the slot is not empty.");

			slots[locateSlot(seqno)] = item;
			update_numberConsumable();
		}

		private void update_numberConsumable()
		{
			while ((numberConsumable < capacity) && (slots[(firstOccupiedSlot + numberConsumable) % capacity] != null))
				numberConsumable++;
		}

		public bool ready()
		{
			// return (numberConsumed < capacity) && (slots[(firstOccupiedSlot + numberConsumed) % capacity] != null);
			return numberConsumed < numberConsumable;
		}

		public object first()
		{
			return slots[firstOccupiedSlot];
		}

		public object consume()
		{
			if (!ready())
				throw new Exception("cannot consume in this window");

			object result = this.NextConsumable;
			numberConsumed++;

			return result;
		}

		public object NextConsumable
		{
			get
			{
				return slots[(firstOccupiedSlot + numberConsumed) % capacity];
			}
		}

		public void remove(uint seqno)
		{
			try
			{
				if (!accepts(seqno))
					throw new Exception("there's no such slot in this window");

				uint slot_offset = seqno - firstOccupiedSeqNo;
				if (slot_offset > numberConsumed)
					throw new Exception("we cannot remove non-consumed slots");

				uint slotno = (firstOccupiedSlot + slot_offset) % capacity;
				slots[slotno] = null;

				while ((slots[firstOccupiedSlot] == null) && (numberConsumed > 0))
				{
					firstOccupiedSlot = (firstOccupiedSlot + 1) % capacity;
					firstOccupiedSeqNo++;
					numberConsumed--;
					numberConsumable--;
				}
			}
			catch (Exception exc)
			{
				throw new Exception("cannot remove " + seqno.ToString() + ",\nerror : " + 
					exc.ToString() + ",\nwindow : " + this.ToString() + "\n\n");
			}
		}

		public bool firstConsumed()
		{
			return numberConsumed > 0;
		}

		public void cleanupOneGuy()
		{
			remove(firstOccupiedSeqNo);
		}

		public System.Collections.ICollection selectObjects(AcceptObjectCallback callback)
		{
			System.Collections.ArrayList results = new System.Collections.ArrayList();
			for (uint slot_offset = 0; slot_offset < capacity; slot_offset++)
			{
				object obj = slots[(firstOccupiedSlot + slot_offset) % capacity];
				if (callback(obj))
					results.Add(obj);
			}
			return results;
		}
	}
}
