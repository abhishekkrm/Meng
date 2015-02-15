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

// #define DEBUG_LogOperations
// #define DEBUG_CollectForwardingStatistics

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation4_
{
	public class AggregationController1 : QS.Fx.Inspection.Inspectable, 
		Base1_.IClient, IControllerClass, FlowControl3.IRetransmittingSender, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		public AggregationController1(QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, double initialForwardingTimeout,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> notificationSenders,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> forwardingSenders)
		{
			this.alarmClock = alarmClock;
			this.clock = clock;
			this.forwardingTimeout = initialForwardingTimeout;
			this.notificationSenders = notificationSenders;
			this.forwardingSenders = forwardingSenders;

			forwardingTimeoutController = new FlowControl3.RetransmissionController1(this);
		}

		public AggregationController1()
		{
		}

		#region Accessors

		public bool ForwardingAllowed
		{
			set { forwardingAllowed = value; }
		}

		public Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> NotificationSenders
		{
			set { notificationSenders = value; }
		}
		
		public Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> ForwardingSenders
		{
			set { forwardingSenders = value; }
		}
		
		#endregion

		private bool forwardingAllowed;

		private QS.Fx.Clock.IAlarmClock alarmClock;
		private QS.Fx.Clock.IClock clock;
		private double forwardingTimeout;
		private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> notificationSenders, forwardingSenders;

		[QS.Fx.Base.Inspectable]
		private FlowControl3.IRetransmissionController forwardingTimeoutController;

#if DEBUG_CollectForwardingStatistics
		[TMS.Inspection.Inspectable]
		private Statistics.Samples nforwardingsSent = new QS.CMS.Statistics.Samples();
		[TMS.Inspection.Inspectable]
		private Statistics.Samples nforwardingsReceived = new QS.CMS.Statistics.Samples();
		[TMS.Inspection.Inspectable]
		private Statistics.Samples forwardingsHelped = new QS.CMS.Statistics.Samples();

		private void forwardingsSample(double sent, double received, bool forwardingCameFirst)
		{
			lock (this)
			{
				forwardingsHelped.addSample(forwardingCameFirst ? 1 : 0);
				nforwardingsSent.addSample(sent);
				nforwardingsReceived.addSample(received);
			}
		}
#endif

		[QS.Fx.Base.Inspectable]
		private class Controller : QS.Fx.Inspection.Inspectable, IAggregationController, IAsyncResult
		{
			#region System.IDisposable Members

			void System.IDisposable.Dispose()
			{
#if DEBUG_LogOperations
				operationsLog.Dispose();
#endif
			}

			#endregion

			public Controller(AggregationController1 owner)
			{
				this.owner = owner;
				timeCreated = owner.clock.Time;
			}

#if DEBUG_LogOperations
			private enum ControllerOperation
			{
				SEND, FORWARDING_CALLBACK, RECEIVED_DATA, RECEIVED_NOTIFICATION, BAD_ACK, 
				BAD_CLEANUP, SET_COMPLETED, DO_FORWARDING, STOP_FORWARDING, FINALIZE_CLEANUP, 
				INITIALIZE, SUBMIT, BEGIN_AGGREGATE, END_AGGREGATE
			}

			// private Base.Logger operationsLog = new QS.CMS.Base.Logger(true, null, true, string.Empty);
			[TMS.Inspection.Inspectable("Operations Log", TMS.Inspection.AttributeAccess.ReadOnly)]
			private Base3.ILogOf<Base3.LoggedOperation> operationsLog = new Base3.LogOf<Base3.LoggedOperation>();
#endif

			private AggregationController1 owner;
			private IChannelController channelController;
			private int seqNo;
			private AsyncCallback completionCallback;
			private object asynchronousState;
			private Aggregation3_.IAggregatable aggregatedObject;
			private System.Collections.Generic.IDictionary<QS.Fx.Network.NetworkAddress, IncomingPeer> incomingPeers;
			private OutgoingPeer outgoingPeer;
			private QS.Fx.Serialization.ISerializable dataObject;
			private bool forwardingAllowed = false;
			private QS.Fx.Clock.IAlarm forwardingAlarm;
			private bool isCompleted = false;
			private bool childrenCompleted = false;
			private int acknowledgementsToGo, forwardingPeerCount;
			private bool doingCleanup = false;
			private double timeCreated;
			private int nforwardingsSent = 0;
#if DEBUG_CollectForwardingStatistics
			private int nforwardingsReceived = 0;
			private bool forwardingCameFirst = false;
#endif
			#region Sending

			private void send(Controller.SendReq sendreq, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> senders)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "Send: " + 
				//	Helpers.ToString.ReceivedObject(sendreq.Address, sendreq.SerializableObject));
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.SEND, sendreq.Address, sendreq.SerializableObject));
#endif

				QS._core_c_.Base3.Message message = channelController.CreateMessage(this, sendreq.SerializableObject);
				senders[sendreq.Address].send(message.destinationLOID, message.transmittedObject);
			}

			private void send(System.Collections.Generic.IList<Controller.SendReq> notifications,
				System.Collections.Generic.IList<Controller.SendReq> forwardings)
			{
				if (notifications != null && notifications.Count > 0)
				{
					foreach (Controller.SendReq sendreq in notifications)
						send(sendreq, owner.notificationSenders);
				}

				if (forwardings != null && forwardings.Count > 0)
				{
// #if DEBUG_CollectForwardingStatistics
					nforwardingsSent++;
// #endif
					foreach (Controller.SendReq sendreq in forwardings)
						send(sendreq, owner.forwardingSenders);
				}
			}

			#endregion

			#region Classes for Peers

			#region Class Peer

			[QS.Fx.Base.Inspectable]
			private abstract class Peer : QS.Fx.Inspection.Inspectable
			{
				public Peer(QS.Fx.Network.NetworkAddress address)
				{
					this.Address = address;
				}

				public QS.Fx.Network.NetworkAddress Address;
				public bool ReceivedData = false;
			}

			#endregion

			#region Class IncomingPeer

			[QS.Fx.Base.Inspectable]
			private class IncomingPeer : Peer
			{
				public IncomingPeer(QS.Fx.Network.NetworkAddress address) : base(address)
				{
				}

				public bool SubtreeCompleted = false;
			}

			#endregion

			#region Class OutgoingPeer

			[QS.Fx.Base.Inspectable]
			private class OutgoingPeer : Peer
			{
				public OutgoingPeer(QS.Fx.Network.NetworkAddress address) : base(address)
				{
				}
			}

			#endregion

			#endregion

			#region Classes for Notifications

			[QS.Fx.Serialization.ClassID(ClassID.Aggregation4_AggregationController1_Notification)]
			private class Notification : QS.Fx.Serialization.ISerializable
			{
				public enum Type : ushort
				{
					ACK,							// this node has received data and does not need forwarding
					SUBTREE_ACK,			// all subtree rooted at this node received data
					CLEANUP,					// the whole tree has data and knows about it, we now do cleanup, top-down
					FLOOD_SUBTREE
				}

				public Notification()
				{
				}

				public Notification(Type type)
				{
					this.type = type;
				}

				private Type type;

				#region Accessors

				public Type TypeOf
				{
					get { return type; }
				}

				#endregion

				#region ISerializable Members

				QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
				{
					get
					{
						return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Aggregation4_AggregationController1_Notification,
							sizeof(ushort), sizeof(ushort), 0);
					}
				}

				unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
				{
					fixed (byte* arrayptr = header.Array)
					{
						byte* headerptr = arrayptr + header.Offset;
						*((ushort*)headerptr) = (ushort)type;
					}
					header.consume(sizeof(ushort));
				}

				unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
				{
					fixed (byte* arrayptr = header.Array)
					{
						byte* headerptr = arrayptr + header.Offset;
						type = (Type) (*((ushort*)headerptr));
					}
					header.consume(sizeof(ushort));
				}

				#endregion

				public override string ToString()
				{
					return type.ToString();
				}
			}

			#endregion

			#region Struct SendReq

			public struct SendReq
			{
				public SendReq(QS.Fx.Network.NetworkAddress address, QS.Fx.Serialization.ISerializable serializableObject)
				{
					this.Address = address;
					this.SerializableObject = serializableObject;
				}

				public QS.Fx.Network.NetworkAddress Address;
				public QS.Fx.Serialization.ISerializable SerializableObject;
			}

			#endregion

			#region Forwarding Callback

			private void forwardingCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "ForwardingCallback");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.FORWARDING_CALLBACK));
#endif

				System.Collections.Generic.List<SendReq> forwardings = new List<SendReq>();

				lock (this)
				{
					if (!isCompleted)
					{
						forwardingAllowed = true;

						if (forwardingPeerCount > 0)
						{
							if (dataObject != null)
								DoForwarding(forwardings);

							alarmRef.Reschedule();
						}
						else
						{
							try
							{
								alarmRef.Cancel();
							}
							catch (Exception)
							{
							}

							forwardingAlarm = null;
						}
					}
				}

				send(null, forwardings);
			}

			#endregion

			#region Internal Processing

			private void ReceivedData(QS.Fx.Serialization.ISerializable data, QS._qss_c_.Aggregation3_.IAggregatable toAggregate)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "ReceivedData: (" + Helpers.ToString.ObjectRef(data) + ", " +
				//	Helpers.ToString.ObjectRef(toAggregate) + ")");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.RECEIVED_DATA, data, toAggregate));
#endif

				System.Collections.Generic.List<SendReq> notifications = new List<SendReq>();
				System.Collections.Generic.List<SendReq> forwardings = new List<SendReq>();

				bool invokeCallback = false, finalizeCleanup = false;

				lock (this)
				{
					if (!isCompleted && dataObject == null)
					{
						if (data == null)
							throw new ArgumentException("The submitted data object is empty!");
						dataObject = data;

						if (aggregatedObject == null)
							aggregatedObject = toAggregate;
						else
							aggregatedObject.aggregateWith(toAggregate);

						if (childrenCompleted)
						{
							SetCompleted(notifications, ref invokeCallback, ref finalizeCleanup);
						}
						else
						{
							if (owner.forwardingAllowed)
							{
								if (outgoingPeer != null)
									notifications.Add(new SendReq(outgoingPeer.Address, new Notification(Notification.Type.ACK)));

								foreach (IncomingPeer incomingPeer in incomingPeers.Values)
									notifications.Add(new SendReq(incomingPeer.Address, new Notification(Notification.Type.ACK)));
							}
						}

						DoForwarding(forwardings);
					}
				}

				send(notifications, forwardings);

				if (invokeCallback)
					completionCallback(this);

				if (finalizeCleanup)
					FinalizeCleanup();
			}

			private void ReceivedNotification(QS._core_c_.Base3.InstanceID sourceIID, Notification notification)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "ReceivedNotification: " + 
				//	Helpers.ToString.ReceivedObject(sourceAddress, notification));
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.RECEIVED_NOTIFICATION, sourceAddress, notification));
#endif

				System.Collections.Generic.List<SendReq> notifications = new List<SendReq>();
				System.Collections.Generic.List<SendReq> forwardings = new List<SendReq>();

				bool invokeCallback = false, finalizeCleanup = false;

				lock (this)
				{
					switch (notification.TypeOf)
					{
						case Notification.Type.ACK:
						case Notification.Type.SUBTREE_ACK:
						{
							Peer peer = null;

							if (outgoingPeer != null && sourceIID.Address.Equals(outgoingPeer.Address))
							{
								peer = outgoingPeer;
							}
							else if (incomingPeers.ContainsKey(sourceIID.Address))
							{
								peer = incomingPeers[sourceIID.Address];
							}
							else
							{
#if DEBUG_LogOperations
								// operationsLog.logMessage(this, "Ack received from an unknown node.");
								operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
									ControllerOperation.BAD_ACK));
#endif
							}

							if (peer != null && !peer.ReceivedData)
							{
								peer.ReceivedData = true;
								forwardingPeerCount--;
								if (forwardingPeerCount == 0)
									StopForwarding();
							}

							if (notification.TypeOf == Notification.Type.SUBTREE_ACK)
							{
								IncomingPeer incomingPeer = peer as IncomingPeer;
								if (incomingPeer != null && !incomingPeer.SubtreeCompleted)
								{
									incomingPeer.SubtreeCompleted = true;
									acknowledgementsToGo--;

									if (acknowledgementsToGo == 0)
									{
										childrenCompleted = true;

										if (dataObject != null)
										{
											SetCompleted(notifications, ref invokeCallback, ref finalizeCleanup);
										}
									}
								}
							}
						}
						break;

						case Notification.Type.FLOOD_SUBTREE:
						{
/*
							foreach (IncomingPeer incomingPeer in incomingPeers)
							{
								if (!incomingPeer.ReceivedData)

							}
*/
						}
						break;

						case Notification.Type.CLEANUP:
						{
							if (!doingCleanup)
							{
								if (outgoingPeer != null && sourceIID.Address.Equals(outgoingPeer.Address))
								{
									InitiateCleanup(notifications);
									finalizeCleanup = true;
								}
								else
								{
#if DEBUG_LogOperations
									// operationsLog.logMessage(this, "Cleanup request received from unknown node.");
									operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
										ControllerOperation.BAD_CLEANUP));
#endif
								}
							}
						}
						break;
					}
				}

				send(notifications, forwardings);

				if (invokeCallback)
					completionCallback(this);

				if (finalizeCleanup)
					FinalizeCleanup();
			}

			private void InitiateCleanup(System.Collections.Generic.List<SendReq> notifications)
			{
				doingCleanup = true;

				foreach (IncomingPeer incomingPeer in incomingPeers.Values)
					notifications.Add(new SendReq(incomingPeer.Address, new Notification(Notification.Type.CLEANUP)));
			}

			private void SetCompleted(
				System.Collections.Generic.List<SendReq> notifications, ref bool invokeCallback, ref bool finalizeCleanup)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "SetCompleted");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.SET_COMPLETED));
#endif

				isCompleted = true;

				if (outgoingPeer != null)
				{
					notifications.Add(
						new SendReq(outgoingPeer.Address, new Notification(Notification.Type.SUBTREE_ACK)));
				}
				else
				{
					InitiateCleanup(notifications);

					if (completionCallback != null)
					{
						invokeCallback = true;
					}
					else
					{
						finalizeCleanup = true;
					}
				}
			}

			private void DoForwarding(System.Collections.Generic.List<SendReq> forwardings)
			{
				if (dataObject == null)
					throw new Exception("Cannot do forwarding, data object not received.");

#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "DoForwarding");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.DO_FORWARDING));
#endif

				if (forwardingAllowed)
				{
					if (!childrenCompleted)
					{
						foreach (IncomingPeer incomingPeer in incomingPeers.Values)
						{
							if (!incomingPeer.ReceivedData)
								forwardings.Add(new SendReq(incomingPeer.Address, dataObject));
						}
					}

					if (outgoingPeer != null && !outgoingPeer.ReceivedData)
						forwardings.Add(new SendReq(outgoingPeer.Address, dataObject));
				}
			}

			private void StopForwarding()
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "StopForwarding");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.STOP_FORWARDING));
#endif

				double time_now = owner.clock.Time;

				if (forwardingAlarm != null)
				{
					try
					{
						forwardingAlarm.Cancel();
					}
					catch (Exception)
					{
					}

					forwardingAlarm = null;
				}

				owner.forwardingTimeoutController.completed(time_now - timeCreated, 0); // nforwardingsSent
			}

			private void FinalizeCleanup()
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "FinalizeCleanup");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.FINALIZE_CLEANUP));
#endif

#if DEBUG_CollectForwardingStatistics
				owner.forwardingsSample(nforwardingsSent, nforwardingsReceived, forwardingCameFirst);
#endif

				channelController.RemoveCompleted(this);
			}

			#endregion

			#region IAggregationController Members

			void IAggregationController.Initialize(IChannelController channelController, int seqNo,
				System.Collections.Generic.ICollection<QS.Fx.Network.NetworkAddress> incomingAddresses, QS.Fx.Network.NetworkAddress outgoingAddress)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "Initialize(" + seqNo.ToString() + ", { " +
				//	Helpers.CollectionHelper.ToStringSeparated<QS.Fx.Network.NetworkAddress>(incomingAddresses, ", ") + " }, " +
				//	Helpers.ToString.Object(outgoingAddress) + ")");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.INITIALIZE, seqNo, incomingAddresses, outgoingAddress));
#endif

				lock (this)
				{
					this.channelController = channelController;
					this.seqNo = seqNo;
					incomingPeers = new System.Collections.Generic.Dictionary<QS.Fx.Network.NetworkAddress, IncomingPeer>(incomingAddresses.Count);
					foreach (QS.Fx.Network.NetworkAddress incomingAddress in incomingAddresses)
						incomingPeers[incomingAddress] = new IncomingPeer(incomingAddress);
					outgoingPeer = (outgoingAddress != null) ? new OutgoingPeer(outgoingAddress) : null;

					if (owner.forwardingAllowed)
					{
						forwardingAlarm = owner.alarmClock.Schedule(
							owner.forwardingTimeout, new QS.Fx.Clock.AlarmCallback(forwardingCallback), null);
					}

					acknowledgementsToGo = incomingPeers.Count;
					childrenCompleted = (acknowledgementsToGo == 0);
					forwardingPeerCount = acknowledgementsToGo + ((outgoingPeer != null) ? 1 : 0);
				}
			}

			void IAggregationController.ReplaceIncoming(QS.Fx.Network.NetworkAddress deadAddress,
				System.Collections.Generic.ICollection<QS.Fx.Network.NetworkAddress> replacementAddresses)
			{
				// ............................................................................................................................................................
			}

			void IAggregationController.ReplaceOutgoing(QS.Fx.Network.NetworkAddress deadAddress, QS.Fx.Network.NetworkAddress replacementAddress)
			{
				// ............................................................................................................................................................
			}

			void IAggregationController.Receive(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
			{
// #if DEBUG_LogOperations
//				operationsLog.logMessage(this, "Receive: " + Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
// #endif

				Notification notification = receivedObject as Notification;
				if (notification != null)
				{
					ReceivedNotification(sourceIID, notification);
				}
				else
				{
#if DEBUG_CollectForwardingStatistics
					nforwardingsReceived++;
					if (dataObject == null)
						forwardingCameFirst = true;
#endif

					((IAggregation)this).Submit(receivedObject, null);
				}
			}

			#endregion

			#region IAggregation Members

			IAsyncResult IAggregation.BeginAggregate(AsyncCallback completionCallback, object asynchronousState)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "BeginAggregate");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.BEGIN_AGGREGATE));
#endif

				if (outgoingPeer != null)
					throw new ArgumentException("Cannot aggregate here because this node is not the aggregation root.");

				this.completionCallback = completionCallback;
				this.asynchronousState = asynchronousState;

				return this;
			}

			void IAggregation.EndAggregate(IAsyncResult asynchronousResult)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "EndAggregate");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.END_AGGREGATE));
#endif

				FinalizeCleanup();
			}

			void IAggregation.Submit(QS.Fx.Serialization.ISerializable data, QS._qss_c_.Aggregation3_.IAggregatable toAggregate)
			{
#if DEBUG_LogOperations
				// operationsLog.logMessage(this, "Submit(" + Helpers.ToString.ObjectRef(data) + ", " +
				//	Helpers.ToString.ObjectRef(toAggregate) + ")");
				operationsLog.Add(Base3.LoggedOperation.Operation<ControllerOperation>(
					ControllerOperation.SUBMIT, data, toAggregate));
#endif

				ReceivedData(data, toAggregate);
			}

			IChannel IAggregation.Channel
			{
				get { return channelController; }
			}

			int IAggregation.SeqNo
			{
				get { return seqNo; }
			}

			QS._qss_c_.Aggregation3_.IAggregatable IAggregation.AggregatedObject
			{
				get { return aggregatedObject; }
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
				get { return isCompleted; }
			}

			#endregion
		}

		#region IClient Members

		uint QS._qss_c_.Base1_.IClient.LocalObjectID
		{
			get { return (uint)ReservedObjectID.Aggregation4_AggregationController1 ; }
		}

		#endregion

		#region IControllerClass Members

		IAggregationController IControllerClass.CreateController()
		{
			return new Controller(this);
		}

		#endregion

		#region IRetransmittingSender Members

		double QS._qss_c_.FlowControl3.IRetransmittingSender.RetransmissionTimeout
		{
			get { return forwardingTimeout; }
			set { forwardingTimeout = value; }
		}

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                return Helpers_.ListOf<QS._core_c_.Components.Attribute>.Collection(new QS._core_c_.Components.Attribute[] {
#if DEBUG_CollectForwardingStatistics
					new Components.Attribute("Forwardings Sent", nforwardingsSent.DataSet),
					new Components.Attribute("Forwardings Received", nforwardingsReceived.DataSet),
					new Components.Attribute("Forwardings Helped", forwardingsHelped.DataSet),
#endif			
					new QS._core_c_.Components.Attribute("Forwarding Timeout Controller", 
						new QS._core_c_.Components.AttributeSet(forwardingTimeoutController.Statistics))
				});
			}
		}

		#endregion
	}
}
