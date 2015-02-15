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
using GOTransport.Common;
using System.Diagnostics;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// Provides a mechanism to add/remove nodes from the group object graph
    /// </summary>
    public class NodeAdaptor
    {
        #region private fields

        private GroupObjectGraph groupObjectGraph;
        private Dictionary<string, Node> nodeMap = new Dictionary<string,Node>();
        private Dictionary<string, Group> groupMap = new Dictionary<string,Group>();

        //observer of changes to node membership information
        private GroupOverlapGraph groupOverlapGraph;

        #endregion

        #region constructors

        public NodeAdaptor(ref GroupObjectGraph nodeGraph, ref GroupOverlapGraph ovGraph)
        {
            groupObjectGraph = nodeGraph;
            groupOverlapGraph = ovGraph;
        }

        #endregion

        #region public methods

        public void AddNode(IGraphElement _groupId, IGraphElement _nodeId)
        {
            try
            {
                if (!nodeMap.ContainsKey(_nodeId.GetID()))
                {
                    Node node = _nodeId as Node;
                    nodeMap[_nodeId.GetID()] = node;
                }

                if (!groupMap.ContainsKey(_groupId.GetID()))
                {
                    Group group = _groupId as Group;
                    groupMap[_groupId.GetID()] = group;
                }

                groupObjectGraph.Join(groupMap[_groupId.GetID()], nodeMap[_nodeId.GetID()]);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1000", "NodeAdaptor::AddNode " + e + ", " + e.StackTrace);
            }
        }

        public void AddNodeList(IGraphElement _group, NodeList _nodeList)
        {
            try
            {
                foreach (IGraphElement node in _nodeList.GetList())
                {
                    AddNode(_group, node);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("1020", "NodeAdaptor::AddNodeList " + e + ", " + e.StackTrace);
            }
        }

        public void RemoveNode(IGraphElement _group, IGraphElement _node)
        {
            try
            {
                if (!nodeMap.ContainsKey(_node.GetID()) || !groupMap.ContainsKey(_group.GetID()))
                {
                    throw new Exception("group or node to be removed is not found");
                }

                groupObjectGraph.Leave(groupMap[_group.GetID()], nodeMap[_node.GetID()]);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1040", "NodeAdaptor::RemoveNode " + e + ", " + e.StackTrace);
            }
        }

        public void RemoveNodeList(IGraphElement _group, NodeList _nodeList)
        {
            try
            {
                foreach (IGraphElement node in _nodeList.GetList())
                {
                    RemoveNode(_group, node);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("1060", "NodeAdaptor::RemoveNodeList " + e + ", " + e.StackTrace);
            }
        }

        public void Connect(Node _node1, Node _node2, double _cost)
        {
            try
            {
                if (!nodeMap.ContainsKey(_node1.GetID()) || !nodeMap.ContainsKey(_node2.GetID()))
                {
                    throw new Exception("atleast one node to connect does not exist");
                }

                // Add an undirected edge between the two nodes
                nodeMap[_node1.GetID()].AddNeighbor(nodeMap[_node2.GetID()], _cost);
                nodeMap[_node2.GetID()].AddNeighbor(nodeMap[_node1.GetID()], _cost);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1080", "NodeAdaptor::Connect " + e + ", " + e.StackTrace);
            }
        }

        public void DisConnect(Node _node1, Node _node2)
        {
            try
            {
                if (!nodeMap.ContainsKey(_node1.GetID()) || !nodeMap.ContainsKey(_node2.GetID()))
                {
                    throw new Exception("atleast one node to disconnect does not exist");
                }

                nodeMap[_node1.GetID()].RemoveNeighbor(nodeMap[_node2.GetID()]);
                nodeMap[_node2.GetID()].RemoveNeighbor(nodeMap[_node1.GetID()]);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1100", "NodeAdaptor::DisConnect " + e + ", " + e.StackTrace);
            }
        }

        public IGraphElement GetGraphElementByName(string _name)
        {
            return nodeMap.ContainsKey(_name)
                            ? nodeMap[_name]
                            : null;
        }

        public IGraphElement GetGroupElementByName(string _name)
        {
            return groupMap.ContainsKey(_name)
                            ? groupMap[_name]
                            : null;
        }

        public void InitializationComplete()
        {
            this.Notify();
        }

        /// <summary>
        /// Update the group overlap graph
        /// </summary>
        private void Notify()
        {
            //groupOverlapGraph.Update(groupObjectGraph);
            groupOverlapGraph.AllSet();
        }
        #endregion
    }
}
