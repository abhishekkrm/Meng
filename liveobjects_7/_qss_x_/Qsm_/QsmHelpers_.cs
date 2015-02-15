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
    public static class QsmHelpers_
    {
        #region _Serialize

        public unsafe static void _Serialize(QS.Fx.Serialization.ISerializable _object, out int _datalength, out IList<QS.Fx.Base.Block> _datablocks)
        {
            if (_object != null)
            {
                QS.Fx.Serialization.SerializableInfo _info = _object.SerializableInfo;
                QS.Fx.Base.ConsumableBlock _header =
                    new QS.Fx.Base.ConsumableBlock((uint)(sizeof(ushort) + 2 * sizeof(int) + _info.HeaderSize));
                _datalength = sizeof(ushort) + 2 * sizeof(int) + _info.Size;
                _datablocks = new List<QS.Fx.Base.Block>(_info.NumberOfBuffers + 1);
                _datablocks.Add(_header.Block);
                fixed (byte* _headerptr = _header.Array)
                {
                    *((ushort*)(_headerptr)) = _info.ClassID;
                    *((int*)(_headerptr + sizeof(ushort))) = _info.HeaderSize;
                    *((int*)(_headerptr + sizeof(ushort) + sizeof(int))) = _info.Size;
                }
                _header.consume(sizeof(ushort) + 2 * sizeof(int));
                _object.SerializeTo(ref _header, ref _datablocks);
            }
            else
            {
                _datalength = 0;
                _datablocks = null;
            }
        }

        #endregion

        #region _Deserialize

        public unsafe static void _Deserialize(int _datalength, IList<QS.Fx.Base.Block> _datablocks, out QS.Fx.Serialization.ISerializable _object)
        {
            if (_datalength > 0)
            {
                byte[] _buffer = new byte[_datalength];
                int _offset = 0;
                foreach (QS.Fx.Base.Block _datablock in _datablocks)
                {
                    Buffer.BlockCopy(_datablock.buffer, (int) _datablock.offset, _buffer, _offset, (int) _datablock.size);
                    _offset += (int) _datablock.size;
                }
                ushort _classid;
                int _headersize, _size;
                fixed (byte* _pbuffer = _buffer)
                {
                    _classid = *((ushort*)(_pbuffer));
                    _headersize = *((int*)(_pbuffer + sizeof(ushort)));
                    _size = *((int*)(_pbuffer + sizeof(ushort) + sizeof(int)));
                }
                QS.Fx.Base.ConsumableBlock _header =
                    new QS.Fx.Base.ConsumableBlock(_buffer, (uint) (sizeof(ushort) + 2 * sizeof(int)), (uint) _headersize);
                QS.Fx.Base.ConsumableBlock _data =
                    new QS.Fx.Base.ConsumableBlock(_buffer, (uint) (sizeof(ushort) + 2 * sizeof(int) + _headersize), (uint) (_size - _headersize));
                _object = QS._core_c_.Base3.Serializer.CreateObject(_classid);
                _object.DeserializeFrom(ref _header, ref _data);
            }
            else
                _object = null;
        }

        #endregion
    }
}
