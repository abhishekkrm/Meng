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
using System.Net;

namespace QS._qss_x_.Applications_
{
    public class Application_002 : QS.Fx.Inspection.Inspectable, Platform_.IApplication
    {
        public Application_002()
        {
        }

        private const int portno = 50000;
        private const int myloid = (int) QS.ReservedObjectID.User_Min + 200;

        private QS.Fx.Platform.IPlatform platform;
        private QS._qss_x_.Platform_.IApplicationContext context;
        private string myname;
        private bool iscoordinator;
        private QS.Fx.Network.NetworkAddress localAddress, coordinatorAddress;
        private int myindex, nmessages, ngroups, nsenders;
        private QS._qss_c_.Base3_.GroupID[] groupIDs;
        private int[] nreceived;
        private double[][] receiveTimes;

        [QS._core_c_.Diagnostics.Component("Framework")]
        [QS._core_c_.Diagnostics2.Module("Framework")]
        private QS._qss_c_.Framework_1_.Framework framework;

        [QS._core_c_.Diagnostics.Component("Regional Controller")]
        [QS._core_c_.Diagnostics2.Module("Regional Controller")]
        private QS._qss_c_.Receivers4.RegionalController regionalController;

        [QS._core_c_.Diagnostics.Component("Regional Senders")]
        [QS._core_c_.Diagnostics2.Module("Regional Senders")]
        private QS._qss_c_.Senders10.RegionalSenders regionalSenders;

        private QS._qss_c_.Multicasting7.DispatcherRV2 dispatcherRV2;

#pragma warning disable 0414

        private double rate;
        private int messagesize;

#pragma warning restore 0414

        #region IApplication Members

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            platform.Logger.Log(this, "Application starting.");

            this.platform = platform;
            this.context = context;

            myname = platform.Network.GetHostName();
            platform.Logger.Log(this, "My name: \"" + myname + "\".");

            for (myindex = context.NodeNames.Length - 1; myindex >= 0 && !context.NodeNames[myindex].Equals(myname); myindex--)
                ;

            platform.Logger.Log(this, "My index: " + myindex);

            if (myindex < 0)
                throw new Exception("Cannot continue, local node name not found on the list of nodes passed in the application context.");

            iscoordinator = myindex == 0;
            if (iscoordinator)
                platform.Logger.Log(this, "Acting as coordinator.");

            localAddress = new QS.Fx.Network.NetworkAddress(platform.Network.Interfaces[0].InterfaceAddress, portno);
            coordinatorAddress = new QS.Fx.Network.NetworkAddress(platform.Network.GetHostEntry(context.NodeNames[0]).AddressList[0], portno);

            framework = new QS._qss_c_.Framework_1_.Framework(
                new QS._core_c_.Base3.InstanceID(localAddress, QS._core_c_.Base3.Incarnation.Current), coordinatorAddress, platform, 
                new QS._qss_c_.Statistics_.MemoryController(), true, 20000, false);

            framework.Demultiplexer.register(myloid, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));

            rate = 5000;

            messagesize = 1000;
            nmessages = 1000;

            QS._qss_c_.Rings6.RateSharingAlgorithmClass rateSharingAlgorithm = QS._qss_c_.Rings6.RateSharingAlgorithmClass.Compete;
            QS._qss_c_.Rings6.ReceivingAgent receivingAgentClass = new QS._qss_c_.Rings6.ReceivingAgent(
                framework.EventLogger, framework.Logger, framework.LocalAddress, framework.AlarmClock, framework.Clock,
                framework.Demultiplexer, framework.BufferedReliableInstanceSinks,
                5, 1, 0.1, 10, 5000, rateSharingAlgorithm, framework.ReliableInstanceSinks, framework.StatisticsController);

            regionalController = new QS._qss_c_.Receivers4.RegionalController(
                framework.LocalAddress, framework.Logger, framework.Demultiplexer, framework.AlarmClock, framework.Clock,
                framework.MembershipController, receivingAgentClass, receivingAgentClass);
            regionalController.IsDisabled = false;

            regionalSenders = new QS._qss_c_.Senders10.RegionalSenders(
                framework.EventLogger, framework.LocalAddress, framework.Logger, framework.AlarmClock, framework.Clock, 
                framework.Demultiplexer, null, regionalController, regionalController, 60, regionalController,
                true, 1000, 5); 
   
            dispatcherRV2 = new QS._qss_c_.Multicasting7.DispatcherRV2(platform.Logger, framework.AlarmClock, framework.Clock,
                framework.Demultiplexer, (uint) QS.ReservedObjectID.Multicasting7_DispatcherRV2, regionalController, 
                framework.MembershipController);

            ngroups = 1;
            nsenders = 1;

            groupIDs = new QS._qss_c_.Base3_.GroupID[ngroups];
            for (int ind = 0; ind < groupIDs.Length; ind++)
                groupIDs[ind] = new QS._qss_c_.Base3_.GroupID((uint)(1000 + ind));

            nreceived = new int[nsenders];
            receiveTimes = new double[nsenders][];
            for (int j = 0; j < nsenders; j++)
                receiveTimes[j] = new double[nmessages];

/*
#if DEBUG_EnableStatistics
                receiveRates = new QS.CMS.Statistics.Samples2D[nsenders];
                totalReceived = new QS.CMS.Statistics.Samples2D[nsenders];
                for (int j = 0; j < nsenders; j++)
                {
                    receiveRates[j] = new QS.CMS.Statistics.Samples2D();
                    totalReceived[j] = new QS.CMS.Statistics.Samples2D();

                    ((QS.CMS.QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(
                        "ReceivedRate_" + j.ToString("000"), new QS.CMS.QS._core_c_.Diagnostics2.Property(receiveRates[j]));
                    ((QS.CMS.QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(
                        "TotalReceived_" + j.ToString("000"), new QS.CMS.QS._core_c_.Diagnostics2.Property(totalReceived[j]));
                }
#endif

                last_nreceived = new int[nsenders];

                measure_latencies = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "measure_latencies");
                    
                QS.CMS.Base.IFactory<QS.CMS.FlowControl7.IRateController> rateControllers = null;
                switch (Convert.ToInt32((string)args["rs_ratecontroller"]))
                {
                    case 1:
                        rateControllers = new QS.CMS.FlowControl7.DummyController1(rate);
                        break;

                    case 2:
                        rateControllers = new QS.CMS.FlowControl7.DummyController2(framework.Clock, rate);
                        break;

                    case 3:
                        rateControllers = new QS.CMS.FlowControl7.RateController1(
                            framework.Clock, Convert.ToDouble((string)args["rs_growth_coefficient"]));
                        break;

                    default:
                        break;
                }

                nodeSinkCollection = framework.Root;
                regionSinkCollection = new QS.CMS.Multicasting7.RegionSinks(framework.MembershipController, nodeSinkCollection);

                regionViewSinkCollection = new QS.CMS.Multicasting7.ReliableRegionViewSinks(framework.StatisticsController,
                    framework.Logger, framework.EventLogger, framework.LocalAddress,
                    framework.AlarmClock, framework.Clock, (uint)QS.ReservedObjectID.Rings6_SenderController1_DataChannel,
                    (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel,
                    regionSinkCollection, regionalController, regionalController, framework.MembershipController, framework.Root,
                    args.contains("rs_timeout") ? Convert.ToDouble((string)args["rs_timeout"]) : 10, rateControllers,
                    QS.TMS.Experiment.Helpers.Args.DoubleOf(args, "rs_warmuptime", 1));

                int maximumPendingCompletion = QS.TMS.Experiment.Helpers.Args.Int32Of(args, "gs_max_pending_ack", 100000);
                int feed_buffer_min = QS.TMS.Experiment.Helpers.Args.Int32Of(args, "gs_feed_buffermin", 50);
                int feed_buffer_max = QS.TMS.Experiment.Helpers.Args.Int32Of(args, "gs_feed_buffermax", 150);

                if (QS.TMS.Experiment.Helpers.Args.BoolOf(args, "dummy_gs"))
                    groupSinkCollection = new QS.CMS.Multicasting7.PlaceholderGSs(framework.MembershipController, regionViewSinkCollection);
                else
                {
                    if (QS.TMS.Experiment.Helpers.Args.BoolOf(args, "alternative_gs"))
                    {
                        groupSinkCollection =
                            new QS.CMS.Multicasting7.AlternativeReliableGroupSinks(
                                framework.StatisticsController, framework.Clock, framework.MembershipController, logger,
                                nodeSinkCollection, ((QS.CMS.Multicasting7.ReliableRegionViewSinks) regionViewSinkCollection),
                                (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, framework.Root, null, 
                                QS.TMS.Experiment.Helpers.Args.DoubleOf(args, "ags_initialrate", 1000));
                    }
                    else
                    {
                        groupSinkCollection =
                            new QS.CMS.Multicasting7.ReliableGroupSinks(
                                framework.StatisticsController, framework.Clock, framework.MembershipController, regionViewSinkCollection, logger,
                                maximumPendingCompletion, feed_buffer_min, feed_buffer_max);
                    }
                }                

                ((QS.CMS.QS._core_c_.Diagnostics2.IContainer) diagnosticsContainer).Register("Sources", diagnosticsContainerForSources); 
                QS.CMS.QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

                clock = QS.CMS.Core.Clock.SharedClock;

                core.Start();

                logger.Log(this, "Ready");
*/

            platform.Logger.Log(this, "Application started.");
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
            platform.Logger.Log(this, "Application stopping.");

            // .....................................................................................................................................................................

            platform.Logger.Log(this, "Application stopped.");
        }

        #endregion

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID senderAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            QS._core_c_.Base2.BlockOfData block = (QS._core_c_.Base2.BlockOfData) receivedObject;

/*
            int seqno, remoteSenderIndex;
            byte[] bytes = block.Buffer;
            double latency;
            unsafe
            {
                fixed (byte* pbytes = bytes)
                {
                    byte* pbuffer = pbytes + block.OffsetWithinBuffer;
                    remoteSenderIndex = *((int*)pbuffer);
                    seqno = *((int*)(pbuffer + sizeof(int)));
                    if (measure_latencies)
                        latency = clock.Time - (*((double*)(pbuffer + 2 * sizeof(int))));
                }
            }
            // seqno = argument.Int2;

            // CMS.Base3.Int32x2 argument = (CMS.Base3.Int32x2)receivedObject;
            // seqno = argument.Int2;
            // remoteSenderIndex = argument.Int1;

            double time_now = framework.Clock.Time;
            if (detailed_timings)
                receiveTimes[remoteSenderIndex][seqno] = time_now;
            nreceived[remoteSenderIndex] = nreceived[remoteSenderIndex] + 1;
            overall_nreceived++;

            if (badtime_track && overall_nreceived > badtime_nmessages)
            {
                if (should_die)
                {
                    GetStatistics(false);
                    Thread.Sleep(5);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                else if (should_sleep)
                {
                    logger.Log(this, "SLEEPING(" + sleep_duration.ToString() + ")");
                    Thread.Sleep((int)Math.Floor(sleep_duration * 1000));
                    should_sleep = false;
                }
                else if (should_drop)
                {
                    logger.Log(this, "Starting to drop...");
                    should_drop = false;
                    // framework.Core.Drop(drop_timetostart, drop_timetostop, repeat_drop);
                    if (drop_duration > 0)
                        core.Drop(drop_interval, drop_duration, drop_repeat);
                    else if (drop_count > 0)
                        core.Drop(drop_interval, drop_count, drop_repeat);
                    else
                        logger.Log(this, "Cannot drop: duration and count are both zero.");
                }

                badtime_track = false;
            }

            if (time_now > lastChecked + 1)
                SampleRates(time_now);
*/

            return null;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            platform.Logger.Log(this, "Application disposing.");

            // .....................................................................................................................................................................

            platform.Logger.Log(this, "Application disposed.");
        }

        #endregion
    }
}
