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

namespace QS._qss_d_.Agent_2_
{
    public class Agent2 : IAgent
    {
        public Agent2(IList<QS.Fx.Network.NetworkAddress> addresses, ConnectCallback connectCallback, DisconnectCallback disconnectCallback, object target)
        {
            this.connectCallback = connectCallback;
            this.disconnectCallback = disconnectCallback;

            this.target = target;

            foreach (QS.Fx.Network.NetworkAddress address in addresses)
            {
                QS._qss_c_.Connections_.TCPListener listener = new QS._qss_c_.Connections_.TCPListener(logger, address,
                    new QS._qss_c_.Connections_.CreateCallback(this.CreateCallback));
                listeners.Add(address, listener);
            }
        }

        private QS.Fx.Logging.ILogger logger = new QS._core_c_.Base.Logger(QS._core_c_.Core.Clock.SharedClock, true);
        private IDictionary<QS.Fx.Network.NetworkAddress, QS._qss_c_.Connections_.TCPListener> listeners =
            new Dictionary<QS.Fx.Network.NetworkAddress, QS._qss_c_.Connections_.TCPListener>();
        private IList<ISessionController> sessionControllers = new List<ISessionController>();
        private object target;
        private ConnectCallback connectCallback;
        private DisconnectCallback disconnectCallback;

        private QS._qss_c_.Connections_.IAsynchronousObject CreateCallback(
            QS.Fx.Network.NetworkAddress address, QS._qss_c_.Connections_.IAsynchronousRef peer)
        {
            QS._core_c_.Base.Logger logger = new QS._core_c_.Base.Logger(QS._core_c_.Core.Clock.SharedClock, true);
            SessionController2 sessionController = new SessionController2(address, logger);
            sessionControllers.Add(sessionController);
            connectCallback(sessionController);
            return new QS._qss_c_.Connections_.ServiceObject(logger, target);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            foreach (ISessionController sessionController in sessionControllers)
            {
                disconnectCallback(sessionController);
                ((IDisposable)sessionController).Dispose();
            }
            sessionControllers.Clear();

            foreach (QS._qss_c_.Connections_.TCPListener listener in listeners.Values)
            {
                ((IDisposable) listener).Dispose();
            }
            listeners.Clear();
        }

        #endregion
    }
}
