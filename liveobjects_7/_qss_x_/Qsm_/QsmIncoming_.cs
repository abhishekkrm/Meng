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
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.QsmObject)]
    public sealed class QsmIncoming_ : QS.Fx.Serialization.ISerializable
    {
        public QsmIncoming_()
        {
        }

        [QS.Fx.Printing.Printable]
        public long _channel;
        [QS.Fx.Printing.Printable]
        public int _datalength;
        [QS.Fx.Printing.Printable]
        public IList<QS.Fx.Base.Block> _datablocks;

        public QsmIncoming_ _link;

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { throw new NotSupportedException(); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            throw new NotSupportedException();
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            fixed (byte* _parray = _header.Array)
            {
                byte* _pheader = _parray + _header.Offset;
                this._channel = *((long*)_pheader);
                _pheader += sizeof(long);
                this._datalength = *((int*)_pheader);
            }
            _header.consume(sizeof(long) + sizeof(int));
            if (this._datalength > 0)
            {
                this._datablocks = new List<QS.Fx.Base.Block>(1);
                QS.Fx.Base.Block _datablock = _data.Block;
                _datablock.offset += (uint) _data.Consumed;
                _datablock.size -= (uint) _data.Consumed;
                this._datablocks.Add(_datablock);
            }
        }
    }
}
