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

namespace QS._qss_c_.Infrastructure.Components
{
    [QS.Fx.Serialization.ClassID(ClassID.Infrastructure_Components_GVID)]
    public sealed class GVID : QS._qss_c_.Base3_.ISerializableID
    {
        public GVID()
        {
        }

        public GVID(Base3_.GVID id)
        {
            this.id = id;
        }

        private Base3_.GVID id;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return id.GroupID; }
            set { id.GroupID = value; }
        }

        public uint ViewSequenceNo
        {
            get { return id.SeqNo; }
            set { id.SeqNo = value; }
        }

        public Base3_.GVID GroupViewID
        {
            get { return id; }
            set { id = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Infrastructure_Components_GVID, 0, 0, 0);
                info.AddAnother(id.SerializableInfo);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            id.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            id.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IKnownClass Members

        ClassID QS._qss_c_.Base3_.IKnownClass.ClassID
        {
            get { return ClassID.Infrastructure_Components_GVID; }
        }

        #endregion

        #region IComparable<ISerializableID> Members

        int IComparable<QS._qss_c_.Base3_.ISerializableID>.CompareTo(QS._qss_c_.Base3_.ISerializableID other)
        {
            if (other is GVID)
                return id.CompareTo(((GVID)other).id);
            else
                throw new Exception("Not comparable, the argument must be a " + GetType().ToString() + ".");
        }

        #endregion

        #region IEquatable<ISerializableID> Members

        bool IEquatable<QS._qss_c_.Base3_.ISerializableID>.Equals(QS._qss_c_.Base3_.ISerializableID other)
        {
            return (other is GVID) && id.Equals(((GVID)other).id);
        }

        #endregion
    }
}
