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

#define DEBUG_RetransmittingScatterer

using System;
using System.Threading;

namespace QS._qss_c_.Scattering_1_
{
	/// <summary>
	/// Summary description for SimpleMulticastingSender.
	/// </summary>
	public class RetransmittingScatterer : Scattering_1_.IRetransmittingScatterer
	{
		public RetransmittingScatterer(QS.Fx.Logging.ILogger logger, Base2_.IDemultiplexer demultiplexer,
			Scattering_1_.IScatterer underlyingScatterer, Base2_.IBlockSender underlyingAcknowledgementSender, 
			Base2_.IIdentifiableObjectContainer objectContainer,
			QS.Fx.Clock.IAlarmClock alarmClock, double retransmissionTimeout, bool exponentialBackoff)
		{
			this.logger = logger;

			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(
				QS.ClassID.Multicasting2_SimpleMulticastingSender_Multicast, typeof(WrappedMessage));
			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(
				QS.ClassID.Multicasting2_SimpleMulticastingSender_Acknowledgement, typeof(Acknowledgement));

			this.demultiplexer = demultiplexer;
			this.alarmClock = alarmClock;
			this.underlyingAcknowledgementSender = underlyingAcknowledgementSender;
			this.underlyingScatterer = underlyingScatterer;
			this.objectContainer = objectContainer;
			this.retransmissionTimeout = retransmissionTimeout;
			this.exponentialBackoff = exponentialBackoff;

			this.alarmCallback = new QS.Fx.Clock.AlarmCallback(this.retransmissionAlarmCallback);
			demultiplexer.register((uint) ReservedObjectID.Scattering_RetransmittingScatterer_MessageChannel, 
				new Base2_.ReceiveCallback(this.receiveMessageCallback));
			demultiplexer.register((uint) ReservedObjectID.Scattering_RetransmittingScatterer_AcknowledgementChannel, 
				new Base2_.ReceiveCallback(this.receiveAcknowledgementCallback));
		}

		private QS.Fx.Logging.ILogger logger;
		private Base2_.IDemultiplexer demultiplexer;
		private Scattering_1_.IScatterer underlyingScatterer;
		private Base2_.IBlockSender underlyingAcknowledgementSender;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private Base2_.IIdentifiableObjectContainer objectContainer;
		private double retransmissionTimeout;
		private bool exponentialBackoff;

		#region Class WrappedMessage

		[QS.Fx.Serialization.ClassID(QS.ClassID.Multicasting2_SimpleMulticastingSender_Multicast)]
		public class WrappedMessage : Base2_.IdentifiableObject
		{
			public WrappedMessage(uint destinationLOID, Base2_.IIdentifiableObject message)
			{
				this.destinationLOID = destinationLOID;
				this.message = message;
			}

			public WrappedMessage()
			{
			}

			protected uint destinationLOID;
			protected Base2_.IIdentifiableObject message;

			public uint DestinationLOID
			{
				get
				{
					return this.destinationLOID;
				}
			}

			public QS._core_c_.Base2.IBase2Serializable Message
			{
				get
				{
					return message;
				}
			}

			public Acknowledgement CreateAcknowledgement
			{
				get
				{
					return new Acknowledgement(message.UniqueID);
				}
			}

			#region Base2.IIdentifiableSerializableObject Members

			public override QS._qss_c_.Base2_.IIdentifiableKey UniqueID
			{
				get
				{
					return message.UniqueID;
				}
			}

			#endregion

			#region Base2.ISerializable Members

			public override uint Size
			{
				get
				{
					return QS._core_c_.Base2.SizeOf.UInt32 + QS._core_c_.Base2.SizeOf.UInt16 + message.Size;
				}
			}

			public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(destinationLOID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) QS._core_c_.Base2.SizeOf.UInt32);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
				Buffer.BlockCopy(BitConverter.GetBytes((ushort) message.ClassID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) QS._core_c_.Base2.SizeOf.UInt16);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
				message.save(blockOfData);
			}

			public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				this.destinationLOID = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
				ClassID messageClassID = 
					(ClassID) BitConverter.ToUInt16(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
				this.message = (Base2_.IIdentifiableObject) 
					QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(messageClassID);
				message.load(blockOfData);
			}

			public override QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.Multicasting2_SimpleMulticastingSender_Multicast;
				}
			}
		
			#endregion
		}

		#endregion

		#region Class Acknowledgement

		[QS.Fx.Serialization.ClassID(QS.ClassID.Multicasting2_SimpleMulticastingSender_Acknowledgement)]
		public class Acknowledgement : Base2_.SerializableWrapper, Base2_.IIdentifiableKey
		{
			public Acknowledgement()
			{
			}

			public Acknowledgement(Base2_.IIdentifiableKey identifyingObject) : base(identifyingObject)
			{
			}

			public QS._qss_c_.Base2_.ContainerClass ContainerClass
			{
				get
				{
					throw new Exception("Operation not permitted in this context.");
					// return QS.CMS.Base2.ContainerClass.DefaultContainer;
				}
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				return ((Base2_.IIdentifiableKey) this.WrappedObject).CompareTo(obj);
			}

			#endregion
		}

		#endregion

		#region Class GenericRequest

		[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
		public class GenericRequest : WrappedMessage, IRetransmittingScattererRequest
		{
			public GenericRequest(uint destinationLOID, Scattering_1_.IScatterSet addressSet, 
				Base2_.IIdentifiableObject message, Base2_.IIdentifiableObjectContainer enclosingObjectContainer,
				CompletionCallback completionCallback) : base(destinationLOID, message)
			{
				this.addressSet = addressSet;
				this.enclosingObjectContainer = enclosingObjectContainer;
				this.externalCompletionCallback = completionCallback;

				addressSet.registerCallback(new CompletionCallback(this.internalCompletionCallback));
			}

			private QS.Fx.Clock.IAlarm alarmRef = null;

			protected Scattering_1_.IScatterSet addressSet;
			protected CompletionCallback externalCompletionCallback;
			protected Base2_.IIdentifiableObjectContainer enclosingObjectContainer;

			protected virtual void completionCallback(bool succeeded, System.Exception exception)
			{
				enclosingObjectContainer.remove(this);
				if (externalCompletionCallback != null)
					externalCompletionCallback(succeeded, exception);
			}

			private void internalCompletionCallback(bool succeeded, System.Exception exception)
			{
				lock (this)
				{
					if (alarmRef != null)
						alarmRef.Cancel();
				}
				
				this.completionCallback(succeeded, exception);
			}

			#region IRetransmittingScattererRequest Members

//			public uint DestinationLOID
//			{
//				get
//				{
//					return destinationLOID;
//				}
//			}

			public QS._qss_c_.Base2_.IIdentifiableObject WrappedIdentifiableObject
			{
				get
				{
					return this;
				}
			}

			public CompletionCallback Callback
			{
				get
				{
					return this.externalCompletionCallback;
				}
			}

			public Scattering_1_.IScatterSet AddressSet
			{
				get
				{
					return addressSet;
				}
			}

			public QS.Fx.Clock.IAlarm AlarmRef
			{
				get
				{
					return alarmRef;
				}

				set
				{
					alarmRef = value;
				}
			}

			#endregion
		}

		#endregion

/*
		#region Class IncomingRequest

		private class IncomingRequest : WrappedMessage
		{
			public IncomingRequest(QS.Fx.Network.NetworkAddress sourceAddress, Base2.IBlockOfData blockOfData) : base()
			{
				this.sourceAddress = sourceAddress;
				this.load(blockOfData);				
			}

			private QS.Fx.Network.NetworkAddress sourceAddress;
		}

		#endregion
*/

		#region Alarm Clock and Network Receive Callbacks

		private QS.Fx.Clock.AlarmCallback alarmCallback = null;
		private void retransmissionAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
		{
			IRetransmittingScattererRequest multicastRequest = (IRetransmittingScattererRequest) alarmRef.Context;
			lock (multicastRequest)
			{				
				Scattering_1_.IScatterAddress scatterAddress = multicastRequest.AddressSet.ScatterAddress;
				if (scatterAddress != null)
				{
					underlyingScatterer.scatter(scatterAddress, 
						(uint) ReservedObjectID.Scattering_RetransmittingScatterer_MessageChannel, 
						multicastRequest.WrappedIdentifiableObject);

					if (exponentialBackoff)
						alarmRef.Reschedule(2 * alarmRef.Timeout);
					else
						alarmRef.Reschedule();
				}
				else
				{
					throw new Exception("Scatter address is NULL.");
 					// objectContainer.remove(multicastRequest);
					// multicastRequest.Callback(false, null);
				}
			}
		}

		private QS._core_c_.Base2.IBase2Serializable receiveMessageCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
//			logger.Log(this, "__________receiveMessageCallback : " + serializableObject.ToString());

			WrappedMessage wrappedMessage = (WrappedMessage) serializableObject;

			try
			{
				this.dispatch(sourceAddress, destinationAddress, wrappedMessage.DestinationLOID, 
					wrappedMessage.Message);

				underlyingAcknowledgementSender.send(
					(uint) ReservedObjectID.Scattering_RetransmittingScatterer_AcknowledgementChannel, 
					sourceAddress, wrappedMessage.CreateAcknowledgement);
			}
			catch (Exception exc)
			{
				logger.Log(this, "Could not deliver message: " + exc.ToString());
			}

			return null;
		}

		protected virtual void dispatch(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			uint destinationLOID, QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			demultiplexer.demultiplex(destinationLOID, sourceAddress, destinationAddress, serializableObject);
		}

		private QS._core_c_.Base2.IBase2Serializable receiveAcknowledgementCallback(
			QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			Acknowledgement acknowledgement = (Acknowledgement) serializableObject;
			Base2_.IIdentifiableKey key = (Base2_.IIdentifiableKey) acknowledgement.WrappedObject;

#if DEBUG_RetransmittingScatterer
//			logger.Log(this, "Acknowledgement(" + sourceAddress.ToString() + ", " + 
//				acknowledgement.ToString() + ")");
#endif
			IRetransmittingScattererRequest multicastRequest = (IRetransmittingScattererRequest) 
				objectContainer.lookup(key.ClassID, key);

			if (multicastRequest != null)
			{
				multicastRequest.AddressSet.acknowledged(sourceAddress);
				// Monitor.Exit(multicastRequest);
            }
#if DEBUG_RetransmittingScatterer

			else
			{
				logger.Log(this, "Could not locate the matching request in object container for acknowledgement " +
					acknowledgement.ToString() + " from " + sourceAddress.ToString());
			}
#endif

			return null;
		}

		#endregion

		#region Scattering2.IRetransmittingScatterer Members

		public Base2_.IIdentifiableObjectContainer IOContainer
		{
			set
			{
				this.objectContainer = value;
			}
		}

		public void multicast(uint destinationLOID, Scattering_1_.IScatterSet addressSet, 
			Base2_.IIdentifiableObject message, CompletionCallback completionCallback)
		{
			IRetransmittingScattererRequest multicastRequest = 
				new GenericRequest(destinationLOID, addressSet, message, objectContainer, completionCallback);
			objectContainer.insert(multicastRequest);

			this.multicast(multicastRequest);
		}

		public void multicast(IRetransmittingScattererRequest multicastRequest)
		{
			lock (multicastRequest)
			{							
				multicastRequest.AlarmRef = alarmClock.Schedule(
					retransmissionTimeout, this.alarmCallback, multicastRequest);

				Scattering_1_.IScatterAddress scatterAddress = multicastRequest.AddressSet.ScatterAddress;

				if (scatterAddress != null)
				{
					underlyingScatterer.scatter(scatterAddress, 
						(uint) ReservedObjectID.Scattering_RetransmittingScatterer_MessageChannel, 
						multicastRequest.WrappedIdentifiableObject);
				}
				else
					multicastRequest.Callback(false, null);
			}
		}

		public uint MTU
		{
			get
			{
				return underlyingScatterer.MTU - (QS._core_c_.Base2.SizeOf.UInt32 + QS._core_c_.Base2.SizeOf.UInt16);
			}
		}

		public double RetransmissionTimeout
		{
			get
			{
				return retransmissionTimeout;
			}

			set
			{
				retransmissionTimeout = value;
			}
		}

		#endregion
	}
}
