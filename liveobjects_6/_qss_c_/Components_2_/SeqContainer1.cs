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

namespace QS._qss_c_.Components_2_
{
    [QS.Fx.Serialization.ClassID(ClassID.Components2_SeqContainer1)]
    public class SeqContainer1 : ISeqContainer
    {
        public SeqContainer1()
        {
        }

        private List<uint> list = new List<uint>();

        #region ISeqContainer Members

        void ISeqContainer.Add(uint seqNo)
        {
            list.Add(seqNo);
        }

        void ISeqContainer.IntersectWith(ISeqContainer anotherContainer)
        {
            System.Collections.ObjectModel.Collection<uint> myCollection = new System.Collections.ObjectModel.Collection<uint>(list);
            list = new List<uint>();
            foreach (uint seqno in anotherContainer)
            {
                if (myCollection.Contains(seqno))
                {
                    list.Add(seqno);
                    myCollection.Remove(seqno);
                }
            }
        }

        #endregion

        #region IEnumerable<uint> Members

        IEnumerator<uint> IEnumerable<uint>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int header_overhead = sizeof(ushort) + list.Count * sizeof(uint);
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Components2_SeqContainer1, (ushort)header_overhead, header_overhead, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((ushort*) headerptr) = (ushort) list.Count;
                headerptr += sizeof(ushort);
                foreach (uint seqno in list)
                {
                    *((uint*)headerptr) = seqno;
                    headerptr += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + list.Count * sizeof(uint));            
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                int count = (int) *((ushort*)headerptr);
                headerptr += sizeof(ushort);
                while (count-- > 0)
                {
                    list.Add(*((uint*)headerptr));
                    headerptr += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + list.Count * sizeof(uint));
        }

        #endregion

        public override string ToString()
        {
            return QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<uint>(list, ",");
        }
    }
}
