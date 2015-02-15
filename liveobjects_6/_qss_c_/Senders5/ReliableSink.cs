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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders5
{
	public class ReliableSink : ISimpleSink, ISource
	{
		private const int default_receiverWindowSize = 100;

		#region Class Receiver

		public class Receiver
		{
			public Receiver(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
				Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink> acknowledgementSinkCollection)
			{
				this.demultiplexer = demultiplexer;
				this.logger = logger;
				this.acknowledgementSinkCollection = acknowledgementSinkCollection;
				demultiplexer.register((uint)ReservedObjectID.Senders5_ReliableSink_MessageChannel,
					new QS._qss_c_.Base3_.ReceiveCallback(MessageCallback));
			}

			private System.Collections.Generic.IDictionary<QS.Fx.Network.NetworkAddress, State> states =
				new System.Collections.Generic.Dictionary<QS.Fx.Network.NetworkAddress, State>();
			private QS.Fx.Logging.ILogger logger;
			private Base3_.IDemultiplexer demultiplexer;
			private Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink> acknowledgementSinkCollection;

			#region Class State

			private class State
			{
				public State(Receiver owner, QS.Fx.Network.NetworkAddress senderAddress)
				{
					this.owner = owner;
					this.senderAddress = senderAddress;
					outgoingChannel = owner.acknowledgementSinkCollection[senderAddress].Register(outgoingQueue);

					incomingWindow = new FlowControl_1_.IncomingWindow(default_receiverWindowSize);
				}

				private Receiver owner;
				private QS.Fx.Network.NetworkAddress senderAddress;
				private RequestQueue outgoingQueue = new RequestQueue();
				private IChannel outgoingChannel;
				private FlowControl_1_.IIncomingWindow incomingWindow;

				public void MessageCallback(QS._core_c_.Base3.InstanceID sourceIID, Request.Message message)
				{
					outgoingQueue.Enqueue(new Command(Command.Type.ACK, message.SeqNo));
					outgoingChannel.Signal();

					if (incomingWindow.accepts(message.SeqNo) && incomingWindow.lookup(message.SeqNo) == null)
					{						
						incomingWindow.insert(message.SeqNo, message.TheMessage);
						while (incomingWindow.ready())
						{
							QS._core_c_.Base3.Message toConsume = (QS._core_c_.Base3.Message) incomingWindow.consume();
							try
							{
								owner.demultiplexer.dispatch(toConsume.destinationLOID, sourceIID, toConsume.transmittedObject);
							}
							catch (Exception exc)
							{
								owner.logger.Log(this, "__MessageCallback: " + exc.ToString());
							}
						}
					}
				}
			}

			#endregion

			#region Managing States

			private State StateOf(QS.Fx.Network.NetworkAddress address)
			{
				bool newlyCreated;
				return StateOf(address, out newlyCreated);
			}

			private State StateOf(QS.Fx.Network.NetworkAddress address, out bool newlyCreated)
			{
				if (newlyCreated = !states.ContainsKey(address))
				{
					State state = new State(this, address);
					states[address] = state;
					return state;
				}
				else
					return states[address];
			}

			#endregion

			#region Callbacks

			private QS.Fx.Serialization.ISerializable MessageCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable dataObject)
			{
				Request.Message message = (Request.Message) dataObject;
				State state;
				lock (this)
				{
					state = StateOf(sourceIID.Address);
					System.Threading.Monitor.Enter(state);
				}

				try
				{
					state.MessageCallback(sourceIID, (Request.Message) dataObject);
				}
				finally
				{
					System.Threading.Monitor.Exit(state);
				}

				return null;
			}

			#endregion
		}

		#endregion

		#region Class Collection

		public class Collection : Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink>
		{
			public Collection(Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink> underlyingSinkCollection,
				Base3_.IDemultiplexer demultiplexer)
			{
				demultiplexer.register((uint)ReservedObjectID.Senders5_ReliableSink_AcknowledgementChannel,
					new QS._qss_c_.Base3_.ReceiveCallback(AcknowledgementCallback));

				this.Callback = new Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink>.ConstructorCallback(
				delegate(QS.Fx.Network.NetworkAddress address)
				{
					return new CollectingSink(new ReliableSink(
						((Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink>)underlyingSinkCollection)[address]));
				});
			}

			#region Acknowledgement Callback

			private QS.Fx.Serialization.ISerializable AcknowledgementCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable dataObject)
			{
				Command command = dataObject as Command;
				if (command != null)
				{
					((ReliableSink)((CollectingSink)collection[sourceIID.Address]).SimpleSink).Acknowledged(sourceIID, command);
				}
				return null;
			}

			#endregion
		}

		#endregion

		public ReliableSink(ICollectingSink collectingSink)
		{
			this.senderSink = collectingSink.Register(this);
			this.source = null;
		}

		public ReliableSink(IGenericSink senderSink) : this(senderSink, null)
		{
		}

		public ReliableSink(IGenericSink senderSink, ISource source)
		{
			this.senderSink = senderSink;
			this.source = source;
		}

		private IGenericSink senderSink;
		private FlowControl_1_.IOutgoingWindow outgoingWindow = 
			new FlowControl_1_.OutgoingWindow(default_receiverWindowSize);
		private ISource source;
		// private uint nextSeqNo = 1;

		public void Acknowledged(QS._core_c_.Base3.InstanceID sourceIID, Command command)
		{
			switch (command.TypeOf)
			{
				case Command.Type.ACK:
				{
					Request request = (Request) outgoingWindow.remove(command.SeqNo);
					if (request != null)
					{
						request.AsynchronousRequest.Completed();
					}

					if (((ISource)this).Ready)
						senderSink.Signal();
				}
				break;
			}
		}

		#region ISource Members

		bool ISource.Ready
		{
			get { return source.Ready && outgoingWindow.hasSpace(); }
		}

		QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> ISource.Get(uint maximumSize)
		{
			lock (this)
			{
				Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> message;
				if (outgoingWindow.hasSpace() && (message = source.Get(maximumSize - Request.HeaderOverhead)) != null)
				{
					Request request = new Request(message);
					request.SeqNo = outgoingWindow.append(request);
					return request;
				}
				else
					return null;
			}
		}

		#endregion

		#region Class Command

		[QS.Fx.Serialization.ClassID(ClassID.Senders5_ReliableSink_Command)]
		public class Command : QS.Fx.Serialization.ISerializable, Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>
		{
			public Command(Type typeOf, uint seqno)
			{
				this.TypeOf = typeOf;
				this.SeqNo = seqno;
			}

			public Command()
			{
			}

			public enum Type : ushort
			{
				ACK
			}

			public Type TypeOf;
			public uint SeqNo;

			#region ISerializable Members

			QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
			{
				get 
				{ 
					return new QS.Fx.Serialization.SerializableInfo((ushort) ClassID.Senders5_ReliableSink_Command, 
					sizeof(ushort) + sizeof(uint), sizeof(ushort) + sizeof(uint), 0); 
				}
			}

			unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
			{
				fixed (byte* arrayptr = header.Array)
				{
					byte* headerptr = arrayptr + header.Offset;
					*((ushort*)headerptr) = (ushort) TypeOf;
					 *((uint*) (headerptr + sizeof(ushort))) = SeqNo;
				}
				header.consume(sizeof(ushort) + sizeof(uint));
			}

			unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
			{
				fixed (byte* arrayptr = header.Array)
				{
					byte* headerptr = arrayptr + header.Offset;
					TypeOf = (Type) (*((ushort*)headerptr));
					SeqNo = *((uint*) (headerptr + sizeof(ushort)));
				}
				header.consume(sizeof(ushort) + sizeof(uint));
			}

			#endregion

			#region IAsynchronousRequest<Message> Members

			QS._core_c_.Base3.Message QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Argument
			{
				get 
				{
					uint channel_loid;
					switch (TypeOf)
					{
						case Type.ACK:
							channel_loid = (uint)ReservedObjectID.Senders5_ReliableSink_AcknowledgementChannel;
							break;

						default:
							channel_loid = (uint)ReservedObjectID.Senders5_ReliableSink_CommandChannel;
							break;
					}

					return new QS._core_c_.Base3.Message(channel_loid, this); 
				}
			}

			void QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Completed()
			{
			}

			void QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Failed(Exception exception)
			{
			}

			#endregion
		}

		#endregion

		#region Class Request

		private class Request : QS.Fx.Serialization.ISerializable, Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>
		{
			public Request(Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> message)
			{
				this.AsynchronousRequest = message;
			}

			public Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> AsynchronousRequest;
			public uint SeqNo = 0;

			private const int header_overhead = 2 * sizeof(uint) + sizeof(ushort);
			public static uint HeaderOverhead
			{
				get { return header_overhead; }
			}

			#region ISerializable Members

			QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
			{
				get 
				{
					return AsynchronousRequest.Argument.transmittedObject.SerializableInfo.Extend(
						(ushort)ClassID.Senders5_ReliableSink_Message, header_overhead, 0, 0);
				}
			}

			unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
			{
				fixed (byte* arrayptr = header.Array)
				{
					byte *headerptr = arrayptr + header.Offset;
					*((uint*) headerptr) = SeqNo;
					*((uint*)(headerptr + sizeof(uint))) = AsynchronousRequest.Argument.destinationLOID;
					*((ushort*)(headerptr + 2 * sizeof(uint))) = 
						AsynchronousRequest.Argument.transmittedObject.SerializableInfo.ClassID;
				}
				header.consume(header_overhead);
				AsynchronousRequest.Argument.transmittedObject.SerializeTo(ref header, ref data);
			}

			void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region Class Message

			[QS.Fx.Serialization.ClassID(ClassID.Senders5_ReliableSink_Message)]
			public class Message : QS.Fx.Serialization.ISerializable
			{
				public Message()
				{
				}

				public QS._core_c_.Base3.Message TheMessage;
				public uint SeqNo;

				#region ISerializable Members

				QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
				{
					get { throw new NotSupportedException(); }
				}

				void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
				{
					throw new NotSupportedException();
				}

				unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
				{
					ushort classID;
					fixed (byte* arrayptr = header.Array)
					{
						byte* headerptr = arrayptr + header.Offset;
						SeqNo = *((uint*)headerptr);
						TheMessage.destinationLOID = *((uint*)(headerptr + sizeof(uint)));
						classID = *((ushort*)(headerptr + 2 * sizeof(uint)));
					}
					header.consume(header_overhead);
					(TheMessage.transmittedObject = QS._core_c_.Base3.Serializer.CreateObject(classID)).DeserializeFrom(ref header, ref data);
				}

				#endregion
			}

			#endregion

			#region IAsynchronousRequest<Message> Members

			QS._core_c_.Base3.Message QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Argument
			{
				get { return new QS._core_c_.Base3.Message((uint) ReservedObjectID.Senders5_ReliableSink_MessageChannel, this); }
			}

			void QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Completed()
			{
				throw new NotSupportedException();
			}

			void QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Failed(Exception exception)
			{
				throw new NotSupportedException();
			}

			#endregion
		}

		#endregion

		#region ISimpleSink Members

		void IGenericSink.Signal()
		{
			senderSink.Signal();
		}

		ISource ISimpleSink.Source
		{
			get { return source; }
			set { source = value; }
		}

		#endregion

//		#region IDisposable Members
//
//		void IDisposable.Dispose()
//		{
//		}
//
//		#endregion
	}
}
