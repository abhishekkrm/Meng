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

namespace QS._qss_c_.Membership_3_.Notifications
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateRegionViewRevision)]
    public sealed class CreateRegionViewRevision : QS.Fx.Serialization.ISerializable
    {
        public CreateRegionViewRevision()
        {
        }

        public CreateRegionViewRevision(
            Base3_.RegionID regionID, uint regionViewSequenceNo, uint regionViewRevisionSequenceNo,
            QS.Fx.Serialization.ISerializable regionViewRevisionAttributes, bool incremental)
            : this(regionID, regionViewSequenceNo, regionViewRevisionSequenceNo, regionViewRevisionAttributes,
            incremental, null)
        {
        }

        public CreateRegionViewRevision(
            Base3_.RegionID regionID, uint regionViewSequenceNo, uint regionViewRevisionSequenceNo, 
            QS.Fx.Serialization.ISerializable regionViewRevisionAttributes, bool incremental, 
            IEnumerable<QS._core_c_.Base3.InstanceID> membersToRemove)

//          IEnumerable<QS._core_c_.Base3.InstanceID> clientsToAdd, IEnumerable<QS._core_c_.Base3.InstanceID> clientsToRemove, 
//          IEnumerable<Base3.GVID> groupViewsToAdd, IEnumerable<Base3.GVID> groupViewsToRemove
        {
            this.regionID = regionID;
            this.regionViewSequenceNo = regionViewSequenceNo;
            this.regionViewRevisionSequenceNo = regionViewRevisionSequenceNo;
            this.regionViewRevisionAttributes = regionViewRevisionAttributes;
            this.incremental = incremental;
            this.membersToRemove =
                (membersToRemove != null) ? new List<QS._core_c_.Base3.InstanceID>(membersToRemove) : new List<QS._core_c_.Base3.InstanceID>();

//          this.clientsToAdd = 
//              (clientsToAdd != null) ? new List<QS._core_c_.Base3.InstanceID>(clientsToAdd) : new List<QS._core_c_.Base3.InstanceID>();
//          this.clientsToRemove =
//              (clientsToRemove != null) ? new List<QS._core_c_.Base3.InstanceID>(clientsToRemove) : new List<QS._core_c_.Base3.InstanceID>();
//          this.groupViewsToAdd =
//              (groupViewsToAdd != null) ? new List<Base3.GVID>(groupViewsToAdd) : new List<Base3.GVID>();
//          this.groupViewsToRemove =
//              (groupViewsToRemove != null) ? new List<Base3.GVID>(groupViewsToRemove) : new List<Base3.GVID>();
        }

        [QS.Fx.Printing.Printable]
        private Base3_.RegionID regionID;
        [QS.Fx.Printing.Printable]
        private uint regionViewSequenceNo, regionViewRevisionSequenceNo;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Serialization.ISerializable regionViewRevisionAttributes;
        [QS.Fx.Printing.Printable]
        private bool incremental;
        [QS.Fx.Printing.Printable]
        private List<QS._core_c_.Base3.InstanceID> membersToRemove; 

//      clientsToAdd, clientsToRemove, 
//      [QS.Fx.Printing.Printable]
//      private List<Base3.GVID> groupViewsToAdd, groupViewsToRemove;

        #region Accessors

        public Base3_.RegionID RegionID
        {
            get { return regionID; }
        }

        public uint RegionViewSequenceNo
        {
            get { return regionViewSequenceNo; }
        }

        public uint RegionViewRevisionSequenceNo
        {
            get { return regionViewRevisionSequenceNo; }
        }

        public QS.Fx.Serialization.ISerializable RegionViewRevisionAttributes
        {
            get { return regionViewRevisionAttributes; }
        }

        public bool Incremental
        {
            get { return incremental; }
        }

        public IList<QS._core_c_.Base3.InstanceID> MembersToRemove
        {
            get { return membersToRemove; }
        }

//        public IList<QS._core_c_.Base3.InstanceID> ClientsToAdd
//        {
//            get { return clientsToAdd; }
//        }
//
//        public IList<QS._core_c_.Base3.InstanceID> ClientsToRemove
//        {
//            get { return clientsToRemove; }
//        }
//
//        public List<Base3.GVID> GroupViewsToAdd
//        {
//            get { return groupViewsToAdd; }
//        }
//
//        public List<Base3.GVID> GroupViewsToRemove
//        {
//            get { return groupViewsToRemove; }
//        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Notifications_CreateRegionViewRevision,
                    sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint), sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint), 0);
                info.AddAnother(regionID.SerializableInfo);
                if (regionViewRevisionAttributes != null)
                    info.AddAnother(regionViewRevisionAttributes.SerializableInfo);
                foreach (QS._core_c_.Base3.InstanceID address in membersToRemove)
                    info.AddAnother(address.SerializableInfo);
                return info;

//                foreach (QS._core_c_.Base3.InstanceID address in clientsToAdd)
//                    info.AddAnother(address.SerializableInfo);
//                foreach (QS._core_c_.Base3.InstanceID address in clientsToRemove)
//                    info.AddAnother(address.SerializableInfo);
//                foreach (Base3.GVID gvid in groupViewsToAdd)
//                    info.AddAnother(gvid.SerializableInfo);
//                foreach (Base3.GVID gvid in groupViewsToRemove)
//                    info.AddAnother(gvid.SerializableInfo);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer)) = (regionViewRevisionAttributes != null) ? regionViewRevisionAttributes.SerializableInfo.ClassID : (ushort)ClassID.Nothing;
                *((uint*)(pbuffer + sizeof(ushort))) = regionViewSequenceNo;
                *((uint*)(pbuffer + sizeof(ushort) + sizeof(uint))) = regionViewRevisionSequenceNo;
                *((bool*)(pbuffer + sizeof(ushort) + 2 * sizeof(uint))) = incremental;
                *((ushort*)(pbuffer + sizeof(bool) + sizeof(ushort) + 2 * sizeof(uint))) = (ushort)membersToRemove.Count;

//                *((ushort*)(pbuffer + sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint))) = (ushort)clientsToAdd.Count;
//                *((ushort*)(pbuffer + sizeof(bool) + 3 * sizeof(ushort) + 2 * sizeof(uint))) = (ushort) clientsToRemove.Count;
//                *((ushort*)(pbuffer + sizeof(bool) + 4 * sizeof(ushort) + 2 * sizeof(uint))) = (ushort) groupViewsToAdd.Count;
//                *((ushort*)(pbuffer + sizeof(bool) + 5 * sizeof(ushort) + 2 * sizeof(uint))) = (ushort) groupViewsToRemove.Count;
            }
            header.consume(sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint));
            regionID.SerializeTo(ref header, ref data);
            if (regionViewRevisionAttributes != null)
                regionViewRevisionAttributes.SerializeTo(ref header, ref data);
            foreach (QS._core_c_.Base3.InstanceID address in membersToRemove)
                address.SerializeTo(ref header, ref data);

//            foreach (QS._core_c_.Base3.InstanceID address in clientsToAdd)
//                address.SerializeTo(ref header, ref data);
//            foreach (QS._core_c_.Base3.InstanceID address in clientsToRemove)
//                address.SerializeTo(ref header, ref data);
//            foreach (Base3.GVID gvid in groupViewsToAdd)
//                gvid.SerializeTo(ref header, ref data);
//            foreach (Base3.GVID gvid in groupViewsToRemove)
//                gvid.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID;
            int count1;
            
//            , count2, count3, count4, count5;

            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer));
                regionViewSequenceNo = *((uint*)(pbuffer + sizeof(ushort)));
                regionViewRevisionSequenceNo = *((uint*)(pbuffer + sizeof(ushort) + sizeof(uint)));
                incremental = *((bool*)(pbuffer + sizeof(ushort) + 2 * sizeof(uint)));
                count1 = (int)(*((ushort*)(pbuffer + sizeof(bool) + sizeof(ushort) + 2 * sizeof(uint))));

//                count2 = (int)(*((ushort*)(pbuffer + sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint))));
//                count3 = (int)(*((ushort*)(pbuffer + sizeof(bool) + 3 * sizeof(ushort) + 2 * sizeof(uint))));
//                count4 = (int)(*((ushort*)(pbuffer + sizeof(bool) + 4 * sizeof(ushort) + 2 * sizeof(uint))));
//                count5 = (int)(*((ushort*)(pbuffer + sizeof(bool) + 5 * sizeof(ushort) + 2 * sizeof(uint))));
            }
            header.consume(sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint));
            regionID.DeserializeFrom(ref header, ref data);
            regionViewRevisionAttributes = QS._core_c_.Base3.Serializer.CreateObject(classID);
            if (regionViewRevisionAttributes != null)
                regionViewRevisionAttributes.DeserializeFrom(ref header, ref data);
            membersToRemove = new List<QS._core_c_.Base3.InstanceID>(count1);
            for (int ind = 0; ind < count1; ind++)
            {
                QS._core_c_.Base3.InstanceID address = new QS._core_c_.Base3.InstanceID();
                address.DeserializeFrom(ref header, ref data);
                membersToRemove.Add(address);
            }

//            clientsToAdd = new List<QS.CMS.QS._core_c_.Base3.InstanceID>(count2);
//            for (int ind = 0; ind < count2; ind++)
//            {
//                QS._core_c_.Base3.InstanceID address = new QS.CMS.QS._core_c_.Base3.InstanceID();
//                address.DeserializeFrom(ref header, ref data);
//                clientsToAdd.Add(address);
//            }
//            clientsToRemove = new List<QS.CMS.QS._core_c_.Base3.InstanceID>(count3);
//            for (int ind = 0; ind < count3; ind++)
//            {
//                QS._core_c_.Base3.InstanceID address = new QS.CMS.QS._core_c_.Base3.InstanceID();
//                address.DeserializeFrom(ref header, ref data);
//                clientsToRemove.Add(address);
//            }
//            groupViewsToAdd = new List<QS.CMS.Base3.GVID>(count4);
//            for (int ind = 0; ind < count4; ind++)
//            {
//                Base3.GVID gvid = new QS.CMS.Base3.GVID();
//                gvid.DeserializeFrom(ref header, ref data);
//                groupViewsToAdd.Add(gvid);
//            }
//            groupViewsToRemove = new List<QS.CMS.Base3.GVID>(count5);
//            for (int ind = 0; ind < count5; ind++)
//            {
//                Base3.GVID gvid = new QS.CMS.Base3.GVID();
//                gvid.DeserializeFrom(ref header, ref data);
//                groupViewsToRemove.Add(gvid);
//            }
        }

        #endregion
    }
}
