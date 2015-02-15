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

using System.Net;

namespace QS._qss_c_.Membership2.Allocation
{
    public interface IAddressPool
    {
        QS.Fx.Network.NetworkAddress AllocateAddress
        {
            get;
        }

        void RecycleAddress(QS.Fx.Network.NetworkAddress address);
    }

    public class AddressPool : IAddressPool
    {
        public readonly static Base1_.Subnet DefaultSubnet = new Base1_.Subnet("224.0.66.0:255.255.255.0");
		public readonly static Components_1_.Range<uint> DefaultPortRange = 
            new QS._qss_c_.Components_1_.Range<uint>(15000, 16000);

        private const int defaultMaximumNumberOfAttempts = 1000;

        public AddressPool() : this(DefaultSubnet, DefaultPortRange)
        {
        }

        public AddressPool(Base1_.Subnet allocationSubnet, Components_1_.Range<uint> allocationPortRange)
        {
            this.allocationSubnet = allocationSubnet;
            this.allocationPortRange = allocationPortRange;

            allocatedPorts = new List<uint>();
            allocatedIPAddresses = new List<IPAddress>();
        }

        private static System.Random random = new System.Random();

        private Base1_.Subnet allocationSubnet;
        private Components_1_.Range<uint> allocationPortRange;

        private System.Collections.Generic.List<uint> allocatedPorts;
        private System.Collections.Generic.List<IPAddress> allocatedIPAddresses;

        #region IAddressPool Members

        public QS.Fx.Network.NetworkAddress AllocateAddress
        {
            get 
            {
                int ind;

                uint portno = 0;

                for (ind = defaultMaximumNumberOfAttempts; ind > 0; ind--)
                {
                    portno = (uint) random.Next((int)allocationPortRange.Minimum, (int)allocationPortRange.Maximum);
                    if (!allocatedPorts.Contains(portno))
                        break;
                }

                if (ind > 0)
                {
                    IPAddress ipAddress = IPAddress.None;

                    for (ind = defaultMaximumNumberOfAttempts; ind > 0; ind--)
                    {
                        ipAddress = allocationSubnet.RandomAddress;
                        byte firstbyte = (ipAddress.GetAddressBytes())[3];
                        if (firstbyte > 0 && firstbyte < 255 && !allocatedIPAddresses.Contains(ipAddress))
                            break;
                    }

                    if (ind > 0)
                    {
                        allocatedPorts.Add(portno);
                        allocatedIPAddresses.Add(ipAddress);

                        return new QS.Fx.Network.NetworkAddress(ipAddress, (int) portno);
                    }
                    else
                        throw new Exception("Could not allocate an IP address.");
                }
                else
                    throw new Exception("Could not allocate a port number.");
            }
        }

        public void RecycleAddress(QS.Fx.Network.NetworkAddress address)
        {
            allocatedIPAddresses.Add(address.HostIPAddress);
            allocatedPorts.Add((uint)address.PortNumber);
        }

        #endregion
    }
}
