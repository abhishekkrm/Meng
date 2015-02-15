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

namespace QS._qss_c_.Rings6
{
    [QS.Fx.Serialization.ClassID(ClassID.Rings6_ReceivingAgent_Agent_Receiver_PartitionAcknowledgement)]
    [QS.Fx.Printing.Printable("Pull", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public class PartitionAcknowledgement : QS.Fx.Serialization.ISerializable
    {
        public PartitionAcknowledgement(
            QS._core_c_.Base3.InstanceID senderAddress, uint partitionIndex, uint partitionCount, 
            uint maximumClean, IList<Base1_.Range<uint>> isolatedAcks, IList<Base1_.Range<uint>> isolatedNaks)
        {
            this.senderAddress = senderAddress;
            this.partitionIndex = partitionIndex;
            this.partitionCount = partitionCount;
            this.maximumClean = maximumClean;
            this.isolatedAcks = isolatedAcks;
            this.isolatedNaks = isolatedNaks;
        }

        public PartitionAcknowledgement()
        {
        }

        [QS.Fx.Printing.Printable]
        private QS._core_c_.Base3.InstanceID senderAddress;
        [QS.Fx.Printing.Printable]
        private uint partitionIndex, partitionCount, maximumClean;
        [QS.Fx.Printing.Printable]
        private IList<Base1_.Range<uint>> isolatedAcks, isolatedNaks;

        #region Accessors

        public QS._core_c_.Base3.InstanceID SenderAddress
        {
            get { return senderAddress; }
        }

        public uint PartitionIndex
        {
            get { return partitionIndex; }
        }

        public uint PartitionCount
        {
            get { return partitionCount; }
        }

        public uint MaximumClean
        {
            get { return maximumClean; }
        }

        public IList<Base1_.Range<uint>> IsolatedAcks
        {
            get { return isolatedAcks; }
        }

        public IList<Base1_.Range<uint>> IsolatedNaks
        {
            get { return isolatedNaks; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int size = (3 + 2 * (((isolatedAcks != null) ? isolatedAcks.Count : 0) + ((isolatedNaks != null) ? isolatedNaks.Count : 0))) * sizeof(uint) + 2 * sizeof(ushort);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Rings6_ReceivingAgent_Agent_Receiver_PartitionAcknowledgement, size, size, 0);
                info.AddAnother(senderAddress.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            senderAddress.SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((uint*)pheader) = partitionIndex;
                *((uint*)(pheader + sizeof(uint))) = partitionCount;
                *((uint*)(pheader + 2 * sizeof(uint))) = maximumClean;
                *((ushort*)(pheader + 3 * sizeof(uint))) = (ushort)((isolatedAcks != null) ? isolatedAcks.Count : 0);
                *((ushort*)(pheader + 3 * sizeof(uint) + sizeof(ushort))) = (ushort)((isolatedNaks != null) ? isolatedNaks.Count : 0);
                pheader += 3 * sizeof(uint) + 2 * sizeof(ushort);
                if (isolatedAcks != null)
                {
                    foreach (Base1_.Range<uint> ack in isolatedAcks)
                    {
                        *((uint*)pheader) = ack.From;
                        *((uint*)(pheader + sizeof(uint))) = ack.To;
                        pheader += 2 * sizeof(uint);
                    }
                }
                if (IsolatedNaks != null)
                {
                    foreach (Base1_.Range<uint> nak in isolatedNaks)
                    {
                        *((uint*)pheader) = nak.From;
                        *((uint*)(pheader + sizeof(uint))) = nak.To;
                        pheader += 2 * sizeof(uint);
                    }
                }
            }
            header.consume((3 + 2 * (((isolatedAcks != null) ? isolatedAcks.Count : 0) + ((isolatedNaks != null) ? isolatedNaks.Count : 0))) * sizeof(uint) + 2 * sizeof(ushort));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            senderAddress = new QS._core_c_.Base3.InstanceID();
            senderAddress.DeserializeFrom(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                partitionIndex = *((uint*)pheader);
                partitionCount = *((uint*)(pheader + sizeof(uint)));
                maximumClean = *((uint*)(pheader + 2 * sizeof(uint)));
                int ackCount = (int)(*((ushort*)(pheader + 3 * sizeof(uint))));
                int nakCount = (int)(*((ushort*)(pheader + 3 * sizeof(uint) + sizeof(ushort))));
                pheader += 3 * sizeof(uint) + 2 * sizeof(ushort);
                isolatedAcks = (ackCount > 0) ? new List<Base1_.Range<uint>>(ackCount) : null;
                isolatedNaks = (nakCount > 0) ? new List<Base1_.Range<uint>>(nakCount) : null;
                if (isolatedAcks != null)
                {
                    while (ackCount-- > 0)
                    {
                        isolatedAcks.Add(new Base1_.Range<uint>(*((uint*)pheader), *((uint*)(pheader + sizeof(uint)))));
                        pheader += 2 * sizeof(uint);
                    }
                }
                if (isolatedNaks != null)
                {
                    while (nakCount-- > 0)
                    {
                        isolatedNaks.Add(new Base1_.Range<uint>(*((uint*)pheader), *((uint*)(pheader + sizeof(uint)))));
                        pheader += 2 * sizeof(uint);
                    }
                }
            }
            header.consume((3 + 2 * (((isolatedAcks != null) ? isolatedAcks.Count : 0) + ((isolatedNaks != null) ? isolatedNaks.Count : 0))) * sizeof(uint) + 2 * sizeof(ushort));
        }

        #endregion
    }
}
