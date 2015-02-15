/* Copyright (c) 2010 Colin Barth. All rights reserved.

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
    [QS.Fx.Printing.Printable("MsgsForEpochToken", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.MsgsForEpochToken)]
    [QS.Fx.Reflection.ValueClass("E2487DCEC0A54da3928BC875662D569F", "MsgsForEpochToken", "")]
    public sealed class MsgsForEpochToken
        : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public MsgsForEpochToken(QS.Fx.Base.Index epoch,
            List<QS.Fx.Serialization.ISerializable> msgList, bool containsData)
        {
            this.epoch = epoch;
            this.msgs = new TokenArray_();
            this.msgs.Add(msgList);
            this.containsMessageData = containsData;
        }

        public MsgsForEpochToken() { }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index epoch;
        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Value_.TokenArray_ msgs;

        private bool containsMessageData;


        #endregion

        #region Accessors

        public QS.Fx.Base.Index Epoch
        {
            get { return this.epoch; }
        }

        public List<QS.Fx.Serialization.ISerializable> Messages
        {
            get { return this.msgs.Tokens; }
        }

        public bool ContainsData
        {
            get { return this.containsMessageData; }
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                //Need to update this to reflect new class ID.
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.MsgsForEpochToken, sizeof(ushort));
                _info.AddAnother(this.epoch.SerializableInfo);
                _info.AddAnother(this.msgs.SerializableInfo);            
                return _info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;
                byte hasData = 0;
                if (containsMessageData)
                    hasData = 1;
                *((byte*)_pheader) = hasData;
                _pheader += sizeof(byte);
                _header.consume(sizeof(byte));
            }
            this.epoch.SerializeTo(ref _header, ref _data);
            this.msgs.SerializeTo(ref _header, ref _data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            fixed (byte* _pheader_0 = _header.Array)
            {
                byte* _pheader = _pheader_0 + _header.Offset;

                byte hasData = *((byte*)_pheader);
                _pheader += sizeof(byte);
                _header.consume(sizeof(byte));
                this.containsMessageData = (hasData == 1);
            }
            this.epoch = new QS.Fx.Base.Index();
            this.epoch.DeserializeFrom(ref _header, ref _data);
            this.msgs = new TokenArray_();
            this.msgs.DeserializeFrom(ref _header, ref _data);
        }

        #endregion

    }
}
