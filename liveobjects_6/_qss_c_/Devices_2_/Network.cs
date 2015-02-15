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

namespace QS._qss_c_.Devices_2_
{
	/// <summary>
	/// Summary description for Network.
	/// </summary>
	public class Network
	{
        static Network()
        {
            localAddresses = (Dns.GetHostAddresses(Dns.GetHostName()));
        }

        public static IPAddress[] LocalAddresses
		{
			get
			{
				return localAddresses;
			}
		}

        private static IPAddress[] localAddresses;

        public static IPAddress AnyAddressOn(Base1_.Subnet subnet, QS._qss_c_.Platform_.IPlatform platform)
		{
			return AnyAddressOn(subnet, platform.NICs);
		}

		public static IPAddress AnyAddressOn(Base1_.Subnet subnet)
		{
			return AnyAddressOn(subnet, localAddresses);
		}

		public static IPAddress AnyAddressOn(System.Collections.Generic.IEnumerable<Base1_.Subnet> subnetCollection)
		{
			return AnyAddressOn(subnetCollection, localAddresses);
		}

		public static IPAddress AnyAddressOn(Base1_.Subnet subnet, IPAddress[] candidateAddresses)
		{
			return AnyAddressOn(new QS._qss_c_.Base1_.Subnet[] { subnet }, candidateAddresses);
		}

		public static IPAddress AnyAddressOn(System.Collections.Generic.IEnumerable<Base1_.Subnet> subnetCollection, IPAddress[] candidateAddresses)
		{
			foreach (IPAddress address in candidateAddresses)
			{
				foreach (Base1_.Subnet subnet in subnetCollection)
				{
					if (subnet.contains(address))
						return address;
				}
			}

			throw new Exception("None of the available addresses is on any of the given subnets.");
		}
    }
}
