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
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GOBaseLibrary.Interfaces;

namespace GOBaseLibrary.Common
{
    /// <summary>
    /// Represents a concrete node in a graph
    /// </summary>
    [Serializable]
    public class Node : IGraphElement
    {
        private String id;
        private String address;
        private SerializableDictionary<Node, double> neighborIDCostMap; // cost of edges between the graph elements
        private List<IGraphElement> groupList;                          // groups to which the node belongs to
        private string ip, port;

        public String Port { get { return port; } set { port = value; } }
        public String Ip { get { return ip; } set { ip = value; } }

        private SerializableDictionary<Node, double> NeighborMap
        {
            get { return neighborIDCostMap; }
            set { neighborIDCostMap = value; }
        }

        [XmlElement]
        public String Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// address contains ip and port separated by a colon
        /// </summary>
        [XmlElement]
        public String Address
        {
            get { return address; }
            set { 
                    address = value; 
                    string[] ipPort = address.Split(':');
                    ip = ipPort[0];
                    port = ipPort[1];
                }
        }
        
        public Node()
        {
            id = "";
            neighborIDCostMap = new SerializableDictionary<Node, double>();
            groupList = new List<IGraphElement>();
        }

        public Node(String nID)
        {
            id = nID;
            neighborIDCostMap = new SerializableDictionary<Node, double>();
            groupList = new List<IGraphElement>();
        }

        public void printGroupList()
        {
            Console.WriteLine("Grouplist: " + groupList);
        }

        #region IGraphElement Members

        /// <summary>
        /// Join a group
        /// </summary>
        /// <param name="group">Group to which the node has to join</param>
        public void Join(IGraphElement group)
        {
            try
            {
                if (groupList != null)
                {
                    groupList.Add(group);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("340", "Node::Join " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// Leave a group
        /// </summary>
        /// <param name="group">Group from which the node has to leave</param>
        public void Leave(IGraphElement group)
        {
            try
            {
                if (groupList != null)
                {
                    groupList.Remove(group);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("360", "Node::Leave " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// an element could belong to many groups
        /// </summary>
        /// <returns>a list of groups to which the current element belongs to</returns>
        public List<IGraphElement> GetJoinedGroupList()
        {
            return groupList;
        }

        /// <summary>
        /// an element could be connected to many neighboring elements
        /// </summary>
        /// <returns>get the neighbors of current element</returns>
        IDictionary<IGraphElement, double> IGraphElement.GetNeighborList()
        {
            try
            {
                if (neighborIDCostMap == null || neighborIDCostMap.Count == 0)
                {
                    return null;
                }

                Dictionary<IGraphElement, double> newDict = new Dictionary<IGraphElement, double>();

                foreach (KeyValuePair<Node, double> kv in this.neighborIDCostMap)
                {
                    newDict.Add(kv.Key, kv.Value);
                }

                return newDict;
            }
            catch (Exception e)
            {
                throw new UseCaseException("380", "Node::GetNeighborList " + e + ", " + e.StackTrace);
            }
        }



        public SerializableDictionary<Node, double> getNeighborList()
        {
            return (neighborIDCostMap);
        }

        /// <summary>
        /// Add an edge for the neighbor and the cost of reaching it
        /// </summary>
        /// <param name="node">neighboring node to add</param>
        /// <param name="edgeCost">cost of reaching the neighbor</param>
        public void AddNeighbor(IGraphElement node, double edgeCost)
        {
            try
            {
                /*
                 * Remove the edge if it already exists
                 */
                if (neighborIDCostMap.Keys != null
                    && neighborIDCostMap.ContainsKey(node as Node))
                {
                    neighborIDCostMap.Remove(node as Node);
                }

                /*
                 * Add the edge
                 */
                neighborIDCostMap.Add(node as Node, edgeCost);
            }
            catch (Exception e)
            {
                throw new UseCaseException("400", "Node::AddNeighbor " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// remove an edge for the neighbor
        /// </summary>
        /// <param name="node">neighboring node to remove</param>
        public void RemoveNeighbor(IGraphElement node)
        {
            try
            {
                neighborIDCostMap.Remove(node as Node);
            }
            catch (Exception e)
            {
                throw new UseCaseException("420", "Node::RemoveNeighbor " + e + ", " + e.StackTrace);
            }
        }

        public string GetID()
        {
            return id;
        }

        public void SetID(String nodeId)
        {
            id = nodeId;
        }

        public double GetEdgeCost(IGraphElement neighbor)
        {
            try
            {
                return neighborIDCostMap == null || !neighborIDCostMap.ContainsKey(neighbor as Node) ? Constants.INFINITY
                                                                                          : neighborIDCostMap[neighbor as Node];
            }
            catch (Exception e)
            {
                throw new UseCaseException("440", "Node::GetEdgeCost " + e + ", " + e.StackTrace);
            }
        }

        public void SetEdgeCost(IGraphElement neighbor, double cost)
        {
            try
            {
                if (neighborIDCostMap != null && neighborIDCostMap.ContainsKey(neighbor as Node))
                {
                    neighborIDCostMap[neighbor as Node] = cost;
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("460", "Node::SetEdgeCost " + e + ", " + e.StackTrace);
            }
        }

        public string GetAddress()
        {
            return address;
        }

        public void SetAddress(String nodeAddress)
        {
            address = nodeAddress;
            
            string[] ipPort = address.Split(':');
            ip = ipPort[0];
            port = ipPort[1];
        }

        #endregion

        /// <summary>
        /// two elements are considered equal if their IDs match
        /// </summary>
        /// <param name="right">element to be compared with 'this' element</param>
        /// <returns>true if IDs are equal, false otherwise</returns>
        public override bool Equals(object right)
        {
            if (right == null)
                return false;

            return this.id == (right as Node).Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // deep copy in separate memory space
        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
}
