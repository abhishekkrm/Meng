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

namespace QS._qss_c_.Receivers5
{
    public struct SeqNoRanges : QS.Fx.Serialization.ISerializable
    {
        public SeqNoRanges(IList<Base1_.Range<uint>> ranges)
        {
            this.ranges = ranges;
        }

        public SeqNoRanges(IEnumerable<Base1_.Range<uint>> ranges)
        {
            this.ranges = new List<Base1_.Range<uint>>(ranges);
        }

        public SeqNoRanges(int anticipatedNumberOfRanges)
        {
            ranges = new List<Base1_.Range<uint>>(anticipatedNumberOfRanges);
        }

        private IList<Base1_.Range<uint>> ranges;

        public IList<Base1_.Range<uint>> Ranges
        {
            get { return ranges; }
        }

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                int size = sizeof(ushort) + 2 * ranges.Count * sizeof(uint);
                return new QS.Fx.Serialization.SerializableInfo((ushort) ClassID.Nothing, size, size, 0);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)pheader) = (ushort)ranges.Count;
                pheader += sizeof(ushort);
                foreach (Base1_.Range<uint> range in ranges)
                {
                    *((uint*)pheader) = range.From;
                    *((uint*)(pheader + sizeof(uint))) = range.To;
                    pheader += 2 * sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + 2 * ranges.Count * sizeof(uint));
        }

        public unsafe void DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                int count = (int) (*((ushort*)pheader));
                ranges = new List<Base1_.Range<uint>>(count);
                pheader += sizeof(ushort);
                while (count-- > 0)
                {
                    ranges.Add(new Base1_.Range<uint>(*((uint*)pheader), *((uint*)(pheader + sizeof(uint)))));
                    pheader += 2 * sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + 2 * ranges.Count * sizeof(uint));
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base1_.Range<uint>>(ranges, ", ");
        }

        #endregion
    }
}
