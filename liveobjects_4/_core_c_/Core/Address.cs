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
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace QS._core_c_.Core
{
    public class Address : IComparable<Address>, IEquatable<Address>
    {
	    public Address(IPAddress nic, IPAddress ipAddress, int portNumber)
	    {
		    this.nic = nic;
		    this.ipAddress = ipAddress;
		    this.portNumber = portNumber;
	    }

	    public IPAddress NIC
	    {
		    get { return nic; }
		    set { this.nic = value; }
	    }

	    public IPAddress IPAddress
	    {
		    get { return ipAddress; }
		    set { ipAddress = value; }
	    }

	    public int PortNumber
	    {
		    get { return portNumber; }
		    set { portNumber = value; }
	    }

	    int IComparable<Address>.CompareTo(Address another)
	    {
		    if (another == null)
			    throw new Exception("Cannot compare with a null pointer.");

		    int result = CompareIPs(nic, another.nic);
		    if (result == 0)
		    {
			    result = CompareIPs(ipAddress, another.ipAddress);
			    if (result == 0)
				    result = portNumber.CompareTo(another.portNumber);
		    }
		    return result;
	    }

	    public override int GetHashCode()
	    {
		    return nic.GetHashCode() ^ ipAddress.GetHashCode() ^ portNumber.GetHashCode();
	    }

	    public override bool Equals(object obj)
	    {
		    Address another = obj as Address;
		    return another != null && nic.Equals(another.nic) && 
			    ipAddress.Equals(another.ipAddress) && portNumber.Equals(another.portNumber);
	    }

	    bool IEquatable<Address>.Equals(Address another) 
	    {
		    return another != null && nic.Equals(another.nic) && 
			    ipAddress.Equals(another.ipAddress) && portNumber.Equals(another.portNumber);
	    }

	    public override string ToString()
	    {
		    return nic.ToString() + ":" + ipAddress.ToString() + ":" + portNumber.ToString();
	    }

        private IPAddress nic, ipAddress;
	    private int portNumber;

	    public unsafe static int CompareIPs(IPAddress address1, IPAddress address2)
	    {
#pragma warning disable 0618
		    Int64 x = address1.Address;
		    Int64 y = address2.Address;
#pragma warning restore 0618

		    int result = ((int)(((byte*) &x)[0])) - ((int)(((byte*) &y)[0]));
		    if (result == 0)
		    {
			    result = ((int)(((byte*) &x)[1])) - ((int)(((byte*) &y)[1]));
			    if (result == 0)
			    {
				    result = ((int)(((byte*) &x)[2])) - ((int)(((byte*) &y)[2]));
				    if (result == 0)
					    result = ((int)(((byte*) &x)[3])) - ((int)(((byte*) &y)[3]));
			    }
		    }
		    return result;
	    }

        public bool IsMulticast
        {
            get
            {
                byte first_byte = (ipAddress.GetAddressBytes())[0];
                return (first_byte > 223 && first_byte < 240);
            }
        }
    };
}
