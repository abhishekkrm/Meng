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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    [QS.Fx.Serialization.ClassID(ClassID.Base3_Segments)]
    public class Segments : QS.Fx.Serialization.ISerializable
    {
        public Segments()
        {
        }

        public Segments(IList<QS.Fx.Base.Block> segments) : this(segments, 0)
        {
            foreach (QS.Fx.Base.Block segment in segments)
                size += (ushort) segment.size;
        }

        public Segments(IList<QS.Fx.Base.Block> segments, int size)
        {
            this.segments = segments;
            this.size = (ushort) size;
        }

        private IList<QS.Fx.Base.Block> segments;
        private ushort size;

        public ushort SegmentSize
        {
            get { return size; }
        }

        public static int HeaderOverhead
        {
            get { return sizeof(ushort); }
        }

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Base3_Segments, sizeof(ushort), size, segments.Count); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = size;
            }
            header.consume(sizeof(ushort));
            foreach (QS.Fx.Base.Block segment in segments)
                data.Add(segment);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                size  = *((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            int size_asint = (int)size;

            segments = new List<QS.Fx.Base.Block>(1);
            segments.Add(new QS.Fx.Base.Block(data.Array, (uint) data.Offset, (uint) size_asint));
            data.consume(size_asint);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("(size:" + size.ToString() + ", data: {");
            foreach (QS.Fx.Base.Block segment in segments)
            {
                if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                    s.Append(System.Text.UnicodeEncoding.Unicode.GetString(segment.buffer, (int) segment.offset, (int) segment.size));
                else
                    throw new Exception("Unmanaged memory is not supported.");
            }
            s.Append("})");
            return s.ToString();
        }

        public void SerializeTo(ref QS.Fx.Base.ConsumableBlock destination_segment)
        {
            foreach (QS.Fx.Base.Block source_segment in segments)
            {
                if ((source_segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && source_segment.buffer != null)
                    Buffer.BlockCopy(source_segment.buffer, (int) source_segment.offset,
                        destination_segment.Array, destination_segment.Offset, (int) source_segment.size);
                else
                    throw new Exception("Unmanaged memory is not supported.");

                destination_segment.consume((int) source_segment.size);
            }
        }
    }
}
