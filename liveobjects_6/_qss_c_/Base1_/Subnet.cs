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
using System.Diagnostics;

namespace QS._qss_c_.Base1_
{
	/// <summary>
	/// Summary description for Subnet.
	/// </summary>
	[Serializable]
	public struct Subnet
	{
        private static void _SetFromString(string s, out IPAddress out_mask, out byte[] out_representative)
        {
            int colon = s.IndexOf(':');
            if (colon < 0)
            {
                int slash = s.IndexOf('/');
                if (slash < 0)
                {
                    string[] ss = s.Split(new char[] { '.' });
                    if (ss.Length != 4)
                        throw new Exception("incorrect format");
                    string addrString = null, maskString = null;
                    for (uint ind = 0; ind < 4; ind++)
                    {
                        bool wildcard = ss[ind].Equals("x");
                        addrString = ((addrString != null) ? (addrString + ".") : "") + (wildcard ? "0" : ss[ind]);
                        maskString = ((maskString != null) ? (maskString + ".") : "") + (wildcard ? "0" : "255");
                    }

                    IPAddress mask = IPAddress.Parse(maskString);
                    out_mask = mask;
                    out_representative = representativeOf(mask, IPAddress.Parse(addrString));
                }
                else
                {
                    string[] ss = s.Split(new char[] { '/' });
                    int bits = Convert.ToInt32(ss[1]);
                    byte[] mask = new byte[4];
                    for (int ind = 0; ind < bits; ind++)
                        mask[ind / 8] |= (byte)(1 << (7 - (ind % 8)));
                    out_mask = IPAddress.Parse(mask[0].ToString() + "." + mask[1].ToString() + "." + mask[2].ToString() + "." + mask[3].ToString());
                    out_representative = representativeOf(out_mask, IPAddress.Parse(ss[0]));
                }
            }
            else
            {
                string[] ss = s.Split(new char[] { ':' });
                IPAddress mask = IPAddress.Parse(ss[1]);
                out_mask = mask;
                out_representative = representativeOf(mask, IPAddress.Parse(ss[0]));
            }
        }

        public static readonly Subnet Any = new Subnet("x.x.x.x");

		public static bool OnSubnets(IPAddress address, System.Collections.Generic.IEnumerable<Subnet> subnets)
		{
			if (subnets == null)
				return true;

			foreach (Subnet subnet in subnets)
			{
				if (subnet.contains(address))
					return true;
			}

			return false;
		}

		public static System.Collections.Generic.IEnumerable<Subnet> Collection(
			System.Collections.Generic.IEnumerable<string> subnetStrings)
		{
			foreach (string subnetString in subnetStrings)
				yield return new Subnet(subnetString);
		}

		public Subnet(string s)
		{
            _SetFromString(s, out this.mask, out this.representative);
		}

		public Subnet(IPAddress address, IPAddress mask)
		{
			this.mask = mask;
			this.representative = representativeOf(mask, address);
		}

		private static byte[] representativeOf(IPAddress mask, IPAddress address)
		{
			byte[] addrBytes = address.GetAddressBytes();
			byte[] maskBytes = mask.GetAddressBytes();
			if (addrBytes.Length != maskBytes.Length)
				throw new Exception("address and mask do not match");
			byte[] rep = new byte[addrBytes.Length];

			for (uint ind = 0; ind < addrBytes.Length; ind++)
				rep[ind] = (byte) (addrBytes[ind] & maskBytes[ind]);			

			return rep;
		}

		public bool contains(IPAddress address)
		{
			return this.Equals(new Subnet(address, this.mask));
		}

		private static System.Random random = new System.Random();
		[System.Xml.Serialization.XmlIgnore]
		public IPAddress RandomAddress
		{
			get
			{
				byte[] maskBytes = mask.GetAddressBytes();
				byte[] address = new byte[representative.Length];
				random.NextBytes(address);

				for (uint ind = 0; ind < representative.Length; ind++)
					address[ind] = (byte) ((address[ind] & (~ maskBytes[ind])) | representative[ind]);

				string s = null;
				for (uint ind = 0; ind < address.Length; ind++)
					s = ((s != null) ? (s + ".") : "") + address[ind].ToString();
				return IPAddress.Parse(s);
			}
		}

        public System.Collections.Generic.IEnumerable<IPAddress> Addresses
        {
            get { return new _Addresses(this); }
        }

        #region _Addresses

        private class _Addresses : System.Collections.Generic.IEnumerable<IPAddress>
        {
            public _Addresses(Subnet subnet)
            {
                mask = subnet.mask.GetAddressBytes();
                representative = subnet.representative;
                temp = new byte[representative.Length];
                for (uint ind = 0; ind < representative.Length; ind++)
                    temp[ind] = 0;

                broadcast = new byte[representative.Length];
                for (int ind = 0; ind < representative.Length; ind++)
                    broadcast[ind] = (byte)((~mask[ind]) | representative[ind]);
            }

            private byte[] representative, mask, temp, broadcast;

            #region IEnumerable<IPAddress> Members

            System.Collections.Generic.IEnumerator<IPAddress> System.Collections.Generic.IEnumerable<IPAddress>.GetEnumerator()
            {
                while (true)
                {
                    bool carry = true;
                    for (int position = temp.Length - 1; carry && position >= 0; position--)
                    {
                        if (carry = (temp[position] == 255))
                            temp[position] = 0;
                        else
                            temp[position]++;
                    }

                    if (carry)
                        break;

                    byte[] address = new byte[representative.Length];
                    bool isbroadcast = true;
                    for (int ind = 0; ind < representative.Length; ind++)
                    {
                        address[ind] = (byte)((temp[ind] & (~mask[ind])) | representative[ind]);
                        if (address[ind] != broadcast[ind])
                            isbroadcast = false;
                    }

                    if (isbroadcast)
                        break;
                    else
                        yield return new IPAddress(address);
                }

                yield break;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.Generic.IEnumerable<IPAddress>)this).GetEnumerator();
            }

            #endregion
        }

        #endregion

        [System.Xml.Serialization.XmlIgnore]
		public unsafe IPAddress this[uint address_index]
		{
			get
			{
				if (representative.Length !=4)
					throw new NotSupportedException();

				byte* addressbytes = stackalloc byte[4];
				fixed (byte* representative_ptr = representative)
				{
					addressbytes[0] = representative_ptr[3];
					addressbytes[1] = representative_ptr[2];
					addressbytes[2] = representative_ptr[1];
					addressbytes[3] = representative_ptr[0];

					*((uint*) addressbytes) += address_index;
				}

				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				for (int ind = 3; ind >= 0; ind--)
				{
					sb.Append((*(addressbytes + ind)).ToString());
					if (ind > 0)
						sb.Append(".");
				}				
				return IPAddress.Parse(sb.ToString());
			}
		}

		private byte[] representative;
		private IPAddress mask;

		[System.Xml.Serialization.XmlAttribute("AsString")]
		public string AsString
		{
			get
			{
				return this.ToString();
			}

			set
			{
                _SetFromString(value, out this.mask, out this.representative);

/*
				string s = value;

				int colon = s.IndexOf(':');
				if (colon < 0)
				{
					string[] ss = s.Split(new char[] { '.' });
					if (ss.Length != 4)
						throw new Exception("incorrect format");
					string addrString = null, maskString = null;
					for (uint ind = 0; ind < 4; ind++)
					{
						bool wildcard = ss[ind].Equals("x");
						addrString = ((addrString != null) ? (addrString + ".") : "") + (wildcard ? "0" : ss[ind]);
						maskString = ((maskString != null) ? (maskString + ".") : "") + (wildcard ? "0" : "255");
					}

					IPAddress mask = IPAddress.Parse(maskString);
					this.mask = mask;
					this.representative = representativeOf(mask, IPAddress.Parse(addrString));
				}
				else
				{
					string[] ss = s.Split(new char[] { ':' });
					IPAddress mask = IPAddress.Parse(ss[1]);
					this.mask = mask;
					this.representative = representativeOf(mask, IPAddress.Parse(ss[0]));
				}
*/

/*
				// this is repetitive code, but the stupid C# compiler doesn't make it easier for me

				string[] ss = value.Split(new char[] {':'});
				IPAddress address = IPAddress.Parse(ss[0]);
				this.mask = IPAddress.Parse(ss[1]);
				byte[] addrBytes = address.GetAddressBytes();
				byte[] maskBytes = mask.GetAddressBytes();
				if (addrBytes.Length != maskBytes.Length)
					throw new Exception("address and mask do not match");
				representative = new byte[addrBytes.Length];

				for (uint ind = 0; ind < addrBytes.Length; ind++)
					representative[ind] = (byte) (addrBytes[ind] & maskBytes[ind]);			
*/ 
			}
		}

		

		public override int GetHashCode()
		{
			return representative.GetHashCode() ^ mask.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if ((obj is Subnet) && representative.Length.Equals(((Subnet) obj).representative.Length) && mask.Equals(((Subnet) obj).mask))
			{
				for (uint ind = 0; ind < representative.Length; ind++)
				{
					if (representative[ind] != ((Subnet) obj).representative[ind])
						return false;
				}

				return true;
			}
			else
				return false;
		}

		public override string ToString()
		{
			string representativeString = null;
			for (uint ind = 0; ind < representative.Length; ind++)
				representativeString = ((representativeString != null) ? (representativeString + ".") : "") + representative[ind].ToString();
			return representativeString + ":" + mask.ToString();
		}
	}
}
