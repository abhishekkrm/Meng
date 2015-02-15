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

// #define DEBUG_RingRRVS

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_c_.Rings4
{
    [QS.Fx.Base.Inspectable]
    public class RingRRVS : QS.Fx.Inspection.Inspectable, 
        Base3_.ISenderCollection<Base3_.RVID, Base3_.IReliableSerializableSender>,
        Base4_.ISinkCollection<Base3_.RVID, Base4_.Asynchronous<QS._core_c_.Base3.Message>>
    {
        #region Constructor

        public RingRRVS(
            QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localAddress, Base3_.IDemultiplexer demultiplexer, 
            QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Membership2.Controllers.IMembershipController membershipController,
            Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> reliableSenderCollection,
            double retransmissionTimeout, int maximumNAKsPerToken)
        {
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.alarmClock = alarmClock;
            this.membershipController = membershipController;
            this.localAddress = localAddress;
            this.reliableSenderCollection = reliableSenderCollection;
            this.regionSenders = regionSenders;
            this.retransmissionTimeout = retransmissionTimeout;
            this.maximumNAKsPerToken = maximumNAKsPerToken;
            this.clock = clock;

            this.dataChannel = (uint) ReservedObjectID.Rings4_RingRRVS_DataChannel;
            this.ackChannel = (uint) ReservedObjectID.Rings4_RingRRVS_AckChannel;
            this.tokenChannel = (uint) ReservedObjectID.Rings4_RingRRVS_TokenChannel;

            demultiplexer.register(dataChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
            demultiplexer.register(ackChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.AcknowledgementCallback));
            demultiplexer.register(tokenChannel, new QS._qss_c_.Base3_.ReceiveCallback(this.ControlReceiveCallback));
            
            ((Membership2.Consumers.IRegionChangeProvider) membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.RegionChangedCallback(this.MembershipCallback);

            senderCollectionInspectableWrapper =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, ISender>("Senders", senderCollection,
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID,ISender>.ConversionCallback(
                    Base3_.RVID.FromString));

            receiverCollectionsInspectableWrapper =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, IReceiverCollection>(
                    "Receiver Collections", receiverCollections,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, IReceiverCollection>.ConversionCallback(
                        Base3_.RVID.FromString));
        }

        #endregion

        #region Adjusting Configuration

        public bool IsDisabled
        {
            get { return disabled; }
            set { disabled = value; }
        }

        public double RetransmissionTimeout
        {
            get { return retransmissionTimeout; }
            set { retransmissionTimeout = value; }
        }

        public double TokenFrequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        public int ReplicationCoefficient
        {
            get { return replication; }
            set { replication = value; }
        }

        public int MaxNAKsPerToken
        {
            get { return this.maximumNAKsPerToken; }
            set { this.maximumNAKsPerToken = value; }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("Senders")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, ISender> senderCollectionInspectableWrapper;
        [QS.Fx.Base.Inspectable("Receiver Collections")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, IReceiverCollection> receiverCollectionsInspectableWrapper;

        private QS.Fx.Logging.ILogger logger;
        private Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> reliableSenderCollection;
        private QS._core_c_.Base3.InstanceID localAddress;
        private Base3_.IDemultiplexer demultiplexer;
        private Membership2.Controllers.IMembershipController membershipController;
        private double retransmissionTimeout, frequency = 1;
        private int replication = 5;
        private int maximumNAKsPerToken;
        private bool disabled = true;

        private uint dataChannel, ackChannel, tokenChannel;

        private IDictionary<Base3_.RVID, IReceiverCollection> receiverCollections = 
            new Dictionary<Base3_.RVID, IReceiverCollection>();
        private IDictionary<Base3_.RVID, ISender> senderCollection = new Dictionary<Base3_.RVID, ISender>();

        #endregion

        #region Create Receiver Collection

        private IReceiverCollection CreateReceiverCollection(Base3_.RVID rvid)
        {
            Membership2.Controllers.IRegionViewController regionVC =
                (Membership2.Controllers.IRegionViewController) membershipController.lookupRegion(rvid.RegionID)[rvid.SeqNo];

            QS._core_c_.Base3.InstanceID[] receiverAddresses = new QS._core_c_.Base3.InstanceID[regionVC.Members.Count];
            regionVC.Members.CopyTo(receiverAddresses, 0);

            return new ReceiverCollection(
                receiverAddresses, localAddress, logger, demultiplexer, alarmClock, clock, reliableSenderCollection, 
                frequency, (uint) replication,
                new RegionContext(rvid, tokenChannel, dataChannel, ackChannel, reliableSenderCollection, logger),
                maximumNAKsPerToken);
        }

        #endregion

        #region Membership Callback

        private void MembershipCallback(QS._qss_c_.Membership2.Consumers.RegionChange change)
        {
            if (!disabled)
            {
                lock (this)
                {
                    Debug.Assert(change != null);
                    Debug.Assert(change.CurrentView != null);
                    Debug.Assert(change.CurrentView.Region != null);

                    Base3_.RVID rvid = new QS._qss_c_.Base3_.RVID(change.CurrentView.Region.ID, change.CurrentView.SeqNo);

                    if (!receiverCollections.ContainsKey(rvid))
                    {
#if DEBUG_RingRRVS
                    logger.Log(this, "__MembershipCallback : Creating " + rvid.ToString());
#endif

                        receiverCollections.Add(rvid, CreateReceiverCollection(rvid));
                    }

                    // TODO: Process recycling of receiver collections for old region views and receivers for crashed nodes.

                    foreach (KeyValuePair<Base3_.RVID, IReceiverCollection> element in receiverCollections)
                    {
                        if (!element.Key.Equals(rvid))
                        {
#if DEBUG_RingRRVS
                        logger.Log(this, "__MembershipCallback : Shutting down " + element.Key.ToString());
#endif

                            element.Value.Shutdown();
                        }
                    }
                }
            }
        }

        #endregion

        #region Control Receive Callback

        private QS.Fx.Serialization.ISerializable ControlReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            ObjectRV objectRV = receivedObject as ObjectRV;
            if (objectRV == null)
                throw new Exception("Received object of unknown type");

/*
#if DEBUG_RingRRVS
            logger.Log
#endif
*/

            IReceiverCollection receiverCollection;
            lock (this)
            {
                receiverCollection = receiverCollections.ContainsKey(objectRV.Address) ? receiverCollections[objectRV.Address] : null;
            }

            if (receiverCollection != null)
                receiverCollection.ReceiveControl(sourceAddress, objectRV.Message);
            else
                logger.Log(this, "Could not deliver message from " + sourceAddress.ToString() +
                    " addressed at a locally nonexistent region view " + objectRV.Address.ToString());

            return null;
        }

        #endregion

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Multicasting5.MessageRV message = receivedObject as Multicasting5.MessageRV;
            if (message != null)
            {
                QS._core_c_.Base3.InstanceID realSourceAddress = (message is Multicasting5.ForwardingRV) 
                    ? ((Multicasting5.ForwardingRV)message).SenderAddress : sourceAddress;

                IReceiverCollection receiverCollection;
                lock (this)
                {
                    receiverCollection = receiverCollections.ContainsKey(message.RVID) ? receiverCollections[message.RVID] : null;
                }

                if (receiverCollection != null)
                    receiverCollection.Receive(realSourceAddress, message.SeqNo, message.EncapsulatedMessage);
                else
                    logger.Log(this, "Could not deliver message from " + realSourceAddress.ToString() +
                        " addressed at a locally nonexistent region view " + message.RVID.ToString());
            }
            else
                logger.Log(this, "Unrecognizable message of type " + 
                    ((receivedObject != null) ? receivedObject.GetType().FullName : "null") + " received from " +
                    sourceAddress.ToString());

            return null;
        }

        #endregion

        #region Acknowledgement Callback

        private QS.Fx.Serialization.ISerializable AcknowledgementCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            ObjectRV ack = receivedObject as ObjectRV;
            if (ack == null)
                throw new Exception("Wrong object type received.");

            ISender sender;
            lock (this)
            {
                sender = senderCollection.ContainsKey(ack.Address) ? senderCollection[ack.Address] : null;
            }

            if (sender != null)
                sender.Acknowledged(sourceIID, ack.Message);
            else
                throw new Exception("Sender for region view " + ack.Address.ToString() + " does not exist any more.");

            return null;
        }

        #endregion

        #region CreateSender

        private ISender CreateSender(Base3_.RVID destinationAddress)
        {
            return new Sender(logger, alarmClock, clock, destinationAddress, regionSenders[destinationAddress.RegionID],
                receiverCollections[destinationAddress].LocalReceiver, dataChannel, retransmissionTimeout);
        }

        #endregion

        #region GetSender

        private ISender GetSender(Base3_.RVID destinationAddress)
        {
            lock (this)
            {
                if (senderCollection.ContainsKey(destinationAddress))
                    return senderCollection[destinationAddress];
                else
                {
                    ISender sender = this.CreateSender(destinationAddress);
                    senderCollection.Add(destinationAddress, sender);
                    return sender;
                }
            }
        }

        #endregion

        #region ISenderCollection<RVID,IReliableSerializableSender> Members

        Base3_.IReliableSerializableSender 
            Base3_.ISenderCollection<Base3_.RVID, Base3_.IReliableSerializableSender>.this[Base3_.RVID destinationAddress]
        {
            get { return this.GetSender(destinationAddress); }
        }

        #endregion

        #region ISinkCollection<RVID,Asynchronous<Message>> Members

        QS._qss_c_.Base4_.IAddressedSink<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>> QS._qss_c_.Base4_.ISinkCollection<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base4_.Asynchronous<QS._core_c_.Base3.Message>>.this[QS._qss_c_.Base3_.RVID destinationAddress]
        {
            get { return this.GetSender(destinationAddress); }
        }

        #endregion
    }
}
