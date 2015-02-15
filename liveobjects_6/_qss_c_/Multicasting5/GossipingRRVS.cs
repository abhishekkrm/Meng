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

// #define DEBUG_GossipingRRVS
// #define DEBUG_LogAcks
#define Optimize_Sending_To_Yourself

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Base.Inspectable]
    public class GossipingRRVS : QS.Fx.Inspection.Inspectable, Base3_.ISenderCollection<Base3_.RVID, Base3_.IReliableSerializableSender> 
        //, Senders8.IMessageSource<QS._core_c_.Base3.InstanceID>
    {
        public const double DefaultInitialRetransmissionTimeout = 3; // in seconds

        #region Constructor

        public GossipingRRVS(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, Base3_.ISenderCollection<Base3_.RegionID,
            Base3_.IReliableSerializableSender> regionSenders, Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock,
            Senders8.ISink<Senders8.IScatteringSource<QS._core_c_.Base3.InstanceID>> messageSink,
            Membership2.Controllers.IMembershipController membershipController, QS.Fx.Clock.IClock clock)
        {
            this.localIID = localIID;
            this.logger = logger;
            this.regionSenders = regionSenders;
            this.demultiplexer = demultiplexer;
            this.membershipController = membershipController;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.initialRetransmissionTimeout = DefaultInitialRetransmissionTimeout;

            scatteringSource = new QS._qss_c_.Senders8.ScatteringSource<QS._core_c_.Base3.InstanceID>(messageSink);

            demultiplexer.register((uint)ReservedObjectID.Multicasting5_GossipingRRVS, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
            demultiplexer.register((uint)ReservedObjectID.Multicasting5_GossipingRRVS_AckChannel, 
                new QS._qss_c_.Base3_.ReceiveCallback(this.AckCallback));

            senderCollectionInspectableProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, Sender>(
                "Sender Collection", senders,
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, Sender>.ConversionCallback(
                Base3_.RVID.FromString));

            receiverCollectionInspectableProxy = new ReceiverCollectionInspectableProxy(this);
        }

        #endregion

        private QS._core_c_.Base3.InstanceID localIID;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.ISenderCollection<Base3_.RegionID, Base3_.IReliableSerializableSender> regionSenders;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private Base3_.IDemultiplexer demultiplexer;
        private IDictionary<Base3_.RVID, Sender> senders = new Dictionary<Base3_.RVID, Sender>();
        private IDictionary<QS._core_c_.Base3.InstanceID, IDictionary<Base3_.RVID, Receiver>> receivers = 
            new Dictionary<QS._core_c_.Base3.InstanceID, IDictionary<Base3_.RVID, Receiver>>();        
        private Senders8.ScatteringSource<QS._core_c_.Base3.InstanceID> scatteringSource;
        private Membership2.Controllers.IMembershipController membershipController;
        private double initialRetransmissionTimeout;

        [QS.Fx.Base.Inspectable("Sender Collection", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, Sender> senderCollectionInspectableProxy;

        [QS.Fx.Base.Inspectable("Receiver Collection", QS.Fx.Base.AttributeAccess.ReadOnly)]
        private ReceiverCollectionInspectableProxy receiverCollectionInspectableProxy;

#if DEBUG_LogAcks
        [TMS.Inspection.Inspectable("Acknowledgement Log")]
        private Base.Logger acknowledgementLog = new QS.CMS.Base.Logger(true);
#endif

        #region Class ReceiverCollection

        private class ReceiverCollectionInspectableProxy : QS.Fx.Inspection.IAttributeCollection
        {
            public ReceiverCollectionInspectableProxy(GossipingRRVS owner)
            {
                this.owner = owner;
            }

            private GossipingRRVS owner;

            #region IAttributeCollection Members

            IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
            {
                get 
                {
                    foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IDictionary<Base3_.RVID, Receiver>> element in owner.receivers)
                        foreach (Base3_.RVID rvid in element.Value.Keys)
                            yield return ((QS.Fx.Serialization.IStringSerializable) element.Key).AsString + "," + rvid.ToString();
                }
            }

            QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
            {
                get 
                {
                    try
                    {
                        int separator = attributeName.LastIndexOf(',');
                        QS._core_c_.Base3.InstanceID instanceID = new QS._core_c_.Base3.InstanceID();
                        ((QS.Fx.Serialization.IStringSerializable) instanceID).AsString = attributeName.Substring(0, separator);
                        string[] elements = attributeName.Substring(separator + 1).Split(':');
                        Base3_.RegionID regionID = new QS._qss_c_.Base3_.RegionID();
                        regionID.AsString = elements[0];
                        uint regionViewSeqNo = Convert.ToUInt32(elements[1]);
                        Base3_.RVID rvid = new QS._qss_c_.Base3_.RVID(regionID, regionViewSeqNo);

                        return new QS.Fx.Inspection.ScalarAttribute(attributeName, owner.receivers[instanceID][rvid]);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot find attribute \"" + attributeName + "\".", exc);
                    }
                }
            }

            #endregion

            #region IAttribute Members

            string QS.Fx.Inspection.IAttribute.Name
            {
                get { return "Receiver Collection"; }
            }

            QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
            {
                get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
            }

            #endregion
        }

        #endregion

        #region Accessors

        public double InitialRetransmissionTimeout
        {
            get { return initialRetransmissionTimeout; }
            set { initialRetransmissionTimeout = value; }
        }

        #endregion

        #region Class Receiver

        [QS.Fx.Base.Inspectable]
        private class Receiver : QS.Fx.Inspection.Inspectable, Senders8.ISource<QS._core_c_.Base3.InstanceID>
        {
            public Receiver(GossipingRRVS owner, Base3_.RVID regionViewID, QS._core_c_.Base3.InstanceID instanceID)
            {
                this.owner = owner;
                this.regionViewID = regionViewID;
                this.instanceID = instanceID;

                channel = ((Senders8.ISink<Senders8.ISource<QS._core_c_.Base3.InstanceID>>)owner.scatteringSource).Register(this);
            }

            private Base3_.RVID regionViewID;
            private QS._core_c_.Base3.InstanceID instanceID;
            private GossipingRRVS owner;
            private IAckCollection ackCollection = new AckCollection();
            private bool ready;
            private Senders8.IChannel channel;

            #region ReceiveCallback

            public void ReceiveCallback(uint messageSeqNo, QS._core_c_.Base3.Message message)
            {
#if DEBUG_GossipingRRVS
                owner.logger.Log(this, "__ReceiveCallback(" + regionViewID.ToString() + ", " + instanceID.ToString() + 
                    ") received (" + messageSeqNo.ToString() + ") : " + message.ToString());
#endif

                bool signal_now;
                lock (this)
                {
                    if (ackCollection.Add(messageSeqNo))
                        owner.demultiplexer.dispatch(message.destinationLOID, instanceID, message.transmittedObject);

                    if (signal_now = !ready)
                        ready = true;
                }

                if (signal_now)
                    channel.Signal();
            }

            #endregion

            #region ISource Members

            bool Senders8.ISource.Ready
            {
                get { return ready; }
            }

            QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> Senders8.ISource.Get(uint maximumSize)
            {
                lock (this)
                {
                    QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> returned_result;
                    if (ready)
                    {
                        ready = false;
                        returned_result = new QS._qss_c_.Base3_.AsynchronousRequest1<QS._core_c_.Base3.Message>(
                            new QS._core_c_.Base3.Message((uint) ReservedObjectID.Multicasting5_GossipingRRVS_AckChannel, 
                            new AcknowledgementRV(regionViewID, (CompressedAckSet) ackCollection.AsCompressed)));
                    }
                    else
                        returned_result = null;

#if DEBUG_GossipingRRVS
                    owner.logger.Log(this, "__ISource.Get(" + regionViewID.ToString() + ", " + instanceID.ToString() +
                        ") returns (" + ((returned_result != null) ? returned_result.ToString()  : "null") + ")");
#endif

                    return returned_result;
                }
            }

            QS._core_c_.Base3.InstanceID Senders8.ISource<QS._core_c_.Base3.InstanceID>.Address
            {
                get { return instanceID; }
            }

            #endregion

            public override string ToString()
            {
                return "GossipingRRVS.Receiver(" + regionViewID.ToString() + ", " + instanceID.ToString() + ")";
            }
        }

        #endregion

        #region AckCallback

        private QS.Fx.Serialization.ISerializable AckCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            AcknowledgementRV ack = receivedObject as AcknowledgementRV;
            if (ack == null)
                throw new Exception("Wrong object type received.");

#if DEBUG_GossipingRRVS
            logger.Log(this, "__AckCallback : from " + sourceIID.ToString() + " received " + receivedObject.ToString());  
#endif

#if DEBUG_LogAcks
            acknowledgementLog.logMessage(null, sourceIID.ToString() + " : " + receivedObject.ToString());
#endif

            Sender sender;
            lock (this)
            {
                if (senders.ContainsKey(ack.RVID))
                    sender = (Sender) senders[ack.RVID];
                else
                    sender = null;
            }

            if (sender != null)
                sender.AcknowledgementCallback(sourceIID, ack.AckCollection);
            else
                throw new Exception("Sender for region view " + ack.RVID.ToString() + " does not exist any more.");

            return null;
        }

        #endregion

        #region ReceiveCallback

        private Receiver GetReceiver(QS._core_c_.Base3.InstanceID sourceIID, Base3_.RVID regionViewID)
        {
            Receiver receiver;
            lock (this)
            {
                IDictionary<Base3_.RVID, Receiver> rv2receiver;
                if (receivers.ContainsKey(sourceIID))
                    rv2receiver = receivers[sourceIID];
                else
                    receivers[sourceIID] = rv2receiver = new Dictionary<Base3_.RVID, Receiver>();

                if (rv2receiver.ContainsKey(regionViewID))
                    receiver = rv2receiver[regionViewID];
                else
                    rv2receiver[regionViewID] = receiver = new Receiver(this, regionViewID, sourceIID);
            }
            return receiver;
        }

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            MessageRV message = receivedObject as MessageRV;
            if (message == null)
                throw new Exception("Wrong message type.");

#if DEBUG_GossipingRRVS
            logger.Log(this, "__ReceiveCallback : from " + sourceIID.ToString() + " received " + receivedObject.ToString());
#endif

            GetReceiver(sourceIID, message.RVID).ReceiveCallback(message.SeqNo, message.EncapsulatedMessage);

            return null;
        }

        #endregion    

        #region Class Sender

        [QS.Fx.Base.Inspectable]
        private class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender
        {
            public Sender(GossipingRRVS owner, Base3_.RVID address)
            {
                this.owner = owner;
                this.address = address;
                this.regionSender = owner.regionSenders[address.RegionID];

                this.regionVC = (Membership2.Controllers.IRegionViewController) 
                    owner.membershipController.lookupRegion(address.RegionID)[address.SeqNo];

                outgoingContainer = new FlowControl5.OutgoingContainer<Request>(owner.clock);
            }

            private GossipingRRVS owner;
            private Base3_.RVID address;
            private Membership2.Controllers.IRegionViewController regionVC;
            private Base3_.IReliableSerializableSender regionSender;
            private FlowControl5.IOutgoingContainer<Request> outgoingContainer;

            #region Retransmissions

            private void TransmissionCallback(Base3_.IAsynchronousOperation asynchronousOperation)
            {
                Request request = (Request) asynchronousOperation.AsyncState;
                lock (request)
                {
                    if (!request.IsCompleted)
                        request.RetransmissionAlarm = owner.alarmClock.Schedule(owner.initialRetransmissionTimeout,
                            new QS.Fx.Clock.AlarmCallback(this.RetransmissionCallback), request);
                }
            }

            private void RetransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                Request request = (Request)alarmRef.Context;
                lock (request)
                {
                    if (!request.IsCompleted)
                    {
                        request.RetransmissionAlarm = null;
                        regionSender.BeginSend((uint)ReservedObjectID.Multicasting5_GossipingRRVS, request,
                            new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.TransmissionCallback), request);
                    }
                }
            }

            #endregion

            #region Processing Completion

            private void RemoveComplete(Request request)
            {
                outgoingContainer.Remove(request.SeqNo);
            }

            #endregion

            #region Acknowledgement Callback

            public void AcknowledgementCallback(QS._core_c_.Base3.InstanceID sourceIID, CompressedAckSet ackCollection)
            {                
#if DEBUG_GossipingRRVS
                owner.logger.Log(this, "__AcknowledgementCallback(" + address.ToString() + ") : instance " + sourceIID.ToString() +
                    " acknowledged " + ackCollection.ToString());
#endif

                lock (this)
                {
                    ackCollection.CutOffSmaller(outgoingContainer.FirstOccupiedSeqNo);
                    
                    foreach (uint seqno in ackCollection)
                    {
                        Request request = outgoingContainer[seqno];
                        if (request != null)
                            request.Acknowledged(sourceIID);
                    }
                }
            }

            #endregion

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
                QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                Request request = new Request(this, address, new QS._core_c_.Base3.Message(destinationLOID, data), completionCallback, asynchronousState);

                lock (this)
                {
                    request.SeqNo = outgoingContainer.Add(request);
                } 

#if DEBUG_GossipingRRVS
                owner.logger.Log(this, "__BeginSend(" + address.ToString() + ") sending out " + request.ToString());
#endif

#if Optimize_Sending_To_Yourself
                owner.GetReceiver(owner.localIID, address).ReceiveCallback(request.SeqNo, request.Message);
                request.Acknowledged(owner.localIID);
#endif

                regionSender.BeginSend((uint)ReservedObjectID.Multicasting5_GossipingRRVS, request, 
                    new QS._qss_c_.Base3_.AsynchronousOperationCallback(this.TransmissionCallback), request);

                return request;
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            #endregion

            #region Class Request

            [QS.Fx.Base.Inspectable]
            private class Request : Base3_.AsynchronousOperation, QS.Fx.Serialization.ISerializable, QS.Fx.Inspection.IInspectable
            {
                public Request(Sender owner, Base3_.RVID regionViewID, QS._core_c_.Base3.Message message, 
                    Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) : base(completionCallback, asynchronousState)
                {
                    this.regionViewID = regionViewID;
                    this.owner = owner;
                    this.message = message;

                    // TODO: Change for something more efficient!
                    pendingAddresses = new System.Collections.Generic.List<QS._core_c_.Base3.InstanceID>(owner.regionVC.Members);
                }

                private Sender owner;
                private Base3_.RVID regionViewID;
                private QS._core_c_.Base3.Message message;
                private uint seqno;
                private QS.Fx.Clock.IAlarm retransmissionAlarm;

                private System.Collections.Generic.List<QS._core_c_.Base3.InstanceID> pendingAddresses;

#if DEBUG_LogAcks
                private Base.Logger acknowledgementLog = new QS.CMS.Base.Logger(true);
#endif

                #region Processing Acknowledgements and Crashes

                // TODO: Add a proper crash processing!

                private void RemoveAddress(QS._core_c_.Base3.InstanceID address)
                {
                    bool completed_now;
                    lock (this)
                    {
                        completed_now = pendingAddresses.Remove(address) && pendingAddresses.Count == 0;
                    }

                    if (completed_now)
                        this.IsCompleted = true;
                }

                public void Acknowledged(QS._core_c_.Base3.InstanceID destinationIID)
                {
#if DEBUG_GossipingRRVS
                    owner.owner.logger.Log(this, "__Acknowledged[" + owner.address.ToString() + ":" + seqno.ToString() + "] " +
                        destinationIID.ToString());
#endif

#if DEBUG_LogAcks
                    acknowledgementLog.logMessage(null, 
                        owner.address.ToString() + ":" + seqno.ToString() + " ack " + destinationIID.ToString());
#endif

                    RemoveAddress(destinationIID);
                }

                #endregion

                #region Accessors

                public QS.Fx.Clock.IAlarm RetransmissionAlarm
                {
                    get { return retransmissionAlarm; }
                    set { retransmissionAlarm = value; }
                }

                public QS._core_c_.Base3.Message Message
                {
                    get { return message; }
                }

                public uint SeqNo
                {
                    get { return seqno; }
                    set { seqno = value; }
                }

                #endregion

                #region Processing Completion

                public override void Unregister()
                {
#if DEBUG_GossipingRRVS
                    owner.owner.logger.Log(this, "__Unregister[" + owner.address.ToString() + ":" + seqno.ToString() + "]");
#endif
                    lock (this)
                    {
                        if (retransmissionAlarm != null)
                            retransmissionAlarm.Cancel();
                        retransmissionAlarm = null;
                    }

                    owner.RemoveComplete(this);
                }

                #endregion

                #region ISerializable Members

                QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
                {
                    get
                    {
                        return regionViewID.SerializableInfo.CombineWith(message.SerializableInfo).Extend(
                            (ushort)ClassID.Multicasting5_MessageRV, (ushort)(sizeof(uint)), 0, 0);
                    }
                }

                unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
                    ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
                {
                    regionViewID.SerializeTo(ref header, ref data);
                    fixed (byte* arrayptr = header.Array)
                    {
                        byte* headerptr = arrayptr + header.Offset;
                        *((uint*)headerptr) = seqno;
                    }
                    header.consume(sizeof(uint));
                    message.SerializeTo(ref header, ref data);                    
                }

                void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
                {
                    throw new NotSupportedException();
                }

                #endregion

                public override string ToString()
                {
                    return "(" + regionViewID.ToString() + ":" + seqno.ToString() + " " + message.ToString() + ")";
                }

                #region IInspectable Members

                private QS.Fx.Inspection.IAttributeCollection attributeCollection;
                QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
                {
                    get 
                    {
                        lock (this)
                        {
                            if (attributeCollection == null)
                                attributeCollection = new QS.Fx.Inspection.AttributesOf(this);
                            return attributeCollection;
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { throw new NotSupportedException(); }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        #region GetSender

        private Sender GetSender(Base3_.RVID address)
        {
            lock (this)
            {
                Sender sender;
                if (senders.ContainsKey(address))
                    sender = senders[address];
                else
                    senders[address] = sender = new Sender(this, address);
                return sender;
            }
        }

        #endregion

        #region ISenderCollection<RVID,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.RVID, QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS._qss_c_.Base3_.RVID destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion
    }
}
