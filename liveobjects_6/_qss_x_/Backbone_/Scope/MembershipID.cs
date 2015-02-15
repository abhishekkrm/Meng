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

namespace QS._qss_x_.Backbone_.Scope
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Backbone_Scope_MembershipID)]
    public sealed class MembershipID : IEquatable<MembershipID>, IComparable<MembershipID>, IComparable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public MembershipID(Base1_.QualifiedID memberid, Base1_.QualifiedID containerid, ulong instanceid)
        {
            this.memberid = memberid;
            this.containerid = containerid;
            this.instanceid = instanceid;
        }

        public MembershipID()
        {
        }

        #endregion

        #region Fields

        private Base1_.QualifiedID containerid, memberid;
        private ulong instanceid;

        #endregion

        #region Internal Interface

        internal Base1_.QualifiedID Container
        {
            get { return containerid; }
        }

        internal Base1_.QualifiedID Member
        {
            get { return memberid; }
        }

        internal ulong Instance
        {
            get { return instanceid; }
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return memberid.ToString() + "@" + containerid.ToString() + "/" + instanceid.ToString();
        }

        public override int GetHashCode()
        {
            return memberid.GetHashCode() ^ containerid.GetHashCode() ^ instanceid.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            MembershipID other = obj as MembershipID;
            return (other != null) && 
                memberid.Equals(other.memberid) && containerid.Equals(other.containerid) && instanceid.Equals(other.instanceid);
        }

        #endregion

        #region IEquatable<AgentID> Members

        bool IEquatable<MembershipID>.Equals(MembershipID other)
        {
            return (other != null) && 
                memberid.Equals(other.memberid) && containerid.Equals(other.containerid) && instanceid.Equals(other.instanceid);
        }

        #endregion

        #region IComparable<AgentID> Members

        int IComparable<MembershipID>.CompareTo(MembershipID other)
        {
            if (other == null)
                throw new Exception("Argument of comparison is null.");

            int result = ((IComparable<Base1_.QualifiedID>) memberid).CompareTo(other.memberid);
            if (result == 0)
            {
                result = ((IComparable<Base1_.QualifiedID>)containerid).CompareTo(other.containerid);
                if (result == 0)
                    result = instanceid.CompareTo(other.instanceid);
            }
            return result;
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            MembershipID other = obj as MembershipID;
            return ((IComparable<MembershipID>)this).CompareTo(other);
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Backbone_Scope_MembershipID);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)memberid).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)containerid).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt64(ref info);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)memberid).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)containerid).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt64(ref header, ref data, instanceid);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            memberid = new QS._qss_x_.Base1_.QualifiedID();
            ((QS.Fx.Serialization.ISerializable)memberid).DeserializeFrom(ref header, ref data);
            containerid = new QS._qss_x_.Base1_.QualifiedID();
            ((QS.Fx.Serialization.ISerializable)containerid).DeserializeFrom(ref header, ref data);
            instanceid = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt64(ref header, ref data);
        }

        #endregion

        #region FromString

        public static MembershipID FromString(string s)
        {
            int index1 = s.IndexOf('@');
            int index2 = s.IndexOf('/');
            return new MembershipID(Base1_.QualifiedID.FromString(s.Substring(0, index1)),
                Base1_.QualifiedID.FromString(s.Substring(index1 + 1, index2 - (index1 + 1))), Convert.ToUInt64(s.Substring(index2 + 1)));
        }

        #endregion
    }
}
