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

namespace QS._qss_c_.Rings4
{
    [QS.Fx.Serialization.ClassID(ClassID.Ring4_NAKs)]
    public class NAKs : QS.Fx.Serialization.ISerializable
    {
        public NAKs()
        {
        }

        public NAKs(uint maximumSeqNo, ICollection<uint> missedSeqNoCollection)
        {
            this.maximumSeqNo = maximumSeqNo;
            this.missedSeqNoCollection = missedSeqNoCollection;
        }

        private uint maximumSeqNo;
        private ICollection<uint> missedSeqNoCollection;

        public uint MaximumSeqNo
        {
            get { return maximumSeqNo; }
        }

        public ICollection<uint> Missed
        {
            get { return missedSeqNoCollection; }
        }

        public override string ToString()
        {
            return "(max:" + maximumSeqNo.ToString() + ", missed:" +
                QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<uint>(missedSeqNoCollection, ",") + ")";
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int size = sizeof(uint) + sizeof(ushort) + sizeof(uint) * missedSeqNoCollection.Count;
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Ring4_NAKs, (ushort)size, size, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)pheader) = maximumSeqNo;
                pheader += sizeof(uint);
                *((ushort*) pheader) = (ushort) missedSeqNoCollection.Count;
                pheader += sizeof(ushort);
                foreach (uint seqno in missedSeqNoCollection)
                {
                    *((uint*)pheader) = seqno;
                    pheader += sizeof(uint);
                }
            }
            header.consume(sizeof(uint) + sizeof(ushort) + sizeof(uint) * missedSeqNoCollection.Count);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                maximumSeqNo  = *((uint*)pheader);
                pheader += sizeof(uint);
                int nelements = (int) (*((ushort*)pheader));
                pheader += sizeof(ushort);
                uint[] elements = new uint[nelements];
                for (int ind = 0; ind < nelements; ind++)
                {
                    elements[ind] = *((uint*)pheader);
                    pheader += sizeof(uint);
                }
                missedSeqNoCollection = elements;
            }
            header.consume(sizeof(uint) + sizeof(ushort) + sizeof(uint) * missedSeqNoCollection.Count);
        }

        #endregion
    }
}
