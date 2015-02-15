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

namespace QS._qss_c_.Framework_1_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Framework: QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
    {
        #region Constructor

        public Framework(QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Network.NetworkAddress coordinatorAddress, QS.Fx.Platform.IPlatform platform, 
            QS._core_c_.Statistics.IStatisticsController statisticsController, bool activate_fd, int mtu, bool allocateMulticastAddressPerGroup) 
        {
            this.logger = platform.Logger;
            this.eventLogger = platform.EventLogger;

            this.platform = platform;
            this.statisticsController = statisticsController;

            // this.diagnosticsComponent = new QS.CMS.QS._core_c_.Diagnostics2.Component(diagnosticsContainer, "Framework");

            this.localAddress = localAddress;
            if (coordinatorAddress == null)
                coordinatorAddress = localAddress.Address; 
            this.coordinatorAddress = coordinatorAddress;

            this.alarmClock = platform.AlarmClock;
            this.clock = platform.Clock;

            logger.Log(this, "Starting the framework.");

            demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(logger, eventLogger);
            root = new QS._qss_c_.Base8_.Root(statisticsController, logger, eventLogger, platform.Clock,
                platform.Network, localAddress, demultiplexer);

            unpacker = new QS._qss_c_.Batching_.Unpacker(demultiplexer);

            reliableInstanceSinks = new QS._qss_c_.Senders11.ReliableInstanceSinks(logger, eventLogger,
                (uint)ReservedObjectID.Receivers5_InstanceReceiver_MessageChannel, localAddress, alarmClock, clock, root);
            instanceAckCollector = new QS._qss_c_.Receivers5.InstanceAckCollector(localAddress, demultiplexer,
                (uint)ReservedObjectID.Receivers5_InstanceReceiver_AcknowledgementChannel,
                new QS._qss_c_.Receivers5.InstanceAckCollector.AcknowledgementCallback(reliableInstanceSinks.Acknowledge));
            simpleInstanceReceiver = new QS._qss_c_.Receivers5.SimpleInstanceReceiver(
                (uint)ReservedObjectID.Receivers5_InstanceReceiver_AcknowledgementChannel, root);
            instanceReceiverController = new QS._qss_c_.Receivers5.InstanceReceiverController(localAddress, logger, eventLogger,
                demultiplexer, alarmClock, clock, (uint)ReservedObjectID.Receivers5_InstanceReceiver_MessageChannel, simpleInstanceReceiver);
            bufferedReliableInstanceSinks = new QS._qss_c_.Base6_.BufferCollection<QS._core_c_.Base3.InstanceID>(reliableInstanceSinks, clock);

            reliableSender = new Senders3.ReliableSender1(logger, demultiplexer, root, alarmClock, 1.0);
            instanceReceiver = new QS._qss_c_.Senders6.InstanceReceiver(logger, localAddress, demultiplexer);
            fdSubscriber = new QS._qss_c_.FailureDetection_.Centralized.Subscriber(logger, demultiplexer);
            if (coordinatorAddress.Equals(localAddress.Address))
            {
                failureDetectionServer = new FailureDetection_.Centralized.Server(
                    logger, demultiplexer, alarmClock, TimeSpan.FromSeconds(1), 5, activate_fd);                
                instanceSender = new Senders6.InstanceSender(logger, failureDetectionServer, root, reliableSender.SenderCollection);
                localFD = failureDetectionServer;
            }
            else
            {
                failureDetectionServer = null;
                instanceSender = new QS._qss_c_.Senders6.InstanceSender(logger, null, root, null);
                localFD = fdSubscriber;
            }
            reconnectingReliableSender =
                new QS._qss_c_.Senders6.ReliableSender(localAddress, logger, demultiplexer, alarmClock, clock, root, localFD);
            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(
                "ReconnectingReliableSender", ((QS._core_c_.Diagnostics2.IModule)reconnectingReliableSender).Component);
            
            choppingSender = new QS._qss_c_.Senders3.ChoppingSender(
                reliableSender.SenderCollection, instanceSender, logger, demultiplexer);

            simpleChoppingSender = 
                new Senders3.SimpleChoppingSender(logger, demultiplexer, reconnectingReliableSender, reconnectingReliableSender);
            
            simpleCaller = new QS._qss_c_.RPC3.SimpleCaller(logger, 
                simpleChoppingSender, //reconnectingReliableSender, 
                demultiplexer, alarmClock, 60);

            if (failureDetectionServer != null)
                fdPublisher = new QS._qss_c_.FailureDetection_.Centralized.Publisher(logger, failureDetectionServer, choppingSender);

            if (coordinatorAddress.Equals(localAddress.Address))
            {
                logger.Log(this, "GMS");
                centralizedServer = new QS._qss_c_.Membership2.Servers.OverlappingServer(logger,
                    demultiplexer, alarmClock, choppingSender.SenderCollection, failureDetectionServer, simpleChoppingSender,
                    allocateMulticastAddressPerGroup);
            }

            membershipController = new QS._qss_c_.Membership2.Controllers.SimpleController(localAddress, root, logger);
            
            clientAgent = new QS._qss_c_.Membership2.ClientAgent.ClientAgent(logger, demultiplexer,
                simpleCaller.SenderCollection[coordinatorAddress], localAddress.Address, membershipController, localAddress);
            
            failureDetectionClient = new QS._qss_c_.FailureDetection_.Centralized.Agent(logger,
                localAddress, ((Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>) root)[coordinatorAddress], 
                alarmClock, TimeSpan.FromSeconds(1), activate_fd);

#region Copied over from SimpleFramework
/*

            rootSender = new QS.CMS.Base3.RootSender(platform.EventLogger,
                incarnation, platform.Logger, device, localAddress.PortNumber, demultiplexer, platform.Clock,
                receiverType != ReceiverType.Base3);

            multicastResponder = new QS.CMS.Connections.MulticastResponder(platform, localAddress);

            platform.Logger.Log(this, "Local_InstanceID : " + rootSender.InstanceID.ToString());

            this.reliableSender = new Senders3.ReliableSender1(
				platform.Logger, demultiplexer, rootSender.SenderCollection, platform.AlarmClock, 1.0);

			instanceID = rootSender.InstanceID; // new QS.CMS.QS._core_c_.Base3.InstanceID(rootSender.Address, 1);

            if (receiverType == ReceiverType.Base7)
            {
                root7 = new QS.CMS.Base7.Root(platform.EventLogger, instanceID, platform.Clock,
                    platform.Connections7, scheduler, demultiplexer);
                dev3MC = root7;
            }
            else
            {
                dev3MC = rootSender;
            }

            unwrapper = new QS.CMS.Buffering3.Unwrapper((uint)ReservedObjectID.Unwrapper, demultiplexer,
                QS.CMS.Buffering3.AccumulatingController.ControllerClass, platform.Logger);

            lazySender = new QS.CMS.Buffering3.LazySender((uint)ReservedObjectID.Unwrapper,
                rootSender.SenderCollection, QS.CMS.Buffering3.AccumulatingController.ControllerClass,
                platform.AlarmClock, TimeSpan.FromSeconds(0.1), platform.Logger);
            thresholdSender = new QS.CMS.Buffering3.ThresholdSender1((uint)ReservedObjectID.Unwrapper,
                rootSender.SenderCollection, QS.CMS.Buffering3.AccumulatingController.ControllerClass,
                platform.AlarmClock, platform.Clock, 0.1, 100, platform.Logger);

            instanceReceiver = new QS.CMS.Senders6.InstanceReceiver(platform.Logger, instanceID, demultiplexer);

            fdSubscriber = new QS.CMS.FailureDetection.Centralized.Subscriber(platform.Logger, demultiplexer);

            if (membershipServerAddress != null && membershipServerAddress.Equals(localAddress))
            {
                failureDetectionServer = new QS.CMS.FailureDetection.Centralized.Server(platform.Logger, demultiplexer, platform.AlarmClock,
                    defaultIntervalBetweenHeartbeats, 10);
                
                instanceSender = new QS.CMS.Senders6.InstanceSender(
                    platform.Logger, failureDetectionServer, rootSender.SenderCollection, reliableSender.SenderCollection);

                localFD = failureDetectionServer;
            }
            else
            {
                failureDetectionServer = null;

                instanceSender = new QS.CMS.Senders6.InstanceSender(platform.Logger, null, rootSender.SenderCollection, null);

                localFD = fdSubscriber;
            }

            choppingSender = new QS.CMS.Senders3.ChoppingSender(
                reliableSender.SenderCollection, instanceSender, platform.Logger, demultiplexer);

            if (failureDetectionServer != null)
            {
                fdPublisher = new QS.CMS.FailureDetection.Centralized.Publisher(platform.Logger, failureDetectionServer, choppingSender);
            }

            reconnectingReliableSender = new QS.CMS.Senders6.ReliableSender(
                instanceID, platform.Logger, demultiplexer, platform.AlarmClock, platform.Clock, rootSender.SenderCollection, localFD);

            aggregationAgent = new QS.CMS.Aggregation.AggregationAgent(platform.Logger, instanceID,
                new Senders6.InstanceSender(platform.Logger, localFD, rootSender.SenderCollection, null), demultiplexer);

            // we switch it onto this sedner to enforce ordering
            simpleCaller = new QS.CMS.RPC3.SimpleCaller(platform.Logger, reconnectingReliableSender, demultiplexer, platform.AlarmClock, 60);

            remotingChannel = new QS.CMS.Interoperability.Remoting.Channel(
                "quicksilver", platform.Logger, simpleCaller.SenderCollection, rootSender.Address, demultiplexer);

            // for now, we don't register the channel
            // ChannelServices.RegisterChannel(remotingChannel);

			if (membershipServerAddress != null)
            {
                if (membershipServerAddress.Equals(localAddress))
                {
                    platform.Logger.Log(this, "GMS: " + membershipServerType.ToString());

                    switch (membershipServerType)
                    {
                        case Membership2.Servers.Type.REGULAR:
                        {
                            centralizedServer = new QS.CMS.Membership2.Servers.RegularServer(platform.Logger,
                                demultiplexer, platform.AlarmClock, choppingSender.SenderCollection, failureDetectionServer, choppingSender);
                        }
                        break;

                        case Membership2.Servers.Type.OVERLAPPING:
                        {
                            centralizedServer = new QS.CMS.Membership2.Servers.OverlappingServer(platform.Logger,
                                demultiplexer, platform.AlarmClock, choppingSender.SenderCollection, failureDetectionServer, choppingSender);
                        }
                        break;

                        default:
                            break;
                    }
                }

                membershipController = new QS.CMS.Membership2.Controllers.SimpleController(instanceID, dev3MC, platform.Logger);
                clientAgent = new QS.CMS.Membership2.ClientAgent.ClientAgent(platform.Logger, demultiplexer,
                    simpleCaller.SenderCollection[membershipServerAddress], rootSender.Address, membershipController, 
					instanceID);

                failureDetectionClient = new QS.CMS.FailureDetection.Centralized.Agent(instanceID,
                    rootSender.SenderCollection[membershipServerAddress], platform.AlarmClock, defaultIntervalBetweenHeartbeats);
            }

			regionSender = new QS.CMS.Multicasting3.RegionSender1(
				platform.Logger, platform.Clock, membershipController, rootSender.SenderCollection, 10);

			aggregationRouter = new Aggregation3.Router1(membershipController, Routing.NoRouting.Algorithm);
			aggregation3Agent = new QS.CMS.Aggregation3.Agent(
				platform.Logger, instanceID, typeof(Aggregation3.Controller1), aggregationRouter,
				((Base3.ISenderClass<QS.CMS.Base3.ISerializableSender>) reliableSender).SenderCollection, // rootSender.SenderCollection, 
				demultiplexer, platform.AlarmClock, Components.SeqCollection<Aggregation3.Controller>.CollectionConstructor);

			// Base3.Serializer.registerClass(ClassID.Base3_GroupID, typeof(Base3.GroupID));
			// Base3.Serializer.registerClass(ClassID.Base3_RegionID, typeof(Base3.RegionID));

			reliableSender2 = new Senders3.ReliableSender1(
				platform.Logger, demultiplexer, rootSender.SenderCollection, platform.AlarmClock, 1.0,
                (uint) ReservedObjectID.Senders3_ReliableSender_MessageChannel_2,
                (uint) ReservedObjectID.Senders3_ReliableSender_AcknowledgementChannel_2);

			aggregation4Controller1 = new Aggregation4.AggregationController1(platform.AlarmClock, platform.Clock,
				0.2, ((Base3.ISenderClass<QS.CMS.Base3.ISerializableSender>) reliableSender2).SenderCollection,
				((Base3.ISenderClass<QS.CMS.Base3.ISerializableSender>) reliableSender).SenderCollection);

			aggregation4Agent = new QS.CMS.Aggregation4.Agent(platform.Logger, instanceID.Address,
				aggregation4Controller1, demultiplexer, Routing.NoRouting.Algorithm, membershipController);

//			if (platform is QS.CMS.Simulations2.SimulatedPlatform)
//			{
//				platform.Logger.Log(this, "Simulated platform detected, adjusting timeouts.");
//				if (centralizedServer != null)
//					centralizedServer.RequestAggregationInterval *= 10;
//			}

            regionalAgent = new QS.CMS.Gossiping2.RegionalAgent(platform.Logger, instanceID, 
                membershipController, platform.AlarmClock,reconnectingReliableSender, demultiplexer, platform.Clock);
            gossipAdapter = new QS.CMS.Gossiping2.GossipAdapter(platform.Logger, regionalAgent);
            receiveRateGCC = new QS.CMS.Gossiping2.ReceiveRateGCC1(platform.Logger, rootSender);
            ((QS.CMS.Gossiping2.IGossipAdapter)gossipAdapter).Register(receiveRateGCC);

            bufferingUNS = new QS.CMS.Senders9.BufferingUNS(platform.Logger, rootSender.SenderCollection,
                QS.CMS.Buffering3.AccumulatingController.ControllerClass, platform.AlarmClock, platform.Clock);
            multicastingURS = new QS.CMS.Multicasting5.MulticastingURS(platform.Logger, membershipController, bufferingUNS);
            simpleURVS = new QS.CMS.Multicasting5.SimpleURVS(platform.Logger, multicastingURS);

            gossipingRRVS = new QS.CMS.Multicasting5.GossipingRRVS(platform.Logger, instanceID, 
                multicastingURS, demultiplexer, platform.AlarmClock, regionalAgent, membershipController, platform.Clock);

            newRoot = new QS.CMS.Base4.Root(platform.Logger, platform.NetworkConnections, incarnation);
            mainConnection = ((Base4.IRoot) newRoot).Connect(localAddress);
            newMulticastingURS = new QS.CMS.Multicasting6.MulticastingURS(platform.Logger, membershipController, mainConnection);
            newSimpleURVS = new QS.CMS.Multicasting6.SimpleURVS(platform.Logger, newMulticastingURS);
            newDelegatingGS = new QS.CMS.Multicasting6.DelegatingGS(platform.Logger, membershipController, newSimpleURVS);

            ringRRVS = new QS.CMS.Rings4.RingRRVS(platform.Logger, instanceID, demultiplexer, platform.AlarmClock, 
                platform.Clock, membershipController, multicastingURS, reconnectingReliableSender, 0.01, 100);

            rings6_receivingAgent1 = new QS.CMS.Rings6.ReceivingAgent(platform.EventLogger, platform.Logger, 
                instanceID, platform.AlarmClock, platform.Clock, demultiplexer, reconnectingReliableSender, 5, 1, 0.1, 10, 5000);
            receivers4_regionalController = new QS.CMS.Receivers4.RegionalController(instanceID, platform.Logger, demultiplexer,
                platform.AlarmClock, platform.Clock, membershipController, rings6_receivingAgent1, rings6_receivingAgent1);
            senders10_regionalSenders = new QS.CMS.Senders10.RegionalSenders(platform.EventLogger, instanceID, 
                platform.Logger, platform.AlarmClock, platform.Clock, demultiplexer, multicastingURS, receivers4_regionalController, 
                receivers4_regionalController, 60, receivers4_regionalController);
            senders10_rrvsSenders = 
                new QS.CMS.Senders10.SenderCollection<QS.CMS.Base3.RVID>(platform.Logger, senders10_regionalSenders);

            if (ReplaceDevices6SinkWithDevices4AsynchronousSender)
                base6root = new QS.CMS.Base6.Root(instanceID, platform.Clock, ((CMS.Devices4.INetwork)platform.NetworkConnections));
            else
                base6root = new QS.CMS.Base6.Root(instanceID, platform.Clock, ((CMS.Devices6.INetwork)platform));

            regionSinks7 = new QS.CMS.Multicasting7.RegionSinks(membershipController, base6root);
            regionViewSinks7 = new QS.CMS.Multicasting7.ReliableRegionViewSinks(platform.Logger, platform.EventLogger, instanceID,
                platform.AlarmClock, platform.Clock, (uint) QS.ReservedObjectID.Rings6_SenderController1_DataChannel, regionSinks7,
                receivers4_regionalController, receivers4_regionalController);
            delegatingGroupSinks7 = new QS.CMS.Multicasting7.DelegatingGroupSinks(membershipController, regionViewSinks7);
            placeholderGSs = new QS.CMS.Multicasting7.PlaceholderGSs(membershipController, regionViewSinks7);
            groupSenders6 = new QS.CMS.Base6.GroupSenders(placeholderGSs);
*/ 
#endregion

            // core.Start();

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        #endregion

        #region Fields

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Statistics.IStatisticsController statisticsController;

        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Network.NetworkAddress coordinatorAddress;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;

        [QS._core_c_.Diagnostics2.Module("Platform")]
        // [QS.CMS.Diagnostics.Component("Platform")]
        private QS.Fx.Platform.IPlatform platform;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;

        private Base3_.IDemultiplexer demultiplexer;
        [QS._core_c_.Diagnostics2.Module("Root")]
        private Base8_.Root root;
        private Batching_.Unpacker unpacker;

        // [QS.CMS.Diagnostics.Component("Reliable Sender")]
        private QS._qss_c_.Senders3.ReliableSender1 reliableSender;
        private QS._qss_c_.Senders6.InstanceSender instanceSender;
        private QS._qss_c_.Senders6.InstanceReceiver instanceReceiver;
        [QS._core_c_.Diagnostics.Component("Reconnecting Reliable Sender")]
        private QS._qss_c_.Senders6.ReliableSender reconnectingReliableSender;        
        private QS._qss_c_.Senders3.ChoppingSender choppingSender;
        private QS._qss_c_.Senders3.SimpleChoppingSender simpleChoppingSender;

        private QS._qss_c_.RPC3.SimpleCaller simpleCaller;

        private QS._qss_c_.FailureDetection_.Centralized.Publisher fdPublisher;
        private QS._qss_c_.FailureDetection_.Centralized.Subscriber fdSubscriber;
        private QS._qss_c_.FailureDetection_.IFailureDetector localFD;
        private QS._qss_c_.FailureDetection_.Centralized.Server failureDetectionServer;
        private QS._qss_c_.FailureDetection_.Centralized.Agent failureDetectionClient;
        
        private QS._qss_c_.Membership2.Servers.GenericServer centralizedServer;
        private QS._qss_c_.Membership2.Controllers.SimpleController membershipController;
        private QS._qss_c_.Membership2.ClientAgent.ClientAgent clientAgent;

        [QS._core_c_.Diagnostics.Component("Reliable Instance Sinks")]
        [QS._core_c_.Diagnostics2.Module("ReliableInstanceSinks")]
        private QS._qss_c_.Senders11.ReliableInstanceSinks reliableInstanceSinks;
        // [QS.CMS.Diagnostics.Component("Simple Instance Receiver")]
        private QS._qss_c_.Receivers5.SimpleInstanceReceiver simpleInstanceReceiver;
        [QS._core_c_.Diagnostics.Component("Instance Receiver Controller")]
        [QS._core_c_.Diagnostics2.Module("InstanceReceiverController")]
        private QS._qss_c_.Receivers5.InstanceReceiverController instanceReceiverController;
        // [QS.CMS.Diagnostics.Component("Instance Ack Collector")]
        private QS._qss_c_.Receivers5.InstanceAckCollector instanceAckCollector;
        [QS._core_c_.Diagnostics2.Module("BufferedReliableInstanceSinks")]
        private QS._qss_c_.Base6_.BufferCollection<QS._core_c_.Base3.InstanceID> bufferedReliableInstanceSinks;

        #endregion

        #region Accessors

        public Base3_.IDemultiplexer Demultiplexer
        {
            get { return demultiplexer; }
        }

        // [QS.CMS.Diagnostics.Component("Root")]
        public Base8_.Root Root
        {
            get { return root; }
        }

        public QS.Fx.Logging.ILogger Logger
        {
            get { return logger; }
        }

        public QS.Fx.Logging.IEventLogger EventLogger
        {
            get { return eventLogger; }
        }

        public QS.Fx.Clock.IAlarmClock AlarmClock
        {
            get { return alarmClock; }
        }

        public QS.Fx.Clock.IClock Clock
        {
            get { return clock; }
        }

        public QS._core_c_.Base3.InstanceID LocalAddress
        {
            get { return localAddress; }
        }

        public QS.Fx.Platform.IPlatform Platform
        {
            get { return platform; }
        }

        public QS._core_c_.Statistics.IStatisticsController StatisticsController
        {
            get { return statisticsController; }
        }

        public QS._qss_c_.Membership2.Controllers.IMembershipController MembershipController
        {
            get { return membershipController; }
        }

        public QS._qss_c_.Membership2.ClientAgent.IClientAgent MembershipAgent
        {
            get { return clientAgent; }
        }

        public QS._qss_c_.Senders6.ReliableSender ReliableSender
        {
            get { return reconnectingReliableSender; }
        }

        public QS._qss_c_.Senders11.ReliableInstanceSinks ReliableInstanceSinks
        {
            get { return reliableInstanceSinks; }
        }

        public QS._qss_c_.Base6_.BufferCollection<QS._core_c_.Base3.InstanceID> BufferedReliableInstanceSinks
        {
            get { return bufferedReliableInstanceSinks; }
        }

        public QS._qss_c_.Membership2.Servers.GenericServer MembershipServer
        {
            get { return this.centralizedServer; }
        }

        public QS._qss_c_.FailureDetection_.Centralized.Server FailureDetectionServer
        {
            get { return failureDetectionServer; }
        }

        public QS._qss_c_.FailureDetection_.Centralized.Agent FailureDetectionAgent
        {
            get { return failureDetectionClient; }
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion
    }
}
