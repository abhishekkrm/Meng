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

// #define DEBUG_EnableStatistics
// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

using QS._qss_e_.Parameters_.Specifications;

namespace QS._qss_e_.Experiments_
{
    /// <summary>
    /// Simple test running the whole protocol stack on top of the single-threaded core.
    /// </summary>
//    [TMS.Experiments.DefaultExperiment]
    public class Experiment_265 : Experiment_200
    {
        private const string DefaultRepositoryPath = "C:\\.QuickSilver\\.Repository";

        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            logger.Log(this, "Initial wait....");
            sleeper.sleep(5);

            logger.Log(this, "Collecting time....");

            // using (QS.CMS.Connections.MulticastCollector collector = new QS.CMS.Connections.MulticastCollector(
            //    new QS.CMS.Platform.PhysicalPlatform(logger), new QS.Fx.Network.NetworkAddress(Application.ClockServiceAddress)))
            // {
            //    sleeper.sleep(arguments.contains("clocksync") ? Convert.ToDouble((string) arguments["clocksync"]) : 5);
            //    foreach (QS.CMS.Base3.Now element in collector.CollectedObjects)
            //        logger.Log(null, element.Address.ToString() + "\n" + element.Time.ToString("000000000.000000000"));
            // }
            try
            {
                using (QS._qss_c_.Devices_3_.Network network = new QS._qss_c_.Devices_3_.Network(logger, 20000))
                {
                    using (QS._qss_c_.Devices_3_.UDPCommunicationsDevice udpdevice =
                        (QS._qss_c_.Devices_3_.UDPCommunicationsDevice)(network).OnSubnets(new QS._qss_c_.Base1_.Subnet[]
                        {
                            new QS._qss_c_.Base1_.Subnet("192.168.2.x"),
                            new QS._qss_c_.Base1_.Subnet("172.23.64.x"),
                            new QS._qss_c_.Base1_.Subnet("172.23.79.x")
                        })[QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP])
                    {
                        logger.Log(null,
                            "Sending clock signal from " + udpdevice.Address.ToString() + " to " + Application.ClockServiceAddress.ToString());

                        QS._qss_c_.Devices_3_.ISender sender = udpdevice.GetSender(Application.ClockServiceAddress);
                        for (uint ind = 1; ind <= 50; ind++)
                        {
                            sender.send(QS._core_c_.Base3.Serializer.ToSegments(new QS._qss_c_.Base3_.SequenceNo(ind)));
                            sleeper.sleep(0.1);
                        }

                        sender.send(QS._core_c_.Base3.Serializer.ToSegments(new QS._qss_c_.Base3_.SequenceNo(0)));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, "Could not send clock synchronization signal.\n" + exc.ToString());
            }

            logger.Log(this, "Waiting 2s....");

            sleeper.sleep(2);

            logger.Log(this, "Changing membership....");

            for (int ind = 0; ind < this.NumberOfApplications; ind++)
            {
                this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("ChangeMembership"), new object[] { ind });
            }

            logger.Log(this, "Waiting for the system to stabilize.");

            sleeper.sleep(Convert.ToDouble((string)arguments["stabilize"]));

            logger.Log(this, "Starting to multicast.");

            nsenders = Convert.ToInt32((string)arguments["nsenders"]);
            for (int ind = 0; ind < nsenders; ind++)
            {
                this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("Send"), new object[] { ind });
            }

            sendingCompleted.WaitOne();

            logger.Log(this, "Multicasting completed, cooling down now...");

            sleeper.sleep(Convert.ToDouble((string)arguments["cooldown"]));

            if (arguments.contains("download") && (arguments["download"].Equals("on") || arguments["download"].Equals("yes")))
            {
                logger.Log(this, "Downloading application statistics...");

                QS._core_c_.Components.AttributeSet applicationStatistics = new QS._core_c_.Components.AttributeSet();
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                {
                    logger.Log(this, "Downloading from " + application.AppID.ToString());
                    object obj = application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { true });
                    if (obj != null && obj is QS._core_c_.Components.Attribute)
                        applicationStatistics.Add((QS._core_c_.Components.Attribute)obj);
                }

                results["Application Statistics"] = new QS._core_c_.Components.Attribute("Application Statistics", applicationStatistics);
            }
            else
            {
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                    application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { false });
            }

            if (arguments.contains("save") && (arguments["save"].Equals("yes") || arguments["save"].Equals("on")))
                QS._qss_e_.Experiment_.Helpers.SaveResults.Save(DefaultRepositoryPath, logger, this, arguments, results);

            logger.Log(this, "Completed.");
        }

        #endregion

        private int nsenders, nsendersCompleted;

        #region Callbacks

        private ManualResetEvent sendingCompleted = new ManualResetEvent(false);
        public void SendingCompleted(QS._core_c_.Components.AttributeSet arguments)
        {
            lock (this)
            {
                nsendersCompleted++;
                if (nsendersCompleted >= nsenders)
                    sendingCompleted.Set();
            }
        }

        #endregion

        #region Class Application

        private const int ANTICIPATED_RUNNING_TIME = 10000;

        protected new class Application : Experiment_200.Application, QS._qss_e_.Runtime_.IControlledApp, QS._qss_c_.Devices_3_.IReceiver
        {
            // private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
            private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

            public static readonly QS.Fx.Network.NetworkAddress ClockServiceAddress =
                new QS.Fx.Network.NetworkAddress("224.81.80.79:63211");

            #region Constructor

            #region IReceiver Members

            private List<KeyValuePair<uint, double>> clockSignals = new List<KeyValuePair<uint, double>>();
            void QS._qss_c_.Devices_3_.IReceiver.receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> data)
            {
                double time_now = framework.Clock.Time;
                uint seqno = ((QS._qss_c_.Base3_.SequenceNo)QS._core_c_.Base3.Serializer.FromSegment(data)).Value;
                clockSignals.Add(new KeyValuePair<uint, double>(seqno, time_now));
                if (seqno == 0)
                {
                    StringBuilder s = new StringBuilder("Clock Signal:\n");
                    foreach (KeyValuePair<uint, double> element in clockSignals)
                        s.AppendLine(element.Key.ToString() + "\t" + element.Value.ToString());
                    logger.Log(null, s.ToString());
                }
            }

            #endregion

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                string rootpath = repository_root + "\\" + repository_key;
                core = new QS._core_c_.Core.Core(rootpath + "\\coreworkdir");
                string fsroot = rootpath + "\\filesystem";

                framework = new QS._qss_c_.Framework_1_.FrameworkOnCore(
                    new QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress,
                    platform.Logger, platform.EventLogger, core, fsroot, false, 0, false);

                network = new QS._qss_c_.Devices_3_.Network(logger, 20000);
                udpdevice = (QS._qss_c_.Devices_3_.UDPCommunicationsDevice)
                    network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];
                listener = udpdevice.ListenAt(ClockServiceAddress, this);

                // multicastResponder = new QS.CMS.Connections.MulticastResponder(platform, 
                //    new QS.CMS.Base3.Now(framework.Clock, new QS.CMS.QS._core_c_.Base3.InstanceID(localAddress, incarnation)), 
                //    new QS.Fx.Network.NetworkAddress(ClockServiceAddress));

                if (args.contains("alarm_quantum"))
                    core.MaximumQuantumForAlarms = Convert.ToDouble((string)args["alarm_quantum"]);

                if (args.contains("io_quantum"))
                    core.MaximumQuantumForCompletionPorts = Convert.ToDouble((string)args["io_quantum"]);

                if (args.contains("fc_unicast_credits"))
                    core.DefaultMaximumSenderUnicastCredits = Convert.ToDouble((string)args["fc_unicast_credits"]);

                if (args.contains("fc_multicast_credits"))
                    core.DefaultMaximumSenderMulticastCredits = Convert.ToDouble((string)args["fc_multicast_credits"]);

                if (args.contains("sender_cc"))
                    core.DefaultMaximumSenderConcurrency = Convert.ToInt32((string)args["sender_cc"]);

                double rate;
                if (args.contains("rate"))
                    rate = Convert.ToDouble((string)args["rate"]);
                else
                    rate = double.PositiveInfinity;

                // framework.Core.DefaultMaximumSenderRate = rate;

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "gui"))
                    AppController.Show("Experiment 263 App Controller", this);

                framework.Demultiplexer.register(myloid, new QS._qss_c_.Base3_.ReceiveCallback(ReceiveCallback));

                messagesize = Convert.ToInt32(args["size"]);
                if (messagesize < sizeof(uint))
                    throw new Exception("Message size too small.");
                nmessages = Convert.ToInt32(args["count"]);

                nsenders = Convert.ToInt32((string)args["nsenders"]);

                QS._qss_c_.Rings6.RateSharingAlgorithmClass rateSharingAlgorithm = (QS._qss_c_.Rings6.RateSharingAlgorithmClass)
                    Enum.Parse(typeof(QS._qss_c_.Rings6.RateSharingAlgorithmClass), (string)args["rs_ratesharing"], true);

                receivingAgentClass = new QS._qss_c_.Rings6.ReceivingAgent(
                    framework.EventLogger, framework.Logger, framework.LocalAddress, framework.AlarmClock, framework.Clock,
                    framework.Demultiplexer, framework.BufferedReliableInstanceSinks, // framework.ReliableSender, 
                    5, 1, 0.1, 10, 5000, rateSharingAlgorithm, framework.ReliableInstanceSinks, framework.StatisticsController);

                if (args.contains("rs_token_rate"))
                {
                    double frequency = Convert.ToDouble((string)args["rs_token_rate"]);
                    receivingAgentClass.TokenRate = frequency;
                }

                if (args.contains("rs_replication"))
                {
                    int replication = Convert.ToInt32((string)args["rs_replication"]);
                    receivingAgentClass.ReplicationCoefficient = (uint)replication;
                }

                receivingAgentClass.PullCaching = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "rs_pullcaching");
                receivingAgentClass.NaksAllowed = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "rs_naks");

                if (args.contains("rs_max_window"))
                    receivingAgentClass.MaximumWindowWidth = (uint)Convert.ToInt32((string)args["rs_max_window"]);

                if (args.contains("rs_max_naks"))
                    receivingAgentClass.MaximumNakRangesPerToken = (uint)Convert.ToInt32((string)args["rs_max_naks"]);

                regionalController = new QS._qss_c_.Receivers4.RegionalController(
                    framework.LocalAddress, framework.Logger, framework.Demultiplexer, framework.AlarmClock, framework.Clock,
                    framework.MembershipController, receivingAgentClass, receivingAgentClass);
                regionalController.IsDisabled = false;

                regionalSenders = new QS._qss_c_.Senders10.RegionalSenders(framework.EventLogger, framework.LocalAddress,
                    framework.Logger, framework.AlarmClock, framework.Clock, framework.Demultiplexer, null, // this null argument needs to be fixed 
                    regionalController, regionalController, 60, regionalController, false, 0, 1);

                ngroups = args.contains("ngroups") ? Convert.ToInt32(args["ngroups"]) : 1;
                nregions = args.contains("nregions") ? Convert.ToInt32(args["nregions"]) : 1;
                groupIDs = new QS._qss_c_.Base3_.GroupID[ngroups];
                for (int ind = 0; ind < groupIDs.Length; ind++)
                    groupIDs[ind] = new QS._qss_c_.Base3_.GroupID((uint)(1000 + ind));
                separator_groupIDs = new QS._qss_c_.Base3_.GroupID[nregions];
                for (int ind = 0; ind < separator_groupIDs.Length; ind++)
                    separator_groupIDs[ind] = new QS._qss_c_.Base3_.GroupID((uint)(1001000 + ind));

                detailed_timings = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "detailed_timings");

                nreceived = new int[nsenders];
                if (detailed_timings)
                {
                    receiveTimes = new double[nsenders][];
                    for (int j = 0; j < nsenders; j++)
                        receiveTimes[j] = new double[nmessages];
                }
                else
                    receiveTimes = null;

#if DEBUG_EnableStatistics
                receiveRates = new QS.CMS.Statistics.SamplesXY[nsenders];
                for (int j = 0; j < nsenders; j++)
                    receiveRates[j] = new QS.CMS.Statistics.SamplesXY();
#endif

                last_nreceived = new int[nsenders];

                QS._qss_c_.Base1_.IFactory<QS._qss_c_.FlowControl7.IRateController> rateControllers = null;
                switch (Convert.ToInt32((string)args["rs_ratecontroller"]))
                {
                    case 1:
                        rateControllers = new QS._qss_c_.FlowControl7.DummyController1(rate);
                        break;

                    case 2:
                        rateControllers = new QS._qss_c_.FlowControl7.DummyController2(framework.Clock, rate);
                        break;

                    case 3:
                        rateControllers = new QS._qss_c_.FlowControl7.RateController1(
                            framework.Clock, Convert.ToDouble((string)args["rs_growth_coefficient"]));
                        break;

                    default:
                        break;
                }

                /*
                                    bool should_adjust = param_adjust_rate.Equals("yes");
                                    QS.CMS.FlowControl3.IEstimatorClass rateEstimatorClass;
                                    if (should_adjust)
                                    {
                //                        rateEstimatorClass = new QS.CMS.FlowControl3.MovingAverageEstimator(
                //                            10, Convert.ToDouble((string)args["rs_multiplier"]), Convert.ToDouble((string)args["rs_maxinc"]), 100, 20000);
                                    }
                */

                nodeSinkCollection = framework.Root;
                regionSinkCollection = new QS._qss_c_.Multicasting7.RegionSinks(framework.MembershipController, nodeSinkCollection);
                regionViewSinkCollection = new QS._qss_c_.Multicasting7.ReliableRegionViewSinks(framework.StatisticsController,
                    framework.Logger, framework.EventLogger, framework.LocalAddress,
                    framework.AlarmClock, framework.Clock, (uint)QS.ReservedObjectID.Rings6_SenderController1_DataChannel,
                    (uint) QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel,
                    regionSinkCollection, regionalController, regionalController, framework.MembershipController, framework.Root,
                    args.contains("rs_timeout") ? Convert.ToDouble((string)args["rs_timeout"]) : 10, rateControllers, 1);

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "dummy_gs"))
                    groupSinkCollection = new QS._qss_c_.Multicasting7.PlaceholderGSs(framework.MembershipController, regionViewSinkCollection);
                else
                    groupSinkCollection =
                        new QS._qss_c_.Multicasting7.ReliableGroupSinks(framework.StatisticsController, framework.Clock, 
                        framework.MembershipController, regionViewSinkCollection, logger, 100000, 50, 150);

                core.Start();

                logger.Log(this, "Ready");
            }

            #endregion

            private QS._qss_c_.Framework_1_.FrameworkOnCore framework;

            [QS._core_c_.Diagnostics.Component("Core")]
            [QS._core_c_.Diagnostics2.Module("Core")]
            private QS._core_c_.Core.Core core;

            private QS._qss_c_.Devices_3_.Network network;
            private QS._qss_c_.Devices_3_.UDPCommunicationsDevice udpdevice;
            private QS._qss_c_.Devices_3_.IListener listener;
            // private QS.CMS.Connections.MulticastResponder multicastResponder;

            private QS._qss_c_.Rings6.ReceivingAgent receivingAgentClass;
            private QS._qss_c_.Receivers4.RegionalController regionalController;
            private QS._qss_c_.Senders10.RegionalSenders regionalSenders;

            private QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> nodeSinkCollection;
            private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RegionID,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionSinkCollection;
            private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RVID,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionViewSinkCollection;
            private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> groupSinkCollection;

            private int ngroups, nregions, nsenders;
            private QS._qss_c_.Base3_.GroupID[] groupIDs, separator_groupIDs;
            private MySource[] mySources;

            [QS._core_c_.Diagnostics.Ignore]
            [QS.Fx.Inspection.Ignore]
            private double[] sendTimes, completionTimes;
            private double[][] receiveTimes;

#if DEBUG_EnableStatistics
            private QS.CMS.Statistics.SamplesXY sendRates, completionRates;
            private QS.CMS.Statistics.SamplesXY[] receiveRates;
#endif

            private double lastChecked;
            private int last_nsent, last_ncompleted;
            private int[] last_nreceived;
            // private double rate;
            private int messagesize, nmessages, nsent, ncompleted, senderIndex;
            private int[] nreceived;
            private bool detailed_timings;

            private void SampleRates(double time_now)
            {
                double time_tolog = (lastChecked + time_now) / 2;
                double time_delta = time_now - lastChecked;

#if DEBUG_EnableStatistics
                if (sendRates == null)
                    sendRates = new QS.CMS.Statistics.SamplesXY();
                sendRates.addSample(time_tolog, (nsent - last_nsent) / time_delta);
#endif

                last_nsent = nsent;

#if DEBUG_EnableStatistics
                if (completionRates == null)
                    completionRates = new QS.CMS.Statistics.SamplesXY();
                completionRates.addSample(time_tolog, (ncompleted - last_ncompleted) / time_delta);
#endif

                last_ncompleted = ncompleted;
                for (int j = 0; j < nsenders; j++)
                {
#if DEBUG_EnableStatistics
                    receiveRates[j].addSample(time_tolog, (nreceived[j] - last_nreceived[j]) / time_delta);
#endif

                    last_nreceived[j] = nreceived[j];
                }
                lastChecked = time_now;
            }

            [QS._core_c_.Diagnostics.Component("Send Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet SendTimes
            {
                get
                {
                    if (sendTimes != null)
                        return new QS._core_e_.Data.DataSeries(sendTimes);
                    else
                        throw new Exception("Unavailable.");
                }
            }

            [QS._core_c_.Diagnostics.Component("Completion Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet CompletionTimes
            {
                get
                {
                    if (completionTimes != null)
                        return new QS._core_e_.Data.DataSeries(completionTimes);
                    else
                        throw new Exception("Unavailable.");
                }
            }

            [QS._core_c_.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
            public QS._core_e_.Data.IDataSet[] ReceiveTimes
            {
                get
                {
                    if (receiveTimes != null)
                    {
                        QS._core_e_.Data.DataSeries[] result = new QS._core_e_.Data.DataSeries[nsenders];
                        for (int ind = 0; ind < nsenders; ind++)
                            result[ind] = new QS._core_e_.Data.DataSeries(receiveTimes[ind]);
                        return result;
                    }
                    else
                        throw new Exception("Unavailable.");
                }
            }

            [QS._core_c_.Diagnostics.Component("Combined Statistics (X = seqno, Y = time)")]
            private QS._core_e_.Data.IDataSet CombinedStatistics
            {
                get
                {
                    if (sendTimes != null && completionTimes != null)
                    {
                        QS._core_e_.Data.DataCo collection = new QS._core_e_.Data.DataCo();
                        collection.Add(new QS._core_e_.Data.Data1D("send times", new QS._core_e_.Data.DataSeries(sendTimes)));
                        collection.Add(new QS._core_e_.Data.Data1D("completion times", new QS._core_e_.Data.DataSeries(completionTimes)));
                        // collection.Add(new QS.TMS.Data.Data1D("receive times", new QS.TMS.Data.DataSeries(receiveTimes)));
                        return collection;
                    }
                    else
                        throw new Exception("Unavailable.");
                }
            }

            #region ChangeMembership

            public void ChangeMembership(int index)
            {
                framework.Platform.Scheduler.Execute(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        List<QS._qss_c_.Base3_.GroupID> groupsToJoin = new List<QS._qss_c_.Base3_.GroupID>();
                        groupsToJoin.AddRange(groupIDs);
                        groupsToJoin.Add(separator_groupIDs[index % nregions]);
                        framework.MembershipAgent.ChangeMembership(groupsToJoin, new List<QS._qss_c_.Base3_.GroupID>());
                    }), null);
            }

            #endregion

            #region Sending

            public void Send(int senderIndex)
            {
                this.senderIndex = senderIndex;

                if (detailed_timings)
                {
                    sendTimes = new double[nmessages];
                    completionTimes = new double[nmessages];
                }
                else
                    sendTimes = completionTimes = null;

                mySources = new MySource[groupIDs.Length];
                for (int ind = 0; ind < groupIDs.Length; ind++)
                {
                    mySources[ind] = new MySource(this, ind, groupIDs.Length, groupIDs[ind], (nmessages - ind) / groupIDs.Length);
                }

                framework.Platform.Scheduler.Execute(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        for (int ind = 0; ind < groupIDs.Length; ind++)
                        {
                            groupSinkCollection[groupIDs[ind]].Send(
                            new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(
                            mySources[ind].GetCallback));
                        }
                    }), null);
            }

            [QS.Fx.Base.Inspectable]
            private class MySource : QS.Fx.Inspection.Inspectable
            {
                public MySource(Application owner, int index, int ngroups, QS._qss_c_.Base3_.GroupID groupID, int nmessages)
                {
                    this.owner = owner;
                    this.index = index;
                    this.ngroups = ngroups;
                    this.groupID = groupID;
                    this.nmessages = nmessages;
                }

                private Application owner;
                private QS._qss_c_.Base3_.GroupID groupID;
                private int index, ngroups, nsent, nmessages;

                public void GetCallback(
                    Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> objectQueue,
                    int maximumNumberOfObjects,
#if UseEnhancedRateControl
                    int maximumNumberOfBytes, 
#endif
                    out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
                    out int numberOfBytesReturned,
#endif
                    out bool moreObjectsAvailable)
                {
                    // TODO: Implement enhanced rate control

                    numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
                    numberOfBytesReturned = 0;
#endif
                    moreObjectsAvailable = true;

                    while (numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
                    {
                        if (nsent < nmessages)
                        {
                            int seqno = (nsent * ngroups) + index;
                            nsent++;
                            owner.nsent++;
                            double time_now = owner.framework.Clock.Time;
                            if (owner.detailed_timings)
                                owner.sendTimes[seqno] = time_now;
                            if (time_now > owner.lastChecked + 1)
                                owner.SampleRates(time_now);

                            byte[] bytes = new byte[owner.messagesize];
                            unsafe
                            {
                                fixed (byte* pbytes = bytes)
                                {
                                    *((int*)pbytes) = owner.senderIndex;
                                    *((int*)(pbytes + sizeof(int))) = seqno;
                                }
                            }
                            QS._core_c_.Base2.BlockOfData block = new QS._core_c_.Base2.BlockOfData(bytes);
                            QS.Fx.Serialization.ISerializable serializableObject = block; // new QS.CMS.Base3.Int32x2(owner.senderIndex, seqno);
                            objectQueue.Enqueue(
                                new QS._qss_c_.Base6_.AsynchronousMessage(new QS._core_c_.Base3.Message(myloid, serializableObject),
                                new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback)));
                            numberOfObjectsReturned++;
                            // numberOfBytesReturned += .................................................................HERE
                        }
                        else
                        {
                            moreObjectsAvailable = false;
                            break;
                        }
                    }
                }

                private void CompletionCallback(bool succeeded, Exception exception, object context)
                {
                    // owner.logger.Log(this,  "__MySource.Callback");
                    int seqno;
                    QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request =
                        (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>)context;
                    QS._core_c_.Base2.BlockOfData block = (QS._core_c_.Base2.BlockOfData)request.Argument.transmittedObject;
                    // CMS.Base3.Int32x2 argument = (CMS.Base3.Int32x2) request.Argument.transmittedObject;
                    byte[] bytes = block.Buffer;
                    unsafe
                    {
                        fixed (byte* pbytes = bytes)
                        {
                            byte* pbuffer = pbytes + block.OffsetWithinBuffer;
                            seqno = *((int*)(pbuffer + sizeof(int)));
                        }
                    }
                    // seqno = argument.Int2;

                    double time_now = owner.framework.Clock.Time;
                    if (owner.detailed_timings)
                        owner.completionTimes[seqno] = time_now;
                    owner.ncompleted++;
                    if (time_now > owner.lastChecked + 1)
                        owner.SampleRates(time_now);

                    if (owner.ncompleted >= owner.nmessages)
                        owner.applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
                }
            }

            #endregion

            #region ReceiveCallback

            private QS.Fx.Serialization.ISerializable ReceiveCallback(
                QS._core_c_.Base3.InstanceID senderAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                QS._core_c_.Base2.BlockOfData block = (QS._core_c_.Base2.BlockOfData)receivedObject;
                int seqno, remoteSenderIndex;
                byte[] bytes = block.Buffer;
                unsafe
                {
                    fixed (byte* pbytes = bytes)
                    {
                        byte* pbuffer = pbytes + block.OffsetWithinBuffer;
                        remoteSenderIndex = *((int*)pbuffer);
                        seqno = *((int*)(pbuffer + sizeof(int)));
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
                if (time_now > lastChecked + 1)
                    SampleRates(time_now);

                return null;
            }

            #endregion

            #region GetStatistics

            public QS._core_c_.Components.Attribute GetStatistics(bool upload)
            {
                return upload ? (new QS._core_c_.Components.Attribute(localAddress.ToString(),
                    QS._qss_c_.Diagnostics_1_.DataCollector.Collect(this))) : (new QS._core_c_.Components.Attribute("foo", "bar"));
            }

            #endregion

            #region Terminating and Disposing

            public override void TerminateApplication(bool smoothly)
            {
                platform.ReleaseResources();
            }

            public override void Dispose()
            {
                listener.Dispose();
                udpdevice.Dispose();
                network.Dispose();
            }

            #endregion

            #region IControlledApp Members

            bool QS._qss_e_.Runtime_.IControlledApp.Running
            {
                get { return core.Running; }
            }

            void QS._qss_e_.Runtime_.IControlledApp.Start()
            {
                core.Start();
            }

            void QS._qss_e_.Runtime_.IControlledApp.Stop()
            {
                core.Stop();
            }

            #endregion
        }

        #endregion

        #region Other Garbage

        public Experiment_265()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
}
