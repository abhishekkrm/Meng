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

namespace QS.Fx.Value.Classes.Desktop
{
    [QS.Fx.Printing.Printable("Object", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Channel_Message_Desktop_Object)]
    public sealed class DesktopObject : IDesktopObject
    {
        #region Constructors

        public DesktopObject(QS.Fx.Base.ID _id, string _label, double _x1, double _y1, double _x2, double _y2, string _objectxml)
        {
            this._id = _id;
            this._label = new QS.Fx.Value.UnicodeText(_label);
            this._x1 = _x1;
            this._y1 = _y1;
            this._x2 = _x2;
            this._y2 = _y2;
            this._objectxml = new QS.Fx.Value.UnicodeText(_objectxml);
        }

        public DesktopObject()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Base.ID _id;
        private QS.Fx.Value.UnicodeText _label, _objectxml;
        private double _x1, _y1, _x2, _y2;

        #endregion

        #region IObject Members

        QS.Fx.Base.ID IDesktopObject.ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        string IDesktopObject.Label
        {
            get { return ((QS.Fx.Value.Classes.IText)_label).Text; }
            set { _label = new QS.Fx.Value.UnicodeText(value); }
        }

        double IDesktopObject.X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }

        double IDesktopObject.Y1
        {
            get { return _y1; }
            set { _y1 = value; }
        }

        double IDesktopObject.X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }

        double IDesktopObject.Y2
        {
            get { return _y2; }
            set { _y2 = value; }
        }

        string IDesktopObject.ObjectXml
        {
            get { return ((QS.Fx.Value.Classes.IText)_objectxml).Text; }
            set { _objectxml = new QS.Fx.Value.UnicodeText(value); }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = 
                    new QS.Fx.Serialization.SerializableInfo(
                        (ushort) QS.ClassID.Fx_Channel_Message_Desktop_Object, 4 * sizeof(double));
                _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._id).SerializableInfo);
                _info.AddAnother(((QS.Fx.Value.Classes.IText)this._label).SerializableInfo);
                _info.AddAnother(((QS.Fx.Value.Classes.IText)this._objectxml).SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((double *) pheader) = _x1;
                pheader += sizeof(double);
                *((double *) pheader) = _y1;
                pheader += sizeof(double);
                *((double *) pheader) = _x2;
                pheader += sizeof(double);
                *((double *) pheader) = _y2;
            }
            header.consume(4 * sizeof(double));
            ((QS.Fx.Serialization.ISerializable)this._id).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._label).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)this._objectxml).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                _x1 = *((double *) pheader);
                pheader += sizeof(double);
                _y1 = *((double *) pheader);
                pheader += sizeof(double);
                _x2 = *((double *) pheader);
                pheader += sizeof(double);
                _y2 = *((double *) pheader);
            }
            header.consume(4 * sizeof(double));
            this._id = new QS.Fx.Base.ID();
            ((QS.Fx.Serialization.ISerializable)this._id).DeserializeFrom(ref header, ref data);
            this._label = new QS.Fx.Value.UnicodeText();
            ((QS.Fx.Serialization.ISerializable)this._label).DeserializeFrom(ref header, ref data);
            this._objectxml = new QS.Fx.Value.UnicodeText();
            ((QS.Fx.Serialization.ISerializable)this._objectxml).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
