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

// #define DEBUG_SerializationChecking

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._core_c_.Base3
{
    public class Serializer : QS._core_x_.Serialization.ISerializer<QS.Fx.Serialization.ISerializable>, QS.Fx.Serialization.ISerializer // , Fx.Serialization.ISerializer<object>
    {
        public static readonly Serializer Global = new Serializer();

        private Serializer()
        {
        }

        #region ISerializer<ISerializable> Members

        IEnumerable<QS.Fx.Base.Block> QS._core_x_.Serialization.ISerializer<QS.Fx.Serialization.ISerializable>.Serialize<C>(C data)
        {
            QS.Fx.Serialization.SerializableInfo info = data.SerializableInfo;
            int totalHeaderSize = info.HeaderSize + sizeof(int);

            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) totalHeaderSize);
            System.Collections.Generic.IList<QS.Fx.Base.Block> buffers = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
            buffers.Add(header.Block);
            
            unsafe
            {
                fixed (byte* pbuffer = header.Array)
                {
                    *((int*)(pbuffer + header.Offset)) = info.HeaderSize;
                }
            }
            header.consume(sizeof(int));

            data.SerializeTo(ref header, ref buffers);

            return buffers;
        }

        C QS._core_x_.Serialization.ISerializer<QS.Fx.Serialization.ISerializable>.Deserialize<C>(ArraySegment<byte> segment, out int nconsumed) 
        {
            int headerSize;
            unsafe
            {
                fixed (byte* pbuffer = segment.Array)
                {
                    headerSize = *((int*) (pbuffer + segment.Offset));
                }
            }
            C dataObject = new C();

            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(segment.Array, (uint) segment.Offset + sizeof(int), (uint) headerSize);
            QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(
                segment.Array, (uint) (segment.Offset + sizeof(int) + headerSize), (uint) (segment.Count - sizeof(int) - headerSize));

            dataObject.DeserializeFrom(ref header, ref data);
            nconsumed = sizeof(int) + headerSize + data.Consumed;

            return dataObject;
        }

        IEnumerable<QS.Fx.Base.Block> QS._core_x_.Serialization.ISerializer<QS.Fx.Serialization.ISerializable>.Serialize(QS.Fx.Serialization.ISerializable data)
        {
            QS.Fx.Serialization.SerializableInfo info = data.SerializableInfo;
            int totalHeaderSize = info.HeaderSize + sizeof(ushort) + sizeof(int);

            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) totalHeaderSize);
            System.Collections.Generic.IList<QS.Fx.Base.Block> buffers = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
            buffers.Add(header.Block);

            unsafe
            {
                fixed (byte* pbuffer = header.Array)
                {
                    *((ushort*)(pbuffer + header.Offset)) = info.ClassID;
                    *((int*)(pbuffer + header.Offset + sizeof(ushort))) = info.HeaderSize;
                }
            }
            header.consume(sizeof(int) + sizeof(ushort));

            data.SerializeTo(ref header, ref buffers);

            return buffers;
        }

        QS.Fx.Serialization.ISerializable QS._core_x_.Serialization.ISerializer<QS.Fx.Serialization.ISerializable>.Deserialize(ArraySegment<byte> segment, out int nconsumed)
        {
            ushort classid;
            int headerSize;
            unsafe
            {
                fixed (byte* pbuffer = segment.Array)
                {
                    classid = *((ushort*)(pbuffer + segment.Offset));
                    headerSize = *((int*)(pbuffer + segment.Offset + sizeof(ushort)));
                }
            }
            QS.Fx.Serialization.ISerializable dataObject = CreateObject(classid);

            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(
                segment.Array, (uint) (segment.Offset + sizeof(int) + sizeof(ushort)), (uint) headerSize);
            QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(
                segment.Array, (uint) (segment.Offset + sizeof(int) + sizeof(ushort) + headerSize), 
                (uint) (segment.Count - sizeof(int) - sizeof(ushort) - headerSize));

            dataObject.DeserializeFrom(ref header, ref data);
            nconsumed = sizeof(int) + sizeof(ushort) + headerSize + data.Consumed;

            return dataObject;
        }

        #endregion

/*
        #region ISerializer<object> Members

        IEnumerable<ArraySegment<byte>> QS.Fx.Serialization.ISerializer<object>.Serialize(object data)
        {
            throw new NotImplementedException();
//          return ToSegments(new XmlObject(data));
        }

        object QS.Fx.Serialization.ISerializer<object>.Deserialize(ArraySegment<byte> data, out int nconsumed)
        {
            throw new NotImplementedException();
/-*
            object obj = FromSegment(data);
            if (obj == null)
                throw new Exception("Deserialized object is NULL.");
            XmlObject xmlObj = obj as XmlObject;
            if (xmlObj == null)
                throw new Exception("Deserialized intermediate object representation is not XmlObject.");
            return xmlObj.Object;
*-/ 
        }

        IEnumerable<ArraySegment<byte>> QS.Fx.Serialization.ISerializer<object>.Serialize<C>(C data)
        {
            return ((QS.Fx.Serialization.ISerializer<ISerializable>)this).Serialize<XmlObject>(new XmlObject(data));
        }

        C QS.Fx.Serialization.ISerializer<object>.Deserialize<C>(ArraySegment<byte> data, out int nconsumed)
        {
            XmlObject xmlObject = ((QS.Fx.Serialization.ISerializer<ISerializable>)this).Deserialize<XmlObject>(data, out nconsumed);
            object obj = xmlObject.Object;
            if (obj is C)
                return (C) obj;
            else
                throw new Exception("Cannot deserialize, the loaded object is not of type " + typeof(C).Name + ".");
        }

        #endregion
*/

/*
        public static void Serialize(ref QS.Fx.Serialization.ISerializable serializableObject,
            ref QS.CMS.Base3.WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data)
        {
#if DEBUG_SerializationChecking
            SerializableInfo info = serializableObject.SerializableInfo;
            int header_offset_before = header.Offset;
            int data_nbuffers_before = data.Count;
#endif

            serializableObject.SerializeTo(ref header, ref data);

#if DEBUG_SerializationChecking            
            if (header.Offset != header_offset_before + info.HeaderSize)
                throw new Exception("While serializing " + Helpers.ToString.ObjectRef(serializableObject) +
                    " : Header was supposed to occupy " + info.HeaderSize.ToString() + " bytes, but it actually took " + 
                    (header.Offset - header_offset_before).ToString() + " bytes.");
            if (data.Count != data_nbuffers_before + info.NumberOfBuffers)
                throw new Exception("While serializing " + Helpers.ToString.ObjectRef(serializableObject) +
                    " : Data was supposed to occupy " + info.NumberOfBuffers.ToString() + " buffers, but it actually took " +
                    (data.Count - data_nbuffers_before).ToString() + " buffers.");
            int data_size_increase = 0;
            for (int ind = data_nbuffers_before; ind < data.Count; ind++)
                data_size_increase += data[ind].Count;
            if (data_size_increase != (info.Size - info.HeaderSize))
                throw new Exception("While serializing " + Helpers.ToString.ObjectRef(serializableObject) +
                    " : Additional data was supposed to occupy " + (info.Size - info.HeaderSize).ToString() + " bytes, but it actually took " +
                    data_size_increase.ToString() + " bytes.");
#endif
        }

        public static void Deserialize(ref QS.Fx.Serialization.ISerializable serializableObject, 
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
// #if DEBUG_SerializationChecking
//            int header_offset_before = header.Offset;
//            // int data_nbuffers_before = data.Count;
//            // int data_size_before = 0;
//            // foreach (ArraySegment<byte> segment in data)
//            //    data_size_before += segment.Count;
// #endif

            serializableObject.DeserializeFrom(ref header, ref data);

// #if DEBUG_SerializationChecking
//            int header_used = header.Offset - header_offset_before;
//            SerializableInfo info = serializableObject.SerializableInfo;
//            if (header_used != info.HeaderSize)
//                throw new Exception("While deserializing " + Helpers.ToString.ObjectRef(serializableObject) +
//                    " : Header was supposed to consume " + info.HeaderSize.ToString() + " bytes, but it actually consumed " +
//                    header_used.ToString() + " bytes.");
// #endif
        }
*/

        static Serializer()
        {
            try
            {
                mappings = new QS._core_c_.Collections.Hashtable(100);
                logger = new Base.Logger(null, true, null);
                foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        Base2.Serializer.registerClasses(
                            logger, assembly, typeof(QS.Fx.Serialization.ISerializable), new QS._core_c_.Base2.Serializer.RegisterClassCallback(registerClass));
                    }
                    catch (Exception _exc)
                    {
                        try
                        {
                            System.Windows.Forms.MessageBox.Show(_exc.ToString(), "Exception",
                                 System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }
                        catch (Exception)
                        {
                        }
                        // throw new Exception("The static initializer for serializer has failed for assembly\"" + assembly.FullName + "\".", _exc);
                    }
                }
            }
            catch (Exception _exc)
            {
                try
                {
                    System.Windows.Forms.MessageBox.Show(_exc.ToString(), "Exception",
                         System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                catch (Exception)
                {
                }
                // throw new Exception("The static initializer for serializer has failed.", _exc);
            }
        }

        public static void registerClass(ushort classID, System.Type type)
        {
            Base2.Serializer.registerClassWith(classID, type, mappings);
        }

        private static Collections.Hashtable mappings;
        private static Base.Logger logger;

        public static QS.Fx.Serialization.ISerializable CreateObject(ushort classID)
        {
            return (classID != (ushort)ClassID.Nothing) ? (QS.Fx.Serialization.ISerializable)Base2.Serializer.CreateObjectWith(classID, mappings) : null;
        }

        public static string Log
        {
            get
            {
                return logger.CurrentContents;
            }
        }

        public static System.Collections.Generic.IList<QS.Fx.Base.Block> FlattenObject(QS.Fx.Serialization.ISerializable data)
        {
            QS.Fx.Serialization.SerializableInfo info = data.SerializableInfo;
            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) info.HeaderSize);
            System.Collections.Generic.IList<QS.Fx.Base.Block> buffers =
                new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
            buffers.Add(header.Block);
            data.SerializeTo(ref header, ref buffers);
            return buffers;
        }

        public static System.Collections.Generic.IList<QS.Fx.Base.Block> ToSegments(QS.Fx.Serialization.ISerializable data)
        {
            QS.Fx.Serialization.SerializableInfo info = data.SerializableInfo;
            int totalHeaderSize = info.HeaderSize + sizeof(ushort) + sizeof(uint);
            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint) totalHeaderSize);
            System.Collections.Generic.IList<QS.Fx.Base.Block> buffers =
                new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
            buffers.Add(header.Block);
            unsafe
            {
                fixed (byte* pbuffer = header.Array)
                {
                    byte* pheader = pbuffer + header.Offset;
                    *((uint*) pheader) = (uint) totalHeaderSize;
                    *((ushort*) (pheader + sizeof(uint))) = info.ClassID;
                }
            }
            header.consume(sizeof(ushort) + sizeof(uint));
            data.SerializeTo(ref header, ref buffers);
            return buffers;
        }

        public static QS.Fx.Serialization.ISerializable FromSegment(ArraySegment<byte> segment)
        {
            int totalHeaderSize;
            ushort classID;
            unsafe
            {
                fixed (byte* pbuffer = segment.Array)
                {
                    byte* pheader = pbuffer + segment.Offset;
                    totalHeaderSize = (int)(*((uint*)pheader));
                    classID = *((ushort*)(pheader + sizeof(uint)));
                }
            }
            QS.Fx.Serialization.ISerializable dataObject = CreateObject(classID);
            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(
                segment.Array, (uint) (segment.Offset + (sizeof(ushort) + sizeof(uint))), (uint) (totalHeaderSize - (sizeof(ushort) + sizeof(uint))));
            QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(
                segment.Array, (uint) (segment.Offset + totalHeaderSize), (uint) (segment.Count - totalHeaderSize));
            dataObject.DeserializeFrom(ref header, ref data);
            return dataObject;
        }

/*
        public static ISerializable PreSerialize(QS.Fx.Serialization.ISerializable data)
        {
            SerializableInfo info = data.SerializableInfo;
            WritableArraySegment<byte> header = new WritableArraySegment<byte>(info.HeaderSize);
            System.Collections.Generic.IList<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>(info.NumberOfBuffers + 1);
            buffers.Add(header.ArraySegment);
            data.SerializeTo(ref header, ref buffers);
            return buffers;



        }
*/

        public static string ToString(QS.Fx.Serialization.IStringSerializable stringSerializable)
		{
			StringBuilder s = new StringBuilder(stringSerializable.ClassID.ToString());
			s.Append(":");
			s.Append(stringSerializable.AsString);
			return s.ToString();
		}

        public static QS.Fx.Serialization.IStringSerializable FromString(string s)
		{
			int separator = s.IndexOf(':');
            QS.Fx.Serialization.IStringSerializable stringSerializable =
                CreateObject(System.Convert.ToUInt16(s.Substring(0, separator))) as QS.Fx.Serialization.IStringSerializable;
			if (stringSerializable != null)
				stringSerializable.AsString = s.Substring(separator + 1);
			return stringSerializable;
		}

        #region ISerializer Members

        QS.Fx.Serialization.ISerializable QS.Fx.Serialization.ISerializer.CreateObject(ushort classid)
        {
            return CreateObject(classid);
        }

        void QS.Fx.Serialization.ISerializer.RegisterClass<C>()
        {
            Type _type = typeof(C);
            object[] _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Serialization.ClassIDAttribute), false);
            if (_attributes.Length != 1)
                throw new Exception("Cannot register class " + _type.ToString() + " because it does not have the ClassID attribute."); 
            ushort _classid = ((QS.Fx.Serialization.ClassIDAttribute) _attributes[0]).ClassID;
            registerClass(_classid, _type);
        }

        #endregion
    }
}
