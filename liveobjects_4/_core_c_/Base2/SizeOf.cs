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

namespace QS._core_c_.Base2
{
	/// <summary>
	/// Summary description for SizeOf.
	/// </summary>
	public class SizeOf
	{
		public static uint Int32
		{
			get
			{
				return sizeOfInt32;
			}
		}

		public static uint UInt32
		{
			get
			{
				return sizeOfUInt32;
			}
		}

		public static uint UInt16
		{
			get
			{
				return sizeOfUInt16;
			}
		}

		public static uint Bool
		{
			get
			{
				return 1;
			}
		}

		public static uint Byte
		{
			get
			{
				return 1;
			}
		}

		public static uint IPAddress
		{
			get
			{
				return 4;
			}
		}

		private static uint sizeOfInt32 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(System.Int32));
		private static uint sizeOfUInt16 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(System.UInt16)); 
		private static uint sizeOfUInt32 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(System.UInt32)); 
		// private static uint sizeOfBool = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(bool)); 
		// private static uint sizeOfIPAddress = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(System.Net.IPAddress));
	}
}
