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
using System.Xml.Serialization;

namespace QS.Fx.Base
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses.UINT64)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_Unsigned_64)]
    [XmlType(TypeName = "UINT64")]
    public sealed class UINT64 : QS.Fx.Serialization.ISerializable, IComparable<UINT64>, IComparable, IEquatable<UINT64>, QS.Fx.Serialization.IStringSerializable
    {
        #region Constructor

        public UINT64(ulong number)
        {
            this.number = number;
        }

        public UINT64(string number)
        {
            this.String = number;
        }

        public UINT64()
        {
        }

        #endregion

        #region Fields

        private ulong number;

        #endregion

        #region Accessors

        [XmlIgnore]
        public ulong Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        [XmlAttribute("value")]
        public string String
        {
            get { return this.number.ToString(); }
            set { this.number = Convert.ToUInt64(value); }
        }

        #endregion

        #region Casting

        public static explicit operator ulong(UINT64 u)
        {
            return u.number;
        }

        public static explicit operator UINT64(ulong number)
        {
            return new UINT64(number);
        }

        public static explicit operator UINT64(string number)
        {
            return new UINT64(number);
        }

        public static explicit operator string(UINT64 number)
        {
            return number.String;
        }

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return this.number.ToString("x");
        }

        public override bool Equals(object obj)
        {
            return (obj is UINT64) && (((UINT64) obj).number == this.number);
        }

        public override int GetHashCode()
        {
            return this.number.GetHashCode();
        }

        #endregion

        #region IComparable<Incarnation> Members

        public int CompareTo(UINT64 other)
        {
            return this.number.CompareTo(other.number);
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Base_Unsigned_64, sizeof(ulong), sizeof(ulong), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* bufferptr = header.Array)
            {
                *((ulong*)(bufferptr + header.Offset)) = this.number;
            }
            header.consume(sizeof(ulong));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* bufferptr = header.Array)
            {
                this.number = *((ulong*)(bufferptr + header.Offset));
            }
            header.consume(sizeof(ulong));
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object other)
        {
            if (other is UINT64)
                return this.number.CompareTo(((UINT64) other).number);
            else
                throw new Exception("Cannot compare with an object that is not an Unsigned_64.");
        }

        #endregion

        #region IEquatable<Incarnation> Members

        bool IEquatable<UINT64>.Equals(UINT64 other)
        {
            return other.number == this.number;
        }

        #endregion

        #region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
        {
            get { return (ushort) QS.ClassID.Fx_Base_Unsigned_64; }
        }

        string QS.Fx.Serialization.IStringSerializable.AsString
        {
            get { return this.String; }
            set { this.String = value; }
        }

        #endregion
    }
}
