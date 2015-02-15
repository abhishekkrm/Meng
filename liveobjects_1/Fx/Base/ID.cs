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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_id)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Base_ID)]
    [XmlType(TypeName = "ID")]
    public sealed class ID : QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.IStringSerializable, IComparable<ID>, IComparable, IEquatable<ID>
    {
        #region Constructors

        public static ID NewID()
        {
            return new ID(Guid.NewGuid());
        }

        public unsafe ID(string _s) 
        {
            this.num = Int128.FromString(_s);
/*
            int _n = _s.Length;
            if ((_s[0] == '{') && (_s[_n - 1] == '}'))
                this.num = Int128.FromString(_s.Substring(1, _n - 2));
            else
                throw new Exception("Malformed identifier.");
*/
        }

        public unsafe ID(Guid guid) : this(new Int128(guid))
        {
        }

        public unsafe ID(Int128 num)
        {
            this.num = num;
        }

        public ID()
        {
        }

        private static readonly ID undefined = new ID(new Int128());

        public static ID Undefined
        {
            get { return undefined; }
        }

        public static ID _0
        {
            get { return undefined; }
        }

        #endregion

        #region Fields

        private Int128 num;

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Base_ID, 0);
                info.AddAnother(num.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            num.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            num.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
/*
            return "{" + num.ToString().ToUpper() + "}";
*/
            return num.ToString().ToUpper();
        }

        public override int GetHashCode()
        {
            return num.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ID other = obj as ID;
            return (other != null) && num.Equals(other.num);
        }

        #endregion

        #region IComparable<Name> Members

        int IComparable<ID>.CompareTo(ID other)
        {
            if (other != null)
                return num.CompareTo(other.num);
            else
                throw new Exception("The argument for comparison is null.");
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            ID other = obj as ID;
            if (other != null)
                return num.CompareTo(other.num);
            else
                throw new Exception("The argument for comparison is either null or is not a" + GetType().FullName + ".");
        }

        #endregion

        #region IEquatable<Name> Members

        bool IEquatable<ID>.Equals(ID other)
        {
            return num.Equals(other.num);
        }

        #endregion

        #region FromString

        public static ID FromString(string s)
        {
            return new ID(s);
        }

        #endregion

        #region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
        {
            get { return (ushort)QS.ClassID.Fx_Base_ID; }
        }

        string QS.Fx.Serialization.IStringSerializable.AsString
        {
            get { return this.num.ToString(); }
            set { this.num = Int128.FromString(value); }
        }

        #endregion
    }
}
