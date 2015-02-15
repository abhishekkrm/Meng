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

namespace QS._qss_c_.Devices_3_
{
    public class Network : INetwork, IDisposable
    {
		public static INetworkInterface FindInterface(INetwork network, Base1_.Subnet subnet)
		{
			return FindInterface(network, new Base1_.Subnet[] { subnet });
		}

		public static INetworkInterface FindInterface(INetwork network, Base1_.Subnet[] subnets)
		{
			foreach (Base1_.Subnet subnet in subnets)
			{
				foreach (INetworkInterface networkInterface in network.NICs)
				{
					if (subnet.contains(networkInterface.Address))
						return networkInterface;
				}
			}

			return null;
		}

		public INetworkInterface OnSubnets(Base1_.Subnet[] subnets)
		{
			return FindInterface(this, subnets);
		}

		public INetworkInterface OnSubnet(Base1_.Subnet subnet)
		{
			return FindInterface(this, subnet);
		}

		public const int DefaultMTU = 20000;

        public Network(QS.Fx.Logging.ILogger logger) : this(logger, DefaultMTU)
        {
        }

        public Network(QS.Fx.Logging.ILogger logger, int maximumTransmissionUnit)
        {
            IPAddress[] localAddresses = Devices_2_.Network.LocalAddresses;
            interfaces = new NetworkInterface[localAddresses.Length];
            for (int ind = 0; ind < interfaces.Length; ind++)
                interfaces[ind] = new NetworkInterface(localAddresses[ind], logger, maximumTransmissionUnit);
        }

        private NetworkInterface[] interfaces;

        #region INetwork Members

        INetworkInterface[] INetwork.NICs
        {
            get { return interfaces; }
        }

        public INetworkInterface this[IPAddress address]
        {
            get
            {
                foreach (NetworkInterface netinf in interfaces)
                {
                    if (netinf.Address.Equals(address))
                        return netinf;
                }
                throw new Exception("No such address");
            }
        }

        #endregion

        public void ReleaseResources()
        {
            foreach (NetworkInterface networkInterface in interfaces)
                networkInterface.ReleaseResources();
        }

        #region System.IDisposable Members

        public void Dispose()
        {
            foreach (NetworkInterface networkInterface in interfaces)
                networkInterface.Dispose();
        }

        #endregion
    }
}
