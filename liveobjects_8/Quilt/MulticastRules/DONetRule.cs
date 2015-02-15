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
using Quilt.Multicast;

namespace Quilt.MulticastRules
{
    public sealed class DONetRule : IRule
    {
        #region Fields

        #endregion

        #region IRule Members

        bool IRule.IsPermitted(Quilt.Bootstrap.GroupManager.MonitorNode node)
        {
            foreach (string patch in node._patches)
            {
                if (patch.Contains(PROTOTYPE.IPMC.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        bool IRule.CanJoin(Quilt.Bootstrap.GroupManager.MonitorNode node, Quilt.Bootstrap.PatchInfo patch)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            EUIDAddress node_euid = member.Addresses.First();
            EUIDAddress patch_euid = patch._patch_euid;

            List<EUIDAddress.RouterInfo> node_stack = node_euid.RouterStack;
            List<EUIDAddress.RouterInfo> patch_stack = patch_euid.RouterStack;

            //Strict version, checks the duplicity of router ip
            foreach (EUIDAddress.RouterInfo router in node_stack)
            {
                // If different clients are behind different interfaces of one router
                // there is no way to identify it for now

                // To identify this, we need to extend HostDetector with SNMP support
                if (patch_stack.Contains(router))
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

            foreach (EUIDAddress.RouterInfo router in node_stack)
            {
                if (!path_stack.Contains(router))
                {
                    path_stack.Add(router);
                }
            }
        }

        QS.Fx.Value.EUIDAddress IRule.CreatePatch(GroupManager.MonitorNode node, Name group_name, out string group_info)
        {
            IMember<Name, Incarnation, Name, EUIDAddress> member = node._node;
            EUIDAddress euid = member.Addresses.First();
            List<EUIDAddress.RouterInfo> node_stack = euid.RouterStack;

            EUIDAddress patch_euid = new EUIDAddress();
            patch_euid.RouterStack = new List<EUIDAddress.RouterInfo>();

            group_info = "";
            foreach (EUIDAddress.RouterInfo router in node_stack)
            {
                patch_euid.RouterStack.Add(router);

                // Set group_info as AS numbers
                if (group_info == "")
                {
                    group_info = router.as_number.ToString();
                }
                else if (!group_info.Contains(router.as_number.ToString()))
                {
                    group_info += ";" + router.as_number.ToString();
                }
            }

            return patch_euid;
        }

        #endregion
    }
}
