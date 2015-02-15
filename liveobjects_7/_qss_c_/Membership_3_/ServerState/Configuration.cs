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

namespace QS._qss_c_.Membership_3_.ServerState
{
/*
    public class Configuration
    {
        public Configuration(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private IDictionary<string, Base3.GroupID> names2groups = new Dictionary<string, Base3.GroupID>();
        private IDictionary<Base3.RegionSig, Region> signatures2regions = new Dictionary<Base3.RegionSig, Region>();
        private IDictionary<Expressions.Expression, Base3.GroupID> expressions2groups = 
            new Dictionary<Expressions.Expression, Base3.GroupID>();
        private int lastUsedGroupID;
        private ulong lastUsedRegionID;
        private IDictionary<Base3.GroupID, Group> groups = new Dictionary<Base3.GroupID, Group>();
        private IDictionary<QS._core_c_.Base3.InstanceID, Node> nodes = new Dictionary<QS._core_c_.Base3.InstanceID, Node>();
        private ICollection<QS._core_c_.Base3.InstanceID> crashed = new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();
        private IDictionary<Base3.RegionID, Region> regions = new Dictionary<Base3.RegionID, Region>();

        #region Processing updates

        private void ProcessUpdates(
            IDictionary<Base3.GroupID, Update.GroupUpdate> groupUpdates, 
            IDictionary<QS._core_c_.Base3.InstanceID, Update.NodeUpdate> nodeUpdates, 
            IDictionary<QS._core_c_.Base3.InstanceID, Notifications.Notification> notifications)
        {
            Components.AutomaticCollection<Base3.RegionID, Components.SetChange<Node>> regionMemberChanges =
                new Components.AutomaticCollection<Base3.RegionID, Components.SetChange<Node>>();

            Components.AutomaticCollection<Base3.RegionID, Components.SetChange<Node>> regionClientChanges =
                new Components.AutomaticCollection<Base3.RegionID, Components.SetChange<Node>>();

            #region Calculating region member changes

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Update.NodeUpdate> element in nodeUpdates)
            {
                QS._core_c_.Base3.InstanceID address = element.Key;
                Update.NodeUpdate nodeUpdate = element.Value;

                if (nodeUpdate.memberOfToAdd.Count > 0 || nodeUpdate.memberOfToRemove.Count > 0 || nodeUpdate.resync)
                {
                    Node node = nodeUpdate.node;
                    Region currentRegion = node.Region;
                    Base3.RegionSig updatedRegionSignature;

                    if (currentRegion == null)
                    {
                        if (!nodeUpdate.created)
                            throw new Exception("Current region can only be null in nodes that just have been created!");

                        if (nodeUpdate.memberOfToRemove.Count > 0)
                            throw new Exception("Internal error: Cannot remove node from group, it is not a member of any groups.");

                        Base3.GroupID[] groupIDs = new QS.CMS.Base3.GroupID[nodeUpdate.memberOfToAdd.Count];
                        nodeUpdate.memberOfToAdd.CopyTo(groupIDs, 0);
                        updatedRegionSignature = new QS.CMS.Base3.RegionSig(groupIDs);
                    }
                    else
                    {
                        // sanity checking to catch potential bugs, we doublecheck everything

                        foreach (Base3.GroupID groupID in currentRegion.Signature.GroupIDs)
                        {
                            if (nodeUpdate.memberOfToAdd.Contains(groupID))
                                throw new Exception("The node is trying to join a group that it is already a member of!");
                        }

                        foreach (Base3.GroupID groupID in node.GroupIDsWhereMember)
                        {
                            if (nodeUpdate.memberOfToAdd.Contains(groupID))
                                throw new Exception("The node is trying to join a group that it is already a member of!");
                        }

                        ICollection<Base3.GroupID> temp = 
                            new System.Collections.ObjectModel.Collection<Base3.GroupID>(currentRegion.Signature.GroupIDs);
                        foreach (Base3.GroupID groupID in nodeUpdate.memberOfToRemove)
                        {
                            if (!temp.Contains(groupID) || !node.GroupIDsWhereMember.Contains(groupID))
                                throw new Exception("The node is trying to leave a group that it is not a member of!");
                        }

                        updatedRegionSignature = currentRegion.Signature.CalculateModifiedSig(
                            new List<Base3.GroupID>(nodeUpdate.memberOfToAdd), 
                            new List<Base3.GroupID>(nodeUpdate.memberOfToRemove));

                        if (!updatedRegionSignature.Equals(currentRegion.Signature))
                        {
                            regionMemberChanges[currentRegion.ID].ToRemove.Add(node);
                        }
                    }

                    // ***************************************************************( HERE )**************************************************************
                    // TODO: Add handling of "resync".
                    // * For all region views the node is currently in, it must be considered dead in the subsequent revision, which
                    //    excludes it from participating in the flushing protocol.
                    // * The groups of which it is a member should install new views. The node should still be in the views, but this time
                    //    it should be marked as one of the nodes that must receive state transfer.
                    //    
                    // 
                    // The way we enforce state transfer.
                    // a) When a group view is created, some nodes will be recipients of state transfer for this particular region view.
                    // b) This implies that for a given region view, it may have recipients, sources, or both.
                    // c) We should mark meta-nodes upon creation accordingly, so that it is clear which can play which roles. This is
                    //      assigned once to the meta-node upon creation and does not change (since a meta-node is specific to a group
                    //      view and region view) unless some nodes die. The particular revision of the meta-node may override this 
                    //      recipient/source configuration.
                    // d) A member of a region view may be source or recipient for different group views. The region view itself or its
                    //     revision does not determine which nodes play which roles.
                    // e) The information about recipients and sources must be contained directly or indirectly in the local view that
                    //      gets installed whenever group views accessing the particular region view changes. Installing a new group view
                    //      will always cause such changes of the local view in each of the region views the group view refers to.
                    //      
                    // ******* there must be something corresponding to group view for local view, which determines the set of receipients
                    //            and sources, sort of like meta-node, but giving only the local perspective, it could be modeled as e.g. channel 
                    //            group or a meta-channel, with particular sub-channels corresponding to senders within it attached to it.....
                    //
                    Region updatedRegion;

                    if (!updatedRegionSignature.CorrespondsToNoGroups)
                    {
                        if (signatures2regions.ContainsKey(updatedRegionSignature))
                        {
                            updatedRegion = signatures2regions[updatedRegionSignature];
                        }
                        else
                        {
                            updatedRegion = new Region(new QS.CMS.Base3.RegionID(++lastUsedRegionID), updatedRegionSignature);
                            signatures2regions[updatedRegionSignature] = updatedRegion;
                            regions.Add(updatedRegion.ID, updatedRegion);
                        }

                        if (!updatedRegionSignature.Equals(currentRegion.Signature))
                        {
                            regionMemberChanges[updatedRegion.ID].ToAdd.Add(node);
                            node.Region = updatedRegion;
                        }
                    }
                }

                if (nodeUpdate.clientOfToAdd.Count > 0 || nodeUpdate.clientOfToRemove.Count > 0)
                {
                    foreach (Base3.GroupID groupID in nodeUpdate.clientOfToAdd)
                    {
                        // ................................................................................................................................................
                        // this is easy, just add the client to wherever it is not known
                    }

                    foreach (Base3.GroupID groupID in nodeUpdate.clientOfToRemove)
                    {
                        // ................................................................................................................................................
                        // this is more serious, removal should cause revision changes in many places
                    }
                }
            }

            #endregion

/-*


            // ................................................................................................................................................

*-/ 
        }

        #endregion

        #region Class Update

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        public class Update
        {
            #region Constructor and fields

            public Update(Configuration configuration)
            {
                this.configuration = configuration;
            }

            private Configuration configuration;
            [QS.Fx.Printing.Printable]
            private IDictionary<Base3.GroupID, GroupUpdate> groupUpdates = new Dictionary<Base3.GroupID, GroupUpdate>();
            [QS.Fx.Printing.Printable]
            private IDictionary<QS._core_c_.Base3.InstanceID, NodeUpdate> nodeUpdates = new Dictionary<QS._core_c_.Base3.InstanceID, NodeUpdate>();

            #endregion

            #region Commit

            public void Commit(out IDictionary<QS._core_c_.Base3.InstanceID, Notifications.Notification> notifications)
            {
                notifications = new Dictionary<QS._core_c_.Base3.InstanceID, Notifications.Notification>();
                if (groupUpdates.Count > 0 || nodeUpdates.Count > 0)
                {
                    configuration.eventLogger.Log(
                        new Logging.Events.SimpleEvent2(0, null, this, "Commit", QS.Fx.Printing.Printable.ToString(this)));
                    configuration.ProcessUpdates(groupUpdates, nodeUpdates, notifications);
                }
            }

            #endregion

            #region GroupUpdate

            [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
            public class GroupUpdate
            {
                public GroupUpdate(Group group, bool created, bool deleted)
                {
                    this.group = group;
                    this.created = created;
                    this.deleted = deleted;
                    this.members = group.AddressesOfMembers;
                    this.clients = group.AddressesOfClients;
                }

                public Group group;
                [QS.Fx.Printing.Printable]
                public bool created, deleted;
                [QS.Fx.Printing.Printable]
                public ICollection<QS._core_c_.Base3.InstanceID> members;
                [QS.Fx.Printing.Printable]
                public ICollection<QS._core_c_.Base3.InstanceID> clients;
                [QS.Fx.Printing.Printable]
                public ICollection<QS._core_c_.Base3.InstanceID> membersToAdd = new System.Collections.ObjectModel.Collection<QS.CMS.QS._core_c_.Base3.InstanceID>();
                [QS.Fx.Printing.Printable]
                public ICollection<QS._core_c_.Base3.InstanceID> membersToRemove = new System.Collections.ObjectModel.Collection<QS.CMS.QS._core_c_.Base3.InstanceID>();
                [QS.Fx.Printing.Printable]
                public ICollection<QS._core_c_.Base3.InstanceID> clientsToAdd = new System.Collections.ObjectModel.Collection<QS.CMS.QS._core_c_.Base3.InstanceID>();
                [QS.Fx.Printing.Printable]
                public ICollection<QS._core_c_.Base3.InstanceID> clientsToRemove = new System.Collections.ObjectModel.Collection<QS.CMS.QS._core_c_.Base3.InstanceID>();
            }

            #endregion

            #region NodeUpdate

            [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
            public class NodeUpdate
            {
                public NodeUpdate(Node node, bool created, bool deleted, bool crashed)
                {
                    this.node = node;
                    this.created = created;
                    this.deleted = deleted;
                    this.memberOf = node.GroupIDsWhereMember;
                    this.clientOf = node.GroupIDsWhereClient;
                }

                public Node node;
                [QS.Fx.Printing.Printable]
                public bool created, deleted, crashed, resync;
                [QS.Fx.Printing.Printable]
                public ICollection<Base3.GroupID> memberOf, clientOf;
                [QS.Fx.Printing.Printable]
                public ICollection<Base3.GroupID> memberOfToAdd = new System.Collections.ObjectModel.Collection<QS.CMS.Base3.GroupID>();
                [QS.Fx.Printing.Printable]
                public ICollection<Base3.GroupID> memberOfToRemove = new System.Collections.ObjectModel.Collection<QS.CMS.Base3.GroupID>();
                [QS.Fx.Printing.Printable]
                public ICollection<Base3.GroupID> clientOfToAdd = new System.Collections.ObjectModel.Collection<QS.CMS.Base3.GroupID>();
                [QS.Fx.Printing.Printable]
                public ICollection<Base3.GroupID> clientOfToRemove = new System.Collections.ObjectModel.Collection<QS.CMS.Base3.GroupID>();
            }

            #endregion

            #region Open

            public void Open(QS._core_c_.Base3.InstanceID requestorAddress,
                Expressions.Expression groupSpecification, Interface.OpeningMode openingMode, Interface.AccessMode accessMode,
                Interface.IGroupType requestedGroupType, Interface.IGroupAttributes requestedGroupAttributes,
                out Base3.GroupID groupID, out Interface.IGroupType groupType, out Interface.IGroupAttributes groupAttributes)
            {
                configuration.eventLogger.Log(new Logging.Events.SimpleEvent2(0, null, this, "__Open", null));

                if (configuration.crashed.Contains(requestorAddress))
                    throw new Exception("The requestor is considered crashed by the GMS.");

                NodeUpdate nodeUpdate;
                if (!nodeUpdates.TryGetValue(requestorAddress, out nodeUpdate))
                {
                    Node node;
                    if (configuration.nodes.TryGetValue(requestorAddress, out node))
                    {
                        nodeUpdate = new NodeUpdate(node, false, false, false);
                    }
                    else
                    {
                        node = new Node(requestorAddress);
                        configuration.nodes.Add(requestorAddress, node);

                        nodeUpdate = new NodeUpdate(node, true, false, false);
                    }
                    nodeUpdates.Add(requestorAddress, nodeUpdate);
                }

                GroupUpdate groupUpdate;

                if (groupSpecification.Type == QS.CMS.Membership3.Expressions.ExpressionType.Group)
                {
                    string groupName = ((Expressions.Group)groupSpecification).Name;                    
                    if (configuration.names2groups.TryGetValue(groupName, out groupID))
                    {
                        if (openingMode == QS.CMS.Membership3.Interface.OpeningMode.CreateNew)
                            throw new Exception("Group already exists.");
                        
                        if (!groupUpdates.TryGetValue(groupID, out groupUpdate))
                        {
                            Group group;
                            if (!configuration.groups.TryGetValue(groupID, out group))
                                throw new Exception("Internal error: group name is known, but group id is not.");

                            groupUpdate = new GroupUpdate(group, false, false);
                            groupUpdates.Add(groupID, groupUpdate);
                        }

                        groupType = groupUpdate.group.Type;
                        groupAttributes = groupUpdate.group.Attributes;

                        if (!groupType.Accepts(groupAttributes, requestedGroupType, requestedGroupAttributes))
                            throw new Exception("Group type or attribute mismatch.");
                    }
                    else
                    {
                        if (openingMode == QS.CMS.Membership3.Interface.OpeningMode.Open)
                            throw new Exception("Group does not exist.");

                        groupID = new QS.CMS.Base3.GroupID((uint)(++configuration.lastUsedGroupID));
                        Group group = new Group(groupName, null, groupID, requestedGroupType, requestedGroupAttributes);
                        configuration.groups.Add(groupID, group);

                        configuration.names2groups.Add(groupName, groupID);

                        groupUpdate = new GroupUpdate(group, true, false);
                        groupUpdates.Add(groupID, groupUpdate);

                        groupType = requestedGroupType;
                        groupAttributes = requestedGroupAttributes;
                    }                     
                }
                else
                {
                    if (openingMode != QS.CMS.Membership3.Interface.OpeningMode.Open)
                        throw new Exception("Cannot explicitly create a derived group.");

                    if ((accessMode & QS.CMS.Membership3.Interface.AccessMode.Member) ==
                        QS.CMS.Membership3.Interface.AccessMode.Member)
                        throw new Exception("Cannot explicitly join a derived group as a member.");

                    if (configuration.expressions2groups.TryGetValue(groupSpecification, out groupID))
                    {
                        if (!groupUpdates.TryGetValue(groupID, out groupUpdate))
                        {
                            Group group;
                            if (!configuration.groups.TryGetValue(groupID, out group))
                                throw new Exception("Internal error: group signature is known, but group id is not.");

                            groupUpdate = new GroupUpdate(group, false, false);
                            groupUpdates.Add(groupID, groupUpdate);
                        }

                        groupType = groupUpdate.group.Type;
                        groupAttributes = groupUpdate.group.Attributes;

                        if (!groupType.Accepts(groupAttributes, requestedGroupType, requestedGroupAttributes))
                            throw new Exception("Group type or attribute mismatch.");
                    }
                    else
                    {
                        groupID = new QS.CMS.Base3.GroupID((uint)(++configuration.lastUsedGroupID));

/-*
                        Types.Derivations.DeriveType(groupSpecification,
                            new QS.CMS.Membership3.Types.Derivations.GetTypeCallback(
                                delegate(string s, out Interface.IGroupType t, out Interface.IGroupAttributes a)
                                {
                                    Base3.GroupID gid;
                                    Group g;
                                    if (!configuration.names2groups.TryGetValue(s, out gid) ||
                                        !configuration.groups.TryGetValue(gid, out g))
                                        throw new Exception("Cannot find group \"" + s + "\".");
                                    t = g.Type;
                                    a = g.Attributes;
                                }), out derivedType, out derivedAttributes);
*-/

                        // TODO: Later we should do some type derivation, but for now we ignore this and just assign here a null, meaningless type.............
                        groupType = null;
                        groupAttributes = null;

                        // TODO: We should also check if the requested type is compliant with this, but we also ignore it.........

                        Group group = new Group(null, groupSpecification, groupID, groupType, groupAttributes);
                        configuration.groups.Add(groupID, group);

                        configuration.expressions2groups.Add(groupSpecification, groupID);

                        groupUpdate = new GroupUpdate(group, true, false);
                        groupUpdates.Add(groupID, groupUpdate);
                    }
                }
                
                if ((accessMode & QS.CMS.Membership3.Interface.AccessMode.Member) == QS.CMS.Membership3.Interface.AccessMode.Member)
                {
                    if (nodeUpdate.memberOfToAdd.Contains(groupID))
                        throw new Exception("Already a member of the group.");

                    if (!nodeUpdate.memberOfToRemove.Remove(groupID))
                    {
                        if (nodeUpdate.memberOf.Contains(groupID))
                            throw new Exception("Already a member of the group.");
                        else
                            nodeUpdate.memberOfToAdd.Add(groupID);
                    }

                    if (groupUpdate.membersToAdd.Contains(requestorAddress))
                        throw new Exception("Already a member of the group.");

                    if (!groupUpdate.membersToRemove.Remove(requestorAddress))
                    {
                        if (groupUpdate.members.Contains(requestorAddress))
                            throw new Exception("Already a member of the group.");
                        else
                            groupUpdate.membersToAdd.Add(requestorAddress);
                    }
                }

                if ((accessMode & QS.CMS.Membership3.Interface.AccessMode.Client) == QS.CMS.Membership3.Interface.AccessMode.Client)
                {
                    if (nodeUpdate.clientOfToAdd.Contains(groupID))
                        throw new Exception("Already a client of the group.");

                    if (!nodeUpdate.clientOfToRemove.Remove(groupID))
                    {
                        if (nodeUpdate.clientOf.Contains(groupID))
                            throw new Exception("Already a client of the group.");
                        else
                            nodeUpdate.clientOfToAdd.Add(groupID);
                    }

                    if (groupUpdate.clientsToAdd.Contains(requestorAddress))
                        throw new Exception("Already a client of the group.");

                    if (!groupUpdate.clientsToRemove.Remove(requestorAddress))
                    {
                        if (groupUpdate.clients.Contains(requestorAddress))
                            throw new Exception("Already a client of the group.");
                        else
                            groupUpdate.clientsToAdd.Add(requestorAddress);
                    }
                }
            }

            #endregion

            #region Close

            public void Close(QS._core_c_.Base3.InstanceID requestorAddress, Base3.GroupID groupID)
            {
                configuration.eventLogger.Log(new Logging.Events.SimpleEvent2(0, null, this, "__Close", null));

                if (configuration.crashed.Contains(requestorAddress))
                    throw new Exception("The requestor is considered crashed by the GMS.");

                NodeUpdate nodeUpdate;
                if (!nodeUpdates.TryGetValue(requestorAddress, out nodeUpdate))
                {
                    Node node;
                    if (configuration.nodes.TryGetValue(requestorAddress, out node))
                    {
                        nodeUpdate = new NodeUpdate(node, false, false, false);
                    }
                    else
                        throw new Exception("Node is not known to this GMS.");
                }

                if (!nodeUpdate.memberOfToRemove.Contains(groupID) &&
                    !nodeUpdate.memberOfToAdd.Remove(groupID) && nodeUpdate.memberOf.Contains(groupID))
                    nodeUpdate.memberOfToRemove.Add(groupID);

                if (!nodeUpdate.clientOfToRemove.Contains(groupID) &&
                    !nodeUpdate.clientOfToAdd.Remove(groupID) && nodeUpdate.clientOf.Contains(groupID))
                    nodeUpdate.clientOfToRemove.Add(groupID);

                GroupUpdate groupUpdate;
                if (!groupUpdates.TryGetValue(groupID, out groupUpdate))
                {
                    Group group;
                    if (configuration.groups.TryGetValue(groupID, out group))
                    {
                        groupUpdate = new GroupUpdate(group, false, false);
                    }
                    else
                        throw new Exception("Group does not exist.");
                }

                if (!groupUpdate.membersToRemove.Contains(requestorAddress) &&
                    !groupUpdate.membersToAdd.Remove(requestorAddress) && groupUpdate.members.Contains(requestorAddress))
                    groupUpdate.membersToRemove.Add(requestorAddress);

                if (!groupUpdate.clientsToRemove.Contains(requestorAddress) &&
                    !groupUpdate.clientsToAdd.Remove(requestorAddress) && groupUpdate.clients.Contains(requestorAddress))
                    groupUpdate.clientsToRemove.Add(requestorAddress);                
            }

            #endregion

            #region Crash

            public void Crash(QS._core_c_.Base3.InstanceID requestorAddress, QS._core_c_.Base3.InstanceID[] deadAddresses)
            {
                configuration.eventLogger.Log(new Logging.Events.SimpleEvent2(0, null, this, "__Crash", null));

                if (configuration.crashed.Contains(requestorAddress))
                    throw new Exception("The requestor is considered crashed by the GMS.");

                foreach (QS._core_c_.Base3.InstanceID address in deadAddresses)
                {
                    try
                    {
                        NodeUpdate nodeUpdate;
                        if (nodeUpdates.TryGetValue(address, out nodeUpdate))
                        {
                            nodeUpdate.crashed = true;
                        }
                        else
                        {
                            Node node;
                            if (configuration.nodes.TryGetValue(address, out node))
                            {
                                nodeUpdate = new NodeUpdate(node, false, false, true);
                            }
                            else
                                throw new Exception("Node is not known to this GMS.");
                        }

                        configuration.crashed.Add(address);

                        foreach (Base3.GroupID groupID in nodeUpdate.memberOfToAdd)
                            groupUpdates[groupID].membersToAdd.Remove(address);
                        nodeUpdate.memberOfToAdd.Clear();

                        foreach (Base3.GroupID groupID in nodeUpdate.clientOfToAdd)
                            groupUpdates[groupID].clientsToAdd.Remove(address);
                        nodeUpdate.clientOfToAdd.Clear();

                        foreach (Base3.GroupID groupID in nodeUpdate.memberOf)
                        {
                            if (!nodeUpdate.memberOfToRemove.Contains(groupID))
                            {
                                nodeUpdate.memberOfToRemove.Add(groupID);
                                groupUpdates[groupID].membersToRemove.Add(address);
                            }
                        }

                        foreach (Base3.GroupID groupID in nodeUpdate.clientOf)
                        {
                            if (!nodeUpdate.clientOfToRemove.Contains(groupID))
                            {
                                nodeUpdate.clientOfToRemove.Add(groupID);
                                groupUpdates[groupID].clientsToRemove.Add(address);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            #endregion

            #region Resync

            public void Resync(QS._core_c_.Base3.InstanceID requestorAddress)
            {
                configuration.eventLogger.Log(new Logging.Events.SimpleEvent2(0, null, this, "__Resync", null));

                if (configuration.crashed.Contains(requestorAddress))
                    throw new Exception("The requestor is considered crashed by the GMS.");

                NodeUpdate nodeUpdate;
                if (!nodeUpdates.TryGetValue(requestorAddress, out nodeUpdate))
                {
                    Node node;
                    if (configuration.nodes.TryGetValue(requestorAddress, out node))
                    {
                        nodeUpdate = new NodeUpdate(node, false, false, false);
                    }
                    else
                        throw new Exception("Node not known to the GMS.");
                }

                nodeUpdate.resync = true;
            }

            #endregion
        }

        #endregion
    }
*/
}
