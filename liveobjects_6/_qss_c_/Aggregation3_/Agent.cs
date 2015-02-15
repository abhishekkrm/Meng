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

// #define DEBUG_AggregationAgent
// #define DEBUG_LogOperations

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation3_
{
	public class Agent : QS.Fx.Inspection.Inspectable, IAgent, Aggregation1_.IAggregationAgent
	{
		public Agent(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localAddress, System.Type controllerClass, 
			IRouter aggregationRouter, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> senderCollection, 
			Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock, 
			Base3_.Constructor<Components_1_.ISeqCollection<Controller>> controllerCollectionConstructor)
		{
			if (!typeof(Controller).IsAssignableFrom(controllerClass))
				throw new ArgumentException("Controller class " + QS._core_c_.Helpers.ToString.Object(controllerClass) + 
					" is not a subtype of " + typeof(Controller).FullName);
			if ((controllerClassConstructor = controllerClass.GetConstructor(Type.EmptyTypes)) == null)
				throw new Exception("Cannot find a no-argument constructor for " + QS._core_c_.Helpers.ToString.Object(controllerClass));

			this.aggregationRouter = aggregationRouter;
			this.logger = logger;
			this.localAddress = localAddress;
			this.senderCollection = senderCollection;
			this.alarmClock = alarmClock;
			this.controllerCollectionConstructor = controllerCollectionConstructor;

			demultiplexer.register((uint)ReservedObjectID.Aggregation3_AggregationAgent,
				new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));

			inspectableAggregationGroupProxy = 
				new QS._qss_e_.Inspection_.DictionaryWrapper2<IGroupID, AggregationGroup>("Aggregation Groups", groups);
		}

		private IRouter aggregationRouter;
		private System.Reflection.ConstructorInfo controllerClassConstructor;
		private QS.Fx.Logging.ILogger logger;
		private QS._core_c_.Base3.InstanceID localAddress;
		private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> senderCollection;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private Base3_.Constructor<Components_1_.ISeqCollection<Controller>> controllerCollectionConstructor;

#if DEBUG_LogOperations
		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private TMS.Inspection.AutoCollection<QS.Fx.Network.NetworkAddress,Base.Logger> operations_log = 
			new QS.TMS.Inspection.AutoCollection<QS.Fx.Network.NetworkAddress,QS.CMS.Base.Logger>();
#endif

		#region Accessors

		public Base3_.Constructor<Components_1_.ISeqCollection<Controller>> ControllerCollectionConstructor
		{
			set { controllerCollectionConstructor = value; }
		}

		public IRouter AggregationRouter
		{
			set { aggregationRouter = value; }
		}

		public System.Type ControllerClass
		{
			set 
			{ 
				if ((controllerClassConstructor = value.GetConstructor(Type.EmptyTypes)) == null)
					throw new Exception("Cannot find a no-argument constructor for " + QS._core_c_.Helpers.ToString.Object(value));
			}
		}

		#endregion

		#region Receive Callback

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{
#if DEBUG_LogOperations
			operations_log[sourceAddress].logMessage(this, Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

			Message wrapped_message = receivedObject as Message;
			if (wrapped_message != null)
			{
				Controller aggregationController = 
					(Controller)((IAgent)this).GetGroup(wrapped_message.channelID.GroupID).GetChannel(
					wrapped_message.channelID.RootAddress).GetController(wrapped_message.seqno);

				if (aggregationController != null)
					aggregationController.receiveCallback(sourceIID, wrapped_message.message);
				else
				{
#if DEBUG_LogOperations
					operations_log[sourceAddress].logMessage(this, "__ReceiveCallback: Controller no longer exists, cannot dispatch " +
						Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif
				}
			}
			else
				throw new Exception("Received wrong message type!");

			return null;
		}

		#endregion

		private Controller createController()
		{
			Controller controller = controllerClassConstructor.Invoke(Helpers_.NoObject.Array) as Controller;
			if (controller == null)
				throw new Exception("Cannot create a controller");
			return controller;
		}

		private System.Collections.Generic.IDictionary<IGroupID, AggregationGroup> groups =
			new System.Collections.Generic.Dictionary<IGroupID, AggregationGroup>();

		[QS.Fx.Base.Inspectable("Aggregation Groups", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper2<IGroupID, AggregationGroup> inspectableAggregationGroupProxy;

		#region Class Message

		[QS.Fx.Serialization.ClassID(ClassID.Aggregation3_Agent_Message)]
		public class Message : QS.Fx.Serialization.ISerializable
		{
			public Message()
			{
			}

			public Message(ChannelID channelID, int seqno, QS.Fx.Serialization.ISerializable message)
			{
				this.channelID = channelID;
				this.seqno = seqno;
				this.message = message;
			}

			public ChannelID channelID;
			public int seqno;
			public QS.Fx.Serialization.ISerializable message;

			#region ISerializable Members

			QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
			{
				get 
				{
					return ((QS.Fx.Serialization.ISerializable)channelID).SerializableInfo.CombineWith(message.SerializableInfo).Extend(
						(ushort) ClassID.Aggregation3_Agent_Message, (ushort) (sizeof(int) + sizeof(ushort)), 0, 0);
				}
			}

			unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
			{
				((QS.Fx.Serialization.ISerializable)channelID).SerializeTo(ref header, ref data);
				fixed (byte* arrayptr = header.Array)
				{
					byte *headerptr = arrayptr + header.Offset;
					*((int*) headerptr) = seqno;
					*((ushort*)(headerptr + sizeof(int))) = message.SerializableInfo.ClassID;
				}
				header.consume(sizeof(int) + sizeof(ushort));
				message.SerializeTo(ref header, ref data);
			}

			unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
			{
				channelID = new ChannelID();
				((QS.Fx.Serialization.ISerializable)channelID).DeserializeFrom(ref header, ref data);
				ushort classID;
				fixed (byte* arrayptr = header.Array)
				{
					byte* headerptr = arrayptr + header.Offset;
					seqno = *((int*)headerptr);
					classID = *((ushort*)(headerptr + sizeof(int)));
				}
				header.consume(sizeof(int) + sizeof(ushort));
				message = QS._core_c_.Base3.Serializer.CreateObject(classID);
				message.DeserializeFrom(ref header, ref data);
			}

			#endregion

			public override string ToString()
			{
				return channelID.ToString() + "," + seqno.ToString() + " " + message.ToString();
			}
		}

		#endregion

		#region Class AggregationGroup

		private class AggregationGroup : QS.Fx.Inspection.Inspectable, IGroup
		{
			public AggregationGroup(Agent owner, IGroupID groupID)
			{
				this.owner = owner;
				this.groupID = groupID;

				inspectableChannelsProxy =
					new QS._qss_e_.Inspection_.DictionaryWrapper2<QS._core_c_.Base3.InstanceID, AggregationChannel>(
						"Channels", channels);
			}

			private Agent owner;
			private IGroupID groupID;
			private System.Collections.Generic.IDictionary<QS._core_c_.Base3.InstanceID, AggregationChannel> channels =
				new System.Collections.Generic.Dictionary<QS._core_c_.Base3.InstanceID, AggregationChannel>();
			private AggregationChannel localChannel;

			[QS.Fx.Base.Inspectable("Channels", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private QS._qss_e_.Inspection_.DictionaryWrapper2<QS._core_c_.Base3.InstanceID, AggregationChannel> inspectableChannelsProxy;

			#region Class AggregationChannel

            private class AggregationChannel : QS.Fx.Inspection.Inspectable, IChannel, QS.Fx.Logging.ILogger
			{
				public AggregationChannel(AggregationGroup owner, QS._core_c_.Base3.InstanceID rootAddress)
				{
					this.owner = owner;
					this.rootAddress = rootAddress;
					controllers = owner.owner.controllerCollectionConstructor();
					controllers.ConstructorCallback = new Components_1_.SeqCollection<Controller>.Constructor(createController);
				}

				private AggregationGroup owner;
				private QS._core_c_.Base3.InstanceID rootAddress;
				[QS.Fx.Base.Inspectable("Controllers", QS.Fx.Base.AttributeAccess.ReadOnly)]
				private Components_1_.ISeqCollection<Controller> controllers;
				
				#region Managing Controllers

				private void send(Controller sender, QS._core_c_.Base3.InstanceID target, QS.Fx.Serialization.ISerializable message)
				{
					owner.owner.senderCollection[target.Address].send(
						(uint)ReservedObjectID.Aggregation3_AggregationAgent,
						new Message(((IChannel)this).ChannelID, ((IController)sender).SeqNo, message));
				}

				private void remove(Controller controller)
				{
					lock (controllers)
					{
						controllers.remove(((IController)controller).SeqNo);
					}
				}

				private Controller createController(int seqno)
				{
					Routing_2_.IStructure<QS._core_c_.Base3.InstanceID> routingStructure;
					owner.owner.aggregationRouter.resolve(((IChannel)this).ChannelID, seqno, out routingStructure);

					IList<QS._core_c_.Base3.InstanceID> incomingAddresses = routingStructure.Incoming(owner.owner.localAddress, rootAddress);
					IList<QS._core_c_.Base3.InstanceID> outgoingAddresses = routingStructure.Outgoing(owner.owner.localAddress, rootAddress);
					if (outgoingAddresses.Count > 1)
						throw new ArgumentException("Cannot have multiple outgoing paths.");
					QS._core_c_.Base3.InstanceID outgoingAddress = (outgoingAddresses.Count > 0) ? outgoingAddresses[0] : null;

					Controller controller = owner.owner.createController();
					controller.preinitialize(this, seqno, new Controller.SendCallback(send), 
						new Controller.RemoveCallback(remove), incomingAddresses, outgoingAddress, this);

					return controller;
				}

				#endregion

				#region IChannel Members

				IController IChannel.GetController(int seqno)
				{
					lock (controllers)
					{
						IController controller = controllers.lookup(seqno);

#if DEBUG_AggregationAgent
//					((QS.Fx.Logging.ILogger )this).logMessage(this, "__GetController(" + seqno.ToString() + ") returned " + 
//						Helpers.ToString.ObjectRef(controller));
#endif

						return controller;
					}
				}

				ChannelID IChannel.ChannelID
				{
					get { return new ChannelID(owner.groupID, rootAddress); }
				}

				#endregion

				#region ILogger Members

				void QS.Fx.Logging.ILogger.Clear()
				{
					throw new NotImplementedException();
				}

                void QS.Fx.Logging.ILogger.Log(object source, string message)
				{
					owner.owner.logger.Log(source, owner.groupID.ToString() + "," +
						rootAddress.ToString() + " " + message);
				}

				#endregion

				#region IConsole Members

                void QS.Fx.Logging.IConsole.Log(string s)
				{
					((QS.Fx.Logging.ILogger)this).Log(null, s);
				}

				#endregion
			}

			#endregion

			#region Channel lookup

			// TODO: Should take into account that hosts may be dead...
			AggregationChannel lookupChannel(QS._core_c_.Base3.InstanceID rootAddress)
			{
				AggregationChannel channel;
				lock (channels)
				{
					if (channels.ContainsKey(rootAddress))
						channel = channels[rootAddress];
					else
						channels[rootAddress] = channel = new AggregationChannel(this, rootAddress);
				}
				return channel;
			}

			#endregion

			#region IAggregationGroup Members

			IChannel IGroup.MyChannel
			{
				get
				{
					lock (this)
					{
						if (localChannel == null)
							localChannel = lookupChannel(owner.localAddress);
					}
					return localChannel;
				}
			}

			IGroupID IGroup.GroupID
			{
				get { return groupID; }
			}

			IChannel IGroup.GetChannel(QS._core_c_.Base3.InstanceID rootAddress)
			{
				return lookupChannel(rootAddress);
			}

			#endregion
		}

		#endregion

		#region IAgent Members

		// TODO: Should take into account the fact that groups (views) may not be active anymore...
		public IGroup GetGroup(IGroupID aggregationGroupID)
		{
			AggregationGroup group;
			lock (groups)
			{
				if (groups.ContainsKey(aggregationGroupID))
					group = groups[aggregationGroupID];
				else
					groups[aggregationGroupID] = group = new AggregationGroup(this, aggregationGroupID);
			}
			return group;
		}

		#endregion

		#region IAggregationAgent Members

		QS._qss_c_.Aggregation1_.IAggregationController QS._qss_c_.Aggregation1_.IAggregationAgent.aggregate(QS._qss_c_.Aggregation1_.IAggregationKey aggregationKey, QS._qss_c_.Aggregation1_.AggregationCallback aggregationCallback)
		{
			Multicasting3.MessageID messageID = aggregationKey as Multicasting3.MessageID;
			if (messageID == null)
				throw new ArgumentException();

			IController aggregationController = ((IAgent)this).GetGroup(new QS._qss_c_.Base3_.ViewID(messageID.GroupID,
				messageID.ViewSeqNo)).MyChannel.GetController((int) messageID.MessageSeqNo);

			aggregationController.BeginAggregate(compatibilityCompletionAsyncCallback, aggregationCallback);

			return (Aggregation1_.IAggregationController) aggregationController;
		}

		private static AsyncCallback compatibilityCompletionAsyncCallback = 
			new AsyncCallback(compatibilityCompletionCallback);
		private static void compatibilityCompletionCallback(IAsyncResult asyncResult)
		{
			((Aggregation1_.AggregationCallback) asyncResult.AsyncState)();
		}

		void QS._qss_c_.Aggregation1_.IAggregationAgent.submit(QS._qss_c_.Aggregation1_.IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress, QS.Fx.Serialization.ISerializable data, IAggregatable toAggregate)
		{
			Multicasting3.MessageID messageID = aggregationKey as Multicasting3.MessageID;
			if (messageID == null)
				throw new ArgumentException();

			IController aggregationController = ((IAgent)this).GetGroup(new QS._qss_c_.Base3_.ViewID(messageID.GroupID, 
				messageID.ViewSeqNo)).GetChannel(rootAddress).GetController((int) messageID.MessageSeqNo);

			aggregationController.Submit(data, toAggregate);
		}

		void QS._qss_c_.Aggregation1_.IAggregationAgent.submit(QS._qss_c_.Aggregation1_.IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress)
		{
			((QS._qss_c_.Aggregation1_.IAggregationAgent)this).submit(aggregationKey, rootAddress, null, null);
		}

		void QS._qss_c_.Aggregation1_.IAggregationAgent.registerClass(QS._qss_c_.Aggregation1_.IAggregationClass aggregationClass)
		{
			throw new NotSupportedException("This aggregation controller can only handle one predefined aggregation class.");
		}

		#endregion
	}
}
