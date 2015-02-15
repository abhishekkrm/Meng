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

// #define DEBUG_TCPListener

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_c_.Connections_
{
    public class TCPListener : IDisposable
    {
        public TCPListener(QS.Fx.Logging.ILogger logger, QS.Fx.Network.NetworkAddress address, CreateCallback createCallback)
            : this(logger, address.HostIPAddress, address.PortNumber, createCallback)
        {
        }

        public TCPListener(QS.Fx.Logging.ILogger logger, IPAddress networkInterface, CreateCallback createCallback)
            : this(logger, networkInterface, 0, createCallback)
        {
        }

        public TCPListener(QS.Fx.Logging.ILogger logger, IPAddress networkInterface, int portno, CreateCallback createCallback)
        {
            this.logger = logger;
            this.createCallback = createCallback;

            listeningAddress = new QS.Fx.Network.NetworkAddress(networkInterface, portno);
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool success = false;
            for (int ind = 0; !success && ind < 1000; ind++)
            {
                try
                {
                    listeningSocket.Bind(new IPEndPoint(
                        listeningAddress.HostIPAddress, listeningAddress.PortNumber = 55000 + random.Next(5000)));
                    success = true;
                }
                catch (Exception)
                {
                }
            }
            if (!success)
                throw new Exception("Could not create TCP listener, could not find an unused port.");

#if DEBUG_TCPListener
            logger.Log(this, "Listening at " + listeningAddress.ToString());
#endif

            listeningSocket.Listen(10);

            listeningSocket.BeginAccept(new AsyncCallback(this.ConnectionCallback), null);
        }

        private System.Random random = new System.Random();
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Network.NetworkAddress listeningAddress;
        private Socket listeningSocket;
        private IList<IDisposable> disposableObjects = new List<IDisposable>();
        private CreateCallback createCallback;

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return listeningAddress; }
        }

        #region ConnectionCallback

        private void ConnectionCallback(IAsyncResult asynchronousResult)
        {
            Socket socket = listeningSocket.EndAccept(asynchronousResult);

            listeningSocket.BeginAccept(new AsyncCallback(this.ConnectionCallback), null);

            IConnection connection = new TCPConnection(logger, 
                new QS.Fx.Network.NetworkAddress(((IPEndPoint)socket.LocalEndPoint).Address, ((IPEndPoint)socket.LocalEndPoint).Port),
                new QS.Fx.Network.NetworkAddress(((IPEndPoint)socket.RemoteEndPoint).Address, ((IPEndPoint)socket.RemoteEndPoint).Port),
                socket);

            IAsynchronousObject client = createCallback(connection.RemoteAddress, connection);
            connection.LocalObject = client;

            lock (this)
            {
                disposableObjects.Add(connection);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                lock (this)
                {
                    try
                    {
                        listeningSocket.Close();
                    }
                    catch (Exception)
                    {
                    }

                    foreach (System.IDisposable obj in disposableObjects)
                    {
                        try
                        {
                            obj.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
