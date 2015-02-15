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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaNonpersistentState)]
    [QS.Fx.Base.Inspectable]
    public sealed class ReplicaNonpersistentState : QS.Fx.Inspection.Inspectable, IReplicaNonpersistentState, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public ReplicaNonpersistentState()
        {
        }

        public ReplicaNonpersistentState(IReplicaPersistentState persistentState)
        {
            this.persistentState = persistentState;

            _ChangeView(persistentState.MembershipView);
        }

        #endregion

        #region Fields

        private IReplicaPersistentState persistentState;
        private ReplicaStatus status = ReplicaStatus.Blocked;
        private Base.MembershipView currentView;
        private bool isInView, isCoordinator, isSingleton;
        private uint positionInView;
        private double discoveryTimestamp, discoveredWeight;
        private QS.Fx.Clock.IAlarm discoveryAlarm, discoveryResendAlarm, pendingOperationsAlarm;
        private bool runningDiscovery, synchronizing, synchronized;
        private PeerInfo[] peers;
        private Queue<Submission> pendingOperations = new Queue<Submission>();

        // these fields are currently not being serialized

        private bool pendingOperationsRegistered;
        private uint lastSubmittedMessageNumber;
        private IDictionary<uint, Request> submittedOperations = new Dictionary<uint, Request>();
        private bool tokenCirculationIsActive;
        private double tokenTimestamp;
        private QS.Fx.Clock.IAlarm tokenCirculationAlarm;

        #endregion

        #region _ChangeView

        private void _ChangeView(QS._qss_x_._Machine_1_.Base.MembershipView newView)
        {
            if (!newView.Equals(currentView))
            {
                Base.MembershipView previousView = currentView;
                currentView = newView;

                PeerInfo[] oldpeers = peers;
                peers = new PeerInfo[newView.Members.Length];

                if (oldpeers != null)
                {
                    for (int oldind = 0; oldind < previousView.Members.Length; oldind++)
                    {
                        if (oldpeers[oldind] != null)
                        {
                            for (int newind = 0; newind < currentView.Members.Length; newind++)
                            {
                                if (currentView.Members[newind].Name.Equals(previousView.Members[oldind].Name))
                                {
                                    if (((IPeerInfo) oldpeers[oldind]).ReplicaIncarnation >= currentView.Members[newind].Incarnation)
                                    {
                                        peers[newind] = oldpeers[oldind];
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                for (int newind = 0; newind < peers.Length; newind++)
                {
                    if (peers[newind] == null)
                        peers[newind] = new PeerInfo(currentView.Members[newind].Name, false, currentView.Members[newind].Incarnation,
                            currentView.Members[newind].Address, 0, 0);
                }

                bool wasCoordinator = isCoordinator;

                positionInView = 0;
                isInView = false;
                isCoordinator = false;
                isSingleton = false;

                if (currentView.SeqNo > 0)
                {
                    System.Diagnostics.Debug.Assert(currentView.Members != null);

                    for (int ind = 0; !isInView && ind < currentView.Members.Length; ind++)
                    {
                        if (currentView.Members[ind].Name.Equals(persistentState.ReplicaName)
                            && currentView.Members[ind].Incarnation.Equals(persistentState.ReplicaIncarnation))
                        {
                            isInView = true;
                            positionInView = (uint) ind;
                            if (positionInView == 0)
                            {
                                isCoordinator = true;
                                isSingleton = (currentView.Members.Length == 1);
                            }
                        }
                    }
                }

                synchronized = isSingleton || wasCoordinator && 
                    ((currentView.Incarnation == previousView.Incarnation) && (currentView.SeqNo == (previousView.SeqNo + 1)));

                discoveredWeight = 0;
                for (int ind = 0; ind < peers.Length; ind++)
                {
                    if (((IPeerInfo) peers[ind]).Discovered)
                        discoveredWeight += currentView.Members[ind].Weight;
                }
            }
        }

        #endregion

        #region IReplicaNonpersistentState Members

        bool IReplicaNonpersistentState.PendingOperationsRegistered
        {
            get { return pendingOperationsRegistered; }
            set { pendingOperationsRegistered = value; }
        }

        QS.Fx.Clock.IAlarm IReplicaNonpersistentState.PendingOperationsAlarm
        {
            get { return pendingOperationsAlarm; }
            set { pendingOperationsAlarm = value; }
        }

        ReplicaStatus IReplicaNonpersistentState.Status
        {
            get { return status; }
            set { status = value; }
        }

        bool IReplicaNonpersistentState.HasView
        {
            get { return currentView != null && currentView.SeqNo > 0 && currentView.Members != null; }
        }

        QS._qss_x_._Machine_1_.Base.MembershipView IReplicaNonpersistentState.CurrentView
        {
            get { return currentView; }
            set { _ChangeView(value); }
        }

        bool IReplicaNonpersistentState.IsInView
        {
            get { return isInView; }
        }

        bool IReplicaNonpersistentState.IsCoordinator
        {
            get { return isCoordinator; }
        }

        bool IReplicaNonpersistentState.IsSingleton
        {
            get { return isSingleton; }
        }

        uint IReplicaNonpersistentState.PositionInView
        {
            get { return positionInView; }
        }

        IPeerInfo IReplicaNonpersistentState.PeerInfo(int index)
        {
            System.Diagnostics.Debug.Assert(currentView != null && currentView.Members != null && index < currentView.Members.Length);

            System.Diagnostics.Debug.Assert(peers != null);

//            if (peers == null)
//            {
//                if (currentView == null || currentView.Members == null || index >= currentView.Members.Length)
//                    throw new Exception();
//
//                peers = new PeerInfo[currentView.Members.Length];
//            }

            System.Diagnostics.Debug.Assert(peers[index] != null);

//            if (peers[index] == null)
//            {
//                peers[index] = new PeerInfo(currentView.Members[index].Name, false,
//                    currentView.Members[index].Incarnation, currentView.Members[index].Address, 0, 0);
//            }

            return peers[index];
        }

        bool IReplicaNonpersistentState.RunningDiscovery
        {
            get { return runningDiscovery; }
            set { runningDiscovery = value; }
        }

        double IReplicaNonpersistentState.DiscoveryTimestamp
        {
            get { return discoveryTimestamp; }
            set { discoveryTimestamp = value; }
        }

        QS.Fx.Clock.IAlarm IReplicaNonpersistentState.DiscoveryAlarm
        {
            get { return discoveryAlarm; }
            set { discoveryAlarm = value; }
        }

        QS.Fx.Clock.IAlarm IReplicaNonpersistentState.DiscoveryResendAlarm
        {
            get { return discoveryResendAlarm; }
            set { discoveryResendAlarm = value; }
        }

        double IReplicaNonpersistentState.DiscoveredWeight
        {
            get { return discoveredWeight; }
            set { discoveredWeight = value; }
        }

        bool IReplicaNonpersistentState.DiscoveredReadQuorum
        {
            get { return currentView != null && currentView.Incarnation > 0 && currentView.SeqNo > 0 && discoveredWeight >= currentView.ReadQuorum; }
        }

        bool IReplicaNonpersistentState.DiscoveredWriteQuorum
        {
            get { return currentView != null && currentView.Incarnation > 0 && currentView.SeqNo > 0 && discoveredWeight >= currentView.WriteQuorum; }
        }

        bool IReplicaNonpersistentState.Synchronizing
        {
            get { return synchronizing; }
            set { synchronizing = value; }
        }

        bool IReplicaNonpersistentState.Synchronized
        {
            get { return synchronized; }
            set { synchronized = value; }
        }

        Queue<Submission> IReplicaNonpersistentState.PendingOperations
        {
            get { return pendingOperations; }
        }

        uint IReplicaNonpersistentState.LastSubmittedMessageNumber
        {
            get { return lastSubmittedMessageNumber; }
            set { lastSubmittedMessageNumber = value; }
        }

        IDictionary<uint, Request> IReplicaNonpersistentState.SubmittedOperations
        {
            get { return submittedOperations; }
        }

        bool IReplicaNonpersistentState.TokenCirculationIsActive
        {
            get { return tokenCirculationIsActive; }
            set { tokenCirculationIsActive = value; }
        }

        double IReplicaNonpersistentState.TokenTimestamp
        {
            get { return tokenTimestamp; }
            set { tokenTimestamp = value; }
        }

        QS.Fx.Clock.IAlarm IReplicaNonpersistentState.TokenCirculationAlarm
        {
            get { return tokenCirculationAlarm; }
            set { tokenCirculationAlarm = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Machine_Components_ReplicaNonpersistentState);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)currentView).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);                
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Double(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Double(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortArrayOf<PeerInfo>(ref info, peers);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                foreach (Submission submission in pendingOperations)
                {
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt16(ref info);
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)submission.Operation).SerializableInfo);
                }
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, (ushort)status);
            ((QS.Fx.Serialization.ISerializable)currentView).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, isInView);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, isCoordinator);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, isSingleton);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, positionInView);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Double(ref header, ref data, discoveryTimestamp);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Double(ref header, ref data, discoveredWeight);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, runningDiscovery);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, synchronizing);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, synchronized);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortArrayOf<PeerInfo>(ref header, ref data, peers);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, (ushort) pendingOperations.Count);
            foreach (Submission submission in pendingOperations)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt16(ref header, ref data, (ushort) ((QS.Fx.Serialization.ISerializable)submission.Operation).SerializableInfo.ClassID);
                ((QS.Fx.Serialization.ISerializable)submission.Operation).SerializeTo(ref header, ref data);
            }
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            status = (ReplicaStatus) QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
            currentView = new QS._qss_x_._Machine_1_.Base.MembershipView();
            ((QS.Fx.Serialization.ISerializable)currentView).DeserializeFrom(ref header, ref data);
            isInView = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            isCoordinator = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            isSingleton = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            positionInView = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            discoveryTimestamp = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Double(ref header, ref data);
            discoveredWeight = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Double(ref header, ref data);
            runningDiscovery = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            synchronizing = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            synchronized = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
            peers = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortArrayOf<PeerInfo>(ref header, ref data);
            int count = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
            while (count-- > 0)
            {
                ushort classid = (ushort) QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt16(ref header, ref data);
                ServiceControl.IServiceControllerOperation operation = (ServiceControl.IServiceControllerOperation)QS._core_c_.Base3.Serializer.CreateObject(classid);
                pendingOperations.Enqueue(new Submission(operation, null, null));
            }
        }

        #endregion
    }
}
