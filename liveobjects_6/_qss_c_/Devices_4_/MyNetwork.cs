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

namespace QS._qss_c_.Devices_4_
{
    public class MyNetwork : INetwork
    {
        public const uint DefaultMTU = 20000;

        public MyNetwork(QS.Fx.Logging.ILogger logger) : this(DefaultMTU, logger)
        {
        }

        public MyNetwork(uint maximumTransmissionUnit, QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;
            this.maximumTransmissionUnit = maximumTransmissionUnit;
            interfaceAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in interfaceAddresses)
                networkConnections.Add(address, new NetworkConnection(this, address));
        }

        private uint maximumTransmissionUnit;
        private IPAddress[] interfaceAddresses;
        private IDictionary<IPAddress, NetworkConnection> networkConnections = new Dictionary<IPAddress, NetworkConnection>();
        private Base4_.IConcurrencyController concurrencyController = new Components_2_.ConcurrencyController();
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock = QS._core_c_.Base2.PreciseClock.Clock;

        #region Class NetworkConnection

        private class NetworkConnection : INetworkConnection, IDisposable
        {
            public NetworkConnection(MyNetwork owner, IPAddress ipAddress)
            {
                this.owner = owner;
                this.ipAddress = ipAddress;
            }

            private MyNetwork owner;
            private IPAddress ipAddress;
            private bool disposed = false;
            private IDictionary<QS.Fx.Network.NetworkAddress, AsynchronousListener> listeners = 
                new Dictionary<QS.Fx.Network.NetworkAddress, AsynchronousListener>();
            private IDictionary<QS.Fx.Network.NetworkAddress, AsynchronousSender> senders =
                new Dictionary<QS.Fx.Network.NetworkAddress, AsynchronousSender>();

            #region INetworkConnection Members

            System.Net.IPAddress INetworkConnection.Address
            {
                get { return ipAddress; }
            }

            INetwork INetworkConnection.Network
            {
                get { return owner; }
            }

            IListener INetworkConnection.Register(QS.Fx.Network.NetworkAddress receivingAddress, ReceiveCallback receiveCallback)
            {
                lock (this)
                {
                    if (!disposed)
                    {
                        if (listeners.ContainsKey(receivingAddress))
                            throw new Exception("Already listening at " + receivingAddress.ToString() + " on " + ipAddress.ToString() + ".");

                        AsynchronousListener listener = new AsynchronousListener(
                            owner.logger, this, ipAddress, receivingAddress, receiveCallback, owner.maximumTransmissionUnit);
                        listeners[receivingAddress] = listener;
                        return listener;
                    }
                    else
                        throw new Base4_.DisposedException();
                }                
            }

            ISender INetworkConnection.this[QS.Fx.Network.NetworkAddress destinationAddress]
            {
                get
                {
                    lock (this)
                    {
                        if (!disposed)
                        {
                            if (senders.ContainsKey(destinationAddress))
                                return senders[destinationAddress];
                            else
                            {
                                AsynchronousSender sender = new AsynchronousSender(
                                    this, destinationAddress, owner.maximumTransmissionUnit, owner.concurrencyController, owner.logger,
                                    owner.clock);
                                senders[destinationAddress] = sender;
                                return sender;
                            }
                        }
                        else
                            throw new Base4_.DisposedException();
                    }
                }
            }

            void INetworkConnection.Unregister(IListener listener)
            {
                lock (this)
                {
                    if (senders.ContainsKey(listener.Address))
                        senders.Remove(listener.Address);
                    else
                        throw new Exception("Not listening at " + listener.Address.ToString() + " on " + ipAddress.ToString() + ".");
                }
            }

            void INetworkConnection.ReleaseResources()
            {
                List<AsynchronousListener> toUnregister = new List<AsynchronousListener>();
                lock (this)
                {
                    foreach (AsynchronousListener listener in listeners.Values)
                        toUnregister.Add(listener);
                    listeners.Clear();

                    foreach (AsynchronousSender sender in senders.Values)
                        ((IDisposable) sender).Dispose();
                    senders.Clear();
                }

                foreach (AsynchronousListener listener in toUnregister)
                    ((IDisposable) listener).Dispose();
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                bool dispose_now;
                lock (this)
                {
                    dispose_now = !disposed;
                    disposed = true;
                }

                if (dispose_now)
                    ((INetworkConnection)this).ReleaseResources();
            }

            #endregion
        }

        #endregion

        #region INetwork Members

        INetworkConnection INetwork.this[System.Net.IPAddress ipAddress]
        {
            get 
            { 
                lock (this)
                {
                    if (networkConnections.ContainsKey(ipAddress))
                        return networkConnections[ipAddress];
                    else
                        throw new Exception("No connection exists with local IP address " + ipAddress.ToString() + "."); 
                }
            }
        }

        IEnumerable<INetworkConnection> INetwork.Resolve(QS._qss_c_.Base1_.Subnet subnetAddress)
        {
            lock (this)
            {
                List<INetworkConnection> candidates = new List<INetworkConnection>();
                foreach (KeyValuePair<IPAddress, NetworkConnection> element in networkConnections)
                {
                    if (subnetAddress.contains(element.Key))
                        candidates.Add(element.Value);
                }
                return candidates;
            }
        }

        IEnumerable<INetworkConnection> INetwork.Resolve(System.Net.IPAddress destinationAddress)
        {            
            // TODO: Implement resolving of a destination address to local IP of an interface that can send there.
            throw new Exception("The method or operation is not implemented.");
        }

        void INetwork.ReleaseResources()
        {
            lock (this)
            {
                foreach (NetworkConnection networkConnection in networkConnections.Values)
                    ((IDisposable) networkConnection).Dispose();

                networkConnections.Clear();
                foreach (IPAddress address in interfaceAddresses)
                    networkConnections.Add(address, new NetworkConnection(this, address));
            }            
        }

        #endregion

        #region IEnumerable<INetworkConnection> Members

        IEnumerator<INetworkConnection> IEnumerable<INetworkConnection>.GetEnumerator()
        {
            lock (this)
            {
                List<INetworkConnection> result = new List<INetworkConnection>();
                foreach (INetworkConnection connection in networkConnections.Values)
                    result.Add(connection);
                return result.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<INetworkConnection>)this).GetEnumerator();
        }

        #endregion
    }
}
