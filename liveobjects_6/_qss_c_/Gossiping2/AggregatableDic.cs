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
    public abstract class AggregatableDic<K> : Aggregation3_.IAggregatable, Base3_.IKnownClass where K : QS.Fx.Serialization.ISerializable, new()
    {
        public AggregatableDic()
        {
        }

        public AggregatableDic(K key, Aggregation3_.IAggregatable element) : this()
        {
            add(key, element);
        }

        public AggregatableDic(IDictionary<K, Aggregation3_.IAggregatable> elements) : this()
        {
            add(elements);
        }

        private System.Collections.Generic.Dictionary<K, Aggregation3_.IAggregatable> dictionary = new Dictionary<K, Aggregation3_.IAggregatable>();

        public System.Collections.Generic.IDictionary<K, Aggregation3_.IAggregatable> Dictionary
        {
            get { return dictionary; }
        }

        #region Adding

        public void add(K key, Aggregation3_.IAggregatable data)
        {
            lock (this)
            {
                _add(key, data);
            }
        }

        public void add(IDictionary<K, Aggregation3_.IAggregatable> elements)
        {
            lock (this)
            {
                _add(elements);
            }
        }

        private void _add(K key, Aggregation3_.IAggregatable data)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key].aggregateWith(data);
            else
                dictionary[key] = data;
        }

        private void _add(IDictionary<K, Aggregation3_.IAggregatable> elements)
        {
            foreach (KeyValuePair<K, Aggregation3_.IAggregatable> element in elements)
                _add(element.Key, element.Value);
        }

        #endregion

        #region IAggregatable Members

        void QS._qss_c_.Aggregation3_.IAggregatable.aggregateWith(QS._qss_c_.Aggregation3_.IAggregatable anotherObject)
        {
            lock (this)
            {
                AggregatableDic<K> another_dic = anotherObject as AggregatableDic<K>;
                if (another_dic == null)
                    throw new ArgumentException("Cannot aggregate with this object.");

                _add(another_dic.Dictionary);
            }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int header_overhead = sizeof(ushort) * (1 + dictionary.Count);
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort) this.ClassID, (ushort)header_overhead, header_overhead, 0);
                foreach (System.Collections.Generic.KeyValuePair<K, Aggregation3_.IAggregatable> entry in dictionary)
                {
                    info.AddAnother(entry.Key.SerializableInfo);
                    info.AddAnother(entry.Value.SerializableInfo);
                }
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) dictionary.Count;
            }
            header.consume(sizeof(ushort));

            foreach (System.Collections.Generic.KeyValuePair<K, Aggregation3_.IAggregatable> entry in dictionary)
            {
                entry.Key.SerializeTo(ref header, ref data);
                fixed (byte* arrayptr = header.Array)
                {
                    *((ushort*)(arrayptr + header.Offset)) = ((QS.Fx.Serialization.ISerializable) entry.Value).SerializableInfo.ClassID;
                }
                header.consume(sizeof(ushort));
                entry.Value.SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort count;
            fixed (byte* arrayptr = header.Array)
            {
                count = *((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));

            while (count-- > 0)
            {
                K key = new K();
                key.DeserializeFrom(ref header, ref data);
                ushort classID;
                fixed (byte* arrayptr = header.Array)
                {
                    classID = *((ushort*)(arrayptr + header.Offset));
                }
                header.consume(sizeof(ushort));
                Aggregation3_.IAggregatable element_data = (Aggregation3_.IAggregatable)QS._core_c_.Base3.Serializer.CreateObject(classID);
                element_data.DeserializeFrom(ref header, ref data);
                dictionary.Add(key, element_data);
            }
        }

        #endregion

        #region IKnownClass Members

        public abstract ClassID ClassID
        {
            get;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("{ ");
            foreach (KeyValuePair<K, Aggregation3_.IAggregatable> element in dictionary)
            {
                s.Append(element.Key.ToString());
                s.Append("=");
                s.Append(element.Value.ToString());
                s.Append(" ");
            }
            s.Append("}");
            return s.ToString();
        }
    }
}
