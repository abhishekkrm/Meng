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

// #define DEBUG_SimpleSender
#define Calculate_Statistics
#define RetransmissionTimeout_AutomaticAdjustment
#define WindowSize_AutomaticAdjustment
// #define DEBUG_AggregationRequests

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
    public class SimpleSender1 : GroupSenderClass<IGroupSender, SimpleSender1.Sender>, ISimpleSender
    {
        public SimpleSender1(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController, 
            Base3_.IDemultiplexer demultiplexer, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection, 
            Aggregation1_.IAggregationAgent aggregationAgent, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, double retransmissionTimeout,
            int outgoingBufferSize, int windowSize) : base()
        {
            this.logger = logger;
            this.membershipController = membershipController;
            this.demultiplexer = demultiplexer;
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.windowSize = windowSize;
            this.aggregationAgent = aggregationAgent;
			this.clock = clock;
			this.alarmClock = alarmClock;
            this.retransmissionTimeout = retransmissionTimeout;
			this.outgoingBufferSize = outgoingBufferSize;

			demultiplexer.register((uint) ReservedObjectID.Multicasting3_SimpleSender, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private Membership2.Controllers.IMembershipController membershipController;
        private Base3_.IDemultiplexer demultiplexer;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
        private int outgoingBufferSize, windowSize;
        private Aggregation1_.IAggregationAgent aggregationAgent;
		private QS.Fx.Clock.IClock clock;
		private QS.Fx.Clock.IAlarmClock alarmClock;
        private double retransmissionTimeout;

#if DEBUG_AggregationRequests
		private Base.Logger aggregation_logger = new QS.CMS.Base.Logger(true, null, false, "");
#endif
	
		#region Calculating Statistics

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get
			{
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
				foreach (Base3_.GroupID groupID in senderCollection.Keys)
				{
					Sender sender = (Sender) senderCollection[groupID];
                    s.Add(new QS._core_c_.Components.Attribute(groupID.ToString(),
						new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector) sender).Statistics)));
				}
				return s;
			}
		}

/*
		public System.Collections.Generic.IList<QS.CMS.Components.Attribute> Statistics
		{
			get
			{
				System.Collections.Generic.IList<QS.CMS.Components.Attribute> statistics = new List<QS.CMS.Components.Attribute>();
				foreach (Sender sender in senderCollection.Values)
				{
					foreach (QS.CMS.Components.Attribute statistic in sender.Statistics)						
						statistics.Add(new QS.CMS.Components.Attribute("Sender[" + sender.GroupID + "]." + statistic.Name, statistic.Value));
				}
				return statistics;
			}
		}
*/

		#endregion

        #region Receive Callback

		QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_SimpleSender
            logger.Log(this, "__ReceiveCallback : " + Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

            Multicasting3.MulticastMessage message = receivedObject as Multicasting3.MulticastMessage;
            if (message != null)
            {
                demultiplexer.dispatch(message.Message.destinationLOID, sourceIID, message.Message.transmittedObject);

#if DEBUG_AggregationRequests
				aggregation_logger.Log("Submit " + message.ID.ToString());
				try
				{
#endif

				// nasty hack
                aggregationAgent.submit(message.ID, sourceIID);

#if DEBUG_AggregationRequests
				}
				catch (Exception exc)
				{
					logger.Log(this, "\nAggregation.Submit failed : " + exc.ToString() + "\n" + exc.StackTrace + 
						"\nAggregation log:\n" + aggregation_logger.CurrentContents + "\n\n");
					throw new Exception("Aggregation.Submit failed", exc);
				}
#endif

            }
            else
                throw new Exception("Received message is of incompatible type.");

            return null;
        }

        #endregion

        #region Class Sender

		public class Sender : GroupSender, FlowControl3.IRetransmittingSender, FlowControl3.IWindowSender, QS._qss_e_.Base_1_.IStatisticsCollector
		{
            public Sender(SimpleSender1 owner, Base3_.GroupID groupID) : base(groupID)
            {
#if DEBUG_SimpleSender
                owner.logger.Log(this, "__constructor: Creating new SimpleSender for group " + groupID.ToString());
#endif

                this.owner = owner;
                retransmissionAlarmCallback = new QS.Fx.Clock.AlarmCallback(retransmissionCallback);
				retransmissionTimeout = owner.retransmissionTimeout;

				// outgoingWindow = new FlowControl.OutgoingWindow((uint) owner.windowSize);
                // outgoingQueue = new Queue<Request>();
				outgoingController = new FlowControl2.OutgoingController<Request>(
					owner.outgoingBufferSize, owner.windowSize, 
                    new QS._qss_c_.FlowControl2.ReadyCallback<Request>(outgoingCallback), owner.clock);

				retransmissionController = new FlowControl3.RetransmissionController1(this);
				windowController = new FlowControl3.WindowController1(owner.clock, this);
			}

            private SimpleSender1 owner;
            private QS.Fx.Clock.AlarmCallback retransmissionAlarmCallback;

            private Membership2.Controllers.IGroupController groupController;
			private Membership2.Controllers.IGroupViewController viewController;

			// private FlowControl.IOutgoingWindow outgoingWindow;
            // private Queue<Request> outgoingQueue;
			[QS.Fx.Base.Inspectable("Outgoing Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl2.IOutgoingController<Request> outgoingController;

			// do not remove
			public string Moo
			{
				get { return null; }
			}

			[QS.Fx.Base.Inspectable("Current Maximum Window Size")]			
			public int WindowSize
			{
				get { return outgoingController.WindowSize; }
			}

			[QS.Fx.Base.Inspectable("Requests Currently In Transit")]
			public int Concurrency
			{
				get { return outgoingController.Concurrency; }
			}

			[QS.Fx.Base.Inspectable("Current Retransmission Timeout", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private double retransmissionTimeout;

			[QS.Fx.Base.Inspectable("Retransmission Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IRetransmissionController retransmissionController;

			[QS.Fx.Base.Inspectable("Window Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IWindowController windowController;

			#region Calculating Statistics and Automatic Adjustment

			private void addSample(Request request)
			{
				retransmissionController.completed(request.roundtripTime, request.nretransmissions);
//-				windowController.completed(requestSeqNo, nretransmissions);
				windowController.completed(request, request.nretransmissions);
			}

			#endregion

			#region Calculating Statistics

			public System.Collections.Generic.IList<QS._core_c_.Components.Attribute> Statistics
			{
				get 
				{
					System.Collections.Generic.List<QS._core_c_.Components.Attribute> list = new List<QS._core_c_.Components.Attribute>();
					list.AddRange(retransmissionController.Statistics);
					list.AddRange(windowController.Statistics);
					return list;
				}
			}

			#endregion

            #region Class Request

			public class Request : MulticastRequest, ITimedRequest
			{
				public override string ToString()
				{
					return "Request: " + message.ToString() + " for " + owner.GroupID.ToString();
				}

				public Request(SimpleSender1.Sender owner, Multicasting3.MulticastMessage message, Membership2.Controllers.IGroupViewController sendingView,
					Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) 
					: base(message, completionCallback, asynchronousState)
                {
                    this.owner = owner;
                    this.sendingView = sendingView;
                }

                private SimpleSender1.Sender owner;
				private Membership2.Controllers.IGroupViewController sendingView;
                private QS.Fx.Clock.IAlarm alarmRef = null;

#if Calculate_Statistics
				public double sendingTime = double.MinValue;
				public int nretransmissions = 0;
				public double roundtripTime = double.MaxValue;
#endif

                public override void Unregister()
                {
                    alarmRef.Cancel();
                    owner.removeComplete(message.ID.MessageSeqNo);
                }

                #region Accessors

                public QS.Fx.Clock.IAlarm AlarmRef
                {
                    get { return alarmRef; }
                    set { alarmRef = value; }
                }

                public Multicasting3.MulticastMessage Message
                {
                    get { return message; }
                }

				public Membership2.Controllers.IGroupViewController SendingView
				{
                    get { return sendingView; }
                }

                #endregion

                public void acknowledgementCallback()
                {
#if DEBUG_SimpleSender
                    owner.owner.logger.Log(this, "__AcknowledgementCallback: " + message.ToString());
#endif
                    this.IsCompleted = true;
                }

				#region ITimedRequest Members

				int ITimedRequest.NumberOfRetransmissions
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
					get { return roundtripTime; }
				}

				#endregion
			}

            #endregion

			private void outgoingCallback(System.Collections.Generic.IEnumerable<Base3_.Seq<Request>> ready_list)
			{
				foreach (Base3_.Seq<Request> ready_element in ready_list)
				{
					Request request = ready_element.Object;
					int seqno = ready_element.SeqNo;

					if (seqno != request.Message.ID.MessageSeqNo)
						owner.logger.Log(this, "__outgoingCallback: SeqNo's don't match");
					else
						processOutgoing(request);
				}
			}

			// this is a nasty hack, for now
			[QS.Fx.Base.Inspectable("Number of Messages Sent", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private volatile uint lastused_seqno = 0;

            private void processOutgoing(Request request)
            {
#if DEBUG_SimpleSender
                owner.logger.Log(this, "__ProcessOutgoing[" + this.groupID.ToString() + "]: " + request.Message.ID.ToString());
#endif

//-				windowController.sending(request.Message.ID.MessageSeqNo);

				if (request.AlarmRef == null)
				{
#if Calculate_Statistics
					if (request.sendingTime < 0)
						request.sendingTime = owner.clock.Time;
#endif

					request.AlarmRef = owner.alarmClock.Schedule(retransmissionTimeout, retransmissionAlarmCallback, request);
				}
				else
					request.AlarmRef.Reschedule();

#if DEBUG_AggregationRequests
				owner.aggregation_logger.Log("Aggregate " + request.Message.ID.ToString());
#endif

				owner.aggregationAgent.aggregate(request.Message.ID, new QS._qss_c_.Aggregation1_.AggregationCallback(request.acknowledgementCallback));

				foreach (QS.Fx.Network.NetworkAddress address in request.SendingView.MulticastAddresses)
                    owner.underlyingSenderCollection[address].send((uint)ReservedObjectID.Multicasting3_SimpleSender, request.Message);
            }

            private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
#if DEBUG_SimpleSender
                owner.logger.Log(this, "__RetransmissionCallback");
#endif

                Request request = alarmRef.Context as Request;
                if (request != null)
                {
                    if (!request.Cancelled)
                    {
#if Calculate_Statistics
						System.Threading.Interlocked.Increment(ref request.nretransmissions);
#endif

						// for now let's just resend all this crap again everywhere and re-aggregate and see what happens
                        processOutgoing(request);
                    }
                }
            }

            private void removeComplete(uint sequenceNo)
            {
                lock (this)
                {
#if DEBUG_SimpleSender
					owner.logger.Log(this, "removeComplete: " + sequenceNo.ToString());
#endif

                    try
                    {
                        // outgoingWindow.remove(sequenceNo);
						Request request = outgoingController.removeCompleted((int) sequenceNo);

#if Calculate_Statistics
						request.roundtripTime = owner.clock.Time - request.sendingTime;
						addSample(request);
#endif
					}
                    catch (Exception exc)
                    {
						owner.logger.Log(this, "removeComplete: " + exc.ToString());
					}

                    // while (outgoingWindow.hasSpace() && outgoingQueue.Count > 0)
                    // {
                    //    Request request = outgoingQueue.Dequeue();
                    //    if (outgoingWindow.append(request) != request.Message.ID.MessageSeqNo)
                    //        throw new Exception("Assigned and available sequence numbers do not match.");

// #if DEBUG_SimpleSender
//                        owner.logger.Log(this, "__BeginSend: sending enqueued for " + this.Address.ToString() + ", " + request.message.ToString());
// #endif

                    //    processOutgoing(request);
                    // }
                }
            }

            #region IGroupSender Members

            public override Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                Request request;
                // bool sending_immediately = false;

                lock (this)
                {
                    if (groupController == null)
                    {
                        groupController = owner.membershipController[groupID];
                        viewController = null;
                    }

                    Membership2.Controllers.IGroupViewController currentViewController = groupController.CurrentView;
                    if (viewController == null || viewController.SeqNo != currentViewController.SeqNo)
                    {
                        viewController = groupController.CurrentView;
                    }

                    Multicasting3.MessageID messageID = new MessageID(groupID, viewController.SeqNo, ++lastused_seqno);
                    Multicasting3.MulticastMessage multicastMessage = new MulticastMessage(messageID, new QS._core_c_.Base3.Message(destinationLOID, data));

                    request = new Request(this, multicastMessage, viewController, completionCallback, asynchronousState);

                    // if (sending_immediately = outgoingWindow.hasSpace())
                    // {
                    //    if (outgoingWindow.append(request) != request.Message.ID.MessageSeqNo)
                    //        throw new Exception("Assigned and available sequence numbers do not match.");
                    // }
                    // else
                    // {
// #if DEBUG_SimpleSender
//                        owner.logger.Log(this, "__BeginSend: enqueueing for " + this.Address.ToString() + ", " + request.message.ToString());
// #endif

                    //    outgoingQueue.Enqueue(request);
                    // }

					outgoingController.schedule(request);


				}

                // if (sending_immediately)
                // {
// #if DEBUG_SimpleSender
//                        owner.logger.Log(this, "__BeginSend: sending immediately to " + this.Address.ToString() + ", " + request.message.ToString());
// #endif

                //    processOutgoing(request);
                // }

                return request;
            }

            public override void EndSend(Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            public override int MTU
            {
                get { throw new System.NotImplementedException(); }
            }

            #endregion

			#region IRetransmittingSender Members

			double QS._qss_c_.FlowControl3.IRetransmittingSender.RetransmissionTimeout
			{
				get { return retransmissionTimeout; }
				set { retransmissionTimeout = value; }
			}

			#endregion

			#region IWindowSender Members

			int QS._qss_c_.FlowControl3.IWindowSender.WindowSize
			{
				get { return outgoingController.WindowSize; }
				set { outgoingController.WindowSize = value; }
			}

			#endregion
		}

        #endregion

        protected override Sender createSender(QS._qss_c_.Base3_.GroupID groupID)
        {
            return new Sender(this, groupID);
        }
    }
}
