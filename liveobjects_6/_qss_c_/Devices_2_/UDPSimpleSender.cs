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
using System.Diagnostics;

namespace QS._qss_c_.Devices_2_
{
	/// <summary>
	/// Summary description for UDPSender.
	/// </summary>
	public class UDPSimpleSender : ISendingDevice
	{
		private const uint anticipatedNumberOfSourceAddresses	=	  5;
		private const uint anticipatedNumberOfSocketsInAPool	=	200;

		public UDPSimpleSender(QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;
			this.socketPools = new QS._qss_c_.Collections_1_.LinkableHashSet(anticipatedNumberOfSourceAddresses);
		}

		#region Class SocketPool

		private class SocketPool : Collections_1_.GenericLinkable
		{
			public SocketPool(QS.Fx.Network.NetworkAddress sourceAddress)
			{
				this.sourceAddress = sourceAddress;
				this.objectPool = new QS._qss_c_.Components_1_.ObjectPool(anticipatedNumberOfSocketsInAPool,
					new Components_1_.AllocateCallback(this.allocateCallback));
			}			

			public Components_1_.ObjectPool objectPool;
			private QS.Fx.Network.NetworkAddress sourceAddress;

			private object allocateCallback()
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				IPEndPoint endpoint = new IPEndPoint(sourceAddress.HostIPAddress, sourceAddress.PortNumber);
				socket.Bind(endpoint);
				
				return socket;
			}

			public override object Contents
			{
				get
				{
					return sourceAddress;
				}
			}
		}

		#endregion

		private Components_1_.IObjectWrapper acquireSocket(QS.Fx.Network.NetworkAddress sourceAddress)
		{
			Components_1_.IObjectWrapper result;

			lock (this)
			{
				SocketPool socketPool = (SocketPool) socketPools.lookup(sourceAddress);
				if (socketPool == null)
				{
					socketPool = new SocketPool(sourceAddress);
					socketPools.insert(socketPool);
				}

				result = socketPool.objectPool.AllocateObject;
			}

			return result;
		}

		protected QS.Fx.Logging.ILogger logger;
		private Collections_1_.ILinkableHashSet socketPools;

		private void asynchronousSendToCallback(System.IAsyncResult asyncResult)
		{
//			Socket socket = (Socket) asyncResult.AsyncState;
			Components_1_.IObjectWrapper socketWrapper = (Components_1_.IObjectWrapper) asyncResult.AsyncState;
			Socket socket = (Socket) socketWrapper.WrappedObject;

			socket.EndSendTo(asyncResult);

			socketWrapper.ReleaseObject();
		}

		#region ISendingDevice Members 

        public IAsyncResult BeginSendTo(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            QS._core_c_.Base2.IBlockOfData blockOfData, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

		public void sendto(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			try
			{
				Debug.Assert(blockOfData.SizeOfData <= MTU);

//				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//				IPEndPoint endpoint = new IPEndPoint(sourceAddress.HostIPAddress, sourceAddress.PortNumber);
//				socket.Bind(endpoint);

				Components_1_.IObjectWrapper socketWrapper = acquireSocket(sourceAddress);
				Socket socket = (Socket) socketWrapper.WrappedObject;

				switch (UDPReceiver.ClassOfAddress(destinationAddress.HostIPAddress))
				{
					case UDPReceiver.Class.UNICAST:
						break;

					case UDPReceiver.Class.BROADCAST:
						socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
						break;

					case UDPReceiver.Class.MULTICAST:
						socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
						break;
				}

				socket.BeginSendTo(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) blockOfData.SizeOfData, SocketFlags.None, 
					new IPEndPoint(destinationAddress.HostIPAddress, destinationAddress.PortNumber), 
					new AsyncCallback(asynchronousSendToCallback), socketWrapper); // socket);
			}
			catch (Exception exc)
			{
				logger.Log(this, "SendTo(" + sourceAddress.ToString() + " -> " + destinationAddress.ToString() + 
					", size = " + blockOfData.SizeOfData + " bytes) failed : " + exc.ToString());
				throw new Exception("Cannot send data.", exc);
			}
		}

		public uint MTU
		{
			get
			{
				return UDPReceiver.MTU;
			}
		}

		#endregion
	}
}
