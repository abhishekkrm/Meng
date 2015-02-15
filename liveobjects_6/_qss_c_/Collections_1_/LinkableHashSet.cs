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
	/// Summary description for LinkableHashSet.
	/// </summary>
	public class LinkableHashSet : ILinkableHashSet
	{
		public LinkableHashSet(uint capacity)
		{
			buckets = new ILinkable[capacity];
			for (uint ind = 0; ind < buckets.Length; ind++)
				buckets[ind] = null;
			count = 0;
		}

		private ILinkable[] buckets;
		private uint count;

		public override string ToString()
		{
			string resultstr = null;
			foreach (ILinkable element in this.Elements)
				resultstr = ((resultstr != null) ? (resultstr + ",") : "") + element.ToString();
			return "{" + resultstr + "}";			
		}

		private void lookupBucketAndElement(object element, ref uint bucketno, ref ILinkable existingElement, ref ILinkable parent)
		{
			bucketno = ((uint) element.GetHashCode()) % ((uint) buckets.Length);
			parent = null;
			for (ILinkable existingGuy = buckets[bucketno]; existingGuy != null; existingGuy = existingGuy.Next)
			{
				if (existingGuy.Equals(element))
				{
					existingElement = existingGuy;
					return;
				}		
			
				parent = existingGuy;
			}
			existingElement = null;
		}

		private class Enumerator : System.Collections.IEnumerator
		{
			public Enumerator(LinkableHashSet associatedSet)
			{
				this.associatedSet = associatedSet;
				this.Reset();
			}

			private LinkableHashSet associatedSet;
			private int bucketNo;
			private ILinkable currentElement;

			#region IEnumerator Members

			public void Reset()
			{
				bucketNo = -1;
				currentElement = null;
			}

			public object Current
			{
				get
				{
					return currentElement.Contents;
				}
			}

			public bool MoveNext()
			{
				if (currentElement == null)
				{
					if (bucketNo > -1)
						return false;
				}
				else
					currentElement = currentElement.Next;

				while (currentElement == null && bucketNo < (associatedSet.buckets.Length - 1))
				{
					bucketNo++;
					currentElement = associatedSet.buckets[bucketNo];
				}

				return currentElement != null;
			}

			#endregion
		}

		#region System.Collections.ICollection Members

		public int Count
		{
			get
			{
				return (int) count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public void CopyTo(System.Array destinationArray, int destinationOffset)
		{
			uint index = (uint) destinationOffset;
			for (uint ind = 0; ind < buckets.Length; ind++)
			{
				for (ILinkable element = buckets[ind]; element != null; element = element.Next)
					destinationArray.SetValue(element.Contents, index++);	
			}
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region ILinkableHashSet Members

		public bool contains(object lookupKey)
		{
			return (lookup(lookupKey) != null);
		}

		public void insert(ILinkable element)
		{
			uint bucketno = 666;
			ILinkable existingGuy = null, parent = null;
			lookupBucketAndElement(element, ref bucketno, ref existingGuy, ref parent);
			if (existingGuy != null)
				throw new Exception("element already exists");

			element.Next = buckets[bucketno];
			buckets[bucketno] = element;

			count++;
		}

		public ILinkable lookup(object lookupKey)
		{
			uint bucketno = 666;
			ILinkable existingGuy = null, parent = null;
			lookupBucketAndElement(lookupKey, ref bucketno, ref existingGuy, ref parent);
			
			return existingGuy;
		}

		public ILinkable remove(object lookupKey)
		{
			uint bucketno = 666;
			ILinkable existingGuy = null, parent = null;
			lookupBucketAndElement(lookupKey, ref bucketno, ref existingGuy, ref parent);

			if (existingGuy != null)
			{
				if (parent != null)
					parent.Next = existingGuy.Next;
				else
					buckets[bucketno] = existingGuy.Next;

				count--;
			}

			return existingGuy;
		}

		public ILinkable[] Elements
		{
			get
			{
				ILinkable[] result = new ILinkable[count];
				uint index = 0;
				for (uint ind = 0; ind < buckets.Length; ind++)
				{
					for (ILinkable element = buckets[ind]; element != null; element = element.Next)
					{
						result[index++] = element;
					}
				}
				return result;
			}
		}

		public System.Array ToArray(System.Type typeOfArrayElements)
		{
			System.Array resultingArray = System.Array.CreateInstance(typeOfArrayElements, count);
			this.CopyTo(resultingArray, 0);
			return resultingArray;
		}

		#endregion
	}
}
