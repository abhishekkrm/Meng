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

// #define DEBUG_Aggregation4_Agent

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation4_
{
	public class Agent : QS.Fx.Inspection.Inspectable, IAgent, Aggregation1_.IAggregationAgent, Base1_.IClient
	{
		public Agent(QS.Fx.Logging.ILogger logger, QS.Fx.Network.NetworkAddress localAddress, IControllerClass controllerClass,
			Base3_.IDemultiplexer demultiplexer, Routing_1_.IRoutingAlgorithm routingAlgorithm, 
			Membership2.Controllers.IMembershipController membershipController)
		{
			this.localAddress = localAddress;
			this.logger = logger;
			this.controllerClass = controllerClass;
			this.routingAlgorithm = routingAlgorithm;
			this.membershipController = membershipController;

			demultiplexer.register(((Base1_.IClient)this).LocalObjectID, new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback));

			inspectableWrapperForGroups =
				new QS._qss_e_.Inspection_.DictionaryWrapper2<QS._qss_c_.Base3_.ViewID, Group>("Groups", groups);
		}

		private QS.Fx.Network.NetworkAddress localAddress;
		private QS.Fx.Logging.ILogger logger;
		private System.Collections.Generic.IDictionary<Base3_.ViewID, Group> groups =
			new SortedDictionary<QS._qss_c_.Base3_.ViewID, Group>();
		private IControllerClass controllerClass;
		private Routing_1_.IRoutingAlgorithm routingAlgorithm;
		private Membership2.Controllers.IMembershipController membershipController;

		[QS.Fx.Base.Inspectable("Groups", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper2<Base3_.ViewID, Group> inspectableWrapperForGroups;

		#region Accessors and Helpers

		private IChannelController GetChannel(Base3_.ViewID viewID, QS.Fx.Network.NetworkAddress rootAddress)
		{
			Group group;
			lock (groups)
			{
				if (groups.ContainsKey(viewID))
					group = groups[viewID];
				else
				{
					group = new Group(this, viewID);
					groups[viewID] = group;
				}
			}

			return group.GetChannel(rootAddress);
		}

		#endregion

		#region Receive Callback

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{
			Message message = receivedObject as Message;
			if (message == null)
				throw new Exception("Wrong message type, " + QS._core_c_.Helpers.ToString.ReceivedObject(sourceIID.Address, receivedObject));

			IChannelController channelController = GetChannel(message.ViewID, message.RootAddress);
			IAggregationController aggregationController = (IAggregationController) channelController[(uint) message.SeqNo];

			if (aggregationController != null)
				aggregationController.Receive(sourceIID, message.DataObject);
			else
			{
#if DEBUG_Aggregation4_Agent
				logger.Log(this, "Cannot deliver message " + Helpers.ToString.ObjectRef(message.DataObject) + 
					" from " + sourceAddress.ToString() + " to controller {" + message.ViewID.ToString() + ", " + 
					message.RootAddress.ToString() + ", " + message.SeqNo.ToString() + "}.");
#endif
			}

			return null;
		}

		#endregion

		#region Class Message

		[QS.Fx.Serialization.ClassID(ClassID.Aggregation4_Agent_Message)]
		private class Message : QS.Fx.Serialization.ISerializable
		{
			public Message()
			{
			}

			public Message(Base3_.ViewID viewID, QS.Fx.Network.NetworkAddress rootAddress, int seqno, QS.Fx.Serialization.ISerializable dataObject)
			{
				this.ViewID = viewID;
				this.SeqNo = seqno;
				this.DataObject = dataObject;
				this.RootAddress = rootAddress;
			}

			public Base3_.ViewID ViewID;
			public int SeqNo;
			public QS.Fx.Network.NetworkAddress RootAddress;
			public QS.Fx.Serialization.ISerializable DataObject;

			#region ISerializable Members

			QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
			{
				get 
				{
					QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
						(ushort)ClassID.Aggregation4_Agent_Message, sizeof(int) + sizeof(ushort), sizeof(int) + sizeof(ushort), 0);
					info.AddAnother(((QS.Fx.Serialization.ISerializable) ViewID).SerializableInfo);
					info.AddAnother(RootAddress.SerializableInfo);
					if (DataObject != null)
						info.AddAnother(DataObject.SerializableInfo);
					return info;
				}
			}

			unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
			{
				((QS.Fx.Serialization.ISerializable)ViewID).SerializeTo(ref header, ref data);
				RootAddress.SerializeTo(ref header, ref data);
				fixed (byte* arrayptr = header.Array)
				{
					byte *headerptr = arrayptr + header.Offset;
					*((int *)(headerptr)) = SeqNo;
					*((ushort*)(headerptr + sizeof(int))) = 
						(DataObject != null) ? DataObject.SerializableInfo.ClassID : ((ushort) ClassID.Nothing);
				}
				header.consume(sizeof(int) + sizeof(ushort));
				if (DataObject != null)
					DataObject.SerializeTo(ref header, ref data);
			}

			unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
			{
				ViewID = new QS._qss_c_.Base3_.ViewID();
				((QS.Fx.Serialization.ISerializable)ViewID).DeserializeFrom(ref header, ref data);
				RootAddress = new QS.Fx.Network.NetworkAddress();
				RootAddress.DeserializeFrom(ref header, ref data);
				ushort classID;
				fixed (byte* arrayptr = header.Array)
				{
					byte* headerptr = arrayptr + header.Offset;
					SeqNo = *((int*)(headerptr));
					classID = *((ushort*)(headerptr + sizeof(int)));
				}
				header.consume(sizeof(int) + sizeof(ushort));
				if (classID != (ushort) ClassID.Nothing)
				{
					DataObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
					DataObject.DeserializeFrom(ref header, ref data);
				}
				else
					DataObject = null;
			}

			#endregion

			public override string ToString()
			{
				return "Message({" + ViewID.ToString() + ", " + RootAddress.ToString() + ", " + SeqNo.ToString() + "}, " +
					QS._core_c_.Helpers.ToString.ObjectRef(DataObject) + ")";
			}
		}

		#endregion

		#region Class Group

		private class Group : QS.Fx.Inspection.Inspectable
		{
			public Group(Agent owner, Base3_.ViewID viewID)
			{
				this.viewID = viewID;
				this.owner = owner;

				viewController =(Membership2.Controllers.IGroupViewController)
					owner.membershipController[viewID.GroupID][viewID.ViewSeqNo];

				List<QS.Fx.Network.NetworkAddress> addresses = new List<QS.Fx.Network.NetworkAddress>();
				foreach (QS._core_c_.Base3.InstanceID instanceID in viewController.Members)
					addresses.Add(instanceID.Address);

				// this may need to be changed to a mutable structure some time later...
				routingStructure = new Routing_2_.ImmutableStructure<QS.Fx.Network.NetworkAddress>(
                    addresses, owner.routingAlgorithm, viewID.GroupID.GetHashCode());

				inspectableWrapperForChannels =
					new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, Channel>("Channels", channels);
			}

			private Base3_.ViewID viewID;
			private Agent owner;
			private System.Collections.Generic.IDictionary<QS.Fx.Network.NetworkAddress, Channel> channels =
				new SortedDictionary<QS.Fx.Network.NetworkAddress, Channel>();
			private Membership2.Controllers.IGroupViewController viewController;
			private Routing_2_.IStructure<QS.Fx.Network.NetworkAddress> routingStructure;

			[QS.Fx.Base.Inspectable("Channels", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, Channel> inspectableWrapperForChannels;

			#region Accessors

			public Base3_.ViewID ViewID
			{
				get { return viewID; }
			}

			public IChannelController GetChannel(QS.Fx.Network.NetworkAddress rootAddress)
			{
				lock (channels)
				{
					if (channels.ContainsKey(rootAddress))
						return channels[rootAddress];
					else
					{
						Channel channel = new Channel(this, rootAddress);
						channels[rootAddress] = channel;
						return channel;
					}
				}
			}

			#endregion

			#region Class Channel

			public class Channel : QS.Fx.Inspection.Inspectable, IChannelController
			{
				public Channel(Group owner, QS.Fx.Network.NetworkAddress rootAddress)
				{
					this.rootAddress = rootAddress;
					this.owner = owner;

					controllers = new Components_1_.SeqCollection<IAggregationController>(
						new QS._qss_c_.Components_1_.SeqCollection<IAggregationController>.Constructor(CreateController));

					incomingAddresses = owner.routingStructure.Incoming(owner.owner.localAddress, rootAddress);
					IList<QS.Fx.Network.NetworkAddress> outgoingAddresses = 
						owner.routingStructure.Outgoing(owner.owner.localAddress, rootAddress);
					if (outgoingAddresses.Count > 1)
						throw new Exception("Bad routing structure used: cannot have more than one outgoing address.");
					if (outgoingAddresses.Count > 0)
						outgoingAddress = outgoingAddresses[0];
				}

				private Group owner;
				[QS.Fx.Base.Inspectable("Controllers", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private Components_1_.ISeqCollection<IAggregationController> controllers;
				private QS.Fx.Network.NetworkAddress rootAddress;

				private IList<QS.Fx.Network.NetworkAddress> incomingAddresses;
				private QS.Fx.Network.NetworkAddress outgoingAddress;

				#region Accessors

				public Group Group
				{
					get { return owner; }
				}

				public QS.Fx.Network.NetworkAddress RootAddress
				{
					get { return rootAddress; }
				}

				#endregion

				#region IChannel Members

				IAggregation IChannel.this[uint seqno]
				{
					get { return controllers.lookup((int)seqno); }
				}

				#endregion

				#region IChannelController Members

				QS._core_c_.Base3.Message IChannelController.CreateMessage(
					IAggregationController sender, QS.Fx.Serialization.ISerializable dataObject)
				{
					return new QS._core_c_.Base3.Message((uint)ReservedObjectID.Aggregation4_Agent,
						new Message(owner.viewID, rootAddress, sender.SeqNo, dataObject));
				}

				void IChannelController.RemoveCompleted(IAggregationController completedAggregation)
				{
					controllers.remove(completedAggregation.SeqNo);
					completedAggregation.Dispose();
				}

				#endregion

				private IAggregationController CreateController(int seqno)
				{
					IAggregationController controller = owner.owner.controllerClass.CreateController();
					controller.Initialize(this, seqno, incomingAddresses, outgoingAddress);
					return controller;
				}
			}

			#endregion
		}

		#endregion

		#region IAgent Members

		Routing_1_.IRoutingAlgorithm IAgent.RoutingAlgorithm
		{
			set { routingAlgorithm = value; }
		}

		public IChannel GetChannel(QS._qss_c_.Base3_.GroupID groupID, uint viewSeqNo)
		{
			return GetChannel(groupID, viewSeqNo, localAddress);
		}

		public IChannel GetChannel(QS._qss_c_.Base3_.GroupID groupID, uint viewSeqNo, QS.Fx.Network.NetworkAddress rootAddress)
		{
			return GetChannel(new Base3_.ViewID(groupID, viewSeqNo), rootAddress);		
		}

		#endregion		
	
		#region Class CompatibilityController

		private class CompatibilityController : QS.Fx.Inspection.Inspectable, Aggregation1_.IAggregationController
		{
			public CompatibilityController(IAggregationController controller, Aggregation1_.AggregationCallback callback)
			{
				this.controller = controller;
				this.callback = callback;
			}

			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			private IAggregationController controller;
			private Aggregation1_.AggregationCallback callback;

			public void completionCallback(IAsyncResult asynchronousResult)
			{
				callback();
				controller.EndAggregate(asynchronousResult);
			}

			#region IAggregationController Members

			QS._qss_c_.Aggregation1_.AggregationID QS._qss_c_.Aggregation1_.IAggregationController.ID
			{
				get 
				{ 
					Group.Channel channel = (Group.Channel) controller.Channel;
					return new QS._qss_c_.Aggregation1_.AggregationID(new Multicasting3.MessageID(
						channel.Group.ViewID.GroupID, channel.Group.ViewID.ViewSeqNo, (uint) controller.SeqNo), 
						new QS._core_c_.Base3.InstanceID(channel.RootAddress, 1));
				}
			}

			QS._qss_c_.Aggregation3_.IAggregatable QS._qss_c_.Aggregation1_.IAggregationController.AggregatedObject
			{
				get { return controller.AggregatedObject; }
			}

			#endregion
		}

		#endregion

		#region IAggregationAgent Members

		void QS._qss_c_.Aggregation1_.IAggregationAgent.registerClass(QS._qss_c_.Aggregation1_.IAggregationClass aggregationClass)
		{
			throw new NotSupportedException();
		}

		QS._qss_c_.Aggregation1_.IAggregationController QS._qss_c_.Aggregation1_.IAggregationAgent.aggregate(QS._qss_c_.Aggregation1_.IAggregationKey aggregationKey, QS._qss_c_.Aggregation1_.AggregationCallback aggregationCallback)
		{
			Multicasting3.MessageID messageID = aggregationKey as Multicasting3.MessageID;
			if (messageID == null)
				throw new ArgumentException("Incorrect key type.");

			IAggregationController aggregationController = (IAggregationController)
				((IAgent)this).GetChannel(messageID.GroupID, messageID.ViewSeqNo)[messageID.MessageSeqNo];
			
			CompatibilityController compatibilityController = 
				new CompatibilityController(aggregationController, aggregationCallback);

			aggregationController.BeginAggregate(compatibilityController.completionCallback, null);

			return compatibilityController;
		}

		void QS._qss_c_.Aggregation1_.IAggregationAgent.submit(QS._qss_c_.Aggregation1_.IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress)
		{
			throw new NotSupportedException();
		}

		void QS._qss_c_.Aggregation1_.IAggregationAgent.submit(QS._qss_c_.Aggregation1_.IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Aggregation3_.IAggregatable toAggregate)
		{
			Multicasting3.MessageID messageID = aggregationKey as Multicasting3.MessageID;
			if (messageID == null)
				throw new ArgumentException("Incorrect key type.");

			IAggregation aggregationController = ((IAgent)this).GetChannel(
				messageID.GroupID, messageID.ViewSeqNo, rootAddress.Address)[messageID.MessageSeqNo];

			if (aggregationController != null)
				aggregationController.Submit(data, toAggregate);
			else
			{
				// controller must have been removed
#if DEBUG_Aggregation4_Agent
				logger.Log(this, "Cannot submit <" + aggregationKey.ToString() + ", " + rootAddress.ToString() + "," +
					Helpers.ToString.Object(data) + ", " + Helpers.ToString.Object(toAggregate) + ">, controller must have been removed.");
#endif
			}
		}

		#endregion

		#region IClient Members

		uint QS._qss_c_.Base1_.IClient.LocalObjectID
		{
			get { return (uint) ReservedObjectID.Aggregation4_Agent; }
		}

		#endregion
	}
}
