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
    /// <summary>
    /// Sent to request the client to create a new group view. This may include the first view in a group, the first group the
    /// client receives, as well as an updated view causing other views to start flushing.
    /// </summary>
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_CreateGroupView)]
    public sealed class CreateGroupView : QS.Fx.Serialization.ISerializable
    {
        public CreateGroupView()
        {
        }

        public CreateGroupView(
            Base3_.GroupID groupID, uint groupViewSequenceNo, QS.Fx.Serialization.ISerializable groupViewAttributes)
            : this(groupID, groupViewSequenceNo, groupViewAttributes, null)
        {
        }

        public CreateGroupView(
            Base3_.GroupID groupID, uint groupViewSequenceNo, QS.Fx.Serialization.ISerializable groupViewAttributes, 
            IEnumerable<CreateMetaNode> metaNodes)
        {
            this.groupID = groupID;
            this.groupViewSequenceNo = groupViewSequenceNo;
            this.groupViewAttributes = groupViewAttributes;
            this.metaNodes = (metaNodes != null) ? new List<CreateMetaNode>(metaNodes) : new List<CreateMetaNode>();
        }

        [QS.Fx.Printing.Printable]
        private Base3_.GroupID groupID;
        [QS.Fx.Printing.Printable]
        private uint groupViewSequenceNo;
        [QS.Fx.Printing.Printable]
        private QS.Fx.Serialization.ISerializable groupViewAttributes;
        [QS.Fx.Printing.Printable]
        private List<CreateMetaNode> metaNodes;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public uint GroupViewSequenceNo
        {
            get { return groupViewSequenceNo; }
        }

        public QS.Fx.Serialization.ISerializable GroupViewAttributes
        {
            get { return groupViewAttributes; }
        }

        public IList<CreateMetaNode> MetaNodes
        {
            get { return metaNodes; }
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
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Notifications_CreateGroupView,
                    sizeof(bool) + 2 * sizeof(ushort) + sizeof(uint), sizeof(bool) + 2 * sizeof(ushort) + sizeof(uint), 0);
                info.AddAnother(groupID.SerializableInfo);
                if (groupViewAttributes != null)
                    info.AddAnother(groupViewAttributes.SerializableInfo);
                foreach (Base3.RVID rvid in regionViews)
                    info.AddAnother(rvid.SerializableInfo);
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
                *((ushort*)(pbuffer)) = (groupViewAttributes != null) ? groupViewAttributes.SerializableInfo.ClassID : (ushort)ClassID.Nothing;
                *((uint*)(pbuffer + sizeof(ushort))) = groupViewSequenceNo;
                *((ushort*)(pbuffer + sizeof(ushort) + sizeof(uint))) = (ushort) regionViews.Count;
                *((bool*)(pbuffer + 2 * sizeof(ushort) + sizeof(uint))) = incremental;
            }
            header.consume(sizeof(bool) + 2 * sizeof(ushort) + sizeof(uint));
            groupID.SerializeTo(ref header, ref data);
            if (groupViewAttributes != null)
                groupViewAttributes.SerializeTo(ref header, ref data);
            foreach (Base3.RVID rvid in regionViews)
                rvid.SerializeTo(ref header, ref data);
*/ 
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            throw new NotImplementedException();

/*
            ushort classID;
            int count1;
            fixed (byte* pbuffer = header.Array)
            {
                classID = *((ushort*)(pbuffer));
                groupViewSequenceNo = *((uint*)(pbuffer + sizeof(ushort)));
                count1 = (int)(*((ushort*)(pbuffer + sizeof(ushort) + sizeof(uint))));
                incremental = *((bool*)(pbuffer + 2 * sizeof(ushort) + sizeof(uint)));
            }
            header.consume(sizeof(bool) + 2 * sizeof(ushort) + sizeof(uint));
            groupID.DeserializeFrom(ref header, ref data);
            groupViewAttributes = Base3.Serializer.CreateObject(classID);
            if (groupViewAttributes != null)
                groupViewAttributes.DeserializeFrom(ref header, ref data);
            regionViews = new List<QS.CMS.Base3.RVID>(count1);
            for (int ind = 0; ind < count1; ind++)
            {
                Base3.RVID rvid = new QS.CMS.Base3.RVID();
                rvid.DeserializeFrom(ref header, ref data);
                regionViews.Add(rvid);
            }
*/ 
        }

        #endregion
    }
}
