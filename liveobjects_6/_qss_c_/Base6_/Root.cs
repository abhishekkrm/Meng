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

namespace QS._qss_c_.Base6_
{
    // ICollectionOf<QS.Fx.Network.NetworkAddress, ISink<IAsynchronous<Base3.Message>>>

    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Root : QS.Fx.Inspection.Inspectable, ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>
    {
        public Root(QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IClock clock, Devices_6_.INetwork network)
        {
            this.localAddress = localAddress;
            this.clock = clock;
            this.network = network;
        }

        public Root(QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IClock clock, Devices_4_.INetwork network)
        {
            this.localAddress = localAddress;
            this.clock = clock;
            this.oldNetwork = network;
        }

        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Clock.IClock clock;
        private Devices_6_.INetwork network;
        private Devices_4_.INetwork oldNetwork;
        [QS._core_c_.Diagnostics.ComponentCollection("Senders")]
        private IDictionary<QS.Fx.Network.NetworkAddress, SerializingSender> senders = 
            new Dictionary<QS.Fx.Network.NetworkAddress, SerializingSender>();

        #region ICollectionOf<NetworkAddress,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> 
            ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[
                QS.Fx.Network.NetworkAddress address]
        {
            get 
            {
                lock (this)
                {
                    if (senders.ContainsKey(address))
                        return senders[address];
                    else
                    {
                        QS._core_c_.Base6.ISink<Base6_.Asynchronous<Devices_6_.Block>> sink;
                        if (network != null)
                            sink = network.Connections[localAddress.Address.HostIPAddress][address];
                        else
                            sink = new Devices_6_.Adapter2AS(oldNetwork[localAddress.Address.HostIPAddress][address]);

                        SerializingSender sender = new SerializingSender(localAddress, clock, sink);
                        senders[address] = sender;
                        return sender;
                    }
                }
            }
        }

        #endregion
    }
}
