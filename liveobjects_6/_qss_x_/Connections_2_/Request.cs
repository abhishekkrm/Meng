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

namespace QS._qss_x_.Connections_2_
{
    public sealed class Request : IAsyncResult
    {
        #region Static Create

        public static Request Create(IConnectionControl connection, AsyncCallback callback, object cookie, out bool connect)
        {
            connection.ReferenceCount = connection.ReferenceCount + 1;

            Request request = new Request(connection, callback, cookie);
            connect = false;

            switch (connection.ConnectionStatus)
            {
                case ConnectionStatus.Connected:
                    {
                        request.Completed(true, null);
                    }
                    break;

                case ConnectionStatus.Connecting:
                case ConnectionStatus.Reconnecting:
                    {
                        connection.Requests.Enqueue(request);
                    }
                    break;

                case ConnectionStatus.Disconnected:
                case ConnectionStatus.Disconnecting:
                    {
                        connection.Requests.Enqueue(request);
                        connect = true;
                    }
                    break;
            }

            return request;
        }

        #endregion

        #region Static GetConnection<ConnectionClass>

        public static ConnectionClass GetConnection<ConnectionClass>(IAsyncResult result)
            where ConnectionClass : class, IConnectionControl
        {
            Request request = result as Request;
            if (request != null)
            {
                ConnectionClass connection = (ConnectionClass)request.GetConnection();
                if (connection == null)
                    throw new Exception();

                return connection;
            }
            else
                throw new Exception();
        }

        #endregion

        #region Constructor

        private Request(IConnectionControl connection, AsyncCallback callback, object cookie)
        {
            this.connection = connection;
            this.callback = callback;
            this.cookie = cookie;
        }

        #endregion

        #region Fields

        private IConnectionControl connection;
        private AsyncCallback callback;
        private object cookie;
        private ManualResetEvent completionEvent;
        private bool completed, completedSynchronously;
        private Exception exception;

        #endregion

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return cookie; }
        }

        System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get 
            {
                lock (this)
                {
                    if (completionEvent == null)
                        completionEvent = new ManualResetEvent(completed);
                    return completionEvent;
                }
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return completedSynchronously; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return completed; }
        }

        #endregion

        #region GetConnection

        public IConnectionControl GetConnection()
        {
            lock (this)
            {
                if (!completed)
                    throw new Exception("Request to connect has not been completed yet.");
                
                if (exception != null)
                    throw new Exception("The attempt to connect failed.", exception);

                return connection;
            }
        }

        #endregion

        #region Completed

        public void Completed(bool synchronously, Exception exception)
        {
            AsyncCallback callback;
            lock (this)
            {
                completed = true;
                completedSynchronously = synchronously;
                this.exception = exception;
                if (completionEvent != null)
                    completionEvent.Set();
                callback = this.callback;
            }

            if (callback != null)
                callback(this);
        }

        #endregion
    }
}
