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

namespace QS._qss_c_.Multicasting7
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting7_MessageRV2)]
    public sealed class MessageRV2 : QS.Fx.Serialization.ISerializable, IMessageRV2
    {
        public MessageRV2()
        {
        }

        public MessageRV2(Base3_.RVID[] regionViewIDs, uint[] messageSeqNos, QS._core_c_.Base3.Message message)
        {
            if (regionViewIDs.Length != messageSeqNos.Length)
                throw new Exception("Bad arguments");

            this.regionViewIDs = regionViewIDs;
            this.messageSeqNos = messageSeqNos;
            this.message = message;
        }

        private Base3_.RVID[] regionViewIDs;
        private uint[] messageSeqNos;
        private QS._core_c_.Base3.Message message;

        #region Accessors

        [QS.Fx.Printing.Printable]
        public QS._core_c_.Base3.Message EncapsulatedMessage
        {
            get { return message; }
        }

        [QS.Fx.Printing.Printable]
        public Base3_.RVID[] RVIDs
        {
            get { return regionViewIDs; }
        }

        [QS.Fx.Printing.Printable]
        public uint[] SeqNos
        {
            get { return messageSeqNos; }
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Multicasting7_MessageRV2, sizeof(ushort) + messageSeqNos.Length * sizeof(uint));
                for (int ind = 0; ind < regionViewIDs.Length; ind++)
                    info.AddAnother(regionViewIDs[ind].SerializableInfo);
                info.AddAnother(message.SerializableInfo);
                return info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((ushort*) headerptr) = (ushort) messageSeqNos.Length;
                headerptr += sizeof(ushort);
                for (int ind = 0; ind < messageSeqNos.Length; ind++)
                {
                    *((uint*)headerptr) = messageSeqNos[ind];
                    headerptr += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + messageSeqNos.Length * sizeof(uint));
            for (int ind = 0; ind < messageSeqNos.Length; ind++)
                regionViewIDs[ind].SerializeTo(ref header, ref data);
            message.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                count = (int)(*((ushort*)headerptr));
                headerptr += sizeof(ushort);
                messageSeqNos = new uint[count];
                for (int ind = 0; ind < count; ind++)
                {
                    messageSeqNos[ind] = *((uint*)headerptr);
                    headerptr += sizeof(uint);
                }
            }
            header.consume(sizeof(ushort) + count * sizeof(uint));
            regionViewIDs = new QS._qss_c_.Base3_.RVID[count];
            for (int ind = 0; ind < count; ind++)
            {
                regionViewIDs[ind] = new QS._qss_c_.Base3_.RVID();
                regionViewIDs[ind].DeserializeFrom(ref header, ref data);
            }
            message.DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
/*
            s.Append("(");
            s.Append(regionViewID.ToString());
            s.Append(":");
            s.Append(messageSeqNo.ToString());
            s.Append(" ");
            s.Append(message.ToString());
            s.Append(")");
*/ 
            return s.ToString();
        }

        #endregion
    }
}
