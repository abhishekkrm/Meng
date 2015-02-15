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
using System.Runtime.Remoting;

namespace GOBaseLibrary.Interfaces
{
    /// <summary>
    /// Node representation
    ///     - A node has an ID and address
    ///     - Knows about groups to which it belongs
    ///     - Knows about its neighbors (and cost of reaching them)
    /// </summary>
    [QS.Fx.Reflection.ValueClass("100`1", "IGraphElement")]
    public interface    IGraphElement
    {
        String GetID();
        void SetID(String nodeID);
        
        String GetAddress();
        void SetAddress(String nodeAddress);
        
        void AddNeighbor(IGraphElement node, double edgeCost);
        void RemoveNeighbor(IGraphElement node);

        double GetEdgeCost(IGraphElement neighbor);
        void SetEdgeCost(IGraphElement neighbor, double cost);

        List<IGraphElement> GetJoinedGroupList();
        IDictionary<IGraphElement, double> GetNeighborList();
    }
}
