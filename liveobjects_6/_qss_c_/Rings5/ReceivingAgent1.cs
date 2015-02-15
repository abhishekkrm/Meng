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

#define DEBUG_ReceivingAgent1

using System;
using System.Collections.Generic;
using System.Text;   

namespace QS._qss_c_.Rings5 
{
    /// <summary>
    /// This class represents a member of a group of receiving nodes. Normally this group will represent
    /// a region view, get created when the region view is created and destroyed when all messages sent 
    /// in the given region view are delivered and acknowledged. It could also be mapped to group view. 
    /// </summary>
    public class ReceivingAgent1 : Receivers4.IReceivingAgentClass
    {
        public ReceivingAgent1(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IAlarmClock alarmClock, 
            QS.Fx.Clock.IClock clock, uint replicationCoefficient)

            // IReceiverClass receiverClass, double tokenRate)
        {
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.localAddress = localAddress;
            this.replicationCoefficient = replicationCoefficient;

            // this.receiverClass = receiverClass;
            // this.tokenRate = tokenRate;
        } 

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private QS._core_c_.Base3.InstanceID localAddress;
        private uint replicationCoefficient;

        // private IReceiverClass receiverClass;
        // private double tokenRate;

        #region Class Agent

        private class Agent : Receivers4.IReceivingAgent
        {
            public Agent(ReceivingAgent1 owner, Receivers4.IReceivingAgentContext context)
            {
                this.owner = owner;
                this.context = context;

                partitionCount = (uint) Math.Floor(((double) context.ReceiverAddresses.Length) / ((double) owner.replicationCoefficient));
                partitions = new Partition[partitionCount];
                for (uint partno = 0; partno < partitionCount; partno++)
                {
                    List<QS._core_c_.Base3.InstanceID> members = new List<QS._core_c_.Base3.InstanceID>();
                    for (uint ind = partno; ind < (uint)context.ReceiverAddresses.Length; ind += partitionCount)
                        members.Add(context.ReceiverAddresses[ind]);


                    partitions[partno] = new Partition();
                }

/*
                replicationAgents = new ReplicationAgent[partitionCount];
                for (uint ind = 0; ind < partitionCount; ind++)
                    replicationAgents[ind] = null;
*/

                positionInGroup = (uint) Array.FindIndex<QS._core_c_.Base3.InstanceID>(context.ReceiverAddresses,
                  new Predicate<QS._core_c_.Base3.InstanceID>(((IEquatable<QS._core_c_.Base3.InstanceID>)owner.localAddress).Equals));


/*
                replicaAssignment = new ReplicaAssignment(
                    context.ReceiverAddresses, owner.localAddress, owner.replicationCoefficient);
*/ 

#if DEBUG_ReceivingAgent1
//                owner.logger.Log(this, "Agent(" + context.ID.ToString() + ") : my position in region is " + position.ToString() + 
//                    " and my position in ring " + myRing.ToString() + " is " + positionInRing.ToString());
#endif                                    

                // tokenRate = owner.tokenRate / context.ReceiverAddresses.Length;

                lock (this)
                {
                    IList<QS._core_c_.Base3.InstanceID> crashedCollection;
                    failureSubscription = context.FailureSource.subscribe(
                        new QS._qss_c_.Failure_.NotificationsCallback(this.CrashCallback), out crashedCollection);
                    this.CrashCallback(crashedCollection);
                }
            }

            private ReceivingAgent1 owner;
            private Receivers4.IReceivingAgentContext context;
            private Failure_.ISubscription failureSubscription;
            private uint partitionCount, positionInGroup;
            private Partition[] partitions;

//            private ReplicaAssignment replicaAssignment;

            // private IDictionary<QS._core_c_.Base3.InstanceID, ReceiverContext> registered = new Dictionary<QS._core_c_.Base3.InstanceID, ReceiverContext>();
            // private QS.Fx.QS.Fx.Clock.IAlarm tokenAlarm;
            // private double tokenRate;

            #region Class Partition

            private class Partition
            {
                public Partition()
                {
                }


            }

            #endregion

            #region CrashCallback

            private void CrashCallback(IList<QS._core_c_.Base3.InstanceID> crashedCollection)
            {
                // .............................................................................
            }

            #endregion

            /*
            #region Class Token

            [QS.Fx.Serialization.ClassID(ClassID.Rings5_RingAgent_Token)]
            private class Token : QS.Fx.Serialization.ISerializable
            {
                public Token()
                {
                }

                #region ISerializable Members

                QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
                {
                    get { throw new Exception("The method or operation is not implemented."); }
                }

                void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
                {
                    throw new Exception("The method or operation is not implemented.");
                }

                void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
                {
                    throw new Exception("The method or operation is not implemented.");
                }

                #endregion
            }

            #endregion

            #region Token Alarm

            private void TokenCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
            {
                // TODO: Add code to stop the circulating token if necessary....
                Token token = new Token();


                // TODO: Add code to adjust the token ricrulation rate based on the information about failures...
                alarmRef.Reschedule(1 / tokenRate);
            }

            private void ActivateToken()
            {
                if (tokenAlarm == null)
                {
                    tokenAlarm = owner.alarmClock.Schedule(
                        1 / tokenRate, new QS.Fx.QS.Fx.Clock.AlarmCallback(this.TokenCallback), null);
                }
            }

            #endregion

            #region GetContext

            ReceiverContext GetContext(QS._core_c_.Base3.InstanceID senderAddress)
            {
                ReceiverContext receiverContext = null;
                lock (this)
                {
                    if (registered.ContainsKey(senderAddress))
                        receiverContext = registered[senderAddress];
                    else
                    {
                        receiverContext = new ReceiverContext(this, senderAddress);
                        receiverContext.Receiver = owner.receiverClass.CreateReceiver(receiverContext);
                        registered.Add(senderAddress, receiverContext);

                        ActivateToken();
                    }
                }

                return receiverContext;
            }

            #endregion
*/

            #region Class Replica

            private class ReplicationAgent
            {
                public ReplicationAgent()
                {
                }

                public void Add(QS._core_c_.Base3.InstanceID sourceAddress, uint sequenceNo, QS._core_c_.Base3.Message message)
                {
                    // ........................................
                }


            }

            #endregion

            #region Receivers4.IReceivingAgent Members

            void Receivers4.IReceivingAgent.Receive(QS._core_c_.Base3.InstanceID sourceAddress, uint sequenceNo, QS._core_c_.Base3.Message message, 
                bool retransmission, bool forwarding)
            {
                uint partition_no = sequenceNo % partitionCount;
                uint inpartition_seqno = sequenceNo / partitionCount;

                lock (this)
                {
/*
                    ReplicationAgent replicationAgent = replicationAgents[partition_no];
                    if (replicationAgent != null)
                        replicationAgent.Add(sourceAddress, inpartition_seqno, message);
*/

                }

/*
                ReceiverContext context = GetContext(sourceAddress);
                if (context != null)
                    context.Receiver.Receive(sequenceNo, message);
*/ 
            }

            void QS._qss_c_.Receivers4.IReceivingAgent.Shutdown()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion

/*
            #region Class ReceiverContext

            private class ReceiverContext : IReceiverContext
            {
                public ReceiverContext(Agent owner, QS._core_c_.Base3.InstanceID senderAddress)
                {
                    this.owner = owner;
                    this.senderAddress = senderAddress;
                }

                private Agent owner;
                private QS._core_c_.Base3.InstanceID senderAddress;
                private IReceiver receiver;

                #region Accessors

                public IReceiver Receiver
                {
                    set { receiver = value; }
                    get { return receiver; }
                }

                #endregion

                #region IReceiverContext Members

                QS.CMS.QS._core_c_.Base3.InstanceID IReceiverContext.SenderAddress
                {
                    get { return senderAddress; }
                }

                QS._core_c_.Base3.InstanceID[] IReceiverContext.ReceiverAddresses
                {
                    get { return owner.context.ReceiverAddresses; }
                }

                #endregion
            }

            #endregion
*/
        }

        #endregion

        #region Receivers4.IReceivingAgentClass Members

        Receivers4.IReceivingAgent Receivers4.IReceivingAgentClass.CreateAgent(Receivers4.IReceivingAgentContext context)
        {
            return new Agent(this, context);
        }

        #endregion
    }
}
