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

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting5_MessageRV)]
    public class MessageRV : QS.Fx.Serialization.ISerializable
    {
        public MessageRV()
        {
        }

        public MessageRV(Base3_.RVID regionViewID, uint messageSeqNo, QS._core_c_.Base3.Message message)
        {
            this.regionViewID = regionViewID;
            this.messageSeqNo = messageSeqNo;
            this.message = message;
        }

        private Base3_.RVID regionViewID;
        private uint messageSeqNo;
        private QS._core_c_.Base3.Message message;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public QS._core_c_.Base3.Message EncapsulatedMessage
        {
            get { return message; }
        }

        [QS.Fx.Printing.Printable]
        public Base3_.RVID RVID
        {
            get { return regionViewID; }
        }

        [QS.Fx.Printing.Printable]
        public uint SeqNo
        {
            get { return messageSeqNo; }
        }

        #endregion

        #region ISerializable Members

        public virtual QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                return regionViewID.SerializableInfo.CombineWith(message.SerializableInfo).Extend(
                    (ushort)ClassID.Multicasting5_MessageRV, (ushort)(sizeof(uint)), 0, 0);
            }
        }

        public virtual unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            regionViewID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((uint*)headerptr) = messageSeqNo;
            }
            header.consume(sizeof(uint));
            message.SerializeTo(ref header, ref data); 
        }

        public virtual unsafe void DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            regionViewID = new QS._qss_c_.Base3_.RVID();
            regionViewID.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                messageSeqNo = *((uint*)headerptr);
            }
            header.consume(sizeof(uint));
            message = new QS._core_c_.Base3.Message();
            message.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("(");
            s.Append(regionViewID.ToString());
            s.Append(":");
            s.Append(messageSeqNo.ToString());
            s.Append(" ");
            s.Append(message.ToString());
            s.Append(")");
            return s.ToString();
        }
    }
}
