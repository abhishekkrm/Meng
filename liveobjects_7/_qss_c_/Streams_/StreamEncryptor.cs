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

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace QS._qss_c_.Streams_
{
	public class StreamEncryptor
	{
		public StreamEncryptor(string algorithmName, byte[] symmetricKey)
		{
			this.algorithmName = algorithmName;
			this.symmetricKey = symmetricKey;
		}

		private string algorithmName;
		private byte[] symmetricKey;

		public Stream Encrypt(Stream sourceStream, out byte[] initializationVector)
		{
			Stream encryptedStream = new MemoryStream();

			SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(algorithmName);
			algorithm.Key = symmetricKey;
			algorithm.GenerateIV();
			initializationVector = algorithm.IV;

			CryptoStream encrypter = new CryptoStream(encryptedStream,
				algorithm.CreateEncryptor(), CryptoStreamMode.Write);

			Copy.BlockCopy(sourceStream, encrypter);
			encrypter.FlushFinalBlock();

			encryptedStream.Seek(0, SeekOrigin.Begin);
			return encryptedStream;
		}

		public Stream Decrypt(Stream encryptedStream, byte[] initializationVector)
		{
			SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(algorithmName);
			algorithm.Key = symmetricKey;
			algorithm.IV = initializationVector;

			return new CryptoStream(encryptedStream, algorithm.CreateDecryptor(),
				CryptoStreamMode.Read);
		}
	}
}
