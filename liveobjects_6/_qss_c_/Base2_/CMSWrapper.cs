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

#define DEBUG_CMSWrapper

using System;
using System.Diagnostics;
using System.Net;

namespace QS._qss_c_.Base2_
{
/*
	/// <summary>
	/// Summary description for CMSWrapper.
	/// </summary>
	public class CMSWrapper : Base.ICMS, Base.ISender, Base.IClient
	{
        public CMSWrapper(Base.IConsole console, uint portNo)
            : this(new Base.Logger(null, false, console),
			new QS.Fx.Network.NetworkAddress(IPAddress.Any, (int) portNo))
		{
		}

		public CMSWrapper(Base.IReadableLogger logger, QS.Fx.Network.NetworkAddress baseAddress) 
			: this(new QS.CMS.Platform.PhysicalPlatform(logger), baseAddress)
		{
			platformShutdownRequired = true;
		}

		public CMSWrapper(Platform.IPlatform underlyingPlatform, QS.Fx.Network.NetworkAddress baseAddress)
		{
			Base.Serializer.Get.register(QS.ClassID.AnyMessage, Base.AnyMessage.Factory);

			Base2.Serializer.CommonSerializer.registerClass(
				QS.ClassID.CMSWrapper_WrappedMessage, typeof(CMSWrapper.WrappedMessage));

			this.underlyingPlatform = underlyingPlatform;
			
			if (baseAddress.HostIPAddress.Equals(IPAddress.Any))
				baseAddress.HostIPAddress = underlyingPlatform.NICs[0];
			this.baseAddress = baseAddress;

			underlyingPlatform.logger.Log(null, "Base Address : " + baseAddress.ToString());

			IDemultiplexer demultiplexer = new Demultiplexer(underlyingPlatform.Logger);
			this.rootSender = new RootSender(baseAddress, underlyingPlatform.UDPDevice, demultiplexer, underlyingPlatform.Logger);			
			demultiplexer.register(this.LocalObjectID, new Base2.ReceiveCallback(this.receiveCallback));

			old_demultiplexer = new Base.SimpleDemultiplexer(100);

			Base.ISender reliableSender = new Senders.ReliableSender(this, old_demultiplexer, underlyingPlatform.AlarmClock, 10, 10, 
				TimeSpan.FromMilliseconds(100), underlyingPlatform.Logger, false);			
			
			Multicasting.IMulticastingDevice multicastingDevice = 
				new Multicasting.DirectMulticastingDevice(this, underlyingPlatform.AlarmClock, underlyingPlatform.Logger);
			Flushing.IFlushingDevice flushingDevice = new Flushing.N2FlushingDevice(underlyingPlatform.Logger, reliableSender, old_demultiplexer);

			virtuallySynchronousSender = new VS3.VSSender(underlyingPlatform.Logger, baseAddress, multicastingDevice, old_demultiplexer, 
				reliableSender, reliableSender, flushingDevice);			 
		}

		private Platform.IPlatform underlyingPlatform;
		private QS.Fx.Network.NetworkAddress baseAddress;
		private Base.IDemultiplexer old_demultiplexer;
		private Base2.RootSender rootSender;
		private bool platformShutdownRequired = false;

		private VS3.VSSender virtuallySynchronousSender;

		#region ICMS Members

		public void join(IPAddress groupAddress, int listeningPortNo)
		{
			throw new Exception("not supported: join; please use QS.Base.CMS instead");
//			underlyingPlatform.UDPDevice.listenAt(
//				baseAddress.HostIPAddress, new QS.Fx.Network.NetworkAddress(groupAddress, listeningPortNo), rootSender.ReceiveCallback);
		}

		public void leave(IPAddress groupAddress)
		{
			throw new Exception("not supported: leave; please use QS.Base.CMS instead");
		}

		public void linkToGMS(QS.GMS.IGMS theGMS)
		{
			GMS.ViewChangeGoAhead viewChangeGoAhead = theGMS.linkCMSToGMS(
				new GMS.ViewChangeRequest(virtuallySynchronousSender.viewChangeRequest),
				new GMS.ViewChangeAllDone(virtuallySynchronousSender.viewChangeAllDone), 
				new GMS.ViewChangeCleanup(virtuallySynchronousSender.viewChangeCleanup));

			Debug.Assert((viewChangeGoAhead != null), "viewChangeGoAhead is null");

			virtuallySynchronousSender.registerGMSCallbacks(viewChangeGoAhead);		
		}

		public void shutdown()
		{
			if (platformShutdownRequired)
				underlyingPlatform.Dispose();
		}

		public Base.ISender[] Senders
		{
			get
			{				
				return new Base.ISender[] { this, virtuallySynchronousSender };
			}
		}

		public Base.IDemultiplexer Demultiplexer
		{
			get
			{
				return old_demultiplexer;
			}
		}

		public QS.Fx.Network.NetworkAddress Address
		{
			get
			{
				return baseAddress;
			}
		}

		#endregion

		#region Receive Callback

		private static uint sizeOfUInt16 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(ushort));
		private static uint sizeOfUInt32 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));

		private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			Base2.IBase2Serializable serializableObject)
		{
			try
			{
				WrappedMessage wrappedMessage = (WrappedMessage) serializableObject;
				Base.ObjectAddress sourceObjectAddress = new Base.ObjectAddress(sourceAddress, wrappedMessage.SourceLOID);

				Base.IMessage message = wrappedMessage.ReconstructMessage;

#if DEBUG_CMSWrapper
				underlyingPlatform.logger.Log(null, "Receive_Callback : " + sourceObjectAddress.ToString() + " -> " + 
					wrappedMessage.DestinationLOID.ToString() + "; " + message.ToString());
#endif

				old_demultiplexer.demultiplex(wrappedMessage.DestinationLOID, sourceObjectAddress, message);
			}
			catch (Exception exc)
			{
				underlyingPlatform.logger.Log(this, "Receive_Callback : " + exc.ToString());
			}

			return null;
		}

		#endregion

		#region Wrapped Message

		[ClassID(QS.ClassID.CMSWrapper_WrappedMessage)]
		private class WrappedMessage : Base2.IBase2Serializable
		{
			public WrappedMessage()
			{
			}

			public WrappedMessage(uint sourceLOID, uint destinationLOID, Base.IMessage data)
			{
				this.sourceLOID = sourceLOID;
				this.destinationLOID = destinationLOID;
				// this.data = data;
				this.messageClassID = (ushort) data.ClassIDAsSerializable;
				System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
				data.save(memoryStream);	
				this.compressedData = new BlockOfData(memoryStream.GetBuffer(), 0, (uint) memoryStream.Length);
			}

			private uint sourceLOID, destinationLOID;
			// private Base.IMessage data;
			private ushort messageClassID;
			private Base2.IBlockOfData compressedData;

			public uint SourceLOID
			{
				get
				{
					return sourceLOID;
				}
			}

			public uint DestinationLOID
			{
				get
				{
					return destinationLOID;
				}
			}

			public Base.IMessage ReconstructMessage
			{
				get
				{
					System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(compressedData.Buffer, 
						(int) compressedData.OffsetWithinBuffer, (int) compressedData.SizeOfData);
					Base.IMessage data = (Base.IMessage) Base.Serializer.Get.createObject((QS.ClassID) this.messageClassID);
					data.load(memoryStream); 
					return data;
				}
			}

			#region Base2.ISerializable Members

			public uint Size
			{
				get
				{
					return 2 * SizeOf.UInt32 + SizeOf.UInt16 + compressedData.Size;
				}
			}

			public QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.CMSWrapper_WrappedMessage;
				}
			}

			public void load(Base2.IBlockOfData blockOfData)
			{
				this.sourceLOID = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(sizeOfUInt32);
				this.destinationLOID = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(sizeOfUInt32);
				this.messageClassID = BitConverter.ToUInt16(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(sizeOfUInt16);

				// now we need to skip through data length
				blockOfData.consume(Base2.SizeOf.UInt32);
				this.compressedData = blockOfData;
			}

			public void save(Base2.IBlockOfData blockOfData)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(this.sourceLOID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) SizeOf.UInt32);
				blockOfData.consume(SizeOf.UInt32);
				Buffer.BlockCopy(BitConverter.GetBytes(this.destinationLOID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) SizeOf.UInt32);
				blockOfData.consume(SizeOf.UInt32);
				Buffer.BlockCopy(BitConverter.GetBytes(this.messageClassID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) SizeOf.UInt16);
				blockOfData.consume(SizeOf.UInt16);
				this.compressedData.save(blockOfData);
			}

			#endregion
		}

		#endregion

		#region ISender Members

		public QS.CMS.Base.IMessageReference send(QS.CMS.Base.IClient theSender, QS.CMS.Base.IAddress destinationAddress, 
			QS.CMS.Base.IMessage message, QS.CMS.Base.SendCallback sendCallback)
		{
			if (!(destinationAddress is Base.ObjectAddress))
				throw new Exception("Only plain ObjectAddress type of destinations are supported by CMSWrapper.");

#if DEBUG_CMSWrapper
			underlyingPlatform.logger.Log(null, "Sending : " + theSender.LocalObjectID.ToString() + " -> " + 
				destinationAddress.ToString() + "; " + message.ToString());
#endif
			
			Base2.IBase2Serializable wrappedMessage = new WrappedMessage(theSender.LocalObjectID, 
				((Base.ObjectAddress) destinationAddress).LocalObjectID, message);

			rootSender.send(this.LocalObjectID, ((Base.ObjectAddress) destinationAddress), wrappedMessage);
			return null;
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{				
				return (uint) ReservedObjectID.CMSWrapper;
			}
		}

		#endregion
	}
*/ 
}
