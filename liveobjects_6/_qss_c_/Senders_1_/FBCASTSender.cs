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

#define DEBUG_FBCASTSender

using System;
using System.Diagnostics;
using QS._qss_c_.VS_2_;
using System.Threading;

namespace QS._qss_c_.Senders_1_
{
	/// <summary>
	/// A simple virtually synchronous sender.
	/// </summary>
/*
	public class FBCASTSender : Base.ISender, Base.IClient, VS2.IVSSender
	{
		public FBCASTSender(VS2.IViewController viewController, Base.IDemultiplexer demultiplexer, Base.ISender underlyingSender, 
			QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, TimeSpan sendingTimeout, QS.Fx.Logging.ILogger logger) 
		{	
			Base.Serializer.Get.register(ClassID.FBCASTSender_OurMessage, new Base.CreateSerializable(OurMessage.createSerializable));
			Base.Serializer.Get.register(ClassID.FBCASTSender_AckMessage, new Base.CreateSerializable(AckMessage.createSerializable));

			this.logger = logger;
			this.viewController = viewController;
			this.demultiplexer = demultiplexer;
			this.underlyingSender = underlyingSender;
			this.alarmClock = alarmClock;

			demultiplexer.register(this, new CMS.Dispatchers.DirectDispatcher(
				new CMS.Base.OnReceive(this.receiveCallback)));	

			this.sendingTimeout = sendingTimeout;
		}	

		private IViewController viewController;
		private Base.IDemultiplexer demultiplexer;
		private QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock;
		private Base.ISender underlyingSender;
		private QS.Fx.Logging.ILogger logger;

		private TimeSpan sendingTimeout;

		public IncomingMessageRef createIncoming(Base.ObjectAddress senderAddress, Group group, Group.View view, 
			uint inViewSeqNo, Base.IMessage message)
		{
			return new IncomingSlot(senderAddress, group, view, inViewSeqNo, message, this);
		}

		private void receiveOurMessage(Base.ObjectAddress sourceObjectAddress, OurMessage ourmsg)
		{
#if DEBUG_FBCASTSender
			logger.Log(this, "received a message from " + sourceObjectAddress.ToString() + ourmsg.ToString());
#endif


			// we should not respond with ACK to a message from unjoined node, but for now we just leave it out.... 

			AckMessage ackmsg = new AckMessage(ourmsg);
			underlyingSender.send(this, sourceObjectAddress, ackmsg, null);		

			Base.ObjectAddress senderAddress = new Base.ObjectAddress((QS.Fx.Network.NetworkAddress) sourceObjectAddress, ourmsg.senderLOID);

			VS2.IncomingSlot incomingSlot = (VS2.IncomingSlot) viewController.lookupOrCreateIncoming(
				senderAddress, ourmsg.groupID, ourmsg.viewSeqNo, ourmsg.inViewSeqNo, ourmsg.realMessage, this, 
				(ourmsg.protocolPhase == ProtocolPhase.INITIALLY_SENDING_MESSAGE));

			if (incomingSlot != null)
			{
				if (ourmsg.protocolPhase > incomingSlot.CurrentPhase)
				{
					incomingSlot.CurrentPhase = ourmsg.protocolPhase;

					Monitor.Exit(incomingSlot);

					switch (incomingSlot.CurrentPhase)
					{
						case ProtocolPhase.MESSAGE_DELIVERED:
						case ProtocolPhase.MESSAGE_CLEANUP_COMPLETE:
						{
							// do nothing; we clean window slots lazily, as new slots are requested
						}
						break;

						default:
						{
							Debug.Assert(false);
						}
						break;
					}					
				}
			}
		}

		private void receiveAckMessage(Base.ObjectAddress sourceObjectAddress, AckMessage ackmsg)
		{
#if DEBUG_FBCASTSender
			logger.Log(this, "received an ACK from " + sourceObjectAddress.ToString() + ackmsg.ToString());
#endif

			VS2.OutgoingSlot	outgoingSlot = 
				(VS2.OutgoingSlot) viewController.lookupOutgoing(ackmsg.groupID, ackmsg.viewSeqNo, ackmsg.inViewSeqNo);

#if DEBUG_FBCASTSender
			logger.Log(this, "located outgoing slot");
#endif

			if (ackmsg.protocolPhase == outgoingSlot.CurrentPhase)
			{
				QS.Fx.Network.NetworkAddress sourceNetAddr = (QS.Fx.Network.NetworkAddress) sourceObjectAddress;

#if DEBUG_FBCASTSender
				logger.Log(this, "marking as confirmed");
#endif

				if (outgoingSlot.markAsConfirmed(sourceNetAddr) && (outgoingSlot.AllConfirmed))
				{
					outgoingSlot.resetACKs();

#if DEBUG_FBCASTSender
					logger.Log(this, "canceling alarm");
#endif

					alarmClock.cancelAlarm(outgoingSlot.alarmRef);

#if DEBUG_FBCASTSender
					logger.Log(this, "old slot phase = " + outgoingSlot.CurrentPhase.ToString());
#endif

					switch (outgoingSlot.CurrentPhase)
					{
						case ProtocolPhase.INITIALLY_SENDING_MESSAGE:
						{
							outgoingSlot.CurrentPhase = ProtocolPhase.MESSAGE_DELIVERED;
							outgoingSlot.disposeOfMessage();
						}
						break;

						case ProtocolPhase.MESSAGE_DELIVERED:
						{
							outgoingSlot.CurrentPhase = ProtocolPhase.MESSAGE_CLEANUP_COMPLETE;
						}
						break;

						case ProtocolPhase.MESSAGE_CLEANUP_COMPLETE:
						{
							outgoingSlot.CurrentPhase = ProtocolPhase.ALL_PHASES_COMPLETE;
							Monitor.Exit(outgoingSlot);

							viewController.acknowledgedOutgoing(ackmsg.groupID, ackmsg.viewSeqNo, ackmsg.inViewSeqNo);
							return;
						}

						default:
						{
							Debug.Assert(false);
						}
						break;
					}

#if DEBUG_FBCASTSender
					logger.Log(this, "new slot phase = " + outgoingSlot.CurrentPhase.ToString() + ", multicasting now");
#endif

					multicast(outgoingSlot);
				}
			}
			else
			{					
				Debug.Assert(ackmsg.protocolPhase < outgoingSlot.CurrentPhase);
			}

			Monitor.Exit(outgoingSlot);
		}

		private void receiveCallback(Base.IAddress source, Base.IMessage message)
		{
			if (!(source is Base.ObjectAddress))
				throw new Exception("Unknown source type");			
			Base.ObjectAddress sourceObjectAddress = (Base.ObjectAddress) source;

			if (message is OurMessage)
			{
				receiveOurMessage(sourceObjectAddress, (OurMessage) message);
			}
			else if (message is AckMessage)
			{
				receiveAckMessage(sourceObjectAddress, (AckMessage) message);
			}
			else if (message is Base.AnyMessage)
			{
				Base.AnyMessage anymsg = (Base.AnyMessage) message;
				
				if (anymsg.Contents is ForwardingRequest.MessageRef)
				{
					ForwardingRequest.MessageRef messageRef = (ForwardingRequest.MessageRef) anymsg.Contents;

#if DEBUG_FBCASTSender
					logger.Log(this, "received a forwarded message ref from " + source.ToString() + 
						", messageRef = " + messageRef.ToString());
#endif

					this.receiveOurMessage(messageRef.originalSender, new OurMessage(ProtocolPhase.INITIALLY_SENDING_MESSAGE,
						messageRef.message, messageRef.destinationViewAddress.GroupID, messageRef.destinationViewAddress.ViewSeqNo,
						messageRef.inViewSeqNo, messageRef.originalSender.LocalObjectID));
				}
				else
					throw new Exception("Oh no!");
			}
			else
				throw new Exception("Wrong message type: " + message.GetType().ToString());
		}

		private bool alarmCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
		{
			OutgoingSlot outgoingSlot = (OutgoingSlot) alarmRef.Context;
			
			lock (outgoingSlot)
			{
				bool resending = (outgoingSlot.CurrentPhase != ProtocolPhase.ALL_PHASES_COMPLETE);
				if (resending)
					multicast(outgoingSlot);

				return resending;
			}
		}

		private void multicast(VS2.OutgoingSlot outgoingSlot)
		{
#if DEBUG_FBCASTSender			
			logger.Log(this, "multicast_enter");
#endif

			OurMessage ourmsg = new OurMessage(outgoingSlot.CurrentPhase, outgoingSlot);					
			System.Collections.ICollection networkAddresses = networkAddresses = outgoingSlot.Pending;

			foreach (QS.Fx.Network.NetworkAddress networkAddress in networkAddresses)
			{
				Base.IAddress address = new Base.ObjectAddress(networkAddress, this.LocalObjectID);
#if DEBUG_FBCASTSender			
				logger.Log(this, "sending to " + address.ToString() + " message " + ourmsg.ToString());
#endif
				underlyingSender.send(this, address, ourmsg, null);		
			}
			

#if DEBUG_FBCASTSender			
			logger.Log(this, "multicast_leave");
#endif
		}

		#region VS.IVSSender Members

		public virtual void readyToSendOut(OutgoingMessageRef outgoingMessageRef)
		{
			VS2.OutgoingSlot outgoingSlot = (VS2.OutgoingSlot) outgoingMessageRef;
#if DEBUG_FBCASTSender			
			logger.Log(this, "readyToSendOut : " + outgoingSlot.ToString());
#endif

			multicast(outgoingSlot);

			QS.Fx.QS.Fx.Clock.IAlarm alarmRef = alarmClock.Schedule(sendingTimeout, 
				new QS.Fx.QS.Fx.Clock.AlarmCallback(this.alarmCallback), outgoingMessageRef);

			outgoingSlot.alarmRef = alarmRef;			
		}
		
		public virtual void readyToDeliver(IncomingMessageRef incomingMessageRef)
		{
			uint[] localReceivers = incomingMessageRef.ViewOf.LocalReceivers;
			foreach (uint receiverLOID in localReceivers)
			{
				demultiplexer.demultiplex(receiverLOID, incomingMessageRef.SenderAddress, incomingMessageRef.Message);
			}
		}

		public void forward(QS.Fx.Network.NetworkAddress forwardingAddress, ForwardingRequest.MessageRef forwardedMessageRef)
		{
			underlyingSender.send(this, new Base.ObjectAddress(forwardingAddress, this.LocalObjectID), 
				new Base.AnyMessage(forwardedMessageRef), null);
		}

		#endregion

		public enum ProtocolPhase
		{
			INITIALLY_SENDING_MESSAGE, // READY_TO_DELIVER_MESSAGE
			MESSAGE_DELIVERED, MESSAGE_CLEANUP_COMPLETE, // FLUSHING
			ALL_PHASES_COMPLETE
		}

		#region ISender Members

		public Base.IMessageReference send(
			Base.IClient theSender, Base.IAddress destinationAddress, 
			Base.IMessage message, Base.SendCallback sendCallback)
		{
			if (destinationAddress is Base.GroupAddress)
			{
#if DEBUG_FBCASTSender
				logger.Log(this, "Base.ISender SEND \"" + message.ToString() + "\" --> " + destinationAddress.ToString());
#endif
				OutgoingSlot outgoingSlot = new OutgoingSlot(theSender.LocalObjectID, 
					((Base.GroupAddress) destinationAddress).GroupID, message, sendCallback);

				viewController.registerOutgoing(outgoingSlot, this);
			}
			else 
				throw new Exception("Wrong address type!");

			return null;
		}

		#endregion

		// --------------------------------------------------------------------------------------------------------

		[System.Serializable]
		public class OurMessage : Base.GenericSerializable, Base.IMessage
		{
			public static Base.ISerializable createSerializable()
			{
				return new OurMessage();
			}

			public OurMessage()
			{
			}

			public OurMessage(ProtocolPhase protocolPhase, Base.IMessage realMessage, GMS.GroupId groupID, 
				uint viewSeqNo, uint inViewSeqNo, uint senderLOID)
			{
				this.protocolPhase = protocolPhase;
				this.realMessage = realMessage;
				this.groupID = groupID;
				this.viewSeqNo = viewSeqNo;
				this.inViewSeqNo = inViewSeqNo;
				this.senderLOID = senderLOID;
			}

			public OurMessage(ProtocolPhase protocolPhase, OutgoingMessageRef messageRef)
			{
				this.protocolPhase = protocolPhase;
				this.realMessage = messageRef.Message;
				this.groupID = messageRef.GroupID;
				this.viewSeqNo = messageRef.View.SeqNo;
				this.inViewSeqNo = messageRef.InViewSeqNo;
				this.senderLOID = messageRef.SenderLOID;
			}

			protected override void copy(object anotherObject)
			{
				OurMessage ourmsg = (OurMessage) anotherObject;

				protocolPhase = ourmsg.protocolPhase;
				realMessage = ourmsg.realMessage;
				groupID = ourmsg.groupID;
				senderLOID = ourmsg.senderLOID;
				viewSeqNo = ourmsg.viewSeqNo;
				inViewSeqNo = ourmsg.inViewSeqNo;
			}

			public override string ToString()
			{
				return "(Sender: " + senderLOID.ToString() + " GID:" + groupID.ToString() + " View:" + viewSeqNo.ToString() + 
					" SeqNo:" + inViewSeqNo.ToString() + " Phase:" + protocolPhase.ToString() + " " +
					((realMessage != null) ? ("\"" +  realMessage.ToString() + "\"") : "null_message") + ")";
			}

			public ProtocolPhase protocolPhase;
			public Base.IMessage realMessage;
			public GMS.GroupId groupID;
			public uint senderLOID, viewSeqNo, inViewSeqNo;

			#region ISerializable Members

			public override QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.FBCASTSender_OurMessage;
				}
			}

			#endregion
		}

		[System.Serializable]
		public class AckMessage : Base.GenericSerializable, Base.IMessage
		{
			public static Base.ISerializable createSerializable()
			{
				return new AckMessage();
			}

			public AckMessage()
			{
			}

			public AckMessage(OurMessage ourmsg)
			{
				protocolPhase = ourmsg.protocolPhase;
				groupID = ourmsg.groupID;
				viewSeqNo = ourmsg.viewSeqNo;
				inViewSeqNo = ourmsg.inViewSeqNo;
			}

			protected override void copy(object anotherObject)
			{
				AckMessage ackmsg = (AckMessage) anotherObject;

				protocolPhase = ackmsg.protocolPhase;
				groupID = ackmsg.groupID;
				viewSeqNo = ackmsg.viewSeqNo;
				inViewSeqNo = ackmsg.inViewSeqNo;
			}

			public override string ToString()
			{
				return "(ACK GID:" + groupID.ToString() + ";VSN:" + viewSeqNo.ToString() + ";SeqNo:" + 
					inViewSeqNo.ToString() + " Phase:" + protocolPhase.ToString() + ")";
			}

			public ProtocolPhase protocolPhase;
			public GMS.GroupId groupID;
			public uint viewSeqNo, inViewSeqNo;

			#region ISerializable Members

			public override QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.FBCASTSender_AckMessage;
				}
			}

			#endregion
		}

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.FBCASTSender;
			}
		}

		#endregion
	}
*/
}
