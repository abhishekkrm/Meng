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
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateGroupViewRevision)]
    public sealed class CreateGroupViewRevision : QS.Fx.Serialization.ISerializable
    {
        public CreateGroupViewRevision()
        {
        }

        public CreateGroupViewRevision(Base3_.GroupID groupID, uint groupViewSequenceNo, uint groupViewRevisionSequenceNo,
            QS.Fx.Serialization.ISerializable groupViewRevisionAttributes)
            : this(groupID, groupViewSequenceNo, groupViewRevisionSequenceNo, groupViewRevisionAttributes, null)
        {
        }

        public CreateGroupViewRevision(Base3_.GroupID groupID, uint groupViewSequenceNo, uint groupViewRevisionSequenceNo,
            QS.Fx.Serialization.ISerializable groupViewRevisionAttributes, IEnumerable<CreateMetaNodeRevision> metaNodeRevisions)
        {
            this.groupID = groupID;
            this.groupViewSequenceNo = groupViewSequenceNo;
            this.groupViewRevisionSequenceNo = groupViewRevisionSequenceNo;
            this.groupViewRevisionAttributes = groupViewRevisionAttributes;
            this.metaNodeRevisions = 
                (metaNodeRevisions != null) ? new List<CreateMetaNodeRevision>(metaNodeRevisions) : new List<CreateMetaNodeRevision>();
        }

        [QS.Fx.Printing.Printable]
        private Base3_.GroupID groupID;
        [QS.Fx.Printing.Printable]
        private uint groupViewSequenceNo, groupViewRevisionSequenceNo;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Serialization.ISerializable groupViewRevisionAttributes;
        [QS.Fx.Printing.Printable]
        private List<CreateMetaNodeRevision> metaNodeRevisions;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public uint GroupViewSequenceNo
        {
            get { return groupViewSequenceNo; }
        }

        public uint GroupViewRevisionSequenceNo
        {
            get { return groupViewRevisionSequenceNo; }
        }

        public QS.Fx.Serialization.ISerializable GroupViewRevisionAttributes
        {
            get { return groupViewRevisionAttributes; }
        }

        public IList<CreateMetaNodeRevision> MetaNodeRevisions
        {
            get { return metaNodeRevisions; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                throw new NotImplementedException();

/*
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Notifications_CreateGroupViewRevision,
                    sizeof(bool) + 4 * sizeof(ushort) + 2 * sizeof(uint), sizeof(bool) + 4 * sizeof(ushort) + 2 * sizeof(uint), 0);
                info.AddAnother(groupID.SerializableInfo);
                if (groupViewRevisionAttributes != null)
                    info.AddAnother(groupViewRevisionAttributes.SerializableInfo);
                foreach (Base3.RVRevID rvrevid in regionViewRevisions)
                    info.AddAnother(rvrevid.SerializableInfo);
                foreach (QS._core_c_.Base3.InstanceID address in clientsToAdd)
                    info.AddAnother(address.SerializableInfo);
                foreach (QS._core_c_.Base3.InstanceID address in clientsToRemove)
                    info.AddAnother(address.SerializableInfo);
                return info;
*/ 
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            throw new NotImplementedException();

/*
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer)) = (groupViewRevisionAttributes != null) ? groupViewRevisionAttributes.SerializableInfo.ClassID : (ushort)ClassID.Nothing;
                *((uint*)(pbuffer + sizeof(ushort))) = groupViewSequenceNo;
                *((uint*)(pbuffer + sizeof(ushort) + sizeof(uint))) = groupViewRevisionSequenceNo;
                *((bool*)(pbuffer + sizeof(ushort) + 2 * sizeof(uint))) = incremental;
                *((ushort*)(pbuffer + sizeof(bool) + sizeof(ushort) + 2 * sizeof(uint))) = (ushort) regionViewRevisions.Count;
                *((ushort*)(pbuffer + sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint))) = (ushort)clientsToAdd.Count;
                *((ushort*)(pbuffer + sizeof(bool) + 3 * sizeof(ushort) + 2 * sizeof(uint))) = (ushort)clientsToRemove.Count;
            }
            header.consume(sizeof(bool) + 4 * sizeof(ushort) + 2 * sizeof(uint));
            groupID.SerializeTo(ref header, ref data);
            if (groupViewRevisionAttributes != null)
                groupViewRevisionAttributes.SerializeTo(ref header, ref data);
            foreach (Base3.RVRevID rvrevid in regionViewRevisions)
                rvrevid.SerializeTo(ref header, ref data);
            foreach (QS._core_c_.Base3.InstanceID address in clientsToAdd)
                address.SerializeTo(ref header, ref data);
            foreach (QS._core_c_.Base3.InstanceID address in clientsToRemove)
                address.SerializeTo(ref header, ref data);
*/ 
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotImplementedException();

/*
            ushort classID;
            int count1, count2, count3;
            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer));
                groupViewSequenceNo = *((uint*)(pbuffer + sizeof(ushort)));
                groupViewRevisionSequenceNo = *((uint*)(pbuffer + sizeof(ushort) + sizeof(uint)));
                incremental = *((bool*)(pbuffer + sizeof(ushort) + 2 * sizeof(uint)));
                count1 = (int)(*((ushort*)(pbuffer + sizeof(bool) + sizeof(ushort) + 2 * sizeof(uint))));
                count2 = (int)(*((ushort*)(pbuffer + sizeof(bool) + 2 * sizeof(ushort) + 2 * sizeof(uint))));
                count3 = (int)(*((ushort*)(pbuffer + sizeof(bool) + 3 * sizeof(ushort) + 2 * sizeof(uint))));
            }
            header.consume(sizeof(bool) + 4 * sizeof(ushort) + 2 * sizeof(uint));
            groupID.DeserializeFrom(ref header, ref data);
            groupViewRevisionAttributes = Base3.Serializer.CreateObject(classID);
            if (groupViewRevisionAttributes != null)
                groupViewRevisionAttributes.DeserializeFrom(ref header, ref data);
            regionViewRevisions = new List<QS.CMS.Base3.RVRevID>(count1);
            for (int ind = 0; ind < count1; ind++)
            {
                Base3.RVRevID rvrevid = new QS.CMS.Base3.RVRevID();
                rvrevid.DeserializeFrom(ref header, ref data);
                regionViewRevisions.Add(rvrevid);
            }
            clientsToAdd = new List<QS.CMS.QS._core_c_.Base3.InstanceID>(count2);
            for (int ind = 0; ind < count2; ind++)
            {
                QS._core_c_.Base3.InstanceID address = new QS.CMS.QS._core_c_.Base3.InstanceID();
                address.DeserializeFrom(ref header, ref data);
                clientsToAdd.Add(address);
            }
            clientsToRemove = new List<QS.CMS.QS._core_c_.Base3.InstanceID>(count3);
            for (int ind = 0; ind < count3; ind++)
            {
                QS._core_c_.Base3.InstanceID address = new QS.CMS.QS._core_c_.Base3.InstanceID();
                address.DeserializeFrom(ref header, ref data);
                clientsToRemove.Add(address);
            }
*/ 
        }

        #endregion
    }
}
