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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Rings6_ReceivingAgent1_IntraPartitionToken)]
    public class AgentIntraPartitionToken : QS.Fx.Serialization.ISerializable
    {
        public AgentIntraPartitionToken(QS._core_c_.Base3.InstanceID interPartitionCreatorAddress, QS._core_c_.Base3.InstanceID partitionCreatorAddress,
            uint round, bool quiescence, bool quiescence_commit)
        {
            this.interPartitionCreatorAddress = interPartitionCreatorAddress;
            this.partitionCreatorAddress = partitionCreatorAddress;
            this.round = round;
            this.quiesce = quiescence;
            this.quiesce_commit = quiescence_commit;
        }

        public AgentIntraPartitionToken()
        {
        }

        [QS.Fx.Printing.Printable]
        private QS._core_c_.Base3.InstanceID partitionCreatorAddress, interPartitionCreatorAddress;
        [QS.Fx.Printing.Printable]
        private IDictionary<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken> receiverTokens =
            new Dictionary<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken>();
        [QS.Fx.Printing.Printable]
        private uint round;
        [QS.Fx.Printing.Printable]
        private bool quiesce, quiesce_commit;

        public QS._core_c_.Base3.InstanceID PartitionCreatorAddress
        {
            get { return partitionCreatorAddress; }
        }

        public QS._core_c_.Base3.InstanceID InterPartitionCreatorAddress
        {
            get { return interPartitionCreatorAddress; }
        }

        public IDictionary<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken> ReceiverTokens
        {
            get { return receiverTokens; }
        }

        public uint Round
        {
            get { return round; }
        }

        public bool Quiescence
        {
            get { return quiesce; }
            set 
            { 
                quiesce = value;
                if (!quiesce)
                    quiesce_commit = false;
            }
        }

        public bool QuiescenceCommit
        {
            get { return quiesce_commit; }
            set { quiesce_commit = value; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    Base3_.SerializationHelper.DictionaryInfo<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken>(
                    ClassID.Rings6_ReceivingAgent1_IntraPartitionToken, receiverTokens);
                info.AddAnother(partitionCreatorAddress.SerializableInfo);
                info.AddAnother(interPartitionCreatorAddress.SerializableInfo);
                info.HeaderSize += sizeof(uint) + 2 * sizeof(bool);
                info.Size += sizeof(uint) + 2 * sizeof(bool);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((uint*)(pbuffer + header.Offset)) = round;
                *((bool*)(pbuffer + header.Offset + sizeof(uint))) = quiesce;
                *((bool*)(pbuffer + header.Offset + sizeof(uint) + sizeof(bool))) = quiesce_commit;
            }
            header.consume(sizeof(uint) + 2 * sizeof(bool));
            partitionCreatorAddress.SerializeTo(ref header, ref data);
            interPartitionCreatorAddress.SerializeTo(ref header, ref data);
            Base3_.SerializationHelper.SerializeDictionary<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken>(
                receiverTokens, ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                round = *((uint*)(pbuffer + header.Offset));
                quiesce = *((bool*)(pbuffer + header.Offset + sizeof(uint)));
                quiesce_commit = *((bool*)(pbuffer + header.Offset + sizeof(uint) + sizeof(bool)));
            }
            header.consume(sizeof(uint) + 2 * sizeof(bool));
            partitionCreatorAddress = new QS._core_c_.Base3.InstanceID();
            interPartitionCreatorAddress = new QS._core_c_.Base3.InstanceID();
            partitionCreatorAddress.DeserializeFrom(ref header, ref data);
            interPartitionCreatorAddress.DeserializeFrom(ref header, ref data);
            Base3_.SerializationHelper.DeserializeDictionary<QS._core_c_.Base3.InstanceID, ReceiverIntraPartitionToken>(
                receiverTokens, ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }
    }
}
