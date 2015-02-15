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

namespace QS._qss_c_.Platform_
{
    public class PlatformOnCore : IPlatform
    {
        public PlatformOnCore(IPlatform platform) : this(platform.Logger, platform.EventLogger)
        {
            if (!(platform is QS._qss_c_.Platform_.PhysicalPlatform))
                throw new Exception("The underlying platform is not a physical platform. Perhaps running in a simulator?");
        }

        public PlatformOnCore(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;

            core.OnError += new QS._core_c_.Core.ErrorCallback(this.CoreErrorCallback);
        }

        private void CoreErrorCallback(string message)
        {
            logger.Log(core, message);
        }

        private QS._core_c_.Core.Core core = new QS._core_c_.Core.Core("C:\\.QuickSilver");

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;

        #region IPlatform Members

        QS.Fx.Logging.ILogger IPlatform.Logger
        {
            get { return logger; }
        }

        QS.Fx.Logging.IEventLogger IPlatform.EventLogger
        {
            get { return eventLogger; }
        }

        QS.Fx.Clock.IAlarmClock IPlatform.AlarmClock
        {
            get { return core; }
        }

        QS.Fx.Clock.IClock IPlatform.Clock
        {
            get { return core; }
        }

        void IPlatform.ReleaseResources()
        {
        }

        QS._core_c_.Core.ICore IPlatform.Core
        {
            get { return core; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            core.Dispose();
        }

        #endregion

        #region ICommunicationSubsystem Members

        System.Net.IPAddress[] QS._qss_c_.Virtualization_.ICommunicationSubsystem.NICs
        {
            get { return Devices_2_.Network.LocalAddresses; }
        }

        QS._qss_c_.Devices_2_.ICommunicationsDevice QS._qss_c_.Virtualization_.ICommunicationSubsystem.UDPDevice
        {
            get { throw new NotSupportedException(); }
        }

        QS._qss_c_.Devices_3_.INetwork QS._qss_c_.Virtualization_.ICommunicationSubsystem.Network
        {
            get { throw new NotSupportedException(); }
        }

        QS._qss_c_.Devices_4_.INetwork QS._qss_c_.Virtualization_.ICommunicationSubsystem.NetworkConnections
        {
            get { throw new NotSupportedException(); }
        }

        QS._qss_c_.Devices_7_.IConnections QS._qss_c_.Virtualization_.ICommunicationSubsystem.Connections7
        {
            get { throw new NotSupportedException(); }
        }

        #endregion

        #region INetwork Members

        QS._qss_c_.Base6_.ICollectionOf<System.Net.IPAddress, QS._qss_c_.Devices_6_.INetworkConnection> QS._qss_c_.Devices_6_.INetwork.Connections
        {
            get { throw new NotSupportedException(); }
        }

        QS._qss_c_.Devices_6_.ReceiveCallback QS._qss_c_.Devices_6_.INetwork.ReceiveCallback
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}
