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

#define DEBUG_MulticastCollector

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace QS._qss_c_.Connections_
{
    public class MulticastCollector : System.IDisposable
    {
        public static C Any<C>(QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Network.NetworkAddress multicastAddress, TimeSpan timeout) 
            where C : class, QS.Fx.Serialization.ISerializable
        {
            C result = null;
            using (MulticastCollector collector = new MulticastCollector(platform, multicastAddress))
            {
                Thread.Sleep(timeout);
                IEnumerator<QS.Fx.Serialization.ISerializable> response = collector.CollectedObjects.GetEnumerator();
                while (result == null && response.MoveNext())
                    result = response.Current as C;
            }
             
            return result;
        }

        public MulticastCollector(QS._qss_c_.Platform_.IPlatform platform)
            : this(platform, MulticastResponder.DefaultAddress)
        {
        }

        public MulticastCollector(QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Network.NetworkAddress multicastAddress)
        {
            foreach (QS._qss_c_.Devices_4_.INetworkConnection connection in platform.NetworkConnections)
            {
                QS._qss_c_.Devices_4_.IListener listener = connection.Register(new QS.Fx.Network.NetworkAddress(connection.Address, 0),
                    new QS._qss_c_.Devices_4_.ReceiveCallback(this.ReceiveCallback));

#if DEBUG_MulticastCollector
                platform.Logger.Log(this, "Listening at " + listener.Address.ToString());
#endif

                connection[multicastAddress].Send(QS._core_c_.Base3.Serializer.ToSegments(listener.Address));
                listeners.Add(listener);
            }
        }

        private List<QS.Fx.Serialization.ISerializable> collectedObjects = new List<QS.Fx.Serialization.ISerializable>();
        private List<QS._qss_c_.Devices_4_.IListener> listeners = new List<QS._qss_c_.Devices_4_.IListener>();

        public IEnumerable<QS.Fx.Serialization.ISerializable> CollectedObjects
        {
            get
            {
                lock (this)
                {
                    return collectedObjects.ToArray();
                }
            }
        }

        #region Receive Callbacks

        private void ReceiveCallback(QS._qss_c_.Devices_4_.IListener listener,
            IPAddress sourceIPAddress, int sourcePortNumber, ArraySegment<byte> segmentOfData)
        {
            lock (this)
            {
                collectedObjects.Add(QS._core_c_.Base3.Serializer.FromSegment(segmentOfData));
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                foreach (QS._qss_c_.Devices_4_.IListener listener in listeners)
                    listener.Dispose();
            }
        }

        #endregion
    }
}
