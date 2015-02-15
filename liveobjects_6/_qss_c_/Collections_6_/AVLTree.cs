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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Collections_6_
{
    public class AVLTree<K, C> : BinaryTree<C>, IAVLTree<K, C> 
        where C : class, IAVLNode<K, C> where K : IComparable<K>
    {
        public AVLTree()
        {
        }

        #region Internal Processing

        private C FindOrAdd(K key, C elementCreated, Base3_.Constructor<C, K> createCallback)
        {
            if (root == null)            
            {
                if (elementCreated != null)
                    root = elementCreated;
                else if (createCallback != null)
                    root = createCallback(key);
                else
                    return null;
                    
                root.Balance = 0;
                root.Left = null;
                root.Right = null;
                root.Parent = null;
                return root;
            }

            C element = root;
            while (true)
            {
                int comparison_result = element.CompareTo(key);
                if (comparison_result == 0)
                {
                    if (elementCreated != null)
                        throw new Exception("Already exists.");
                    return element;
                }

                if (comparison_result > 0)
                {
                    if (element.Left != null)
                        element = element.Left;
                    else
                    {
                        C new_element;
                        if (elementCreated != null)
                            new_element = elementCreated;
                        else if (createCallback != null)
                            new_element = createCallback(key);
                        else
                            return null;

                        new_element.Left = null;
                        new_element.Right = null;
                        new_element.Parent = element;
                        new_element.Balance = 0;
                        element.Left = new_element;

                        AdjustAfterAdding(new_element);

                        return new_element;
                    }
                }
                else
                {
                    if (element.Right != null)
                        element = element.Right;
                    else
                    {
                        C new_element;
                        if (elementCreated != null)
                            new_element = elementCreated;
                        else if (createCallback != null)
                            new_element = createCallback(key);
                        else
                            return null;

                        new_element.Left = null;
                        new_element.Right = null;
                        new_element.Parent = element;
                        new_element.Balance = 0;
                        element.Right = new_element;

                        AdjustAfterAdding(new_element);

                        return new_element;
                    }
                }
            }
        }

        private void AdjustAfterAdding(C element)
        {
            while (true)
            {
                C parent = element.Parent;
                if (ReferenceEquals(element, parent.Left))
                    parent.Balance = parent.Balance + 1;
                else
                    parent.Balance = parent.Balance - 1;

                if (parent.Balance < -1)
                {
                    if (element.Balance < 0)
                    {
                        C grandparent = parent.Parent;

                        if (grandparent != null)
                        {
                            if (ReferenceEquals(grandparent.Right, parent))
                                grandparent.Right = element;
                            else
                                grandparent.Left = element;
                        }
                        element.Parent = grandparent;
                        parent.Parent = element;

                        parent.Right = element.Left;
                        if (parent.Right != null)
                            parent.Right.Parent = parent;

                        element.Left = parent;

                        element.Balance = 0;
                        parent.Balance = 0;
                    }
                    else
                    {
                        C grandparent = parent.Parent;
                        C child = element.Left;

                        if (grandparent != null)
                        {
                            if (ReferenceEquals(grandparent.Right, parent))
                                grandparent.Right = child;
                            else
                                grandparent.Left = child;
                        }
                        child.Parent = grandparent;
                        parent.Parent = child;
                        element.Parent = child;

                        element.Left = child.Right;
                        if (element.Left != null)
                            element.Left.Parent = element;

                        parent.Right = child.Left;
                        if (parent.Right != null)
                            parent.Right.Parent = parent;

                        child.Right = element;
                        child.Left = parent;

                        if (child.Balance > 0)
                        {
                            element.Balance = -1;
                            parent.Balance = 0;
                        }
                        else
                        {
                            element.Balance = 0;
                            parent.Balance = 1;
                        }

                        child.Balance = 0;
                    }

                    return;
                }
                else if (parent.Balance > +1)
                {
                    if (element.Balance > 0)
                    {
                        C grandparent = parent.Parent;
                        
                        if (grandparent != null)
                        {
                            if (ReferenceEquals(grandparent.Left, parent))
                                grandparent.Left = element;
                            else 
                                grandparent.Right = element;
                        }
                        element.Parent = grandparent;
                        parent.Parent = element;

                        parent.Left = element.Right;
                        if (parent.Left != null)
                            parent.Left.Parent = parent;

                        element.Right = parent;

                        element.Balance = 0;
                        parent.Balance = 0;
                    }
                    else
                    {
                        C grandparent = parent.Parent;
                        C child = element.Right;

                        if (grandparent != null)
                        {
                            if (ReferenceEquals(grandparent.Left, parent))
                                grandparent.Left = child;
                            else
                                grandparent.Right = child;
                        }
                        child.Parent = grandparent;
                        parent.Parent = child;
                        element.Parent = child;

                        element.Right = child.Left;
                        if (element.Right != null)
                            element.Right.Parent = element;

                        parent.Left = child.Right;
                        if (parent.Left != null)
                            parent.Left.Parent = parent;

                        child.Left = element;
                        child.Right = parent;

                        if (child.Balance < 0)
                        {
                            element.Balance = 1;
                            parent.Balance = 0;
                        }
                        else
                        {
                            element.Balance = 0;
                            parent.Balance = -1;
                        }

                        child.Balance = 0;
                    }

                    return;
                }
                else if (parent.Parent == null || parent.Balance == 0)
                    return;
                else
                    element = parent;
            }
        }

        #endregion

        #region IBinaryTree<C> Members

        C IBinaryTree<C>.Root
        {
            get { return root; }
        }

        #endregion

        #region IAVLTree<K,C> Members

        void IAVLTree<K, C>.Add(C element)
        {
            FindOrAdd(element.Key, element, null);            
        }

        #endregion
    }
}
