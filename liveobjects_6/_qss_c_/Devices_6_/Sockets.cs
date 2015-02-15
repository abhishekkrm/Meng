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

namespace QS._qss_c_.Devices_6_
{
    public static class Sockets
    {
        public static Socket CreateSenderUDPSocket(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress destinationAddress)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endpoint = new IPEndPoint(interfaceAddress, 0);
            socket.Bind(endpoint);

            if (destinationAddress.HostIPAddress.Equals(IPAddress.Broadcast))
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            else
            {
                byte firstbyte = (destinationAddress.HostIPAddress.GetAddressBytes())[0];
                if (firstbyte > 223 && firstbyte < 240)
                {
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
                }
            }

            socket.Connect(new IPEndPoint(destinationAddress.HostIPAddress, destinationAddress.PortNumber));

            return socket;
        }

        public static Socket CreateReceiverUDPSocket(IPAddress interfaceAddress, ref QS.Fx.Network.NetworkAddress listeningAddress)
        {
            if (IPAddress.Any.Equals(listeningAddress.HostIPAddress))
                listeningAddress.HostIPAddress = interfaceAddress;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint sipep = new IPEndPoint(interfaceAddress, listeningAddress.PortNumber);
            
            bool is_broadcast = IPAddress.Broadcast.Equals(listeningAddress);
            byte firstByte = (listeningAddress.HostIPAddress.GetAddressBytes())[0];
            bool is_multicast = (firstByte >= 224 && firstByte <= 239);

            if (is_broadcast || is_multicast)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            else
            {
                if (!interfaceAddress.Equals(listeningAddress.HostIPAddress))
                    throw new Exception("When listening at a unicast address, the listening IP address should be same as IP of the interface.");
            }

            socket.Bind(sipep);

            if (is_multicast)
            {
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                    new MulticastOption(listeningAddress.HostIPAddress, interfaceAddress));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
            }
             
            if (is_broadcast)
            {
                // socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
            }

            if (listeningAddress.PortNumber == 0)
                listeningAddress.PortNumber = ((IPEndPoint)socket.LocalEndPoint).Port;

            return socket;
        }
    }
}
