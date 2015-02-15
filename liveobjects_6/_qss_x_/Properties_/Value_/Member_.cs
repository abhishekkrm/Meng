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
    [QS.Fx.Printing.Printable("Member", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.MembershipChannel_Member_)]
    public sealed class Member_ : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Member_(
            QS.Fx.Serialization.ISerializable _identifier, 
            bool _operational, 
            QS.Fx.Serialization.ISerializable _incarnation, 
            QS.Fx.Serialization.ISerializable _name,
            QS.Fx.Serialization.ISerializable[] _addresses)
        {
            this._identifier = _identifier;
            this._operational = _operational;
            this._incarnation = _incarnation;
            this._name = _name;
            this._addresses = _addresses;
        }

        public Member_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("identifier")]
        private QS.Fx.Serialization.ISerializable _identifier;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("operational")]
        private bool _operational;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("incarnation")]
        private QS.Fx.Serialization.ISerializable _incarnation;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("name")]
        private QS.Fx.Serialization.ISerializable _name;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("addresses")]
        private QS.Fx.Serialization.ISerializable[] _addresses;

        #endregion

        #region Accessors

        public QS.Fx.Serialization.ISerializable Identifier
        {
            get { return this._identifier; }
        }

        public bool Operational
        {
            get { return this._operational; }
        }

        public QS.Fx.Serialization.ISerializable Incarnation
        {
            get { return this._incarnation; }
        }

        public QS.Fx.Serialization.ISerializable Name
        {
            get { return this._name; }
        }

        public QS.Fx.Serialization.ISerializable[] Addresses
        {
            get { return this._addresses; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.MembershipChannel_Member_,
                        sizeof(bool) + sizeof(uint) + (3 + ((this._addresses != null) ? this._addresses.Length : 0)) * sizeof(ushort));
                if (this._identifier != null)
                    _info.AddAnother(this._identifier.SerializableInfo);
                if (this._incarnation != null)
                    _info.AddAnother(this._incarnation.SerializableInfo);
                if (this._name != null)
                    _info.AddAnother(this._name.SerializableInfo);
                if (this._addresses != null)
                    foreach (QS.Fx.Serialization.ISerializable _address in this._addresses)
                        _info.AddAnother(_address.SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;
                *((ushort*)_pheader) = ((this._identifier != null) ? this._identifier.SerializableInfo.ClassID : (ushort) 0);
                _pheader += sizeof(ushort);
                *((ushort*)_pheader) = ((this._incarnation != null) ? this._incarnation.SerializableInfo.ClassID : (ushort) 0);
                _pheader += sizeof(ushort);
                *((ushort*)_pheader) = ((this._name != null) ? this._name.SerializableInfo.ClassID : (ushort) 0);
                _pheader += sizeof(ushort);
                *((bool*)_pheader) = this._operational;
                _pheader += sizeof(bool);
                if (this._addresses != null)
                {
                    *((uint*)_pheader) = (uint)this._addresses.Length;
                    _pheader += sizeof(uint);
                    foreach (QS.Fx.Serialization.ISerializable _address in this._addresses)
                    {
                        *((ushort*)_pheader) = _address.SerializableInfo.ClassID;
                        _pheader += sizeof(ushort);
                    }
                }
                else
                    *((uint*)_pheader) = 0;
            }
            _header.consume(sizeof(bool) + sizeof(uint) + (3 + ((this._addresses != null) ? this._addresses.Length : 0)) * sizeof(ushort));
            if (this._identifier != null)
                this._identifier.SerializeTo(ref _header, ref _data);
            if (this._incarnation != null)
                this._incarnation.SerializeTo(ref _header, ref _data);
            if (this._name != null)
                this._name.SerializeTo(ref _header, ref _data);
            if (this._addresses != null)
                foreach (QS.Fx.Serialization.ISerializable _address in this._addresses)
                    _address.SerializeTo(ref _header, ref _data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;
                ushort _classid_identifier = *((ushort*)_pheader);
                this._identifier = (_classid_identifier != 0) ? QS._core_c_.Base3.Serializer.CreateObject(_classid_identifier) : null;
                _pheader += sizeof(ushort);
                ushort _classid_incarnation = *((ushort*)_pheader);
                _pheader += sizeof(ushort);
                this._incarnation = (_classid_incarnation != 0) ? QS._core_c_.Base3.Serializer.CreateObject(_classid_incarnation) : null;
                ushort _classid_name = *((ushort*)_pheader);
                _pheader += sizeof(ushort);
                this._name = (_classid_name != 0) ? QS._core_c_.Base3.Serializer.CreateObject(_classid_name) : null;
                this._operational = *((bool*)_pheader);
                _pheader += sizeof(bool);
                int _addresses_length = (int)(*((uint*)_pheader));
                _pheader += sizeof(uint);
                if (_addresses_length > 0)
                {
                    this._addresses = new QS.Fx.Serialization.ISerializable[_addresses_length];
                    for (int _i = 0; _i < _addresses_length; _i++)
                    {
                        ushort _classid_address = *((ushort*)_pheader);
                        _pheader += sizeof(ushort);
                        this._addresses[_i] = QS._core_c_.Base3.Serializer.CreateObject(_classid_address);
                    }
                }
                else
                    this._addresses = null;
            }
            _header.consume(sizeof(bool) + sizeof(uint) + (3 + ((this._addresses != null) ? this._addresses.Length : 0)) * sizeof(ushort));
            if (this._identifier != null)
                this._identifier.DeserializeFrom(ref _header, ref _data);
            if (this._incarnation != null)
                this._incarnation.DeserializeFrom(ref _header, ref _data);
            if (this._name != null)
                this._name.DeserializeFrom(ref _header, ref _data);
            if (this._addresses != null)
                foreach (QS.Fx.Serialization.ISerializable _address in this._addresses)
                    _address.DeserializeFrom(ref _header, ref _data);
        }

        #endregion
    }
}
