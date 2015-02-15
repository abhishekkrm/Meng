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

namespace QS._qss_d_.Helpers_
{
	public static class Compressor
	{
		public static byte[] Compress(byte[] data)
		{
			return Compress(data, 0, data.Length);
		}

		public static byte[] Compress(byte[] data, int offset, int length)
		{
			MemoryStream ms = new MemoryStream();
			GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
			compressedzipStream.Write(data, offset, length);
			compressedzipStream.Close();

			byte[] compressedData = new byte[ms.Length];
			Buffer.BlockCopy(ms.GetBuffer(), 0, compressedData, 0, (int) ms.Length);

			return compressedData;
		}

		public static byte[] Uncompress(byte[] data)
		{
			return Uncompress(data, 0, data.Length);
		}

		private const int BUFFERSIZE = 1000;
		public static byte[] Uncompress(byte[] data, int offset, int length)
		{
			MemoryStream ms = new MemoryStream(data, offset, length);
			GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);

			MemoryStream data_ms = new MemoryStream();
			byte[] data_buffer = new byte[BUFFERSIZE];
			while (true)
			{
				int nbytesread = compressedzipStream.Read(data_buffer, 0, BUFFERSIZE);
				if (nbytesread == 0)
					break;

				data_ms.Write(data_buffer, 0, nbytesread);
			}

			byte[] uncompressedData = new byte[data_ms.Length];
			Buffer.BlockCopy(data_ms.GetBuffer(), 0, uncompressedData, 0, (int) data_ms.Length);

			return uncompressedData;
		}
	}
}
