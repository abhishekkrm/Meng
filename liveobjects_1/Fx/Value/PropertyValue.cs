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
    [QS.Fx.Printing.Printable("PropertyValue", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_propertyvalue)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.PropertyValue)]
    public sealed class PropertyValue : QS.Fx.Serialization.ISerializable, QS.Fx.Value.Classes.IPropertyValue
    {
        #region Constructor

        public PropertyValue
        (
            QS.Fx.Base.Index _index,
            PropertyVersion _version,
            QS.Fx.Serialization.ISerializable _value
        )
        {
            this._index = _index;
            this._version = _version;
            this._value = _value;
        }

        public PropertyValue()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("index")]
        private QS.Fx.Base.Index _index;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("version")]
        private PropertyVersion _version;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("value")]
        private QS.Fx.Serialization.ISerializable _value;

        #endregion

        #region Accessors

        public QS.Fx.Base.Index _Index
        {
            get { return this._index; }
        }

        public PropertyVersion _Version
        {
            get { return this._version; }
        }

        public QS.Fx.Serialization.ISerializable _Value
        {
            get { return this._value; }
        }

        #endregion

        #region IProperty Members

        QS.Fx.Base.Index QS.Fx.Value.Classes.IPropertyValue.Index
        {
            get { return this._index; }
        }

        QS.Fx.Value.Classes.IPropertyVersion QS.Fx.Value.Classes.IPropertyValue.Version
        {
            get { return this._version; }
        }

        QS.Fx.Serialization.ISerializable QS.Fx.Value.Classes.IPropertyValue.Value
        {
            get { return this._value; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.PropertyValue, sizeof(ushort));
                info.AddAnother(((QS.Fx.Serialization.ISerializable) this._index).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) this._version).SerializableInfo);
                if (this._value != null)
                    info.AddAnother(this._value.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pheader_0 = header.Array)
            {
                byte* pheader = pheader_0 + header.Offset;
                if (this._value != null)
                    *((ushort*)pheader) = ((QS.Fx.Serialization.ISerializable)this._value).SerializableInfo.ClassID;
                else
                    *((ushort*)pheader) = 0;
            }
            header.consume(sizeof(ushort));
            ((QS.Fx.Serialization.ISerializable)this._index).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._version).SerializeTo(ref header, ref data);
            if (this._value != null)
                ((QS.Fx.Serialization.ISerializable) this._value).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort _valueclassid;
            fixed (byte* _pheader_0 = header.Array)
            {
                byte* _pheader = _pheader_0 + header.Offset;
                _valueclassid = *((ushort*) _pheader);
            }
            header.consume(sizeof(ushort));
            this._index = new QS.Fx.Base.Index();
            this._version = new PropertyVersion();
            ((QS.Fx.Serialization.ISerializable)this._index).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._version).DeserializeFrom(ref header, ref data);
            if (_valueclassid != 0)
            {
                this._value = QS.Fx.Serialization.Serializer.Internal.CreateObject(_valueclassid);
                ((QS.Fx.Serialization.ISerializable)this._value).DeserializeFrom(ref header, ref data);
            }
            else
                this._value = null;
        }

        #endregion
    }
}
