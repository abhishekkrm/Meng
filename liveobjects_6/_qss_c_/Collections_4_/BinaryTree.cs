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

namespace QS._qss_c_.Collections_4_
{
	public class BinaryTree<C> : Collections_1_.IPriorityQueue<C>, ICollection<C> where C : System.IComparable
	{
		public BinaryTree()
		{
		}

		protected Element root, minimum;
		protected int count;

		#region Internal Processing

		protected Element Insert(C dataObject)
		{
			if (root == null)
			{
				count = 1;
				return (root = minimum = new BinaryTree<C>.Element(dataObject, null, null, null));
			}
			else
			{
				count++;
				Element parent = root;
				while (true)
				{
					switch (dataObject.CompareTo(parent.Object))
					{
						case -1:
							{
								if (parent.Left != null)
									parent = parent.Left;
								else
								{
									BinaryTree<C>.Element new_element = new BinaryTree<C>.Element(dataObject, null, null, parent);
									parent.Left = new_element;
									if (ReferenceEquals(minimum, parent))
										minimum = new_element;
									return new_element;
								}
							}
							break;

						case 0:
//							{
//								count--;
//								throw new Exception("Element aready exists in the collection.");
//							}

						case +1:
							{
								if (parent.Right != null)
									parent = parent.Right;
								else
									return (parent.Right = new BinaryTree<C>.Element(dataObject, null, null, parent));
							}
							break;
					}
				}
			}
		}

		protected Element DeleteMin()
		{
			if (count == 0)
				throw new Exception("Colection is empty.");

			count--;
			Element element = minimum;
			if (ReferenceEquals(root, element))
			{
				root = minimum = element.Right;
				if (element.Right != null)
				{
					element.Right.Parent = null;
					while (minimum.Left != null)
						minimum = minimum.Left;
				}
			}
			else
			{
				element.Parent.Left = element.Right;
				if (element.Right != null)
				{
					element.Right.Parent = element.Parent;
					minimum = element.Right;
					while (minimum.Left != null)
						minimum = minimum.Left;
				}
				else
				{
					minimum = element.Parent;
				}
			}

			return element;
		}

		protected void Clear()
		{
			root = minimum = null;
			count = 0;
		}

		#endregion

		#region Class Element

		protected class Element
		{
			public Element(C obj, Element left, Element right, Element parent)
			{
				this.Object = obj;
				this.Left = left;
				this.Right = right;
				this.Parent = parent;
			}

			public Element Left, Right, Parent;
			public C Object;
		}

		#endregion

		#region IPriorityQueue Members

		void QS._qss_c_.Collections_1_.IPriorityQueue<C>.insert(C o)
		{
			Insert(o);
		}

		C QS._qss_c_.Collections_1_.IPriorityQueue<C>.findmin()
		{
			if (minimum == null)
				throw new Exception("Collection is empty.");
			return minimum.Object;
		}

		C QS._qss_c_.Collections_1_.IPriorityQueue<C>.deletemin()
		{
			return DeleteMin().Object;
		}

		uint QS._qss_c_.Collections_1_.IPriorityQueue<C>.size()
		{
			return (uint) count;
		}

		bool QS._qss_c_.Collections_1_.IPriorityQueue<C>.isempty()
		{
			return count == 0;
		}

		void QS._qss_c_.Collections_1_.IPriorityQueue<C>.Clear()
		{
			Clear();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<C> Members

		private enum ComingFrom
		{
			PARENT, LEFT_CHILD, RIGHT_CHILD
		}

		// browsing the tree in the in-order fashion
		IEnumerator<C> IEnumerable<C>.GetEnumerator()
		{
			Element current_element = root;
			ComingFrom coming_from = ComingFrom.PARENT;

			while (current_element != null)
			{
				switch (coming_from)
				{
					case ComingFrom.PARENT:
					{
						if (current_element.Left != null)
						{
							current_element = current_element.Left;
							coming_from = ComingFrom.PARENT;
						}
						else
						{
							coming_from = ComingFrom.LEFT_CHILD;
						}
					}
					break;

					case ComingFrom.LEFT_CHILD:
					{
						yield return current_element.Object;
						if (current_element.Right != null)
						{
							current_element = current_element.Right;
							coming_from = ComingFrom.PARENT;
						}
						else
						{
							coming_from = ComingFrom.RIGHT_CHILD;
						}
					}
					break;

					case ComingFrom.RIGHT_CHILD:
					{
						if (current_element.Parent == null)
						{
							current_element = null;
						}
						else
						{
							if (ReferenceEquals(current_element, current_element.Parent.Left))
								coming_from = ComingFrom.LEFT_CHILD;
							else if (ReferenceEquals(current_element, current_element.Parent.Right))
								coming_from = ComingFrom.RIGHT_CHILD;
							else
								throw new Exception("Internal error, tree has been changed during enumeration.");

							current_element = current_element.Parent;
						}
					}
					break;
				}
			}
		}

		#endregion

		#region ICollection<C> Members

		void ICollection<C>.Add(C item)
		{
			Insert(item);
		}

		void ICollection<C>.Clear()
		{
			Clear();
		}

		bool ICollection<C>.Contains(C item)
		{
			throw new NotImplementedException();
		}

		void ICollection<C>.CopyTo(C[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		int ICollection<C>.Count
		{
			get { return count; }
		}

		bool ICollection<C>.IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<C>.Remove(C item)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class BinaryTree 
		: BinaryTree<System.IComparable>, Collections_1_.IPriorityQueue, System.Collections.ICollection

	{
		public BinaryTree()
		{
		}

		#region IPriorityQueue Members

		void QS._qss_c_.Collections_1_.IPriorityQueue.insert(IComparable o)
		{
			((Collections_1_.IPriorityQueue<IComparable>)this).insert(o);
		}

		IComparable QS._qss_c_.Collections_1_.IPriorityQueue.findmin()
		{
			return ((Collections_1_.IPriorityQueue<IComparable>)this).findmin();
		}

		IComparable QS._qss_c_.Collections_1_.IPriorityQueue.deletemin()
		{
			return ((Collections_1_.IPriorityQueue<IComparable>)this).deletemin();
		}

		uint QS._qss_c_.Collections_1_.IPriorityQueue.size()
		{
			return ((Collections_1_.IPriorityQueue<IComparable>)this).size();
		}

		bool QS._qss_c_.Collections_1_.IPriorityQueue.isempty()
		{
			return ((Collections_1_.IPriorityQueue<IComparable>)this).isempty();
		}

		void QS._qss_c_.Collections_1_.IPriorityQueue.Clear()
		{
			((Collections_1_.IPriorityQueue<IComparable>)this).Clear();
		}

		#endregion

		#region Tests

		public static void Test1()
		{
			System.Random r = new System.Random();

			QS._qss_c_.Collections_4_.BinaryTree t = new QS._qss_c_.Collections_4_.BinaryTree();
			for (int ind = 0; ind < 100000; ind++)
			{
				((QS._qss_c_.Collections_1_.IPriorityQueue)t).insert(r.NextDouble());
			}

			double x = -1;
			while (!((QS._qss_c_.Collections_1_.IPriorityQueue)t).isempty())
			{
				double y = (double)((QS._qss_c_.Collections_1_.IPriorityQueue)t).deletemin();
				if (x > y)
					throw new Exception("Ordering property not satisfied.");

				x = y;

				if (r.NextDouble() < 0.5)
				{
					((QS._qss_c_.Collections_1_.IPriorityQueue)t).insert(x + r.NextDouble());
				}
			}
		}

		public static void Test2()
		{
			System.Random r = new System.Random();

			QS._qss_c_.Collections_4_.BinaryTree<double> t = new QS._qss_c_.Collections_4_.BinaryTree<double>();
			for (int ind = 0; ind < 100000; ind++)
			{
				((System.Collections.Generic.ICollection<double>) t).Add(r.NextDouble());
			}

			double x = -1;
			foreach (double y in t)
			{
				if (x > y)
					throw new Exception("Ordering property not satisfied.");
				x = y;
			}
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		int System.Collections.ICollection.Count
		{
			get { return count; }
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
			foreach (System.IComparable o in ((System.Collections.Generic.ICollection<System.IComparable>)this))
				yield return o;
		}

		#endregion


	}
}
