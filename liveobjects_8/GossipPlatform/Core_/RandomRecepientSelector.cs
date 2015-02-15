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
using System.Linq;
using GOTransport;
using GOTransport.Common;
using System.Diagnostics;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// selector for randomly selecting recepient graph element
    /// </summary>
    public class RandomRecepientSelector : IRecepientSelector
    {
        #region private fields

        private GroupObjectGraph groupObjectGraph;
        private GroupOverlapGraph groupOverlapGraph;
        private NodeAdaptor nodeAdaptor;
        private string name;
        private ICollection<Node> recepientList;
        private Node currentNode;
        private int numberOfNeighbors;

        #endregion

        #region constructors

        public RandomRecepientSelector(ref string _name,
            ref GroupObjectGraph _groupObjectGraph,
            ref GroupOverlapGraph _groupOverlapGraph,
            ref NodeAdaptor _nodeAdaptor)
        {
            name = _name;
            groupObjectGraph = _groupObjectGraph;
            groupOverlapGraph = _groupOverlapGraph;
            nodeAdaptor = _nodeAdaptor;
            numberOfNeighbors = 0;
        }

        #endregion

        #region public methods

        public void SetWorkingGraph()
        {
            currentNode = nodeAdaptor.GetGraphElementByName(this.name) as Node;
            SerializableDictionary<Node, double> neighbors = currentNode.getNeighborList();

            if (neighbors == null || neighbors.Count == 0 || neighbors.Keys == null || neighbors.Keys.Count == 0)
            {
                Console.WriteLine("RandomRecepientSelector::Select: current node [" + currentNode.Id
                                + " does not have any neighbors, exiting");
                recepientList = null;
                return;
            }
            recepientList = neighbors.Keys;
            numberOfNeighbors = recepientList.Count;
        }

        #endregion

        #region IRecepientSelector Members

        IGraphElement IRecepientSelector.Select()
        {
            if (numberOfNeighbors > 0)
            {
                int rand = new Random().Next(0, numberOfNeighbors);

                return recepientList.ElementAt(rand);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
