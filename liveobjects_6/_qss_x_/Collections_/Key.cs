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

namespace QS._qss_x_.Collections_
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Collections_Key)]
    public sealed class Key : IKey<string, Key>, QS.Fx.Serialization.ISerializable
    {
        public Key(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public Key()
        {
        }

        private string name, value;
        private IDictionary<string, Key> subkeys;

        #region IKey<string, Key> Members

        string IKey<string, Key>.Name
        {
            get { return name; }
        }

        string IKey<string, Key>.Value
        {
            get { return value; }
        }

        bool IKey<string, Key>.Subkey(string name, out Key subkey)
        {
            lock (this)
            {
                if (subkeys != null)
                    return subkeys.TryGetValue(name, out subkey);
                else
                {
                    subkey = null;
                    return false;
                }
            }
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int headersize, size, numbuffers;

                headersize = sizeof(byte) + sizeof(ushort);
                numbuffers = 0;
                size = headersize + System.Text.Encoding.ASCII.GetBytes(name).Length;

                if (value != null)
                {
                    headersize += sizeof(ushort);
                    numbuffers += 1;
                    size += sizeof(ushort) + System.Text.Encoding.ASCII.GetBytes(value).Length;
                }

                int numsubkeys = (subkeys != null) ? subkeys.Count : 0;

                if (numsubkeys > 0)
                {
                    headersize += sizeof(ushort);
                    size += sizeof(ushort);
                }

                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Collections_Key, headersize, size, numbuffers);

                if (numsubkeys > 0)
                {
                    foreach (Key key in subkeys.Values)
                        info.AddAnother(((QS.Fx.Serialization.ISerializable)key).SerializableInfo);
                }

                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            byte[] name_bytes = System.Text.Encoding.ASCII.GetBytes(name);
            byte[] value_bytes = (value != null) ? System.Text.Encoding.ASCII.GetBytes(value) : null;
            int num_subkeys = (subkeys != null) ? subkeys.Count : 0;

            if (name_bytes.Length > ushort.MaxValue || value_bytes.Length > ushort.MaxValue || num_subkeys > ushort.MaxValue)
                throw new Exception("Cannot serialize, either strings are too long or there are too many subkeys.");

            int nconsumed = 0;
            fixed (byte* parray = header.Array)
            {
                byte *pheader = parray + header.Offset;
                *pheader = (byte)(((value != null) ? 1 : 0) | ((num_subkeys > 0) ? 2 : 0));
                nconsumed += sizeof(byte);

                *((ushort*)(pheader + nconsumed)) = (ushort) name_bytes.Length;
                nconsumed += sizeof(ushort);

                if (value != null)
                {
                    *((ushort*)(pheader + nconsumed)) = (ushort) value_bytes.Length;
                    nconsumed += sizeof(ushort);
                }

                if (num_subkeys > 0)
                {
                    *((ushort*)(pheader + nconsumed)) = (ushort) num_subkeys;
                    nconsumed += sizeof(ushort);
                }
            }
            header.consume(nconsumed);

            data.Add(new QS.Fx.Base.Block(name_bytes));
            if (value != null)
                data.Add(new QS.Fx.Base.Block(value_bytes));

            foreach (Key key in subkeys.Values)
                ((QS.Fx.Serialization.ISerializable)key).SerializeTo(ref header, ref data);   
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            byte kindof;
            int name_length, value_length, num_subkeys;
            int nconsumed = 0;
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                kindof = *pheader;
                nconsumed += sizeof(byte);

                name_length = (int)(*((ushort*)(pheader + nconsumed)));
                nconsumed += sizeof(ushort);

                if ((kindof & 1) == 1)
                {
                    value_length = (int)(*((ushort*)(pheader + nconsumed)));
                    nconsumed += sizeof(ushort);
                }
                else
                    value_length = -1;

                if ((kindof & 2) == 2)
                {
                    num_subkeys = (int)(*((ushort*)(pheader + nconsumed)));
                    nconsumed += sizeof(ushort);
                }
                else
                    num_subkeys = -1;
            }
            header.consume(nconsumed);

            name = System.Text.Encoding.ASCII.GetString(data.Array, data.Offset, name_length);
            data.consume(name_length);

            if (value_length >= 0)
            {
                value = System.Text.Encoding.ASCII.GetString(data.Array, data.Offset, value_length);
                data.consume(value_length);
            }

            if (num_subkeys > 0)
            {
                subkeys = new Dictionary<string, Key>(num_subkeys);
                while (num_subkeys-- > 0)
                {
                    Key key = new Key();
                    ((QS.Fx.Serialization.ISerializable)key).DeserializeFrom(ref header, ref data);
                    subkeys.Add(key.name, key);
                }
            }
        }

        #endregion

/*
        #region Class Operation

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Collections_Key_Operation)]
        public sealed class Operation : Persistence.IOperation<Key>, QS.Fx.Serialization.ISerializable
        {
            public enum Type : byte
            {
                Add, Remove, Update
            }

            public Operation(Type type, IList<string> path, string value)
            {
                this.type = type;
                this.path = path;
                this.value = value;
            }

            public Operation(Type type, IList<string> path) : this(type, path, null)
            {
            }

            public Operation()
            {
            }

            private Type type;
            private IList<string> path;
            private string value;

            #region Persistence.IOperation<Key> Members

            void Persistence.IOperation<Key>.Execute(Key target)
            {
                switch (type)
                {
                    case Type.Add:
                        {
                            Key current = target;
                            bool isnew = false;
                            foreach (string name in path)
                            {
                                Key next;
                                if (!current.subkeys.TryGetValue(name, out next))
                                {
                                    next = new Key(name, null);
                                }
                            }

                        }
                        break;

                    case Type.Remove:
                        {
                        }
                        break;

                    case Type.Update:
                        {
                        }
                        break;
                }
/-*
                if (path.Count > 1)
                {
                    lock (target)
                    {
                        if (target.subkeys != null)
                    }
                }
                else
                {

                }
*-/ 
            }

            #endregion

            #region QS.Fx.Serialization.ISerializable Members

            unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
                ref QS.CMS.Base3.WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data)
            {
                throw new NotImplementedException();
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
                ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

*/
    }
}
