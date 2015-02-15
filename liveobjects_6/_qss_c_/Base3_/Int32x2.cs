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
    [QS.Fx.Serialization.ClassID(ClassID.Int32x2)]
    public class Int32x2 : QS.Fx.Serialization.ISerializable
    {
        public Int32x2(int int1, int int2)
        {
            this.int1 = int1;
            this.int2 = int2;
        }

        public Int32x2()
        {
        }

        private int int1, int2;

        public int Int1
        {
            set { int1 = value; }
            get { return int1; }
        }

        public int Int2
        {
            set { int2 = value; }
            get { return int2; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Int32x2, 2 * sizeof(int), 2 * sizeof(int), 0); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((int*) pheader) = int1;
                *((int*) (pheader + sizeof(int))) = int2;
            }
            header.consume(2 * sizeof(int));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                int1 = *((int*)pheader);
                int2 = *((int*)(pheader + sizeof(int)));
            }
            header.consume(2 * sizeof(int));
        }

        #endregion

        public override string ToString()
        {
            return "(" + int1.ToString() + ", " + int2.ToString() + ")";
        }

        public override bool Equals(object obj)
        {
            if (obj is Int32x2)
            {
                Int32x2 other = (Int32x2) obj;
                return int1 == other.int1 && int2 == other.int2;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return int1 ^ int2;
        }
    }
}
