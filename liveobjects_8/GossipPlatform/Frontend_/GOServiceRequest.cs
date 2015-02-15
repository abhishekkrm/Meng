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
using System.Xml.Serialization;
using GOTransport.Common;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Frontend
{
    #region public types

    public class AddNodeRequest
    {
        public Group group { get; set; }
        public Node node { get; set; }
    }

    public class AddNodeListRequest
    {
        public Group group { get; set; }
        public NodeList nodeList { get; set; }
    }

    public class RemoveNodeRequest
    {
        public Group group { get; set; }
        public Node node { get; set; }
    }

    public class RemoveNodeListRequest
    {
        public Group group { get; set; }
        public NodeList nodeList { get; set; }
    }

    public class ConnectRequest
    {
        public Node node1 { get; set; }
        public Node node2 { get; set; }
        public double cost { get; set; }
    }

    public class SendRequest
    {
        public Rumor rumor { get; set; }
    }

    public class SetWorkingContextRequest
    {
        public String port { get; set; }
    }

    public class InitializationComplete
    {
        // no parameters for now
    }

    #endregion

    [QS.Fx.Reflection.ValueClass("30`1", "GOServiceRequest")]
    public sealed class GOServiceRequest
    {
        #region constructor

        public GOServiceRequest() { }

        #endregion

        #region public fields

        [XmlElement]
        public AddNodeRequest addNodeRequest;

        [XmlElement]
        public AddNodeListRequest addNodeListRequest;

        [XmlElement]
        public RemoveNodeRequest removeNodeRequest;

        [XmlElement]
        public RemoveNodeListRequest removeNodeListRequest;

        [XmlElement]
        public SendRequest sendRequest;

        [XmlElement]
        public ConnectRequest connectRequest;

        public SetWorkingContextRequest setWorkingContextRequest;

        public InitializationComplete initializationCompleteRequest;

        public enum Type {  AddNode, 
                            AddNodeList, 
                            RemoveNode, 
                            RemoveNodeList, 
                            ConnectRequest, 
                            IsReady, 
                            Send, 
                            Receive, 
                            SetWorkingContext,
                            InitializationComplete};

        [XmlElement]
        public Type requestType;

        [XmlElement]
        public Object requestParameter;

        #endregion

        #region public methods

        public void AddNode(IGraphElement _node, IGraphElement _group)
        {
            requestType = Type.AddNode;
            addNodeRequest = new AddNodeRequest();
            addNodeRequest.group = (Group)_group;
            addNodeRequest.node = (Node)_node;
            requestParameter = addNodeRequest;
        }

        public void AddNodeList(IGraphElement _group, NodeList _nodeList)
        {
            requestType = Type.AddNodeList;
            addNodeListRequest.group = (Group) _group;
            addNodeListRequest.nodeList = _nodeList;
            requestParameter = addNodeListRequest;
        }

        public void RemoveNode(IGraphElement _node, IGraphElement _group)
        {
            requestType = Type.RemoveNode;
            removeNodeRequest = new RemoveNodeRequest();
            removeNodeRequest.group = (Group)_group;
            removeNodeRequest.node = (Node)_node;
            requestParameter = removeNodeRequest;
        }

        public void RemoveNodeList(IGraphElement _group, NodeList _nodeList)
        {
            requestType = Type.RemoveNodeList;
            removeNodeListRequest.group = (Group) _group;
            removeNodeListRequest.nodeList = _nodeList;
            requestParameter = removeNodeListRequest;
        }

        public void Send(IGossip _rumor)
        {
            requestType = Type.Send;
            sendRequest.rumor = _rumor as Rumor;
            requestParameter = sendRequest;
        }

        public void Connect(IGraphElement _node1, IGraphElement _node2, double _cost)
        {
            requestType = Type.ConnectRequest;
            connectRequest.node1 = _node1 as Node;
            connectRequest.node2 = _node2 as Node;
            connectRequest.cost = _cost;
            requestParameter = connectRequest;
        }

        public void SetWorkingContext(String _port)
        {
            requestType = Type.SetWorkingContext;
            setWorkingContextRequest.port = _port;
            requestParameter = setWorkingContextRequest;
        }

        public void InitializationComplete()
        {
            requestType = Type.InitializationComplete;
            requestParameter = initializationCompleteRequest;
        }

        public Object GetParameter()
        {
            return requestParameter;
        }

        #endregion
    }
}
