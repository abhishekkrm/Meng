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
    [QS.Fx.Printing.Printable("ReliableMsgSetToken", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.ReliableMsgSetToken)]
    [QS.Fx.Reflection.ValueClass("9F6030219C0A46f1B445D7B04D178AD2", "ReliableMsgSetToken", "")]
    public sealed class ReliableMsgSetToken
        : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public ReliableMsgSetToken (QS.Fx.Base.Identifier originalID, QS.Fx.Base.Identifier senderID,
            List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken> msgList)
        {
            this.originalID = originalID;
            this.senderID = senderID;
            this.msgList = msgList;
            this.msgs = new TokenArray_();
            foreach (QS._qss_x_.Properties_.Value_.MsgsForEpochToken t in this.msgList)
            {
                this.msgs.Add(t);
            }
        }

        public ReliableMsgSetToken() { }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private List<MsgsForEpochToken> msgList;
        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier originalID;
        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier senderID;
        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Value_.TokenArray_ msgs;

        #endregion

        #region Accessors

        public QS.Fx.Base.Identifier OriginalId
        {
            get { return this.originalID; }
        }

        public QS.Fx.Base.Identifier SenderId
        {
            get { return this.senderID; }
        }

        public List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken> Messages
        {
            get { return this.msgList; }
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                //Need to update this to reflect new class ID.
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.ReliableMsgSetToken, sizeof(ushort));
                _info.AddAnother(this.originalID.SerializableInfo);
                _info.AddAnother(this.senderID.SerializableInfo);
                _info.AddAnother(this.msgs.SerializableInfo);            
                return _info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            this.originalID.SerializeTo(ref _header, ref _data);
            this.senderID.SerializeTo(ref _header, ref _data);
            this.msgs.SerializeTo(ref _header, ref _data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {         
            this.originalID = new QS.Fx.Base.Identifier();
            this.originalID.DeserializeFrom(ref _header, ref _data);
            this.senderID = new QS.Fx.Base.Identifier();
            this.senderID.DeserializeFrom(ref _header, ref _data);
            this.msgs = new TokenArray_();
            this.msgs.DeserializeFrom(ref _header, ref _data);
            this.msgList = new List<MsgsForEpochToken>();
            foreach (QS.Fx.Serialization.ISerializable m in msgs.Tokens)
            {
                QS._qss_x_.Properties_.Value_.MsgsForEpochToken t = (QS._qss_x_.Properties_.Value_.MsgsForEpochToken)m;
                this.msgList.Add(t);
            }
        }

        #endregion

    }
}
