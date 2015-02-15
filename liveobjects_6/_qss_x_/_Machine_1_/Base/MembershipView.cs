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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Base_MembershipView)]
    public class MembershipView : IMembershipView, QS.Fx.Serialization.ISerializable, IEquatable<MembershipView>
    {
        #region Constructors

        public static MembershipView None
        {
            get { return new MembershipView(0, 0, new MemberInfo[0], 0, 0); }
        }

        public MembershipView(uint incarnation, uint seqno, MemberInfo[] members, double readQuorum, double writeQuorum)
        {
            this.incarnation = incarnation;
            this.seqno = seqno;
            this.members = members;
            this.readQuorum = readQuorum;
            this.writeQuorum = writeQuorum;
        }

        public MembershipView()
        {
        }

        #endregion

        #region Fields

        private uint incarnation, seqno;
        private MemberInfo[] members;
        private double readQuorum, writeQuorum;

        #endregion

        #region Accessors

        [QS.Fx.Printing.Printable]
        public uint Incarnation
        {
            get { return incarnation; }
        }

        [QS.Fx.Printing.Printable]
        public uint SeqNo
        {
            get { return seqno; }
        }

        [QS.Fx.Printing.Printable]
        public MemberInfo[] Members
        {
            get { return members; }
        }

        [QS.Fx.Printing.Printable]
        public double ReadQuorum
        {
            get { return readQuorum; }
        }

        [QS.Fx.Printing.Printable]
        public double WriteQuorum
        {
            get { return writeQuorum; }
        }

        #endregion

        #region IMembershipView Members

        uint IMembershipView.Incarnation
        {
            get { return incarnation; }
        }

        uint IMembershipView.SeqNo
        {
            get { return seqno; }
        }

        IMemberInfo[] IMembershipView.Members
        {
            get { return members; }
        }

        double IMembershipView.ReadQuorum
        {
            get { return readQuorum; }
        }

        double IMembershipView.WriteQuorum
        {
            get { return writeQuorum; }
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Machine_Base_MembershipView, 0);

                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortArrayOf<MemberInfo>(ref info, members);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Double(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Double(ref info);

                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, incarnation);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, seqno);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortArrayOf<MemberInfo>(ref header, ref data, members);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Double(ref header, ref data, readQuorum);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Double(ref header, ref data, writeQuorum);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            incarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            seqno = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            members = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortArrayOf<MemberInfo>(ref header, ref data);
            readQuorum = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Double(ref header, ref data);
            writeQuorum = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Double(ref header, ref data);
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        public override bool Equals(object obj)
        {
            MembershipView other = obj as MembershipView;
            if (other != null && other.incarnation == incarnation && other.seqno == seqno && other.members.Length == members.Length
                && readQuorum == other.readQuorum && writeQuorum == other.writeQuorum)
            {
                for (int ind = 0; ind < members.Length; ind++)
                {
                    if (!members[ind].Equals(other.members[ind]))
                        return false;
                }

                return true;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            int hashcode = incarnation.GetHashCode() ^ seqno.GetHashCode() ^ readQuorum.GetHashCode() ^ writeQuorum.GetHashCode();
            foreach (MemberInfo member in members)
                hashcode = hashcode ^ member.GetHashCode();
            return hashcode;
        }

        #endregion

        #region IEquatable<MembershipView> Members

        bool IEquatable<MembershipView>.Equals(MembershipView other)
        {
            if (other != null && other.incarnation == incarnation && other.seqno == seqno && other.members.Length == members.Length
                && readQuorum == other.readQuorum && writeQuorum == other.writeQuorum)
            {
                for (int ind = 0; ind < members.Length; ind++)
                {
                    if (!members[ind].Equals(other.members[ind]))
                        return false;
                }

                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
