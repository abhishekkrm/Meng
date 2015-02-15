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

// #define DEBUG_TCPConnection

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QS._qss_c_.Connections_
{
    public class TCPConnection : IConnection
    {
        public TCPConnection(
            QS.Fx.Logging.ILogger logger, QS.Fx.Network.NetworkAddress localAddress, QS.Fx.Network.NetworkAddress remoteAddress, Socket socket)
        {
            this.logger = logger;
            this.localAddress = localAddress;
            this.remoteAddress = remoteAddress;
            this.socket = socket;

#if DEBUG_TCPConnection
            logger.Log(this, "Connected: " + localAddress.ToString() + " - " + remoteAddress.ToString());
#endif

            socket.BeginReceive(
                incomingSize, 0, incomingSize.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), null);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Network.NetworkAddress localAddress, remoteAddress;
        private Socket socket;
        private IAsynchronousObject localObject;
        private byte[] incomingSize = new byte[sizeof(int)];
        // private Mutex outgoingLock = new Mutex();
        private uint lastAssignedSequenceNo;
        private IDictionary<uint, Request> requests = new Dictionary<uint, Request>();
        private bool disconnecting, disconnected;

        #region Class Request

        private class Request : IAsyncResult
        {
            public Request(QS.Fx.Serialization.ISerializable argumentObject, AsyncCallback callback, object state)
            {
                this.argumentObject = argumentObject;
                this.callback = callback;
                this.state = state;
            }

            private QS.Fx.Serialization.ISerializable argumentObject, responseObject;
            private AsyncCallback callback;
            private object state;
            private bool isCompleted, succeeded;
            private ManualResetEvent completedEvent;
            private System.Exception exception;

            #region Accessors

            public void Completed(bool succeeded, QS.Fx.Serialization.ISerializable responseObject, System.Exception exception)
            {
                lock (this)
                {
                    if (isCompleted)
                        throw new Exception("Already marked as completed.");

                    isCompleted = true;
                    if (completedEvent != null)
                        completedEvent.Set();

                    this.succeeded = succeeded;
                    this.responseObject = responseObject;
                    this.exception = exception;                    
                }

                if (callback != null)
                    callback(this);
            }

            public QS.Fx.Serialization.ISerializable Result
            {
                get
                {
                    lock (this)
                    {
                        if (!isCompleted)
                            throw new Exception("Not completed!");

                        if (succeeded)
                            return responseObject;
                        else
                            throw exception;
                    }
                }
            }

            #endregion

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get { return state; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get 
                {
                    lock (this)
                    {
                        if (completedEvent == null)
                            completedEvent = new ManualResetEvent(isCompleted);
                    }

                    return completedEvent;
                }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return isCompleted; }
            }

            #endregion
        }

        #endregion

        #region IAsynchronousRef Members

        void IAsynchronousRef.Call(QS.Fx.Serialization.ISerializable argumentObject)
        {
            if (disconnecting || disconnected)
                throw new Exception("Cannot call, disconnecting or disconnected.");

            SendObject(new Message(argumentObject));
        }

        QS.Fx.Serialization.ISerializable IAsynchronousRef.SynchronousCall(QS.Fx.Serialization.ISerializable argumentObject)
        {            
            IAsyncResult result = ((IAsynchronousRef) this).BeginCall(argumentObject, null, null);
            result.AsyncWaitHandle.WaitOne();
            return ((IAsynchronousRef) this).EndCall(result);
        }

        IAsyncResult IAsynchronousRef.BeginCall(
            QS.Fx.Serialization.ISerializable argumentObject, AsyncCallback callback, object state)
        {
            if (disconnecting || disconnected)
                throw new Exception("Cannot call, disconnecting or disconnected.");

            Request request = new Request(argumentObject, callback, state);
            uint sequenceNo;
            lock (this)
            {
                sequenceNo = ++lastAssignedSequenceNo;
                requests.Add(sequenceNo, request);
            }

            SendObject(new Message(sequenceNo, argumentObject));

            return request;
        }

        QS.Fx.Serialization.ISerializable IAsynchronousRef.EndCall(IAsyncResult result)
        {
            Request request = result as Request;
            if (request == null)
                throw new ArgumentException();

            return request.Result;
        }

        #endregion

        #region ReceiveObject

        private void ReceiveObject(QS.Fx.Serialization.ISerializable incomingObject)
        {
#if DEBUG_TCPConnection
            logger.Log(this, "__ReceiveObject: " + Helpers.ToString.Object(incomingObject));
#endif

            try
            {
                Message message = incomingObject as Message;
                if (message == null)
                    throw new Exception("Received an object of a wrong type.");

                switch (message.TypeOf)
                {
                    case Message.Type.Request:
                        {
                            localObject.AsynchronousCall(message.ArgumentObject, new ResponseCallback(
                                delegate(bool succeeded, QS.Fx.Serialization.ISerializable responseObject, Exception exception)
                                {
                                    SendObject(new Message(message.SequenceNo, succeeded, responseObject, exception));
                                }));
                        }
                        break;

                    case Message.Type.Response:
                        {
                            Request request;
                            lock (this)
                            {
                                if (!requests.ContainsKey(message.SequenceNo))
                                    throw new Exception("No such request.");

                                request = requests[message.SequenceNo];
                                requests.Remove(message.SequenceNo);
                            }

                            request.Completed(message.Succeeded, message.ArgumentObject, message.Exception);
                        }
                        break;

                    case Message.Type.OneWayRequest:
                        {
                            localObject.AsynchronousCall(message.ArgumentObject);
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, "__ReceiveObject : " + exc.ToString());
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            disconnecting = true;

            try
            {
                socket.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Sending

        private void SendObject(QS.Fx.Serialization.ISerializable outgoingObject)
        {
#if DEBUG_TCPConnection
            logger.Log(this, "__SendObject: " + Helpers.ToString.Object(outgoingObject));
#endif

            // if (!outgoingLock.WaitOne())
            //    throw new Exception("Could not send, cannot lock.");

            // try
            // {

            lock (this)
            {
                IList<QS.Fx.Base.Block> dataSegments = QS._core_c_.Base3.Serializer.ToSegments(outgoingObject);
                int size = 0;
                foreach (QS.Fx.Base.Block segment in dataSegments)
                    size += (int) segment.size;

                byte[] outgoingSize = new byte[sizeof(int)];
                unsafe
                {
                    fixed (byte* pbuffer = outgoingSize)
                    {
                        *((int*)pbuffer) = size;
                    }
                }

                List<ArraySegment<byte>> segments = new List<ArraySegment<byte>>();
                segments.Add(new ArraySegment<byte>(outgoingSize));
                foreach (QS.Fx.Base.Block ss in dataSegments)
                {
                    if ((ss.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && ss.buffer != null)
                        segments.Add(new ArraySegment<byte>(ss.buffer, (int)ss.offset, (int)ss.size));
                    else
                        throw new Exception("Unmanaged memory is not suported here.");
                }

                // socket.BeginSend(segments, SocketFlags.None, new AsyncCallback(this.SendCallback), null);
                socket.Send(segments);
            }

            // }
            // catch (Exception)
            // {
            //    outgoingLock.ReleaseMutex();
            //    throw;
            // }
        }

        #endregion

        #region SendCallback

        private void SendCallback(IAsyncResult asynchronousResult)
        {
            // outgoingLock.ReleaseMutex();
        }

        #endregion

        #region ReceiveCallback

        private void ReceiveCallback(IAsyncResult asynchronousResult)
        {
            QS.Fx.Serialization.ISerializable receivedObject = null;

            try
            {
                if (socket.EndReceive(asynchronousResult) < incomingSize.Length)
                    throw new Exception("Could not process incoming message, not enough data.");

                int count;
                unsafe
                {
                    fixed (byte* pbuffer = incomingSize)
                    {
                        count = *((int*)pbuffer);
                    }
                }

                byte[] buffer = new byte[count];
                int nreceived = 0;
                do
                {
                    int nreceived_now = socket.Receive(buffer, nreceived, count - nreceived, SocketFlags.None);
                    if (nreceived_now <= 0)
                        throw new Exception("Could not receive, less data arrived than specified in the header.");
                    nreceived += nreceived_now;
                }
                while (nreceived < count);

#if DEBUG_TCPConnection
                logger.Log(this, "__ReceiveCallback: " + count.ToString() + " bytes");
#endif

                receivedObject = QS._core_c_.Base3.Serializer.FromSegment(new ArraySegment<byte>(buffer));
            }
#if DEBUG_TCPConnection
            catch (Exception exc)
#else
            catch (Exception)
#endif
            {
                try
                {
                    socket.Close();
                }
                catch (Exception)
                {
                }

                disconnected = true;
                // if (!disconnecting)
                //    logger.Log(this, "Connection was terminated by the other side.");

#if DEBUG_TCPConnection
                logger.Log(this, "__ReceiveCallback : " + exc.ToString());
#endif
            }

            if (!disconnected)
                socket.BeginReceive(
                    incomingSize, 0, incomingSize.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), null);

            if (receivedObject != null)
                this.ReceiveObject(receivedObject);
        }

        #endregion

        #region IConnection Members

        IAsynchronousObject IConnection.LocalObject
        {
            get { return localObject; }
            set { localObject = value; }
        }

        QS.Fx.Network.NetworkAddress IConnection.LocalAddress
        {
            get { return localAddress; }
        }

        QS.Fx.Network.NetworkAddress IConnection.RemoteAddress
        {
            get { return remoteAddress; }
        }

        #endregion
    }
}
