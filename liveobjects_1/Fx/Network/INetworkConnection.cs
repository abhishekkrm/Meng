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
    /// This class represents a virtualized network stack, with one or more network interface adapters. 
    /// The stack only supports UDP communication.
    /// </summary>
    public interface INetworkConnection
    {
        /// <summary>
        /// The list of available network interface adpaters.
        /// </summary>
        INetworkInterface[] Interfaces
        {
            get;
        }

        /// <summary>
        /// Allows to find a network interface adapter with the specific IP address assigned to it.
        /// </summary>
        /// <param name="interfaceAddress">The local IP address of the network interface adapter.</param>
        /// <returns>The interface adapter with the given local address.</returns>
        INetworkInterface GetInterface(IPAddress interfaceAddress);

        /// <summary>
        /// Returns the network name of the local machine.
        /// </summary>
        /// <returns></returns>
        string GetHostName();

        /// <summary>
        /// Allows for name resolution using the host's network name.
        /// </summary>
        /// <param name="hostname">Network name of the host to resolve.</param>
        /// <returns>The structure containing the host's name and IP addresses.</returns>
        IPHostEntry GetHostEntry(string hostname);

        /// <summary>
        /// Allows for name resolution using the host's IP address.
        /// </summary>
        /// <param name="address">IP address of the host to resolve.</param>
        /// <returns>The structure containing the host's name and IP addresses.</returns>
        IPHostEntry GetHostEntry(IPAddress address);
    }
}
