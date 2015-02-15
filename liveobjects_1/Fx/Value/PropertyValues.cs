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
    [QS.Fx.Printing.Printable("PropertyValues", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_propertyvalues)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.PropertyValues)]
    public sealed class PropertyValues : QS.Fx.Serialization.ISerializable, QS.Fx.Value.Classes.IPropertyValues
    {
        #region Constructor

        public PropertyValues
        (
            PropertyValue[] _properties
        )
        {
            this._properties = _properties;
        }

        public PropertyValues()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("properties")]
        private PropertyValue[] _properties;

        #endregion

        #region Accessors

        public PropertyValue[] _Items
        {
            get { return this._properties; }
        }

        #endregion

        #region IPropertyValues Members

        IEnumerable<QS.Fx.Value.Classes.IPropertyValue> QS.Fx.Value.Classes.IPropertyValues.Items
        {
            get
            {
                QS.Fx.Value.Classes.IPropertyValue[] _properties = new QS.Fx.Value.Classes.IPropertyValue[this._properties.Length];
                this._properties.CopyTo(_properties, 0);
                return _properties;
            }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.PropertyValues, sizeof(ushort));
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            int _length = this._properties.Length;
            fixed (byte* pheader_0 = header.Array)
            {
                byte* pheader = pheader_0 + header.Offset;
                *((ushort*)pheader) = (ushort)_length;
            }
            header.consume(sizeof(ushort));
            for (int _i = 0; _i < _length; _i++)
            {
                ((QS.Fx.Serialization.ISerializable)this._properties[_i]).SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int _length;
            fixed (byte* pheader_0 = header.Array)
            {
                byte* pheader = pheader_0 + header.Offset;
                _length = (int)(*((ushort*)pheader));
            }
            header.consume(sizeof(ushort));
            this._properties = new PropertyValue[_length];
            for (int _i = 0; _i < _length; _i++)
            {
                this._properties[_i] = new PropertyValue();
                ((QS.Fx.Serialization.ISerializable)this._properties[_i]).DeserializeFrom(ref header, ref data);
            }
        }

        #endregion
    }
}
