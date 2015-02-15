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
    [QS.Fx.Printing.Printable("Membership", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.MembershipChannel_Membership_)]
    public sealed class Membership_ : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Membership_(QS.Fx.Serialization.ISerializable _incarnation, bool _incremental, Member_[] _members)
        {
            this._incarnation = _incarnation;
            this._incremental = _incremental;
            this._members = _members;
        }

        public Membership_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("incarnation")]
        private QS.Fx.Serialization.ISerializable _incarnation;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("incremental")]
        private bool _incremental;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable("members")]
        private Member_[] _members;

        #endregion

        #region Accessors

        public QS.Fx.Serialization.ISerializable Incarnation
        {
            get { return this._incarnation; }
        }

        public bool Incremental
        {
            get { return this._incremental; }
        }

        public Member_[] Members
        {
            get { return this._members; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.MembershipChannel_Membership_, sizeof(bool) + sizeof(uint) + sizeof(ushort));
                _info.AddAnother(this._incarnation.SerializableInfo);
                if (this._members != null)
                    foreach (Member_ _member in this._members)
                        _info.AddAnother(((QS.Fx.Serialization.ISerializable)_member).SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;
                *((ushort*)_pheader) = this._incarnation.SerializableInfo.ClassID;
                _pheader += sizeof(ushort);
                *((bool*)_pheader) = this._incremental;
                _pheader += sizeof(bool);
                *((uint*)_pheader) = ((this._members != null) ? ((uint)this._members.Length) : 0);
            }
            _header.consume(sizeof(bool) + sizeof(uint) + sizeof(ushort));
            this._incarnation.SerializeTo(ref _header, ref _data);
            if (this._members != null)
                foreach (Member_ _member in this._members)
                    ((QS.Fx.Serialization.ISerializable)_member).SerializeTo(ref _header, ref _data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            int _members_length;
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;
                ushort _classid_incarnation = *((ushort*)_pheader);
                _pheader += sizeof(ushort);
                this._incarnation = QS._core_c_.Base3.Serializer.CreateObject(_classid_incarnation);
                this._incremental = *((bool*)_pheader);
                _pheader += sizeof(bool);
                _members_length = (int)(*((uint*)_pheader));
                if (_members_length > 0)
                    this._members = new Member_[_members_length];
                else
                    this._members = null;
            }
            _header.consume(sizeof(bool) + sizeof(uint) + sizeof(ushort));
            this._incarnation.DeserializeFrom(ref _header, ref _data);
            if (this._members != null)
            {
                for (int _i = 0; _i < _members_length; _i++)
                {
                    Member_ _member = new Member_();
                    ((QS.Fx.Serialization.ISerializable) _member).DeserializeFrom(ref _header, ref _data);
                    this._members[_i] = _member;
                }
            }
        }

        #endregion
    }
}
