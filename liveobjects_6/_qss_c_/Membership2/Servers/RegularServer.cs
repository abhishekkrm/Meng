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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Membership2.Servers
{
    public class RegularServer : GenericServer
    {
        public RegularServer(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock,
            Base3_.ISenderCollection<Base3_.IReliableSerializableSender> senderCollection, FailureDetection_.IFailureDetector failureDetectionServer,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> instanceSenders) 
            : base(logger, demultiplexer, alarmClock, senderCollection, failureDetectionServer, instanceSenders)
        {
            groups = new Collections_2_.SplayOf<Base3_.GroupID, Group>();
        }

        protected override void processRequest(ICollection<Protocol.IMembershipChangeRequest> requests,
            ref Components_1_.AutomaticCollection<QS._core_c_.Base3.InstanceID, Protocol.Notification> notifications)
        {
            lock (this)
            {
                Components_1_.AutomaticCollection<Base3_.GroupID, Components_1_.SetChange<QS._core_c_.Base3.InstanceID>> groupChanges =
                    new QS._qss_c_.Components_1_.AutomaticCollection<QS._qss_c_.Base3_.GroupID,QS._qss_c_.Components_1_.SetChange<QS._core_c_.Base3.InstanceID>>();

                foreach (Protocol.IMembershipChangeRequest request in requests)
                {
                    foreach (Base3_.GroupID groupID in request.ToLeave)
                        groupChanges[groupID].ToRemove.Add(request.InstanceID);
                    foreach (Base3_.GroupID groupID in request.ToJoin)
                        groupChanges[groupID].ToAdd.Add(request.InstanceID);
                }

                foreach (KeyValuePair<Base3_.GroupID, Components_1_.SetChange<QS._core_c_.Base3.InstanceID>> change in groupChanges.Collection)
                {
                    Base3_.GroupID groupID = change.Key;
                    Components_1_.SetChange<QS._core_c_.Base3.InstanceID> setChange = change.Value;

                    Group group = groups[groupID];
                    if (group == null)
                    {
                        if (setChange.ToRemove.Count > 0)
                        {
                            logger.Log(this, "Warning: Could not remove nodes " + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(
                                setChange.ToRemove, ",") + " from group " + groupID.ToString() + " because no such group currently exists on this server.");
                        }

                        if (setChange.ToAdd.Count > 0)
                        {
                            group = new Group(groupID, addressPool.AllocateAddress);
                            groups.insert(group);

                            Group.View view = new Group.View(1, setChange.ToAdd);
                            group.CurrentView = view;

                            Base3_.RegionID regionID = new Base3_.RegionID((ulong)groupID.ToUInt32);
                            Base3_.RegionSig regionSig = new QS._qss_c_.Base3_.RegionSig(new Base3_.GroupID[] { groupID });
                            Base3_.IDWithSequenceNo<Base3_.RegionID> regionView = new Base3_.IDWithSequenceNo<Base3_.RegionID>(regionID, 1);

                            Protocol.Notification.RegionInfo_Complete regionInfo =
                                new Protocol.Notification.RegionInfo_Complete(regionView, regionSig, group.MulticastAddress, setChange.ToAdd);
                            Protocol.Notification.GroupInfo_Complete groupInfo = new Protocol.Notification.GroupInfo_Complete(
                                new Base3_.IDWithSequenceNo<Base3_.GroupID>(groupID, 1), 
                                new Base3_.IDWithSequenceNo<Base3_.RegionID>[] { regionView }
#if OPTION_EnableMulticastPerGroup
                                , group.MulticastAddress
#endif
                            );

                            foreach (QS._core_c_.Base3.InstanceID instanceID in setChange.ToAdd)
                            {
                                notifications[instanceID].RegionsToCreate.Add(regionInfo);
                                notifications[instanceID].GroupsToCreate.Add(groupInfo);
                            }
                        }
                    }
                    else
                    {
                        logger.Log(this, "Not implemented!");

                        // ...................................................................

                    }

                    // ...................................................................
                }

                // ....................................

            }
        }

        private Collections_2_.IBinaryTreeOf<Base3_.GroupID, Group> groups;

        #region Class Group

        private class Group : Collections_2_.BTNOf<Base3_.GroupID>
        {
            public Group(Base3_.GroupID groupID, QS.Fx.Network.NetworkAddress multicastAddress) : base(groupID)
            {
                this.MulticastAddress = multicastAddress;
            }

            public Group()
            {
            }

            public QS.Fx.Network.NetworkAddress MulticastAddress;
            public View CurrentView = null;

            // .............

            #region Class View

            public class View
            {
                public View(uint seqno, ICollection<QS._core_c_.Base3.InstanceID> members)
                {
                    this.seqno = seqno;
                    this.members = new List<QS._core_c_.Base3.InstanceID>(members);
                }

                private uint seqno;
                private List<QS._core_c_.Base3.InstanceID> members;

                // .............

                #region ToString

                public override string ToString()
                {
                    return "View #" + seqno.ToString() + ", Members: " +
                        QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(members, ",");
                }

                #endregion
            }

            #endregion

            #region Accessorts, Helpers, ToString etc.

            public Base3_.GroupID GroupID
            {
                get { return this.key; }
            }

            public override string ToString()
            {
                return "Group " + this.GroupID.ToString() + " ; Current View: " +
                    ((this.CurrentView != null) ? CurrentView.ToString() : "(null)");
            }

            #endregion
        }

        #endregion

        #region Accessors, Helpers, ToString etc.

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("Current Configuration:\n");
            foreach (Group group in groups)
                s.AppendLine(group.ToString());
            return s.ToString();
        }

        #endregion
    }
}
