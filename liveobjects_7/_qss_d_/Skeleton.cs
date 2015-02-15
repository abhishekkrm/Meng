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
using System.Security.Cryptography;
using System.IO;

namespace QS._qss_d_
{
/*
	/// <summary>
	/// Aaa...
	/// </summary>
	public abstract class Skeleton : CMS.Base.IClient
	{
		public const uint HOST_MANAGEMENT_SERVICE_PORT_NUMBER_UCAST_UDP		=			 10666;
		public const uint HOST_MANAGEMENT_SERVICE_PORT_NUMBER_UCAST_TCP		=			 10667;
		public const uint HOST_MANAGEMENT_SERVICE_PORT_NUMBER_MCAST			=			 10668;						   

		public const string HOST_MANAGEMENT_SERVICE_IPMULTICASTADDR	=	"224.99.99.99";

		private const string CRYPTOGRAPHIC_KEY_PATH	=  "C:\\.testing\\service_key.xml";

		public Skeleton(CMS.Base.IReadableLogger logger, bool is_server)
		{
			uint udp_unicast_portno		= 
				is_server ? Skeleton.HOST_MANAGEMENT_SERVICE_PORT_NUMBER_UCAST_UDP : 0;
			uint tcp_unicast_portno		= 
				is_server ? Skeleton.HOST_MANAGEMENT_SERVICE_PORT_NUMBER_UCAST_TCP : 0;

			string multicast_addr	= Skeleton.HOST_MANAGEMENT_SERVICE_IPMULTICASTADDR;
			uint multicast_portno	= Skeleton.HOST_MANAGEMENT_SERVICE_PORT_NUMBER_MCAST;

			try
			{
				this.logger = logger;
				logger.Log(this, "Initialization begins...");

				serializer.register(ClassID.AnyMessage, CMS.Base.AnyMessage.Factory);
				CMS.RPC.Server.registerSerializableClasses();
				CMS.Base.EmptyObject.registerSerializableClass();

				communicationsDeviceUDP = 
					new CMS.Devices.UDPCommunicationsDevice(logger, is_server, (int) udp_unicast_portno, 5);
				communicationsDeviceUDP.join(
					IPAddress.Parse(multicast_addr), (int) multicast_portno);					

				communicationsDeviceTCP =
					new CMS.Devices.TCPCommunicationsDevice("ServiceSkeleton_TCP", 
                    CMS.Devices2.Network.LocalAddresses[0], logger, is_server, (int) tcp_unicast_portno, 5);

/-*
				logger.Log(this, "Addresses assigned : UDP = " + 
					communicationsDeviceUDP.IPAddress.ToString() + ":" + 
					communicationsDeviceUDP.PortNumber.ToString() + ", TCP = " +
					communicationsDeviceTCP.IPAddress.ToString() + ":" + 
					communicationsDeviceTCP.PortNumber.ToString());
*-/

				demultiplexer = new CMS.Base.SimpleDemultiplexer(5);
				demultiplexer.register(this, new CMS.Dispatchers.DirectDispatcher(
					new CMS.Base.OnReceive(this.receiveCallback)));

				CMS.Base.ISender baseUDPSender = new CMS.Senders.BaseSender(communicationsDeviceUDP, 
					communicationsDeviceUDP, new CMS.Devices.IReceivingDevice[] { communicationsDeviceUDP }, 
					demultiplexer, logger);
				CMS.Base.ISender baseTCPSender = new CMS.Senders.BaseSender(communicationsDeviceTCP, 
					null, new CMS.Devices.IReceivingDevice[] { communicationsDeviceTCP }, demultiplexer, logger);
				
				System.Xml.Serialization.XmlSerializer keySerializer = 
					new System.Xml.Serialization.XmlSerializer(typeof(byte[]));
				TextReader keyReader = new StreamReader(
					CRYPTOGRAPHIC_KEY_PATH, System.Text.Encoding.Unicode);
				byte[] cryptographicKey = (byte[]) keySerializer.Deserialize(keyReader);
				keyReader.Close();

				sender = new CMS.Senders.CryptoSender(baseTCPSender, demultiplexer, logger, 
					SymmetricAlgorithm.Create(), cryptographicKey);

				multicastingSender = new CMS.Senders.CryptoSender(baseUDPSender, demultiplexer, logger, 
					SymmetricAlgorithm.Create(), cryptographicKey);

				logger.Log(this, "Initialization completed");
			}
			catch (Exception exc)
			{
				try
				{
					this.shutdown();
				}
				catch (Exception)
				{
				}

				throw new Exception("Could not start... " + exc.ToString());
			}
		}

		public void shutdown()
		{
			logger.Log(this, "Shutting down...");

			communicationsDeviceTCP.shutdown();
			communicationsDeviceUDP.shutdown();
		}

		private CMS.Base.ISerializer serializer = CMS.Base.Serializer.Get;

		private CMS.Devices.UDPCommunicationsDevice communicationsDeviceUDP;
		private CMS.Devices.TCPCommunicationsDevice communicationsDeviceTCP;
		
		protected CMS.Base.IDemultiplexer demultiplexer;
		protected CMS.Base.IReadableLogger logger;
		protected CMS.Base.ISender sender, multicastingSender;

		protected abstract void receiveCallback(CMS.Base.IAddress source, CMS.Base.IMessage message);

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) QS.ReservedObjectID.HostManagementObject;
			}
		}

		#endregion
	}
*/
}
