/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Threading;
using System.Reflection;
using GOTransport.Common;
using GOTransport.Frontend;
using GOTransport.Core;
using System.Diagnostics;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Frontend
{
    [QS.Fx.Reflection.ComponentClass("2`1", "GOTransport")]
    public sealed class GOTransport : 
        IGOTransport,
        IGORequest,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GOServiceRequest, GOServiceResponse>
    {

        #region constructor
        static int staticCounter = 0;

        public GOTransport(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("GOConnection_URL", QS.Fx.Reflection.ParameterClass.Value)]
            string _goConnectionUrl,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<GOServiceRequest, GOServiceResponse>> _channel)
        {

            if (goTransportEndpoint == null && channelendpoint == null && channelconnection == null)
            {
                goTransportEndpoint = _mycontext.DualInterface<IGOResponse, IGORequest>(this);
                channelendpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<GOServiceRequest, GOServiceResponse>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GOServiceRequest, GOServiceResponse>>(this);
                channelconnection = channelendpoint.Connect(_channel.Dereference(_mycontext).Channel);

                Type typeOfRemoteObject = typeof(IGOConnection);
                AppConfigReader configReader = new AppConfigReader();
                string urlOfRemoteObject = _goConnectionUrl;
                goConnection = (IGOConnection)Activator.GetObject(typeOfRemoteObject,
                                urlOfRemoteObject);
                //(goConnection as GOConnection).Touch();

            }
        }

        #endregion

        #region private fields

        private IDictionary<string, Rumor> myIncomingRumors = new Dictionary<string, Rumor>();
        private bool ready;
        static private QS.Fx.Endpoint.Internal.IDualInterface<IGOResponse, IGORequest> goTransportEndpoint = null;
        static private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<GOServiceRequest, GOServiceResponse>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GOServiceRequest, GOServiceResponse>> channelendpoint = null;
        static private QS.Fx.Endpoint.IConnection channelconnection = null;

        static private IGOConnection goConnection = null;

        #endregion

        #region IGOTransport members

        QS.Fx.Endpoint.Classes.IDualInterface<IGOResponse, IGORequest> IGOTransport.goTransport
        {
            get { return goTransportEndpoint; }
        }

        #endregion

        // [TODO]
        #region ICheckpointedCommunicationChannelClient members

        GOServiceResponse QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GOServiceRequest, GOServiceResponse>.Checkpoint()
        {
            lock (this)
            {
                return new GOServiceResponse((new List<Rumor>(this.myIncomingRumors.Values)).ToArray());
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GOServiceRequest, GOServiceResponse>.Initialize(GOServiceResponse _checkpoint)
        {
            lock (this)
            {
                this.myIncomingRumors.Clear();
                if ((_checkpoint != null) && (_checkpoint.rumorList != null))
                    foreach (Rumor account in _checkpoint.rumorList)
                        this.myIncomingRumors.Add(account.Id, account);
                this.ready = true;
                if (goTransportEndpoint.IsConnected)
                {
                    goTransportEndpoint.Interface.Ready();
                    //this.goTransportEndpoint.Interface.Alert(new Rumor("alert : " + counter, "new payload"));
                    //counter++;
                }
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GOServiceRequest, GOServiceResponse>.Receive(GOServiceRequest _message)
        {
            switch (_message.requestType)
            {
                case GOServiceRequest.Type.Send:
                    // [TODO]
                    break;
            }
        }

        #endregion

        #region IGORequest Members

        bool IGORequest.Ready()
        {
            return this.ready;
        }       

        void IGORequest.Subscribe(ISubscription subscriptionParameterList)
        {
            // [TODO]
            throw new NotImplementedException();
        }

        void IGORequest.AddNode(IGraphElement _group, IGraphElement _node)
        {
            goConnection.AddNode(_group as Group, _node as Node);
        }

        void IGORequest.AddNodeList(IGraphElement _group, NodeList _nodeList)
        {
            goConnection.AddNodeList(_group as Group, _nodeList);
        }

        void IGORequest.RemoveNode(IGraphElement _group, IGraphElement _node)
        {
            goConnection.RemoveNode(_group as Group, _node as Node);
        }

        void IGORequest.RemoveNodeList(IGraphElement _group, NodeList _nodeList)
        {
            goConnection.RemoveNodeList(_group as Group, _nodeList);
        }

        void IGORequest.Send(IGossip _gossip)
        {
            goConnection.Send(_gossip as Rumor);
        }

        IGossip IGORequest.Receive()
        {
            return goConnection.Receive();
        }

        bool IGORequest.IsReady()
        {
            return goConnection.IsReady();
        }

        void IGORequest.Clear()
        {
            goConnection.Clear();
        }

        void IGORequest.Connect(IGraphElement _node1, IGraphElement _node2, double _cost)
        {
            goConnection.Connect(_node1 as Node, _node2 as Node, _cost);
        }

        void IGORequest.SetWorkingContext(String _port)
        {
            goConnection.SetWorkingContext(_port);
        }

        void IGORequest.InitializationComplete()
        {
            goConnection.InitializationComplete();
            if (staticCounter == 0)
            {
                goTransportEndpoint.Interface.Alert(new Rumor("hello world", "something"));
                staticCounter = 1;
            }
        }

        Rumor IGORequest.BeginToFetch()
        {
            return goConnection.BeginToFetch();
        }

        void IGORequest.DisConnect(IGraphElement _node1, IGraphElement _node2)
        {
            goConnection.DisConnect(_node1 as Node, _node2 as Node);
        }

        Boolean IGORequest.HasGraphData()
        {
            return (goConnection as GOConnection).HasGraphData();
        }

        void IGORequest.SetHasGraphData(Boolean _flag)
        {
            (goConnection as GOConnection).SetHasGraphData(_flag);
        }

        #endregion
    }
}
