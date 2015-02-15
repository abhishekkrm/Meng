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

// #define DEBUG_Receiver
// #define STATISTICS_ForwardingActions
// #define STATISTICS_TokenActions

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings4
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Receiver : QS.Fx.Inspection.Inspectable, IReceiver
    {
        public Receiver(QS._core_c_.Base3.InstanceID senderAddress, QS._core_c_.Base3.InstanceID[] receiverAddresses, uint localAddress,
            QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, uint replicationCoefficient, IReceiverContext context,
            int maximumNAKsPerToken, QS.Fx.Clock.IClock clock)
            : this(senderAddress, receiverAddresses, localAddress, logger, demultiplexer, replicationCoefficient, context,
            new ForwardingScheme(true, true), maximumNAKsPerToken, clock)
        {
        }

        public Receiver(QS._core_c_.Base3.InstanceID senderAddress, QS._core_c_.Base3.InstanceID[] receiverAddresses, uint localAddress,
            QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, uint replicationCoefficient, IReceiverContext context,
            ForwardingScheme forwardingScheme, int maximumNAKsPerToken, QS.Fx.Clock.IClock clock)
        {
            this.clock = clock;             
            this.senderAddress = senderAddress;
            this.receiverAddresses = receiverAddresses;
            this.localAddress = localAddress;
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.replicationCoefficient = replicationCoefficient;
            this.context = context;
            this.forwardingScheme = forwardingScheme;
            this.maximumNAKsPerToken = maximumNAKsPerToken;

            cellSize = (uint) Math.Ceiling(((double) receiverAddresses.Length) / ((double) replicationCoefficient));
        }

        private QS._core_c_.Base3.InstanceID senderAddress;
        private uint localAddress, replicationCoefficient, cellSize;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;
        private Base3_.IDemultiplexer demultiplexer;
        private QS._core_c_.Base3.InstanceID[] receiverAddresses;
        private uint aggregatedMaxSeqNo, lastCutOff;
        private NAKs cleanupNAKs = new NAKs(0, new uint[] { });
        private Receivers_1_.IAckCollection ackCollection = new Receivers_1_.AckCollection1();
        private ForwardingScheme forwardingScheme;
        private IReceiverContext context;
        private IDictionary<uint, QS._core_c_.Base3.Message> messageCache = new Dictionary<uint, QS._core_c_.Base3.Message>();
        private int maximumNAKsPerToken;
        private uint lastUsedTokenSeqNo;

#if STATISTICS_ForwardingActions
        [QS.CMS.Diagnostics.Component("Requesting Times")]
        private Statistics.Samples timeSeries_requestingTimes = new QS.CMS.Statistics.Samples();
        [QS.CMS.Diagnostics.Component("Forwarding Times")]
        private Statistics.Samples timeSeries_forwardingTimes = new QS.CMS.Statistics.Samples();
#endif

#if STATISTICS_TokenActions
        [QS.CMS.Diagnostics.Component("Token Creation Times (X=time, Y=seqno)")]
        private Statistics.SamplesXY timeSeries_tokenCreationTimes = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Token Forwarding Times (X=time, Y=seqno)")]
        private Statistics.SamplesXY timeSeries_tokenForwardingTimes = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Token Completion Times (X=time, Y=seqno)")]
        private Statistics.SamplesXY timeSeries_tokenCompletionTimes = new QS.CMS.Statistics.SamplesXY();
#endif

        #region Class ForwardingScheme

        public class ForwardingScheme
        {
            public ForwardingScheme(bool pushAllowed, bool pullAllowed)
            {
                this.pushAllowed = pushAllowed;
                this.pullAllowed = pullAllowed;

                cachingNecessary = pushAllowed || pullAllowed;
            }

            private bool pushAllowed, pullAllowed, cachingNecessary;

            public bool PushAllowed
            {
                get { return pushAllowed; }
            }

            public bool PullAllowed
            {
                get { return pullAllowed; }
            }

            public bool CachingNecessary
            {
                get { return cachingNecessary; }
            }

            public override string ToString()
            {
                return "(push:" + (PushAllowed ? "on" : "off") + ", pull:" + (PullAllowed ? "on" : "off") + ")";
            }
        }

        #endregion
        
        #region GetInfo

        private void GetInfo(uint cutoffSeqNo, out uint maximumSeqNo, out IEnumerable<uint> nakCollection)
        {
            maximumSeqNo = ackCollection.Maximum;
            nakCollection = ackCollection.CutOff(cutoffSeqNo);

#if DEBUG_Receiver
            logger.Log(this, "__GetInfo(" + senderAddress.ToString() + "): " +
                maximumSeqNo.ToString() + ", { " + Helpers.CollectionHelper.ToStringSeparated<uint>(nakCollection, ", ") + " }");
#endif
        }

        #endregion

        #region Class Control

        [QS.Fx.Serialization.ClassID(ClassID.Ring4_Receiver_Control)]
        private class Control : QS.Fx.Serialization.ISerializable
        {
            public enum Command : byte
            {
                Pull
            }

            public Control()
            {
            }

            public Control(Command command, uint sequenceNo)
            {
                this.command = command;
                this.sequenceNo = sequenceNo;
            }

            private Command command;
            private uint sequenceNo;

            public Command CommandType
            {
                get { return command; }
            }

            public uint SequenceNo
            {
                get { return sequenceNo; }
            }

            public override string ToString()
            {
                return "(" + command.ToString() + ", " + sequenceNo.ToString() + ")";
            }

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    return new QS.Fx.Serialization.SerializableInfo(
                        (ushort)ClassID.Ring4_Receiver_Control, (ushort)(sizeof(byte) + sizeof(uint)), (sizeof(byte) + sizeof(uint)), 0);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* pbuffer = header.Array)
                {
                    byte *pheader = pbuffer + header.Offset;
                    *pheader = (byte)command;
                    *((uint*)(pheader + sizeof(byte))) = sequenceNo;
                }
                header.consume(sizeof(byte) + sizeof(uint));
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* pbuffer = header.Array)
                {
                    byte* pheader = pbuffer + header.Offset;
                    command = (Command) (*pheader);
                    sequenceNo = *((uint*)(pheader + sizeof(byte)));
                }
                header.consume(sizeof(byte) + sizeof(uint));
            }

            #endregion
        }

        #endregion

        #region Push and Pull

        private void Push(uint sequenceNo, QS._core_c_.Base3.Message message, ICollection<uint> destinationAddresses)
        {
#if DEBUG_Receiver
            logger.Log(this, "__Push : " + message.ToString() + " to " +
                QS.CMS.Helpers.CollectionHelper.ToStringSeparated<uint>(destinationAddresses, ","));
#endif

#if STATISTICS_ForwardingActions
            timeSeries_forwardingTimes.addSample(clock.Time);
#endif

            List<QS._core_c_.Base3.InstanceID> addresses = new List<QS._core_c_.Base3.InstanceID>();
            foreach (uint address in destinationAddresses)
                addresses.Add(receiverAddresses[address]);
            context.Forward(sequenceNo, message, addresses);
        }

        private void Pull(uint sequenceNo, uint providerAddress)
        {
#if DEBUG_Receiver
            logger.Log(this, "__Pull : " + sequenceNo.ToString() + " from " + providerAddress.ToString());
#endif

#if STATISTICS_ForwardingActions
            timeSeries_requestingTimes.addSample(clock.Time);
#endif

            context.SendControl(receiverAddresses[providerAddress], new Control(Control.Command.Pull, sequenceNo));
        }

        #endregion

        #region CleanupCache

        private void CleanupCache(NAKs cleanupNAKs)
        {
            List<uint> toRemove = new List<uint>();
            foreach (uint seqno in messageCache.Keys)
            {
                if (seqno <= cleanupNAKs.MaximumSeqNo && !cleanupNAKs.Missed.Contains(seqno))
                    toRemove.Add(seqno);
            }

#if DEBUG_Receiver
            logger.Log(this, "__CleanupCache(" + senderAddress.ToString() + "): removing from cache {" + 
                Helpers.CollectionHelper.ToStringSeparated<uint>(toRemove, ",") + "} out of a total of " + 
                messageCache.Count.ToString() + " messages");
#endif

            foreach (uint seqno in toRemove)
                messageCache.Remove(seqno);
        }

        #endregion

        #region IReceiver Members

        void IReceiver.ReceiveControl(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable message)
        {
#if DEBUG_Receiver
            logger.Log(this, "__ReceiveControl(" + sourceAddress.ToString() + "): " + message.ToString());
#endif

            Control control = message as Control;
            if (control == null)
                throw new Exception("Wrong message type");

            switch (control.CommandType)
            {
                case Control.Command.Pull:
                {
                    if (messageCache.ContainsKey(control.SequenceNo))
                        context.Forward(control.SequenceNo, messageCache[control.SequenceNo],
                            new QS._core_c_.Base3.InstanceID[] { sourceAddress });
                }
                break;
            }
        }

        void IReceiver.Receive(uint sequenceNo, QS._core_c_.Base3.Message message)
        {
#if DEBUG_Receiver
            logger.Log(this, "__Receive(" + senderAddress.ToString() + "): " + 
                sequenceNo.ToString() + ", " + message.ToString());
#endif

            lock (this)
            {
                if (ackCollection.Add(sequenceNo))
                {
                    if (forwardingScheme.CachingNecessary 
                        && (localAddress % cellSize == sequenceNo % cellSize) && !messageCache.ContainsKey(sequenceNo))
                    {
#if DEBUG_Receiver
                        logger.Log(this, "__Receive(" + senderAddress.ToString() + "): Caching " + sequenceNo.ToString());
#endif

                        messageCache.Add(sequenceNo, message);
                    }

                    demultiplexer.dispatch(message.destinationLOID, senderAddress, message.transmittedObject);
                }
            }

#if DEBUG_Receiver
            logger.Log(this, "__Receive(" + senderAddress.ToString() + "): AckCollection = " +
                ackCollection.ToString());
#endif
        }

        void IReceiver.Process(out QS.Fx.Serialization.ISerializable outgoingObject)
        {
            lock (this)
            {
                uint maximumSeqNo;
                IEnumerable<uint> nakCollection;
                GetInfo(aggregatedMaxSeqNo, out maximumSeqNo, out nakCollection);

                lastCutOff = lastCutOff + (uint) this.maximumNAKsPerToken;
                if (maximumSeqNo < lastCutOff)
                    lastCutOff = maximumSeqNo;

                Token token = new Token(
                    localAddress, ++lastUsedTokenSeqNo, aggregatedMaxSeqNo, lastCutOff, nakCollection, cleanupNAKs);

                outgoingObject = token;

#if STATISTICS_TokenActions
                timeSeries_tokenCreationTimes.addSample(clock.Time, token.TokenSeqNo);
#endif
            }

#if DEBUG_Receiver
            logger.Log(this, "__Process(" + senderAddress.ToString() + "): Outgoing new " + outgoingObject.ToString()); 
#endif
        }

        private void Push(Token token)
        {
            foreach (KeyValuePair<uint, Token.NAK> incomingNak in token.NakCollection)
            {
                if (!incomingNak.Value.Forwarded && localAddress % cellSize == incomingNak.Key % cellSize &&
                    messageCache.ContainsKey(incomingNak.Key))
                {
                    incomingNak.Value.Forwarded = true;
                    Push(incomingNak.Key, messageCache[incomingNak.Key], incomingNak.Value.MissingAddresses);
                }
            }
        }

        void IReceiver.Process(QS.Fx.Serialization.ISerializable incomingObject, out QS.Fx.Serialization.ISerializable outgoingObject)
        {
#if DEBUG_Receiver
            logger.Log(this, "__Process(" + senderAddress.ToString() + "): Incoming " + incomingObject.ToString());
#endif

            lock (this)
            {
                Token token = incomingObject as Token;
                if (token == null)
                    throw new Exception("Received an unrecognized object.");

#if STATISTICS_TokenActions
                timeSeries_tokenForwardingTimes.addSample(clock.Time, token.TokenSeqNo);
#endif

                if (forwardingScheme.PushAllowed)
                    Push(token);

                CleanupCache(token.CleanupCacheNAKs);

                uint maximumSeqNo;
                IEnumerable<uint> nakCollection, newNakCollection;
                GetInfo(token.CutoffSeqNo, out maximumSeqNo, out nakCollection);

                token.Append(localAddress, maximumSeqNo, nakCollection, out newNakCollection);

                if (forwardingScheme.PullAllowed)
                {
                    foreach (uint seqno in newNakCollection)
                    {
                        if (localAddress > seqno % cellSize)
                        {
                            uint providerAddress = (uint)
                                ((localAddress / cellSize) - ((localAddress % cellSize > seqno % cellSize) ? 0 : 1)) * cellSize + seqno % cellSize;

                            while (true)
                            {
                                if (!token.NakCollection[seqno].MissingAddresses.Contains(providerAddress))
                                {
                                    Pull(seqno, providerAddress);
                                    break;
                                }

                                if (providerAddress >= cellSize)
                                    providerAddress -= cellSize;
                                else
                                {
#if DEBUG_Receiver
                                    logger.Log(this, "__Process(" + senderAddress.ToString() + "): Cannot pull " + seqno.ToString() +
                                        " from anywhere");
#endif

                                    break;
                                }
                            }
                        }
                    }
                }

                outgoingObject = token;
            }

#if DEBUG_Receiver
            logger.Log(this, "__Process(" + senderAddress.ToString() + "): Outgoing " + outgoingObject.ToString());
#endif
        }

        void IReceiver.Process(QS.Fx.Serialization.ISerializable incomingObject)
        {
#if DEBUG_Receiver
            logger.Log(this, "__Process(" + senderAddress.ToString() + "): Incoming last " + incomingObject.ToString());
#endif

            lock (this)
            {
                Token token = incomingObject as Token;
                if (token == null)
                    throw new Exception("Received an unrecognized object.");

#if STATISTICS_TokenActions
                timeSeries_tokenCompletionTimes.addSample(clock.Time, token.TokenSeqNo);
#endif

                aggregatedMaxSeqNo = token.MaximumSeqNo;

                if (forwardingScheme.PushAllowed)
                    Push(token);

                List<uint> allNAKs = new List<uint>();
                List<uint> senderNAKs = new List<uint>();
                foreach (KeyValuePair<uint, Token.NAK> element in token.NakCollection)
                {
                    allNAKs.Add(element.Key);
                    bool covered = true;
                    foreach (uint address in element.Value.MissingAddresses)
                    {
                        if (address % cellSize == element.Key % cellSize)
                        {
                            covered = false;
                            break;
                        }
                    }

                    if (!covered)
                        senderNAKs.Add(element.Key);
                }

                cleanupNAKs = new NAKs(token.CutoffSeqNo, allNAKs);

#if DEBUG_Receiver
                logger.Log(this, "__Process(" + senderAddress.ToString() + "): CleanupNAKs = " + cleanupNAKs.ToString());
#endif

                CleanupCache(cleanupNAKs);

                NAKs acknowledgementNAKs = new NAKs(token.CutoffSeqNo, senderNAKs);

#if DEBUG_Receiver
                logger.Log(this, "__Process(" + senderAddress.ToString() + "): AcknowledgementNAKs = " + 
                    acknowledgementNAKs.ToString());
#endif

                context.Acknowledge(new QS._core_c_.Base3.Message(0, acknowledgementNAKs));
            }
        }

        #endregion
    }
}
