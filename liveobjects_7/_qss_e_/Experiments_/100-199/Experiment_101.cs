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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_e_.Experiments_
{
/*
    [TMS.Base.Arguments("-nnodes:20 -timeout:1.0")]
    public class Experiment_101 : Experiment_100
    {
        public Experiment_101()
        {
        }

        protected override QS.CMS.Components.AttributeSet initializeWith(out Type testClass)
        {
            testClass = typeof(TestApp);
            return new QS.CMS.Components.AttributeSet("timeout", experimentArgs["timeout"]);
        }

        protected override void experimentWork()
        {
            GMS.GroupId groupID = new GMS.GroupId(2000);
            GMS.ISubView[] subviews = new GMS.ISubView[numberOfNodes];
            for (uint ind = 0; ind < numberOfNodes; ind++)
                subviews[ind] = new GMS.ClientServer.ImmutableSubView((int) TestApp.port_number, addresses[ind], 
                    new uint[] { TestApp.application_loid });
            GMS.ClientServer.ImmutableView membershipView = new GMS.ClientServer.ImmutableView(1, subviews);

            Coordinator.invoke(typeof(TestApp).GetMethod("initialize_experiment"),
                new object[] { groupID, CMS.Base2.CompatibilitySOWrapper.Object2ByteArray(membershipView),
					Convert.ToUInt32(experimentArgs["count"]), Convert.ToUInt32(experimentArgs["window"]) });
        }

        public new class TestApp : Experiment_100.TestApp
        {
            protected override void generate_results(QS.CMS.Components.AttributeSet resultAttributes)
            {
                // ..................

                throw new NotImplementedException();
            }

            private void initialize_experiment(GMS.GroupId groupID, byte[] membershipViewAsBytes,
                uint numberOfMulticasts, uint numberOfConcurrentMulticasts)
            {
                GMS.ClientServer.ImmutableView membershipView = (GMS.ClientServer.ImmutableView)
                    CMS.Base2.CompatibilitySOWrapper.ByteArray2Object(membershipViewAsBytes);
                this.completionCallback = new QS.CMS.Scattering.Callback(completion_upcall);

                membershipServer.distributeVCNotification(groupID, membershipView);

                platform.AlarmClock.scheduleAnAlarm(3.0, new CMS.Base.AlarmCallback(this.alarmCallback), null);
            }

            private void alarmCallback(CMS.Base.IAlarmRef alarmRef)
            {
            }

		    private CMS.Scattering.Callback completionCallback;
            private void completion_upcall(bool succeeded, System.Exception exception)
            {
            }

            public const uint application_loid = 1000;

            public TestApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args) : base(platform, args)
            {
                demultiplexer = new CMS.Base2.Demultiplexer(platform.Logger);
                rootSender = new QS.CMS.Base2.RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);
                scatterer = new CMS.Scattering.Scatterer(rootSender, platform.Logger);
                retransmittingScatterer = new QS.CMS.Scattering.SimpleRS(rootSender, scatterer, platform.AlarmClock,
                    demultiplexer, platform.Logger, 100, System.Convert.ToDouble(args["timeout"]));
                masterContainer = new CMS.Base2.MasterIOC();

                demultiplexer.register(application_loid, new CMS.Base2.ReceiveCallback(this.receiveCallback));

                if (isCoordinator)
                {
                    membershipServer = new QS.CMS.Membership.Server(platform.Logger, retransmittingScatterer);
                    allocationServer = new CMS.IPMulticast.AllocationServer(demultiplexer, platform.Logger);
                }

                CMS.Base.Serializer.Get.register(QS.ClassID.Mahesh_CSGMSImmutableView,
                    new CMS.Base.CreateSerializable(GMS.ClientServer.ImmutableView.factory));
                membershipClient = new QS.CMS.Membership.Client(platform.Logger, demultiplexer);
                rpcProxy = new CMS.RPC2.RPCProxy(demultiplexer, rootSender, platform.Logger, masterContainer, platform.AlarmClock);
                allocationClient = new CMS.Allocation.AllocationClient(new CMS.Base.ObjectAddress(
                    coordinatorAddress.HostIPAddress, (int)port_number, (uint)QS.ReservedObjectID.IPMulticast_AllocationServer),
                    rpcProxy, demultiplexer, platform.Logger);



            }

            private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress,
                QS.Fx.Network.NetworkAddress destinationAddress, CMS.Base2.IBase2Serializable serializableObject)
            {
                platform.Logger.Log(this, "ReceiveCallback : " + serializableObject.ToString());
                return null;
            }

            private CMS.Base2.IDemultiplexer demultiplexer;
            private CMS.Base2.RootSender rootSender;
            private CMS.Scattering.IScatterer scatterer;
            private CMS.Scattering.IRetransmittingScatterer retransmittingScatterer;
            private CMS.Allocation.AllocationClient allocationClient;
            private CMS.IPMulticast.AllocationServer allocationServer;
            private CMS.Membership.Server membershipServer;
            private CMS.Membership.Client membershipClient;
            private CMS.RPC2.RPCProxy rpcProxy;
            private CMS.Base2.IMasterIOC masterContainer;
        }
    }
*/ 
}
