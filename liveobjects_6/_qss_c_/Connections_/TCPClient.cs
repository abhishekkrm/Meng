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

namespace QS._qss_c_.Connections_
{
    public class TCPClient : IClientConnector
    {
        public TCPClient(QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        private QS.Fx.Logging.ILogger logger;

        #region IClientConnector Members

        IConnection IClientConnector.Connect(QS.Fx.Network.NetworkAddress serverAddress, IAsynchronousObject localObject)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            socket.Connect(new IPEndPoint(serverAddress.HostIPAddress, serverAddress.PortNumber));
            if (!socket.Connected)
                throw new Exception("Cannot connect to the destination.");

            TCPConnection connection = new TCPConnection(
                logger, new QS.Fx.Network.NetworkAddress(((IPEndPoint)socket.LocalEndPoint).Address, 
                ((IPEndPoint)socket.LocalEndPoint).Port), serverAddress, socket);
            ((IConnection)connection).LocalObject = localObject;

            return connection;
        }

        #endregion
    }
}
