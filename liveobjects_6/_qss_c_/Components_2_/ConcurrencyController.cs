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

namespace QS._qss_c_.Components_2_
{
    public class ConcurrencyController : Base4_.IConcurrencyController
    {
        public ConcurrencyController() : this(1)
        {
        }

        public ConcurrencyController(int concurrency)
        {
            this.credit = concurrency;
        }

        private int credit;
        private Queue<Base3_.NoArgumentCallback> pendingQueue = new Queue<QS._qss_c_.Base3_.NoArgumentCallback>();

        #region IConcurrencyController Members

        bool QS._qss_c_.Base4_.IConcurrencyController.Consume(QS._qss_c_.Base3_.NoArgumentCallback readyCallback)
        {
            bool consumed_now;
            lock (this)
            {
                consumed_now = credit > 0;
                if (consumed_now)
                    credit--;
                else
                    pendingQueue.Enqueue(readyCallback);
            }
            return consumed_now;
        }

        void QS._qss_c_.Base4_.IConcurrencyController.Release()
        {
            Base3_.NoArgumentCallback readyCallback = null;
            lock (this)
            {
                if (pendingQueue.Count > 0)
                    readyCallback = pendingQueue.Dequeue();
                else
                    credit++;
            }
            if (readyCallback != null)
                readyCallback();
        }

        #endregion
    }
}
