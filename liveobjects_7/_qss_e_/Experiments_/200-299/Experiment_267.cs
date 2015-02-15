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

#define DEBUG_EnableStatistics

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

using QS._qss_e_.Parameters_.Specifications;

namespace QS._qss_e_.Experiments_
{
#if !DISABLE_CLOCK_SYNC_EXPERIMENTS
    public class Experiment_267 : Experiment_200
    {
        private const string DefaultRepositoryPath = "C:\\.QuickSilver\\.Repository";

        #region experimentWork

        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
            logger.Log(this, "Initial wait....");
            sleeper.sleep(5);

            QS._qss_c_.Base1_.Subnet subnet = new QS._qss_c_.Base1_.Subnet((string)arguments["subnet"]);

            IPAddress ipaddr = IPAddress.None;
            bool found = false;
            foreach (IPAddress addr in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (subnet.contains(addr))
                {
                    ipaddr = addr;
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new Exception("Could not find a local address on the desired subnet.");

            logger.Log(this, "Clock synchronization");

            int ntransmissions = 50000;
            QS.Fx.Network.NetworkAddress clockSynchronizationAddress = new QS.Fx.Network.NetworkAddress("224.99.88.77:34567");

            foreach (Runtime_.IApplicationRef application in this.Applications)
                application.invoke(typeof(Application).GetMethod("SynchronizeClocks"),
                    new object[] { clockSynchronizationAddress.ToString(), ntransmissions, 500 });

            QS._core_c_.ClockSynchronization.Coordinator.Synchronize(
                ipaddr, clockSynchronizationAddress.HostIPAddress, clockSynchronizationAddress.PortNumber, ntransmissions, 50);

            int minimum_idleoffset = arguments.contains("minimum_idleoffset") ? Convert.ToInt32((string)arguments["minimum_idleoffset"]) : 0;
            bool dedicatedgms = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(arguments, "dedicated_gms");
            int gmsoffset = Math.Max(minimum_idleoffset, (dedicatedgms ? 1 : 0));

            ICollection<int> crashing_indices = new System.Collections.ObjectModel.Collection<int>();
            if (arguments.contains("crash_nodes"))
            {
                foreach (string s in ((string)arguments["crash_nodes"]).Split(','))
                {
                    string ss = s.Trim();
                    if (ss.Length > 0)
                    {
                        int cind = Convert.ToInt32(ss);
                        logger.Log(this, "Will crash { " + cind.ToString() + " }");
                        crashing_indices.Add(cind);
                        this.ApplicationOf(cind).invoke(typeof(Application).GetMethod("ShouldCrash"), new object[] { });
                    }
                }
            }

            ICollection<int> freezing_indices = new System.Collections.ObjectModel.Collection<int>();
            if (arguments.contains("sleep_nodes"))
            {
                double duration = arguments.contains("sleep_delay") ? Convert.ToDouble((string)arguments["sleep_delay"]) : 1;
                foreach (string s in ((string)arguments["sleep_nodes"]).Split(','))
                {
                    string ss = s.Trim();
                    if (ss.Length > 0)
                    {
                        int cind = Convert.ToInt32(ss);
                        logger.Log(this, "Will freeze { " + cind.ToString() + " } for " + duration.ToString() + " seconds");
                        freezing_indices.Add(cind);
                        this.ApplicationOf(cind).invoke(typeof(Application).GetMethod("ShouldFreeze"), new object[] { duration });
                    }
                }
            }

            ICollection<int> delayedjoin_indices = new System.Collections.ObjectModel.Collection<int>();
            if (arguments.contains("delayjoin_nodes"))
            {
                foreach (string s in ((string)arguments["delayjoin_nodes"]).Split(','))
                {
                    string ss = s.Trim();
                    if (ss.Length > 0)
                    {
                        int cind = Convert.ToInt32(ss);
                        logger.Log(this, "Will delay join of { " + cind.ToString() + " }.");
                        delayedjoin_indices.Add(cind);
                    }
                }
            }

            ICollection<int> drop_indices = new System.Collections.ObjectModel.Collection<int>();
            if (arguments.contains("drop_nodes"))
            {
                foreach (string s in ((string)arguments["drop_nodes"]).Split(','))
                {
                    string ss = s.Trim();
                    if (ss.Length > 0)
                    {
                        int cind = Convert.ToInt32(ss);
                        logger.Log(this, "Will drop at { " + cind.ToString() + " }.");
                        drop_indices.Add(cind);
                        this.ApplicationOf(cind).invoke(typeof(Application).GetMethod("ShouldDrop"),
                            new object[] 
                            { 
                                QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(arguments, "drop_interval", 60),
                                QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(arguments, "drop_duration", 0),
                                QS._qss_e_.Experiment_.Helpers.Args.Int32Of(arguments, "drop_count", 0),
                                QS._qss_e_.Experiment_.Helpers.Args.BoolOf(arguments, "drop_repeat", false)
                            });
                    }
                }
            }

            logger.Log(this, "Waiting 2s....");

            sleeper.sleep(2);

            nsenders = Convert.ToInt32((string)arguments["nsenders"]);
            ngroups = arguments.contains("ngroups") ? Convert.ToInt32(arguments["ngroups"]) : 1;
            nregions = arguments.contains("nregions") ? Convert.ToInt32(arguments["nregions"]) : 1;

#if !DEBUG_DoNotUseScenarios

            if (arguments.contains("scenario"))
            {
                string scenario_name = (string)arguments["scenario"];
                Type scenario_clrtype = Type.GetType("QS.TMS.Scenarios." + scenario_name);
                if (scenario_clrtype == null || !typeof(Scenarios_.IScenarioClass).IsAssignableFrom(scenario_clrtype))
                    throw new Exception("Class QS.TMS.Scenarios." + scenario_name + " either does not exist or is not a scenario class.");

                System.Reflection.ConstructorInfo constructorInfo = scenario_clrtype.GetConstructor(Type.EmptyTypes);
                if (constructorInfo == null)
                    throw new Exception("The specified scenario class does not have a default constructor.");

                Scenarios_.IScenarioClass scenarioClass = (Scenarios_.IScenarioClass)constructorInfo.Invoke(new object[] { });
                if (scenarioClass == null)
                    throw new Exception("Could not construct the requested scenario class.");

                IDictionary<string, string> scenario_attributes = new Dictionary<string, string>();
                string scenario_argument_prefix = "scenario_";
                IEnumerator<QS._core_c_.Components.Attribute> attribs_enum = arguments.Attributes;
                while (attribs_enum.MoveNext())
                {
                    QS._core_c_.Components.Attribute attribute = attribs_enum.Current;
                    if (attribute.Name.StartsWith(scenario_argument_prefix))
                        scenario_attributes.Add(attribute.Name.Substring(scenario_argument_prefix.Length), (string)attribute.Value);
                }

                scenario = scenarioClass.Create(this.NumberOfApplications - gmsoffset, nsenders, ngroups, nregions, scenario_attributes);
            }
            else
            {
                throw new Exception("No scenario specified.");
            }

#endif

            logger.Log(this, "Changing membership....");

            int max_subscribe = arguments.contains("max_subscribe") ? Convert.ToInt32(arguments["max_subscribe"]) : 1000;
            double subscribe_interval = arguments.contains("subscribe_interval") ? Convert.ToDouble(arguments["subscribe_interval"]) : 60;
            bool subscribe_confirm = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(arguments, "subscribe_confirm");

            Queue<string>[] to_subscribe = new Queue<string>[this.NumberOfApplications];
            int npasses = 0;

            for (int ind = gmsoffset; ind < this.NumberOfApplications; ind++)
            {
                int[] where_to_subscribe = scenario.WhereToSubscribe(ind - gmsoffset);
                if (where_to_subscribe.Length > 0)
                {
                    if (to_subscribe[ind] == null)
                        to_subscribe[ind] = new Queue<string>();

                    for (int k = 0; k < Math.Ceiling(((double)where_to_subscribe.Length) / ((double)max_subscribe)); k++)
                    {
                        int how_many = Math.Min(max_subscribe, where_to_subscribe.Length - k * max_subscribe);
                        int[] some = new int[how_many];
                        Array.Copy(where_to_subscribe, k * max_subscribe, some, 0, how_many);
                        to_subscribe[ind].Enqueue(QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<int>(some, ","));
                    }

                    npasses = Math.Max(npasses, to_subscribe[ind].Count);
                }
            }

            bool all_subscribed = false;
            int passno = 0;
            do
            {
                passno++;
                logger.Log(this, "Subscription pass " + passno.ToString() + " / " + npasses.ToString());

                all_subscribed = true;

                for (int ind = gmsoffset; ind < this.NumberOfApplications; ind++)
                {
                    if (!delayedjoin_indices.Contains(ind) && to_subscribe[ind] != null && to_subscribe[ind].Count > 0)
                    {
#if DEBUG_DoNotUseScenarios

                        this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("ChangeMembership"), new object[] { ind });

#else
                        string group_ids_string = to_subscribe[ind].Dequeue();
                        logger.Log(this, "Node " + this.ApplicationOf(ind).Address + " will subscribe to { " + group_ids_string + " }.");

                        this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("SubscribeToGroups"),
                            new object[] { ind, group_ids_string });

                        if (to_subscribe[ind].Count > 0)
                            all_subscribed = false;
                    }

                    //                    else
                    //                    {
                    //                        logger.Log(this, "Node " + this.ApplicationOf(ind).Address + " will not subscribe anywhere.");
                    //                    }
#endif
                }

                logger.Log(this, "Waiting for everybody to subscribe....");
                sleeper.sleep(subscribe_interval);

                if (subscribe_confirm)
                {
                    System.Windows.Forms.MessageBox.Show("Subscribing completed?", "Confirmation", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Stop, System.Windows.Forms.MessageBoxDefaultButton.Button1,
                        System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                }
            }
            while (!all_subscribed);

            logger.Log(this, "Waiting for the system to stabilize (1).");

            sleeper.sleep(Convert.ToDouble((string)arguments["stabilize"]));

            if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(arguments, "manual_release"))
            {
                System.Windows.Forms.MessageBox.Show("Are you ready?", "Confirmation", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Stop, System.Windows.Forms.MessageBoxDefaultButton.Button1,
                    System.Windows.Forms.MessageBoxOptions.ServiceNotification);
            }

            logger.Log(this, "Preparing for multicasting.");

#if DEBUG_DoNotUseScenarios
            
            for (int ind = 0; ind < nsenders; ind++)
            {
                this.ApplicationOf(
                    (dedicated_gms ? ((ind % (this.NumberOfApplications - 1)) + 1) : (ind % this.NumberOfApplications))                    
                    ).invoke(typeof(Application).GetMethod("Send"), new object[] { ind });
            }

#else

            int sender_index = 0;
            for (int ind = gmsoffset; ind < this.NumberOfApplications; ind++)
            {
                int[] where_to_publish = scenario.WhereToPublish(ind - gmsoffset);
                if (where_to_publish.Length > 0)
                {
                    string group_ids_string = QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<int>(where_to_publish, ",");

                    logger.Log(this, "Node " + this.ApplicationOf(ind).Address + " will publish in { " + group_ids_string + " }.");

                    this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("PublishInGroups"),
                        new object[] { sender_index++, group_ids_string });
                }
                else
                {
                    logger.Log(this, "Node " + this.ApplicationOf(ind).Address + " will not publish anywhere.");
                }
            }

            if (sender_index != nsenders)
                throw new Exception("The number of senders generated by the scenario does not match the number of senders requested.");

#endif

            logger.Log(this, "Waiting for the system to stabilize (2).");

            sleeper.sleep(5);

            logger.Log(this, "Starting to multicast.");

            if (delayedjoin_indices.Count > 0)
            {
                double joindelay = QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(arguments, "delayjoin_delay", 60);
                logger.Log(this, "Some nodes will join after { " + joindelay.ToString() + " } seconds of delay.");
                sleeper.sleep(joindelay);

                logger.Log(this, "Executing the delayed joins now...");

                all_subscribed = false;
                passno = 0;
                do
                {
                    passno++;
                    logger.Log(this, "Delayed subscription pass " + passno.ToString() + " / " + npasses.ToString());

                    all_subscribed = true;

                    for (int ind = gmsoffset; ind < this.NumberOfApplications; ind++)
                    {
                        if (delayedjoin_indices.Contains(ind) && to_subscribe[ind] != null && to_subscribe[ind].Count > 0)
                        {
#if DEBUG_DoNotUseScenarios

                            this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("ChangeMembership"), new object[] { ind });

#else
                            string group_ids_string = to_subscribe[ind].Dequeue();
                            logger.Log(this, "Node " + this.ApplicationOf(ind).Address + " will subscribe to { " + group_ids_string + " }.");

                            this.ApplicationOf(ind).invoke(typeof(Application).GetMethod("SubscribeToGroups"),
                                new object[] { ind, group_ids_string });

                            if (to_subscribe[ind].Count > 0)
                                all_subscribed = false;
                        }
#endif
                    }

                    //                    logger.Log(this, "Waiting for the delayed nodes to subscribe....");
                    //                    sleeper.sleep(subscribe_interval);
                }
                while (!all_subscribed);
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
                logger.Log(this, "About to save statistics.");

                Queue<IAsyncResult> waitHandles = new Queue<IAsyncResult>();
                for (int appind = 0; appind < this.NumberOfApplications; appind++)
                {
                    if (crashing_indices.Contains(appind))
                    {
                        logger.Log(this, "Not saving statistics at { " + appind.ToString() + " }");
                    }
                    else
                    {
                        QS._qss_e_.Runtime_.IApplicationRef application = this.ApplicationOf(appind);
                        logger.Log(this, "[" + (Interlocked.Increment(ref nsaving) + 1).ToString() + "] saving statistics at " + application.Address);
                        waitHandles.Enqueue(
                            application.BeginInvoke(
                                typeof(Application).GetMethod("GetStatistics"), new object[] { false }, new AsyncCallback(
                                    delegate(IAsyncResult result)
                                    {
                                        logger.Log(this, "[" + (Interlocked.Increment(ref nsaved) + 1).ToString() + "] statistics saved at " + ((QS._qss_e_.Runtime_.IApplicationRef)result.AsyncState).Address);
                                    }), application));
                    }
                }

                while (waitHandles.Count > 0)
                    waitHandles.Dequeue().AsyncWaitHandle.WaitOne();

                logger.Log(this, "All statistics saved.");
            }

            if (arguments.contains("save") && (arguments["save"].Equals("yes") || arguments["save"].Equals("on")))
                QS._qss_e_.Experiment_.Helpers.SaveResults.Save(DefaultRepositoryPath, logger, this, arguments, results);

            logger.Log(this, "Completed.");

            if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(arguments, "approve_closing"))
            {
                System.Windows.Forms.MessageBox.Show("Approve closing?", "Confirmation", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Stop, System.Windows.Forms.MessageBoxDefaultButton.Button1,
                    System.Windows.Forms.MessageBoxOptions.ServiceNotification);
            }
        }

        #endregion

        #region Fields

        private int nsenders, ngroups, nregions, nsendersCompleted;
        private int nsaving, nsaved;
        private Scenarios_.IScenario scenario;

        #endregion

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

        public void NotMuchHappeningAroundHere(QS._core_c_.Components.AttributeSet arguments)
        {
            logger.Log(null, "Mot much happening around here.\n" + QS.Fx.Printing.Printable.ToString(arguments));
        }

        #endregion

        #region Constants

        private const int ANTICIPATED_RUNNING_TIME = 10000;

        #endregion

        #region Class Application

        protected new class Application : Experiment_200.Application, QS._qss_e_.Runtime_.IControlledApp, QS._qss_c_.Devices_3_.IReceiver
        {
            #region Constants

            // private static readonly QS.Fx.Network.NetworkAddress groupAddress = new QS.Fx.Network.NetworkAddress("224.12.34.56:12000");
            private const uint myloid = (uint)ReservedObjectID.User_Min + 10;

            public static readonly QS.Fx.Network.NetworkAddress ClockServiceAddress = new QS.Fx.Network.NetworkAddress("224.81.80.79:63211");

            #endregion

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

            #region Constructor

            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                monitoring_activity = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "monitoring_activity");
                monitoring_interval = QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(args, "monitoring_interval", 10);

#pragma warning disable 0162
                // some check on ifdefs
                if (QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.Agent.ProcessingCrashes
                    || QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.AgentCore.ProcessingCrashes)
                    throw new Exception("Crahs processing ifdefs are inconsistent.");
#pragma warning restore 0162

                bool activate_fd = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "fd_enabled");
                // bool dedicated_gms = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "dedicated_gms");

                string rootpath = repository_root + "\\" + repository_key;
                core = new QS._core_c_.Core.Core(rootpath + "\\coreworkdir");
                string fsroot = rootpath + "\\filesystem";

                framework = new QS._qss_c_.Framework_1_.FrameworkOnCore(
                    new QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress,
                    logger, platform.EventLogger, core, fsroot, activate_fd,
                    (args.contains("MTU") ? Convert.ToInt32((string)args["MTU"]) : 0),
                    QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "gms_groupalloc"));

                performanceLog = new QS._qss_c_.Diagnostics_3_.PerformanceLog(framework.Clock, framework.AlarmClock, 1);

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "measure_totalcpu"))
                    performanceLog.AddCounter("Processor", "_Total", "% Processor Time");

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "diagnose_nodes"))
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

                /*
                                performanceLog.AddCounter("Network Interface", null, null);
                                performanceLog.AddCounter(".NET CLR Networking", "_global_", null);
                                performanceLog.AddCounter(".NET CLR Memory", "_Global_", "% Time in GC");                
                */

                network = new QS._qss_c_.Devices_3_.Network(logger, 20000);
                udpdevice = (QS._qss_c_.Devices_3_.UDPCommunicationsDevice)
                    network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];
                listener = udpdevice.ListenAt(ClockServiceAddress, this);

                badtime_track = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "badtime_track", false);
                badtime_coeff = QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(args, "badtime_coeff", 0.5);

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
                    framework.Root.DefaultDrainSynchronouslyForUnicastAddresses = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "drain_unicast");

                if (args.contains("drain_multicast"))
                    framework.Root.DefaultDrainSynchronouslyForMulticastAddresses = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "drain_multicast");

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

                core.ContinueIOOnTimeWarps = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "timewarps_continueio");

                if (args.contains("core_maxcc"))
                    core.MaximumConcurrency = Convert.ToInt32((string)args["core_maxcc"]);

                if (args.contains("core_mintx"))
                    core.MinimumTransmitted = Convert.ToInt32((string)args["core_mintx"]);

                if (args.contains("core_maxtx"))
                    core.MaximumTransmitted = Convert.ToInt32((string)args["core_maxtx"]);

                batching = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "batching");
                if (batching)
                    batching_buffersize = Convert.ToInt32((string)args["batching_buffersize"]);

                /*
                                if (args.contains("def_rccls_unicast"))
                                    framework.Core.DefaultUnicastRateControllerClass = GetRateControllerClass((string)args["def_rccls_unicast"]);

                                if (args.contains("def_rccls_multicast"))
                                    framework.Core.DefaultMulticastRateControllerClass = GetRateControllerClass((string)args["def_rccls_multicast"]);
                */

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
                badtime_nmessages = (int)Math.Floor(((double)nmessages) * badtime_coeff);

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

                receivingAgentClass.ForwardingAllowed = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "rs_forwarding", false);

                if (args.contains("rs_max_window"))
                    receivingAgentClass.MaximumWindowWidth = (uint)Convert.ToInt32((string)args["rs_max_window"]);

                if (args.contains("rs_max_naks"))
                    receivingAgentClass.MaximumNakRangesPerToken = (uint)Convert.ToInt32((string)args["rs_max_naks"]);

                regionalController = new QS._qss_c_.Receivers4.RegionalController(
                    framework.LocalAddress, framework.Logger, framework.Demultiplexer, framework.AlarmClock, framework.Clock,
                    framework.MembershipController, receivingAgentClass, receivingAgentClass);
                regionalController.IsDisabled = false;

                bool buffering_unrecognized = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "buffering_unrecognized");
                int maximum_unrecognized = QS._qss_e_.Experiment_.Helpers.Args.Int32Of(args, "maximum_unrecognized", 1000);
                double unrecognized_timeout = QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(args, "unrecognized_timeout", 5);

                regionalSenders = new QS._qss_c_.Senders10.RegionalSenders(
                    framework.EventLogger, framework.LocalAddress, framework.Logger, framework.AlarmClock, framework.Clock,
                    framework.Demultiplexer, null, // this null argument needs to be fixed 
                    regionalController, regionalController, 60, regionalController,
                    buffering_unrecognized, maximum_unrecognized, unrecognized_timeout);

                dispatcherRV2 = new QS._qss_c_.Multicasting7.DispatcherRV2(logger, framework.AlarmClock, framework.Clock,
                    framework.Demultiplexer, (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, regionalController,
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
                receiveRates = new QS._qss_c_.Statistics_.Samples2D[nsenders];
                totalReceived = new QS._qss_c_.Statistics_.Samples2D[nsenders];
                for (int j = 0; j < nsenders; j++)
                {
                    receiveRates[j] = new QS._qss_c_.Statistics_.Samples2D();
                    totalReceived[j] = new QS._qss_c_.Statistics_.Samples2D();

                    ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(
                        "ReceivedRate_" + j.ToString("000"), new QS._core_c_.Diagnostics2.Property(receiveRates[j]));
                    ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(
                        "TotalReceived_" + j.ToString("000"), new QS._core_c_.Diagnostics2.Property(totalReceived[j]));
                }
#endif

                last_nreceived = new int[nsenders];

                measure_latencies = QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "measure_latencies");

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
                    (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel,
                    regionSinkCollection, regionalController, regionalController, framework.MembershipController, framework.Root,
                    args.contains("rs_timeout") ? Convert.ToDouble((string)args["rs_timeout"]) : 10, rateControllers,
                    QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(args, "rs_warmuptime", 1));

                int maximumPendingCompletion = QS._qss_e_.Experiment_.Helpers.Args.Int32Of(args, "gs_max_pending_ack", 100000);
                int feed_buffer_min = QS._qss_e_.Experiment_.Helpers.Args.Int32Of(args, "gs_feed_buffermin", 50);
                int feed_buffer_max = QS._qss_e_.Experiment_.Helpers.Args.Int32Of(args, "gs_feed_buffermax", 150);

                if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "dummy_gs"))
                    groupSinkCollection = new QS._qss_c_.Multicasting7.PlaceholderGSs(framework.MembershipController, regionViewSinkCollection);
                else
                {
                    if (QS._qss_e_.Experiment_.Helpers.Args.BoolOf(args, "alternative_gs"))
                    {
                        groupSinkCollection =
                            new QS._qss_c_.Multicasting7.AlternativeReliableGroupSinks(
                                framework.StatisticsController, framework.Clock, framework.MembershipController, logger,
                                nodeSinkCollection, ((QS._qss_c_.Multicasting7.ReliableRegionViewSinks)regionViewSinkCollection),
                                (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, framework.Root, null,
                                QS._qss_e_.Experiment_.Helpers.Args.DoubleOf(args, "ags_initialrate", 1000));
                    }
                    else
                    {
                        groupSinkCollection =
                            new QS._qss_c_.Multicasting7.ReliableGroupSinks(
                                framework.StatisticsController, framework.Clock, framework.MembershipController, regionViewSinkCollection, logger,
                                maximumPendingCompletion, feed_buffer_min, feed_buffer_max);
                    }
                }

                ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("Sources", diagnosticsContainerForSources);
                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

                clock = QS._core_c_.Core.Clock.SharedClock;

                core.Start();

                logger.Log(this, "Ready");
            }

            #endregion

            private double badtime_coeff;
            private bool badtime_track;
            private int badtime_nmessages;

            #region Clock synchronization

            private QS.Fx.Network.NetworkAddress clockSynchronizationAddress;
            private int clocksync_ntransmissions, clocksync_aggregationsize;
            public void SynchronizeClocks(string address, int ntransmissions, int aggregationsize)
            {
                clockSynchronizationAddress = new QS.Fx.Network.NetworkAddress(address);
                clocksync_ntransmissions = ntransmissions;
                clocksync_aggregationsize = aggregationsize;
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ClockSynchronizationCallback));
            }

            private void ClockSynchronizationCallback(object o)
            {
                logger.Log(this, "SynchronizeClocks_Start");

                QS._core_c_.ClockSynchronization.Client.Accept(logger,
                    framework.LocalAddress.Address.HostIPAddress, clockSynchronizationAddress.HostIPAddress,
                    clockSynchronizationAddress.PortNumber, clocksync_ntransmissions, clocksync_aggregationsize);

                logger.Log(this, "SynchronizeClocks_Stop");
            }

            #endregion

            #region Fields

            private bool monitoring_activity;
            private double monitoring_interval;

            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
            private QS._core_c_.Diagnostics2.Container diagnosticsContainerForSources = new QS._core_c_.Diagnostics2.Container();

            [QS._core_c_.Diagnostics.Component("Performance Log")]
            [QS._core_c_.Diagnostics2.Module("PerformanceLog")]
            private QS._qss_c_.Diagnostics_3_.PerformanceLog performanceLog;

            [QS._core_c_.Diagnostics.Component("Core")]
            [QS._core_c_.Diagnostics2.Module("Core")]
            private QS._core_c_.Core.Core core;

            [QS._core_c_.Diagnostics.Component("Framework")]
            [QS._core_c_.Diagnostics2.Module("Framework")]
            private QS._qss_c_.Framework_1_.FrameworkOnCore framework;

            private QS.Fx.Clock.IClock clock;

            private QS._qss_c_.Devices_3_.Network network;
            private QS._qss_c_.Devices_3_.UDPCommunicationsDevice udpdevice;
            private QS._qss_c_.Devices_3_.IListener listener;
            // private QS.CMS.Connections.MulticastResponder multicastResponder;

            private QS._qss_c_.Rings6.ReceivingAgent receivingAgentClass;
            [QS._core_c_.Diagnostics.Component("Regional Controller")]
            [QS._core_c_.Diagnostics2.Module("RegionalController")]
            private QS._qss_c_.Receivers4.RegionalController regionalController;
            [QS._core_c_.Diagnostics.Component("Regional Senders")]
            [QS._core_c_.Diagnostics2.Module("RegionalSenders")]
            private QS._qss_c_.Senders10.RegionalSenders regionalSenders;

            private QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> nodeSinkCollection;
            private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RegionID,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionSinkCollection;
            [QS._core_c_.Diagnostics2.Module("ReliableRegionViewSinks")]
            private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RVID,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionViewSinkCollection;
            [QS._core_c_.Diagnostics2.Module("ReliableGroupSinks")]
            private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> groupSinkCollection;

            private QS._qss_c_.Multicasting7.DispatcherRV2 dispatcherRV2;

            private int nsenders, batching_buffersize;
#if DEBUG_DoNotUseScenarios
            private int ngroups, nregions;
            private QS.CMS.Base3.GroupID[] groupIDs, separator_groupIDs;
#endif
            private MySource[] mySources;
            private bool batching;

            [QS._core_c_.Diagnostics.Ignore]
            [QS.Fx.Inspection.Ignore]
            private double[] sendTimes, completionTimes;
            private double[][] receiveTimes;

            private bool measure_latencies;

#if DEBUG_EnableStatistics
            [QS._core_c_.Diagnostics.Component("Send Rates")]
            [QS._core_c_.Diagnostics2.Property("SendRates")]
            private QS._qss_c_.Statistics_.Samples2D sendRates;

            [QS._core_c_.Diagnostics.Component("Total Sent")]
            [QS._core_c_.Diagnostics2.Property("TotalSent")]
            private QS._qss_c_.Statistics_.Samples2D totalSent;

            [QS._core_c_.Diagnostics.Component("Completion Rates")]
            [QS._core_c_.Diagnostics2.Property("CompletionRates")]
            private QS._qss_c_.Statistics_.Samples2D completionRates;

            [QS._core_c_.Diagnostics.Component("Total Completed")]
            [QS._core_c_.Diagnostics2.Property("TotalCompleted")]
            private QS._qss_c_.Statistics_.Samples2D totalCompleted;

            [QS._core_c_.Diagnostics.Component("Total Received Overall")]
            [QS._core_c_.Diagnostics2.Property("TotalReceivedOverall")]
            private QS._qss_c_.Statistics_.Samples2D totalReceivedOverall;

            [QS._core_c_.Diagnostics.Component("Received Rate Overall")]
            [QS._core_c_.Diagnostics2.Property("ReceivedRateOverall")]
            private QS._qss_c_.Statistics_.Samples2D receivedRateOverall;

            [QS._core_c_.Diagnostics.Component("Pending Completion")]
            [QS._core_c_.Diagnostics2.Property("PendingCompletion")]
            private QS._qss_c_.Statistics_.Samples2D pendingCompletion;

            // no diagnostics here, these guys are registered explicitly in the constructor
            [QS._core_c_.Diagnostics.ComponentCollection("Receive Rates")]
            private QS._qss_c_.Statistics_.Samples2D[] receiveRates;
            [QS._core_c_.Diagnostics.ComponentCollection("Total Received")]
            private QS._qss_c_.Statistics_.Samples2D[] totalReceived;
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

            [QS.Fx.Base.Inspectable("Collected Statistics", QS.Fx.Base.AttributeAccess.ReadOnly)]
            private QS._core_c_.Components.AttributeSet collected_statistics;

            #endregion

            #region GetRateControllerClass

            /*
            private QS.CMS.RateControl.IRateControllerClass GetRateControllerClass(string s)
            {
                if (s.Equals("1"))
                {
                    return new QS.CMS.RateControl.RateController1.Class();
                }
                else if (s.Equals("2"))
                {
                }
                else if (s.Equals("3"))
                {
                }
                else
                    throw new Exception("Unknown rate controller class.");
            }
*/

            #endregion

            #region Method SampleRates

            private void SampleRates(double time_now)
            {
                double time_tolog = (lastChecked + time_now) / 2;
                double time_delta = time_now - lastChecked;

#if DEBUG_EnableStatistics
                if (sendRates == null)
                    sendRates = new QS._qss_c_.Statistics_.Samples2D();
                sendRates.Add(time_tolog, (nsent - last_nsent) / time_delta);

                if (totalSent == null)
                    totalSent = new QS._qss_c_.Statistics_.Samples2D();
                totalSent.Add(time_tolog, ((double)nsent));
#endif

                last_nsent = nsent;

#if DEBUG_EnableStatistics
                if (completionRates == null)
                    completionRates = new QS._qss_c_.Statistics_.Samples2D();
                completionRates.Add(time_tolog, (ncompleted - last_ncompleted) / time_delta);

                if (totalCompleted == null)
                    totalCompleted = new QS._qss_c_.Statistics_.Samples2D();
                totalCompleted.Add(time_tolog, ((double)ncompleted));

                if (pendingCompletion == null)
                    pendingCompletion = new QS._qss_c_.Statistics_.Samples2D();
                pendingCompletion.Add(time_tolog, ((double)(nsent - ncompleted)));

                if (totalReceivedOverall == null)
                    totalReceivedOverall = new QS._qss_c_.Statistics_.Samples2D();
                totalReceivedOverall.Add(time_tolog, ((double)overall_nreceived));

                if (receivedRateOverall == null)
                    receivedRateOverall = new QS._qss_c_.Statistics_.Samples2D();
                receivedRateOverall.Add(time_tolog, (overall_nreceived - last_overall_nreceived) / time_delta);
#endif

                last_ncompleted = ncompleted;
                last_overall_nreceived = overall_nreceived;

                for (int j = 0; j < nsenders; j++)
                {
#if DEBUG_EnableStatistics
                    receiveRates[j].Add(time_tolog, (nreceived[j] - last_nreceived[j]) / time_delta);
                    totalReceived[j].Add(time_tolog, ((double)nreceived[j]));
#endif

                    last_nreceived[j] = nreceived[j];
                }
                lastChecked = time_now;
            }

            #endregion

            #region Properties

#if DEBUG_EnableStatistics
            [QS._core_c_.Diagnostics.Component("Total Received Overlayed (X = time, Y = nreceived)")]
            [QS._core_c_.Diagnostics2.Property("TotalReceivedOverlayed")]
            public QS._core_e_.Data.IDataSet TotalReceivedOverlayed
            {
                get
                {
                    QS._core_e_.Data.DataCo collection = new QS._core_e_.Data.DataCo();
                    if (totalReceived != null)
                    {
                        for (int ind = 0; ind < totalReceived.Length; ind++)
                            collection.Add(new QS._core_e_.Data.Data2D(ind.ToString(), totalReceived[ind].Samples));
                    }
                    return collection;
                }
            }

            [QS._core_c_.Diagnostics.Component("Receive Rates Overlayed (X = time, Y = rate)")]
            [QS._core_c_.Diagnostics2.Property("ReceiveRatesOverlayed")]
            public QS._core_e_.Data.IDataSet ReceiveRatesOverlayed
            {
                get
                {
                    QS._core_e_.Data.DataCo collection = new QS._core_e_.Data.DataCo();
                    if (totalReceived != null)
                    {
                        for (int ind = 0; ind < receiveRates.Length; ind++)
                            collection.Add(new QS._core_e_.Data.Data2D(ind.ToString(), receiveRates[ind].Samples));
                    }
                    return collection;
                }
            }
#endif

            [QS._core_c_.Diagnostics.Component("Send Times (X = seqno, Y = time)")]
            [QS._core_c_.Diagnostics2.Property("SendTimes")]
            public QS._core_e_.Data.IDataSet SendTimes
            {
                get
                {
                    if (sendTimes != null)
                        return new QS._core_e_.Data.DataSeries(sendTimes);
                    else
                        return null;
                }
            }

            [QS._core_c_.Diagnostics.Component("Completion Times (X = seqno, Y = time)")]
            [QS._core_c_.Diagnostics2.Property("CompletionTimes")]
            public QS._core_e_.Data.IDataSet CompletionTimes
            {
                get
                {
                    if (completionTimes != null)
                        return new QS._core_e_.Data.DataSeries(completionTimes);
                    else
                        return null;
                }
            }

            [QS._core_c_.Diagnostics.ComponentCollection("Receive Times (X = seqno, Y = time)")]
            [QS._core_c_.Diagnostics2.Property("ReceiveTimes")]
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
                        return null;
                }
            }

            [QS._core_c_.Diagnostics.Component("Combined Statistics (X = seqno, Y = time)")]
            [QS._core_c_.Diagnostics2.Property("CombinedStatistics")]
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
                        return null;
                }
            }

            #endregion

            public QS._core_c_.Components.AttributeSet GetPerformanceCounters()
            {
                QS._core_c_.Components.AttributeSet counters_attributes = new QS._core_c_.Components.AttributeSet();

                // ((QS.TMS.Inspection.IInspectable) performanceLog).
                // performanceLog

                return counters_attributes;
            }

            #region ShouldCrash and ShouldFreeze

            public void ShouldDrop(double interval, double duration, int count, bool repeat)
            {
                logger.Log(this, "__________SHOULD_DROP");
                should_drop = true;
                drop_interval = interval;
                drop_duration = duration;
                drop_count = count;
                drop_repeat = repeat;
            }

            public void ShouldCrash()
            {
                logger.Log(this, "__________SHOULD_CRASH");
                should_die = true;
            }

            public void ShouldFreeze(double duration)
            {
                logger.Log(this, "__________SHOULD_SLEEP");
                should_sleep = true;
                sleep_duration = duration;
            }

            #endregion

            #region ChangeMembership

#if DEBUG_DoNotUseScenarios

            public void ChangeMembership(int index)
            {
                framework.Core.ScheduleCall(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        logger.Log(this, "__________CHANGE_MEMBERSHIP(" + index.ToString() + ")");

                        List<QS.CMS.Base3.GroupID> groupsToJoin = new List<QS.CMS.Base3.GroupID>();
                        groupsToJoin.AddRange(groupIDs);
                        groupsToJoin.Add(separator_groupIDs[index % nregions]);
                        framework.MembershipAgent.ChangeMembership(groupsToJoin, new List<QS.CMS.Base3.GroupID>());
                    }), null);
            }

#else

            public void SubscribeToGroups(int index, string groups)
            {
                string[] groupids = groups.Split(',');
                List<QS._qss_c_.Base3_.GroupID> groupnos = new List<QS._qss_c_.Base3_.GroupID>();
                foreach (string gid in groupids)
                    groupnos.Add(new QS._qss_c_.Base3_.GroupID((uint)(1000 + Convert.ToInt32(gid))));

                framework.Platform.Scheduler.Execute(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        logger.Log(this, "__________CHANGE_MEMBERSHIP( " + index.ToString() + ", { " +
                            QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._qss_c_.Base3_.GroupID>(groupnos, ",") + "} )");

                        framework.MembershipAgent.ChangeMembership(groupnos, new List<QS._qss_c_.Base3_.GroupID>());
                    }), null);
            }

#endif

            #endregion

            #region Monitoring

            private QS.Fx.Clock.IAlarm monitoringAlarm;
            private int lastchecked_nsent, lastchecked_ncompleted;
            private void MonitoringCallback(QS.Fx.Clock.IAlarm alarm)
            {
                if (nsent < nmessages)
                {
                    if (nsent == lastchecked_nsent && ncompleted == lastchecked_ncompleted)
                    {
                        QS._core_c_.Components.AttributeSet attribs = new QS._core_c_.Components.AttributeSet();
                        attribs["address"] = localAddress.ToString();
                        attribs["nsent"] = nsent.ToString();
                        attribs["ncompleted"] = ncompleted.ToString();
                        applicationController.upcall("NotMuchHappeningAroundHere", attribs);
                    }

                    lastchecked_nsent = nsent;
                    lastchecked_ncompleted = ncompleted;

                    alarm.Reschedule();
                }
            }

            #endregion

            #region Sending

#if DEBUG_DoNotUseScenarios

            #region Send

            public void Send(int senderIndex)
            {
                this.senderIndex = senderIndex;

                logger.Log(this, "__________SEND(" + senderIndex.ToString() + ")");

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
                    mySources[ind] = new MySource(
                        this, ind, groupIDs.Length, groupIDs[ind], (int) Math.Floor(((double) (nmessages - (ind + 1))) / ((double) groupIDs.Length)) + 1);
                }

                framework.Core.ScheduleCall(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        for (int ind = 0; ind < groupIDs.Length; ind++)
                        {
                            QS.CMS.Base6.GetObjectsCallback<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>> getCallback =
                                new QS.CMS.Base6.GetObjectsCallback<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>(mySources[ind].GetCallback);

                            QS.CMS.Base6.ISink<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>> underlyingSink = groupSinkCollection[groupIDs[ind]];

                            if (batching)
                                underlyingSink = new QS.CMS.Batching.BatchingSink(
                                    framework.Core, framework.Core, framework.Logger, underlyingSink, batching_buffersize);

                            underlyingSink.Send(getCallback);
                        }
                    }), null);
            }

            #endregion

#else

            #region PublishInGroups

            public void PublishInGroups(int senderIndex, string groups)
            {
                this.senderIndex = senderIndex;

                logger.Log(this, "__________SEND(" + senderIndex.ToString() + ")");

                if (detailed_timings)
                {
                    sendTimes = new double[nmessages];
                    completionTimes = new double[nmessages];
                }
                else
                    sendTimes = completionTimes = null;

                string[] groupids = groups.Split(',');
                List<QS._qss_c_.Base3_.GroupID> groupnos = new List<QS._qss_c_.Base3_.GroupID>();
                foreach (string gid in groupids)
                    groupnos.Add(new QS._qss_c_.Base3_.GroupID((uint)(1000 + Convert.ToInt32(gid))));

                QS._qss_c_.Base3_.GroupID[] group_IDs = groupnos.ToArray();

                mySources = new MySource[group_IDs.Length];
                for (int ind = 0; ind < group_IDs.Length; ind++)
                {
                    mySources[ind] = new MySource(
                        this, ind, group_IDs.Length, group_IDs[ind], (int)Math.Floor(((double)(nmessages - (ind + 1))) / ((double)group_IDs.Length)) + 1);
                }

                framework.Platform.Scheduler.Execute(new AsyncCallback(
                    delegate(IAsyncResult result)
                    {
                        if (monitoring_activity)
                            monitoringAlarm = framework.AlarmClock.Schedule(monitoring_interval, new QS.Fx.Clock.AlarmCallback(this.MonitoringCallback), null);

                        for (int ind = 0; ind < group_IDs.Length; ind++)
                        {
                            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getCallback =
                                new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(mySources[ind].GetCallback);

                            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> underlyingSink = groupSinkCollection[group_IDs[ind]];

                            if (batching)
                                underlyingSink = new QS._qss_c_.Batching_.BatchingSink(
                                    framework.Clock, framework.AlarmClock, framework.Logger, underlyingSink, batching_buffersize);

                            underlyingSink.Send(getCallback);
                        }
                    }), null);
            }

            #endregion

#endif

            #endregion

            #region Class MySource

            [QS.Fx.Base.Inspectable]
            private class MySource : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
            {
                private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

                #region IModule Members

                QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
                {
                    get { return diagnosticsContainer; }
                }

                #endregion

                public MySource(Application owner, int index, int ngroups, QS._qss_c_.Base3_.GroupID groupID, int nmessages)
                {
                    this.owner = owner;
                    this.index = index;
                    this.ngroups = ngroups;
                    this.groupID = groupID;
                    this.nmessages = nmessages;

                    this.clock = QS._core_c_.Core.Clock.SharedClock;

#if DEBUG_EnableStatistics
                    ts_GetCallbackOverheads = owner.framework.StatisticsController.Allocate2D(
                        "get callback overheads", "", "time", "s", "", "overhead", "s", "");
                    ts_GetCallbackNumberOfObjectsReturned = owner.framework.StatisticsController.Allocate2D(
                        "number of objects returned by get callback", "", "time", "s", "", "number of objects returned", "", "");
#endif

                    ((QS._core_c_.Diagnostics2.IContainer)owner.diagnosticsContainerForSources).Register(groupID.ToString(), diagnosticsContainer);
                    QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
                }

                private Application owner;
                private QS._qss_c_.Base3_.GroupID groupID;
                private int index, ngroups, nsent, nmessages;
                private QS.Fx.Clock.IClock clock;

#if DEBUG_EnableStatistics
                [QS._core_c_.Diagnostics2.Property("GetCallbackOverheads")]
                private QS._core_c_.Statistics.ISamples2D ts_GetCallbackOverheads;

                [QS._core_c_.Diagnostics2.Property("GetCallbackNumberOfObjectsReturned")]
                private QS._core_c_.Statistics.ISamples2D ts_GetCallbackNumberOfObjectsReturned;
#endif

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

#if DEBUG_EnableStatistics
                    double tt1 = owner.framework.Clock.Time;
#endif

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
                                    if (owner.measure_latencies)
                                        *((double*)(pbytes + 2 * sizeof(int))) = clock.Time;
                                }
                            }
                            QS._core_c_.Base2.BlockOfData block = new QS._core_c_.Base2.BlockOfData(bytes);
                            QS.Fx.Serialization.ISerializable serializableObject = block; // new QS.CMS.Base3.Int32x2(owner.senderIndex, seqno);
                            objectQueue.Enqueue(
                                new QS._qss_c_.Base6_.AsynchronousMessage(new QS._core_c_.Base3.Message(myloid, serializableObject),
                                new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback)));
                            numberOfObjectsReturned++;
                            // numberOfBytesReturned += ............................................HERE
                        }
                        else
                        {
                            double time_now = owner.framework.Clock.Time;
                            owner.SampleRates(time_now);

                            moreObjectsAvailable = false;
                            break;
                        }
                    }

#if DEBUG_EnableStatistics
                    double tt2 = owner.framework.Clock.Time;
                    ts_GetCallbackOverheads.Add(tt1, tt2 - tt1);
                    ts_GetCallbackNumberOfObjectsReturned.Add(tt2, numberOfObjectsReturned);
#endif
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
                    {
                        owner.SampleRates(time_now);
                        owner.applicationController.upcall("SendingCompleted", QS._core_c_.Components.AttributeSet.None);
                    }
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

                return null;
            }

            #endregion

            #region GetStatistics

            public QS._core_c_.Components.Attribute GetStatistics(bool upload)
            {
                logger.Log(this, "_____GetStatistics : Enter");

                framework.StatisticsController.Close();

                logger.Log(this, "_____GetStatistics : Collecting all statistics");

                // collected_statistics = QS.CMS.Diagnostics.DataCollector.Collect(this);
                collected_statistics = QS._core_c_.Diagnostics2.Helper.Collect(diagnosticsContainer);

                logger.Log(this, "_____GetStatistics : Dumping to hard drive");

                QS._qss_e_.Experiment_.Helpers.DumpResults.Dump(logger, collected_statistics, repository_root, repository_key);

                logger.Log(this, "_____GetStatistics : Preparing return value");

                QS._core_c_.Components.Attribute returned_statistics =
                    upload ? (new QS._core_c_.Components.Attribute(localAddress.ToString(), collected_statistics))
                        : (new QS._core_c_.Components.Attribute("foo", "bar"));

                logger.Log(this, "_____GetStatistics : Leave");

                return returned_statistics;
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

        public Experiment_267()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }

        #endregion
    }
#endif
}
