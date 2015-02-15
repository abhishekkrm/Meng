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

#define DEBUG_MulticastResponder

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_c_.Connections_
{
    public class MulticastResponder : System.IDisposable
    {
        public static readonly QS.Fx.Network.NetworkAddress DefaultAddress = new QS.Fx.Network.NetworkAddress("224.99.99.99:65000");

        public MulticastResponder(QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Serialization.ISerializable responseObject) 
            : this(platform, responseObject, DefaultAddress)
        {
        }

        public MulticastResponder(QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Serialization.ISerializable responseObject, 
            QS.Fx.Network.NetworkAddress listeningAddress)
        {
            this.response = QS._core_c_.Base3.Serializer.ToSegments(responseObject);
            this.logger = platform.Logger;
            foreach (QS._qss_c_.Devices_4_.INetworkConnection connection in platform.NetworkConnections)
            {
                listeners.Add(connection.Register(listeningAddress, new QS._qss_c_.Devices_4_.ReceiveCallback(this.ReceiveCallback)));
            }
        }

        public MulticastResponder(QS.Fx.Logging.ILogger logger, QS.Fx.Network.NetworkAddress listeningAddress,
            IEnumerable<KeyValuePair<QS._qss_c_.Devices_4_.INetworkConnection, QS.Fx.Serialization.ISerializable>> responseObjects)
        {
            this.logger = logger;
            foreach (KeyValuePair<QS._qss_c_.Devices_4_.INetworkConnection, QS.Fx.Serialization.ISerializable> element in responseObjects)
            {
#if DEBUG_MulticastResponder
                logger.Log(this, "Responding at " + element.Key.Address.ToString() + " with " + element.Value.ToString()); 
#endif

                listeners.Add(element.Key.Register(listeningAddress, new QS._qss_c_.Devices_4_.ReceiveCallback(
                    delegate(QS._qss_c_.Devices_4_.IListener listener,
                        IPAddress sourceIPAddress, int sourcePortNumber, ArraySegment<byte> segmentOfData)
                    {
                        try
                        {
                            QS.Fx.Network.NetworkAddress responseAddress =
                                QS._core_c_.Base3.Serializer.FromSegment(segmentOfData) as QS.Fx.Network.NetworkAddress;

#if DEBUG_MulticastResponder
                        logger.Log(this, "Responding to " + responseAddress.ToString());
#endif

                            if (responseAddress != null)
                                listener.NetworkConnection[responseAddress].Send(QS._core_c_.Base3.Serializer.ToSegments(element.Value));
                        }
                        catch (Exception)
                        {
                        }
                    })));
            }
        }

        private QS.Fx.Logging.ILogger logger;
        private IList<QS.Fx.Base.Block> response;
        private IList<QS._qss_c_.Devices_4_.IListener> listeners = new List<QS._qss_c_.Devices_4_.IListener>();

        #region Receive Callbacks

        private void ReceiveCallback(QS._qss_c_.Devices_4_.IListener listener, 
            IPAddress sourceIPAddress, int sourcePortNumber, ArraySegment<byte> segmentOfData)
        {
            QS.Fx.Network.NetworkAddress responseAddress = 
                QS._core_c_.Base3.Serializer.FromSegment(segmentOfData) as QS.Fx.Network.NetworkAddress;
            if (responseAddress != null)
                listener.NetworkConnection[responseAddress].Send(response);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                foreach (QS._qss_c_.Devices_4_.IListener listener in listeners)
                {
                    try
                    {
                        listener.Dispose();
                    }
                    catch (Exception)
                    {
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
