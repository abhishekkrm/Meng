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

// #define DEBUG_Server

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.FailureDetection_.Centralized
{
    [QS.Fx.Base.Inspectable]
    public class Server : QS.Fx.Inspection.Inspectable, IFailureDetector
    {
        public TimeSpan HeartbeatTimeout
        {
            get { return heartbeatTimeout; }
            set { heartbeatTimeout = value; }
        }

        public int MaximumMissed
        {
            get { return maximumMissed; }
            set { maximumMissed = value; }
        }

        public Server(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock, 
            TimeSpan heartbeatTimeout, int maximumMissed, bool enabled)
        {
            this.logger = logger;
            this.maximumMissed = maximumMissed;
            this.alarmClock = alarmClock;
            this.heartbeatTimeout = heartbeatTimeout;

            demultiplexer.register((uint)ReservedObjectID.FailureDetection_Centralized_Server, new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback));

            this.enabled = enabled;
            if (enabled)
                alarm = alarmClock.Schedule(heartbeatTimeout.TotalSeconds, new QS.Fx.Clock.AlarmCallback(checkingCallback), null);
        }

        #region Class HeartbeatMessage

        [QS.Fx.Serialization.ClassID(ClassID.FailureDetection_Centralized_Server_HeartbeatMessage)]
        public class HeartbeatMessage : QS.Fx.Serialization.ISerializable
        {
            public HeartbeatMessage()
            {
            }

            public HeartbeatMessage(QS._core_c_.Base3.InstanceID instanceID)
            {
                this.instanceID = instanceID;
            }

            private QS._core_c_.Base3.InstanceID instanceID;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    QS.Fx.Serialization.SerializableInfo info = instanceID.SerializableInfo;
                    info.ClassID = (ushort) ClassID.FailureDetection_Centralized_Server_HeartbeatMessage;
                    return info; 
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                instanceID.SerializeTo(ref header, ref data);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                instanceID = new QS._core_c_.Base3.InstanceID();
                instanceID.DeserializeFrom(ref header, ref data);
            }

            #endregion
}

        #endregion

        #region Class Node

        [QS.Fx.Base.Inspectable]
        private class Node : QS.Fx.Inspection.Inspectable
        {
            public Node(QS._core_c_.Base3.InstanceID instanceID)
            {
                this.instanceID = instanceID;
            }

            private QS._core_c_.Base3.InstanceID instanceID;
            private int heartbeatsMissed = 0;

            public QS._core_c_.Base3.InstanceID InstanceID
            {
                get { return instanceID; }
                set { instanceID = value; }
            }

            public int HeartbeatsMissed
            {
                get { return heartbeatsMissed; }
                set { heartbeatsMissed = value; }
            }
        }

        #endregion

        #region Enabling and disabling

        public bool Enabled
        {
            get { return enabled; }
            set 
            {
                lock (this)
                {
                    if (value != enabled)
                    {
                        enabled = value;
                        if (enabled)
                        {
                            if (alarm != null)
                                alarm.Reschedule(heartbeatTimeout.TotalSeconds);
                            else
                                alarm = alarmClock.Schedule(heartbeatTimeout.TotalSeconds, new QS.Fx.Clock.AlarmCallback(checkingCallback), null);
                        }
                        else
                        {
                            if (alarm != null)
                            {
                                alarm.Cancel();
                                alarm = null;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        private QS.Fx.Logging.ILogger logger;
        [QS.Fx.Base.Inspectable]
        private System.Collections.Generic.IDictionary<QS.Fx.Network.NetworkAddress, Node> nodes =
            new System.Collections.Generic.Dictionary<QS.Fx.Network.NetworkAddress, Node>();
        private System.Collections.Generic.Queue<Change> changeQueue = new Queue<Change>();
        private int maximumMissed;
        private event ChangeCallback onChange;
        private bool enabled;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IAlarm alarm;
        private TimeSpan heartbeatTimeout;

        private System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID> deadAddresses = 
            new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();

        public IEnumerator<QS._core_c_.Base3.InstanceID> LiveInstances
        {
            get
            {
                foreach (Node node in nodes.Values)
                    yield return node.InstanceID;
            }
        }

        private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_Server
            logger.Log(this, sourceIID.ToString() + " : " + receivedObject.ToString());
#endif

            lock (nodes)
            {
                if (nodes.ContainsKey(sourceIID.Address))
                {
                    Node node = nodes[sourceIID.Address];
                    int comparison = ((IComparable<QS._core_c_.Base3.InstanceID>)sourceIID).CompareTo(node.InstanceID);
                    if (comparison >= 0)
                    {
                        if (comparison > 0)
                        {
                            changeQueue.Enqueue(new Change(node.InstanceID, Action.CRASHED));
                            changeQueue.Enqueue(new Change(sourceIID, Action.STARTED));
                            node.InstanceID = sourceIID;
                        }

                        nodes[sourceIID.Address].HeartbeatsMissed = 0;
                    }
                }
                else
                {
                    if (!deadAddresses.Contains(sourceIID))
                    {
                        nodes[sourceIID.Address] = new Node(sourceIID);
                        changeQueue.Enqueue(new Change(sourceIID, Action.STARTED));
                    }
                }
            }

            return null;
        }

        [Logging_1_.IgnoreCallbacks]
        private void checkingCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
#if DEBUG_Server
            logger.Log(this, "checkingCallback");
#endif

            if (enabled)
            {
                lock (nodes)
                {
                    List<Node> toremove = new List<Node>();
                    foreach (Node node in nodes.Values)
                    {
                        node.HeartbeatsMissed = node.HeartbeatsMissed + 1;
                        if (node.HeartbeatsMissed > maximumMissed)
                        {
#if DEBUG_Server
                            logger.Log(this, "__________@@@@@@@@@@__________CheckingCallback: CRASHED { " +
                                node.InstanceID.ToString() + " } __________@@@@@@@@@@__________");
#endif

                            changeQueue.Enqueue(new Change(node.InstanceID, Action.CRASHED));
                            toremove.Add(node);
                            deadAddresses.Add(node.InstanceID);
                        }
                    }

                    foreach (Node node in toremove)
                        nodes.Remove(node.InstanceID.Address);

                    if (changeQueue.Count > 0)
                    {
                        onChange(changeQueue);
                        changeQueue.Clear();
                    }
                }

                alarmRef.Reschedule(heartbeatTimeout.TotalSeconds);
            }
        }

        #region IFailureDetector Members

        event ChangeCallback IFailureDetector.OnChange
        {
            add { onChange += value; }
            remove { onChange -= value; }
        }

        #endregion
    }
}
