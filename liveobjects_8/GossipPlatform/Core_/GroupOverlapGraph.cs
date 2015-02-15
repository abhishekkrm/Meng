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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Threading;
using GOTransport.Common;
using System.Diagnostics;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// Graph representation of intersections in group membership (common members across groups)
    /// more specifically, a graph (G, E), where 
    /// 'G' represents the vertices in the graph - represents the group
    /// 'E' represents the edges in the graph    - such that there is an edge E from group G1 to Group G2 
    ///                                            iff G1 and G2 overlap (i.e. have atleast one common node)
    ///                                            and the cost of edge is equal to number of common nodes
    /// </summary>
    public class GroupOverlapGraph : GroupObjectGraph
    {
        #region private fields

        // A list of all overlapping groups
        private List<IGraphElement> groupOverlapList;
        private AutoResetEvent allset;

        #endregion

        #region internal methods

        internal void SetDebuggingContext(DebuggingContext _debuggingContext)
        {
            this.debuggingContext = _debuggingContext;
        }

        #endregion

        #region constructors

        /// <summary>
        /// Given an object graph, overlaps can be found out
        /// </summary>
        /// <param name="groupObjectGraph">group object graph from which we derive the overlaps</param>
        public GroupOverlapGraph(ref GroupObjectGraph _groupObjectGraph)
        {
            groupOverlapList = new List<IGraphElement>();

            Update(_groupObjectGraph);
            allset = new AutoResetEvent(false);
        }

        #endregion

        #region public methods

        /// <summary>
        /// overlap size - number of common members between two groups
        /// </summary>
        /// <param name="graph">group object graph</param>
        /// <param name="group1">group 1</param>
        /// <param name="group2">group 2</param>
        /// <returns></returns>
        public int OverlapSize(ref GroupObjectGraph _graph, IGraphElement _group1, IGraphElement _group2)
        {
            try
            {
                List<IGraphElement> nodelist = _graph.GetNodeList();
                int overlapSize = 0;

                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "GroupOverlapGraph::OverlapSize: total number of nodes: " + nodelist.Count);
                foreach (IGraphElement node in nodelist)
                {
                    List<IGraphElement> groupList = node.GetJoinedGroupList();

                    if (groupList.Contains(_group1) && groupList.Contains(_group2))
                    {
                        overlapSize++;
                    }
                }

                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "GroupOverlapGraph::OverlapSize: distance between group ["
                                    + _group1.GetID() + "] and group [" + _group2.GetID() + "] is "
                                    + overlapSize);
                return overlapSize;
            }
            catch (Exception e)
            {
                throw new UseCaseException("1120", "GroupOverlapGraph::OverlapSize " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// Add a group to the overlap graph
        /// </summary>
        /// <param name="group"></param>
        public void Add(IGraphElement _group)
        {
            try
            {
                groupOverlapList.Add(_group);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1140", "GroupOverlapGraph::Add " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// Remove a group from overlap graph
        /// </summary>
        /// <param name="group"></param>
        public void Remove(IGraphElement _group)
        {
            try
            {
                groupOverlapList.Remove(_group);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1160", "GroupOverlapGraph::Remove " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// get a list of all group nodes in the overlap graph
        /// </summary>
        /// <returns></returns>
        public List<IGraphElement> GetGroupOverlapList()
        {
            return groupOverlapList;
        }

        /// <summary>
        /// Node memberships have changed, hence reflect them in group overlap graph
        /// </summary>
        public void Update(GroupObjectGraph _groupObjectGraph)
        {
            try
            {
                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "GroupOverlapGraph::Update: groupOverlap's hashcode: " + this.GetHashCode());

                List<IGraphElement> nodes = _groupObjectGraph.GetNodeList();

                if (nodes == null || nodes.Count == 0)
                {
                    Debug.WriteLineIf(Utils.debugSwitch.Verbose, debuggingContext + "GroupOverlapGraph::Update: no nodes in groupObjectGraph");
                    return;
                }
                Debug.WriteLineIf(Utils.debugSwitch.Verbose, debuggingContext + "GroupOverlapGraph::Update: nodes' hashcode: " + nodes.GetHashCode());

                foreach (IGraphElement node in nodes)
                {
                    List<IGraphElement> groups = node.GetJoinedGroupList();

                    if (groups == null || groups.Count == 0 || groups.Count == 1)
                    {
                        continue;
                    }

                    for (int groupIter1 = 0; groupIter1 < groups.Count; groupIter1++)
                    {
                        Group groupNode1 = groups[groupIter1] as Group;
                        for (int groupIter2 = groupIter1; groupIter2 < groups.Count; groupIter2++)
                        {
                            Group groupNode2 = groups[groupIter2] as Group;

                            if (!groupNode1.GetID().Equals(groupNode2.GetID()))
                            {
                                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                                    debuggingContext
                                                    + "GroupOverlapGraph::Update: Adding overlap between "
                                                    + "group " + groupNode1.GetID() + " and group " + groupNode2.GetID());

                                groupNode1.AddNeighbor(groupNode2, OverlapSize(ref _groupObjectGraph, groupNode1, groupNode2));
                                groupNode2.AddNeighbor(groupNode1, OverlapSize(ref _groupObjectGraph, groupNode2, groupNode1));

                                if (!groupOverlapList.Contains(groupNode2))
                                {
                                    groupOverlapList.Add(groupNode2);
                                }
                            }
                        }
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                            debuggingContext
                                            + "GroupOverlapGraph::Update: Also Adding to groupOverlap[hashcode: "
                                            + groupOverlapList.GetHashCode() + "] " + groupNode1.GetID());
                        if (!groupOverlapList.Contains(groupNode1))
                        {
                            groupOverlapList.Add(groupNode1);
                        }
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose, debuggingContext + "GroupOverlapGraph::Update: groupOverlapList[hashcode: " + groupOverlapList.GetHashCode()
                                        + " size: " + groupOverlapList.Count);
                    }
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("1180", "GroupOverlapGraph::Update " + e + ", " + e.StackTrace);
            }
        }

        internal void AllSet()
        {
            allset.Set();
        }

        public void WaitOne()
        {
            allset.WaitOne();
        }

        public new int GetSize()
        {
            if (groupOverlapList == null || groupOverlapList.Count == 0)
            {
                return 0;
            }

            return groupOverlapList.Count;
        }

        public new List<IGraphElement> GetNodeList()
        {
            return this.groupOverlapList;
        }

        #endregion
    }
}
