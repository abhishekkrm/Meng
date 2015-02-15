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

namespace QS.Fx.Scheduling
{
    public sealed class SinglethreadedQueue : IQueue
    {
        #region Constructor

        public SinglethreadedQueue()
        {
        }

        #endregion

        #region Fields

        private bool recheck, blocked;
        private QS.Fx.Base.IEvent root;
        private Stack<QS.Fx.Base.IEvent> todo = new Stack<QS.Fx.Base.IEvent>();

        #endregion

        #region IQueue Members

        void IQueue.Enqueue(QS.Fx.Base.IEvent e)
        {
            QS.Fx.Base.IEvent myroot;
            do
            {
                myroot = root;
                e.Next = myroot;
            }
            while (Interlocked.CompareExchange<QS.Fx.Base.IEvent>(ref root, e, e.Next) != myroot);
            recheck = true;
            blocked = true;
        }

        QS.Fx.Base.IEvent IQueue.Dequeue()
        {
            blocked = false;
            while (recheck)
            {
                recheck = false;
                QS.Fx.Base.IEvent e = Interlocked.Exchange<QS.Fx.Base.IEvent>(ref root, null);
                while (e != null)
                {
                    QS.Fx.Base.IEvent n = e.Next;
                    e.Next = null;
                    todo.Push(e);
                    e = n;
                }
            }
            if (todo.Count > 0)
            {
                blocked = true;
                return todo.Pop();
            }
            else
                return null;
        }

        bool QS.Fx.Scheduling.IQueue.Blocked
        {
            get { return this.blocked; }
        }

        #endregion
    }
}
