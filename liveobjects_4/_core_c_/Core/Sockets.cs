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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace QS._core_c_.Core
{
    public static class Sockets
    {
        public static bool DisableIPMulticastLoopback = true;

        public static Socket CreateSendSocket(Address address, int internalBufferSize)
        {
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint endpoint = new IPEndPoint(address.NIC, 0);
			socket.Bind(endpoint);

            if (internalBufferSize > 0)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, internalBufferSize);

			if (address.IPAddress.Equals(IPAddress.Broadcast))
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			else
			{
				byte first_byte = (address.IPAddress.GetAddressBytes())[0];
				if (first_byte > 223 && first_byte < 240)
				{
                    
					socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, QS.Fx.Object.Runtime4.QsmTTL);
                    
                    if (DisableIPMulticastLoopback)
					    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
				}
			}

			socket.Connect(new IPEndPoint(address.IPAddress, address.PortNumber));

			return socket;
        }
        
        public static Socket CreateReceiveSocket(Address address, bool forcereuse, int internalBufferSize)
        {
			if (address.IPAddress.Equals(IPAddress.Any))
				address.IPAddress = address.NIC;

			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			bool is_broadcast = address.IPAddress.Equals(IPAddress.Broadcast);
			byte first_byte = (address.IPAddress.GetAddressBytes())[0];
			bool is_multicast = (first_byte >= 224 && first_byte <= 239);

			if (is_broadcast || is_multicast || forcereuse)
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			else
			{
				if (!address.NIC.Equals(address.IPAddress))
					throw new Exception(
						"When listening at a unicast address, the listening IP address should be same as IP of the interface.");
			}

            // socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            if (internalBufferSize > 0)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, internalBufferSize);

			socket.Bind(new IPEndPoint(address.NIC, address.PortNumber));

			if (is_multicast)
			{
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
					new MulticastOption(address.IPAddress, address.NIC));
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
			}

//				if (is_broadcast)
//				{
//					socket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::Broadcast, 0);
//				}

			if (address.PortNumber == 0)
				address.PortNumber = ((IPEndPoint)(socket.LocalEndPoint)).Port;

			return socket;
        }
    }
}
