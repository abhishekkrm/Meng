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
    [QS.Fx.Serialization.ClassID(ClassID.Rings4_Token)]
    public class Token : QS.Fx.Serialization.ISerializable
    {
        public Token(uint nodeAddress, uint tokenSeqNo,
            uint cutoffSeqNo, uint maximumSeqNo, IEnumerable<uint> nakCollection, NAKs cleanupCacheNAKs)
        {
            this.tokenSeqNo = tokenSeqNo;
            this.cutoffSeqNo = cutoffSeqNo;
            this.maximumSeqNo = maximumSeqNo;
            this.nakCollection = new Dictionary<uint, NAK>();
            this.cleanupCacheNAKs = cleanupCacheNAKs;

            foreach (uint seqno in nakCollection)
            {
                if (seqno < cutoffSeqNo)
                    this.nakCollection.Add(seqno, new NAK(nodeAddress));
            }
        }

        private uint tokenSeqNo, cutoffSeqNo, maximumSeqNo;
        private IDictionary<uint, NAK> nakCollection;
        private NAKs cleanupCacheNAKs;

        public Token()
        {
        }

        #region Append

        public void Append(
            uint nodeAddress, uint maximumSeqNo, IEnumerable<uint> nakCollection, out IEnumerable<uint> newNAKs)
        {
            List<uint> theNewNAKs = new List<uint>();

            if (maximumSeqNo > this.maximumSeqNo)
                this.maximumSeqNo = maximumSeqNo;

            foreach (uint seqno in nakCollection)
            {
                if (seqno <= cutoffSeqNo)
                {
                    if (this.nakCollection.ContainsKey(seqno))
                        this.nakCollection[seqno].Append(nodeAddress);
                    else
                        this.nakCollection.Add(seqno, new NAK(nodeAddress));

                    theNewNAKs.Add(seqno);
                }
            }

            newNAKs = theNewNAKs;
        }

        #endregion

        #region Class NAK

        public class NAK : QS.Fx.Serialization.ISerializable
        {
            public NAK()
            {
            }

            public NAK(uint address)
            {
                missingAddresses = new List<uint>();
                missingAddresses.Add(address);
            }

            private IList<uint> missingAddresses;
            private bool forwarded = false;

            public void Append(uint address)
            {
                missingAddresses.Add(address);
            }

            #region Accessors

            public IList<uint> MissingAddresses
            {
                get { return missingAddresses; }
            }

            public bool Forwarded
            {
                get { return forwarded; }
                set { forwarded = value; }
            }

            #endregion

            #region Printing

            public override string ToString()
            {
                return (forwarded ? "fwd" : "") +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<uint>(missingAddresses, ",");                
            }

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                { 
                    int size = sizeof(bool) + sizeof(ushort) + missingAddresses.Count * sizeof(uint);
                    return new QS.Fx.Serialization.SerializableInfo((ushort) size, size, 0);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* pbuffer = header.Array)
                {
                    byte* pheader = pbuffer + header.Offset;
                    *((bool*)pheader) = forwarded;
                    pheader += sizeof(bool);
                    *((ushort*)pheader) = (ushort)missingAddresses.Count;
                    pheader += sizeof(ushort);
                    foreach (uint address in missingAddresses)
                    {
                        *((uint*)pheader) = address;
                        pheader += sizeof(uint);
                    }
                }
                header.consume(sizeof(bool) + sizeof(ushort) + missingAddresses.Count * sizeof(uint));
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* pbuffer = header.Array)
                {
                    byte* pheader = pbuffer + header.Offset;
                    forwarded = *((bool*)pheader);
                    pheader += sizeof(bool);
                    int naddresses = (int) (*((ushort*)pheader));
                    missingAddresses = new List<uint>(naddresses);
                    pheader += sizeof(ushort);
                    while (naddresses-- > 0)
                    {
                        uint address = *((uint*)pheader);
                        pheader += sizeof(uint);
                        missingAddresses.Add(address);
                    }
                }
                header.consume(sizeof(bool) + sizeof(ushort) + missingAddresses.Count * sizeof(uint));
            }

            #endregion
        }

        #endregion

        #region Accessors

        public uint TokenSeqNo
        {
            get { return tokenSeqNo; }
        }

        public uint CutoffSeqNo
        {
            get { return cutoffSeqNo; }
        }

        public uint MaximumSeqNo
        {
            get { return maximumSeqNo; }
        }

        public IDictionary<uint, NAK> NakCollection
        {
            get { return nakCollection; }
        }

        public NAKs CleanupCacheNAKs
        {
            get { return cleanupCacheNAKs; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int size = 3 * sizeof(uint) + sizeof(ushort) + nakCollection.Count * sizeof(uint);
                foreach (KeyValuePair<uint, NAK> element in nakCollection)
                    size += ((QS.Fx.Serialization.ISerializable)element.Value).SerializableInfo.Size;
                
                return ((QS.Fx.Serialization.ISerializable) cleanupCacheNAKs).SerializableInfo.Extend(
                    (ushort) ClassID.Rings4_Token, (ushort) size, 0, 0); 
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)cleanupCacheNAKs).SerializeTo(ref header, ref data);

            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)pheader) = cutoffSeqNo;
                *((uint*)(pheader + sizeof(uint))) = maximumSeqNo;
                *((ushort*)(pheader + 2 * sizeof(uint))) = (ushort) nakCollection.Count;
                *((uint*)(pheader + 2 * sizeof(uint) + sizeof(ushort))) = tokenSeqNo;
            }
            header.consume(3 * sizeof(uint) + sizeof(ushort));

            foreach (KeyValuePair<uint, NAK> element in nakCollection)
            {
                fixed (byte* pbuffer = header.Array)
                {
                    *((uint*)(pbuffer + header.Offset)) = element.Key;
                }
                header.consume(sizeof(uint));
                ((QS.Fx.Serialization.ISerializable) element.Value).SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            cleanupCacheNAKs = new NAKs();
            ((QS.Fx.Serialization.ISerializable)cleanupCacheNAKs).DeserializeFrom(ref header, ref data);

            int nelements;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                cutoffSeqNo  = *((uint*)pheader);
                maximumSeqNo  = *((uint*)(pheader + sizeof(uint)));
                nelements = (int) (*((ushort*)(pheader + 2 * sizeof(uint))));
                tokenSeqNo = (*((uint*)(pheader + 2 * sizeof(uint) + sizeof(ushort))));
            }
            header.consume(3 * sizeof(uint) + sizeof(ushort));

            nakCollection = new Dictionary<uint, NAK>(nelements);
            while (nelements-- > 0)
            {
                uint seqno;
                fixed (byte* pbuffer = header.Array)
                {
                    seqno = *((uint*)(pbuffer + header.Offset));
                }
                header.consume(sizeof(uint));
                NAK nak = new NAK();
                ((QS.Fx.Serialization.ISerializable)nak).DeserializeFrom(ref header, ref data);
                nakCollection.Add(seqno, nak);
            }
        }

        #endregion

        #region Printing

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine("CutoffSeqNo = " + cutoffSeqNo.ToString());
            s.AppendLine("MaximumSeqNo = " + maximumSeqNo.ToString());
            foreach (KeyValuePair<uint, NAK> element in nakCollection)
                s.AppendLine("NAK(" + element.Key.ToString() + ") = " + element.Value.ToString());
            s.AppendLine("CleanupNAKs = " + cleanupCacheNAKs.ToString());

            return s.ToString();
        }

        #endregion
    }
}
