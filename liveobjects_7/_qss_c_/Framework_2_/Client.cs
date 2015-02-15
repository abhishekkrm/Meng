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

#define DEBUG_PermitLogging

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Framework_2_
{
    /// <summary>
    /// Encapsulated a local QSM client.
    /// </summary>
    [QS.Fx.Base.Inspectable]
    public sealed class Client : QS.Fx.Inspection.Inspectable, IClient, QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent>
    {
        #region Constructor

        /// <summary>
        /// Constructs the client.
        /// </summary>
        /// <param name="configuration">Client configuration.</param>
        /// <returns>The constructed client.</returns>
        public static IClient Create(ClientConfiguration configuration)
        {
            return new Client(configuration);
        }

        private Client(ClientConfiguration configuration)
        {
            this.configuration = configuration;

            // for now this is the only choice we have

            if (configuration.ProcessAffinity > 0)
                System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(configuration.ProcessAffinity);

            logger = new QS._core_c_.Base.Logger(QS._core_c_.Core.Clock.SharedClock, true);
            eventLogger = new QS._qss_c_.Logging_1_.EventLogger(QS._core_c_.Core.Clock.SharedClock, true);

            if (configuration.LogToConsole)
                logger.Console = QS._qss_c_.Base1_.Console.StandardConsole;

            if (configuration.LogToFile)
                logger.Console = new QS._qss_d_.Components_.FileLogger("quicksilver_log.txt");

            if (configuration.LogTo != null)
                logger.Console = configuration.LogTo;

            subnet = new QS._qss_c_.Base1_.Subnet((configuration.Subnet != null) ? configuration.Subnet : "x.x.x.x");

            if (configuration.Port == 0)
                throw new Exception("Port number is not set or set to 0.");

            foreach (System.Net.IPAddress ipaddress in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
            {
                if ((ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) && subnet.contains(ipaddress))
                {
                    localAddress = new QS.Fx.Network.NetworkAddress(ipaddress, (int) configuration.Port);
                    break;
                }
            }

            if (localAddress == null)
                throw new Exception("Could not find any interface attached to the requested subnet " + subnet.ToString());

            instanceID = (configuration.Incarnation != 0) ?
                new QS._core_c_.Base3.InstanceID(localAddress, configuration.Incarnation) : new QS._core_c_.Base3.InstanceID(localAddress, DateTime.Now);

#if DEBUG_PermitLogging
            if (configuration.Verbose)
                logger.Log(this, "Complete local instance address is " + instanceID.ToString());
#endif

            if (configuration.GMSAddress != null)
            {
                if (configuration.GMSPort == 0)
                    throw new Exception("GMS port number is not set or set to 0.");
            }
            else
            {
                if (configuration.GMSPort != 0 && configuration.GMSPort != configuration.Port)
                    throw new Exception("GMS address not set, but GMS port number set to a value different than the local port.");

                configuration.GMSAddress = localAddress.HostIPAddress.ToString();
                configuration.GMSPort = configuration.Port; 
            }

            gmsAddress = new QS.Fx.Network.NetworkAddress(
                System.Net.IPAddress.Parse(configuration.GMSAddress), (int) configuration.GMSPort);

#if DEBUG_PermitLogging
            if (configuration.Verbose)
                logger.Log(this, "GMS address is " + gmsAddress.ToString());
            if (configuration.Verbose && gmsAddress.Equals(localAddress))
                logger.Log(this, "This node is hosting the GMS.");
#endif

            string repository_root = (configuration.WorkingPath != null) ? configuration.WorkingPath : "C:\\QuickSilver\\Temp";
            if (!System.IO.Directory.Exists(repository_root))
                System.IO.Directory.CreateDirectory(repository_root);

            QS._qss_c_.Multicasting7.ReliableRegionViewSink.DisableSoftwareLoopback = configuration.DisableSoftwareLoopback;
            QS._core_c_.Core.Sockets.DisableIPMulticastLoopback = configuration.DisableMulticastLoopback;

            logger.Log("DisableSoftwareLoopback: " + (configuration.DisableSoftwareLoopback ? "yes" : "no"));
            logger.Log("DisableMulticastLoopback: " + (configuration.DisableMulticastLoopback ? "yes" : "no"));

            string rootpath = repository_root;
            core = new QS._core_c_.Core.Core(rootpath + "\\resultsdir");
            string fsroot = rootpath + "\\filesystem";

            framework = new QS._qss_c_.Framework_1_.FrameworkOnCore(
                instanceID, gmsAddress, logger, eventLogger, core,  fsroot, !configuration.DisableFailureDetection,
                (int)configuration.MTU, configuration.EnableHybridMulticast);

#pragma warning disable 0162
            if (QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.Agent.ProcessingCrashes
                || QS._qss_c_.Rings6.Receiver.ProcessingCrashes != QS._qss_c_.Rings6.AgentCore.ProcessingCrashes)
                throw new Exception("Bad build, fix ifdefs for crash processing.");
#pragma warning restore 0162

            if (configuration.GMSBatchingTime != 0 && framework.MembershipServer != null)
                framework.MembershipServer.RequestAggregationInterval = (double) configuration.GMSBatchingTime;

            if (configuration.GMSMaxHeartbeatsMissed != 0 && framework.FailureDetectionServer != null)
                framework.FailureDetectionServer.MaximumMissed = (int) configuration.GMSMaxHeartbeatsMissed;

            if (configuration.GMSHeartbeatTimeout != 0 && framework.FailureDetectionServer != null)
                framework.FailureDetectionServer.HeartbeatTimeout = TimeSpan.FromSeconds(configuration.GMSHeartbeatTimeout);

            if (configuration.HeartbeatInterval != 0 && framework.FailureDetectionAgent != null)
                framework.FailureDetectionAgent.HeartbeatInterval = TimeSpan.FromSeconds(configuration.HeartbeatInterval);

            framework.Root.DefaultDrainSynchronouslyForUnicastAddresses = configuration.SynchronouslyDrainUnicast;
            framework.Root.DefaultDrainSynchronouslyForMulticastAddresses = configuration.SynchronouslyDrainMulticast;
            framework.Root.DefaultNumberOfReceiveBuffersForUnicastAddresses =
                (configuration.NumberOfReceiveBuffersForUnicast > 0) ? ((int)configuration.NumberOfReceiveBuffersForUnicast) : 100;
            framework.Root.DefaultNumberOfReceiveBuffersForMulticastAddresses =
                (configuration.NumberOfReceiveBuffersForMulticast > 0) ? ((int)configuration.NumberOfReceiveBuffersForMulticast) : 100;

            framework.Root.DefaultAfdReceiveBufferSizeForUnicastAddresses = (int) configuration.SizeOfAdfReceiveBuffersForUnicast;
            framework.Root.DefaultAfdReceiveBufferSizeForMulticastAddresses = (int) configuration.SizeOfAdfReceiveBuffersForMulticast;
            framework.Root.DefaultAfdSendBufferSizeForUnicastAddresses = (int) configuration.SizeOfAdfSendBuffersForUnicast;
            framework.Root.DefaultAfdSendBufferSizeForMulticastAddresses = (int) configuration.SizeOfAdfSendBuffersForMulticast;

            core.MaximumQuantumForAlarms = configuration.AlarmQuantum;
            core.MaximumQuantumForCompletionPorts = configuration.IOQuantum;

            if (configuration.DefaultMaximumRateForUnicast > 0)
                core.DefaultMaximumSenderUnicastRate = configuration.DefaultMaximumRateForUnicast;

            if (configuration.DefaultMaximumRateForMulticast > 0)
                core.DefaultMaximumSenderMulticastRate = configuration.DefaultMaximumRateForMulticast;

            if (configuration.DefaultMaximumSenderUnicastCredits > 0)
                core.DefaultMaximumSenderUnicastCredits = configuration.DefaultMaximumSenderUnicastCredits;

            if (configuration.DefaultMaximumSenderMulticastCredits > 0)
                core.DefaultMaximumSenderMulticastCredits = configuration.DefaultMaximumSenderMulticastCredits;

            if (configuration.DefaultUnicastSenderConcurrency > 0)
                framework.Root.DefaultUnicastSenderConcurrency = (int) configuration.DefaultUnicastSenderConcurrency;

            if (configuration.DefaultMulticastSenderConcurrency > 0)
                framework.Root.DefaultMulticastSenderConcurrency = (int) configuration.DefaultMulticastSenderConcurrency;

            core.ContinueIOOnTimeWarps = configuration.ContinueIOOnTimeWarps;

            if (configuration.CoreMaximumConcurrency > 0)
                core.MaximumConcurrency = (int) configuration.CoreMaximumConcurrency;

            if (configuration.CoreMinimumTransmitted > 0)
                core.MinimumTransmitted = (int) configuration.CoreMinimumTransmitted;

            if (configuration.CoreMaximumTransmitted > 0)
                core.MaximumTransmitted = (int) configuration.CoreMaximumTransmitted;

//            batching = QS.TMS.Experiment.Helpers.Args.BoolOf(args, "batching");
//            if (batching)
//                batching_buffersize = Convert.ToInt32((string)args["batching_buffersize"]);

            QS._qss_c_.Rings6.RateSharingAlgorithmClass rateSharingAlgorithm = QS._qss_c_.Rings6.RateSharingAlgorithmClass.Compete;

            receivingAgentClass = new QS._qss_c_.Rings6.ReceivingAgent(
                framework.EventLogger, framework.Logger, framework.LocalAddress, framework.AlarmClock, framework.Clock,
                framework.Demultiplexer, framework.BufferedReliableInstanceSinks,  
                5, 1, 0.1, 10, 5000, rateSharingAlgorithm, framework.ReliableInstanceSinks, framework.StatisticsController);

            if (configuration.TokenRate > 0)
                receivingAgentClass.TokenRate = configuration.TokenRate;

            if (configuration.ReplicationCoefficient != 0)
                receivingAgentClass.ReplicationCoefficient = configuration.ReplicationCoefficient;

            receivingAgentClass.PullCaching = !configuration.DisablePullCaching;
            receivingAgentClass.NaksAllowed = !configuration.DisableNaks;
            receivingAgentClass.ForwardingAllowed = !configuration.DisableForwarding;

            if (configuration.NakWindowWidth > 0)
                receivingAgentClass.MaximumWindowWidth = configuration.NakWindowWidth;

            if (configuration.MaximumNakRangesPerToken > 0)
                receivingAgentClass.MaximumNakRangesPerToken = configuration.MaximumNakRangesPerToken;

            regionalController = new QS._qss_c_.Receivers4.RegionalController(
                framework.LocalAddress, framework.Logger, framework.Demultiplexer, framework.AlarmClock, framework.Clock,
                framework.MembershipController, receivingAgentClass, receivingAgentClass);
            regionalController.IsDisabled = false;

            bool buffering_unrecognized = !configuration.DoNotBufferUnrecognized;
            int maximum_unrecognized = (configuration.MaximumUnrecognizedToBuffer > 0) ? (int) configuration.MaximumUnrecognizedToBuffer : 1000;
            double unrecognized_timeout = (configuration.UnrecognizedRetryTimeout > 0) ? configuration.UnrecognizedRetryTimeout : 5;

            regionalSenders = new QS._qss_c_.Senders10.RegionalSenders(
                framework.EventLogger, framework.LocalAddress, framework.Logger, framework.AlarmClock, framework.Clock,
                framework.Demultiplexer, null, // this null argument needs to be fixed ,
                regionalController, regionalController, configuration.MulticastRetransmissionTimeout, regionalController,                
                buffering_unrecognized, maximum_unrecognized, unrecognized_timeout); 

            dispatcherRV2 = new QS._qss_c_.Multicasting7.DispatcherRV2(logger, framework.AlarmClock, framework.Clock,
                framework.Demultiplexer, (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, regionalController, framework.MembershipController);

            QS._qss_c_.Base1_.IFactory<QS._qss_c_.FlowControl7.IRateController> rateControllers =
                new QS._qss_c_.FlowControl7.DummyController2(framework.Clock, 
                    (configuration.DefaultMaximumRateForMulticast > 0) ? configuration.DefaultMaximumRateForMulticast : double.PositiveInfinity);

            nodeSinkCollection = framework.Root;
            regionSinkCollection = new QS._qss_c_.Multicasting7.RegionSinks(framework.MembershipController, nodeSinkCollection);

            regionViewSinkCollection = new QS._qss_c_.Multicasting7.ReliableRegionViewSinks(framework.StatisticsController,
                framework.Logger, framework.EventLogger, framework.LocalAddress,
                framework.AlarmClock, framework.Clock, (uint)QS.ReservedObjectID.Rings6_SenderController1_DataChannel,
                (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel,
                regionSinkCollection, regionalController, regionalController, framework.MembershipController, framework.Root,
                (configuration.MulticastRetransmissionTimeout > 0) ? configuration.MulticastRetransmissionTimeout : 60, rateControllers,
                (configuration.MulticastSenderDelay > 0) ? configuration.MulticastSenderDelay : 3);

            int maximumPendingCompletion = (configuration.MaximumUnacknowledged > 0) ? (int) configuration.MaximumUnacknowledged : 100000;
            int feed_buffer_min = (configuration.MinimumToBufferPerRegionSink > 0) ? (int) configuration.MinimumToBufferPerRegionSink : 50;
            int feed_buffer_max = (configuration.MaximumToBufferPerRegionSink > 0) ? (int) configuration.MaximumToBufferPerRegionSink : 150;

            groupSinkCollection =
                new QS._qss_c_.Multicasting7.ReliableGroupSinks(
                    framework.StatisticsController, framework.Clock, framework.MembershipController, regionViewSinkCollection, logger,
                    maximumPendingCompletion, feed_buffer_min, feed_buffer_max);

            if (configuration.EnableHybridMulticast)
            {
                hybridGroupSinkCollection =
                    new QS._qss_c_.Multicasting7.AlternativeReliableGroupSinks(
                        framework.StatisticsController, framework.Clock, framework.MembershipController, logger,
                        nodeSinkCollection, ((QS._qss_c_.Multicasting7.ReliableRegionViewSinks)regionViewSinkCollection),
                        (uint)QS.ReservedObjectID.Multicasting7_DispatcherRV2, framework.Root, null,
                        (configuration.DefaultMaximumRateForMulticast > 0) ? configuration.DefaultMaximumRateForMulticast : 5000);
            }

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

            clock = QS._core_c_.Core.Clock.SharedClock;

            ((QS._qss_c_.Membership2.Consumers.IGroupCreationAndRemovalProvider)framework.MembershipController).OnChange +=
                new QS._qss_c_.Membership2.Consumers.GroupCreationOrRemovalCallback(this.GroupCreationOrRemovalCallback);

#if DEBUG_PermitLogging
            if (configuration.Verbose)
                logger.Log(this, "All initialization completed, starting the core thread.");
#endif

            framework.Demultiplexer.register((uint) ReservedObjectID.Framework2_Group, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));

            core.Start();

            completionWorker = new QS._qss_c_.Synchronization_1_.NonblockingWorker<QS.Fx.Base.IEvent>(
                new QS.Fx.Base.ContextCallback<QS.Fx.Base.IEvent>(this.CompletionCallback));
        }

        #endregion

        #region Fields

        private ClientConfiguration configuration;
        private QS._qss_c_.Base1_.Subnet subnet;
        private QS.Fx.Network.NetworkAddress localAddress, gmsAddress;
        private QS._core_c_.Base3.InstanceID instanceID;
        private QS._core_c_.Base.Logger logger;
        private QS._qss_c_.Logging_1_.EventLogger eventLogger;
        private QS.Fx.Clock.IClock clock;
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._qss_c_.Rings6.ReceivingAgent receivingAgentClass;
        private QS._qss_c_.Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> nodeSinkCollection;
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RegionID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionSinkCollection;
        private QS._qss_c_.Multicasting7.DispatcherRV2 dispatcherRV2;

        [QS._core_c_.Diagnostics.Component("Core")]
        [QS._core_c_.Diagnostics2.Module("Core")]
        private QS._core_c_.Core.Core core;

        [QS._core_c_.Diagnostics2.Module("Framework")]
        private QS._qss_c_.Framework_1_.FrameworkOnCore framework;

        [QS._core_c_.Diagnostics2.Module("RegionalController")]
        private QS._qss_c_.Receivers4.RegionalController regionalController;

        [QS._core_c_.Diagnostics2.Module("ReliableRegionViewSinks")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.RVID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> regionViewSinkCollection;

        [QS._core_c_.Diagnostics2.Module("ReliableGroupSinks_Regular")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> groupSinkCollection;

        // [QS.CMS.QS._core_c_.Diagnostics2.Module("ReliableGroupSinks_Hybrid")]
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_c_.Base3_.GroupID,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> hybridGroupSinkCollection;

        [QS._core_c_.Diagnostics2.Module("RegionalSenders")]
        private QS._qss_c_.Senders10.RegionalSenders regionalSenders;

        private IDictionary<Base3_.GroupID, Group> groups = new Dictionary<Base3_.GroupID, Group>();
        private ICollection<Group> groupsToSubscribe = new System.Collections.ObjectModel.Collection<Group>();
        private ICollection<Group> groupsToUnsubscribe = new System.Collections.ObjectModel.Collection<Group>();
        private bool waitingToIssueMembershipChangeRequest;
        private QS.Fx.Clock.IAlarm deferredMembershipChangeRequestAlarm;
        private QS._qss_c_.Synchronization_1_.NonblockingWorker<QS.Fx.Base.IEvent> completionWorker;

        #endregion

        #region IClient Members

        public IAsyncResult BeginOpen(QS._qss_c_.Base3_.GroupID groupID, AsyncCallback asyncCallback, object asyncState)
        {
            return BeginOpen(groupID, GroupOptions.Defaults, asyncCallback, asyncState);
        }

        public IAsyncResult BeginOpen(
            QS._qss_c_.Base3_.GroupID groupID, GroupOptions options, AsyncCallback asyncCallback, object asyncState)
        {
            if ((options & GroupOptions.FastSendCallback) != GroupOptions.FastSendCallback)
                throw new Exception("In this release the FastSendCallback option is mandatory.");

            if (((options & GroupOptions.Hybrid) == GroupOptions.Hybrid) && !configuration.EnableHybridMulticast)
                throw new Exception("Hybrid multicasting disabled, set the EnableHybridMulticast option to enable it.");

            OpenRequest openreq = new OpenRequest(
                new QS.Fx.Base.ContextCallback<OpenRequest>(this.OpenCallback), groupID, options, asyncCallback, asyncState);
            core.Schedule(openreq);
            return openreq;           
        }

        public QS._qss_c_.Framework_2_.IGroup EndOpen(IAsyncResult asyncResult)
        {
            return ((OpenRequest) asyncResult).GroupRef;
        }

        public QS._qss_c_.Framework_2_.IGroup Open(QS._qss_c_.Base3_.GroupID groupID)
        {
            return Open(groupID, GroupOptions.Defaults);
        }

        public QS._qss_c_.Framework_2_.IGroup Open(QS._qss_c_.Base3_.GroupID groupID, GroupOptions options) 
        {
            IAsyncResult result = BeginOpen(groupID, options, null, null);

            if (!result.CompletedSynchronously)
                result.AsyncWaitHandle.WaitOne();

            return EndOpen(result);
        }

        public QS.Fx.Clock.IClock Clock
        {
            get { return framework.Clock; }
        }

        #endregion

        #region OpenCallback

        private void OpenCallback(OpenRequest openreq)
        {
            Group group;

            if (!groups.TryGetValue(openreq.GroupID, out group))
            {
                group = new Group(
                    openreq.GroupID, (configuration.Verbose ? logger : null), 
                    framework.Demultiplexer, groupSinkCollection, hybridGroupSinkCollection, this, completionWorker, 
                    new QS.Fx.Base.ContextCallback<Group>(this.SubscribeCallback),
                    new QS.Fx.Base.ContextCallback<Group>(this.CancelSubscribeCallback),
                    new QS.Fx.Base.ContextCallback<Group>(this.UnsubscribeCallback),
                    new QS.Fx.Base.ContextCallback<Group>(this.CancelUnsubscribeCallback),
                    new QS.Fx.Base.ContextCallback<Group>(this.RemoveCallback));

                groups.Add(openreq.GroupID, group);
            }

            group.ProcessOpenRequest(openreq);
        }

        #endregion

        #region Subscribe and Unsubscribe Callbacks

        private void SubscribeCallback(Group group)
        {
            groupsToSubscribe.Add(group);
            UpdateMembershipChangeAlarm();
        }

        private void CancelSubscribeCallback(Group group)
        {
            groupsToSubscribe.Remove(group);
            UpdateMembershipChangeAlarm();
        }

        private void UnsubscribeCallback(Group group)
        {
            groupsToUnsubscribe.Add(group);
            UpdateMembershipChangeAlarm();
        }

        private void CancelUnsubscribeCallback(Group group)
        {
            groupsToUnsubscribe.Remove(group);
            UpdateMembershipChangeAlarm();
        }

        private void UpdateMembershipChangeAlarm()
        {
            if (groupsToSubscribe.Count + groupsToUnsubscribe.Count > 0)
            {
                if (!waitingToIssueMembershipChangeRequest)
                {
                    waitingToIssueMembershipChangeRequest = true;
                    if (deferredMembershipChangeRequestAlarm == null)
                        deferredMembershipChangeRequestAlarm =
                            core.Schedule(configuration.MembershipChangeClientBatchingInterval,
                            new QS.Fx.Clock.AlarmCallback(this.MembershipChangeIssueRequestsCallback), null);
                    else
                    {
                        // should not really happen anyway
                        deferredMembershipChangeRequestAlarm.Reschedule(configuration.MembershipChangeClientBatchingInterval);
                    }
                }
            }
            else
            {
                if (waitingToIssueMembershipChangeRequest)
                {
                    waitingToIssueMembershipChangeRequest = false;
                    try
                    {
                        deferredMembershipChangeRequestAlarm.Cancel();
                    }
                    catch (Exception)
                    {
                    }
                    deferredMembershipChangeRequestAlarm = null;
                }
            }
        }

        #endregion

        #region MembershipChangeIssueRequestsCallback

        private void MembershipChangeIssueRequestsCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            waitingToIssueMembershipChangeRequest = false;
            deferredMembershipChangeRequestAlarm = null;

            List<Base3_.GroupID> toJoin = new List<QS._qss_c_.Base3_.GroupID>();
            List<Base3_.GroupID> toLeave = new List<QS._qss_c_.Base3_.GroupID>();

            foreach (Group group in groupsToSubscribe)
            {
                if (group.ReallyWantsToSubscribe())
                    toJoin.Add(group.ID);
            }

            groupsToSubscribe.Clear();

            foreach (Group group in groupsToUnsubscribe)
            {
                if (group.ReallyWantsToUnsubscribe())
                    toLeave.Add(group.ID);
            }

            groupsToUnsubscribe.Clear();

            if (toJoin.Count + toLeave.Count > 0)
            {
#if DEBUG_PermitLogging
                if (configuration.Verbose)
                    logger.Log(this, "Issuing membership change request for { add : " +
                        QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.GroupID>(toJoin, ", ") + "; remove: " +
                        QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.GroupID>(toLeave, ", ") + " }.");
#endif

                framework.MembershipAgent.ChangeMembership(toJoin, toLeave);
            }
        }

        #endregion

        #region GroupCreationOrRemovalCallback

        private void GroupCreationOrRemovalCallback(
            IEnumerable<QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval> notifications)
        {
            foreach (Membership2.Consumers.GroupCreationOrRemoval groupCreationOrRemoval in notifications)
            {
                Group group;
                if (groups.TryGetValue(groupCreationOrRemoval.ID, out group))
                    group.MembershipChanged(groupCreationOrRemoval.Creation);
            }
        }

        #endregion

        #region RemoveCallback

        private void RemoveCallback(Group group)
        {
#if DEBUG_PermitLogging
            if (configuration.Verbose)
                logger.Log(this, "Removing group " + group.ID.ToString());
#endif

            groups.Remove(group.ID);
        }

        #endregion

        #region Callback

        private void CompletionCallback(QS.Fx.Base.IEvent request)
        {
#if DEBUG_PermitLogging
            if (configuration.Verbose)
                logger.Log(this, "Processing completion of " + request.GetType().ToString() + ".");
#endif

            request.Handle();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
#if DEBUG_PermitLogging
            if (configuration.Verbose)
                logger.Log(this, "Disposing the client");
#endif

            ((IDisposable)completionWorker).Dispose();
            core.Stop();
            core.Dispose();
        }

        #endregion

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sender, QS.Fx.Serialization.ISerializable __message)
        {
            Message _message = __message as Message;
            if (_message != null)
            {
                Group group;
                if (groups.TryGetValue(_message.GroupID, out group))
                    return group.ReceiveCallback(sender, _message.Object);
                else
                    return null;
            }
            else
            {
                SendRequest _request = __message as SendRequest;
                if (_request != null)
                    return _request.GroupRef.Group.ReceiveCallback(sender, _request.Message);
                else
                {
                    string _s = "Received message of an unknown type; possibly using two incompatible versions of QSM.";
                    System.Diagnostics.Debug.Assert(false, _s);
                    throw new Exception(_s);
                }
            }
        }

        #endregion

        #region INonblockingWorker<QS.Fx.Base.Event> Members

        void QS._qss_c_.Synchronization_1_.INonblockingWorker<QS.Fx.Base.IEvent>.Process(QS.Fx.Base.IEvent request)
        {
            this.core.Schedule(request);
        }

        #endregion
    }
}
