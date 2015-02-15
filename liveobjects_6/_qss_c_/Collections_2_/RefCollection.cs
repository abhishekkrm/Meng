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

using System.Threading;

namespace QS._qss_c_.Collections_2_
{
    public interface IRefCollection<K,T> where T : System.IDisposable
    {
        T this[K key]
        {
            get;
        }
    }

    public class RefCollection<K,T> : IRefCollection<K,T> 
        where T : RefCollection<K,T>.Element, new() where K : System.IComparable, System.IComparable<K>
    {
        public delegate void InitializeCallback(T element);

        public RefCollection(InitializeCallback initializeCallback)
        {
            this.initializeCallback = initializeCallback;
        }

        private readonly Collections_2_.IBinaryTreeOf<K, T> elements = new Collections_2_.SplayOf<K, T>();
        private InitializeCallback initializeCallback;

        public abstract class Element : Collections_2_.BTNOf<K>, System.IDisposable
        {
            public Element()
            {
            }

            public RefCollection<K,T> containingCollection;
            public int nreferences = 1;

            protected abstract void disposingOf();

            #region IDisposable Members

            public void Dispose()
            {
                if (Interlocked.Decrement(ref nreferences) == 0)
                {
                    lock (containingCollection.elements)
                    {
                        if (nreferences == 0)
                            containingCollection.elements.remove(this.key);
                    }
                }
            }

            #endregion
        }

        #region IRefCollection<K,T> Members

        public T this[K key]
        {
            get 
            {
                T element;
                bool createdAnew;

                lock (elements)
                {
                    element = elements.lookupOrCreate(key, out createdAnew);

                    if (createdAnew)
                    {
                        element.containingCollection = this;
                        this.initializeCallback(element);
                    }
                    else
                        Interlocked.Increment(ref element.nreferences);
                }

                return element;
            }
        }

        #endregion
    }
}
