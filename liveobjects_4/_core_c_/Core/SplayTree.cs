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

// #define DEBUG_SanityChecking
// #define DEBUG_TrackChanges
// #define DEBUG_SelfCheck

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._core_c_.Core
{
    public sealed class SplayTree<K,C> : BST<K, C>, IBST<K,C>, IPriorityQueue<C> 
        where K : IComparable<K> where C : class, IBSTNode<K, C>
    {
        #region Constructor

        public SplayTree() : base()
		{
        }

        #endregion

        #region IBST<K,C> Members

        bool IBST<K,C>.Insert(C element, bool duplicatesAllowed)
		{
            return Insert(element, duplicatesAllowed);
		}

        void IBST<K, C>.Remove(C node)
        {
            Remove(node);
        }

		#endregion

		#region Insert

        protected override bool Insert(C element, bool duplicatesAllowed)
        {
            if (base.Insert(element, duplicatesAllowed))
            {
#if DEBUG_SelfCheck
                ICollection<C> _expected = _collectme();
#endif

                Splay(element);
                root = element;

#if DEBUG_SelfCheck
                _compare(_expected, _collectme());
#endif

                return true;
            }
            else
                return false;
        }

        #endregion

        #region Remove

        private void Remove(C node)
		{
#if DEBUG_SanityChecking
            C temp = node;
            while (temp.Parent != null)
                temp = temp.Parent;
            if (!ReferenceEquals(temp, root))
                throw new Exception("Cannot remove, the node given as argument is not part of this tree.");
#endif

			Splay(node);

			if (node.Left != null)
			{
				((C) node.Left).Parent = default(C);
				if (node.Right != null)
				{
					((C) node.Right).Parent = default(C);
					root = JoinSubtrees(node.Left, node.Right);
				}
				else
					root = node.Left;
			}
			else
			{
				if (node.Right != null)
				{
					((C) node.Right).Parent = default(C);
					root = node.Right;
				}
				else
					root = default(C);
			}

			numberOfNodes--;

            node.Parent = default(C);
			node.Left = default(C);
			node.Right = default(C);
		}

		#endregion

        #region IPriorityQueue<C> Members

        void IPriorityQueue<C>.Clear()
        {
            this.root = default(C);
            this.numberOfNodes = 0;
        }

        int IPriorityQueue<C>.Count
        {
            get { return numberOfNodes; }
        }

        bool IPriorityQueue<C>.IsEmpty
        {
            get { return root == null; }
        }

        void IPriorityQueue<C>.Enqueue(C element)
		{
#if DEBUG_TrackChanges
            Console.WriteLine("__________SplayTree.Enqueue_Enter : " + QS.Fx.Printing.Printable.ToString(element));
            Console.WriteLine("_(contents)____________________________________________________________");
            _printme();
            Console.WriteLine("_____________________________________________________________________");
#endif

			Insert(element, true);

#if DEBUG_TrackChanges
            Console.WriteLine("__________SplayTree.Enqueue_Leave : " + QS.Fx.Printing.Printable.ToString(element));
            Console.WriteLine("_(contents)____________________________________________________________");
            _printme();
            Console.WriteLine("_____________________________________________________________________");
#endif
		}

		C IPriorityQueue<C>.Dequeue()
		{
#if DEBUG_TrackChanges
            Console.WriteLine("__________SplayTree.Dequeue_Enter");
            Console.WriteLine("_(contents)____________________________________________________________");
            _printme();
            Console.WriteLine("_____________________________________________________________________");
#endif

			C head = ((IPriorityQueue<C>) this).Head;
			Remove(head);

#if DEBUG_TrackChanges
            Console.WriteLine("__________SplayTree.Dequeue_Leave : " + QS.Fx.Printing.Printable.ToString(head));
            Console.WriteLine("_(contents)____________________________________________________________");
            _printme();
            Console.WriteLine("_____________________________________________________________________");
#endif

			return head;
		}

        void IPriorityQueue<C>.Remove(C node)
        {
            Remove(node);
        }

		C IPriorityQueue<C>.Head
		{
			get
			{
				C smallest = root;

				if (smallest != null)
				{
					while (smallest.Left != null)
						smallest = smallest.Left;

					Splay(smallest);
					root = smallest;
				}

				return smallest;
			}
		}

		#endregion

		#region Splay

		private void Splay(C node)
		{
			while (node.Parent != null)
			{
				if (((C) node.Parent).Parent != null)
				{
					bool b1 = ReferenceEquals(node, ((C) node.Parent).Left);
					bool b2 = ReferenceEquals(node.Parent, ((C) ((C) node.Parent).Parent).Left);

					Rotate(((b1 && !b2) || (!b1 && b2)) ? node : node.Parent);
				}

				Rotate(node);
			}
		}

        #endregion

        #region Rotate

        private void Rotate(C node)
		{
			if (node.Parent == null)
				throw new Exception("Internal error: Cannot rotate at this node becase it has no parent.");

			C parent = node.Parent;
			C grandparent = parent.Parent;

			if (ReferenceEquals(node, parent.Left))
			{
				parent.Left = node.Right;
				if (node.Right != null)
					((C) node.Right).Parent = parent;
				node.Right = parent;
			}
			else
			{
				parent.Right = node.Left;
				if (node.Left != null)
					((C) node.Left).Parent = parent;
				node.Left = parent;
			}

			parent.Parent = node;
			node.Parent = grandparent;

			if (grandparent != null)
			{
				if (ReferenceEquals(parent, grandparent.Left))
					grandparent.Left = node;
				else
					grandparent.Right = node;
			}
		}

		#endregion

        #region JoinSubtrees

        private C JoinSubtrees(C left, C right)
		{
			C lmax = left;
			while (lmax.Right != null)
				lmax = lmax.Right;
			
			Splay(lmax);

			lmax.Right = right;
			right.Parent = lmax;

			return lmax;
		}

		#endregion
    }
}
