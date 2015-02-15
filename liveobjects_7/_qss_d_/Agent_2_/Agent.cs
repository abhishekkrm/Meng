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

namespace QS._qss_d_.Agent_2_
{
    public class Agent : IDisposable
    {
        public Agent(IList<QS.Fx.Network.NetworkAddress> addresses, ConnectCallback connectCallback,
            DisconnectCallback disconnectCallback, IService service)
        {
            this.connectCallback = connectCallback;
            this.disconnectCallback = disconnectCallback;
            this.service = service;

            listeners = new Dictionary<QS.Fx.Network.NetworkAddress, Socket>(addresses.Count);
            foreach (QS.Fx.Network.NetworkAddress address in addresses)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(address.HostIPAddress, address.PortNumber));
                socket.Listen(10);
                socket.BeginAccept(new AsyncCallback(this.ConnectedCallback), socket);
                listeners.Add(address, socket);
            }
        }

        private IDictionary<QS.Fx.Network.NetworkAddress, Socket> listeners;
        private IList<SessionController> sessions = new List<SessionController>();
        private ConnectCallback connectCallback;
        private DisconnectCallback disconnectCallback;
        private IService service;

        #region ListeningCallback

        private void ConnectedCallback(IAsyncResult result)
        {
            Socket socket = ((Socket) result.AsyncState).EndAccept(result);
            SessionController session = new SessionController(socket, new EndSessionCallback(this.EndCallback), service);
            sessions.Add(session);

            connectCallback(session);
        }

        #endregion

        #region EndSessionCallback

        private void EndCallback(SessionController session)
        {
            disconnectCallback(session);
            ((IDisposable)session).Dispose();
            sessions.Remove(session);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            foreach (SessionController session in sessions)
            {
                disconnectCallback(session);
                ((IDisposable) session).Dispose();
            }
            sessions.Clear();

            foreach (Socket socket in listeners.Values)
            {
                socket.Disconnect(false);
                socket.Close();
            }
            listeners.Clear();
        }

        #endregion
    }
}
