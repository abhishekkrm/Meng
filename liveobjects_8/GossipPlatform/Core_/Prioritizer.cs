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
using System.Threading;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// Encapsulates the logic required to determine a list of Rumors for tranmistting as part of current message
    /// Requires three pieces of information
    ///     - GroupObjectGraph
    ///     - GroupOverlapGraph
    ///     - RumorBuffer
    /// </summary>
    public class Prioritizer
    {
        #region private fields

        private GroupObjectGraph groupObjectGraph;
        private GroupOverlapGraph groupOverlapGraph;
        private RumorBuffer rumorBuffer;
        private List<IGraphElement> groupNodeList;
        private Double[,] distance;
        private DebuggingContext debuggingContext;

        /*
         * Distances between every pair of group objects in group overlap graph
         */
        private List<DistanceMap> distanceMapList;

        #endregion

        #region types

        /// <summary>
        /// Represents two distance between two IGraphElement objects
        /// </summary>
        class DistanceMap
        {
            internal double cost;

            internal IGraphElement element1;
           
            internal IGraphElement element2;
        }

        #endregion

        #region constructors

        public Prioritizer(ref GroupObjectGraph _groupObjectGraph, 
                            ref GroupOverlapGraph _groupOverlapGraph, 
                            ref RumorBuffer _rumorBuffer,
                            ref DebuggingContext _debuggingContext)
        {
            try
            {
                groupObjectGraph = _groupObjectGraph;
                groupOverlapGraph = _groupOverlapGraph;
                rumorBuffer = _rumorBuffer;
                debuggingContext = _debuggingContext;

                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "Prioritizer::Prioritizer: initializing with size: "
                                    + groupOverlapGraph.GetSize());

                distance = new Double[groupOverlapGraph.GetSize(), groupOverlapGraph.GetSize()];
                distanceMapList = new List<DistanceMap>();
                groupNodeList = groupOverlapGraph.GetNodeList();
                int n = groupNodeList.Count;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        DistanceMap distanceMap = new DistanceMap();
                        distanceMap.element1 = groupNodeList[i];
                        distanceMap.element2 = groupNodeList[j];

                        if (i == j)
                        {
                            distanceMap.cost = 0;
                        }
                        else if (groupNodeList[i].GetNeighborList().ContainsKey(groupNodeList[j]))
                        {
                            distanceMap.cost = groupObjectGraph.ExpectedHittingTime(
                                                    groupOverlapGraph.OverlapSize(ref groupObjectGraph, groupNodeList[i], groupNodeList[j]),
                                                    groupObjectGraph.GetSize(groupNodeList[i]),
                                                    1);
                            Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                            debuggingContext
                                            + "Expected hitting time from "
                                            + groupNodeList[i].GetID() + " to " + groupNodeList[j].GetID()
                                            + " is " + distanceMap.cost);
                        }
                        else
                        {
                            distanceMap.cost = Constants.INFINITY;
                        }
                        distanceMapList.Add(distanceMap);
                        this.setDistance(groupNodeList[i], groupNodeList[j], distanceMap.cost);
                        this.setDistance(groupNodeList[j], groupNodeList[i], distanceMap.cost);
                    }
                }

                this.FindShortestPaths();
            }
            catch (Exception e)
            {
                throw new UseCaseException("700", "Prioritizer::Prioritizer " + e + ", " + e.StackTrace);
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Distance between two IGraphElement objects
        /// </summary>
        /// <param name="element1">IGraphElement of interest</param>
        /// <param name="element2">IGraphElement of interest</param>
        /// <returns></returns>
        private double getDistance(IGraphElement _element1, IGraphElement _element2)
        {
            try
            {
                foreach (DistanceMap map in distanceMapList)
                {
                    if (map.element1 == _element1 && map.element2 == _element2
                        || map.element2 == _element1 && map.element1 == _element2)
                    {
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                            debuggingContext
                                            + "returning cost from " + _element1.GetID()
                                            + " to " + _element2.GetID()
                                            + " is " + map.cost);
                        return map.cost;
                    }
                }

                if (_element1 == _element2)
                {
                    return 0;
                }

                return Constants.INFINITY;
            }
            catch (Exception e)
            {
                throw new UseCaseException("720", "Prioritizer::getDistance " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// Set the distance between two IGraphElement objects
        /// </summary>
        /// <param name="element1">IGraphElement of interest</param>
        /// <param name="element2">IGraphElement of interest</param>
        /// <param name="cost">Distance between the two IGraphElement objects</param>
        private void setDistance(IGraphElement _element1, IGraphElement _element2, double _cost)
        {
            try
            {
                foreach (DistanceMap map in distanceMapList)
                {
                    if (map.element1 == _element1 && map.element2 == _element2)
                    {
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                            debuggingContext
                                            + "setting cost from " + _element1.GetID() + " to " + _element2.GetID()
                                            + " is " + map.cost);
                        map.cost = _cost;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("740", "Prioritizer::setDistance " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// Floyd Warshall algorithm to find shortest paths between every pair of gossip objects
        /// </summary>
        private void FindShortestPaths()
        {
            try
            {
                int n = groupNodeList.Count;
                int k = 0;

                for (k = 0; k < n; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            double dik = getDistance(groupNodeList[i], groupNodeList[k]);
                            double dkj = getDistance(groupNodeList[k], groupNodeList[j]);
                            double dij = getDistance(groupNodeList[i], groupNodeList[j]);
                            if (dik + dkj < dij)
                            {
                                setDistance(groupNodeList[i], groupNodeList[j], dik + dkj);
                                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                                    debuggingContext
                                                    + "Setting distance between "
                                                    + groupNodeList[i].GetID() + " and " + groupNodeList[j]
                                                    + " to " + (dik + dkj));
                            }
                        }
                    }
                }

                /*
                 * Now, we have reconstructed the graph with new edge weights, where each edge represents the 
                 * expected hitting time
                 */
            }
            catch (Exception e)
            {
                throw new UseCaseException("760", "Prioritizer::FindShortestPaths " + e + ", " + e.StackTrace);
            }
        }

        double utility(IGraphElement _srcNode, IGraphElement _destinationNode, double _timestamp)
        {
            try
            {
                List<IGraphElement> dstGroup = _destinationNode.GetJoinedGroupList();
                int dstSize = dstGroup.Count;

                double numerator = 0;

                return (numerator / dstSize);
            }
            catch (Exception e)
            {
                throw new UseCaseException("780", "Prioritizer::utility " + e + ", " + e.StackTrace);
            }
        }

        #endregion

        #region public methods

        // Expected utility of including message r from s to d
        public double utility(IGraphElement _destinationNode, IGossip _rumor)
        {
            try
            {
                IGraphElement j = _rumor.DestinationGroup;
                double min_del = 10000.0;
                double del;

                // Find the best object d is a member of
                List<IGraphElement> dstGroupList = _destinationNode.GetJoinedGroupList();

                for (int i = 0; i < dstGroupList.Count; i++)
                {
                    del = getDistance(dstGroupList[i], j);
                    Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                        debuggingContext
                                        + "Prioritizer::utility: Distance between destination group [" + j.GetID() + "] and "
                                        + " its neigboring group [" + dstGroupList[i].GetID() + "] is " + del);
                    if (del < min_del)
                    {
                        min_del = del;
                    }
                }

                int sizeofJ = groupObjectGraph.GetSize(j);
                double retValue = groupObjectGraph.getNumberOfSuseptibles(sizeofJ, min_del) / sizeofJ;
                Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                    debuggingContext
                                    + "Prioritizer::utility: min_del: " + min_del + ", sizeofJ: "
                                    + sizeofJ + ", utility: " + retValue);
                return retValue;
            }
            catch (Exception e)
            {
                throw new UseCaseException("800", "Prioritizer::utility " + e + ", " + e.StackTrace);
            }
        }

        #endregion
    }
}
