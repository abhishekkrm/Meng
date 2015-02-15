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

// #define DEBUG_BufferingUNS
// #define STATISTICS_MeasureBatchingPerformance

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders9
{
    /// <summary>
    /// This is an unreliable node sender that does buffering and notifies the caller when a request is actually sent.
    /// </summary>
    public class BufferingUNS : Base3_.SenderCollection<QS.Fx.Network.NetworkAddress, Base3_.IReliableSerializableSender>, QS._qss_e_.Base_1_.IStatisticsCollector
    {
        public BufferingUNS(QS.Fx.Logging.ILogger logger, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection,
            Buffering_3_.IControllerClass bufferingControllerClass, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock)
        {
            this.logger = logger;
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.bufferingControllerClass = bufferingControllerClass;
            this.alarmClock = alarmClock;
            this.clock = clock;
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
        private Buffering_3_.IControllerClass bufferingControllerClass;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private double defaultRate = 100;
        private bool batching_enabled_by_default = true, flow_control_enabled_by_default = true;
        private int burstSize = 10;

        public int BurstSize
        {
            get { return burstSize; }
            set { burstSize = value; }
        }

        public bool BatchingEnabledByDefault
        {
            get { return batching_enabled_by_default; }
            set { batching_enabled_by_default = value; }
        }

        public bool FlowControlEnabledByDefault
        {
            get { return flow_control_enabled_by_default; }
            set { flow_control_enabled_by_default = value; }
        }

        public double DefaultRate
        {
            get { return defaultRate; }
            set { defaultRate = value; }
        }

        #region Class Sender

        [QS.Fx.Base.Inspectable]
        private class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender, QS._qss_e_.Base_1_.IStatisticsCollector
        {
            public Sender(BufferingUNS owner, QS.Fx.Network.NetworkAddress destinationAddress)
            {
                this.owner = owner;
                this.destinationAddress = destinationAddress;
                this.underlyingSender = owner.underlyingSenderCollection[destinationAddress];
                bufferingController = owner.bufferingControllerClass.CreateController(underlyingSender.MTU);

                if (owner.flow_control_enabled_by_default)
                    flowController = new FlowControl6.RateController1(owner.logger,
                    owner.clock, owner.alarmClock, owner.defaultRate, owner.burstSize, 
                    new QS._qss_c_.Base3_.NoArgumentCallback(this.ReadyCallback));
                else
                    flowController = FlowControl6.NoController.Controller;

                this.batching_enabled = owner.batching_enabled_by_default;

                owner.logger.Log(null, "BufferingUNS.Sender " + destinationAddress.ToString() + " : Batching " +
                    (batching_enabled ? "Enabled" : "Disabled"));
            }

            private bool batching_enabled;
            private BufferingUNS owner;
            private QS.Fx.Network.NetworkAddress destinationAddress;
            private QS._qss_c_.Base3_.ISerializableSender underlyingSender;
            private Queue<Request> pendingQueue = new Queue<Request>();
            private FlowControl6.IFlowController flowController;
            private Buffering_3_.IController bufferingController;

#if STATISTICS_MeasureBatchingPerformance
            private QS.CMS.Statistics.SamplesXY batchingSamples = new QS.CMS.Statistics.SamplesXY();
#endif

            #region Class Request

            private class Request : Base3_.IAsynchronousOperation
            {
                public Request(QS._core_c_.Base3.Message message, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) 
                {
                    this.completionCallback = completionCallback;
                    this.asynchronousState = asynchronousState;
                    this.message = message;
                }

                private Base3_.AsynchronousOperationCallback completionCallback;
                private object asynchronousState;
                private QS._core_c_.Base3.Message message;
                private bool completed;

                public QS._core_c_.Base3.Message Message
                {
                    get { return message; }
                }

                public void Completed()
                {
                    bool invoke_callback;
                    lock (this)
                    {
                        invoke_callback = !completed && completionCallback != null;
                        completed = true;
                    }

                    if (invoke_callback)
                        completionCallback(this);
                }

                #region IAsynchronousOperation Members

                void QS._qss_c_.Base3_.IAsynchronousOperation.Cancel()
                {
                    throw new NotSupportedException();
                }

                void QS._qss_c_.Base3_.IAsynchronousOperation.Ignore()
                {
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
            }

            #endregion

            #region Internal Processing

            private void ReadyCallback()
            {
#if DEBUG_BufferingUNS
                owner.logger.Log(this, "__ReadyCallback: " + destinationAddress.ToString());
#endif

                List<Request> completionQueue = new List<Request>();

                lock (this)
                {
                    while (flowController.Ready && pendingQueue.Count > 0)
                    {
                        if (batching_enabled)
                        {
                            while (pendingQueue.Count > 0 &&
                                pendingQueue.Peek().Message.transmittedObject.SerializableInfo.Size < bufferingController.CurrentCapacity)
                            {
                                Request request = pendingQueue.Dequeue();
                                bufferingController.append(request.Message.destinationLOID, request.Message.transmittedObject);
                                completionQueue.Add(request);
                            }

                            bufferingController.flush();

                            while (bufferingController.ReadyQueue.Count > 0)
                            {
                                Buffering_3_.IMessageCollection messageCollection = bufferingController.ReadyQueue.Dequeue();

#if DEBUG_BufferingUNS
                        owner.logger.Log(this, "__ReadyCallback: " + destinationAddress.ToString() + " sending " + messageCollection.ToString());
#endif

                                underlyingSender.send((uint)ReservedObjectID.Unwrapper, messageCollection);

#if STATISTICS_MeasureBatchingPerformance
                                lock (batchingSamples)
                                {
                                    batchingSamples.addSample(owner.clock.Time, messageCollection.Count);
                                }
#endif
                            }
                        }
                        else
                        {
                            Request request = pendingQueue.Dequeue();
                            completionQueue.Add(request);

                            underlyingSender.send(request.Message.destinationLOID, request.Message.transmittedObject);

#if STATISTICS_MeasureBatchingPerformance
                            lock (batchingSamples)
                            {
                                batchingSamples.addSample(owner.clock.Time, 1);
                            }
#endif
                        }

                        flowController.Consume();
                    }
                }

                foreach (Request request in completionQueue)
                    request.Completed();
            }

            #endregion

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
                QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
#if DEBUG_BufferingUNS
                owner.logger.Log(this, "__BeginSend: " + destinationAddress.ToString() + "," + destinationLOID.ToString() + ", " + data.ToString());
#endif

                Request request = new Request(new QS._core_c_.Base3.Message(destinationLOID, data), completionCallback, asynchronousState);

                // fix this.......................................................................................

                bool synchronous = flowController.Ready;
                if (synchronous)
                    flowController.Consume();
                else
                {
                    lock (this)
                    {
                        synchronous = flowController.TryConsume();
                        if (!synchronous)
                            pendingQueue.Enqueue(request);
                    }
                }

                if (synchronous)
                {
                    underlyingSender.send(destinationLOID, data);
                    request.Completed();

#if STATISTICS_MeasureBatchingPerformance
                    lock (batchingSamples)
                    {
                        batchingSamples.addSample(owner.clock.Time, 1);
                    }
#endif
                }

                return request;
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return destinationAddress; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);                
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IStatisticsCollector Members

            IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
            {
                get 
                {
                    List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();
#if STATISTICS_MeasureBatchingPerformance
                    statistics.Add(new Components.Attribute("Batch_Sizes", batchingSamples.DataSet));
#endif
                    return statistics;
                }
            }

            #endregion
        }

        #endregion

        protected override QS._qss_c_.Base3_.IReliableSerializableSender CreateSender(QS.Fx.Network.NetworkAddress address)
        {
            return new Sender(this, address);
        }

        #region IStatisticsCollector Members

        IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
        {
            get 
            {
                return QS._qss_c_.Helpers_.ListOf<QS._core_c_.Components.Attribute>.Singleton(
                    QS._qss_e_.Base_1_.Statistics.StatisticsOf<QS.Fx.Network.NetworkAddress, Base3_.IReliableSerializableSender>("Senders", this.senders)); 
            }
        }

        #endregion
    }
}
