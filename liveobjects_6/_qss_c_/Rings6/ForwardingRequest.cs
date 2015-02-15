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

namespace QS._qss_c_.Rings6
{
    [QS.Fx.Serialization.ClassID(ClassID.Rings6_ReceivingAgent_Agent_Receiver_Forward)]
    [QS.Fx.Printing.Printable("Forward", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public class ForwardingRequest : QS.Fx.Serialization.ISerializable
    {
        public ForwardingRequest(QS._core_c_.Base3.InstanceID senderAddress, uint seqno, QS._core_c_.Base3.Message message)
        {
            this.senderAddress = senderAddress;
            this.seqno = seqno;
            this.message = message;
        }

        public ForwardingRequest()
        {
        }

        [QS.Fx.Printing.Printable]
        private QS._core_c_.Base3.InstanceID senderAddress;
        [QS.Fx.Printing.Printable]
        private uint seqno;
        [QS.Fx.Printing.Printable]
        private QS._core_c_.Base3.Message message;

        public QS._core_c_.Base3.InstanceID SenderAddress
        {
            get { return senderAddress; }
        }

        public uint SequenceNo
        {
            get { return seqno; }
        }

        public QS._core_c_.Base3.Message Message
        {
            get { return message; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Rings6_ReceivingAgent_Agent_Receiver_Forward, (ushort)sizeof(uint), sizeof(uint), 0);
                info.AddAnother(senderAddress.SerializableInfo);
                info.AddAnother(message.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            senderAddress.SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                *((uint*)(pbuffer + header.Offset)) = seqno;
            }
            header.consume(sizeof(uint));
            message.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            senderAddress = new QS._core_c_.Base3.InstanceID();
            senderAddress.DeserializeFrom(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                seqno = *((uint*)(pbuffer + header.Offset));
            }
            header.consume(sizeof(uint));
            message.DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
