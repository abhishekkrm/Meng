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

namespace QS._qss_x_.Base1_
{
    public sealed class ReadonlyDictionaryOf<K,V> : IDictionary<K,V>
    {
        public ReadonlyDictionaryOf(IDictionary<K,V> _dictionary)
        {
            this._dictionary = _dictionary;
        }

        private IDictionary<K, V> _dictionary;

        #region IDictionary<K,V> Members

        void IDictionary<K, V>.Add(K key, V value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<K, V>.ContainsKey(K key)
        {
            if (_dictionary != null)
            {
                lock (_dictionary)
                {
                    return _dictionary.ContainsKey(key);
                }
            }
            else
                return false;
        }

        ICollection<K> IDictionary<K, V>.Keys
        {
            get
            {
                if (_dictionary != null)
                {
                    lock (_dictionary)
                    {
                        System.Collections.ObjectModel.Collection<K> _keys = new System.Collections.ObjectModel.Collection<K>();
                        foreach (K _key in _dictionary.Keys)
                            _keys.Add(_key);
                        return _keys;
                    }
                }
                else
                    return new System.Collections.ObjectModel.Collection<K>();
            }
        }

        bool IDictionary<K, V>.Remove(K key)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<K, V>.TryGetValue(K key, out V value)
        {
            if (_dictionary != null)
            {
                lock (_dictionary)
                {
                    return _dictionary.TryGetValue(key, out value);
                }
            }
            else
            {
                value = default(V);
                return false;
            }
        }

        ICollection<V> IDictionary<K, V>.Values
        {
            get
            {
                if (_dictionary != null)
                {
                    lock (_dictionary)
                    {
                        System.Collections.ObjectModel.Collection<V> _values = new System.Collections.ObjectModel.Collection<V>();
                        foreach (V _value in _dictionary.Values)
                            _values.Add(_value);
                        return _values;
                    }
                }
                else
                    return new System.Collections.ObjectModel.Collection<V>();
            }
        }

        V IDictionary<K, V>.this[K key]
        {
            get
            {
                if (_dictionary != null)
                {
                    lock (_dictionary)
                    {
                        return _dictionary[key];
                    }
                }
                else
                    throw new Exception("This dictionary doesn't contain an element with such key.");
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<K,V>> Members

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<K, V>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            if (_dictionary != null)
            {
                lock (_dictionary)
                {
                    return _dictionary.Contains(item);
                }
            }
            else
                return false;
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            if (_dictionary != null)
            {
                lock (_dictionary)
                {
                    _dictionary.CopyTo(array, arrayIndex);
                }
            }
        }

        int ICollection<KeyValuePair<K, V>>.Count
        {
            get
            {
                if (_dictionary != null)
                {
                    lock (_dictionary)
                    {
                        return _dictionary.Count;
                    }
                }
                else
                    return 0;
            }
        }

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<K,V>> Members

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            if (_dictionary != null)
            {
                lock (_dictionary)
                {
                    List<KeyValuePair<K, V>> _elements = new List<KeyValuePair<K, V>>();
                    foreach (KeyValuePair<K, V> _element in _dictionary)
                        _elements.Add(_element);
                    return _elements.GetEnumerator();
                }
            }
            else
                return ((IEnumerable<KeyValuePair<K, V>>)(new KeyValuePair<K, V>[0])).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
