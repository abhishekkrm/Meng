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
using System.Net;

namespace QS._qss_c_.Virtualization_
{
	/// <summary>
	/// Summary description for SubnetAddress.
	/// </summary>
/* 
	public class SubnetAddress
	{
		public SubnetAddress()
		{
		}

		public SubnetAddress(IPAddress localAddress, IPAddress subnetMask)
		{
			byte[] localAddressAsBytes = localAddress.GetAddressBytes();
			byte[] subnetMaskAsBytes = subnetMask.GetAddressBytes();

			uint numberOfBytes = 0; 
			while (numberOfBytes < 4 && subnetMaskAsBytes[numberOfBytes] == 255)
				numberOfBytes++;
			for (uint ind = numberOfBytes; ind < 4; ind++)
			{
				if (subnetMaskAsBytes[ind] != 0)
					throw new Exception("subnet mask was given in an incorrect format");
			}
			addressBytes = new byte[numberOfBytes];
			for (uint ind = 0; ind < numberOfBytes; ind++)
				addressBytes[ind] = localAddressAsBytes[ind];
		}

		public SubnetAddress(string addressAsString)
		{
			this.AsString = addressAsString;
		}

		public string AsString
		{
			get
			{
				string result = null;
				for (uint ind = 0; ind < 4; ind++)
					result = ((result != null) ? "." : "") + ((ind < addressBytes.Length) ? addressBytes[ind].ToString() : "x");
				return result;
			}

			set
			{
				byte[] stringBytes = new byte[4];
				int position = 0, numberOfBytes = 0;
				for (uint ind = 0; ind < 4; ind++)
				{
					int dotposition = ((ind < 3) ? (value.IndexOf(".", position)) : value.Length);
					string cell = value.Substring(position, dotposition - position);
					position = dotposition;
					if (!cell.Equals("x"))
					{
						stringBytes[ind] = Convert.ToByte(cell);
						numberOfBytes++;
					}
				}

				addressBytes = new byte[numberOfBytes];
				for (uint ind = 0; ind < numberOfBytes; ind++)
					addressBytes[ind] = stringBytes[ind];
			}
		}

		public override int GetHashCode()
		{
			return addressBytes.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is SubnetAddress) && addressBytes.Equals(((SubnetAddress) obj).addressBytes);
		}

		public override string ToString()
		{
			return this.AsString;
		}

		public uint Length
		{
			get
			{
				return (uint) addressBytes.Length;
			}
		}

		[NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		private byte[] addressBytes;
	}
*/	
}
