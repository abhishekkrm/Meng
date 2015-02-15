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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Membership2.ServerState
{
    public class RegionView
    {
        public RegionView(Region region, uint seqno, ICollection<Node> nodes)
        {
            this.region = region;
            this.seqno = seqno;
			this.nodes = new System.Collections.ObjectModel.ReadOnlyCollection<Node>(new List<Node>(nodes));
        }

        private RegionView()
        {
        }

        private Region region;
        private uint seqno;
        private System.Collections.ObjectModel.ReadOnlyCollection<Node> nodes;

        public Region Region
        {
            get { return region; }
        }

        public uint SeqNo
        {
            get { return seqno; }
        }

        public Base3_.IDWithSequenceNo<Base3_.RegionID> ViewID
        {
            get { return new Base3_.IDWithSequenceNo<Base3_.RegionID>(region.RegionID, seqno); }
        }

        public System.Collections.Generic.ICollection<Node> Nodes
        {
            get { return nodes; }
        }

        public RegionView UpdatedView(ICollection<Node> nodesToAdd, ICollection<Node> nodesToRemove)
        {
            RegionView updatedView = new RegionView();
            updatedView.region = region;
            updatedView.seqno = seqno + 1;
            List<Node> newnodes = new List<Node>(nodes.Count + nodesToAdd.Count - nodesToRemove.Count);
            foreach (Node node in nodes)
            {
                if (!nodesToRemove.Contains(node))
                    newnodes.Add(node);
            }
            newnodes.AddRange(nodesToAdd);
			updatedView.nodes = new System.Collections.ObjectModel.ReadOnlyCollection<Node>(newnodes);

            return updatedView;
        }

        public override string ToString()
        {
            return "(RegionView " + region.RegionID.ToString() + ":" + seqno.ToString() + ")";
/*
            StringBuilder s = new StringBuilder("#" + seqno.ToString() + ", nodes: ");
            foreach (Node node in nodes)
                s.Append(node.ToString() + " ");
            return s.ToString();
*/ 
        }
    }
}
