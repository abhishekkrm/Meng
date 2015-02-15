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

namespace QS._core_c_.Core
{
    public class InOrderWalker<C> : IEnumerator<C>, System.Collections.IEnumerator 
    where C : class, IBTNode<C>
    {
        public InOrderWalker(C root)
        {
            this.root = root;
            this.current = null;
        }

        private C root, current;
        private bool finished;

        #region IEnumerator<C> Members

        C IEnumerator<C>.Current
        {
            get 
            {
                if (current == null)
                    throw new Exception("Not started or already finished enumerating.");
                return current; 
            }
        }

        #endregion

        #region IEnumerator Members

        bool System.Collections.IEnumerator.MoveNext()
        {
            if (finished)
                throw new Exception("Already finished enumerating.");

            if (current == null)
            {
                if (root == null)
                {
                    current = null;
                    finished = true;
                    return false;
                }
                else
                {
                    current = root;
                    while (current.Left != null)
                        current = current.Left;
                    return true;
                }
            }
            else
            {
                if (current.Right != null)
                {
                    current = current.Right;
                    while (current.Left != null)
                        current = current.Left;
                    return true;
                }
                else
                {
                    while (!ReferenceEquals(current, root))
                    {
                        C previous = current;
                        current = current.Parent;

                        if (ReferenceEquals(previous, current.Left))
                            return true;
                    }

                    current = null;
                    finished = true;
                    return false;
                }
            }
        }

        void System.Collections.IEnumerator.Reset()
        {
            current = null;
            finished = false;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return ((IEnumerator<C>)this).Current; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
