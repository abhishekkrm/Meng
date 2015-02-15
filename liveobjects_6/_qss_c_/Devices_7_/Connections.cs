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
    public class Connections : QS.Fx.Inspection.Inspectable, 
        IConnections, QS._qss_c_.Base6_.ICollectionOf<System.Net.IPAddress, IConnection>
    {
        private const int DefaultMTU = 65000;

        public Connections(QS.Fx.Logging.IEventLogger eventLogger) : this(eventLogger, DefaultMTU)
        {
        }

        public Connections(QS.Fx.Logging.IEventLogger eventLogger, int mtu)
        {
            foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
                connections.Add(address, new Connection(address, mtu, eventLogger, QS._core_c_.Base2.PreciseClock.Clock));
        }

        [QS._core_c_.Diagnostics.ComponentCollection("Connections")]
        private IDictionary<IPAddress, Connection> connections = new Dictionary<IPAddress, Connection>();

        #region INetwork Members

        QS._qss_c_.Base6_.ICollectionOf<System.Net.IPAddress, IConnection> IConnections.Connections
        {
            get { return this; }
        }

        #endregion

        #region ICollectionOf<IPAddress,IConnection> Members

        IConnection QS._qss_c_.Base6_.ICollectionOf<System.Net.IPAddress, IConnection>.this[System.Net.IPAddress address]
        {
            get { return connections[address]; }
        }

        #endregion

        #region IEnumerable<IConnection> Members

        IEnumerator<IConnection> IEnumerable<IConnection>.GetEnumerator()
        {
            foreach (Connection connection in connections.Values)
                yield return connection;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IConnection>)this).GetEnumerator();
        }

        #endregion
    }
}
