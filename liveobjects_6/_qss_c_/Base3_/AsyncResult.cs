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

namespace QS._qss_c_.Base3_
{
    public class AsyncResult<ContextClass> : IAsyncResult
    {
        public AsyncResult(AsyncCallback callback, object state, ContextClass context)
        {
            this.callback = callback;
            this.state = state;
            this.context = context;
        }

        private AsyncCallback callback;
        private object state;
        private ContextClass context;
        private ManualResetEvent completionWaitHandle;
        private bool completed, synchronously, succeeded;
        private Exception exception;

        #region Accessors
        
        public AsyncCallback Callback
        {
            get { return callback; }
        }

        public object State
        {
            get { return state; }
        }

        public ContextClass Context
        {
            get { return context; }
            set { context = value; }
        }

        public bool Succeeded
        {
            get { return succeeded; }
        }

        public Exception Exception
        {
            get { return exception; }
        }

        #endregion

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return state; }
        }

        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get 
            {
                lock (this)
                {
                    if (completionWaitHandle == null)
                        completionWaitHandle = new ManualResetEvent(completed);
                    return completionWaitHandle;
                }
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return synchronously; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return completed; }
        }

        #endregion

        #region Completion

        public void Completed(bool synchronously, bool succeeded, Exception exception)
        {
            AsyncCallback callback_toinvoke;
            lock (this)
            {
                if (completed)
                    throw new Exception("Already completed.");
                completed = true;

                this.synchronously = synchronously;
                this.succeeded = succeeded;
                this.exception = exception;

                if (completionWaitHandle == null)
                    completionWaitHandle = new ManualResetEvent(true);
                else
                    completionWaitHandle.Set();

                callback_toinvoke = this.callback;
            }

            if (callback_toinvoke != null)
                callback_toinvoke(this);
        }

        #endregion
    }
}
