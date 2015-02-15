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

namespace QS.Fx.Network
{
    /// <summary>
    /// This class represents a single network interface adapter.
    /// </summary>
    public interface INetworkInterface
    {
        /// <summary>
        /// The local IP address that the adapter is associated with.
        /// </summary>
        IPAddress InterfaceAddress
        {
            get;
        }

        /// <summary>
        /// Creates a receive socket associated with the given network interface adapter, for the given IP address to listen at.
        /// </summary>
        /// <param name="address">The address to listen at. It can be an IP multicast address.</param>
        /// <param name="callback">The callback to invoke when a UDP packet arrives.</param>
        /// <param name="context">The context object to pass to the callback that can be used to associate additional 
        /// state with the receive socket.</param>
        /// <param name="parameters">Additional configuration parameters for the receive socket. The list of parameters
        /// depends on whether the device is physical or virtual.</param>
        /// <returns>The object that represents the receive socket.</returns>
        QS.Fx.Network.IListener Listen(QS.Fx.Network.NetworkAddress address,  QS.Fx.Network.ReceiveCallback callback, object context,
            params QS.Fx.Base.IParameter[] parameters);

        /// <summary>
        /// Creates a send socket associated with the given network interface adapter, for the given IP address to send to. 
        /// </summary>
        /// <param name="address">The address to send to. It can be an IP multicast address.</param>
        /// <param name="parameters">Additional configuration parameters for the receive socket. The list of parameters
        /// depends on whether the device is physical or virtual.</param>
        /// <returns>The object that represents the send socket.</returns>
        QS.Fx.Network.ISender GetSender(QS.Fx.Network.NetworkAddress address, params QS.Fx.Base.IParameter[] parameters);
    }
}
