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
    public class TCPServer : System.IDisposable
    {
        public TCPServer(
            QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Network.NetworkAddress multicastAddress, CreateCallback createCallback)
        {
            this.logger = platform.Logger;
            this.createCallback = createCallback;

            List<KeyValuePair<QS._qss_c_.Devices_4_.INetworkConnection, QS.Fx.Serialization.ISerializable>> responderAddresses =
                new List<KeyValuePair<QS._qss_c_.Devices_4_.INetworkConnection,QS.Fx.Serialization.ISerializable>>();
            foreach (QS._qss_c_.Devices_4_.INetworkConnection networkConnection in platform.NetworkConnections)
            {
                TCPListener listener = new TCPListener(platform.Logger, networkConnection.Address, createCallback);
                listeners.Add(listener);
                responderAddresses.Add(new KeyValuePair<QS._qss_c_.Devices_4_.INetworkConnection,QS.Fx.Serialization.ISerializable>(
                    networkConnection, listener.Address));
            }

            responder = new MulticastResponder(platform.Logger, multicastAddress, responderAddresses);
        }

        public TCPServer(QS.Fx.Logging.ILogger logger, CreateCallback createCallback)
        {
            this.logger = logger;
            this.createCallback = createCallback;

            foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName())) 
            {
                listeners.Add(new TCPListener(logger, address, createCallback));
            }
        }

        private QS.Fx.Logging.ILogger logger;
        private CreateCallback createCallback;
        private IList<TCPListener> listeners = new List<TCPListener>();
        private MulticastResponder responder;

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {
            try
            {
                lock (this)
                {
                    foreach (TCPListener listener in listeners)
                    {
                        try
                        {
                            ((IDisposable)listener).Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (responder != null)
                    {
                        try
                        {
                            ((System.IDisposable)responder).Dispose();
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
