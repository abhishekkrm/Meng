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
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Base.Inspectable]
    public sealed class ReplicaPersistentState : QS.Fx.Inspection.Inspectable, IReplicaPersistentState, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public ReplicaPersistentState()
        {
        }

        #endregion

        #region Fields

        private string machineName, replicaName;
        private uint replicaIncarnation, latestCommittedMessageNumber;
        private QS._qss_x_.Base1_.Address replicaAddress = new QS._qss_x_.Base1_.Address();
        private IList<QS._qss_x_.Base1_.Address> discoveryAddresses = new List<QS._qss_x_.Base1_.Address>();
        private Base.MembershipView membershipView = QS._qss_x_._Machine_1_.Base.MembershipView.None;
        private QS._qss_x_.Base1_.Address disseminationAddress = new QS._qss_x_.Base1_.Address();

        #endregion

        #region IReplicaPersistentState Members

        [QS.Fx.Printing.Printable]
        string IReplicaPersistentState.ReplicaName
        {
            get { return replicaName; }
        }

        [QS.Fx.Printing.Printable]
        uint IReplicaPersistentState.ReplicaIncarnation
        {
            get { return replicaIncarnation; }
        }

        [QS.Fx.Printing.Printable]
        string IReplicaPersistentState.MachineName
        {
            get { return machineName; }
        }

        [QS.Fx.Printing.Printable]
        QS._qss_x_.Base1_.Address IReplicaPersistentState.ReplicaAddress
        {
            get { return replicaAddress; }
        }

        [QS.Fx.Printing.Printable]
        IList<QS._qss_x_.Base1_.Address> IReplicaPersistentState.DiscoveryAddresses
        {
            get { return discoveryAddresses; }
        }

        [QS.Fx.Printing.Printable]
        Base.MembershipView IReplicaPersistentState.MembershipView
        {
            get { return membershipView; }
        }

        [QS.Fx.Printing.Printable]
        QS._qss_x_.Base1_.Address IReplicaPersistentState.DisseminationAddress
        {
            get { return disseminationAddress; }
        }

        [QS.Fx.Printing.Printable]
        uint IReplicaPersistentState.LastCommittedMessageNumber
        {
            get { return latestCommittedMessageNumber; }
        }

        #endregion

        #region Private Setters

        private void _SetReplicaName(string replicaName)
        {
            this.replicaName = replicaName;
        }

        private void _SetReplicaIncarnation(uint replicaIncarnation)
        {
            this.replicaIncarnation = replicaIncarnation;
        }

        private void _SetMachineName(string machineName)
        {
            this.machineName = machineName;
        }

//        private void _SetMachineIncarnation(uint machineIncarnation)
//        {
//            this.machineIncarnation = machineIncarnation;
//        }

        private void _SetMembershipView(Base.MembershipView membershipView)
        {
//            if (this.membershipView != membershipView)
//            {
//                if (this.membershipView != null && suspectedInView != null)
//                {
//                    List<uint> suspectedInNewView = new List<uint>();
//                    foreach (uint index in suspectedInView)
//                    {
//                        Base.MemberInfo memberInfo = this.membershipView.Members[index];
//                        for (uint newindex = 0; newindex < membershipView.Members.Length; newindex++)
//                        {
//                            Base.MemberInfo other = membershipView.Members[newindex];
//                            if (other.Address.Equals(memberInfo.Address) && other.Incarnation.Equals(memberInfo.Incarnation))
//                                suspectedInNewView.Add(newindex);
//                        }
//                    }
//
//                    if (suspectedInNewView.Count > 0)
//                        suspectedInView = suspectedInNewView;
//                    else
//                        suspectedInView = null;
//                }

                this.membershipView = membershipView;

//            }
        }

//        private void _AddSuspectedInView(IList<uint> suspectedInView)
//        {
//            foreach (uint index in suspectedInView)
//            {
//                if (!this.suspectedInView.Contains(index))
//                    this.suspectedInView.Add(index);
//            }
//        }

        private void _SetReplicaAddress(QS._qss_x_.Base1_.Address replicaAddress)
        {
            this.replicaAddress = replicaAddress;
        }

        private void _SetDiscoveryAddresses(IList<QS._qss_x_.Base1_.Address> discoveryAddresses)
        {
            this.discoveryAddresses = discoveryAddresses;
        }

        private void _AddDiscoveryAddresses(IList<QS._qss_x_.Base1_.Address> discoveryAddresses)
        {
            foreach (QS._qss_x_.Base1_.Address address in discoveryAddresses)
            {
                if (!this.discoveryAddresses.Contains(address))
                    this.discoveryAddresses.Add(address);
            }
        }

        private void _RemoveDiscoveryAddresses(IList<QS._qss_x_.Base1_.Address> discoveryAddresses)
        {
            foreach (QS._qss_x_.Base1_.Address address in discoveryAddresses)
                this.discoveryAddresses.Remove(address);
        }

        private void _SetDisseminationAddress(QS._qss_x_.Base1_.Address disseminationAddress)
        {
            this.disseminationAddress = disseminationAddress;
        }

        #endregion

        #region QS.Fx.Serialization.ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState, 0);

                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, replicaName);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, machineName);                               
                info.AddAnother(((QS.Fx.Serialization.ISerializable) replicaAddress).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortListOf<QS._qss_x_.Base1_.Address>(ref info, discoveryAddresses);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) membershipView).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable) disseminationAddress).SerializableInfo);
                QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);

                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, replicaName);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, replicaIncarnation);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, machineName);            
            ((QS.Fx.Serialization.ISerializable) replicaAddress).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortListOf<QS._qss_x_.Base1_.Address>(ref header, ref data, discoveryAddresses);
            ((QS.Fx.Serialization.ISerializable) membershipView).SerializeTo(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) disseminationAddress).SerializeTo(ref header, ref data);
            QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, latestCommittedMessageNumber);
         }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            replicaName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            replicaIncarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            machineName = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);                        
            ((QS.Fx.Serialization.ISerializable) replicaAddress).DeserializeFrom(ref header, ref data);
            discoveryAddresses = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortListOf<QS._qss_x_.Base1_.Address>(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) membershipView).DeserializeFrom(ref header, ref data);
            ((QS.Fx.Serialization.ISerializable) disseminationAddress).DeserializeFrom(ref header, ref data);
            latestCommittedMessageNumber = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
        }

        #endregion

        #region Interface IAction

        public interface IAction : QS.Fx.Serialization.ISerializable, Persistence_.IOperation<ReplicaPersistentState>
        {
        }

        #endregion

        #region Actions

        #region SetReplicaName

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetReplicaName)]
        public class SetReplicaName : IAction
        {
            public SetReplicaName()
            {
            }

            public SetReplicaName(string name)
            {
                this.name = name;
            }

            private string name;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetReplicaName, 0, 0, 0);

                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, name);

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, name);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                name = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                target._SetReplicaName(name);
            }

            #endregion
        }

        #endregion

        #region SetReplicaIncarnation

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetReplicaIncarnation)]
        public class SetReplicaIncarnation : IAction
        {
            public SetReplicaIncarnation()
            {
            }

            public SetReplicaIncarnation(uint incarnation)
            {
                this.incarnation = incarnation;
            }

            private uint incarnation;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetReplicaIncarnation, 0, 0, 0);

                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, incarnation);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                incarnation = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                target._SetReplicaIncarnation(incarnation);
            }

            #endregion
        }

        #endregion

        #region SetMachineName

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetMachineName)]
        public class SetMachineName : IAction
        {
            public SetMachineName()
            {
            }

            public SetMachineName(string name)
            {
                this.name = name;
            }

            private string name;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort) QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetMachineName, 0, 0, 0);

                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ASCIIString(ref info, name);

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_ASCIIString(ref header, ref data, name);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                name = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ASCIIString(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                target._SetMachineName(name);
            }

            #endregion
        }

        #endregion

        #region SetMachineIncarnation

//        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetMachineIncarnation)]
//        public class SetMachineIncarnation : IAction
//        {
//            public SetMachineIncarnation()
//            {
//            }
//
//            public SetMachineIncarnation(uint incarnation)
//            {
//                this.incarnation = incarnation;
//            }
//
//            private uint incarnation;
//
//            #region ISerializable Members
//
//            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
//            {
//                get
//                {
//                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
//                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetMachineIncarnation, 0, 0, 0);
//
//                    QS.CMS.Base3.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
//
//                    return info;
//                }
//            }
//
//            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
//            {
//                QS.CMS.Base3.SerializationHelper.Serialize_UInt32(ref header, ref data, incarnation);
//            }
//
//            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
//            {
//                incarnation = QS.CMS.Base3.SerializationHelper.Deserialize_UInt32(ref header, ref data);
//            }
//
//            #endregion
//
//            #region IOperation<ReplicaPersistentState> Members
//
//            void QS.Fx.Persistence.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
//            {
//                target._SetMachineIncarnation(incarnation);
//            }
//
//            #endregion
//        }

        #endregion

        #region SetMembershipView

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetMembershipView)]
        public class SetMembershipView : IAction
        {
            public SetMembershipView()
            {
            }

            public SetMembershipView(Base.MembershipView membershipView)
            {
                this.membershipView = membershipView;
            }

            private Base.MembershipView membershipView;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetMembershipView, 0, 0, 0);

                    info.AddAnother(((QS.Fx.Serialization.ISerializable)membershipView).SerializableInfo);                    

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                ((QS.Fx.Serialization.ISerializable)membershipView).SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                membershipView = new QS._qss_x_._Machine_1_.Base.MembershipView();
                ((QS.Fx.Serialization.ISerializable)membershipView).DeserializeFrom(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                target._SetMembershipView(membershipView);
            }

            #endregion
        }

        #endregion

        #region SetReplicaAddress

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetReplicaAddress)]
        public class SetReplicaAddress : IAction
        {
            public SetReplicaAddress()
            {
            }

            public SetReplicaAddress(QS._qss_x_.Base1_.Address address)
            {
                this.address = address;
            }

            private QS._qss_x_.Base1_.Address address;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetReplicaAddress, 0, 0, 0);

                    info.AddAnother(((QS.Fx.Serialization.ISerializable)address).SerializableInfo);

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                ((QS.Fx.Serialization.ISerializable)address).SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                address = new QS._qss_x_.Base1_.Address();
                ((QS.Fx.Serialization.ISerializable)address).DeserializeFrom(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                target._SetReplicaAddress(address);
            }

            #endregion
        }

        #endregion

        #region ModifyDiscoveryAddresses

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_ModifyDiscoveryAddresses)]
        public class ModifyDiscoveryAddresses : IAction
        {
            public enum Modification : byte
            {
                Set, Add, Remove
            }

            public ModifyDiscoveryAddresses()
            {
            }

            public ModifyDiscoveryAddresses(Modification modification, IList<QS._qss_x_.Base1_.Address> addresses)
            {
                this.modification = modification;
                this.addresses = addresses;
            }

            private Modification modification;
            private IList<QS._qss_x_.Base1_.Address> addresses;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_ModifyDiscoveryAddresses, 
                        sizeof(byte), sizeof(byte), 0);

                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortListOf<QS._qss_x_.Base1_.Address>(ref info, addresses);

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* pheaderarray = header.Array)
                {
                    *(pheaderarray + header.Offset) = (byte)modification;
                }
                header.consume(sizeof(byte));
                QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortListOf<QS._qss_x_.Base1_.Address>(ref header, ref data, addresses);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* pheaderarray = header.Array)
                {
                    modification = (Modification)(*(pheaderarray + header.Offset));
                }
                header.consume(sizeof(byte));
                addresses = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortListOf<QS._qss_x_.Base1_.Address>(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                switch (modification)
                {
                    case Modification.Set:
                        {
                            target._SetDiscoveryAddresses(addresses);                                
                        }
                        break;

                    case Modification.Add:
                        {
                            target._AddDiscoveryAddresses(addresses);
                        }
                        break;

                    case Modification.Remove:
                        {
                            target._RemoveDiscoveryAddresses(addresses);
                        }
                        break;
                }
            }

            #endregion
        }

        #endregion

        #region SetDisseminationAddress

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetDisseminationAddress)]
        public class SetDisseminationAddress : IAction
        {
            public SetDisseminationAddress()
            {
            }

            public SetDisseminationAddress(QS._qss_x_.Base1_.Address address)
            {
                this.address = address;
            }

            private QS._qss_x_.Base1_.Address address;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_SetDisseminationAddress, 0, 0, 0);

                    info.AddAnother(((QS.Fx.Serialization.ISerializable)address).SerializableInfo);

                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                ((QS.Fx.Serialization.ISerializable)address).SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                address = new QS._qss_x_.Base1_.Address();
                ((QS.Fx.Serialization.ISerializable)address).DeserializeFrom(ref header, ref data);
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                target._SetDisseminationAddress(address);
            }

            #endregion
        }

        #endregion

        #region AddSuspectedInView

//        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_AddSuspectedInView)]
//        public class AddSuspectedInView : IAction
//        {
//            public AddSuspectedInView()
//            {
//            }
//
//            public AddSuspectedInView(IList<uint> suspectedInView)
//            {
//                this.suspectedInView = suspectedInView;
//            }
//
//            private IList<uint> suspectedInView;
//
//            #region ISerializable Members
//
//            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
//            {
//                get
//                {
//                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
//                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_AddSuspectedInView,
//                        sizeof(ushort) + sizeof(uint) * ((suspectedInView != null) ? suspectedInView.Count : 0));
//                    return info;
//                }
//            }
//
//            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
//                ref QS.CMS.Base3.WritableArraySegment<byte> header, ref IList<ArraySegment<byte>> data)
//            {
//                fixed (byte* pheaderarray = header.Array)
//                {
//                    byte* pheader = pheaderarray + header.Offset;
//                    *((ushort*)pheader) = (ushort) suspectedInView.Count;
//                    pheader += sizeof(ushort);
//                    if (suspectedInView != null)
//                    {
//                        foreach (uint removedGuy in suspectedInView)
//                        {
//                            *((uint*)pheader) = removedGuy;
//                            pheader += sizeof(uint);
//                        }
//                    }
//                }
//                header.consume(sizeof(ushort) + sizeof(uint) * ((suspectedInView != null) ? suspectedInView.Count : 0));
//            }
//
//            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
//            {
//                fixed (byte* pheaderarray = header.Array)
//                {
//                    byte* pheader = pheaderarray + header.Offset;
//                    int nguys = (int)(*((ushort*)pheader));
//                    pheader += sizeof(ushort);
//                    if (nguys > 0)
//                    {
//                        suspectedInView = new List<uint>(nguys);
//                        for (int ind = 0; ind < nguys; ind++)
//                        {
//                            suspectedInView.Add(*((uint*)pheader));
//                            pheader += sizeof(uint);
//                        }
//                    }
//                    else
//                        suspectedInView = null;
//                }
//                header.consume(sizeof(ushort) + sizeof(uint) * ((suspectedInView != null) ? suspectedInView.Count : 0));
//            }
//
//            #endregion
//
//            #region IOperation<ReplicaPersistentState> Members
//
//            void QS.Fx.Persistence.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
//            {
//                target._AddSuspectedInView(suspectedInView);
//            }
//
//            #endregion
//        }

        #endregion

        #region AddServiceControllerOperations

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_AddServiceControllerOperations)]
        public class AddServiceControllerOperations : IAction
        {
            public AddServiceControllerOperations()
            {
            }

            public AddServiceControllerOperations(uint seqno, IList<ServiceControl.IServiceControllerOperation> operations)
            {
                this.seqno = seqno;
                this.operations = operations;
            }

            private uint seqno;
            private IList<ServiceControl.IServiceControllerOperation> operations;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation_AddServiceControllerOperations);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_UInt32(ref info);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_ShortListOfUnknown<ServiceControl.IServiceControllerOperation>(ref info, operations);
                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
                ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_UInt32(ref header, ref data, seqno);
                QS._qss_c_.Base3_.SerializationHelper.Serialize_ShortListOfUnknown<ServiceControl.IServiceControllerOperation>(ref header, ref data, operations);                
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                seqno = QS._qss_c_.Base3_.SerializationHelper.Deserialize_UInt32(ref header, ref data);
                operations = QS._qss_c_.Base3_.SerializationHelper.Deserialize_ShortListOfUnknown<ServiceControl.IServiceControllerOperation>(ref header, ref data);                
            }

            #endregion

            #region IOperation<ReplicaPersistentState> Members

            void QS._qss_x_.Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState target)
            {
                System.Diagnostics.Debug.Assert(seqno > target.latestCommittedMessageNumber);

                if (seqno <= target.latestCommittedMessageNumber)
                    throw new Exception("Message committed out of order.");

                target.latestCommittedMessageNumber = seqno;

                // TODO: Now we should actually implement this change...........................
                System.Diagnostics.Debug.Assert(false, "Not Implemented");
            }

            #endregion
        }

        #endregion

        #endregion

        #region Class Operation

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation)]
        public class Operation : Persistence_.IOperation<ReplicaPersistentState>, QS.Fx.Serialization.ISerializable
        {
            public Operation(IAction[] actions)
            {
                this.actions = actions;
            }

            public Operation()
            {
            }

            private IAction[] actions;

            #region QS.Fx.Serialization.ISerializable Members

            unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    int headersize = sizeof(ushort) * (actions.Length + 1);
                    QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Machine_Components_ReplicaPersistentState_Operation, headersize, headersize, 0);
                    for (int ind = 0; ind < actions.Length; ind++)
                        info.AddAnother(actions[ind].SerializableInfo);
                    return info;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
                ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                if (actions.Length > ushort.MaxValue)
                    throw new Exception();
                fixed (byte* pheaderarray = header.Array)
                {
                    byte *pheader = pheaderarray + header.Offset;
                    *((ushort*) pheader) = (ushort) actions.Length;
                    pheader += sizeof(ushort);
                    for (int ind = 0; ind < actions.Length; ind++)
                    {
                        *((ushort*)pheader) = (ushort) actions[ind].SerializableInfo.ClassID;
                        pheader += sizeof(ushort);
                    }
                }
                header.consume(sizeof(ushort) * (actions.Length + 1));
                for (int ind = 0; ind < actions.Length; ind++)
                    actions[ind].SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
                ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* pheaderarray = header.Array)
                {
                    byte* pheader = pheaderarray + header.Offset;
                    actions = new IAction[(int)(*((ushort*)pheader))];
                    pheader += sizeof(ushort);
                    for (int ind = 0; ind < actions.Length; ind++)
                    {
                        actions[ind] = (IAction) QS._core_c_.Base3.Serializer.CreateObject(*((ushort*)pheader));
                        pheader += sizeof(ushort);
                    }
                }
                header.consume(sizeof(ushort) * (actions.Length + 1));
                for (int ind = 0; ind < actions.Length; ind++)
                    actions[ind].DeserializeFrom(ref header, ref data);
            }

            #endregion

            #region Persistence.IOperation<ReplicaPersistentState> Members

            void Persistence_.IOperation<ReplicaPersistentState>.Execute(ReplicaPersistentState state)
            {
                for (int ind = 0; ind < actions.Length; ind++)
                    actions[ind].Execute(state);                
            }

            #endregion
        }

        #endregion
    }
}
