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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

#endregion

namespace QS._qss_e_.Experiments_
{
    // [TMS.Base.Arguments("-nnodes:2 -time:1000 -mttf:500 -downtime:5 -ngroups:1")]

    [QS._qss_e_.Base_1_.Arguments("-nnodes:20 -time:10000 -stayon -ngroups:1")]
    public class Experiment_202 : Experiment_200
    {
        public Experiment_202()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        private const int NUMBER_OF_MESSAGES = 10;
        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            double time = Convert.ToDouble((string)arguments["time"]);

/*
            environment.AlarmClock.scheduleAnAlarm(time / 2, new QS.CMS.Base.AlarmCallback(delegate(QS.CMS.Base.IAlarmRef alarmRef)
            {
                NodeController(1).Crash(true);
            }), null);

            for (int ind = 0; ind < NUMBER_OF_MESSAGES; ind++)
            {
                Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { Node(1).NICs[0].ToString() });
                sleeper.sleep(time / NUMBER_OF_MESSAGES);
            }
*/

            sleeper.sleep(time);
        }

        [QS.Fx.Base.Inspectable]
        protected new class Application : Experiment_200.Application
        {
            private const uint myloid = 1000;

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                framework = new QS._qss_c_.Framework_1_.SimpleFramework(incarnation, platform, localAddress, coordinatorAddress);
                framework.Demultiplexer.register(myloid, new QS._qss_c_.Base3_.ReceiveCallback(ReceiveCallback));
                senderCollection = framework.ReconnectingReliableSender;

                framework.FailureDetector.OnChange += new QS._qss_c_.FailureDetection_.ChangeCallback(FailureDetector_OnChange);

                groupIDs = new QS._qss_c_.Base3_.GroupID[Convert.ToInt32(args["ngroups"])];
                for (int ind = 0; ind < groupIDs.Length; ind++)
                    groupIDs[ind] = new QS._qss_c_.Base3_.GroupID((uint)(1000 + ind));

                framework.MembershipAgent.ChangeMembership(
                    new List<QS._qss_c_.Base3_.GroupID>(groupIDs), new List<QS._qss_c_.Base3_.GroupID>());
            }

            private void FailureDetector_OnChange(IEnumerable<QS._qss_c_.FailureDetection_.Change> changes)
            {
                logger.Log(this, "__FailureDetector_OnChange: " + 
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._qss_c_.FailureDetection_.Change>(changes, ","));
            }

            private QS._qss_c_.Framework_1_.SimpleFramework framework;
            private QS._qss_c_.Base3_.GroupID[] groupIDs;
            private QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.IReliableSerializableSender> senderCollection;

            public void Send(string destinationAddressString)
            {
                QS.Fx.Network.NetworkAddress destinationAddress =
                    new QS.Fx.Network.NetworkAddress(IPAddress.Parse(destinationAddressString), localAddress.PortNumber);
                senderCollection[destinationAddress].BeginSend(myloid, new QS._core_c_.Base2.StringWrapper("Kaziun."), null, null);
            }

            private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID,
                QS.Fx.Serialization.ISerializable receivedObject)
            {
                logger.Log(this, QS._core_c_.Helpers.ToString.ReceivedObject(sourceIID, receivedObject));
                return null;
            }

            public override void TerminateApplication(bool smoothly)
            {
                framework.Dispose();
                platform.ReleaseResources();
            }

            public override void Dispose()
            {
            }
        }
    }
}
