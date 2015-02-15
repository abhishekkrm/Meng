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

namespace QS._qss_c_.Framework_2_
{
    /// <summary>
    /// Configuration settings for a local QSM client.
    /// </summary>
    [QS.Fx.Printing.Printable("Client Configuration", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Implicit)]
    [Serializable]
    public sealed class ClientConfiguration
    {
        #region Constructors 

        public ClientConfiguration(string local_subnet, uint port_number, string gms_address) : this()
        {
            this.subnet = local_subnet;
            this.portno = port_number;
            this.gmsAddress = gms_address;
        }

        public ClientConfiguration(string local_subnet, string gms_address) : this()
        {
            this.subnet = local_subnet;
            this.gmsAddress = gms_address;
        }

        public ClientConfiguration(uint port_number, string gms_address) : this()
        {
            this.portno = port_number;
            this.gmsAddress = gms_address;
        }

        public ClientConfiguration(uint port_number) : this()
        {
            this.portno = port_number;
        }

        public ClientConfiguration(string gms_address) : this()
        {
            this.gmsAddress = gms_address;
        }

        public ClientConfiguration()
        {
            subnet = "x.x.x.x";
            portno = gmsPort = 65000;
            repositoryPath = "C:\\QuickSilver\\Temp";
            mtu = 20480;
            gmsMaxHeartbeatsMissed = 10;
            gmsHeartbeatTimeout = 1;
            heartbeatInterval = 1;
            drainunicast = true;
            numOfBuffersForUnicast = 100;
            numOfBuffersForMulticast = 100;
            sizeOfAdfSendBuffersForUnicast = 8192;
            sizeOfAdfSendBuffersForMulticast = 1048576;
            sizeOfAdfReceiveBuffersForUnicast = 4194304;
            sizeOfAdfReceiveBuffersForMulticast = 4194304;
            alarm_quantum = 0.005;
            io_quantum = 0.05;
            default_mcrate = 10000;
            default_ucrate = 300;
            defaultMaximumSenderUnicastCredits = 100;
            defaultMaximumSenderMulticastCredits = 500;
            defaultUnicastSenderConcurrency = 50;
            defaultMulticastSenderConcurrency = 500;
            token_rate = 5;
            replication_coefficient = 5;
            nakWindowWidth = 1000000;
            maximumNakRangesPerToken = 20;
            max_unrecognized_to_buffer = 30000;
            unrecognized_retry_timeout = 3;
            multicastRetransmissionTimeout = 100;
            sender_delay = 0.2;
            maximumUnacknowledged = 100000;
            feed_buffer_min = 50;
            feed_buffer_max = 150;
            processAffinity = 1;

            client_membership_batching_interval = 0.05;
            gmsBatchingTime = 0.1;

            // verbose = true;
            // logToConsole = true;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.NonPrintable]
        private string subnet, gmsAddress, repositoryPath;
        [QS.Fx.Printing.NonPrintable]
        private uint portno, incarnation, mtu, gmsMaxHeartbeatsMissed, numOfBuffersForUnicast, numOfBuffersForMulticast,
            sizeOfAdfSendBuffersForUnicast, sizeOfAdfSendBuffersForMulticast, sizeOfAdfReceiveBuffersForUnicast, sizeOfAdfReceiveBuffersForMulticast,
            defaultMaximumSenderUnicastCredits, defaultMaximumSenderMulticastCredits, defaultUnicastSenderConcurrency, defaultMulticastSenderConcurrency,
            coreMaximumConcurrency, coreMinimumTransmitted, coreMaximumTransmitted, replication_coefficient, nakWindowWidth, maximumNakRangesPerToken,
            max_unrecognized_to_buffer, maximumUnacknowledged, feed_buffer_min, feed_buffer_max, gmsPort, processAffinity;
        [QS.Fx.Printing.NonPrintable]
        private bool disable_fd, drainunicast, drainmulticast, continueIOOnTimeWarps, disablePullCaching, disableNaks, 
            disableForwarding, do_not_buffer_unrecognized, enableHybridMulticast, logToConsole, logToFile, verbose, openLogWindow,
            disableIPMulticastLoopback = true, disableSoftwareLoopback = false; // allocateIPMulticastAddressPerGroup
        [QS.Fx.Printing.NonPrintable]
        private double gmsBatchingTime, gmsHeartbeatTimeout, heartbeatInterval, alarm_quantum, io_quantum, default_mcrate, default_ucrate, token_rate,
            unrecognized_retry_timeout, multicastRetransmissionTimeout, sender_delay, client_membership_batching_interval;
        [QS.Fx.Printing.NonPrintable]
        private QS.Fx.Logging.IConsole log_to;

        #endregion

        #region Accessors

        public bool DisableMulticastLoopback        
        {
            get { return disableIPMulticastLoopback; }
            set { disableIPMulticastLoopback = value; }
        }

        public bool DisableSoftwareLoopback
        {
            get { return disableSoftwareLoopback; }
            set { disableSoftwareLoopback = value; }
        }

        /// <summary>
        /// The subnet at which the network interface to use is located. The accepted formats include four octets separated by dots and with "x"
        /// representing the wildcard (e.g. 192.168.0.x, 172.23.x.x), and ip address and mask separated by a colon, (e.g. 192.168.0.0:255.255.255.0).
        /// </summary>
        public string Subnet
        {
            get { return subnet; }
            set { subnet = value; }
        }

        /// <summary>
        /// Port number to use for the main receive socket at this QSM instance.
        /// </summary>
        public uint Port
        {
            get { return portno; }
            set { portno = value; }
        }

        /// <summary>
        /// Current incarnation number. Incarnation numbers for the same ip/port pair must be increasing, 
        /// although do not need to do so at unit inrements. If not specified, the incarnation number is generated automatically
        /// from the local time (with the precision down to seconds).
        /// </summary>
        public uint Incarnation
        {
            get { return incarnation; }
            set { incarnation = value; }
        }

        /// <summary>
        /// IP address of the GMS service. If not set, the local instance will host such service.
        /// </summary>
        public string GMSAddress
        {
            get { return gmsAddress; }
            set { gmsAddress = value; }
        }

        /// <summary>
        /// Port for the GMS service. If the local instance hosts the GMS, this port must be left unassigned or 
        /// same as the value assigned to the Port property. 
        /// </summary>
        public uint GMSPort
        {
            get { return gmsPort; }
            set { gmsPort = value; }
        }

        /// <summary>
        /// A local path where QSM can save its traces and other temporary working files.
        /// </summary>
        public string WorkingPath
        {
            get { return repositoryPath; }
            set { repositoryPath = value; }
        }

        /// <summary>
        /// When this option is set, no heartbeats will be sent to the failure detector and the process can crash without being detected.
        /// </summary>
        public bool DisableFailureDetection
        {
            get { return disable_fd; }
            set { disable_fd = value; }
        }

        /// <summary>
        /// Maximum size of data that can be sent over the network. Must be less than 64K. 
        /// The larger the value, the more memory used for receive buffers.
        /// </summary>
        public uint MTU
        {
            get { return mtu; }
            set { mtu = value; }
        }

//        public bool GMSAllocateIPMulticastAddressPerGroup
//        {
//            get { return allocateIPMulticastAddressPerGroup; }
//            set { allocateIPMulticastAddressPerGroup = value; }
//        }

        public double GMSBatchingTime
        {
            get { return gmsBatchingTime; }
            set { gmsBatchingTime = value; }
        }

        public uint GMSMaxHeartbeatsMissed
        {
            get { return gmsMaxHeartbeatsMissed; }
            set { gmsMaxHeartbeatsMissed = value; }
        }

        public double GMSHeartbeatTimeout
        {
            get { return gmsHeartbeatTimeout; }
            set { gmsHeartbeatTimeout = value; }
        }

        public double HeartbeatInterval
        {
            get { return heartbeatInterval; }
            set { heartbeatInterval = value; }
        }

        public bool SynchronouslyDrainUnicast
        {
            get { return drainunicast; }
            set { drainunicast = value; }
        }

        public bool SynchronouslyDrainMulticast
        {
            get { return drainmulticast; }
            set { drainmulticast = value; }
        }

        public uint NumberOfReceiveBuffersForUnicast
        {
            get { return numOfBuffersForUnicast; }
            set { numOfBuffersForUnicast = value; }
        }

        public uint NumberOfReceiveBuffersForMulticast
        {
            get { return numOfBuffersForMulticast; }
            set { numOfBuffersForMulticast = value; }
        }

        public uint SizeOfAdfSendBuffersForUnicast
        {
            get { return sizeOfAdfSendBuffersForUnicast; }
            set { sizeOfAdfSendBuffersForUnicast = value; }
        }

        public uint SizeOfAdfSendBuffersForMulticast
        {
            get { return sizeOfAdfSendBuffersForMulticast; }
            set { sizeOfAdfSendBuffersForMulticast = value; }
        }

        public uint SizeOfAdfReceiveBuffersForUnicast
        {
            get { return sizeOfAdfReceiveBuffersForUnicast; }
            set { sizeOfAdfReceiveBuffersForUnicast = value; }
        }

        public uint SizeOfAdfReceiveBuffersForMulticast
        {
            get { return sizeOfAdfReceiveBuffersForMulticast; }
            set { sizeOfAdfReceiveBuffersForMulticast = value; }
        }

        /// <summary>
        /// The length of the alarm quantum in seconds.
        /// </summary>
        public double AlarmQuantum
        {
            get { return alarm_quantum; }
            set { alarm_quantum = value; }
        }

        /// <summary>
        /// The length of the I/O quantum in seconds.
        /// </summary>
        public double IOQuantum
        {
            get { return io_quantum; }
            set { io_quantum = value; }
        }

        /// <summary>
        /// Default maximum sending rate for unicast (control) traffic.
        /// </summary>
        public double DefaultMaximumRateForUnicast
        {
            get { return default_ucrate; }
            set { default_ucrate = value; }
        }

        /// <summary>
        /// Default maximum sending rate for multicast traffic.
        /// </summary>
        public double DefaultMaximumRateForMulticast
        {
            get { return default_mcrate; }
            set { default_mcrate = value; }
        }

        public uint DefaultMaximumSenderUnicastCredits
        {
            get { return defaultMaximumSenderUnicastCredits; }
            set { defaultMaximumSenderUnicastCredits = value; }
        }

        public uint DefaultMaximumSenderMulticastCredits
        {
            get { return defaultMaximumSenderMulticastCredits; }
            set { defaultMaximumSenderMulticastCredits = value; }
        }

        public uint DefaultUnicastSenderConcurrency
        {
            get { return defaultUnicastSenderConcurrency; }
            set { defaultUnicastSenderConcurrency = value; }
        }

        public uint DefaultMulticastSenderConcurrency
        {
            get { return defaultMulticastSenderConcurrency; }
            set { defaultMulticastSenderConcurrency = value; }
        }

        public bool ContinueIOOnTimeWarps
        {
            get { return continueIOOnTimeWarps; }
            set { continueIOOnTimeWarps = value; }
        }

        public uint CoreMaximumConcurrency
        {
            get { return coreMaximumConcurrency; }
            set { coreMaximumConcurrency = value; }
        }

        public uint CoreMinimumTransmitted
        {
            get { return coreMinimumTransmitted; }
            set { coreMinimumTransmitted = value; }
        }

        public uint CoreMaximumTransmitted
        {
            get { return coreMaximumTransmitted; }
            set { coreMaximumTransmitted = value; }
        }

        /// <summary>
        /// The rate at which loss recovery tokens are circulated.
        /// </summary>
        public double TokenRate
        {
            get { return token_rate; }
            set { token_rate = value; }
        }

        /// <summary>
        /// Number of receivers caching each message.
        /// </summary>
        public uint ReplicationCoefficient
        {
            get { return replication_coefficient; }
            set { replication_coefficient = value; }
        }

        public bool DisablePullCaching
        {
            get { return disablePullCaching; }
            set { disablePullCaching = value; }
        }

        public bool DisableNaks
        {
            get { return disableNaks; }
            set { disableNaks = value; }
        }

        public bool DisableForwarding
        {
            get { return disableForwarding; }
            set { disableForwarding = value; }
        }

        public uint NakWindowWidth
        {
            get { return nakWindowWidth; }
            set { nakWindowWidth = value; }
        }

        public uint MaximumNakRangesPerToken
        {
            get { return maximumNakRangesPerToken; }
            set { maximumNakRangesPerToken = value; } 
        }

        public bool DoNotBufferUnrecognized
        {
            get { return do_not_buffer_unrecognized; }
            set { do_not_buffer_unrecognized = value; }
        }

        public uint MaximumUnrecognizedToBuffer
        {
            get { return max_unrecognized_to_buffer; }
            set { max_unrecognized_to_buffer = value; }
        }

        public double UnrecognizedRetryTimeout
        {
            get { return unrecognized_retry_timeout; }
            set { unrecognized_retry_timeout = value; }
        }

        public double MulticastRetransmissionTimeout
        {
            get { return multicastRetransmissionTimeout; }
            set { multicastRetransmissionTimeout = value; }
        }

        public double MulticastSenderDelay
        {
            get { return sender_delay; }
            set { sender_delay = value; }
        }

        public uint MaximumUnacknowledged
        {
            get { return maximumUnacknowledged; }
            set { maximumUnacknowledged = value; }
        }

        public uint MinimumToBufferPerRegionSink
        {
            get { return feed_buffer_min; }
            set { feed_buffer_min = value; }
        }

        public uint MaximumToBufferPerRegionSink
        {
            get { return feed_buffer_max; }
            set { feed_buffer_max = value; }
        }

        public bool EnableHybridMulticast
        {
            get { return enableHybridMulticast; }
            set { enableHybridMulticast = value; }
        }

        /// <summary>
        /// When this is set, log messages are sent to the standard console.
        /// </summary>
        public bool LogToConsole
        {
            get { return logToConsole; }
            set { logToConsole = value; }
        }

        public bool LogToFile
        {
            get { return logToFile; }
            set { logToFile = value; }
        }

        public QS.Fx.Logging.IConsole LogTo
        {
            get { return log_to; }
            set { log_to = value; }
        }

        public bool OpenLogWindow
        {
            get { return openLogWindow; }
            set { openLogWindow = value; }
        }

        /// <summary>
        /// When this is set, various internal log messages are generated.
        /// </summary>
        public bool Verbose
        {
            get { return verbose; }
            set { verbose = value; }
        }

        /// <summary>
        /// Time to wait before processing a membership change request. The larger the value, the more batching and the better performance.
        /// Larger values (in 10s of seconds) are recommended if many clients subscribing to thousands of groups are present.
        /// </summary>
        public double MembershipChangeClientBatchingInterval
        {
            get { return client_membership_batching_interval; }
            set { client_membership_batching_interval = value; }
        }

        /// <summary>
        /// Controls CPU affinity for the local process. If not set (0), affinity is not set. On multiprocessor machines, the recommended
        /// setting is 1 (default), which will restrict the application to run on a single CPU. This is required to prevent inconsistent clock readings.
        /// Allowing QSM to run on multiple CPUs, suhc as by leaving this option unset, is only safe if all the CPUs provide consistent 
        /// values of the RDTSC counter.
        /// </summary>
        public uint ProcessAffinity
        {
            get { return processAffinity; }
            set { processAffinity = value; }
        }

        #endregion
    }
}
