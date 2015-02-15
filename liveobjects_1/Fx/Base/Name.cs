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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_name)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Name)]
    [XmlType(TypeName = "Name")]
    public sealed class Name : QS.Fx.Serialization.ISerializable, IComparable<Name>, IComparable, IEquatable<Name>
    {
        #region Constructors

        public Name(string name)
        {
            this.name = name;
        }

        public Name()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private string name;

        #endregion

        #region Accessors

        [XmlAttribute("value")]
        public string String
        {
            get { return this.name; }
            set { this.name = value; }
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return (this.name != null) ? this.name : string.Empty;
        }

        public override int GetHashCode()
        {
            return (this.name != null) ? this.name.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            return (obj is Name) && this.name.Equals(((Name) obj).name);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if ((obj == null) || (!(obj is Name)))
                throw new Exception();
            return this.name.CompareTo(((Name) obj).name);
        }

        #endregion

        #region IComparable<Name> Members

        int IComparable<Name>.CompareTo(Name other)
        {
            if (other == null)
                throw new Exception();
            else
                return (name == null) ? ((other.name == null) ? 0 : -1) :
                    ((other.name == null) ? 1 : name.CompareTo(other.name));
        }

        #endregion

        #region IEquatable<Name> Members

        bool IEquatable<Name>.Equals(Name other)
        {
            return (other != null) && ((this.name != null) ? ((other.name != null) && this.name.Equals(other.name)) : (other.name == null));
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int length = (name != null) ? (2 * name.Length) : 0;
                if (length > (int) ushort.MaxValue)
                    throw new Exception();
                return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Name, sizeof(ushort), sizeof(ushort) + length, ((length > 0) ? 1 : 0));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            int length = (name != null) ? (2 * name.Length) : 0;
            if (length > (int) ushort.MaxValue)
                throw new Exception();
            fixed (byte* pheader0 = header.Array)
            {
                byte* pheader = pheader0 + header.Offset;
                *((ushort*) pheader) = (ushort) length;
            }
            header.consume(sizeof(ushort));
            if (length > 0)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(name);
                if (bytes.Length != length)
                    throw new Exception();
                data.Add(new QS.Fx.Base.Block(bytes));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int length;
            fixed (byte* pheader0 = header.Array)
            {
                byte* pheader = pheader0 + header.Offset;
                length = (int)(*((ushort*)pheader));
            }
            header.consume(sizeof(ushort));
            if (length > 0)
            {
                this.name = Encoding.Unicode.GetString(data.Array, data.Offset, length);
                data.consume(length);
            }
            else
                this.name = null;
        }

        #endregion
    }
}
