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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Machine_MemberInfo)]
    public sealed class MemberInfo : QS.Fx.Serialization.ISerializable, IEquatable<MemberInfo> // , IMemberInfo
    {
        public MemberInfo(QS.Fx.Base.ID id, uint incarnation, QS._qss_x_.Base1_.Address address, double weight)
        {
            this.id = id;
            this.incarnation = incarnation;
            this.address = address;
            this.weight = weight;
        }

        public MemberInfo()
        {
        }

        private QS.Fx.Base.ID id;
        private uint incarnation;
        private QS._qss_x_.Base1_.Address address;
        private double weight;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public QS.Fx.Base.ID ID
        {
            get { return id; }
        }

        [QS.Fx.Printing.Printable]
        public uint Incarnation
        {
            get { return incarnation; }
        }

        [QS.Fx.Printing.Printable]
        public QS._qss_x_.Base1_.Address Address
        {
            get { return address; }
        }

        [QS.Fx.Printing.Printable]
        public double Weight
        {
            get { return weight; }
        }

        #endregion

        #region Overrides from Object

        public override bool Equals(object obj)
        {
            return ((IEquatable<MemberInfo>)this).Equals(obj as MemberInfo);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ address.GetHashCode() ^ incarnation.GetHashCode() ^ weight.GetHashCode();
        }

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region IEquatable<MemberInfo> Members

        bool IEquatable<MemberInfo>.Equals(MemberInfo other)
        {
            return (other != null) ? (((IEquatable<QS.Fx.Base.ID>)id).Equals(other.id) && (incarnation == other.incarnation)
                && ((IEquatable<QS._qss_x_.Base1_.Address>)address).Equals(other.address) && (weight == other.weight)) : false;
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Machine_MemberInfo, sizeof(uint) + sizeof(double));
                info.AddAnother(((QS.Fx.Serialization.ISerializable) id).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) address).SerializableInfo);
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
                *((double*)pheader) = weight;
            }
            header.consume(sizeof(uint) + sizeof(double));
            ((QS.Fx.Serialization.ISerializable) id).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) address).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                incarnation = *((uint*)pheader);
                pheader += sizeof(uint);
                weight = *((double*)pheader);
            }
            header.consume(sizeof(uint) + sizeof(double));
            id = new QS.Fx.Base.ID();
            address = new QS._qss_x_.Base1_.Address();
            ((QS.Fx.Serialization.ISerializable) id).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) address).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
