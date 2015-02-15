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
    public class CompatibilityWrapper : INetwork
    {
        public CompatibilityWrapper(Devices_3_.INetwork network)
        {
            this.network = network;
            foreach (Devices_3_.INetworkInterface networkInterface in network.NICs)
                connections.Add(networkInterface.Address, new Connection(this, networkInterface));
        }

        private Devices_3_.INetwork network;
        private IDictionary<IPAddress, Connection> connections = new Dictionary<IPAddress, Connection>();

        #region Class Connection

        private class Connection : INetworkConnection
        {
            public Connection(CompatibilityWrapper owner, Devices_3_.INetworkInterface networkInterface)
            {
                this.owner = owner;
                this.networkInterface = networkInterface;
                device = networkInterface[QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];
            }

            private CompatibilityWrapper owner;
            private Devices_3_.INetworkInterface networkInterface;
            private Devices_3_.ICommunicationsDevice device;
            private IDictionary<QS.Fx.Network.NetworkAddress, Sender> senders = new Dictionary<QS.Fx.Network.NetworkAddress, Sender>();

            #region Sender

            private class Sender : ISender
            {
                public Sender(Connection owner, QS.Fx.Network.NetworkAddress destinationAddress)
                {
                    this.owner = owner;
                    this.destinationAddress = destinationAddress;
                    underlyingSender = owner.device.GetSender(destinationAddress);
                }

                private Connection owner;
                private QS.Fx.Network.NetworkAddress destinationAddress;
                private Devices_3_.ISender underlyingSender;

                #region Class Channel

                private class Channel : Base4_.IChannel
                {
                    public Channel(Sender owner, Devices_3_.ISender sender, uint maximumSize,
                        QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> getObjectCallback)
                    {
                        this.owner = owner;
                        this.getObjectCallback = getObjectCallback;
                        this.sender = sender;
                        this.maximumSize = maximumSize;
                    }

                    private Sender owner;
                    private QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> getObjectCallback;
                    private uint maximumSize;
                    private Devices_3_.ISender sender;
                
                    #region IChannel Members

                    void  QS._qss_c_.Base4_.IChannel.Signal()
                    {
                        Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> workingQueue =
                            new Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>();

                        while (getObjectCallback(ref workingQueue, maximumSize))
                            ;

                        foreach (QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>> element in workingQueue)
                        {
                            sender.send(element.EncapsulatedObject);
                            if (element.CompletionCallback != null)
                                element.CompletionCallback(true, null, element.AsynchronousState);
                        }
                    }

                    #endregion

                    #region IDisposable Members

                    void  IDisposable.Dispose()
                    {
                    }

                    #endregion
                }

                #endregion

                #region ISink<Asynchronous<IList<QS.Fx.Base.Block>>> Members

                QS._qss_c_.Base4_.IChannel
                    QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>.Register(
                    QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> getObjectCallback)
                {
                    return new Channel(this, underlyingSender, (uint)owner.device.MTU, getObjectCallback);
                }

                uint QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>.MTU
                {
	                get { return (uint) owner.device.MTU; }
                }

                #endregion
            
                #region ISender Members

                void ISender.Send(IList<QS.Fx.Base.Block> segments)
                {
                    throw new NotImplementedException();
                }

                INetworkConnection  ISender.NetworkConnection
                {
	                get { return owner; }
                }

/*
                QS.Fx.Network.NetworkAddress  ISender.Address
                {
	                get { return destinationAddress; }
                }
*/

                #endregion

                #region IAddressedSink<NetworkAddress,Asynchronous<IList<ArraySegment<byte>>>> Members

                QS.Fx.Network.NetworkAddress QS._qss_c_.Base4_.IAddressedSink<QS.Fx.Network.NetworkAddress, QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>.Address
                {
	                get { return destinationAddress; }
                }

                #endregion
            }

            #endregion

            #region Class Listener

            private class Listener : IListener, Devices_3_.IReceiver
            {
                public Listener(Connection owner, QS.Fx.Network.NetworkAddress receivingAddress, ReceiveCallback receiveCallback) 
                {
                    this.owner = owner;
                    this.receivingAddress = receivingAddress;
                    this.receiveCallback = receiveCallback;
                }

                private Connection owner;
                private QS.Fx.Network.NetworkAddress receivingAddress;
                private ReceiveCallback receiveCallback;
                private Devices_3_.IListener underlyingListener;
            
                public Devices_3_.IListener UnderlyingListener
                {
                    get { return underlyingListener; }
                    set { underlyingListener = value; }
                }

                #region IListener Members

                INetworkConnection  IListener.NetworkConnection
                {
	                get { return owner; }
                }

                QS.Fx.Network.NetworkAddress  IListener.Address
                {
	                get { return receivingAddress; }
                }

                #endregion

                #region IDisposable Members

                void  IDisposable.Dispose()
                {
 	                underlyingListener.Dispose();
                }

                #endregion
            
                #region IReceiver Members

                void  QS._qss_c_.Devices_3_.IReceiver.receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
                {
 	                receiveCallback(this, sourceAddress.HostIPAddress, sourceAddress.PortNumber, data);
                }

                #endregion
            }

            #endregion

            #region INetworkConnection Members

            IListener INetworkConnection.Register(QS.Fx.Network.NetworkAddress receivingAddress, ReceiveCallback receiveCallback)
            {
                Listener listener = new Listener(this, receivingAddress, receiveCallback);
                listener.UnderlyingListener = device.ListenAt(receivingAddress, listener);
                return listener;
            }

            void INetworkConnection.Unregister(IListener listener)
            {
                if (listener is Listener)
                    ((Listener) listener).UnderlyingListener.Dispose();
                else
                    throw new ArgumentException("Wrong object type.");
            }

            ISender INetworkConnection.this[QS.Fx.Network.NetworkAddress destinationAddress]
            {
                get 
                { 
                    lock (this)
                    {
                        if (senders.ContainsKey(destinationAddress))
                            return senders[destinationAddress];
                        else
                        {
                            Sender sender = new Sender(this, destinationAddress);
                            senders[destinationAddress] = sender;
                            return sender;
                        }
                    }
                }
            }

            IPAddress INetworkConnection.Address
            {
                get { return networkInterface.Address; }
            }

            INetwork INetworkConnection.Network
            {
                get { return owner; }
            }

            void INetworkConnection.ReleaseResources()
            {
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
                    if (connections.ContainsKey(ipAddress))
                        return connections[ipAddress];
                    else
                        throw new Exception("No such connection.");
                }
            }
        }

        IEnumerable<INetworkConnection> INetwork.Resolve(QS._qss_c_.Base1_.Subnet subnetAddress)
        {
            throw new NotImplementedException();
        }

        IEnumerable<INetworkConnection> INetwork.Resolve(System.Net.IPAddress destinationAddress)
        {
            throw new NotImplementedException();
        }

        void INetwork.ReleaseResources()
        {            
        }

        #endregion

        #region IEnumerable<INetworkConnection> Members

        IEnumerator<INetworkConnection> IEnumerable<INetworkConnection>.GetEnumerator()
        {
            List<INetworkConnection> result = new List<INetworkConnection>();
            lock (this)
            {
                foreach (Connection connection in connections.Values)
                    result.Add(connection);
            }
            return result.GetEnumerator();
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
