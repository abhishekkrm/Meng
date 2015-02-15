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

// #define DEBUG_UseOldInterfacesToCore
// #define DEBUG_LogCallbacks

// ----- THESE PRODUCE HUGE DATA DUMPS -----

// #define DEBUG_CollectDetailedStatistics

// ------------------------------------------------------------------

// #define UseEnhancedRateControl

// #define OPTION_FlattenOutgoingData

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Base8_
{
    [QS.Fx.Base.Inspectable]
    [QS._core_c_.Diagnostics.ComponentContainer] 
    public class SerializingSender : QS.Fx.Inspection.Inspectable,
        QS.Fx.Network.ISource, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>, Base3_.IReliableSerializableSender, 
        QS._core_c_.FlowControl3.IRateControlled, QS._core_c_.Diagnostics2.IModule, IDisposable
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public const int DefaultBufferCapacity = 100;

        #region Constructors

        public SerializingSender(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Clock.IClock clock,
            QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Network.NetworkAddress address,
#if DEBUG_UseOldInterfacesToCore
            Core.ISender sender, 
#else
            QS.Fx.Network.ISender sender,
#endif
            QS._core_c_.Base3.InstanceID localAddress) 
            : this(statisticsController, clock, logger, eventLogger, address, sender, localAddress, DefaultBufferCapacity)
        {
        }

        public SerializingSender(QS._core_c_.Statistics.IStatisticsController statisticsController, QS.Fx.Clock.IClock clock, 
            QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Network.NetworkAddress address,
#if DEBUG_UseOldInterfacesToCore
            Core.ISender sender, 
#else
            QS.Fx.Network.ISender sender,
#endif
            QS._core_c_.Base3.InstanceID localAddress, int bufferCapacity)
        {
            this.statisticsController = statisticsController;
            this.clock = clock;
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.address = address;
            this.sender = sender;
            this.localAddress = localAddress;
            this.bufferCapacity = bufferCapacity;

            myCallback1 = new QS.Fx.Base.ContextCallback(this.CompletionCallback1);
            myCallback2 = new QS.Fx.Base.ContextCallback(this.CompletionCallback2);

#if DEBUG_UseOldInterfacesToCore
#else
            maximumRate = sender.Parameters.Get<double>(
                QS._core_c_.Core.SenderInfo.Parameters.MaximumRate, QS.Fx.Base.ParameterAccess.Unrestricted);
            underlyingMTU = sender.Parameters.Get<int>(QS._core_c_.Core.SenderInfo.Parameters.MTU).Value;
            myMTU = underlyingMTU - QS._qss_c_.Base3_.Root.HeaderOverhead;
#endif

#if DEBUG_CollectDetailedStatistics

            ts_NumberOfMessagesBuffered = statisticsController.Allocate2D("number of messages buffered", "", "time", "s", "", "", "", "");
            ts_NumberOfCallbacksRegistered = statisticsController.Allocate2D("number of callbacks registered", "", "time", "s", "", "", "", "");
            ts_NumberOfSerializedPacketsInOutgoingQueue = statisticsController.Allocate2D(
                "number of serialized packets in outgoing queue", "", "time", "s", "", "", "", "");
            ts_NumberOfPacketsReturnedByGetCallback = statisticsController.Allocate2D(
                "number of packets returned by get callback", "", "time", "s", "", "", "", "");
            ts_GetOverhead = statisticsController.Allocate2D("get overhead", "", "time", "s", "", "", "", "");
            ts_GetCallbackOverhead = statisticsController.Allocate2D("get callback overhead", "", "time", "s", "", "", "", "");

#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        #endregion

        #region Fields

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS.Fx.Network.NetworkAddress address;

#if DEBUG_UseOldInterfacesToCore
        [Diagnostics.Component("Underlying Sender")]
        private Core.ISender sender;
#else
        private QS.Fx.Network.ISender sender;
        private QS.Fx.Base.IParameter<double> maximumRate;
        private int underlyingMTU, myMTU;
#endif

        private QS._core_c_.Base3.InstanceID localAddress;
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> callbackQueue =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> bufferedQueue = 
            new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();
        private Queue<Outgoing> outgoingQueue = new Queue<Outgoing>();
        private bool registered;
        private QS.Fx.Base.ContextCallback myCallback1, myCallback2;
        private int bufferCapacity;

        #endregion

        #region Some Stats

#if DEBUG_CollectDetailedStatistics

        [QS._core_c_.Diagnostics2.Property("NumberOfMessagesBuffered")]
        private Statistics.ISamples2D ts_NumberOfMessagesBuffered;

        [QS._core_c_.Diagnostics2.Property("NumberOfCallbacksRegistered")]
        private Statistics.ISamples2D ts_NumberOfCallbacksRegistered;

        [QS._core_c_.Diagnostics2.Property("NumberOfSerializedPacketsInOutgoingQueue")]
        private Statistics.ISamples2D ts_NumberOfSerializedPacketsInOutgoingQueue;

        [QS._core_c_.Diagnostics2.Property("NumberOfPacketsReturnedByGetCallback")]
        private Statistics.ISamples2D ts_NumberOfPacketsReturnedByGetCallback;

        [QS._core_c_.Diagnostics2.Property("GetOverhead")]
        private Statistics.ISamples2D ts_GetOverhead;

        [QS._core_c_.Diagnostics2.Property("GetCallbackOverhead")]
        private Statistics.ISamples2D ts_GetCallbackOverhead;

#endif

        [QS.Fx.Base.Inspectable("Number of Messages Buffered (Not Serialized Yet)")]
        private int BufferedQueueCount
        {
            get { return bufferedQueue.Count; }
        }

        [QS.Fx.Base.Inspectable("Number of Callbacks Currently Registered")]
        private int CallbackQueueCount
        {
            get { return callbackQueue.Count; }
        }

        [QS.Fx.Base.Inspectable("Number of (Serialized) Packets in Outgoing Queue")]
        private int OutgoingQueueCount
        {
            get { return outgoingQueue.Count; }
        }

        #endregion

        #region Completion Callbacks

        private void CompletionCallback1(object context)
        {
#if DEBUG_LogCallbacks
            logger.Log(this, "_CompletionCallback1");
#endif

            QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request = context as QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>;
            if (request == null)
                logger.Log(this, "Internal error: context in the callback is not of type Base6.IAsynchronous<Base3.Message>.");

            QS._core_c_.Base6.CompletionCallback<object> completionCallback = request.CompletionCallback;
            if (completionCallback != null)
            {
                try
                {
#if DEBUG_LogCallbacks
                    logger.Log(this, "_CompletionCallback1: Invoking method " + 
                        completionCallback.Method.ToString() + " against " + completionCallback.Target.ToString());
#endif

                    completionCallback(true, null, request.Context);
                }
                catch (Exception exc)
                {
                    logger.Log(this, QS._core_c_.Helpers.ToString.CallbackException(completionCallback, exc));
                }
            }
            else
            {
#if DEBUG_LogCallbacks
                logger.Log(this, "_CompletionCallback1: Completion callback is " +
                    ((request.CompletionCallback != null) ? (request.CompletionCallback.GetType().ToString()) : "(null)"));
#endif
            }
        }

        private void CompletionCallback2(object context)
        {
#if DEBUG_LogCallbacks
            logger.Log(this, "_CompletionCallback2");
#endif

            AsynchronousOperation operation = context as AsynchronousOperation;
            if (operation == null)
                logger.Log(this, "Internal error: context in the callback is not of type AsynchronousOperation.");

            operation.Completed();
        }

        #endregion

        #region Struct Outgoing

        private struct Outgoing
        {
            public Outgoing(QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request, QS.Fx.Network.Data data)
            {
                this.Request = request;
                this.Data = data;
            }

            public QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> Request;
            public QS.Fx.Network.Data Data;
        }

        #endregion

        #region ISource Members

        bool QS.Fx.Network.ISource.Get(int maximumSize, 
            out QS.Fx.Network.Data data, out QS.Fx.Base.ContextCallback callback, out object context, out bool moreAvailable)
        {
#if DEBUG_CollectDetailedStatistics
            double tt1 = clock.Time;
#endif

            Outgoing outgoing;
            bool available;
            if (outgoingQueue.Count == 0)
            {
                if (bufferedQueue.Count == 0)
                {
                    while (bufferedQueue.Count < bufferCapacity && callbackQueue.Count > 0)
                    {
                        QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getCallback = callbackQueue.Dequeue();

#if DEBUG_CollectDetailedStatistics
                        double tt3 = clock.Time;
#endif

                        // TODO: Implement enhanced rate control

                        int nreturned;
#if UseEnhancedRateControl
                        int nbytesreturned;
#endif
                        bool hasmore;
                        getCallback(bufferedQueue, bufferCapacity - bufferedQueue.Count, 
#if UseEnhancedRateControl                            
                            int.MaxValue, // needs to be changed
#endif                            
                            out nreturned, 
#if UseEnhancedRateControl                            
                            out nbytesreturned, 
#endif                            
                            out hasmore);

                        if (hasmore)
                            callbackQueue.Enqueue(getCallback);

#if DEBUG_CollectDetailedStatistics
                        double tt4 = clock.Time;
                        ts_GetCallbackOverhead.Add(tt3, tt4 - tt3);
                        ts_NumberOfPacketsReturnedByGetCallback.Add(tt3, nreturned);
                        ts_NumberOfMessagesBuffered.Add(tt4, bufferedQueue.Count);
                        ts_NumberOfCallbacksRegistered.Add(tt4, callbackQueue.Count);
#endif

                        if (nreturned == 0)
                            break;
                    }
                }

                if (bufferedQueue.Count == 0)
                {
                    outgoing = default(Outgoing);
                    available = false;
                }
                else
                {
                    QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request = bufferedQueue.Dequeue();

                    IList<QS.Fx.Base.Block> segments;
                    uint transmittedSize;
                    Base3_.Root.Encode(localAddress, request.Argument.destinationLOID, request.Argument.transmittedObject,
                        out segments, out transmittedSize);

#if OPTION_FlattenOutgoingData
                    // FROM:OSDI2006
                    byte[] _flattened_outgoing_data = new byte[transmittedSize];
                    int _outgoing_offset = 0;
                    foreach (ArraySegment<byte> _outgoing_segment in segments)
                    {
                        Buffer.BlockCopy(_outgoing_segment.Array, _outgoing_segment.Offset, _flattened_outgoing_data, _outgoing_offset, _outgoing_segment.Count);
                        _outgoing_offset += _outgoing_segment.Count;
                    }

                    if (_outgoing_offset != transmittedSize)
                        throw new Exception("Could not flatten data, buffer sizes mismatch.");

                    List<ArraySegment<byte>> _flattened_outgoing_segments = new List<ArraySegment<byte>>(1);
                    _flattened_outgoing_segments.Add(new ArraySegment<byte>(_flattened_outgoing_data));
                    outgoing = new Outgoing(request, new Core.Data(_flattened_outgoing_segments, (int) transmittedSize));
#else
                    outgoing = new Outgoing(request, new QS.Fx.Network.Data(segments, (int)transmittedSize));
#endif

                    available = outgoing.Data.Size <= maximumSize;
                    if (!available)
                        outgoingQueue.Enqueue(outgoing);

#if DEBUG_CollectDetailedStatistics
                    double tt5 = clock.Time;
                    ts_NumberOfMessagesBuffered.Add(tt5, bufferedQueue.Count);
                    ts_NumberOfSerializedPacketsInOutgoingQueue.Add(tt5, outgoingQueue.Count);
#endif
                }
            }
            else
            {
                available = outgoingQueue.Peek().Data.Size <= maximumSize;
                if (available)
                    outgoing = outgoingQueue.Dequeue();
                else
                    outgoing = default(Outgoing);

#if DEBUG_CollectDetailedStatistics
                double tt6 = clock.Time;
                ts_NumberOfSerializedPacketsInOutgoingQueue.Add(tt6, outgoingQueue.Count);
#endif
            }

            moreAvailable = outgoingQueue.Count > 0 || bufferedQueue.Count > 0 || callbackQueue.Count > 0;
            if (!moreAvailable)
                registered = false;

            if (available)
            {
                data = outgoing.Data;
                callback = myCallback1;
                context = outgoing.Request;
            }
            else
            {
                data = default(QS.Fx.Network.Data);
                callback = null;
                context = null;
            }

#if DEBUG_CollectDetailedStatistics
            double tt2 = clock.Time;
            ts_GetOverhead.Add(tt1, tt2 - tt1);
#endif

            return available;
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
            bool signal_now;
            lock (this)
            {
                callbackQueue.Enqueue(getObjectsCallback);
                signal_now = !registered;
                registered = true;

#if DEBUG_CollectDetailedStatistics
                double tt6 = clock.Time;
                ts_NumberOfCallbacksRegistered.Add(tt6, callbackQueue.Count);
#endif
            }

            if (signal_now)
                sender.Send(this);
        }

        #endregion

        #region Class AsynchronousOperation

        private class AsynchronousOperation : Base3_.IAsynchronousOperation
        {
            public AsynchronousOperation(Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                this.completionCallback = completionCallback;
                this.asynchronousState = asynchronousState;
            }

            private Base3_.AsynchronousOperationCallback completionCallback;
            private bool completed;
            private object asynchronousState;

            #region IAsynchronousOperation Members

            void QS._qss_c_.Base3_.IAsynchronousOperation.Cancel()
            {
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
                get { throw new NotSupportedException(); }
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

            public void Completed()
            {
                completed = true;
                if (completionCallback != null)
                    completionCallback(this);
            }
        }

        #endregion

        #region IReliableSerializableSender Members

        Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
            QS.Fx.Serialization.ISerializable data, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
        {
            AsynchronousOperation operation = new AsynchronousOperation(completionCallback, asynchronousState);

            IList<QS.Fx.Base.Block> segments;
            uint transmittedSize;
            Base3_.Root.Encode(localAddress, destinationLOID, data, out segments, out transmittedSize);

            sender.Send(new QS.Fx.Network.Data(segments, (int)transmittedSize), myCallback2, operation);

            return operation;
        }

        void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
        {
        }

        #endregion

        #region ISerializableSender Members

        void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
        {
            IList<QS.Fx.Base.Block> segments;
            uint transmittedSize;
            Base3_.Root.Encode(localAddress, destinationLOID, data, out segments, out transmittedSize);
            if ((int) transmittedSize > 
#if DEBUG_UseOldInterfacesToCore
                sender.MTU
#else
                underlyingMTU
#endif
                )
                throw new Exception("Cannot send message, data is " + transmittedSize.ToString() + 
                    " bytes in size while the transport channel MTU is only " + 
#if DEBUG_UseOldInterfacesToCore
                    sender.MTU.ToString() + 
#else
                    underlyingMTU.ToString() +
#endif
                    " bytes.");

            QS.Fx.Network.Data datatosend = new QS.Fx.Network.Data(segments, (int)transmittedSize);

            // sanity check
            // datatosend._CheckSize();

            sender.Send(datatosend);
        }

        QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
        {
            get { return address; }
        }

        int QS._qss_c_.Base3_.ISerializableSender.MTU
        {
            get 
            { 
#if DEBUG_UseOldInterfacesToCore
                return sender.MTU - Base3.Root.HeaderOverhead; 
#else
                return myMTU;
#endif
            }
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IRateControlled Members

        double QS._core_c_.FlowControl3.IRateControlled.MaximumRate
        {
            get 
            { 
#if DEBUG_UseOldInterfacesToCore
                return sender.MaximumRate; 
#else
                return maximumRate.Value;
#endif
            }

            set 
            {
#if DEBUG_UseOldInterfacesToCore
                sender.MaximumRate = value; 
#else
                maximumRate.Value = value;
#endif
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if ((this.sender != null) && (this.sender is IDisposable))
                    ((IDisposable) this.sender).Dispose();
            }
        }

        #endregion
    }
}
