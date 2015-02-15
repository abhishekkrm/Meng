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

namespace QS._qss_c_.Base6_
{
    public class AsynchronousRequest : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, IAsyncResult
    {
        public AsynchronousRequest(QS._core_c_.Base3.Message message, AsyncCallback callback, object context)
        {
            this.message = message;
            this.callback = callback;
            this.context = context;
        }

        private QS._core_c_.Base3.Message message;
        private AsyncCallback callback;
        private ManualResetEvent completionHandle;
        private bool completed, succeeded;
        private Exception exception;
        private object context;
        private static QS._core_c_.Base6.CompletionCallback<object> internalCompletionCallback =
            new QS._core_c_.Base6.CompletionCallback<object>(AsynchronousRequest.InternalCompletionCallback);

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return message; }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return this; }
        }

        QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return new QS._core_c_.Base6.CompletionCallback<object>(AsynchronousRequest.InternalCompletionCallback); }
        }

        #endregion

        #region Internal Completion Callback

        private static void InternalCompletionCallback(bool succeeded, System.Exception exception, object context)        
        {
            ((AsynchronousRequest)context).InternalCompletionCallback(succeeded, exception);
        }

        private void InternalCompletionCallback(bool succeeded, System.Exception exception)
        {
            AsyncCallback callback_to_invoke_now = null;
            lock (this)
            {
                if (!completed)
                {
                    completed = true;
                    this.succeeded = succeeded;
                    this.exception = exception;
                    if (completionHandle != null)
                        completionHandle.Set();
                    callback_to_invoke_now = callback;
                }
            }

            if (callback_to_invoke_now != null)
                callback_to_invoke_now(this);
        }

        #endregion

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return context; }
        }

        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get 
            {
                lock (this)
                {
                    if (completionHandle == null)
                        completionHandle = new ManualResetEvent(completed);
                    return completionHandle;
                }
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return false; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return completed; }
        }

        #endregion

        #region Retrieving the result

        public void ConsumeResult()
        {
            lock (this)
            {
                if (!completed)
                    throw new Exception("Not completed.");
                if (!succeeded)
                    throw new Exception("Operation failed.\n", exception);
            }
        }

        #endregion
    }
}
