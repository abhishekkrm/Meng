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

// #define DEBUG_RegionalAgent
// #define DEBUG_Shipments
// #define STATISTICS_CollectTokenStatistics
// #define DEBUG_UpdateAddresses

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Gossiping2
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class RegionalAgent : QS.Fx.Inspection.Inspectable, QS._qss_e_.Base_1_.IStatisticsCollector, IRegionalAgent, 
        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender>, 
        Senders8.ISink<Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID>>
    {
        public const double DefaultTokenRate = 1;

        #region Constructor

        public RegionalAgent(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, Membership2.Controllers.IMembershipController membershipController,
            QS.Fx.Clock.IAlarmClock alarmClock, Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> senderCollection, 
            Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IClock clock)
        {
            this.logger = logger;
            this.localIID = localIID;
            this.membershipController = membershipController;
            this.alarmClock = alarmClock;
            this.senderCollection = senderCollection;
            this.demultiplexer = demultiplexer;
            this.clock = clock;

            this.TokenRate = DefaultTokenRate;

            demultiplexer.register((uint)ReservedObjectID.Gossiping2_RegionalAgent, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
            demultiplexer.register((uint)ReservedObjectID.Gossiping2_RegionalAgent_InterregionalChannel, 
                new QS._qss_c_.Base3_.ReceiveCallback(this.InterregionalReceiveCallback));

            ((Membership2.Consumers.IRegionChangeProvider) membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.RegionChangedCallback(this.RegionChangeCallback);
        }

        #endregion

        #region TokenRate

        public double TokenRate
        {
            get { return 1 / gossipingInterval; }
            set 
            { 
                gossipingInterval = 1 / value;
                Agent agent = currentAgent;
                if (agent != null)
                    agent.SetGossipingInterval(gossipingInterval);
            }
        }

        #endregion

#if STATISTICS_CollectTokenStatistics
        [QS.CMS.Diagnostics.Component("Token Creation Times")]
        private CMS.Statistics.Samples tokenCreationTimes = new QS.CMS.Statistics.Samples();
        [QS.CMS.Diagnostics.Component("Collected Token Sizes")]
        private CMS.Statistics.SamplesXY collectedTokenSizes = new QS.CMS.Statistics.SamplesXY();
#endif

        private bool gossiping_enabled = false;

        public bool Enabled
        {
            get { return gossiping_enabled; }
            set { gossiping_enabled = true; }
        }

        private double gossipingInterval;
        private QS.Fx.Logging.ILogger logger;
        private QS._core_c_.Base3.InstanceID localIID;
        private QS.Fx.Clock.IClock clock;
        private Agent currentAgent;
        private Membership2.Controllers.IMembershipController membershipController;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> senderCollection;
        private GetAggregatableCallback getAggregatableCallback;
        private SetAggregatableCallback setAggregatableCallback;
        private System.Random myrandom = new System.Random();
        private Base3_.IDemultiplexer demultiplexer;

        private Queue<QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID>> sources =
            new Queue<QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID>>();
        private IShipmentCollection shipments = new ShipmentCollection();

        #region Send

        private void Send(QS._core_c_.Base3.InstanceID destinationAddress, uint destinationLOID, QS.Fx.Serialization.ISerializable data)
        {
            lock (shipments)
            {
                shipments.Add(localIID, destinationAddress, new QS._core_c_.Base3.Message(destinationLOID, data));
            }
        }

        #endregion

        #region Managing Shipments

        private void Ship(IShipmentCollection shipmentCollection)
        {
            lock (shipments)
            {
                shipmentCollection.Add(shipments);
                shipments.Clear();
            }

            lock (sources)
            {
                foreach (QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID> source in sources)
                {
                    Nullable<Base3_.Addressed<QS._core_c_.Base3.InstanceID, Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>>> element;
                    while (source.Ready && (element = source.Get(uint.MaxValue)).HasValue)
                        shipmentCollection.Add(localIID, element.Value.Address, element.Value.Content.Argument);
                }
                sources.Clear();
            }
        }

        private void Unload(IShipmentCollection shipmentCollection)
        {
            lock (shipments)
            {
                foreach (QS._core_c_.Base3.InstanceID destinationAddress in shipmentCollection.Destinations)
                {
                    IShipment shipment = shipmentCollection[destinationAddress];
                    if (destinationAddress.Equals(localIID))
                    {
                        foreach (QS._core_c_.Base3.InstanceID sourceAddress in shipment.Sources)
                            foreach (QS._core_c_.Base3.Message message in shipment[sourceAddress])
                                demultiplexer.dispatch(message.destinationLOID, sourceAddress, message.transmittedObject);
                    }
                    else if (destinationAddress.Address.Equals(localIID.Address))
                    {
                        logger.Log(this, "__Unload : Throwing away messages for another incarnation " + destinationAddress.ToString());
                    }
                    else
                    {
                        // we should also check if this node still exists, hasn;t failed, but we leave it out for now...

                        shipments.Add(destinationAddress, shipment);
                    }
                }
            }
        }

        private void RegionalForward(Membership2.ClientState.IRegionView regionVC, IShipmentCollection shipmentCollection)
        {
            lock (shipments)
            {
                foreach (QS._core_c_.Base3.InstanceID destinationAddress in regionVC.Members)
                {
                    IShipment shipment = shipments.Remove(destinationAddress);
                    if (shipment != null)
                        shipmentCollection.Add(destinationAddress, shipment);
                }
            }
        }

        private void CleanupRegional(Base3_.RegionID regionID)
        {
            // ......................................... get rid of messages that do not belong anywhere (those from from removed nodes or regions)
        }

        #endregion

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            IntraregionalMessage message = receivedObject as IntraregionalMessage;
            if (message != null)
            {
                Agent agent = null;
                lock (this)
                {
                    if (currentAgent != null && currentAgent.RegionView.Region.ID.Equals(message.RegionID))
                        agent = currentAgent;
                }

                if (agent != null)
                    agent.ReceiveCallback(sourceIID, message);
                else
                {
#if DEBUG_RegionalAgent
                    logger.Log(this, "No agent for region " + message.RegionID.ToString());
#endif
                }
            }
            else
            {
#if DEBUG_RegionalAgent
                logger.Log(this, "Wrong message type.");
#endif
            }

            return null;
        }

        #endregion

        #region Interregional Receive Callback

        private QS.Fx.Serialization.ISerializable InterregionalReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            InterregionalMessage message = receivedObject as InterregionalMessage;
            if (message != null)
            {
                Agent agent = null;
                lock (this)
                {
                    if (currentAgent != null && currentAgent.RegionView.Region.ID.Equals(message.DestinationRegionID))
                        agent = currentAgent;
                }

                if (agent != null)
                    agent.InterregionalReceiveCallback(sourceIID, message);
            }

            return null;
        }

        #endregion

        #region Internal Processing

        // This is deliberately written in a way as to tolerate small inconsistencies.
        private void RegionChangeCallback(QS._qss_c_.Membership2.Consumers.RegionChange change)
        {
            if (!gossiping_enabled)
                return;

            lock (this)
            {
                bool newlyCreated = false;
                switch (change.LocalChange)
                {
                    case Membership2.Consumers.RegionChange.KindOf.ENTERED_REGION:
                    case Membership2.Consumers.RegionChange.KindOf.SWITCHED_REGION:
                    {
                        if (currentAgent != null)
                        {
                            if (currentAgent.RegionView.Region.ID.Equals(change.CurrentView.Region.ID))
                            {
                                if (!currentAgent.RegionView.SeqNo.Equals(change.CurrentView.SeqNo))
                                    currentAgent.RegionView = change.CurrentView;
                            }
                            else
                                ((IDisposable)currentAgent).Dispose();
                        }
                        else
                        {
                            currentAgent = new Agent(this, change.CurrentView, 
                                ((Membership2.ClientState.IConfiguration) membershipController).NeighboringRegions);
                            newlyCreated = true;
                        }
                    }
                    break;

                    case Membership2.Consumers.RegionChange.KindOf.LEFT_REGION:
                    {
                        if (currentAgent != null)
                        {
                            ((IDisposable) currentAgent).Dispose();
                            currentAgent = null;
                        }
                    }
                    break;

                    case Membership2.Consumers.RegionChange.KindOf.MEMBERSHIP_CHANGED:
                    {
                        if (currentAgent != null)
                        {
                            if (currentAgent.RegionView.Region.ID.Equals(change.CurrentView.Region.ID))
                            {
                                if (currentAgent.RegionView.SeqNo != change.CurrentView.SeqNo)
                                    currentAgent.RegionView = change.CurrentView;
                            }
                            else
                            {
                                ((IDisposable)currentAgent).Dispose();
                                currentAgent = new Agent(this, change.CurrentView,
                                    ((Membership2.ClientState.IConfiguration)membershipController).NeighboringRegions);
                                newlyCreated = true;
                            }
                        }
                        else
                        {
                            currentAgent = new Agent(this, change.CurrentView,
                                ((Membership2.ClientState.IConfiguration)membershipController).NeighboringRegions);
                            newlyCreated = true;
                        }
                    }
                    break;
                }

                if ((change.RegionsAdded.Count > 0 || change.RegionsRemoved.Count > 0) && currentAgent != null && !newlyCreated)
                {
                    currentAgent.NeighborsChanged(change.RegionsAdded, change.RegionsRemoved);
                }
            }
        }

        #endregion

        #region Class IntraregionalMessage

        [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_RegionalAgent_IntraregionalMessage)]
        private class IntraregionalMessage : QS.Fx.Serialization.ISerializable
        {
            public IntraregionalMessage()
            {
            }

            public IntraregionalMessage(Base3_.RegionID regionID, Aggregation3_.IAggregatable aggregatableObject)
            {
                this.regionID = regionID;
                this.aggregatableObject = aggregatableObject;
            }

            private Base3_.RegionID regionID;
            private Aggregation3_.IAggregatable aggregatableObject;
            private RingMessageContainer ringMessageContainer = new RingMessageContainer();
            private ShipmentCollection shipmentCollection = new ShipmentCollection();

            public Base3_.RegionID RegionID
            {
                get { return regionID; }
            }

            public Aggregation3_.IAggregatable AggregatableObject
            {
                set { aggregatableObject = value; }
                get { return aggregatableObject; }
            }

            public RingMessageContainer RingMessageContainer
            {
                get { return ringMessageContainer; }
            }

            public IShipmentCollection ShipmentCollection
            {
                get { return shipmentCollection; }
            }

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    return ((QS.Fx.Serialization.ISerializable) shipmentCollection).SerializableInfo.CombineWith(regionID.SerializableInfo).CombineWith(
                        aggregatableObject.SerializableInfo).CombineWith(((QS.Fx.Serialization.ISerializable) ringMessageContainer).SerializableInfo).Extend(
                        (ushort)ClassID.Gossiping2_RegionalAgent_IntraregionalMessage, sizeof(ushort), 0, 0);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                regionID.SerializeTo(ref header, ref data);
                ushort classID = aggregatableObject.SerializableInfo.ClassID;

                if (classID == 1)
                    throw new Exception("*********************INTERNAL ERROR*********************");

                fixed (byte* arrayptr = header.Array)
                {
                    *((ushort*)(arrayptr + header.Offset)) = classID;
                }
                header.consume(sizeof(ushort));
                aggregatableObject.SerializeTo(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)ringMessageContainer).SerializeTo(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)shipmentCollection).SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                regionID = new QS._qss_c_.Base3_.RegionID();
                regionID.DeserializeFrom(ref header, ref data);
                ushort classID;
                fixed (byte* arrayptr = header.Array)
                {
                    classID = *((ushort*)(arrayptr + header.Offset));
                }
                header.consume(sizeof(ushort));
                aggregatableObject = (Aggregation3_.IAggregatable) QS._core_c_.Base3.Serializer.CreateObject(classID);
                aggregatableObject.DeserializeFrom(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)ringMessageContainer).DeserializeFrom(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)shipmentCollection).DeserializeFrom(ref header, ref data);
            }

            #endregion

            public override string ToString()
            {
                return "RegionID: " + regionID.ToString() + "; AggregatableObject: " + aggregatableObject.ToString() + "; RingMessages: " +
                    ringMessageContainer.ToString() + "; ShipmentCollection: " + shipmentCollection.ToString();
            }
        }

        #endregion

        #region Class InterregionalMessage

        [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_RegionalAgent_InterregionalMessage)]
        private class InterregionalMessage : QS.Fx.Serialization.ISerializable
        {
            public InterregionalMessage()
            {
            }

            public InterregionalMessage(Base3_.RegionID sourceRID, Base3_.RegionID destinationRID, Aggregation3_.IAggregatable aggregatableObject)
            {
                this.sourceRID = sourceRID;
                this.destinationRID = destinationRID;
                this.aggregatableObject = aggregatableObject;
            }

            private Base3_.RegionID sourceRID, destinationRID;
            private Aggregation3_.IAggregatable aggregatableObject;
            private ShipmentCollection shipmentCollection = new ShipmentCollection();

            public Base3_.RegionID SourceRegionID
            {
                get { return sourceRID; }
            }

            public Base3_.RegionID DestinationRegionID
            {
                get { return destinationRID; }
            }

            public Aggregation3_.IAggregatable AggregatableObject
            {
                set { aggregatableObject = value; }
                get { return aggregatableObject; }
            }

            public IShipmentCollection ShipmentCollection
            {
                get { return shipmentCollection; }
            }

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    return ((QS.Fx.Serialization.ISerializable)shipmentCollection).SerializableInfo.CombineWith(sourceRID.SerializableInfo).CombineWith(
                        destinationRID.SerializableInfo).CombineWith(aggregatableObject.SerializableInfo).Extend(
                        (ushort)ClassID.Gossiping2_RegionalAgent_InterregionalMessage, sizeof(ushort), 0, 0);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                sourceRID.SerializeTo(ref header, ref data);
                destinationRID.SerializeTo(ref header, ref data);
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    *((ushort*)(headerptr)) = aggregatableObject.SerializableInfo.ClassID;
                }
                header.consume(sizeof(ushort));
                aggregatableObject.SerializeTo(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)shipmentCollection).SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                sourceRID = new QS._qss_c_.Base3_.RegionID();
                sourceRID.DeserializeFrom(ref header, ref data);
                destinationRID = new QS._qss_c_.Base3_.RegionID();
                destinationRID.DeserializeFrom(ref header, ref data);
                ushort classID;
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    classID = *((ushort*)(headerptr));
                }
                header.consume(sizeof(ushort));
                aggregatableObject = (Aggregation3_.IAggregatable)QS._core_c_.Base3.Serializer.CreateObject(classID);
                aggregatableObject.DeserializeFrom(ref header, ref data);
                ((QS.Fx.Serialization.ISerializable)shipmentCollection).DeserializeFrom(ref header, ref data);
            }

            #endregion

            public override string ToString()
            {
                return "SourceRID: " + sourceRID.ToString() + "; DestinationRID: " + destinationRID.ToString() +
                    "; AggregatableObject: " + aggregatableObject.ToString() + "; ShipmentCollection: " + shipmentCollection.ToString();
            }
        }

        #endregion

        #region Class Agent

        private class Agent : System.IDisposable
        {
            public Agent(RegionalAgent owner, Membership2.ClientState.IRegionView regionView, 
                IEnumerable<Membership2.ClientState.IRegion> neighboringRegions)
            {
#if DEBUG_RegionalAgent
                owner.logger.Log(this, "__Agent.Create : " + regionView.Region.ID.ToString() + ":" + regionView.SeqNo.ToString());
#endif

                this.owner = owner;
                this.regionView = regionView;

                SetGossipingInterval(owner.gossipingInterval);

                UpdateAddresses();
            }

            private RegionalAgent owner;
            private Membership2.ClientState.IRegionView regionView;
            private QS.Fx.Clock.IAlarm gossipingAlarm;
            private bool leader;
            private QS._core_c_.Base3.InstanceID leaderAddress, successorAddress;
            private QS._qss_c_.Base3_.ISerializableSender successorSender;
            private double gossipingInterval;

//          private IDictionary<Base3.RegionID, IList<Aggregation3.IAggregatable>> aggregatedQueue =
//              new Dictionary<Base3.RegionID, IList<Aggregation3.IAggregatable>>();
            private IDictionary<QS._core_c_.Base3.InstanceID, IList<QS.Fx.Serialization.ISerializable>> forwardingQueue =
                new Dictionary<QS._core_c_.Base3.InstanceID, IList<QS.Fx.Serialization.ISerializable>>();

            public void SetGossipingInterval(double gossipingInterval)
            {
                this.gossipingInterval = gossipingInterval;
            }

            #region AggregatedValue

            private void AggregatedValue(Aggregation3_.IAggregatable aggregatedObject)
            {
#if DEBUG_RegionalAgent
                owner.logger.Log(this, "__ReceiveCallback: Aggregated { " + aggregatedObject.ToString() + " }");
#endif

                foreach (Membership2.Controllers.IRegionController regionController in owner.membershipController.NeighboringRegions)
                {
                    Membership2.ClientState.IRegionView regionVC = regionController.CurrentView;
                    ICollection<QS._core_c_.Base3.InstanceID> addressesCollection = regionVC.Members;
                    QS._core_c_.Base3.InstanceID[] addresses = new QS._core_c_.Base3.InstanceID[addressesCollection.Count];
                    addressesCollection.CopyTo(addresses, 0);
                    QS._core_c_.Base3.InstanceID randomAddress = addresses[owner.myrandom.Next(addresses.Length)];

                    InterregionalMessage message = new InterregionalMessage(regionView.Region.ID, regionController.ID, aggregatedObject);

                    if (!regionController.ID.Equals(this.regionView.Region.ID))
                    {
                        owner.RegionalForward(regionVC, message.ShipmentCollection);
                    }

#if DEBUG_RegionalAgent
                    owner.logger.Log(this, "__ReceiveCallback(" + regionView.Region.ToString() + ":" + regionView.SeqNo.ToString() +
                        ") : Sending " + message.ToString() + " to " + randomAddress.ToString());
#endif

#if DEBUG_Shipments
                    owner.logger.Log(this, "__AggregatedValue(" + regionView.Region.ID.ToString() + ":" +
                        regionView.SeqNo.ToString() + "): Shipping to another region (" + regionController.ID.ToString() + ") messages " + 
                        message.ShipmentCollection.ToString());
#endif

                    ((QS._qss_c_.Base3_.ISerializableSender)owner.senderCollection[randomAddress]).send(
                        (uint) ReservedObjectID.Gossiping2_RegionalAgent_InterregionalChannel, message);
                }

                owner.CleanupRegional(regionView.Region.ID); // get rid of messages that do not belong anywhere

                // owner.setAggregatableCallback(aggregatedObject);
            }

            #endregion

            #region Receive Callbacks

            public void ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID, IntraregionalMessage message)
            {
#if DEBUG_RegionalAgent
                owner.logger.Log(this, "__ReceiveCallback(" + regionView.Region.ID.ToString() + ":" + regionView.SeqNo.ToString() + 
                    ") : from " + sourceIID.ToString() + " we got { " + message.ToString() + " }");
#endif

                lock (this)
                {
#if STATISTICS_CollectTokenStatistics
                    lock (owner.collectedTokenSizes)
                    {
                        owner.collectedTokenSizes.addSample(owner.clock.Time, ((QS.Fx.Serialization.ISerializable) message).SerializableInfo.Size);
                    }
#endif

                    foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IList<QS.Fx.Serialization.ISerializable>> element in message.RingMessageContainer.MessageCollections)
                    {
                        if (successorAddress != null && !successorAddress.Equals(element.Key))
                            foreach (RegionAggregated regionAggregated in element.Value)
                                ForwardingAdd(element.Key, regionAggregated);

                        foreach (RegionAggregated regionAggregated in element.Value)
                            foreach (Aggregation3_.IAggregatable aggregatedObject in regionAggregated.AggregatedObjects)
                                owner.setAggregatableCallback(regionAggregated.RegionID, aggregatedObject);
                    }

#if DEBUG_Shipments
                    owner.logger.Log(this, "__ReceiveCallback(" + regionView.Region.ID.ToString() + ":" +
                        regionView.SeqNo.ToString() + "): Unloading " + message.ShipmentCollection.ToString());
#endif

                    owner.Unload(message.ShipmentCollection);

                    if (leader)
                    {
                        AggregatedValue(message.AggregatableObject);
                    }
                    else
                    {
                        message.AggregatableObject.aggregateWith(owner.getAggregatableCallback());

                        message.RingMessageContainer.MessageCollections.Clear();
                        ForwardNow(message);

                        message.ShipmentCollection.Clear();
                        owner.Ship(message.ShipmentCollection);

#if DEBUG_Shipments
                        owner.logger.Log(this, "__ReceiveCallback(" + regionView.Region.ID.ToString() + ":" +
                            regionView.SeqNo.ToString() + "): Shipping in an old token " + message.ShipmentCollection.ToString());
#endif

#if DEBUG_RegionalAgent
                        owner.logger.Log(this, "__ReceiveCallback(" + regionView.Region.ID.ToString() + ":" + regionView.SeqNo.ToString() +
                            ") : Sending " + message.ToString() + " to " + successorAddress.ToString());
#endif

                        successorSender.send((uint)ReservedObjectID.Gossiping2_RegionalAgent, message);
                    }
                }
            }

            public void InterregionalReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID, InterregionalMessage message)
            {
#if DEBUG_RegionalAgent
                owner.logger.Log(this, "__InterregionalReceiveCallback(" + regionView.Region.ID.ToString() + ":" + regionView.SeqNo.ToString() + 
                    ") : from " + sourceIID.ToString() + " we got { " + message.ToString() + " }");
#endif

                lock (this)
                {
#if DEBUG_Shipments
                    owner.logger.Log(this, "__InterregionalReceiveCallback(" + regionView.Region.ID.ToString() + ":" +
                        regionView.SeqNo.ToString() + "): Unloading " + message.ShipmentCollection.ToString());
#endif

                    owner.Unload(message.ShipmentCollection);

                    ForwardingAdd(owner.localIID, new RegionAggregated(message.SourceRegionID, message.AggregatableObject));
                    owner.setAggregatableCallback(message.SourceRegionID, message.AggregatableObject);
                }
            }

            #endregion

            #region ForwardingAdd

            private void ForwardingAdd(QS._core_c_.Base3.InstanceID originAddress, QS.Fx.Serialization.ISerializable message)
            {
                IList<QS.Fx.Serialization.ISerializable> list;
                if (forwardingQueue.ContainsKey(originAddress))
                    list = forwardingQueue[originAddress];
                else
                {
                    list = new List<QS.Fx.Serialization.ISerializable>();
                    forwardingQueue.Add(originAddress, list);
                }

                list.Add(message);
            }

            private void ForwardingAdd(QS._core_c_.Base3.InstanceID originAddress, IEnumerable<QS.Fx.Serialization.ISerializable> messages)
            {
                IList<QS.Fx.Serialization.ISerializable> list;
                if (forwardingQueue.ContainsKey(originAddress))
                    list = forwardingQueue[originAddress];
                else
                {
                    list = new List<QS.Fx.Serialization.ISerializable>();
                    forwardingQueue.Add(originAddress, list);
                }

                foreach (QS.Fx.Serialization.ISerializable message in messages)
                    list.Add(message);
            }

            #endregion

            #region Alarm Callbacks

            private void GossipingCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (this)
                {
#if DEBUG_RegionalAgent
                    owner.logger.Log(this, "__GossipingCallback(" + regionView.Region.ID.ToString() + ":" + regionView.SeqNo.ToString() + ")");
#endif

                    if (successorSender != null)
                    {
#if STATISTICS_CollectTokenStatistics
                        lock (owner.tokenCreationTimes)
                        {
                            owner.tokenCreationTimes.addSample(owner.clock.Time);
                        }
#endif

                        IntraregionalMessage message = new IntraregionalMessage(regionView.Region.ID, owner.getAggregatableCallback());

                        ForwardNow(message);
                        owner.Ship(message.ShipmentCollection);

#if DEBUG_Shipments
                        owner.logger.Log(this, "__GossipingCallback(" + regionView.Region.ID.ToString() + ":" +
                            regionView.SeqNo.ToString() + "): Shipping in a new token " + message.ShipmentCollection.ToString());
#endif

#if DEBUG_RegionalAgent
                        owner.logger.Log(this, "__GossipingCallback(" + regionView.Region.ID.ToString() + ":" + regionView.SeqNo.ToString() + 
                            ") : Sending " + message.ToString() + " to " + successorAddress.ToString());
#endif

                        successorSender.send((uint)ReservedObjectID.Gossiping2_RegionalAgent, message);
                    }
                    else
                        AggregatedValue(owner.getAggregatableCallback());

                    if (leader)
                        alarmRef.Reschedule(this.gossipingInterval);
                }
            }

            #endregion

            #region ForwardNow

            private void ForwardNow(IntraregionalMessage message)
            {
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IList<QS.Fx.Serialization.ISerializable>> element in forwardingQueue)
                {
                    if (regionView.Members.Contains(element.Key))
                        message.RingMessageContainer.MessageCollections.Add(element.Key, element.Value);
                }

                forwardingQueue.Clear();
            }

            #endregion

            #region Region Change Callbacks

            public void NeighborsChanged(
                IEnumerable<Membership2.ClientState.IRegion> toAdd, IEnumerable<Membership2.ClientState.IRegion> toRemove)
            {
            }

            private void ChangeView(Membership2.ClientState.IRegionView updatedView)
            {
                this.regionView = updatedView;
                UpdateAddresses();
            }

            #endregion

            #region Internal Processing, Helpers etc.

            private void UpdateAddresses()
            {
#if DEBUG_UpdateAddresses
                StringBuilder s = new StringBuilder("__UpdateAddresses : Members { ");
                foreach (QS._core_c_.Base3.InstanceID memberAddress in regionView.Members)
                    s.Append(memberAddress.ToString() + " ");
                s.Append("}");
                owner.logger.Log(this, s.ToString());
#endif

                leaderAddress = owner.localIID;
                leader = true;
                successorAddress = null;
                foreach (QS._core_c_.Base3.InstanceID memberAddress in regionView.Members)
                {
                    if (((IComparable<QS._core_c_.Base3.InstanceID>) memberAddress).CompareTo(leaderAddress) < 0)
                    {
                        leaderAddress = memberAddress;
                        leader = false;
                    }

                    if (((IComparable<QS._core_c_.Base3.InstanceID>) memberAddress).CompareTo(owner.localIID) > 0 && 
                        ((successorAddress == null) || (((IComparable<QS._core_c_.Base3.InstanceID>) memberAddress).CompareTo(successorAddress) < 0)))
                        successorAddress = memberAddress;
                }

                if (successorAddress == null)
                    successorAddress = leaderAddress;

#if DEBUG_UpdateAddresses
                owner.logger.Log(this, 
                    "__UpdateAddresses : IsLeader = " + leader.ToString() + ", Successor = " + successorAddress.ToString());
#endif

                successorSender = successorAddress.Equals(owner.localIID) ? null : owner.senderCollection[successorAddress];

                if (leader)
                {
                    if (gossipingAlarm == null)
                        gossipingAlarm = owner.alarmClock.Schedule(this.gossipingInterval,
                            new QS.Fx.Clock.AlarmCallback(this.GossipingCallback), null);
                }
                else
                {
                    if (gossipingAlarm != null)
                    {
                        gossipingAlarm.Cancel();
                        gossipingAlarm = null;
                    }
                }
            }

            #endregion

            #region Accessors

            public Membership2.ClientState.IRegionView RegionView
            {
                get { return regionView; }
                set 
                {
                    if (!value.Region.ID.Equals(regionView.Region.ID))
                        throw new ArgumentException("Cannot update to a region view in a different region, need to create a new agent.");
                    ChangeView(value);
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                lock (this)
                {
                    if (gossipingAlarm != null)
                    {
                        gossipingAlarm.Cancel();
                        gossipingAlarm = null;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region IRegionalAgent Members

        GetAggregatableCallback IRegionalAgent.GetAggregatableCallback
        {
            set
            {
                lock (this)
                {
                    if (getAggregatableCallback != null)
                        throw new Exception("GetAggregatable callback already set!");

#if DEBUG_RegionalAgent
                    logger.Log(this, "Setting GetAggregatableCallback.");
#endif
                    getAggregatableCallback = value;
                }
            }
        }

        SetAggregatableCallback IRegionalAgent.SetAggregatableCallback
        {
            set
            {
                lock (this)
                {
                    if (setAggregatableCallback != null)
                        throw new Exception("SetAggregatable callback already set!");

#if DEBUG_RegionalAgent
                    logger.Log(this, "Setting SetAggregatableCallback.");
#endif
                    setAggregatableCallback = value;
                }
            }
        }

        #endregion

        #region Class InstanceSenderWrapper

        private class InstanceSenderWrapper : QS._qss_c_.Base3_.ISerializableSender
        {
            public InstanceSenderWrapper(RegionalAgent owner, QS._core_c_.Base3.InstanceID destinationAddress)
            {
                this.owner = owner;
                this.destinationAddress = destinationAddress;
            }

            private RegionalAgent owner;
            private QS._core_c_.Base3.InstanceID destinationAddress;

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return destinationAddress.Address; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                owner.Send(destinationAddress, destinationLOID, data);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region ISenderCollection<InstanceID,ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get { return new InstanceSenderWrapper(this, destinationAddress); }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { throw new NotImplementedException(); }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { throw new NotImplementedException(); }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ISink<ScatteringSource<InstanceID>> Members

        QS._qss_c_.Senders8.IChannel QS._qss_c_.Senders8.ISink<QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID>>.Register(
            QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID> source)
        {
            return new Channel(this, source);
        }

        private class Channel : QS._qss_c_.Senders8.IChannel
        {
            public Channel(RegionalAgent owner, QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID> source)
            {
                this.owner = owner;
                this.source = source;
            }

            private RegionalAgent owner;
            private QS._qss_c_.Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID> source;

            #region IChannel Members

            void QS._qss_c_.Senders8.IChannel.Signal()
            {
                lock (owner.sources)
                {
#if DEBUG_RegionalAgent
                    owner.logger.Log(this, "__IChannel.Signal : " + source.ToString());
#endif
                    owner.sources.Enqueue(source);
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region IStatisticsCollector Members

        IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
        {
            get 
            { 
                List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();
#if STATISTICS_CollectTokenStatistics
                statistics.Add(new Components.Attribute("Token Creation Times", tokenCreationTimes.DataSet));
                statistics.Add(new Components.Attribute("Collected Token Sizes", collectedTokenSizes.DataSet));
#endif
                return statistics;
            }
        }

        #endregion
    }
}
