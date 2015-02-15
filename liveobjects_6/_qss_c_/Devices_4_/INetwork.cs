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

namespace QS._qss_c_.Devices_4_
{
    /// <summary>
    /// Interface representing a coillection of connections of a single host to the network. 
    /// </summary>
    public interface INetwork : System.Collections.Generic.IEnumerable<INetworkConnection>
    {
        /// <summary>
        /// Return the network connection associated with the given local IP address.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        INetworkConnection this[IPAddress ipAddress]
        {
            get;
        }

        /// <summary>
        /// Find all network connections connected to the given subnet.
        /// </summary>
        /// <param name="subnetAddress">The address of the requested subnet.</param>
        /// <returns></returns>
        IEnumerable<INetworkConnection> Resolve(Base1_.Subnet subnetAddress);

        /// <summary>
        /// Find all network connections that can be used to send to the given address.
        /// </summary>
        /// <param name="destinationAddress">The IP address of the destination.</param>
        /// <returns></returns>
        IEnumerable<INetworkConnection> Resolve(IPAddress destinationAddress);

        /// <summary>
        /// Release all resources associated with all network connections.
        /// </summary>
        void ReleaseResources();
    }
}
