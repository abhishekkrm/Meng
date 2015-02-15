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
	/// Summary description for OutgoingWindow.
	/// </summary>
	public class OutgoingWindow : QS.Fx.Inspection.Inspectable, IOutgoingWindow, System.Collections.IEnumerable
	{
		public OutgoingWindow(uint capacity)
		{
			this.capacity = capacity;
			this.slots = new object[capacity];
			for (int ind = 0; ind < capacity; ind++)
				slots[ind] = null;
			this.numberOfOccupiedSlots = 0;
			this.nextAvailableSeqNo = 1;
		}

		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected object[] slots;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint firstOccupiedSlot;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint lastOccupiedSlot;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint firstOccupiedSeqNo;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint nextAvailableSeqNo;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint capacity;
		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected uint numberOfOccupiedSlots;

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int ind = 0; ind < numberOfOccupiedSlots; ind++)
            {
                object obj = slots[(firstOccupiedSlot + ind) % capacity];
                if (obj != null)
                    yield return obj;
            }
        }

        #endregion

        public uint NextAvailableSeqNo
		{
			get
			{
				return nextAvailableSeqNo;
			}
		}

		[QS.Fx.Base.Inspectable]
		public string AllToString
		{
			get { return makeString(true); }
		}

		[QS.Fx.Base.Inspectable]
		public string _ToString
		{
			get { return makeString(false); }
		}

		public override string ToString()
		{
			return makeString(false);
		}

		private string makeString(bool elementDetails)
		{
			string result = "[ ";
			for (uint ind = 0; ind < capacity; ind++)
			{
				string elemstr = (slots[ind] != null) ? (elementDetails ? (slots[ind].ToString()) : "#") : "_";
				string slotstr = ((numberOfOccupiedSlots > 0) ? ((ind == firstOccupiedSlot) ? (">" + firstOccupiedSeqNo.ToString() + ":" + elemstr) : 
					((ind == lastOccupiedSlot) ? (elemstr + ":" + (firstOccupiedSeqNo + numberOfOccupiedSlots - 1).ToString() + "<") : elemstr)) : elemstr);
				result = result + slotstr + " ";
			}
			result = result + "; next = " + nextAvailableSeqNo + " ]";
			return result;
		}

		public void removeOldest()
		{
			remove(firstOccupiedSeqNo);
		}

		public object oldest()
		{
            if (numberOfOccupiedSlots > 0)
                return slots[firstOccupiedSlot];
            else
                return null; // throw new Exception("window is empty");
		}

		public bool hasSpace()
		{
			return numberOfOccupiedSlots < capacity;
		}

		public uint append(object item)
		{
			if (numberOfOccupiedSlots < capacity)
			{
				uint seqno = nextAvailableSeqNo++;
				if (numberOfOccupiedSlots > 0)
				{
					lastOccupiedSlot = (lastOccupiedSlot + 1) % capacity;
				}
				else
				{
					firstOccupiedSlot = lastOccupiedSlot = 0;
					firstOccupiedSeqNo = seqno;
				}
				slots[lastOccupiedSlot] = item;
				numberOfOccupiedSlots++;
				return seqno;
			}
			else
				throw new Exception("Not enough space in this window!");
		}

		protected bool locateSlot(uint seqno, ref uint slotno)
		{
			if (numberOfOccupiedSlots > 0)
			{
				if (seqno < firstOccupiedSeqNo)
					return false;
				else
				{
					uint slot_offset = seqno - firstOccupiedSeqNo;
					if (slot_offset < numberOfOccupiedSlots)
					{
						slotno = (firstOccupiedSlot + slot_offset) % capacity;
						return true;
					}
					else
						return false;
				}
			}
			else
				return false;
		}

		public object lookup(uint seqno)
		{
			uint slotno = 0; // stupid compiler generates warnings
			return locateSlot(seqno, ref slotno) ? slots[slotno] : null;
		}

		public object remove(uint seqno)
		{
			uint slotno = 0; // stupid compiler generates warnings
			if (locateSlot(seqno, ref slotno))
			{
				object item = slots[slotno];
				slots[slotno] = null;

				while ((numberOfOccupiedSlots > 0) && (slots[firstOccupiedSlot] == null))
				{
					firstOccupiedSlot = (firstOccupiedSlot + 1) % capacity;
					firstOccupiedSeqNo++;
					numberOfOccupiedSlots--;
				}

				return item;
			}
			else
				return null;
		}

		public uint Size
		{
			get
			{
				return capacity;
			}
		}

		public uint Used
		{
			get
			{
				return numberOfOccupiedSlots;
			}
		}

		public uint Free
		{
			get
			{
				return capacity - numberOfOccupiedSlots;
			}
		}
    }
}
