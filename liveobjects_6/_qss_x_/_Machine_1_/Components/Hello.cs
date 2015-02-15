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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_Hello)]
    public class Hello : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public Hello(string replicaName, uint replicaIncarnation, QS._qss_x_.Base1_.Address replicaAddress, string machineName, 
            Base.MembershipView membershipView, bool respond)
        {
            if (replicaName == null)
                throw new Exception("Replica name is null.");

            if (replicaIncarnation == 0)
                throw new Exception("Replica incarnation is zero.");

            if (replicaAddress == null)
                throw new Exception("Replica address is null.");

            if (machineName == null)
                throw new Exception("Machine name is null.");

            if (membershipView == null)
                throw new Exception("Membership view is null.");

            this.replicaName = replicaName;
            this.replicaIncarnation = replicaIncarnation;
            this.replicaAddress = replicaAddress;
            this.machineName = machineName;
            this.membershipView = membershipView;
            this.respond = respond;
        }

        public Hello()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private string replicaName;
        [QS.Fx.Printing.Printable]
        private uint replicaIncarnation;
        [QS.Fx.Printing.Printable]
        private QS._qss_x_.Base1_.Address replicaAddress;
        [QS.Fx.Printing.Printable]
        private string machineName;
        [QS.Fx.Printing.Printable]
        private Base.MembershipView membershipView;
        [QS.Fx.Printing.Printable]
        private bool respond;

        #endregion

        #region Accessors

        public string ReplicaName
        {
            get { return replicaName; }
            set { replicaName = value; }
        }

        public uint ReplicaIncarnation
        {
            get { return replicaIncarnation; }
            set { replicaIncarnation = value; }
        }

        public QS._qss_x_.Base1_.Address ReplicaAddress
        {
            get { return replicaAddress; }
            set { replicaAddress = value; }
        }

        public string MachineName
        {
            get { return machineName; }
            set { machineName = value; }
        }

        public Base.MembershipView MembershipView
        {
            get { return membershipView; }
            set { membershipView = value; }
        }

        public bool Respond
        {
            get { return respond; }
            set { respond = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Machine_Components_Hello, 0);
 
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, replicaName);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) replicaAddress).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, machineName);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)membershipView).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, replicaName);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, replicaIncarnation);
            ((QS.Fx.Serialization.ISerializable)replicaAddress).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, machineName);
            ((QS.Fx.Serialization.ISerializable)membershipView).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, respond);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            replicaName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            replicaIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            replicaAddress = new QS._qss_x_.Base1_.Address();
            ((QS.Fx.Serialization.ISerializable)replicaAddress).DeserializeFrom(ref header, ref data);
            machineName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            membershipView = new QS._qss_x_._Machine_1_.Base.MembershipView();
            ((QS.Fx.Serialization.ISerializable)membershipView).DeserializeFrom(ref header, ref data);
            respond = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
        }

        #endregion

        #region Overrides from System.Object

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion
    }
}
