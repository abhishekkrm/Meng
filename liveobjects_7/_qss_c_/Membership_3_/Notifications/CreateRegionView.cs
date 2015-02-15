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
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateRegionView)]
    public sealed class CreateRegionView : QS.Fx.Serialization.ISerializable
    {
        public CreateRegionView()
        {
        }

        public CreateRegionView(Base3_.RegionID regionID, uint regionViewSequenceNo,
            QS.Fx.Serialization.ISerializable regionViewAttributes, bool incremental)
            : this(regionID, regionViewSequenceNo, regionViewAttributes, incremental, null, null)
        {
        }

        public CreateRegionView(Base3_.RegionID regionID, uint regionViewSequenceNo, 
            QS.Fx.Serialization.ISerializable regionViewAttributes, bool incremental,
            IEnumerable<QS._core_c_.Base3.InstanceID> membersToAdd, IEnumerable<QS._core_c_.Base3.InstanceID> membersToRemove)
        {
            this.regionID = regionID;
            this.regionViewSequenceNo = regionViewSequenceNo;
            this.regionViewAttributes = regionViewAttributes;
            this.incremental = incremental;
            this.membersToAdd = (membersToAdd != null) ? 
                new List<QS._core_c_.Base3.InstanceID>(membersToAdd) : new List<QS._core_c_.Base3.InstanceID>();
            this.membersToRemove = (membersToRemove != null) ?
                new List<QS._core_c_.Base3.InstanceID>(membersToRemove) : new List<QS._core_c_.Base3.InstanceID>();
        }

        [QS.Fx.Printing.Printable]
        private Base3_.RegionID regionID;
        [QS.Fx.Printing.Printable]
        private uint regionViewSequenceNo;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Serialization.ISerializable regionViewAttributes;
        [QS.Fx.Printing.Printable]
        private bool incremental;
        [QS.Fx.Printing.Printable]
        private List<QS._core_c_.Base3.InstanceID> membersToAdd, membersToRemove;

        #region Accessors

        public Base3_.RegionID RegionID
        {
            get { return regionID; }
        }

        public uint RegionViewSequenceNo
        {
            get { return regionViewSequenceNo; }
        }

        public QS.Fx.Serialization.ISerializable RegionViewAttributes
        {
            get { return regionViewAttributes; }
        }

        public bool Incremental
        {
            get { return incremental; }
        }
        
        public IList<QS._core_c_.Base3.InstanceID> MembersToAdd
        {
            get { return membersToAdd; }
        }
            
        public IList<QS._core_c_.Base3.InstanceID> MembersToRemove
        {
            get { return membersToRemove; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Notifications_CreateRegionView,
                    sizeof(bool) + 3 * sizeof(ushort) + sizeof(uint), sizeof(bool) + 3 * sizeof(ushort) + sizeof(uint), 0);
                info.AddAnother(regionID.SerializableInfo);
                if (regionViewAttributes != null)
                    info.AddAnother(regionViewAttributes.SerializableInfo);
                foreach (QS._core_c_.Base3.InstanceID address in membersToAdd)
                    info.AddAnother(address.SerializableInfo);
                foreach (QS._core_c_.Base3.InstanceID address in membersToRemove)
                    info.AddAnother(address.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer)) = (regionViewAttributes != null) ? regionViewAttributes.SerializableInfo.ClassID : (ushort)ClassID.Nothing;
                *((uint*)(pbuffer + sizeof(ushort))) = regionViewSequenceNo;
                *((bool*)(pbuffer + sizeof(ushort) + sizeof(uint))) = incremental;
                *((ushort*)(pbuffer + sizeof(ushort) + sizeof(uint) + sizeof(bool))) = (ushort) membersToAdd.Count;
                *((ushort*)(pbuffer + 2 * sizeof(ushort) + sizeof(uint) + sizeof(bool))) = (ushort)membersToRemove.Count;
            }
            header.consume(sizeof(bool) + 3 * sizeof(ushort) + sizeof(uint));
            regionID.SerializeTo(ref header, ref data);
            if (regionViewAttributes != null)
                regionViewAttributes.SerializeTo(ref header, ref data);
            foreach (QS._core_c_.Base3.InstanceID address in membersToAdd)
                address.SerializeTo(ref header, ref data);
            foreach (QS._core_c_.Base3.InstanceID address in membersToRemove)
                address.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count1, count2;
            ushort classID;
            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer));
                regionViewSequenceNo  = *((uint*)(pbuffer + sizeof(ushort)));
                incremental = *((bool*)(pbuffer + sizeof(ushort) + sizeof(uint)));
                count1 = (int)(*((ushort*)(pbuffer + sizeof(ushort) + sizeof(uint) + sizeof(bool))));
                count2 = (int)(*((ushort*)(pbuffer + 2 * sizeof(ushort) + sizeof(uint) + sizeof(bool))));
            }
            header.consume(sizeof(bool) + 3 * sizeof(ushort) + sizeof(uint));
            regionID.DeserializeFrom(ref header, ref data);
            regionViewAttributes = QS._core_c_.Base3.Serializer.CreateObject(classID);
            if (regionViewAttributes != null)
                regionViewAttributes.DeserializeFrom(ref header, ref data);
            membersToAdd = new List<QS._core_c_.Base3.InstanceID>(count1);
            for (int ind = 0; ind < count1; ind++)
            {
                QS._core_c_.Base3.InstanceID address = new QS._core_c_.Base3.InstanceID();
                address.DeserializeFrom(ref header, ref data);
                membersToAdd.Add(address);
            }
            membersToRemove = new List<QS._core_c_.Base3.InstanceID>(count2);
            for (int ind = 0; ind < count2; ind++)
            {
                QS._core_c_.Base3.InstanceID address = new QS._core_c_.Base3.InstanceID();
                address.DeserializeFrom(ref header, ref data);
                membersToRemove.Add(address);
            }
        }

        #endregion
    }
}
