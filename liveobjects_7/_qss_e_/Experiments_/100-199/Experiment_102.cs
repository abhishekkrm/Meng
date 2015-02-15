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
using System.Net;
using System.Threading;

#endregion

namespace QS._qss_e_.Experiments_
{
/*
    [TMS.Base.Arguments("-nnodes:20 -count:5")]
    public class Experiment_102 : Experiment_100
    {
        public Experiment_102()
        {
        }

        private static System.Type testClass = typeof(QS.TMS.Experiments.Experiment_102.TestApp);
        protected override QS.CMS.Components.AttributeSet initializeWith(out System.Type testClass)
        {
            testClass = Experiment_102.testClass;
            return CMS.Components.AttributeSet.None;
        }

        protected override void experimentWork()
        {
            System.Reflection.MethodInfo method = testClass.GetMethod("start_multicasting");
            if (method == null)
                throw new Exception("Cannot find the method!");
            Coordinator.invoke(method, new object[] { 
                (uint) Convert.ToUInt32((string) experimentArgs["count"]), numberOfNodes });
        }

        public new class TestApp : Experiment_100.TestApp
        {
            protected override void generate_results(QS.CMS.Components.AttributeSet resultAttributes)
            {
                resultAttributes["roundtrip_times"] = new TMS.Data.DataSeries(roundtrip_times);
                resultAttributes["interarrival_times"] = new TMS.Data.DataSeries(interarrival_times);
            }

            private AutoResetEvent multicastCompleted;
            private double[] interarrival_times, roundtrip_times;
            private int numberOfPendingAcks;
            public void start_multicasting(uint numberOfMulticasts, uint numberOfNodes)
            {
                CMS.Base2.BlockOfData blockOfData = 
                    new QS.CMS.Base2.BlockOfData(localAddress.Size + CMS.Base2.SizeOf.UInt32);
                ((CMS.Base2.IBase2Serializable)localAddress).save(blockOfData);
                CMS.Base2.Serializer.saveUInt32(0, blockOfData);

                roundtrip_times = new double[numberOfMulticasts];
                interarrival_times = new double[numberOfMulticasts - 1];
                
                double oldFinishedTime = 0.0;
                for (uint ind = 0; ind < numberOfMulticasts; ind++)
                {
                    numberOfPendingAcks = (int) numberOfNodes - 1;
                    blockOfData.resetCursor();

                    double startingTime = platform.Clock.Time;

                    platform.UDPDevice.sendto(localAddress, groupAddress, blockOfData);
                    multicastCompleted.WaitOne();

                    double finishedTime = platform.Clock.Time;

                    roundtrip_times[ind] = finishedTime - startingTime;
                    if (ind > 0)
                        interarrival_times[ind - 1] = finishedTime - oldFinishedTime;

                    oldFinishedTime = finishedTime;
                }

                // experimentComplete.Set();
                completed();
            }
           
            public static QS.Fx.Network.NetworkAddress groupAddress =
                new QS.Fx.Network.NetworkAddress(IPAddress.Parse("224.9.9.9"), 33333);

            public TestApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args) : base(platform, args)
            {
                QS.CMS.Devices2.OnReceiveCallback callback = new QS.CMS.Devices2.OnReceiveCallback(this.receiveCallback);
                platform.UDPDevice.listenAt(localAddress.HostIPAddress, localAddress, callback);
                if (!isCoordinator)
                    platform.UDPDevice.listenAt(localAddress.HostIPAddress, groupAddress, callback);
                else
                    multicastCompleted = new AutoResetEvent(false);
            }

            private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
                QS.Fx.Network.NetworkAddress destinationAddress, CMS.Base2.IBlockOfData blockOfData)
            {
//                platform.Logger.Log(this, "__receiveCallback : " + sourceAddress.ToString() + " -> " +
//                    destinationAddress.ToString() + ", " + blockOfData.Size.ToString() + " bytes");

                if (isCoordinator)
                {
                    uint seqno = CMS.Base2.Serializer.loadUInt32(blockOfData);
                    // platform.Logger.Log(this, "ACK: " + seqno.ToString());

                    if (Interlocked.Decrement(ref numberOfPendingAcks) == 0)
                        multicastCompleted.Set();
                }
                else
                {
                    QS.Fx.Network.NetworkAddress responseAddress = new QS.Fx.Network.NetworkAddress();
                    ((CMS.Base2.IBase2Serializable) responseAddress).load(blockOfData);

//                    platform.Logger.Log(this, "ResponseAddress : " + responseAddress.ToString());

                    platform.UDPDevice.sendto(localAddress, responseAddress, blockOfData);
                }
            }
       }
    }
*/ 
}
