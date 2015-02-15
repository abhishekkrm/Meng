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

#define DEBUG_CollectStatistics
// #define DEBUG_Quiescence

#define OPTION_ProcessingCrashes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings6
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Agent : QS.Fx.Inspection.Inspectable, Receivers4.IReceivingAgent, IReceiverContext, IAgentCoreContext, QS._core_c_.Diagnostics2.IModule, IDisposable
    {
        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Constructor

        public Agent(Receivers4.IReceivingAgentContext context, IAgentContext agentContext,
            IAgentConfiguration agentConfiguration, IReceiverConfiguration receiverConfiguration, QS._core_c_.Statistics.IStatisticsController statisticsController)
        {
            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("Receivers", diagnosticsContainerForReceivers);

            receivingRateCalculator = new FlowControl3.RateCalculator3(agentContext.Clock, TimeSpan.FromSeconds(1));

            this.statisticsController = statisticsController;
            this.context = context;
            this.agentContext = agentContext;
            this.agentConfiguration = agentConfiguration;
            this.receiverConfiguration = receiverConfiguration;

            _receiverCollection = new _ReceiverCollection(receiverCollection);

            context.Demultiplexer.register((uint)ReservedObjectID.Rings6_ReceivingAgent1_InterPartitionChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.InterPartitionTokenReceiveCallback));
            context.Demultiplexer.register((uint)ReservedObjectID.Rings6_ReceivingAgent1_IntraPartitionChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.IntraPartitionTokenReceiveCallback));
            context.Demultiplexer.register((uint)ReservedObjectID.Rings6_ReceivingAgent1_ForwardingChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.ForwardingReceiveCallback));
            context.Demultiplexer.register((uint)ReservedObjectID.Rings6_ReceivingAgent1_RequestingChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.RequestingReceiveCallback));

            numberOfNodes = (uint)context.ReceiverAddresses.Length;
            nodeNumber = (uint)Array.FindIndex<QS._core_c_.Base3.InstanceID>(context.ReceiverAddresses,
              new Predicate<QS._core_c_.Base3.InstanceID>(((IEquatable<QS._core_c_.Base3.InstanceID>) agentContext.LocalAddress).Equals));
            numberOfPartitions = (uint)Math.Floor(((double)numberOfNodes) / ((double) agentConfiguration.NumberOfReplicas));
            if (numberOfPartitions < 1)
                numberOfPartitions = 1;
            partitionNumber = nodeNumber % numberOfPartitions;
            positionInPartition = nodeNumber / numberOfPartitions;
            partitionSize = (uint)Math.Floor(1 + ((double)(numberOfNodes - 1 - partitionNumber)) / ((double)numberOfPartitions));
            partitionMembers = new QS._core_c_.Base3.InstanceID[partitionSize];
            for (uint ind = 0; ind < partitionSize; ind++)
                partitionMembers[ind] = context.ReceiverAddresses[partitionNumber + numberOfPartitions * ind];

#if OPTION_ProcessingCrashes
            partitionMembersCrashed = new bool[partitionSize];
            regionViewMembersCrashed = new bool[numberOfNodes];
#endif

            nextInPartition = context.ReceiverAddresses[
                partitionNumber + numberOfPartitions * ((positionInPartition + 1) % partitionSize)];
            // previousInPartition = context.ReceiverAddresses[
            //    partitionNumber + numberOfPartitions * ((positionInPartition + partitionSize - 1) % partitionSize)];
            nextAcrossPartitions = context.ReceiverAddresses[(partitionNumber + 1) % numberOfPartitions];
            isLeader = nodeNumber == 0;
            isPartitionLeader = positionInPartition == 0;

#if DEBUG_ReceivingAgent1
            StringBuilder ss = new StringBuilder();
            ss.AppendLine("numberOfNodes = " + numberOfNodes.ToString());
            ss.AppendLine("nodeNumber = " + nodeNumber.ToString());
            ss.AppendLine("numberOfPartitions = " + numberOfPartitions.ToString());
            ss.AppendLine("partitionNumber = " + partitionNumber.ToString());
            ss.AppendLine("positionInPartition = " + positionInPartition.ToString());
            ss.AppendLine("partitionSize = " + partitionSize.ToString());
            ss.AppendLine("partitionMembers = \n{\n  " + Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(partitionMembers, ",\n  ") + "\n}\n");
            ss.AppendLine("nextInPartition = " + nextInPartition.ToString());
            // log("previousInPartition = " + previousInPartition.ToString());
            ss.AppendLine("nextAcrossPartitions = " + nextAcrossPartitions.ToString());
            ss.AppendLine("isLeader = " + isLeader.ToString());
            log("Initializing the agent.", ss.ToString());
#endif

            intraPartitionSuccessorSender = agentContext.SenderCollection[nextInPartition];
            interPartitionSuccessorSender = agentContext.SenderCollection[nextAcrossPartitions]; ;

            agentCore = new AgentCore(this);

            if (isLeader)
            {
                if (agentConfiguration.TokenCirculationRate > 0)
                    tokenAlarm = agentContext.AlarmClock.Schedule(
                        1 / agentConfiguration.TokenCirculationRate, new QS.Fx.Clock.AlarmCallback(this.TokenCallback), null);
                else
                {
#if DEBUG_ReceivingAgent1
                        log("Token circulation rate set to ZERO, not scheduling token interval.", "");
#endif
                }
            }

            lock (this)
            {
                IList<QS._core_c_.Base3.InstanceID> failedAddresses;
                failureSubscription = context.FailureSource.subscribe(
                    new QS._qss_c_.Failure_.NotificationsCallback(this.FailureCallback), out failedAddresses);
                if (failedAddresses.Count > 0)
                    this.FailureCallback(failedAddresses);
            }

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);

            agentContext.Logger.Log(this, "__________Agent(" + context.ID.ToString() + ").Created { partition " + 
                partitionNumber.ToString() + " with members ( " + 
                QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(partitionMembers, ", ") + " ) }");
        }

        #endregion

        #region Fields

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsContainerForReceivers = new QS._core_c_.Diagnostics2.Container();

        private QS._core_c_.Statistics.IStatisticsController statisticsController;
        private Receivers4.IReceivingAgentContext context;
        private Failure_.ISubscription failureSubscription;
        private IAgentContext agentContext;
        private IAgentConfiguration agentConfiguration;
        private IReceiverConfiguration receiverConfiguration;

        private uint numberOfNodes, numberOfPartitions, nodeNumber, partitionNumber, positionInPartition, partitionSize;
        private QS._core_c_.Base3.InstanceID[] partitionMembers;

#if OPTION_ProcessingCrashes
        private bool[] regionViewMembersCrashed, partitionMembersCrashed;
#endif

        private QS._core_c_.Base3.InstanceID nextInPartition, nextAcrossPartitions;
        private bool isLeader, isPartitionLeader;
        private QS.Fx.Clock.IAlarm tokenAlarm;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<QS._core_c_.Base3.InstanceID, Receiver> receiverCollection = new Dictionary<QS._core_c_.Base3.InstanceID, Receiver>();
        private Base3_.IReliableSerializableSender intraPartitionSuccessorSender, interPartitionSuccessorSender;
        private _ReceiverCollection _receiverCollection;

        private double lastTokenTime;

        private IAgentCore agentCore;

        private FlowControl3.IRateCalculator receivingRateCalculator;
        private int numberOfOtherSenders;

        private bool disposed;

        #endregion

        #region Collecting statistics

#if DEBUG_CollectStatistics
        [QS._core_c_.Diagnostics.Component("Token Creation Times (X=time, Y=round)")]
        [QS._core_c_.Diagnostics2.Property("TokenCreationTimes")]
        private Statistics_.Samples2D timeSeries_tokenCreationTimes = new QS._qss_c_.Statistics_.Samples2D("Rings6.Agent.TokenCreationTimes");
        [QS._core_c_.Diagnostics.Component("INTRA-partition Token Receive Times (X=time, Y=round)")]
        [QS._core_c_.Diagnostics2.Property("IntraTokenReceiveTimes")]
        private Statistics_.Samples2D timeSeries_intraTokenReceiveTimes = new QS._qss_c_.Statistics_.Samples2D("Rings6.Agent.IntraTokenReceiveTimes");
        [QS._core_c_.Diagnostics.Component("INTER-partition Token Receive Times (X=time, Y=round)")]
        [QS._core_c_.Diagnostics2.Property("InterTokenReceiveTimes")]
        private Statistics_.Samples2D timeSeries_interTokenReceiveTimes = new QS._qss_c_.Statistics_.Samples2D("Rings6.Agent.InterTokenReceiveTimes");
        [QS._core_c_.Diagnostics.Component("Token Roundtrip Times (X=creation time, Y=roundtrip time)")]
        [QS._core_c_.Diagnostics2.Property("TokenRoundtripTimes")]
        private Statistics_.Samples2D timeSeries_tokenRoundtripTimes = new QS._qss_c_.Statistics_.Samples2D("Rings6.Agent.TokenRoundtripTimes");

        [QS._core_c_.Diagnostics.Component("Combined Token Statistics (X=time, Y=round)")]
        [QS._core_c_.Diagnostics2.Property("CombinedTokenStatistics")]
        private QS._core_e_.Data.DataCo TimeSeries_CombinedTokenStatistics
        {
            get
            {
                QS._core_e_.Data.DataCo dataCo = new QS._core_e_.Data.DataCo("Combined Token Statistics", "", "Time", "s", "", "Round Number", "", "");
                dataCo.Add(new QS._core_e_.Data.Data2D("creation", timeSeries_tokenCreationTimes.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("receive intra", timeSeries_intraTokenReceiveTimes.Samples));
                dataCo.Add(new QS._core_e_.Data.Data2D("receive inter", timeSeries_interTokenReceiveTimes.Samples));
                return dataCo;
            }
        }
#endif

        #endregion

        #region Failure Processing

        private void FailureCallback(IList<QS._core_c_.Base3.InstanceID> failedAddresses)
        {
            agentContext.Logger.Log(this, "__FailureCallback( " + this.context.ID + ") : crashed { " +
                QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(failedAddresses, ", ") + " }");

            if (disposed)
                agentContext.Logger.Log(this, "__FailureCallback( " + this.context.ID + ") : ignoring because this agent was disposed");
            else
            {

#if OPTION_ProcessingCrashes
                lock (this)
                {
                    agentContext.Logger.Log(this, "__FailureCallback : processing failure notifications currently not implemented");

                    foreach (QS._core_c_.Base3.InstanceID badaddress in failedAddresses)
                    {
                        uint badindex = (uint)Array.FindIndex<QS._core_c_.Base3.InstanceID>(context.ReceiverAddresses,
                            new Predicate<QS._core_c_.Base3.InstanceID>(((IEquatable<QS._core_c_.Base3.InstanceID>)badaddress).Equals));

                        if (!regionViewMembersCrashed[badindex])
                        {
                            agentContext.Logger.Log(this, "__FailureCallback : Crashed node { " + badindex.ToString() + " } in region.");
                            regionViewMembersCrashed[badindex] = true;

                            uint badpartno = badindex % numberOfPartitions;
                            uint badpartindex = badindex / numberOfPartitions;

                            if (badpartno == partitionNumber)
                            {
                                agentContext.Logger.Log(this, "__FailureCallback : Crashed node { " + badpartindex.ToString() + " } in partition.");
                                partitionMembersCrashed[badpartindex] = true;

                                if (nextInPartition.Equals(badaddress))
                                {
                                    int offset = 1;
                                    while (offset <= partitionSize && partitionMembersCrashed[((positionInPartition + offset) % partitionSize)])
                                        offset++;

                                    nextInPartition = context.ReceiverAddresses[
                                        partitionNumber + numberOfPartitions * ((positionInPartition + offset) % partitionSize)];
                                    intraPartitionSuccessorSender = agentContext.SenderCollection[nextInPartition];

                                    agentContext.Logger.Log(this, "__FailureCallback : New next in partition { " + nextInPartition.ToString() + " }.");
                                }
                            }

                            if (nextAcrossPartitions.Equals(badaddress))
                            {
                                bool found = false;
                                for (uint crosspartition_offset = 1; !found && crosspartition_offset <= numberOfPartitions; crosspartition_offset++)
                                {
                                    uint candidate_partno = (partitionNumber + crosspartition_offset) % numberOfPartitions;
                                    uint candidate_partsize =
                                        (uint)Math.Floor(1 + ((double)(numberOfNodes - 1 - candidate_partno)) / ((double)numberOfPartitions));
                                    for (uint candidate_partindex = 0; !found && candidate_partindex < candidate_partsize; candidate_partindex++)
                                    {
                                        uint candidate_index = candidate_partno + numberOfPartitions * candidate_partindex;
                                        if (!regionViewMembersCrashed[candidate_index])
                                        {
                                            found = true;
                                            nextAcrossPartitions = context.ReceiverAddresses[candidate_index];
                                        }
                                    }
                                }

                                if (!found)
                                {
                                    throw new Exception("Internal error: inconsistent data structures, could not decide which node is next across partitions.");
                                }

                                interPartitionSuccessorSender = agentContext.SenderCollection[nextAcrossPartitions];

                                agentContext.Logger.Log(this, "__FailureCallback : New next across partitions { " + nextAcrossPartitions.ToString() + " }.");
                            }
                        }
                    }

                    if (!isPartitionLeader)
                    {
                        bool become_leader = true;
                        for (int ind = 0; ind < positionInPartition; ind++)
                        {
                            if (!partitionMembersCrashed[ind])
                            {
                                become_leader = false;
                                break;
                            }
                        }

                        if (become_leader)
                        {
                            isPartitionLeader = true;
                            agentContext.Logger.Log(this, "__FailureCallback : Becoming partition leader.");
                        }
                    }

                    if (!isLeader)
                    {
                        bool become_leader = true;
                        for (int ind = 0; ind < nodeNumber; ind++)
                        {
                            if (!regionViewMembersCrashed[ind])
                            {
                                become_leader = false;
                                break;
                            }
                        }

                        if (become_leader)
                        {
                            isLeader = true;
                            agentContext.Logger.Log(this, "__FailureCallback : Becoming region leader.");

                            ResumeCallback(null, null);
                        }
                    }
                }
#endif

            }
        }

        #endregion

        #region logging

#if DEBUG_ReceivingAgent1
        private void log(string description, string details)
        {
            if (owner.eventLogger.Enabled)
                owner.eventLogger.Log(new MyEvent(owner.clock.Time, owner.localAddress, this, description, details));
        }
#endif

        #endregion

        #region Class _ReceiverCollection

        private class _ReceiverCollection : IEnumerable<KeyValuePair<QS._core_c_.Base3.InstanceID,
                    IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>>>
        {
            public _ReceiverCollection(IDictionary<QS._core_c_.Base3.InstanceID, Receiver> receiverCollection)
            {
                this.receiverCollection = receiverCollection;
            }

            private IDictionary<QS._core_c_.Base3.InstanceID, Receiver> receiverCollection;

            #region IEnumerable<KeyValuePair<InstanceID,IPartitionedTokenRingMember<ReceiverIntraPartitionToken,ReceiverInterPartitionToken>>> Members

            IEnumerator<KeyValuePair<QS._core_c_.Base3.InstanceID, 
                IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken>>> 
                    IEnumerable<KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<
                        ReceiverIntraPartitionToken, ReceiverInterPartitionToken>>>.GetEnumerator()
            {
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Receiver> element in receiverCollection)
                    yield return new KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<
                        ReceiverIntraPartitionToken, ReceiverInterPartitionToken>>(element.Key, element.Value);
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<
                        ReceiverIntraPartitionToken, ReceiverInterPartitionToken>>>)this).GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "Agent(" + context.ID.ToString() + ")";
        }

        #endregion

        #region Token Receive Callbacks

        private QS.Fx.Serialization.ISerializable InterPartitionTokenReceiveCallback(
            QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            lock (this)
            {
                AgentInterPartitionToken token = receivedObject as AgentInterPartitionToken;
                if (token == null)
                    throw new Exception("Received an object of incompatible type.");
                else
                {
#if DEBUG_CollectStatistics
                        if (timeSeries_interTokenReceiveTimes.Enabled)
                            timeSeries_interTokenReceiveTimes.Add(agentContext.Clock.Time, (int)token.Round);
#endif
                }

#if DEBUG_ReceivingAgent1
                log("Received INTER from " + sourceAddress.Address.ToString(), QS.Fx.Printing.Printable.ToString(token));
#endif

                this.ProcessInterPartitionToken(sourceAddress, token);
            }

            return null;
        }

        private QS.Fx.Serialization.ISerializable IntraPartitionTokenReceiveCallback(
            QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            lock (this)
            {
                AgentIntraPartitionToken token = receivedObject as AgentIntraPartitionToken;
                if (token == null)
                    throw new Exception("Received an object of incompatible type.");
                else
                {
#if DEBUG_CollectStatistics
                        if (timeSeries_intraTokenReceiveTimes.Enabled)
                            timeSeries_intraTokenReceiveTimes.Add(agentContext.Clock.Time, (int)token.Round);
#endif
                }

#if DEBUG_ReceivingAgent1
                log("Received INTRA from " + sourceAddress.Address.ToString(), QS.Fx.Printing.Printable.ToString(token));
#endif

                this.ProcessIntraPartitionToken(sourceAddress, token);
            }

            return null;
        }

        #endregion

        #region ForwardingReceiveCallback

        private QS.Fx.Serialization.ISerializable ForwardingReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            ForwardingRequest forwardingRequest = receivedObject as ForwardingRequest;
            if (forwardingRequest != null)
            {
                Receiver receiver;
                lock (this)
                {
                    receiver = this.GetReceiver(forwardingRequest.SenderAddress);
                    if (receiver != null)
                        receiver.ProcessForwardingRequest(sourceAddress, forwardingRequest);
                }
            }

            return null;
        }

        #endregion

        #region RequestingReceiveCallback

        private QS.Fx.Serialization.ISerializable RequestingReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            PartitionAcknowledgement pullRequest = receivedObject as PartitionAcknowledgement;
            if (pullRequest != null)
            {
                Receiver receiver;
                lock (this)
                {
                    receiver = this.GetReceiver(pullRequest.SenderAddress);
                    if (receiver != null)
                        receiver.ProcessPullRequest(sourceAddress, pullRequest);
                }
            }

            return null;
        }

        #endregion

        #region ProcessIntraPartitionToken

        private void ProcessIntraPartitionToken(QS._core_c_.Base3.InstanceID senderAddress, AgentIntraPartitionToken token)
        {
            lock (this)
            {
                uint creatorIndex = (uint)Array.FindIndex<QS._core_c_.Base3.InstanceID>(partitionMembers,
                  new Predicate<QS._core_c_.Base3.InstanceID>(((IEquatable<QS._core_c_.Base3.InstanceID>)token.PartitionCreatorAddress).Equals));
                uint senderIndex = (uint)Array.FindIndex<QS._core_c_.Base3.InstanceID>(partitionMembers,
                  new Predicate<QS._core_c_.Base3.InstanceID>(((IEquatable<QS._core_c_.Base3.InstanceID>)senderAddress).Equals));

                uint sender_distance = (senderIndex + partitionSize - creatorIndex) % partitionSize;
                uint current_distance = (positionInPartition + partitionSize - creatorIndex) % partitionSize;

                bool finalize_token = current_distance < sender_distance; // we must have gone a full round around and past the creator
                if (finalize_token)
                {
                    if (numberOfPartitions == 1)
                    {
                        agentCore.Process(senderAddress, token);

#if OPTION_ProcessingCrashes
                        if (agentCore.IsActive)
                        {
#endif
                            
                            RecreateToken();

#if OPTION_ProcessingCrashes
                        }
#endif

                        }
                    else
                    {
                        AgentInterPartitionToken forwardedToken;
                        agentCore.Process(senderAddress, token, out forwardedToken);

                        QS._core_c_.Base3.Message wrapper = context.Message(
                            new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_InterPartitionChannel, forwardedToken),
                            Receivers4.DestinationType.Receiver);
                        interPartitionSuccessorSender.BeginSend(wrapper.destinationLOID, wrapper.transmittedObject, null, null);

#if DEBUG_ReceivingAgent1
                    log("Sending to " + interPartitionSuccessorSender.Address.ToString(),
                        QS.Fx.Printing.Printable.ToString(forwardedToken));
#endif
                    }
                }
                else
                {
                    AgentIntraPartitionToken forwardedToken;
                    agentCore.Process(senderAddress, token, out forwardedToken);

                    QS._core_c_.Base3.Message wrapper = context.Message(
                        new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_IntraPartitionChannel, forwardedToken),
                        Receivers4.DestinationType.Receiver);
                    intraPartitionSuccessorSender.BeginSend(wrapper.destinationLOID, wrapper.transmittedObject, null, null);

#if DEBUG_ReceivingAgent1
                log("Sending to " + intraPartitionSuccessorSender.Address.ToString(),
                    QS.Fx.Printing.Printable.ToString(forwardedToken));
#endif
                }
            }
        }

        #endregion

        #region ProcessInterPartitionToken

        private void ProcessInterPartitionToken(QS._core_c_.Base3.InstanceID senderAddress, AgentInterPartitionToken token)
        {
            lock (this)
            {
                if (isLeader)
                {
                    agentCore.Process(senderAddress, token);

#if OPTION_ProcessingCrashes
                    if (agentCore.IsActive)
                    {
#endif

                        RecreateToken();

#if OPTION_ProcessingCrashes
                    }
#endif

                }
                else
                {
                    if (partitionSize == 1)
                    {
                        AgentInterPartitionToken forwardedToken;
                        agentCore.Process(senderAddress, token, out forwardedToken);

                        QS._core_c_.Base3.Message wrapper = context.Message(
                            new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_InterPartitionChannel, forwardedToken),
                            Receivers4.DestinationType.Receiver);
                        interPartitionSuccessorSender.BeginSend(wrapper.destinationLOID, wrapper.transmittedObject, null, null);

#if DEBUG_ReceivingAgent1
                    log("Sending to " + interPartitionSuccessorSender.Address.ToString(),
                        QS.Fx.Printing.Printable.ToString(forwardedToken));
#endif
                    }
                    else
                    {
                        AgentIntraPartitionToken forwardedToken;
                        agentCore.Process(senderAddress, token, out forwardedToken);

                        QS._core_c_.Base3.Message wrapper = context.Message(
                            new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_IntraPartitionChannel, forwardedToken),
                            Receivers4.DestinationType.Receiver);
                        intraPartitionSuccessorSender.BeginSend(wrapper.destinationLOID, wrapper.transmittedObject, null, null);

#if DEBUG_ReceivingAgent1
                    log("Sending to " + intraPartitionSuccessorSender.Address.ToString(),
                        QS.Fx.Printing.Printable.ToString(forwardedToken));
#endif
                    }
                }
            }
        }

        #endregion

        #region RecreateToken

        private void RecreateToken()
        {
            if (tokenAlarm != null)
                tokenAlarm.Cancel();

            double desired_interval = 1 / agentConfiguration.TokenCirculationRate;
            double minimum_interval = 0.5 * desired_interval;
            double roundtrip_time = agentContext.Clock.Time - lastTokenTime;

#if DEBUG_CollectStatistics
            timeSeries_tokenRoundtripTimes.Add(lastTokenTime, roundtrip_time);
#endif

            double new_timeout = desired_interval - roundtrip_time;

            if (new_timeout < minimum_interval)
                new_timeout = minimum_interval;

            if (new_timeout <= 0)
                TokenCallback(tokenAlarm);
            else
                tokenAlarm = agentContext.AlarmClock.Schedule(
                    new_timeout, new QS.Fx.Clock.AlarmCallback(this.TokenCallback), null);
        }

        #endregion

        #region TokenCallback

        private void TokenCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
#if OPTION_ProcessingCrashes
            if (agentCore.IsActive)
            {
#endif
                bool already_rescheduled = false;
                lock (this)
                {
                    lastTokenTime = agentContext.Clock.Time;

                    try
                    {
                        if (nextInPartition.Equals(agentContext.LocalAddress))
                        {
                            if (nextAcrossPartitions.Equals(agentContext.LocalAddress))
                            {
                                agentCore.Process();

#if OPTION_ProcessingCrashes
                                if (agentCore.IsActive)
                                {
#endif

                                    RecreateToken();
                                    already_rescheduled = true;

#if OPTION_ProcessingCrashes
                                }
#endif

                            }
                            else
                            {
                                AgentInterPartitionToken token;
                                agentCore.Process(out token);

                                QS._core_c_.Base3.Message wrapper = context.Message(
                                    new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_InterPartitionChannel, token),
                                    Receivers4.DestinationType.Receiver);
                                interPartitionSuccessorSender.BeginSend(wrapper.destinationLOID, wrapper.transmittedObject, null, null);

#if DEBUG_ReceivingAgent1
                            log("Sending to " + interPartitionSuccessorSender.Address.ToString(),
                                QS.Fx.Printing.Printable.ToString(forwardedToken));
#endif
                            }
                        }
                        else
                        {
                            AgentIntraPartitionToken token;
                            agentCore.Process(out token);

#if DEBUG_CollectStatistics
                            if (timeSeries_tokenCreationTimes.Enabled)
                                timeSeries_tokenCreationTimes.Add(agentContext.Clock.Time, (int)token.Round);
#endif

                            QS._core_c_.Base3.Message wrapper = context.Message(
                                new QS._core_c_.Base3.Message((uint)ReservedObjectID.Rings6_ReceivingAgent1_IntraPartitionChannel, token),
                                Receivers4.DestinationType.Receiver);
                            intraPartitionSuccessorSender.BeginSend(wrapper.destinationLOID, wrapper.transmittedObject, null, null);

#if DEBUG_ReceivingAgent1
                            log("Sending to " + intraPartitionSuccessorSender.Address.ToString(),
                                QS.Fx.Printing.Printable.ToString(token));
#endif
                        }
                    }
                    finally
                    {
                        if (!already_rescheduled 
#if OPTION_ProcessingCrashes
                            && agentCore.IsActive
#endif
                            )
                        {
                            this.tokenAlarm = agentContext.AlarmClock.Schedule(
                                1 / agentConfiguration.MinimumTokenCirculationRate, new QS.Fx.Clock.AlarmCallback(this.TokenCallback), null);
                        }
                    }
                }
#if OPTION_ProcessingCrashes
            }
            else
            {
#if DEBUG_Quiescence
                agentContext.Logger.Log(this, "_____TokenCallback[" + context.ID + "] : Not Processing ( AgentCore.IsActive = false )");
#endif
            }
#endif
        }

        #endregion

        #region IReceivingAgent Members

        void QS._qss_c_.Receivers4.IReceivingAgent.Shutdown()
        {
            agentContext.Logger.Log(this, "__________Agent(" + context.ID.ToString() + ").Shutdown : Not Implemented Properly");

            lock (this)
            {
#if DEBUG_ReceivingAgent1
                    log("Shutdown.", "");
#endif

                // Hack.Start

                if ((tokenAlarm != null) && !tokenAlarm.Cancelled)
                    tokenAlarm.Cancel();                
                
                if ((agentCore != null) && (agentCore is IDisposable))
                    ((IDisposable) agentCore).Dispose();                
                agentCore = null;

                if (receiverCollection != null)
                {
                    foreach (Receiver receiver in receiverCollection.Values)
                    {
                        if (receiver is IDisposable)
                            ((IDisposable)receiver).Dispose();
                    }
                    receiverCollection.Clear();
                }

                // private Failure.ISubscription failureSubscription;        
                // context.FailureSource.unsubscribe ????

                // Hack.Stop

#if OPTION_ProcessingCrashes
#else
                if (tokenAlarm != null)
                    tokenAlarm.cancel();
                tokenAlarm = null;
#endif
            }
        }

        void QS._qss_c_.Receivers4.IReceivingAgent.Receive(
            QS._core_c_.Base3.InstanceID sourceAddress, uint sequenceNo, QS._core_c_.Base3.Message message, bool retransmission, bool forwarding)
        {
            Receiver receiver;
            lock (this)            
            {
                if (disposed)
                    agentContext.Logger.Log("region " + context.ID.ToString() + " cannot receive because it is disposed");
                {
                    receiver = this.GetReceiver(sourceAddress);
                    receiver.Receive(sequenceNo, message, retransmission, forwarding);
                }
            }
        }

        #endregion

        #region GetReceiver

        private Receiver GetReceiver(QS._core_c_.Base3.InstanceID senderAddress)
        {
            Receiver receiver;

            if (receiverCollection.ContainsKey(senderAddress))
                receiver = receiverCollection[senderAddress];
            else
            {
                receiverCollection.Add(senderAddress, receiver = new Receiver(senderAddress, this, receiverConfiguration, statisticsController));
                if (!senderAddress.Equals(agentContext.LocalAddress))
                    numberOfOtherSenders++;

#if OPTION_ProcessingCrashes
                receiver.OnResume += new EventHandler(this.ResumeCallback);
                ResumeCallback(receiver, null);
#endif

                ((QS._core_c_.Diagnostics2.IContainer) diagnosticsContainerForReceivers).Register(
                    senderAddress.ToString(), ((QS._core_c_.Diagnostics2.IModule) receiver).Component);
            }

            return receiver;
        }

        #endregion

        #region ResumeCallback

#if OPTION_ProcessingCrashes
        private void ResumeCallback(object sender, EventArgs e)
        {
            Receiver receiver = (Receiver) sender;

#if DEBUG_Quiescence
            if (receiver != null)
                agentContext.Logger.Log(this, "__________RESUME[" + context.ID + "]__________( " + receiver.ToString() + " )__________");
#endif

            agentCore.Resume();

            if (isLeader)
            {
                RecreateToken();
            }
            else
            {
                // TODO: We should notify the region leader or pass an empty token to resume in case the leader does not get these messages.
            }
        }
#endif

        #endregion

        #region IReceiverContext Members

        QS.Fx.Logging.ILogger IReceiverContext.Logger
        {
            get { return agentContext.Logger; }
        }

        QS.Fx.Logging.IEventLogger IReceiverContext.EventLogger
        {
            get { return agentContext.EventLogger; }
        }

        QS._core_c_.Base3.InstanceID IReceiverContext.LocalAddress
        {
            get { return agentContext.LocalAddress; }
        }

        string IReceiverContext.Name
        {
            get { return context.ID.ToString(); }
        }

        uint IReceiverContext.NumberOfPartitions
        {
            get { return numberOfPartitions; }
        }

        uint IReceiverContext.PartitionNumber
        {
            get { return partitionNumber; }
        }

        QS._qss_c_.Base3_.IDemultiplexer IReceiverContext.Demultiplexer
        {
            get { return agentContext.Demultiplexer; }
        }

        QS.Fx.Clock.IClock IReceiverContext.Clock
        {
            get { return agentContext.Clock; }
        }

        QS._core_c_.Base3.InstanceID[] IReceiverContext.ReceiverAddresses
        {
            get { return context.ReceiverAddresses; }
        }

        QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.IReliableSerializableSender> IReceiverContext.SenderCollection
        {
            get { return agentContext.SenderCollection; }
        }

        Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> IReceiverContext.SinkCollection
        {
            get { return agentContext.SinkCollection; }
        }

        Receivers4.IReceivingAgentContext IReceiverContext.AgentContext
        {
            get { return context; }
        }

        void IReceiverContext.RateSample()
        {
            receivingRateCalculator.sample();
        }

        double IReceiverContext.ReceiveRate
        {
            get { return receivingRateCalculator.Rate / ((numberOfOtherSenders > 0) ? numberOfOtherSenders : 1); }
        }

/*
        int IReceiverContext.NumberOfOtherSenders
        {
            get 
            {
                int count = 0;
                foreach (QS._core_c_.Base3.InstanceID address in receiverCollection.Keys)
                {
                    if (!address.Equals(agentContext.LocalAddress))
                        count++;
                }

                return count;
            }
        }
*/

        #endregion

        #region IAgentCoreContext Members

        string IAgentCoreContext.ID
        {
            get { return context.ID.ToString(); }
        }

        QS.Fx.Logging.ILogger IAgentCoreContext.Logger
        {
            get { return agentContext.Logger; }
        }

        QS._core_c_.Base3.InstanceID IAgentCoreContext.LocalAddress
        {
            get { return agentContext.LocalAddress; }
        }

        IEnumerable<KeyValuePair<QS._core_c_.Base3.InstanceID, IPartitionedTokenRingMember<ReceiverIntraPartitionToken,
            ReceiverInterPartitionToken>>> IAgentCoreContext.ReceiverCollection
        {
            get 
            {                 
                return _receiverCollection; 
            }
        }

        IPartitionedTokenRingMember<ReceiverIntraPartitionToken, ReceiverInterPartitionToken> 
            IAgentCoreContext.ReceiverAt(QS._core_c_.Base3.InstanceID address)
        {
            return GetReceiver(address);
        }

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
