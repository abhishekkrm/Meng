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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace QS._qss_c_.Senders_1_
{
	/// <summary>
	/// This is a simple sender based on the UDP protocol, unicasting packets to all
	/// hosts specified in the destination address independently. This sender spawns
	/// its own thread for the purpose of listening on local socket. This sender can
	/// also be used for IP multicasting.
	/// </summary>
	public class BaseSender : Base1_.ISender
	{
		public BaseSender(Devices_1_.IUnicastingDevice unicastingDevice, 
			Devices_1_.IMulticastingDevice multicastingDevice, Devices_1_.IReceivingDevice[] receivingDevices, 
			Base1_.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger)
		{
			Base1_.Serializer.Get.register(ClassID.BaseSender_OurMessage, 
				new QS._core_c_.Base.CreateSerializable(OurMessage.createSerializable));

			this.unicastingDevice = unicastingDevice;
			this.multicastingDevice = multicastingDevice;
			this.demultiplexer = demultiplexer;
			this.logger = logger;

			Devices_1_.OnReceiveCallback receiveCallback = new Devices_1_.OnReceiveCallback(OnReceiveCallback);
			foreach (Devices_1_.IReceivingDevice receivingDevice in receivingDevices)
				receivingDevice.registerOnReceiveCallback(receiveCallback);
		}

		private Devices_1_.IUnicastingDevice unicastingDevice;
		private Devices_1_.IMulticastingDevice multicastingDevice;
		private Base1_.IDemultiplexer demultiplexer;
		private QS.Fx.Logging.ILogger logger = null;

		private class OurMessage : QS._core_c_.Base.IMessage
		{
			public static QS._core_c_.Base.IBase1Serializable createSerializable()
			{
				return new OurMessage();
			}

			public OurMessage()
			{
			}

			public OurMessage(uint senderPort, uint senderLOID, 
				uint targetLOID, QS._core_c_.Base.IMessage message)
			{
				this.senderPort = senderPort;
				this.senderLOID = senderLOID;
				this.targetLOID = targetLOID;
				this.message = message;
			}

			public uint senderPort, senderLOID, targetLOID;
			public QS._core_c_.Base.IMessage message;

			#region IMessage Members

			public ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.BaseSender_OurMessage;
				}
			}

			public void save(Stream memoryStream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(
					(ushort) ClassID.BaseSender_OurMessage);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(senderPort);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(senderLOID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(targetLOID);
				memoryStream.Write(buffer, 0, buffer.Length);				
				buffer = System.BitConverter.GetBytes((ushort) message.ClassIDAsSerializable);
				memoryStream.Write(buffer, 0, buffer.Length);
				message.save(memoryStream);
			}

			public void load(Stream memoryStream)
			{
				// memoryStream.Seek(0, SeekOrigin.Begin);
				byte[] buffer = new byte[16];
				memoryStream.Read(buffer, 0, 16);
				ushort wrapperClassID = System.BitConverter.ToUInt16(buffer, 0);
				Debug.Assert(wrapperClassID == (ushort) ClassID.BaseSender_OurMessage);
				senderPort = System.BitConverter.ToUInt32(buffer, 2);
				senderLOID = System.BitConverter.ToUInt32(buffer, 6);
				targetLOID = System.BitConverter.ToUInt32(buffer, 10);				
				ushort messageClassID = System.BitConverter.ToUInt16(buffer, 14);				
				message = (QS._core_c_.Base.IMessage) Base1_.Serializer.Get.createObject((ClassID) messageClassID);
				message.load(memoryStream); 
			}

			#endregion
		}

		#region ISender Members

		public QS._qss_c_.Base1_.IMessageReference send(
			Base1_.IClient theSender, Base1_.IAddress address, 
			QS._core_c_.Base.IMessage message, Base1_.SendCallback callback)
		{
			foreach (Base1_.ObjectAddress objectAddress in address.Destinations)
			{
				MemoryStream memoryStream = new MemoryStream();
				OurMessage ourmsg = new OurMessage((uint) unicastingDevice.PortNumber, 
					theSender.LocalObjectID, objectAddress.LocalObjectID, message);
				ourmsg.save(memoryStream);

				IPAddress ipAddress = objectAddress.NetworkAddress.HostIPAddress;
				byte[] addressBytes = ipAddress.GetAddressBytes();
				if (addressBytes[0] < 224 || addressBytes[0] > 239)
				{
					unicastingDevice.unicast(ipAddress, objectAddress.NetworkAddress.PortNumber,
						memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
				}
				else
				{
					if (multicastingDevice != null)
					{
						multicastingDevice.multicast(ipAddress, objectAddress.NetworkAddress.PortNumber,
							memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
					}
					else
						throw new Exception("multicasting not supported");
				}
			}

			return null;
		}

		#endregion

		private void OnReceiveCallback(
			IPAddress senderAddress, int senderPortNo, byte[] buffer, uint bufferSize)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream(buffer);
				OurMessage ourmsg = new OurMessage();
				ourmsg.load(memoryStream);

				Base1_.IAddress sourceAddress = new Base1_.ObjectAddress(senderAddress, 
					(ourmsg.senderPort != 0) ? ((int) ourmsg.senderPort) : senderPortNo, ourmsg.senderLOID);
	
				demultiplexer.demultiplex(
					ourmsg.targetLOID, sourceAddress, ourmsg.message);
			}
			catch (Exception exc)
			{
				logger.Log(this, "OnReceiveCallback, " + exc.ToString());
			}
		}
	}
}
