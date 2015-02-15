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

namespace Quilt.MulticastRules
{
    public interface IRule
    {
        /// <summary>
        /// True if node is permitted to run protocol
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool IsPermitted(GroupManager.MonitorNode node);

        /// <summary>
        /// True if node can join patch
        /// </summary>
        /// <param name="node"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        bool CanJoin(GroupManager.MonitorNode node, PatchInfo patch);

        /// <summary>
        /// Add node to a patch
        /// </summary>
        /// <param name="node"></param>
        /// <param name="patch"></param>
        void Join(GroupManager.MonitorNode node, PatchInfo patch);

        /// <summary>
        /// Create a new patch using node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        EUIDAddress CreatePatch(GroupManager.MonitorNode node, Name group_name, out string group_info);
    }
}
