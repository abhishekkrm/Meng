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

namespace QS._qss_c_.Devices_4_
{
/*
    public class UDPCommunicationsDevice         
    {
        private const uint DefaultMaximumTransmissionUnit = 5000;
        private const uint DefaultMaximumNumberOfConcurrentSendOperations = 20;

        public UDPCommunicationsDevice()
            : this(DefaultMaximumTransmissionUnit, DefaultMaximumNumberOfConcurrentSendOperations)
        {
        }

        public UDPCommunicationsDevice(uint maximumTransmissionUnit, uint maximumNumberOfConcurrentSendOperations)
        {
            this.maximumTransmissionUnit = maximumTransmissionUnit;
            this.maximumNumberOfConcurrentSendOperations = maximumNumberOfConcurrentSendOperations;
        }

        private uint maximumTransmissionUnit, maximumNumberOfConcurrentSendOperations, 
            numberOfConcurrentSendOperations = 0;
        private Queue<Sender> pendingQueue = new Queue<Sender>();
        private System.Collections.Generic.IDictionary<Base3.Address, Sender> senders =
            new System.Collections.Generic.Dictionary<Base3.Address, Sender>();

        #region Internal Processing

        private void Signaled(Sender sender)
        {
            lock (this)
            {
                pendingQueue.Enqueue(sender);
                ProcessQueue();
            }
        }

        private void ProcessQueue()
        {            
            while (numberOfConcurrentSendOperations < maximumNumberOfConcurrentSendOperations && pendingQueue.Count > 0)
            {
                Sender sender = pendingQueue.Dequeue();

                bool sent;
                Monitor.Exit(this);
                try
                {
                    sent = sender.Send(maximumTransmissionUnit);
                }
                finally
                {
                    sent = false;
                    Monitor.Enter(this);
                }

                if (sent)
                {
                    numberOfConcurrentSendOperations++;
                    pendingQueue.Enqueue(sender);
                }
            }
        }

        public void Completed()
        {
            lock (this)
            {
                numberOfConcurrentSendOperations--;
                ProcessQueue();
            }
        }

        #endregion

        #region Class Sender

        private class Sender : Base4.IAddressedSink<Base3.Address, QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>>
        {
            public Sender(UDPCommunicationsDevice owner, IPAddress sourceAddress, Base3.Address destinationAddress)
            {
                this.owner = owner;
                this.sourceAddress = sourceAddress;
                this.destinationAddress = destinationAddress;
                destinationNetworkAddress = (QS.Fx.Network.NetworkAddress) destinationAddress;
            }

            private UDPCommunicationsDevice owner;
            private IPAddress sourceAddress;
            private Base3.Address destinationAddress;
            private QS.Fx.Network.NetworkAddress destinationNetworkAddress;
            private bool signaled = false;
            private Queue<Channel> pendingQueue = new Queue<Channel>();
            private Queue<SendOperation> operationQueue = new Queue<SendOperation>();

            #region SendOperation

            private class SendOperation
            {
                public SendOperation(Socket socket)
                {
                    this.socket = socket;
                }

                private Socket socket;
                private Base4.Callback completionCallback;
                private object asynchronousState;

                public void Clear()
                {
                    completionCallback = null;
                    asynchronousState = null;
                }

                public Socket Socket
                {
                    get { return socket; }
                }

                public Base4.Callback Callback
                {
                    get { return completionCallback; }
                    set { completionCallback = value; }
                }

                public object AsynchronousState
                {
                    get { return asynchronousState; }
                    set { asynchronousState = value; }
                }
            }

            #endregion

            #region Sending

            private void Send(Base4.Asynchronous<IList<ArraySegment<byte>>> toSend)
            {
                SendOperation sendOperation = this.AllocateOperation;

                try
                {
                    sendOperation.Callback = toSend.Callback;
                    sendOperation.AsynchronousState = toSend.AsynchronousState;

                    IAsyncResult asynchronousSend = sendOperation.Socket.BeginSend(
                        toSend.EncapsulatedObject, SocketFlags.None, new AsyncCallback(this.Callback), sendOperation);

                    if (asynchronousSend.CompletedSynchronously)
                    {
                        Recycle(sendOperation);
                        owner.Completed();

                        toSend.Callback(true, null, toSend.AsynchronousState);
                    }
                }
                catch (Exception exc)
                {
                    Recycle(sendOperation);
                    owner.Completed();

                    toSend.Callback(false, exc, toSend.AsynchronousState);
                }
            }

            private void Callback(IAsyncResult asynchronousResult)
            {
                SendOperation sendOperation = (SendOperation) asynchronousResult.AsyncState;

                sendOperation.Callback(true, null, sendOperation.AsynchronousState);

                Recycle(sendOperation);
                owner.Completed();
            }

            #endregion

            #region Allocating Operations

            private void Recycle(SendOperation sendOperation)
            {
                lock (operationQueue)
                {
                    sendOperation.Clear();
                    operationQueue.Enqueue(sendOperation);
                }
            }

            private SendOperation AllocateOperation
            {
                get
                {
                    lock (operationQueue)
                    {
                        if (operationQueue.Count > 0)
                            return operationQueue.Dequeue();
                        else
                        {
                            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            IPEndPoint endpoint = new IPEndPoint(sourceAddress, 0);
                            socket.Bind(endpoint);

                            if (destinationNetworkAddress.HostIPAddress.Equals(IPAddress.Broadcast))
                                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                            else
                            {
                                byte firstbyte = (destinationNetworkAddress.HostIPAddress.GetAddressBytes())[0];
                                if (firstbyte > 223 && firstbyte < 240)
                                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                            }

                            socket.Connect(destinationNetworkAddress.HostIPAddress, destinationNetworkAddress.PortNumber);

                            return new SendOperation(socket);
                        }
                    }
                }
            }

            #endregion

            #region Managing Pending Queue

            private void Signaled(Channel channel)
            {
                bool signalOwner;
                lock (pendingQueue)
                {
                    pendingQueue.Enqueue(channel);
                    if (signalOwner = !signaled)
                        signaled = true;
                }

                if (signalOwner)
                    owner.Signaled(this);
            }

            public bool Send(uint maximumSize)
            {
                Nullable<Base4.Asynchronous<IList<ArraySegment<byte>>>> toSend = null;
                lock (pendingQueue)
                {
                    while (!toSend.HasValue && pendingQueue.Count > 0)
                    {
                        Channel channel = pendingQueue.Dequeue();
                        if (channel.Source.Ready)
                        {
                            Base4.Asynchronous<IList<ArraySegment<byte>>> request = channel.Source.Get(maximumSize);
                            if (request.EncapsulatedObject != null)
                            {
                                toSend = request;
                                if (channel.Source.Ready)
                                    pendingQueue.Enqueue(channel);
                            }
                        }
                    }

                    if (!toSend.HasValue)
                        signaled = false;
                }

                if (toSend.HasValue)
                {
                    Send(toSend.Value);
                    return true;
                }
                else
                    return false;
            }

            #endregion

            #region IAddressedSink<Address,Asynchronous<IList<ArraySegment<byte>>>> Members

            QS.CMS.Base3.Address QS.CMS.Base4.IAddressedSink<QS.CMS.Base3.Address, QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>>.Address
            {
                get { return destinationAddress; }
            }

            #endregion

            #region ISink<Asynchronous<IList<ArraySegment<byte>>>> Members

            QS.CMS.Base4.IChannel QS.CMS.Base4.ISink<QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>>.Register(
                QS.CMS.Base4.ISource<QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>> source)
            {
                return new Channel(this, source);
            }

            uint QS.CMS.Base4.ISink<QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>>.MTU
            {
                get { return owner.maximumTransmissionUnit; }
            }

            #endregion

            #region Class Channel

            private class Channel : Base4.IChannel
            {
                public Channel(Sender owner, Base4.ISource<QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>> source)
                {
                    this.owner = owner;
                    this.source = source;
                }

                private Sender owner;
                private Base4.ISource<QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>> source;

                public Base4.ISource<QS.CMS.Base4.Asynchronous<IList<ArraySegment<byte>>>> Source
                {
                    get { return source; }
                }

                #region IChannel Members

                void QS.CMS.Base4.IChannel.Signal()
                {
                    owner.Signaled(this);
                }

                #endregion

                #region IDisposable Members

                void IDisposable.Dispose()
                {
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
*/ 
}
