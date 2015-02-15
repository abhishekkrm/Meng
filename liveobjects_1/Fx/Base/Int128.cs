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

namespace QS.Fx.Base
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    public struct Int128 : QS.Fx.Serialization.ISerializable, IComparable<Int128>, IEquatable<Int128>
    {
        #region Constructor

        public Int128(ulong u1, ulong u2)
        {
            this.u1 = u1;
            this.u2 = u2;
        }

        public unsafe Int128(Guid guid)
        {
            ulong u1, u2;

            fixed (byte* pguid = guid.ToByteArray())
            {
                byte* pu1 = (byte*)(&u1);
                byte* pu2 = (byte*)(&u2);

                pu1[0] = pguid[6];
                pu1[1] = pguid[7];

                pu1[2] = pguid[4];
                pu1[3] = pguid[5];

                pu1[4] = pguid[0];
                pu1[5] = pguid[1];
                pu1[6] = pguid[2];
                pu1[7] = pguid[3];

                pu2[0] = pguid[15];
                pu2[1] = pguid[14];
                pu2[2] = pguid[13];
                pu2[3] = pguid[12];
                pu2[4] = pguid[11];
                pu2[5] = pguid[10];
                pu2[6] = pguid[9];
                pu2[7] = pguid[8];
            }

            this.u1 = u1;
            this.u2 = u2;
        }

        #endregion

        private ulong u1, u2;

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Nothing, 2 * sizeof(ulong)); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((ulong*)pheader) = u1;
                *((ulong*)(pheader + sizeof(ulong))) = u2;
            }
            header.consume(2 * sizeof(ulong));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                u1 = *((ulong*)pheader);
                u2 = *((ulong*)(pheader + sizeof(ulong)));
            }
            header.consume(2 * sizeof(ulong));
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return (u1 != 0UL) ? (u1.ToString("x") + u2.ToString("x16")) : u2.ToString("x");
        }

        public override int GetHashCode()
        {
            return u1.GetHashCode() ^ u2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Int128)
            {
                Int128 other = (Int128) obj;
                return u1.Equals(other.u1) && u2.Equals(other.u2);
            }
            else
                return false;
        }

        #endregion

        #region IComparable<Int128> Members

        public int CompareTo(Int128 other)
        {
            int comparison_result = u1.CompareTo(other.u1);
            return (comparison_result != 0) ? comparison_result : u2.CompareTo(other.u2);
        }

        #endregion

        #region IEquatable<Int128> Members

        public bool Equals(Int128 other)
        {
            return u1.Equals(other.u1) && u2.Equals(other.u2);
        }

        #endregion

        #region FromString

        public static Int128 FromString(string s)
        {
            Int128 result = new Int128();

            if (s.Length > 16)
            {
                string ss = s.PadLeft(32, '0');
                if (ss.Length != 32)
                    throw new Exception();

                string s1 = ss.Substring(0, 16);
                string s2 = ss.Substring(16, 16);

                result.u1 = Convert.ToUInt64(s1, 16);
                result.u2 = Convert.ToUInt64(s2, 16);
            }
            else
            {
                result.u1 = 0;
                result.u2 = Convert.ToUInt64(s, 16);
            }

            return result;
        }

        #endregion
    }
}
