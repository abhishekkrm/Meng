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
using GOTransport.Common;
using System.Diagnostics;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// Graph representation of the group membership
    /// Represents a graph (G, E), where 
    /// 'G' represents the vertices in the graph - represents the node
    /// 'E' represents the edges in the graph    - such that there is an edge E from node N1 to node N2  
    ///                                            iff N1 and N2 are neighbors
    ///                                            
    /// In addition, two types of mappings are maintained for efficient lookups
    ///             - Group to node mapping 
    ///                 (Hash of group name to list of nodes in that group)
    ///             - Node to Group mapping 
    ///                 (Each node keeps track of its membership to different groups)
    /// </summary>
    public class GroupObjectGraph : IEqualityComparer<IGraphElement>
    {
        #region private fields

        // Maps a group to a list of Nodes that belong to the group
        private Dictionary<IGraphElement, List<IGraphElement>> groupNodeMap;
        private AutoResetEvent NodeInfoNotifier;
        double maxT, c;

        #endregion

        #region protected fields

        protected DebuggingContext debuggingContext;

        #endregion

        #region constructors

        public GroupObjectGraph()
        {
            GraphElementComparer geComparer = new GraphElementComparer();
            groupNodeMap = new Dictionary<IGraphElement, List<IGraphElement>>(geComparer);

            maxT = double.Parse("50");
            c = Double.Parse("1");

            debuggingContext = new DebuggingContext();
        }

        public GroupObjectGraph(Dictionary<string, string> config)
        {
            GraphElementComparer geComparer = new GraphElementComparer();
            groupNodeMap = new Dictionary<IGraphElement, List<IGraphElement>>(geComparer);

            maxT = double.Parse(config["MAX_T"]);
            c = Double.Parse(config["NUMBER_OF_RECEPIENTS"]);

            debuggingContext = new DebuggingContext();
        }

        #endregion

        #region public methods

        /// <summary>
        ///  node joins a group
        /// </summary>
        /// <param name="group">group of interest</param>
        /// <param name="node">node which has to join the group</param>
        public void Join(IGraphElement group, IGraphElement node)
        {
            try
            {
                // Check if there is a node list for this group, otherwise create a new list
                if (groupNodeMap != null && !groupNodeMap.ContainsKey(group))
                {
                    groupNodeMap[group] = new List<IGraphElement>();
                }

                if (groupNodeMap[group] != null && groupNodeMap[group].Contains(node))
                {
                    throw new Exception("duplicate node add attempt");
                }
                groupNodeMap[group].Add(node);
                (node as Node).Join(group);
                this.NodeInfoNotifier.Set();
            }
            catch (Exception e)
            {
                throw new UseCaseException("1200", "GroupObjectGraph::Join " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// node leaves a group
        /// </summary>
        /// <param name="group">group of interest</param>
        /// <param name="node">node which has to leave the group</param>
        public void Leave(IGraphElement group, IGraphElement node)
        {
            try
            {
                if (groupNodeMap != null && groupNodeMap.ContainsKey(group)
                                        && groupNodeMap[group].Contains(node))
                {
                    groupNodeMap[group].Remove(node);
                }

                (node as Node).Leave(group);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1220", "GroupObjectGraph::Leave " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// get a list of all nodes in a group
        /// </summary>
        /// <param name="group">group of interest</param>
        /// <returns>list of nodes which belong to the group of interest</returns>
        public List<IGraphElement> GetNodesInGroup(IGraphElement group)
        {
            try
            {
                return groupNodeMap != null ? groupNodeMap[group]
                                            : null;
            }
            catch (Exception e)
            {
                throw new UseCaseException("1240", "GroupObjectGraph::GetNodesInGroup " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// get a list of all nodes
        /// </summary>
        /// <returns>list of all nodes</returns>
        public List<IGraphElement> GetNodeList()
        {
            try
            {
                List<IGraphElement> allNodes = new List<IGraphElement>();

                if (groupNodeMap == null || groupNodeMap.Values == null
                                         || groupNodeMap.Values.Count == 0)
                {
                    return null;
                }

                foreach (List<IGraphElement> nodeList in groupNodeMap.Values)
                {
                    allNodes.AddRange(nodeList);
                }

                return removeDuplicates(allNodes);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1260", "GroupObjectGraph::GetNodeList " + e + ", " + e.StackTrace);
            }
        }

        static List<IGraphElement> removeDuplicates(List<IGraphElement> inputList)
        {
            try
            {
                Dictionary<IGraphElement, int> uniqueStore = new Dictionary<IGraphElement, int>();
                List<IGraphElement> finalList = new List<IGraphElement>();

                if (inputList == null || inputList.Count == 0)
                {
                    return null;
                }

                foreach (IGraphElement currValue in inputList)
                {
                    if (!uniqueStore.ContainsKey(currValue))
                    {
                        uniqueStore.Add(currValue, 0);
                        finalList.Add(currValue);
                    }
                }
                return finalList;
            }
            catch (Exception e)
            {
                throw new UseCaseException("1280", "GroupObjectGraph::removeDuplicates " + e + ", " + e.StackTrace);
            }
        }
        /// <summary>
        /// get the size (i.e number of nodes) of the group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int GetSize(IGraphElement group)
        {
            return groupNodeMap == null || !groupNodeMap.ContainsKey(group) ? 0
                                                                            : groupNodeMap[group].Count;
        }

        /// <summary>
        /// get the size (i.e number of nodes) in the whole graph
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public int GetSize()
        {
            if (groupNodeMap == null)
            {
                return 0;
            }

            return this.GetNodeList().Count;
        }

        /// <summary>
        /// number of suseptibles
        /// </summary>
        /// <param name="group">group of interest</param>
        /// <param name="timestep">timestep at which we are interested in</param>
        /// <returns>number of suseptibles in the group of interest</returns>
        public double getNumberOfSuseptibles(double n, double timestep)
        {
            try
            {
                double retValue;
                retValue = Math.Exp(-c * timestep / n);                    // for push

                Debug.Assert(n != 0, "n should not be zero");
                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "GroupObjectGraph::GetnumberOfSuseptibles(" + n + ", " + timestep + "): " + retValue);
                return retValue;
            }
            catch (Exception e)
            {
                throw new UseCaseException("1300", "GroupObjectGraph::getNumberOfSuseptibles " + e + ", " + e.StackTrace);
            }
        }

        //Expected hitting time of k special nodes in a group of n
        public double ExpectedHittingTime(double k, double n, int t)
        {
            try
            {
                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "GroupObjectGraph::ExpectedHittingTime: Max_t: " + maxT + ", c: " + c + " t: " + t);

                double p = Math.Exp(Math.Log(1.0 - k / n) * c * (n - getNumberOfSuseptibles(n, t)));
                if (t > maxT)
                {
                    Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                        debuggingContext
                                        + "Exp(" + maxT + ", " + c + ", " + t + "): " + 1);
                    return 1.0;
                }
                else
                {
                    double retValue = (double)t * (1.0 - p) + ExpectedHittingTime(k, n, t + 1) * p;
                    Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                        debuggingContext
                                        + "Exp(" + maxT + ", " + c + ", " + t + "): " + retValue);

                    return retValue;
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("1320", "GroupObjectGraph::ExpectedHittingTime " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// check if a node belongs to a group
        /// </summary>
        /// <param name="group">group which has to be checked</param>
        /// <param name="node">node which has to be checked</param>
        /// <returns>boolean value indicating whether the node belongs to the group or not</returns>
        public bool IsMember(Group group, Node node)
        {
            try
            {
                if (groupNodeMap != null && groupNodeMap.Count != 0)
                {
                    List<IGraphElement> nodes = groupNodeMap[group];
                    if (nodes != null && nodes.Contains(node))
                    {
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                            debuggingContext
                                            + "GroupObjectGraph::IsMember: group " + group.GetID() + ", node " + node.GetID() + ":true");
                        return true;
                    }
                }

                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "GroupObjectGraph::IsMember: group " + group.GetID() + ", node " + node.GetID() + ":false");
                return false;
            }
            catch (Exception e)
            {
                throw new UseCaseException("1340", "GroupObjectGraph::IsMember " + e + ", " + e.StackTrace);
            }
        }

        public Group SelectCmnGroup(IGraphElement node1, IGraphElement node2)
        {
            if (node1 == null || node2 == null)
            {
                return null;
            }

            List<IGraphElement> groupsOfNode1 = node1.GetJoinedGroupList();
            
            if (groupsOfNode1 == null || groupsOfNode1.Count == 0)
            {
                return null;
            }

            foreach (Group group in groupsOfNode1)
            {
                if (IsMember(group as Group, node2 as Node))
                {
                    return group;
                }
            }

            return null;
        }


        #region IEqualityComparer<Group> Members

        bool IEqualityComparer<IGraphElement>.Equals(IGraphElement x, IGraphElement y)
        {

            string xId = (x as Node).GetID();
            string yId = (y as Node).GetID(); ;

            return xId.Equals(yId) ? true
                              : false;

        }

        int IEqualityComparer<IGraphElement>.GetHashCode(IGraphElement obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        public void SetNodeInfoNotifier(System.Threading.AutoResetEvent autoResetEvent)
        {
            this.NodeInfoNotifier = autoResetEvent;
        }

        #endregion
    }
}
