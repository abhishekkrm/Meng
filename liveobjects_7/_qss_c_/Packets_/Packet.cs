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
    public sealed class Packet : IPacket
    {
        public Packet(
            double time, QS.Fx.Network.NetworkAddress source, QS.Fx.Network.NetworkAddress destination, uint streamid, 
            uint seqno, uint channel, QS._core_c_.Base3.InstanceID sender, QS.Fx.Serialization.ISerializable receivedObject)
        {
            this.time = time;
            this.source = source;
            this.destination = destination;
            this.streamid = streamid;
            this.seqno = seqno;
            this.channel = channel;
            this.sender = sender;
            this.receivedObject = receivedObject;
        }

        private double time;
        private QS.Fx.Network.NetworkAddress source, destination;
        private uint streamid, seqno, channel;
        private QS._core_c_.Base3.InstanceID sender;
        private QS.Fx.Serialization.ISerializable receivedObject;

        #region Accessors

        double IPacket.Time
        {
            get { return time; }
            set { time = value; }
        }

        QS.Fx.Network.NetworkAddress IPacket.Source
        {
            get { return source; }
            set { source = value; }
        }

        QS.Fx.Network.NetworkAddress IPacket.Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        uint IPacket.Stream
        {
            get { return streamid; }
            set { streamid = value; }
        }

        uint IPacket.SequenceNo
        {
            get { return seqno; }
            set { seqno = value; }
        }

        QS._core_c_.Base3.InstanceID IPacket.Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        uint IPacket.Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        QS.Fx.Serialization.ISerializable IPacket.Object
        {
            get { return receivedObject; }
            set { receivedObject = value; }
        }

        #endregion
    }
}
