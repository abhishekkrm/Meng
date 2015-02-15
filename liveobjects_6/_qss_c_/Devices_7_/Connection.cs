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

namespace QS._qss_c_.Devices_7_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Connection : QS.Fx.Inspection.Inspectable,
        IConnection, IDisposable, Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, IListener>
    {
        public Connection(IPAddress interfaceAddress, int mtu, QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Clock.IClock clock)
        {
            this.clock = clock;
            this.interfaceAddress = interfaceAddress;
            this.mtu = mtu;
            this.eventLogger = eventLogger;
        }

        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private int mtu;
        private IPAddress interfaceAddress;
        [QS._core_c_.Diagnostics.ComponentCollection("Listeners")]
        private IDictionary<QS.Fx.Network.NetworkAddress, Listener> listeners = new Dictionary<QS.Fx.Network.NetworkAddress, Listener>();

        #region IConnection Members

        System.Net.IPAddress IConnection.Address
        {
            get { return interfaceAddress; }
        }

        Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, IListener> IConnection.Listeners
        {
            get { return this; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            foreach (IDisposable element in listeners.Values)
            {
                try
                {
                    element.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region ICollectionOf<NetworkAddress,IListener> Members

        IListener QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, IListener>.this[QS.Fx.Network.NetworkAddress address]
        {
            get 
            {
                lock (this)
                {
                    Listener listener;
                    if (listeners.TryGetValue(address, out listener))
                        return listener;
                    else
                    {
                        listener = new Listener(interfaceAddress, address, mtu, eventLogger, clock);
                        listeners.Add(address, listener);
                        return listener;
                    }
                }
            }
        }

        #endregion
    }
}
