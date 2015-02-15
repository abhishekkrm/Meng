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
using System.Threading;

namespace QS._qss_c_.Synchronization_1_
{
    public class NonblockingStack<C> : IAccumulatingStack<C> where C : class, QS._core_c_.Synchronization.ILinkable<C>
    {
        public NonblockingStack()
        {
        }

        private C head;

        #region IAccumulatingStack<C> Members

        void IAccumulatingStack<C>.Add(C element)
        {
            ((IAccumulatingStack<C>)this).Add(new ChainOf<C>(element));
        }

        void IAccumulatingStack<C>.Add(ChainOf<C> elementChain)
        {
            if (elementChain.First.Next == null && elementChain.Last != elementChain.First || elementChain.Last.Next != null)
                throw new Exception("The element chain being added is formed incorrectly.");

            C currentHead;
            do
            {
                currentHead = this.head;
                elementChain.Last.Next = currentHead;

            }
            while (!ReferenceEquals(Interlocked.CompareExchange<C>(ref this.head, elementChain.First, currentHead), currentHead));
        }

        C IAccumulatingStack<C>.AccumulatedChain
        {
            get { return Interlocked.Exchange<C>(ref this.head, null); }
        }

        #endregion
    }
}
