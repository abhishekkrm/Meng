/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
using System.Linq;
using System.Text;

using QS.Fx.Base;
using QS.Fx.Value;
using QS.Fx.Value.Classes;

using Quilt.Bootstrap;
using Quilt.HostDetector.NATCheck;

namespace Quilt.MulticastRules
{
    public sealed class OMNIRule:IRule
    {
        #region Field

        private Dictionary<string, GroupManager.MonitorNode> _roots = new Dictionary<string,GroupManager.MonitorNode>();

        #endregion

        #region GetRoot

        private GroupManager.MonitorNode GetRoot(PatchInfo patch)
        {
            string desp = patch._patch_description.String;
            string group_name = desp.Split('|')[2];

            GroupManager.MonitorNode node;
            if (!_roots.TryGetValue(group_name, out node))
            {
                throw new Exception("OMNIRule.GetRoot no root for IPMC address " + group_name);
            }

            return node;
        }

        #endregion

        #region IRule Members

        bool IRule.IsPermitted(Quilt.Bootstrap.GroupManager.MonitorNode node)
        {
            // OMNI protocol has no special permission requirement
            return true;
        }

        bool IRule.CanJoin(Quilt.Bootstrap.GroupManager.MonitorNode node, Quilt.Bootstrap.PatchInfo patch)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            IMember<Name, Incarnation, Name, EUIDAddress> root = GetRoot(patch)._node;

            // Comment out for experiment

            //if (member.Identifier.Equals(root.Identifier))
            //{
            //    // Check root
            //    EUIDAddress node_euid = member.Addresses.First();
            //    EUIDAddress.ProtocolInfo tcp_info = node_euid.GetProtocolInfo("TCP");
            //    EUIDAddress.ProtocolInfo stun_info = node_euid.GetProtocolInfo("UDP");

            //    // Unaccessable root
            //    if (tcp_info.proto_direct >= Direction.DIRECTION.UNIDIRECTION ||
            //        stun_info.proto_direct >= Direction.DIRECTION.UNIDIRECTION)
            //    {
            //        //_roots.Remove(patch._patch_description.String);
            //        return false;
            //    }
            //}

            return true;
        }

        void IRule.Join(Quilt.Bootstrap.GroupManager.MonitorNode node, Quilt.Bootstrap.PatchInfo patch)
        {
            // Do nothing?
            return;
        }

        QS.Fx.Value.EUIDAddress IRule.CreatePatch(GroupManager.MonitorNode node, Name group_name, out string group_info)
        {
            // Set root
            if (!_roots.ContainsKey(group_name.String))
            {
                _roots.Add(group_name.String, node);
            }
            else
            {
                _roots[group_name.String] = node;
            }

            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;

            // Use root's euid as patch euid
            EUIDAddress euid = new EUIDAddress(member.Addresses.First().String);

            // Id of the root node
            group_info = group_name.String + "|" + member.Identifier.String;

            return euid;
        }

        #endregion
    }
}
