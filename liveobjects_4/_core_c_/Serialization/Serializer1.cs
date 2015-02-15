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
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace QS._core_c_.Serialization
{
    public class Serializer1 : ISerializer
    {
        public static readonly Serializer1 Serializer = new Serializer1();

        private Serializer1()
        {
        }


        #region ISerializer Members

        void ISerializer.Serialize(object serializedObject, Stream stream)
        {
            if (serializedObject is QS.Fx.Serialization.ISerializable)
            {
                stream.WriteByte((byte)SerializationClass.Base3_ISerializable);
                IList<QS.Fx.Base.Block> segments = Base3.Serializer.ToSegments((QS.Fx.Serialization.ISerializable)serializedObject);
                int count = 0;
                foreach (QS.Fx.Base.Block segment in segments)
                    count += (int) segment.size;
                byte[] countBytes = BitConverter.GetBytes(count);
                stream.Write(countBytes, 0, countBytes.Length);
                foreach (QS.Fx.Base.Block segment in segments)
                {
                    if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                        stream.Write(segment.buffer, (int) segment.offset, (int) segment.size);
                    else
                        throw new Exception("Cannot serialize, unmanaged memory is not supported by this serializer.");
                }
            }
            else
            {
                throw new Exception("Cannot serialize, type \"" + serializedObject.GetType().ToString() + 
                    "\" does not fall under any known serialization class.");
            }
        }

        object ISerializer.Deserialize(Stream stream)
        {
            int b = stream.ReadByte();
            if (b < 0)
                throw new Exception("Cannot read from the stream: not enough data.");
            switch ((SerializationClass)((byte)b))
            {
                case SerializationClass.Base3_ISerializable:
                {
                    byte[] countBytes = new byte[Marshal.SizeOf(typeof(int))];
                    stream.Read(countBytes, 0, countBytes.Length);
                    int count = BitConverter.ToInt32(countBytes, 0);
                    byte[] streamBytes = new byte[count];
                    if (stream.Read(streamBytes, 0, streamBytes.Length) < count)
                        throw new Exception("Count not read the whole object from the stream.");
                    return Base3.Serializer.FromSegment(new ArraySegment<byte>(streamBytes));
                }


                default:
                    throw new Exception("Unknown serialiation class.");
            }
        }

        #endregion
    }
}
