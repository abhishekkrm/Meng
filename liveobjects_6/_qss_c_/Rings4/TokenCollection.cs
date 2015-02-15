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

namespace QS._qss_c_.Rings4
{
    [QS.Fx.Serialization.ClassID(ClassID.Rings4_TokenCollection)]
    public class TokenCollection : QS.Fx.Serialization.ISerializable
    {
        public TokenCollection(QS._core_c_.Base3.InstanceID creatorAddress, uint seqno)
        {
            this.creatorAddress = creatorAddress;
            this.seqno = seqno;
        }

        public TokenCollection()
        {
        }

        private QS._core_c_.Base3.InstanceID creatorAddress;
        private IDictionary<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> tokens = 
            new Dictionary<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable>();
        private uint seqno;

        #region Accessors

        public uint SeqNo
        {
            get { return seqno; }
        }

        public IDictionary<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> Tokens
        {
            get { return tokens; }
        }

        public QS._core_c_.Base3.InstanceID CreatorAddress
        {
            get { return creatorAddress; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int size = sizeof(ushort) + sizeof(uint) + creatorAddress.SerializableInfo.Size + tokens.Count * sizeof(ushort);
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> element in tokens)
                    size += element.Key.SerializableInfo.Size + element.Value.SerializableInfo.Size;
                return new QS.Fx.Serialization.SerializableInfo((ushort) ClassID.Rings4_TokenCollection, (ushort)size, size, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            creatorAddress.SerializeTo(ref header, ref data);
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer + header.Offset)) = (ushort) tokens.Count;
                *((uint*)(pbuffer + header.Offset + sizeof(ushort))) = seqno;
            }
            header.consume(sizeof(ushort) + sizeof(uint));
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> element in tokens)
            {
                element.Key.SerializeTo(ref header, ref data);
                fixed (byte* pbuffer = header.Array)
                {
                    *((ushort*)(pbuffer + header.Offset)) = element.Value.SerializableInfo.ClassID;
                }
                header.consume(sizeof(ushort));
                element.Value.SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            creatorAddress = new QS._core_c_.Base3.InstanceID();
            creatorAddress.DeserializeFrom(ref header, ref data);
            int nelements;
            fixed (byte* pbuffer = header.Array)
            {
                nelements = (int) (*((ushort*)(pbuffer + header.Offset)));
                seqno = *((uint*)(pbuffer + header.Offset + sizeof(ushort)));
            }
            header.consume(sizeof(ushort) + sizeof(uint));
            tokens = new Dictionary<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable>(nelements);
            while (nelements-- > 0)
            {
                QS._core_c_.Base3.InstanceID iid = new QS._core_c_.Base3.InstanceID();
                iid.DeserializeFrom(ref header, ref data);
                ushort classID;
                fixed (byte* pbuffer = header.Array)
                {
                    classID = *((ushort*)(pbuffer + header.Offset));
                }
                header.consume(sizeof(ushort));
                QS.Fx.Serialization.ISerializable x = QS._core_c_.Base3.Serializer.CreateObject(classID);
                x.DeserializeFrom(ref header, ref data);
                tokens.Add(iid, x);
            }
        }

        #endregion

        #region Printing

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine("Creator = " + creatorAddress.ToString());
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> element in tokens)
                s.AppendLine("Sender " + element.Key.ToString() + " : " + element.Value.ToString());

            return s.ToString();
        }

        #endregion
    }
}
