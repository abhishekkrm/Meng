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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for OutgoingVector.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.OutgoingVector)]
	public class OutgoingVector : QS._core_c_.Base2.IBase2Serializable, QS._core_c_.Base2.IOutgoingData
	{
		public OutgoingVector()
		{
		}

		public OutgoingVector(QS._core_c_.Base2.IBlockOfData[] blocksOfData)
		{
			this.blocksOfData = blocksOfData;
			foreach (QS._core_c_.Base2.IBlockOfData blockOfData in blocksOfData)
				remainingSize += blockOfData.SizeOfData;
		}

		private QS._core_c_.Base2.IBlockOfData[] blocksOfData;
		private uint remainingSize = 0;
		private uint indexOfTheFirstBlock = 0;

		#region ISerializable Members

		public QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.OutgoingVector;
			}
		}

		public uint Size
		{
			get
			{
				return remainingSize;
			}
		}

		public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			uint nbytes = this.remainingSize;
			this.serializeTo(blockOfData.Buffer, blockOfData.OffsetWithinBuffer, blockOfData.SizeOfData, nbytes);
			blockOfData.consume(nbytes);
		}

		public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			throw new Exception("not supported");
		}

		private void serializeTo(byte[] bufferForData, uint offsetWithinBuffer, uint spaceWithinBuffer, uint bytesToSerialize)
		{
			Debug.Assert(bytesToSerialize <= spaceWithinBuffer && bytesToSerialize <= remainingSize);
			
			while (bytesToSerialize > 0)
			{
				uint chunkSize = (blocksOfData[indexOfTheFirstBlock].SizeOfData < bytesToSerialize) 
					? blocksOfData[indexOfTheFirstBlock].SizeOfData : bytesToSerialize;

				Buffer.BlockCopy(blocksOfData[indexOfTheFirstBlock].Buffer, (int) blocksOfData[indexOfTheFirstBlock].OffsetWithinBuffer,
					bufferForData, (int) offsetWithinBuffer, (int) chunkSize);

				blocksOfData[indexOfTheFirstBlock].consume(chunkSize);
				if (blocksOfData[indexOfTheFirstBlock].SizeOfData == 0)
					indexOfTheFirstBlock++;
				remainingSize -= chunkSize;
				offsetWithinBuffer += chunkSize;
				bytesToSerialize -= chunkSize;
			}
		}

		#endregion

		#region IOutgoingData Members

		public QS._core_c_.Base2.IOutgoingData splitAt(uint bytesToChopOff)
		{
			Debug.Assert(bytesToChopOff <= remainingSize);

			uint blocksToChopOff = 0, bytesInBlocks = 0;
			while (bytesInBlocks < bytesToChopOff)
				bytesInBlocks += blocksOfData[indexOfTheFirstBlock + (blocksToChopOff++)].SizeOfData;

			QS._core_c_.Base2.IBlockOfData[] choppedOffBlocks = new QS._core_c_.Base2.IBlockOfData[blocksToChopOff];
			for (uint ind = 0; ind < (blocksToChopOff - 1); ind++)
				choppedOffBlocks[ind] = blocksOfData[indexOfTheFirstBlock + ind];

			uint remainder = bytesInBlocks - bytesToChopOff;
			if (remainder > 0)
			{
				choppedOffBlocks[blocksToChopOff - 1] = blocksOfData[indexOfTheFirstBlock + blocksToChopOff - 1].chopOff(
					blocksOfData[indexOfTheFirstBlock + blocksToChopOff - 1].SizeOfData - remainder);
				indexOfTheFirstBlock += blocksToChopOff - 1;
			}
			else
			{
				choppedOffBlocks[blocksToChopOff - 1] = blocksOfData[indexOfTheFirstBlock + blocksToChopOff - 1];
				indexOfTheFirstBlock += blocksToChopOff;
			}

			remainingSize -= bytesToChopOff;

			return new OutgoingVector(choppedOffBlocks);
		}

		#endregion
	}
}
