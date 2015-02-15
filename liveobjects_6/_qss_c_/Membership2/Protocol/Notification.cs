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

#define OPTION_EnableMulticastPerGroup

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Membership2.Protocol
{
    [QS.Fx.Printing.Printable("__________mmmmmmmmmm__________Membership_Notification__________mmmmmmmmmm__________mmmmmmmmmm__________", 
        QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership2_Protocol_Notification)]
    public class Notification : QS.Fx.Serialization.ISerializable
    {

/*
 
        RegionsToCreate:
        - The region gets newly created and this node is in one of the groups overlapping on this region.
        - The region existed before and may or may not have changed, the node joined one of the groups overlapping on this region.

        RegionsChanged:
        - 
 
        RegionsToDelete:
        - 
 
        GroupsToCreate:
        - 
 
        GroupsChanged:
        - 
 
        GroupsToDelete:
        - 
 
*/

        public Notification()
        {
        }

        #region Initialization

        public void Init()
        {
            RegionsToCreate = new List<RegionInfo_Complete>();
            RegionsChanged = new List<RegionInfo_ToUpdate>();
            RegionsToDelete = new List<Base3_.RegionID>();
            GroupsToCreate = new List<GroupInfo_Complete>();
            GroupsChanged = new List<GroupInfo_ToUpdate>();
            GroupsToDelete = new List<Base3_.GroupID>();
        }

        public static Notification Create()
        {
            Notification notification = new Notification();
            notification.Init();
            return notification;
        }

        #endregion

        #region Definitions of the Constituent Structures

        #region Struct RegionInfo_Complete

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        public struct RegionInfo_Complete : QS.Fx.Serialization.ISerializable
        {
            [QS.Fx.Printing.Printable]
            public Base3_.IDWithSequenceNo<Base3_.RegionID> View;
            [QS.Fx.Printing.Printable]
            public Base3_.RegionSig Definition;
            [QS.Fx.Printing.Printable]
            public QS.Fx.Network.NetworkAddress MulticastAddress;
            [QS.Fx.Printing.Printable]
            public List<QS._core_c_.Base3.InstanceID> Members;

            public RegionInfo_Complete(Base3_.IDWithSequenceNo<Base3_.RegionID> view, Base3_.RegionSig definition,
                QS.Fx.Network.NetworkAddress multicastAddress) : this(view, definition, multicastAddress, null)
            {
            }

            public RegionInfo_Complete(Base3_.IDWithSequenceNo<Base3_.RegionID> view, Base3_.RegionSig definition,
                QS.Fx.Network.NetworkAddress multicastAddress, ICollection<QS._core_c_.Base3.InstanceID> instanceIDs)
            {
                this.View = view;
                this.Definition = definition;
                this.MulticastAddress = multicastAddress;
                this.Members = (instanceIDs != null) ? new List<QS._core_c_.Base3.InstanceID>(instanceIDs) : new List<QS._core_c_.Base3.InstanceID>();
            }

            public override string ToString()
            {
                return "Region " + QS._core_c_.Helpers.ToString.Object(View) + "  " + QS._core_c_.Helpers.ToString.Object(Definition) + 
                    " Address: " + QS._core_c_.Helpers.ToString.Object(MulticastAddress) + " ; Nodes: " + 
                    Base3_.Convert.ArrayToString<QS._core_c_.Base3.InstanceID>("", Members.ToArray()) + "\n";
            }

            #region ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get 
                { 
                    return View.SerializableInfo.CombineWith(Definition.SerializableInfo).CombineWith(MulticastAddress.SerializableInfo).CombineWith(
                        Base3_.SerializationHelper.SerializableInfoOfListOfFixed<QS._core_c_.Base3.InstanceID>(Members));
                }
            }

            public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                View.SerializeTo(ref header, ref data);
                Definition.SerializeTo(ref header, ref data);
                MulticastAddress.SerializeTo(ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<QS._core_c_.Base3.InstanceID>(Members, ref header, ref data);
            }

            public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                View.DeserializeFrom(ref header, ref data);
                Definition = new QS._qss_c_.Base3_.RegionSig();
                Definition.DeserializeFrom(ref header, ref data);
                MulticastAddress = new QS.Fx.Network.NetworkAddress();
                MulticastAddress.DeserializeFrom(ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<QS._core_c_.Base3.InstanceID>,QS._core_c_.Base3.InstanceID>(
                    out Members, Base3_.Constructors<QS._core_c_.Base3.InstanceID>.ListConstructor, ref header, ref data);
            }

            #endregion
        }

        #endregion

        #region Struct RegionInfo_ToUpdate

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        public struct RegionInfo_ToUpdate : QS.Fx.Serialization.ISerializable
        {
            [QS.Fx.Printing.Printable]
            public Base3_.IDWithSequenceNo<Base3_.RegionID> View;
            [QS.Fx.Printing.Printable]
            public List<QS._core_c_.Base3.InstanceID> ToAdd, ToRemove;

            public RegionInfo_ToUpdate(Base3_.IDWithSequenceNo<Base3_.RegionID> view)
            {
                this.View = view;
                this.ToAdd = new List<QS._core_c_.Base3.InstanceID>();
                this.ToRemove = new List<QS._core_c_.Base3.InstanceID>();
            }

            public override string ToString()
            {
                return "Region " + QS._core_c_.Helpers.ToString.Object(View) + " ; ToAdd: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(ToAdd, ",") + " ; ToRemove: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(ToRemove, ",") + "\n";
            }

            #region ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get 
                { 
                    return View.SerializableInfo.CombineWith(Base3_.SerializationHelper.SerializableInfoOfListOfFixed<QS._core_c_.Base3.InstanceID>(ToAdd).CombineWith(
                        Base3_.SerializationHelper.SerializableInfoOfListOfFixed<QS._core_c_.Base3.InstanceID>(ToRemove)));
                }
            }

            public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                View.SerializeTo(ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<QS._core_c_.Base3.InstanceID>(ToAdd, ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<QS._core_c_.Base3.InstanceID>(ToRemove, ref header, ref data);
            }

            public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                View.DeserializeFrom(ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<QS._core_c_.Base3.InstanceID>, QS._core_c_.Base3.InstanceID>(out ToAdd,
                    Base3_.Constructors<QS._core_c_.Base3.InstanceID>.ListConstructor, ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<QS._core_c_.Base3.InstanceID>, QS._core_c_.Base3.InstanceID>(out ToRemove,
                    Base3_.Constructors<QS._core_c_.Base3.InstanceID>.ListConstructor, ref header, ref data);
            }

            #endregion
        }

        #endregion

        #region Struct GroupInfo_Complete

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        public struct GroupInfo_Complete : QS.Fx.Serialization.ISerializable
        {
            [QS.Fx.Printing.Printable]
            public Base3_.IDWithSequenceNo<Base3_.GroupID> View;
            [QS.Fx.Printing.Printable]
            public List<Base3_.IDWithSequenceNo<Base3_.RegionID>> Regions;
#if OPTION_EnableMulticastPerGroup
            [QS.Fx.Printing.Printable]
            public QS.Fx.Network.NetworkAddress MulticastAddress;
#endif

            public GroupInfo_Complete(Base3_.IDWithSequenceNo<Base3_.GroupID> view
#if OPTION_EnableMulticastPerGroup                
                , QS.Fx.Network.NetworkAddress multicastAddress
#endif
                )
                : this(view, null
#if OPTION_EnableMulticastPerGroup                
                    , multicastAddress
#endif
                )
            {
            }

            public GroupInfo_Complete(Base3_.IDWithSequenceNo<Base3_.GroupID> view, ICollection<Base3_.IDWithSequenceNo<Base3_.RegionID>> regions
#if OPTION_EnableMulticastPerGroup
                , QS.Fx.Network.NetworkAddress multicastAddress
#endif                
                )
            {
                this.View = view;
                this.Regions = (regions != null) ? new List<Base3_.IDWithSequenceNo<Base3_.RegionID>>(regions) : 
                    new List<Base3_.IDWithSequenceNo<Base3_.RegionID>>();
#if OPTION_EnableMulticastPerGroup
                this.MulticastAddress = multicastAddress;
#endif                
            }

            #region ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get 
                {
                    QS.Fx.Serialization.SerializableInfo info = View.SerializableInfo;
                    info.AddAnother(Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.IDWithSequenceNo<Base3_.RegionID>>(Regions));
#if OPTION_EnableMulticastPerGroup
                    info.AddAnother(this.MulticastAddress.SerializableInfo);
#endif                
                    return info;
                }
            }

            public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                View.SerializeTo(ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<Base3_.IDWithSequenceNo<Base3_.RegionID>>(Regions, ref header, ref data);
#if OPTION_EnableMulticastPerGroup
                this.MulticastAddress.SerializeTo(ref header, ref data);
#endif
            }

            public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                View.DeserializeFrom(ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<Base3_.IDWithSequenceNo<Base3_.RegionID>>, Base3_.IDWithSequenceNo<Base3_.RegionID>>(
                    out Regions, Base3_.Constructors<Base3_.IDWithSequenceNo<Base3_.RegionID>>.ListConstructor, ref header, ref data);
                this.MulticastAddress = new QS.Fx.Network.NetworkAddress();
#if OPTION_EnableMulticastPerGroup
                this.MulticastAddress.DeserializeFrom(ref header, ref data);
#endif
            }

            #endregion

            public override string ToString()
            {
                return "Group " + View.ToString() + " Regions: " +
                    Base3_.Convert.ArrayToString<Base3_.IDWithSequenceNo<Base3_.RegionID>>("", Regions.ToArray()) + "\n";
            }
        }

        #endregion

        #region Struct GroupInfo_ToUpdate

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        public struct GroupInfo_ToUpdate : QS.Fx.Serialization.ISerializable
        {
            [QS.Fx.Printing.Printable]
            public Base3_.IDWithSequenceNo<Base3_.GroupID> View;
            [QS.Fx.Printing.Printable]
            public List<Base3_.IDWithSequenceNo<Base3_.RegionID>> ToAdd, ToUpdate;
            [QS.Fx.Printing.Printable]
            public List<Base3_.RegionID> ToRemove;

            public GroupInfo_ToUpdate(Base3_.IDWithSequenceNo<Base3_.GroupID> view)
            {
                this.View = view;
                this.ToAdd = new List<Base3_.IDWithSequenceNo<Base3_.RegionID>>();
                this.ToUpdate = new List<Base3_.IDWithSequenceNo<Base3_.RegionID>>();
                this.ToRemove = new List<Base3_.RegionID>();
            }

            public override string ToString()
            {
                return "Group " + QS._core_c_.Helpers.ToString.Object(View) + " ; ToAdd: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.IDWithSequenceNo<Base3_.RegionID>>(ToAdd, ",") + " ; ToUpdate: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.IDWithSequenceNo<Base3_.RegionID>>(ToUpdate, ",") + " ; ToRemove: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.RegionID>(ToRemove, ",") + "\n";
            }

            #region ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get 
                {
                    return View.SerializableInfo.CombineWith(
                        Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.IDWithSequenceNo<Base3_.RegionID>>(ToAdd)).CombineWith(
                        Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.IDWithSequenceNo<Base3_.RegionID>>(ToUpdate)).CombineWith(
                        Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.RegionID>(ToRemove));
                }
            }

            public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                View.SerializeTo(ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<Base3_.IDWithSequenceNo<Base3_.RegionID>>(ToAdd, ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<Base3_.IDWithSequenceNo<Base3_.RegionID>>(ToUpdate, ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<Base3_.RegionID>(ToRemove, ref header, ref data);
            }

            public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                View.DeserializeFrom(ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<Base3_.IDWithSequenceNo<Base3_.RegionID>>, Base3_.IDWithSequenceNo<Base3_.RegionID>>(out ToAdd,
                    Base3_.Constructors<Base3_.IDWithSequenceNo<Base3_.RegionID>>.ListConstructor, ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<Base3_.IDWithSequenceNo<Base3_.RegionID>>, Base3_.IDWithSequenceNo<Base3_.RegionID>>(out ToUpdate,
                    Base3_.Constructors<Base3_.IDWithSequenceNo<Base3_.RegionID>>.ListConstructor, ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<Base3_.RegionID>, Base3_.RegionID>(out ToRemove,
                    Base3_.Constructors<Base3_.RegionID>.ListConstructor, ref header, ref data);
            }

            #endregion
        }

        #endregion

        #endregion

        [QS.Fx.Printing.Printable("Receiver", QS.Fx.Printing.PrintingStyle.Compact)]
        public QS._core_c_.Base3.InstanceID ReceiverIID;
        [QS.Fx.Printing.Printable("SeqNo", QS.Fx.Printing.PrintingStyle.Compact)]
        public uint SequenceNo;
        [QS.Fx.Printing.Printable]
        public List<RegionInfo_Complete> RegionsToCreate;
        [QS.Fx.Printing.Printable]
        public List<RegionInfo_ToUpdate> RegionsChanged;
        [QS.Fx.Printing.Printable]
        public List<Base3_.RegionID> RegionsToDelete;
        [QS.Fx.Printing.Printable]
        public List<GroupInfo_Complete> GroupsToCreate;
        [QS.Fx.Printing.Printable]
        public List<GroupInfo_ToUpdate> GroupsChanged;
        [QS.Fx.Printing.Printable]
        public List<Base3_.GroupID> GroupsToDelete;

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    Base3_.SerializationHelper.SerializableInfoOfList<RegionInfo_Complete>(RegionsToCreate).CombineWith(
                    Base3_.SerializationHelper.SerializableInfoOfList<RegionInfo_ToUpdate>(RegionsChanged)).CombineWith(
                    Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.RegionID>(RegionsToDelete)).CombineWith(
                    Base3_.SerializationHelper.SerializableInfoOfList<GroupInfo_Complete>(GroupsToCreate)).CombineWith(
                    Base3_.SerializationHelper.SerializableInfoOfList<GroupInfo_ToUpdate>(GroupsChanged)).CombineWith(
                    Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.GroupID>(GroupsToDelete));
                return (info.CombineWith(ReceiverIID.SerializableInfo)).Extend(
                    (ushort) ClassID.Membership2_Protocol_Notification, (ushort) sizeof(uint), 0, 0);
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            ReceiverIID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                *((uint*)(arrayptr + header.Offset)) = SequenceNo;
            }
            header.consume(sizeof(uint));
            Base3_.SerializationHelper.SerializeCollection<RegionInfo_Complete>(RegionsToCreate, ref header, ref data);
            Base3_.SerializationHelper.SerializeCollection<RegionInfo_ToUpdate>(RegionsChanged, ref header, ref data);
            Base3_.SerializationHelper.SerializeCollection<Base3_.RegionID>(RegionsToDelete, ref header, ref data);
            Base3_.SerializationHelper.SerializeCollection<GroupInfo_Complete>(GroupsToCreate, ref header, ref data);
            Base3_.SerializationHelper.SerializeCollection<GroupInfo_ToUpdate>(GroupsChanged, ref header, ref data);
            Base3_.SerializationHelper.SerializeCollection<Base3_.GroupID>(GroupsToDelete, ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ReceiverIID = new QS._core_c_.Base3.InstanceID();
            ReceiverIID.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                SequenceNo = *((uint*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(uint));
            Base3_.SerializationHelper.DeserializeCollection<List<RegionInfo_Complete>, RegionInfo_Complete>(
                out RegionsToCreate, Base3_.Constructors<RegionInfo_Complete>.ListConstructor, ref header, ref data);
            Base3_.SerializationHelper.DeserializeCollection<List<RegionInfo_ToUpdate>, RegionInfo_ToUpdate>(
                out RegionsChanged, Base3_.Constructors<RegionInfo_ToUpdate>.ListConstructor, ref header, ref data);
            Base3_.SerializationHelper.DeserializeCollection<List<Base3_.RegionID>, Base3_.RegionID>(
                out RegionsToDelete, Base3_.Constructors<Base3_.RegionID>.ListConstructor, ref header, ref data);
            Base3_.SerializationHelper.DeserializeCollection<List<GroupInfo_Complete>, GroupInfo_Complete>(
                out GroupsToCreate, Base3_.Constructors<GroupInfo_Complete>.ListConstructor, ref header, ref data);
            Base3_.SerializationHelper.DeserializeCollection<List<GroupInfo_ToUpdate>, GroupInfo_ToUpdate>(
                out GroupsChanged, Base3_.Constructors<GroupInfo_ToUpdate>.ListConstructor, ref header, ref data);
            Base3_.SerializationHelper.DeserializeCollection<List<Base3_.GroupID>, Base3_.GroupID>(
                out GroupsToDelete, Base3_.Constructors<Base3_.GroupID>.ListConstructor, ref header, ref data);
        }

        #endregion
  
        #region ToString

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
/*
            StringBuilder s = new StringBuilder();
            s.AppendLine("\nNotification #" + SequenceNo.ToString() + " for " + ReceiverIID.ToString() + ".\n");
            s.AppendLine("\nRegions To Create:\n\n");
            s.AppendLine(Helpers.CollectionHelper.ToStringConcatenated<RegionInfo_Complete>(RegionsToCreate));
            s.AppendLine("\nRegions Changed:\n\n");
            s.AppendLine(Helpers.CollectionHelper.ToStringConcatenated<RegionInfo_ToUpdate>(RegionsChanged));
            s.AppendLine("\nRegions To Delete:\n\n");
            s.AppendLine(Helpers.CollectionHelper.ToStringSeparated<Base3.RegionID>(RegionsToDelete, "\n"));
            s.AppendLine("\nGroups To Create:\n\n");
            s.AppendLine(Helpers.CollectionHelper.ToStringConcatenated<GroupInfo_Complete>(GroupsToCreate));
            s.AppendLine("\nGroups Changed:\n\n");
            s.AppendLine(Helpers.CollectionHelper.ToStringConcatenated<GroupInfo_ToUpdate>(GroupsChanged));
            s.AppendLine("\nGroups To Delete:\n\n");
            s.AppendLine(Helpers.CollectionHelper.ToStringSeparated<Base3.GroupID>(GroupsToDelete, "\n"));
            return s.ToString();
*/ 
        }
    
        #endregion
    }
}
