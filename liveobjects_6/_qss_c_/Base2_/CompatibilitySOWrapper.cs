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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for BlockWrapper.
	/// </summary>

	[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
	public class CompatibilitySOWrapper : QS._core_c_.Base2.BlockOfData
	{
		public CompatibilitySOWrapper(QS._core_c_.Base.IBase1Serializable serializable) : base(convertToStream(serializable))
		{
		}

		private static System.IO.MemoryStream convertToStream(QS._core_c_.Base.IBase1Serializable serializableObject)
		{
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
			ushort classID = (ushort) serializableObject.ClassIDAsSerializable;
			memoryStream.Write(BitConverter.GetBytes(classID), 0, (int) QS._core_c_.Base2.SizeOf.UInt16);
			serializableObject.save(memoryStream);
			return memoryStream;
		}

		private static QS._core_c_.Base.IBase1Serializable loadObjectFromBytes(byte[] buffer, uint offset, uint size)
		{
			ushort classID = BitConverter.ToUInt16(buffer, (int) offset);			
			QS._core_c_.Base.IBase1Serializable serializableObject = Base1_.Serializer.Get.createObject((QS.ClassID) classID);
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(
				buffer, (int) (offset + QS._core_c_.Base2.SizeOf.UInt16), (int) (size - QS._core_c_.Base2.SizeOf.UInt16));
			serializableObject.load(memoryStream);
			return serializableObject;
		}

		public QS._core_c_.Base.IBase1Serializable ReconstructObject
		{
			get
			{
				return loadObjectFromBytes(this.Buffer, this.OffsetWithinBuffer, this.SizeOfData);
			}
		}

		public static byte[] Object2ByteArray(QS._core_c_.Base.IBase1Serializable serializableObject)
		{
			System.IO.MemoryStream memoryStream = convertToStream(serializableObject);
			byte[] bytes = new byte[memoryStream.Length];
			System.Buffer.BlockCopy(memoryStream.GetBuffer(), 0, bytes, 0, (int) memoryStream.Length);
			return bytes;
		}

		public static QS._core_c_.Base.IBase1Serializable ByteArray2Object(byte[] objectArrayOfBytes)
		{
			return loadObjectFromBytes(objectArrayOfBytes, 0, (uint) objectArrayOfBytes.Length);
		}
	}
}
