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
    public class BSTNode<K, C> : IBSTNode<K, BSTNode<K, C>> where K : IComparable<K>
    {
        public BSTNode(K key, C data, BSTNode<K, C> parent, BSTNode<K, C> left, BSTNode<K, C> right)
        {
            this.key = key;
            this.data = data;
            this.parent = parent;
            this.left = left;
            this.right = right;
        }

        public BSTNode()
        {
        }

        private K key;
        private C data;
        private BSTNode<K, C> parent, left, right;

        public C Data
        {
            get { return data; }
            set { data = value; }
        }

        #region IBSTNode<K, BSTNode<K, C>> Members

        K INode<K>.Key
        {
            get { return key; }
        }

        BSTNode<K, C> IBTNode<BSTNode<K, C>>.Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        BSTNode<K, C> IBTNode<BSTNode<K, C>>.Left
        {
            get { return left; }
            set { left = value; }
        }

        BSTNode<K, C> IBTNode<BSTNode<K, C>>.Right
        {
            get { return right; }
            set { right = value; }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("TreeNode(");
            s.Append(QS._core_c_.Helpers.ToString.Object(key));
            s.Append(", ");
            s.Append(QS._core_c_.Helpers.ToString.Object(data));
            s.Append(")");
            return s.ToString();
        }

        #endregion

        #region IComparable<K> Members

        int IComparable<K>.CompareTo(K other)
        {
            return key.CompareTo(other);
        }

        #endregion

/*
        #region IComparable<TreeNode<K,C>> Members

        int IComparable<TreeNode<K, C>>.CompareTo(TreeNode<K, C> other)
        {
            return key.CompareTo(other.key);
        }

        #endregion
*/
    }

    public class BSTNode<C> : IBSTNode<C, BSTNode<C>> where C : IComparable<C>
    {
        public BSTNode(C data, BSTNode<C> parent, BSTNode<C> left, BSTNode<C> right)
        {
            this.data = data;
            this.parent = parent;
            this.left = left;
            this.right = right;
        }

        public BSTNode()
        {
        }

        private C data;
        private BSTNode<C> parent, left, right;

        public C Data
        {
            get { return data; }
            set { data = value; }
        }

        #region IBSTNode<C, BSTNode<C>> Members

        C INode<C>.Key
        {
            get { return data; }
        }

        BSTNode<C> IBTNode<BSTNode<C>>.Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        BSTNode<C> IBTNode<BSTNode<C>>.Left
        {
            get { return left; }
            set { left = value; }
        }

        BSTNode<C> IBTNode<BSTNode<C>>.Right
        {
            get { return right; }
            set { right = value; }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("TreeNode(");
            s.Append(QS._core_c_.Helpers.ToString.Object(data));
            s.Append(")");
            return s.ToString();
        }

        #endregion

        #region IComparable<C> Members

        int IComparable<C>.CompareTo(C other)
        {
            return data.CompareTo(other);
        }

        #endregion

/*
        #region IComparable<TreeNode<C>> Members

        int IComparable<TreeNode<C>>.CompareTo(TreeNode<C> other)
        {
            return data.CompareTo(other.data);
        }

        #endregion
*/ 
    }
}
