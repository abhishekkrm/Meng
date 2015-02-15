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
    [QS._qss_x_.Platform_.Application("QSM")]
    public sealed class QSM : QS._qss_x_.Platform_.IApplication
    {
        #region Constructor

        public QSM()
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Fields

        [QS._core_c_.Diagnostics.Component("Platform")]
        private QS.Fx.Platform.IPlatform platform;

        private QS._qss_x_.Platform_.IApplicationContext context;

        [QS._core_c_.Diagnostics.Component("Framework")]
        [QS._core_c_.Diagnostics2.Module("Framework")]
        private QS._qss_c_.Framework_1_.Framework framework;

        private QS.Fx.Logging.ILogger logger;

        private QS._core_c_.Base3.Incarnation incarnation;
        private QS.Fx.Network.NetworkAddress localAddress, coordinatorAddress;
        private bool isCoordinator;

        #region (other shit)

/*
        private int appno;

		protected QS.CMS.Components.AttributeSet launchargs;

        [TMS.Inspection.Inspectable("Experiment Path")]
        protected string experimentPath;

        protected string repository_root, repository_key;

        private bool monitoring_activity;
        private double monitoring_interval;

        private QS.CMS.QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS.CMS.QS._core_c_.Diagnostics2.Container();
        private QS.CMS.QS._core_c_.Diagnostics2.Container diagnosticsContainerForSources = new QS.CMS.QS._core_c_.Diagnostics2.Container();

        [QS.CMS.Diagnostics.Component("Performance Log")]
        [QS.CMS.QS._core_c_.Diagnostics2.Module("PerformanceLog")]
        private QS.CMS.Diagnostics3.PerformanceLog performanceLog;

        [QS.CMS.Diagnostics.Component("Core")]
        [QS.CMS.QS._core_c_.Diagnostics2.Module("Core")]
        private QS.CMS.Core.Core core;

        private QS.Fx.QS.Fx.Clock.IClock clock;

        private QS.CMS.Devices3.Network network;
        private QS.CMS.Devices3.UDPCommunicationsDevice udpdevice;
        private QS.CMS.Devices3.IListener listener;
        // private QS.CMS.Connections.MulticastResponder multicastResponder;

        private QS.CMS.Rings6.ReceivingAgent receivingAgentClass;
        [QS.CMS.Diagnostics.Component("Regional Controller")]
        [QS.CMS.QS._core_c_.Diagnostics2.Module("RegionalController")]
        private QS.CMS.Receivers4.RegionalController regionalController;
        [QS.CMS.Diagnostics.Component("Regional Senders")]
        [QS.CMS.QS._core_c_.Diagnostics2.Module("RegionalSenders")]
        private QS.CMS.Senders10.RegionalSenders regionalSenders;

        private QS.CMS.Base6.ICollectionOf<QS.Fx.Network.NetworkAddress,
            QS.CMS.Base6.ISink<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>> nodeSinkCollection;
        private QS.CMS.Base6.ICollectionOf<QS.CMS.Base3.RegionID,
            QS.CMS.Base6.ISink<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>> regionSinkCollection;
        [QS.CMS.QS._core_c_.Diagnostics2.Module("ReliableRegionViewSinks")]
        private QS.CMS.Base6.ICollectionOf<QS.CMS.Base3.RVID,
            QS.CMS.Base6.ISink<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>> regionViewSinkCollection;
        [QS.CMS.QS._core_c_.Diagnostics2.Module("ReliableGroupSinks")]
        private QS.CMS.Base6.ICollectionOf<QS.CMS.Base3.GroupID,
            QS.CMS.Base6.ISink<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>> groupSinkCollection;

        private QS.CMS.Multicasting7.DispatcherRV2 dispatcherRV2;

        private int nsenders, batching_buffersize;
#if DEBUG_DoNotUseScenarios
            private int ngroups, nregions;
            private QS.CMS.Base3.GroupID[] groupIDs, separator_groupIDs;
#endif
        private MySource[] mySources;
        private bool batching;

        [QS.CMS.Diagnostics.Ignore]
        [QS.TMS.Inspection.Ignore]
        private double[] sendTimes, completionTimes;
        private double[][] receiveTimes;

        private bool measure_latencies;

#if DEBUG_EnableStatistics
            [QS.CMS.Diagnostics.Component("Send Rates")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("SendRates")]
            private QS.CMS.Statistics.Samples2D sendRates;

            [QS.CMS.Diagnostics.Component("Total Sent")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("TotalSent")]
            private QS.CMS.Statistics.Samples2D totalSent;

            [QS.CMS.Diagnostics.Component("Completion Rates")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("CompletionRates")]
            private QS.CMS.Statistics.Samples2D completionRates;

            [QS.CMS.Diagnostics.Component("Total Completed")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("TotalCompleted")]
            private QS.CMS.Statistics.Samples2D totalCompleted;

            [QS.CMS.Diagnostics.Component("Total Received Overall")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("TotalReceivedOverall")]
            private QS.CMS.Statistics.Samples2D totalReceivedOverall;

            [QS.CMS.Diagnostics.Component("Received Rate Overall")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("ReceivedRateOverall")]
            private QS.CMS.Statistics.Samples2D receivedRateOverall;

            [QS.CMS.Diagnostics.Component("Pending Completion")]
            [QS.CMS.QS._core_c_.Diagnostics2.Property("PendingCompletion")]
            private QS.CMS.Statistics.Samples2D pendingCompletion;

            // no diagnostics here, these guys are registered explicitly in the constructor
            [QS.CMS.Diagnostics.ComponentCollection("Receive Rates")]
            private QS.CMS.Statistics.Samples2D[] receiveRates;
            [QS.CMS.Diagnostics.ComponentCollection("Total Received")]
            private QS.CMS.Statistics.Samples2D[] totalReceived;
#endif

        private double lastChecked;
        private int last_nsent, last_ncompleted, last_overall_nreceived;
        private int[] last_nreceived;
        // private double rate;
        private int messagesize, nmessages, nsent, ncompleted, senderIndex, overall_nreceived;
        private int[] nreceived;
        private bool detailed_timings, should_die, should_sleep;
        private double sleep_duration;

        private bool should_drop, drop_repeat;
        private double drop_interval, drop_duration;
        private int drop_count;

        [QS.TMS.Inspection.Inspectable("Collected Statistics", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
        private QS.CMS.Components.AttributeSet collected_statistics;

        private double badtime_coeff;
        private bool badtime_track;
        private int badtime_nmessages;
*/

        #endregion

        #endregion

        #region Parameter names

        private const string PARAMETER_FD_ENABLED = "fd_enabled";
        private const string PARAMETER_DEDICATED_GMS = "dedicated_gms";
        private const string PARAMETER_MTU = "mtu";
        private const string PARAMETER_GMS_GROUPALLOC = "gms_groupalloc";

        #endregion

        #region Constants

        private const int DefaultPortNumber = 12022;

/*
        // private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
        private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

        public static readonly QS.Fx.Network.NetworkAddress ClockServiceAddress =
            new QS.Fx.Network.NetworkAddress("224.81.80.79:63211");
*/

        #endregion

        #region IApplication.Start

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            this.platform = platform;
            this.context = context;

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            bool parameter_activate_fd = context.Arguments.ContainsKey(PARAMETER_FD_ENABLED) ? 
                Convert.ToBoolean(context.Arguments[PARAMETER_FD_ENABLED]) : false;

//            bool parameter_dedicated_gms = context.Arguments.ContainsKey(PARAMETER_DEDICATED_GMS) ? 
//                Convert.ToBoolean(context.Arguments[PARAMETER_DEDICATED_GMS]) : false;

            int parameter_mtu = context.Arguments.ContainsKey(PARAMETER_MTU) ? Convert.ToInt32(context.Arguments[PARAMETER_MTU]) : 0;

            bool parameter_gms_groupalloc = context.Arguments.ContainsKey(PARAMETER_GMS_GROUPALLOC) ? 
                Convert.ToBoolean(context.Arguments[PARAMETER_FD_ENABLED]) : false;

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            logger = platform.Logger;

            QS._core_c_.Base2.Serializer.CommonSerializer.registerClasses(logger);

            IPAddress localIPAddress = platform.Network.Interfaces[0].InterfaceAddress;
            localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, DefaultPortNumber);
            logger.Log("Base Address Chosen : " + localAddress.ToString());

            coordinatorAddress = new QS.Fx.Network.NetworkAddress(
                platform.Network.GetHostEntry(context.NodeNames[0]).AddressList[0], DefaultPortNumber);
            isCoordinator = localAddress.Equals(coordinatorAddress);
            logger.Log(isCoordinator ? "Acting as Coordinator." : ("Coordinator Address : " + coordinatorAddress.ToString() + "."));
            
            incarnation = QS._core_c_.Base3.Incarnation.Current;
            logger.Log("Application starting up now, incarnation " + incarnation.ToString() + ".");
            
#pragma warning disable 0162
            // some check on ifdefs
            if (QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.Agent.ProcessingCrashes
                || QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.AgentCore.ProcessingCrashes)
            {
                throw new Exception("Crahs processing ifdefs are inconsistent.");
            }
#pragma warning restore 0162

/*
            this.appno = Convert.ToInt32((string)args["_appno"]);
            experimentPath = (string)args["experimentPath"] + "\\Process_" + appno.ToString();
            repository_root = experimentPath;
            repository_key = "Incarnation_" + incarnation.ToString();
            logger.Log(this, "Experiment Path : \"" + experimentPath + "\"");

            QS.GUI.Components.RepositorySubmit1.DefaultPath = new string[] { 
                this.GetType().DeclaringType.Name, 
                (string) args["_timestamp"], 
                System.Net.Dns.GetHostName() + "_app" + (string) args["_appno"], "" }; 

            monitoring_activity = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "monitoring_activity");
            monitoring_interval = QS.TMS.Experiment.Helpers.Args.DoubleOf(args, "monitoring_interval", 10);
*/

            framework = new QS._qss_c_.Framework_1_.Framework(
                new QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress, platform,
                null, // statistics controller is needed here........ such as core.StatisticsController
                parameter_activate_fd, parameter_mtu, parameter_gms_groupalloc);

            #region (other shit)

/*
            performanceLog = new QS.CMS.Diagnostics3.PerformanceLog(framework.Clock, framework.AlarmClock, 1);

            if (QS.TMS.Experiment.Helpers.Args.BoolOf(args, "measure_totalcpu"))
                performanceLog.AddCounter("Processor", "_Total", "% Processor Time");

            if (QS.TMS.Experiment.Helpers.Args.BoolOf(args, "diagnose_nodes"))
            {
                performanceLog.AddCounter("Processor", "_Total", "% Processor Time");
                performanceLog.AddCounter("Processor", "_Total", "Interrupts/sec");
                performanceLog.AddCounter("System", "", "System Calls/sec");
                performanceLog.AddCounter("System", "", "File Data Operations/sec");
                performanceLog.AddCounter("System", "", "Context Switches/sec");
                performanceLog.AddCounter("UDPv4", "", "Datagrams Received Errors");
                performanceLog.AddCounter("PhysicalDisk", "_Total", "Disk Transfers/sec");
                performanceLog.AddCounter("Network Interface", null, "Packets Received Discarded");
                performanceLog.AddCounter("Network Interface", null, "Packets Received Errors");
                performanceLog.AddCounter("Memory", "", "Page Faults/sec");
                performanceLog.AddCounter("IPv4", "", "Datagrams Received Discarded");
                performanceLog.AddCounter(".NET CLR Exceptions", "_Global_", "# of Exceps Thrown / sec");
            }

/-*
            performanceLog.AddCounter("Network Interface", null, null);
            performanceLog.AddCounter(".NET CLR Networking", "_global_", null);
            performanceLog.AddCounter(".NET CLR Memory", "_Global_", "% Time in GC");                
*-/ 

            network = new QS.CMS.Devices3.Network(logger, 20000);
            udpdevice = (QS.CMS.Devices3.UDPCommunicationsDevice) 
                network[localAddress.HostIPAddress][QS.CMS.Devices3.CommunicationsDevice.Class.UDP];
            listener = udpdevice.ListenAt(ClockServiceAddress, this);

            badtime_track = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "badtime_track", false);
            badtime_coeff = QS.TMS.Experiment.Helpers.Args.DoubleOf(args, "badtime_coeff", 0.5);

            // multicastResponder = new QS.CMS.Connections.MulticastResponder(platform, 
            //    new QS.CMS.Base3.Now(framework.Clock, new QS.CMS.QS._core_c_.Base3.InstanceID(localAddress, incarnation)), 
            //    new QS.Fx.Network.NetworkAddress(ClockServiceAddress));

            if (args.contains("gms_batching_time") && framework.MembershipServer != null)
                framework.MembershipServer.RequestAggregationInterval = Convert.ToDouble((string)args["gms_batching_time"]);

            if (args.contains("fd_heartbeat_maxmissed") && framework.FailureDetectionServer != null)
                framework.FailureDetectionServer.MaximumMissed = Convert.ToInt32((string)args["fd_heartbeat_maxmissed"]);

            if (args.contains("fd_heartbeat_timeout") && framework.FailureDetectionServer != null)
                framework.FailureDetectionServer.HeartbeatTimeout = 
                    TimeSpan.FromSeconds(Convert.ToDouble((string)args["fd_heartbeat_timeout"]));

            if (args.contains("fd_heartbeat_interval") && framework.FailureDetectionAgent != null)
                framework.FailureDetectionAgent.HeartbeatInterval =
                    TimeSpan.FromSeconds(Convert.ToDouble((string)args["fd_heartbeat_interval"]));

//                if (args.contains("fd_enabled") && framework.FailureDetectionAgent != null)
//                    framework.FailureDetectionAgent.Enabled = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "fd_enabled", true);

            if (args.contains("drain_unicast"))
                framework.Root.DefaultDrainSynchronouslyForUnicastAddresses = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "drain_unicast");

            if (args.contains("drain_multicast"))
                framework.Root.DefaultDrainSynchronouslyForMulticastAddresses = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "drain_multicast");

            if (args.contains("nbuffers_unicast"))
                framework.Root.DefaultNumberOfReceiveBuffersForUnicastAddresses = Convert.ToInt32((string)args["nbuffers_unicast"]);

            if (args.contains("nbuffers_multicast"))
                framework.Root.DefaultNumberOfReceiveBuffersForMulticastAddresses = Convert.ToInt32((string)args["nbuffers_multicast"]);

            if (args.contains("adf_rcvbuf_unicast"))
                framework.Root.DefaultAfdReceiveBufferSizeForUnicastAddresses = Convert.ToInt32((string)args["adf_rcvbuf_unicast"]);

            if (args.contains("adf_rcvbuf_multicast"))
                framework.Root.DefaultAfdReceiveBufferSizeForMulticastAddresses = Convert.ToInt32((string)args["adf_rcvbuf_multicast"]);

            if (args.contains("adf_sndbuf_unicast"))
                framework.Root.DefaultAfdSendBufferSizeForUnicastAddresses = Convert.ToInt32((string)args["adf_sndbuf_unicast"]);

            if (args.contains("adf_sndbuf_multicast"))
                framework.Root.DefaultAfdSendBufferSizeForMulticastAddresses = Convert.ToInt32((string)args["adf_sndbuf_multicast"]);

            if (args.contains("alarm_quantum"))
                core.MaximumQuantumForAlarms = Convert.ToDouble((string)args["alarm_quantum"]);

            if (args.contains("io_quantum"))
                core.MaximumQuantumForCompletionPorts = Convert.ToDouble((string)args["io_quantum"]);

            if (args.contains("default_unicast_rate"))
                core.DefaultMaximumSenderUnicastRate = Convert.ToDouble((string)args["default_unicast_rate"]);

            if (args.contains("default_multicast_rate"))
                core.DefaultMaximumSenderMulticastRate = Convert.ToDouble((string)args["default_multicast_rate"]);

            if (args.contains("fc_unicast_credits"))
                core.DefaultMaximumSenderUnicastCredits = Convert.ToDouble((string)args["fc_unicast_credits"]);

            if (args.contains("fc_multicast_credits"))
                core.DefaultMaximumSenderMulticastCredits = Convert.ToDouble((string)args["fc_multicast_credits"]);
            
            if (args.contains("unicast_sender_cc"))
                framework.Root.DefaultUnicastSenderConcurrency = Convert.ToInt32((string)args["unicast_sender_cc"]);

            if (args.contains("multicast_sender_cc"))
                framework.Root.DefaultMulticastSenderConcurrency = Convert.ToInt32((string)args["multicast_sender_cc"]);

            core.ContinueIOOnTimeWarps = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "timewarps_continueio");

            if (args.contains("core_maxcc"))
                core.MaximumConcurrency = Convert.ToInt32((string)args["core_maxcc"]);

            if (args.contains("core_mintx"))
                core.MinimumTransmitted = Convert.ToInt32((string)args["core_mintx"]);

            if (args.contains("core_maxtx"))
                core.MaximumTransmitted = Convert.ToInt32((string)args["core_maxtx"]);

            batching = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "batching");
            if (batching)
                batching_buffersize = Convert.ToInt32((string)args["batching_buffersize"]);

/-*
            if (args.contains("def_rccls_unicast"))
                framework.Core.DefaultUnicastRateControllerClass = GetRateControllerClass((string)args["def_rccls_unicast"]);

            if (args.contains("def_rccls_multicast"))
                framework.Core.DefaultMulticastRateControllerClass = GetRateControllerClass((string)args["def_rccls_multicast"]);
*-/

            double rate;
            if (args.contains("rate"))
                rate = Convert.ToDouble((string)args["rate"]);
            else
                rate = double.PositiveInfinity;

            // framework.Core.DefaultMaximumSenderRate = rate;

            if (QS.TMS.Experiment.Helpers.Args.BoolOf(args, "gui"))
                AppController.Show("Experiment 263 App Controller", this);

            framework.Demultiplexer.register(myloid, new QS.CMS.Base3.ReceiveCallback(ReceiveCallback));

            messagesize = Convert.ToInt32(args["size"]);
            if (messagesize < sizeof(uint))
                throw new Exception("Message size too small.");
            nmessages = Convert.ToInt32(args["count"]);
            badtime_nmessages = (int) Math.Floor(((double)nmessages) * badtime_coeff);

            nsenders = Convert.ToInt32((string) args["nsenders"]);

            QS.CMS.Rings6.RateSharingAlgorithmClass rateSharingAlgorithm = (QS.CMS.Rings6.RateSharingAlgorithmClass) 
                Enum.Parse(typeof(QS.CMS.Rings6.RateSharingAlgorithmClass), (string)args["rs_ratesharing"], true);

            receivingAgentClass = new QS.CMS.Rings6.ReceivingAgent(
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

            receivingAgentClass.PullCaching = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "rs_pullcaching");
            receivingAgentClass.NaksAllowed = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "rs_naks");

            receivingAgentClass.ForwardingAllowed = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "rs_forwarding", false);

            if (args.contains("rs_max_window"))
                receivingAgentClass.MaximumWindowWidth = (uint)Convert.ToInt32((string)args["rs_max_window"]);

            if (args.contains("rs_max_naks"))
                receivingAgentClass.MaximumNakRangesPerToken = (uint)Convert.ToInt32((string)args["rs_max_naks"]);

            regionalController = new QS.CMS.Receivers4.RegionalController(
                framework.LocalAddress, framework.Logger, framework.Demultiplexer, framework.AlarmClock, framework.Clock,
                framework.MembershipController, receivingAgentClass, receivingAgentClass);
            regionalController.IsDisabled = false;

            bool buffering_unrecognized = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "buffering_unrecognized");
            int maximum_unrecognized = QS.TMS.Experiment.Helpers.Args.Int32Of(args, "maximum_unrecognized", 1000);
            double unrecognized_timeout = QS.TMS.Experiment.Helpers.Args.DoubleOf(args, "unrecognized_timeout", 5);

            regionalSenders = new QS.CMS.Senders10.RegionalSenders(
                framework.EventLogger, framework.LocalAddress, framework.Logger, framework.AlarmClock, framework.Clock, 
                framework.Demultiplexer, null, // this null argument needs to be fixed 
                regionalController, regionalController, 60, regionalController, 
                buffering_unrecognized, maximum_unrecognized, unrecognized_timeout); 

            dispatcherRV2 = new QS.CMS.Multicasting7.DispatcherRV2(logger, framework.AlarmClock, framework.Clock,
                framework.Demultiplexer, (uint) QS.ReservedObjectID.Multicasting7_DispatcherRV2, regionalController, 
                framework.MembershipController); 

#if DEBUG_DoNotUseScenarios
            ngroups = args.contains("ngroups") ? Convert.ToInt32(args["ngroups"]) : 1;
            nregions = args.contains("nregions") ? Convert.ToInt32(args["nregions"]) : 1;

            groupIDs = new QS.CMS.Base3.GroupID[ngroups];
            for (int ind = 0; ind < groupIDs.Length; ind++)
                groupIDs[ind] = new QS.CMS.Base3.GroupID((uint)(1000 + ind));
            separator_groupIDs = new QS.CMS.Base3.GroupID[nregions];
            for (int ind = 0; ind < separator_groupIDs.Length; ind++)
                separator_groupIDs[ind] = new QS.CMS.Base3.GroupID((uint)(1001000 + ind));
#endif

            detailed_timings = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "detailed_timings");

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

/-*
                bool should_adjust = param_adjust_rate.Equals("yes");
                QS.CMS.FlowControl3.IEstimatorClass rateEstimatorClass;
                if (should_adjust)
                {
//                        rateEstimatorClass = new QS.CMS.FlowControl3.MovingAverageEstimator(
//                            10, Convert.ToDouble((string)args["rs_multiplier"]), Convert.ToDouble((string)args["rs_maxinc"]), 100, 20000);
                }
*-/

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

            #endregion
        }

        #endregion

        #region IApplication.Stop

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
        }

        #endregion
    }
}
