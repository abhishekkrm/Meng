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

namespace QS._qss_x_.Network_
{
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public sealed class PhysicalNetworkConnection : QS.Fx.Network.INetworkConnection
    {
        public PhysicalNetworkConnection(QS._core_c_.Core.ICore core)
        {
            this.core = core;
            foreach (System.Net.IPAddress ipAddress in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    networkInterfaces.Add(ipAddress, new PhysicalNetworkInterface(core, ipAddress));
            }
        }

        private QS._core_c_.Core.ICore core;
        private IDictionary<System.Net.IPAddress, PhysicalNetworkInterface> networkInterfaces =
            new Dictionary<System.Net.IPAddress, PhysicalNetworkInterface>();

        #region INetwork Members

        QS.Fx.Network.INetworkInterface[] QS.Fx.Network.INetworkConnection.Interfaces
        {
            get 
            {
                lock (this)
                {
                    List<QS.Fx.Network.INetworkInterface> result = new List<QS.Fx.Network.INetworkInterface>();
                    foreach (QS.Fx.Network.INetworkInterface networkInterface in networkInterfaces.Values)
                        result.Add(networkInterface);
                    return result.ToArray();
                }
            }
        }

        QS.Fx.Network.INetworkInterface QS.Fx.Network.INetworkConnection.GetInterface(System.Net.IPAddress interfaceAddress)
        {
            lock (this)
            {
                PhysicalNetworkInterface networkInterface;
                if (!networkInterfaces.TryGetValue(interfaceAddress, out networkInterface))
                    throw new Exception("Cannot get interface, none of the available interfaces has address " + interfaceAddress.ToString());
                return networkInterface;
            }
        }

        string QS.Fx.Network.INetworkConnection.GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }

        System.Net.IPHostEntry QS.Fx.Network.INetworkConnection.GetHostEntry(string hostname)
        {
            return System.Net.Dns.GetHostEntry(hostname);
        }

        System.Net.IPHostEntry QS.Fx.Network.INetworkConnection.GetHostEntry(System.Net.IPAddress address)
        {
            return System.Net.Dns.GetHostEntry(address);
        }

        #endregion
    }
}
