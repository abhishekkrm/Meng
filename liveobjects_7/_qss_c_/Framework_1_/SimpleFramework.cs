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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace QS._qss_c_.Framework_1_
{
    [QS._core_c_.Diagnostics.ComponentContainer(QS.Fx.Diagnostics.SelectionOption.Explicit)]
	[QS.Fx.Base.Inspectable]
    public class SimpleFramework : QS.Fx.Inspection.Inspectable, System.IDisposable, QS._qss_e_.Base_1_.IStatisticsCollector
    {
        public enum ReceiverType
        {
            Base3, Base7
        };

        public static bool ReplaceDevices6SinkWithDevices4AsynchronousSender = false;

        #region Additional Constructors

        public SimpleFramework() : this(QS._core_c_.Base3.Incarnation.Current)
		{
		}

        public SimpleFramework(QS._core_c_.Base3.Incarnation incarnation) : this(incarnation, 0, null)
        {
        }

        public SimpleFramework(QS._core_c_.Base3.Incarnation incarnation, int portno, QS.Fx.Network.NetworkAddress membershipServerAddress) 
            : this(incarnation, true, new QS._qss_c_.Platform_.PhysicalPlatform(), new QS.Fx.Network.NetworkAddress(
            Devices_2_.Network.LocalAddresses[0], portno), membershipServerAddress)
        {
        }

/*

        public SimpleFramework(Platform.IPlatform platform, Base.Subnet subnet) : this(platform, subnet, 0)
        {
        }

        public SimpleFramework(Platform.IPlatform platform, Base.Subnet subnet, int portno) : this(platform, Devices2.Network.AnyAddressOn(subnet, platform), portno)
        {
        }

        public SimpleFramework(Platform.IPlatform platform, IPAddress address) : this(platform, address, 0)
        {
        }

        public SimpleFramework(Platform.IPlatform platform, IPAddress address, int portno) : this(platform, new QS.Fx.Network.NetworkAddress(address, portno))
        {
        }
*/

        public SimpleFramework(QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Network.NetworkAddress localAddress,
            QS.Fx.Network.NetworkAddress membershipServerAddress)
			: this(QS._core_c_.Base3.Incarnation.Current, false, platform, localAddress, membershipServerAddress)
		{
		}

        public SimpleFramework(QS._core_c_.Base3.Incarnation incarnation, QS._qss_c_.Platform_.IPlatform platform, QS.Fx.Network.NetworkAddress localAddress,
            QS.Fx.Network.NetworkAddress membershipServerAddress) 
            : this(incarnation, false, platform, localAddress, membershipServerAddress)
        {
        }

        private SimpleFramework(QS._core_c_.Base3.Incarnation incarnation, bool disposeOfPlatform, QS._qss_c_.Platform_.IPlatform platform, 
			QS.Fx.Network.NetworkAddress localAddress, QS.Fx.Network.NetworkAddress membershipServerAddress) 
			: this(incarnation, disposeOfPlatform, platform, localAddress, membershipServerAddress, 
				Membership2.Servers.Type.OVERLAPPING, ReceiverType.Base3)
        {
        }

        #endregion

        private static readonly TimeSpan defaultIntervalBetweenHeartbeats = TimeSpan.FromSeconds(1);

        #region Main Constructor

#pragma warning disable 0162

        public SimpleFramework(QS._core_c_.Base3.Incarnation incarnation, bool disposeOfPlatform, QS._qss_c_.Platform_.IPlatform platform, 
			QS.Fx.Network.NetworkAddress localAddress, QS.Fx.Network.NetworkAddress membershipServerAddress, 
			Membership2.Servers.Type membershipServerType, ReceiverType receiverType)
        {
			platform.Logger.Log(this, "Compiled for: " + QS.Platform.Target.ToString() + ".");

            this.receiverType = receiverType;
            this.disposeOfPlatform = disposeOfPlatform;
            this.platform = platform;

            scheduler = new QS._qss_c_.Scheduling_1_.Scheduler1(platform.Clock, platform.EventLogger);

            device = platform.Network[localAddress.HostIPAddress][QS._qss_c_.Devices_3_.CommunicationsDevice.Class.UDP];

            if (platform is QS._qss_c_.Platform_.PhysicalPlatform)
            {
                device.MTU = 65000;
            }

            demultiplexer = new QS._qss_c_.Base3_.Demultiplexer(platform.Logger, platform.EventLogger);

            rootSender = new QS._qss_c_.Base3_.RootSender(platform.EventLogger,
                incarnation, platform.Logger, device, localAddress.PortNumber, demultiplexer, platform.Clock,
                receiverType != ReceiverType.Base3);

            multicastResponder = new QS._qss_c_.Connections_.MulticastResponder(platform, localAddress);

            platform.Logger.Log(this, "Local_InstanceID : " + rootSender.InstanceID.ToString());

            this.reliableSender = new Senders3.ReliableSender1(
				platform.Logger, demultiplexer, rootSender.SenderCollection, platform.AlarmClock, 1.0);

			instanceID = rootSender.InstanceID; // new QS.CMS.QS._core_c_.Base3.InstanceID(rootSender.Address, 1);

            if (receiverType == ReceiverType.Base7)
            {
                root7 = new QS._qss_c_.Base7_.Root(platform.EventLogger, instanceID, platform.Clock,
                    platform.Connections7, scheduler, demultiplexer);
                dev3MC = root7;
            }
            else
            {
                dev3MC = rootSender;
            }

            unwrapper = new QS._qss_c_.Buffering_3_.Unwrapper((uint)ReservedObjectID.Unwrapper, demultiplexer,
                QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass, platform.Logger);

            lazySender = new QS._qss_c_.Buffering_3_.LazySender((uint)ReservedObjectID.Unwrapper,
                rootSender.SenderCollection, QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass,
                platform.AlarmClock, TimeSpan.FromSeconds(0.1), platform.Logger);
            thresholdSender = new QS._qss_c_.Buffering_3_.ThresholdSender1((uint)ReservedObjectID.Unwrapper,
                rootSender.SenderCollection, QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass,
                platform.AlarmClock, platform.Clock, 0.1, 100, platform.Logger);

            instanceReceiver = new QS._qss_c_.Senders6.InstanceReceiver(platform.Logger, instanceID, demultiplexer);

            fdSubscriber = new QS._qss_c_.FailureDetection_.Centralized.Subscriber(platform.Logger, demultiplexer);

            if (membershipServerAddress != null && membershipServerAddress.Equals(localAddress))
            {
                failureDetectionServer = new QS._qss_c_.FailureDetection_.Centralized.Server(platform.Logger, demultiplexer, platform.AlarmClock,
                    defaultIntervalBetweenHeartbeats, 10, false);
                
                instanceSender = new QS._qss_c_.Senders6.InstanceSender(
                    platform.Logger, failureDetectionServer, rootSender.SenderCollection, reliableSender.SenderCollection);

                localFD = failureDetectionServer;
            }
            else
            {
                failureDetectionServer = null;

                instanceSender = new QS._qss_c_.Senders6.InstanceSender(platform.Logger, null, rootSender.SenderCollection, null);

                localFD = fdSubscriber;
            }

            choppingSender = new QS._qss_c_.Senders3.ChoppingSender(
                reliableSender.SenderCollection, instanceSender, platform.Logger, demultiplexer);

            if (failureDetectionServer != null)
            {
                fdPublisher = new QS._qss_c_.FailureDetection_.Centralized.Publisher(platform.Logger, failureDetectionServer, choppingSender);
            }

            reconnectingReliableSender = new QS._qss_c_.Senders6.ReliableSender(
                instanceID, platform.Logger, demultiplexer, platform.AlarmClock, platform.Clock, rootSender.SenderCollection, localFD);

            aggregationAgent = new QS._qss_c_.Aggregation1_.AggregationAgent(platform.Logger, instanceID,
                new Senders6.InstanceSender(platform.Logger, localFD, rootSender.SenderCollection, null), demultiplexer);

            // we switch it onto this sedner to enforce ordering
            simpleCaller = new QS._qss_c_.RPC3.SimpleCaller(platform.Logger, reconnectingReliableSender, demultiplexer, platform.AlarmClock, 60);

            remotingChannel = new QS._qss_c_.Interoperability.Remoting.Channel(
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
                            centralizedServer = new QS._qss_c_.Membership2.Servers.RegularServer(platform.Logger,
                                demultiplexer, platform.AlarmClock, choppingSender.SenderCollection, failureDetectionServer, choppingSender);
                        }
                        break;

                        case Membership2.Servers.Type.OVERLAPPING:
                        {
                            centralizedServer = new QS._qss_c_.Membership2.Servers.OverlappingServer(platform.Logger,
                                demultiplexer, platform.AlarmClock, choppingSender.SenderCollection, failureDetectionServer, choppingSender, false);
                        }
                        break;

                        default:
                            break;
                    }
                }

                membershipController = new QS._qss_c_.Membership2.Controllers.SimpleController(instanceID, dev3MC, platform.Logger);
                clientAgent = new QS._qss_c_.Membership2.ClientAgent.ClientAgent(platform.Logger, demultiplexer,
                    simpleCaller.SenderCollection[membershipServerAddress], rootSender.Address, membershipController, 
					instanceID);

                failureDetectionClient = new QS._qss_c_.FailureDetection_.Centralized.Agent(platform.Logger, instanceID,
                    rootSender.SenderCollection[membershipServerAddress], platform.AlarmClock, defaultIntervalBetweenHeartbeats, false);
            }

			regionSender = new QS._qss_c_.Multicasting3.RegionSender1(
				platform.Logger, platform.Clock, membershipController, rootSender.SenderCollection, 10);

			aggregationRouter = new Aggregation3_.Router1(membershipController, Routing_1_.NoRouting.Algorithm);
			aggregation3Agent = new QS._qss_c_.Aggregation3_.Agent(
				platform.Logger, instanceID, typeof(Aggregation3_.Controller1), aggregationRouter,
				((Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>) reliableSender).SenderCollection, // rootSender.SenderCollection, 
				demultiplexer, platform.AlarmClock, Components_1_.SeqCollection<Aggregation3_.Controller>.CollectionConstructor);

			// Base3.Serializer.registerClass(ClassID.Base3_GroupID, typeof(Base3.GroupID));
			// Base3.Serializer.registerClass(ClassID.Base3_RegionID, typeof(Base3.RegionID));

			reliableSender2 = new Senders3.ReliableSender1(
				platform.Logger, demultiplexer, rootSender.SenderCollection, platform.AlarmClock, 1.0,
                (uint) ReservedObjectID.Senders3_ReliableSender_MessageChannel_2,
                (uint) ReservedObjectID.Senders3_ReliableSender_AcknowledgementChannel_2);

			aggregation4Controller1 = new Aggregation4_.AggregationController1(platform.AlarmClock, platform.Clock,
				0.2, ((Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>) reliableSender2).SenderCollection,
				((Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>) reliableSender).SenderCollection);

			aggregation4Agent = new QS._qss_c_.Aggregation4_.Agent(platform.Logger, instanceID.Address,
				aggregation4Controller1, demultiplexer, Routing_1_.NoRouting.Algorithm, membershipController);

//			if (platform is QS.CMS.Simulations2.SimulatedPlatform)
//			{
//				platform.Logger.Log(this, "Simulated platform detected, adjusting timeouts.");
//				if (centralizedServer != null)
//					centralizedServer.RequestAggregationInterval *= 10;
//			}

            regionalAgent = new QS._qss_c_.Gossiping2.RegionalAgent(platform.Logger, instanceID, 
                membershipController, platform.AlarmClock,reconnectingReliableSender, demultiplexer, platform.Clock);
            gossipAdapter = new QS._qss_c_.Gossiping2.GossipAdapter(platform.Logger, regionalAgent);
            receiveRateGCC = new QS._qss_c_.Gossiping2.ReceiveRateGCC1(platform.Logger, rootSender);
            ((QS._qss_c_.Gossiping2.IGossipAdapter)gossipAdapter).Register(receiveRateGCC);

            bufferingUNS = new QS._qss_c_.Senders9.BufferingUNS(platform.Logger, rootSender.SenderCollection,
                QS._qss_c_.Buffering_3_.AccumulatingController.ControllerClass, platform.AlarmClock, platform.Clock);
            multicastingURS = new QS._qss_c_.Multicasting5.MulticastingURS(platform.Logger, membershipController, bufferingUNS);
            simpleURVS = new QS._qss_c_.Multicasting5.SimpleURVS(platform.Logger, multicastingURS);

            gossipingRRVS = new QS._qss_c_.Multicasting5.GossipingRRVS(platform.Logger, instanceID, 
                multicastingURS, demultiplexer, platform.AlarmClock, regionalAgent, membershipController, platform.Clock);

            newRoot = new QS._qss_c_.Base4_.Root(platform.Logger, platform.NetworkConnections, incarnation);
            mainConnection = ((Base4_.IRoot) newRoot).Connect(localAddress);
            newMulticastingURS = new QS._qss_c_.Multicasting6.MulticastingURS(platform.Logger, membershipController, mainConnection);
            newSimpleURVS = new QS._qss_c_.Multicasting6.SimpleURVS(platform.Logger, newMulticastingURS);
            newDelegatingGS = new QS._qss_c_.Multicasting6.DelegatingGS(platform.Logger, membershipController, newSimpleURVS);

            ringRRVS = new QS._qss_c_.Rings4.RingRRVS(platform.Logger, instanceID, demultiplexer, platform.AlarmClock, 
                platform.Clock, membershipController, multicastingURS, reconnectingReliableSender, 0.01, 100);

            // TODO: Fix this....
            throw new NotImplementedException("Cannot create this framework because reliable instance sinks have not been hooked up here yet.");

            rings6_receivingAgent1 = new QS._qss_c_.Rings6.ReceivingAgent(platform.EventLogger, platform.Logger, 
                instanceID, platform.AlarmClock, platform.Clock, demultiplexer, reconnectingReliableSender, 5, 1, 0.1, 10, 5000,
                QS._qss_c_.Rings6.RateSharingAlgorithmClass.Compete, null, null);

            receivers4_regionalController = new QS._qss_c_.Receivers4.RegionalController(instanceID, platform.Logger, demultiplexer,
                platform.AlarmClock, platform.Clock, membershipController, rings6_receivingAgent1, rings6_receivingAgent1);
            senders10_regionalSenders = new QS._qss_c_.Senders10.RegionalSenders(platform.EventLogger, instanceID, 
                platform.Logger, platform.AlarmClock, platform.Clock, demultiplexer, multicastingURS, receivers4_regionalController, 
                receivers4_regionalController, 60, receivers4_regionalController, false, 0, 1);
            senders10_rrvsSenders = 
                new QS._qss_c_.Senders10.SenderCollection<QS._qss_c_.Base3_.RVID>(platform.Logger, senders10_regionalSenders);

            if (ReplaceDevices6SinkWithDevices4AsynchronousSender)
                base6root = new QS._qss_c_.Base6_.Root(instanceID, platform.Clock, ((QS._qss_c_.Devices_4_.INetwork)platform.NetworkConnections));
            else
                base6root = new QS._qss_c_.Base6_.Root(instanceID, platform.Clock, ((QS._qss_c_.Devices_6_.INetwork)platform));

            regionSinks7 = new QS._qss_c_.Multicasting7.RegionSinks(membershipController, base6root);
            regionViewSinks7 = new QS._qss_c_.Multicasting7.ReliableRegionViewSinks(null, platform.Logger, platform.EventLogger, instanceID,
                platform.AlarmClock, platform.Clock, (uint) QS.ReservedObjectID.Rings6_SenderController1_DataChannel,
                (uint)QS.ReservedObjectID.Rings6_SenderController1_RetransmissionChannel, 
                regionSinks7, receivers4_regionalController, receivers4_regionalController,membershipController, null, 10, null, 1);
            delegatingGroupSinks7 = new QS._qss_c_.Multicasting7.DelegatingGroupSinks(membershipController, regionViewSinks7, 100000);
            placeholderGSs = new QS._qss_c_.Multicasting7.PlaceholderGSs(membershipController, regionViewSinks7);
            groupSenders6 = new QS._qss_c_.Base6_.GroupSenders(placeholderGSs, platform.Clock);
        }

#pragma warning restore 0162

        #endregion

        private ReceiverType receiverType;
        private QS._qss_c_.Connections_.MulticastResponder multicastResponder;
        private bool disposeOfPlatform;
        private QS._qss_c_.Platform_.IPlatform platform;
        private QS._qss_c_.Devices_3_.ICommunicationsDevice device;
        private QS._qss_c_.Base3_.IDemultiplexer demultiplexer;
		[QS._qss_e_.Base_1_.StatisticsCollector]
        [QS._core_c_.Diagnostics.Component("Root Sender")]
        private QS._qss_c_.Base3_.RootSender rootSender;
		// [TMS.Base.StatisticsCollector]
		private QS._qss_c_.Senders3.ReliableSender1 reliableSender, reliableSender2;
		private QS._qss_c_.RPC3.SimpleCaller simpleCaller;
        private QS._qss_c_.Interoperability.Remoting.Channel remotingChannel;
        private QS._qss_c_.Membership2.Servers.GenericServer centralizedServer;
        private QS._core_c_.Base3.InstanceID instanceID;
        private QS._qss_c_.Membership2.Controllers.SimpleController membershipController;
        private QS._qss_c_.Membership2.ClientAgent.ClientAgent clientAgent;
        private QS._qss_c_.Buffering_3_.Unwrapper unwrapper;
        private QS._qss_c_.Aggregation1_.AggregationAgent aggregationAgent;
        private QS._qss_c_.Buffering_3_.LazySender lazySender;
        private QS._qss_c_.Buffering_3_.ThresholdSender1 thresholdSender;
        private QS._qss_c_.Senders3.ChoppingSender choppingSender;
		private QS._qss_c_.Aggregation3_.Agent aggregation3Agent;
		// [TMS.Base.StatisticsCollector]
		private QS._qss_c_.Multicasting3.RegionSender1 regionSender;
		private QS._qss_c_.Aggregation3_.IRouter aggregationRouter;
		// [TMS.Base.StatisticsCollector]
		private QS._qss_c_.Aggregation4_.AggregationController1 aggregation4Controller1;
		private QS._qss_c_.Aggregation4_.Agent aggregation4Agent;
        private QS._qss_c_.FailureDetection_.Centralized.Server failureDetectionServer;
        private QS._qss_c_.FailureDetection_.Centralized.Agent failureDetectionClient;
        private QS._qss_c_.Senders6.InstanceSender instanceSender;
        private QS._qss_c_.FailureDetection_.Centralized.Publisher fdPublisher;
        private QS._qss_c_.FailureDetection_.Centralized.Subscriber fdSubscriber;
        private QS._qss_c_.FailureDetection_.IFailureDetector localFD;
        [QS._core_c_.Diagnostics.Component("Reconnecting Reliable Sender")]
        private QS._qss_c_.Senders6.ReliableSender reconnectingReliableSender;
        private QS._qss_c_.Senders6.InstanceReceiver instanceReceiver;
        [QS._qss_e_.Base_1_.StatisticsCollector]
        private QS._qss_c_.Gossiping2.RegionalAgent regionalAgent;
        private QS._qss_c_.Gossiping2.GossipAdapter gossipAdapter;
        [QS._qss_e_.Base_1_.StatisticsCollector]
        private QS._qss_c_.Gossiping2.ReceiveRateGCC1 receiveRateGCC;
        [QS._qss_e_.Base_1_.StatisticsCollector]
        private Senders9.BufferingUNS bufferingUNS;
        private Multicasting5.MulticastingURS multicastingURS;
        private Multicasting5.SimpleURVS simpleURVS;
        private Multicasting5.GossipingRRVS gossipingRRVS;
        private Base4_.Root newRoot;
        private Base4_.IConnection mainConnection;
        private Multicasting6.MulticastingURS newMulticastingURS;
        private Multicasting6.SimpleURVS newSimpleURVS;
        private Multicasting6.DelegatingGS newDelegatingGS;
        private Rings4.RingRRVS ringRRVS;
        [QS._core_c_.Diagnostics.Component("Receivers4.RegionalController")]
        private Receivers4.RegionalController receivers4_regionalController;
        private Rings6.ReceivingAgent rings6_receivingAgent1;        
        private Senders10.RegionalSenders senders10_regionalSenders;
        [QS._core_c_.Diagnostics.Component("Senders10.RRVSSenders")]
        private Senders10.SenderCollection<Base3_.RVID> senders10_rrvsSenders;
        [QS._core_c_.Diagnostics.Component("Base6.Root")]
        private Base6_.Root base6root;
        private Multicasting7.RegionSinks regionSinks7;
        [QS._core_c_.Diagnostics.Component("Multicasting7.ReliableRegionViewSinks")]
        private Multicasting7.ReliableRegionViewSinks regionViewSinks7;
        private Multicasting7.DelegatingGroupSinks delegatingGroupSinks7;
        private Multicasting7.PlaceholderGSs placeholderGSs;
        private Base6_.GroupSenders groupSenders6;
        private Scheduling_1_.Scheduler1 scheduler;
        private Base7_.Root root7;
        private Devices_3_.IMembershipController dev3MC;

        #region Accessors

        public Base6_.Root Root6
        {
            get { return base6root; }
        }

        public Multicasting7.RegionSinks RegionSinks7
        {
            get { return regionSinks7; }
        }

        public Multicasting7.ReliableRegionViewSinks RegionViewSinks7
        {
            get { return regionViewSinks7; }
        }

        public Multicasting7.DelegatingGroupSinks DelegatingGroupSinks7
        {
            get { return delegatingGroupSinks7; }
        }

        public Multicasting7.PlaceholderGSs PlaceholderGSs
        {
            get { return placeholderGSs; }
        }

        public Base6_.GroupSenders GroupSenders6
        {
            get { return groupSenders6; }
        }

        public Rings6.ReceivingAgent Rings6_ReceivingAgent1
        {
            get { return rings6_receivingAgent1; }
        }

        public Senders10.SenderCollection<Base3_.RVID> RRVS10
        {
            get { return senders10_rrvsSenders; }
        }

        public Receivers4.RegionalController Receivers4_RegionalController
        {
            get { return receivers4_regionalController; }
        }

        public Rings4.RingRRVS RingRRVS
        {
            get { return ringRRVS; }
        }

        public Base4_.Root NewRoot
        {
            get { return newRoot; }
        }

        public Multicasting6.MulticastingURS NewMulticastingURS
        {
            get { return newMulticastingURS; }
        }

        public Multicasting6.SimpleURVS NewSimpleURVS
        {
            get { return newSimpleURVS; }
        }

        public Multicasting6.DelegatingGS NewDelegatingGS
        {
            get { return newDelegatingGS; }
        }

        public QS._qss_c_.Gossiping2.RegionalAgent RegionalAgent
        {
            get { return regionalAgent; }
        }

        public Multicasting5.GossipingRRVS GossipingRRVS
        {
            get { return gossipingRRVS; }
        }

        public Multicasting5.SimpleURVS SimpleURVS
        {
            get { return simpleURVS; }
        }

        public Senders9.BufferingUNS BufferingUNS
        {
            get { return bufferingUNS; }
        }

        public Multicasting5.MulticastingURS MulticastingURS
        {
            get { return multicastingURS; }
        }

        public QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> UnreliableInstanceSenderCollection
        {
            get { return instanceSender; }
        }

        public QS._qss_c_.Senders6.ReliableSender ReconnectingReliableSender
        {
            get { return reconnectingReliableSender; }
        }

        public QS._qss_c_.FailureDetection_.IFailureDetector FailureDetector
        {
            get { return localFD; }
        }

        public QS._core_c_.Base3.InstanceID InstanceID
        {
            get { return instanceID; }
        }

		public QS._qss_c_.Aggregation4_.AggregationController1 Aggregation4Controller1
		{
			get { return aggregation4Controller1; }
		}

		[QS.Fx.Base.Inspectable]
		public string DumpThreadStates
		{
			get { return Debugging_.Threads.DumpStates; }
		}

		public QS._qss_c_.Aggregation3_.IRouter AggregationRouter
		{
			get { return aggregationRouter; }
		}

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Senders3.ChoppingSender ChoppingSender
		{
            get { return choppingSender; }
        }

		[QS.Fx.Base.Inspectable]
        public QS._qss_c_.Aggregation1_.AggregationAgent AggregationAgent
        {
            get { return aggregationAgent; }
        }

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Aggregation3_.Agent Aggregation3Agent
		{
			get { return aggregation3Agent; }
		}

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Aggregation4_.Agent Aggregation4Agent
		{
			get { return aggregation4Agent; }
		}

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Multicasting3.RegionSender1 RegionSender
		{
			get { return regionSender; }
		}

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Membership2.Controllers.IMembershipController MembershipController
		{
            get { return membershipController; }
        }

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Buffering_3_.LazySender LazySender
		{
            get { return lazySender; }
        }

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Buffering_3_.ThresholdSender1 ThresholdSender
		{
            get { return thresholdSender; }
        }

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Membership2.ClientAgent.IClientAgent MembershipAgent
		{
            get { return clientAgent; }
        }

		[QS.Fx.Base.Inspectable]
        public QS._qss_c_.Platform_.IPlatform Platform
		{
            get { return platform; }
        }

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Devices_3_.ICommunicationsDevice Device
		{
            get { return device; }
        }

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Base3_.IDemultiplexer Demultiplexer
		{
            get { return demultiplexer; }
        }

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Base3_.RootSender RootSender
		{
            get { return rootSender; }
        }

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableCaller> SerializableCaller
		{
            get { return simpleCaller; }
        }

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Base3_.ISenderClass<Base3_.IReliableSerializableSender> ReliableSender
		{
            get { return reliableSender; }
        }

		[QS.Fx.Base.Inspectable]
		public QS._qss_c_.Base3_.ISenderClass<Base3_.IReliableSerializableSender> ReliableSender2
		{
			get { return reliableSender2; }
		}

		// [TMS.Inspection.Inspectable]
		public QS._qss_c_.Buffering_3_.Unwrapper Unwrapper
		{
			get { return unwrapper; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            platform.Logger.Log(this, "__Dispose");
            rootSender.Dispose();
            platform.ReleaseResources();
            if (disposeOfPlatform)
                platform.Dispose();
        }

        #endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get { return QS._qss_e_.Inspection_.StatisticsOf.GetStatistics(this); }
		}

		#endregion
	}
}
