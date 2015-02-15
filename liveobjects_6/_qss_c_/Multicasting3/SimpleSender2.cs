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

// #define DEBUG_SimpleSender2
// #define DEBUG_DeadlockDetection
// #define DEBUG_RequestOperations
// #define DEBUG_LogReqStatistics
// #define DEBUG_LogReceivedMessages

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class SimpleSender2 : GroupSenderClass<IGroupSender, SimpleSender2.Sender>, ISimpleSender
	{
        public SimpleSender2(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController,
			Aggregation1_.IAggregationAgent aggregationAgent, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, Base3_.IDemultiplexer demultiplexer,
            Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> underlyingInstanceSenderCollection,
			double initialRetransmissionTimeout, int groupBufferSize, int initialGroupWindowSize, int initialRegionWindowSize,
			RegionController.FlowControlScheme flowControlScheme)
		{
			this.defaultFCScheme = flowControlScheme;
			this.logger = logger;
			this.membershipController = membershipController;
			this.groupBufferSize = groupBufferSize;
			this.initialGroupWindowSize = initialGroupWindowSize;
			this.initialRegionWindowSize = initialRegionWindowSize;
			this.aggregationAgent = aggregationAgent;
			this.alarmClock = alarmClock;
			this.clock = clock;
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.underlyingInstanceSenderCollection = underlyingInstanceSenderCollection;
			this.initialRetransmissionTimeout = initialRetransmissionTimeout;
			this.demultiplexer = demultiplexer;
			demultiplexer.register((uint) ReservedObjectID.Multicasting3_SimpleSender, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));
			demultiplexer.register((uint)ReservedObjectID.RegionGossipingChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.regionGossipingCallback));
			regionControllerCollection = new QS._core_c_.Collections.Hashtable(100);

			regionControllersProxy = new QS._qss_e_.Inspection_.DictionaryWrapper(
				"Region Controllers", regionControllerCollection, new QS._qss_e_.Inspection_.DictionaryWrapper.ConversionCallback(Base3_.RegionID.String2Object));

			logger.Log(this, "Default Flow Control Scheme: " + flowControlScheme.ToString());
		}

        private QS.Fx.Logging.ILogger logger;
		private Membership2.Controllers.IMembershipController membershipController;
		[QS.Fx.Base.Inspectable("Aggregation Agent", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Aggregation1_.IAggregationAgent aggregationAgent;
		private int groupBufferSize, initialGroupWindowSize, initialRegionWindowSize;
		private QS._core_c_.Collections.Hashtable regionControllerCollection;
		private QS.Fx.Clock.IClock clock;
		private QS.Fx.Clock.IAlarmClock alarmClock;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> underlyingInstanceSenderCollection;
		private Base3_.IDemultiplexer demultiplexer;
		private double initialRetransmissionTimeout;

		[QS.Fx.Base.Inspectable("Region Controllers")]
		private QS._qss_e_.Inspection_.DictionaryWrapper regionControllersProxy;

		private FlowControl3.IRateCalculator receivingRateCalculator = new FlowControl3.RateCalculator2();

#if DEBUG_LogReceivedMessages
		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private TMS.Inspection.AutoCollection<QS.Fx.Network.NetworkAddress, Base.Logger> received_logs =
			new QS.TMS.Inspection.AutoCollection<QS.Fx.Network.NetworkAddress, QS.CMS.Base.Logger>();
#endif

		#region Calculating Statistics

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get
			{
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();

				QS._core_c_.Components.AttributeSet a = new QS._core_c_.Components.AttributeSet();				
				foreach (Sender sender in senderCollection.Values)
				{
                    a.Add(new QS._core_c_.Components.Attribute(sender.GroupID.ToString(),
						new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector)sender).Statistics)));
				}
				s.Add(new QS._core_c_.Components.Attribute("Group Senders", a));

				a = new QS._core_c_.Components.AttributeSet();
				foreach (RegionController region_controller in regionControllerCollection.Values)
				{
                    a.Add(new QS._core_c_.Components.Attribute(region_controller.RegionID.ToString(),
						new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector)region_controller).Statistics)));
				}
				s.Add(new QS._core_c_.Components.Attribute("Region Controllers", a));

				return s;
			}
		}

/*
		public System.Collections.Generic.IList<QS.CMS.Components.Attribute> Statistics
		{
			get
			{
				System.Collections.Generic.IList<QS.CMS.Components.Attribute> statistics = new List<QS.CMS.Components.Attribute>();
				foreach (RegionController regionController in regionControllerCollection.Values)
					foreach (QS.CMS.Components.Attribute statistic in regionController.Statistics)
						statistics.Add(new QS.CMS.Components.Attribute("RegionController[" + regionController.RegionID + "]." + statistic.Name, statistic.Value));
				foreach (Sender sender in senderCollection.Values)
					foreach (QS.CMS.Components.Attribute statistic in sender.Statistics)
						statistics.Add(new QS.CMS.Components.Attribute("Sender[" + sender.GroupID + "]." + statistic.Name, statistic.Value));
				return statistics;
			}
		}
*/

		#endregion

		#region Receive Callback

		QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{
#if DEBUG_LogReceivedMessages
			received_logs[sourceAddress].logMessage(this, Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

			receivingRateCalculator.sample();

#if DEBUG_SimpleSender2
            logger.Log(this, "__ReceiveCallback : " + Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

			Multicasting3.MulticastMessage message = receivedObject as Multicasting3.MulticastMessage;
			if (message != null)
			{
				demultiplexer.dispatch(message.Message.destinationLOID, sourceIID, message.Message.transmittedObject);

				// nasty hack
				aggregationAgent.submit(message.ID, sourceIID, message.Message.transmittedObject,
					new RegionMin(membershipController.MyRID, receivingRateCalculator.Rate));
			}
			else
				throw new Exception("Received message is of incompatible type.");

			return null;
		}

		#endregion

		private QS.Fx.Serialization.ISerializable regionGossipingCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{


			return null;
		}

		#region Class RegionController

		private RegionController.FlowControlScheme defaultFCScheme = RegionController.FlowControlScheme.WINDOW;

		public class RegionController : QS.Fx.Inspection.Inspectable, QS._qss_e_.Base_1_.IStatisticsCollector
		{
			public RegionController(SimpleSender2 owner, Membership2.Controllers.IRegionController membershipRegionController)
				: this(owner, membershipRegionController, owner.defaultFCScheme)
			{
			}

			public RegionController(SimpleSender2 owner, Membership2.Controllers.IRegionController membershipRegionController,
				FlowControlScheme flowControlScheme)
			{
				this.owner = owner;
				underlyingSender = owner.underlyingSenderCollection[membershipRegionController.Address];
				this.membershipRegionController = membershipRegionController;

				this.flowControlScheme = flowControlScheme;

				// outgoingController = new FlowControl2.OutgoingController<Request>(owner.regionBufferSize, owner.initialRegionWindowSize,
				//	new QS.CMS.FlowControl2.ReadyCallback<Request>(outgoingCallback));

				switch (flowControlScheme)
				{
					case FlowControlScheme.WINDOW:
					{
						FlowControl3.QueueFlowController<Sender.Request> flowController =
							new FlowControl3.QueueFlowController<Sender.Request>(owner.initialRegionWindowSize,
							new FlowControl3.ReadyCallback<Sender.Request>(readyCallback));
						this.flowController = flowController;
						windowController = new FlowControl3.WindowController1(owner.clock, flowController);
					}
					break;

					case FlowControlScheme.RATE:
					{
						FlowControl3.RateFlowController<Sender.Request> flowController =
							new FlowControl3.RateFlowController<Sender.Request>(owner.clock, owner.alarmClock,
							100, 10, new FlowControl3.ReadyCallback<Sender.Request>(readyCallback));
						this.flowController = flowController;
						rateController = new FlowControl3.RateController1(
							flowController, new FlowControl3.SmoothingEstimator(0.9, 1000), 1.2);
					}
					break;
				}
			}

			public enum FlowControlScheme
			{
				WINDOW, RATE
			}
			private FlowControlScheme flowControlScheme;
			private SimpleSender2 owner;
			private QS._qss_c_.Base3_.ISerializableSender underlyingSender;

			private Membership2.Controllers.IRegionController membershipRegionController;

			[QS.Fx.Base.Inspectable("Window Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IWindowController windowController;

			[QS.Fx.Base.Inspectable("Rate Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IRateController rateController;

			[QS.Fx.Base.Inspectable("Flow Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IFlowController<Sender.Request> flowController;

			#region Accessors

			public Base3_.RegionID RegionID
			{
				get { return membershipRegionController.ID; }
			}

			#endregion

			private void readyCallback(IEnumerable<Sender.Request> toSend)
			{
				foreach (Sender.Request request in toSend)
					sendOut(request);
			}

			private void sendOut(Sender.Request request)
			{
#if DEBUG_SimpleSender2
				owner.logger.Log(this, "Sending out " + request.Message.ID.ToString() + " to " + this.RegionID.ToString() + " via address " +
					underlyingSender.Address.ToString());
#endif

				underlyingSender.send((uint)ReservedObjectID.Multicasting3_SimpleSender, request.Message);
				request.shipped(this);
			}

			public void submit(Sender.Request groupRequest)
			{
#if DEBUG_DeadlockDetection
				Debugging.Locking.Check(this, owner.logger);
#endif

				flowController.submit(groupRequest);
			}

			public void release(Sender.Request groupRequest)
			{
#if DEBUG_SimpleSender2
				owner.logger.Log(this, "__RegionController(" + membershipRegionController.ID.ToString() + ") releasing " + 
					groupRequest.Message.ID.ToString());
#endif

				if (flowControlScheme == FlowControlScheme.WINDOW)
				{
//-					windowController.completed(groupRequest.Message.ID.MessageSeqNo, groupRequest.nretransmissions);
					windowController.completed(groupRequest, groupRequest.nretransmissions);
				}

#if DEBUG_DeadlockDetection
				Debugging.Locking.Check(this, owner.logger);
#endif
				
				flowController.release();
			}

			public void resubmit(Sender.Request groupRequest)
			{
				flowController.resubmit(groupRequest);
				// sendOut(groupRequest);
			}

//			#region IWindowSender Members
//
//			int QS.CMS.FlowControl3.IWindowSender.WindowSize
//			{
//				get { return windowSize; }
//				set 
//				{ 
//					// should lock and do some work, but we ignore it for now...
//					windowSize = value; 
//				}
//			}
//
//			#endregion

			#region IStatisticsCollector Members

			public IList<QS._core_c_.Components.Attribute> Statistics
			{
				get 
				{
                    List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
					if (windowController != null)
						s.AddRange(windowController.Statistics);
					if (rateController != null)
						s.AddRange(((QS._qss_e_.Base_1_.IStatisticsCollector) rateController).Statistics);
					return s;
				}
			}

			#endregion

			public double ReceivingRate
			{
				set 
				{ 
					if (flowControlScheme == FlowControlScheme.RATE)
						rateController.Rate = value;
				}
			}
		}

		#endregion

		private RegionController lookupRegionController(Base3_.RegionID regionID)
		{
			lock (this)
			{
				try
				{
					return (RegionController)regionControllerCollection[regionID];
				}
				catch (Exception exc)
				{
					StringBuilder s = new StringBuilder("__lookupRegionController(" + regionID.ToString() + 
						"), cannot find this region; existing regions: ");
					foreach (object o in regionControllerCollection.Keys)
						s.Append(o.ToString() + " ");

					throw new Exception(s.ToString(), exc);
				}
			}
		}

		private RegionController lookupRegionController(Membership2.Controllers.IRegionController membershipRegionController)
		{
			lock (this)
			{
				QS._core_c_.Collections.IDictionaryEntry dic_en = regionControllerCollection.lookupOrCreate(membershipController.MyRID);
				RegionController regionController = (RegionController)dic_en.Value;
				if (regionController == null)
				{
					regionController = new RegionController(this, membershipRegionController);
					dic_en.Value = regionController;
				}

				return regionController;
			}
		}

        #region Class Sender

        [QS._core_c_.Diagnostics.ComponentContainer]
		public class Sender : GroupSender, FlowControl3.IRetransmittingSender, QS._qss_e_.Base_1_.IStatisticsCollector
		{
			public Sender(SimpleSender2 owner, Base3_.GroupID groupID) : base(groupID)
			{
				this.owner = owner;
				retransmissionAlarmCallback = new QS.Fx.Clock.AlarmCallback(retransmissionCallback);
				retransmissionTimeout = owner.initialRetransmissionTimeout;

				outgoingController = new FlowControl2.OutgoingController<Request>(
					owner.groupBufferSize, owner.initialGroupWindowSize, 
                    new QS._qss_c_.FlowControl2.ReadyCallback<Request>(outgoingCallback), owner.clock);

				retransmissionController = new FlowControl3.RetransmissionController1(this);
			}

			private SimpleSender2 owner;
			private Membership2.Controllers.IGroupController groupController = null;
			private Membership2.Controllers.IGroupViewController groupViewController = null;
			private RegionController[] regionControllers = null;

			[QS.Fx.Base.Inspectable("Outgoing Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl2.IOutgoingController<Request> outgoingController;

			private double retransmissionTimeout;
			private QS.Fx.Clock.AlarmCallback retransmissionAlarmCallback;

			[QS.Fx.Base.Inspectable("Retransmission Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private FlowControl3.IRetransmissionController retransmissionController;

			// this is a nasty hack, for now
			[QS.Fx.Base.Inspectable("Number of Messages Submitted", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private uint lastused_seqno = 0;

//			private Statistics.Samples completionTimes = new QS.CMS.Statistics.Samples();

#if DEBUG_LogReqStatistics
            [QS.CMS.Diagnostics.Component("Completion Times")]
			private Statistics.SamplesXY completionTimes = new QS.CMS.Statistics.SamplesXY();
            [QS.CMS.Diagnostics.Component("Retransmission Factors")]
			private Statistics.SamplesXY retransmissionFactors = new QS.CMS.Statistics.SamplesXY();
#endif

			private void addSample(Request request)
			{
#if DEBUG_LogReqStatistics
				if (request.nretransmissions == 0)
					completionTimes.addSample(request.sendingTime, request.roundtripTime);
				retransmissionFactors.addSample(request.sendingTime, request.nretransmissions);
#endif

				retransmissionController.completed(request.roundtripTime, request.nretransmissions);				
//				completionTimes.addSample(completionTime);
			}

			#region Calculating Statistics

			public System.Collections.Generic.IList<QS._core_c_.Components.Attribute> Statistics
			{
				get 
				{
                    List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
					s.AddRange(retransmissionController.Statistics);
#if DEBUG_LogReqStatistics
					s.Add(new QS.CMS.Components.Attribute("Completion Times", completionTimes.DataSet));
					s.Add(new QS.CMS.Components.Attribute("Retransmission Factors", retransmissionFactors.DataSet));
#endif
					return s; 
				}
			}

			#endregion

			#region Class Request

			public class Request : MulticastRequest, ITimedRequest
			{
				public Request(SimpleSender2.Sender owner, Multicasting3.MulticastMessage message, Membership2.Controllers.IGroupViewController sendingVC,
					RegionController[] regionControllers, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) 
					: base(message, completionCallback, asynchronousState)
				{
					this.owner = owner;
					this.sendingVC = sendingVC;
					this.regionControllers = regionControllers;

					regions_pending = regionControllers.Length;
				}

				[QS.Fx.Base.Inspectable("Aggregation Controller", QS.Fx.Base.AttributeAccess.ReadOnly)]
				public Aggregation1_.IAggregationController aggregationController = null;

				private SimpleSender2.Sender owner;
				private Membership2.Controllers.IGroupViewController sendingVC;
				private RegionController[] regionControllers;

				[QS.Fx.Base.Inspectable("RetransmissionAlarmRef", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private QS.Fx.Clock.IAlarm alarmRef = null;
				[QS.Fx.Base.Inspectable("Number of Regions Pending", QS.Fx.Base.AttributeAccess.ReadOnly)]
				public int regions_pending;

				public double sendingTime = double.MinValue;
				public double roundtripTime = double.MaxValue;
				public int nretransmissions = 0;

#if DEBUG_RequestOperations
				[QS.TMS.Inspection.Inspectable("Operations Log", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
				public QS.CMS.Base.Logger requestOperationsLog = new QS.CMS.Base.Logger(true, null, false, string.Empty);
#endif

				public void shipped(RegionController regionController)
				{
#if DEBUG_RequestOperations
					requestOperationsLog.writeLine("shipped(" + regionController.RegionID.ToString() + ")_begin");
#endif

#if DEBUG_DeadlockDetection
					Debugging.Locking.Check(this, owner.owner.logger);
#endif
					lock (this)
					{
#if DEBUG_SimpleSender2
						owner.owner.logger.Log(this, "Shipped " + message.ID.ToString() + " to " + regionController.RegionID.ToString() + ".");
#endif

						regions_pending--;
						if (regions_pending == 0)
						{
#if DEBUG_SimpleSender2
							owner.owner.logger.Log(this, "Shipped " + message.ID.ToString() + " everywhere, now setting up alarm for " + 
								owner.retransmissionTimeout.ToString() + " seconds.");
#endif

							if (alarmRef == null)
							{
								if (sendingTime < 0)
									sendingTime = owner.owner.clock.Time;

								alarmRef = owner.owner.alarmClock.Schedule(owner.retransmissionTimeout, owner.retransmissionAlarmCallback, this);
							}
							else
								alarmRef.Reschedule();
						}
					}

#if DEBUG_RequestOperations
					requestOperationsLog.writeLine("shipped(" + regionController.RegionID.ToString() + ")_end");
#endif
				}

				public override void Unregister()
				{
#if DEBUG_RequestOperations
					requestOperationsLog.writeLine("unregister_begin");
#endif

#if DEBUG_SimpleSender2
					owner.owner.logger.Log(this, "__Unregistering " + message.ID.ToString());
#endif

					// lock (this)
					// {						
					try
					{
						if (alarmRef != null)
						{
							alarmRef.Cancel();
							alarmRef = null;
						}
					}
					catch (Exception exc)
					{
						owner.owner.logger.Log(this, "__Unregister: Could not cancel alarm.\n" + exc.ToString());
					}

					// }

					foreach (RegionController regionController in regionControllers)
						regionController.release(this);
					owner.removeComplete(message.ID.MessageSeqNo);

#if DEBUG_RequestOperations
					requestOperationsLog.writeLine("unregister_end");
#endif
				}

				#region Accessors

				public Multicasting3.MulticastMessage Message
				{
					get { return message; }
				}

				public Membership2.Controllers.IGroupViewController SendingView
				{
					get { return sendingVC; }
				}

				public RegionController[] RegionControllers
				{
					get { return regionControllers; }
				}

				#endregion

				public void acknowledgementCallback()
				{
#if DEBUG_RequestOperations
					requestOperationsLog.writeLine("acknowledgementCallback_begin");
#endif

					RegionMin regionMin = aggregationController.AggregatedObject as RegionMin;
					if (regionMin != null)
					{
						foreach (KeyValuePair<Base3_.RegionID, Multicasting3.Minimum> some_min in regionMin.Dictionary)
						{
							owner.owner.lookupRegionController(some_min.Key).ReceivingRate = some_min.Value.Value;
						}
					}

					this.IsCompleted = true;

#if DEBUG_RequestOperations
					requestOperationsLog.writeLine("acknowledgementCallback_end");
#endif
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

			#region Callbacks

			private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
				Request request = alarmRef.Context as Request;
				if (request != null)
				{
#if DEBUG_SimpleSender2
					owner.logger.Log(this, "__RetransmissionCallback: " + request.Message.ToString());
#endif

#if DEBUG_RequestOperations
					request.requestOperationsLog.writeLine("retransmissionCallback_begin");
#endif

//					lock (request)
//					{
						if (!request.Cancelled)
						{
							request.nretransmissions++;

							request.regions_pending = request.RegionControllers.Length;
							foreach (RegionController regionController in request.RegionControllers)
							{
								// regionController.release(request);
								// regionController.submit(request);
								regionController.resubmit(request);
							}
						}
//					}

#if DEBUG_RequestOperations
						request.requestOperationsLog.writeLine("retransmissionCallback_end");
#endif
				}
			}

			private void removeComplete(uint sequenceNo)
			{
#if DEBUG_SimpleSender2
				owner.logger.Log(this, "__RemoveComplete_Enter: " + sequenceNo.ToString());

//				if (!System.Threading.Monitor.TryEnter(this, TimeSpan.FromSeconds(3)))
//				{
//					owner.logger.Log(this, "__RemoveComplete: Cannot grab a lock.");
//					throw new Exception("Cannot lock.");
//				}
//				else
//					System.Threading.Monitor.Exit(this);
#endif

#if DEBUG_DeadlockDetection
				Debugging.Locking.Check(this, owner.logger);
#endif
				lock (this) // thread #1 cannot lock
				{
					try
					{
						Request request = outgoingController.removeCompleted((int) sequenceNo);

#if DEBUG_RequestOperations
						request.requestOperationsLog.writeLine("removeComplete");
#endif

#if DEBUG_SimpleSender2
						owner.logger.Log(this, "__RemoveComplete: Message " + request.Message.ID.ToString());
#endif

						if (request.sendingTime < 0)
							throw new Exception("SENDING TIME INCORRECT!");
						else
						{
							request.roundtripTime = owner.clock.Time - request.sendingTime;
							addSample(request);
						}
					}
					catch (Exception exc)
					{
						owner.logger.Log(this, "removeComplete: " + exc.ToString());
					}
				}

#if DEBUG_SimpleSender2
				owner.logger.Log(this, "__RemoveComplete_Leave: " + sequenceNo.ToString());
#endif
			}

			private void processOutgoing(Request request)
			{
				// owner.logger.Log(this, "__processOutgoing: Aggregating " + request.Message.ToString());

#if DEBUG_RequestOperations
				request.requestOperationsLog.writeLine("processOutgoing: begin, aggregate");
#endif

				request.aggregationController = owner.aggregationAgent.aggregate(
                    request.Message.ID, new QS._qss_c_.Aggregation1_.AggregationCallback(request.acknowledgementCallback));

				// owner.logger.Log(this, "__processOutgoing: Sending out " + request.Message.ToString());

#if DEBUG_RequestOperations
				request.requestOperationsLog.writeLine("processOutgoing: submit");
#endif

				foreach (RegionController regionController in request.RegionControllers)
					regionController.submit(request);

#if DEBUG_RequestOperations
				request.requestOperationsLog.writeLine("processOutgoing: completed");
#endif
			}

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

			#endregion

			#region IGroupSender Members

			public override Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data,
				Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
			{
				Request request = null;

#if DEBUG_SimpleSender2
//				try
//				{
//					if (!System.Threading.Monitor.TryEnter(this, TimeSpan.FromSeconds(3)))
//						throw new Exception("__BeginSend: Cannot grab a lock.");
//
//					System.Threading.Monitor.Exit(this);
//				}
//				catch (Exception exc)
//				{
//					owner.logger.Log(this, exc.ToString() + "\n" + exc.StackTrace.ToString());
//					throw new Exception("Cannot send.\n", exc);
//				}
#endif

#if DEBUG_DeadlockDetection
				Debugging.Locking.Check(this, owner.logger);
#endif
				lock (this)
				{
					if (groupController == null)
					{
						groupController = owner.membershipController[groupID];
						groupViewController = null;
					}

					if (groupViewController == null || groupViewController.SeqNo != groupController.CurrentView.SeqNo)
					{
						groupViewController = groupController.CurrentView;

						Membership2.Controllers.IRegionViewController[] regionViewControllers = groupViewController.RegionViewControllers;
						regionControllers = new RegionController[regionViewControllers.Length];
						for (int ind = 0; ind < regionViewControllers.Length; ind++)
							regionControllers[ind] = owner.lookupRegionController(regionViewControllers[ind].RegionController);
					}

					Multicasting3.MessageID messageID = new MessageID(groupID, groupViewController.SeqNo, ++lastused_seqno);
					Multicasting3.MulticastMessage multicastMessage = new MulticastMessage(messageID, new QS._core_c_.Base3.Message(destinationLOID, data));

					request = new Request(this, multicastMessage, groupViewController, regionControllers, completionCallback, asynchronousState);

					outgoingController.schedule(request);
				}

				return request;
			}

			public override void EndSend(Base3_.IAsynchronousOperation asynchronousOperation)
			{
				// .....................................................
				throw new System.NotImplementedException();
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
		}

		#endregion

		protected override Sender createSender(QS._qss_c_.Base3_.GroupID groupID)
		{
			return new Sender(this, groupID);
		}
	}
}
