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

namespace QS._qss_c_.Embeddings2
{
    [QS.Fx.Serialization.ClassID(ClassID.Embeddings2_ReplicationGroupType)]
    public class ReplicationGroupType : Membership_3_.Interface.IGroupType
    {
        public ReplicationGroupType()
        {
        }

        public ReplicationGroupType(System.Type type)
        {
            // this.name = type.Name;
            this.type = new QS._qss_c_.Embeddings2.Types.Interface(type);
        }

        // private string name;
        private Embeddings2.Types.Interface type;

        #region Accessors

        public Embeddings2.Types.Interface InterfaceType
        {
            get { return type; }
        }

        #endregion

        #region IGroupType Members

        string QS._qss_c_.Membership_3_.Interface.IGroupType.Name
        {
            get { return "ReplicationGroup"; }
        }

        bool QS._qss_c_.Membership_3_.Interface.IGroupType.Accepts(Membership_3_.Interface.IGroupAttributes attributes,
            Membership_3_.Interface.IGroupType otherType, Membership_3_.Interface.IGroupAttributes otherAttributes)
        {
            return (otherType is ReplicationGroupType) && type.Equals(((ReplicationGroupType)otherType).type);
        }

        #endregion

        #region IEquatable<IGroupType> Members

        bool IEquatable<QS._qss_c_.Membership_3_.Interface.IGroupType>.Equals(QS._qss_c_.Membership_3_.Interface.IGroupType other)
        {
            return (other is ReplicationGroupType) && type.Equals(((ReplicationGroupType)other).type);
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Embeddings2_ReplicationGroupType, 0, 0, 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)type).SerializableInfo);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)type).SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            type = new QS._qss_c_.Embeddings2.Types.Interface();
            ((QS.Fx.Serialization.ISerializable)type).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
