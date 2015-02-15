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

// #define DEBUG_SimulatedNetwork

// #define DEBUG_EnableLoggingOfPacketTransmissions

using System;
using System.Threading;
using System.Net;

namespace QS._qss_c_.Virtualization_
{
	/// <summary>
	/// Summary description for SimulatedNetwork.
	/// </summary>
	public class VirtualNetwork : Virtualization_.INetwork
	{
		private const uint defaultAnticipatedNumberOfNetworkClients = 100;
		private const uint defaultAnticipatedNumberOfGroups = 20;

		private const double defaultPacketLossRate = 0.0001; // 0.01;
		private static Random_.IRandomGenerator defaultSourceOfLatency = new Random_.Uniform(0.0001, 0.0002);

		public VirtualNetwork(Base1_.Subnet subnet, uint networkMTU, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger)
			: this(subnet, networkMTU, alarmClock, logger, defaultPacketLossRate, defaultSourceOfLatency)
		{
		}

		public Base1_.Subnet Subnet
		{
			get
			{
				return this.subnet;
			}
		}

		public VirtualNetwork(Base1_.Subnet subnet, uint networkMTU, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, 
			double packetLossRate, Random_.IRandomGenerator sourceOfLatency)
		{
			this.alarmClock = alarmClock;
			this.logger = logger;
			this.networkMTU = networkMTU;
			this.clients = new Collections_1_.LinkableHashSet(defaultAnticipatedNumberOfNetworkClients);
			this.subnet = subnet;
			this.groups = new Collections_1_.LinkableHashSet(defaultAnticipatedNumberOfGroups);
            this.senderPacketLossRate = 0;
			this.receiverPacketLossRate = packetLossRate;
			this.sourceOfLatency = sourceOfLatency;
			this.random = new System.Random();
		}

		private QS.Fx.Logging.ILogger logger;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private uint networkMTU;
		private Collections_1_.ILinkableHashSet clients, groups;
		private Base1_.Subnet subnet;
		private double senderPacketLossRate, receiverPacketLossRate;
		private Random_.IRandomGenerator sourceOfLatency;
		private System.Random random;

		private bool randomAllocation = false;

#if DEBUG_EnableLoggingOfPacketTransmissions
        private System.Collections.Generic.List<TMS.Monitoring.PacketTransmission> packetTransmissions =
            new System.Collections.Generic.List<TMS.Monitoring.PacketTransmission>();

        public System.Collections.Generic.List<TMS.Monitoring.PacketTransmission> PacketTransmissions
        {
            get { return packetTransmissions; }
        }
#endif

		public bool RandomAllocation
		{
			get { return randomAllocation; }
			set { randomAllocation = value; }
		}

		private const uint defaultAnticipatedNumberOfMembers = 10;

		#region Group

		private class Group : Collections_1_.GenericLinkable
		{
			public Group(IPAddress groupAddress)
			{
				this.groupAddress = groupAddress;
				this.memberAddresses = new Collections_1_.HashSet(defaultAnticipatedNumberOfMembers);
			}

			private IPAddress groupAddress;
			private Collections_1_.ISet memberAddresses;
			
			public void add(IPAddress memberAddress)
			{
				if (!memberAddresses.contains(memberAddress))
					memberAddresses.insert(memberAddress);
			}

			public void remove(IPAddress memberAddress)
			{
				memberAddresses.remove(memberAddress);
			}

			public IPAddress[] MemberAddresses
			{
				get
				{
					return (IPAddress[]) memberAddresses.ToArray(typeof(IPAddress));
				}
			}

			public override object Contents
			{
				get
				{
					return groupAddress;
				}
			}
		}

		#endregion

		#region Client

		private class Client : Collections_1_.GenericLinkable, Virtualization_.INetworkInterface
		{
			public Client(IPAddress localAddress, VirtualNetwork encapsulatingSimulatedNetwork, 
				QS._qss_c_.Virtualization_.PacketArrivedCallback packetArrivedCallback)
			{
				this.localAddress = localAddress;
				this.encapsulatingSimulatedNetwork = encapsulatingSimulatedNetwork;
				this.packetArrivedCallback = packetArrivedCallback;
			}

			private IPAddress localAddress;
			private QS._qss_c_.Virtualization_.PacketArrivedCallback packetArrivedCallback;
			private VirtualNetwork encapsulatingSimulatedNetwork;

			public override object Contents
			{
				get
				{
					return localAddress;
				}
			}

			#region INetworkInterface Members

			public System.Net.IPAddress Address
			{
				get
				{
					return localAddress;
				}
			}

			public Virtualization_.INetwork Network
			{
				get
				{
					return encapsulatingSimulatedNetwork;
				}
			}

			public void sendto(System.Net.IPAddress anotherAddress, QS._core_c_.Base2.IBase2Serializable packet)
			{
				encapsulatingSimulatedNetwork.send(this.localAddress, anotherAddress, packet);
			}

			public void receive(System.Net.IPAddress sourceAddress, System.Net.IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet)
			{
				this.packetArrivedCallback(sourceAddress, destinationAddress, packet);
			}

			public void disconnect()
			{
				encapsulatingSimulatedNetwork.disconnect(this.localAddress);
			}

			public void join(IPAddress groupAddress)
			{
				encapsulatingSimulatedNetwork.addToGroup(groupAddress, this.localAddress);			
			}

			public void leave(IPAddress groupAddress)
			{
				encapsulatingSimulatedNetwork.removeFromGroup(groupAddress, this.localAddress);			
			}

			#endregion
		}

		#endregion

		private void send(IPAddress sourceAddress, IPAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable packet)
		{
#if DEBUG_SimulatedNetwork
			logger.Log(null, "SimulatedNetwork: send " + sourceAddress.ToString() + " to " + destinationAddress.ToString() + 
				" " + packet.Size.ToString() + " bytes");
#endif

			QS._core_c_.Base2.BlockOfData blockOfData = new QS._core_c_.Base2.BlockOfData(new byte[packet.Size]); 
			packet.save(blockOfData);
			
			// some hack
			blockOfData.resetCursor();

			this.transmit(sourceAddress, destinationAddress, blockOfData, true);
		}

		#region Shipment

		private class Shipment
		{
			public Shipment(IPAddress sourceAddress, IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet, bool isOutgoing)
			{
				this.sourceAddress = sourceAddress;
				this.destinationAddress = destinationAddress;
				this.packet = packet;
				this.isOutgoing = isOutgoing;
			}

			public IPAddress sourceAddress, destinationAddress;
			public QS._core_c_.Base2.IBlockOfData packet;
			public bool isOutgoing;

			public override string ToString()
			{
				return "<" + sourceAddress.ToString() + " --> " + destinationAddress.ToString() + (isOutgoing ? ", out, " : ", ") +
					packet.Size.ToString() + " bytes>";
			}
		}

		#endregion

		protected virtual void transmit(IPAddress sourceAddress, IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet, bool isOutgoing)
		{
            if (random.NextDouble() > senderPacketLossRate)
            {
                double delay = sourceOfLatency.Get;

#if DEBUG_SimulatedNetwork
                logger.Log(null, "delay = " + delay.ToString());
#endif
                alarmClock.Schedule(delay, new QS.Fx.Clock.AlarmCallback(this.transmitAlarmCallback),
                    new Shipment(sourceAddress, destinationAddress, packet, isOutgoing));
            }
            else
            {
#if DEBUG_SimulatedNetwork
                logger.Log(null, "Packet from " + sourceAddress.ToString() + " to " + destinationAddress.ToString() + " has been lost in transit.");
#endif

#if DEBUG_EnableLoggingOfPacketTransmissions
                packetTransmissions.Add(
                    new QS.TMS.Monitoring.PacketTransmission(new QS.Fx.Network.NetworkAddress(sourceAddress, 0),
                    new QS.Fx.Network.NetworkAddress(destinationAddress, 0), new QS.Fx.Network.NetworkAddress(IPAddress.None, 0),
                    false, "dropped at the sender", 0, 0, ""));
#endif
            }
        }

		private void transmitAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
		{
			Shipment shipment = (Shipment) alarmRef.Context;
			this.dispatch(shipment.sourceAddress, shipment.destinationAddress, shipment.packet, shipment.isOutgoing);
		}

		protected void dispatch(IPAddress sourceAddress, IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet, bool isOutgoing)
		{
			Devices_2_.UDPReceiver.Class classOfAddress = Devices_2_.UDPReceiver.ClassOfAddress(destinationAddress);

			switch (classOfAddress)
			{
				case Devices_2_.UDPReceiver.Class.UNICAST:
				{
					if (subnet.contains(destinationAddress))
					{
						this.dispatchToSingleAddress(sourceAddress, destinationAddress, destinationAddress, packet);
					}
					else
					{
						if (isOutgoing)
							this.sendOut(sourceAddress, classOfAddress, destinationAddress, packet);
						else
							throw new Exception("incoming packet undeliverable, no such host on this network");
					}
				}
				break;

				case Devices_2_.UDPReceiver.Class.BROADCAST:
				case Devices_2_.UDPReceiver.Class.MULTICAST:
				{
					dispatchToGroupAddress(sourceAddress, destinationAddress, packet);
					if (isOutgoing)
						this.sendOut(sourceAddress, classOfAddress, destinationAddress, packet);
				}
				break;
			}
		}

		private void dispatchToSingleAddress(IPAddress sourceAddress, IPAddress clientAddress, IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet)
		{
            if (random.NextDouble() > receiverPacketLossRate)
            {
                Monitor.Enter(clients);

                Client destinationClient = (Client)clients.lookup(clientAddress);

                if (destinationClient != null)
                {
#if DEBUG_EnableLoggingOfPacketTransmissions
                    packetTransmissions.Add(
                        new QS.TMS.Monitoring.PacketTransmission(new QS.Fx.Network.NetworkAddress(sourceAddress, 0),
                        new QS.Fx.Network.NetworkAddress(destinationAddress, 0),
                        new QS.Fx.Network.NetworkAddress(clientAddress, 0),
                        true, null, 0, 0, ""));
#endif

                    lock (destinationClient)
                    {
                        Monitor.Exit(clients);

                        destinationClient.receive(sourceAddress, destinationAddress, packet);
                    }
                }
                else
                {
                    Monitor.Exit(clients);

                    // throw new Exception("cannot deliver, there is no node with address " + destinationAddress.ToString() + " on this network");

#if DEBUG_EnableLoggingOfPacketTransmissions
                    packetTransmissions.Add(
                        new QS.TMS.Monitoring.PacketTransmission(new QS.Fx.Network.NetworkAddress(sourceAddress, 0),
                        new QS.Fx.Network.NetworkAddress(destinationAddress, 0),
                        new QS.Fx.Network.NetworkAddress(clientAddress, 0),
                        false, "no such address", 0, 0, ""));
#endif
                }
            }
            else
            {
#if DEBUG_SimulatedNetwork
                logger.Log(null, "Packet from " + sourceAddress.ToString() + " to " + destinationAddress.ToString() + 
                    " dropped at the receiver " + clientAddress.ToString() + ".");
#endif

#if DEBUG_EnableLoggingOfPacketTransmissions
                packetTransmissions.Add(
                    new QS.TMS.Monitoring.PacketTransmission(new QS.Fx.Network.NetworkAddress(sourceAddress, 0),
                    new QS.Fx.Network.NetworkAddress(destinationAddress, 0), 
                    new QS.Fx.Network.NetworkAddress(clientAddress, 0),
                    false, "dropped at the receiver", 0, 0, ""));
#endif
            }
		}

		private void dispatchToGroupAddress(IPAddress sourceAddress, IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet)
		{
			Monitor.Enter(groups);			
			Group group = (Group) groups.lookup(destinationAddress);
			if (group != null)
			{
				lock (group)
				{
					Monitor.Exit(groups);

					foreach (IPAddress address in group.MemberAddresses)
						this.dispatchToSingleAddress(sourceAddress, address, destinationAddress, packet.StandaloneCopy); // could probably do shallow copy as well
				}
			}
			else
				Monitor.Exit(groups);
		}

		private void addToGroup(IPAddress groupAddress, IPAddress memberAddress)
		{
			lock (groups)
			{
				Group group = (Group) groups.lookup(groupAddress);
				if (group == null)
				{
					group = new Group(groupAddress);
					groups.insert(group);
				}

				group.add(memberAddress);
			}
		}

		private void removeFromGroup(IPAddress groupAddress, IPAddress memberAddress)
		{
			lock (groups)
			{
				Group group = (Group) groups.lookup(groupAddress);
				if (group != null)
					group.remove(memberAddress);
			}
		}

		protected virtual void sendOut(IPAddress sourceAddress, Devices_2_.UDPReceiver.Class classOfAddress, 
			IPAddress destinationAddress, QS._core_c_.Base2.IBlockOfData packet)
		{
			if (classOfAddress == Devices_2_.UDPReceiver.Class.UNICAST)
				throw new Exception("canot deliver, address " + destinationAddress.ToString() + " is outside of the local subnet");
		}

		#region INetwork Members

		public uint MTU
		{
			get
			{
				return networkMTU;
			}
		}		

		private const uint maximumNumberOfAttempts = 100;
		private const uint maximumOffset = 100000;
		public Virtualization_.INetworkInterface connect(QS._qss_c_.Virtualization_.PacketArrivedCallback packetArrivedCallback)
		{
			Client client = null;
			lock (clients)
			{
				IPAddress candidateAddress = IPAddress.None;
				bool found = false;

				if (randomAllocation)
				{
					for (uint countdown = maximumNumberOfAttempts; !found && countdown > 0; countdown--)
						found = !clients.contains(candidateAddress = subnet.RandomAddress);
				}
				else
				{
					for (uint offset = 0; !found && offset < maximumOffset; offset++)
						found = !clients.contains(candidateAddress = subnet[offset]);
				}

				if (found)
				{
					client = new Client(candidateAddress, this, packetArrivedCallback);
					Monitor.Enter(client);

					clients.insert(client);
				}
			}

			if (client != null)
			{
				// ...................................

				Monitor.Exit(client);
			}
			else
				throw new Exception("could not connect to the network, possibly not enough available addresses left");
			
			return client;
		}

		#endregion

		private void disconnect(IPAddress localAddress)
		{
			lock (clients)
			{
				clients.remove(localAddress);
			}
		}

		public override string ToString()
		{
			return subnet.ToString();
		}
	}
}
