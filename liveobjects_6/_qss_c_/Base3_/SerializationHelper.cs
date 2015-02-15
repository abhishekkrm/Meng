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

namespace QS._qss_c_.Base3_
{
    public static class SerializationHelper 
    {
        public static void ExtendSerializableInfo_ShortListOfUnknown<C>(ref QS.Fx.Serialization.SerializableInfo info, IList<C> list) where C : QS.Fx.Serialization.ISerializable
        {
            int n = sizeof(ushort) * (1 + list.Count);
            
            info.HeaderSize += n;
            info.Size += n;

            if (list != null)
            {
                foreach (C obj in list)
                    info.AddAnother(obj.SerializableInfo);
            }
        }

        public unsafe static void Serialize_ShortListOfUnknown<C>(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, IList<C> list) where C : QS.Fx.Serialization.ISerializable
        {
            int count = (list != null) ? list.Count : 0;

            if (count > ushort.MaxValue)
                throw new Exception("Cannot serialize, list too long.");

            fixed (byte* pheaderarray = header.Array)
            {
                byte *pheader = pheaderarray + header.Offset;
                
                *((ushort*) pheader) = (ushort) count;

                if (count > 0)
                {
                    foreach (C obj in list)
                    {
                        pheader += sizeof(ushort);
                        *((ushort*) pheader) = ((QS.Fx.Serialization.ISerializable) obj).SerializableInfo.ClassID;
                    }
                }
            }

            header.consume(sizeof(ushort) * (1 + count));

            if (count > 0)
            {
                foreach (C obj in list)
                    obj.SerializeTo(ref header, ref data);
            }
        }

        public unsafe static IList<C> Deserialize_ShortListOfUnknown<C>(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data) where C : class, QS.Fx.Serialization.ISerializable
        {
            int count;
            IList<C> list = null;

            fixed (byte* pheaderarray = header.Array)
            {
                byte* pheader = pheaderarray + header.Offset;

                count = (int)(*((ushort*) pheader));

                if (count > 0)
                {
                    list = new List<C>(count);

                    for (int ind = 0; ind < count; ind++)
                    {
                        pheader += sizeof(ushort);
                        ushort classid = *((ushort*) pheader);

                        C obj = (C) QS._core_c_.Base3.Serializer.CreateObject(classid);
                        list.Add(obj);
                    }
                }
            }

            header.consume(sizeof(ushort) * (1 + count));

            if (count > 0)
            {
                foreach (C obj in list)
                    obj.DeserializeFrom(ref header, ref data);

                return list;
            }
            else
                return new List<C>(0);
        }

        public static void ExtendSerializableInfo_ShortListOf<C>(ref QS.Fx.Serialization.SerializableInfo info, IList<C> list) where C : QS.Fx.Serialization.ISerializable
        {
            info.HeaderSize += sizeof(ushort);
            info.Size += sizeof(ushort);

            if (list != null)
            {
                foreach (C obj in list)
                    info.AddAnother(obj.SerializableInfo);
            }
        }

        public unsafe static void Serialize_ShortListOf<C>(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, IList<C> list) where C : QS.Fx.Serialization.ISerializable
        {
            if (list != null && list.Count > ushort.MaxValue)
                throw new Exception("Cannot serialize, list too long.");
            fixed (byte* pheaderarray = header.Array)
            {
                *((ushort*)(pheaderarray + header.Offset)) = (list != null) ? (ushort) list.Count: ((ushort)0);
            }
            header.consume(sizeof(ushort));
            if (list != null)
            {
                foreach (C obj in list)                
                    obj.SerializeTo(ref header, ref data);
            }
        }

        public unsafe static IList<C> Deserialize_ShortListOf<C>(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data) where C : QS.Fx.Serialization.ISerializable, new()
        {
            int listsize;
            fixed (byte* pheaderarray = header.Array)
            {
                listsize = (int)(*((ushort*)(pheaderarray + header.Offset)));
            }
            header.consume(sizeof(ushort));

            if (listsize > 0)
            {
                IList<C> list = new List<C>(listsize);
                for (int ind = 0; ind < listsize; ind++)
                {
                    C obj = new C();
                    obj.DeserializeFrom(ref header, ref data);
                    list.Add(obj);
                }
                return list;
            }
            else
                return new List<C>(0);
        }

        public static void ExtendSerializableInfo_ShortArrayOf<C>(ref QS.Fx.Serialization.SerializableInfo info, C[] array) where C : QS.Fx.Serialization.ISerializable
        {
            info.HeaderSize += sizeof(ushort);
            info.Size += sizeof(ushort);

            if (array != null)
            {
                for (int ind = 0; ind < array.Length; ind++)
                    info.AddAnother(array[ind].SerializableInfo);
            }
        }

        public unsafe static void Serialize_ShortArrayOf<C>(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, C[] array) where C : QS.Fx.Serialization.ISerializable
        {
            if (array != null && array.Length > ushort.MaxValue)
                throw new Exception("Cannot serialize, array too big.");
            fixed (byte* pheaderarray = header.Array)
            {
                *((ushort*)(pheaderarray + header.Offset)) = (array != null) ? (ushort) array.Length : ((ushort) 0);
            }
            header.consume(sizeof(ushort));
            if (array != null)
            {
                for (int ind = 0; ind < array.Length; ind++)
                    array[ind].SerializeTo(ref header, ref data);
            }
        }

        public unsafe static C[] Deserialize_ShortArrayOf<C>(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data) where C : QS.Fx.Serialization.ISerializable, new()
        {
            int arraysize;
            fixed (byte* pheaderarray = header.Array)
            {
                arraysize = (int)(*((ushort*)(pheaderarray + header.Offset)));
            }
            header.consume(sizeof(ushort));

            if (arraysize > 0)
            {
                C[] array = new C[arraysize];
                for (int ind = 0; ind < arraysize; ind++)
                {
                    array[ind] = new C();
                    array[ind].DeserializeFrom(ref header, ref data);
                }
                return array;
            }
            else 
                return new C[0];
        }

        public static void ExtendSerializableInfo_UnicodeString(ref QS.Fx.Serialization.SerializableInfo info, string s)
        {
            info.HeaderSize += sizeof(ushort);
            info.Size += sizeof(ushort);
            if (s != null && s.Length > 0)
            {
                info.Size += Encoding.Unicode.GetBytes(s).Length;
                info.NumberOfBuffers += 1;
            }
        }

        public unsafe static void Serialize_UnicodeString(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, string s)
        {
            byte[] bytes = (s != null && s.Length > 0) ? Encoding.Unicode.GetBytes(s) : null;
            if (bytes != null && bytes.Length > ushort.MaxValue)
                throw new Exception("Cannot serialize, string too long.");
            fixed (byte* pheaderarray = header.Array)
            {
                *((ushort*)(pheaderarray + header.Offset)) = (bytes != null) ? (ushort)bytes.Length : ((ushort)0);
            }
            header.consume(sizeof(ushort));
            if (bytes != null)
                data.Add(new QS.Fx.Base.Block(bytes));
        }

        public unsafe static string Deserialize_UnicodeString(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int nbytes;
            fixed (byte* pheaderarray = header.Array)
            {
                nbytes = (int)(*((ushort*)(pheaderarray + header.Offset)));
            }
            header.consume(sizeof(ushort));
            string s;
            if (nbytes > 0)
            {
                s = Encoding.Unicode.GetString(data.Array, data.Offset, nbytes);
                data.consume(nbytes);
            }
            else
                s = null;
            return s;
        }

        public static void ExtendSerializableInfo_ASCIIString(ref QS.Fx.Serialization.SerializableInfo info, string s)
        {
            info.HeaderSize += sizeof(ushort);
            info.Size += sizeof(ushort);
            if (s != null && s.Length > 0)
            {
                info.Size += Encoding.ASCII.GetBytes(s).Length;
                info.NumberOfBuffers += 1;
            }
        }

        public unsafe static void Serialize_ASCIIString(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, string s)
        {
            byte[] bytes = (s != null && s.Length > 0) ? Encoding.ASCII.GetBytes(s) : null;
            if (bytes != null && bytes.Length > ushort.MaxValue)
                throw new Exception("Cannot serialize, string too long.");
            fixed (byte* pheaderarray = header.Array)
            {
                *((ushort*)(pheaderarray + header.Offset)) = (bytes != null) ? (ushort)bytes.Length : ((ushort)0);
            }
            header.consume(sizeof(ushort));
            if (bytes != null)
                data.Add(new QS.Fx.Base.Block(bytes));
        }

        public unsafe static string Deserialize_ASCIIString(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int nbytes;
            fixed (byte* pheaderarray = header.Array)
            {
                nbytes = (int)(*((ushort*)(pheaderarray + header.Offset)));
            }
            header.consume(sizeof(ushort));
            string s;
            if (nbytes > 0)
            {
                s = Encoding.ASCII.GetString(data.Array, data.Offset, nbytes);
                data.consume(nbytes);
            }
            else
                s = null;
            return s;
        }

        public static void ExtendSerializableInfo_Int32(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(int);
            info.Size += sizeof(int);
        }

        public unsafe static void Serialize_Int32(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, int n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((int*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(int));
        }

        public unsafe static int Deserialize_Int32(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((int*)(pheaderarray + header.Offset));
            }
            header.consume(sizeof(int));
            return n;
        }

        public static void ExtendSerializableInfo_UInt32(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(uint);
            info.Size += sizeof(uint);
        }

        public unsafe static void Serialize_UInt32(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, uint n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((uint*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(uint));
        }

        public unsafe static uint Deserialize_UInt32(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            uint n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((uint*)(pheaderarray + header.Offset));
            }
            header.consume(sizeof(uint));
            return n;
        }

        public static void ExtendSerializableInfo_UInt64(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(ulong);
            info.Size += sizeof(ulong);
        }

        public unsafe static void Serialize_UInt64(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, ulong n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((ulong*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(ulong));
        }

        public unsafe static ulong Deserialize_UInt64(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ulong n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((ulong*)(pheaderarray + header.Offset));
            }
            header.consume(sizeof(ulong));
            return n;
        }

        public static void ExtendSerializableInfo_Double(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(double);
            info.Size += sizeof(double);
        }

        public unsafe static void Serialize_Double(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, double n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((double*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(double));
        }

        public unsafe static double Deserialize_Double(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            double n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((double *)(pheaderarray + header.Offset));
            }
            header.consume(sizeof(double));
            return n;
        }

        public static void ExtendSerializableInfo_Float(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(float);
            info.Size += sizeof(float);
        }

        public unsafe static void Serialize_Float(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, float n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((float*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(float));
        }

        public unsafe static float Deserialize_Float(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            float n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((float*)(pheaderarray + header.Offset));
            }
            header.consume(sizeof(float));
            return n;
        }

        public static void ExtendSerializableInfo_Bool(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(bool);
            info.Size += sizeof(bool);
        }

        public unsafe static void Serialize_Bool(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, bool n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((bool*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(bool));
        }

        public unsafe static bool Deserialize_Bool(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            bool n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((bool*) (pheaderarray + header.Offset));
            }
            header.consume(sizeof(bool));
            return n;
        }

        public static void ExtendSerializableInfo_Byte(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(byte);
            info.Size += sizeof(byte);
        }

        public unsafe static void Serialize_Byte(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, byte n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *(pheaderarray + header.Offset) = n;
            }
            header.consume(sizeof(byte));
        }

        public unsafe static byte Deserialize_Byte(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            byte n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *(pheaderarray + header.Offset);
            }
            header.consume(sizeof(byte));
            return n;
        }

        public static void ExtendSerializableInfo_Int16(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(short);
            info.Size += sizeof(short);
        }

        public unsafe static void Serialize_Int16(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, short n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((short*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(short));
        }

        public unsafe static short Deserialize_Int16(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            short n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((short *) (pheaderarray + header.Offset));
            }
            header.consume(sizeof(short));
            return n;
        }

        public static void ExtendSerializableInfo_UInt16(ref QS.Fx.Serialization.SerializableInfo info)
        {
            info.HeaderSize += sizeof(ushort);
            info.Size += sizeof(ushort);
        }

        public unsafe static void Serialize_UInt16(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data, ushort n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((ushort *) (pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(ushort));
        }

        public unsafe static ushort Deserialize_UInt16(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((ushort *) (pheaderarray + header.Offset));
            }
            header.consume(sizeof(ushort));
            return n;
        }

/*
        public unsafe static void ExtendSerializableInfo_Value<T>(ref SerializableInfo info) 
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            info.HeaderSize += size;
            info.Size += size;
        }

        public unsafe static void Serialize_Value<T>(ref WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data, T n)
        {
            fixed (byte* pheaderarray = header.Array)
            {
                *((T*)(pheaderarray + header.Offset)) = n;
            }
            header.consume(sizeof(T));
        }

        public unsafe static T Deserialize_Value<T>(ref WritableArraySegment<byte> header, ref WritableArraySegment<byte> data)
        {
            T n;
            fixed (byte* pheaderarray = header.Array)
            {
                n = *((T*)(pheaderarray + header.Offset));
            }
            header.consume(sizeof(T));
            return n;
        }
*/

/*
        public static unsafe void SerializeDictionary<K, C>(System.Collections.Generic.IDictionary<K, C> dictionary,
            ref WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data)
            where K : ISerializable
            where C : ISerializable
        {
        }
*/

        public static QS.Fx.Serialization.SerializableInfo SerializableInfoOfArray(int nelements, ushort headersize)
        {
            int totalsize = nelements * headersize + sizeof(ushort);
            return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Nothing, (ushort)totalsize, totalsize, 0);
        }

        /// <summary>
        /// Warning: This one calculates size based only on the size of the first element. 
        /// </summary>
        public static QS.Fx.Serialization.SerializableInfo SerializableInfoOfListOfFixed<T>(System.Collections.Generic.IList<T> list) where T : QS.Fx.Serialization.ISerializable
        {
            if (list.Count > 0)
            {
                QS.Fx.Serialization.SerializableInfo info = list[0].SerializableInfo;
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Nothing, (ushort)(list.Count * info.HeaderSize + sizeof(ushort)),
                    list.Count * info.Size + sizeof(ushort), list.Count * info.NumberOfBuffers);
            }
            else
                return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Nothing, (ushort)sizeof(ushort), sizeof(ushort), 0);
        }

        public static QS.Fx.Serialization.SerializableInfo SerializableInfoOfList<T>(System.Collections.Generic.IList<T> list) where T : QS.Fx.Serialization.ISerializable
        {
            int headersize = sizeof(ushort);
            int size = sizeof(ushort);
            int nbuffers = 0;
            foreach (T element in list)
            {
                QS.Fx.Serialization.SerializableInfo info = element.SerializableInfo;
                headersize += info.HeaderSize;
                size += info.Size;
                nbuffers += info.NumberOfBuffers;
            }
            return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Nothing, headersize, size, nbuffers);
        }

        public static unsafe void SerializeCollection<T>(System.Collections.Generic.ICollection<T> collection,
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data) where T : QS.Fx.Serialization.ISerializable
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) collection.Count;
            }
            header.consume(sizeof(ushort));
            foreach (T element in collection)
                element.SerializeTo(ref header, ref data);
        }

        public static unsafe void DeserializeCollection<C, T>(out C collection, Constructor<C, int> constructor, ref QS.Fx.Base.ConsumableBlock header,
            ref QS.Fx.Base.ConsumableBlock data)
            where T : QS.Fx.Serialization.ISerializable, new()
            where C : System.Collections.Generic.ICollection<T>
        {
            int length;
            fixed (byte* arrayptr = header.Array)
            {
                length = (int)*((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            collection = constructor(length);
            for (int ind = 0; ind < length; ind++)
            {
                T element = new T();
                element.DeserializeFrom(ref header, ref data);
                collection.Add(element);
            }
        }

        public static unsafe void DeserializeCollection<C,T>(ref C collection, 
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            where T : QS.Fx.Serialization.ISerializable, new() 
            where C : System.Collections.Generic.ICollection<T>
        {
            int length;
            fixed (byte* arrayptr = header.Array)
            {
                length = (int)*((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            for (int ind = 0; ind < length; ind++)
            {
                T element = new T();
                element.DeserializeFrom(ref header, ref data);
                collection.Add(element);
            }
        }

        public static unsafe void SerializeArray<T>(T[] array, ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            where T : QS.Fx.Serialization.ISerializable
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort)array.Length;
            }
            header.consume(sizeof(ushort));
            for (int ind = 0; ind < array.Length; ind++)
                array[ind].SerializeTo(ref header, ref data);
        }

        public static unsafe void DeserializeArray<T>(out T[] array, ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            where T : QS.Fx.Serialization.ISerializable
        {
            int length;
            fixed (byte* arrayptr = header.Array)
            {
                length = (int)*((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            array = new T[length];
            for (int ind = 0; ind < length; ind++)
                array[ind].DeserializeFrom(ref header, ref data);
        }

        public static unsafe QS.Fx.Serialization.SerializableInfo DictionaryInfo<K, C>(ClassID classID, IDictionary<K, C> dictionary)
            where K : QS.Fx.Serialization.ISerializable
            where C : QS.Fx.Serialization.ISerializable
        {
            QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)classID, (ushort)sizeof(ushort), sizeof(ushort), 0);
            foreach (KeyValuePair<K, C> element in dictionary)
            {
                info.AddAnother(element.Key.SerializableInfo);
                info.AddAnother(element.Value.SerializableInfo);
            }
            return info;
        }

        public static unsafe void SerializeDictionary<K,C>(IDictionary<K,C> dictionary,
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            where K : QS.Fx.Serialization.ISerializable
            where C : QS.Fx.Serialization.ISerializable
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)pheader) = (ushort) dictionary.Count;
            }
            header.consume(sizeof(ushort));
            foreach (KeyValuePair<K, C> element in dictionary)
            {
                element.Key.SerializeTo(ref header, ref data);
                element.Value.SerializeTo(ref header, ref data);
            }
        }

        public static unsafe void DeserializeDictionary<K,C>(IDictionary<K, C> dictionary,
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            where K : QS.Fx.Serialization.ISerializable, new()
            where C : QS.Fx.Serialization.ISerializable, new()
        {
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                count = (int)(*((ushort*)pheader));
            }
            header.consume(sizeof(ushort));
            while (count-- > 0)
            {
                KeyValuePair<K, C> element = new KeyValuePair<K, C>(new K(), new C());
                element.Key.DeserializeFrom(ref header, ref data);
                element.Value.DeserializeFrom(ref header, ref data);
                dictionary.Add(element);
            }
        }
    }
}
