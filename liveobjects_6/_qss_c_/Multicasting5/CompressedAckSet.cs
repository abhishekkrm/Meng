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

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting5_CompressedAckSet)]
    public class CompressedAckSet : QS.Fx.Serialization.ISerializable, IEnumerable<uint>
    {
        public CompressedAckSet()
        {
        }

        public CompressedAckSet(IEnumerable<Components_2_.Range<uint>> ranges)
        {
            this.ranges.AddRange(ranges);
        }

        private List<Components_2_.Range<uint>> ranges = new List<QS._qss_c_.Components_2_.Range<uint>>();

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
                int size = sizeof(ushort) + 2 * sizeof(uint) * ranges.Count;
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Multicasting5_CompressedAckSet, (ushort)size, size, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {            
            fixed (byte* arrayptr = header.Array)
            {
                byte *headerptr = arrayptr + header.Offset;
                *((ushort*) headerptr) = (ushort) ranges.Count;
                headerptr += sizeof(ushort);
                foreach (Components_2_.Range<uint> range in ranges)
                {
                    *((uint*)headerptr) = range.From;
                    *((uint*)(headerptr + sizeof(uint))) = range.To;
                    headerptr += 2 * sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + 2 * sizeof(uint) * ranges.Count);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                int count = (int)*((ushort*)headerptr);
                headerptr += sizeof(ushort);
                while (count-- > 0)
                {
                    ranges.Add(new QS._qss_c_.Components_2_.Range<uint>(*((uint*)headerptr), *((uint*)(headerptr + sizeof(uint)))));
                    headerptr += 2 * sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + 2 * sizeof(uint) * ranges.Count);
        }

        #endregion

        public override string ToString()
        {
            return QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Components_2_.Range<uint>>(ranges, ",");
        }

        public void CutOffSmaller(uint firstSeqNo)
        {
            if (ranges.Count > 0 && ranges[0].From < firstSeqNo)
            {
                int toGo = ranges.Count;
                List<Components_2_.Range<uint>> new_ranges = null;
                bool appending = false;
                foreach (Components_2_.Range<uint> range in ranges)
                {
                    if (appending)
                    {
                        new_ranges.Add(range);
                    }
                    else
                    {
                        if (range.To >= firstSeqNo)
                        {
                            uint lowerBound = range.From;
                            if (firstSeqNo > lowerBound)
                                lowerBound = firstSeqNo;
                            new_ranges = new List<Components_2_.Range<uint>>(toGo);
                            new_ranges.Add(new Components_2_.Range<uint>(lowerBound, range.To));
                            appending = true;
                        }
                        toGo--;
                    }
                }

                ranges = (new_ranges != null) ? new_ranges : new List<Components_2_.Range<uint>>(0);
            }
        }

        #region IEnumerable<uint> Members

        IEnumerator<uint> IEnumerable<uint>.GetEnumerator()
        {
            foreach (Components_2_.Range<uint> range in ranges)
            {
                for (uint ind = range.From; ind <= range.To; ind++)
                    yield return ind;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<uint>)this).GetEnumerator();
        }

        #endregion
    }
}
