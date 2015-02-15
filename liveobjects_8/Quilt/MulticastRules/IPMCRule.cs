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
using System.Net;

using QS.Fx.Base;
using QS.Fx.Value;
using QS.Fx.Value.Classes;

using Quilt.Bootstrap;

namespace Quilt.MulticastRules
{
    public sealed class IPMCRule : IRule
    {
        #region Field

        // Group to IPMC address, starting from 224.0.1.0 to 238.255.255.255
        // for testing, use 224.0.1.0 ~ 224.0.1.255
        private Dictionary<string, string> _group_infos = new Dictionary<string, string>();
        private IPAddress _last_mc_addr = IPAddress.Parse("224.0.23.178");

        #endregion

        #region IRule Members

        bool IRule.IsPermitted(Quilt.Bootstrap.GroupManager.MonitorNode node)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            EUIDAddress euid = member.Addresses.First();

            // Check whether this node is behind IPMC enabled router
            if (euid.IPMCRange > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IRule.CanJoin(Quilt.Bootstrap.GroupManager.MonitorNode node, Quilt.Bootstrap.PatchInfo patch)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            EUIDAddress euid = member.Addresses.First();

            // Check whether the node has a shared IPMC path with the patch
            List<EUIDAddress.RouterInfo> node_stack = euid.RouterStack;
            List<EUIDAddress.RouterInfo> path_stack = patch._patch_euid.RouterStack;

            for (int i = 0; i < euid.IPMCRange; i++)
            {
                EUIDAddress.RouterInfo router = node_stack[i];

                // If different clients are behind different interfaces of one router
                // there is no way to identify it for now

                // To identify this, we need to extend HostDetector with SNMP support
                if (path_stack.Contains(router))
                {
                    return true;
                }
            }

            return false;
        }

        void IRule.Join(Quilt.Bootstrap.GroupManager.MonitorNode node, Quilt.Bootstrap.PatchInfo patch)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            EUIDAddress euid = member.Addresses.First();

            List<EUIDAddress.RouterInfo> node_stack = euid.RouterStack;
            List<EUIDAddress.RouterInfo> path_stack = patch._patch_euid.RouterStack;

            // Add IPMC enabled routers of new node into patch collection
            for (int i = 0; i < euid.IPMCRange; i++)
            {
                EUIDAddress.RouterInfo router = node_stack[i];
                if (!path_stack.Contains(router))
                {
                    path_stack.Add(router);
                }
            }
        }

        EUIDAddress IRule.CreatePatch(Quilt.Bootstrap.GroupManager.MonitorNode node, Name group_name, out string group_info)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            EUIDAddress euid = member.Addresses.First();
            List<EUIDAddress.RouterInfo> node_stack = euid.RouterStack;

            EUIDAddress patch_euid = new EUIDAddress();
            patch_euid.RouterStack = new List<EUIDAddress.RouterInfo>();

            // Add IPMC enabled routers of new node into patch collection
            for (int i = 0; i < euid.IPMCRange; i++)
            {
                EUIDAddress.RouterInfo router = node_stack[i];
                patch_euid.RouterStack.Add(router);
            }

            if (!_group_infos.TryGetValue(group_name.String, out group_info))
            {
                // Increase the ip multicast address for new group
                Byte[] bytes = _last_mc_addr.GetAddressBytes();
                if (bytes[3] <= 255)
                {
                    bytes[3]++;
                }
                else
                {
                    bytes[3] = 0;
                }
                _last_mc_addr = new IPAddress(bytes);

                // Return the new address
                group_info = _last_mc_addr.ToString();

                // Insert new group
                _group_infos.Add(group_name.String, group_info);
            }

            return patch_euid;
        }

        #endregion
    }
}
