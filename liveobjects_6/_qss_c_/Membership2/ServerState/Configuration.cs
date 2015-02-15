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

// #define OPTION_DisableMulticastPerGroup
// #define DEBUG_Configuration

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Membership2.ServerState
{
    public class Configuration : IConfiguration
    {
        #region Constructor

        public Configuration(Allocation.IAddressPool addressPool, QS.Fx.Logging.ILogger logger, bool allocateMulticastAddressPerGroup)
        {
            this.addressPool = addressPool;
            this.logger = logger;
            this.allocateMulticastAddressPerGroup = allocateMulticastAddressPerGroup;

            nodes = new Dictionary<QS._core_c_.Base3.InstanceID, Node>();
            regions_bysig = new Dictionary<QS._qss_c_.Base3_.RegionSig, Region>();
            groups = new Dictionary<QS._qss_c_.Base3_.GroupID, Group>();
            regions = new Dictionary<QS._qss_c_.Base3_.RegionID, Region>();
        }

        #endregion

        #region Fields

        private Dictionary<QS._core_c_.Base3.InstanceID, Node> nodes;
        private Dictionary<Base3_.RegionSig, Region> regions_bysig;
        private Dictionary<Base3_.GroupID, Group> groups;
        private Dictionary<Base3_.RegionID, Region> regions;

        private ulong numberOfRegionIDsAssigned = 0;
        private QS.Fx.Logging.ILogger logger;
        private Allocation.IAddressPool addressPool;
        private bool allocateMulticastAddressPerGroup;

        #endregion

        #region IConfiguration Members

        public void Change(IEnumerable<Protocol.IMembershipChangeRequest> requests,
            ref Components_1_.AutomaticCollection<QS._core_c_.Base3.InstanceID, Protocol.Notification> notifications)
        {
            lock (this)
            {
// #if DEBUG_Configuration
//                logger.Log(this, "__Configuration.Change: Begin");
// #endif

// #if DEBUG_Configuration
//                foreach (Protocol.IMembershipChangeRequest request in requests)
//                    logger.Log(this, "Request: " + request.ToString());
// #endif 

                Components_1_.AutomaticCollection<Base3_.RegionID, Components_1_.SetChange<Node>> regionChanges =
                    new Components_1_.AutomaticCollection<Base3_.RegionID, Components_1_.SetChange<Node>>();

                // Request to join/leave and crash notifications are first translated to changes per region: what nodes enter/leave a given region
                foreach (Protocol.IMembershipChangeRequest request in requests)
                {
                    Node node;
                    if (nodes.ContainsKey(request.InstanceID))
                    {
                        node = nodes[request.InstanceID];
                    }
                    else
                    {
                        // New node gets created
                        node = new Node(request.InstanceID);
                        nodes[request.InstanceID] = node;
                    }

                    Region region; // Original region of the node
                    Base3_.RegionSig regionSig;

                    bool ignore_request = false;

                    if ((region = node.Region) == null) // New node will have its region set to null as it has not been initialized yet
                    {
                        if (request.ToLeave.Count > 0)
                            throw new Exception("Node " + request.InstanceID.ToString() +
                                " requested to leave >0 groups, but it's not known to belong to any groups at the moment!");

                        // Calculate signature for the new node based on join requests
                        regionSig = new QS._qss_c_.Base3_.RegionSig(QS._core_c_.Helpers.CollectionHelper.List2Array<Base3_.GroupID>(request.ToJoin));
                    }
                    else
                    {
                        // do not need to rejoin groups we're already a member of
                        foreach (Base3_.GroupID groupID in node.Region.RegionSig.GroupIDs)
                            request.ToJoin.Remove(groupID);


                        // Calculate the modified signature of the existing node based on its join/leave requests
                        if (request.Crashed)
                            regionSig = new QS._qss_c_.Base3_.RegionSig(QS._core_c_.Helpers.CollectionHelper.List2Array<Base3_.GroupID>(request.ToJoin));
                        else
                            regionSig = region.RegionSig.CalculateModifiedSig(request.ToJoin, request.ToLeave);

                        if (!regionSig.Equals(node.Region.RegionSig))
                        {
                            regionChanges[region.RegionID].ToRemove.Add(node); // Specify that we should remove node from original region

                            // FIX:5/3/2007 --> we mark region for deletion, but we will reintroduce it if we run into region updates
                            notifications[node.InstanceID].RegionsToDelete.Add(region.RegionID);
                        }
                        else // no need to do anything, this request does not affect node membership
                        {
                            ignore_request = true;
                        }
                    }

                    if (!ignore_request)
                    {
                        if (regionSig.GroupIDs.Length > 0)
                        {
                            if (regions_bysig.ContainsKey(regionSig))
                            {
                                region = regions_bysig[regionSig];
                            }
                            else
                            {
                                // Create region if it didn't exist yet, it means new nodes just moved there, region will appear as new element in group views
                                region = new Region(new Base3_.RegionID(++numberOfRegionIDsAssigned), regionSig, addressPool.AllocateAddress);
                                regions_bysig[regionSig] = region;
                                regions[region.RegionID] = region;
                            }
                        }
                        else
                            region = null;

                        if (region != null && (!request.Crashed || request.ToJoin.Count > 0))
                        {
                            // Unless the node is crashed, specify that it should be added to the new region
                            regionChanges[region.RegionID].ToAdd.Add(node);
                            node.Region = region;
                        }
                        else
                        {
                            // Node is crashed, there is no need to keep track of it any more
                            if (request.Crashed)
                                nodes.Remove(request.InstanceID);
                            else
                                node.Region = null;
                        }
                    }
                }

// #if DEBUG_Configuration
//                 foreach (KeyValuePair<Base3.RegionID, Components.SetChange<Node>> regionChange in regionChanges.Collection)
//                     logger.Log(this, "RegionChange: " + regionChange.Key.ToString() + regionChange.Value.ToString());
// #endif 

                Components_1_.AutomaticCollection<Base3_.GroupID, Components_1_.SetChangeWithUpdates<RegionView>> groupChanges =
                    new Components_1_.AutomaticCollection<Base3_.GroupID, Components_1_.SetChangeWithUpdates<RegionView>>();

                // Changes in regions will result in modifying region structure; they will now translate into requests for modifications of group views
                foreach (KeyValuePair<Base3_.RegionID, Components_1_.SetChange<Node>> regionChange in regionChanges.Collection)
                {
                    // Get hold of the region to be change; note new regions are already there, preinitialized by the preceding phase
                    Region region = regions[regionChange.Key];
                    Components_1_.SetChange<Node> nodeChanges = regionChange.Value;

                    RegionView currentView = region.CurrentView;
                    RegionView updatedView = null;

                    if (currentView == null)
                    {
                        // This is a new region, just create the initial view based on requests to add nodes
                        updatedView = new RegionView(region, 1, nodeChanges.ToAdd);
                    }
                    else // This is an existing region, so we will need a region view change
                    {
                        // If we add nodes or don't remove everything in the region, region is modified: doesn't get deleted
                        if (nodeChanges.ToAdd.Count > 0 || nodeChanges.ToRemove.Count < currentView.Nodes.Count)
                        {
                            // Calculate the difference, create the new view
                            updatedView = currentView.UpdatedView(nodeChanges.ToAdd, nodeChanges.ToRemove);

// #if DEBUG_Configuration
//                            logger.Log(this, "__________UpdateView: base view " + currentView.ToString() + ", add " + 
//                                Helpers.CollectionHelper.ToStringSeparated<Node>(nodeChanges.ToAdd, ",") + ", remove " +
//                                Helpers.CollectionHelper.ToStringSeparated<Node>(nodeChanges.ToRemove, ",") + ", yields " + updatedView.ToString());
// #endif

                        }
                        else // Region will disappear, everything in it is deleted and nothing is added
                        {
                            regions.Remove(region.RegionID);
                            regions_bysig.Remove(region.RegionSig);

                            addressPool.RecycleAddress(region.Address);
                        }
                    }

                    // Adjust the current region view, this completes region update
                    region.CurrentView = updatedView;

                    // Iterate over all groups in region signature, those groups are affected by change in this region
                    foreach (Base3_.GroupID groupID in region.RegionSig.GroupIDs)
                    {
                        Components_1_.SetChangeWithUpdates<RegionView> sch = groupChanges[groupID]; // get reference to the modified spec
                        if (currentView != null) // Not a new region, existed before
                        {
                            if (updatedView != null) // Region is modified, so the group switches old version of it to a new version of it, request group mod
                            {
                                sch.ToUpdate.Add(new Base3_.Pair<RegionView>(currentView, updatedView));
                            }
                            else
                            {
                                sch.ToRemove.Add(currentView); // Region disappears, so remove it from the group

                                // FIX 5/7/2007
                                // we will now iterate over all members of the current views that could have been referencing this region and ask them to remove the region
                                Base3_.RegionID _rid = currentView.Region.RegionID;
                                foreach (Base3_.GroupID _gid in currentView.Region.RegionSig.GroupIDs)
                                {
                                    Group _g;
                                    if (groups.TryGetValue(_gid, out _g))
                                    {
                                        foreach (RegionView _rv in _g.CurrentView.SubViews)
                                        {
                                            foreach (Node _n in _rv.Nodes)
                                            {
                                                List<Base3_.RegionID> _nn = notifications[_n.InstanceID].RegionsToDelete;
                                                if (!_nn.Contains(_rid))
                                                    _nn.Add(_rid);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else // This is a new region view, hence the region didn't exist, unless it's empty request it to be added to the group
                        {
                            if (updatedView != null && !sch.ToAdd.Contains(updatedView))
                                sch.ToAdd.Add(updatedView);
                        }
                    }
                }

// #if DEBUG_Configuration
//                foreach (KeyValuePair<Base3.GroupID, Components.SetChangeWithUpdates<RegionView>> groupChange in groupChanges.Collection)
//                    logger.Log(this, "GroupChange: " + groupChange.Key.ToString() + groupChange.Value.ToString());
// #endif 

                // These are lists of nodes that need the full spec of changes in given regions (as opposed to just diffs of regions they already know)
                Components_1_.AutomaticCollection<Base3_.RegionID, System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>> addregion_consumers =
                    new QS._qss_c_.Components_1_.AutomaticCollection<QS._qss_c_.Base3_.RegionID, System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>>();

                // These are list of nodes that need info about region update
                Components_1_.AutomaticCollection<Base3_.RegionID, System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>> updateregion_consumers =
                    new QS._qss_c_.Components_1_.AutomaticCollection<QS._qss_c_.Base3_.RegionID, System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>>();

                foreach (KeyValuePair<Base3_.GroupID, Components_1_.SetChangeWithUpdates<RegionView>> groupChange in groupChanges.Collection)
                {
                    Group group;
                    if (groups.ContainsKey(groupChange.Key))
                        group = groups[groupChange.Key];
                    else
                    {
                        // Create groups if necessary
                        group = new Group(groupChange.Key
// #if !OPTION_DisableMulticastPerGroup
                            , (allocateMulticastAddressPerGroup ? addressPool.AllocateAddress : new QS.Fx.Network.NetworkAddress("0.0.0.0:0"))
// #endif
                            );
                        groups[groupChange.Key] = group;
                    }
                    Components_1_.SetChangeWithUpdates<RegionView> subviewChanges = groupChange.Value; // get ref to the specification what to mod

                    GroupView currentView = group.CurrentView;
                    GroupView updatedView = null;

                    if (currentView == null) // This is a new group
                    {
                        // sanity check: cannot remove from a new empty group
                        if (subviewChanges.ToRemove.Count > 0 || subviewChanges.ToUpdate.Count > 0)
                            throw new Exception("No current view in this group, cannot delete/update regions!");

                        updatedView = new GroupView(group, 1, subviewChanges.ToAdd); // just take the added region views

                        // May not be the best place for this: all nodes in this region are first learning about the region (it must be that this region didn't 
                        // exist), so we specify that these nodes should get the full spec of the region in their membership change notification.
                        // We do it here because that we need to notify not only nodes in given regions but also nodes in other regions that happen to 
                        // share the same groups
                        foreach (RegionView regionView in subviewChanges.ToAdd)
                        {
                            ICollection<QS._core_c_.Base3.InstanceID> toAdd = addregion_consumers[regionView.ViewID.ID];
                            foreach (Node node in updatedView.Nodes)
                                if (!toAdd.Contains(node.InstanceID))
                                    toAdd.Add(node.InstanceID);
                        }
                    }
                    else // Group is not new, it's being changed
                    {
                        if (subviewChanges.ToAdd.Count > 0 || subviewChanges.ToUpdate.Count > 0 || subviewChanges.ToRemove.Count < currentView.SubViews.Count)
                        { 
                            // This is executed when group is not emptied of all regions, so the group doesn't need to be deleted

                            IList<Node> nodes_removed, nodes_added, nodes_unaffected;
                            updatedView = currentView.CalculateUpdatedView(
                                subviewChanges, out nodes_removed, out nodes_added, out nodes_unaffected);

// #if DEBUG_Configuration
//                            logger.Log(this, "__________CalculateUpdatedView\n{\n__OLD_VIEW:\n" + 
//                                currentView.ToString() + "\n__NEW_VIEW:\n" + updatedView.ToString() + "\n__NODES_REMOVED:\n" +
//                                Helpers.CollectionHelper.ToStringSeparated<Node>(nodes_removed, ",") + "\n__NODES_ADDED:\n" +
//                                Helpers.CollectionHelper.ToStringSeparated<Node>(nodes_added, ",") + "\n__NODES_UNAFFECTED:\n" +
//                                Helpers.CollectionHelper.ToStringSeparated<Node>(nodes_unaffected, ",") + "\n}");
// #endif

                            // nodes joining the group receive full info about regions that the group previously consisted of
                            foreach (RegionView rv in currentView.SubViews)
                            {
                                foreach (Node n in nodes_added)
                                    addregion_consumers[rv.Region.RegionID].Add(n.InstanceID);

                                foreach (Node n in nodes_removed)
                                {
                                    bool remove = true;
                                    
                                    if (n.Region != null)
                                    {
                                        foreach (Base3_.GroupID _gid in n.Region.RegionSig.GroupIDs)
                                        {
                                            Group _g;
                                            if (groups.TryGetValue(_gid, out _g))
                                            {
                                                foreach (RegionView __rv in _g.CurrentView.SubViews)
                                                {
                                                    if (__rv.Region.RegionID.Equals(rv.Region.RegionID))
                                                    {
                                                        remove = false;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (!remove)
                                                break;
                                        }
                                    }

                                    if (remove)
                                    {
                                        if (!notifications[n.InstanceID].RegionsToDelete.Contains(rv.Region.RegionID))
                                            notifications[n.InstanceID].RegionsToDelete.Add(rv.Region.RegionID);
                                    }
                                }
                            }

                            // This is a group update spec to be distributed to all nodes that need a diff (don't join or leave)
                            Protocol.Notification.GroupInfo_ToUpdate to_update = new Protocol.Notification.GroupInfo_ToUpdate(updatedView.ViewID);

                            foreach (RegionView regionView in subviewChanges.ToAdd)
                            {
                                to_update.ToAdd.Add(regionView.ViewID); // new region appears, reflect it in the diff

                                // None of the nodes in this group knew about this region, so we include all nodes in the group in the list of 
                                // nodes that need to receive full spec of the new region
                                ICollection<QS._core_c_.Base3.InstanceID> toAdd = addregion_consumers[regionView.ViewID.ID];
                                foreach (Node node in updatedView.Nodes)
                                    if (!toAdd.Contains(node.InstanceID))
                                        toAdd.Add(node.InstanceID);
                            }

                            // add region view updates in the group view to the distributed diff
                            foreach (Base3_.Pair<RegionView> update_pair in subviewChanges.ToUpdate)
                            {
                                to_update.ToUpdate.Add(update_pair.Element2.ViewID);

                                // Again, none of these nodes knew about the region
                                ICollection<QS._core_c_.Base3.InstanceID> toAdd = addregion_consumers[update_pair.Element2.ViewID.ID];
                                foreach (Node node in nodes_added)
                                    if (!toAdd.Contains(node.InstanceID))
                                        toAdd.Add(node.InstanceID);
                            }

                            // add region view deletions to the diff
                            foreach (RegionView regionView in subviewChanges.ToRemove)
                                to_update.ToRemove.Add(regionView.ViewID.ID);

                            // All nodes that have not been added or removed will get the diff describing update done to the group as well as
                            // updates for all the modified regions that stay within the group
                            foreach (Node node in nodes_unaffected)
                            {
                                notifications[node.InstanceID].GroupsChanged.Add(to_update);

                                foreach (Base3_.Pair<RegionView> update_pair in subviewChanges.ToUpdate)
                                {
                                    System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID> nodes_to_notify =
                                        updateregion_consumers[update_pair.Element1.Region.RegionID];
                                    if (!nodes_to_notify.Contains(node.InstanceID))
                                    {
                                        nodes_to_notify.Add(node.InstanceID);

//                                        // FIX:5/7/2007 --> if we marked region for deletion, now we reintroduce it after learning that it shouldn't have been deleted after all
//                                        notifications[node.InstanceID].RegionsToDelete.Remove(update_pair.Element1.Region.RegionID);

                                    }
                                }
                            }

                            // - need to notify nodes they may leave regions
                        }
                        else
                        {
                            // The group gets deleted, just delete it
                            groups.Remove(group.GroupID);
                        }
                    }

                    group.CurrentView = updatedView;
                }

// #if DEBUG_Configuration
//                foreach (KeyValuePair<Base3.RegionID, List<QS._core_c_.Base3.InstanceID>> addregion_consumer in addregion_consumers.Collection)
//                    logger.Log(this, "AddRegion_Consumer: " + addregion_consumer.Key.ToString() + 
//                        Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(addregion_consumer.Value, ", "));
// #endif 

                foreach (KeyValuePair<Base3_.RegionID, System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>> item in updateregion_consumers.Collection)
                {
                    Protocol.Notification.RegionInfo_ToUpdate info =
                        new QS._qss_c_.Membership2.Protocol.Notification.RegionInfo_ToUpdate(regions[item.Key].CurrentView.ViewID);
                    Components_1_.SetChange<Node> regchg = regionChanges[item.Key];
                    foreach (Node node in regchg.ToAdd)
                        info.ToAdd.Add(node.InstanceID);
                    foreach (Node node in regchg.ToRemove)
                        info.ToRemove.Add(node.InstanceID);

                    foreach (QS._core_c_.Base3.InstanceID instanceID in item.Value)
                        notifications[instanceID].RegionsChanged.Add(info);
                }

                foreach (Protocol.IMembershipChangeRequest request in requests)
                {
                    Protocol.Notification notification = notifications[request.InstanceID];

                    // For all nodes that requested to join groups, send them full spec of the groups they've joined
                    foreach (Base3_.GroupID groupID in request.ToJoin)
                    {
                        Group group = groups[groupID];

                        notification.GroupsToCreate.Add(new Protocol.Notification.GroupInfo_Complete(
                            group.CurrentView.ViewID, group.CurrentView.SubViewIDs
// #if !OPTION_DisableMulticastPerGroup
                            , group.MulticastAddress
// #endif                            
                            ));
                    }

                    // For all nodes that requested to leave groups, notify them they're leaving these groups
                    foreach (Base3_.GroupID groupID in request.ToLeave)
                    {
                        notification.GroupsToDelete.Add(groupID);
                    }
                }

                // For all nodes that need new region info (as calculated previously), send them full region specs
                foreach (KeyValuePair<Base3_.RegionID, System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>> item in addregion_consumers.Collection)
                {
                    Region region = regions[item.Key];
                    Protocol.Notification.RegionInfo_Complete info = 
                        new Protocol.Notification.RegionInfo_Complete(region.CurrentView.ViewID, region.RegionSig, region.Address);
                    foreach (Node node in region.CurrentView.Nodes)
                        info.Members.Add(node.InstanceID);

                    logger.Log(this, "Region " + region.RegionID.ToString() + ", notifying nodes { " + 
                        QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(item.Value, ",") + " }");

                    foreach (QS._core_c_.Base3.InstanceID instanceID in item.Value)
                    {
                        // logger.Log(this, "Adding to " + instanceID.ToString() + " region_add " + region.RegionID.ToString());
                        notifications[instanceID].RegionsToCreate.Add(info);
                    }
                }

                // Place instance identifiers and sequence numbers in notifications
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification> notification in notifications.Collection)
                {
                    notification.Value.ReceiverIID = notification.Key;
                    if (nodes.ContainsKey(notification.Key))
                    {
                        Node node = nodes[notification.Key];
                        node.NotificationsSent = notification.Value.SequenceNo = node.NotificationsSent + 1;
                    }
                    else
                    {
                        // Must be that we just removed the node, so nothing gets sent to it anyway
                    }
                }

// #if DEBUG_Configuration
//                logger.Log(this, "__Configuration.Change: End");
// #endif

                // FIX 5/7/2007 --> don't send incremental update and full configuration both at the same time; always use full config if unsure
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification> notification in notifications.Collection)
                {
                    List<Protocol.Notification.RegionInfo_ToUpdate> _u_i_todelete = new List<QS._qss_c_.Membership2.Protocol.Notification.RegionInfo_ToUpdate>();
                    foreach (Protocol.Notification.RegionInfo_Complete _u_c in notification.Value.RegionsToCreate)
                    {
                        foreach (Protocol.Notification.RegionInfo_ToUpdate _u_i in notification.Value.RegionsChanged)
                        {
                            if (_u_i.View.Equals(_u_c.View))
                                _u_i_todelete.Add(_u_i);
                        }
                    }

                    foreach (Protocol.Notification.RegionInfo_ToUpdate _u_i in _u_i_todelete)
                        notification.Value.RegionsChanged.Remove(_u_i);
                }

                // FIX 5/7/2007 --> don't send request to remove regions that shouldn't actually be removed because other nodes are in them
                foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification> notification in notifications.Collection)
                {
                    Node _n;
                    if (nodes.TryGetValue(notification.Key, out _n))
                    {
                        if (_n.Region != null)
                        {
                            List<Base3_.RegionID> _rid_canceldelete = new List<QS._qss_c_.Base3_.RegionID>();
                            
                            foreach (Base3_.RegionID _rid in notification.Value.RegionsToDelete)
                            {
                                Region _r;
                                if (regions.TryGetValue(_rid, out _r))
                                {
                                    if (_r.CurrentView.Nodes.Count > 0)
                                    {
                                        bool interestoverlap = false;
                                        foreach (Base3_.GroupID _gid in _r.RegionSig.GroupIDs)
                                        {
                                            foreach (Base3_.GroupID __gid in _n.Region.RegionSig.GroupIDs)
                                            {
                                                if (__gid.Equals(_gid))
                                                {
                                                    interestoverlap = true;
                                                    break;
                                                }
                                            }
                                            if (interestoverlap)
                                                break;
                                        }

                                        if (interestoverlap)
                                        {
                                            _rid_canceldelete.Add(_rid);
                                        }
                                    }
                                }
                            }

                            foreach (Base3_.RegionID _rid in _rid_canceldelete)
                                notification.Value.RegionsToDelete.Remove(_rid);
                        }
                    }
                }
            }
        }

        // DONE: creation of a group, deletion of a group, group modification, creation of a region
        // TODO: deletion of a region, region modification

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("ServerState.Configuration\n\n");
/*
            s.AppendLine("Groups:\n");
            foreach (Group group in groups.Values)
                s.AppendLine(group.ToString());
            s.AppendLine("Regions:\n");
            foreach (Region region in regions.Values)
                s.AppendLine(region.ToString());
            s.AppendLine("Nodes:\n");
            foreach (Node node in nodes.Values)
                s.AppendLine(node.ToString());
            s.AppendLine();
*/ 
            return s.ToString();
        }

        #endregion
    }
}
