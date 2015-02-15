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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_index)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Index)]
    [XmlType(TypeName = "Index")]
    public sealed class Index 
        : QS.Fx.Serialization.ISerializable, IComparable<Index>, IComparable, IEquatable<Index>, IIncrementable<Index>, QS.Fx.Serialization.IStringSerializable
    {
        #region Constructor

        public Index(uint number)
        {
            this.number = number;
        }

        public Index(int number)
        {
            this.number = (uint) number;
        }

        public Index()
        {
        }

        #endregion

        #region Fields

        private uint number;

        #endregion

        #region Casting

        public static explicit operator uint (Index i)
        {
            return i.number;
        }

        #endregion

        #region Accessors

        [XmlAttribute("value")]
        public string String
        {
            get { return this.number.ToString(); }
            set { this.number = Convert.ToUInt32(value); }
        }

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return this.number.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj is Index) && (((Index) obj).number == this.number);
        }

        public override int GetHashCode()
        {
            return this.number.GetHashCode();
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object other)
        {
            if (other is Index)
                return this.number.CompareTo(((Index)other).number);
            else
                throw new Exception("Cannot compare with an object that is not an Index.");
        }

        #endregion

        #region IComparable<Index> Members

        public int CompareTo(Index other)
        {
            return this.number.CompareTo(other.number);
        }

        #endregion

        #region IEquatable<Index> Members

        bool IEquatable<Index>.Equals(Index other)
        {
            return other.number == this.number;
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Index, sizeof(uint), sizeof(uint), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* bufferptr = header.Array)
            {
                *((uint*)(bufferptr + header.Offset)) = this.number;
            }
            header.consume(sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* bufferptr = header.Array)
            {
                this.number = *((uint*)(bufferptr + header.Offset));
            }
            header.consume(sizeof(uint));
        }

        #endregion

        #region IIncrementable<Index> Members

        Index IIncrementable<Index>.Incremented
        {
            get { return new Index(this.number + (uint) 1); }
        }

        #endregion

        #region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
        {
            get { return (ushort)QS.ClassID.Index; }
        }

        string QS.Fx.Serialization.IStringSerializable.AsString
        {
            get { return this.String; }
            set { this.String = value; }
        }

        #endregion
    }
}
