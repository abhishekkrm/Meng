/*

Copyright (c) 2004-2009 Jared Cantwell. All rights reserved.

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
    [QS.Fx.Printing.Printable("Array", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Array_)]
    [QS.Fx.Reflection.ValueClass("5851B2CB179E422e8E6A0B6A0D9B595A", "Array_", "")]
    public sealed class Array_
        : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Array_()
        {
            this._payloadList = new List<QS.Fx.Serialization.ISerializable>();
        }


        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private List<QS.Fx.Serialization.ISerializable> _payloadList;

        #endregion

        #region Accessors

        public List<QS.Fx.Serialization.ISerializable> Tokens
        {
            get { return this._payloadList; }
        }

        #endregion

        public void Add(QS.Fx.Serialization.ISerializable _token)
        {
            this._payloadList.Add(_token);
            this._payloadList.Sort();
        }

        public void Add(List<QS.Fx.Serialization.ISerializable> _tokens)
        {
            this._payloadList.AddRange(_tokens);
            this._payloadList.Sort();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Array_)) return false;
            Array_ other = (Array_)obj;

            // Invariant, payload list is always kept sorted.
            if (other._payloadList.Count != _payloadList.Count)
                return false;

            for (int i = 0; i < _payloadList.Count; i++)
            {
                if (!_payloadList[i].Equals(other._payloadList[i]))
                    return false;
            }

            return true;
        }

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Array_, sizeof(int) + this._payloadList.Count * sizeof(ushort));

                foreach (QS.Fx.Serialization.ISerializable item in this._payloadList)
                    _info.AddAnother(item.SerializableInfo);

                return _info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            lock (this)
            {
                fixed (byte* _pheader_0 = _header.Array)
                {
                    byte* _pheader = _pheader_0 + _header.Offset;

                    *((int*)_pheader) = this._payloadList.Count;
                    _pheader += sizeof(int);
                    _header.consume(sizeof(int));

                    foreach (QS.Fx.Serialization.ISerializable item in this._payloadList)
                    {
                        *((ushort*)_pheader) = ((item != null) ? item.SerializableInfo.ClassID : (ushort)0);
                        _pheader += sizeof(ushort);
                        _header.consume(sizeof(ushort));
                    }

                    foreach (QS.Fx.Serialization.ISerializable item in this._payloadList)
                    {
                        item.SerializeTo(ref _header, ref _data);
                    }
                }
            }
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;

                int count = *((int*)_pheader);
                _pheader += sizeof(int);
                _header.consume(sizeof(int));

                ushort[] class_ids = new ushort[count];

                QS.Fx.Serialization.ISerializable item;

                for (int i = 0; i < count; i++)
                {
                    ushort _classid_name = *((ushort*)_pheader);
                    _pheader += sizeof(ushort);
                    _header.consume(sizeof(ushort));

                    class_ids[i] = _classid_name;
                }

                for (int i = 0; i < count; i++)
                {
                    ushort _classid_name = class_ids[i];

                    //item = (_classid_name != 0) ? QS._core_c_.Base3.Serializer.CreateObject(_classid_name) : null;
                    item = QS._core_c_.Base3.Serializer.CreateObject(_classid_name);

                    item.DeserializeFrom(ref _header, ref _data);
                    this._payloadList.Add(item);
                }
            }
        }

        #endregion

    }
}
