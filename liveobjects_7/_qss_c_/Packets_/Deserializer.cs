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

namespace QS._qss_c_.Packets_
{
    public static class Deserializer
    {
        public enum Mode
        {
            Deserialize, Recognize
        }

        public unsafe static IPacket Deserialize(
            double time, IPAddress sourceip, int sourceport, IPAddress destinationip, int destinationport, byte[] data, Mode mode)
        {
            QS.Fx.Network.NetworkAddress source, destination;
            uint streamid, seqno, channel;
            QS._core_c_.Base3.InstanceID sender;
            QS.Fx.Serialization.ISerializable receivedObject;

            source = new QS.Fx.Network.NetworkAddress(sourceip, sourceport);
            destination = new QS.Fx.Network.NetworkAddress(destinationip, destinationport);

            if (data.Length < 2 * sizeof(int))
                throw new Exception("Packet too short.");

            unsafe
            {
                fixed (byte* pdata = data)
                {
                    streamid = (uint)(*((int*)pdata));
                    seqno = (uint)(*((int*) (pdata + sizeof(int))));
                }
            }

            switch (mode)
            {
                case Mode.Deserialize:
                    {
                        QS._qss_c_.Base3_.Root.Decode(
                            source.HostIPAddress, new QS.Fx.Base.Block(data, (uint) (2 * sizeof(int)), (uint) (data.Length - 2 * sizeof(int))),
                            out sender, out channel, out receivedObject);
                    }
                    break;

                case Mode.Recognize:
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(data, 2 * sizeof(int), data.Length - 2 * sizeof(int));
                        uint portno, incarnation, header_size;
                        ushort class_id;

                        fixed (byte* segmentptr = segment.Array)
                        {
                            byte* headerptr = segmentptr + segment.Offset;
                            portno = *((uint*)headerptr);
                            channel = *((uint*)(headerptr + sizeof(uint)));
                            class_id = *((ushort*)(headerptr + 2 * sizeof(uint)));
                            header_size = *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort)));
                            incarnation = (*((uint*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))));
                        }

                        sender = new QS._core_c_.Base3.InstanceID(new QS.Fx.Network.NetworkAddress(source.HostIPAddress, (int)portno), incarnation);

                        int segment_offset = 4 * sizeof(uint) + sizeof(ushort);

                        Base3_.WritableArraySegment<byte> header_segment =
                            new Base3_.WritableArraySegment<byte>(
                            segment.Array, segment.Offset + segment_offset, Math.Min((int) header_size, segment.Count - segment_offset));

                        int data_offset = segment_offset + (int) header_size;

                        Base3_.WritableArraySegment<byte> data_segment;
                        if (data_offset < segment.Count)
                            data_segment = new Base3_.WritableArraySegment<byte>(
                                segment.Array, segment.Offset + data_offset, segment.Count - data_offset);
                        else
                            data_segment = new Base3_.WritableArraySegment<byte>(segment.Array, 0, 0);

                        Recognizer.Recognize(class_id, header_segment, data_segment, out receivedObject);
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }

            return new Packet(time, source, destination, streamid, seqno, channel, sender, receivedObject);
        }
    }
}
