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

// #define DEBUG_ReceiverCollection
// #define STATISTICS_MonitorTokenSizes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings4
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class ReceiverCollection : QS.Fx.Inspection.Inspectable, IReceiverCollection
    {
        public ReceiverCollection(QS._core_c_.Base3.InstanceID[] receiverAddresses, QS._core_c_.Base3.InstanceID localAddress,
            QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection,
            double frequency, uint replicationCoefficient, IContext context, int maximumNAKsPerToken)
        {
            this.receiverAddresses = new QS._core_c_.Base3.InstanceID[receiverAddresses.Length];
            receiverAddresses.CopyTo(this.receiverAddresses, 0);
            Array.Sort<QS._core_c_.Base3.InstanceID>(this.receiverAddresses);
            this.localAddress = localAddress;
            localNodeNo = (uint) Array.BinarySearch<QS._core_c_.Base3.InstanceID>(this.receiverAddresses, localAddress);
            if (localNodeNo < 0)
                throw new Exception("Local address is not on the list of receivers.");
            leader = (localNodeNo == 0);
            successorNodeNo = (uint)((localNodeNo + 1) % this.receiverAddresses.Length);
            successorAddress = this.receiverAddresses[successorNodeNo];
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.frequency = frequency;
            this.replicationCoefficient = replicationCoefficient;
            this.alarmClock = alarmClock;
            this.senderCollection = senderCollection;
            successorSender = senderCollection[successorAddress];
            this.context = context;
            this.maximumNAKsPerToken = maximumNAKsPerToken;
            this.clock = clock;

            if (leader)
                tokenAlarmRef = 
                    alarmClock.Schedule(1 / frequency, new QS.Fx.Clock.AlarmCallback(this.TokenAlarm), null);

#if DEBUG_ReceiverCollection
            logger.Log(this, "__Constructor(" + context.Name + ") : Successor " + successorAddress.ToString() +
                "\nReceiverAddresses:\n" + 
                Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(this.receiverAddresses, "\n"));
#endif

            receiversInspectableWrapper = new QS._qss_e_.Inspection_.DictionaryWrapper2<QS._core_c_.Base3.InstanceID, IReceiver>(
                "Receivers", receivers);
        }

        [QS.Fx.Base.Inspectable("Receivers")]
        private QS._qss_e_.Inspection_.DictionaryWrapper2<QS._core_c_.Base3.InstanceID, IReceiver> receiversInspectableWrapper;

        private QS._core_c_.Base3.InstanceID[] receiverAddresses;
        private QS._core_c_.Base3.InstanceID localAddress, successorAddress;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private double frequency;
        private uint localNodeNo, successorNodeNo, replicationCoefficient;
        private bool leader;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection;
        private Base3_.IReliableSerializableSender successorSender;
        private QS.Fx.Clock.IAlarm tokenAlarmRef;
        private IDictionary<QS._core_c_.Base3.InstanceID, IReceiver> receivers = new Dictionary<QS._core_c_.Base3.InstanceID, IReceiver>();
        private IContext context;
        private int maximumNAKsPerToken;
        private QS.Fx.Clock.IClock clock;
        private uint lastUsedTokenSeqNo = 0;

#if STATISTICS_MonitorTokenSizes
        [QS.CMS.Diagnostics.Component("Token Sizes")]
        private QS.CMS.Statistics.SamplesXY timeSeries_tokenSizes = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Create Receiver

        private IReceiver CreateReceiver(QS._core_c_.Base3.InstanceID sourceAddress)
        {
            return new Receiver(
                sourceAddress, receiverAddresses, localNodeNo, logger, demultiplexer, replicationCoefficient,
                new ReceiverContext(sourceAddress, context, (uint) MyChannel.Control, senderCollection),
                maximumNAKsPerToken, clock);
        }

        private IReceiver GetReceiver(QS._core_c_.Base3.InstanceID sourceAddress)
        {
            IReceiver receiver;
            lock (this)
            {
                if (receivers.ContainsKey(sourceAddress))
                    receiver = receivers[sourceAddress];
                else
                    receivers[sourceAddress] = receiver = this.CreateReceiver(sourceAddress);
            }
            return receiver;
        }

        #endregion

        private enum MyChannel : uint
        {
            Token, Control
        }

        #region IReceiverCollection Members

        IReceiver IReceiverCollection.LocalReceiver
        {
            get { return GetReceiver(localAddress); }
        }

        void IReceiverCollection.Receive(
            QS._core_c_.Base3.InstanceID sourceAddress, uint sequenceNo, QS._core_c_.Base3.Message message)
        {
            GetReceiver(sourceAddress).Receive(sequenceNo, message);
        }

        void IReceiverCollection.Shutdown()
        {
            // TODO: Should implement some kind of smooth shutdown............

            lock (this)
            {
#if DEBUG_ReceiverCollection
                logger.Log(this, "__Shutdown(" + context.Name + ")");
#endif

                if (tokenAlarmRef != null)
                    tokenAlarmRef.Cancel();
                tokenAlarmRef = null;
            }
        }

        void IReceiverCollection.ReceiveControl(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.Message receivedMessage)
        {
            switch (receivedMessage.destinationLOID)
            {
                case ((uint)MyChannel.Token):
                {
                    lock (this)
                    {
                        TokenCollection tokenCollection = receivedMessage.transmittedObject as TokenCollection;
                        if (tokenCollection == null)
                            throw new Exception("Unrecognized object received.");

#if STATISTICS_MonitorTokenSizes
                        timeSeries_tokenSizes.addSample(clock.Time, ((QS.Fx.Serialization.ISerializable)tokenCollection).SerializableInfo.Size);
#endif

                        if (leader)
                            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> element in tokenCollection.Tokens)
                                GetReceiver(element.Key).Process(element.Value);
                        else
                        {
                            TokenCollection forwardedCollection = new TokenCollection(localAddress, tokenCollection.SeqNo);
                            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, QS.Fx.Serialization.ISerializable> element in tokenCollection.Tokens)
                            {
                                QS.Fx.Serialization.ISerializable forwardedObject;
                                GetReceiver(element.Key).Process(element.Value, out forwardedObject);
                                forwardedCollection.Tokens.Add(element.Key, forwardedObject);
                            }

                            QS._core_c_.Base3.Message message = context.Wrap(new QS._core_c_.Base3.Message((uint) MyChannel.Token, forwardedCollection));
                            successorSender.BeginSend(message.destinationLOID, message.transmittedObject, null, null);
                        }
                    }
                }
                break;

                case ((uint)MyChannel.Control):
                {
                    ControlReq controlReq = receivedMessage.transmittedObject as ControlReq;
                    if (controlReq == null)
                        throw new Exception("Wrong object received.");

                    IReceiver receiver = null;
                    lock (this)
                    {
                        if (receivers.ContainsKey(controlReq.Address))
                            receiver = receivers[controlReq.Address];
                    }

                    if (receiver != null)
                        receiver.ReceiveControl(sourceAddress, controlReq.Message);
                    else
                        throw new Exception("No receiver for " + controlReq.Address.ToString());
                }
                break;
            }
        }

        #endregion

        #region Token Alarm

        private void TokenAlarm(QS.Fx.Clock.IAlarm alarmRef)
        {
#if DEBUG_ReceiverCollection
            logger.Log(this, "__TokenAlarm(" + context.Name + ")");
#endif

            lock (this)
            {
                TokenCollection tokenCollection = new TokenCollection(localAddress, ++lastUsedTokenSeqNo);
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, IReceiver> element in receivers)
                {
                    QS.Fx.Serialization.ISerializable token;
                    element.Value.Process(out token);
                    if (token != null)
                        tokenCollection.Tokens.Add(element.Key, token);
                }

                QS._core_c_.Base3.Message message = context.Wrap(new QS._core_c_.Base3.Message((uint) MyChannel.Token, tokenCollection));
                successorSender.BeginSend(message.destinationLOID, message.transmittedObject, null, null);
            }

            alarmRef.Reschedule();
        }

        #endregion
    }
}
