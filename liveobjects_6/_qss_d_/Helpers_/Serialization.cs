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
using System.IO;
using System.Xml.Serialization;

namespace QS._qss_d_.Helpers_
{
    public static class Serialization
    {
        public static ArraySegment<byte> Serialize<C>(C data)
        {
            MemoryStream stream = new MemoryStream();
            (new XmlSerializer(typeof(C))).Serialize(stream, data);
            
            return new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        }

        public static ArraySegment<byte> Serialize(object data)
        {
            if (data == null)
                throw new ArgumentException("Cannot serialize a NULL object.");

            MemoryStream stream = new MemoryStream();
            (new XmlSerializer(typeof(string))).Serialize(stream, data.GetType().ToString());
            (new XmlSerializer(data.GetType())).Serialize(stream, data);

            return new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        }

        public static ArraySegment<byte> SerializeArray(object[] data)
        {
            if (data == null)
                throw new ArgumentException("Cannot serialize a NULL array of objects.");

            MemoryStream stream = new MemoryStream();
            (new XmlSerializer(typeof(int))).Serialize(stream, data.Length);
            foreach (object o in data)
            {
                if (o == null)
                    throw new ArgumentException("Cannot serialize a NULL object.");

                (new XmlSerializer(typeof(string))).Serialize(stream, o.GetType().ToString());
                (new XmlSerializer(o.GetType())).Serialize(stream, o);
            }

            return new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        }

        public static byte[] ToBytes(ArraySegment<byte> segment)
        {
            byte[] anotherOne = new byte[segment.Count];
            Buffer.BlockCopy(segment.Array, segment.Offset, anotherOne, 0, segment.Count);
            return anotherOne;
        }

        public static C Deserialize<C>(byte[] bytes)
        {            
            return (C) (new XmlSerializer(typeof(C))).Deserialize(new MemoryStream(bytes));
        }

        public static object Deserialize(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            Type objectClass = Type.GetType((string) (new XmlSerializer(typeof(string))).Deserialize(stream));
            return (new XmlSerializer(objectClass)).Deserialize(stream);
        }

        public static object[] DeserializeArray(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            object[] elements = new object[(int) (new XmlSerializer(typeof(int))).Deserialize(stream)];
            for (int ind = 0; ind < elements.Length; ind++)
            {
                Type objectClass = Type.GetType((string)(new XmlSerializer(typeof(string))).Deserialize(stream));
                elements[ind] = (new XmlSerializer(objectClass)).Deserialize(stream);
            }
            return elements;
        }
    }
}
