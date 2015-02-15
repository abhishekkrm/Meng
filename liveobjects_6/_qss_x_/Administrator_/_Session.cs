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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QS._qss_x_.Administrator_
{
/*
    public sealed class Session : IDisposable
    {
        #region Constructor

        public delegate void DisconnectCallback(Session session, int sessionno);

        public Session(Socket socket, int sessionno, DisconnectCallback disconnectcallback)
        {
            this.socket = socket;
            this.sessionno = sessionno;
            this.disconnectcallback = disconnectcallback;
            exiting = false;
            thread = new Thread(new ThreadStart(this._ThreadCallback));
            thread.Start();
        }

        #endregion

        #region Fields

        private Socket socket;
        private int sessionno;
        private DisconnectCallback disconnectcallback;
        private bool exiting;
        private Thread thread;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                exiting = true;
                if (socket != null)
                {
                    try
                    {
                        socket.Disconnect(true);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
                if ((thread != null) && !thread.Join(TimeSpan.FromMilliseconds(100)))
                    thread.Abort();
            }
        }

        #endregion

        #region _ThreadCallback

        private void _ThreadCallback()
        {
            while (!exiting)
            {
                try
                {



                    lock (this)
                    {

                    }
                }
                catch (Exception exc)
                {
                    if (!exiting)
                    {
                        exiting = true;
                        ThreadPool.QueueUserWorkItem(
                            new WaitCallback(delegate(object o) { disconnectcallback(this, sessionno); }));
                    }
                }
            }
        }

        #endregion
    }
*/ 
}
