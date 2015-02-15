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

#define DEBUG_SimpleSender3
#define DEBUG_CheckDeadlock

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
	public class SimpleSender3 : GroupSenderClass<IGroupSender, SimpleSender3.Sender>, ISimpleSender
	{
		public SimpleSender3(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController,
			IRegionSender regionSender, Aggregation3_.IAgent aggregationAgent, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock,
			Base3_.IDemultiplexer demultiplexer)
		{
			this.logger = logger;
			this.membershipController = membershipController;
			this.regionSender = regionSender;
			this.aggregationAgent = aggregationAgent;
			this.clock = clock;
			this.alarmClock = alarmClock;
			this.demultiplexer = demultiplexer;

			demultiplexer.register((uint)ReservedObjectID.Multicasting3_SimpleSender3, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));
		}

		private QS.Fx.Logging.ILogger logger;
		private Membership2.Controllers.IMembershipController membershipController;
		private IRegionSender regionSender;
		private Aggregation3_.IAgent aggregationAgent;
		private QS.Fx.Clock.IClock clock;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private Base3_.IDemultiplexer demultiplexer;

		private FlowControl3.IRateCalculator receivingRateCalculator = new FlowControl3.RateCalculator2();

		#region Receive Callback

		QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{
			QS.Fx.Network.NetworkAddress sourceAddress = sourceIID.Address;

			receivingRateCalculator.sample();

#if DEBUG_SimpleSender3
//			logger.Log(this, Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

			Multicasting3.MulticastMessage message = receivedObject as Multicasting3.MulticastMessage;
			if (message != null)
			{
				Base3_.ViewID viewID = new Base3_.ViewID(message.ID.GroupID, message.ID.ViewSeqNo);
				Aggregation3_.IController aggregationController = aggregationAgent.GetGroup(viewID).GetChannel(
					new QS._core_c_.Base3.InstanceID(sourceAddress, 1)).GetController((int) message.ID.MessageSeqNo);

				if (aggregationController != null)
					aggregationController.Submit(message.Message.transmittedObject, 
						new RegionMin(membershipController.MyRegion.ID, receivingRateCalculator.Rate));

				demultiplexer.dispatch(
					message.Message.destinationLOID, sourceIID, message.Message.transmittedObject);
			}
			else
				throw new Exception("Received message is of incompatible type.");

			return null;
		}

		#endregion

		#region Class Sender

		public class Sender : GroupSender, FlowControl3.IRetransmittingSender, QS._qss_e_.Base_1_.IStatisticsCollector
		{
			public Sender(SimpleSender3 owner, Base3_.GroupID groupID) : base(groupID)
			{
				this.owner = owner;
				outgoingController = new FlowControl2.OutgoingController<Request>(
					5000, 5000, new QS._qss_c_.FlowControl2.ReadyCallback<Request>(outgoingCallback), owner.clock);

				// for now, just resolve it statically
				groupController = owner.membershipController[groupID];

				retransmissionAlarmCallback = new QS.Fx.Clock.AlarmCallback(retransmissionCallback);
				aggregationAsyncCallback = new AsyncCallback(aggregationCallback);

				retransmissionController = new QS._qss_c_.FlowControl3.RetransmissionController1(this);
			}

			private SimpleSender3 owner;
			private Membership2.Controllers.IGroupController groupController;
			private Membership2.Controllers.IGroupViewController groupVC = null;
			private uint lastused_seqno = 0;
			[QS.Fx.Base.Inspectable("Outgoing Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl2.IOutgoingController<Request> outgoingController;
			private QS.Fx.Clock.AlarmCallback retransmissionAlarmCallback;
			private Aggregation3_.IGroup aggregationGroup;
			private AsyncCallback aggregationAsyncCallback;

			private double retransmissionTimeout = 0.01;

			private FlowControl3.IRetransmissionController retransmissionController;

			#region Class Request

			public class Request : MulticastRequest, IRegionRequest, ITimedRequest
			{
				public Request(SimpleSender3.Sender owner, Multicasting3.MulticastMessage message, 
					Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState, 
					IRegionSink[] regionSinks) : base(message, completionCallback, asynchronousState)
				{
					this.owner = owner;
					this.regionSinks = regionSinks;
					regionsToGo = regionSinks.Length;

#if DEBUG_SimpleSender3
                    operation_log = new QS._core_c_.Base.Logger(owner.owner.clock, true, null, false, string.Empty);
#endif                    
				}

				private SimpleSender3.Sender owner;
				// private Multicasting3.MulticastMessage message;
				[QS.Fx.Base.Inspectable("Region Sinks", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private IRegionSink[] regionSinks;
				private int regionsToGo;
				[QS.Fx.Base.Inspectable("Alarm Ref", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private QS.Fx.Clock.IAlarm alarmRef = null;
				[QS.Fx.Base.Inspectable("Aggregation Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private Aggregation3_.IController aggregationController;
				[QS.Fx.Base.Inspectable("Sending Time", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private double sendingTime = double.MinValue;
				[QS.Fx.Base.Inspectable("RTT", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private double roundtripTime = double.MaxValue;
				[QS.Fx.Base.Inspectable("Number of Retransmissions", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private int nretransmissions = 0;

#if DEBUG_SimpleSender3
				[QS.Fx.Base.Inspectable("Operation Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private QS._core_c_.Base.Logger operation_log;
#endif

				#region Accessors

				[QS.Fx.Base.Inspectable]
				public uint SeqNo
				{
					get { return message.ID.MessageSeqNo; }
				}

				public IRegionSink[] RegionSinks
				{
					get { return regionSinks; }
				}

				public Multicasting3.MulticastMessage Message
				{
					get { return message; }
				}

				#endregion

				#region Aggregation Callback

				public void acknowledgementCallback()
				{
#if DEBUG_SimpleSender3
					operation_log.Log(this, "acknowledged");
#endif

					foreach (KeyValuePair<Base3_.RegionID, Multicasting3.Minimum> regionmin in 
						((RegionMin) aggregationController.AggregatedObject).Dictionary)
					{
						owner.owner.regionSender[regionmin.Key].ReceivingRate = regionmin.Value.Value;
					}

					if (sendingTime < 0)
						owner.owner.logger.Log(this, "SENDING TIME INCORRECT!");
					else
					{
						roundtripTime = owner.owner.clock.Time - sendingTime;
						owner.retransmissionController.completed(roundtripTime, nretransmissions);
					}

					// locked internally in Base3.AsynchronousOperation
					this.IsCompleted = true;
				}

				#endregion

				#region Sending

				public void send()
				{
#if DEBUG_SimpleSender3
					operation_log.Log(this, "send");
#endif

					aggregationController = owner.aggregationGroup.MyChannel.GetController((int) message.ID.MessageSeqNo);
					aggregationController.BeginAggregate(owner.aggregationAsyncCallback, this);

					foreach (IRegionSink regionSink in regionSinks)
						regionSink.Submit(this);
				}

				#endregion

				#region Retransmission

				public void resend()
				{
#if DEBUG_CheckDeadlock
					QS._qss_c_.Debugging_.Locking.WaitCheck(this, owner.owner.logger, TimeSpan.FromSeconds(1));
#endif
					lock (this)
					{
#if DEBUG_SimpleSender3
						operation_log.Log(this, "resend");
#endif

						nretransmissions++;

						if (!this.Cancelled)
						{
							regionsToGo = regionSinks.Length;
							foreach (IRegionSink regionSink in regionSinks)
								regionSink.Resubmit(this);
						}
					}
				}

				#endregion

				#region Overriden from Base3.AsynchronousOperation

				public override void Unregister()
				{
#if DEBUG_SimpleSender3
					operation_log.Log(this, "unregister_Enter");
#endif

					alarmRef.Cancel();
					alarmRef = null;

#if DEBUG_SimpleSender3
					operation_log.Log(this, "unregister_Pass1");
#endif

					foreach (IRegionSink regionSink in regionSinks)
						regionSink.Release(this);

#if DEBUG_SimpleSender3
					operation_log.Log(this, "unregister_Pass2");
#endif

					owner.removeComplete(this);

#if DEBUG_SimpleSender3
					operation_log.Log(this, "unregister_Leave");
#endif
				}

				#endregion

				#region IRegionRequest Members

				uint IRegionRequest.DestinationLOID
				{
					get { return (uint) ReservedObjectID.Multicasting3_SimpleSender3; }
				}

				QS.Fx.Serialization.ISerializable IRegionRequest.Data
				{
					get { return message; }
				}

				void IRegionRequest.Completed()
				{
#if DEBUG_SimpleSender3
					operation_log.Log(this, "shipped");
#endif

#if DEBUG_CheckDeadlock
					QS._qss_c_.Debugging_.Locking.WaitCheck(this, owner.owner.logger, TimeSpan.FromSeconds(1));
#endif
					lock (this)
					{
						regionsToGo--;
						if (regionsToGo == 0)
						{
							if (alarmRef == null)
							{
								sendingTime = owner.owner.clock.Time;

								alarmRef = owner.owner.alarmClock.Schedule(
									owner.retransmissionTimeout, owner.retransmissionAlarmCallback, this);
							}
							else
								alarmRef.Reschedule();
						}
					}
				}

				#endregion

				#region ITimedOperation Members

				double Base3_.ITimedOperation.StartingTime
				{
					get { return sendingTime; }
				}

				double Base3_.ITimedOperation.Duration
				{
					get { return roundtripTime; }
				}

				#endregion

				#region ITimedRequest Members

				int ITimedRequest.NumberOfRetransmissions
				{
					get { return nretransmissions; }
				}

				#endregion
			}

			#endregion

			#region Callbacks

			private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
				Request request = alarmRef.Context as Request;
				if (request != null)
				{
#if DEBUG_SimpleSender3
//					owner.logger.Log(this, "__RetransmissionCallback: " + request.Message.ToString());
#endif
					request.resend();
				}
			}

			private void aggregationCallback(IAsyncResult asyncResult)
			{
				Request request = asyncResult.AsyncState as Request;
				if (request != null)
				{
					request.acknowledgementCallback();
				}
				else
					throw new Exception("Request is NULL.");
			}

			private void outgoingCallback(System.Collections.Generic.IEnumerable<Base3_.Seq<Request>> ready_list)
			{
				throw new Exception("Outgoing callback was never supposed to be called!");
				// sendOut(ready_list);
			}

			private void removeComplete(Request request)
			{
				System.Collections.Generic.IEnumerable<Base3_.Seq<Request>> ready = null;

#if DEBUG_CheckDeadlock
				QS._qss_c_.Debugging_.Locking.WaitCheck(this, owner.logger, TimeSpan.FromSeconds(1));
#endif
				lock (this)
				{
					try
					{
						outgoingController.removeCompleted((int)request.SeqNo, out ready);
					}
					catch (Exception exc)
					{
						owner.logger.Log(this, "__RemoveComplete: " + exc.ToString());
					}
				}

				if (ready != null)
					sendOut(ready);
			}

			#endregion

			#region Sending Out

			private void sendOut(Base3_.Seq<Request> request_tosend)
			{
				Request request = request_tosend.Object;
				int seqno = request_tosend.SeqNo;
				if (seqno != request.Message.ID.MessageSeqNo)
					owner.logger.Log(this, "__OutgoingCallback: SeqNo's don't match");
				else
					request.send();
			}

			private void sendOut(System.Collections.Generic.IEnumerable<Base3_.Seq<Request>> requests_tosend)
			{
				foreach (Base3_.Seq<Request> request_tosend in requests_tosend)
					sendOut(request_tosend);
			}

			#endregion

			#region IGroupSender Members

			public override Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data,
				Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
			{
				Request request;
				System.Nullable<Base3_.Seq<Request>> ready_tosend;

#if DEBUG_CheckDeadlock
				QS._qss_c_.Debugging_.Locking.WaitCheck(this, owner.logger, TimeSpan.FromSeconds(1));
#endif
				lock (this)
				{
					Membership2.Controllers.IGroupViewController currentVC = groupController.CurrentView;
					if (groupVC == null || groupVC.SeqNo != currentVC.SeqNo)
					{
						groupVC = currentVC;
						lastused_seqno = 0;

						aggregationGroup = owner.aggregationAgent.GetGroup(new Base3_.ViewID(groupID, groupVC.SeqNo));
					}

					MessageID messageID = new MessageID(groupID, groupVC.SeqNo, ++lastused_seqno);
					Membership2.Controllers.IRegionViewController[] regionVCs = groupVC.RegionViewControllers;

					MulticastMessage message = new MulticastMessage(messageID, new QS._core_c_.Base3.Message(destinationLOID, data));

					IRegionSink[] regionSinks = new IRegionSink[regionVCs.Length];
					for (int ind = 0; ind < regionVCs.Length; ind++)
						regionSinks[ind] = owner.regionSender[regionVCs[ind].Region.ID];

					request = new Request(this, message, completionCallback, asynchronousState, regionSinks);

					outgoingController.schedule(request, out ready_tosend);
				}

				if (ready_tosend.HasValue)
					sendOut(ready_tosend.Value);

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

			#region IStatisticsCollector Members

			IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
			{
				get { return retransmissionController.Statistics; }
			}

			#endregion
		}

		#endregion

		#region Overrides

		protected override Sender createSender(QS._qss_c_.Base3_.GroupID groupID)
		{
			return new Sender(this, groupID);
		}

		#endregion

		#region ISimpleSender Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
				foreach (Sender sender in senderCollection.Values)
				{
                    s.Add(new QS._core_c_.Components.Attribute(sender.GroupID.ToString(),
						new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector) sender).Statistics)));
				}
				return s;
			}
		}

		#endregion
	}
}
