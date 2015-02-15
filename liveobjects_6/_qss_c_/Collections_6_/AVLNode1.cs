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
    public class AVLNode1<K, C> : IAVLNode<K, AVLNode1<K, C>> where K : IComparable<K>
    {
        public AVLNode1(K key, C data)
        {
            this.key = key;
            this.data = data;
        }

        public AVLNode1(
            K key, C data, int balance, AVLNode1<K, C> parent, AVLNode1<K, C> left, AVLNode1<K, C> right)
            : this(key, data)
        {
            this.balance = balance;
            this.parent = parent;
            this.left = left;
            this.right = right;
        }

        private K key;
        private C data;
        private int balance;
        private AVLNode1<K,C> parent, left, right;

        public C Data
        {
            get { return data; }
            set { data = value; }
        }

        #region IAVLNode<K,AVLNode1<K,C>> Members

        int IAVLNode<K, AVLNode1<K, C>>.Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        #endregion

        #region IBSTNode<K,AVLNode1<K,C>> Members

        K INode<K>.Key
        {
            get { return key; }
        }

        #endregion

        #region IBTNode<AVLNode1<K,C>> Members

        AVLNode1<K, C> IBTNode<AVLNode1<K, C>>.Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        AVLNode1<K, C> IBTNode<AVLNode1<K, C>>.Left
        {
            get { return left; }
            set { left = value; }
        }

        AVLNode1<K, C> IBTNode<AVLNode1<K, C>>.Right
        {
            get { return right; }
            set { right = value; }
        }

        #endregion

        #region IComparable<K> Members

        int IComparable<K>.CompareTo(K other)
        {
            return key.CompareTo(other);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("AVLNode(");
            s.Append(QS._core_c_.Helpers.ToString.Object(key));
            s.Append(", ");
            s.Append(QS._core_c_.Helpers.ToString.Object(data));
            s.Append(", ");
            s.Append(balance.ToString());
            s.Append(")");
            return s.ToString();
        }
    }
}
