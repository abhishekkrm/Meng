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

// #define DEBUG_Sender
// #define STATISTICS_TrackAcknowledgementCounts
// #define STATISTICS_RetransmissionRates
// #define STATISTICS_TransmissionTimes
// #define STATISTICS_TimeToSend
// #define STATISTICS_AcknowledgementLatencies
// #define STATISTICS_CompletionCallbacks
// #define STATISTICS_RetransmissionCallbacks

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings4
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Sender : QS.Fx.Inspection.Inspectable, ISender, FlowControl3.IRetransmittingSender        
    {
        private const double Maximum_RetransmissionTimeout = 5.00;

        public Sender(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Base3_.RVID destinationAddress, 
            Base3_.IReliableSerializableSender regionSender, IReceiver localReceiver, 
            uint dataChannel, double retransmissionTimeout)
        {
            this.alarmClock = alarmClock;
            this.logger = logger;
            this.destinationAddress = destinationAddress;
            this.regionSender = regionSender;
            this.retransmissionTimeout = retransmissionTimeout;
            this.localReceiver = localReceiver;
            this.dataChannel = dataChannel;
            this.clock = clock;
            this.maximumRetransmissionTimeout = Maximum_RetransmissionTimeout;

            outgoingContainer = new FlowControl5.OutgoingContainer<Request>(clock);

            retransmissionController = new FlowControl3.RetransmissionController1(this,
                new QS._qss_c_.FlowControl3.RetransmissionController1.Configuration(retransmissionTimeout, 0.95, 2));
        }

        private QS.Fx.Clock.IAlarmClock alarmClock;
        private Base3_.RVID destinationAddress;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IReliableSerializableSender regionSender;
        private uint dataChannel;
        private FlowControl5.IOutgoingContainer<Request> outgoingContainer;
        private double maximumRetransmissionTimeout, retransmissionTimeout;
        private IReceiver localReceiver;
        private QS.Fx.Clock.IClock clock;
        private FlowControl3.IRetransmissionController retransmissionController;

#if STATISTICS_TrackAcknowledgementCounts
        [QS.CMS.Diagnostics.Component("Ack Counts")]
        private Statistics.SamplesXY timeSeries_acknowledgementCounts = new QS.CMS.Statistics.SamplesXY();
#endif

#if STATISTICS_RetransmissionRates
        [QS.CMS.Diagnostics.Component("Retransmission Rates")]
        private Statistics.Samples timeSeries_retransmissionRates = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_TransmissionTimes
        [QS.CMS.Diagnostics.Component("Transmission Times")]
        private Statistics.Samples timeSeries_transmissionTimes = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_TimeToSend
        [QS.CMS.Diagnostics.Component("Time To Send")]
        private Statistics.Samples timeSeries_timeToSend = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_AcknowledgementLatencies
        [QS.CMS.Diagnostics.Component("Ack Latencies")]
        private Statistics.Samples timeSeries_acknowledgementLatencies = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_CompletionCallbacks
        [QS.CMS.Diagnostics.Component("Completion Callbacks")]
        private Statistics.SamplesXY timeSeries_completionCallbacks = new QS.CMS.Statistics.SamplesXY();
#endif

#if STATISTICS_RetransmissionCallbacks
        [QS.CMS.Diagnostics.Component("Retransmission Callbacks")]
        private Statistics.SamplesXY timeSeries_retransmissionCallbacks = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Class Request

        private class Request : RequestRV
        {
            public Request(Sender owner, Base3_.RVID regionViewID, QS._core_c_.Base3.Message message,
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
                : base(regionViewID, message, completionCallback, asynchronousState)
            {
                this.owner = owner;

// #if STATISTICS_TransmissionTimes || STATISTICS_TimeToSend || STATISTICS_AcknowledgementLatencies
                this.initiationTime = owner.clock.Time;
// #endif
            }

            private Sender owner;

// #if STATISTICS_RetransmissionRates
            public uint nretransmissions;
// #endif

// #if STATISTICS_TransmissionTimes || STATISTICS_TimeToSend || STATISTICS_AcknowledgementLatencies
            public double initiationTime, transmissionTime, acknowledgementTime;
// #endif

            public void Completed()
            {
// #if STATISTICS_TransmissionTimes || STATISTICS_TimeToSend || STATISTICS_AcknowledgementLatencies
                this.acknowledgementTime = owner.clock.Time;
// #endif

                this.IsCompleted = true;
            }

            public override void Unregister()
            {
                base.Unregister();
                owner.RemoveComplete(this);
            }
        }

        #endregion

        #region Retransmissions

        private void TransmissionCallback(Base3_.IAsynchronousOperation asynchronousOperation)
        {
            Request request = (Request)asynchronousOperation.AsyncState;

#if DEBUG_Sender
            logger.Log(this, "__TransmissionCallback(" + destinationAddress.ToString() + ": " + request.ToString());
#endif

            lock (request)
            {
// #if STATISTICS_TransmissionTimes || STATISTICS_TimeToSend || STATISTICS_AcknowledgementLatencies
                if (request.transmissionTime == 0)
                    request.transmissionTime = clock.Time;
// #endif

                if (!request.IsCompleted)
                    request.RetransmissionAlarm = alarmClock.Schedule(
                        retransmissionTimeout < maximumRetransmissionTimeout ? retransmissionTimeout : maximumRetransmissionTimeout,
                        new QS.Fx.Clock.AlarmCallback(this.RetransmissionCallback), request);
            }
        }

        private void RetransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            Request request = (Request)alarmRef.Context;

#if DEBUG_Sender
            logger.Log(this, "__RetransmissionCallback(" + destinationAddress.ToString() + ": " + request.ToString());
#endif

            bool should_send = false;
            lock (request)
            {
                if (!request.IsCompleted)
                {
// #if STATISTICS_RetransmissionRates
                    request.nretransmissions++;
// #endif

                    request.RetransmissionAlarm = null;
                    should_send = true;
                }
            }

            if (should_send)
            {
                regionSender.BeginSend(dataChannel, request,
                    new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.TransmissionCallback), request);

#if STATISTICS_RetransmissionCallbacks
                timeSeries_retransmissionCallbacks.addSample(clock.Time, request.SeqNo);
#endif
            }
        }

        #endregion

        #region Processing Completion

        private void RemoveComplete(Request request)
        {
            lock (this)
            {
#if STATISTICS_RetransmissionRates
                timeSeries_retransmissionRates.addSample((int)request.SeqNo, (double)request.nretransmissions);
#endif

#if STATISTICS_TransmissionTimes
                timeSeries_transmissionTimes.addSample((int)request.SeqNo, request.transmissionTime);
#endif

#if STATISTICS_TimeToSend
                timeSeries_timeToSend.addSample((int)request.SeqNo, request.transmissionTime - request.initiationTime);
#endif

                double completion_time = request.acknowledgementTime - request.transmissionTime;
#if STATISTICS_AcknowledgementLatencies
                timeSeries_acknowledgementLatencies.addSample((int)request.SeqNo, completion_time);
#endif

#if STATISTICS_CompletionCallbacks
                timeSeries_completionCallbacks.addSample(clock.Time, request.SeqNo);
#endif

                retransmissionController.completed(completion_time, (int) request.nretransmissions);

                outgoingContainer.Remove(request.SeqNo);
            }
        }

        #endregion

        #region ISender Members

        void ISender.Acknowledged(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.Message message)
        {
            NAKs naks = message.transmittedObject as NAKs;
            if (naks == null)
                throw new Exception("Wrong message received.");

#if DEBUG_Sender
            logger.Log(this, "__Acknowledged(" + destinationAddress.ToString() + ") : " + naks.ToString());
#endif

            lock (this)
            {
#if STATISTICS_TrackAcknowledgementCounts
                double now = clock.Time;
                int ncompletions = 0;
#endif

                for (uint seqno = outgoingContainer.FirstOccupiedSeqNo; seqno <= naks.MaximumSeqNo; seqno++)
                {
                    if (!naks.Missed.Contains(seqno))
                    {
                        Request request = outgoingContainer[seqno];
                        if (request != null)
                        {
#if DEBUG_Sender
                            logger.Log(this, "__Acknowledged(" + destinationAddress.ToString() + ") : Completed " + 
                                seqno.ToString());
#endif

                            request.Completed();

#if STATISTICS_TrackAcknowledgementCounts
                            ncompletions++;
#endif
                        }
                    }
                }

#if STATISTICS_TrackAcknowledgementCounts
                timeSeries_acknowledgementCounts.addSample(now, ncompletions);
#endif
            }
        }

        #endregion

        #region IReliableSerializableSender Members

        Base3_.IAsynchronousOperation Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
            QS.Fx.Serialization.ISerializable data, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
        {
            Request request = new Request(
                this, destinationAddress, new QS._core_c_.Base3.Message(destinationLOID, data), completionCallback, asynchronousState);

            lock (this)
            {
                request.SeqNo = outgoingContainer.Add(request);
            }

#if DEBUG_Sender
            logger.Log(this, "__BeginSend(" + destinationAddress.ToString() + ": " + request.ToString());
#endif

            regionSender.BeginSend(dataChannel, request,
                new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.TransmissionCallback), request);

            localReceiver.Receive(request.SeqNo, request.Message);

            return request;
        }

        void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
        {
        }

        #endregion

        #region ISerializableSender Members

        void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
        {
            ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
        }

        int QS._qss_c_.Base3_.ISerializableSender.MTU
        {
            get { throw new NotImplementedException(); }
        }

        QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
        {
            get { return regionSender.Address; }
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IAddressedSink<RVID,Asynchronous<Message>> Members

        QS._qss_c_.Base3_.RVID QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Address
        {
            get { throw new NotSupportedException(); }
        }

        #endregion

        #region ISink<Asynchronous<Message>> Members

        QS._qss_c_.Base4_.IChannel QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.Register(QS._qss_c_.Base4_.GetObjectsCallback<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> getObjectCallback)
        {



            throw new Exception("The method or operation is not implemented.");



        }

        uint QS._qss_c_.Base4_.ISink<QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
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
}
