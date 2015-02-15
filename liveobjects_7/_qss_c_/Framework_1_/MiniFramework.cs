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

namespace QS._qss_c_.Framework_1_
{
    public class MiniFramework : IFramework
    {
        public MiniFramework(QS._qss_c_.Base1_.Subnet subnet) : this(subnet, 0)
        {
        }

        public MiniFramework(QS._qss_c_.Base1_.Subnet subnet, int portno) : this(QS._core_c_.Base.Logger.StandardConsole, subnet, portno)
        {
        }

        private const int DefaultMTU = 1600;

        public MiniFramework(QS.Fx.Logging.ILogger logger, QS._qss_c_.Base1_.Subnet subnet, int portno)
        {
            this.logger = logger;
            demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(logger, Logging_1_.NoLogger.Logger);
            network = new QS._qss_c_.Devices_3_.Network(logger);
            communicationsDevice = new QS._qss_c_.Devices_3_.UDPCommunicationsDevice(network.OnSubnet(subnet), logger, DefaultMTU);
            rootSender = new QS._qss_c_.Base3_.RootSender(Logging_1_.NoLogger.Logger,
                logger, communicationsDevice, portno, demultiplexer, QS._core_c_.Base2.PreciseClock.Clock, false);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS._qss_c_.Base3_.Demultiplexer demultiplexer;
        private QS._qss_c_.Devices_3_.Network network;
        private QS._qss_c_.Devices_3_.UDPCommunicationsDevice communicationsDevice;
        private QS._qss_c_.Base3_.RootSender rootSender;

        #region IFramework Members

        QS._qss_c_.Base3_.IDemultiplexer IFramework.Demultiplexer
        {
            get { return demultiplexer; }
        }

        QS.Fx.Logging.ILogger IFramework.Logger
        {
            get { return logger; }
        }

        QS._qss_c_.Base3_.RootSender IFramework.RootSender
        {
            get { return rootSender; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            rootSender.Dispose();
            communicationsDevice.Dispose();
            network.Dispose();
        }

        #endregion
}
}
