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

namespace QS._qss_c_.Membership_3_.ClientState
{
/*
    public sealed class Configuration
    {
        public Configuration(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;

        private IDictionary<Base3.GroupID, Group> groups = new Dictionary<Base3.GroupID, Group>();
        private IDictionary<Base3.RegionID, Region> regions = new Dictionary<Base3.RegionID, Region>();

        #region Update

        public void Update(Notifications.Notification notification)
        {
            #region Adding groups

            foreach (Notifications.CreateGroup createGroup in notification.Groups)
            {
                Group group = new Group(createGroup.GroupID, createGroup.GroupAttributes);
                lock (this)
                {
                    groups.Add(createGroup.GroupID, group);
                }
            }

            #endregion

            #region Adding client views

            foreach (Notifications.CreateClientView createClientView in notification.ClientViews)
            {
                Group group;
                lock (this)
                {
                    if (!groups.TryGetValue(createClientView.GroupID, out group))
                        throw new Exception("Group " + createClientView.GroupID.ToString() + " does not locally exist.");
                }

                lock (group)
                {
                    List<QS._core_c_.Base3.InstanceID> clientAddresses;
                    if (createClientView.Incremental)
                    {
                        if (group.CurrentClientView == null)
                            throw new Exception("The client view is incremental, but there is no client view installed.");
                        if (createClientView.SequenceNo != (group.CurrentClientView.SequenceNo + 1))
                            throw new Exception("The new client view is not an immediate successor of the current client view.");
                        
                        clientAddresses = new List<QS.CMS.QS._core_c_.Base3.InstanceID>(group.CurrentClientView.ClientAddresses);
                        clientAddresses.AddRange(createClientView.ClientsToAdd);
                        foreach (QS._core_c_.Base3.InstanceID address in createClientView.ClientsToRemove)
                            clientAddresses.Remove(address);
                    }
                    else
                        clientAddresses = new List<QS.CMS.QS._core_c_.Base3.InstanceID>(createClientView.ClientsToAdd);

                    ClientView clientView = new ClientView(group, createClientView.SequenceNo, clientAddresses);

                    group.ClientViews.Add(clientView.SequenceNo, clientView);
                    group.CurrentClientView = clientView;
                }
            }

            #endregion

            #region Adding regions

            foreach (Notifications.CreateRegion createRegion in notification.Regions)
            {
                Region region = new Region(createRegion.RegionID, createRegion.RegionSignature, createRegion.RegionAttributes);
                lock (this)
                {
                    regions.Add(createRegion.RegionID, region);
                }
            }

            #endregion

            #region Adding region views

            foreach (Notifications.CreateRegionView createRegionView in notification.RegionViews)
            {
                Region region;
                lock (this)
                {
                    if (!regions.TryGetValue(createRegionView.RegionID, out region))
                        throw new Exception("Cannot create region view " + createRegionView.RegionViewSequenceNo.ToString() +
                            ", region " + createRegionView.RegionID.ToString() + " does not exist locally.");                        
                }

                lock (region)
                {
                    IList<QS._core_c_.Base3.InstanceID> members;
                    if (createRegionView.Incremental)
                    {
                        RegionView currentView = region.CurrentView;
                        if (currentView == null)
                            throw new Exception("The received view is incremental, but no view is currently locally installed.");
                        if (createRegionView.RegionViewSequenceNo != (currentView.SequenceNo + 1))
                            throw new Exception("The received region view is not a successor of the currently installed view.");

                        members = new List<QS._core_c_.Base3.InstanceID>(region.CurrentView.Members);
                        foreach (QS._core_c_.Base3.InstanceID address in createRegionView.MembersToRemove)
                            members.Remove(address);
                        foreach (QS._core_c_.Base3.InstanceID address in createRegionView.MembersToAdd)
                            members.Add(address);
                    }
                    else
                        members = createRegionView.MembersToAdd;
                    QS._core_c_.Base3.InstanceID[] members_array = new QS.CMS.QS._core_c_.Base3.InstanceID[members.Count];
                    members.CopyTo(members_array, 0);
                    Array.Sort<QS._core_c_.Base3.InstanceID>(members_array);

                    RegionView regionView = new RegionView(region, createRegionView.RegionViewSequenceNo, members_array);

                    region.RegionViews.Add(regionView.SequenceNo, regionView);
                    region.CurrentView = regionView;

                    Member member = new Member(logger, eventLogger, regionView);

                    //.................................................................
                }
            }

            #endregion

            #region Adding metanodes

            ICollection<MetaNode> new_metanodes = new System.Collections.ObjectModel.Collection<MetaNode>();
            IDictionary<Base3.GVID, IDictionary<Base3.RVID, MetaNode>> metanode_lists = 
                new Dictionary<Base3.GVID, IDictionary<Base3.RVID, MetaNode>>();

            foreach (Notifications.CreateMetaNode createMetaNode in notification.MetaNodes)
            {
                MetaNode metanode = new MetaNode(null, createMetaNode.RegionViewID);
                new_metanodes.Add(metanode);

                IDictionary<Base3.RVID, MetaNode> metanode_list;
                if (!metanode_lists.TryGetValue(createMetaNode.GroupViewID, out metanode_list))
                    metanode_lists.Add(createMetaNode.GroupViewID, metanode_list = new Dictionary<Base3.RVID, MetaNode>());
                metanode_list.Add(metanode.RegionViewID, metanode);
            }

            #endregion

/-*
            #region Adding group views

            foreach (Notifications.CreateGroupView createGroupView in notification.GroupViews)
            {
                Group group;
                lock (this)
                {
                    if (!groups.TryGetValue(createGroupView.GroupID, out group))
                        throw new Exception("Group " + createGroupView.GroupID.ToString() + " does not locally exist.");
                }

                Base3.GVID gvid = new QS.CMS.Base3.GVID(createGroupView.GroupID, createGroupView.GroupViewSequenceNo);
                IDictionary<Base3.RVID, MetaNode> metanodes;
                if (!metanode_lists.TryGetValue(gvid, out metanodes))
                    throw new Exception("Cannot create group because no metanodes have been defined for it.");

                Base3.RVID[] metanode_ids = (new List<Base3.RVID>(metanodes.Keys)).ToArray();
                Array.Sort<Base3.RVID>(metanode_ids);
                Base3.RVID[] region_views = (new List<Base3.RVID>(createGroupView.RegionViews)).ToArray();
                Array.Sort<Base3.RVID>(region_views);
                if (metanode_ids.Length != region_views.Length)
                    throw new Exception("Defined metanode list different from region view list for the group.");
                for (int ind = 0; ind < metanode_ids.Length; ind++)
                    if (!metanode_ids[ind].Equals(region_views[ind]))
                        throw new Exception("Defined metanode list different from region view list for the group.");

                GroupView groupView = new GroupView(group, gvid.SeqNo, metanodes.Values);

                metanode_lists.Remove(gvid);
                foreach (MetaNode metanode in metanodes.Values)
                {
                    new_metanodes.Remove(metanode);
                    metanode.GroupView = groupView;
                }

                // .............creating members?
            }

            if (new_metanodes.Count > 0 || metanode_lists.Count > 0)
                throw new Exception("Some of the metanode definitions received have not been associated with any group views.");

            #endregion
*-/

            #region Adding metanode revisions

/-*
            ICollection<MetaNode> new_metanodes = new System.Collections.ObjectModel.Collection<MetaNode>();
            IDictionary<Base3.GVID, IDictionary<Base3.RVID, MetaNode>> metanode_lists = 
                new Dictionary<Base3.GVID, IDictionary<Base3.RVID, MetaNode>>();
*-/ 

            foreach (Notifications.CreateMetaNodeRevision createMetaNodeRevision in notification.MetaNodeRevisions)
            {
/-*
                MetaNode metanode = new MetaNode(null, createMetaNode.RegionViewID);
                new_metanodes.Add(metanode);

                IDictionary<Base3.RVID, MetaNode> metanode_list;
                if (!metanode_lists.TryGetValue(createMetaNode.GroupViewID, out metanode_list))
                    metanode_lists.Add(createMetaNode.GroupViewID, metanode_list = new Dictionary<Base3.RVID, MetaNode>());
                metanode_list.Add(metanode.RegionViewID, metanode);
*-/
            }

            #endregion


            #region Adding group view revisions

            foreach (Notifications.CreateGroupViewRevision createGroupViewRevision in notification.GroupViewRevisions)
            {
                Group group;
                lock (this)
                {
                    if (!groups.TryGetValue(createGroupViewRevision.GroupID, out group))
                        throw new Exception("Cannot add group view revision, group " + createGroupViewRevision.GroupID.ToString() + " does not exist.");
                }

                GroupView groupView;
                lock (group)
                {
                    if (!group.MembershipViews.TryGetValue(createGroupViewRevision.GroupViewSequenceNo, out groupView))
                        throw new Exception("Cannot add group view revision, view " + createGroupViewRevision.GroupViewSequenceNo.ToString() + 
                            " in group" + createGroupViewRevision.GroupID.ToString() + " does not exist.");
                }

                // GroupViewRevision groupViewRevision = new GroupViewRevision();
/-*
    
                GroupView groupView = group.View(createGroupViewRevision.GroupViewSequenceNo);
    
                groupView.CreateRevision(createGroupViewRevision.GroupViewRevisionSequenceNo,
                    createGroupViewRevision.GroupViewRevisionAttributes, createGroupViewRevision.Incremental,
                    createGroupViewRevision.RegionViewRevisions);
*-/ 

            }

            #endregion

            #region Adding region view revisions

            foreach (Notifications.CreateRegionViewRevision createRegionViewRevision in notification.RegionViewRevisions)
            {


/-*
                Region region;
                lock (this)
                {
                    region = regions[createRegionViewRevision.RegionID];
                }

                RegionView regionView = region.View(createRegionViewRevision.RegionViewSequenceNo);

                regionView.CreateRevision(createRegionViewRevision.RegionViewRevisionSequenceNo,
                    createRegionViewRevision.RegionViewRevisionAttributes,
                    createRegionViewRevision.Incremental, createRegionViewRevision.ClientsToAdd,
                    createRegionViewRevision.ClientsToRemove, createRegionViewRevision.MembersToRemove,
                    createRegionViewRevision.GroupViewsToAdd, createRegionViewRevision.GroupViewsToRemove);
*-/

            }

            #endregion

            #region Adding local views

            foreach (Notifications.CreateLocalView createLocalView in notification.LocalViews)
            {
            }

            #endregion

            #region Adding global views

            foreach (Notifications.CreateGlobalView createGlobalView in notification.GlobalViews)
            {
            }

            #endregion

            #region Adding incoming views

            foreach (Notifications.CreateIncomingView createIncomingView in notification.IncomingViews)
            {
            }

            #endregion

            #region Adding sessions

            foreach (Notifications.CreateSession createSession in notification.Sessions)
            {
            }

            #endregion

            #region Adding session views

            foreach (Notifications.CreateSessionView createSessionView in notification.SessionViews)
            {
            }

            #endregion
        }

        #endregion
    }
*/
}
