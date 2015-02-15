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

namespace QS._qss_x_.Platform_
{
    public class PhysicalPlatform : QS.Fx.Inspection.Inspectable, QS.Fx.Platform.IPlatform, QS._core_c_.Diagnostics2.IModule
    {
        public PhysicalPlatform(
            QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, QS._core_c_.Core.ICore core, string fsroot)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.core = core;

            filesystem = new QS._qss_x_.Filesystem_.PhysicalFilesystem(core, fsroot);
            networkConnection = new QS._qss_x_.Network_.PhysicalNetworkConnection(core);

            core.Logger = logger;
            core.EventLogger = eventLogger;

            core.OnError += new QS._core_c_.Core.ErrorCallback(
                delegate (string message)
                {
                    logger.Log(core, message);
                });

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Logging.ILogger logger;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Logging.IEventLogger eventLogger;
        [QS.Fx.Base.Inspectable]
        private QS._core_c_.Core.ICore core;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Filesystem_.PhysicalFilesystem filesystem;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Network_.PhysicalNetworkConnection networkConnection;

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IPlatform Members

        QS.Fx.Clock.IClock QS.Fx.Platform.IPlatform.Clock
        {
            get { return core; }
        }

        QS.Fx.Clock.IAlarmClock QS.Fx.Platform.IPlatform.AlarmClock
        {
            get { return core; }
        }

        QS.Fx.Scheduling.IScheduler QS.Fx.Platform.IPlatform.Scheduler
        {
            get { return core; }
        }

        QS.Fx.Logging.ILogger QS.Fx.Platform.IPlatform.Logger
        {
            get { return logger; }
        }

        QS.Fx.Logging.IEventLogger QS.Fx.Platform.IPlatform.EventLogger
        {
            get { return eventLogger; }
        }

        QS.Fx.Filesystem.IFilesystem QS.Fx.Platform.IPlatform.Filesystem
        {
            get { return filesystem; }
        }

        QS.Fx.Network.INetworkConnection QS.Fx.Platform.IPlatform.Network
        {
            get { return networkConnection; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // core.Dispose();
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion
    }
}
