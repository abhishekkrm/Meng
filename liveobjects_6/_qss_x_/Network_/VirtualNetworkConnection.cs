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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Network_
{
    public sealed class VirtualNetworkConnection : QS.Fx.Inspection.Inspectable, QS.Fx.Network.INetworkConnection
    {
        public VirtualNetworkConnection(string name, QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock,
            QS.Fx.Scheduling.IScheduler scheduler, QS._core_c_.Statistics.IStatisticsController statisticsController, 
            IVirtualNetwork network)
        {
            this.name = name;
            this.logger = logger;
            this.statisticsController = statisticsController;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.network = network;
            this.scheduler = scheduler;

            networkInterface = new VirtualNetworkInterface(name, clock, alarmClock, scheduler, statisticsController, network);
        }

        [QS.Fx.Base.Inspectable]
        private string name;

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Scheduling.IScheduler scheduler;
        private IVirtualNetwork network;
        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;

        [QS.Fx.Base.Inspectable]
        private VirtualNetworkInterface networkInterface;

        #region INetworkConnection Members

        QS.Fx.Network.INetworkInterface[] QS.Fx.Network.INetworkConnection.Interfaces
        {
            get { return new QS.Fx.Network.INetworkInterface[] { networkInterface }; }
        }

        QS.Fx.Network.INetworkInterface QS.Fx.Network.INetworkConnection.GetInterface(System.Net.IPAddress interfaceAddress)
        {
            lock (this)
            {
                if (interfaceAddress.Equals(((QS.Fx.Network.INetworkInterface)networkInterface).InterfaceAddress))
                    return networkInterface;
                else
                    throw new Exception("Not interface exists for address " + interfaceAddress.ToString() + ".");
            }
        }

        string QS.Fx.Network.INetworkConnection.GetHostName()
        {
            return name;
        }

        System.Net.IPHostEntry QS.Fx.Network.INetworkConnection.GetHostEntry(string hostname)
        {
            return network.GetHostEntry(hostname);
        }

        System.Net.IPHostEntry QS.Fx.Network.INetworkConnection.GetHostEntry(System.Net.IPAddress address)
        {
            return network.GetHostEntry(address);
        }

        #endregion

        public void Reset()
        {
// #if DEBUG_LogGenerously
//            logger.Log("Resetting network connection.");
// #endif

            networkInterface.Reset();
        }
    }
}
