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

namespace QS._qss_c_.Buffering_1_
{
	/// <summary>
	/// Summary description for OutgoingBuffer.
	/// </summary>
/*	/// 
	public class SplittingBuffer : IOutgoingBuffer
	{
		public SplittingBuffer(uint maximumNumberOfBytes)
		{
			this.maximumNumberOfBlocks = maximumNumberOfBytes / 9;
			outgoingSlots = new OutgoingSlot[maximumNumberOfBlocks];				
			numberOfBlocks = 0;
			numberOfBytes = 1; // the incomplete bit
			this.maximumNumberOfBytes = maximumNumberOfBytes;
			this.incomplete = this.full = false;
		}

		private struct OutgoingSlot
		{
			public uint destinationLOID;
			public Base2.IOutgoingData dataBlock;
		}

		private OutgoingSlot[] outgoingSlots;

		private uint numberOfBlocks, maximumNumberOfBlocks, numberOfBytes, maximumNumberOfBytes;
		private bool incomplete, full;

		private static uint sizeOfUInt32 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));

		#region IOutgoingBuffer Members

		// public Base2.ISerializable RemoveContents
		// {
		//	get
		//	{
		//		return null;
		//	}
		// }

		public bool Full
		{
			get
			{
				return full;
			}
		}

		public bool append(uint destinationLOID, ref Base2.IOutgoingData outgoingData)
		{
			Debug.Assert(!full);

			uint bytesRemaining = maximumNumberOfBytes - numberOfBytes;
			uint overhead = 2 * sizeOfUInt32;
			uint bytesRequired = outgoingData.Size + overhead;

			outgoingSlots[numberOfBlocks].destinationLOID = destinationLOID;

			bool moreToAppend = bytesRequired > bytesRemaining;

			if (moreToAppend)
			{				
				outgoingSlots[numberOfBlocks++].dataBlock= outgoingData.splitAt(bytesRemaining - overhead);
				numberOfBytes += bytesRemaining;

				full = true;
			}
			else
			{
				outgoingSlots[numberOfBlocks++].dataBlock = outgoingData;
				numberOfBytes += bytesRequired;

				full = numberOfBytes > maximumNumberOfBytes - (overhead + 1) || numberOfBlocks == maximumNumberOfBlocks;
			}			

			return !moreToAppend;
		}

		#endregion

		#region ISerializable Members

		public QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.SplittingBuffer;
			}
		}

		public uint Size
		{
			get
			{
				return numberOfBytes;
			}
		}

		public void save(Base2.IBlockOfData blockOfData)
		{
			Debug.Assert(numberOfBytes <= blockOfData.SizeOfData);

			blockOfData.Buffer[blockOfData.OffsetWithinBuffer] = BitConverter.GetBytes(incomplete)[0];				
			blockOfData.consume(1);
			
			for (uint ind = 0; ind < numberOfBlocks; ind++)
			{
				Buffer.BlockCopy(BitConverter.GetBytes(outgoingSlots[ind].destinationLOID), 0, 
					blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) sizeOfUInt32);
				blockOfData.consume(sizeOfUInt32);
				
				uint blockSize = outgoingSlots[ind].dataBlock.Size;
				Buffer.BlockCopy(BitConverter.GetBytes(blockSize), 0, blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer, (int) sizeOfUInt32);
				blockOfData.consume(sizeOfUInt32);

				outgoingSlots[ind].dataBlock.save(blockOfData);
			}
		}

		public void load(Base2.IBlockOfData blockOfData)
		{
			throw new Exception("not supported");
		}

		#endregion
	}
*/	
}
