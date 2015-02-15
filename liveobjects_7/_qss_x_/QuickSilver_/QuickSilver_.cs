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

#define DEBUG_UplinkImplementedOverSimpleTcpChannel

using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_x_.QuickSilver_
{
    public sealed class QuickSilver_ : IDisposable
    {
        #region Constants

        private const string _c_configuration_alarm_quantum = "alarm_quantum";
        private const string _c_configuration_buffering_unrecognized = "buffering_unrecognized";
        private const string _c_configuration_default_multicast_rate = "default_multicast_rate";
        private const string _c_configuration_default_unicast_rate = "default_unicast_rate";
        private const string _c_configuration_disable_ipmulticast_loopback = "disable_ipmulticast_loopback";
        private const string _c_configuration_disable_software_loopback = "disable_software_loopback";
        private const string _c_configuration_drain_multicast = "drain_multicast";
        private const string _c_configuration_drain_unicast = "drain_unicast";
        private const string _c_configuration_enable_hybrid_multicast = "enable_hybrid_multicast";
        private const string _c_configuration_fc_multicast_credits = "fc_multicast_credits";
        private const string _c_configuration_fc_unicast_credits = "fc_unicast_credits";
        private const string _c_configuration_fd_enabled = "fd_enabled";
        private const string _c_configuration_fd_heartbeat_interval = "fd_heartbeat_interval";
        private const string _c_configuration_fd_heartbeat_maxmissed = "fd_heartbeat_maxmissed";
        private const string _c_configuration_fd_heartbeat_timeout = "fd_heartbeat_timeout";
        private const string _c_configuration_gms_address = "gms_address";
        private const string _c_configuration_gms_batching_time = "gms_batching_time";
        private const string _c_configuration_gms_groupalloc = "gms_groupalloc";
        private const string _c_configuration_gs_feed_buffermax = "gs_feed_buffermax";
        private const string _c_configuration_gs_feed_buffermin = "gs_feed_buffermin";
        private const string _c_configuration_gs_max_pending_ack = "gs_max_pending_ack";
        private const string _c_configuration_io_quantum = "io_quantum";
        private const string _c_configuration_maximum_unrecognized = "maximum_unrecognized";
        private const string _c_configuration_mtu = "mtu";
        private const string _c_configuration_multicast_sender_cc = "multicast_sender_cc";
        private const string _c_configuration_nbuffers_multicast = "nbuffers_multicast";
        private const string _c_configuration_nbuffers_unicast = "nbuffers_unicast";
        private const string _c_configuration_port = "port";
        private const string _c_configuration_process_affinity = "process_affinity";
        private const string _c_configuration_process_priorityclass = "process_priorityclass";
        private const string _c_configuration_qsm = "qsm";
        private const string _c_configuration_qsm_application = "qsm_application";
        private const string _c_configuration_qsm_controller = "qsm_controller";
        private const string _c_configuration_qsm_controller_channels = "qsm_controller_channels";
        private const string _c_configuration_rs_ack_with_table = "rs_ack_with_table";
        private const string _c_configuration_rs_adjuster_growth = "rs_adjuster_growth";
        private const string _c_configuration_rs_adjuster_intertia = "rs_adjuster_intertia";
        private const string _c_configuration_rs_adjuster_maxadd = "rs_adjuster_maxadd";
        private const string _c_configuration_rs_adjuster_maxaddcalc = "rs_adjuster_maxaddcalc";
        private const string _c_configuration_rs_adjuster_maxzoom = "rs_adjuster_maxzoom";
        private const string _c_configuration_rs_adjuster_maxzoomcalc = "rs_adjuster_maxzoomcalc";
        private const string _c_configuration_rs_avoid_token_convoys = "rs_avoid_token_convoys";
        private const string _c_configuration_rs_check_partition_naks = "rs_check_partition_naks";
        private const string _c_configuration_rs_do_not_recreate_token = "rs_do_not_recreate_token";
        private const string _c_configuration_rs_forwarding = "rs_forwarding";
        private const string _c_configuration_rs_growth_coefficient = "rs_growth_coefficient";
        private const string _c_configuration_rs_max_naks = "rs_max_naks";
        private const string _c_configuration_rs_max_window = "rs_max_window";
        private const string _c_configuration_rs_maxinc = "rs_maxinc";
        private const string _c_configuration_rs_minimum_token_interarrival = "rs_minimum_token_interarrival";
        private const string _c_configuration_rs_minimum_token_rate = "rs_minimum_token_rate";
        private const string _c_configuration_rs_multiplier = "rs_multiplier";
        private const string _c_configuration_rs_naks = "rs_naks";
        private const string _c_configuration_rs_pullcaching = "rs_pullcaching";
        private const string _c_configuration_rs_rate = "rs_rate";
        private const string _c_configuration_rs_ratecontroller = "rs_ratecontroller";
        private const string _c_configuration_rs_ratesharing = "rs_ratesharing";
        private const string _c_configuration_rs_replication = "rs_replication";
        private const string _c_configuration_rs_timeout = "rs_timeout";
        private const string _c_configuration_rs_token_rate = "rs_token_rate";
        private const string _c_configuration_rs_warmuptime = "rs_warmuptime";
        private const string _c_configuration_subnet = "subnet";
        private const string _c_configuration_system_bufsize_multicast_receive = "system_bufsize_multicast_receive";
        private const string _c_configuration_system_bufsize_multicast_send = "system_bufsize_multicast_send";
        private const string _c_configuration_system_bufsize_unicast_receive = "system_bufsize_unicast_receive";
        private const string _c_configuration_system_bufsize_unicast_send = "system_bufsize_unicast_send";
        private const string _c_configuration_timewarps_continueio = "timewarps_continueio";
        private const string _c_configuration_transmission_cc = "transmission_cc";
        private const string _c_configuration_transmission_high = "transmission_high";
        private const string _c_configuration_transmission_low = "transmission_low";
        private const string _c_configuration_unicast_sender_cc = "unicast_sender_cc";
        private const string _c_configuration_unrecognized_timeout = "unrecognized_timeout";
        private const string _c_configuration_uplink_address = "uplink_address";
        private const string _c_configuration_uplink_capacity_inbound_control = "uplink_capacity_inbound_control";
        private const string _c_configuration_uplink_capacity_inbound_data = "uplink_capacity_inbound_data";
        private const string _c_configuration_uplink_capacity_outbound_control = "uplink_capacity_outbound_control";
        private const string _c_configuration_uplink_capacity_outbound_data = "uplink_capacity_outbound_data";
        private const string _c_configuration_uplink_controller = "uplink_controller";
        private const string _c_configuration_uplink_port = "uplink_port";
        private const string _c_configuration_uplink_subnet = "uplink_subnet";
        private const string _c_configuration_verbose = "verbose";

        #endregion

        #region Constructor

        public QuickSilver_
        (
            QS.Fx.Object.IContext _mycontext, 
            QS.Fx.Configuration.IConfiguration _configuration,
            QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Qsm_.QsmControl_>> _connection,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer
        )
        {
            this._mycontext = _mycontext;
            this._configuration = _configuration;
            this._connection = _connection;
            this._deserializer = _deserializer;
            QS.Fx.Configuration.IParameter _parameter;
            this._logger = new QS._core_c_.Base.Logger(QS._core_c_.Core.Clock.SharedClock, true);
            this._logger.Console = new QS._qss_d_.Components_.FileLogger(
                QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + @"\logs\quicksilver.txt");
            this._eventlogger = new QS._qss_c_.Logging_1_.EventLogger(QS._core_c_.Core.Clock.SharedClock, true);
            this._verbose =
                 (this._configuration.TryGetParameter(_c_configuration_verbose, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            uint _process_affinity =
                (this._configuration.TryGetParameter(_c_configuration_process_affinity, out _parameter) && (_parameter.Value.Length > 0)) ? 
                    Convert.ToUInt32(_parameter.Value) : 
                    1;
            try
            {
                uint _affinity = (uint) Process.GetCurrentProcess().ProcessorAffinity.ToInt32();
                _affinity = _affinity & _process_affinity;
                if (_affinity == 0)
                    _affinity = 1;
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(_affinity);
                _affinity = (uint)Process.GetCurrentProcess().ProcessorAffinity.ToInt32();
                if (this._verbose)
                    _logger.Log(this, "Process affinity mask set to " + _affinity.ToString() + ".");                
            }
            catch (Exception _exc)
            {
                _logger.Log(this, "Could not set process affinity mask.");
            }
            ProcessPriorityClass _process_priorityclass =
                (this._configuration.TryGetParameter(_c_configuration_process_priorityclass, out _parameter) && (_parameter.Value.Length > 0)) ?
                    (ProcessPriorityClass) Enum.Parse(typeof(ProcessPriorityClass), _parameter.Value) :
                    ProcessPriorityClass.Normal;
            try
            {
                Process.GetCurrentProcess().PriorityClass = _process_priorityclass;
                if (this._verbose)
                    _logger.Log(this, "Process priority class set to " + Process.GetCurrentProcess().PriorityClass.ToString() + ".");
            }
            catch (Exception _exc)
            {
                _logger.Log(this, "Could not set process priority class.");
            }
            this._myroot = QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + @"\temp\quicksilver." +
                DateTime.Now.ToString("yyyyMMddHHmmssffffff") + Process.GetCurrentProcess().Id.ToString("000000");
            if (!Directory.Exists(this._myroot))
                Directory.CreateDirectory(this._myroot);
            this._core = new QS._core_c_.Core.Core(this._myroot + @"\data");
            this._core.MaximumQuantumForAlarms =
                (this._configuration.TryGetParameter(_c_configuration_alarm_quantum, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToDouble(_parameter.Value) :
                    0.005;
            this._core.MaximumQuantumForCompletionPorts =
                (this._configuration.TryGetParameter(_c_configuration_io_quantum, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToDouble(_parameter.Value) :
                    0.05;
            this._core.DefaultMaximumSenderUnicastRate =
                (this._configuration.TryGetParameter(_c_configuration_default_unicast_rate, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToDouble(_parameter.Value) :
                    300;
            this._core.DefaultMaximumSenderMulticastRate =
                (this._configuration.TryGetParameter(_c_configuration_default_multicast_rate, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToDouble(_parameter.Value) :
                    1000;
            this._core.DefaultMaximumSenderUnicastCredits =
                (this._configuration.TryGetParameter(_c_configuration_fc_unicast_credits, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToDouble(_parameter.Value) :
                    100;
            this._core.DefaultMaximumSenderMulticastCredits =
                (this._configuration.TryGetParameter(_c_configuration_fc_multicast_credits, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToDouble(_parameter.Value) :
                    500;
            this._core.ContinueIOOnTimeWarps =
                (this._configuration.TryGetParameter(_c_configuration_timewarps_continueio, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            this._core.MaximumConcurrency =
                (this._configuration.TryGetParameter(_c_configuration_transmission_cc, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    10000;
            this._core.MinimumTransmitted =
                (this._configuration.TryGetParameter(_c_configuration_transmission_low, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    1000;
            this._core.MaximumTransmitted =
                (this._configuration.TryGetParameter(_c_configuration_transmission_high, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    10000000;
            this._core.ChannelCapacityIncomingControl =
                (this._configuration.TryGetParameter(_c_configuration_uplink_capacity_inbound_control, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    1024;
            this._core.ChannelCapacityIncoming =
                (this._configuration.TryGetParameter(_c_configuration_uplink_capacity_inbound_data, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    1048576;
            this._core.ChannelCapacityOutgoingControl =
                (this._configuration.TryGetParameter(_c_configuration_uplink_capacity_outbound_control, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    1024;
            this._core.ChannelCapacityOutgoing =
                (this._configuration.TryGetParameter(_c_configuration_uplink_capacity_outbound_data, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    1048576;
            this._core.ChannelController =
                (this._configuration.TryGetParameter(_c_configuration_uplink_controller, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            bool _qsm =
                (this._configuration.TryGetParameter(_c_configuration_qsm, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            bool _qsm_application =
                (this._configuration.TryGetParameter(_c_configuration_qsm_application, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            bool _qsm_controller =
                (this._configuration.TryGetParameter(_c_configuration_qsm_controller, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            string _qsm_controller_channels =
                (this._configuration.TryGetParameter(_c_configuration_qsm_controller_channels, out _parameter) && (_parameter.Value.Length > 0)) ?
                _parameter.Value :
                null;
            if (_qsm)
            {
                this._subnet =
                    new QS._qss_c_.Base1_.Subnet(
                        (this._configuration.TryGetParameter(_c_configuration_subnet, out _parameter) && (_parameter.Value.Length > 0)) ?
                            _parameter.Value :
                            "0.0.0.0/0");
                int _port =
                    (this._configuration.TryGetParameter(_c_configuration_port, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        59996;
                if (_port <= 0)
                    throw new Exception("Illegal port number or port number not configured.");
                foreach (IPAddress _ipaddress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if ((_ipaddress.AddressFamily == AddressFamily.InterNetwork) && _subnet.contains(_ipaddress))
                    {
                        this._localaddress = new QS.Fx.Network.NetworkAddress(_ipaddress, _port);
                        break;
                    }
                }
                if (this._localaddress == null)
                    throw new Exception("Could not find any interface attached to the requested subnet " + _subnet.ToString());
                this._instanceid = new QS._core_c_.Base3.InstanceID(this._localaddress, DateTime.Now);
                if (this._verbose)
                    _logger.Log(this, "Complete local instance address is " + _instanceid.ToString());
                this._gmsaddress =
                    (this._configuration.TryGetParameter(_c_configuration_gms_address, out _parameter) && (_parameter.Value.Length > 0)) ?
                        new QS.Fx.Network.NetworkAddress(_parameter.Value) :
                        this._localaddress;
                if (this._verbose)
                {
                    this._logger.Log(this, "GMS address is " + this._gmsaddress.ToString());
                    if (this._gmsaddress.Equals(this._localaddress))
                        this._logger.Log(this, "This node is hosting the GMS.");
                }
                QS._qss_c_.Multicasting7.ReliableRegionViewSink.DisableSoftwareLoopback =
                    (this._configuration.TryGetParameter(_c_configuration_disable_software_loopback, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        false;
                QS._core_c_.Core.Sockets.DisableIPMulticastLoopback =
                    (this._configuration.TryGetParameter(_c_configuration_disable_ipmulticast_loopback, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                bool _fd_enabled =
                    (this._configuration.TryGetParameter(_c_configuration_fd_enabled, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                int _mtu =
                    (this._configuration.TryGetParameter(_c_configuration_mtu, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        65535;
                bool _enable_hybrid_multicast =
                    (this._configuration.TryGetParameter(_c_configuration_enable_hybrid_multicast, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        false;
                this._framework = new QS._qss_c_.Framework_1_.FrameworkOnCore(
                    this._instanceid, this._gmsaddress, this._logger, this._eventlogger, this._core, this._myroot + @"\files",
                    _fd_enabled,
                    _mtu,
                    _enable_hybrid_multicast);
#pragma warning disable 0162
                if (QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.Agent.ProcessingCrashes ||
                    QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.AgentCore.ProcessingCrashes)
                {
                    throw new Exception("Bad build, fix ifdefs for crash processing.");
                }
#pragma warning restore 0162
                double _gms_batching_time =
                    (this._configuration.TryGetParameter(_c_configuration_gms_batching_time, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        1;
                if (this._framework.MembershipServer != null)
                    this._framework.MembershipServer.RequestAggregationInterval = _gms_batching_time;
                int _fd_heartbeat_maxmissed =
                    (this._configuration.TryGetParameter(_c_configuration_fd_heartbeat_maxmissed, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        5;
                double _fd_heartbeat_timeout =
                    (this._configuration.TryGetParameter(_c_configuration_fd_heartbeat_timeout, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        2;
                if (this._framework.FailureDetectionServer != null)
                {
                    this._framework.FailureDetectionServer.MaximumMissed = _fd_heartbeat_maxmissed;
                    this._framework.FailureDetectionServer.HeartbeatTimeout = TimeSpan.FromSeconds(_fd_heartbeat_timeout);
                }
                double _fd_heartbeat_interval =
                    (this._configuration.TryGetParameter(_c_configuration_fd_heartbeat_interval, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        1;
                if (this._framework.FailureDetectionAgent != null)
                    this._framework.FailureDetectionAgent.HeartbeatInterval = TimeSpan.FromSeconds(_fd_heartbeat_interval);
                this._framework.Root.DefaultDrainSynchronouslyForUnicastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_drain_unicast, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                this._framework.Root.DefaultDrainSynchronouslyForMulticastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_drain_multicast, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        false;
                this._framework.Root.DefaultAfdReceiveBufferSizeForUnicastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_system_bufsize_unicast_receive, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        4194304;
                this._framework.Root.DefaultAfdReceiveBufferSizeForMulticastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_system_bufsize_multicast_receive, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        4194304;
                this._framework.Root.DefaultAfdSendBufferSizeForUnicastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_system_bufsize_unicast_send, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        8192;
                this._framework.Root.DefaultAfdSendBufferSizeForMulticastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_system_bufsize_multicast_send, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        1048576;
                this._framework.Root.DefaultNumberOfReceiveBuffersForUnicastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_nbuffers_unicast, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        100;
                this._framework.Root.DefaultNumberOfReceiveBuffersForMulticastAddresses =
                    (this._configuration.TryGetParameter(_c_configuration_nbuffers_multicast, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        100;
                this._framework.Root.DefaultUnicastSenderConcurrency =
                    (this._configuration.TryGetParameter(_c_configuration_unicast_sender_cc, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        50;
                this._framework.Root.DefaultMulticastSenderConcurrency =
                    (this._configuration.TryGetParameter(_c_configuration_multicast_sender_cc, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        500;
                string _rs_ratesharing =
                    (this._configuration.TryGetParameter(_c_configuration_rs_ratesharing, out _parameter) && (_parameter.Value.Length > 0)) ?
                        _parameter.Value :
                        "Compete";
                QS._qss_c_.Rings6.RateSharingAlgorithmClass _ratesharingalgorithm;
                if (_rs_ratesharing.Equals("Compete"))
                    _ratesharingalgorithm = QS._qss_c_.Rings6.RateSharingAlgorithmClass.Compete;
                else if (_rs_ratesharing.Equals("FairShare"))
                    _ratesharingalgorithm = QS._qss_c_.Rings6.RateSharingAlgorithmClass.FairShare;
                else
                    throw new NotImplementedException();
                this._receivingagentclass = new QS._qss_c_.Rings6.ReceivingAgent(
                    this._framework.EventLogger, this._framework.Logger, this._framework.LocalAddress, this._framework.AlarmClock, this._framework.Clock,
                    this._framework.Demultiplexer, this._framework.BufferedReliableInstanceSinks,
                    5, 1, 0.1, 10, 5000, _ratesharingalgorithm, this._framework.ReliableInstanceSinks, this._framework.StatisticsController);
                this._receivingagentclass.TokenRate =
                    (this._configuration.TryGetParameter(_c_configuration_rs_token_rate, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        1;
                this._receivingagentclass.ReplicationCoefficient =
                    (this._configuration.TryGetParameter(_c_configuration_rs_replication, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToUInt32(_parameter.Value) :
                        5;
                this._receivingagentclass.PullCaching =
                    (this._configuration.TryGetParameter(_c_configuration_rs_pullcaching, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                this._receivingagentclass.NaksAllowed =
                    (this._configuration.TryGetParameter(_c_configuration_rs_naks, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                this._receivingagentclass.ForwardingAllowed =
                    (this._configuration.TryGetParameter(_c_configuration_rs_forwarding, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                this._receivingagentclass.MaximumWindowWidth =
                    (this._configuration.TryGetParameter(_c_configuration_rs_max_window, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToUInt32(_parameter.Value) :
                        1000000;
                this._receivingagentclass.MaximumNakRangesPerToken =
                    (this._configuration.TryGetParameter(_c_configuration_rs_max_naks, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToUInt32(_parameter.Value) :
                        20;
                this._regionalcontroller = new QS._qss_c_.Receivers4.RegionalController(
                    this._framework.LocalAddress, this._framework.Logger, this._framework.Demultiplexer, this._framework.AlarmClock, this._framework.Clock,
                    this._framework.MembershipController, this._receivingagentclass, this._receivingagentclass);
                this._regionalcontroller.IsDisabled = false;
                bool _buffering_unrecognized =
                    (this._configuration.TryGetParameter(_c_configuration_buffering_unrecognized, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToBoolean(_parameter.Value) :
                        true;
                int _maximum_unrecognized =
                    (this._configuration.TryGetParameter(_c_configuration_maximum_unrecognized, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        30000;
                double _unrecognized_timeout =
                    (this._configuration.TryGetParameter(_c_configuration_unrecognized_timeout, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        3;
                double _rs_timeout =
                    (this._configuration.TryGetParameter(_c_configuration_rs_timeout, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        300;
                double _rs_warmuptime =
                    (this._configuration.TryGetParameter(_c_configuration_rs_warmuptime, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        3;
                this._regionalsenders = new QS._qss_c_.Senders10.RegionalSenders(
                    this._framework.EventLogger, this._framework.LocalAddress, this._framework.Logger, this._framework.AlarmClock, this._framework.Clock,
                    this._framework.Demultiplexer, null, // this null argument needs to be fixed ,
                    this._regionalcontroller, this._regionalcontroller, _rs_timeout, this._regionalcontroller,
                    _buffering_unrecognized, _maximum_unrecognized, _unrecognized_timeout);
                this._dispatcherrv2 = new QS._qss_c_.Multicasting7.DispatcherRV2(this._logger, this._framework.AlarmClock, this._framework.Clock,
                    this._framework.Demultiplexer, (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, this._regionalcontroller,
                    this._framework.MembershipController);
                double _rs_rate =
                    (this._configuration.TryGetParameter(_c_configuration_rs_rate, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        5000;
                int _rs_ratecontroller =
                    (this._configuration.TryGetParameter(_c_configuration_rs_ratecontroller, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        3;
                double _rs_growth_coefficient =
                    (this._configuration.TryGetParameter(_c_configuration_rs_growth_coefficient, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToDouble(_parameter.Value) :
                        0.1;
                QS._qss_c_.Base1_.IFactory<QS._qss_c_.FlowControl7.IRateController> _ratecontrollers = null;
                switch (_rs_ratecontroller)
                {
                    case 1:
                        _ratecontrollers = new QS._qss_c_.FlowControl7.DummyController1(_rs_rate);
                        break;
                    case 2:
                        _ratecontrollers = new QS._qss_c_.FlowControl7.DummyController2(this._framework.Clock, _rs_rate);
                        break;
                    case 3:
                        _ratecontrollers = new QS._qss_c_.FlowControl7.RateController1(this._framework.Clock, _rs_growth_coefficient);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                this._nodesinkcollection = this._framework.Root;
                this._regionsinkcollection = new QS._qss_c_.Multicasting7.RegionSinks(this._framework.MembershipController, this._nodesinkcollection);
                this._regionviewsinkcollection = new QS._qss_c_.Multicasting7.ReliableRegionViewSinks(
                    this._framework.StatisticsController, this._framework.Logger, this._framework.EventLogger, this._framework.LocalAddress,
                    this._framework.AlarmClock, this._framework.Clock, (uint)QS.ReservedObjectID.Rings6_SenderController1_DataChannel,
                    (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel,
                    this._regionsinkcollection, this._regionalcontroller, this._regionalcontroller, this._framework.MembershipController, this._framework.Root,
                    _rs_timeout, _ratecontrollers, _rs_warmuptime);
                int _gs_max_pending_ack =
                    (this._configuration.TryGetParameter(_c_configuration_gs_max_pending_ack, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        100000;
                int _gs_feed_buffermin =
                    (this._configuration.TryGetParameter(_c_configuration_gs_feed_buffermin, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        50;
                int _gs_feed_buffermax =
                    (this._configuration.TryGetParameter(_c_configuration_gs_feed_buffermax, out _parameter) && (_parameter.Value.Length > 0)) ?
                        Convert.ToInt32(_parameter.Value) :
                        150;
                this._groupsinkcollection =
                    new QS._qss_c_.Multicasting7.ReliableGroupSinks(
                        this._framework.StatisticsController, this._framework.Clock, this._framework.MembershipController,
                        this._regionviewsinkcollection, this._logger, _gs_max_pending_ack, _gs_feed_buffermin, _gs_feed_buffermax);
                if (_enable_hybrid_multicast)
                {
                    this._hybridgroupsinkcollection =
                        new QS._qss_c_.Multicasting7.AlternativeReliableGroupSinks(
                            this._framework.StatisticsController, this._framework.Clock, this._framework.MembershipController, this._logger,
                            this._nodesinkcollection, ((QS._qss_c_.Multicasting7.ReliableRegionViewSinks)this._regionviewsinkcollection),
                            (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, this._framework.Root, null, _rs_rate);
                }
            }
            QS._core_c_.Diagnostics2.Helper.RegisterLocal(this._diagnosticscontainer, this);
            this._clock = QS._core_c_.Core.Clock.SharedClock;
/*
            ((QS._qss_c_.Membership2.Consumers.IGroupCreationAndRemovalProvider)framework.MembershipController).OnChange +=
                new QS._qss_c_.Membership2.Consumers.GroupCreationOrRemovalCallback(this.GroupCreationOrRemovalCallback);
*/
            if (this._verbose)
                this._logger.Log(this, "All initialization completed, starting the core thread.");
/*
            framework.Demultiplexer.register((uint) ReservedObjectID.Framework2_Group, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
*/
            this._core.Start();
/*
            this._completionworker = new QS._qss_c_.Synchronization_1_.NonblockingWorker<QS._core_c_.Core.IRequest>(
                new QS.Fx.Base.ContextCallback<QS._core_c_.Core.IRequest>(this._CompletionCallback));
*/
            string _uplink_address =
                (this._configuration.TryGetParameter(_c_configuration_uplink_address, out _parameter) && (_parameter.Value.Length > 0)) ?
                    _parameter.Value :
                    null;
            int _uplink_port =
                (this._configuration.TryGetParameter(_c_configuration_uplink_port, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    0;
            string _uplink_subnet =
                (this._configuration.TryGetParameter(_c_configuration_uplink_subnet, out _parameter) && (_parameter.Value.Length > 0)) ?
                    _parameter.Value :
                    null;
            bool _uplink_controller =
                (this._configuration.TryGetParameter(_c_configuration_uplink_controller, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToBoolean(_parameter.Value) :
                    false;
            if (_qsm_application)
            {
                this._qsmapplication = new QS._qss_x_.Qsm_.QsmApplication_(_mycontext, this, this._deserializer);
#if DEBUG_UplinkImplementedOverSimpleTcpChannel
                QS._qss_x_.Uplink_1_.Uplink_ _uplink = 
                    new QS._qss_x_.Uplink_1_.Uplink_
                    (
                        _mycontext,
                        this,
                        _uplink_address, 
                        _uplink_subnet, 
                        this._qsmapplication
                    );
#else
                QS._qss_x_.Uplink_2_.Uplink_ _uplink = 
                    new QS._qss_x_.Uplink_2_.Uplink_(this._core, _uplink_port, this._qsmapplication);
#endif
                this._qsmapplication._Connect(_uplink);
                this._disposable.Add(_uplink);
            }
            if (_qsm)
            {
                this._qsmcontroller =
                    new QS._qss_x_.Qsm_.QsmController_(_mycontext, this, _qsm_controller, _qsm_controller_channels, this._connection, this._deserializer);
                if (_uplink_controller)
                {
#if DEBUG_UplinkImplementedOverSimpleTcpChannel
                    QS._qss_x_.Uplink_1_.UplinkController_ _uplinkcontroller =
                        new QS._qss_x_.Uplink_1_.UplinkController_
                        (
                            _mycontext,
                            this,
                            _uplink_address,
                            _uplink_subnet,
                            _uplink_port,
                            this._qsmcontroller
                        );
#else
                    QS._qss_x_.Uplink_2_.UplinkController_ this._uplinkcontroller = 
                        new QS._qss_x_.Uplink_2_.UplinkController_(this._core, _uplink_port, this._qsm);
#endif
                    this._disposable.Add(_uplinkcontroller);
                }
            }
        }

        #endregion

        #region Destructor

        ~QuickSilver_()
        {
            this._Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                    foreach (IDisposable _disposable in this._disposable)
                    {
                        try
                        {
                            _disposable.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
/*
                    ((IDisposable) this._completionworker).Dispose();
*/
                    this._core.Stop();
                    this._core.Dispose();
                }
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Configuration.IConfiguration _configuration;
        private QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Qsm_.QsmControl_>> _connection;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        private int _disposed;
        private QS._core_c_.Base.Logger _logger;
        private QS._qss_c_.Logging_1_.EventLogger _eventlogger;
        private bool _verbose;
        private QS._qss_c_.Base1_.Subnet _subnet;
        private QS.Fx.Network.NetworkAddress _localaddress, _gmsaddress;
        private QS._core_c_.Base3.InstanceID _instanceid;
        private QS.Fx.Clock.IClock _clock;
        private string _myroot;
        private QS._core_c_.Diagnostics2.Container _diagnosticscontainer = new QS._core_c_.Diagnostics2.Container();
        private QS._qss_c_.Rings6.ReceivingAgent _receivingagentclass;
        private QS._qss_c_.Multicasting7.DispatcherRV2 _dispatcherrv2;
        private QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _nodesinkcollection;
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RegionID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _regionsinkcollection;
        [QS._core_c_.Diagnostics.Component("Core")]
        [QS._core_c_.Diagnostics2.Module("Core")]
        private QS._core_c_.Core.Core _core;
        [QS._core_c_.Diagnostics2.Module("Framework")]
        private QS._qss_c_.Framework_1_.FrameworkOnCore _framework;
        [QS._core_c_.Diagnostics2.Module("RegionalController")]
        private QS._qss_c_.Receivers4.RegionalController _regionalcontroller;
        [QS._core_c_.Diagnostics2.Module("RegionalSenders")]
        private QS._qss_c_.Senders10.RegionalSenders _regionalsenders;
        [QS._core_c_.Diagnostics2.Module("ReliableRegionViewSinks")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RVID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _regionviewsinkcollection;
        [QS._core_c_.Diagnostics2.Module("ReliableGroupSinks_Regular")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _groupsinkcollection;
        // [QS.CMS.QS._core_c_.Diagnostics2.Module("ReliableGroupSinks_Hybrid")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _hybridgroupsinkcollection;
        private QS._qss_x_.Qsm_.QsmApplication_ _qsmapplication;
        private QS._qss_x_.Qsm_.QsmController_ _qsmcontroller;
        private List<IDisposable> _disposable = new List<IDisposable>();

/*
        private QS._core_c_.Synchronization.INonblockingWorker<QS._core_c_.Core.IRequest> _completionworker;
        private IDictionary<Base3_.GroupID, Group> groups = new Dictionary<Base3_.GroupID, Group>();
        private ICollection<Group> groupsToSubscribe = new System.Collections.ObjectModel.Collection<Group>();
        private ICollection<Group> groupsToUnsubscribe = new System.Collections.ObjectModel.Collection<Group>();
        private bool waitingToIssueMembershipChangeRequest;
        private QS.Fx.Clock.IAlarm deferredMembershipChangeRequestAlarm;
*/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Core

        public QS._core_c_.Core.Core _Core
        {
            get { return this._core; }
        }

        #endregion

        #region _Framework

        public QS._qss_c_.Framework_1_.FrameworkOnCore _Framework
        {
            get { return this._framework; }
        }

        #endregion

        #region _GroupSinks

        public QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _GroupSinks
        {
            get { return this._groupsinkcollection; }
        }

        #endregion

        #region _HybridGroupSinks

        public QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> _HybridGroupSinks
        {
            get { return this._hybridgroupsinkcollection; }
        }

        #endregion

        #region _QsmApplication

        public QS._qss_x_.Qsm_.QsmApplication_ _QsmApplication
        {
            get { return this._qsmapplication; }
        }

        #endregion

        #region _QsmController

        public QS._qss_x_.Qsm_.QsmController_ _QsmController
        {
            get { return this._qsmcontroller; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

//        #region _CompletionCallback
//
//        private void _CompletionCallback(QS._core_c_.Core.IRequest _request)
//        {
//            _request.Process();
//        }
//
//        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
