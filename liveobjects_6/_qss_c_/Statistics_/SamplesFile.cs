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
using System.Threading;

namespace QS._qss_c_.Statistics_
{
/*
    public class SamplesFile : IDisposable
    {
        public SamplesFile(Core.IFile output_file, string attrribute_name, string name, string description,
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description)
        {
            this.output_file = output_file;
            this.attrribute_name = attrribute_name;
            this.name = name;
            this.description = description;
            this.x_name = x_name;
            this.x_units = x_units;
            this.x_description = x_description;
            this.y_name = y_name;
            this.y_units = y_units;
            this.y_description = y_description;

            byte[] bytes;
            
            bytes = Encoding.Unicode.GetBytes(attrribute_name);
            _Write(BitConverter.GetBytes(bytes.Length));
            _Write(bytes);

            bytes = new byte[1];
            bytes[0] = (byte) Serialization.SerializationClass.Base3_ISerializable;
            _Write(bytes);

            QS.Fx.Serialization.SerializableInfo serializableInfo = 
                new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.TMS_Data_Data1D, 2 * sizeof(ushort) + sizeof(int));
            Base2.StringWrapper w1 = new Base2.StringWrapper(name);
            Base2.StringWrapper w2 = new Base2.StringWrapper(description);
            serializableInfo.AddAnother(w1.SerializableInfo);
            serializableInfo.AddAnother(w2.SerializableInfo);
            serializableInfo.AddAnother(((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo);
            serializableInfo.AddAnother(((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo);

            int total_headersize = serializableInfo.HeaderSize + sizeof(ushort) + sizeof(uint);

            Base3.WritableArraySegment<byte> header = new Base3.WritableArraySegment<byte>(total_headersize);
            IList<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>(1);
            buffers.Add(header.ArraySegment);
            unsafe
            {
                fixed (byte* pbuffer = header.Array)
                {
                    byte* pheader = pbuffer + header.Offset;
                    *((uint*)pheader) = (uint)total_headersize;
                    *((ushort*)(pheader + sizeof(uint))) = serializableInfo.ClassID;
                }
            }
            header.consume(sizeof(ushort) + sizeof(uint));

            w1.SerializeTo(ref header, ref buffers);
            w2.SerializeTo(ref header, ref buffers);

            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*)pheader) = 666666; // this.data.Length;--------------------------
                pheader += sizeof(int);
                for (int ind = 0; ind < this.data.Length; ind++)
                {
                    *((double*)pheader) = this.data[ind];
                    pheader += sizeof(double);
                }
                *((ushort*)pheader) = ((QS.Fx.Serialization.ISerializable)xAxis).SerializableInfo.ClassID;
                *((ushort*)(pheader + sizeof(ushort))) = ((QS.Fx.Serialization.ISerializable)yAxis).SerializableInfo.ClassID;
            }

            // data.SerializeTo(ref header, ref buffers);




/-*

 --------------------------
 
            header.consume(2 * sizeof(ushort) + sizeof(int) + this.data.Length * sizeof(double));
            ((QS.Fx.Serialization.ISerializable)xAxis).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable)yAxis).SerializeTo(ref header, ref data); 
 *-/ 

/-*
            return buffers;

            ---------------------

                IList<ArraySegment<byte>> segments = Base3.Serializer.ToSegments((QS.Fx.Serialization.ISerializable)serializedObject);
                int count = 0;
                foreach (ArraySegment<byte> segment in segments)
                    count += segment.Count;
                byte[] countBytes = BitConverter.GetBytes(count);
                stream.Write(countBytes, 0, countBytes.Length);
                foreach (ArraySegment<byte> segment in segments)
                    stream.Write(segment.Array, segment.Offset, segment.Count);
*-/ 
        }

        private Core.IFile output_file;
        private string attrribute_name;
        private int closed;

/-*
        private string name, description, x_name, x_units, x_description, y_name, y_units, y_description;
*-/

        private void _Write(byte[] bytes)
        {
            // .................
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (!Interlocked.CompareExchange<int>(ref closed, 1, 0))
            {
                // should finalize this now...........
            }
        }

        #endregion
    }
*/
}
