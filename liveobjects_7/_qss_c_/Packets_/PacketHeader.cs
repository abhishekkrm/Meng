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

namespace QS._qss_c_.Packets_
{
/*
    public class PacketHeader
    {
        public PacketHeader(double time, IPAddress sourceip, int sourceport, IPAddress destinationip, int destinationport, byte[] data)
        {
            this.time = time;
            this.source = new QS.Fx.Network.NetworkAddress(sourceip, sourceport);
            this.destination = new QS.Fx.Network.NetworkAddress(destinationip, destinationport);

            if (data.Length < 2 * sizeof(int))
                throw new Exception("Packet too short.");

            unsafe
            {
                fixed (byte* pdata = data)
                {
                    this.streamid = (uint)(*((int*)pdata));
                    this.seqno = (uint)(*((int*)(pdata + sizeof(int))));
                }
            }

            QS.CMS.Base3.Root.Decode(source.HostIPAddress, new ArraySegment<byte>(data, , data.Length - 2 * sizeof(int)),
                out this.sender, out this.channel, out this.receivedObject);

            uint responsePortNo, headerSize;
            ushort classID;
            QS.CMS.Base3.Incarnation incarnation;

            fixed (byte* packetptr = data)
            {
                byte* headerptr = packetptr + 2 * sizeof(int);
                responsePortNo = *((uint*) headerptr);                
                this.channel = *((uint*) (headerptr + sizeof(uint)));
                this.classID = *((ushort*)(headerptr + 2 * sizeof(uint)));
                headerSize = *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort)));
                incarnation = new QS.CMS.Base3.Incarnation(*((uint*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))));
            }

            ArraySegment<byte> header = new WritableArraySegment<byte>(packet.Array, 
                packet.Offset + 4 * sizeof(uint) + sizeof(ushort), (int) headerSize);

            WritableArraySegment<byte> data = new WritableArraySegment<byte>(packet.Array,
                (int)(packet.Offset + (4 * sizeof(uint) + sizeof(ushort) + headerSize)), 
                (int)(packet.Count - (4 * sizeof(uint) + sizeof(ushort) + headerSize)));

            receivedObject.DeserializeFrom(ref header, ref data);

            senderAddress = new InstanceID(
                new QS.Fx.Network.NetworkAddress(interfaceAddress, (int) responsePortNo), incarnation);

        }

        private double time;
        private QS.Fx.Network.NetworkAddress source, destination;
        private uint streamid, seqno, channel;
        private QS._core_c_.Base3.InstanceID sender;
        private ushort classid;
        private long position;

        #region Accessors

        public double Time
        {
            get { return time; }
        }

        public QS.Fx.Network.NetworkAddress Source
        {
            get { return source; }
        }

        public QS.Fx.Network.NetworkAddress Destination
        {
            get { return destination; }
        }

        public uint Stream
        {
            get { return streamid; }
        }

        public uint SequenceNo
        {
            get { return seqno; }
        }

        public QS._core_c_.Base3.InstanceID Sender
        {
            get { return sender; }
        }

        public uint Channel
        {
            get { return channel; }
        }

        public long Position
        {
            get { return position; }
        }

        #endregion
    }
*/ 
}
