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
    public interface INonblockingWorker<C> : IDisposable where C : class, QS.Fx.Base.IEvent
    {
        void Process(C request);
    }

    public sealed class NonblockingWorker<C> : INonblockingWorker<C> where C : class, QS.Fx.Base.IEvent
    {
        public NonblockingWorker(QS.Fx.Base.ContextCallback<C> processingCallback)
        {
            this.processingCallback = processingCallback;
            thread = new Thread(new ThreadStart(this.ThreadCallback));
            thread.Name = this.GetType().ToString() + "(" + 
                processingCallback.Target.GetType().ToString() + "." + processingCallback.Method.Name + ")";
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        private Thread thread;
        private AutoResetEvent more = new AutoResetEvent(false);
        private bool processing;
        private QS.Fx.Scheduling.IQueue pending = new QS.Fx.Scheduling.SinglethreadedQueue();
        private bool disposing;
        private QS.Fx.Base.ContextCallback<C> processingCallback;

        void INonblockingWorker<C>.Process(C request)
        {
            pending.Enqueue(request);
            if (!processing)
            {
                processing = true;
                more.Set();
            }
        }

        private void ThreadCallback()
        {
            while (!disposing)
            {
                more.WaitOne();

                while (true)
                {
                    QS.Fx.Base.IEvent e = pending.Dequeue();
                    if (e == null)
                        break;
                    C request = (C) e;
                    processingCallback(request);
                }

                processing = false;

                while (true)
                {
                    QS.Fx.Base.IEvent e = pending.Dequeue();
                    if (e == null)
                        break;
                    C request = (C)e;
                    processingCallback(request);
                }
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            disposing = true;
            more.Set();
            if (!thread.Join(1000))
            {
                thread.Abort();
                thread.Join();
            }
        }

        #endregion
    }
}
