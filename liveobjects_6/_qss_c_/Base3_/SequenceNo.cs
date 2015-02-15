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

namespace QS._qss_c_.Base3_
{
    [QS.Fx.Serialization.ClassID(ClassID.SequenceNo)]
    public class SequenceNo : QS.Fx.Serialization.ISerializable
    {
        public SequenceNo(uint seqno)
        {
            this.seqno = seqno;
        }

        public SequenceNo()
        {
        }

        private uint seqno;

        public uint Value
        {
            set { seqno = value; }
            get { return seqno; }
        }

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.SequenceNo, (ushort)sizeof(uint), sizeof(uint), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((uint*)(pbuffer + header.Offset)) = seqno;
            }
            header.consume(sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                seqno  = * ((uint*)(pbuffer + header.Offset));
            }
            header.consume(sizeof(uint));
        }

        #endregion

        public override string ToString()
        {
            return seqno.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj is SequenceNo && ((SequenceNo) obj).seqno.Equals(seqno)) || (obj is uint && ((uint) obj).Equals(seqno));
        }

        public override int GetHashCode()
        {
            return seqno.GetHashCode();
        }
    }
}
