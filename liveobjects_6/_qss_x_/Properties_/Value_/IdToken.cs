/* Copyright (c) 2004-2009 Jared Cantwell (jmc279@cornell.edu). All rights reserved.

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
SUCH DAMAGE. */

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Value_
{
    [QS.Fx.Printing.Printable("IdToken", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.IdToken_)]
    [QS.Fx.Reflection.ValueClass("1AB1C22BF2314bbb92710F24200B0091", "IdToken_", "")]
    public sealed class IdToken_
        : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public IdToken_
        (
            QS.Fx.Base.MessageIdentifier _mid,
            QS.Fx.Serialization.ISerializable _payload
        )
        {
            this._mid = _mid;
            this._payload = _payload;
        }

        public IdToken_() { }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.MessageIdentifier _mid;
        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Serialization.ISerializable _payload;

        #endregion

        #region Accessors

        public QS.Fx.Base.MessageIdentifier Id
        {
            get { return this._mid; }
            set { this._mid = value; }
        }

        public QS.Fx.Serialization.ISerializable Payload
        {
            get { return this._payload; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.IdToken_, sizeof(ushort));
                _info.AddAnother(this._mid.SerializableInfo);
                _info.AddAnother(this._payload.SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;

                *((ushort*)_pheader) = ((this._payload != null) ? this._payload.SerializableInfo.ClassID : (ushort)0);
                _header.consume(sizeof(ushort));
            }

            if (this._payload != null)
                this._payload.SerializeTo(ref _header, ref _data);

            this._mid.SerializeTo(ref _header, ref _data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;

                ushort _classid_name = *((ushort*)_pheader);
                _pheader += sizeof(ushort);
                _header.consume(sizeof(ushort));

                this._payload = (_classid_name != 0) ? QS._core_c_.Base3.Serializer.CreateObject(_classid_name) : null;
            }

            if (this._payload != null)
                this._payload.DeserializeFrom(ref _header, ref _data);

            this._mid = new QS.Fx.Base.MessageIdentifier();
            this._mid.DeserializeFrom(ref _header, ref _data);
        }

        #endregion

    }
}

