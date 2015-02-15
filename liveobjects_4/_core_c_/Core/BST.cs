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

// #define DEBUG_TrackChanges
// #define DEBUG_SelfCheck

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._core_c_.Core
{
	public class BST<K,C> : QS.Fx.Inspection.Inspectable, IBST<K, C> 
        where K : IComparable<K> where C : class, IBSTNode<K, C>
    {
        #region Constructor

        public BST()
		{
			root = default(C);
			numberOfNodes = 0;
        }

        #endregion

        #region Fields

        protected C root;
		[QS.Fx.Base.Inspectable("Count", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected int numberOfNodes;

        #endregion

        #region Insertion

        protected virtual bool Insert(C element, bool duplicatesAllowed)
        {
#if DEBUG_SelfCheck
            ICollection<C> _expected = _collectme();
            if (!_expected.Contains(element))
                _expected.Add(element);
#endif

            if (root != null)
            {
                C parent = root;
                K key = element.Key;

                while (true)
                {
                    int comparison = parent.CompareTo(key);
                    if (comparison == 0)
                    {
                        if (ReferenceEquals(parent, element))
                            throw new Exception("Cannot add the exact same element twice!");

                        if (!duplicatesAllowed)
                            return false;

                        if (parent.Right == null)
                        {
                            parent.Right = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Right;
                    }
                    else if (comparison > 0)
                    {
                        if (parent.Left == null)
                        {
                            parent.Left = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Left;
                    }
                    else
                    {
                        if (parent.Right == null)
                        {
                            parent.Right = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Right;
                    }
                }
            }
            else
            {
                element.Parent = null;
                root = element;
            }

            element.Left = null;
            element.Right = null;

            numberOfNodes++;

#if DEBUG_SelfCheck
            _compare(_expected, _collectme());
#endif

            return true;
        }

/*
        protected virtual bool InsertAt(C element, C location, bool duplicatesAllowed)
        {
#if DEBUG_SelfCheck
            ICollection<C> _expected = _collectme();
            if (!_expected.Contains(element))
                _expected.Add(element);
#endif

            if (root != null)
            {
                C parent = location;
                K key = element.Key;
                bool goup;
                if (parent == null)
                {
                    parent = root;
                    goup = false;
                }
                else
                    goup = true;
                int comparison = parent.CompareTo(key);
                if (goup)
                {
                    if (comparison > 0)
                    {
                        C grandparent = parent.Parent;
                        while (grandparent != null)
                        {
                            if (ReferenceEquals(grandparent.Left, parent))
                                grandparent = grandparent.Parent;
                            else
                            {
                                int grandparentcomparison = grandparent.CompareTo(key);
                                if (grandparentcomparison > 0)
                                {
                                    parent = grandparent;
                                    comparison = grandparentcomparison;
                                    grandparent = parent.Parent;
                                }
                                else if (grandparentcomparison < 0)
                                {

                                }
                                else
                                    break;
                            }
                        }


                        //C grandparent = parent.Parent;
                        //if (grandparent == null)
                        //    break;
                        //else
                        //{

                        //    int grandparentcomparison = grandparent.CompareTo(key);
                        //    if (grandparentcomparison == 0)
                        //        break;
                        //    else if (grandparentcomparison > 0)
                        //    {
                        //        if (comparison > 0)
                        //        {

                        //        }
                        //        else
                        //        {

                        //        }
                        //    }
                        //    else
                        //    {
                        //    }
                        //}
                    }
                    else if (comparison < 0)
                    {


                    }
                }

                
                {
                    if (comparison == 0)
                    {
                        if (ReferenceEquals(parent, element))
                            throw new Exception("Cannot add the exact same element twice!");
                        if (!duplicatesAllowed)
                            return false;
                        if (parent.Right == null)
                        {
                            parent.Right = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Right;
                    }
                    else if (comparison > 0)
                    {






                        if (parent.Left == null)
                        {
                            parent.Left = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Left;
                    }
                    else
                    {
                        if (parent.Right == null)
                        {
                            parent.Right = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Right;
                    }


                }




                while (true)
                {
                    int comparison = parent.CompareTo(key);
                    if (comparison == 0)
                    {
                        if (ReferenceEquals(parent, element))
                            throw new Exception("Cannot add the exact same element twice!");

                        if (!duplicatesAllowed)
                            return false;

                        if (parent.Right == null)
                        {
                            parent.Right = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Right;
                    }
                    else if (comparison > 0)
                    {






                        if (parent.Left == null)
                        {
                            parent.Left = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Left;
                    }
                    else
                    {
                        if (parent.Right == null)
                        {
                            parent.Right = element;
                            element.Parent = parent;
                            break;
                        }
                        else
                            parent = parent.Right;
                    }
                }
            }
            else
            {
                element.Parent = null;
                root = element;
            }

            element.Left = null;
            element.Right = null;

            numberOfNodes++;

#if DEBUG_SelfCheck
            _compare(_expected, _collectme());
#endif

            return true;
        }
*/

		#endregion

        #region Merging

        private void Merge(IBST<K, C> other)
        {
            C current = other.Root;
            while (current != null)
            {
                C left;
                while ((left = current.Left) != null)
                    current = left;
                C parent = current.Parent;
                current.Parent = null;
                C right = current.Right;
                current.Right = null;
                this.Insert(current, true);
                if (right != null)
                {
                    current = right;
                    right.Parent = parent;
                    if (parent != null)
                        parent.Left = right;
                }
                else
                {
                    current = parent;
                    if (parent != null)
                        parent.Left = null;
                }
            }
        }

        #endregion

        #region IBST<K,C> Members

        bool IBST<K, C>.Insert(C element)
        {
            return Insert(element, false);
        }

        bool IBST<K, C>.Insert(C element, bool duplicatesAllowed)
        {
            return Insert(element, duplicatesAllowed);
        }

        void IBST<K, C>.Merge(IBST<K, C> other)
        {
            Merge(other);
        }

		void IBST<K,C>.Remove(C node)
		{
			throw new NotImplementedException();
		}

		C IBST<K,C>.Root
		{
			get { return root; }
		}

		int IBST<K,C>.Count
		{
			get { return numberOfNodes; }
		}

		bool IBST<K,C>.IsEmpty
		{
			get { return root == null; }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.Generic.IEnumerator<C> GetEnumerator()
        {
            return new InOrderWalker<C>(root);
        }

        System.Collections.Generic.IEnumerator<C> System.Collections.Generic.IEnumerable<C>.GetEnumerator()
		{
            return GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
            return GetEnumerator();
        }

        #endregion

        #region Searching

        public enum SearchResult
		{
			Found, Smaller, Greater, None
		}

		public void BinarySearch(K key, out C node, out SearchResult result)
		{
			if (root != null)
				BinarySearch(root, key, out node, out result);
			else
			{
				node = null;
				result = SearchResult.None;
			}
		}

		public static void BinarySearch(C subtreeRoot, K key, out C node, out SearchResult result)
		{
			while (true)
			{
				int comparison = subtreeRoot.CompareTo(key);
				if (comparison == 0)
				{
					node = subtreeRoot;
					result = SearchResult.Found;
					break;
				}
				else
				{
					C child = (comparison < 0) ? subtreeRoot.Right : subtreeRoot.Left;
					if (child == null)
					{
						node = subtreeRoot;
						result = (comparison < 0) ? SearchResult.Smaller : SearchResult.Greater;
						break;
					}
					else
						subtreeRoot = child;
				}
			}
		}

		#endregion

#if DEBUG_SelfCheck
        protected ICollection<C> _collectme()
        {
            System.Collections.ObjectModel.Collection<C> collection = new System.Collections.ObjectModel.Collection<C>();
            _collectme(root, collection);
            return collection;
        }

        protected void _collectme(C element, ICollection<C> collection)
        {
            if (element != null)
            {
                _collectme(element.Left, collection);
                collection.Add(element);
                _collectme(element.Right, collection);
            }
        }

        protected void _compare(ICollection<C> before, ICollection<C> after)
        {
            foreach (C element in before)
            {
                if (!after.Remove(element))
                    throw new Exception("Dissappeared: " + QS.Fx.Printing.Printable.ToString(element));
            }

            if (after.Count > 0)
            {
                IEnumerator<C> iter = after.GetEnumerator();
                iter.MoveNext();
                throw new Exception("Got left behind: " + QS.Fx.Printing.Printable.ToString(iter.Current));
            }
        }
#endif

#if DEBUG_TrackChanges
        protected void _printme()
        {
            _printme(root);
        }

        private void _printme(C element)
        {
            if (element != null)
            {
                _printme(element.Left);
                Console.WriteLine(QS.Fx.Printing.Printable.ToString(element));
                _printme(element.Right);
            }
        }
#endif
    }
}
