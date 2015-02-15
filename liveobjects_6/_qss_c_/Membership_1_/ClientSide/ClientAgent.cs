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

namespace QS._qss_c_.Membership_1_.ClientSide
{
/*
    public class ClientAgent : IClientAgent
    {
        public ClientAgent(QS.Fx.Logging.ILogger logger, Base2.IDemultiplexer demultiplexer, 
            QS.Fx.Network.NetworkAddress serverAddress, RPC2.IRPCProxy rpcProxy)
        {
            this.logger = logger;
            this.serverAddress = new Base.ObjectAddress(serverAddress, (uint) ReservedObjectID.Membership_CentralizedServer);
            this.rpcProxy = rpcProxy;
            this.responseRPCCallback = new QS.CMS.RPC2.RPCCallback(responseCallback);

            demultiplexer.register((uint) ReservedObjectID.Membership_ClientAgent, 
                new QS.CMS.Base2.ReceiveCallback(this.notificationCallback));

            groupCollection = new Collections.HashedSplaySet(20);
        }

        private QS.Fx.Logging.ILogger logger;
        private Base.ObjectAddress serverAddress;
        private RPC2.IRPCProxy rpcProxy;
        private RPC2.RPCCallback responseRPCCallback;
        private Collections.IRawBinaryTree groupCollection;

        private class Group : Collections.GenericBinaryTreeNode
        {
            public Group(GMS.GroupId groupID)
            {
                this.groupID = groupID;
                incomingWindow = new FlowControl.IncomingWindow(20);
            }

            public GMS.GroupId groupID;
            public ServerSide.IMembershipView currentView = null;

            public FlowControl.IIncomingWindow incomingWindow;

            public override IComparable Contents
            {
                get { return groupID; }
            }
        }

        private QS.GMS.ViewChangeRequest vcr;
        private QS.GMS.ViewChangeAllDone vcad;
        private QS.GMS.ViewChangeCleanup vcc;

        private Base2.IBase2Serializable notificationCallback(
            QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBase2Serializable argumentObject)
        {
            ServerSide.IMembershipView membershipView = (ServerSide.IMembershipView) 
                ((Base2.XmlWrapper) ((Components.Sequencer.IWrappedObject) argumentObject).SerializableObject).Object;

            logger.Log(this, "notificationCallback : " + membershipView.ToString());

            lock (groupCollection)
            {
                Group group = (Group) groupCollection.lookup(membershipView.GroupID);
                if (group == null)
                    groupCollection.insert(group = new Group(membershipView.GroupID));



                // ...........


            }

            return null;
        }

        private void viewChangeGoAheadCallback(GMS.GroupId gid, uint seqno)
        {
            // ...............
        }

        private void responseCallback(object contextObject, Base2.IBase2Serializable result)
        {
            logger.Log(this, "responseCallback : " + contextObject.ToString() + " --> " + result.ToString());
        }

        #region IGMS Members

        void QS.GMS.IGMS.joinGroup(QS.GMS.GroupId gid, uint nid, QS.GMS.ViewChangeUpcall vcu)
        {
            Components.AttributeSet request = new Components.AttributeSet(new Components.Attribute[] {
                new Components.Attribute("request", ServerSide.CentralizedServer.RequestType.JOIN),
                new Components.Attribute("groupid", gid), new Components.Attribute("loid", nid) });
            rpcProxy.call(serverAddress, new Base2.XmlWrapper(request), responseRPCCallback, request);
        }

        void QS.GMS.IGMS.leaveGroup(QS.GMS.GroupId gid, uint nid)
        {
            Components.AttributeSet request = new Components.AttributeSet(new Components.Attribute[] {
                new Components.Attribute("request", ServerSide.CentralizedServer.RequestType.LEAVE),
                new Components.Attribute("groupid", gid), new Components.Attribute("loid", nid) });
            rpcProxy.call(serverAddress, new Base2.XmlWrapper(request), responseRPCCallback, request);
        }

        QS.GMS.ViewChangeGoAhead QS.GMS.IGMS.linkCMSToGMS(QS.GMS.ViewChangeRequest vcr, 
            QS.GMS.ViewChangeAllDone vcad, QS.GMS.ViewChangeCleanup vcc)
        {
            this.vcr = vcr;
            this.vcad = vcad;
            this.vcc = vcc;

            return new QS.GMS.ViewChangeGoAhead(this.viewChangeGoAheadCallback);
        }

        #endregion
    }
*/
}
