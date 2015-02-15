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

namespace QS._qss_c_.Framework_2_
{
    public sealed class OpenRequest : IAsyncResult, QS.Fx.Base.IEvent
    {
        public OpenRequest(QS.Fx.Base.ContextCallback<OpenRequest> processingCallback, 
            Base3_.GroupID groupID, GroupOptions options, AsyncCallback callback, object state)
        {
            this.groupID = groupID;
            this.callback = callback;
            this.options = options;
            this.state = state;
            this.processingCallback = processingCallback;
        }

        private Base3_.GroupID groupID;
        private AsyncCallback callback;
        private object state;
        private GroupOptions options;
        private bool completed;
        private System.Threading.ManualResetEvent completionEvent;
        private QS.Fx.Base.IEvent corenext;
        private QS.Fx.Base.ContextCallback<OpenRequest> processingCallback;
        private GroupRef groupRef;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public AsyncCallback Callback
        {
            get { return callback; }
        }

        public GroupOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        public GroupRef GroupRef
        {
            get { return groupRef; }
            set { groupRef = value; }
        }

        public object AsyncState
        {
            get { return state; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get 
            {
                lock (this)
                {
                    if (completionEvent == null)
                        completionEvent = new System.Threading.ManualResetEvent(completed);
                }

                return completionEvent;
            }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return completed; }
            set { completed = true; }
        }

        #endregion

        #region IRequest Members

        void QS.Fx.Base.IEvent.Handle()
        {
            if (completed)
            {
                if (completionEvent != null)
                    completionEvent.Set();

                if (callback != null)
                    callback(this);
            }
            else
                processingCallback(this);
        }

        #endregion

        #region IItem<IRequest> Members

        QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
        {
            get { return corenext; }
            set { corenext = value; }
        }

        #endregion

        QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
        {
            get { return QS.Fx.Base.SynchronizationOption.None; }
        }

    }
}
