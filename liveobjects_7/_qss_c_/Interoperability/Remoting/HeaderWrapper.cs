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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;

namespace QS._qss_c_.Interoperability.Remoting
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Interoperability_Remoting_HeaderWrapper)]
    public class HeaderWrapper : QS.Fx.Serialization.ISerializable
    {
        public HeaderWrapper()
        {
        }

        public HeaderWrapper(ITransportHeaders transportHeaders)       
        {
            buffers = new System.Collections.Generic.List<ArraySegment<byte>>();

            size = 0;

            foreach (System.Collections.DictionaryEntry entry in transportHeaders)
            {
                string k = (string) entry.Key;
                string v = (string) entry.Value;

                byte[] kb = System.Text.ASCIIEncoding.ASCII.GetBytes(k);
                byte[] vb = System.Text.ASCIIEncoding.ASCII.GetBytes(v);

                buffers.Add(new ArraySegment<byte>(kb));
                buffers.Add(new ArraySegment<byte>(vb));

                size += kb.Length + vb.Length;
            }

            headerSize = sizeof(ushort) * (buffers.Count + 1);
            size += headerSize;
        }

        private System.Collections.Generic.IList<ArraySegment<byte>> buffers;
        private int headerSize, size;

        public ITransportHeaders TransportHeaders
        {
            get
            {
                ITransportHeaders result = new TransportHeaders();

                System.Collections.Generic.IEnumerator<ArraySegment<byte>> en = buffers.GetEnumerator();
                while (en.MoveNext())
                {
                    ArraySegment<byte> seg = (ArraySegment<byte>) en.Current;
                    string k = System.Text.ASCIIEncoding.ASCII.GetString(seg.Array, seg.Offset, seg.Count);
                    if (!en.MoveNext())
                        throw new ArgumentException();
                    seg = (ArraySegment<byte>)en.Current;
                    string v = System.Text.ASCIIEncoding.ASCII.GetString(seg.Array, seg.Offset, seg.Count);

                    result[k] = v;
                }

                return result;
            }
        }

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            { 
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort) QS.ClassID.Interoperability_Remoting_HeaderWrapper, (ushort) headerSize, size, buffers.Count); 
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                ushort* headerptr = (ushort *) (arrayptr + header.Offset);
                *headerptr = (ushort) buffers.Count;
                foreach (ArraySegment<byte> seg in buffers)
                {
                    headerptr++;
                    *headerptr = (ushort) seg.Count;
                    data.Add(new QS.Fx.Base.Block(seg));
                }
            }

            header.consume(headerSize);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            buffers = new System.Collections.Generic.List<ArraySegment<byte>>();

            fixed (byte* arrayptr = header.Array)
            {
                ushort* headerptr = (ushort*) (arrayptr + header.Offset);
                int nbuffers = (int) (*headerptr);
                for (int ind = 0; ind < nbuffers; ind++)
                {
                    headerptr++;
                    int nbytes = (int) (*headerptr);
                    ArraySegment<byte> seg = new ArraySegment<byte>(data.Array, data.Offset, nbytes);
                    buffers.Add(seg);
                    data.consume(nbytes);
                }

                header.consume(sizeof(ushort) * (nbuffers + 1));
            }
        }

        #endregion
    }
}
