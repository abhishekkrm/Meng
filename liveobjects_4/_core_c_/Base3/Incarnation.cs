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

namespace QS._core_c_.Base3
{
    [Serializable]
	public struct Incarnation : System.IComparable<Incarnation>, QS.Fx.Serialization.ISerializable
	{
		public static Incarnation Current
		{
			get { return new Incarnation(System.DateTime.Now); }
		}

		public Incarnation(System.DateTime time) 
			: this(((uint) Math.Floor(((TimeSpan) (time - (new DateTime(2000, 1, 1)))).TotalSeconds)))
		{
		}

		public Incarnation(double x) : this((uint) Math.Floor(100 * x))
		{
		}

		public Incarnation(uint incarnationSeqNo)
		{
			this.incarnationSeqNo = incarnationSeqNo;
		}

		private uint incarnationSeqNo;

		public uint SeqNo
		{
			get { return incarnationSeqNo; }
			set { incarnationSeqNo = value; }
		}

		public static explicit operator uint(Incarnation incarnation)
		{
			return incarnation.incarnationSeqNo;
		}

		public static explicit operator Incarnation(uint incarnationSeqNo)
		{
			return new Incarnation(incarnationSeqNo);
		}

		public override string ToString()
		{
			return incarnationSeqNo.ToString(); // "0000000000"); // "X8"
		}

        public override bool Equals(object obj)
        {
            return (obj is Incarnation) && (((Incarnation)obj).incarnationSeqNo == incarnationSeqNo);
        }

        #region IComparable<Incarnation> Members

        public int CompareTo(Incarnation other)
        {
            return incarnationSeqNo.CompareTo(other.incarnationSeqNo);
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)sizeof(uint), sizeof(uint), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* bufferptr = header.Array)
            {
                *((uint*)(bufferptr + header.Offset)) = incarnationSeqNo;
            }
            header.consume(sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* bufferptr = header.Array)
            {
                incarnationSeqNo  = *((uint*)(bufferptr + header.Offset));
            }
            header.consume(sizeof(uint));
        }

        #endregion

        public override int GetHashCode()
        {
            return incarnationSeqNo.GetHashCode();
        }
    }
}
