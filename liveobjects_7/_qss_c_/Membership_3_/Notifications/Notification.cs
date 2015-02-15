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
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Notifications_Notification)]
    public sealed class Notification : QS.Fx.Serialization.ISerializable
    {
        public Notification()
        {
        }

        public Notification(int sequenceNo)
        {
            this.sequenceNo = sequenceNo;
        }

        [QS.Fx.Printing.Printable]
        private int sequenceNo;
        [QS.Fx.Printing.Printable]
        private List<CreateRegion> regions = new List<CreateRegion>();
        [QS.Fx.Printing.Printable]
        private List<CreateRegionView> regionViews = new List<CreateRegionView>();
        [QS.Fx.Printing.Printable]
        private List<CreateRegionViewRevision> regionViewRevisions = new List<CreateRegionViewRevision>();
        [QS.Fx.Printing.Printable]
        private List<CreateGroup> groups = new List<CreateGroup>();
        [QS.Fx.Printing.Printable]
        private List<CreateGroupView> groupViews = new List<CreateGroupView>();
        [QS.Fx.Printing.Printable]
        private List<CreateGroupViewRevision> groupViewRevisions = new List<CreateGroupViewRevision>();
        [QS.Fx.Printing.Printable]
        private List<CreateMetaNode> metanodes = new List<CreateMetaNode>();
        [QS.Fx.Printing.Printable]
        private List<CreateMetaNodeRevision> metanodeRevisions = new List<CreateMetaNodeRevision>();
        [QS.Fx.Printing.Printable]
        private List<CreateLocalView> localViews = new List<CreateLocalView>();
        [QS.Fx.Printing.Printable]
        private List<CreateGlobalView> globalViews = new List<CreateGlobalView>();
        [QS.Fx.Printing.Printable]
        private List<CreateIncomingView> incomingViews = new List<CreateIncomingView>();
        [QS.Fx.Printing.Printable]
        private List<CreateSession> sessions = new List<CreateSession>();
        [QS.Fx.Printing.Printable]
        private List<CreateSessionView> sessionViews = new List<CreateSessionView>();
        [QS.Fx.Printing.Printable]
        private List<CreateClientView> clientViews = new List<CreateClientView>();

        #region Accessors

        public int SequenceNo
        {
            get { return sequenceNo; }
        }

        public IEnumerable<CreateGroup> Groups
        {
            get { return groups; }
        }

        public IEnumerable<CreateGroupView> GroupViews
        {
            get { return groupViews; }
        }

        public IEnumerable<CreateGroupViewRevision> GroupViewRevisions
        {
            get { return groupViewRevisions; }
        }

        public IEnumerable<CreateRegion> Regions
        {
            get { return regions; }
        }

        public IEnumerable<CreateRegionView> RegionViews
        {
            get { return regionViews; }
        }

        public IEnumerable<CreateRegionViewRevision> RegionViewRevisions
        {
            get { return regionViewRevisions; }
        }

        public IEnumerable<CreateMetaNode> MetaNodes
        {
            get { return metanodes; }
        }

        public IEnumerable<CreateMetaNodeRevision> MetaNodeRevisions
        {
            get { return metanodeRevisions; }
        }

        public IEnumerable<CreateLocalView> LocalViews
        {
            get { return localViews; }
        }

        public IEnumerable<CreateGlobalView> GlobalViews
        {
            get { return globalViews; }
        }

        public IEnumerable<CreateIncomingView> IncomingViews
        {
            get { return incomingViews; }
        }

        public IEnumerable<CreateSession> Sessions
        {
            get { return sessions; }
        }

        public IEnumerable<CreateSessionView> SessionViews
        {
            get { return sessionViews; }
        }

        public IEnumerable<CreateClientView> ClientViews
        {
            get { return clientViews; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Membership3_Notifications_Notification, 6 * sizeof(ushort), 6 * sizeof(ushort), 0);
                foreach (CreateGroup element in groups)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element).SerializableInfo);
                foreach (CreateGroupView element in groupViews)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element).SerializableInfo);
                foreach (CreateGroupViewRevision element in groupViewRevisions)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element).SerializableInfo);
                foreach (CreateRegion element in regions)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element).SerializableInfo);
                foreach (CreateRegionView element in regionViews)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element).SerializableInfo);
                foreach (CreateRegionViewRevision element in regionViewRevisions)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)element).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *((ushort*)(pheader)) = (ushort) groups.Count;
                *((ushort*)(pheader + sizeof(ushort))) = (ushort) groupViews.Count;
                *((ushort*)(pheader + 2 * sizeof(ushort))) = (ushort)groupViewRevisions.Count;
                *((ushort*)(pheader + 3 * sizeof(ushort))) = (ushort)regions.Count;
                *((ushort*)(pheader + 4 * sizeof(ushort))) = (ushort)regionViews.Count;
                *((ushort*)(pheader + 5 * sizeof(ushort))) = (ushort)regionViewRevisions.Count;
            }
            header.consume(6 * sizeof(ushort));
            foreach (CreateGroup element in groups)
                ((QS.Fx.Serialization.ISerializable)element).SerializeTo(ref header, ref data);
            foreach (CreateGroupView element in groupViews)
                ((QS.Fx.Serialization.ISerializable)element).SerializeTo(ref header, ref data);
            foreach (CreateGroupViewRevision element in groupViewRevisions)
                ((QS.Fx.Serialization.ISerializable)element).SerializeTo(ref header, ref data);
            foreach (CreateRegion element in regions)
                ((QS.Fx.Serialization.ISerializable)element).SerializeTo(ref header, ref data);
            foreach (CreateRegionView element in regionViews)
                ((QS.Fx.Serialization.ISerializable)element).SerializeTo(ref header, ref data);
            foreach (CreateRegionViewRevision element in regionViewRevisions)
                ((QS.Fx.Serialization.ISerializable)element).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count1, count2, count3, count4, count5, count6;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                count1 = (int)(*((ushort*)(pheader)));
                count2 = (int)(*((ushort*)(pheader + sizeof(ushort))));
                count3 = (int)(*((ushort*)(pheader + 2 * sizeof(ushort))));
                count4 = (int)(*((ushort*)(pheader + 3 * sizeof(ushort))));
                count5 = (int)(*((ushort*)(pheader + 4 * sizeof(ushort))));
                count6 = (int)(*((ushort*)(pheader + 5 * sizeof(ushort))));
            }
            header.consume(6 * sizeof(ushort));
            for (int ind = 0; ind < count1; ind++)
            {
                CreateGroup element = new CreateGroup();
                ((QS.Fx.Serialization.ISerializable)element).DeserializeFrom(ref header, ref data);
                groups.Add(element);
            }
            for (int ind = 0; ind < count2; ind++)
            {
                CreateGroupView element = new CreateGroupView();
                ((QS.Fx.Serialization.ISerializable)element).DeserializeFrom(ref header, ref data);
                groupViews.Add(element);
            }
            for (int ind = 0; ind < count3; ind++)
            {
                CreateGroupViewRevision element = new CreateGroupViewRevision();
                ((QS.Fx.Serialization.ISerializable)element).DeserializeFrom(ref header, ref data);
                groupViewRevisions.Add(element);
            }
            for (int ind = 0; ind < count4; ind++)
            {
                CreateRegion element = new CreateRegion();
                ((QS.Fx.Serialization.ISerializable)element).DeserializeFrom(ref header, ref data);
                regions.Add(element);
            }
            for (int ind = 0; ind < count5; ind++)
            {
                CreateRegionView element = new CreateRegionView();
                ((QS.Fx.Serialization.ISerializable)element).DeserializeFrom(ref header, ref data);
                regionViews.Add(element);
            }
            for (int ind = 0; ind < count6; ind++)
            {
                CreateRegionViewRevision element = new CreateRegionViewRevision();
                ((QS.Fx.Serialization.ISerializable)element).DeserializeFrom(ref header, ref data);
                regionViewRevisions.Add(element);
            }
        }

        #endregion
    }
}
