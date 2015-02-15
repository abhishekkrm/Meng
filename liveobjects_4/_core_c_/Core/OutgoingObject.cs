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

namespace QS._core_c_.Core
{
/*
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_outgoingobject)]
    public struct OutgoingObject : QS.Fx.Serialization.ISerializable
    {
        public OutgoingObject(int operation, long channel, long connection, int sequenceno, int datalength, IList<QS.Fx.Base.Block> datablocks)
        {
            this.operation = operation;
            this.channel = channel;
            this.connection = connection;
            this.sequenceno = sequenceno;
            this.datalength = datalength;
            this.datablocks = datablocks;
        }

        public int operation;
        public long channel;
        public long connection;
        public int sequenceno;
        public int datalength;
        public IList<QS.Fx.Base.Block> datablocks;

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort) QS.ClassID.ChannelObject,
                    3 * sizeof(int) + 2 * sizeof(long),
                    3 * sizeof(int) + 2 * sizeof(long) + datalength,
                    (datablocks != null) ? datablocks.Count : 0);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((int*)pheader) = operation;
                pheader += sizeof(int);
                *((long*)pheader) = channel;
                pheader += sizeof(long);
                *((long*)pheader) = connection;
                pheader += sizeof(long);
                *((int*)pheader) = sequenceno;
                pheader += sizeof(int);
                *((int*)pheader) = datalength;
            }
            header.consume(3 * sizeof(int) + 2 * sizeof(long));
            if (datablocks != null)
            {
                foreach (QS.Fx.Base.Block datablock in datablocks)
                    data.Add(datablock);
            }
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                operation = *((int*)pheader);
                pheader += sizeof(int);
                channel = *((long*)pheader);
                pheader += sizeof(long);
                connection = *((long*)pheader);
                pheader += sizeof(long);
                sequenceno = *((int*)pheader);
                pheader += sizeof(int);
                datalength = *((int*)pheader);
            }
            header.consume(3 * sizeof(int) + 2 * sizeof(long));
            if (datalength > 0)
            {
                datablocks = new List<QS.Fx.Base.Block>(1);
                datablocks.Add(new QS.Fx.Base.Block(data.Array, (uint) data.Offset, (uint) datalength));
            }
        }

        public unsafe void SerializeTo(out int datalength, out IList<QS.Fx.Base.Block> datablocks)
        {
            datalength = 3 * sizeof(int) + 2 * sizeof(long) + this.datalength;
            byte[] header = new byte[3 * sizeof(int) + 2 * sizeof(long)];
            fixed (byte* pheader1 = header)
            {
                byte* pheader = pheader1;
                *((int*)pheader) = this.operation;
                pheader += sizeof(int);
                *((long*)pheader) = this.channel;
                pheader += sizeof(long);
                *((long*)pheader) = this.connection;
                pheader += sizeof(long);
                *((int*)pheader) = this.sequenceno;
                pheader += sizeof(int);
                *((int*)pheader) = this.datalength;
            }
            datablocks = new List<QS.Fx.Base.Block>(this.datablocks.Count + 1);
            datablocks.Add(new QS.Fx.Base.Block(header));
            if (this.datablocks != null)
                foreach (QS.Fx.Base.Block datablock in this.datablocks)
                    datablocks.Add(datablock);
        }
        public unsafe void DeserializeFrom(int datalength, IList<QS.Fx.Base.Block> datablocks)
        {
            throw new NotImplementedException();
        }
    }
*/
}
