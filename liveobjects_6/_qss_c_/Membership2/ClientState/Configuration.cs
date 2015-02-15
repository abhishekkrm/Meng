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

#define DEBUG_Configuration

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Membership2.ClientState
{
    public abstract class Configuration<GroupClass, GroupViewClass, RegionClass, RegionViewClass> 
		: QS.Fx.Inspection.Inspectable, IConfiguration, Consumers.IRegionChangeProvider, Consumers.IGroupChangeProvider,
        Consumers.IGroupCreationAndRemovalProvider
        where GroupClass : class, IGroup 
        where GroupViewClass : class, IGroupView
        where RegionClass : class, IRegion
        where RegionViewClass : class, IRegionView
    {
        protected Configuration(QS.Fx.Logging.ILogger logger)
        {
			this.logger = logger;

			regions = new Dictionary<Base3_.RegionID, RegionClass>();
            groups = new Dictionary<Base3_.GroupID, GroupClass>();

			regions_inspectableProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RegionID, RegionClass>("Regions", regions,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RegionID,RegionClass>.ConversionCallback(Base3_.RegionID.FromString));
			groups_inspectableProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.GroupID, GroupClass>("Groups", groups,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.GroupID, GroupClass>.ConversionCallback(Base3_.GroupID.FromString));

            nodeToRegionViewMapping = new Dictionary<QS._core_c_.Base3.InstanceID, RegionViewClass>();
		}

		protected QS.Fx.Logging.ILogger logger;

		[QS.Fx.Base.Inspectable("Regions", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RegionID, RegionClass> regions_inspectableProxy;
		[QS.Fx.Base.Inspectable("Groups", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.GroupID, GroupClass> groups_inspectableProxy;

		protected IDictionary<Base3_.RegionID, RegionClass> regions;
        protected IDictionary<Base3_.GroupID, GroupClass> groups;

        protected IDictionary<QS._core_c_.Base3.InstanceID, RegionViewClass> nodeToRegionViewMapping;

        protected abstract RegionClass createRegion(Base3_.RegionID regionID, Base3_.RegionSig regionSig, QS.Fx.Network.NetworkAddress multicastAddress);
        protected abstract RegionViewClass createRegionView(RegionClass region, uint seqNo, QS._core_c_.Base3.InstanceID[] members);
        protected abstract GroupClass createGroup(Base3_.GroupID groupID
#if OPTION_EnableMulticastPerGroup
            , QS.Fx.Network.NetworkAddress multicastAddress
#endif
            );
        protected abstract GroupViewClass createGroupView(GroupClass group, uint seqNo, RegionViewClass[] regionViews);
        protected abstract void deleteGroup(GroupClass group);
        protected abstract void deleteRegion(RegionClass region);        

        private event Consumers.RegionChangedCallback onChange;
        private event Consumers.GroupChangedCallback onGroupChange;
        private event Consumers.GroupCreationOrRemovalCallback onGroupCreationOrRemoval;

		private QS._core_c_.Base3.InstanceID localInstanceID;
		protected QS._core_c_.Base3.InstanceID LocalInstanceID
		{
			set { localInstanceID = value; }
		}

		private Base3_.RegionID myRID;

		public Base3_.RegionID MyRID
		{
			get { return myRID; }
			private set { myRID = value; }
		}

        private IRegionView myRegionView;

        public IRegionView MyRegionView
        {
            get { return myRegionView; }
        }

        #region IConfiguration Members

        IEnumerable<IRegion> IConfiguration.NeighboringRegions
        {
            get
            {
                List<IRegion> neighboringRegions = new List<IRegion>();
                foreach (RegionClass region in regions.Values)
                    neighboringRegions.Add(region);
                return neighboringRegions;                    
            }
        }

        public void Change(QS._qss_c_.Membership2.Protocol.Notification notification)
        {
            // TODO: Process crashes in regions somehow..................

            lock (this)
            {
                Consumers.RegionChange regionChange = new Consumers.RegionChange();
                List<Consumers.GroupChange> groupChanges = new List<Consumers.GroupChange>();
                List<Consumers.GroupCreationOrRemoval> groupCreationsAndRemovals = 
                    new List<QS._qss_c_.Membership2.Consumers.GroupCreationOrRemoval>();

#if DEBUG_Configuration
                logger.Log(this, "\n__________Change:\n" + notification.ToString() + "\n");
#endif

				try
				{
					foreach (Protocol.Notification.RegionInfo_Complete region_info in notification.RegionsToCreate)
					{
						Base3_.RegionID regionID = region_info.View.ID;
                        if (!regions.ContainsKey(regionID))
                        {
                            RegionClass region = this.createRegion(regionID, region_info.Definition, region_info.MulticastAddress);
                            RegionViewClass regionView = this.createRegionView(region, region_info.View.SeqNo, region_info.Members.ToArray());
                            region.InstallView(regionView);

                            regions[regionID] = region;

                            // just a quick hack
                            if (regionView.Members.Contains(localInstanceID))
                            {
                                myRID = regionView.Region.ID;

                                regionChange.LocalChange = (myRegionView != null)
                                    ? QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.SWITCHED_REGION
                                    : QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.ENTERED_REGION;
                                regionChange.CurrentView = myRegionView = regionView;
                            }

                            if (myRegionView != null && myRegionView.Region != null)
                                regionChange.RegionsAdded.Add(myRegionView.Region);

                            foreach (QS._core_c_.Base3.InstanceID nodeAddress in region_info.Members)
                                nodeToRegionViewMapping[nodeAddress] = regionView;
                        }
                        else
                        {
                            logger.Log(this, "Warning: region " + regionID.ToString() + " already exists.");

                            RegionClass existingRegion = regions[regionID];

                            if (region_info.View.SeqNo > existingRegion.CurrentView.SeqNo)
                            {
                                logger.Log(this, "Warning: using new region info instead of incremental update to advance the view.");

                                RegionViewClass regionView = this.createRegionView(
                                    existingRegion, region_info.View.SeqNo, region_info.Members.ToArray());
                                existingRegion.InstallView(regionView);

                                if (regionView.Members.Contains(localInstanceID))
                                {
                                    myRID = regionView.Region.ID;

                                    regionChange.LocalChange = (myRegionView != null)
                                        ? QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.SWITCHED_REGION
                                        : QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.ENTERED_REGION;
                                    regionChange.CurrentView = myRegionView = regionView;
                                }

                                if (myRegionView != null && myRegionView.Region != null)
                                    regionChange.RegionsAdded.Add(myRegionView.Region);

                                foreach (QS._core_c_.Base3.InstanceID nodeAddress in region_info.Members)
                                    nodeToRegionViewMapping[nodeAddress] = regionView;                                                                
                            }
                            else
                            {
                                logger.Log(this, "Warning: dropping an old region view info with view " + region_info.View.SeqNo.ToString() + 
                                    " in region " + existingRegion.ID.ToString() + ":" + existingRegion.CurrentView.SeqNo.ToString() + ".");
                            }
                        }
					}

					foreach (Protocol.Notification.RegionInfo_ToUpdate region_info in notification.RegionsChanged)
					{
						Base3_.IDWithSequenceNo<Base3_.RegionID> regionViewID = region_info.View;
						Base3_.Assertions.Assert(regions.ContainsKey(regionViewID.ID), "region " + regionViewID.ID.ToString() + " does not exist");
						RegionClass region = regions[regionViewID.ID];

						uint expected_seqno = region.CurrentView.SeqNo + 1;
						Base3_.Assertions.Assert(regionViewID.SeqNo == expected_seqno, "new region view has seqno = " + regionViewID.SeqNo.ToString() +
							" while we were expecting the updated view to have seqno = " + expected_seqno.ToString());

						List<QS._core_c_.Base3.InstanceID> members = new List<QS._core_c_.Base3.InstanceID>(
							region.CurrentView.Members.Count + region_info.ToAdd.Count - region_info.ToRemove.Count);
						members.AddRange(region.CurrentView.Members);
						foreach (QS._core_c_.Base3.InstanceID instanceID in region_info.ToRemove)
							members.Remove(instanceID);
						members.AddRange(region_info.ToAdd);

						RegionViewClass regionView = this.createRegionView(region, regionViewID.SeqNo, members.ToArray());
						region.InstallView(regionView);

						// just a quick hack
                        if (regionView.Members.Contains(localInstanceID))
                        {
                            myRID = regionView.Region.ID;

                            regionChange.LocalChange = QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.MEMBERSHIP_CHANGED;
                            regionChange.CurrentView = myRegionView = regionView;
                        }

                        foreach (QS._core_c_.Base3.InstanceID nodeAddress in region_info.ToAdd)
                            nodeToRegionViewMapping[nodeAddress] = regionView;
					}

					foreach (Protocol.Notification.GroupInfo_Complete group_info in notification.GroupsToCreate)
					{
						Base3_.GroupID groupID = group_info.View.ID;
						Base3_.Assertions.Assert(!groups.ContainsKey(groupID), "group " + groupID.ToString() + " already exists");

                        int nnodes = 0;

						RegionViewClass[] regionViews = new RegionViewClass[group_info.Regions.Count];
						int index = 0;
						foreach (Base3_.IDWithSequenceNo<Base3_.RegionID> regionViewID in group_info.Regions)
						{
							Base3_.Assertions.Assert(regions.ContainsKey(regionViewID.ID), "region " + regionViewID.ID.ToString() + " does not exist");
							RegionClass region = regions[regionViewID.ID];
							IRegionView inteface_regionView = region[regionViewID.SeqNo];
							Base3_.Assertions.Assert(inteface_regionView is RegionViewClass, "region view is of incompatible type");
							RegionViewClass regionView = (RegionViewClass)inteface_regionView;
							Base3_.Assertions.Assert(regionView != null, "region view " + regionViewID.ID.ToString() + ":" +
								regionViewID.SeqNo.ToString() + " does not exist");

							regionViews[index++] = regionView;

                            nnodes += regionView.Members.Count;
						}

						GroupClass group = this.createGroup(groupID
#if OPTION_EnableMulticastPerGroup
                            , group_info.MulticastAddress
#endif                            
                            );
						GroupViewClass groupView = this.createGroupView(group, group_info.View.SeqNo, regionViews);
						group.InstallView(groupView);

						groups[groupID] = group;

                        logger.Log(this, "__Configuration.Change : Group " + groupID.ToString() + " now has " + 
                            nnodes.ToString() + " nodes.");

                        groupCreationsAndRemovals.Add(
                            new Consumers.GroupCreationOrRemoval(groupID, true, group));
					}

					foreach (Protocol.Notification.GroupInfo_ToUpdate group_info in notification.GroupsChanged)
					{
						Base3_.IDWithSequenceNo<Base3_.GroupID> groupViewID = group_info.View;
						Base3_.Assertions.Assert(groups.ContainsKey(groupViewID.ID), "group " + groupViewID.ID.ToString() + " does not exist");
						GroupClass group = groups[groupViewID.ID];

						uint expected_seqno = group.CurrentView.SeqNo + 1;
						Base3_.Assertions.Assert(groupViewID.SeqNo == expected_seqno, "new group view has seqno = " + groupViewID.SeqNo.ToString() +
							" while we were expecting the updated view to have seqno = " + expected_seqno.ToString());

                        Dictionary<Base3_.RegionID, RegionViewClass> regionViews = new Dictionary<Base3_.RegionID, RegionViewClass>();
                        foreach (IRegionView regionView in group.CurrentView.RegionViews)
                            regionViews.Add(regionView.Region.ID, (RegionViewClass) regionView);

                        foreach (Base3_.RegionID rid in group_info.ToRemove)
                        {
                            if (regionViews.ContainsKey(rid))
                                regionViews.Remove(rid);
                        }

                        foreach (Base3_.IDWithSequenceNo<Base3_.RegionID> rvid in group_info.ToUpdate)
                        {
                            regionViews.Remove(rvid.ID);
                            RegionClass region = regions[rvid.ID];
                            IRegionView interface_regionView = region[rvid.SeqNo];
                            RegionViewClass regionView = (RegionViewClass)interface_regionView;
                            regionViews.Add(rvid.ID, regionView);
                        }

                        foreach (Base3_.IDWithSequenceNo<Base3_.RegionID> rvid in group_info.ToAdd)
                        {
                            RegionClass region = regions[rvid.ID];
                            IRegionView interface_regionView = region[rvid.SeqNo];
                            RegionViewClass regionView = (RegionViewClass)interface_regionView;
                            regionViews.Add(rvid.ID, regionView);
                        }

                        RegionViewClass[] regionViews_asArray = new RegionViewClass[regionViews.Count];
                        regionViews.Values.CopyTo(regionViews_asArray, 0);
                        GroupViewClass groupView = this.createGroupView(group, expected_seqno, regionViews_asArray);

                        group.InstallView(groupView);

                        int nnodes = 0;
                        foreach (IRegionView rv in groupView.RegionViews)
                            nnodes += rv.Members.Count;

                        logger.Log(this, "__Configuration.Change : Group " + group.ID.ToString() + " now has " +
                            nnodes.ToString() + " nodes.");

                        groupChanges.Add(new QS._qss_c_.Membership2.Consumers.GroupChange(groupView));
					}

					foreach (Base3_.GroupID groupID in notification.GroupsToDelete)
					{
						Base3_.Assertions.Assert(groups.ContainsKey(groupID), "group " + groupID.ToString() + " does not exist");

                        GroupClass group = groups[groupID];
                        groups.Remove(groupID);

                        this.deleteGroup(group);

                        groupCreationsAndRemovals.Add(
                            new Consumers.GroupCreationOrRemoval(groupID, false, null));
					}

					foreach (Base3_.RegionID regionID in notification.RegionsToDelete)
					{
						Base3_.Assertions.Assert(regions.ContainsKey(regionID), "region " + regionID.ToString() + " does not exist");

                        RegionClass region = regions[regionID];
                        ((IDisposable)region).Dispose();

                        regions.Remove(regionID);

						this.deleteRegion(region);

                        regionChange.RegionsRemoved.Add(region);

                        if (myRegionView != null && myRegionView.Region.ID.Equals(regionID))
                        {
                            myRegionView = null;                            
                            regionChange.LocalChange = QS._qss_c_.Membership2.Consumers.RegionChange.KindOf.LEFT_REGION;
                        }
					}
				}
				catch (Exception exc)
				{
					logger.Log(this, "__Change: " + exc.ToString());
				}


                if (onChange != null && regionChange != null) // && regionChange.CurrentView != null)
                {
                    // FIX: 5/7/2007
                    regionChange.CurrentView = myRegionView;

                    onChange(regionChange);
                }

                if (onGroupChange != null && groupChanges != null)
                    onGroupChange(groupChanges);

                if (onGroupCreationOrRemoval != null && groupCreationsAndRemovals.Count > 0)
                    onGroupCreationOrRemoval(groupCreationsAndRemovals);
			}
        }

        #endregion

        #region IRegionChangeProvider Members

        event QS._qss_c_.Membership2.Consumers.RegionChangedCallback QS._qss_c_.Membership2.Consumers.IRegionChangeProvider.OnChange
        {
            add
            {
                lock (this)
                {
                    onChange += value;
                }
            }

            remove 
            {
                lock (this)
                {
                    onChange -= value;
                }
            }
        }

        #endregion

        #region IGroupChangeProvider Members

        event QS._qss_c_.Membership2.Consumers.GroupChangedCallback QS._qss_c_.Membership2.Consumers.IGroupChangeProvider.OnChange
        {
            add
            {
                lock (this)
                {
                    onGroupChange += value;
                }
            }

            remove
            {
                lock (this)
                {
                    onGroupChange -= value;
                }
            }
        }

        #endregion

        #region IGroupCreationAndRemovalProvider Members

        event QS._qss_c_.Membership2.Consumers.GroupCreationOrRemovalCallback QS._qss_c_.Membership2.Consumers.IGroupCreationAndRemovalProvider.OnChange
        {
            add 
            {
                lock (this)
                {
                    onGroupCreationOrRemoval += value;
                }
            }
            
            remove 
            {
                lock (this)
                {
                    onGroupCreationOrRemoval -= value;
                }
            }
        }

        #endregion
    }
}
