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

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_PeerInfo)]
    [QS.Fx.Base.Inspectable]
    public sealed class PeerInfo : QS.Fx.Inspection.Inspectable, IPeerInfo, QS.Fx.Serialization.ISerializable
    {
        public PeerInfo(string replicaName, bool discovered, uint replicaIncarnation, QS._qss_x_.Base1_.Address replicaAddress, uint machineIncarnation, uint currentViewNumber)
        {
            this.replicaName = replicaName;
            this.discovered = discovered;
            this.replicaIncarnation = replicaIncarnation;
            this.replicaAddress = replicaAddress;
            this.machineIncarnation = machineIncarnation;
            this.currentViewNumber = currentViewNumber;
        }

        public PeerInfo()
        {
        }

        private string replicaName;
        private bool discovered;
        private QS._qss_x_.Base1_.Address replicaAddress;
        private uint replicaIncarnation, machineIncarnation, currentViewNumber;

        #region IPeerInfo Members

        string IPeerInfo.ReplicaName
        {
            get { return replicaName; }
        }

        bool IPeerInfo.Discovered
        {
            get { return discovered; }
            set { discovered = value; }
        }

        uint IPeerInfo.ReplicaIncarnation
        {
            get { return replicaIncarnation; }
            set { replicaIncarnation = value; }
        }

        QS._qss_x_.Base1_.Address IPeerInfo.ReplicaAddress
        {
            get { return replicaAddress; }
            set { replicaAddress = value; }
        }

        uint IPeerInfo.MachineIncarnation
        {
            get { return machineIncarnation; }
            set { machineIncarnation = value; }
        }

        uint IPeerInfo.CurrentViewNumber
        {
            get { return currentViewNumber; }
            set { currentViewNumber = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Machine_Components_ReplicaNonpersistentState);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, replicaName);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)replicaAddress).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, replicaName);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, discovered);
            ((QS.Fx.Serialization.ISerializable)replicaAddress).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, replicaIncarnation);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, machineIncarnation);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, currentViewNumber);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            replicaName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            discovered = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            replicaAddress = new QS._qss_x_.Base1_.Address();
            ((QS.Fx.Serialization.ISerializable)replicaAddress).DeserializeFrom(ref header, ref data);
            replicaIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            machineIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            currentViewNumber = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);           
        }

        #endregion
    }
}
