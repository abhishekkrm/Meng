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

namespace QS._qss_c_.Remoting_.Encryption
{
	public static class Keys
	{
		public static byte[] Load(string filename)
		{
			byte[] bytes;
			using (FileStream stream = new FileStream(filename, FileMode.Open))
			{
				bytes = new byte[(int) stream.Length];
				if (stream.Read(bytes, 0, bytes.Length) != bytes.Length)
					throw new Exception("Could not load the whole key from file.");
			}

			return bytes;
		}

		public static void Save(byte[] bytes, string filename)
		{
			using (FileStream stream = new FileStream(filename, FileMode.CreateNew))
			{
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
			}
		}

		public enum AlgorithmClass
		{
			DES, TripleDES, RC2, Rijndael
		}

		public static string AlgorithmName(AlgorithmClass algorithmClass)
		{
			return algorithmClass.ToString();
		}

		public static IEnumerable<int> KeySizes(string algorithmName)
		{
			SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(algorithmName);
			if (algorithm == null)
				throw new ArgumentException("Unknown algorithm.");

			foreach (KeySizes size in algorithm.LegalKeySizes)
			{
				if (size.SkipSize != 0)
				{
					for (int i = size.MinSize; i <= size.MaxSize; i = i + size.SkipSize)
						yield return i;
				}
				else
				{
					yield return size.MinSize;

					if (size.MinSize != size.MaxSize)
						yield return size.MaxSize;
				}
			}
		}

		public static byte[] GenerateKey(string algorithmName, int keySize)
		{
			SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(algorithmName);
			algorithm.KeySize = keySize;
			algorithm.GenerateKey();

			return algorithm.Key;
		}
	}
}
