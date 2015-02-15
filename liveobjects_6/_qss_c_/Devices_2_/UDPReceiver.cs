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
	/// Summary description for UDPReceiver.
	/// </summary>
	public class UDPReceiver : AsynchronousSocketReceiver, IListener
	{
		public const uint MTU = 20000;

		public static Class ClassOfAddress(IPAddress address)
		{
			if (IPAddress.Broadcast.Equals(address))
				return Class.BROADCAST;
			else
			{
				byte firstByte = (address.GetAddressBytes())[0];
				return ((firstByte < 224 || firstByte > 239) ? Class.UNICAST : Class.MULTICAST);
			}
		}

		private const uint RECEIVER_BUFFERSIZE = MTU + 100;

		public enum Class
		{
			UNICAST, MULTICAST, BROADCAST
		}

		public static Socket createSocket(IPAddress localAddress, ref QS.Fx.Network.NetworkAddress listeningAtAddress)
		{
			if (IPAddress.Any.Equals(listeningAtAddress.HostIPAddress))
				listeningAtAddress.HostIPAddress = localAddress;

			Class addressClass = ClassOfAddress(listeningAtAddress.HostIPAddress);

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint sipep = new IPEndPoint(localAddress, listeningAtAddress.PortNumber); // IPAddress.Any, ...

			switch (addressClass)
			{
				case Class.MULTICAST:
				case Class.BROADCAST:
				{
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				}
				break;

				default:
					break;
			}

			socket.Bind(sipep);

			switch (addressClass)
			{
				case Class.UNICAST:
					Debug.Assert(localAddress.Equals(listeningAtAddress.HostIPAddress));
					break;

				case Class.MULTICAST:
					{
						socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
							new MulticastOption(listeningAtAddress.HostIPAddress, localAddress));
                        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
                    }
					break;

				case Class.BROADCAST:
					// socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
					break;
			}

			if (listeningAtAddress.PortNumber == 0)
				listeningAtAddress.PortNumber = ((IPEndPoint) socket.LocalEndPoint).Port;

			return socket;
		}

		public UDPReceiver(IPAddress localAddress, QS.Fx.Network.NetworkAddress listeningAtAddress, QS.Fx.Logging.ILogger logger, bool processAsynchronously, 
			OnReceiveCallback receiveCallback) : base(UDPReceiver.createSocket(localAddress, ref listeningAtAddress), 
			RECEIVER_BUFFERSIZE, logger, processAsynchronously)
		{
			this.listeningAtAddress = listeningAtAddress;
			this.receiveCallback = receiveCallback;
		}

		protected override void process(byte[] bufferWithData, uint bytesReceived, IPAddress sourceAddress, uint sourcePort)
		{
			receiveCallback(new QS.Fx.Network.NetworkAddress(sourceAddress, (int) sourcePort), listeningAtAddress, 
				new QS._core_c_.Base2.BlockOfData(bufferWithData, 0, bytesReceived));
		}

		private QS.Fx.Network.NetworkAddress listeningAtAddress;
		private OnReceiveCallback receiveCallback;

		#region IListener Members

		public QS.Fx.Network.NetworkAddress Address
		{
			get
			{
				return listeningAtAddress;
			}
		}

		#endregion
	}
}
