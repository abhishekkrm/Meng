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

namespace QS.Fx.Value
{
    [QS.Fx.Printing.Printable("PropertyVersion", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_propertyversion)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.PropertyVersion)]
    public sealed class PropertyVersion : QS.Fx.Serialization.ISerializable, QS.Fx.Value.Classes.IPropertyVersion
    {
        #region Constructor

        public PropertyVersion
        (
            QS.Fx.Base.Incarnation incarnation,
            QS.Fx.Base.Index index
        )
        {
            if ((incarnation == null) || (index == null))
                throw new ArgumentException();
            this.incarnation = incarnation;
            this.index = index;
        }

        public PropertyVersion
        (
            ulong incarnation,
            uint index
        )
        {
            this.incarnation = new QS.Fx.Base.Incarnation(incarnation);
            this.index = new QS.Fx.Base.Index(index);
        }

        public PropertyVersion()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("incarnation")]
        private QS.Fx.Base.Incarnation incarnation;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("index")]
        private QS.Fx.Base.Index index;

        #endregion

        #region Accessors

        public QS.Fx.Base.Incarnation Incarnation
        {
            get { return this.incarnation; }
        }

        public QS.Fx.Base.Index Index
        {
            get { return this.index; }
        }

        #endregion

        #region QS.Fx.Value.Classes.IPropertyVersion Members

        QS.Fx.Base.Incarnation QS.Fx.Value.Classes.IPropertyVersion.Incarnation
        {
            get { return this.incarnation; }
        }

        QS.Fx.Base.Index QS.Fx.Value.Classes.IPropertyVersion.Index
        {
            get { return this.index; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.PropertyVersion, 0);
                info.AddAnother(this.incarnation.SerializableInfo);
                info.AddAnother(this.index.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            this.incarnation.SerializeTo(ref header, ref data);
            this.index.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            this.incarnation = new QS.Fx.Base.Incarnation();
            this.incarnation.DeserializeFrom(ref header, ref data);
            this.index = new QS.Fx.Base.Index();
            this.index.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IComparable<IPropertyVersion> Members

        int IComparable<QS.Fx.Value.Classes.IPropertyVersion>.CompareTo(QS.Fx.Value.Classes.IPropertyVersion other)
        {
            int result = this.incarnation.CompareTo(other.Incarnation);
            if (result == 0)
                result = this.index.CompareTo(other.Index);
            return result;
        }

        #endregion

        #region IEquatable<QS.Fx.Value.Classes.IPropertyVersion> Members

        bool IEquatable<QS.Fx.Value.Classes.IPropertyVersion>.Equals(QS.Fx.Value.Classes.IPropertyVersion other)
        {
            return this.incarnation.Equals(other.Incarnation) && this.index.Equals(other.Index);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            QS.Fx.Value.Classes.IPropertyVersion other = obj as QS.Fx.Value.Classes.IPropertyVersion;
            if (other != null)
                return ((IComparable<QS.Fx.Value.Classes.IPropertyVersion>) this).CompareTo(other);
            else
                throw new ArgumentException();
        }

        #endregion

        #region Overridden from System.Object

        public override bool  Equals(object obj)
        {
            QS.Fx.Value.Classes.IPropertyVersion other = obj as QS.Fx.Value.Classes.IPropertyVersion;
            if (other != null)
                return ((IEquatable<QS.Fx.Value.Classes.IPropertyVersion>) this).Equals(other);
            else
                return false; 	         
        }

        public override int  GetHashCode()
        {
 	         return this.incarnation.GetHashCode() ^ this.index.GetHashCode();
        }

        public override string  ToString()
        {
            StringBuilder ss = new StringBuilder();
            ss.Append(this.incarnation.ToString());
            ss.Append(":");
            ss.Append(this.index.ToString());
            return ss.ToString();
        }

        #endregion
    }
}
