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

namespace QS._qss_x_.Base1_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_SessionID)]
    public sealed class SessionID : QS.Fx.Serialization.ISerializable, IComparable<SessionID>, IEquatable<SessionID>, IComparable
    {
        #region Constructor

        public SessionID(QS._qss_x_.Base1_.QualifiedID topicid, uint incarnation)
        {
            this.topicid = topicid;
            this.incarnation = incarnation;
        }

        public SessionID()
        {
        }

        #endregion

        #region Fields

        private QS._qss_x_.Base1_.QualifiedID topicid;
        private uint incarnation;

        #endregion

        #region IComparable<SessionID> Members

        int IComparable<SessionID>.CompareTo(SessionID other)
        {
            if (other == null)
                throw new ArgumentNullException();
            int result = ((IComparable<QualifiedID>)topicid).CompareTo(other.topicid);
            if (result == 0)
                result = incarnation.CompareTo(other.incarnation);
            return result;
        }

        #endregion

        #region IEquatable<SessionID> Members

        bool IEquatable<SessionID>.Equals(SessionID other)
        {
            return (other != null) && ((IEquatable<QualifiedID>)topicid).Equals(other.topicid) && incarnation.Equals(other.incarnation);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<SessionID>)this).CompareTo((SessionID) obj);
        }

        #endregion

        #region Overrides from System.Object

        public override bool Equals(object obj)
        {
            return ((IEquatable<SessionID>)this).Equals(obj as SessionID);
        }

        public override int GetHashCode()
        {
            return topicid.GetHashCode() ^ incarnation.GetHashCode();
        }

        public override string ToString()
        {
            return topicid.ToString() + "#" + incarnation.ToString();
        }

        #endregion

        #region FromString

        public static SessionID FromString(string s)
        {
            int ind = s.IndexOf('#');
            QualifiedID t = QualifiedID.FromString(s.Substring(0, ind));
            uint i = Convert.ToUInt32(s.Substring(ind + 1));
            return new SessionID(t, i);
        }

        #endregion

        #region Accessors

        public QualifiedID Topic
        {
            get { return topicid; }
            set
            {
                if (value != null)
                    topicid = value;
                else
                    throw new Exception("Name cannot be null.");
            }
        }

        public uint Incarnation
        {
            get { return incarnation; }
            set { incarnation = value; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Base_SessionID, 0);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) topicid).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable) topicid).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, incarnation);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            topicid = new QualifiedID();
            ((QS.Fx.Serialization.ISerializable) topicid).DeserializeFrom(ref header, ref data);
            incarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
        }

        #endregion
    }
}
