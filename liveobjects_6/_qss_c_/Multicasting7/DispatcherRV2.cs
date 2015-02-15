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

namespace QS._qss_c_.Multicasting7
{
    public class DispatcherRV2
    {
        public DispatcherRV2(
            QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Base3_.IDemultiplexer demultiplexer,
            uint transmissionChannel, Receivers4.IReceivingAgentCollection<Base3_.RVID> receivingAgentCollection,
            Membership2.Controllers.IMembershipController membershipController)
        {
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.demultiplexer = demultiplexer;
            this.receivingAgentCollection = receivingAgentCollection;
            this.membershipController = membershipController;

            demultiplexer.register(transmissionChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private Base3_.IDemultiplexer demultiplexer;
        private Receivers4.IReceivingAgentCollection<Base3_.RVID> receivingAgentCollection;
        private Membership2.Controllers.IMembershipController membershipController;

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Multicasting7.IMessageRV2 message = (Multicasting7.IMessageRV2) receivedObject;
            if (message != null)
            {
                for (int ind = 0; ind < message.SeqNos.Length; ind++)
                {
                    Base3_.RVID rvid = message.RVIDs[ind];
                    Membership2.Controllers.IRegionViewController regionVC;
                    if (membershipController.TryGetRegionView(rvid, out regionVC))
                    {
                        uint seqno = message.SeqNos[ind];
                        Receivers4.IReceivingAgent agent = receivingAgentCollection[rvid];
                        if (agent != null)
                            agent.Receive(sourceAddress, seqno, message.EncapsulatedMessage, false, false);
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
