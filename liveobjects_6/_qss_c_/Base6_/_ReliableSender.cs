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
using System.Threading;

namespace QS._qss_c_.Base6_
{
/*
    public class ReliableSender : Source<IAsynchronous<Base3.Message>>, 
        ISource<IAsynchronous<Base3.Message>>, Base3.IReliableSerializableSender
    {
        public ReliableSender()
        {
        }

        private Queue<Request> requests = new Queue<Request>();
        private bool signaled = false;

        #region Class Request

        private class Request : IAsynchronous<Base3.Message>, Base3.IAsynchronousOperation
        {
            public Request(uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                QS.CMS.Base3.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                this.message = new QS.CMS.Base3.Message(destinationLOID, data);
                this.completionCallback = completionCallback;
                this.asynchronousState = asynchronousState;
            }

            private Base3.Message message;
            private QS.CMS.Base3.AsynchronousOperationCallback completionCallback;
            private object asynchronousState;
            private bool completed = false, succeeded;
            private ManualResetEvent completion;
            private System.Exception exception;

            private void Callback(bool succeeded, System.Exception exception, IAsynchronous<Base3.Message> request)
            {
                if (!ReferenceEquals(this, request))
                    throw new Exception("Completion callback invoked on the wrong object.");

                Base3.AsynchronousOperationCallback completionCallback = null;
                lock (this)
                {
                    if (!completed)
                    {
                        completed = true;
                        this.succeeded = succeeded;
                        this.exception = exception;

                        if (completion != null)
                            completion.Set();
                        completionCallback = this.completionCallback;
                    }
                }

                if (completionCallback != null)
                    completionCallback(this);
            }

            public void Check()
            {
                if (!succeeded)
                    throw new Exception("Could not deliver the message.", exception);
            }

            #region IAsynchronous<Message> Members

            QS.CMS.Base3.Message IAsynchronous<QS.CMS.Base3.Message>.Object
            {
                get { return message; }
            }

            Callback<IAsynchronous<QS.CMS.Base3.Message>> IAsynchronous<QS.CMS.Base3.Message>.Callback
            {
                get { return new Callback<IAsynchronous<QS.CMS.Base3.Message>>(this.Callback); }
            }

            #endregion

            #region IAsynchronousOperation Members

            void QS.CMS.Base3.IAsynchronousOperation.Cancel()
            {
                throw new NotSupportedException("This type of request cannot be cancelled.");
            }

            void QS.CMS.Base3.IAsynchronousOperation.Ignore()
            {
                lock (this)
                {
                    completionCallback = null;
                }
            }

            bool QS.CMS.Base3.IAsynchronousOperation.Cancelled
            {
                get { return false; }
            }

            #endregion

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get { return asynchronousState; }
            }

            System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
            {                
                get 
                {
                    lock (this)
                    {
                        if (completion == null)
                            completion = new ManualResetEvent(completed);
                        return completion;
                    }                    
                }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return completed; }
            }

            #endregion
        }

        #endregion

        #region ISource<IAsynchronous<Message>> Members

        void ISource<IAsynchronous<QS.CMS.Base3.Message>>.GetObjects(
            Queue<IAsynchronous<QS.CMS.Base3.Message>> objectQueue, 
            int maximumNumberOfObjects, out int numberOfObjectsReturned, out bool moreObjectsAvailable)
        {            
            lock (this)
            {
                numberOfObjectsReturned = 0;
                moreObjectsAvailable = true;
                while (true)
                {
                    if (requests.Count > 0)
                    {
                        if (numberOfObjectsReturned < maximumNumberOfObjects)
                        {
                            objectQueue.Enqueue(requests.Dequeue());
                            numberOfObjectsReturned++;
                        }
                        else
                            break;
                    }
                    else
                    {
                        moreObjectsAvailable = false;
                        signaled = false;
                        break;
                    }
                }
            }
        }

        #endregion

        #region IReliableSerializableSender Members

        QS.CMS.Base3.IAsynchronousOperation QS.CMS.Base3.IReliableSerializableSender.BeginSend(
            uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
            QS.CMS.Base3.AsynchronousOperationCallback completionCallback, object asynchronousState)
        {
            Request request = new Request(destinationLOID, data, completionCallback, asynchronousState);
            bool signal_now = false;
            lock (this)
            {
                signal_now = !signaled;
                if (signal_now && channel == null)
                    throw new Exception("Cannot send, this sender is currently not connected to any active channel.");

                requests.Enqueue(request);                
                signaled = true;
            }

            if (signal_now)
                channel.Signal();

            return request;
        }

        void QS.CMS.Base3.IReliableSerializableSender.EndSend(QS.CMS.Base3.IAsynchronousOperation asynchronousOperation)
        {
            Request request = asynchronousOperation as Request;
            if (request == null)
                throw new Exception("Wrong operation type.");

            request.Check();
        }

        #endregion

        #region ISerializableSender Members

        QS.Fx.Network.NetworkAddress QS.CMS.Base3.ISerializableSender.Address
        {
            get { throw new NotSupportedException("This operation is not valid in this context."); }
        }

        void QS.CMS.Base3.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
        {
            ((Base3.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
        }

        int QS.CMS.Base3.ISerializableSender.MTU
        {
            get { return int.MaxValue; } // needs to be changed
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            throw new NotSupportedException("This operation is not valid in this context.");
        }

        #endregion
    }
*/
}
