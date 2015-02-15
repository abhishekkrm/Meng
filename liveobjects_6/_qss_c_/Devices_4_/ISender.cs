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

namespace QS._qss_c_.Devices_4_
{
    /// <summary>
    /// An object capable of sending to a given address, attached to a specific netwokr interface.
    /// </summary>
    public interface ISender
        : Base4_.IAddressedSink<QS.Fx.Network.NetworkAddress, QS._qss_c_.Base4_.Asynchronous<IList<QS.Fx.Base.Block>>>
    {
        /// <summary>
        /// The network connection to which the listener is attached.
        /// </summary>
        INetworkConnection NetworkConnection
        {
            get;
        }

/*
        /// <summary>
        /// Destination address for this sender.
        /// </summary>
        QS.Fx.Network.NetworkAddress Address
        {
            get;
        }
*/
        void Send(IList<QS.Fx.Base.Block> segments);
    }
}
