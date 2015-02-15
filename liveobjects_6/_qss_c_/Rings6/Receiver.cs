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

// #define DEBUG_ReceivingAgent1
// #define DEBUG_ReceivingAgent1_ShowReceives

#define DEBUG_CollectStatistics
#define DEBUG_CollectStatistics_LogIndividualReceiveTimes

#define OPTION_UseForwardingSinks
#define OPTION_UseSenderAckNakClients

// #define DEBUG_Quiescence
#define OPTION_ProcessingCrashes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public sealed class Receiver
        : QS.Fx.Inspection.Inspectable, IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>, QS._core_c_.Diagnostics2.IModule, IDisposable
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Constructor

        public Receiver(QS._core_c_.Base3.InstanceID senderAddress, 
            IReceiverContext receiverContext, IReceiverConfiguration receiverConfiguration, QS._core_c_.Statistics.IStatisticsController statisticsController)
        {
            this.receiverContext = receiverContext;
            this.receiverConfiguration = receiverConfiguration;
            this.clock = receiverContext.Clock;
            this.senderAddress = senderAddress;

#if DEBUG_ReceivingAgent1
            log("Creating the receiver.", "");
#endif

            ackCollection = new Receivers4.AckCollection1(clock);
            partitionAckCollection = new Receivers4.AckCollection1(clock);
            messageRepository = new Receivers4.MessageRepository1<QS._core_c_.Base3.Message>();

#if OPTION_UseForwardingSinks
            forwarder = new Forwarder(receiverContext.SinkCollection, messageRepository, clock, 
                new WrappingCallback(this.WrappingCallback), receiverContext.Logger);
#else
            forwardingBucket = new ForwardingBucket(
                new ForwardingCallback(this.PartitionForwardCallback), messageRepository, clock);

            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("StatusesOfPeers", diagnosticsContainerForStatusesOfOtherPeers);
#endif

#if OPTION_UseSenderAckNakClients
            senderAckNakClient = new SenderAckNakClient();
#endif

            tableOfMaxContiguous = new uint[receiverContext.NumberOfPartitions];

            receivingRateCalculator = new FlowControl3.RateCalculator3(clock, TimeSpan.FromSeconds(1));
            this.rateSharingAlgorithm = receiverConfiguration.RateSharingAlgorithm;
            isSender = receiverContext.LocalAddress.Equals(senderAddress);

#if DEBUG_CollectStatistics
            timeSeries_dataReceiveTimes = statisticsController.Allocate2D(
                "receives", "received data", "time", "s", "time when message was received", "seqno", "", "sequence number of the received message");
#endif

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
        }

        #endregion

        #region WrappingCallback

        private QS._core_c_.Base3.Message WrappingCallback(uint seqno, QS._core_c_.Base3.Message message)
        {
            return receiverContext.AgentContext.Message(
                new QS._core_c_.Base3.Message(
                    (uint) ReservedObjectID.Rings6_ReceivingAgent1_ForwardingChannel, 
                    new ForwardingRequest(
                        senderAddress, 
                        Partition2Region(seqno), 
                        message)),
                Receivers4.DestinationType.Receiver);
        }

        #endregion

        #region Fields

        private bool disposed;

        private QS._core_c_.Base3.InstanceID senderAddress;
        [QS._core_c_.Diagnostics.Component("Acks")]
        private Receivers4.IAckCollection ackCollection;
        [QS._core_c_.Diagnostics.Component("Partition Acks")]
        private Receivers4.IAckCollection partitionAckCollection;
        [QS._core_c_.Diagnostics.Component("Cache")]
        private Receivers4.IMessageRepository<QS._core_c_.Base3.Message> messageRepository;

        private uint maximumSeenLastRound, partition_maximumStable, maximumToClean, partition_maximumToClean;
        private uint[] tableOfMaxContiguous;
        private QS.Fx.Clock.IClock clock;
        private IReceiverContext receiverContext;
        private IReceiverConfiguration receiverConfiguration;

        private bool isSender;
        private FlowControl3.IRateCalculator receivingRateCalculator;
        private RateSharingAlgorithmClass rateSharingAlgorithm;

#if OPTION_ProcessingCrashes
        private bool quiescence;
        private event EventHandler resumeCallback;
#endif

#if OPTION_UseSenderAckNakClients
        [QS._core_c_.Diagnostics.Component("SenderAckNakClient")]
        [QS._core_c_.Diagnostics2.Module("SenderAckNakClient")]
        private ISenderAckNakClient senderAckNakClient;
#endif

        #endregion

        #region Statistics

#if DEBUG_CollectStatistics

#if DEBUG_CollectStatistics_LogIndividualReceiveTimes
        // [QS.CMS.Diagnostics.Component("Data Received (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("Receives")]
        private QS._core_c_.Statistics.ISamples2D timeSeries_dataReceiveTimes;
#endif

        [QS._core_c_.Diagnostics.Component("Forwardings Received (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("ForwardingsReceived")]
        private Statistics_.Samples2D timeSeries_forwardingReceiveTimes = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.ForwardingReceiveTimes");
        [QS._core_c_.Diagnostics.Component("Pull Nak Counts (X=time, Y=count)")]
        [QS._core_c_.Diagnostics2.Property("PullNakCounts")]
        private Statistics_.Samples2D timeSeries_requestToForwardCounts = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.RequestToForwardCounts");
        [QS._core_c_.Diagnostics.Component("Forward Coverage (X=time, Y=ratio)")]
        [QS._core_c_.Diagnostics2.Property("ForwardCoverage")]
        private Statistics_.Samples2D timeSeries_forwardingCoverage = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.ForwardingCoverage");
        [QS._core_c_.Diagnostics.Component("Acks To Sender : Max Contiguous (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("AcksToSender-MaxContiguous")]
        private Statistics_.Samples2D timeSeries_acksToSenderMaxContiguous = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.AcksToSenderMaxContiguous");
        [QS._core_c_.Diagnostics.Component("Acks To Sender : Total Ack Count (X=time, Y=count)")]
        [QS._core_c_.Diagnostics2.Property("AcksToSenderTotalAckCount")]
        private Statistics_.Samples2D timeSeries_acksToSenderTotalAckCount = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.AcksToSenderTotalAckCount");
        [QS._core_c_.Diagnostics.Component("Acks To Sender : Isolated Ranges (X=time, Y=count)")]
        [QS._core_c_.Diagnostics2.Property("AcksToSenderIsolatedRanges")]
        private Statistics_.Samples2D timeSeries_acksToSenderIsolatedRanges = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.AcksToSenderIsolatedRanges");
        [QS._core_c_.Diagnostics.Component("Acks To Sender : Max Isolated Ack (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("AcksToSenderMaxIsolatedAck")]
        private Statistics_.Samples2D timeSeries_acksToSenderMaxIsolatedAck = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.AcksToSenderMaxIsolatedAck");

        [QS._core_c_.Diagnostics.Component("Local Max contiguous (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("LocalMaxContiguous")]
        private Statistics_.Samples2D timeSeries_localMaxContiguous = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.LocalMaxContiguous");
        [QS._core_c_.Diagnostics.Component("Predecessors Max contiguous (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("PredecessorsMaxContiguous")]
        private Statistics_.Samples2D timeSeries_predecessorsMaxContiguous = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.PredecessorsMaxContiguous");
        [QS._core_c_.Diagnostics.Component("Cutoff (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("CutOff")]
        private Statistics_.Samples2D timeSeries_cutoff = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.Cutoff");
        [QS._core_c_.Diagnostics.Component("Local Max Seen (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("LocalMaxSeen")]
        private Statistics_.Samples2D timeSeries_localMaxSeen = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.LocalMaxSeen");
        [QS._core_c_.Diagnostics.Component("Predecessors Max Seen (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("PredecessorsMaxSeen")]
        private Statistics_.Samples2D timeSeries_predecessorsMaxSeen = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.PredecessorsMaxSeen");
        [QS._core_c_.Diagnostics.Component("Maximum To Clean (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("MaximumToClean")]
        private Statistics_.Samples2D timeSeries_maximumToClean = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.MaximumToClean");
        [QS._core_c_.Diagnostics.Component("Naks Reported in Tokens (X=time, Y=naks)")]
        [QS._core_c_.Diagnostics2.Property("NaksReportedInTokens")]
        private Statistics_.Samples2D timeSeries_naksReportedInTokens = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.NaksReportedInTokens");

        [QS._core_c_.Diagnostics.Component("Receive Rates Reported in Tokens (X=time, Y=receive rate)")]
        [QS._core_c_.Diagnostics2.Property("ReceiveRatesReportedInTokens")]
        private Statistics_.Samples2D timeSeries_receiveRatesReportedInTokens = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.ReceiveRatesReportedInTokens");
        
        [QS._core_c_.Diagnostics.Component("Combined Minimum Receive Rates (X=time, Y=receive rate)")]
        [QS._core_c_.Diagnostics2.Property("CombinedMinimumReceiveRates")]
        private Statistics_.Samples2D timeSeries_combinedMinimumReceiveRates = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.CombinedMinimumReceiveRates");
        [QS._core_c_.Diagnostics.Component("Combined Average Receive Rates (X=time, Y=receive rate)")]
        [QS._core_c_.Diagnostics2.Property("CombinedAverageReceiveRates")]
        private Statistics_.Samples2D timeSeries_combinedAverageReceiveRates = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.CombinedAverageReceiveRates");
        [QS._core_c_.Diagnostics.Component("Combined Maximum Receive Rates (X=time, Y=receive rate)")]
        [QS._core_c_.Diagnostics2.Property("CombinedMaximumReceiveRates")]
        private Statistics_.Samples2D timeSeries_combinedMaximumReceiveRates = new QS._qss_c_.Statistics_.Samples2D("Rings6.Receiver.CombinedMaximumReceiveRates");

        [QS._core_c_.Diagnostics.Component("Combined Statistics (X=time, Y=seqno)")]
        [QS._core_c_.Diagnostics2.Property("CombinedStatistics")]
        private QS._core_e_.Data.DataCo TimeSeries_CombinedStatistics
        {
            get
            {
                QS._core_e_.Data.DataCo dataCo = new QS._core_e_.Data.DataCo("Combined Statistics", "", "Time", "s", "", "Packet Sequence Number", "", "");
                // dataCo.Add(new QS.TMS.Data.Data2D("data received", timeSeries_dataReceiveTimes.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("local max contiguous", timeSeries_localMaxContiguous.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("predecessors max contiguous", timeSeries_predecessorsMaxContiguous.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("cutoff", timeSeries_cutoff.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("local max seen", timeSeries_localMaxSeen.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("predecessors max seen", timeSeries_predecessorsMaxSeen.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("maximum to clean", timeSeries_maximumToClean.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("naks reported in tokens", timeSeries_naksReportedInTokens.Samples));
                return dataCo;
            }
        }
#endif

        #endregion

        #region Forwarding sinks and buckets

#if OPTION_UseForwardingSinks
        
        [QS._core_c_.Diagnostics.Component("Forwarder")]
        [QS._core_c_.Diagnostics2.Module("Forwarder")]
        private IForwarder forwarder;

#else

        private IDictionary<QS._core_c_.Base3.InstanceID, StatusOfAnotherPeer> statusesOfOtherPeers =
            new Dictionary<QS._core_c_.Base3.InstanceID, StatusOfAnotherPeer>();
        private QS.CMS.QS._core_c_.Diagnostics2.Container diagnosticsContainerForStatusesOfOtherPeers = new QS.CMS.QS._core_c_.Diagnostics2.Container();

        [Diagnostics.Component("Forwarding Bucket")]
        private IForwardingBucket forwardingBucket;

        #region GetStatusOfAnotherPeer

        private StatusOfAnotherPeer GetStatusOfAnotherPeer(QS._core_c_.Base3.InstanceID address)
        {
            StatusOfAnotherPeer statusOfAnotherPeer;
            if (!statusesOfOtherPeers.TryGetValue(address, out statusOfAnotherPeer))
                statusesOfOtherPeers.Add(address, statusOfAnotherPeer = new StatusOfAnotherPeer(this, address));
            return statusOfAnotherPeer;
        }

        #endregion

        #region Class StatusOfAnotherPeer

        [Diagnostics.ComponentContainer]
        private class StatusOfAnotherPeer : TMS.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
        {
            private QS.CMS.QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS.CMS.QS._core_c_.Diagnostics2.Container();

        #region IModule Members

            QS.CMS.QS._core_c_.Diagnostics2.IComponent QS.CMS.QS._core_c_.Diagnostics2.IModule.Component
            {
                get { return diagnosticsContainer; }
            }

        #endregion

            public StatusOfAnotherPeer(Receiver owner, QS._core_c_.Base3.InstanceID address)
            {
                this.owner = owner;
                this.address = address;

                ((QS._core_c_.Diagnostics2.IContainer)owner.diagnosticsContainerForStatusesOfOtherPeers).Register(address.ToString(), diagnosticsContainer);

                // ......................................................................................................................................................                

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
            }

            private Receiver owner;
            private QS._core_c_.Base3.InstanceID address;

#if DEBUG_CollectStatistics
            [Diagnostics.Component("Forwarding Times (X=time, Y=seqno)")]
            [QS._core_c_.Diagnostics2.Property("ForwardingTimes")]
            private QS.CMS.Statistics.Samples2D timeseries_forwardingTimes = 
                new Statistics.Samples2D("Rings6.Receiver.StatusOfAnotherPeer.ForwardingTimes");
#endif

            public void Forwarded(uint seqno)
            {
#if DEBUG_CollectStatistics
                timeseries_forwardingTimes.Add(owner.clock.Time, seqno);
#endif
            }
        }

        #endregion

#endif

        #endregion

        #region logging

#if DEBUG_ReceivingAgent1
        private void log(string description, string details)
        {
            if (receiverContext.EventLogger.Enabled)
                receiverContext.EventLogger.Log(
                    new MyEvent(receiverContext.Clock.Time, receiverContext.LocalAddress, this, description, details));
        }
#endif

        #endregion

        #region Receive

        public void Receive(uint sequenceNo, QS._core_c_.Base3.Message message, bool retransmission, bool forwarding)
        {
            double time_now = receiverContext.Clock.Time;

#if DEBUG_ReceivingAgent1 && DEBUG_ReceivingAgent1_ShowReceives
                    log("Receive(" + sequenceNo.ToString() + ")", message.ToString());
#endif

#if DEBUG_ReceivingAgent1 && DEBUG_ReceivingAgent1_ShowReceives
                    log("Status_Before(" + sequenceNo.ToString() + ")", StatusString());
#endif

#if DEBUG_CheckStatus
                    _CheckStatus("Receive_Entering : " + sequenceNo.ToString());
#endif

#if DEBUG_CollectStatistics
                    if (timeSeries_dataReceiveTimes.Enabled)
                    {
                        lock (timeSeries_dataReceiveTimes)
                        {
                            if (timeSeries_dataReceiveTimes.Enabled)
                                timeSeries_dataReceiveTimes.Add(time_now, sequenceNo);
                        }
                    }
#endif

            bool should_deliver;
            lock (this)
            {
                should_deliver = ackCollection.Add(sequenceNo);
                if (should_deliver)
                {
                    SampleRate();

                    uint message_partNo = (sequenceNo - 1) % receiverContext.NumberOfPartitions;
                    if (message_partNo == receiverContext.PartitionNumber)
                    {
                        uint partitionSeqNo = ((sequenceNo - 1 - receiverContext.PartitionNumber) / receiverContext.NumberOfPartitions) + 1;
                        partitionAckCollection.Add(partitionSeqNo);
                        messageRepository.Add(partitionSeqNo, message);

#if OPTION_UseForwardingSinks
                        forwarder.Receive(sequenceNo);                        
#else
                        if (receiverConfiguration.PullCaching)
                            forwardingBucket.Add(sequenceNo, message);
#endif
                    }
                    else
                    {
                        partitionAckCollection.Seen(this.UpperBound_Region2Partition(sequenceNo));
                    }
                }
            }

            if (should_deliver)
                receiverContext.Demultiplexer.dispatch(message.destinationLOID, senderAddress, message.transmittedObject);

#if DEBUG_ReceivingAgent1 && DEBUG_ReceivingAgent1_ShowReceives
                    log("Status_After(" + sequenceNo.ToString() + ")", StatusString());
#endif

#if DEBUG_CheckStatus
                    _CheckStatus("Receive_Leaving : " + sequenceNo.ToString());
#endif

#if OPTION_ProcessingCrashes
            if (quiescence)
            {
                quiescence = false;

                if (resumeCallback != null)
                {
#if DEBUG_Quiescence
                    receiverContext.logger.Log(this, "Received message { " + sequenceNo.ToString() + 
                        " } while quiescence = true, resuming now.");
#endif

                    resumeCallback(this, null);
                }
                else
                {
#if DEBUG_Quiescence
                    receiverContext.logger.Log(this, "Cannot resume, no callback defined.");
#endif
                }
            }
#endif

        }

        #endregion

        #region SequenceNo Conversions

        private uint LowerBound_Region2Partition(uint bound)
        {
            return Receivers4.AckHelpers.SeqNo.ToPartitionLowerBound(
                bound, receiverContext.PartitionNumber, receiverContext.NumberOfPartitions);
        }

        private uint LowerBound_Region2Partition(uint bound, uint partno)
        {
            return Receivers4.AckHelpers.SeqNo.ToPartitionLowerBound(bound, partno, receiverContext.NumberOfPartitions);
        }

        private uint UpperBound_Region2Partition(uint bound)
        {
            return Receivers4.AckHelpers.SeqNo.ToPartitionUpperBound(
                bound, receiverContext.PartitionNumber, receiverContext.NumberOfPartitions);
        }

        private uint UpperBound_Region2Partition(uint bound, uint partno)
        {
            return Receivers4.AckHelpers.SeqNo.ToPartitionUpperBound(bound, partno, receiverContext.NumberOfPartitions);
        }

        private uint Partition2Region(uint bound)
        {
            return Receivers4.AckHelpers.SeqNo.FromPartition(
                bound, receiverContext.PartitionNumber, receiverContext.NumberOfPartitions);
        }

        #endregion

        #region GenerateNaks

        private void GenerateNaks(uint partitionCutOff, IList<Base1_.Range<uint>> nakCollection, out uint partitionCovered)
        {
            Receivers4.AckHelpers.GetTrimmedNaks(partitionAckCollection.Missing, partitionCutOff,
                receiverConfiguration.MaximumNakRangesPerToken, nakCollection, out partitionCovered);

#if DEBUG_CollectStatistics
            double time_now = clock.Time;
            foreach (Base1_.Range<uint> range in nakCollection)
                for (uint seqno = range.From; seqno <= range.To; seqno++)
                    timeSeries_naksReportedInTokens.Add(time_now, seqno);
#endif
        }

        #endregion

        #region CalculateRemotePulls

        private IList<Base1_.Range<uint>>[] CalculateRemotePulls(uint cutoff, uint[] tableOfCutOffs)
        {
#if DEBUG_ReceivingAgent1
                    log("__________CalculateRemotePulls : Missing = { " + 
                        Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(ackCollection.Missing, ", ") + " }", StatusString());
#endif

            return Receivers4.AckHelpers.PartitionNaks(
                ackCollection.Missing, cutoff, tableOfCutOffs, receiverContext.NumberOfPartitions, receiverContext.PartitionNumber);
        }

        #endregion

        #region IntersectNaks

        private void IntersectNaks(
            IEnumerable<Base1_.Range<uint>> missing1, uint covered1, IEnumerable<Base1_.Range<uint>> missing2, uint covered2,
            IList<Base1_.Range<uint>> commonNaks, out uint commonCovered, uint maximumNaksAllowed)
        {
            Receivers4.AckHelpers.IntersectNaks(
                missing1, covered1, missing2, covered2, commonNaks, out commonCovered, maximumNaksAllowed);
        }

        #endregion

        #region RandomHostFromAnotherPartition

        private QS._core_c_.Base3.InstanceID RandomHostFromAnotherPartition(uint partno)
        {
            // TODO: Should account for failures when choosing a node........................................................................
            return receiverContext.ReceiverAddresses[partno];
        }

        #endregion

        #region RemotePulls

        private void RemotePulls(uint cutoff, uint[] tableOfCutOffs)
        {
            if (receiverConfiguration.ForwardingAllowed && cutoff > 0)
            {
                IList<Base1_.Range<uint>>[] requests = CalculateRemotePulls(cutoff, tableOfCutOffs);
                for (uint partno = 0; partno < receiverContext.NumberOfPartitions; partno++)
                {
                    if (requests[partno] != null && requests[partno].Count > 0)
                    {
                        QS._core_c_.Base3.InstanceID node = RandomHostFromAnotherPartition(partno);
                        if (node == null)
                        {
                            PullFromSender(partno, null, requests[partno]);
                        }
                        else
                        {
#if DEBUG_ReceivingAgent1
                                    log("Remote pull from partition " + partno.ToString() + " from node " + node.ToString(), "Requests { " +
                                        Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(requests[partno], ", ") + " }.");
#endif

                            PartitionAcknowledgement partitionAck =
                                new PartitionAcknowledgement(senderAddress, partno, receiverContext.NumberOfPartitions, 0, null, requests[partno]);
                            QS._core_c_.Base3.Message wrapper = receiverContext.AgentContext.Message(
                                new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_RequestingChannel, partitionAck),
                                Receivers4.DestinationType.Receiver);
                            ((Base3_.IReliableSerializableSender) receiverContext.SenderCollection[node]).BeginSend(
                                wrapper.destinationLOID, wrapper.transmittedObject, null, null);
                        }
                    }
                }
            }
        }

        #endregion

        #region Process Forwarded Messages

        public void ProcessForwardingRequest(QS._core_c_.Base3.InstanceID sourceAddress, ForwardingRequest request)
        {
#if DEBUG_CollectStatistics
            if (timeSeries_forwardingReceiveTimes.Enabled)
            {
                lock (timeSeries_forwardingReceiveTimes)
                {
                    if (timeSeries_forwardingReceiveTimes.Enabled)
                        timeSeries_forwardingReceiveTimes.Add(receiverContext.Clock.Time, request.SequenceNo);
                }
            }
#endif

#if DEBUG_ReceivingAgent1
            log("Received a FORWARDED message from " + sourceAddress.ToString() + " containing message " + 
                request.SequenceNo.ToString(), QS.Fx.Printing.Printable.ToString(request.Message));                        
#endif

            this.Receive(request.SequenceNo, request.Message, false, true);
        }

        #endregion

        #region PullFromSender

        private void PullFromSender(uint partno, IList<Base1_.Range<uint>> acks, IList<Base1_.Range<uint>> naks)
        {
            if ((acks != null && acks.Count > 0) || (naks != null && naks.Count > 0))
            {
#if DEBUG_ReceivingAgent1
                        log("Pulling from sender, in partition " + partno.ToString() + " requests { " +
                            Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(naks, ", ") + " }.", "");
#endif

                PartitionAcknowledgement partitionAck = 
                    new PartitionAcknowledgement(senderAddress, partno, receiverContext.NumberOfPartitions, 0, acks, naks);
                QS._core_c_.Base3.Message wrapper = receiverContext.AgentContext.Message(
                    new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_NakChannel, partitionAck),
                    Receivers4.DestinationType.Sender);
                ((Base3_.IReliableSerializableSender) receiverContext.SenderCollection[senderAddress]).BeginSend(
                    wrapper.destinationLOID, wrapper.transmittedObject, null, null);
            }
        }

        #endregion

        #region AcknowledgeToSender

        private void AcknowledgeToSender(uint maximumClean, IList<Base1_.Range<uint>> isolatedAcks, 
            double minimumReceiveRate, double averageReceiveRate, double maximumReceiveRate)
        {
#if DEBUG_ReceivingAgent1
                    log("Acknowledging to sender, maximumClean = " + maximumClean.ToString(), 
                        (isolatedAcks != null) ? ("Isolated Acks:\n" +
                            Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(isolatedAcks, ", ")) : "");
#endif

#if DEBUG_CollectStatistics
            double time_now = clock.Time;
            timeSeries_acksToSenderMaxContiguous.Add(time_now, maximumClean);
            int totalAckCount = (int) maximumClean;
            if (isolatedAcks != null)
            {
                timeSeries_acksToSenderIsolatedRanges.Add(time_now, isolatedAcks.Count);
                int maxIsolatedAck = (int)maximumClean;
                foreach (Base1_.Range<uint> range in isolatedAcks)
                {
                    totalAckCount += (int)(range.To - range.From + 1);
                    if (range.To > maxIsolatedAck)
                        maxIsolatedAck = (int)range.To;
                }
                timeSeries_acksToSenderMaxIsolatedAck.Add(time_now, maxIsolatedAck);
                timeSeries_acksToSenderTotalAckCount.Add(time_now, totalAckCount);
            }
#endif

            Acknowledgement acknowledgement = new Acknowledgement(
                maximumClean, isolatedAcks, minimumReceiveRate, averageReceiveRate, maximumReceiveRate);
            QS._core_c_.Base3.Message wrapper = receiverContext.AgentContext.Message(
                new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_AckChannel, acknowledgement),
                Receivers4.DestinationType.Sender);
            ((Base3_.IReliableSerializableSender) receiverContext.SenderCollection[senderAddress]).BeginSend(
                wrapper.destinationLOID, wrapper.transmittedObject, null, null);
        }

        #endregion

        #region PushAndPull

        private void PushAndPull(
            QS._core_c_.Base3.InstanceID forwarderAddress, IList<Base1_.Range<uint>> forward1to2, IList<Base1_.Range<uint>> forward2to1)
        {
            if (receiverConfiguration.ForwardingAllowed && (forward1to2.Count > 0 || forward2to1.Count > 0))
            {
#if DEBUG_ReceivingAgent1
                log("Doing a local push/pull with " + forwarderAddress.ToString() + ", push { " +
                    Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(forward2to1, ", ") + " }, pull { " +
                    Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(forward1to2, ", ") + " }", "");
#endif

                if (forward2to1.Count > 0)
                    Forward(forwarderAddress, forward2to1);

                if (forward1to2.Count > 0)
                {
                    PartitionAcknowledgement partitionAck = new PartitionAcknowledgement(
                        senderAddress, receiverContext.PartitionNumber, receiverContext.NumberOfPartitions, 0, null, forward1to2);
                    QS._core_c_.Base3.Message wrapper = receiverContext.AgentContext.Message(
                        new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_RequestingChannel, partitionAck),
                        Receivers4.DestinationType.Receiver);
                    ((Base3_.IReliableSerializableSender) receiverContext.SenderCollection[forwarderAddress]).BeginSend(
                        wrapper.destinationLOID, wrapper.transmittedObject, null, null);
                }
            }
        }

        #endregion

        #region ProcessPullRequest

        public void ProcessPullRequest(QS._core_c_.Base3.InstanceID requestorAddress, PartitionAcknowledgement request)
        {
#if DEBUG_CollectStatistics
            if (timeSeries_requestToForwardCounts.Enabled)
            {
                lock (timeSeries_requestToForwardCounts)
                {
                    if (timeSeries_requestToForwardCounts.Enabled)
                    {
                        double nakCount = 0;
                        foreach (Base1_.Range<uint> naks in request.IsolatedNaks)
                            nakCount += (naks.To - naks.From + 1);
                        timeSeries_requestToForwardCounts.Add(receiverContext.Clock.Time, nakCount);
                    }
                }
            }
#endif

#if DEBUG_ReceivingAgent1
            log("Received a PULL request from " + requestorAddress.ToString(), QS.Fx.Printing.Printable.ToString(request));                        
#endif

            if (request.PartitionIndex != receiverContext.PartitionNumber)
                throw new Exception("Wrong pull request: the requested partition number does not match the local assignment.");

            Forward(requestorAddress, request.IsolatedNaks);
        }

        #endregion

        #region Forwarding

        #region Forward a set of messages

        private void Forward(QS._core_c_.Base3.InstanceID destinationAddress, IList<Base1_.Range<uint>> requests)
        {
#if DEBUG_ReceivingAgent1
            log("Forwarding to " + destinationAddress.ToString() + " partition messages { " + 
                Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(requests, ", ") + " }.", "");
#endif

#if OPTION_UseForwardingSinks
            forwarder.Forward(destinationAddress, requests);
#else

            uint nrequested = 0, ncompleted = 0;
            foreach (Base.Range<uint> request in requests)
            {
                for (uint seqno = request.From; seqno <= request.To; seqno++)
                {
                    nrequested++;
                    Base3.Message message = new QS.CMS.Base3.Message();
                    if (messageRepository.Get(seqno, ref message))
                    {
                        ncompleted++;
                        Forward(destinationAddress, Partition2Region(seqno), message);
                    }
                    else
                    {
#if DEBUG_ReceivingAgent1
                                log("Error: CANNOT FORWARD to " + destinationAddress.ToString() + " message " + seqno.ToString(),
                                    StatusString());
#endif

                        if (receiverConfiguration.PullCaching)
                            forwardingBucket.Schedule(seqno, destinationAddress);
                    }
                }
            }

#if DEBUG_CollectStatistics
            if (timeSeries_forwardingCoverage.Enabled)
            {
                lock (timeSeries_forwardingCoverage)
                {
                    if (timeSeries_forwardingCoverage.Enabled)
                        timeSeries_forwardingCoverage.Add(
                            receiverContext.Clock.Time, ((double)ncompleted) / ((double)nrequested));
                }
            }
#endif

#endif
        }

        #endregion

#if OPTION_UseForwardingSinks
#else
        #region Forward a single message

        private void Forward(QS._core_c_.Base3.InstanceID destinationAddress, uint sequenceNo, Base3.Message request)
        {
#if DEBUG_ReceivingAgent1
            log("Forwarding to " + destinationAddress.ToString() + " region message " + sequenceNo.ToString(), 
                QS.Fx.Printing.Printable.ToString(request));
#endif

            ForwardingRequest forwardRequest = new ForwardingRequest(senderAddress, sequenceNo, request);
            Base3.Message wrapper = receiverContext.AgentContext.Message(
                new Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_ForwardingChannel, forwardRequest),
                Receivers4.DestinationType.Receiver);

            GetStatusOfAnotherPeer(destinationAddress).Forwarded(sequenceNo);

            ((Base3.IReliableSerializableSender) receiverContext.SenderCollection[destinationAddress]).BeginSend(
                wrapper.destinationLOID, wrapper.transmittedObject, null, null);
        }

        #endregion

        #region PartitionForwardCallback

        private Base3.IAsynchronousOperation PartitionForwardCallback(QS._core_c_.Base3.InstanceID address,
            uint sequenceNo, Base3.Message message, Base3.AsynchronousOperationCallback callback, object state)
        {
            ForwardingRequest forwardRequest = new ForwardingRequest(senderAddress, Partition2Region(sequenceNo), message);
            Base3.Message wrapper = receiverContext.AgentContext.Message(
                new Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_ForwardingChannel, forwardRequest),
                Receivers4.DestinationType.Receiver);

            GetStatusOfAnotherPeer(address).Forwarded(sequenceNo);

            return ((Base3.IReliableSerializableSender) receiverContext.SenderCollection[address]).BeginSend(
                wrapper.destinationLOID, wrapper.transmittedObject, callback, state);
        }

        #endregion
#endif

        #endregion

        #region PartitionPulls

        private void PartitionPulls(IList<Base1_.Range<uint>> aggregatedNaks, uint aggregatedCovered)
        {
            if ((receiverConfiguration.NaksAllowed && aggregatedNaks.Count > 0) || (receiverConfiguration.AcksAllowed && aggregatedCovered > 0))
            {
#if DEBUG_ReceivingAgent1
                        log("Partition requests from " + senderAddress.ToString(), "Packets { " +
                            Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(aggregatedNaks, ", ") + " }.");
#endif

                if (receiverConfiguration.AcksAllowed)
                {

                    // TODO: Add ack processing...
                }

                PullFromSender(receiverContext.PartitionNumber, null, aggregatedNaks);
            }
        }

        #endregion

        #region _CheckStatus

#if DEBUG_CheckStatus
                private void _CheckStatus(string pointInTimeString)
                {
                    try
                    {
                        IList<Base.Range<uint>> partitioned1 = Receivers4.AckHelpers.PartitionNaks(ackCollection.Missing,
                            ackCollection.MaximumSeen + 1, owner.numberOfPartitions, owner.numberOfPartitions)[owner.partitionNumber];
                        if (partitioned1 == null)
                            partitioned1 = new List<Base.Range<uint>>();
                        else
                            partitioned1 = Receivers4.AckHelpers.CompressNaks(partitioned1);

                        IEnumerator<Base.Range<uint>> x1 = partitioned1.GetEnumerator();
                        IEnumerator<Base.Range<uint>> x2 = partitionAckCollection.Missing.GetEnumerator();
                        bool d1 = !x1.MoveNext();
                        bool d2 = !x2.MoveNext();

                        while (!d1 || !d2)
                        {
                            if (d1 || d2 || x1.Current.From != x2.Current.From || x2.Current.To != x2.Current.To)
                                throw new Exception("Global and partition ack collections mismatch!\npartitioned1 = " +
                                    Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(partitioned1, ", "));

                            d1 = !x1.MoveNext();
                            d2 = !x2.MoveNext();
                        }

                        IEnumerator<Base.Range<uint>> naks1 = partitionAckCollection.Missing.GetEnumerator();
                        IEnumerator<Base.Range<uint>> naks2 =
                            messageRepository._Missing(partitionAckCollection.MaximumSeen).GetEnumerator();
                        bool finished1 = !naks1.MoveNext();
                        bool finished2 = !naks2.MoveNext();

                        while (!finished1 || !finished2)
                        {
                            if (finished1 || finished2 ||
                                naks1.Current.From != naks2.Current.From || naks1.Current.To != naks2.Current.To)
                                throw new Exception("Ack collection and message repository mismatch!");

                            finished1 = !naks1.MoveNext();
                            finished2 = !naks2.MoveNext();
                        }
                    }
                    catch (Exception exc)
                    {
#if DEBUG_ReceivingAgent1
                        owner.owner.eventLogger.Log(
                            new Logging.Events.ExceptionCaught(owner.owner.clock.Time, owner.owner.localAddress, exc,
                                "Point in time : \"" + pointInTimeString + "\"\n" + StatusString()));
#else
                        owner.owner.logger.Log(this, exc.ToString());
#endif
                    }
                }
#endif

        #endregion

        #region StatusString

#if DEBUG_ReceivingAgent1
                private string StatusString()
                {
                    return "Current Status:\n\nAckCollection : " + QS.Fx.Printing.Printable.ToString(ackCollection) + "NAKs: { " +
                        Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(ackCollection.Missing, ", ") + 
                        " }\n\nPartitionAckCollection : " + QS.Fx.Printing.Printable.ToString(partitionAckCollection) + "NAKs: { " +
                        Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(partitionAckCollection.Missing, ", ") + 
                        " }\n\nMessageCache : " + QS.Fx.Printing.Printable.ToString(messageRepository) + "\nNAKs: { " +
                        Helpers.CollectionHelper.ToStringSeparated<Base.Range<uint>>(
                            messageRepository._Missing(partitionAckCollection.MaximumSeen), ", ") + " } by " + 
                            partitionAckCollection.MaximumSeen.ToString() + "\n\n";
                }

#endif

        #endregion

        #region CalculateCutOff

        private uint CalculateCutOff()
        {
            uint cutoff = maximumSeenLastRound;
            if (cutoff > maximumToClean + receiverConfiguration.MaximumWindowWidth)
                cutoff = maximumToClean + receiverConfiguration.MaximumWindowWidth;
            return cutoff;
        }

        #endregion

        #region UpdateTableOfMaxContiguous

        private void UpdateTableOfMaxContiguous(uint[] tableOfMaxContiguous)
        {
            if (tableOfMaxContiguous.Length != this.tableOfMaxContiguous.Length)
                throw new Exception("Internal error: number of partitions does not match!");

            for (int ind = 0; ind < tableOfMaxContiguous.Length; ind++)
                if (tableOfMaxContiguous[ind] > this.tableOfMaxContiguous[ind])
                    this.tableOfMaxContiguous[ind] = tableOfMaxContiguous[ind];
        }

        #endregion

        #region Managing receive rates

        double GetReceiveRate()
        {
            double rate;
            switch (rateSharingAlgorithm)
            {
                case RateSharingAlgorithmClass.Compete:
                    rate = receivingRateCalculator.Rate;
                    break;

                case RateSharingAlgorithmClass.FairShare:
                    rate = receiverContext.ReceiveRate;
                    break;

                default:
                    throw new Exception("Boo.");
            }            

#if DEBUG_CollectStatistics
            if (timeSeries_receiveRatesReportedInTokens.Enabled)
                timeSeries_receiveRatesReportedInTokens.Add(clock.Time, rate);
#endif

            return rate;
        }

        private void SampleRate()
        {
            switch (rateSharingAlgorithm)
            {
                case RateSharingAlgorithmClass.Compete:
                    receivingRateCalculator.sample();
                    break;

                case RateSharingAlgorithmClass.FairShare:
                    if (!isSender)
                        receiverContext.RateSample();
                    break;

                default:
                    throw new Exception("Boo.");
            }
        }

//        double CombineReceiveRates(double rate1, double rate2)
//        {
//            return Math.Min(rate1, rate2);
//        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "Receiver(" + receiverContext.Name + " : " + senderAddress.ToString() + ")";
        }

        #endregion

        #region IPartitionedTokenRingMember<ReceiverIntraPartitionToken,ReceiverInterPartitionToken> Members

        #region Case 1

// ***(Case 1)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process()
        {
#if DEBUG_ReceivingAgent1
            log("___Case_1", StatusString());
#endif

            if (ackCollection.MaxContiguous > maximumToClean)
            {
                partition_maximumToClean = partition_maximumStable = maximumToClean = ackCollection.MaxContiguous;

#if OPTION_UseForwardingSinks
                forwarder.Cancel(maximumToClean);
#endif
                messageRepository.CleanUp(maximumToClean);

                double myrate = GetReceiveRate();
                AcknowledgeToSender(maximumToClean, null, myrate, myrate, myrate);
            }

            uint newCutOff = this.CalculateCutOff();
            IList<Base1_.Range<uint>> naks = new List<Base1_.Range<uint>>();
            uint covered;
            this.GenerateNaks(newCutOff, naks, out covered);

            this.PartitionPulls(naks, covered);

            maximumSeenLastRound = ackCollection.MaximumSeen;
        }

        #endregion

        #region Case 2

// ***(Case 2)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            out ReceiverIntraPartitionToken outgoingToken)
        {
#if DEBUG_ReceivingAgent1
            log("___Case_2", StatusString());
#endif

            outgoingToken = new ReceiverIntraPartitionToken();
            uint partitionCutOff;

            outgoingToken.MaximumSeen = ackCollection.MaximumSeen;
            outgoingToken.PartitionMaximumSeen = partitionAckCollection.MaximumSeen;

            outgoingToken.CutOff = this.CalculateCutOff();

            outgoingToken.TableOfCutOffs = tableOfMaxContiguous;
            outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

            outgoingToken.PartitionCutOff = partitionCutOff = this.UpperBound_Region2Partition(outgoingToken.CutOff);
            outgoingToken.PartitionCleanup = partition_maximumToClean;
            outgoingToken.PartitionMaxContiguous = partitionAckCollection.MaxContiguous;
            outgoingToken.MaxContiguous = ackCollection.MaxContiguous;
            outgoingToken.MaximumToClean = maximumToClean;

            maximumSeenLastRound = outgoingToken.MaximumSeen;

            uint partitionCovered;
            this.GenerateNaks(partitionCutOff, outgoingToken.PartitionNaks, out partitionCovered);
            outgoingToken.PartitionCovered = partitionCovered;

            outgoingToken.PartitionAggregatedCovered = outgoingToken.PartitionCovered;
            foreach (Base1_.Range<uint> nak in outgoingToken.PartitionNaks)
                outgoingToken.PartitionAggregatedNaks.Add(nak);

            RemotePulls(outgoingToken.CutOff, outgoingToken.TableOfCutOffs);

            double myrate = GetReceiveRate();
            outgoingToken.MinimumReceiveRate = myrate;
            outgoingToken.MaximumReceiveRate = myrate;
            outgoingToken.CumulatedReceiveRate = myrate;
            outgoingToken.MemberCount = 1;
        }

        #endregion

        #region Case 3

// ***(Case 3)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            out ReceiverInterPartitionToken outgoingToken)
        {
#if DEBUG_ReceivingAgent1
            log("___Case_3", StatusString());
#endif

            throw new Exception("The method or operation is not implemented.");

/*
            // ...................................................................................................................................................... 
 
*/ 
        }

        #endregion

        #region Case 4

// ***(Case 4)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, ReceiverIntraPartitionToken incomingToken)
        {
#if DEBUG_ReceivingAgent1
            log("___Case_4", StatusString());
#endif

            if (incomingToken != null)
            {
                UpdateTableOfMaxContiguous(incomingToken.TableOfMaxContiguous);
                if (incomingToken.PartitionMaxContiguous > tableOfMaxContiguous[receiverContext.PartitionNumber])
                    tableOfMaxContiguous[receiverContext.PartitionNumber] = incomingToken.PartitionMaxContiguous;

                if (incomingToken.MaximumToClean > maximumToClean)
                {
                    maximumToClean = partition_maximumToClean = incomingToken.MaximumToClean;

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }

                if (incomingToken.PartitionMaxContiguous > partition_maximumStable)
                    partition_maximumStable = incomingToken.PartitionMaxContiguous;

                ackCollection.Seen(incomingToken.MaximumSeen);

                uint partitionCovered;
                IList<Base1_.Range<uint>> localNaks = new List<Base1_.Range<uint>>();
                this.GenerateNaks(incomingToken.PartitionCutOff, localNaks, out partitionCovered);

                IList<Base1_.Range<uint>> forward1to2 = new List<Base1_.Range<uint>>(), forward2to1 = new List<Base1_.Range<uint>>();

                Receivers4.AckHelpers.CompareNaks(
                    incomingToken.PartitionCovered, incomingToken.PartitionNaks, partitionCovered, localNaks, forward1to2, forward2to1);

                PushAndPull(incomingAddress, forward1to2, forward2to1);

                if (incomingToken.MaxContiguous > 0)
                {
                    if (incomingToken.MaxContiguous > maximumToClean)
                    {
                        maximumToClean = partition_maximumToClean = incomingToken.MaxContiguous;

#if OPTION_UseForwardingSinks
                        forwarder.Cancel(partition_maximumToClean);
#endif
                        messageRepository.CleanUp(partition_maximumToClean);
                    }

#if DEBUG_ReceivingAgent1
                    log("Communicate maxContiguous = " +
                        incomingToken.MaxContiguous.ToString() + " to sender " + senderAddress.ToString(), "");
#endif

                    double minimumRate = incomingToken.MinimumReceiveRate;
                    double averageRate = incomingToken.CumulatedReceiveRate / ((double)incomingToken.MemberCount);
                    double maximumRate = incomingToken.MaximumReceiveRate;

#if DEBUG_CollectStatistics
                    if (timeSeries_combinedMinimumReceiveRates.Enabled)
                        timeSeries_combinedMinimumReceiveRates.Add(clock.Time, minimumRate);
                    if (timeSeries_combinedAverageReceiveRates.Enabled)
                        timeSeries_combinedAverageReceiveRates.Add(clock.Time, averageRate);
                    if (timeSeries_combinedMaximumReceiveRates.Enabled)
                        timeSeries_combinedMaximumReceiveRates.Add(clock.Time, maximumRate);
#endif
                    AcknowledgeToSender(incomingToken.MaxContiguous, null, minimumRate, averageRate, maximumRate);
                }
            }

            maximumSeenLastRound = ackCollection.MaximumSeen;

            if (incomingToken != null)
            {
                PartitionPulls(incomingToken.PartitionAggregatedNaks, incomingToken.PartitionAggregatedCovered);
            }
        }

        #region Old Junk

/*
                    if (incomingToken != null)
                    {
                        if (incomingToken.PartitionMaxContiguous > tableOfMaxContiguous[receiverContext.PartitionNumber])
                            tableOfMaxContiguous[receiverContext.PartitionNumber] = incomingToken.PartitionMaxContiguous;
                        outgoingToken.TableOfCutOffs = incomingToken.TableOfCutOffs;
                        outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

                        if (incomingToken.MaximumToClean > maximumToClean)
                        {
                            maximumToClean = incomingToken.MaximumToClean;
                            partition_maximumToClean = this.UpperBound_Region2Partition(maximumToClean);

#if OPTION_UseForwardingSinks
                        forwarder.Canceled(partition_maximumToClean);
#endif
                            messageRepository.CleanUp(partition_maximumToClean);
                        }

                        if (incomingToken.PartitionCleanup > partition_maximumToClean)
                        {
                            partition_maximumToClean = incomingToken.PartitionCleanup;

#if OPTION_UseForwardingSinks
                        forwarder.Canceled(partition_maximumToClean);
#endif
                            messageRepository.CleanUp(partition_maximumToClean);
                        }

                        if (incomingToken.PartitionMaxContiguous > partition_maximumStable)
                            partition_maximumStable = incomingToken.PartitionMaxContiguous;

                        ackCollection.Seen(incomingToken.MaximumSeen);
                        partitionAckCollection.Seen(incomingToken.PartitionMaximumSeen);
                        outgoingToken.CutOff = incomingToken.CutOff;

                        uint partitionCovered;
                        IList<Base.Range<uint>> localNaks = new List<Base.Range<uint>>();
                        this.GenerateNaks(incomingToken.PartitionCutOff, localNaks, out partitionCovered);

                        IList<Base.Range<uint>> forward1to2 = new List<Base.Range<uint>>(), forward2to1 = new List<Base.Range<uint>>();

                        Receivers4.AckHelpers.CompareNaks(
                            incomingToken.PartitionCovered, incomingToken.PartitionNaks, partitionCovered, localNaks, forward1to2, forward2to1);

                        PushAndPull(incomingAddress, forward1to2, forward2to1);

                        outgoingToken.MaxContiguous = incomingToken.MaxContiguous;
                    }
                    else
                    {
                        outgoingToken.CutOff = this.CalculateCutOff();
                        outgoingToken.MaxContiguous = 0;
                        outgoingToken.TableOfCutOffs = tableOfMaxContiguous;
                        outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;
                    }
                    outgoingToken.MaximumSeen = ackCollection.MaximumSeen;
                    outgoingToken.MaximumToClean = maximumToClean;

                    maximumSeenLastRound = outgoingToken.MaximumSeen;

                    if (incomingToken != null)
                    {
                        PartitionPulls(incomingToken.PartitionAggregatedNaks);
                    }
*/

        #endregion

        #endregion

        #region Case 5

// ***(Case 5)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, ReceiverIntraPartitionToken incomingToken, 
            out ReceiverIntraPartitionToken outgoingToken)
        {
            // intra token may be null in case this is not the first node in the partition, sender discovered while forwarding in progress

#if DEBUG_ReceivingAgent1
            log("___Case_5", StatusString());
#endif

            outgoingToken = new ReceiverIntraPartitionToken();

            double myrate = GetReceiveRate();
            double minimumRate = myrate, cumulatedRate = myrate, maximumRate = myrate;
            int membercount = 1;

            if (incomingToken != null)
            {
                minimumRate = Math.Min(minimumRate, incomingToken.MinimumReceiveRate);
                maximumRate = Math.Max(maximumRate, incomingToken.MaximumReceiveRate);
                cumulatedRate = cumulatedRate + incomingToken.CumulatedReceiveRate;
                membercount = membercount + incomingToken.MemberCount;

#if DEBUG_CollectStatistics
                double time_now = clock.Time;
                timeSeries_predecessorsMaxContiguous.Add(time_now, incomingToken.MaxContiguous);
                timeSeries_predecessorsMaxSeen.Add(time_now, incomingToken.MaximumSeen);
                timeSeries_localMaxSeen.Add(time_now, ackCollection.MaximumSeen);
                timeSeries_localMaxContiguous.Add(time_now, ackCollection.MaxContiguous);
                timeSeries_cutoff.Add(time_now, incomingToken.CutOff);
                timeSeries_maximumToClean.Add(time_now, incomingToken.MaximumToClean);
#endif

                UpdateTableOfMaxContiguous(incomingToken.TableOfMaxContiguous);
                outgoingToken.TableOfCutOffs = incomingToken.TableOfCutOffs;
                outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

                if (incomingToken.PartitionCleanup > partition_maximumToClean)
                {
                    partition_maximumToClean = incomingToken.PartitionCleanup;

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }

                ackCollection.Seen(incomingToken.MaximumSeen);
                partitionAckCollection.Seen(incomingToken.PartitionMaximumSeen);
                outgoingToken.CutOff = incomingToken.CutOff;
                outgoingToken.PartitionCutOff = incomingToken.PartitionCutOff;
                outgoingToken.PartitionMaxContiguous = partitionAckCollection.MaxContiguous;
                if (incomingToken.PartitionMaxContiguous < outgoingToken.PartitionMaxContiguous)
                    outgoingToken.PartitionMaxContiguous = incomingToken.PartitionMaxContiguous;
                outgoingToken.MaxContiguous = ackCollection.MaxContiguous;
                if (incomingToken.MaxContiguous < outgoingToken.MaxContiguous)
                    outgoingToken.MaxContiguous = incomingToken.MaxContiguous;

                if (incomingToken.MaximumToClean > maximumToClean)
                {
                    maximumToClean = incomingToken.MaximumToClean;
                    partition_maximumToClean = this.UpperBound_Region2Partition(maximumToClean);

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }
            }
            else
            {
                outgoingToken.TableOfCutOffs = tableOfMaxContiguous;
                outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

                outgoingToken.CutOff = this.CalculateCutOff();
                outgoingToken.PartitionCutOff = this.UpperBound_Region2Partition(outgoingToken.CutOff);
                outgoingToken.PartitionMaxContiguous = 0;
                outgoingToken.MaxContiguous = 0;
            }
            outgoingToken.MaximumSeen = ackCollection.MaximumSeen;
            outgoingToken.PartitionMaximumSeen = partitionAckCollection.MaximumSeen;
            outgoingToken.PartitionCleanup = partition_maximumToClean;
            outgoingToken.MaximumToClean = maximumToClean;

            maximumSeenLastRound = ackCollection.MaximumSeen;

            uint partitionCovered;
            this.GenerateNaks(outgoingToken.PartitionCutOff, outgoingToken.PartitionNaks, out partitionCovered);
            outgoingToken.PartitionCovered = partitionCovered;

            if (incomingToken != null)
            {
                uint partitionAggregatedCovered;
                IntersectNaks(incomingToken.PartitionAggregatedNaks, incomingToken.PartitionAggregatedCovered, 
                    partitionAckCollection.Missing,
                    partitionAckCollection.MaximumSeen, outgoingToken.PartitionAggregatedNaks, out partitionAggregatedCovered,
                    receiverConfiguration.MaximumNakRangesPerToken);
                outgoingToken.PartitionAggregatedCovered = partitionAggregatedCovered;
            }
            else
            {
                outgoingToken.PartitionAggregatedCovered = outgoingToken.PartitionCovered;
                foreach (Base1_.Range<uint> nak in outgoingToken.PartitionNaks)
                    outgoingToken.PartitionAggregatedNaks.Add(nak);
            }

            if (incomingToken != null)
            {
                IList<Base1_.Range<uint>> forward1to2 = new List<Base1_.Range<uint>>(), forward2to1 = new List<Base1_.Range<uint>>();

                Receivers4.AckHelpers.CompareNaks(incomingToken.PartitionCovered, incomingToken.PartitionNaks,
                    outgoingToken.PartitionCovered, outgoingToken.PartitionNaks, forward1to2, forward2to1);

                PushAndPull(incomingAddress, forward1to2, forward2to1);
            }

            RemotePulls(outgoingToken.CutOff, outgoingToken.TableOfCutOffs);

            outgoingToken.MinimumReceiveRate = minimumRate;
            outgoingToken.CumulatedReceiveRate = cumulatedRate;
            outgoingToken.MemberCount = membercount;
            outgoingToken.MaximumReceiveRate = maximumRate;
        }

        #endregion

        #region Case 6

// ***(Case 6)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, ReceiverIntraPartitionToken incomingToken, 
            out ReceiverInterPartitionToken outgoingToken)
        {
            // intra token may be null if only at the partition colection time we discover the sender

#if DEBUG_ReceivingAgent1
            log("___Case_6", StatusString());
#endif

            outgoingToken = new ReceiverInterPartitionToken();

            double myrate = GetReceiveRate();
            double minimumRate = myrate, cumulatedRate = myrate, maximumRate = myrate;
            int membercount = 1;

            if (incomingToken != null)
            {
                minimumRate = Math.Min(minimumRate, incomingToken.MinimumReceiveRate);
                maximumRate = Math.Max(maximumRate, incomingToken.MaximumReceiveRate);
                cumulatedRate = cumulatedRate + incomingToken.CumulatedReceiveRate;
                membercount = membercount + incomingToken.MemberCount;

                UpdateTableOfMaxContiguous(incomingToken.TableOfMaxContiguous);
                if (incomingToken.PartitionMaxContiguous > tableOfMaxContiguous[receiverContext.PartitionNumber])
                    tableOfMaxContiguous[receiverContext.PartitionNumber] = incomingToken.PartitionMaxContiguous;
                outgoingToken.TableOfCutOffs = incomingToken.TableOfCutOffs;
                outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

                if (incomingToken.MaximumToClean > maximumToClean)
                {
                    maximumToClean = incomingToken.MaximumToClean;
                    partition_maximumToClean = this.UpperBound_Region2Partition(maximumToClean);

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }

                if (incomingToken.PartitionCleanup > partition_maximumToClean)
                {
                    partition_maximumToClean = incomingToken.PartitionCleanup;

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }

                if (incomingToken.PartitionMaxContiguous > partition_maximumStable)
                    partition_maximumStable = incomingToken.PartitionMaxContiguous;

                ackCollection.Seen(incomingToken.MaximumSeen);
                partitionAckCollection.Seen(incomingToken.PartitionMaximumSeen);
                outgoingToken.CutOff = incomingToken.CutOff;

                uint partitionCovered;
                IList<Base1_.Range<uint>> localNaks = new List<Base1_.Range<uint>>();
                this.GenerateNaks(incomingToken.PartitionCutOff, localNaks, out partitionCovered);

                IList<Base1_.Range<uint>> forward1to2 = new List<Base1_.Range<uint>>(), forward2to1 = new List<Base1_.Range<uint>>();

                Receivers4.AckHelpers.CompareNaks(
                    incomingToken.PartitionCovered, incomingToken.PartitionNaks, partitionCovered, localNaks, forward1to2, forward2to1);

                PushAndPull(incomingAddress, forward1to2, forward2to1);

                outgoingToken.MaxContiguous = incomingToken.MaxContiguous;
            }
            else
            {
                outgoingToken.CutOff = this.CalculateCutOff();
                outgoingToken.MaxContiguous = 0;
                outgoingToken.TableOfCutOffs = tableOfMaxContiguous;
                outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;
            }
            outgoingToken.MaximumSeen = ackCollection.MaximumSeen;
            outgoingToken.MaximumToClean = maximumToClean;

            maximumSeenLastRound = outgoingToken.MaximumSeen;

            if (incomingToken != null)
            {
                PartitionPulls(incomingToken.PartitionAggregatedNaks, incomingToken.PartitionAggregatedCovered);
            }

            outgoingToken.MinimumReceiveRate = minimumRate;
            outgoingToken.CumulatedReceiveRate = cumulatedRate;
            outgoingToken.MemberCount = membercount;
            outgoingToken.MaximumReceiveRate = maximumRate;
        }

        #endregion

        #region Case 7

// ***(Case 7)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, ReceiverInterPartitionToken incomingToken)
        {
            // inter token may be null if only at the global collection time we discover the sender

#if DEBUG_ReceivingAgent1
            log("___Case_7", StatusString());
#endif

            if (incomingToken != null)
            {
                UpdateTableOfMaxContiguous(incomingToken.TableOfMaxContiguous);

                if (incomingToken.MaximumToClean > maximumToClean)
                {
                    maximumToClean = incomingToken.MaximumToClean;
                    partition_maximumToClean = this.UpperBound_Region2Partition(maximumToClean);

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }

                ackCollection.Seen(incomingToken.MaximumSeen);

                if (incomingToken.MaxContiguous > 0)
                {
                    if (incomingToken.MaxContiguous > maximumToClean)
                    {
                        maximumToClean = incomingToken.MaxContiguous;
                        partition_maximumToClean = this.UpperBound_Region2Partition(maximumToClean);

#if OPTION_UseForwardingSinks
                        forwarder.Cancel(partition_maximumToClean);
#endif
                        messageRepository.CleanUp(partition_maximumToClean);
                    }

#if DEBUG_ReceivingAgent1
                    log("Communicate maxContiguous = " +
                        incomingToken.MaxContiguous.ToString() + " to sender " + senderAddress.ToString(), "");
#endif

                    double minimumRate = incomingToken.MinimumReceiveRate;
                    double averageRate = incomingToken.CumulatedReceiveRate / ((double)incomingToken.MemberCount);
                    double maximumRate = incomingToken.MaximumReceiveRate;

#if DEBUG_CollectStatistics
                    if (timeSeries_combinedMinimumReceiveRates.Enabled)
                        timeSeries_combinedMinimumReceiveRates.Add(clock.Time, minimumRate);
                    if (timeSeries_combinedAverageReceiveRates.Enabled)
                        timeSeries_combinedAverageReceiveRates.Add(clock.Time, averageRate);
                    if (timeSeries_combinedMaximumReceiveRates.Enabled)
                        timeSeries_combinedMaximumReceiveRates.Add(clock.Time, maximumRate);
#endif
                    AcknowledgeToSender(incomingToken.MaxContiguous, null, minimumRate, averageRate, maximumRate);
                }
            }

            maximumSeenLastRound = ackCollection.MaximumSeen;
        }

        #endregion

        #region Case 8

// ***(Case 8)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, ReceiverInterPartitionToken incomingToken, 
            out ReceiverIntraPartitionToken outgoingToken)
        {
            // inter token may be null if we discover sender in a partition other than the first, while token forwarding is in progress

#if DEBUG_ReceivingAgent1
            log("___Case_8", StatusString());
#endif

            outgoingToken = new ReceiverIntraPartitionToken();
            uint partitionCutOff;

            double myrate = GetReceiveRate();
            double minimumRate = myrate, cumulatedRate = myrate, maximumRate = myrate;
            int membercount = 1;

            if (incomingToken != null)
            {
                minimumRate = Math.Min(minimumRate, incomingToken.MinimumReceiveRate);
                maximumRate = Math.Max(maximumRate, incomingToken.MaximumReceiveRate);
                cumulatedRate = cumulatedRate + incomingToken.CumulatedReceiveRate;
                membercount = membercount + incomingToken.MemberCount;

                UpdateTableOfMaxContiguous(incomingToken.TableOfMaxContiguous);
                outgoingToken.TableOfCutOffs = incomingToken.TableOfCutOffs;
                outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

                ackCollection.Seen(incomingToken.MaximumSeen);
                partitionAckCollection.Seen(this.UpperBound_Region2Partition(incomingToken.MaximumSeen));
                outgoingToken.CutOff = incomingToken.CutOff;
                outgoingToken.MaxContiguous = incomingToken.MaxContiguous;
                if (ackCollection.MaxContiguous < outgoingToken.MaxContiguous)
                    outgoingToken.MaxContiguous = ackCollection.MaxContiguous;

                if (incomingToken.MaximumToClean > maximumToClean)
                {
                    maximumToClean = incomingToken.MaximumToClean;
                    partition_maximumToClean = this.UpperBound_Region2Partition(maximumToClean);

#if OPTION_UseForwardingSinks
                    forwarder.Cancel(partition_maximumToClean);
#endif
                    messageRepository.CleanUp(partition_maximumToClean);
                }
            }
            else
            {
                outgoingToken.TableOfCutOffs = tableOfMaxContiguous;
                outgoingToken.TableOfMaxContiguous = tableOfMaxContiguous;

                outgoingToken.CutOff = this.CalculateCutOff();
                outgoingToken.MaxContiguous = 0;
            }

            outgoingToken.MaximumSeen = ackCollection.MaximumSeen;
            outgoingToken.PartitionMaximumSeen = partitionAckCollection.MaximumSeen;
            outgoingToken.PartitionCutOff = partitionCutOff = this.UpperBound_Region2Partition(outgoingToken.CutOff);
            outgoingToken.PartitionCleanup = partition_maximumToClean;
            outgoingToken.PartitionMaxContiguous = partitionAckCollection.MaxContiguous;
            outgoingToken.MaximumToClean = maximumToClean;

            maximumSeenLastRound = outgoingToken.MaximumSeen;

            uint partitionCovered;
            this.GenerateNaks(partitionCutOff, outgoingToken.PartitionNaks, out partitionCovered);
            outgoingToken.PartitionCovered = partitionCovered;

            outgoingToken.PartitionAggregatedCovered = outgoingToken.PartitionCovered;
            foreach (Base1_.Range<uint> nak in outgoingToken.PartitionNaks)
                outgoingToken.PartitionAggregatedNaks.Add(nak);

            RemotePulls(outgoingToken.CutOff, outgoingToken.TableOfCutOffs);

            outgoingToken.MinimumReceiveRate = minimumRate;
            outgoingToken.CumulatedReceiveRate = cumulatedRate;
            outgoingToken.MemberCount = membercount;
            outgoingToken.MaximumReceiveRate = maximumRate;
        }

        #endregion

        #region Case 9

// ***(Case 9)**********************************************************************************************************************************************

        void IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.Process(
            QS._core_c_.Base3.InstanceID incomingAddress, ReceiverInterPartitionToken incomingToken, 
            out ReceiverInterPartitionToken outgoingToken)
        {
#if DEBUG_ReceivingAgent1
            log("___Case_9", StatusString());
#endif

            throw new Exception("The method or operation is not implemented.");

/*
            // ......................................................................................................................................................  
 
*/ 
        }

// ************************************************************************************************************************************************************

        #endregion

#if OPTION_ProcessingCrashes

        #region IsActive

        bool IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>.IsActive
        {
            get { return maximumSeenLastRound > maximumToClean; }
        }

        #endregion

        #region Quiescence

        public void Quiesce()
        {
            if (!quiescence)
            {
                if (maximumSeenLastRound > maximumToClean)
                {
                    if (resumeCallback != null)
                        resumeCallback(this, null);
                }
                else
                    quiescence = true;                
            }
        }

        public void Resume()
        {
            quiescence = false;
        }

        public event EventHandler OnResume
        {
            add { resumeCallback += value; }
            remove { resumeCallback -= value; }
        }

        #endregion

#endif

        #endregion

        #region ProcessingCrashes

        public const bool ProcessingCrashes =
#if OPTION_ProcessingCrashes
            true
#else
            false
#endif
            ;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            disposed = true;
        }

        #endregion
    }
}
