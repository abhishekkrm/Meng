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
	/// Summary description for FittingBuffer.
	/// </summary>
	/// 
	public class AccumulatingController : IBufferController
	{
		public AccumulatingController()
		{
			OutgoingBuffer.register_serializable();
		}

		#region OutgoingBuffer

		private class OutgoingBuffer : IOutgoingBuffer
		{
			public static void register_serializable()
			{
				QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(
					QS.ClassID.AccumulatingBufferController_OutgoingBuffer, typeof(BufferedObjects));
			}

			public OutgoingBuffer(uint bufferSize)
			{
				maximumNumberOfBytes = bufferSize;
				maximumNumberOfBlocks = maximumNumberOfBytes / 10;
				outgoingSlots = new OutgoingSlot[maximumNumberOfBlocks];
				numberOfBlocks = 0;
				numberOfBytes = QS._core_c_.Base2.SizeOf.UInt32;
			}

			private uint numberOfBytes, maximumNumberOfBytes, numberOfBlocks, maximumNumberOfBlocks;
			private OutgoingSlot[] outgoingSlots;
			private static uint messageOverhead = QS._core_c_.Base2.SizeOf.UInt16 + QS._core_c_.Base2.SizeOf.UInt32;
			private static uint minimumOverhead = messageOverhead + QS._core_c_.Base2.SizeOf.UInt32;

			public static uint MinimumOverhead
			{
				get
				{
					return minimumOverhead;
				}
			}

			private struct OutgoingSlot
			{
				public uint destinationLOID;
				public QS._core_c_.Base2.IBase2Serializable serializableObject;
			}

			#region IOutgoingBuffer Members

			public QS._core_c_.Base2.IBase2Serializable append(uint destinationLOID, QS._core_c_.Base2.IBase2Serializable serializableObject)
			{
				if (serializableObject.Size > maximumNumberOfBytes - minimumOverhead)
					throw new Exception("Data too big to fit into this buffer.");
				
				uint bytesRemaining = maximumNumberOfBytes - numberOfBytes;
				uint bytesNecessary = serializableObject.Size + messageOverhead;
				
				QS._core_c_.Base2.IBase2Serializable returnedObject = null;
				if (bytesNecessary > bytesRemaining)
				{
					returnedObject = new BufferedObjects(this);
					outgoingSlots = new OutgoingSlot[maximumNumberOfBlocks];
					numberOfBlocks = 0;
					numberOfBytes = QS._core_c_.Base2.SizeOf.UInt32;					
				}

				outgoingSlots[numberOfBlocks].destinationLOID = destinationLOID;
				outgoingSlots[numberOfBlocks].serializableObject = serializableObject;

				numberOfBlocks++;
				numberOfBytes += serializableObject.Size + messageOverhead;

				return returnedObject;
			}

			#endregion	
	
			#region Class BufferedObjects

			[QS.Fx.Serialization.ClassID(QS.ClassID.AccumulatingBufferController_OutgoingBuffer)]
			private class BufferedObjects : IBufferedObjects
			{
				public BufferedObjects()
				{
				}

				public BufferedObjects(OutgoingBuffer buffer)
				{
					outgoingSlots = buffer.outgoingSlots;
					numberOfBytes = buffer.numberOfBytes;
					numberOfBlocks = buffer.numberOfBlocks;
				}

				private OutgoingSlot[] outgoingSlots;
				private uint numberOfBytes, numberOfBlocks, blocksConsumed;

				#region IBufferedObjects Members

				public bool remove(out uint destinationLOID, out QS._core_c_.Base2.IBase2Serializable serializableObject)
				{
					if (blocksConsumed < numberOfBlocks)
					{
						destinationLOID = outgoingSlots[blocksConsumed].destinationLOID;
						serializableObject = outgoingSlots[blocksConsumed].serializableObject;
						blocksConsumed++;
						return true;
					}
					else
					{
						destinationLOID = 0;
						serializableObject = null;
						return false;
					}
				}

				#endregion

				#region ISerializable Members

				public uint Size
				{
					get
					{
						return numberOfBytes + QS._core_c_.Base2.SizeOf.UInt32;
					}
				}

				public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
				{
					numberOfBlocks = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
					outgoingSlots = new OutgoingSlot[numberOfBlocks];
					for (uint ind = 0; ind < numberOfBlocks; ind++)
					{
						outgoingSlots[ind].destinationLOID = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
						ushort classID = QS._core_c_.Base2.Serializer.loadUInt16(blockOfData);
						outgoingSlots[ind].serializableObject = 
							QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject((QS.ClassID) classID);
						outgoingSlots[ind].serializableObject.load(blockOfData);
					}
					blocksConsumed = numberOfBytes = 0;
				}

				public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
				{
					QS._core_c_.Base2.Serializer.saveUInt32(numberOfBlocks, blockOfData);
					for (uint ind = 0; ind < numberOfBlocks; ind++)
					{
						QS._core_c_.Base2.Serializer.saveUInt32(outgoingSlots[ind].destinationLOID, blockOfData);
						QS._core_c_.Base2.Serializer.saveUInt16((ushort) outgoingSlots[ind].serializableObject.ClassID, blockOfData);
						outgoingSlots[ind].serializableObject.save(blockOfData);
					}
				}

				public QS.ClassID ClassID
				{
					get
					{
						return ClassID.AccumulatingBufferController_OutgoingBuffer;
					}
				}

				#endregion
			}

			#endregion
		}

		#endregion

		#region IBufferController Members

		public uint MinimumOverhead
		{
			get
			{
				return OutgoingBuffer.MinimumOverhead;
			}
		}

		public IOutgoingBuffer CreateOutgoingBuffer(uint bufferSize)
		{		
			return new OutgoingBuffer(bufferSize);
		}

		#endregion
	}
}
