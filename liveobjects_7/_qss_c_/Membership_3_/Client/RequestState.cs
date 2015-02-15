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

namespace QS._qss_c_.Membership_3_.Client
{
    public class RequestState : IAsyncResult
    {
        public RequestState(Requests.Request request, AsyncCallback asyncCallback, object asyncState)
        {
            this.request = request;
            this.asyncCallback = asyncCallback;
            this.asyncState = asyncState;
        }

        private Requests.Request request;
        private AsyncCallback asyncCallback;
        private object asyncState;
        private System.Threading.ManualResetEvent waitHandle;
        private bool completed;
        private Responses.Response response;

        #region Completed

        public void Completed(Responses.Response response)
        {
            AsyncCallback callback = null;
            lock (this)
            {
                this.response = response;
                completed = true;
                if (waitHandle != null)
                    waitHandle.Set();
                callback = asyncCallback;
            }

            if (callback != null)
                callback(this);
        }

        #endregion

        #region Accessors

        public Requests.Request Request
        {
            get { return request; }
        }

        public Responses.Response Response
        {
            get { return response; }
        }

        #endregion

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return asyncState; }
        }

        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get 
            {
                lock (this)
                {
                    if (waitHandle == null)
                        waitHandle = new System.Threading.ManualResetEvent(completed);
                    return waitHandle;
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
    }
}
