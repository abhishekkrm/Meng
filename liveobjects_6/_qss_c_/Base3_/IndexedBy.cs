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

// #define Using_VS2005Beta1

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    public interface IIndexedBy<K> : System.IComparable, System.IComparable<K>, System.IComparable<IIndexedBy<K>> 
        where K : System.IComparable, System.IComparable<K>
    {
        K Key
        {
            get;
            set;
        }
    }

    public class IndexedBy<K> : IIndexedBy<K> where K : System.IComparable, System.IComparable<K>
    {
        public IndexedBy()
        {
        }

        public IndexedBy(K key)
        {
            this.key = key;
        }

        protected K key;

        #region IIndexedBy<K> Members

        K IIndexedBy<K>.Key
        {
            get { return key; }
            set { key = value; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            return ((IComparable)this).CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().Name + "[" + key.ToString() + "]";
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            IIndexedBy<K> other = obj as IIndexedBy<K>;
            if (other != null)
                return ((System.IComparable<K>) key).CompareTo(other.Key);
            else
            {
                if (obj != null && obj is K)
                    return ((System.IComparable<K>)key).CompareTo((K) obj);
                else
                    throw new Exception("Cannot compare " + this.GetType().Name + " with " + ((obj != null) ? obj.GetType().Name : "(null)"));
            }
        }

        #endregion

        #region IComparable<K> Members

        int IComparable<K>.CompareTo(K other)
        {
            return ((System.IComparable<K>) key).CompareTo(other);
        }

#if Using_VS2005Beta1
        bool IComparable<K>.Equals(K other)
        {
            return key.Equals(other);
        }
#endif

        #endregion

        #region IComparable<IIndexedBy<K>> Members

        int IComparable<IIndexedBy<K>>.CompareTo(IIndexedBy<K> other)
        {
            return ((System.IComparable<K>)key).CompareTo(other.Key);
        }

#if Using_VS2005Beta1
        bool IComparable<IIndexedBy<K>>.Equals(IIndexedBy<K> other)
        {
            return key.Equals(other.Key);
        }
#endif

        #endregion
    }
}
