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
    [QS.Fx.Printing.Printable("KVSetToken", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.KVSetToken_)]
    [QS.Fx.Reflection.ValueClass("B22880EF4A5C4a328C82CE2495F6B3EF", "KVSetToken_", "")]
    public sealed class KVSetToken_
        : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public KVSetToken_()
        {
            this._tokens = new List<KVToken_>();
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private List<KVToken_> _tokens;

        #endregion

        #region Accessors

        public List<KVToken_> Tokens
        {
            get { return this._tokens; }
        }

        #endregion

        public void Add(KVToken_ _token)
        {
            this._tokens.Add(_token);
        }

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.KVSetToken_, sizeof(int));

                foreach (KVToken_ item in this._tokens)
                    _info.AddAnother(((QS.Fx.Serialization.ISerializable)item).SerializableInfo);

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

                    *((int*)_pheader) = this._tokens.Count;
                    _pheader += sizeof(int);
                    _header.consume(sizeof(int));

                    foreach (KVToken_ item in this._tokens)
                    {
                        ((QS.Fx.Serialization.ISerializable)item).SerializeTo(ref _header, ref _data);
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

                for (int i = 0; i < count; i++)
                {
                    QS.Fx.Serialization.ISerializable item = new KVToken_();
                    item.DeserializeFrom(ref _header, ref _data);
                    this._tokens.Add((KVToken_)item);
                }
            }
        }

        #endregion

    }
}
