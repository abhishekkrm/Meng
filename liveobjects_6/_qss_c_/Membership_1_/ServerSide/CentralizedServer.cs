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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Membership_1_.ServerSide
{
    public class CentralizedServer
    {
        public enum RequestType
        {
            JOIN, LEAVE
        }

        public CentralizedServer(QS.Fx.Logging.ILogger logger, Scattering_1_.IRetransmittingScatterer retransmittingScatterer,
            Base2_.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            this.retransmittingScatterer = retransmittingScatterer;
            demultiplexer.register((uint) ReservedObjectID.Membership_CentralizedServer, 
                new Base2_.ReceiveCallback(this.receiveCallback));
            groupCollection = new Collections_1_.HashedSplaySet(100);
        }

        private void notification(QS.Fx.Network.NetworkAddress[] destinations, MembershipView membershipView)
        {
			Scattering_1_.IScatterSet scatterSet = new Scattering_1_.ScatterSet(destinations);

            retransmittingScatterer.multicast((uint)ReservedObjectID.Membership_ClientAgent, scatterSet,
                Components_1_.Sequencer.wrap(new Base2_.XmlWrapper(membershipView)),
                new Scattering_1_.CompletionCallback(this.scatteringCompletionCallback));
        }

        private void scatteringCompletionCallback(bool success, System.Exception exception)
        {
            logger.Log(this, "ScatteringCompletionCallback: success = " + success.ToString() +
                (success ? "" : (", exception = \"" + exception.ToString() + "\"")));
        }

        #region Class Group

        private class Group : Collections_1_.GenericBinaryTreeNode
        {
            public Group(GMS.GroupId groupID, CentralizedServer associatedCS)
            {
                this.groupID = groupID;
                this.associatedCS = associatedCS;
                currentView = new MembershipView(groupID, 0);
            }

            private GMS.GroupId groupID;
            private CentralizedServer associatedCS;
            private MembershipView currentView;

            #region Join and Leave Requests

            public void processJoinRequest(QS.Fx.Network.NetworkAddress sourceAddress, uint loid)
            {
                MembershipView newView = currentView.CreateCopy;
                newView.add(sourceAddress, loid);
                currentView = newView;

                associatedCS.notification(currentView.Addresses, newView);
            }

            public void processLeaveRequest(QS.Fx.Network.NetworkAddress sourceAddress, uint loid)
            {
                MembershipView newView = currentView.CreateCopy;
                newView.remove(sourceAddress, loid);

                associatedCS.notification(currentView.Addresses, newView);

                currentView = newView;
            }

            #endregion

            #region Collections.GenericBinaryTreeNode Overrides

		    public override System.IComparable Contents
		    {
			    get
			    {
				    return groupID;
			    }
		    }

		    #endregion
        }

        #endregion

        private QS.Fx.Logging.ILogger logger;
        private Scattering_1_.IRetransmittingScatterer retransmittingScatterer;
        private Collections_1_.IRawBinaryTree groupCollection;

        #region Join and Leave Requests

        private void processJoinRequest(QS.Fx.Network.NetworkAddress sourceAddress, GMS.GroupId groupID, uint loid)
        {
            lock (groupCollection)
            {
                Group group = (Group) groupCollection.lookup(groupID);
                if (group == null)
                {
                    group = new Group(groupID, this);
                    groupCollection.insert(group);
                }

                group.processJoinRequest(sourceAddress, loid);
            }
        }

        private void processLeaveRequest(QS.Fx.Network.NetworkAddress sourceAddress, GMS.GroupId groupID, uint loid)
        {
            lock (groupCollection)
            {
                Group group = (Group)groupCollection.lookup(groupID);
                if (group == null)
                {
                    group = new Group(groupID, this);
                    groupCollection.insert(group);
                }

                group.processLeaveRequest(sourceAddress, loid);
            }
        }

        #endregion

        #region Receive Callback

        private QS._core_c_.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress,
            QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable argumentObject)
        {
            try
            {
                QS._core_c_.Components.AttributeSet args = (QS._core_c_.Components.AttributeSet) ((Base2_.XmlWrapper) argumentObject).Object;

                switch ((RequestType)args["request"])
                {
                    case RequestType.JOIN:
                    {
                        processJoinRequest(sourceAddress, (GMS.GroupId)args["groupid"], (uint)args["loid"]);
                        return Base2_.NullObject.Object;
                    }

                    case RequestType.LEAVE:
                    {
                        processLeaveRequest(sourceAddress, (GMS.GroupId)args["groupid"], (uint)args["loid"]);
                        return Base2_.NullObject.Object;
                    }

                    default:
                        throw new Exception("Unknown request type.");
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, "ReceiveCallback : While processing \"" + argumentObject.ToString() + " : " +
                    argumentObject.GetType().FullName + "\" from " + sourceAddress.ToString() + "\n" + exc.StackTrace);
            }

            return null;
        }

        #endregion
    }
}
