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
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;

namespace QS._qss_x_.Platform_
{
    public static class Application
    {
        #region Parameter Names

        private const string PARAMETER_CONFIG = "config";
        private const string PARAMETER_OUTPUT = "output";
        private const string PARAMETER_LOAD = "load";
        private const string PARAMETER_MACHINES = "machines";
        private const string PARAMETER_APPLICATION = "application";
        private const string PARAMETER_GUI = "gui";
        private const string PARAMETER_VERBOSE = "verbose";
        private const string PARAMETER_TIME = "time";
        private const string PARAMETER_ENGINECONFIG = "engine";
        private const string PARAMETER_APPCONFIG = "appconfig";

        private const string PARAMETER_ENGINE_ALARM_QUANTUM = "timer_quantum";
        private const string PARAMETER_ENGINE_IO_QUANTUM = "io_quantum";
        private const string PARAMETER_ENGINE_DEFAULT_UNICAST_RATE = "unicast_rate";
        private const string PARAMETER_ENGINE_DEFAULT_MULTICAST_RATE = "multicast_rate";
        private const string PARAMETER_ENGINE_FC_UNICAST_CREDITS = "unicast_credits";
        private const string PARAMETER_ENGINE_FC_MULTICAST_CREDITS = "multicast_credits";
//        private const string PARAMETER_ENGINE_UNICAST_SENDER_CC = "unicast_sender_cc";
//        private const string PARAMETER_ENGINE_MULTICAST_SENDER_CC = "multicast_sender_cc";
        private const string PARAMETER_ENGINE_TIMEWARPS_CONTINUEIO = "detect_timewarps";
        private const string PARAMETER_ENGINE_MAXCC = "max_concurrency";
        private const string PARAMETER_ENGINE_MINTX = "min_transferred";
        private const string PARAMETER_ENGINE_MAXTX = "max_transferred";

        #endregion

        #region Parameter Values

        private const string DEFAULT_OUTPUT = "output.txt";
        private const string DEFAULT_MACHINES = "machines.txt";
        private const string DEFAULT_TEMP = "temp";
        private const string DEFAULT_FSROOT = "temp\\fsroot";
//        private const string DEFAULT_ENGINEPARAMETER = "engine.";
//        private const string DEFAULT_APPLICATIONPARAMETER = "application.";
        private const string DEFAULT_TIME = "60";
        private const string DEFAULT_ENGINECONFIG = "engine.xml";
        private const string DEFAULT_APPCONFIG = "application.xml";

        #endregion

        #region Public Method Run

        [DllImport("KERNEL32.DLL")]
        private static extern void ExitProcess(int uExitCode); 

        public static void Run(string[] _arguments)
        {
            #region Read parameters

            Dictionary<string, string> parameters = new Dictionary<string,string>();
            List<string> arguments = new List<string>();

            foreach (string _argument in _arguments)
            {
                string __argument = _argument.Replace("\"", "").Trim();
                if (__argument.StartsWith("-") || __argument.StartsWith("/"))
                {
                    __argument = __argument.TrimStart('-', '/');
                    int index = __argument.IndexOf(":");
                    if (index > 0)
                        parameters.Add(__argument.Substring(0, index), __argument.Substring(index + 1));
                    else
                        parameters.Add(__argument, null);
                }
                else
                    arguments.Add(__argument);
            }

            string rootdir = Process.GetCurrentProcess().MainModule.FileName;
            rootdir = rootdir.Substring(0, rootdir.LastIndexOf('\\') + 1);

            if (parameters.ContainsKey(PARAMETER_CONFIG))
            {
                Configuration _parameters;
                using (StreamReader reader = new StreamReader(rootdir + parameters[PARAMETER_CONFIG]))
                    _parameters = (Configuration)(new XmlSerializer(typeof(Configuration))).Deserialize(reader);
                foreach (Parameter _parameter in _parameters.Parameters)
                    parameters.Add(_parameter.Name, _parameter.Value);
                parameters.Remove(PARAMETER_CONFIG);
            }

            #endregion

            #region Prepare output logger

            string outputfile = rootdir +
                (parameters.ContainsKey(PARAMETER_OUTPUT) ? parameters[PARAMETER_OUTPUT] : DEFAULT_OUTPUT);
            parameters.Remove(PARAMETER_OUTPUT);

            if (File.Exists(outputfile))
                File.Delete(outputfile);

            QS.Fx.Logging.ILogger output = new QS._qss_d_.Components_.FileLogger(outputfile);

            #endregion

            try
            {
                #region Handle verbose switch

                bool verbose = parameters.ContainsKey(PARAMETER_VERBOSE);
                parameters.Remove(PARAMETER_VERBOSE);

                #endregion

                #region Load libraries

                if (parameters.ContainsKey(PARAMETER_LOAD))
                {
                    foreach (string _library in parameters[PARAMETER_LOAD].Split(';'))
                    {
                        string library = rootdir + _library;                        
                        if (verbose)
                            output.Log("Loading : " + library);
                        Assembly.LoadFile(library);
                    }
                    parameters.Remove(PARAMETER_LOAD);
                }

                #endregion

                #region Locate application

                if (!parameters.ContainsKey(PARAMETER_APPLICATION))
                    throw new Exception("Application not specified.");
                string _application = parameters[PARAMETER_APPLICATION];
                parameters.Remove(PARAMETER_APPLICATION);

                QS._qss_x_.Platform_.IApplication application = null;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(QS._qss_x_.Platform_.IApplication).IsAssignableFrom(type))
                        {
                            object[] _customattributes = type.GetCustomAttributes(typeof(QS._qss_x_.Platform_.ApplicationAttribute), false);
                            if (_customattributes != null && _customattributes.Length == 1)
                            {
                                QS._qss_x_.Platform_.ApplicationAttribute _customattribute = _customattributes[0] as QS._qss_x_.Platform_.ApplicationAttribute;
                                if (_customattribute != null && _customattribute.Name.Equals(_application))
                                {
                                    ConstructorInfo constructorinfo = type.GetConstructor(Type.EmptyTypes);
                                    if (constructorinfo != null)
                                    {
                                        application = constructorinfo.Invoke(new object[] { }) as QS._qss_x_.Platform_.IApplication;
                                        if (application == null)
                                            throw new Exception("Could not instantiate application \"" + _application + "\" from class " + type.FullName);

                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (application != null)
                        break;
                }

                if (application == null)
                    throw new Exception("Could not locate application \"" + _application.ToString() + "\" in any of the loaded assemblies.");

                #endregion

                #region Load machines file

                string machinesfile = rootdir +
                    (parameters.ContainsKey(PARAMETER_MACHINES) ? parameters[PARAMETER_MACHINES] : DEFAULT_MACHINES);
                parameters.Remove(PARAMETER_MACHINES);

                string[] machines;
                using (StreamReader reader = new StreamReader(machinesfile))
                {
                    List<string> _machines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        string machine = reader.ReadLine();
                        if (machine != null)
                        {
                            machine = machine.Trim();
                            if (machine.Length > 0)
                                _machines.Add(machine);
                        }
                    }
                    machines = _machines.ToArray();
                }

                #endregion

                #region Handle gui switch

                bool gui = parameters.Remove(PARAMETER_GUI);

                #endregion

                #region Create core

                string tempdir = rootdir + DEFAULT_TEMP;
                if (!Directory.Exists(tempdir))
                    Directory.CreateDirectory(tempdir);

                QS._core_c_.Core.Core core = new QS._core_c_.Core.Core(tempdir);

                #endregion

                #region Read engine parameters

                string engineconfigfile = rootdir + (parameters.ContainsKey(PARAMETER_ENGINECONFIG) ? parameters[PARAMETER_ENGINECONFIG] : DEFAULT_ENGINECONFIG);
                parameters.Remove(PARAMETER_ENGINECONFIG);

                Dictionary<string, string> engine_parameters = new Dictionary<string, string>();
                Configuration _engineparameters;
                using (StreamReader reader = new StreamReader(engineconfigfile))
                    _engineparameters = (Configuration)(new XmlSerializer(typeof(Configuration))).Deserialize(reader);
                foreach (Parameter _parameter in _engineparameters.Parameters)
                    engine_parameters.Add(_parameter.Name, _parameter.Value);

                #endregion

                #region Process engine configuration options

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_ALARM_QUANTUM))
                    core.MaximumQuantumForAlarms = Convert.ToDouble(engine_parameters[PARAMETER_ENGINE_ALARM_QUANTUM]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_IO_QUANTUM))
                    core.MaximumQuantumForCompletionPorts = Convert.ToDouble(engine_parameters[PARAMETER_ENGINE_IO_QUANTUM]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_DEFAULT_UNICAST_RATE))
                    core.DefaultMaximumSenderUnicastRate = Convert.ToDouble(engine_parameters[PARAMETER_ENGINE_DEFAULT_UNICAST_RATE]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_DEFAULT_MULTICAST_RATE))
                    core.DefaultMaximumSenderMulticastRate = Convert.ToDouble(engine_parameters[PARAMETER_ENGINE_DEFAULT_MULTICAST_RATE]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_FC_UNICAST_CREDITS))
                    core.DefaultMaximumSenderUnicastCredits = Convert.ToDouble(engine_parameters[PARAMETER_ENGINE_FC_UNICAST_CREDITS]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_FC_MULTICAST_CREDITS))
                    core.DefaultMaximumSenderMulticastCredits = Convert.ToDouble(engine_parameters[PARAMETER_ENGINE_FC_MULTICAST_CREDITS]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_TIMEWARPS_CONTINUEIO))
                    core.ContinueIOOnTimeWarps = Convert.ToBoolean(engine_parameters[PARAMETER_ENGINE_TIMEWARPS_CONTINUEIO]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_MAXCC))
                    core.MaximumConcurrency = Convert.ToInt32(engine_parameters[PARAMETER_ENGINE_MAXCC]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_MINTX))
                    core.MinimumTransmitted = Convert.ToInt32(engine_parameters[PARAMETER_ENGINE_MINTX]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_MAXTX))
                    core.MaximumTransmitted = Convert.ToInt32(engine_parameters[PARAMETER_ENGINE_MAXTX]);

                #endregion

                #region Create platform

                string filesystemdir = rootdir + DEFAULT_FSROOT;
                QS._qss_x_.Platform_.PhysicalPlatform platform = new PhysicalPlatform(output, null, core, filesystemdir);

                #endregion

                #region Read application parameters

                string appconfigfile = rootdir + (parameters.ContainsKey(PARAMETER_APPCONFIG) ? parameters[PARAMETER_APPCONFIG] : DEFAULT_APPCONFIG);
                parameters.Remove(PARAMETER_APPCONFIG);

                Dictionary<string, string> application_parameters = new Dictionary<string, string>();
                Configuration _applicationparameters;
                using (StreamReader reader = new StreamReader(engineconfigfile))
                    _applicationparameters = (Configuration)(new XmlSerializer(typeof(Configuration))).Deserialize(reader);
                foreach (Parameter _parameter in _applicationparameters.Parameters)
                    application_parameters.Add(_parameter.Name, _parameter.Value);

                #endregion

                #region Create application context

                QS._qss_x_.Platform_.ApplicationContext context = new QS._qss_x_.Platform_.ApplicationContext(machines, application_parameters);

                #endregion

                #region Start engine and the application

                if (verbose)
                    output.Log("__________Engine : Start");
                core.Start();

                if (verbose)
                    output.Log("__________Application : Start");
                application.Start(platform, context);

                #endregion

                #region Work

                double time =
                    Convert.ToDouble(parameters.ContainsKey(PARAMETER_TIME) ? parameters[PARAMETER_TIME] : DEFAULT_TIME);

                DateTime endtime = DateTime.Now + TimeSpan.FromSeconds(time);
                while (true)
                {
                    DateTime now = DateTime.Now;
                    if (now >= endtime)
                        break;
                    Thread.Sleep(endtime - now);
                }

                #endregion

                #region Stop engine and the application

                if (verbose)
                    output.Log("__________Application : Stop");
                application.Stop();

                if (verbose)
                    output.Log("__________Application : Dispose");
                application.Dispose();

                if (verbose)
                    output.Log("__________Engine : Stop");
                core.Stop();

                if (verbose)
                    output.Log("__________Engine : Dispose");
                core.Dispose();

                #endregion

                #region Exit

                if (verbose)
                    output.Log("__________Exiting");

                ExitProcess(0);

                #endregion
            }
            catch (Exception exc)
            {
                #region Exit

                output.Log("\n\n" + (new string('-', 40)) + "\nUNHANDLED EXCEPTION:\n" + (new string('-', 40)) + "\n\n" + exc.ToString());

                ExitProcess(-1);

                #endregion
            }
        }

        #endregion

        #region Class Parameter

        [XmlType("Parameter")]
        public sealed class Parameter
        {
            public Parameter()
            {
            }

            public Parameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            private string name, value;

            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            [XmlAttribute("value")]
            public string Value
            {
                get { return value; }
                set { this.value = value; }
            }
        }

        #endregion

        #region Class Parameters

        [XmlType("Parameters")]
        public sealed class Configuration
        {
            public Configuration()
            {
            }

            private Parameter[] parameters;

            [XmlElement("parameter")]
            public Parameter[] Parameters
            {
                get { return parameters; }
                set { parameters = value; }
            }            
        }

        #endregion
    }
}

// --@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


// --@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*
                monitoring_activity = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "monitoring_activity");
                monitoring_interval = QS.TMS.Experiment.Helpers.Args.DoubleOf(args, "monitoring_interval", 10);

#pragma warning disable 0162
                // some check on ifdefs
                if (QS.CMS.Rings6.Receiver.ProcessingCrashes != QS.CMS.Rings6.Agent.ProcessingCrashes
                    || QS.CMS.Rings6.Receiver.ProcessingCrashes != QS.CMS.Rings6.AgentCore.ProcessingCrashes)
                    throw new Exception("Crahs processing ifdefs are inconsistent.");
#pragma warning restore 0162

                bool activate_fd = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "fd_enabled");
                // bool dedicated_gms = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "dedicated_gms");
 
  
                framework = new QS.CMS.Framework.FrameworkOnCore(
                    new QS.CMS.QS._core_c_.Base3.InstanceID(localAddress, incarnation), coordinatorAddress, 
                    logger, platform.EventLogger, core, fsroot, activate_fd,
                    (args.contains("MTU") ? Convert.ToInt32((string)args["MTU"]) : 0),
                    QS.TMS.Experiment.Helpers.Args.BoolOf(args, "gms_groupalloc"));

                 if (engine_parameters.ContainsKey(PARAMETER_ENGINE_UNICAST_SENDER_CC))
                    framework.Root.DefaultUnicastSenderConcurrency = Convert.ToInt32((string)args["unicast_sender_cc"]);

                if (engine_parameters.ContainsKey(PARAMETER_ENGINE_MULTICAST_SENDER_CC))
                    framework.Root.DefaultMulticastSenderConcurrency = Convert.ToInt32((string)args["multicast_sender_cc"]);

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
*/
