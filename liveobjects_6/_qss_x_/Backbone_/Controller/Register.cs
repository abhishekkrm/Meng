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

namespace QS._qss_x_.Backbone_.Controller
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Backbone_Controller_Register)]
    public sealed class Register : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public Register(QS.Fx.Base.ID domainid, string domainname, QS.Fx.Base.ID providerid, QS.Fx.Base.ID topicid, Scope.MembershipType registrationtype)
        {
            this.domainid = domainid;
            this.domainname = domainname;
            this.providerid = providerid;
            this.topicid = topicid;
            this.registrationtype = registrationtype;
        }

        public Register()
        {
        }

        #endregion

        #region Overrides from System.Object

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private QS.Fx.Base.ID domainid, providerid, topicid;

        [QS.Fx.Printing.Printable]
        private string domainname;

        [QS.Fx.Printing.Printable]
        private Scope.MembershipType registrationtype;

        #endregion

        #region Accessors

        public QS.Fx.Base.ID DomainID
        {
            get { return domainid; }
        }

        public string DomainName
        {
            get { return domainname; }
        }

        public QS.Fx.Base.ID ProviderID
        {
            get { return providerid; }
        }

        public QS.Fx.Base.ID TopicID
        {
            get { return topicid; }
        }

        public Scope.MembershipType RegistrationType
        {
            get { return registrationtype; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Backbone_Controller_Register, 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)domainid).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, domainname);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)providerid).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)topicid).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Byte(ref info);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)domainid).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, domainname);
            ((QS.Fx.Serialization.ISerializable)providerid).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)topicid).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Byte(ref header, ref data, (byte) registrationtype);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            domainid = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable)domainid).DeserializeFrom(ref header, ref data);
            domainname = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            providerid = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable)providerid).DeserializeFrom(ref header, ref data);
            topicid = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable)topicid).DeserializeFrom(ref header, ref data);
            registrationtype = (QS._qss_x_.Backbone_.Scope.MembershipType) QS._qss_c_.Base3_.SerializationHelper.Deserialize_Byte(ref header, ref data);
        }

        #endregion
    }
}
