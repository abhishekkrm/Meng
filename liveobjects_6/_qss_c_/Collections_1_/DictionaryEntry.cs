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

namespace QS._qss_c_.Collections_1_
{
    public struct DictionaryEntry<KeyClass, ValueClass> : IEquatable<DictionaryEntry<KeyClass, ValueClass>> 
        //, IComparable <DictionaryEntry<KeyClass, ValueClass>>
    {
        public DictionaryEntry(KeyClass key, ValueClass value)
        {
            this.Key = key;
            this.Value = value;
        }

        public KeyClass Key;
        public ValueClass Value;

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is DictionaryEntry<KeyClass, ValueClass>)
            {
                DictionaryEntry<KeyClass, ValueClass> other = (DictionaryEntry<KeyClass, ValueClass>)obj;
                return Key.Equals(other.Key) && Value.Equals(other.Value);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Value.GetHashCode();
        }

        public override string ToString()
        {
            return "<" + ((Key != null) ? Key.ToString() : "null") + ", " + ((Value != null) ? Value.ToString() : "null") + ">";
        }

        #endregion

        #region IEquatable<DictionaryEntry<KeyClass,ValueClass>> Members

        bool IEquatable<DictionaryEntry<KeyClass, ValueClass>>.Equals(DictionaryEntry<KeyClass, ValueClass> other)
        {
            return Key.Equals(other.Key) && Value.Equals(other.Value);
        }

        #endregion

/*
        private const bool componentsAreComparable = typeof(IComparable<KeyClass>).IsAssignableFrom(typeof(KeyClass)) &&
            typeof(IComparable<ValueClass>).IsAssignableFrom(typeof(ValueClass));

        #region IComparable<DictionaryEntry<KeyClass,ValueClass>> Members

        int IComparable<DictionaryEntry<KeyClass, ValueClass>>.CompareTo(DictionaryEntry<KeyClass, ValueClass> other)
        {
            if (componentsAreComparable)
            {
                return ((IComparable<KeyClass>) Key
            }
            else
                throw new NotSupportedException("Elements of this class are not comparable.");            
        }

        #endregion
*/ 
    }
}
