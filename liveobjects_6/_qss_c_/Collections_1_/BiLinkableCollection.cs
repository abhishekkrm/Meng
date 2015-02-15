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

namespace QS._qss_c_.Collections_1_
{
	/// <summary>
	/// Summary description for RawCollection.
	/// </summary>
	public class BiLinkableCollection : IBiLinkableCollection
	{
		public BiLinkableCollection()
		{
			first = last = null;
			count = 0;
		}

		private IBiLinkable first, last;
		private uint count;

		#region IBiLinkableCollection Members

		public int Count
		{
			get
			{
				return (int) count;
			}
		}

		public void insertAtHead(IBiLinkable element)
		{
			element.NextBiLinkable = first;
			element.PrevBiLinkable = null;

			if (first != null)
				first.PrevBiLinkable = element;
			else
				last = element;

			first = element;
			count++;
		}

		public void insertAtTail(IBiLinkable element)
		{
			element.PrevBiLinkable = last;
			element.NextBiLinkable = null;

			if (last != null)
				last.NextBiLinkable = element;
			else
				first = element;

			last = element;
			count++;
		}

		public void remove(IBiLinkable element)
		{
			if (element.PrevBiLinkable != null)
				element.PrevBiLinkable.NextBiLinkable = element.NextBiLinkable;
			else
				first = element.NextBiLinkable;

			if (element.NextBiLinkable != null)
				element.NextBiLinkable.PrevBiLinkable = element.PrevBiLinkable;
			else
				last = element.PrevBiLinkable;

			count--;
		}

		public IBiLinkable elementAtHead()
		{
			return first;
		}

		public IBiLinkable elementAtTail()
		{
			return last;
		}

		public IBiLinkable[] Elements
		{
			get
			{
				IBiLinkable[] resultArray = new IBiLinkable[count];
				uint ind = 0;
				for (IBiLinkable element = first; element != null; element = element.NextBiLinkable)
					resultArray[ind++] = element;
				return resultArray;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return (int) count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			for (IBiLinkable curr = first; curr != null; curr = curr.NextBiLinkable)
				array.SetValue(curr.Contents, index++);
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region Class Enumerator

		private class Enumerator : System.Collections.IEnumerator
		{
			public Enumerator(BiLinkableCollection collection)
			{
				this.collection = collection;
			}

			private BiLinkableCollection collection;
			private IBiLinkable current = null;
			private bool positionedAtTheHead = true;

			#region IEnumerator Members

			public void Reset()
			{
				current = null;
				positionedAtTheHead = true;
			}

			public object Current
			{
				get
				{
					return current;
				}
			}

			public bool MoveNext()
			{
				if (positionedAtTheHead)
				{
					current = collection.first;
					positionedAtTheHead = false;
				}
				else if (current != null)
				{
					current = current.NextBiLinkable;
				}
				else
					return false;

				return current != null;
			}

			#endregion
		}

		#endregion
	}
}
