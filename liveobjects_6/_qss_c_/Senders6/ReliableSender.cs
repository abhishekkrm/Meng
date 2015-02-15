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

// #define DEBUG_ReliableSender
#define STATISTICS_SendTimes
#define STATISTICS_CompletionTimes
#define STATISTICS_TimeToAcknowledge
#define STATISTICS_RetransmitTimes
#define STATISTICS_RetransmissionRates

#define STATISTICS_NonredundancyIndicator
#define STATISTICS_Received

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders6
{
    // [TMS.Inspection.Inspectable]
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class ReliableSender : QS.Fx.Inspection.IInspectable,
        Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>, Base3_.ISenderCollection<Base3_.IReliableSerializableSender>,
        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender>,
        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsContainerForSenders = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsContainerForReceivers = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(0.05);
        private const int WindowSize = 100;
        private const bool Exponential_Backoff = true;
        private const bool Adaptive_Adjustment = true;
        private const double Default_RetransmissionTimeout = 0.01;
        private const double Minimum_RetransmissionTimeout = 0.01;
        private const double Maximum_RetransmissionTimeout = 0.50;

        public ReliableSender(QS._core_c_.Base3.InstanceID localIID, QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
            QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection,
            FailureDetection_.IFailureDetector failureDetector)
            : this(localIID, logger, demultiplexer, alarmClock, clock, underlyingSenderCollection, failureDetector,
                Default_RetransmissionTimeout, Exponential_Backoff, Adaptive_Adjustment,
                Minimum_RetransmissionTimeout, Maximum_RetransmissionTimeout)
        {
        }

        public ReliableSender(QS._core_c_.Base3.InstanceID localIID, QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, 
            QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock,
            Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection, 
            FailureDetection_.IFailureDetector failureDetector, 
            double defaultRetransmissionTimeout, bool exponentialBackoff, bool adaptiveTimeoutAdjustment,
            double minimumRetransmissionTimeout, double maximumRetransmissionTimeout)
        {
            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("Senders", diagnosticsContainerForSenders);
            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("Receivers", diagnosticsContainerForReceivers);

            this.defaultRetransmissionTimeout = defaultRetransmissionTimeout;
            this.exponentialBackoff = exponentialBackoff;
            this.adaptiveTimeoutAdjustment = adaptiveTimeoutAdjustment;
            this.minimumRetransmissionTimeout = minimumRetransmissionTimeout;
            this.maximumRetransmissionTimeout = maximumRetransmissionTimeout;

            this.localIID = localIID;
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.underlyingSenderCollection = underlyingSenderCollection;

            demultiplexer.register((uint)ReservedObjectID.Senders6_ReliableSender_MessageChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.messageCallback));
            demultiplexer.register((uint)ReservedObjectID.Senders6_ReliableSender_AcknowledgementChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.acknowledgementCallback));

            if (failureDetector != null)
                failureDetector.OnChange += new QS._qss_c_.FailureDetection_.ChangeCallback(failureDetector_OnChange);

            inspectableSendersWrapper = new QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress,Sender>("Senders", senders);
            inspectableReceiversWrapper = new QS._qss_e_.Inspection_.DictionaryWrapper2<QS._core_c_.Base3.InstanceID,Receiver>("Receivers", receivers);
        }

        #region Processing Crashes

        private void failureDetector_OnChange(IEnumerable<QS._qss_c_.FailureDetection_.Change> changes)
        {
            lock (this)
            {
                foreach (QS._qss_c_.FailureDetection_.Change change in changes)
                {
                    if (change.Action == QS._qss_c_.FailureDetection_.Action.CRASHED)
                    {
                        if (senders.ContainsKey(change.InstanceID.Address))
                        {
                            senders[change.InstanceID.Address].crashingCallback(change.InstanceID.Incarnation);
                        }

                        if (receivers.ContainsKey(change.InstanceID))
                        {
                            receivers[change.InstanceID].crashingCallback();
                        }
                    }
                }
            }
        }

        #endregion

        private double defaultRetransmissionTimeout, minimumRetransmissionTimeout, maximumRetransmissionTimeout;
        private bool exponentialBackoff, adaptiveTimeoutAdjustment;
        private QS._core_c_.Base3.InstanceID localIID;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private System.Collections.Generic.IDictionary<QS.Fx.Network.NetworkAddress, Sender> senders =
            new System.Collections.Generic.Dictionary<QS.Fx.Network.NetworkAddress, Sender>();
        [QS._core_c_.Diagnostics.ComponentCollection]
        private System.Collections.Generic.IDictionary<QS._core_c_.Base3.InstanceID, Receiver> receivers =
            new System.Collections.Generic.Dictionary<QS._core_c_.Base3.InstanceID, Receiver>();
//        private System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> crashedCollection =
//            new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();

        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS.Fx.Network.NetworkAddress, Sender> inspectableSendersWrapper;
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS._core_c_.Base3.InstanceID, Receiver> inspectableReceiversWrapper;
            
        #region Receive Callbacks

        private QS.Fx.Serialization.ISerializable messageCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Message message = receivedObject as Message;
            if (message != null)
            {
#if DEBUG_ReliableSender
                logger.Log(this, "From " + sourceIID.ToString() + " we received message " + receivedObject.ToString() + ".");
#endif

                Receiver receiver;
                lock (this)
                {
                    if (receivers.ContainsKey(sourceIID))
                        receiver = receivers[sourceIID];
                    else
                    {
//                        if (!crashedCollection.Contains(sourceIID))
//                        {
                            receiver = new Receiver(this, sourceIID);
                            receivers[sourceIID] = receiver;
//                        }
//                        else
//                        {
                            // ignore, this is a message from a dead process
//                            receiver = null;
//                        }
                    }
                }

                if (receiver != null)
                    receiver.messageCallback(message);
            }
            else
                throw new Exception("Wrong message type.");

            return null;
        }

        private QS.Fx.Serialization.ISerializable acknowledgementCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Message message = receivedObject as Message;
            if (message != null)
            {
#if DEBUG_ReliableSender
                logger.Log(this, "From " + sourceIID.ToString() + " we received response " + receivedObject.ToString() + ".");
#endif

                Sender sender;
                lock (this)
                {
                    if (senders.ContainsKey(sourceIID.Address))
                        sender = senders[sourceIID.Address];
                    else
                        sender = null;
                }

                if (sender != null)
                    sender.acknowledgementCallback(sourceIID.Incarnation, message);
            }
            else
                throw new Exception("Wrong message type.");

            return null;
        }

        #endregion

        #region Class Message

        [QS.Fx.Serialization.ClassID(ClassID.Senders6_ReliableSender_Message)]
        private class Message : QS.Fx.Serialization.ISerializable
        {
            public const int HeaderOverhead = sizeof(byte) + sizeof(ushort) + 3 * sizeof(uint);

            public enum Type : byte
            {
                CONNECT,                        // -
                CONNECT_ACK,                // incarnation
                DATA,                               // incarnation, seqno, message
                DATA_ACK,                       // incarnation, seqno
                REQUEST_RECONNECT    // -
            }

            #region Constructors

            public Message()
            {
            }

            public Message(Type type)
            {
                this.TypeOf = type;
            }

            public Message(Type type, QS._core_c_.Base3.Incarnation receiverIncarnation) : this(type)
            {
                this.ReceiverIncarnation = receiverIncarnation;
            }

            public Message(Type type, QS._core_c_.Base3.Incarnation receiverIncarnation, uint sequenceNo) : this(type, receiverIncarnation)
            {
                this.SequenceNo = sequenceNo;
            }

            public Message(Type type, QS._core_c_.Base3.Incarnation receiverIncarnation, uint sequenceNo, QS._core_c_.Base3.Message transmittedMessage)
                : this(type, receiverIncarnation, sequenceNo)
            {
                this.TransmittedMessage = transmittedMessage;
            }

            #endregion

            public Type TypeOf;
            public QS._core_c_.Base3.Incarnation ReceiverIncarnation;
            public uint SequenceNo;
            public QS._core_c_.Base3.Message TransmittedMessage;

            #region DetermineComponents

            private void DetermineComponents(out bool needsReceiverIncarnation, out bool needsSequenceNo, out bool needsTransmittedMessage)
            {
                switch (TypeOf)
                {
                    case Type.DATA:
                        needsReceiverIncarnation = needsSequenceNo = needsTransmittedMessage = true;
                        break;

                    case Type.DATA_ACK:
                        needsReceiverIncarnation = needsSequenceNo = true;
                        needsTransmittedMessage = false;
                        break;

                    case Type.CONNECT_ACK:
                        needsReceiverIncarnation = true;
                        needsTransmittedMessage = needsSequenceNo = false;
                        break;

                    default:
                        needsReceiverIncarnation = needsSequenceNo = needsTransmittedMessage = false;
                        break;
                }
            }

            #endregion

            #region Printing

            public override string ToString()
            {
                StringBuilder s = new StringBuilder(TypeOf.ToString());
                s.Append("(");
                bool needsReceiverIncarnation, needsSequenceNo, needsTransmittedMessage;
                DetermineComponents(out needsReceiverIncarnation, out needsSequenceNo, out needsTransmittedMessage);
                if (needsReceiverIncarnation)
                {
                    s.Append("RecInc(");
                    s.Append(ReceiverIncarnation.ToString());
                    s.Append(")");
                }
                if (needsSequenceNo)
                {
                    s.Append("SeqNo(");
                    s.Append(SequenceNo.ToString());
                    s.Append(")");
                }
                if (needsTransmittedMessage)
                {
                    s.Append("Messg(");
                    s.Append(TransmittedMessage.ToString());
                    s.Append(")");
                }
                s.Append(")");
                return s.ToString();
            }

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                { 
                    bool needsReceiverIncarnation, needsSequenceNo, needsTransmittedMessage;
                    DetermineComponents(out needsReceiverIncarnation, out needsSequenceNo, out needsTransmittedMessage);

                    QS.Fx.Serialization.SerializableInfo info = needsTransmittedMessage
                        ? TransmittedMessage.SerializableInfo.Extend((ushort)ClassID.Senders6_ReliableSender_Message, 0, 0, 0)
                        : new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Senders6_ReliableSender_Message, 0, 0, 0);

                    int growheader = ((needsReceiverIncarnation ? 1 : 0) + (needsSequenceNo ? 1 : 0)) * sizeof(uint) + sizeof(byte);
                    info.HeaderSize += (ushort) growheader;
                    info.Size += growheader;
                    
                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                bool needsReceiverIncarnation, needsSequenceNo, needsTransmittedMessage;
                DetermineComponents(out needsReceiverIncarnation, out needsSequenceNo, out needsTransmittedMessage);

                int nappended = 1;
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    *headerptr = (byte) TypeOf;

                    if (needsReceiverIncarnation)
                    {
                        *((uint *)(headerptr + nappended)) = ReceiverIncarnation.SeqNo;
                        nappended += sizeof(uint);
                    }

                    if (needsSequenceNo)
                    {
                        *((uint *)(headerptr + nappended)) = SequenceNo;
                        nappended += sizeof(uint);
                    }
                }
                header.consume(nappended);

                if (needsTransmittedMessage)
                    TransmittedMessage.SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                int nread = 1;
                bool needsReceiverIncarnation, needsSequenceNo, needsTransmittedMessage;

                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    TypeOf = (Type)(*headerptr);

                    DetermineComponents(out needsReceiverIncarnation, out needsSequenceNo, out needsTransmittedMessage);

                    if (needsReceiverIncarnation)
                    {
                        ReceiverIncarnation = new QS._core_c_.Base3.Incarnation(*((uint *)(headerptr + nread)));
                        nread += sizeof(uint);
                    }

                    if (needsSequenceNo)
                    {
                        SequenceNo = *((uint *)(headerptr + nread));
                        nread += sizeof(uint);
                    }
                }
                header.consume(nread);

                if (needsTransmittedMessage)
                    TransmittedMessage.DeserializeFrom(ref header, ref data);
            }

            #endregion
        }

        #endregion

        #region Class Receiver

        [QS._core_c_.Diagnostics.ComponentContainer]
        [QS.Fx.Base.Inspectable]
        private class Receiver : QS.Fx.Inspection.Inspectable
        {
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            public Receiver(ReliableSender owner, QS._core_c_.Base3.InstanceID sourceAddress)
            {
                ((QS._core_c_.Diagnostics2.IContainer)owner.diagnosticsContainerForReceivers).Register(sourceAddress.ToString(), diagnosticsContainer);

                this.owner = owner;
                this.sourceAddress = sourceAddress;
                underlyingSender = owner.underlyingSenderCollection[sourceAddress.Address];
                incomingWindow = new FlowControl_1_.IncomingWindow(WindowSize);

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
            }

            private ReliableSender owner;
            private QS._core_c_.Base3.InstanceID sourceAddress;
            private QS._qss_c_.Base3_.ISerializableSender underlyingSender;
            private FlowControl_1_.IIncomingWindow incomingWindow;

#if STATISTICS_NonredundancyIndicator
            [QS._core_c_.Diagnostics.Component("Nonredundancy Indicator (X = time, Y = nonredundant)")]
            [QS._core_c_.Diagnostics2.Property("NonredundancyIndicator")]
            private QS._qss_c_.Statistics_.Samples2D timeSeries_nonredundancyIndicator = new QS._qss_c_.Statistics_.Samples2D("Senders6.ReliableSender.Receiver.NonredundancyIndicator");
#endif

#if STATISTICS_Received
            [QS._core_c_.Diagnostics.Component("Receives (X = time, Y = seqno)")]
            [QS._core_c_.Diagnostics2.Property("Receives")]
            private QS._qss_c_.Statistics_.Samples2D timeSeries_receives = new QS._qss_c_.Statistics_.Samples2D("Senders6.ReliableSender.Receiver.Receives");
#endif

            #region Message Callback

            public void messageCallback(Message message)
            {
#if DEBUG_ReliableSender
                owner.logger.Log(this, "__MessageCallback(" + sourceAddress.ToString() + ") : " + message.ToString());
#endif

                lock (this)
                {
                    switch (message.TypeOf)
                    {
                        case Message.Type.CONNECT:
                        {
                            underlyingSender.send((uint)ReservedObjectID.Senders6_ReliableSender_AcknowledgementChannel,
                                new Message(Message.Type.CONNECT_ACK, sourceAddress.Incarnation));
                        }
                        break;

                        case Message.Type.DATA:
                        {
                            if (message.ReceiverIncarnation.Equals(owner.localIID.Incarnation))
                            {
#if STATISTICS_NonredundancyIndicator
                                timeSeries_nonredundancyIndicator.Add(owner.clock.Time, message.SequenceNo);
#endif

                                if (incomingWindow.accepts(message.SequenceNo) && incomingWindow.lookup(message.SequenceNo) == null)
                                {
#if STATISTICS_NonredundancyIndicator
                                    timeSeries_nonredundancyIndicator.Add(owner.clock.Time, 1);
#endif

#if DEBUG_ReliableSender
                                    owner.logger.Log(this, "__MessageCallback(" + sourceAddress.ToString() + ") : Acknowledging_New " +
                                        message.SequenceNo.ToString());
#endif

                                    underlyingSender.send((uint)ReservedObjectID.Senders6_ReliableSender_AcknowledgementChannel,
                                        new Message(Message.Type.DATA_ACK, sourceAddress.Incarnation, message.SequenceNo));

                                    incomingWindow.insert(message.SequenceNo, message);
                                    while (incomingWindow.ready())
                                    {
                                        Message messageToConsume = (Message) incomingWindow.consume();
                                        incomingWindow.cleanupOneGuy();

#if DEBUG_ReliableSender
                                        owner.logger.Log(this, "__MessageCallback(" + sourceAddress.ToString() + ") : Delivering " +
                                            messageToConsume.SequenceNo.ToString());
#endif
                                        owner.demultiplexer.dispatch(messageToConsume.TransmittedMessage.destinationLOID, sourceAddress,
                                            messageToConsume.TransmittedMessage.transmittedObject);
                                    }
                                }
                                else if (message.SequenceNo <= incomingWindow.lastConsumedSeqNo())
                                {
#if STATISTICS_NonredundancyIndicator
                                    timeSeries_nonredundancyIndicator.Add(owner.clock.Time, 0);
#endif

#if DEBUG_ReliableSender
                                    owner.logger.Log(this, "__MessageCallback(" + sourceAddress.ToString() + ") : Acknowledging_Old " + 
                                        message.SequenceNo.ToString());
#endif

                                    underlyingSender.send((uint)ReservedObjectID.Senders6_ReliableSender_AcknowledgementChannel,
                                        new Message(Message.Type.DATA_ACK, sourceAddress.Incarnation, message.SequenceNo));
                                }
                                else
                                {
#if STATISTICS_NonredundancyIndicator
                                    timeSeries_nonredundancyIndicator.Add(owner.clock.Time, -1);
#endif

                                    // cannot accept this message........... for future extensions
                                }
                            }
                            else
                            {
                                underlyingSender.send((uint)ReservedObjectID.Senders6_ReliableSender_AcknowledgementChannel,
                                    new Message(Message.Type.REQUEST_RECONNECT));
                            }
                        }
                        break;

                        default:
                            break;
                    }
                }
            }

            #endregion

            #region CrashingCallback

            public void crashingCallback()
            {
                // .................................................................................................................................................
            }

            #endregion
        }

        #endregion

        #region Class Sender

        [QS._core_c_.Diagnostics.ComponentContainer]
        [QS.Fx.Base.Inspectable]
        private class Sender : QS.Fx.Inspection.Inspectable, Base3_.IReliableSerializableSender, FlowControl3.IRetransmittingSender
        {
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            public Sender(ReliableSender owner, QS.Fx.Network.NetworkAddress destinationAddress)
            {
                ((QS._core_c_.Diagnostics2.IContainer) owner.diagnosticsContainerForSenders).Register(
                    destinationAddress.ToString(), diagnosticsContainer);

                this.owner = owner;
                this.destinationAddress = destinationAddress;
                underlyingSender = owner.underlyingSenderCollection[destinationAddress];

                this.retransmissionTimeout = owner.defaultRetransmissionTimeout;
                this.exponentialBackoff = owner.exponentialBackoff;
                this.adaptiveAdjustment = owner.adaptiveTimeoutAdjustment;
                this.maximumRetransmissionTimeout = owner.maximumRetransmissionTimeout;
                this.minimumRetransmissionTimeout = owner.minimumRetransmissionTimeout;

                if (adaptiveAdjustment)
                {
                    retransmissionController = new FlowControl3.RetransmissionController1(this,
                        new QS._qss_c_.FlowControl3.RetransmissionController1.Configuration(owner.defaultRetransmissionTimeout, 0.95, 2));

                    QS._core_c_.Diagnostics2.IModule module = retransmissionController as QS._core_c_.Diagnostics2.IModule;
                    if (module != null)
                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("RetransmissionController", module.Component);
                        
                }

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
            }

            private bool exponentialBackoff, adaptiveAdjustment;
            private double retransmissionTimeout, minimumRetransmissionTimeout, maximumRetransmissionTimeout;

            private ReliableSender owner;
            private QS.Fx.Network.NetworkAddress destinationAddress;
            private QS._qss_c_.Base3_.ISerializableSender underlyingSender;

            private Queue<Request> pendingQueue = new Queue<Request>();
            private QS._core_c_.Base3.Incarnation incarnation;
            private bool connected = false, connecting = false;
            private FlowControl_1_.IOutgoingWindow outgoingWindow;

            private FlowControl3.IRetransmissionController retransmissionController;

            public override string ToString()
            {
                return "Sender(" + destinationAddress.ToString() + ")";
            }

            #region IRetransmittingSender Members

            double QS._qss_c_.FlowControl3.IRetransmittingSender.RetransmissionTimeout
            {
                get { return retransmissionTimeout; }
                set 
                {
                    double newTimeout = value;
                    if (newTimeout < minimumRetransmissionTimeout)
                        newTimeout = minimumRetransmissionTimeout;
                    else if (newTimeout > maximumRetransmissionTimeout)
                        newTimeout = maximumRetransmissionTimeout;

                    retransmissionTimeout = newTimeout; 
                }
            }

            #endregion

#if STATISTICS_SendTimes
            [QS._core_c_.Diagnostics.Component("Send Times (X = time)")]
            [QS._core_c_.Diagnostics2.Property("SendTimes")]
            private QS._qss_c_.Statistics_.Samples1D timeSeries_sendTimes = new QS._qss_c_.Statistics_.Samples1D("Senders6.ReliableSender.SendTimes");
#endif

#if STATISTICS_CompletionTimes
            [QS._core_c_.Diagnostics.Component("Completion Times (X = time, Y = seqno)")]
            [QS._core_c_.Diagnostics2.Property("CompletionTimes")]
            private QS._qss_c_.Statistics_.Samples2D timeSeries_completionTimes = new QS._qss_c_.Statistics_.Samples2D("Senders6.ReliableSender.CompletionTimes");
#endif

#if STATISTICS_RetransmitTimes
            [QS._core_c_.Diagnostics.Component("Retransmission Times (X = time, Y = seqno)")]
            [QS._core_c_.Diagnostics2.Property("RetransmissionTimes")]
            private QS._qss_c_.Statistics_.Samples2D timeSeries_retransmitTimes = new QS._qss_c_.Statistics_.Samples2D("Senders6.ReliableSender.RetransmissionTimes");
#endif

#if STATISTICS_TimeToAcknowledge
            [QS._core_c_.Diagnostics.Component("Time To Acknowledge (X = seqno, Y = delay)")]
            [QS._core_c_.Diagnostics2.Property("TimeToAcknowledge")]
            private QS._qss_c_.Statistics_.Samples2D timeSeries_timeToAcknowledge = new QS._qss_c_.Statistics_.Samples2D("Senders6.ReliableSender.TimeToAcknowledge");
#endif

#if STATISTICS_RetransmissionRates
            [QS._core_c_.Diagnostics.Component("Retransmission Rates (X = seqno, Y = nretransmissions)")]
            [QS._core_c_.Diagnostics2.Property("RetransmissionRates")]
            private QS._qss_c_.Statistics_.Samples2D timeSeries_retransmissionRates = new QS._qss_c_.Statistics_.Samples2D("Senders6.ReliableSender.RetransmissionTimes");
#endif

            #region Managing Instance Senders

            public InstanceSender GetInstanceSender(QS._core_c_.Base3.Incarnation incarnation)
            {
                return new InstanceSender(this, incarnation);
            }

            #endregion

            #region Class InstanceSender

            public class InstanceSender : Base3_.IReliableSerializableSender
            {
                public InstanceSender(Sender owner, QS._core_c_.Base3.Incarnation incarnation)
                {
                    this.owner = owner;
                    this.incarnation = incarnation;
                }

                private Sender owner;
                private QS._core_c_.Base3.Incarnation incarnation;

                #region IReliableSerializableSender Members

                QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
                    QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
                {
                    return owner.BeginSend(destinationLOID, incarnation.SeqNo, data, completionCallback, asynchronousState);
                }

                void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
                {
                    ((Base3_.IReliableSerializableSender)owner).EndSend(asynchronousOperation);
                }

                #endregion

                #region ISerializableSender Members

                QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
                {
                    get { return owner.destinationAddress; }
                }

                void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
                {
                    ((Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
                }

                int QS._qss_c_.Base3_.ISerializableSender.MTU
                {
                    get { return ((QS._qss_c_.Base3_.ISerializableSender)owner).MTU; }
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

            #region Internal Processing

            public void crashingCallback(QS._core_c_.Base3.Incarnation incarnation)
            {
                lock (this)
                {
                    if (connected && this.incarnation.SeqNo == incarnation.SeqNo)
                        reconnect();
                }
            }

            private void processQueue()
            {
                try
                {
                    while (pendingQueue.Count > 0 && outgoingWindow.hasSpace())
                    {
                        Request request = pendingQueue.Dequeue();

                        if (!request.Incarnation.HasValue || request.Incarnation.Value == this.incarnation.SeqNo)
                        {
                            request.SequenceNo = outgoingWindow.append(request);

#if STATISTICS_SendTimes
                            timeSeries_sendTimes.Add(owner.clock.Time);
#endif

// #if STATISTICS_TimeToAcknowledge
                            request.SendTime = owner.clock.Time;
// #endif

                            lock (request)
                            {
                                underlyingSender.send((uint)ReservedObjectID.Senders6_ReliableSender_MessageChannel,
                                    new Message(Message.Type.DATA, incarnation, request.SequenceNo, request.Message));

                                request.RetransmissionAlarm = owner.alarmClock.Schedule(retransmissionTimeout,
                                    new QS.Fx.Clock.AlarmCallback(this.retransmissionAlarmCallback), request);
                            }
                        }
                        else
                        {
                            // dropping request, wrong incarnation
                        }
                    }
                }
                catch (Exception exc)
                {
                    owner.logger.Log(this, "__ProcessQueue(" + destinationAddress.ToString() + ") : Caught exception :" + 
                        exc.ToString());
                }
            }

            private void reconnect()
            {
                connected = false;

                Queue<Request> undeliveredQueue = new Queue<Request>();
                while (outgoingWindow.oldest() != null)
                {
                    Request request = (Request)outgoingWindow.oldest();
                    if (request.RetransmissionAlarm != null)
                    {
                        request.RetransmissionAlarm.Cancel();
                        request.RetransmissionAlarm = null;
                    }
                    request.Acknowledged = false;

                    undeliveredQueue.Enqueue(request);
                    outgoingWindow.removeOldest();
                }

                while (pendingQueue.Count > 0)
                    undeliveredQueue.Enqueue(pendingQueue.Dequeue());

                pendingQueue = undeliveredQueue;
                outgoingWindow = null;

                connect();
            }

            private void connect()
            {
                connecting = true;

                underlyingSender.send(
                    (uint)ReservedObjectID.Senders6_ReliableSender_MessageChannel, new Message(Message.Type.CONNECT));
                owner.alarmClock.Schedule(ConnectionTimeout.TotalSeconds,
                    new QS.Fx.Clock.AlarmCallback(connectionAlarmCallback), null);
            }

            #endregion

            #region Acknowledgement Callback

            public void acknowledgementCallback(QS._core_c_.Base3.Incarnation acknowledgementIncarnation, Message message)
            {
#if DEBUG_ReliableSender
                owner.logger.Log(this, "__AcknowledgementCallback(" + destinationAddress.ToString() + ") : " + 
                    acknowledgementIncarnation.ToString() + ", " + message.ToString());
#endif

                List<Request> completedGuys = new List<Request>();

                lock (this)
                {
                    switch (message.TypeOf)
                    {
                        case Message.Type.CONNECT_ACK:
                        {
                            if (message.ReceiverIncarnation.Equals(owner.localIID.Incarnation))
                            {
                                if (connecting)
                                {
                                    incarnation = acknowledgementIncarnation;
                                    connecting = false;
                                    connected = true;
                                    outgoingWindow = new FlowControl_1_.OutgoingWindow(WindowSize);

                                    processQueue();
                                }
                            }
                        }
                        break;

                        case Message.Type.DATA_ACK:
                        {
                            if (message.ReceiverIncarnation.Equals(owner.localIID.Incarnation) && 
                                connected && acknowledgementIncarnation.Equals(incarnation))
                            {
                                Request request = (Request) outgoingWindow.lookup(message.SequenceNo);
                                if (request != null)
                                {
#if STATISTICS_CompletionTimes
                                    timeSeries_completionTimes.Add(owner.clock.Time, message.SequenceNo);
#endif

                                    double completion_time = owner.clock.Time - request.SendTime;

#if STATISTICS_TimeToAcknowledge
                                    timeSeries_timeToAcknowledge.Add(message.SequenceNo, completion_time);
#endif

#if STATISTICS_RetransmissionRates
                                    timeSeries_retransmissionRates.Add(message.SequenceNo, request.NRetransmissions);
#endif

                                    if (adaptiveAdjustment)
                                        retransmissionController.completed(completion_time, request.NRetransmissions);                                    

                                    lock (request)
                                    {
                                        if (request.RetransmissionAlarm != null)
                                        {
                                            request.RetransmissionAlarm.Cancel();
                                            request.RetransmissionAlarm = null;
                                        }

                                        request.Acknowledged = true;
                                    }

                                    while (true)
                                    {
                                        Request req = (Request) outgoingWindow.oldest();
                                        if (req == null || !req.Acknowledged)
                                            break;
                                        req.SetCompleted();
                                        completedGuys.Add(req);

                                        outgoingWindow.removeOldest();
                                    }
                                        
                                    processQueue();
                                }
                            }
                        }
                        break;

                        case Message.Type.REQUEST_RECONNECT:
                        {
                            reconnect();
                        }
                        break;

                        default:
                            break;
                    }
                }

                foreach (Request request in completedGuys)
                {
                    try
                    {
                        request.InvokeCallback();
                    }
                    catch (Exception exc)
                    {
                        owner.logger.Log(this, "__InvokeCallback: " + exc.ToString());
                    }
                }
            }

            #endregion

            #region Alarm Callbacks

            [Logging_1_.IgnoreCallbacks]
            private void connectionAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (this)
                {
                    if (!connected)
                    {
                        underlyingSender.send(
                            (uint)ReservedObjectID.Senders6_ReliableSender_MessageChannel, new Message(Message.Type.CONNECT));
                        alarmRef.Reschedule();
                    }
                }
            }

            private void retransmissionAlarmCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (this)
                {
                    if (connected)
                    {
                        Request request = (Request) alarmRef.Context;
                        lock (request)
                        {
                            if (request.RetransmissionAlarm != null)
                            {
                                underlyingSender.send((uint)ReservedObjectID.Senders6_ReliableSender_MessageChannel,
                                    new Message(Message.Type.DATA, incarnation, request.SequenceNo, request.Message));

                                if (this.exponentialBackoff)
                                {
                                    double timeout = alarmRef.Timeout * 2;
                                    if (timeout > maximumRetransmissionTimeout)
                                        timeout = maximumRetransmissionTimeout;
                                    alarmRef.Reschedule(timeout);
                                }
                                else
                                    alarmRef.Reschedule(retransmissionTimeout);

// #if STATISTICS_RetransmissionRates
                                request.NRetransmissions = request.NRetransmissions + 1;
// #endif

#if STATISTICS_RetransmitTimes
                                timeSeries_retransmitTimes.Add(owner.clock.Time, request.SequenceNo);
#endif
                            }
                        }

#if STATISTICS_RetransmitTimes
                        timeSeries_retransmitTimes.Add(owner.clock.Time, request.SequenceNo);
#endif
                    }
                }
            }

            #endregion

            #region Class Request

            [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
            [QS.Fx.Base.Inspectable] 
            private class Request : QS.Fx.Inspection.Inspectable, Base3_.IAsynchronousOperation
            {
                public Request(uint destinationLOID, System.Nullable<uint> incarnation, QS.Fx.Serialization.ISerializable data,
                    QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
                {
                    message = new QS._core_c_.Base3.Message(destinationLOID, data);
                    this.completionCallback = completionCallback;
                    this.asynchronousState = asynchronousState;
                    this.incarnation = incarnation;
                }

                [QS.Fx.Printing.Printable]
                private QS._core_c_.Base3.Message message;
                private QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback;
                private object asynchronousState;
                [QS.Fx.Printing.Printable]
                private uint sequenceNo;
                private QS.Fx.Clock.IAlarm retransmissionAlarm;
                private bool acknowledged = false, completed = false;
                [QS.Fx.Printing.Printable]
                private System.Nullable<uint> incarnation;

                public override string ToString()
                {
                    return QS.Fx.Printing.Printable.ToString(this);
                }

// #if STATISTICS_TimeToAcknowledge
                private double sendTime;

                public double SendTime
                {
                    get { return sendTime; }
                    set { sendTime = value; }
                }
// #endif

// #if STATISTICS_RetransmissionRates
                private int nretransmissions = 0;

                public int NRetransmissions
                {
                    get { return nretransmissions; }
                    set { nretransmissions = value; }
                }
// #endif

                public void SetCompleted()
                {
                    lock (this)
                    {
                        completed = true;
                        if (retransmissionAlarm != null)
                            retransmissionAlarm.Cancel();
                        retransmissionAlarm = null;
                    }
                }

                public void InvokeCallback()
                {
                    Base3_.AsynchronousOperationCallback callback = completionCallback;
                    if (callback != null)
                        callback(this);
                }

                #region Accessors

                public System.Nullable<uint> Incarnation
                {
                    get { return incarnation; }
                }

                public bool Acknowledged
                {
                    get { return acknowledged; }
                    set { acknowledged = value; }
                }

                public uint SequenceNo
                {
                    set { sequenceNo = value; }
                    get { return sequenceNo; }
                }

                public QS._core_c_.Base3.Message Message
                {
                    get { return message; }
                }

                public QS.Fx.Clock.IAlarm RetransmissionAlarm
                {
                    set { retransmissionAlarm = value; }
                    get { return retransmissionAlarm; }
                }

                #endregion

                #region IAsynchronousOperation Members

                void QS._qss_c_.Base3_.IAsynchronousOperation.Cancel()
                {
                    throw new NotSupportedException("Cannot cancel, this functionality is disabled.");
                }

                void QS._qss_c_.Base3_.IAsynchronousOperation.Ignore()
                {
                    completionCallback = null;
                }

                bool QS._qss_c_.Base3_.IAsynchronousOperation.Cancelled
                {
                    get { return false; }
                }

                #endregion

                #region IAsyncResult Members

                object IAsyncResult.AsyncState
                {
                    get { return asynchronousState; }
                }

                System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
                {
                    get { throw new NotSupportedException(); }
                }

                bool IAsyncResult.CompletedSynchronously
                {
                    get { return false; }
                }

                bool IAsyncResult.IsCompleted
                {
                    get { return completed; }
                }

                #endregion
            }
                
            #endregion        

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(
                uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                return BeginSend(destinationLOID, null, data, completionCallback, asynchronousState);
            }

            public QS._qss_c_.Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, System.Nullable<uint> incarnation, 
                QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                Request request = new Request(destinationLOID, incarnation, data, completionCallback, asynchronousState);

                lock (this)
                {
                    pendingQueue.Enqueue(request);

                    if (connected)
                        processQueue();
                    else
                        if (!connecting)
                            connect();
                }

                return request;
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {                
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return destinationAddress; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((QS._qss_c_.Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { return underlyingSender.MTU - Message.HeaderOverhead; }
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

        #region Function GetSender

        private Sender GetSender(QS.Fx.Network.NetworkAddress destinationAddress)
        {
            lock (this)
            {
                if (senders.ContainsKey(destinationAddress))
                    return senders[destinationAddress];
                else
                {
                    Sender sender = new Sender(this, destinationAddress);
                    senders[destinationAddress] = sender;
                    return sender;
                }
            }
        }

        #endregion

        #region ISenderCollection<IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get  { return new string[] { "Senders", "Receivers" }; }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get 
            {
                if (attributeName.Equals("Senders"))
                    return inspectableSendersWrapper;
                else if (attributeName.Equals("Receivers"))
                    return inspectableReceiversWrapper;
                else
                    throw new ArgumentException("No such attribute."); 
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "ReliableSender_Collection"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion

        #region ISenderCollection<ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region ISenderCollection<InstanceID,ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get { return GetSender(destinationAddress.Address).GetInstanceSender(destinationAddress.Incarnation); }
        }

        #endregion

        #region ISenderCollection<InstanceID,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get { return GetSender(destinationAddress.Address).GetInstanceSender(destinationAddress.Incarnation); }
        }

        #endregion

        #region IInspectable Members

        QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
        {
            get { return this; }
        }

        #endregion
    }  
}
