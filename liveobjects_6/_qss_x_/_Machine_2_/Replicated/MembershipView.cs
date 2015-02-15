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

namespace QS._qss_x_._Machine_2_.Replicated
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Machine_MembershipView)]
    public sealed class MembershipView : QS.Fx.Serialization.ISerializable, IEquatable<MembershipView> // , IMembershipView
    {
        public static MembershipView None
        {
            get { return new MembershipView(0, 0, new MemberInfo[0], 0, 0); }
        }

        public MembershipView(uint incarnation, uint number, MemberInfo[] members, double readquorum, double writequorum)
        {
            this.incarnation = incarnation;
            this.number = number;
            this.members = members;
            this.readquorum = readquorum;
            this.writequorum = writequorum;
        }

        public MembershipView()
        {
        }

        private uint incarnation, number;
        private MemberInfo[] members;
        private double readquorum, writequorum;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public uint Incarnation
        {
            get { return incarnation; }
        }

        [QS.Fx.Printing.Printable]
        public uint Number
        {
            get { return number; }
        }

        [QS.Fx.Printing.Printable]
        public MemberInfo[] Members
        {
            get { return members; }
        }

        [QS.Fx.Printing.Printable]
        public double ReadQuorum
        {
            get { return readquorum; }
        }

        [QS.Fx.Printing.Printable]
        public double WriteQuorum
        {
            get { return writequorum; }
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<MembershipView>)this).Equals(obj as MembershipView);
        }

        public override int GetHashCode()
        {
            int hashcode = incarnation.GetHashCode() ^ number.GetHashCode() ^ readquorum.GetHashCode() ^ writequorum.GetHashCode();
            foreach (MemberInfo member in members)
                hashcode = hashcode ^ member.GetHashCode();
            return hashcode;
        }

        #endregion

        #region IEquatable<MembershipView> Members

        bool IEquatable<MembershipView>.Equals(MembershipView other)
        {
            if (other != null && other.incarnation == incarnation && other.number == number && other.members.Length == members.Length
                && readquorum == other.readquorum && writequorum == other.writequorum)
            {
                for (int ind = 0; ind < members.Length; ind++)
                {
                    if (!((IEquatable<MemberInfo>) members[ind]).Equals(other.members[ind]))
                        return false;
                }

                return true;
            }
            else
                return false;
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Machine_MembershipView, 
                        2 * sizeof(uint) + 2 * sizeof(double) + sizeof(ushort));
                foreach (MemberInfo memberinfo in members)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable) memberinfo).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((uint*)pheader) = incarnation;
                pheader += sizeof(uint);
                *((uint*)pheader) = number;
                pheader += sizeof(uint);
                *((double*)pheader) = readquorum;
                pheader += sizeof(double);
                *((double*)pheader) = writequorum;
                pheader += sizeof(double);
                *((ushort*)pheader) = (ushort) members.Length;
            }
            header.consume(2 * sizeof(uint) + 2 * sizeof(double) + sizeof(ushort));
            foreach (MemberInfo memberinfo in members)
                ((QS.Fx.Serialization.ISerializable)memberinfo).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                incarnation = *((uint*)pheader);
                pheader += sizeof(uint);
                number = *((uint*)pheader);
                pheader += sizeof(uint);
                readquorum = *((double*)pheader);
                pheader += sizeof(double);
                writequorum = *((double*)pheader);
                pheader += sizeof(double);
                count = (int)(*((ushort*)pheader));
            }
            header.consume(2 * sizeof(uint) + 2 * sizeof(double) + sizeof(ushort));
            members = new MemberInfo[count];
            for (int index = 0; index < count; index++)
            {
                members[index] = new MemberInfo();
                ((QS.Fx.Serialization.ISerializable) members[index]).DeserializeFrom(ref header, ref data);
            }
        }

        #endregion
    }
}
