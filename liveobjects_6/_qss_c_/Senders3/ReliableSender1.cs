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

// #define DEBUG_ReliableSender
// #define DEBUG_LogOperations
// #define DEBUG_LogOperationsInRequest

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Senders3
{
    public class ReliableSender1 : Base3_.SenderClass<Base3_.IReliableSerializableSender>, 
		Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>,
        IDisposable
    {
        private const int defaultWindowSize = 100;

        private const double DefaultMinimumRetransmissionTimeout = 0.02;
        private const double DefaultMaximumRetransmissionTimeout = 0.2;

        public ReliableSender1(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, 
            Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection, QS.Fx.Clock.IAlarmClock alarmClock, 
			double initialRetransmissionTimeout) 
            : this(logger, demultiplexer, underlyingSenderCollection, alarmClock, initialRetransmissionTimeout,
                (uint)ReservedObjectID.Senders3_ReliableSender_MessageChannel,
                (uint)ReservedObjectID.Senders3_ReliableSender_AcknowledgementChannel)
        {
        }

        public ReliableSender1(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
            Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection, QS.Fx.Clock.IAlarmClock alarmClock,
            double initialRetransmissionTimeout, uint messageChannelLOID, uint acknowledgementChannelLOID)
        {
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.alarmClock = alarmClock;
			this.initialRetransmissionTimeout = initialRetransmissionTimeout;

			demultiplexer.register(messageChannelLOID, new QS._qss_c_.Base3_.ReceiveCallback(this.messageCallback));
            demultiplexer.register(acknowledgementChannelLOID, new QS._qss_c_.Base3_.ReceiveCallback(this.acknowledgementCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
        private QS.Fx.Clock.IAlarmClock alarmClock;
		private double initialRetransmissionTimeout;

		public Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> UnderlyingSenderCollection
		{
			set { underlyingSenderCollection = value; }
		}

#if DEBUG_LogOperations
		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private TMS.Inspection.AutoCollection<QS.Fx.Network.NetworkAddress, Base.Logger> operations_log =
			new QS.TMS.Inspection.AutoCollection<QS.Fx.Network.NetworkAddress, QS.CMS.Base.Logger>();
#endif

		protected override Base3_.IReliableSerializableSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Sender(this, destinationAddress);
        }

        #region Callbacks

		private QS.Fx.Serialization.ISerializable messageCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_LogOperations
			operations_log[sourceAddress].logMessage(this, Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

#if DEBUG_ReliableSender
            logger.Log(this, "__MessageCallback : from " + sourceIID.ToString() + ", " + receivedObject.ToString());
#endif

            Sender.IncomingRequest request = receivedObject as Sender.IncomingRequest;
            if (request != null)
            {
                underlyingSenderCollection[sourceIID.Address].send(
                    (uint)ReservedObjectID.Senders3_ReliableSender_AcknowledgementChannel, new Sender.Ack(request.SeqNo));

                // no duplicate handling
                demultiplexer.dispatch(request.Object.destinationLOID, sourceIID, request.Object.transmittedObject);
            }

            return null;
        }

		private QS.Fx.Serialization.ISerializable acknowledgementCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Sender.Ack acknowledgement = receivedObject as Sender.Ack;
            if (acknowledgement != null)
            {
#if DEBUG_ReliableSender
                logger.Log(this, "__AcknowledgementCallback : from " + sourceIID.ToString() + ", " + acknowledgement.SeqNo.ToString());
#endif

                ((Sender)this.SenderCollection[sourceIID.Address]).acknowledged(acknowledgement.SeqNo);
            }

            return null;
        }

        #endregion

        #region Class Sender

        public class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender, 
			FlowControl3.IRetransmittingSender, QS._qss_e_.Base_1_.IStatisticsCollector,
            IDisposable
        {
            public Sender(ReliableSender1 owner, QS.Fx.Network.NetworkAddress destinationAddress)
            {
                this.owner = owner;
                this.underlyingSender = owner.underlyingSenderCollection[destinationAddress];
                outgoingWindow = new FlowControl_1_.OutgoingWindow(defaultWindowSize);
                outgoingQueue = new Queue<Request>();
                alarmCallback = new QS.Fx.Clock.AlarmCallback(this.retransmissionCallback);

				retransmissionTimeout = owner.initialRetransmissionTimeout;
                minimumRetransmissionTimeout = DefaultMinimumRetransmissionTimeout;
                maximumRetransmissionTimeout = DefaultMaximumRetransmissionTimeout;

				retransmissionController = new FlowControl3.RetransmissionController1(this);

                owner.logger.Log(this, 
                    "__ReliableSender(" + destinationAddress.ToString() + "): Underlying MTU = " + underlyingSender.MTU.ToString());
			}

            private ReliableSender1 owner;
            private QS._qss_c_.Base3_.ISerializableSender underlyingSender;
			[QS.Fx.Base.Inspectable("Outgoing Window", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private FlowControl_1_.IOutgoingWindow outgoingWindow;
            private uint lastused_seqno = 0;
            private QS.Fx.Clock.AlarmCallback alarmCallback;
            private double minimumRetransmissionTimeout, maximumRetransmissionTimeout;

#if DEBUG_LogOperations
			[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
			private Base.Logger operations_log = new QS.CMS.Base.Logger(true, null, true, string.Empty);
#endif

			[QS.Fx.Base.Inspectable("Retransmission Timeout", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private double retransmissionTimeout;

			[QS.Fx.Base.Inspectable("Retransmission Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IRetransmissionController retransmissionController;

			private System.Object samplelock = new Object();
			private void add_sample(Multicasting3.ITimedRequest request)
			{
				lock (samplelock)
				{
					retransmissionController.completed(request.Duration, request.NumberOfRetransmissions);
				}
			}

			[QS.Fx.Base.Inspectable("Outgoing Queue", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private Queue<Request> outgoingQueue;

			public void acknowledged(uint seqno)
            {
#if DEBUG_LogOperations
				operations_log.logMessage(this, "Acknowledged " + seqno.ToString());
#endif
				lock (this)
                {
                    try
                    {
                        Request request = (Request) outgoingWindow.remove(seqno);

#if DEBUG_LogOperationsInRequest
						request.operations_log.logMessage(this, "Acknowledged");
#endif

                        if (request != null)
                        {
                            request.acknowledgementTime = QS._core_c_.Base2.PreciseClock.Clock.Time;

                            if (request.alarmRef != null)
                            {
                                request.alarmRef.Cancel();
                                request.alarmRef = null;
                            }

                            add_sample(request);
                            request.IsCompleted = true;

#if DEBUG_ReliableSender
                            owner.logger.Log(this, "Acknowledged " + seqno.ToString() + " at " + this.Address.ToString() + ".");
#endif
                        }
                        else
                        {
#if DEBUG_ReliableSender
                            owner.logger.Log(this, "Could not acknowledge " + seqno.ToString() + " at " + this.Address.ToString() + ", not found.\n" +
                                outgoingWindow.ToString());
#endif
                        }
                    }
                    catch (Exception exc)
                    {
                        owner.logger.Log(this, "Sender[" + underlyingSender.Address.ToString() + "]: "+ exc.ToString());
                    }

                    while (outgoingWindow.hasSpace() && outgoingQueue.Count > 0)
                    {
                        Request request = outgoingQueue.Dequeue();
                        if (outgoingWindow.append(request) != request.message.SeqNo)
                            throw new Exception("Assigned and available sequence numbers do not match.");

#if DEBUG_ReliableSender
                        owner.logger.Log(this, "__BeginSend: sending enqueued for " + this.Address.ToString() + ", " + request.message.ToString());
#endif

#if DEBUG_LogOperationsInRequest
						request.operations_log.logMessage(this, "Sending_Enqueued");
#endif

						request.sendingTime = QS._core_c_.Base2.PreciseClock.Clock.Time;
						underlyingSender.send((uint) ReservedObjectID.Senders3_ReliableSender_MessageChannel, request.message);
                        request.alarmRef = owner.alarmClock.Schedule(retransmissionTimeout, alarmCallback, request);
                    }
                }
            }

            private void unregister(Request request)
            {
				// request.alarmRef.Cancel();
#if DEBUG_LogOperationsInRequest
				request.operations_log.logMessage(this, "Unregistering");
#endif
			}

            private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                Request request = (Request)alarmRef.Context;
				bool should_send;
				lock (request)
				{
					if (should_send = !request.IsCompleted && !request.Cancelled && request.alarmRef != null)
					{
						request.nretransmissions++;

#if DEBUG_ReliableSender
						owner.logger.Log(this, "__RetransmissionCallback : to " + this.Address.ToString() + ", " + request.message.ToString());
#endif
						alarmRef.Reschedule(retransmissionTimeout);
					}
				}

				if (should_send)
				{
#if DEBUG_LogOperations
					operations_log.logMessage(this, "Resending " + Helpers.ToString.ReceivedObject(underlyingSender.Address, 
						request.message) + " after " + alarmRef.Timeout.ToString() + "s timeout.");
#endif

#if DEBUG_LogOperationsInRequest
					request.operations_log.logMessage(this, "Resending after " + alarmRef.Timeout.ToString() + "s timeout.");
#endif

					underlyingSender.send((uint)ReservedObjectID.Senders3_ReliableSender_MessageChannel, request.message);
				}
			}

            #region Class OutgoingRequest

            public class OutgoingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
            {
                public new const int HeaderOverhead = 
                    Base3_.InSequence<QS._core_c_.Base3.Message>.HeaderOverhead + QS._core_c_.Base3.Message.HeaderOverhead;

                public OutgoingRequest(uint seqno, QS._core_c_.Base3.Message message) : base(seqno, message)
                {
                }

                public override ClassID ClassID
                {
                    get { return QS.ClassID.Senders3_ReliableSender_Message; }
                }
            }

            #endregion

            #region Class IncomingRequest

            [QS.Fx.Serialization.ClassID(QS.ClassID.Senders3_ReliableSender_Message)]
            public class IncomingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
            {
                public IncomingRequest()
                {
                }
            }

            #endregion

            #region Class Ack

            [QS.Fx.Serialization.ClassID(ClassID.Senders3_ReliableSender_Ack)]
            public class Ack : Base3_.Acknowledgement
            {
                public Ack()
                {
                }

                public Ack(uint seqno) : base(seqno)
                {
                }

                public override ClassID ClassID
                {
                    get { return ClassID.Senders3_ReliableSender_Ack; }
                }
            }

            #endregion

            #region Class Request

            private class Request : Base3_.AsynchronousOperation.Inspectable, Multicasting3.ITimedRequest
            {
                public Request(ReliableSender1.Sender owner, uint seqno, QS._core_c_.Base3.Message message,
                    Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) 
                    : base(completionCallback, asynchronousState)
                {
                    this.owner = owner;
                    this.message = new OutgoingRequest(seqno, message);
                }

				[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
                public QS.Fx.Clock.IAlarm alarmRef = null;
				[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
				public OutgoingRequest message;

				private ReliableSender1.Sender owner;

#if DEBUG_LogOperationsInRequest
				[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
				public Base.Logger operations_log = new QS.CMS.Base.Logger(true, null, true, string.Empty);
#endif

				public override void Unregister()
                {
                    owner.unregister(this);
                }

				[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
				public double sendingTime;
				[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
				public double acknowledgementTime = double.NaN;
				[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
				public int nretransmissions;

				#region ITimedRequest Members

				int QS._qss_c_.Multicasting3.ITimedRequest.NumberOfRetransmissions
				{
					get { return nretransmissions; }
				}

				#endregion

				#region ITimedOperation Members

				double QS._qss_c_.Base3_.ITimedOperation.StartingTime
				{
					get { return sendingTime; }
				}

				double QS._qss_c_.Base3_.ITimedOperation.Duration
				{
					get { return acknowledgementTime - sendingTime; }
				}

				#endregion

				public override string ToString()
				{
					return "Request_" + message.ToString();
				}
			}

            #endregion

            #region IReliableSerializableSender Members

            public QS.Fx.Network.NetworkAddress Address
            {
                get { return underlyingSender.Address; }
            }

            public Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                Request request;
                bool sending_immediately = false;
                lock (this)
                {
                    request = new Request(this, ++lastused_seqno, new QS._core_c_.Base3.Message(destinationLOID, data),
                        completionCallback, asynchronousState);
                    if (sending_immediately = outgoingWindow.hasSpace())
                    {
                        if (outgoingWindow.append(request) != request.message.SeqNo)
                            throw new Exception("Assigned and available sequence numbers do not match.");
                    }
                    else
                    {
#if DEBUG_ReliableSender
                        owner.logger.Log(this, "__BeginSend: enqueueing for " + this.Address.ToString() + ", " + request.message.ToString());
#endif

                        outgoingQueue.Enqueue(request);
                    }
                }

                if (sending_immediately)
                {
#if DEBUG_ReliableSender
                    owner.logger.Log(this, "__BeginSend: sending immediately to " + this.Address.ToString() + ", " + request.message.ToString());
#endif

#if DEBUG_LogOperations
					operations_log.logMessage(this, "Sending " + request.message.ToString());
#endif

#if DEBUG_LogOperationsInRequest
					request.operations_log.logMessage(this, "Sending_Immediately");
#endif

					request.sendingTime = QS._core_c_.Base2.PreciseClock.Clock.Time;
					underlyingSender.send((uint) ReservedObjectID.Senders3_ReliableSender_MessageChannel, request.message);
                    request.alarmRef = owner.alarmClock.Schedule(retransmissionTimeout, alarmCallback, request);
                }

                return request;
            }

            public void EndSend(Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            public int MTU
            {
                get { return underlyingSender.MTU - OutgoingRequest.HeaderOverhead; }
            }

            #endregion

            #region ISerializableSender Members

            public void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                Base3_.IAsynchronousOperation asynchronousOperation = this.BeginSend(destinationLOID, data, null, null);
                asynchronousOperation.Ignore();
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotImplementedException();
            }

#endregion

			#region IRetransmittingSender Members

			double QS._qss_c_.FlowControl3.IRetransmittingSender.RetransmissionTimeout
			{
				get { return retransmissionTimeout; }
				set 
                {
                    if (value < minimumRetransmissionTimeout)
                        value = minimumRetransmissionTimeout;
                    if (value > maximumRetransmissionTimeout)
                        value = maximumRetransmissionTimeout;
                    retransmissionTimeout = value; 
                }
			}

			#endregion

			#region IStatisticsCollector Members

			IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
			{
				get { return retransmissionController.Statistics; }
			}

			#endregion

			public override string ToString()
			{
				return "Sender_" + this.Address.ToString();
			}

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if ((underlyingSender != null) && (underlyingSender is IDisposable))
                    ((IDisposable)underlyingSender).Dispose();
            }

            #endregion
        }

        #endregion

		#region ISenderClass<ISerializableSender> Members

		QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>.SenderCollection
		{
			get 
			{
				return new Base3_.SenderCollectionCast<Base3_.IReliableSerializableSender, QS._qss_c_.Base3_.ISerializableSender>(
					((Base3_.ISenderClass<Base3_.IReliableSerializableSender>)this).SenderCollection);
			}
		}

		QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>.CreateSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return this.createSender(destinationAddress);
		}

		#endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
