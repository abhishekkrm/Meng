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

namespace QS._qss_c_.Membership_3_.ClientState
{
/*
    public sealed class GroupView
    {
        public GroupView(Group group, uint sequenceNo, IEnumerable<MetaNode> metanodes)
        {
            this.group = group;
            this.sequenceNo = sequenceNo;

            IDictionary<Base3.RVID, MetaNode> metanode_dic = new Dictionary<Base3.RVID, MetaNode>();
            foreach (MetaNode metanode in metanodes)
                metanode_dic.Add(metanode.RegionViewID, metanode);
            Base3.RVID[] metanode_ids = (new List<Base3.RVID>(metanode_dic.Keys)).ToArray();
            Array.Sort<Base3.RVID>(metanode_ids);
            this.metanodes = new MetaNode[metanode_ids.Length];
            for (int ind = 0; ind < metanode_ids.Length; ind++)
                this.metanodes[ind] = metanode_dic[metanode_ids[ind]];
        }

        private Group group;
        private uint sequenceNo;
        private MetaNode[] metanodes;
        private GroupViewRevision currentRevision;
        private GlobalView currentGlobalView;

        #region Accessors

        public Group Group
        {
            get { return group; }
        }

        public uint SequenceNo
        {
            get { return sequenceNo; }
        }

        public MetaNode[] MetaNodes
        {
            get { return metanodes; }
        }

        public GroupViewRevision CurrentRevision
        {
            get { return currentRevision; }
        }

        public GlobalView CurrentGlobalView
        {
            get { return currentGlobalView; }
        }

        #endregion

/-*
        #region Updates

        public void CreateRevision(
            uint groupViewRevisionSequenceNo, QS.Fx.Serialization.ISerializable groupViewRevisionAttributes, 
            bool incremental, IList<Base3.RVRevID> regionViewRevisions)
        {

            // TODO: Implement this...............................................

        }

        #endregion
*-/
    }
*/
}
