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

namespace QS._qss_x_.Properties_.Value_
{
    [QS.Fx.Printing.Printable("DelegationControl", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.DelegationControl_)]
    public sealed class DelegationControl_ : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public DelegationControl_
        (
            Operation_ _operation,
            QS.Fx.Serialization.ISerializable _identifier,
            string _object
        )
        {
            this._operation = _operation;
            this._identifier = _identifier;
            this._object = _object;
        }

        public DelegationControl_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("operation")]
        private Operation_ _operation;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("identifier")]
        private QS.Fx.Serialization.ISerializable _identifier;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("object")]
        private string _object;

        #endregion

        #region Enum Operation_

        public enum Operation_ : byte
        {
            Register_,
            Unregister_,
            Delegate_,
            Undelegate_
        }

        #endregion

        #region Accessors

        public Operation_ _Operation
        {
            get { return this._operation; }
        }

        public QS.Fx.Serialization.ISerializable _Identifier
        {
            get { return this._identifier; }
        }

        public string _Object
        {
            get { return this._object; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int _length = (this._object != null) ? (2 * this._object.Length) : 0;
                if (_length > (int) ushort.MaxValue)
                    throw new Exception();
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort) QS.ClassID.DelegationControl_, 2 * sizeof(ushort) + sizeof(byte), 2 * sizeof(ushort) + sizeof(byte) + _length, ((_length > 0) ? 1 : 0));
                _info.AddAnother(this._identifier.SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            int _length = (this._object!= null) ? (2 * this._object.Length) : 0;
            if (_length > (int) ushort.MaxValue)
                throw new Exception();
            fixed (byte* _pheader0 = _header.Array)
            {
                byte* _pheader = _pheader0 + _header.Offset;
                *((byte*)_pheader) = (byte)this._operation;
                _pheader += sizeof(byte);
                *((ushort*)_pheader) = (ushort) this._identifier.SerializableInfo.ClassID;
                _pheader += sizeof(ushort);
                *((ushort*)_pheader) = (ushort)_length;
            }
            _header.consume(2 * sizeof(ushort) + sizeof(byte));
            this._identifier.SerializeTo(ref _header, ref _data);
            if (_length > 0)
            {
                byte[] _bytes = Encoding.Unicode.GetBytes(this._object);
                if (_bytes.Length != _length)
                    throw new Exception();
                _data.Add(new QS.Fx.Base.Block(_bytes));
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            ushort _classid;
            int _length;
            fixed (byte* _pheader0 = _header.Array)
            {
                byte* _pheader = _pheader0 + _header.Offset;
                this._operation = (Operation_)(*((byte*)_pheader));
                _pheader += sizeof(byte);
                _classid = (*((ushort*)_pheader));
                _pheader += sizeof(ushort); 
                _length = (int)(*((ushort*)_pheader));
            }
            _header.consume(2 * sizeof(ushort) + sizeof(byte));
            this._identifier = QS._core_c_.Base3.Serializer.CreateObject(_classid);
            this._identifier.DeserializeFrom(ref _header, ref _data);
            if (_length > 0)
            {
                this._object = Encoding.Unicode.GetString(_data.Array, _data.Offset, _length);
                _data.consume(_length);
            }
            else
                this._object = null;
        }

        #endregion
    }
}
