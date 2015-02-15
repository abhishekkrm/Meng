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

namespace QS._qss_c_.Components_1_
{
	/// <summary>
	/// Summary description for SeqNo.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.Sequencer_Wrapper_SeqNo)]
	public class SeqNo : Base2_.IIdentifiableKey, QS.Fx.Serialization.ISerializable
	{
		public SeqNo()
		{
		}

		public SeqNo(uint seqNo)
		{
			this.seqNo = seqNo;
		}

		private uint seqNo;

		public QS._qss_c_.Base2_.ContainerClass ContainerClass
		{
			get
			{
				return QS._qss_c_.Base2_.ContainerClass.DefaultContainer;
			}
		}

		public override int GetHashCode()
		{
			return this.ClassID.GetHashCode() ^ seqNo.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is SeqNo) && this.ClassID.Equals(((SeqNo) obj).ClassID) && seqNo.Equals(((SeqNo) obj).seqNo);
		}

		public override string ToString()
		{
			return seqNo.ToString();
		}

		public uint SequenceNo
		{
			get
			{
				return seqNo;
			}
		}

		public virtual int CompareTo(object obj)
		{
			Base2_.IIdentifiableKey anotherGuy = (Base2_.IIdentifiableKey) obj;
			int result = this.ClassID.CompareTo(anotherGuy.ClassID);
			return (result == 0) ? seqNo.CompareTo(((SeqNo) obj).seqNo) : result;
		}

		#region ISerializable Members

		public virtual uint Size
		{
			get
			{
				return QS._core_c_.Base2.SizeOf.UInt32;
			}
		}

		public virtual void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			seqNo = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
		}

		public virtual void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(seqNo), 0, 
				blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) QS._core_c_.Base2.SizeOf.UInt32);
			blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
		}

		public virtual QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.Sequencer_Wrapper_SeqNo;
			}
		}

		#endregion

        #region QS.Fx.Serialization.ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Sequencer_Wrapper_SeqNo, sizeof(uint), sizeof(uint), 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((uint*)(arrayptr + header.Offset)) = seqNo;
            }
            header.consume(sizeof(uint));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                seqNo = *((uint*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(uint));
        }

        #endregion
    }
}
