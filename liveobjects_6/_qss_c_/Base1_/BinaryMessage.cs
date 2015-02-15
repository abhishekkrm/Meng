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
using System.IO;

namespace QS._qss_c_.Base1_
{
	/// <summary>
	/// Summary description for BinaryMessage.
	/// </summary>
	public class BinaryMessage : QS._core_c_.Base.IMessage
	{
		public static  QS._core_c_.Base.CreateSerializable Factory = new QS._core_c_.Base.CreateSerializable(createSerializable);

		private static QS._core_c_.Base.IBase1Serializable createSerializable()
		{
			return new BinaryMessage(null);
		}

		public BinaryMessage(byte[] binaryBlock)
		{
			this.binaryBlock = binaryBlock;
		}

		private byte[] binaryBlock;

		public byte[] Bytes
		{
			get
			{
				return binaryBlock;
			}
		}

		#region ISerializable Members

		public ClassID ClassIDAsSerializable
		{
			get
			{
				return ClassID.BinaryMessage;
			}
		}

		public void save(Stream memoryStream)
		{
			uint numberOfBytes = (uint) binaryBlock.Length;
			byte[] buffer = System.BitConverter.GetBytes(numberOfBytes);
			memoryStream.Write(buffer, 0, buffer.Length);
			memoryStream.Write(binaryBlock, 0, binaryBlock.Length);
		}

		public void load(Stream memoryStream)
		{
			byte[] buffer = new byte[4];
			memoryStream.Read(buffer, 0, 4);
			uint numberOfBytes = System.BitConverter.ToUInt32(buffer, 0);
			binaryBlock = new byte[numberOfBytes];
			memoryStream.Read(binaryBlock, 0, (int) numberOfBytes); 
		}

		#endregion
	}		
}
