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

// #define DEBUG_LogResends
// #define DEBUG_EnableStatistics
// #define DEBUG_LogTransmissionCallbacks

#define OPTION_NewReceivingCode

#define DEBUG_LogDroppingStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders10
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class RegionalSenders : QS.Fx.Inspection.Inspectable, Senders10.ISenderControllerClass<Base3_.RVID>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        private const double Default_MaximumRetransmissionTimeout = 60.00;

        public RegionalSenders(QS.Fx.Logging.IEventLogger eventLogger, QS._core_c_.Base3.InstanceID localAddress,
            QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Base3_.IDemultiplexer demultiplexer,
            Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders,
            Receivers4.IReceivingAgentCollection<Base3_.RVID> receivingAgentCollection,
            Receivers4.ICollectingAgentCollection<Base3_.RVID> collectingAgentCollection, double retransmissionTimeout,
            Receivers4.IRegionalController regionalController, 
            bool buffering_unrecognized, int maximum_unrecognized, double unrecognized_timeout)
        {
            this.eventLogger = eventLogger;
            this.localAddress = localAddress;
            this.logger = logger;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.dataChannel = (uint) QS.ReservedObjectID.Rings6_SenderController1_DataChannel;
            this.retransmissionChannel = (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel;
            this.regionSenders = regionSenders;
            this.retransmissionTimeout = retransmissionTimeout;
            this.maximumRetransmissionTimeout = Default_MaximumRetransmissionTimeout;
            this.receivingAgentCollection = receivingAgentCollection;
            this.collectingAgentCollection = collectingAgentCollection;
            this.regionalController = regionalController;

            this.buffering_unrecognized = buffering_unrecognized;
            this.maximum_unrecognized = maximum_unrecognized;
            this.unrecognized_timeout = unrecognized_timeout;

            demultiplexer.register(this.dataChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.DataReceiveCallback));
            demultiplexer.register(this.retransmissionChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.RetransmissionReceiveCallback));

#if DEBUG_LogDroppingStatistics
            ts_Dropped = new QS._qss_c_.Statistics_.Samples2D("Dropped");
            ts_Postponed = new QS._qss_c_.Statistics_.Samples2D("Postponed");
            ts_ReplayedDropped = new QS._qss_c_.Statistics_.Samples2D("ReplayedDropped");
            ts_ReplayedOK = new QS._qss_c_.Statistics_.Samples2D("ReplayedOK");
            ts_ReplayedException = new QS._qss_c_.Statistics_.Samples2D("ReplayedException");
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;        
        private Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders;
        private uint dataChannel, retransmissionChannel;
        private double retransmissionTimeout, maximumRetransmissionTimeout;
        private Receivers4.IReceivingAgentCollection<Base3_.RVID> receivingAgentCollection;
        private Receivers4.ICollectingAgentCollection<Base3_.RVID> collectingAgentCollection;
        private Receivers4.IRegionalController regionalController;

        private bool buffering_unrecognized;
        private int maximum_unrecognized;
        private double unrecognized_timeout;

        #region Struct Unrecognized

        private struct Unrecognized
        {
            public Unrecognized(double time, QS._core_c_.Base3.InstanceID address, Multicasting5.MessageRV obj)
            {
                this.Time = time;
                this.Address = address;
                this.Object = obj;
            }

            public double Time;
            public QS._core_c_.Base3.InstanceID Address;
            public Multicasting5.MessageRV Object;
        }

        #endregion

        private Queue<Unrecognized> unrecognized = new Queue<Unrecognized>();
        private bool unrecognizedWaiting;
        private QS.Fx.Clock.IAlarm unrecognizedAlarm;

#if DEBUG_LogDroppingStatistics
        [QS._core_c_.Diagnostics2.Property("Dropped")]
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Statistics.ISamples2D ts_Dropped;

        [QS._core_c_.Diagnostics2.Property("Postponed")]
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Statistics.ISamples2D ts_Postponed;

        [QS._core_c_.Diagnostics2.Property("ReplayedDropped")]
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Statistics.ISamples2D ts_ReplayedDropped;

        [QS._core_c_.Diagnostics2.Property("ReplayedOK")]
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Statistics.ISamples2D ts_ReplayedOK;

        [QS._core_c_.Diagnostics2.Property("ReplayedException")]
        [QS._core_c_.Diagnostics.Component]
        private QS._core_c_.Statistics.ISamples2D ts_ReplayedException;
#endif

        #region UnrecognizedCallback

        private void UnrecognizedCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            unrecognizedWaiting = false;
            double now = clock.Time;
            while (unrecognized.Count > 0)
            {
                double timestamp = unrecognized.Peek().Time;
                double togo = timestamp + unrecognized_timeout - now; 
                if (togo > 0)
                {
                    alarmRef.Reschedule(togo);
                    unrecognizedWaiting = true;
                    break;
                }
                else
                {
                    Unrecognized message = unrecognized.Dequeue();
                    try
                    {
                        Receivers4.IReceivingAgent receivingAgent;
                        if (receivingAgentCollection.TryGetAgent(message.Object.RVID, out receivingAgent, false))
                        {
                            receivingAgent.Receive(message.Address, message.Object.SeqNo, message.Object.EncapsulatedMessage, false, false);

#if DEBUG_LogDroppingStatistics
                            ts_ReplayedOK.Add(clock.Time, message.Object.SeqNo);
#endif
                        }
                        else
                        {
#if DEBUG_LogDroppingStatistics
                            ts_ReplayedDropped.Add(clock.Time, message.Object.SeqNo);
#endif
                        }
                    }
                    catch (Exception)
                    {
#if DEBUG_LogDroppingStatistics
                        ts_ReplayedException.Add(clock.Time, message.Object.SeqNo);
#endif
                    }
                }
            }
        }

        #endregion

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable DataReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Multicasting5.MessageRV message = receivedObject as Multicasting5.MessageRV;
            if (message != null)
            {
#if OPTION_NewReceivingCode
                if (buffering_unrecognized)
                {
                    Receivers4.IReceivingAgent receivingAgent;
                    if (receivingAgentCollection.TryGetAgent(message.RVID, out receivingAgent, false))
                    {
                        receivingAgent.Receive(sourceAddress, message.SeqNo, message.EncapsulatedMessage, false, false);
                    }
                    else
                    {
                        receivingAgent = null;
                        if (unrecognized.Count < maximum_unrecognized)
                        {
#if DEBUG_LogDroppingStatistics
                            ts_Postponed.Add(clock.Time, message.SeqNo);
#endif

                            unrecognized.Enqueue(new Unrecognized(clock.Time, sourceAddress, message));
                            if (!unrecognizedWaiting)
                            {
                                unrecognizedWaiting = true;
                                if (unrecognizedAlarm == null)
                                    unrecognizedAlarm = alarmClock.Schedule(
                                        unrecognized_timeout, new QS.Fx.Clock.AlarmCallback(this.UnrecognizedCallback), null);
                                else
                                    unrecognizedAlarm.Reschedule(unrecognized_timeout);
                            }
                        }
                        else
                        {
#if DEBUG_LogDroppingStatistics
                            ts_Dropped.Add(clock.Time, message.SeqNo);
#endif
                        }
                    }
                }
                else
                {
                    Receivers4.IReceivingAgent receivingAgent;
                    if (receivingAgentCollection.TryGetAgent(message.RVID, out receivingAgent, true))
                    {
                        receivingAgent.Receive(sourceAddress, message.SeqNo, message.EncapsulatedMessage, false, false);
                    }
                    else
                    {
#if DEBUG_LogDroppingStatistics
                        ts_Dropped.Add(clock.Time, message.SeqNo);
#endif

                        // just ignore this message.....................................
                    }
                }
#else
                receivingAgentCollection[message.RVID].Receive(sourceAddress, message.SeqNo, message.EncapsulatedMessage, false, false);
#endif
            }
            return null;
        }

        private QS.Fx.Serialization.ISerializable RetransmissionReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Multicasting5.MessageRV message = receivedObject as Multicasting5.MessageRV;
            if (message != null)
                receivingAgentCollection[message.RVID].Receive(sourceAddress, message.SeqNo, message.EncapsulatedMessage, true, false);
            return null;
        }

        #endregion

        #region Class Sender

        [QS._core_c_.Diagnostics.ComponentContainer]
        private class Sender 
            : QS.Fx.Inspection.Inspectable, Senders10.ISenderController<Base3_.RVID>, FlowControl3.IRetransmittingSender
        {
            public Sender(RegionalSenders owner,
                Base3_.RVID destinationAddress)
            {
                this.owner = owner;
                this.destinationAddress = destinationAddress;
                this.collectingAgent = owner.collectingAgentCollection[destinationAddress];

                this.retransmissionTimeout = owner.retransmissionTimeout;
                outgoingContainer = new FlowControl5.OutgoingContainer<Request>(owner.clock);
                regionSender = owner.regionSenders[destinationAddress.RegionID];
                retransmissionController = new FlowControl3.RetransmissionController1(this,
                    new QS._qss_c_.FlowControl3.RetransmissionController1.Configuration(retransmissionTimeout, 0.95, 2));

                retransmissionController.MinimumTimeout = 2;
                retransmissionController.MaximumTimeout = 3;
            }

            [QS._core_c_.Diagnostics.Component("Outgoing Container")]
            private FlowControl5.IOutgoingContainer<Request> outgoingContainer;
            [QS._core_c_.Diagnostics.Component("Region Sender")]
            private Base3_.IReliableSerializableSender regionSender;
            [QS._core_c_.Diagnostics.Component("Acknowledgement Provider")]
            private Receivers4.ICollectingAgent collectingAgent;
            [QS._core_c_.Diagnostics.Component("Retransmission Controller")]
            private FlowControl3.IRetransmissionController retransmissionController;

            private Base3_.RVID destinationAddress;
            private RegionalSenders owner;
            private double retransmissionTimeout;

#if DEBUG_EnableStatistics
            [Diagnostics.Component("Creation Times (X = seqno, Y = time)")]
            private Statistics.Samples timeSeries_creationTimes = new QS.CMS.Statistics.Samples();            
            [Diagnostics.Component("Transmission Times (X = seqno, Y = time)")]
            private Statistics.Samples timeSeries_transmissionTimes = new QS.CMS.Statistics.Samples();
            [Diagnostics.Component("Retransmission Times (X = time, Y = seqno)")]
            private Statistics.SamplesXY timeSeries_retransmissionCallbacks = new QS.CMS.Statistics.SamplesXY();
            [Diagnostics.Component("Completion Times (X = time, Y = seqno)")]
            private Statistics.SamplesXY timeSeries_completionCallbacks = new QS.CMS.Statistics.SamplesXY();
            [Diagnostics.Component("Transmission Latencies (X = seqno, Y = time)")]
            private Statistics.Samples timeSeries_timeToSend = new QS.CMS.Statistics.Samples();
            [Diagnostics.Component("Acknowledgement Latencies (X = seqno, Y = time)")]
            private Statistics.Samples timeSeries_acknowledgementLatencies = new QS.CMS.Statistics.Samples();
            [Diagnostics.Component("Retransmission Rates (X = seqno, Y = count)")]
            private Statistics.Samples timeSeries_retransmissionRates = new QS.CMS.Statistics.Samples();
            [Diagnostics.Component("Completion Times (X = seqno, Y = time)")]
            private Statistics.Samples timeSeries_requestCompletionTimes = new QS.CMS.Statistics.Samples();
            [Diagnostics.Component("Creation to Completion")]
            private TMS.Data.MultiSeries CreationToCompletion
            {
                get
                {
                    TMS.Data.MultiSeries series = new QS.TMS.Data.MultiSeries();
                    series.Series.Add("creation", (TMS.Data.DataSeries) timeSeries_creationTimes.DataSet);
                    series.Series.Add("transmission", (TMS.Data.DataSeries) timeSeries_transmissionTimes.DataSet);
                    series.Series.Add("completion", (TMS.Data.DataSeries) timeSeries_requestCompletionTimes.DataSet);
                    return series;
                }
            }
#endif

            public override string ToString()
            {
                return "Sender(" + destinationAddress.ToString() + ")";
            }

            #region Class Request

            [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
            private class Request : Rings4.RequestRV, Receivers4.IAcknowledgeable
            {
                public Request(Sender owner, Base3_.RVID regionViewID, QS._core_c_.Base3.Message message,
                    Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
                    : base(regionViewID, message, completionCallback, asynchronousState)
                {
                    this.owner = owner;
                    this.creationTime = owner.owner.clock.Time;
                }

                private Sender owner;
                [QS.Fx.Printing.Printable]
                private bool currently_sending;
                [QS.Fx.Printing.Printable]
                private uint nretransmissions;
                [QS.Fx.Printing.Printable]
                private double creationTime, transmissionTime, acknowledgementTime;
                [QS.Fx.Printing.Printable]
                private bool is_transmitted;

                public void Completed()
                {
                    this.acknowledgementTime = owner.owner.clock.Time;
                    this.IsCompleted = true;
                }

                public override void Unregister()
                {
                    base.Unregister();
                    owner.RemoveComplete(this);
                }

                public bool _IsTransmitted
                {
                    get { return is_transmitted; }
                    set { is_transmitted = value; }
                }

                #region Accessors

                public bool CurrentlySending
                {
                    get { return currently_sending; }
                    set { currently_sending = value; }
                }

                public uint NRetransmissions
                {
                    get { return nretransmissions; }
                    set { nretransmissions = value; }
                }

                public double CreationTime
                {
                    get { return creationTime; }
                    set { creationTime = value; }
                }
                
                public double TransmissionTime
                {
                    get { return transmissionTime; }
                    set { transmissionTime = value; }
                }

                public double AcknowledgementTime
                {
                    get { return acknowledgementTime; }
                    set { acknowledgementTime = value; }
                }

                #endregion

                #region IAcknowledgeable Members

                uint QS._qss_c_.Receivers4.IAcknowledgeable.SequenceNo
                {
                    get { return this.SeqNo; }
                }

                QS._core_c_.Base3.Message QS._qss_c_.Receivers4.IAcknowledgeable.Message
                {
                    get { return this.Message; }
                }

                void QS._qss_c_.Receivers4.IAcknowledgeable.Acknowledged()
                {
                    this.Completed();
                }

                void QS._qss_c_.Receivers4.IAcknowledgeable.Resend()
                {
                    owner.Resend(this);
                }

                #endregion
            }

            #endregion

            #region Transmissions and Retransmissions

            private void TransmissionCallback(Base3_.IAsynchronousOperation asynchronousOperation)
            {
                Request request = (Request)asynchronousOperation.AsyncState;

#if DEBUG_LogTransmissionCallbacks
                if (owner.eventLogger.Enabled)
                    owner.eventLogger.Log(new MyEvent(owner.clock.Time, owner.localAddress, this,
                        "TransmissionCallback : " + request.SeqNo.ToString(), QS.Fx.Printing.Printable.ToString(request)));
#endif

                bool transmitted_now;
                lock (request)
                {
                    request.CurrentlySending = false;

                    if (transmitted_now = !request._IsTransmitted)
                    {
                        request.TransmissionTime = owner.clock.Time;
                        request._IsTransmitted = true;
                    }

                    if (!request.IsCompleted)
                        request.RetransmissionAlarm = 
                            owner.alarmClock.Schedule(retransmissionTimeout < owner.maximumRetransmissionTimeout ? 
                                retransmissionTimeout : owner.maximumRetransmissionTimeout, 
                            new QS.Fx.Clock.AlarmCallback(this.RetransmissionCallback), request);
                }

#if DEBUG_EnableStatistics
                if (transmitted_now && timeSeries_transmissionTimes.Enabled)
                {
                    lock (this)
                    {
                        timeSeries_transmissionTimes.addSample((int)request.SeqNo, request.TransmissionTime);
                    }
                }
#endif
            }

            private void RetransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                Request request = (Request)alarmRef.Context;
                Resend(request);
            }

            #endregion

            #region Processing Resend Requests

            private void Resend(Request request)
            {
                bool should_send = false;
                lock (request)
                {
                    if (!request.IsCompleted)
                    {
                        if (!request.CurrentlySending)
                        {
                            request.NRetransmissions = request.NRetransmissions + 1;
                            request.RetransmissionAlarm = null;
                            should_send = true;
                            request.CurrentlySending = true;
                        }
                        else
                        {
#if DEBUG_LogResends
                            if (owner.eventLogger.Enabled)
                                owner.eventLogger.Log(new MyEvent(owner.clock.Time, owner.localAddress, this,
                                    "Not resending " + request.SeqNo.ToString() + " because apparently it is already being sent.", 
                                        QS.Fx.Printing.Printable.ToString(request)));
#endif
                        }
                    }
                }

                if (should_send)
                {
#if DEBUG_EnableStatistics
                    if (timeSeries_retransmissionCallbacks.Enabled)
                        lock (this)
                        {
                            timeSeries_retransmissionCallbacks.addSample(owner.clock.Time, (int)request.SeqNo);
                        }
#endif

#if DEBUG_LogResends
                    if (owner.eventLogger.Enabled)
                        owner.eventLogger.Log(new MyEvent(owner.clock.Time, owner.localAddress, this,
                            "Resending " + request.SeqNo.ToString(), QS.Fx.Printing.Printable.ToString(request)));
#endif

                    regionSender.BeginSend(owner.dataChannel, request,
                        new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.TransmissionCallback), request);
                }
            }

            #endregion

            #region Processing Completion

            private void RemoveComplete(Request request)
            {
                lock (this)
                {
                    double completion_time = request.AcknowledgementTime - request.TransmissionTime;
                    retransmissionController.completed(completion_time, (int) request.NRetransmissions);
                    outgoingContainer.Remove(request.SeqNo);

#if DEBUG_EnableStatistics
                    if (timeSeries_completionCallbacks.Enabled | timeSeries_timeToSend.Enabled |
                        timeSeries_acknowledgementLatencies.Enabled | timeSeries_retransmissionRates.Enabled |
                        timeSeries_requestCompletionTimes.Enabled)
                    {
                        lock (this)
                        {
                            if (timeSeries_completionCallbacks.Enabled)
                                timeSeries_completionCallbacks.addSample(request.AcknowledgementTime, (double)request.SeqNo);

                            if (timeSeries_requestCompletionTimes.Enabled)
                                timeSeries_requestCompletionTimes.addSample((int) request.SeqNo, request.AcknowledgementTime);

                            if (timeSeries_timeToSend.Enabled)
                                timeSeries_timeToSend.addSample((int)request.SeqNo, request.TransmissionTime - request.CreationTime);

                            if (timeSeries_acknowledgementLatencies.Enabled)
                                timeSeries_acknowledgementLatencies.addSample((int)request.SeqNo, completion_time);

                            if (timeSeries_retransmissionRates.Enabled)
                                timeSeries_retransmissionRates.addSample((int)request.SeqNo, (double)request.NRetransmissions);
                        }
                    }
#endif
                }
            }

            #endregion

            #region IReliableSerializableSender Members

            Base3_.IAsynchronousOperation Base3_.IReliableSerializableSender.BeginSend(
                uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {

                Request request = new Request(this, destinationAddress, new QS._core_c_.Base3.Message(destinationLOID, data), 
                    completionCallback, asynchronousState);
                request.CurrentlySending = true;

                lock (this)
                {
                    request.SeqNo = outgoingContainer.Add(request);

#if DEBUG_EnableStatistics
                    if (timeSeries_creationTimes.Enabled)
                        timeSeries_creationTimes.addSample((int)request.SeqNo, request.CreationTime);
#endif
                }

                collectingAgent.Collect(request);

                regionSender.BeginSend(owner.dataChannel, request,
                    new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.TransmissionCallback), request);

                owner.regionalController.Receive(
                    owner.localAddress, destinationAddress, request.SeqNo, request.Message, false, false);

                return request;
            }

            void Base3_.IReliableSerializableSender.EndSend(Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            #endregion

            #region ISenderController<RVID> Members

            QS._qss_c_.Base3_.RVID QS._qss_c_.Senders10.ISenderController<QS._qss_c_.Base3_.RVID>.DestinationAddress
            {
                get { return destinationAddress; }
            }

            #endregion

            #region ISerializableSender Members

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotImplementedException(); }
            }

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { throw new NotSupportedException(); }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IRetransmittingSender Members

            double QS._qss_c_.FlowControl3.IRetransmittingSender.RetransmissionTimeout
            {
                get { return retransmissionTimeout; }
                set { retransmissionTimeout = value; }
            }

            #endregion
        }

        #endregion

        #region ISenderControllerClass<RVID> Members

        QS._qss_c_.Senders10.ISenderController<QS._qss_c_.Base3_.RVID> 
            QS._qss_c_.Senders10.ISenderControllerClass<QS._qss_c_.Base3_.RVID>.CreateSender(
                QS._qss_c_.Base3_.RVID destinationAddress)
        {
            return new Sender(this, destinationAddress);
        }

        #endregion
    }
}
