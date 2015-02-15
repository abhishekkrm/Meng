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
    public sealed class QsmOutgoing_  : 
        QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>,
        QS.Fx.Serialization.ISerializable
    {
        public QsmOutgoing_(long _channel, int _datalength, IList<QS.Fx.Base.Block> _datablocks,
            QS._core_c_.Base6.CompletionCallback<QsmOutgoing_> _completioncallback)
        {
            this._channel = _channel;
            this._datalength = _datalength;
            this._datablocks = _datablocks;
            this._completioncallback = _completioncallback;
        }

        [QS.Fx.Printing.Printable]
        public long _channel;
        [QS.Fx.Printing.Printable]
        public int _datalength;
        [QS.Fx.Printing.Printable]
        public IList<QS.Fx.Base.Block> _datablocks;

        public QS._core_c_.Base6.CompletionCallback<QsmOutgoing_> _completioncallback;
        public QsmOutgoing_ _link;

        private static readonly QS._core_c_.Base6.CompletionCallback<object> _internalcompletioncallback =
            new QS._core_c_.Base6.CompletionCallback<object>(_CompletionCallback);

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = 
                    new QS.Fx.Serialization.SerializableInfo(
                        (ushort) QS.ClassID.QsmObject, 
                        sizeof(long) + sizeof(int),
                        sizeof(long) + sizeof(int) + this._datalength,
                        (this._datablocks != null) ? this._datablocks.Count : 0);
                return _info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _parray = _header.Array)
            {
                byte* _pheader = _parray + _header.Offset;
                *((long*)_pheader) = this._channel;
                _pheader += sizeof(long);
                *((int*)_pheader) = this._datalength;
            }
            _header.consume(sizeof(long) + sizeof(int));
            if (this._datalength > 0)
            {
                foreach (QS.Fx.Base.Block _datablock in this._datablocks)
                    _data.Add(_datablock);
            }
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            throw new NotSupportedException();
        }

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get { return new QS._core_c_.Base3.Message((uint) QS.ReservedObjectID.QsmChannel, this); }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return this; }
        }

        QS._core_c_.Base6.CompletionCallback<object> 
            QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return _internalcompletioncallback; }
        }

        private static void _CompletionCallback(bool _succeded, Exception _exception, object _context)
        {
            QsmOutgoing_ _outgoingobject = (QsmOutgoing_) _context;
            if (_outgoingobject != null)
                _outgoingobject._completioncallback(_succeded, _exception, _outgoingobject);
            else
                throw new ArgumentException();
        }
    }
}
