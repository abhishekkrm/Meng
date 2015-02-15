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
using System.Diagnostics;

namespace QS._qss_c_.Virtualization_
{
	/// <summary>
	/// Summary description for VirtualSubnet.
	/// </summary>
	public class VirtualSubnet
	{
/*
		#region Routing Configuration

		[Serializable]
		public class RoutingConfiguration
		{
			public RoutingConfiguration()
			{
			}

			public void storeAsXML(string path)
			{
				System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(RoutingConfiguration));
				System.IO.TextWriter w = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode);
				s.Serialize(w, this);
				w.Close();		
			}

			public static RoutingConfiguration loadFromXML(string path)
			{
				System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(RoutingConfiguration));
				System.IO.TextReader r = new System.IO.StreamReader(path, System.Text.Encoding.Unicode);
				RoutingConfiguration config = (RoutingConfiguration) s.Deserialize(r);
				r.Close();
				return config;
			}

			public class SubnetAssignment
			{
				public SubnetAssignment()
				{
				}

				public string AsString
				{
					get
					{
						return subnet.ToString() + "/" + localAddress.ToString();
					}

					set
					{
						int separatorpos = value.IndexOf("/");
						subnet = new SubnetC(value.Substring(0, separatorpos));
						localAddress = IPAddress.Parse(value.Substring(separatorpos + 1));
					}
				}

				[System.Xml.Serialization.XmlIgnore] public IPAddress localAddress;
				[System.Xml.Serialization.XmlIgnore] public SubnetC subnet;
			}

			public SubnetAssignment[] subnetAssignments;
		}

		#endregion

		public VirtualSubnet(Devices2.ICommunicationsDevice underlyingCommunicationsDevice, QS.Fx.Network.NetworkAddress localAddress,
			RoutingConfiguration routingConfiguration)
		{
			this.underlyingCommunicationsDevice = underlyingCommunicationsDevice;
			this.localAddress = localAddress;
			this.subnetMappings = new Collections.Hashtable((uint) routingConfiguration.subnetAssignments.Length);

			this.localSubnet = null;
			foreach (RoutingConfiguration.SubnetAssignment subnetAssignment in routingConfiguration.subnetAssignments)
			{
				subnetMappings[subnetAssignment.subnet] = subnetAssignment.localAddress;

				if (subnetAssignment.localAddress.Equals(localAddress.HostIPAddress))
				{
					if (this.localSubnet != null)
						throw new Exception("local address is assigned to more than one subnet");
					this.localSubnet = subnetAssignment.subnet;
				}
			}

			if (this.localSubnet == null)
				throw new Exception("local address is not assigned to any subnet");

			callbacks = new Devices2.OnReceiveCallback[256];
			for (uint ind = 0; ind < 256; ind++)
				callbacks[ind] = null;

			underlyingCommunicationsDevice.listenAt(localAddress, new Devices2.OnReceiveCallback(this.receiveCallback));
		}

		private Devices2.ICommunicationsDevice underlyingCommunicationsDevice;
		private QS.Fx.Network.NetworkAddress localAddress;		
		private SubnetC localSubnet;
		private Collections.IDictionary subnetMappings;
		private Devices2.OnReceiveCallback[] callbacks;
		private static Random random = new Random();

		public void registerClient(Devices2.OnReceiveCallback receiveCallback, ref IPAddress assignedAddress)		
		{
			lock (this)
			{
				uint foundind = 0;
				bool found = false;
				for (uint startind = (uint) random.Next(256), ind = 0; !found && ind < 256; ind++)
				{
					foundind = (startind + ind) % 256;
					found = (foundind != 0) && (foundind != 255) && (callbacks[foundind] == null);
				}

				if (found)
				{
					callbacks[foundind] = receiveCallback;
					assignedAddress = IPAddress.Parse(localSubnet.AsString.Replace("x", foundind.ToString()));
				}
				else
					throw new Exception("too many clients registered");
			}
		}

		public void unregisterClient(IPAddress assignedAddress)
		{
			throw new Exception("not implemented");

			// lock (this)
			// {
			// }
		}

		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBlockOfData blockOfData)
		{
			Packet packet = new Packet(blockOfData);			
			SubnetC destinationSubnet = new SubnetC(packet.DestinationAddress.HostIPAddress);
			if (!destinationSubnet.Equals(localSubnet))
				throw new Exception("packet arrived at a wrong destination");
			byte hostid = (packet.DestinationAddress.HostIPAddress.GetAddressBytes())[3];

			Devices2.OnReceiveCallback callback = null;
			lock (this)
			{
				callback = callbacks[hostid];
			}

			if (callback != null)
			{
				callback(packet.SourceAddress, packet.DestinationAddress, packet.Data);
			}
			else
				throw new Exception("host " + hostid.ToString() + " does not exist on this subnet");
		}

		public uint MTU
		{
			get
			{				
				return underlyingCommunicationsDevice.MTU - 2 * QS.Fx.Network.NetworkAddress.SizeAsBlockOfData;
			}
		}

		#region Packet

		private struct Packet : Base2.IOutgoingData
		{ 
			public Packet(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBlockOfData blockOfData)
			{
				this.sourceAddress = sourceAddress;
				this.destinationAddress = destinationAddress;
				this.blockOfData = blockOfData;
			}

			public Packet(Base2.IBlockOfData blockOfData)
			{
				sourceAddress = new QS.Fx.Network.NetworkAddress(blockOfData);
				blockOfData.consume(QS.Fx.Network.NetworkAddress.SizeAsBlockOfData);
				destinationAddress = new QS.Fx.Network.NetworkAddress(blockOfData);
				blockOfData.consume(QS.Fx.Network.NetworkAddress.SizeAsBlockOfData);
				this.blockOfData = blockOfData;
			}
			
			private QS.Fx.Network.NetworkAddress sourceAddress, destinationAddress;
			private Base2.IBlockOfData blockOfData;

			public QS.Fx.Network.NetworkAddress SourceAddress
			{
				get
				{
					return sourceAddress;
				}
			}

			public QS.Fx.Network.NetworkAddress DestinationAddress
			{
				get
				{
					return destinationAddress;
				}
			}

			public Base2.IBlockOfData Data
			{
				get
				{
					return blockOfData;
				}
			}

			public uint Size
			{
				get
				{					
					return 2 * QS.Fx.Network.NetworkAddress.SizeAsBlockOfData + blockOfData.SizeOfData;
				}
			}

			public void serializeTo(byte[] bufferForData, uint offsetWithinBuffer, uint spaceWithinBuffer)
			{
				sourceAddress.serializeTo(bufferForData, offsetWithinBuffer, spaceWithinBuffer);
				destinationAddress.serializeTo(bufferForData, 
					offsetWithinBuffer + QS.Fx.Network.NetworkAddress.SizeAsBlockOfData, spaceWithinBuffer - QS.Fx.Network.NetworkAddress.SizeAsBlockOfData); 
				Buffer.BlockCopy(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, bufferForData, 
					(int) (offsetWithinBuffer + 2 * QS.Fx.Network.NetworkAddress.SizeAsBlockOfData), (int) blockOfData.SizeOfData);
			}
		}

		#endregion

		public void send(IPAddress sourceIPAddress, QS.Fx.Network.NetworkAddress destinationAddress, QS.CMS.Base2.IBlockOfData blockOfData)
		{
			if (blockOfData.SizeOfData > this.MTU)
				throw new Exception("data too big");

			QS.Fx.Network.NetworkAddress sourceAddress = new QS.Fx.Network.NetworkAddress(sourceIPAddress, 0);
			SubnetC destinationSubnet = new SubnetC(destinationAddress.HostIPAddress);

			if (destinationSubnet.Equals(localSubnet))
			{
				this.receiveCallback(sourceAddress, destinationAddress, blockOfData);
			}
			else
			{
				IPAddress destinationPhysicalIPAddress = null;
				lock (this)
				{
					Collections.IDictionaryEntry dic_en = subnetMappings.lookup(destinationSubnet);
					if (dic_en != null)
						destinationPhysicalIPAddress = (IPAddress) dic_en.Value;
				}

				if (destinationPhysicalIPAddress != null)
				{
					Packet packet = new Packet(sourceAddress, destinationAddress, blockOfData);
					byte[] bytes = new byte[packet.Size];
					packet.serializeTo(bytes, 0, (uint) bytes.Length);
					underlyingCommunicationsDevice.sendto(new QS.Fx.Network.NetworkAddress(destinationPhysicalIPAddress, localAddress.PortNumber), 
						new Base2.BlockOfData(bytes));
				}
			}
		}
*/		
	}
}

/*
			VirtualSubnet.RoutingConfiguration routingConfig = new VirtualSubnet.RoutingConfiguration();
			routingConfig.subnetAssignments = new VirtualSubnet.RoutingConfiguration.SubnetAssignment[44];
			for (uint ind = 0; ind < 44; ind++)
				routingConfig.subnetAssignments[ind] = new VirtualSubnet.RoutingConfiguration.SubnetAssignment();

			routingConfig.subnetAssignments[0].localAddress = IPAddress.Parse("128.84.223.163");
			routingConfig.subnetAssignments[0].subnet = new SubnetC("100.0.1.x");
			routingConfig.subnetAssignments[1].localAddress = IPAddress.Parse("128.84.223.165");
			routingConfig.subnetAssignments[1].subnet = new SubnetC("100.0.2.x");
			routingConfig.subnetAssignments[2].localAddress = IPAddress.Parse("128.84.223.113");
			routingConfig.subnetAssignments[2].subnet = new SubnetC("100.0.3.x");
			routingConfig.subnetAssignments[3].localAddress = IPAddress.Parse("128.84.223.167");
			routingConfig.subnetAssignments[3].subnet = new SubnetC("100.0.4.x");
			routingConfig.subnetAssignments[4].localAddress = IPAddress.Parse("128.84.223.105");
			routingConfig.subnetAssignments[4].subnet = new SubnetC("100.0.5.x");

			for (uint ind = 1; ind < 40; ind++)
			{
				string hostname = "cfs" + ind.ToString("00");

				routingConfig.subnetAssignments[4 + ind].localAddress = Dns.Resolve(hostname).AddressList[0];
				routingConfig.subnetAssignments[4 + ind].subnet = new SubnetC("100.1." + ind.ToString() + ".x");
			}

			routingConfig.storeAsXML("routing-configuration.xml"); 
*/
