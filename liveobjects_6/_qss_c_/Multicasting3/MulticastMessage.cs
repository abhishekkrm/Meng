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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting3_MulticastMessage)]
    public class MulticastMessage : QS.Fx.Serialization.ISerializable
    {
        public MulticastMessage()
        {
        }

        public MulticastMessage(MessageID messageID, QS._core_c_.Base3.Message message)
        {
            this.messageID = messageID;
            this.message = message;
        }

        private MessageID messageID;
        private QS._core_c_.Base3.Message message;

        public MessageID ID
        {
            get { return messageID; }
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
                QS.Fx.Serialization.SerializableInfo info = ((QS.Fx.Serialization.ISerializable)messageID).SerializableInfo.CombineWith(message.SerializableInfo);
                info.ClassID = (ushort)ClassID.Multicasting3_MulticastMessage;
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ((QS.Fx.Serialization.ISerializable)messageID).SerializeTo(ref header, ref data);
            message.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            messageID = new MessageID();
            ((QS.Fx.Serialization.ISerializable)messageID).DeserializeFrom(ref header, ref data);
            message.DeserializeFrom(ref header, ref data);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("MulticastMessage(");
            s.Append(messageID.ToString());
            s.Append(", ");
            s.Append(message.ToString());
            s.Append(")");
            return s.ToString();
        }
    }
}
