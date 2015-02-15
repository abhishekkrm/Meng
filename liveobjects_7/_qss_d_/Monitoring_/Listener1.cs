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

namespace QS._qss_d_.Monitoring_
{
    public class Listener1 : IListener
    {
        public Listener1(QS.Fx.Clock.IClock clock, IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress listeningAddress, uint bufferSize)
        {
            this.clock = clock;
            this.interfaceAddress = interfaceAddress;
            this.bufferSize = bufferSize;
            this.listeningAddress = listeningAddress;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(interfaceAddress, listeningAddress.PortNumber));
            byte firstbyte = (listeningAddress.HostIPAddress.GetAddressBytes())[0];
            if (firstbyte >= 224 && firstbyte <= 239)
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                    new MulticastOption(listeningAddress.HostIPAddress, interfaceAddress));

            remoteAddress = new IPEndPoint(IPAddress.Any, 0);
            buffer = new byte[bufferSize];
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteAddress, 
                new AsyncCallback(this.ReceiveCallback), null);
        }

        private QS.Fx.Clock.IClock clock;
        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress listeningAddress;
        private Socket socket;
        private EndPoint remoteAddress;
        private byte[] buffer;
        private uint bufferSize;
        private IList<IPacket> packets = new List<IPacket>();

        #region ReceiveCallback

        private void ReceiveCallback(System.IAsyncResult asyncResult)
        {
            double time = clock.Time;
            int nreceived = socket.EndReceiveFrom(asyncResult, ref remoteAddress);
            IPAddress ipAddress = ((IPEndPoint)remoteAddress).Address;
            int portno = ((IPEndPoint)remoteAddress).Port;

            QS._core_c_.Base3.InstanceID senderAddress;
            uint channel;
            QS.Fx.Serialization.ISerializable receivedObject;
            QS._qss_c_.Base3_.Root.Decode(interfaceAddress, new QS.Fx.Base.Block(buffer, 0, (uint) nreceived), 
                out senderAddress, out channel, out receivedObject);

            Packet packet = new Packet(senderAddress, listeningAddress, channel, time, receivedObject);
            lock (packets)
            {
                packets.Add(packet);
            }

            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteAddress,
                new AsyncCallback(this.ReceiveCallback), null);
        }

        #endregion

        #region IListener Members

        IEnumerable<IPacket> IListener.Received
        {
            get { return packets; }
        }

        #endregion
    }
}
