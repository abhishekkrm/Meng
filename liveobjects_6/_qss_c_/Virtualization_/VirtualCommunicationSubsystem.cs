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

// #define DEBUG_VirtualCommunicationSubsystem

using System;
using System.Net;
using System.Threading;

namespace QS._qss_c_.Virtualization_
{
	/// <summary>
	/// Summary description for VirtualCommunicationSubsystem.
	/// </summary>
	public class VirtualCommunicationSubsystem : QS.Fx.Inspection.Inspectable, ICommunicationSubsystem, QS._qss_c_.Devices_2_.ICommunicationsDevice
	{
		public VirtualCommunicationSubsystem(Virtualization_.INetwork[] networks, QS._core_c_.Base.IReadableLogger logger)
		{
			this.logger = logger;
			this.networkClients = new QS._core_c_.Collections.Hashtable((uint) networks.Length);

            localAddresses = new IPAddress[networks.Length];
            int address_ind = 0;

            localMTU = uint.MaxValue;
			foreach (Virtualization_.INetwork network in networks)
			{
				NetworkClient networkClient = this.createClient(network);
				networkClients[networkClient.NIC.Address] = networkClient;

				if (network.MTU < localMTU)
					localMTU = network.MTU;

                localAddresses[address_ind++] = networkClient.NIC.Address;
            }

            compatibilityWrapper = new QS._qss_c_.Devices_3_.CompatibilityNetworkWrapper(localAddresses, this);
            newWrapper = new Devices_4_.CompatibilityWrapper(compatibilityWrapper);
        }

        public void ReleaseResources()
        {
            foreach (NetworkClient networkClient in networkClients.Values)
                networkClient.ReleaseResources();
        }

		protected virtual NetworkClient createClient(Virtualization_.INetwork network)
		{
			return new NetworkClient(network, this);
		}

		protected QS._core_c_.Base.IReadableLogger logger;

		private QS._core_c_.Collections.IDictionary networkClients;
		private uint localMTU;
        private Devices_3_.CompatibilityNetworkWrapper compatibilityWrapper;
        private IPAddress[] localAddresses;
        private Devices_4_.CompatibilityWrapper newWrapper;

		#region NetworkClient Members

		protected class NetworkClient : IDisposable
		{
			private const uint defaultAnticipatedNumberOfListeners = 10;

			public NetworkClient(Virtualization_.INetwork network, VirtualCommunicationSubsystem encapsulatingVCS)
			{
				this.encapsulatingVCS = encapsulatingVCS;
				this.networkInterface = network.connect(new Virtualization_.PacketArrivedCallback(this.packetArrivedCallback));
				this.listeners = new Collections_1_.LinkableHashSet(defaultAnticipatedNumberOfListeners);
			}

			protected VirtualCommunicationSubsystem encapsulatingVCS;

			private Virtualization_.INetworkInterface networkInterface;
			private Collections_1_.ILinkableHashSet listeners;

            public void ReleaseResources()
            {
                System.Collections.Generic.List<Listener> toremove = new System.Collections.Generic.List<Listener>();
                lock (listeners)
                {
                    foreach (Listener listener in listeners.Elements)
                        toremove.Add(listener);
                }

                foreach (Listener listener in toremove)
                {
                    try
                    {
                        listener.shutdown();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

			private class Listener : Collections_1_.GenericLinkable, Devices_2_.IListener
			{
				public Listener(QS.Fx.Network.NetworkAddress receiverAddress, Devices_2_.OnReceiveCallback receiveCallback, NetworkClient encapsulatingNetworkClient)
				{
					this.receiverAddress = receiverAddress;
					this.receiveCallback = receiveCallback;
					this.encapsulatingNetworkClient = encapsulatingNetworkClient;
				}

				private QS.Fx.Network.NetworkAddress receiverAddress;
				private Devices_2_.OnReceiveCallback receiveCallback;
				private NetworkClient encapsulatingNetworkClient;

				public override int GetHashCode()
				{
					return receiverAddress.GetHashCode();
				}

				public override bool Equals(object obj)
				{
					return receiverAddress.Equals(obj);
				}

				public void dispatch(QS.Fx.Network.NetworkAddress sourceAddress, QS._core_c_.Base2.IBlockOfData blockOfData)
				{
#if DEBUG_VirtualCommunicationSubsystem
					encapsulatingNetworkClient.encapsulatingVCS.logger.Log(null, 
						"Listener(" + receiverAddress.ToString() + "): dispatch from " + sourceAddress.ToString() + " packet with " + 
						blockOfData.Size.ToString() + " bytes");
#endif

					this.receiveCallback(sourceAddress, this.receiverAddress, blockOfData);
				}

				#region IListener Members

				public QS.Fx.Network.NetworkAddress Address
				{
					get
					{
						return this.receiverAddress;
					}
				}

				public void shutdown()
				{
					encapsulatingNetworkClient.unregister(this.receiverAddress);
				}

				#endregion
			}

            private void unregister(QS.Fx.Network.NetworkAddress receivingAddress)
            {
                lock (listeners)
                {
                    if (Devices_2_.UDPReceiver.ClassOfAddress(receivingAddress.HostIPAddress).Equals(Devices_2_.UDPReceiver.Class.MULTICAST))
                        networkInterface.leave(receivingAddress.HostIPAddress);

                    listeners.remove(receivingAddress);
                }
			}

			public void sendto(IPAddress destinationAddress, WrappedData wrappedData)
			{
				networkInterface.sendto(destinationAddress, wrappedData);
			}

            public virtual IAsyncResult BeginSendTo(IPAddress destinationAddress, WrappedData wrappedData,
                AsyncCallback callback, object state)
            {
                throw new NotSupportedException();
            }

			public Devices_2_.IListener listenAt(QS.Fx.Network.NetworkAddress receivingAddress, Devices_2_.OnReceiveCallback receiveCallback)
			{
				Listener listener = null;
				lock (listeners)
				{
					if (!listeners.contains(receivingAddress))
					{
						listener = new Listener(receivingAddress, receiveCallback, this);
						listeners.insert(listener);

                        if (Devices_2_.UDPReceiver.ClassOfAddress(receivingAddress.HostIPAddress).Equals(Devices_2_.UDPReceiver.Class.MULTICAST))
                            networkInterface.join(receivingAddress.HostIPAddress);
					}
                    else
                        throw new Exception("Already listening at this address.");
				}

				return listener;
			}

			protected virtual void packetArrivedCallback(IPAddress sourceIPAddress, IPAddress destinationIPAddress, QS._core_c_.Base2.IBlockOfData blockOfData)
			{
// #if DEBUG_VirtualCommunicationSubsystem
//				encapsulatingVCS.logger.Log(null, 
//					"VirtualCommunicationSubsystem: packetArrived, from " + sourceIPAddress.ToString() + " to " + 
//					destinationIPAddress.ToString() + " with " + blockOfData.Size.ToString() + " bytes");
// #endif

				WrappedData wrappedData = new WrappedData(blockOfData);

				QS.Fx.Network.NetworkAddress sourceAddress = new QS.Fx.Network.NetworkAddress(sourceIPAddress, (int) wrappedData.SourcePort);
				QS.Fx.Network.NetworkAddress destinationAddress = new QS.Fx.Network.NetworkAddress(destinationIPAddress, (int) wrappedData.DestinationPort);
				QS._core_c_.Base2.IBlockOfData data = wrappedData.Data;

#if DEBUG_VirtualCommunicationSubsystem
				encapsulatingVCS.logger.Log(null, 
					"VirtualCommunicationSubsystem: packetArrived, from " + sourceAddress.ToString() + " to " + 
					destinationAddress.ToString() + " with " + data.SizeOfData.ToString() + " bytes");
#endif

				Listener listener = null;
				lock (listeners)
				{
					listener = (Listener) listeners.lookup(destinationAddress);
				}

				if (listener != null)
					listener.dispatch(sourceAddress, data);
			}

			public Virtualization_.INetworkInterface NIC
			{
				get
				{
					return this.networkInterface;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				networkInterface.disconnect();
			}

			#endregion
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			lock (networkClients)
			{
				foreach (IPAddress localAddress in networkClients.Keys)
				{
					try
					{
						((NetworkClient) networkClients.remove(localAddress).Value).Dispose();
					}
					catch (Exception)
					{
					}
				}
			}
		}

		#endregion

		#region ICommunicationSubsystem Members

		public System.Net.IPAddress[] NICs
		{
			get
			{
/*
				object[] temp = networkClients.Keys;
				System.Net.IPAddress[] addresses = new System.Net.IPAddress[temp.Length];
				for (uint ind = 0; ind < temp.Length; ind++)
					addresses[ind] = (System.Net.IPAddress) temp[ind];
				return addresses;
*/
                return localAddresses;
            }
		}

        public Devices_3_.INetwork Network
        {
            get
            {
                return compatibilityWrapper;
            }
        }

        public Devices_4_.INetwork NetworkConnections
        {
            get { return newWrapper; }
        }

        public QS._qss_c_.Devices_2_.ICommunicationsDevice UDPDevice
		{
			get
			{
				return this;
			}
		}

        Devices_7_.IConnections Virtualization_.ICommunicationSubsystem.Connections7
        {
            get { throw new NotImplementedException(); }
        }

		#endregion

		#region Wrapped Data

		[QS.Fx.Serialization.ClassID(QS.ClassID.VirtualCommunicationSubsystem_WrappedData)]
		protected class WrappedData : QS._core_c_.Base2.IBase2Serializable
		{
			public WrappedData()
			{
			}

			public WrappedData(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				this.load(blockOfData);
			}

			public WrappedData(uint sourcePort, uint destinationPort, QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				this.sourcePort = sourcePort;
				this.destinationPort = destinationPort;
				this.blockOfData = blockOfData;
			}

			private uint sourcePort, destinationPort;
			private QS._core_c_.Base2.IBlockOfData blockOfData;

			public uint SourcePort
			{
				get
				{
					return sourcePort;
				}
			}

			public uint DestinationPort
			{
				get
				{
					return destinationPort;
				}
			}

			public QS._core_c_.Base2.IBlockOfData Data
			{
				get
				{
					return blockOfData;
				}
			}

			private static uint sizeOfUInt32 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));

			#region ISerializable Members

			public QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.VirtualCommunicationSubsystem_WrappedData;
				}
			}

			public uint Size
			{
				get
				{					
					return blockOfData.Size + 2 * sizeOfUInt32;
				}
			}

			public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(sourcePort), 0, blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer, (int) sizeOfUInt32);
				blockOfData.consume(sizeOfUInt32);
				Buffer.BlockCopy(BitConverter.GetBytes(destinationPort), 0, blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer, (int) sizeOfUInt32);
				blockOfData.consume(sizeOfUInt32);
				this.blockOfData.save(blockOfData);
			}

			public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				this.sourcePort = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(sizeOfUInt32);
				this.destinationPort = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(sizeOfUInt32);

				//a a little hack
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
				this.blockOfData = blockOfData;
			}

			#endregion
		}

		#endregion

		#region ISendingDevice Members

		public uint MTU
		{
			get
			{
				return localMTU;
			}
		}

		public void sendto(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			NetworkClient networkClient = null;
			lock (networkClients)
			{
				networkClient = (NetworkClient)networkClients[sourceAddress.HostIPAddress];
				if (networkClient != null)
					Monitor.Enter(networkClient);
			}

			if (networkClient != null)
			{
				try
				{
					WrappedData wrappedData = new WrappedData((uint)sourceAddress.PortNumber, (uint)destinationAddress.PortNumber, blockOfData);
					networkClient.sendto(destinationAddress.HostIPAddress, wrappedData);
				}
				catch (Exception exc)
				{
					logger.Log(this, "__SendTo(" + QS._core_c_.Helpers.ToString.Object(sourceAddress) + ", " + 
						QS._core_c_.Helpers.ToString.Object(destinationAddress) + ", " + 
						((blockOfData != null) ? (blockOfData.Size.ToString() + " bytes") : "(null)") + "), " + exc.ToString());
				}

				Monitor.Exit(networkClient);
			}
		}

        public IAsyncResult BeginSendTo(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            QS._core_c_.Base2.IBlockOfData blockOfData, AsyncCallback callback, object state)
        {
            NetworkClient networkClient = null;
            lock (networkClients)
            {
                networkClient = (NetworkClient)networkClients[sourceAddress.HostIPAddress];
                if (networkClient != null)
                    Monitor.Enter(networkClient);
            }

            if (networkClient != null)
            {
                IAsyncResult result;
                try
                {
                    WrappedData wrappedData = new WrappedData((uint)sourceAddress.PortNumber, (uint)destinationAddress.PortNumber, blockOfData);
                    result = networkClient.BeginSendTo(destinationAddress.HostIPAddress, wrappedData, callback, state);
                }
                catch (Exception exc)
                {
                    logger.Log(this, "__SendTo(" + QS._core_c_.Helpers.ToString.Object(sourceAddress) + ", " +
                        QS._core_c_.Helpers.ToString.Object(destinationAddress) + ", " +
                        ((blockOfData != null) ? (blockOfData.Size.ToString() + " bytes") : "(null)") + "), " + exc.ToString());
                    result = null;
                }

                Monitor.Exit(networkClient);

                return result;
            }
            else
                throw new Exception("Wrong address.");
        }

		#endregion

		#region IReceivingDevice Members

		public QS._qss_c_.Devices_2_.IListener listenAt(System.Net.IPAddress localAddress, QS.Fx.Network.NetworkAddress receivingAddress, 
			QS._qss_c_.Devices_2_.OnReceiveCallback receiveCallback)
		{
#if DEBUG_VirtualCommunicationSubsystem
			logger.Log(null, "listenAt(" + localAddress.ToString() + ", " + receivingAddress.ToString());
#endif

			if (receivingAddress.HostIPAddress.Equals(IPAddress.Any) || receivingAddress.HostIPAddress.Equals(IPAddress.None))
				receivingAddress.HostIPAddress = localAddress;

			NetworkClient networkClient = null;
			lock (networkClients)
			{
				networkClient = (NetworkClient) networkClients[localAddress];
				Monitor.Enter(networkClient);
			}

			Devices_2_.IListener listener = networkClient.listenAt(receivingAddress, receiveCallback);

			Monitor.Exit(networkClient);

			return listener;
		}

		public void shutdown()
		{
		}

		#endregion

        #region Devices6.INetwork Members

        QS._qss_c_.Base6_.ICollectionOf<IPAddress, QS._qss_c_.Devices_6_.INetworkConnection> QS._qss_c_.Devices_6_.INetwork.Connections
        {
            get { throw new NotSupportedException(); }
        }

        Devices_6_.ReceiveCallback Devices_6_.INetwork.ReceiveCallback
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}
