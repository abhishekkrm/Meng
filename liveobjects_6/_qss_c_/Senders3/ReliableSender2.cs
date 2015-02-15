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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Senders3
{
	public class ReliableSender2 : Base3_.SenderClass<Base3_.IReliableSerializableSender>,
		Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>
	{
		public ReliableSender2(
			QS._core_c_.Base3.InstanceID instanceID, QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> messageSenderCollection,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> commandSenderCollection,
			QS.Fx.Clock.IAlarmClock alarmClock)
		{
			this.instanceID = instanceID;
			this.logger = logger;
			this.demultiplexer = demultiplexer;
			this.alarmClock = alarmClock;
			this.messageSenderCollection = messageSenderCollection;
			this.commandSenderCollection = commandSenderCollection;

			demultiplexer.register((uint)ReservedObjectID.Senders3_ReliableSender2_MessageChannel,
				new QS._qss_c_.Base3_.ReceiveCallback(this.messageCallback));
			demultiplexer.register((uint)ReservedObjectID.Senders3_ReliableSender2_CommandChannel,
				new QS._qss_c_.Base3_.ReceiveCallback(this.commandCallback));
		}

		private QS._core_c_.Base3.InstanceID instanceID;
		private QS.Fx.Logging.ILogger logger;
		private Base3_.IDemultiplexer demultiplexer;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> messageSenderCollection, commandSenderCollection;

		#region Callbacks

		private QS.Fx.Serialization.ISerializable messageCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{
			/*

			#if DEBUG_LogOperations
						operations_log[sourceAddress].logMessage(this, Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
			#endif

						Sender.IncomingRequest request = receivedObject as Sender.IncomingRequest;
						if (request != null)
						{
							underlyingSenderCollection[sourceAddress].send(
								(uint)ReservedObjectID.Senders3_ReliableSender_AcknowledgementChannel, new Sender.Ack(request.SeqNo));

							// no duplicate handling
							demultiplexer.dispatch(request.Object.destinationLOID, sourceAddress, request.Object.transmittedObject);
						}
			*/

			return null;
		}

		private QS.Fx.Serialization.ISerializable commandCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
		{
			Sender.Command command = receivedObject as Sender.Command;
			if (command != null)
			{
/*
				((Sender)this.SenderCollection[sourceAddress]).ReceivedCommand(command);
*/ 
			}

			return null;
		}

		#endregion

		#region Class Sender

		[QS.Fx.Base.Inspectable]
		public class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender
		{
			public Sender(ReliableSender2 owner, QS.Fx.Network.NetworkAddress destinationAddress)
			{
				this.owner = owner;
				this.destinationAddress = destinationAddress;
				this.underlyingMessageSender = owner.messageSenderCollection[destinationAddress];
				this.underlyingCommandSender = owner.commandSenderCollection[destinationAddress];

/*
				outgoingWindow = new FlowControl.OutgoingWindow(defaultWindowSize);
*/
                outgoingWindow = null;

				alarmCallback = new QS.Fx.Clock.AlarmCallback(this.retransmissionCallback);
			}

			private const double default_initialRetransmissionTimeout = 0.02;
			private const double default_initialConnectingTimeout = 0.05;
			private const double default_maximumConnectingTimeout = 600;

			private ReliableSender2 owner;
			private QS.Fx.Network.NetworkAddress destinationAddress;
			private QS._qss_c_.Base3_.ISerializableSender underlyingMessageSender, underlyingCommandSender;
			private bool connected = false, connecting = false;
            private uint nextSeqNo, connectSeqNo; // , destinationWindowSize ..........................is it used???
			private FlowControl_1_.IOutgoingWindow outgoingWindow;
			private QS.Fx.Clock.AlarmCallback alarmCallback;
			private Queue<Request> outgoingQueue = new Queue<Request>();
			private double retransmissionTimeout = default_initialRetransmissionTimeout;
			private QS.Fx.Clock.IAlarm connectingAlarm;
			private double initialConnectingTimeout = default_initialConnectingTimeout;
			private double maximumConnectingTimeout = default_maximumConnectingTimeout;

			#region Internal Processing

			private void BeginConnect()
			{
				connecting = true;
				underlyingCommandSender.send((uint)ReservedObjectID.Senders3_ReliableSender2_CommandChannel, 
					new Command(Command.Type.CONNECT, connectSeqNo = 1));
				connectingAlarm = owner.alarmClock.Schedule(
					initialConnectingTimeout, new QS.Fx.Clock.AlarmCallback(connectingCallback), null);
			}

/*
//			private void Unregister(Request request)
//			{
//				request.alarmRef.Cancel();
//			}
*/

			#endregion

			#region Callbacks

			private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
/*
				Request request = (Request)alarmRef.Context;
				bool should_send;
				lock (request)
				{
					if (should_send = !request.IsCompleted && !request.Cancelled)
					{
#if DEBUG_ReliableSender
						owner.logger.Log(this, "__RetransmissionCallback : to " + this.Address.ToString() + ", " + request.message.ToString());
#endif
						alarmRef.Reschedule();
					}
				}

				if (should_send)
					underlyingSender.send((uint)ReservedObjectID.Senders3_ReliableSender_MessageChannel, request.message);
*/
			}

			private void connectingCallback(QS.Fx.Clock.IAlarm alarmRef)
			{
				lock (this)
				{
					underlyingCommandSender.send((uint)ReservedObjectID.Senders3_ReliableSender2_CommandChannel,
						new Command(Command.Type.CONNECT, ++connectSeqNo));
					double timeout = 2 * alarmRef.Timeout;
					if (timeout > maximumConnectingTimeout)
						timeout = maximumConnectingTimeout;
					alarmRef.Reschedule(timeout);
				}
			}

			#endregion

			#region Class Request

			[QS.Fx.Base.Inspectable]
			private class Request : Base3_.AsynchronousOperation.Inspectable
			{
				public Request(Sender owner, uint seqno, QS._core_c_.Base3.Message message,
					Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
					: base(completionCallback, asynchronousState)
				{
					this.owner = owner;
					this.message = new OutgoingRequest(seqno, message);
				}

				public OutgoingRequest message;
				private Sender owner;
				public QS.Fx.Clock.IAlarm alarmRef = null;

				public override void Unregister()
				{
/*
					owner.Unregister(this);
*/ 
				}
			}

			#endregion

			#region Class Command

			[QS.Fx.Serialization.ClassID(ClassID.Senders3_ReliableSender2_Command)]
			public class Command : QS.Fx.Serialization.ISerializable
			{
				public enum Type : byte
				{
					CONNECT, ACK, CONNECT_ACK
				}

				public Command()
				{
				}

				public Command(Type type) : this(type, 0)
				{
				}

				public Command(Type type, uint seqno)
				{
					this.TypeOf = type;
					this.SeqNo = seqno;
				}

				public Type TypeOf;
				public uint SeqNo;

				#region ISerializable Members

				QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
				{
					get 
					{ 
						return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Senders3_ReliableSender2_Command, 
							(ushort)(sizeof(byte) + sizeof(uint)), sizeof(byte) + sizeof(uint), 0);
					}
				}

				unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
				{
					fixed (byte* arrayptr = header.Array)
					{
						byte* headerptr = arrayptr + header.Offset;
						*((byte*)headerptr) = (byte)TypeOf;
						*((uint*)(headerptr + sizeof(byte))) = SeqNo;
					}
					header.consume(sizeof(byte) + sizeof(uint));
				}

				unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
				{
					fixed (byte* arrayptr = header.Array)
					{
						byte* headerptr = arrayptr + header.Offset;
						TypeOf = (Type) (*((byte*)headerptr));
						SeqNo = *((uint*)(headerptr + sizeof(byte)));
					}
					header.consume(sizeof(byte) + sizeof(uint));
				}

				#endregion
			}

			#endregion

			#region IReliableSerializableSender Members

			public Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data,
				Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
			{
				Request request = null;
				bool sending_immediately = false;
				lock (this)
				{
					if (connected)
					{
						request = new Request(this, nextSeqNo++, new QS._core_c_.Base3.Message(destinationLOID, data), 
							completionCallback, asynchronousState);

						if (sending_immediately = outgoingWindow.hasSpace())
						{
							if (outgoingWindow.append(request) != request.message.SeqNo)
								throw new Exception("Internal error: Assigned and available sequence numbers do not match.");
						}
						else
						{
							outgoingQueue.Enqueue(request);
						}
					}
					else
					{
						outgoingQueue.Enqueue(request);
						if (!connecting)
							BeginConnect();
					}
				}

				if (sending_immediately)
				{
					request.alarmRef = owner.alarmClock.Schedule(retransmissionTimeout, alarmCallback, request);
					underlyingMessageSender.send((uint)ReservedObjectID.Senders3_ReliableSender2_MessageChannel, request.message);
				}

				return request;
			}

			public void EndSend(Base3_.IAsynchronousOperation asynchronousOperation)
			{
			}

			public QS.Fx.Network.NetworkAddress Address
			{
				get { return destinationAddress; }
			}

			public int MTU
			{
				get 
				{ 
					// return 2000; 
					throw new NotImplementedException();
				} 
			}

			#endregion

/*
			public void acknowledged(uint seqno)
			{
				lock (this)
				{
					try
					{
						Request request = (Request)outgoingWindow.remove(seqno);
						request.IsCompleted = true;
					}
					catch (Exception)
					{
					}

					while (outgoingWindow.hasSpace() && outgoingQueue.Count > 0)
					{
						Request request = outgoingQueue.Dequeue();
						if (outgoingWindow.append(request) != request.message.SeqNo)
							throw new Exception("Assigned and available sequence numbers do not match.");

#if DEBUG_ReliableSender
                        owner.logger.Log(this, "__BeginSend: sending enqueued for " + this.Address.ToString() + ", " + request.message.ToString());
#endif

						underlyingSender.send((uint)ReservedObjectID.Senders3_ReliableSender_MessageChannel, request.message);
						request.alarmRef = owner.alarmClock.Schedule(owner.retransmissionTimeout, alarmCallback, request);
					}
				}
			}
*/

			#region Class OutgoingRequest

			public class OutgoingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
			{
				public OutgoingRequest(uint seqno, QS._core_c_.Base3.Message message)
					: base(seqno, message)
				{
				}

				public override ClassID ClassID
				{
					get { return QS.ClassID.Senders3_ReliableSender2_Message; }
				}
			}

			#endregion

			#region Class IncomingRequest

			[QS.Fx.Serialization.ClassID(QS.ClassID.Senders3_ReliableSender2_Message)]
			public class IncomingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
			{
				public IncomingRequest()
				{
				}
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
				return destinationAddress.CompareTo((obj is Sender) ? (obj as Sender).destinationAddress : obj);
			}

			#endregion
		}

		#endregion

		#region CreateSender

		protected override Base3_.IReliableSerializableSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return new Sender(this, destinationAddress);
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
	}
}
