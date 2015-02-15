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

namespace QS._qss_x_.Qsm_
{
    [QS.Fx.Printing.Printable("QsmControl", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_qsmcontrol)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.QsmControl)]
    public sealed class QsmControl_ : QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public QsmControl_(QsmOperation_ _operation, long _channel, long _connection, int _sequenceno, QS.Fx.Serialization.ISerializable _object)
        {
            this._operation = _operation;
            this._channel = _channel;
            this._connection = _connection;
            this._sequenceno = _sequenceno;
            this._object = _object;
        }

        public QsmControl_()
        {
        }

        #endregion

        #region Fields

        public QsmOperation_ _operation;
        public long _channel, _connection;
        public int _sequenceno;
        public QS.Fx.Serialization.ISerializable _object;
        public QsmControl_ _link;

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = 
                    new QS.Fx.Serialization.SerializableInfo(
                        (ushort) QS.ClassID.QsmControl, 2 * sizeof(long) + 2 * sizeof(int) + sizeof(ushort));
                if (this._object != null)
                    _info.AddAnother(this._object.SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _parray = _header.Array)
            {
                byte* _pheader = _parray + _header.Offset;
                *((int*) _pheader) = (byte) this._operation;
                _pheader += sizeof(int);
                *((long*) _pheader) = this._channel;                
                _pheader += sizeof(long);
                *((long*) _pheader) = this._connection;                
                _pheader += sizeof(long);
                *((int*) _pheader) = this._sequenceno;                
                _pheader += sizeof(int);
                *((ushort*) _pheader) = (this._object != null) ? this._object.SerializableInfo.ClassID : (ushort) 0;
            }
            _header.consume(2 * sizeof(long) + 2 * sizeof(int) + sizeof(ushort));
            if (this._object != null)
                this._object.SerializeTo(ref _header, ref _data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            ushort _classid;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pheader = _parray + _header.Offset;
                this._operation = (QsmOperation_)(*((int*) _pheader));
                _pheader += sizeof(int);
                this._channel = *((long*) _pheader);
                _pheader += sizeof(long);
                this._connection = *((long*) _pheader);
                _pheader += sizeof(long);
                this._sequenceno = *((int*) _pheader);
                _pheader += sizeof(int);
                _classid = *((ushort*) _pheader);
            }
            _header.consume(2 * sizeof(long) + 2 * sizeof(int) + sizeof(ushort));
            if (_classid > 0)
            {
                this._object = QS._core_c_.Base3.Serializer.CreateObject(_classid);
                this._object.DeserializeFrom(ref _header, ref _data);
            }
            else
                this._object = null;
        }

        #endregion
    }
}
