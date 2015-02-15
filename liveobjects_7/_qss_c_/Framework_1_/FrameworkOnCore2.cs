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
/*
    [QS.TMS.Inspection.Inspectable]
    public class FrameworkOnCore2 : QS.TMS.Inspection.Inspectable, QS.TMS.Runtime.IControlledApp
    {
        public FrameworkOnCore2(
             QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Network.NetworkAddress gmsAddress, QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger) 
        {
            this.localAddress = localAddress;
            this.gmsAddress = gmsAddress;
            this.logger = logger;
            this.eventLogger = eventLogger;

            // Creating the very basic services

            core = new QS.CMS.Core.Core("C:\\.QuickSilver");
            core.OnError += 
                new QS.CMS.Core.ErrorCallback(
                    delegate(string s) 
                    { 
                        this.logger.Log(this.core, s); 
                    });

            alarmClock = core;
            clock = core;
            demultiplexer = new QS.CMS.Base3.Demultiplexer(logger, eventLogger);
            root = new QS.CMS.Base8.Root(core.StatisticsController, logger, eventLogger, core, localAddress, demultiplexer);

            // Creating the higher-level services

            failureDetector = new QS.CMS.FailureDetection.DummyFD();
            reconnectingReliableSender = 
                new QS.CMS.Senders6.ReliableSender(localAddress, logger, demultiplexer, alarmClock, clock, root, failureDetector);

            if (gmsAddress.Equals(localAddress.Address))
            {
                logger.Log(this, "This node is hosting the GMS.");
                centralizedGMS = new QS.CMS.Membership3.Server.CentralizedGMS(
                    logger, eventLogger, demultiplexer, alarmClock, TimeSpan.FromSeconds(2), reconnectingReliableSender);
            }

            membershipClient = new QS.CMS.Membership3.Client.Client(
                ((Base3.ISenderCollection<Base3.IReliableSerializableSender>) reconnectingReliableSender)[gmsAddress], failureDetector,
                logger, eventLogger, demultiplexer);

            Embeddings2.GroupServices.Connect(membershipClient, logger);
            groupServices = Embeddings2.GroupServices.Shared;
        }

        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Network.NetworkAddress gmsAddress;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;

        private Core.Core core;

        private Base.IAlarmClock alarmClock;
        private Base2.IClock clock;
        private Base3.IDemultiplexer demultiplexer;
        private Base8.Root root;

        private QS.CMS.FailureDetection.DummyFD failureDetector;
        private QS.CMS.Senders6.ReliableSender reconnectingReliableSender;
        private Membership3.Server.CentralizedGMS centralizedGMS;
        private Membership3.Client.Client membershipClient;
        private Embeddings2.GroupServices groupServices;

        public Core.Core Core
        {
            get { return core; }
        }

        public Embeddings2.GroupServices GroupServices
        {
            get { return groupServices; }
        }

        #region IControlledApp Members

        bool QS.TMS.Runtime.IControlledApp.Running
        {
            get { return core.Running; }
        }

        void QS.TMS.Runtime.IControlledApp.Start()
        {
            core.Start();
        }

        void QS.TMS.Runtime.IControlledApp.Stop()
        {
            core.Stop();
        }

        #endregion
    }
*/
}
