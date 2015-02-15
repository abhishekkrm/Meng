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

using System;
using System.Threading;
using System.IO;
using System.Net;

namespace QS._qss_c_.Senders_1_
{
/*
	/// <summary>
	/// This is a simple reliable sender with retransmissions, exponential back-off, duplicate detection, ordering, simple flow
	/// control using a form of a simple sliding window protocol.
	/// </summary>

	public class ReliableSender : Base.ISender, Base.IClient
	{
        public ReliableSender(Base.ISender underlyingSender, Base.IDemultiplexer demultiplexer, QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, 
			uint anticipatedNumberOfCommunicaingPeers, uint windowSize, TimeSpan timeoutForResending, QS.Fx.Logging.ILogger logger, 
			bool exponentialBackOff) : this(underlyingSender, demultiplexer, alarmClock, anticipatedNumberOfCommunicaingPeers, windowSize, timeoutForResending, logger,
			exponentialBackOff, (uint) ReservedObjectID.ReliableSender)
		{
		}

        public ReliableSender(Base.ISender underlyingSender, Base.IDemultiplexer demultiplexer, QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, 
			uint anticipatedNumberOfCommunicaingPeers, uint windowSize, TimeSpan timeoutForResending, QS.Fx.Logging.ILogger logger, 
			bool exponentialBackOff, uint objectID)
		{
			this.objectID = objectID;

			Base.Serializer.Get.register(ClassID.ReliableSender_OurMessage, new Base.CreateSerializable(OurMessage.createSerializable));
			Base.Serializer.Get.register(ClassID.ReliableSender_AckMessage, new Base.CreateSerializable(AckMessage.createSerializable));

			this.demultiplexer = demultiplexer;
			this.underlyingSender = underlyingSender;
			this.alarmClock = alarmClock;
			this.timeoutForResending = timeoutForResending;

			demultiplexer.register(this, new Dispatchers.DirectDispatcher(new Base.OnReceive(this.receiveCallback)));

			this.windowSize = windowSize;

			outgoingWindows = new Collections.Hashtable(anticipatedNumberOfCommunicaingPeers);
			incomingWindows = new Collections.Hashtable(anticipatedNumberOfCommunicaingPeers);

			waitingOutgoing = new Collections.RawQueue();

			this.logger = logger;
			this.exponentialBackOff = exponentialBackOff;
		}

		private uint objectID;

		private Base.ISender underlyingSender;
		private Base.IDemultiplexer demultiplexer;
        private QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock;
		private TimeSpan timeoutForResending;
		private QS.Fx.Logging.ILogger logger;
		private bool exponentialBackOff;

		private uint windowSize;
		private Collections.Hashtable outgoingWindows, incomingWindows;
		private Collections.IRawQueue waitingOutgoing;

		// -----------------------------------------------------------------------------------------------------------------------------------

		#region ISender Members

		public Base.IMessageReference send(Base.IClient theSender, Base.IAddress destinationAddress, Base.IMessage message, 
			Base.SendCallback sendCallback)
		{
			if (destinationAddress is Base.ObjectAddress)
			{
				QS.Fx.Network.NetworkAddress networkAddress = (QS.Fx.Network.NetworkAddress) destinationAddress;

				// logger.Log(this, "looking for or creating an outgoing window");

				FlowControl.IOutgoingWindow outgoingWindow = null;
				lock (outgoingWindows)
				{
					Collections.IDictionaryEntry dic_en = outgoingWindows.lookupOrCreate(networkAddress);
					if (dic_en.Value == null)
					{
						outgoingWindow = new FlowControl.OutgoingWindow(windowSize);
						dic_en.Value = outgoingWindow;					
					}
					else
						outgoingWindow = (FlowControl.IOutgoingWindow) dic_en.Value;

					// logger.Log(this, "getting hold of the window we found or created");

					Monitor.Enter(outgoingWindow);
				}

				// logger.Log(this, "we have a window");
#if DEBUG_ReliableSender
				logger.Log(null, "ENTER: " + outgoingWindow.ToString());
#endif

				OutgoingMessageRef messageRef;

				bool sendingNow = outgoingWindow.hasSpace();
				if (!sendingNow)
				{
					// logger.Log(this, "cleaning up a little before sending");

					messageRef = (OutgoingMessageRef) outgoingWindow.oldest();
					if (messageRef.should_cancel)
					{
						outgoingWindow.removeOldest();
						sendingNow = true;
					}
				}

				// logger.Log(this, "creating outgoing message reference");

				messageRef = new OutgoingMessageRef(theSender, (Base.ObjectAddress) destinationAddress, message, sendCallback);

				if (sendingNow)
				{
					Monitor.Enter(messageRef);
					messageRef.message.localSeqNo = outgoingWindow.append(messageRef);
				}
				else
				{
					waitingOutgoing.enqueue(messageRef);
				}

#if DEBUG_ReliableSender
				logger.Log(null, "LEAVE: " + outgoingWindow.ToString());
#endif

				Monitor.Exit(outgoingWindow);

				if (sendingNow)
				{
#if DEBUG_ReliableSender
					logger.Log(this, "sending message, slot " + messageRef.message.localSeqNo.ToString());
#endif

					underlyingSender.send(this, new Base.ObjectAddress(networkAddress, this.LocalObjectID), messageRef.message, null);

					messageRef.resendingAlarm = alarmClock.Schedule(
						timeoutForResending.TotalSeconds, new QS.Fx.QS.Fx.Clock.AlarmCallback(this.alarmCallback), messageRef);

					Monitor.Exit(messageRef);
				}
				else
				{
#if DEBUG_ReliableSender
					logger.Log(this, "enqueued message");
#endif
				}

				return messageRef;
			}
			else	
				throw new Exception("this address type is not supported");
		}

		#endregion

		private class OutgoingMessageRef : Collections.GenericLinkable, Base.IMessageReference
		{
			public OutgoingMessageRef(Base.IClient theSender, Base.ObjectAddress destinationAddress, Base.IMessage message, 
				Base.SendCallback sendCallback)
			{
				this.theSender = theSender;
				this.destinationAddress = destinationAddress;
				this.message = new OurMessage(theSender.LocalObjectID, destinationAddress.LocalObjectID, 0, message);
				this.acknowledgeCallback = sendCallback;

				should_cancel = should_ignore = false;
			}

			public Base.IClient theSender;
			public Base.ObjectAddress destinationAddress;
			public OurMessage message;
			public Base.SendCallback acknowledgeCallback;
            public QS.Fx.QS.Fx.Clock.IAlarm resendingAlarm;

			public bool should_cancel, should_ignore;

			#region IMessageReference Members

			public QS.CMS.Base.IClient Sender
			{
				get
				{
					return theSender;
				}
			}

			public QS.CMS.Base.IAddress Address
			{
				get
				{
					return destinationAddress;
				}
			}

			public QS.CMS.Base.IMessage Message
			{
				get
				{
					return message;
				}
			}

			public void cancel()
			{
				lock (this)
				{
					message = null;
					should_cancel = true;
				}
			}

			public void ignore()
			{
				lock (this)
				{
					should_ignore = true;
				}
			}

			#endregion
		}

        private void alarmCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
		{
			OutgoingMessageRef messageRef = (OutgoingMessageRef) alarmRef.Context;

#if DEBUG_ReliableSender
			logger.Log(this, "alarm fired for message " + messageRef.message.localSeqNo.ToString());
#endif

			lock (messageRef)
			{
				if (!messageRef.should_cancel)
				{
#if DEBUG_ReliableSender
					logger.Log(this, "resending message " + messageRef.message.localSeqNo.ToString());
#endif

					underlyingSender.send(this, new Base.ObjectAddress(
						(QS.Fx.Network.NetworkAddress) messageRef.destinationAddress, LocalObjectID), messageRef.message, null);

					double new_interval = exponentialBackOff ? (2 * alarmRef.Timeout) : alarmRef.Timeout;
					alarmRef.Reschedule(new_interval);
				}
				else
				{
					// logger.Log(this, "this alarm was cancelled, though");
				}
			}
		}

		private void receiveCallback(Base.IAddress source, Base.IMessage message)
		{
			if (!(source is Base.ObjectAddress))
				throw new Exception("unrecognized address type");

			if (message is OurMessage)
			{
				processOurMessage(((Base.ObjectAddress) source), ((OurMessage) message));
			}
			else if (message is AckMessage)
			{
				processAckMessage(((Base.ObjectAddress) source), ((AckMessage) message));
			}
			else 
				throw new Exception("unrecognized message type");
		}

		private void processAckMessage(Base.ObjectAddress sourceAddress, AckMessage message)
		{
#if DEBUG_ReliableSender
			logger.Log(this, "received ACK from " + sourceAddress.ToString() + ", message " + message.localSeqNo);
#endif

			QS.Fx.Network.NetworkAddress networkAddress = (QS.Fx.Network.NetworkAddress) sourceAddress;

			FlowControl.IOutgoingWindow outgoingWindow = null;
			lock (outgoingWindows)
			{
				Collections.IDictionaryEntry dic_en = outgoingWindows.lookup(networkAddress);
				if (dic_en.Value == null)
					return;
				outgoingWindow = (FlowControl.IOutgoingWindow) dic_en.Value;

				Monitor.Enter(outgoingWindow);
			}

#if DEBUG_ReliableSender
			logger.Log(null, "ENTER: " + outgoingWindow.ToString());
#endif

			// logger.Log(this, "we found a window for this ACK");

			OutgoingMessageRef messageRef = (OutgoingMessageRef) outgoingWindow.remove(message.localSeqNo);

			// logger.Log(this, "we removed the message from the outgoing window");

			if (messageRef == null)
			{
#if DEBUG_ReliableSender
				logger.Log(this, "there is nothing to ACK anymore for message " + message.localSeqNo.ToString());
#endif

				Monitor.Exit(outgoingWindow);
				return;
			}

			// logger.Log(this, "we found a slot for this ACK");

			Monitor.Enter(messageRef);

			// logger.Log(this, "cancelling alarm for " + sourceAddress.ToString() + ", message " + message.localSeqNo);

			messageRef.resendingAlarm.Cancel();
			messageRef.resendingAlarm = null;

			Collections.IRawQueue sendingQueue = null;

			if (outgoingWindow.hasSpace())
			{
				sendingQueue = new Collections.RawQueue();

				while (outgoingWindow.hasSpace())
				{
					OutgoingMessageRef waitingRef = (OutgoingMessageRef) waitingOutgoing.dequeue();
					if (waitingRef == null)
						break;

					Monitor.Enter(waitingRef);

					waitingRef.message.localSeqNo = outgoingWindow.append(waitingRef);
					sendingQueue.enqueue(waitingRef);
				}
			}

#if DEBUG_ReliableSender
			logger.Log(null, "LEAVE: " + outgoingWindow.ToString());
#endif

			Monitor.Exit(outgoingWindow);

			if (sendingQueue != null)
			{
				while (true)
				{
					OutgoingMessageRef waitingRef = (OutgoingMessageRef) sendingQueue.dequeue();
		
					if (waitingRef != null)
					{
						underlyingSender.send(this, new Base.ObjectAddress((QS.Fx.Network.NetworkAddress) waitingRef.destinationAddress,
							this.LocalObjectID), waitingRef.message, null);

						waitingRef.resendingAlarm = alarmClock.Schedule(
							timeoutForResending.TotalSeconds, new QS.Fx.QS.Fx.Clock.AlarmCallback(this.alarmCallback), waitingRef);

						Monitor.Exit(waitingRef);
					}
					else
						break;
				}
			}

			bool should_ignore = messageRef.should_ignore;
			Monitor.Exit(messageRef);
	
			if (!should_ignore && messageRef.acknowledgeCallback != null)
				messageRef.acknowledgeCallback(messageRef, true, null);			
		}

		private void processOurMessage(Base.ObjectAddress sourceAddress, OurMessage message)
		{
#if DEBUG_ReliableSender
			logger.Log(this, "received message from " + sourceAddress.ToString() + ", seqno = " + message.localSeqNo);
#endif

			bool should_sendack = false;
//			bool should_sendnack = false;

			QS.Fx.Network.NetworkAddress networkAddress = (QS.Fx.Network.NetworkAddress) sourceAddress;

			FlowControl.IIncomingWindow incomingWindow = null;
			lock (incomingWindows)
			{
				Collections.IDictionaryEntry dic_en = incomingWindows.lookupOrCreate(networkAddress);
				if (dic_en.Value == null)
				{
					incomingWindow = new FlowControl.IncomingWindow(windowSize);
					dic_en.Value = incomingWindow;					
				}
				else
					incomingWindow = (FlowControl.IIncomingWindow) dic_en.Value;

				Monitor.Enter(incomingWindow);
			}

#if DEBUG_ReliableSender
			logger.Log(null, "ENTER: " + incomingWindow.ToString());
#endif

			if (incomingWindow.accepts(message.localSeqNo))
			{
				should_sendack = true;

				if (incomingWindow.lookup(message.localSeqNo) == null)
				{
					incomingWindow.insert(message.localSeqNo, message);
				
					while (incomingWindow.ready())
					{
						OurMessage anotherMessage = (OurMessage) incomingWindow.consume();
						incomingWindow.cleanupOneGuy();

						Monitor.Exit(incomingWindow);

						demultiplexer.demultiplex(anotherMessage.targetLOID, 
							new Base.ObjectAddress(networkAddress, anotherMessage.senderLOID), anotherMessage.messageObj);

						Monitor.Enter(incomingWindow);
					}
				}
			}
			else
			{
				should_sendack = incomingWindow.consumed(message.localSeqNo);
//				should_sendnack = true;
			}

#if DEBUG_ReliableSender
			logger.Log(null, "LEAVE: " + incomingWindow.ToString());
#endif

			Monitor.Exit(incomingWindow);

			if (should_sendack)
			{
				AckMessage ackmsg = new AckMessage(message.localSeqNo);
				underlyingSender.send(this, sourceAddress, ackmsg, null);
			}
//			else if (should_sendnack)
//			{
//			}
		}
	
		[Serializable]
		private class OurMessage : Base.IMessage
		{
			public static Base.IBase1Serializable createSerializable()
			{
				return new OurMessage();
			}

			private OurMessage()
			{
			}

			public OurMessage(uint senderLOID, uint targetLOID, uint localSeqNo, Base.IMessage messageObj)
			{
				this.senderLOID = senderLOID;
				this.targetLOID = targetLOID;
				this.localSeqNo = localSeqNo;
				this.messageObj = messageObj;
			}

			public uint senderLOID, targetLOID, localSeqNo;
			public Base.IMessage messageObj;

			public QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.ReliableSender_OurMessage;
				}
			}

			public void save(Stream stream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(senderLOID);
				stream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(targetLOID);
				stream.Write(buffer, 0, buffer.Length);								
				buffer = System.BitConverter.GetBytes(localSeqNo);
				stream.Write(buffer, 0, buffer.Length);								
				buffer = System.BitConverter.GetBytes((ushort) messageObj.ClassIDAsSerializable);
				stream.Write(buffer, 0, buffer.Length);			
				messageObj.save(stream);
			}

			public void load(Stream stream)
			{
				byte[] buffer = new byte[14];
				stream.Read(buffer, 0, 14);
				senderLOID = System.BitConverter.ToUInt32(buffer, 0);
				targetLOID = System.BitConverter.ToUInt32(buffer, 4);				
				localSeqNo = System.BitConverter.ToUInt32(buffer, 8);
				ushort messageClassID = System.BitConverter.ToUInt16(buffer, 12);				
				messageObj = (Base.IMessage) Base.Serializer.Get.createObject((ClassID) messageClassID);
				messageObj.load(stream); 
			}

			public override string ToString()
			{
				return messageObj.ToString ();
			}
		}

		[Serializable]
		private class AckMessage : Base.IMessage
		{
			public static Base.IBase1Serializable createSerializable()
			{
				return new AckMessage();
			}

			public AckMessage()
			{
			}

			public AckMessage(uint localSeqNo)
			{
				this.localSeqNo = localSeqNo;
			}

			public uint localSeqNo;

			public QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.ReliableSender_AckMessage;
				}
			}

			public void save(Stream stream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(localSeqNo);
				stream.Write(buffer, 0, buffer.Length);								
			}

			public void load(Stream stream)
			{
				byte[] buffer = new byte[4];
				stream.Read(buffer, 0, 4);
				localSeqNo = System.BitConverter.ToUInt32(buffer, 0);
			}
		}

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{				
				return this.objectID;
			}
		}

		#endregion
	}
*/
}
