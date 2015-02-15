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

namespace QS._qss_d_.Agent_2_
{
    public class Client : IClient, IDisposable
    {
        public Client(QS.Fx.Network.NetworkAddress serviceAddress, ISession session)
        {
            this.serviceAddress = serviceAddress;
            this.session = session;

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(serviceAddress.HostIPAddress, serviceAddress.PortNumber));

            this.sessionController = new SessionController(socket, new EndSessionCallback(this.EndCallback), session); 
        }

        private QS.Fx.Network.NetworkAddress serviceAddress;
        private ISession session;
        private Socket socket;
        private SessionController sessionController;

        #region EndCallback

        private void EndCallback(SessionController session)
        {
            ((IDisposable)session).Dispose();
        }

        #endregion

        #region IClient Members

        IChannel IClient.Channel
        {
            get { return sessionController; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IDisposable) sessionController).Dispose();
        }

        #endregion
    }
}
