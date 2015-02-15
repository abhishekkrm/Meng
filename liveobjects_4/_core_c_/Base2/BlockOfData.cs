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
using System.Diagnostics;

namespace QS._core_c_.Base2
{
	/// <summary>
	/// Summary description for BlockOfData.
	/// </summary>
	public interface IBlockOfData : IBase2Serializable, QS.Fx.Serialization.ISerializable
	{
		byte[] Buffer
		{
			get;
		}

		uint OffsetWithinBuffer
		{
			get;
		}

		uint SizeOfData
		{
			get;
		}

		void consume(uint numberOfBytes);

		IBlockOfData chopOff(uint bytesToChopOff);

        IBlockOfData ShallowCopy
        {
            get;
        }

        IBlockOfData StandaloneCopy
        {
            get;
        }
    }

//	[System.Serializable]
	[QS.Fx.Serialization.ClassID(QS.ClassID.BlockOfData)]
	public class BlockOfData : IBlockOfData, IOutgoingData, Base.IBase1Serializable // , System.Runtime.Serialization.ISerializable
	{
		public System.IO.MemoryStream AsStream
		{
			get
			{
				return new System.IO.MemoryStream(bufferWithData, (int) offsetWithinBuffer, (int) sizeOfData);
			}
		}

		public BlockOfData()
		{
		}

		public BlockOfData(uint size)
			: this(new byte[size], 0, size)
		{
		}

		public BlockOfData(System.IO.MemoryStream memoryStream) 
			: this(memoryStream.GetBuffer(), 0, (uint) memoryStream.Length)
		{
		}

		public BlockOfData(byte[] bufferWithData) 
			: this(bufferWithData, 0, (uint) bufferWithData.Length)
		{
		}

		public BlockOfData(byte[] bufferWithData, uint offsetWithinBuffer, uint sizeOfData)
		{
			this.bufferWithData = bufferWithData;
			this.initialOffsetWithinBuffer = this.offsetWithinBuffer = offsetWithinBuffer;
			this.initialSizeOfData = this.sizeOfData = sizeOfData;
		}

/*
		#region System.Runtime.Serialization.ISerializable Members

		protected BlockOfData(System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context)
		{
			sizeOfData = info.GetUInt32("length");
			bufferWithData = info.

			n1 = info.GetInt32("i");
			n2 = info.GetInt32("j");
			str = info.GetString("k");
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("length", sizeOfData);
			info.A
			info.AddValue("buffer", bufferWithData, typeof(byte[]));
			info.AddValue("offset", offsetWithinBuffer);
		}

		#endregion
*/

		private byte[] bufferWithData;
		private uint initialOffsetWithinBuffer, offsetWithinBuffer, initialSizeOfData, sizeOfData;

        public IBlockOfData ShallowCopy
        {
            get
            {
                return new BlockOfData(bufferWithData, offsetWithinBuffer, sizeOfData);
            }
        }

        public IBlockOfData StandaloneCopy
        {
            get
            {
                byte[] bytes = new byte[sizeOfData];
                System.Buffer.BlockCopy(bufferWithData, (int) offsetWithinBuffer, bytes, 0, (int) sizeOfData);
                return new BlockOfData(bytes, 0, sizeOfData);
            }
        }

        public void resetCursor()
		{
			this.offsetWithinBuffer = this.initialOffsetWithinBuffer;
			this.sizeOfData = this.initialSizeOfData;
		}

		#region IBlockOfData Members

		public void consume(uint numberOfBytes)
		{
			Debug.Assert(numberOfBytes <= sizeOfData);
			offsetWithinBuffer += numberOfBytes;
			sizeOfData -=numberOfBytes;
		}

		public IBlockOfData chopOff(uint bytesToChopOff)
		{
			Debug.Assert(bytesToChopOff <= sizeOfData);
			IBlockOfData choppedOffBlock = new BlockOfData(bufferWithData, offsetWithinBuffer, bytesToChopOff);

			offsetWithinBuffer += bytesToChopOff;
			sizeOfData -= bytesToChopOff;

			return choppedOffBlock;
		}

		public byte[] Buffer
		{
			get
			{
				return bufferWithData;
			}
		}

		public uint OffsetWithinBuffer
		{
			get
			{
				return offsetWithinBuffer;
			}
		}

		public uint SizeOfData
		{
			get
			{
				return sizeOfData;
			}
		}

		#endregion

		#region ISerializable Members

		public virtual QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.BlockOfData;
			}
		}

		public virtual uint Size
		{
			get
			{				
				return sizeOfData + SizeOf.UInt32;
			}
		}

		public virtual void save(Base2.IBlockOfData blockOfData)
		{
			// sanity check
			if (blockOfData.SizeOfData < this.Size)
			{
				throw new Exception("Cannot serialize " + this.Size.ToString() + 
					" into a buffer with space for only " + blockOfData.SizeOfData.ToString() + " bytes.");
			}

			System.Buffer.BlockCopy(BitConverter.GetBytes(sizeOfData), 0, 
				blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) SizeOf.UInt32);
			blockOfData.consume(SizeOf.UInt32);
			System.Buffer.BlockCopy(this.bufferWithData, (int) this.offsetWithinBuffer, 
				blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) this.sizeOfData); 
			blockOfData.consume(this.sizeOfData);
		}

		public virtual void load(Base2.IBlockOfData blockOfData)
		{
			this.initialOffsetWithinBuffer = this.offsetWithinBuffer = 0;
			this.initialSizeOfData = this.sizeOfData = 
				BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
			blockOfData.consume(SizeOf.UInt32);
			this.bufferWithData = new byte[this.initialSizeOfData];
			System.Buffer.BlockCopy(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, 
				this.bufferWithData, 0, (int) this.initialSizeOfData);
			blockOfData.consume(this.initialSizeOfData);
		}

		private void serializeTo(byte[] bufferForData, uint offsetWithinBuffer, uint spaceWithinBuffer, uint bytesToSerialize)
		{
			Debug.Assert(bytesToSerialize <= sizeOfData && bytesToSerialize <= spaceWithinBuffer);

			System.Buffer.BlockCopy(this.bufferWithData, (int) this.offsetWithinBuffer, bufferForData, (int) offsetWithinBuffer, (int) bytesToSerialize); 
			this.consume(bytesToSerialize);
		}

		#endregion

        #region QS.Fx.Serialization.ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort) QS.ClassID.BlockOfData, sizeof(uint), sizeof(uint) + (int) sizeOfData, ((bufferWithData != null) && (sizeOfData > 0)) ? 1 : 0);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* freeptr = arrayptr + header.Offset;
                *((uint*) freeptr) = sizeOfData;
            }
            header.consume(sizeof(uint));
            if ((bufferWithData != null) && (sizeOfData > 0))
                data.Add(new QS.Fx.Base.Block(bufferWithData, offsetWithinBuffer, sizeOfData));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte* ourptr = arrayptr + header.Offset;
                sizeOfData = *((uint*)ourptr);
            }
            header.consume(sizeof(uint));
            if (sizeOfData > 0)
            {
                bufferWithData = new byte[sizeOfData];
                System.Buffer.BlockCopy(data.Array, data.Offset, bufferWithData, 0, (int) sizeOfData);
                data.consume((int)sizeOfData);
            }
            else
                bufferWithData = null;
            initialOffsetWithinBuffer = offsetWithinBuffer = 0;
            initialSizeOfData = sizeOfData;
        }

        #endregion

		#region IOutgoingData Members

		public IOutgoingData splitAt(uint bytesToChopOff)
		{
			return (IOutgoingData) this.chopOff(bytesToChopOff);
		}

		#endregion

        public override string ToString()
        {
            return "BlockOfData(" + sizeOfData.ToString() + ")";
        }

		#region ISerializable Members

		QS.ClassID QS._core_c_.Base.IBase1Serializable.ClassIDAsSerializable
		{
			get { return this.ClassID; }
		}

		void QS._core_c_.Base.IBase1Serializable.save(System.IO.Stream stream)
		{
			byte[] buffer;
			buffer = System.BitConverter.GetBytes(sizeOfData);
			stream.Write(buffer, 0, buffer.Length);
			stream.Write(bufferWithData, (int) offsetWithinBuffer, (int) sizeOfData);
		}

		void QS._core_c_.Base.IBase1Serializable.load(System.IO.Stream stream)
		{
			byte[] buffer = new byte[System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint))];
			stream.Read(buffer, 0, buffer.Length);
			initialSizeOfData = sizeOfData = System.BitConverter.ToUInt32(buffer, 0);
			bufferWithData = new byte[sizeOfData];
			stream.Read(bufferWithData, 0, (int) sizeOfData);
			initialOffsetWithinBuffer = offsetWithinBuffer = 0;
		}

		#endregion
	}
}
