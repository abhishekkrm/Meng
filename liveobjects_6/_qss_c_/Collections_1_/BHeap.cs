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

namespace QS._qss_c_.Collections_1_
{
	public class BHeap<C> : IPriorityQueue<C> /* , ISet<C> */ where C : System.IComparable<C>
	{
		public BHeap(uint initialCapacity, double growthFactor)
		{
			currentCapacity = initialCapacity;
			capacityGrowthFactor = growthFactor;
			numberOfElements = 0;
			elements = new C[currentCapacity];
		}

		private double capacityGrowthFactor;
		private uint currentCapacity, numberOfElements;
		private C[] elements;

		#region Internal Operations

		private void heapify()
		{
			int i = 0;
			while (true)
			{
				int l = 2 * i + 1;
				int r = 2 * i + 2;

				int nexti =
					(l < numberOfElements && elements[l].CompareTo(elements[i]) < 0)
					? l : i;

				if (r < numberOfElements
					&& elements[r].CompareTo(elements[nexti]) < 0)
					nexti = r;

				if (nexti == i)
					break;

				C temp = elements[i];
				elements[i] = elements[nexti];
				elements[nexti] = temp;

				i = nexti;
			}
		}

		private void increaseSize(uint increaseByHowMuch)
		{
			C[] newelements = new C[currentCapacity + increaseByHowMuch];
			System.Array.Copy(elements, 0, newelements, 0, numberOfElements);
			currentCapacity += increaseByHowMuch;
			elements = newelements;
		}

		private void insert(C o)
		{
			if (numberOfElements == currentCapacity - 1)
				increaseSize((uint)(currentCapacity * capacityGrowthFactor));

			uint position = numberOfElements++;
			while (position > 0)
			{
				uint itsparent = (position - 1) / 2;
				if (elements[itsparent].CompareTo(o) > 0)
				{
					elements[position] = elements[itsparent];
					position = itsparent;
				}
				else
					break;
			}

			elements[position] = o;
		}

		#endregion

		#region IPriorityQueue<C> Members

		void IPriorityQueue<C>.insert(C o)
		{
			insert(o);
		}

		C IPriorityQueue<C>.findmin()
		{
			if (numberOfElements > 0)
				return elements[0];
			else
				throw new System.Exception();
		}

		C IPriorityQueue<C>.deletemin()
		{
			C result;
			if (numberOfElements > 0)
				result = elements[0];
			else
				throw new System.Exception();

			numberOfElements--;
			elements[0] = elements[numberOfElements];
			elements[numberOfElements] = default(C);

			heapify();

			return result;
		}

		uint IPriorityQueue<C>.size()
		{
			return numberOfElements;
		}

		bool IPriorityQueue<C>.isempty()
		{
			return numberOfElements == 0;
		}

		void IPriorityQueue<C>.Clear()
		{
			while (numberOfElements > 0)
				elements[--numberOfElements] = default(C);
		}

		#endregion

/*
		#region ISet<C> Members

		void ISet<C>.Insert(C element)
		{
			insert(element);
		}

		void ISet<C>.Remove(C element)
		{
		}

		bool ISet<C>.Contains(C element)
		{
			throw new System.NotImplementedException();
		}

		#endregion
*/
	}

	public class BHeap : IPriorityQueue, QS.Fx.Inspection.IAttributeCollection, System.Collections.ICollection
	{
		public BHeap(uint initialCapacity, double growthFactor)
		{
			currentCapacity = initialCapacity;
			capacityGrowthFactor = growthFactor;
			numberOfElements = 0;
			elements = new System.IComparable [currentCapacity];
		}

		public class Exception : System.Exception
		{
			public Exception() : base("BHeap: Empty!")
			{
			}
		}

		public void Clear()
		{
			while (numberOfElements > 0)
				elements[--numberOfElements] = null;
		}

		private double capacityGrowthFactor;
		private uint currentCapacity, numberOfElements;
		private System.IComparable [] elements;
		
		private void heapify()
		{			
			int i = 0;
			while (true)
			{
				int l = 2 * i + 1;
				int r = 2 * i + 2;

				int nexti = 
					(l < numberOfElements && elements[l].CompareTo(elements[i]) < 0)
					? l : i;

				if (r < numberOfElements
					&& elements[r].CompareTo(elements[nexti]) < 0)
					nexti = r;

				if (nexti == i)
					break;

				System.IComparable temp = elements[i];
				elements[i] = elements[nexti];
				elements[nexti] = temp;

				i = nexti;
			}
		}
		
		private void increaseSize(uint increaseByHowMuch)
		{
			System.IComparable [] newelements = 
				new System.IComparable[currentCapacity + increaseByHowMuch];
			System.Array.Copy(elements, 0, newelements, 0, numberOfElements);
			currentCapacity += increaseByHowMuch;
			elements = newelements;
		}

		#region IPriorityQueue Members

		public void insert(System.IComparable o)
		{
			if (numberOfElements == currentCapacity - 1)
				increaseSize((uint) (currentCapacity * capacityGrowthFactor));

			uint position = numberOfElements++; 
			while (position > 0)
			{
				uint itsparent = (position - 1) / 2;
				if (elements[itsparent].CompareTo(o) > 0)
				{
					elements[position] = elements[itsparent];
					position = itsparent;
				}
				else
					break;
			}

			elements[position] = o;
		}

		public System.IComparable findmin()
		{
			if (numberOfElements > 0)
				return elements[0];
			else
				throw new Exception();
		}

		public System.IComparable deletemin()
		{
			System.IComparable result;
			if (numberOfElements > 0)
				result = elements[0];
			else
				throw new Exception();

			numberOfElements--;
			elements[0] = elements[numberOfElements];
			elements[numberOfElements] = null;
			
			heapify();

			return result;
		}

		public uint size()
		{
			return numberOfElements;
		}

		public bool isempty()
		{
			return numberOfElements == 0;
		}

		#endregion

		#region IAttributeCollection Members

		System.Collections.Generic.IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
		{
			get 
			{
				for (int ind = 0; ind < numberOfElements; ind++)
					yield return ind.ToString();
			}
		}

		private System.Collections.Generic.List<System.IComparable> SortedElements
		{
			get
			{
				System.Collections.Generic.List<System.IComparable> result = new System.Collections.Generic.List<System.IComparable>();
				for (int ind = 0; ind < numberOfElements; ind++)
					result.Add(elements[ind]);
				result.Sort();
				return result;
			}
		}

		QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
		{
			get 
			{ 
				int ind = System.Convert.ToInt32(attributeName);
				return new QS.Fx.Inspection.ScalarAttribute(attributeName, 
					(((uint) ind) < numberOfElements) ? SortedElements[ind] : null);
			}
		}

		#endregion

		#region IAttribute Members

		string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "Priority Queue"; }
		}

		QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			throw new System.NotImplementedException();
		}

		int System.Collections.ICollection.Count
		{
			get { return (int) numberOfElements; }
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get { return false; }
		}

		object System.Collections.ICollection.SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
