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

// #define DEBUG_Root

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._qss_c_.Base4_
{
    public class Root : IRoot
    {
        public Root(QS.Fx.Logging.ILogger logger, Devices_4_.INetwork network, QS._core_c_.Base3.Incarnation incarnation)
        {
            this.logger = logger;
            this.network = network;
            this.incarnation = incarnation;
        }

        private QS.Fx.Logging.ILogger logger;
        private Devices_4_.INetwork network;
        private QS._core_c_.Base3.Incarnation incarnation;
        private IDictionary<IPAddress, IConnection> connections = new Dictionary<IPAddress, IConnection>();

        #region Class Connection

        private class Connection : IConnection
        {
            public Connection(Root owner, QS.Fx.Network.NetworkAddress localAddress)
            {
                this.owner = owner;
                this.localAddress = localAddress;
            }

            private Root owner;
            private QS.Fx.Network.NetworkAddress localAddress;
            private IDictionary<QS.Fx.Network.NetworkAddress, Sender> senders = new Dictionary<QS.Fx.Network.NetworkAddress, Sender>();

            #region IConnection Members

            QS.Fx.Network.NetworkAddress IConnection.Address
            {
                get { return localAddress; }
            }

            IAddressedSink<QS.Fx.Network.NetworkAddress, Asynchronous<QS._core_c_.Base3.Message>>
                ISinkCollection<QS.Fx.Network.NetworkAddress, Asynchronous<QS._core_c_.Base3.Message>>.this[QS.Fx.Network.NetworkAddress destinationAddress]
            {
                get 
                {
                    lock (this)
                    {
                        if (senders.ContainsKey(destinationAddress))
                            return senders[destinationAddress];
                        else
                        {
                            Sender sender = new Sender(owner, localAddress, destinationAddress);
                            senders[destinationAddress] = sender;
                            return sender;
                        }
                    }                    
                }
            }

            #endregion
        }

        #endregion

        #region Class Sender

        private class Sender : Base4_.IAddressedSink<QS.Fx.Network.NetworkAddress, Asynchronous<QS._core_c_.Base3.Message>> 
        {
            private const int HeaderOverhead = 4 * sizeof(uint) + sizeof(ushort);

            public Sender(Root owner, QS.Fx.Network.NetworkAddress localAddress, QS.Fx.Network.NetworkAddress destinationAddress)
            {
                this.owner = owner;
                this.incarnationSeqNo = owner.incarnation.SeqNo;
                this.localport = (uint) localAddress.PortNumber;
                this.underlyingSender = owner.network[localAddress.HostIPAddress][destinationAddress];
                this.outgoingChannel = underlyingSender.Register(
                    new GetObjectsCallback<Asynchronous<IList<QS.Fx.Base.Block>>>(this.GetObjectsCallback));
                this.destinationAddress = destinationAddress;
            }

            private Root owner;
            private Devices_4_.ISender underlyingSender;
            private uint localport, incarnationSeqNo;
            private QS.Fx.Network.NetworkAddress destinationAddress;
            private Base4_.IChannel outgoingChannel;
            private Queue<ISource<Asynchronous<QS._core_c_.Base3.Message>>> pendingQueue = 
                new Queue<ISource<Asynchronous<QS._core_c_.Base3.Message>>>();
            private bool waiting = false;
            private Queue<Asynchronous<QS._core_c_.Base3.Message>> internalWorkingQueue = new Queue<Asynchronous<QS._core_c_.Base3.Message>>();

            #region GetObjectsCallback

            private bool GetObjectsCallback(
                ref Queue<Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> returnedObjects, uint maximumSize)
            {
#if DEBUG_Root
                owner.logger.Log(this, "__GetObjectsCallback(" + this.destinationAddress.ToString() + ")");
#endif

                lock (this)
                {
                    bool ready = false;
                    while (!ready && pendingQueue.Count > 0)
                    {
                        ISource<Asynchronous<QS._core_c_.Base3.Message>> source = pendingQueue.Dequeue();
                        if (source.GetObjects(ref internalWorkingQueue, maximumSize - HeaderOverhead))
                        {
                            ready = true;
                            pendingQueue.Enqueue(source);
                        }
                    }

                    if (ready)
                    {
                        foreach (Asynchronous<QS._core_c_.Base3.Message> request in internalWorkingQueue)
                        {
#if DEBUG_Root
                            owner.logger.Log(this, "__GetObjectsCallback(" + this.destinationAddress.ToString() + 
                                ") : Pulled " + request.ToString());
#endif

                            IList<QS.Fx.Base.Block> segments = new List<QS.Fx.Base.Block>();

                            QS._core_c_.Base3.Message message = request.EncapsulatedObject;
                            QS.Fx.Serialization.ISerializable serializableObject = message.transmittedObject;
                            QS.Fx.Serialization.SerializableInfo info = serializableObject.SerializableInfo;

                            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) (info.HeaderSize + HeaderOverhead));
                            segments.Add(header.Block);

                            unsafe
                            {
                                fixed (byte* headerptr = header.Array)
                                {
                                    *((uint*)headerptr) = localport;
                                    *((uint*)(headerptr + sizeof(uint))) = message.destinationLOID;
                                    *((ushort*)(headerptr + 2 * sizeof(uint))) = info.ClassID;
                                    *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort))) = (uint) info.HeaderSize;
                                    *((uint*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))) = incarnationSeqNo;
                                }
                            }
                            header.consume(HeaderOverhead);
                            serializableObject.SerializeTo(ref header, ref segments);

                            returnedObjects.Enqueue(
                                new Asynchronous<IList<QS.Fx.Base.Block>>(
                                    segments, request.CompletionCallback, request.AsynchronousState));
                        }
                        internalWorkingQueue.Clear();
                    }

                    return ready;
                }
            }

            #endregion

            #region SourceCallback

            private void SourceCallback(ISource<Asynchronous<QS._core_c_.Base3.Message>> source)
            {
                bool signaled_now;
                lock (this)
                {                    
                    signaled_now = !waiting;
                    pendingQueue.Enqueue(source);
                    waiting = true;
                }

                if (signaled_now)
                    outgoingChannel.Signal();
            }

            #endregion

            #region ISink<Asynchronous<Base3.Message>> Members

            IChannel ISink<Asynchronous<QS._core_c_.Base3.Message>>.Register(
                GetObjectsCallback<Asynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
            {
                return new Channel<Asynchronous<QS._core_c_.Base3.Message>>(
                    new SourceCallback<Asynchronous<QS._core_c_.Base3.Message>>(this.SourceCallback), getObjectsCallback);
            }

            uint ISink<Asynchronous<QS._core_c_.Base3.Message>>.MTU
            {
                get { return underlyingSender.MTU - HeaderOverhead; }
            }

            #endregion

            #region IAddressedSink<NetworkAddress,Asynchronous<Message>> Members

            QS.Fx.Network.NetworkAddress IAddressedSink<QS.Fx.Network.NetworkAddress, Asynchronous<QS._core_c_.Base3.Message>>.Address
            {
                get { return destinationAddress; }
            }

            #endregion
        }

        #endregion

        #region IRoot Members

        IConnection IRoot.Connect(QS.Fx.Network.NetworkAddress localAddress)
        {
            lock (this)
            {
                if (connections.ContainsKey(localAddress.HostIPAddress))
                    throw new Exception("Already connected at this local interface.");
                else
                {
                    Connection connection = new Connection(this, localAddress);
                    connections[localAddress.HostIPAddress] = connection;
                    return connection;
                }
            }
        }

        IConnection IRoot.this[IPAddress ipAddress]
        {
            get 
            {
                lock (this)
                {
                    if (connections.ContainsKey(ipAddress))
                        return connections[ipAddress];
                    else
                        throw new Exception("No connection has been setup for this local address.");
                }
            }
        }

        IEnumerable<IConnection> IRoot.Resolve(QS._qss_c_.Base1_.Subnet subnet)
        {
            List<IConnection> result = new List<IConnection>();
            foreach (Devices_4_.INetworkConnection networkConnection in network.Resolve(subnet))
            {
                if (connections.ContainsKey(networkConnection.Address))
                    result.Add(connections[networkConnection.Address]);
            }
            return result;
        }

        IEnumerable<IConnection> IRoot.Resolve(IPAddress destinationAddress)
        {
            List<IConnection> result = new List<IConnection>();
            foreach (Devices_4_.INetworkConnection networkConnection in network.Resolve(destinationAddress))
            {
                if (connections.ContainsKey(networkConnection.Address))
                    result.Add(connections[networkConnection.Address]);
            }
            return result;
        }

        #endregion

        #region IEnumerable<IConnection> Members

        IEnumerator<IConnection> IEnumerable<IConnection>.GetEnumerator()
        {
            return connections.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return connections.Values.GetEnumerator();
        }

        #endregion
    }
}
