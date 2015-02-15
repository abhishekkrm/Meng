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

namespace QS._qss_c_.Components_1_
{
    public class AutomaticCollection<K, V> where V : new()
    {
        public AutomaticCollection() : this(new Base3_.Constructor<V, K>(Create))
        {
        }

        public AutomaticCollection(Base3_.Constructor<V> constructor) : this(new Base3_.Constructor<V, K>((new ConstructorWrapper(constructor)).Create))
        {
        }

        public AutomaticCollection(Base3_.Constructor<V, K> constructor)
        {
            this.constructor = constructor;
            changes = new System.Collections.Generic.Dictionary<K, V>();
        }

        #region Constructor Wrappers

        private static V Create(K key)
        {
            return new V();
        }

        private class ConstructorWrapper
        {
            public ConstructorWrapper(Base3_.Constructor<V> constructor)
            {
                this.constructor = constructor;
            }

            private Base3_.Constructor<V> constructor;

            public V Create(K key)
            {
                return constructor();
            }
        }

        #endregion

        private System.Collections.Generic.IDictionary<K, V> changes;
        private Base3_.Constructor<V, K> constructor = null;

        public V this[K key]
        {
            get
            {
                V change;
                if (!changes.ContainsKey(key))
                    changes.Add(key, change = this.constructor(key));
                else
                    change = changes[key];
                return change;
            }
        }

        public IEnumerable<KeyValuePair<K, V>> Collection
        {
            get { return changes; }
        }
    }
}
