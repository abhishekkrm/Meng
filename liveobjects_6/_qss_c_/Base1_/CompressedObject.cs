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

using System.IO;
using System.IO.Compression;

namespace QS._qss_c_.Base1_
{
	[QS.Fx.Serialization.ClassID(ClassID.CompressedObject)]
	public class CompressedObject : QS._core_c_.Base.IMessage
	{
		public CompressedObject()
		{
		}

		public CompressedObject(QS._core_c_.Base.IBase1Serializable innerObject)
		{
			this.innerObject = innerObject;
		}

		private QS._core_c_.Base.IBase1Serializable innerObject;

		public QS._core_c_.Base.IBase1Serializable Object
		{
			get { return innerObject; }
		}

		#region ISerializable Members

		ClassID QS._core_c_.Base.IBase1Serializable.ClassIDAsSerializable
		{
			get { return ClassID.CompressedObject; }
		}

		void QS._core_c_.Base.IBase1Serializable.save(Stream stream)
		{
			byte[] bytes = BitConverter.GetBytes((ushort)innerObject.ClassIDAsSerializable);
			stream.Write(bytes, 0, bytes.Length);
			
			MemoryStream compressedData = new MemoryStream();
			GZipStream compressedzipStream = new GZipStream(compressedData, CompressionMode.Compress, true);
			innerObject.save(compressedzipStream);
			compressedzipStream.Close();

			bytes = BitConverter.GetBytes((int) compressedData.Length);
			stream.Write(bytes, 0, bytes.Length);

			stream.Write(compressedData.GetBuffer(), 0, (int) compressedData.Length);
		}

		void QS._core_c_.Base.IBase1Serializable.load(Stream stream)
		{
			byte[] bytes = new byte[System.Runtime.InteropServices.Marshal.SizeOf(typeof(ushort))];
			stream.Read(bytes, 0, bytes.Length);
			innerObject = (QS._core_c_.Base.IBase1Serializable)Serializer.Get.createObject((ClassID)BitConverter.ToUInt16(bytes, 0));

			bytes = new byte[System.Runtime.InteropServices.Marshal.SizeOf(typeof(int))];
			stream.Read(bytes, 0, bytes.Length);		
			int datasize = BitConverter.ToInt32(bytes, 0);

			bytes = new byte[datasize];
			stream.Read(bytes, 0, datasize);
			MemoryStream compressedData = new MemoryStream(bytes);
			GZipStream compressedzipStream = new GZipStream(compressedData, CompressionMode.Decompress);			
			innerObject.load(compressedzipStream);
		}

		#endregion
	}
}
