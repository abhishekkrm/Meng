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

namespace QS._qss_c_.Gossiping2
{
    [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_RingMessageContainer)]
    public class RingMessageContainer : QS.Fx.Serialization.ISerializable
    {
        public RingMessageContainer()
        {
            messageCollection = new System.Collections.Generic.Dictionary<QS._core_c_.Base3.InstanceID, System.Collections.Generic.IList<QS.Fx.Serialization.ISerializable>>();
        }

        private System.Collections.Generic.IDictionary<QS._core_c_.Base3.InstanceID, System.Collections.Generic.IList<QS.Fx.Serialization.ISerializable>> messageCollection;

        public void Add(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable serializableObject)
        {
            if (messageCollection.ContainsKey(address))
                messageCollection[address].Add(serializableObject);
            else 
            {
                System.Collections.Generic.IList<QS.Fx.Serialization.ISerializable> list = new List<QS.Fx.Serialization.ISerializable>();
                list.Add(serializableObject);
                messageCollection.Add(address, list);
            }
        }

        public IDictionary<QS._core_c_.Base3.InstanceID, IList<QS.Fx.Serialization.ISerializable>> MessageCollections
        {
            get { return messageCollection; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int header_overhead = sizeof(ushort) * (messageCollection.Count + 1);
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Gossiping2_RingMessageContainer, 
                    (ushort) header_overhead, header_overhead, 0);
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, System.Collections.Generic.IList<QS.Fx.Serialization.ISerializable>> element in messageCollection)
                {
                    info.AddAnother(element.Key.SerializableInfo);
                    header_overhead = sizeof(ushort) * element.Value.Count;
                    info.HeaderSize += (ushort) header_overhead;
                    info.Size += header_overhead;
                    foreach (QS.Fx.Serialization.ISerializable serializableObject in element.Value)
                        info.AddAnother(serializableObject.SerializableInfo);
                }
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) messageCollection.Count;
            }
            header.consume(sizeof(ushort));
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, System.Collections.Generic.IList<QS.Fx.Serialization.ISerializable>> element in messageCollection)
            {
                element.Key.SerializeTo(ref header, ref data);
                fixed (byte* arrayptr = header.Array)
                {
                    *((ushort*)(arrayptr + header.Offset)) = (ushort) element.Value.Count;
                }
                header.consume(sizeof(ushort));
                foreach (QS.Fx.Serialization.ISerializable serializableObject in element.Value)
                {
                    fixed (byte* arrayptr = header.Array)
                    {
                        *((ushort*)(arrayptr + header.Offset)) = serializableObject.SerializableInfo.ClassID;
                    }
                    header.consume(sizeof(ushort));
                    serializableObject.SerializeTo(ref header, ref data);
                }
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int dictionaryCount;
            fixed (byte* arrayptr = header.Array)
            {
                dictionaryCount = (int)*((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            while (dictionaryCount-- > 0)
            {
                QS._core_c_.Base3.InstanceID instanceID = new QS._core_c_.Base3.InstanceID();
                instanceID.DeserializeFrom(ref header, ref data);
                int listCount;
                fixed (byte* arrayptr = header.Array)
                {
                    listCount = (int) *((ushort*)(arrayptr + header.Offset));
                }
                header.consume(sizeof(ushort));
                List<QS.Fx.Serialization.ISerializable> list = new List<QS.Fx.Serialization.ISerializable>(listCount);
                while (listCount-- > 0)
                {
                    ushort classID;
                    fixed (byte* arrayptr = header.Array)
                    {
                        classID = *((ushort*)(arrayptr + header.Offset));
                    }
                    header.consume(sizeof(ushort));
                    QS.Fx.Serialization.ISerializable serializableObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
                    serializableObject.DeserializeFrom(ref header, ref data);
                    list.Add(serializableObject);
                }
                messageCollection.Add(instanceID, list);
            }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("{ ");
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IList<QS.Fx.Serialization.ISerializable>> element in messageCollection)
            {
                s.Append(element.Key.ToString());
                bool isfirst = true;
                foreach (QS.Fx.Serialization.ISerializable serializableObject in element.Value)
                {
                    s.Append(isfirst ? ":" : ",");
                    s.Append((serializableObject != null) ? serializableObject.ToString() : "(null)");
                }
                s.Append(" ");
            }
            s.Append("}");
            return s.ToString();
        }
    }
}
