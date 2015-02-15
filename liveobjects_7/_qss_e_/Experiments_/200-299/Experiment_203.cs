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

// #define DEBUG_Experiment_203
// #define DEBUG_IndividualizePackets

// #define DEBUG_EnableStatistics

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace QS._qss_e_.Experiments_ 
{
    [QS._qss_e_.Base_1_.Arguments(

        "-stayon:yes -stabilize:15 -cooldown:15 -fwd:off -fwdcaching:off -maxwin:1000 -sync_clocks:off -rec:1 -wrapper:yes " +
        "-nmessages:10000 -nofc:yes -rate:5000 -maxnaks:100 -burst:1000000 -msgsize:1000 -allsend:no -ngroups:1 -fcburst:100 " +
        "-buffersize:25000 -batching:no -senderclass:600 -token:5 -replication:5 -timeout:2 -download:no -save:no")]

//        "-stayon:yes -stabilize:5 -cooldown:5 -fwd:on -fwdcaching:on -maxwin:1000 -sync_clocks:off " +
//        "-nmessages:20000 -rate:5000 -maxnaks:100 -burst:25000 -msgsize:100 -allsend:no -ngroups:1 -fcburst:10 " +
//        "-buffersize:20000 -batching:no -senderclass:503 -token:5 -replication:5 -timeout:2 -download:no -save:yes")]

//        "-stayon:yes -stabilize:100 -cooldown:20 " +
//        "-nmessages:100 -rate:500 -tokensize:100 -burst:5000 -msgsize:100 -allsend:no -ngroups:1 -fcburst:10 " +
//        "-buffersize:20000 -batching:no -senderclass:503 -token:5 -replication:5 -timeout:2")]

//        "-stayon:yes -stabilize:20 -cooldown:20 " +
//        "-nmessages:100000 -rate:500 -tokensize:100 -burst:5000 -msgsize:100 -allsend:no -ngroups:1 -fcburst:10 " +
//        "-buffersize:20000 -batching:no -senderclass:502 -token:5 -replication:5 -timeout:2")]

//        "-stayon:yes -stabilize:20 -cooldown:20 " + 
//        "-nmessages:1 -msgsize:100 -allsend:no -ngroups:1 -burst:1 -fcburst:10 -buffersize:1000 -batching:no " +
//        "-senderclass:503 -token:5 -replication:5 -timeout:2 -rate:1000 -tokensize:10")]

//        "-stayon:yes -stabilize:100 -cooldown:20 -gms:simple " + 
//        "-nmessages:5000 -allsend:no -ngroups:100 -burst:10000 -buffersize:100 " +
//        "-senderclass:200 -token:0.001 " +
//        "-windowsize:1 -timeout:0.05 -routing:log -fc:window -fwd:off -ack_channel:root -flushing_interval:0.005 ")]

//        "-stayon:yes -stabilize:100 -cooldown:20 -gms:overlapping " + 
//        "-nmessages:50000 -allsend:no -ngroups:100 -burst:10000 -buffersize:2000 " +
//        "-senderclass:501 -rate:1000 -rrvs_timeout:0.3 -token:10 -batching:off " +
//        "-windowsize:1 -timeout:0.05 -routing:no -fc:window -fwd:off -ack_channel:root -flushing_interval:0.005 ")]

//        "-stayon:yes -stabilize:100 -cooldown:20 -gms:overlapping -token:0.01 " + 
//        "-nmessages:1000 -allsend:yes -ngroups:1 -burst:5000 -buffersize:5000 " +
//        "-senderclass:200 -windowsize:1 -timeout:0.05 -routing:no -fc:window -fwd:off " +
//        "-ack_channel:root -flushing_interval:0.005 ")]

//       "-nmessages:200000 -burst:40000 -senderclass:501 -buffersize:40000 -rate:500 -rrvs_timeout:0.5 " +
//       "-token:10 -stayon:yes -stabilize:20 -cooldown:20")]

//       "-nmessages:25000 -stayon:yes -crashonce:yes -senderclass:200 -routing:btree,2 -ack_channel:lazy -fc:window -buffersize:1000")]
//       "-nnodes:20 -time:1000 -stayon -ngroups:1 -nmessages:1000 " +
//       "-senderclass:202 -windowsize:1 -timeout:0.05 -routing:btree,2 -buffersize:5000 -fc:window -fwd:on -ack_channel:root")]

//       "-nnodes:2 -time:1000 -mttf:500 -downtime:5 -ngroups:1")]

    // [TMS.Experiments.DefaultExperiment]
    public class Experiment_203 : Experiment_200
    {
        private const string DefaultRepositoryPath = "C:\\.QuickSilver\\.Repository";

        public Experiment_203()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            double time = arguments.contains("time") ? Convert.ToDouble((string) arguments["time"]) : 0;
            int nmessages = Convert.ToInt32((string) arguments["nmessages"]);
            this.results = results;

            crashOnce = arguments.contains("crashonce");

            if (arguments.contains("sync_clocks") && arguments["sync_clocks"].Equals("on"))
            {
                logger.Log(this, "Synchronizing clocks....");
                for (int ind = 1; ind < this.NumberOfApplications; ind++)
                {
                    this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("SynchronizeClocks"), new object[] { 100 });
                    sleeper.sleep(0.5);
                }
            }

            logger.Log(this, "Changing membership....");
            for (int ind = 0; ind < this.NumberOfApplications; ind++)
            {
                this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("ChangeMembership"), new object[] { });
            }

            double stabilize_time = arguments.contains("stabilize") ? Convert.ToDouble((string) arguments["stabilize"]) : 5;
            logger.Log(this, "Waiting " + stabilize_time.ToString() + " seconds for the system to stabilize.");
            sleeper.sleep(stabilize_time);

            int burst_size = ((arguments.contains("burst")) ? Convert.ToInt32((string)arguments["burst"]) : 0);

            logger.Log(this, "Starting to multicast messages....");

            bool allsend = (arguments.contains("allsend") && arguments["allsend"].Equals("yes"));
            if (allsend)
            {
                logger.Log(this, "All nodes sending.");
                for (int ind = 0; ind < this.NumberOfApplications; ind++)
                    this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("Send"), new object[] { nmessages, burst_size });
                completionsToGo = this.NumberOfApplications;
            }
            else
            {
                logger.Log(this, "Only coordinator sending.");
                Coordinator.invoke(typeof(Application).GetMethod("Send"), new object[] { nmessages, burst_size });
                completionsToGo = 1;
            }

            // sleeper.sleep(time);
            experimentCompletedEvent.WaitOne();

            if (arguments.contains("cooldown"))
            {
                logger.Log(this, "Cooling down....");
                sleeper.sleep(Convert.ToDouble((string)arguments["cooldown"]));
            }

            if (arguments.contains("download") && (arguments["download"].Equals("on") || arguments["download"].Equals("yes")))
            {
                logger.Log(this, "Downloading application statistics...");

                QS._core_c_.Components.AttributeSet applicationStatistics = new QS._core_c_.Components.AttributeSet();
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                {
                    logger.Log(this, "Downloading from " + application.AppID.ToString());
                    object obj = application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { true });
                    if (obj != null && obj is QS._core_c_.Components.Attribute)
                    {
                        QS._core_c_.Components.Attribute statistics = (QS._core_c_.Components.Attribute)obj;
                        applicationStatistics.Add(statistics);
                    }
                }

                QS._core_c_.Components.Attribute appStats = 
                    new QS._core_c_.Components.Attribute("Application Statistics", applicationStatistics);
                results["Application Statistics"] = appStats;
            }
            else
            {
                foreach (QS._qss_e_.Runtime_.IApplicationRef application in this.Applications)
                    application.invoke(typeof(Application).GetMethod("GetStatistics"), new object[] { false });
            }

            if (arguments.contains("save") && (arguments["save"].Equals("yes") || arguments["save"].Equals("on")))
            {
                QS._core_e_.Repository.IRepository repository = new QS._qss_e_.Repository_.Repository(DefaultRepositoryPath);
                if (repository != null)
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        string elementName = "Results_" + now.ToString("yyyyMMddHHmmssfff");

                        QS._core_c_.Components.AttributeSet _attributeSet = new QS._core_c_.Components.AttributeSet();
                        _attributeSet["Time Stamp"] = now.ToString("MM/dd/yyyy HH:mm");
                        _attributeSet["Experiment Class"] = this.GetType().ToString();
                        _attributeSet["Arguments"] = arguments;
                        _attributeSet["Collected Results"] = results;
                        repository.Add(elementName, _attributeSet);

                        logger.Log(this, "Results saved as \"" + elementName + "\".");

                        // results["saved"] = "http://" + environment. elementName
                    }
                    catch (Exception exc)
                    {
                        logger.Log(this, "Could not save results.\n" + exc.ToString());
                    }
                }
            }
        }

        #endregion

        private int completionsToGo;
        private List<QS._core_c_.Base3.InstanceID> completedAddresses = new List<QS._core_c_.Base3.InstanceID>();

        #region experimentCompleted

        private System.Threading.ManualResetEvent experimentCompletedEvent = new System.Threading.ManualResetEvent(false);
        private QS._core_c_.Components.IAttributeSet results;
        public void experimentCompleted(QS._core_c_.Components.AttributeSet args)
        {
            lock (this)
            {
                QS._core_c_.Base3.InstanceID completedAddress = (QS._core_c_.Base3.InstanceID) args["_iid"];
                if (!completedAddresses.Contains(completedAddress))
                {
                    logger.Log(this, "Completed: " + completedAddress.ToString());

                    completedAddresses.Add(completedAddress);
                    completionsToGo--;

                    QS._core_c_.Components.AttributeSet thisGuyAttributes = new QS._core_c_.Components.AttributeSet();
                    results["Results from " + completedAddress.ToString()] = thisGuyAttributes;

                    args.remove("_iid");
                    System.Collections.Generic.IEnumerator<QS._core_c_.Components.Attribute> attrib_enum = 
                        ((QS._core_c_.Components.IAttributeSet)args).Attributes;
                    while (attrib_enum.MoveNext())
                    {
                        QS._core_c_.Components.Attribute attrib = attrib_enum.Current;
                        thisGuyAttributes[attrib.Name] = attrib.Value;
                    }

                    if (completionsToGo == 0)
                        experimentCompletedEvent.Set();
                }
            }
        }

        #endregion

        #region percentageCompleted

        bool crashOnce = false, alreadyCrashed = false;
        public void percentageCompleted(QS._core_c_.Components.AttributeSet args)
        {
            QS._core_c_.Base3.InstanceID iid = (QS._core_c_.Base3.InstanceID)args["_iid"];
            double percentage = (double) args["percentage"];
            logger.Log(null, "Completed on " + iid.ToString() + " : So far " + percentage.ToString("00.00") + "%");

            bool scheduleCrash = false;
            lock (this)
            {
                if (crashOnce && !alreadyCrashed && percentage > 50)
                    scheduleCrash = alreadyCrashed = true;
            }

            if (scheduleCrash)
            {
                logger.Log(null, "Scheduling crash.");
                environment.AlarmClock.Schedule(0, new QS.Fx.Clock.AlarmCallback(delegate(QS.Fx.Clock.IAlarm alarmRef)
                {
                    logger.Log(null, "Crashing single node.");
                    NodeController(1).Crash(true);
                }), null);
            }
        }

        #endregion

        #region Class Application

        [QS._core_c_.Diagnostics.ComponentContainer]
        protected new class Application : Experiment_200.Application
        {
            private const uint myloid = 1000;
            private const int defaultOutgoingBufferSize = 20000;
            private const int CLIENT_MAX_BURST_SIZE = defaultOutgoingBufferSize;

            private const int ServerClockSynchronizationPort = 12987;

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "wrapper"))
                    QS._qss_c_.Framework_1_.SimpleFramework.ReplaceDevices6SinkWithDevices4AsynchronousSender = true;

                QS._qss_c_.Framework_1_.SimpleFramework.ReceiverType receiverType;
                switch (Convert.ToInt32((string)args["rec"]))
                {
                    case 1:
                        receiverType = QS._qss_c_.Framework_1_.SimpleFramework.ReceiverType.Base3;
                        break;

                    case 2:
                        receiverType = QS._qss_c_.Framework_1_.SimpleFramework.ReceiverType.Base7;
                        break;

                    default:
                        throw new Exception("Unknown receiver type.");
                }

                framework = new QS._qss_c_.Framework_1_.SimpleFramework(
                    incarnation, false, platform, localAddress, coordinatorAddress, ((args.contains("gms") && args["gms"].Equals("simple")) 
                    ? QS._qss_c_.Membership2.Servers.Type.REGULAR : QS._qss_c_.Membership2.Servers.Type.OVERLAPPING), receiverType);
                framework.Demultiplexer.register(myloid, new QS._qss_c_.Base3_.ReceiveCallback(ReceiveCallback));
                
                // old stuff.........

                int windowSize = args.contains("windowsize") ? Convert.ToInt32((string)args["windowsize"]) : 1;
                double retransmissionTimeout = args.contains("timeout") ? Convert.ToDouble((string)args["timeout"]) : 0.05;

                framework.LazySender.FlushingInterval = TimeSpan.FromSeconds(
                    args.contains("flushing_interval") ? Convert.ToDouble((string)args["flushing_interval"]) : 0.005);

                if (args.contains("buffering_threshold"))
                {
                    string threshold_string = (string)args["buffering_threshold"];
                    int slash_position = threshold_string.IndexOf('/');
                    double threshold_value = Convert.ToDouble(threshold_string.Substring(0, slash_position));
                    double reference_interval = Convert.ToDouble(threshold_string.Substring(slash_position + 1));
                    platform.Logger.Log(this, "Setting threshold to " + threshold_value.ToString() + " / " + reference_interval.ToString());
                    framework.ThresholdSender.SetThreshold(reference_interval, threshold_value);
                }

                QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender> acknowledgementSenderClass;
                string ackchannel_name = args.contains("ack_channel") ? (string)args["ack_channel"] : null;
                if (ackchannel_name == null || ackchannel_name.Equals("root"))
                    acknowledgementSenderClass = (QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>)framework.RootSender;
                else if (ackchannel_name.Equals("lazy"))
                {
                    acknowledgementSenderClass = (QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>)framework.LazySender;
                }
                else if (ackchannel_name.Equals("threshold"))
                {
                    acknowledgementSenderClass = (QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>)framework.ThresholdSender;
                }
                else
                    throw new ArgumentException("Acknowledgement channel type unknown.");

                QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> ackISC =
                    new QS._qss_c_.Senders6.InstanceSender(
                        platform.Logger, framework.FailureDetector, acknowledgementSenderClass.SenderCollection, null);

                framework.AggregationAgent.UnderlyingSenderCollection = ackISC; //  acknowledgementSenderClass.SenderCollection;

                ((QS._qss_c_.Senders3.ReliableSender1)framework.ReliableSender2).UnderlyingSenderCollection =
                    acknowledgementSenderClass.SenderCollection;

                QS._qss_c_.Routing_1_.IRoutingAlgorithm routingAlgorithm;
                string algorithm_name = args.contains("routing") ? (string)args["routing"] : null;
                if (algorithm_name == null || algorithm_name.Equals("no"))
                    routingAlgorithm = QS._qss_c_.Routing_1_.NoRouting.Algorithm;
                else if (algorithm_name.Equals("ring"))
                    routingAlgorithm = QS._qss_c_.Routing_1_.RingRouting.Algorithm;
                else if (algorithm_name.StartsWith("prefix"))
                {
                    uint routing_base = Convert.ToUInt32(algorithm_name.Substring(algorithm_name.IndexOf(",") + 1));
                    routingAlgorithm = QS._qss_c_.Routing_1_.PrefixRouting.Algorithm(routing_base);
                }
                else if (algorithm_name.StartsWith("btree"))
                {
                    uint routing_base = Convert.ToUInt32(algorithm_name.Substring(algorithm_name.IndexOf(",") + 1));
                    routingAlgorithm = QS._qss_c_.Routing_1_.BalancedTreeRouting.Algorithm(routing_base);
                }
                else if (algorithm_name.StartsWith("log"))
                {
                    routingAlgorithm = new QS._qss_c_.Routing_1_.LogRouting(); // no base needs to be specified
                }
                else
                    throw new ArgumentException("Unknown routing algorithm");

                QS._qss_c_.Aggregation1_.IAggregationClass aggregationClass = 
                    new QS._qss_c_.Multicasting3.AggregationClass(platform.Logger,  framework.MembershipController, routingAlgorithm);
                framework.AggregationAgent.registerClass(aggregationClass);

                framework.AggregationRouter.RoutingAlgorithm = routingAlgorithm;
                ((QS._qss_c_.Aggregation4_.IAgent)framework.Aggregation4Agent).RoutingAlgorithm = routingAlgorithm;

                int sender_type = Convert.ToInt32(args["senderclass"] as string);
                switch (sender_type)
                {
                    case 100:
                    {
                        simpleSender = new QS._qss_c_.Multicasting3.SimpleSender1(framework.Platform.Logger, framework.MembershipController,
                            framework.Demultiplexer, framework.RootSender.SenderCollection, framework.AggregationAgent, platform.Clock, 
                            platform.AlarmClock, retransmissionTimeout, defaultOutgoingBufferSize, windowSize);
                    }
                    break;

                    case 200:
                    case 201:
                    case 202:
                    {
                        int buffersize = defaultOutgoingBufferSize;
                        if (args.contains("buffersize"))
                            buffersize = Convert.ToInt32(args["buffersize"]);

                        QS._qss_c_.Aggregation1_.IAggregationAgent aggregationAgent = null;
                        switch (sender_type)
                        {
                            case 200:
                            aggregationAgent = framework.AggregationAgent;
                            break;

                            case 201:
                            aggregationAgent = framework.Aggregation3Agent;
                            break;

                            case 202:
                            aggregationAgent = framework.Aggregation4Agent;
                            break;
                        }

                        QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme fcscheme;
                        if (args.contains("fc"))
                        {
                            if (args["fc"].Equals("window"))
                                fcscheme = QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme.WINDOW;
                            else if (args["fc"].Equals("rate"))
                                fcscheme = QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme.RATE;
                            else
                                throw new ArgumentException();
                        }
                        else
                            fcscheme = QS._qss_c_.Multicasting3.SimpleSender2.RegionController.FlowControlScheme.WINDOW;

                        simpleSender = new QS._qss_c_.Multicasting3.SimpleSender2(framework.Platform.Logger,
                            framework.MembershipController, aggregationAgent, framework.Platform.Clock,
                            framework.Platform.AlarmClock, framework.Demultiplexer, framework.RootSender.SenderCollection,
                            framework.UnreliableInstanceSenderCollection, retransmissionTimeout, buffersize, buffersize, windowSize, fcscheme);
                    }
                    break;

                    case 300:
                    {
                        framework.RegionSender.InitialWindowSize = windowSize;
                        simpleSender = new QS._qss_c_.Multicasting3.SimpleSender3(
                            framework.Platform.Logger, framework.MembershipController, framework.RegionSender,
                            framework.AggregationAgent, // framework.Aggregation3Agent, 
                            framework.Platform.Clock, framework.Platform.AlarmClock, framework.Demultiplexer);
                    }
                    break;

                    case 500:
                    case 501:
                    case 502:
                    case 503:
                    {
                        int buffersize = defaultOutgoingBufferSize;
                        if (args.contains("buffersize"))
                            buffersize = Convert.ToInt32(args["buffersize"]);

                        QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base3_.IReliableSerializableSender> rvidSenders = null;
                        switch (sender_type)
                        {
                            case 500: 
                                rvidSenders = framework.SimpleURVS; 
                                break;

                            case 501: 
                                rvidSenders = framework.GossipingRRVS; 
                                break;

                            case 502:
                                rvidSenders = framework.RingRRVS;
                                framework.RingRRVS.RetransmissionTimeout = retransmissionTimeout;
                                framework.RingRRVS.IsDisabled = false;
                                break;

                            case 503:
                                rvidSenders = framework.RRVS10;
                                framework.Receivers4_RegionalController.IsDisabled = false;
                                break;
                        }

                        simpleSender = new QS._qss_c_.Multicasting5.DelegatingGS(framework.Platform.Logger, 
                            framework.Platform.Clock,framework.MembershipController, rvidSenders, buffersize); 
                    }
                    break;

                    case 600:
                    {
                        QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinks = null;
                        switch (sender_type)
                        {
                            case 600:
                                sinks = framework.PlaceholderGSs;
                                framework.Receivers4_RegionalController.IsDisabled = false;
                                break;
                        }

                        simpleSender = new QS._qss_c_.Base6_.GroupSenders(sinks, platform.Clock);                        
                    }
                    break;

                    default:
                        throw new Exception("Sender class not specified or invalid.");
                }

                bool forwardingAllowed = args.contains("fwd") && args["fwd"].Equals("on");
                framework.Aggregation4Controller1.ForwardingAllowed = forwardingAllowed;
                framework.Rings6_ReceivingAgent1.ForwardingAllowed = forwardingAllowed;

                framework.FailureDetector.OnChange += new QS._qss_c_.FailureDetection_.ChangeCallback(FailureDetector_OnChange);

                groupIDs = new QS._qss_c_.Base3_.GroupID[args.contains("ngroups") ? Convert.ToInt32(args["ngroups"]) : 1];
                for (int ind = 0; ind < groupIDs.Length; ind++)
                    groupIDs[ind] = new QS._qss_c_.Base3_.GroupID((uint)(1000 + ind));

                platform.AlarmClock.Schedule(5, new QS.Fx.Clock.AlarmCallback(multicastRateCallback), null);

                if (args.contains("rate"))
                    framework.BufferingUNS.DefaultRate = Convert.ToDouble((string) args["rate"]);

                if (args.contains("rrvs_timeout"))
                    framework.GossipingRRVS.InitialRetransmissionTimeout = Convert.ToDouble((string) args["rrvs_timeout"]);

                if (args.contains("token"))
                {
                    double frequency = Convert.ToDouble((string)args["token"]);
                    framework.RegionalAgent.TokenRate = frequency;
                    framework.RingRRVS.TokenFrequency = frequency;
                    framework.Rings6_ReceivingAgent1.TokenRate = frequency;
                }

                if (args.contains("replication"))
                {
                    int replication = Convert.ToInt32((string)args["replication"]);
                    framework.RingRRVS.ReplicationCoefficient = replication;
                    framework.Rings6_ReceivingAgent1.ReplicationCoefficient = (uint) replication;
                }

                if (args.contains("batching") && 
                    (args["batching"].Equals("no") || args["batching"].Equals("off") || args["batching"].Equals("disabled")))
                    framework.BufferingUNS.BatchingEnabledByDefault = false;

                if (args.contains("msgsize"))
                    msgsize = Convert.ToInt32((string)args["msgsize"]);

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "nofc"))
                    framework.BufferingUNS.FlowControlEnabledByDefault = false;

                if (args.contains("fcburst"))
                    framework.BufferingUNS.BurstSize = Convert.ToInt32((string)args["fcburst"]);

                if (args.contains("tokensize"))
                    framework.RingRRVS.MaxNAKsPerToken = Convert.ToInt32((string)args["tokensize"]);

                if (args.contains("maxnaks"))
                    framework.Rings6_ReceivingAgent1.MaximumNakRangesPerToken = Convert.ToUInt32((string)args["maxnaks"]);

                framework.Rings6_ReceivingAgent1.PullCaching = (args.contains("fwdcaching") && args["fwdcaching"].Equals("on"));

                if (args.contains("maxwin"))
                    framework.Rings6_ReceivingAgent1.MaximumWindowWidth = Convert.ToUInt32((string)args["maxwin"]);

                if (platform is QS._qss_c_.Platform_.PhysicalPlatform)
                {
                    synchronizationAgent1 = new QS._qss_c_.Time_.Synchronization_.SynchronizationAgent1(
                        new QS.Fx.Network.NetworkAddress(localAddress.HostIPAddress, 
                            coordinatorAddress.Equals(localAddress) ? ServerClockSynchronizationPort : 0), platform.Clock);
                }
            }

            #endregion

            private QS._qss_c_.Time_.Synchronization_.SynchronizationAgent1 synchronizationAgent1;

            #region Failure Processing

            private void FailureDetector_OnChange(IEnumerable<QS._qss_c_.FailureDetection_.Change> changes)
            {
                logger.Log(this, "__FailureDetector_OnChange: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._qss_c_.FailureDetection_.Change>(changes, ","));
            }

            #endregion

            [QS._core_c_.Diagnostics.Component("Framework")]
            private QS._qss_c_.Framework_1_.SimpleFramework framework;
            private QS._qss_c_.Base3_.GroupID[] groupIDs;
            // private CMS.Base3.ISenderCollection<CMS.Base3.IReliableSerializableSender> senderCollection;
            [QS._core_c_.Diagnostics.Component("Group Sender")]
            private QS._qss_c_.Multicasting3.ISimpleSender simpleSender;
            private int msgsize = 100;

            #region SynchronizeClocks

            public void SynchronizeClocks(int nsamples)
            {
                throw new Exception("This functionality is disabled because the IClock interface no longer includes the Adjust() method.");

/*
                if (platform is CMS.Platform.PhysicalPlatform)
                {
                    CMS.Time.Synchronization.SynchronizationAgent1.Client client =
                        new QS.CMS.Time.Synchronization.SynchronizationAgent1.Client(localAddress.HostIPAddress,
                            new QS.Fx.Network.NetworkAddress(coordinatorAddress.HostIPAddress, ServerClockSynchronizationPort),
                                platform.Clock);

                    logger.Log(this, "Synchronizing clocks.");
                    for (int ind = 0; ind < 3; ind++)
                    {
                        double clock_drift = client.Synchronize(nsamples);
                        logger.Log(this, "Drift: " + clock_drift.ToString("000.000000000000"));
                        platform.Clock.Adjust(clock_drift);
                    }
                }
*/ 
            }

            #endregion

            #region ChangeMembership

            public void ChangeMembership()
            {
                framework.MembershipAgent.ChangeMembership(
                    new List<QS._qss_c_.Base3_.GroupID>(groupIDs), new List<QS._qss_c_.Base3_.GroupID>());
            }

            #endregion

            #region Send

            private const int ClientBatchSize = 1000;

            public void Send(int nmessages, int burstsize)
            {
                if (burstsize == 0)
                    burstsize = CLIENT_MAX_BURST_SIZE;

// #if DEBUG_Experiment_203
                logger.Log(this, "__Send(" + nmessages.ToString() + ", " + burstsize.ToString() +
                    ")_Enter______________________________");
// #endif

                this.nmessages = nmessages;

                asynchronousCallback = new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.completionCallback);

                byte[] messageBytes = new byte[msgsize];
                byte[] commentBytes = System.Text.Encoding.ASCII.GetBytes("A nice little message.");
                Buffer.BlockCopy(commentBytes, 0, messageBytes, 0,
                    (commentBytes.Length < messageBytes.Length) ? commentBytes.Length : messageBytes.Length);
                messageToSend = new QS._core_c_.Base2.BlockOfData(messageBytes);

                startingTimes = new double[nmessages];
                completionTimes = new double[nmessages];

                nmessages_sent = nmessages_received = 0;

                isSending = true;

                int client_concurrency = nmessages < burstsize ? nmessages : burstsize;

                SendBatch(client_concurrency);

                logger.Log(this, "__Send(" + nmessages.ToString() + ", " + burstsize.ToString() +
                    ")_Leave______________________________");
            }

            private void SendBatch(int client_concurrency)
            {
                int tosend_now = client_concurrency;
                if (ClientBatchSize < tosend_now)
                    tosend_now = ClientBatchSize;
                for (int ind = 0; ind < tosend_now; ind++)
                    send_message();

                int tosend_later = client_concurrency - tosend_now;

                if (tosend_later > 0)
                    platform.AlarmClock.Schedule(0, new QS.Fx.Clock.AlarmCallback(sendBatchCallback), tosend_later);
                else
                {
                    logger.Log(this, "____________Client_Initial_Burst_Sending_Complete____________________");
                }
            }

            private void sendBatchCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                SendBatch((int) alarmRef.Context);
            }

            #endregion

            private int nmessages;
            [QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
            private int nmessages_sent;
            [QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
            private int nmessages_received;
            private double[] startingTimes, completionTimes;
            private QS._qss_c_.Base3_.AsynchronousOperationCallback asynchronousCallback;
            private QS.Fx.Serialization.ISerializable messageToSend;
            private bool isSending = false, isCompleted = false;

            #region send_message

            private int send_message()
            {
                int message_ind = Interlocked.Increment(ref nmessages_sent) - 1;

                if (message_ind < nmessages)
                {
                    int group_ind = message_ind % groupIDs.Length;
                    startingTimes[message_ind] = platform.Clock.Time;

                    QS._qss_c_.Multicasting3.IGroupSender mysender = simpleSender[groupIDs[group_ind]];

                    // int current_windowSize = (int)(Math.Ceiling((message_ind * windowSize) / ((double) nmessages)));
                    // mysender.WindowSize = current_windowSize;

#if DEBUG_IndividualizePackets
                    mysender.BeginSend(myloid, new QS.CMS.Base2.StringWrapper("A nice little message (", message_ind.ToString(), ")."), 
                        asynchronousCallback, message_ind);
#else
                    mysender.BeginSend(myloid, messageToSend, asynchronousCallback, message_ind);
#endif
                }

                return message_ind;
            }

            #endregion

            #region multicastRateCallback

            private int nmessages_received_snapshot = int.MinValue;
            [QS._qss_c_.Logging_1_.IgnoreCallbacks]
            private void multicastRateCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                if (!isCompleted && isSending)
                {
                    int nmessages_received_now = nmessages_received;
                    bool trouble = nmessages_received_now == nmessages_received_snapshot;
                    nmessages_received_snapshot = nmessages_received_now;

                    if (trouble)
                        platform.Logger.Log(this, "Warning: Not multicasting!");
                    else
                    {
                        QS._core_c_.Components.AttributeSet attribset = new QS._core_c_.Components.AttributeSet(
                                "percentage", 100 * ((double)nmessages_received_now) / ((double)nmessages));
                        attribset["_iid"] = framework.InstanceID;
                        applicationController.upcall("percentageCompleted", attribset);
                    }
                }

                int this_delivered = nmessages_delivered;
                double this_time = platform.Clock.Time;
                if (last_checked > 0)
                {
                    double delivery_rate = (this_delivered - last_delivered) / (this_time - last_checked);
                    platform.Logger.Log(this, "Received: " + this_delivered.ToString("000000000") + 
                        ", Current_Rate: " + delivery_rate.ToString("0000.00"));
                }

                last_delivered = this_delivered;
                last_checked = this_time;

                alarmRef.Reschedule();
            }

            #endregion

            #region ReceiveCallback

            [QS.Fx.Base.Inspectable]
            private int nmessages_delivered = 0, last_delivered = 0;
            [QS.Fx.Base.Inspectable]
            private double last_checked = -1;
            private System.Object nmessages_delivered_lock = new System.Object();

#if DEBUG_EnableStatistics
            [QS.CMS.Diagnostics.Component]
            private QS.CMS.Statistics.Samples receiveTimes = new QS.CMS.Statistics.Samples();
#endif

            private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID,
                QS.Fx.Serialization.ISerializable receivedObject)
            {
                lock (nmessages_delivered_lock)
                {
                    nmessages_delivered = nmessages_delivered + 1;
#if DEBUG_EnableStatistics
                    receiveTimes.addSample(platform.Clock.Time);
#endif
                }
                // Interlocked.Increment(ref nmessages_delivered);

#if DEBUG_Experiment_203
                logger.Log(this, QS.CMS.Helpers.ToString.ReceivedObject(sourceIID, receivedObject));
#endif

                return null;
            }

            #endregion

            #region completionCallback

            private void completionCallback(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
                int message_ind = (int)asynchronousOperation.AsyncState;
                completionTimes[message_ind] = platform.Clock.Time;

                int nmessages_acked;
                lock (this)
                {
                    nmessages_acked = nmessages_received = nmessages_received + 1;
                }

                // if (Interlocked.Increment(ref nmessages_received) < nmessages)
                if (nmessages_acked < nmessages)
                {
                    if (nmessages_sent < nmessages)
                        send_message();
                }
                else
                {
                    bool send_results;
                    lock (this)
                    {
                        send_results = !isCompleted;
                        isCompleted = true;
                    }

                    if (send_results)
                    {
                        QS._core_c_.Components.AttributeSet resultAttributes = new QS._core_c_.Components.AttributeSet();
                        resultAttributes["_iid"] = framework.InstanceID;
                        resultAttributes["StartingTimes"] = new QS._core_e_.Data.DataSeries(startingTimes);
                        resultAttributes["Completion Times"] = new QS._core_e_.Data.DataSeries(completionTimes);
                        resultAttributes["Sender Statistics"] = new QS._core_c_.Components.AttributeSet(simpleSender.Statistics);
                        applicationController.upcall("experimentCompleted", resultAttributes);
                    }
                }
            }

            #endregion

            #region Terminating and Disposing

            public override void TerminateApplication(bool smoothly)
            {
                framework.Dispose();
                platform.ReleaseResources();
            }

            public override void Dispose()
            {
            }

            #endregion

            [QS.Fx.Base.Inspectable("Collected Statistics", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private QS._core_c_.Components.Attribute collectedStatistics;

            [QS.Fx.Base.Inspectable("Freshly Collected Statistics", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private QS._core_c_.Components.Attribute CollectedStatistics
            {
                get { return (collectedStatistics = GetStatistics(false)); }
            }

            public QS._core_c_.Components.Attribute GetStatistics(bool upload)
            {
                collectedStatistics = new QS._core_c_.Components.Attribute(
                    localAddress.ToString(), QS._qss_c_.Diagnostics_1_.DataCollector.Collect(this));

                return upload ? collectedStatistics : new QS._core_c_.Components.Attribute("foo", "bar");

/*
                CMS.Components.AttributeSet statistics = new QS.CMS.Components.AttributeSet();
                statistics.Add(new QS.CMS.Components.Attribute("Receive Times", receiveTimes.DataSet));
                statistics.Add(new QS.CMS.Components.Attribute(
                    "Framework", new CMS.Components.AttributeSet(((TMS.Base.IStatisticsCollector)framework).Statistics)));
                return new CMS.Components.Attribute(localAddress.ToString() + ":" + incarnation.ToString(), statistics);
*/ 
            }
        }

        #endregion
    }
}
