/*

Copyright (c) 2010 Matt Pearson. All rights reserved.

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
using System.Linq;
using System.Text;
using QS.Fx.Base;
using QS.Fx.Serialization;

namespace Generator
{
    /// <summary>
    /// A message that contains a text payload of arbitrary length, plus a timestamp
    /// and ID of the sender.
    /// </summary>
    [ClassID(QS.ClassID.Matt_TimedMessage)]
    [QS.Fx.Reflection.ValueClass("A936CED0B34B85672DEE74F486BBEC8C", "TimedMessage",
        "A text message with timestamp and sender/receiver information")]
    public sealed class TimedMessage : ISerializable
    {
        private const int ID_FIELD_LEN = 10; // always use 10 bytes for machine name

        // serialized as follows:
        // header contains sequence, timestamp, length of fromid, length of payload
        // individual buffers for fromid, payload
        // total size over wire is 30 bytes plus payload size
        public uint Sequence { get; set; }
        public double Timestamp { get; set; }
        public string FromId { get; set; }
        public string Payload { get; set; }

        public TimedMessage() { } // serializer needs this

        public TimedMessage(uint seq, double ts, string f, string p)
        {
            this.Sequence = seq;
            this.Timestamp = ts;
            this.FromId = f;
            this.Payload = p;
        }

        #region ISerializable Members

        public SerializableInfo SerializableInfo
        {
            get { return this.MakeInfo(); }
        }

        private SerializableInfo MakeInfo()
        {
            int headersize = sizeof(uint) * 3 + sizeof(double); // 2 lengths and seq, plus timestamp
            int size = headersize + ID_FIELD_LEN + Encoding.UTF8.GetByteCount(this.Payload);
            int buffers = 2;
            return new SerializableInfo((ushort) QS.ClassID.Matt_TimedMessage, headersize, size, buffers);
        }

        private byte[] padArray(byte[] input, int size)
        {
            byte[] ret = new byte[size];

            for (int i = 0; i < Math.Min(size, input.Length); i++)
                ret[i] = input[i];

            return ret;
        }

        public unsafe void SerializeTo(ref ConsumableBlock header, ref IList<Block> data)
        {
            byte[] fromArray = Encoding.UTF8.GetBytes(this.FromId);
            Block fromBlock = new Block(padArray(fromArray, ID_FIELD_LEN));
            Block payloadBlock = new Block(Encoding.UTF8.GetBytes(this.Payload));
            fixed (byte* pheader = header.Array)
            {
                int offset = header.Offset;
                *((uint*)(pheader + offset)) = (uint)fromArray.Length;
                offset += sizeof(uint);
                *((uint*)(pheader + offset)) = payloadBlock.size;
                offset += sizeof(uint);
                *((uint*)(pheader + offset)) = this.Sequence;
                offset += sizeof(uint);
                *((double*)(pheader + offset)) = this.Timestamp;
                offset += sizeof(double);
                header.consume(offset - header.Offset);
            }
            data.Add(fromBlock);
            data.Add(payloadBlock);
        }

        public unsafe void DeserializeFrom(ref ConsumableBlock header, ref ConsumableBlock data)
        {
            uint fromlen, payloadlen;
            fixed (byte* pheader = header.Array)
            {
                int offset = header.Offset;
                fromlen = *((uint*)(pheader + offset));
                offset += sizeof(uint);
                payloadlen = *((uint*)(pheader + offset));
                offset += sizeof(uint);
                this.Sequence = *((uint*)(pheader + offset));
                offset += sizeof(uint);
                this.Timestamp = *((double*)(pheader + offset));
                offset += sizeof(double);
                header.consume(offset - header.Offset);
            }
            FromId = Encoding.UTF8.GetString(data.Array, data.Offset, Math.Min((int) fromlen, ID_FIELD_LEN));
            while (FromId.Length < fromlen)
                FromId += "x"; // pad out to actual length with "x"
            data.consume(ID_FIELD_LEN);
            Payload = Encoding.UTF8.GetString(data.Array, data.Offset, (int) payloadlen);
            data.consume((int) payloadlen);
        }

        #endregion
    }
}
