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
    public class GroupView
    {
        public GroupView(Group group, uint seqno, ICollection<RegionView> subViews)
        {
            this.group = group;
            this.seqno = seqno;
			this.regionViews = new System.Collections.ObjectModel.ReadOnlyCollection<RegionView>(new List<RegionView>(subViews));
        }

        private GroupView()
        {
        }

        private Group group;
        private uint seqno;
        private System.Collections.ObjectModel.ReadOnlyCollection<RegionView> regionViews;

        public Base3_.IDWithSequenceNo<Base3_.GroupID> ViewID
        {
            get { return new Base3_.IDWithSequenceNo<Base3_.GroupID>(group.GroupID, seqno); }
        }

        public uint SeqNo
        {
            get { return seqno; }
        }

        public System.Collections.Generic.ICollection<RegionView> SubViews
        {
            get { return regionViews; }
        }

        public ICollection<Node> Nodes
        {
            get
            {
                List<Node> nodes = new List<Node>();
                foreach (RegionView regionView in regionViews)
                    nodes.AddRange(regionView.Nodes);
                return nodes;
            }
        }

        public ICollection<Base3_.IDWithSequenceNo<Base3_.RegionID>> SubViewIDs
        {
            get
            {
                Base3_.IDWithSequenceNo<Base3_.RegionID>[] subviewIDs = new Base3_.IDWithSequenceNo<Base3_.RegionID>[regionViews.Count];
                int ind = 0;
                foreach (RegionView regionView in regionViews)
                    subviewIDs[ind++] = regionView.ViewID;
                return subviewIDs;
            }
        }

        public GroupView CalculateUpdatedView(Components_1_.SetChangeWithUpdates<RegionView> changes, 
            out IList<Node> removed, out IList<Node> added, out IList<Node> unaffected)
        {
            List<Node> nodes_removed = new List<Node>();
            List<Node> nodes_added = new List<Node>();
            List<Node> nodes_unaffected = new List<Node>();

            GroupView updatedView = new GroupView();
            updatedView.group = group;
            updatedView.seqno = seqno + 1;
            List<RegionView> newsubviews = new List<RegionView>(regionViews.Count + changes.ToAdd.Count - changes.ToRemove.Count);

            foreach (RegionView subview in regionViews)
            {
                newsubviews.Add(subview);
                nodes_unaffected.AddRange(subview.Nodes);
            }

            foreach (RegionView subview in changes.ToRemove)
            {
                newsubviews.Remove(subview);
                foreach (Node node in subview.Nodes)
                {
                    nodes_unaffected.Remove(node);
                    nodes_removed.Add(node);
                }
            }

            foreach (Base3_.Pair<RegionView> subview_pair in changes.ToUpdate)
            {
                newsubviews.Remove(subview_pair.Element1);
                newsubviews.Add(subview_pair.Element2);
                foreach (Node node in subview_pair.Element1.Nodes)
                {
                    nodes_unaffected.Remove(node);
                    nodes_removed.Add(node);
                }
                foreach (Node node in subview_pair.Element2.Nodes)
                {
                    if (nodes_removed.Contains(node))
                    {
                        nodes_removed.Remove(node);
                        nodes_unaffected.Add(node);
                    }
                    else
                    {
                        nodes_added.Add(node);
                    }
                }
            }

            foreach (RegionView subview in changes.ToAdd)
            {
                newsubviews.Add(subview);
                foreach (Node node in subview.Nodes)
                {
                    if (nodes_removed.Contains(node))
                    {
                        nodes_removed.Remove(node);
                        nodes_unaffected.Add(node);
                    }
                    else
                    {
                        nodes_added.Add(node);
                    }
                }
            }

			updatedView.regionViews = new System.Collections.ObjectModel.ReadOnlyCollection<RegionView>(newsubviews);

            removed = nodes_removed;
            added = nodes_added;
            unaffected = nodes_unaffected;

            return updatedView;
        }

        public override string ToString()
        {
            return "(GroupView " + group.GroupID.ToString() + ":" + seqno.ToString() + ")";
/*
            StringBuilder s = new StringBuilder("#" + seqno.ToString() + ", subviews: ");
            foreach (RegionView subview in regionViews)
                s.Append(subview.Region.RegionID.ToString() + ":" + subview.SeqNo.ToString() + " ");
            return s.ToString();
*/ 
        }
    }
}
