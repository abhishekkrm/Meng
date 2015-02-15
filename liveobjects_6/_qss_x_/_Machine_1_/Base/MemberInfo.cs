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

namespace QS._qss_x_._Machine_1_.Base
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Base_MemberInfo)]
    public class MemberInfo : IMemberInfo, QS.Fx.Serialization.ISerializable, IEquatable<MemberInfo>
    {
        public MemberInfo(string replicaName, uint replicaIncarnation, QS._qss_x_.Base1_.Address replicaAddress, double weight)
        {
            this.replicaName = replicaName;
            this.replicaIncarnation = replicaIncarnation;
            this.replicaAddress = replicaAddress;
            this.weight = weight;
        }

        public MemberInfo()
        {
        }

        private string replicaName;
        private uint replicaIncarnation;
        private QS._qss_x_.Base1_.Address replicaAddress;
        private double weight;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public string Name
        {
            get { return replicaName; }
        }

        [QS.Fx.Printing.Printable]
        public uint Incarnation
        {
            get { return replicaIncarnation; }
        }

        [QS.Fx.Printing.Printable]
        public QS._qss_x_.Base1_.Address Address
        {
            get { return replicaAddress; }
        }

        [QS.Fx.Printing.Printable]
        public double Weight
        {
            get { return weight; }
        }

        #endregion

        #region IMemberInfo Members

        string IMemberInfo.Name
        {
            get { return replicaName; }
        }

        uint IMemberInfo.Incarnation
        {
            get { return replicaIncarnation; }
        }

        QS._qss_x_.Base1_.Address IMemberInfo.Address
        {
            get { return replicaAddress; }
        }

        double IMemberInfo.Weight
        {
            get { return weight; }
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Machine_Base_MemberInfo, 0);

                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, replicaName);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) replicaAddress).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Double(ref info);

                return info;                    
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, replicaName);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, replicaIncarnation);
            ((QS.Fx.Serialization.ISerializable) replicaAddress).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Double(ref header, ref data, weight);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            replicaName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            replicaIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            replicaAddress = new QS._qss_x_.Base1_.Address();
            ((QS.Fx.Serialization.ISerializable)replicaAddress).DeserializeFrom(ref header, ref data);
            weight = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Double(ref header, ref data);
        }

        #endregion

        #region Overrides from Object

        public override bool Equals(object obj)
        {
            MemberInfo other = obj as MemberInfo;
            return (other != null) ? (replicaName.Equals(other.replicaName) && replicaIncarnation.Equals(other.replicaIncarnation)
                && replicaAddress.Equals(other.replicaAddress) && weight.Equals(other.weight)) : false;
        }

        public override int GetHashCode()
        {
            return replicaName.GetHashCode() ^ replicaAddress.GetHashCode() ^ replicaIncarnation.GetHashCode() ^ weight.GetHashCode();
        }

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region IEquatable<MemberInfo> Members

        bool IEquatable<MemberInfo>.Equals(MemberInfo other)
        {
            return replicaName.Equals(other.replicaName) && replicaIncarnation.Equals(other.replicaIncarnation)
                && replicaAddress.Equals(other.replicaAddress) && weight.Equals(other.weight);
        }

        #endregion
    }
}
