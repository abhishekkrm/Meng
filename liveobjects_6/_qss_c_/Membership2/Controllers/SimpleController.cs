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

#define DEBUG_SimpleController
#define DEBUG_CrashProcessing

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Membership2.Controllers
{
    public class SimpleController 
        : ClientState.Configuration<SimpleController.Group, SimpleController.GroupView, SimpleController.Region, SimpleController.RegionView>,
            IMembershipController
    {
        public SimpleController(QS._core_c_.Base3.InstanceID instanceID, Devices_3_.IMembershipController ipmulticastMembershipController, QS.Fx.Logging.ILogger logger)
			: base(logger)
        {
            this.instanceID = instanceID;
            this.ipmulticastMembershipController = ipmulticastMembershipController;

			// some quick hack
			this.LocalInstanceID = instanceID;
		}

        private QS._core_c_.Base3.InstanceID instanceID;
        private Devices_3_.IMembershipController ipmulticastMembershipController;

        #region ClientState.Configuration Overrides

        protected override Region createRegion(Base3_.RegionID regionID, Base3_.RegionSig regionSig, QS.Fx.Network.NetworkAddress multicastAddress)
        {
            return new Region(this, regionID, regionSig, multicastAddress);
        }

        protected override RegionView createRegionView(Region region, uint seqNo, QS._core_c_.Base3.InstanceID[] members)
        {
            return new RegionView(this, region, seqNo, members);
        }

        protected override Group createGroup(Base3_.GroupID groupID
#if OPTION_EnableMulticastPerGroup
            , QS.Fx.Network.NetworkAddress multicastAddress
#endif            
            )
        {
            return new Group(this, groupID
#if OPTION_EnableMulticastPerGroup
                , multicastAddress
#endif                
                );
        }

        protected override GroupView createGroupView(Group group, uint seqNo, RegionView[] regionViews)
        {
            return new GroupView(this, group, seqNo, regionViews);
        }

        protected override void deleteGroup(Group group)
        {
        }

        protected override void deleteRegion(Region region)
        {
        }

        #endregion

        #region Class Group

        public class Group : ClientState.Group<GroupView>, IGroupController
        {
            public Group(SimpleController owner, Base3_.GroupID groupID
#if OPTION_EnableMulticastPerGroup
                , QS.Fx.Network.NetworkAddress multicastAddress
#endif                
                ) : base(groupID
#if OPTION_EnableMulticastPerGroup
                    , multicastAddress
#endif                                
                )
            {
                this.owner = owner;
            }

            private SimpleController owner;
#if OPTION_EnableMulticastPerGroup
            private Devices_3_.IListener ipmulticast_listener = null;
#endif

            #region ClientState.Group Overrides

            protected override void initialize()
            {
#if DEBUG_SimpleController
                owner.logger.Log(this, "__Initialize");
#endif

#if OPTION_EnableMulticastPerGroup
                if (this.MulticastAddress.PortNumber > 0 && QS.Fx.Network.NetworkAddress.IsMulticastIPAddress(this.MulticastAddress.HostIPAddress))
                {
                    ipmulticast_listener = owner.ipmulticastMembershipController.Join(this.MulticastAddress);
#if DEBUG_SimpleController
                    owner.logger.Log(this, "__Initialize : Group " + groupID.ToString() + " listening at " + this.MulticastAddress.ToString());
#endif
                }
#endif
            }

            protected override void flush(GroupView flushedView,
                IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses, IEnumerable<QS._core_c_.Base3.InstanceID> unaffectedAddresses)
            {
                flushedView.ProcessCrashed(removedAddresses);
            }

            protected override void crashed(GroupView oldView, IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses)
            {
                oldView.ProcessCrashed(removedAddresses);
            }

            #endregion

            #region IGroupController Members

			IGroupViewController IGroupController.CurrentView
			{
                get { return this.currentView; }
            }

            #endregion
        }

        #endregion

        #region Class GroupView

        public class GroupView : ClientState.GroupView<Group, RegionView>, IGroupViewController
        {
            public GroupView(SimpleController owner, Group group, uint seqNo, RegionView[] regionViews) 
				: base(group, seqNo, regionViews)
            {
                this.owner = owner;

				multicastAddresses = new QS.Fx.Network.NetworkAddress[regionViews.Length];
                members = new List<QS._core_c_.Base3.InstanceID>();

                for (uint ind = 0; ind < regionViews.Length; ind++)
                {
                    multicastAddresses[ind] = regionViews[ind].Region.Address;
                    members.AddRange(regionViews[ind].Members);
                }

                members.Sort();

				regionVCs = new IRegionViewController[regionViews.Length];
				regionViews.CopyTo(regionVCs, 0);
            }

            private SimpleController owner;
			private QS.Fx.Network.NetworkAddress[] multicastAddresses;
            private List<QS._core_c_.Base3.InstanceID> members;
			private IRegionViewController[] regionVCs;

            private List<QS._core_c_.Base3.InstanceID> deadAddresses = new List<QS._core_c_.Base3.InstanceID>();
            private List<Subscription> failureSubscriptions = new List<Subscription>();

            #region IViewController Members

            public override void ProcessCrashed(IEnumerable<QS._core_c_.Base3.InstanceID> crashedAddresses)
            {
#if DEBUG_CrashProcessing
                owner.logger.Log(null, "__________GroupView(" + this.group.ID.ToString() + ":" + this.seqno.ToString() + ").ProcessCrashed");
#endif

                lock (this)
                {
                    // Registering the newly crashed addresses and notifying all subscribers

                    List<QS._core_c_.Base3.InstanceID> newlyCrashed = new List<QS._core_c_.Base3.InstanceID>();
                    foreach (QS._core_c_.Base3.InstanceID address in crashedAddresses)
                        if (members.Contains(address) && !deadAddresses.Contains(address))
                            newlyCrashed.Add(address);

#if DEBUG_CrashProcessing
                    owner.logger.Log(this, "__ProcessCrashed(" + 
                        this.group.ID.ToString() + ":" + this.seqno.ToString() + ") : Newly crashed { " +
                        QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(newlyCrashed, ",") + " }");
#endif

                    List<Subscription> toCall = new List<Subscription>(failureSubscriptions);
                    foreach (Subscription subscription in toCall)
                        subscription.Callback(newlyCrashed);

                    deadAddresses.AddRange(newlyCrashed);

                    // Additional processing...........


                }
            }

			public IGroupController GroupController
			{
				get { return group; }
			}

			public QS.Fx.Network.NetworkAddress[] MulticastAddresses
            {
                get { return multicastAddresses; }
            }

            public IList<QS._core_c_.Base3.InstanceID> Members
            {
                get { return members; }
            }

			public IRegionViewController[] RegionViewControllers
			{
				get { return regionVCs; }
			}

            #endregion

            #region Failure.ISource Members

            QS._qss_c_.Failure_.ISubscription QS._qss_c_.Failure_.ISource.subscribe(
                QS._qss_c_.Failure_.NotificationsCallback notificationsCallback, out IList<QS._core_c_.Base3.InstanceID> synchronousNotifications)
            {
                Subscription subscription;
                lock (this)
                {
                    subscription = new Subscription(this, notificationsCallback);
                    failureSubscriptions.Add(subscription);
                    synchronousNotifications = new List<QS._core_c_.Base3.InstanceID>(deadAddresses);
                }
                return subscription;
            }

            private void unsubscribe(Subscription subscription)
            {
                lock (this)
                {
                    failureSubscriptions.Remove(subscription);
                }
            }

            #endregion

            #region Class Subscription

            private class Subscription : Failure_.ISubscription
            {
                public Subscription(GroupView owner, Failure_.NotificationsCallback notificationsCallback)
                {
                    this.owner = owner;
                    this.notificationsCallback = notificationsCallback;
                }

                private GroupView owner;
                private Failure_.NotificationsCallback notificationsCallback;

                public Failure_.NotificationsCallback Callback
                {
                    get { return notificationsCallback; }
                }

                #region ISubscription Members

                void QS._qss_c_.Failure_.ISubscription.Cancel()
                {
                    owner.unsubscribe(this);
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region Class Region

        public class Region : ClientState.Region<RegionView>, IRegionController
        {
            public Region(SimpleController owner, Base3_.RegionID regionID, Base3_.RegionSig regionSig, QS.Fx.Network.NetworkAddress multicastAddress) 
                : base(regionID, regionSig, multicastAddress)
            {
                this.owner = owner;
            }

            private SimpleController owner;
            private Devices_3_.IListener ipmulticast_listener = null;

            #region ClientState.Region Overrides

            protected override void initialize()
            {
#if DEBUG_SimpleController
                owner.logger.Log(this, "__Initialize");
#endif

                if (currentView.Members.Contains(owner.instanceID))
                    ipmulticast_listener = owner.ipmulticastMembershipController.Join(this.Address);
            }

            protected override void flush(RegionView flushedView,
                IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses, IEnumerable<QS._core_c_.Base3.InstanceID> unaffectedAddresses)
            {
                if (ipmulticast_listener == null && currentView.Members.Contains(owner.instanceID))
                    ipmulticast_listener = owner.ipmulticastMembershipController.Join(this.Address);

                flushedView.ProcessCrashed(removedAddresses);
            }

            protected override void crashed(RegionView oldView, IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses)
            {
                oldView.ProcessCrashed(removedAddresses);
            }

            protected override void cleanup()
            {
#if DEBUG_SimpleController
                owner.logger.Log(this, "__Cleanup");
#endif

                try
                {
                    if (ipmulticast_listener != null)
                        ipmulticast_listener.shutdown();
                }
                catch (Exception exc)
                {
                    owner.logger.Log(this, "while cleaning up region " + this.regionID.ToString() + "\n" + exc.ToString());
                }

                try
                {
                    if (ipmulticast_listener != null)
                        ipmulticast_listener.Dispose();
                }
                catch (Exception exc)
                {
                    owner.logger.Log(this, "while cleaning up region " + this.regionID.ToString() + "\n" + exc.ToString());
                }
            }

            #endregion

            public bool TryGetView(uint seqno, out RegionView regionView)
            {
                return viewCollection.TryGetValue(seqno, out regionView);
            }
        }

        #endregion

        #region Class RegionView

        public class RegionView : ClientState.RegionView<Region>, IRegionViewController
        {
            public RegionView(SimpleController owner, Region region, uint seqNo, QS._core_c_.Base3.InstanceID[] members) : base(region, seqNo, members)
            {
                this.owner = owner;
			}

            private SimpleController owner;
            private List<QS._core_c_.Base3.InstanceID> deadAddresses = new List<QS._core_c_.Base3.InstanceID>();
            private List<Subscription> failureSubscriptions = new List<Subscription>();



			#region IRegionView Members

			public IRegionController RegionController
			{
				get { return region; }
			}

			#endregion

            public override void ProcessCrashed(IEnumerable<QS._core_c_.Base3.InstanceID> crashedAddresses)
            {
#if DEBUG_CrashProcessing
                owner.logger.Log(null, 
                    "__________RegionView(" + this.region.ID.ToString() + ":" + this.seqno.ToString() +  ").ProcessCrashed");
#endif

                lock (this)
                {
                    // Registering the newly crashed addresses and notifying all subscribers

                    List<QS._core_c_.Base3.InstanceID> newlyCrashed = new List<QS._core_c_.Base3.InstanceID>();
                    ICollection<QS._core_c_.Base3.InstanceID> memberList = 
                        new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>(members);
                    foreach (QS._core_c_.Base3.InstanceID address in crashedAddresses)
                        if (memberList.Contains(address) && !deadAddresses.Contains(address))
                            newlyCrashed.Add(address);

#if DEBUG_CrashProcessing
                    owner.logger.Log(this, "__ProcessCrashed(" +
                        this.region.ID.ToString() + ":" + this.seqno.ToString() + ") : Newly crashed { " +
                        QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(newlyCrashed, ",") + " }");
#endif

                    List<Subscription> toCall = new List<Subscription>(failureSubscriptions);
                    foreach (Subscription subscription in toCall)
                        subscription.Callback(newlyCrashed);

                    deadAddresses.AddRange(newlyCrashed);

                    // Additional processing...........

                }
            }

            #region ISource Members

            QS._qss_c_.Failure_.ISubscription QS._qss_c_.Failure_.ISource.subscribe(QS._qss_c_.Failure_.NotificationsCallback notificationsCallback, out IList<QS._core_c_.Base3.InstanceID> synchronousNotifications)
            {
                Subscription subscription;
                lock (this)
                {
                    subscription = new Subscription(this, notificationsCallback);
                    failureSubscriptions.Add(subscription);
                    synchronousNotifications = new List<QS._core_c_.Base3.InstanceID>(deadAddresses);
                }
                return subscription;
            }

            private void unsubscribe(Subscription subscription)
            {
                lock (this)
                {
                    failureSubscriptions.Remove(subscription);
                }
            }

            #endregion

            #region Class Subscription

            private class Subscription : Failure_.ISubscription
            {
                public Subscription(RegionView owner, Failure_.NotificationsCallback notificationsCallback)
                {
                    this.owner = owner;
                    this.notificationsCallback = notificationsCallback;
                }

                private RegionView owner;
                private Failure_.NotificationsCallback notificationsCallback;

                public Failure_.NotificationsCallback Callback
                {
                    get { return notificationsCallback; }
                }

                #region ISubscription Members

                void QS._qss_c_.Failure_.ISubscription.Cancel()
                {
                    owner.unsubscribe(this);
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region IMembershipController Members

        public IRegionViewController RegionViewOf(QS._core_c_.Base3.InstanceID address)
        {
            return nodeToRegionViewMapping[address];
        }

        public IGroupController this[Base3_.GroupID groupID]
        {
            get
            {
                return groups[groupID];
            }
        }

		public IGroupController lookupGroup(Base3_.GroupID groupID)
		{
			return groups[groupID];
		}

		public IRegionController lookupRegion(Base3_.RegionID regionID)
		{
			return regions[regionID];
		}

		public IRegionController MyRegion
		{
			get { return regions[MyRID]; }
		}

        IEnumerable<IRegionController> IMembershipController.NeighboringRegions
        {
            get 
            {
                lock (this)
                {
                    IRegionController[] result = new IRegionController[regions.Count];
                    int ind = 0;
                    foreach (Region region in regions.Values)
                        result[ind++] = region;
                    return result;
                }
            }
        }

        bool IMembershipController.TryGetRegionView(Base3_.RVID rvid, out IRegionViewController regionVC)
        {
            Region region;
            if (regions.TryGetValue(rvid.RegionID, out region))
            {
                RegionView regionView;
                if (region.TryGetView(rvid.SeqNo, out regionView))
                {
                    regionVC = (IRegionViewController)regionView;
                    return true;                        
                }
            }

            regionVC = null;
            return false;
        }

        #endregion
    }
}
