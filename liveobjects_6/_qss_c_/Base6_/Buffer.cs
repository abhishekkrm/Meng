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

#define DEBUG_CollectStatistics
// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Base6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Buffer : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public Buffer(QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink, QS.Fx.Clock.IClock clock)
        {
            this.sink = sink;
            this.clock = clock;

#if DEBUG_CollectStatistics
            if (timeSeries_bufferSizes.Enabled)
                timeSeries_bufferSizes.Add(clock.Time, 0);
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private QS.Fx.Clock.IClock clock;
        private Queue<Request> pendingQueue = new Queue<Request>();
        private bool registered;
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink;

#if DEBUG_CollectStatistics
        [QS._core_c_.Diagnostics2.Property("BufferSizes")]
        private QS._qss_c_.Statistics_.Samples2D timeSeries_bufferSizes = new QS._qss_c_.Statistics_.Samples2D();

        [QS._core_c_.Diagnostics2.Property("InsertionTimes")]
        private QS._qss_c_.Statistics_.Samples1D timeSeries_insertionTimes = new QS._qss_c_.Statistics_.Samples1D();

        [QS._core_c_.Diagnostics2.Property("RemovalTimes")]
        private QS._qss_c_.Statistics_.Samples1D timeSeries_removalTimes = new QS._qss_c_.Statistics_.Samples1D();

        [QS._core_c_.Diagnostics2.Property("RemovalCounts")]
        private QS._qss_c_.Statistics_.Samples2D timeSeries_removalCounts = new QS._qss_c_.Statistics_.Samples2D();
#endif

        #region GetObjectsCallback

        private void GetObjectsCallback(Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoingQueue,
                int maximumNumberOfObjects,
#if UseEnhancedRateControl
                int maximumNumberOfBytes, 
#endif
 out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
                out int numberOfBytesReturned,
#endif
 out bool moreObjectsAvailable)
        {
            // TODO: Implement enhanced rate control

            lock (this)
            {
                numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                    numberOfBytesReturned = 0;
#endif
                moreObjectsAvailable = true;

                while (true)
                {
                    if (pendingQueue.Count > 0)
                    {
                        if (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                        {
                            Request request = pendingQueue.Dequeue();
                            outgoingQueue.Enqueue(request);

#if DEBUG_CollectStatistics
                            if (timeSeries_removalTimes.Enabled)
                                timeSeries_removalTimes.Add(clock.Time);
#endif

                            numberOfObjectsReturned++;
                            // numberOfBytesReturned += .................................................................HERE
                        }
                        else
                            break;
                    }
                    else
                    {
                        moreObjectsAvailable = false;
                        registered = false;
                        break;
                    }
                }
            }

#if DEBUG_CollectStatistics
            double time = clock.Time;

            if (timeSeries_bufferSizes.Enabled)
                timeSeries_bufferSizes.Add(time, pendingQueue.Count);

            if (timeSeries_removalCounts.Enabled)
                timeSeries_removalCounts.Add(time, (double)numberOfObjectsReturned);
#endif

            // TODO: May add something to signal to the sender that the buffer is empty...................

/*
            if (!moreObjectsAvailable)
            {
            }
*/
        }

        #endregion

        #region Class Request

        private class Request : Base3_.IAsynchronousOperation, QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>
        {
            public Request(QS._core_c_.Base3.Message message, 
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                this.message = message;
                this.completionCallback = completionCallback;
                this.asynchronousState = asynchronousState;
            }

            private QS._core_c_.Base3.Message message;
            private Base3_.AsynchronousOperationCallback completionCallback;
            private object asynchronousState;
            private bool completed;
            private ManualResetEvent completion;

            #region IAsynchronousOperation Members

            void QS._qss_c_.Base3_.IAsynchronousOperation.Cancel()
            {
                throw new NotSupportedException("This type of request cannot be cancelled.");
            }

            void QS._qss_c_.Base3_.IAsynchronousOperation.Ignore()
            {
                completionCallback = null;
            }

            bool QS._qss_c_.Base3_.IAsynchronousOperation.Cancelled
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

            #region IAsynchronous<Message,object> Members

            QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
            {
                get { return message; }
            }

            object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
            {
                get { return this; }
            }

            QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
            {
                get { return new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback); }
            }

            private void CompletionCallback(bool succeeded, Exception exception, object context)
            {
                Base3_.AsynchronousOperationCallback callbackToCall = null;
                lock (this)
                {
                    if (!completed)
                    {
                        completed = true;
                        callbackToCall = completionCallback;

                        if (completion != null)
                            completion.Set();
                    }
                }

                if (callbackToCall != null)
                    callbackToCall(this);
            }

            #endregion
        }

        #endregion

        #region IReliableSerializableSender Members

        QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(
            uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
            QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
        {
            Request request = new Request(new QS._core_c_.Base3.Message(destinationLOID, data), completionCallback, asynchronousState);
            bool signal_now;
            lock (this)
            {
                pendingQueue.Enqueue(request);
                signal_now = !registered;
                registered = true;

#if DEBUG_CollectStatistics
                double time = clock.Time;

                if (timeSeries_bufferSizes.Enabled)
                    timeSeries_bufferSizes.Add(time, pendingQueue.Count);

                if (timeSeries_insertionTimes.Enabled)
                    timeSeries_insertionTimes.Add(time);
#endif
            }

            if (signal_now)
                sink.Send(new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback));

            return request;
        }

        void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
        {
        }

        #endregion

        #region ISerializableSender Members

        QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
        {
            get { throw new NotSupportedException(); }
        }

        void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
        {
            ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
        }

        int QS._qss_c_.Base3_.ISerializableSender.MTU
        {
            get { throw new NotSupportedException(); }
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
