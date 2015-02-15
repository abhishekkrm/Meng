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

namespace QS._qss_c_.Components_2_
{
    public abstract class SerializableDictOf<K,C> : Base3_.KnownClass, QS.Fx.Serialization.ISerializable, Base3_.IFormattable
        where K : QS.Fx.Serialization.ISerializable, new() where C : class, QS.Fx.Serialization.ISerializable, new()
    {
        public SerializableDictOf()
        {
        }

        private IDictionary<K, C> dictionary;

        protected virtual IDictionary<K, C> CreateDictionary(int nelements)
        {
            return SerializableDictOf<K, C>.Create(nelements);
        }

        public IDictionary<K, C> Dictionary
        {
            get
            {
                if (dictionary == null)
                    dictionary = this.CreateDictionary(0);

                return dictionary;
            }
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public C GetCreate(K key)
        {
            IDictionary<K, C> dictionary = this.Dictionary;

            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
            {
                C value = new C();
                dictionary[key] = value;
                return value;
            }
        }

        public C this[K key]
        {
            get { return (dictionary == null) ? null : (dictionary.ContainsKey(key) ? dictionary[key] : null); }
        }

        public C Remove(K key)
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                C data = dictionary[key];
                dictionary.Remove(key);
                return data;
            }
            else
                return null;
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return SerializableDictOf<K, C>.SerializableInfo(this.ClassID, dictionary); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            Serialize(dictionary, ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            dictionary = Deserialize(new CreateCallback(this.CreateDictionary), ref header, ref data);
        }

        #endregion

        #region Static Stuff

        public static QS.Fx.Serialization.SerializableInfo SerializableInfo(ClassID classID, IDictionary<K, C> dictionary)
        {
            QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort) classID, (ushort)sizeof(ushort), sizeof(ushort), 0);
            if (dictionary != null)
            {
                foreach (KeyValuePair<K, C> element in dictionary)
                {
                    info.AddAnother(element.Key.SerializableInfo);
                    info.AddAnother(element.Value.SerializableInfo);
                }
            }
            return info;
        }

        public unsafe static void Serialize(IDictionary<K, C> dictionary,
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) ((dictionary != null) ? dictionary.Count : 0);
            }
            header.consume(sizeof(ushort));
            if (dictionary != null)
            {
                foreach (KeyValuePair<K, C> element in dictionary)
                {
                    element.Key.SerializeTo(ref header, ref data);
                    element.Value.SerializeTo(ref header, ref data);
                }
            }
        }

        public delegate IDictionary<K, C> CreateCallback(int nelements);

        public unsafe static IDictionary<K, C> Deserialize(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            return Deserialize(DefaultCreateCallback, ref header, ref data);
        }

        private static IDictionary<K, C> Create(int nelements)
        {
            return (nelements > 0) ? new Dictionary<K, C>(nelements) : new Dictionary<K, C>();
        }

        private static CreateCallback DefaultCreateCallback = new CreateCallback(Create);

        public unsafe static IDictionary<K, C> Deserialize(CreateCallback createCallback,
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            IDictionary<K, C> dictionary;

            int count;
            fixed (byte* arrayptr = header.Array)
            {
                count = (int)(*((ushort*)(arrayptr + header.Offset)));
            }
            header.consume(sizeof(ushort));

            dictionary = createCallback(count);

            while (count-- > 0)
            {
                K key = new K();
                key.DeserializeFrom(ref header, ref data);
                C value = new C();
                value.DeserializeFrom(ref header, ref data);
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("{");
            bool separate = false;
            foreach (KeyValuePair<K, C> element in dictionary)
            {
                if (separate)
                    s.Append(", ");
                else
                    separate = true;

                s.Append(element.Key.ToString());
                s.Append(" : ");
                s.Append(element.Value.ToString());
            }
            s.Append("}");
            return s.ToString();
        }

        #region IFormattable Members

        string QS._qss_c_.Base3_.IFormattable.ToString(int columnOffset)
        {
            StringBuilder s = new StringBuilder();
            string columns = new string(' ', columnOffset);
            if (dictionary != null)
            {
                s.Append(columns);
                s.AppendLine("{");
                foreach (KeyValuePair<K,C> element in dictionary)
                {
                    if (element.Key is Base3_.IFormattable)
                        s.Append(((Base3_.IFormattable) element.Key).ToString(columnOffset + 2));
                    else
                    {
                        s.Append(columns);
                        s.Append("  ");
                        s.AppendLine(element.Key.ToString());
                    }
                    if (element.Value is Base3_.IFormattable)
                        s.Append(((Base3_.IFormattable) element.Value).ToString(columnOffset + 2));
                    else
                    {
                        s.Append(columns);
                        s.Append("  ");
                        s.AppendLine(element.Value.ToString());
                    }
                }
                s.Append(columns);
                s.AppendLine("}");
            }
            else
            {
                s.Append(columns);
                s.AppendLine("{}");
            }
            return s.ToString();
        }

        #endregion
    }
}
