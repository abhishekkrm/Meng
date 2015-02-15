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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_e_.Patterns_
{
    public class OverlapPattern1 : IOverlapPattern
    {
        public OverlapPattern1(int numberOfNodes, int numberOfGroups, int numberOfRegions, int numberOfGroupsPerNode) 
            // , double averageGroupSize) 
            // int numberOfRegions) //, int groupSize, int regionSize)
        {
            this.numberOfNodes = numberOfNodes;
            this.numberOfGroups = numberOfGroups;
            this.numberOfRegions = numberOfRegions;
            this.numberOfGroupsPerNode = numberOfGroupsPerNode;

            node2region = new int[numberOfNodes];
            region2node = new List<int>[numberOfRegions];
            region2group = new List<int>[numberOfRegions];
            group2region = new List<int>[numberOfGroups];

            int nodesConsumed = 0;
            for (int regionsConsumed = 0; regionsConsumed < numberOfRegions; regionsConsumed++)
            {
                region2node[regionsConsumed] = new List<int>();

                int nodesToConsume = (int)
                    Math.Round(((double) (numberOfNodes - nodesConsumed)) / ((double) (numberOfRegions - regionsConsumed)));
                while (nodesToConsume > 0)
                {
                    node2region[nodesConsumed] = regionsConsumed;
                    region2node[regionsConsumed].Add(nodesConsumed);

                    nodesConsumed++;
                    nodesToConsume--;
                }
            }

            int ind;

            Queue<int> regionQueue = new Queue<int>();
            for (ind = 0; ind < numberOfGroups; ind++)
                regionQueue.Enqueue(ind);

            for (ind = 0; ind < numberOfGroups; ind++)
                group2region[ind] = new List<int>();

            for (ind = 0; ind < numberOfRegions; ind++)
            {
                region2group[ind] = new List<int>();
                for (int iind = 0; iind < numberOfGroupsPerNode; iind++)
                {
                    int groupno = regionQueue.Dequeue();
                    regionQueue.Enqueue(groupno);

                    region2group[ind].Add(groupno);
                    group2region[groupno].Add(ind);
                }
            }

            averageGroupSize = 0;
            averageRegionsPerGroups = 0;
            for (ind = 0; ind < numberOfGroups; ind++)
            {
                averageRegionsPerGroups += group2region[ind].Count;
                foreach (int regionNo in group2region[ind])
                    averageGroupSize += region2node[regionNo].Count;
            }

            averageGroupSize = averageGroupSize / numberOfGroups;
            averageRegionsPerGroups = averageRegionsPerGroups / numberOfGroups;
        }

        private int numberOfNodes, numberOfGroups, numberOfRegions, numberOfGroupsPerNode; // , groupSize, regionSize;
        private int[] node2region;
        private List<int>[] region2node, region2group, group2region;
        private double averageGroupSize, averageRegionsPerGroups;

        #region Printing

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.Append("node2region = ");
            foreach (int x in node2region)
                s.Append(x.ToString() + " ");
            s.AppendLine();

            s.Append("region2node = ");
            foreach (List<int> x in region2node)
            {
                s.Append("( ");
                foreach (int y in x)
                    s.Append(y.ToString() + " ");
                s.Append(") ");
            }
            s.AppendLine();

            s.Append("region2group = ");
            foreach (List<int> x in region2group)
            {
                s.Append("( ");
                foreach (int y in x)
                    s.Append(y.ToString() + " ");
                s.Append(") ");
            }
            s.AppendLine();

            s.Append("group2region = ");
            foreach (List<int> x in group2region)
            {
                s.Append("( ");
                foreach (int y in x)
                    s.Append(y.ToString() + " ");
                s.Append(") ");
            }
            s.AppendLine();

            return s.ToString();
        }

        #endregion

        #region IOverlapPattern Members

        double IOverlapPattern.AverageGroupSize
        {
            get { return averageGroupSize; }
        }

        double IOverlapPattern.AverageRegionsPerGroup
        {
            get { return averageRegionsPerGroups; }
        }

        double IOverlapPattern.AverageGroupsPerNode
        {
            get { return (double)numberOfGroupsPerNode; }
        }

        double IOverlapPattern.AverageRegionSize
        {
            get { return ((double)numberOfNodes) / ((double)numberOfRegions); }
        }

        int IOverlapPattern.NumberOfNodes
        {
            get { return numberOfNodes; }
        }

        int IOverlapPattern.NumberOfGroups
        {
            get { return numberOfGroups; }
        }

        int IOverlapPattern.NumberOfRegions
        {
            get { return numberOfRegions; }
        }

        #endregion
    }
}
