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

#define OPTION_DisableScatterGatherForMonoCompatibility

// #define DEBUG_AsynchronousSender
#define DEBUG_ShowingSendErrors
#define DEBUG_CheckingAssertions
// #define STATISTICS_RecordSendingTimes

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QS._qss_c_.Devices_4_
{
    public class AsynchronousSender : ISender, System.IDisposable
    {
        public AsynchronousSender(INetworkConnection networkConnection, QS.Fx.Network.NetworkAddress destinationAddress,
            uint maximumTransmissionUnit, Base4_.IConcurrencyController concurrencyController, QS.Fx.Logging.ILogger logger,
            QS.Fx.Clock.IClock clock)
        {
            this.networkConnection = networkConnection;
            this.destinationAddress = destinationAddress;
            this.maximumTransmissionUnit = maximumTransmissionUnit;
            this.concurrencyController = concurrencyController;
            this.logger = logger;
            this.clock = clock;

            this.internalChannel =
                ((QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>)this).Register(
                    new QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>(
                        this.InternalGetObjectsCallback));                
        }

        private INetworkConnection networkConnection;
        private QS.Fx.Network.NetworkAddress destinationAddress;
        private uint maximumTransmissionUnit;
        private Base4_.IConcurrencyController concurrencyController;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;

        private Queue<Channel> pendingQueue = new Queue<Channel>();
        private bool waiting = false;
        private Queue<Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> toSend =
            new Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>();
        private Queue<SendOperation> operationQueue = new Queue<SendOperation>();

        private Base4_.IChannel internalChannel;
        private Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> internalQueue =
            new Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>();

        private bool InternalGetObjectsCallback(
            ref Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> returnedObjects, uint maximumSize)
        {
            lock (internalQueue)
            {
                if (internalQueue.Count > 0)
                {
                    returnedObjects.Enqueue(internalQueue.Dequeue());
                    return true;
                }
                else
                    return false;
            }
        }

#if STATISTICS_RecordSendingTimes
        private QS.CMS.Statistics.Samples sendingTimes = new QS.CMS.Statistics.Samples();
#endif

        #region ISender Members

        void ISender.Send(IList<QS.Fx.Base.Block> segments)
        {
            lock (internalQueue)
            {
                internalQueue.Enqueue(new QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>(segments, null, null));
            }
            internalChannel.Signal();
        }

        INetworkConnection ISender.NetworkConnection
        {
            get { return networkConnection; }
        }

/*
        QS.Fx.Network.NetworkAddress ISender.Address
        {
            get { return destinationAddress; }
        }
*/

        #endregion

        #region IAddressedSink<NetworkAddress,Asynchronous<IList<QS.Fx.Base.Block>>> Members

        QS.Fx.Network.NetworkAddress QS._qss_c_.Base4_.IAddressedSink<QS.Fx.Network.NetworkAddress,
            QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>.Address
        {
            get { return destinationAddress; }
        }

        #endregion

        #region ISink<Asynchronous<IList<ArraySegment<byte>>>> Members

        QS._qss_c_.Base4_.IChannel QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>.Register(
            Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> getObjectsCallback)
        {
            return new Channel(this, getObjectsCallback);
        }

        uint QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>.MTU
        {
            get { return maximumTransmissionUnit; }
        }

        #endregion

        #region SendOperation

        private class SendOperation
        {
            public SendOperation(Socket socket)
            {
                this.socket = socket;
            }

            private Socket socket;
            private Base4_.CompletionCallback completionCallback;
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

            public Base4_.CompletionCallback CompletionCallback
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

        #region Allocating Operations

        private void Recycle(SendOperation sendOperation)
        {
            sendOperation.Clear();
            operationQueue.Enqueue(sendOperation);
        }

        private SendOperation AllocateOperation
        {
            get
            {
                if (operationQueue.Count > 0)
                    return operationQueue.Dequeue();
                else
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint endpoint = new IPEndPoint(networkConnection.Address, 0);
                    socket.Bind(endpoint);

                    if (destinationAddress.HostIPAddress.Equals(IPAddress.Broadcast))
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    else
                    {
                        byte firstbyte = (destinationAddress.HostIPAddress.GetAddressBytes())[0];
                        if (firstbyte > 223 && firstbyte < 240)
                            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                    }

                    socket.Connect(new IPEndPoint(destinationAddress.HostIPAddress, destinationAddress.PortNumber));

                    return new SendOperation(socket);
                }
            }
        }

        #endregion

        #region Internal Processing

        private void ChannelSignaled(Channel channel)
        {
#if DEBUG_AsynchronousSender
            logger.Log(this, "__ChannelSignaled(" + destinationAddress.ToString() + ") : " + channel.ToString());
#endif

            lock (this)
            {
                if (waiting)
                {
                    pendingQueue.Enqueue(channel);
                }
                else
                {
                    while (channel.GetObject(ref toSend, maximumTransmissionUnit))
                    {
#if DEBUG_CheckingAssertions
                        Debug.Assert(toSend.Count > 0);
#endif

                        if (concurrencyController.Consume(new Base3_.NoArgumentCallback(this.ConsumeCallback)))
                        {
                            Send(toSend.Dequeue());
                        }
                        else
                        {
                            pendingQueue.Enqueue(channel);
                            waiting = true;
                            break;
                        }
                    }
                }
            }
        }

        private void ConsumeCallback()
        {
            lock (this)
            {
                bool ready = true;
                while (ready)
                {
#if DEBUG_CheckingAssertions
                    Debug.Assert(toSend.Count > 0);
#endif

                    Send(toSend.Dequeue());

                    ready = toSend.Count > 0;
                    while (!ready && pendingQueue.Count > 0)
                    {                        
                        Channel channel = pendingQueue.Dequeue();
                        ready = channel.GetObject(ref toSend, maximumTransmissionUnit);
                        if (ready)
                            pendingQueue.Enqueue(channel);
                    }

                    if (ready)
                        ready = concurrencyController.Consume(new Base3_.NoArgumentCallback(this.ConsumeCallback));
                    else
                        waiting = false;
                }
            }
        }

        private void Send(Base4_.Asynchronous<IList<QS.Fx.Base.Block>> request)
        {
#if DEBUG_AsynchronousSender
            logger.Log(this, "__Send(" + destinationAddress.ToString() + ") : " + request.ToString());
#endif

            SendOperation sendOperation = this.AllocateOperation;

            try
            {
                sendOperation.CompletionCallback = request.CompletionCallback;
                sendOperation.AsynchronousState = request.AsynchronousState;

#if STATISTICS_RecordSendingTimes
                lock (sendingTimes)
                {
                    sendingTimes.addSample(clock.Time);
                }
#endif

#if OPTION_DisableScatterGatherForMonoCompatibility
                byte[] flattened_object = Base3_.BufferHelper.FlattenBuffers(request.EncapsulatedObject);
                IAsyncResult asynchronousSend = sendOperation.Socket.BeginSend(flattened_object, 0, flattened_object.Length, 
                    SocketFlags.None, new AsyncCallback(this.CompletionCallback), sendOperation);
#else
                IAsyncResult asynchronousSend = sendOperation.Socket.BeginSend(
                    request.EncapsulatedObject, SocketFlags.None, new AsyncCallback(this.CompletionCallback), sendOperation);
#endif

                if (asynchronousSend.CompletedSynchronously)
                {
                    Base4_.CompletionCallback callback = request.CompletionCallback;
                    System.Exception exception = null;
                    bool succeeded = true;
                    try
                    {
                        if (sendOperation.Socket.EndSend(asynchronousSend) <= 0)
                            throw new Exception("No bytes sent.");
                    }
                    catch (Exception exc)
                    {
                        exception = exc;
                        succeeded = false;
                    }

                    Recycle(sendOperation);

#if DEBUG_ShowingSendErrors
                    if (!succeeded)
                        logger.Log(this, "__Send: Synchronous send failed, " + exception.ToString());
#endif

                    if (callback != null)
                    {
                        Monitor.Exit(this);
                        try
                        {
                            callback(succeeded, exception, request.AsynchronousState);
                        }
#if DEBUG_ShowingSendErrors
                        catch (Exception exc)
                        {
                            logger.Log(this, "__Send: Could not invoke callback, " + exc.ToString());
#else
                        catch (Exception)
                        {
#endif
                        }
                        Monitor.Enter(this);
                    }

                    concurrencyController.Release();
                }
            }
            catch (Exception exc)
            {
#if DEBUG_ShowingSendErrors
                logger.Log(this, "__Send: Asynchronous send failed, " + exc.ToString());
#endif

                Recycle(sendOperation);
                concurrencyController.Release();

                Base4_.CompletionCallback callback = request.CompletionCallback;
                if (callback != null)
                {
                    Monitor.Exit(this);
                    try
                    {
                        callback(false, exc, request.AsynchronousState);
                    }
#if DEBUG_ShowingSendErrors
                    catch (Exception iexc)
                    {
                        logger.Log(this, "__Send: Could not invoke callback, " + iexc.ToString());
#else
                    catch (Exception)
                    {
#endif
                    }
                    Monitor.Enter(this);
                }
            }
        }

        private void CompletionCallback(IAsyncResult result)
        {
// #if DEBUG_CheckingAssertions
//            Debug.Assert(!result.CompletedSynchronously);
// #endif

            if (!result.CompletedSynchronously)
            {
                SendOperation sendOperation = (SendOperation)result.AsyncState;
                Base4_.CompletionCallback callback = sendOperation.CompletionCallback;
                System.Exception exception = null;
                bool succeeded = true;
                try
                {
                    if (sendOperation.Socket.EndSend(result) <= 0)
                        throw new Exception("No bytes sent.");
                }
                catch (Exception exc)
                {
                    exception = exc;
                    succeeded = false;
                }
                object context = sendOperation.AsynchronousState;

                lock (this)
                {
                    Recycle(sendOperation);
                }

                if (callback != null)
                    callback(succeeded, exception, context);

                concurrencyController.Release();
            }
        }

        #endregion

        #region Class Channel

        private class Channel : Base4_.IChannel
        {
            public Channel(AsynchronousSender owner, 
                Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> getObjectsCallback)
            {
                this.owner = owner;
                this.getObjectsCallback = getObjectsCallback;
            }

            private AsynchronousSender owner;
            private Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> getObjectsCallback;
            private bool signaled = false;

            public bool GetObject(ref Queue<QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>> returnedObjects, uint maximumSize)
            {
                if (getObjectsCallback(ref returnedObjects, maximumSize))
                    return true;
                else
                {
                    lock (this)
                    {
                        if (getObjectsCallback(ref returnedObjects, maximumSize))
                            return true;
                        else
                        {
                            signaled = false;
                            return false;
                        }
                    }
                }
            }

            #region IChannel Members

            void QS._qss_c_.Base4_.IChannel.Signal()
            {
                bool signaled_now;
                lock (this)
                {
                    signaled_now = !signaled;
                    signaled = true;
                }

                if (signaled_now)
                    owner.ChannelSignaled(this);
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO: We should render this sender unusable.............................................................
        }

        #endregion
    }
}
