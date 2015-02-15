/*

Copyright (c) 2009 Rinku Agarwal. All rights reserved.

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
using System.Linq;
using System.Text;

namespace QS._qss_x_.Values_
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.SwitchMessage_)]
    [QS.Fx.Reflection.ValueClass("E38768F31BD743729FF637FB82FE59DE", "SwitchMessage_", "")]
    public sealed class SwitchMessage_   
       
        :QS.Fx.Serialization.ISerializable
    {
        public SwitchMessage_(QS.Fx.Serialization.ISerializable _m, int _msgID, int _type, int _index)
        {
            if ((_m != null) && !(_m is QS.Fx.Serialization.ISerializable))
            {
                throw new Exception("MessageClass must be ISerializable!");
            }

            this._o = _m;
            this._msgID = _msgID;
            this._type = _type;
            this._index = _index;
        }

        public SwitchMessage_() {
            this._o = null;
            this._msgID = 0;
            this._type = 0;
            this._index = 0;
        } // serialization constructor

        public int _index;
        public QS.Fx.Serialization.ISerializable _o;
        public int _type;
        public int _msgID;

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.SwitchMessage_, sizeof(int) + sizeof(int) + sizeof(ushort) + sizeof(int));
                if (this._o != null)
                    _info.AddAnother(this._o.SerializableInfo);

                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;
                *((int*)_pheader) = this._msgID;
                _pheader += sizeof(int);
                *((int*)_pheader) = this._type;
                _pheader += sizeof(int);
                *((int*)_pheader) = this._index;
                _pheader += sizeof(int);
                *((ushort*)_pheader) = ((this._o != null) ? this._o.SerializableInfo.ClassID : (ushort)0);

                _header.consume(sizeof(int) + sizeof(int) + sizeof(ushort) + sizeof(int));
            }
            if (this._o != null)
                this._o.SerializeTo(ref _header, ref _data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            ushort classID = 0;

            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;

                int _msgID = *((int*)_pheader);
                _pheader += sizeof(int);

                int _type = *((int*)_pheader);
                _pheader += sizeof(int);

                int _index = *((int*)_pheader);
                _pheader += sizeof(int);

                classID = *((ushort*)_pheader);
                _pheader += sizeof(ushort);

                _header.consume(sizeof(int) + sizeof(int) + sizeof(ushort) + sizeof(int));

                this._msgID = _msgID;
                this._type = _type;
                this._index = _index;
            }

            if (classID != 0)
            {
                this._o = QS.Fx.Serialization.Serializer.Internal.CreateObject(classID);
            }
            else
            {
                this._o = null;                
            }
            if (this._o != null)
                this._o.DeserializeFrom(ref _header, ref _data);
        }
        #endregion
    }
}
