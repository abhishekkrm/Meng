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
    /// Interface representing a single connection to the network, bound to a specific local IP address.
    /// </summary>
    public interface INetworkConnection
    {
        /// <summary>
        /// Local IP address.
        /// </summary>
        IPAddress Address
        {
            get;
        }

        /// <summary>
        /// The object representing all network connections.
        /// </summary>
        INetwork Network
        {
            get;
        }

        /// <summary>
        /// Start receiving data on a given address.
        /// </summary>
        /// <param name="receivingAddress">Address at which the receiving socket should be installed.</param>
        /// <param name="receiveCallback">Callback to invoke when data arrives.</param>
        /// <returns>An object representing a receiving socket and the module listening on it.</returns>
        IListener Register(QS.Fx.Network.NetworkAddress receivingAddress, ReceiveCallback receiveCallback);

        /// <summary>
        /// Stop listening on a given address.
        /// </summary>
        /// <param name="listener"></param>
        void Unregister(IListener listener);

        /// <summary>
        /// Return an object representing a sending socket for a given destination address.
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <returns></returns>
        ISender this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get;
        }

        /// <summary>
        /// Unregister all listeners, invalidate all senders, and release all resources associated with this interface.
        /// </summary>
        void ReleaseResources();
    }
}
