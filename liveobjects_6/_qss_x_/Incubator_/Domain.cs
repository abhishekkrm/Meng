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

namespace QS._qss_x_.Incubator_
{
    public sealed class Domain : QS.Fx.Inspection.Inspectable
    {
        internal Domain(QS._qss_x_.Base1_.QualifiedID id, bool isroot, bool islocal, Domain superdomain, Domain[] subdomains, 
            Node node, Cluster cluster, Agent agent, QS._qss_c_.Base3_.Logger logger)
        {
            this.id = id;
            this.isroot = isroot;
            this.islocal = islocal;
            this.superdomain = superdomain;
            this.subdomains = subdomains;
            this.node = node;
            this.cluster = cluster;
            this.agent = agent;
            this.logger = logger;
        }

        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Base3_.Logger logger;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.QualifiedID id;
        [QS.Fx.Base.Inspectable]
        internal bool isroot, islocal;
        [QS.Fx.Base.Inspectable]
        internal Domain superdomain;
        [QS.Fx.Base.Inspectable]
        internal Domain[] subdomains;
        [QS.Fx.Base.Inspectable]
        internal Node node;
        [QS.Fx.Base.Inspectable]
        internal Cluster cluster;
        [QS.Fx.Base.Inspectable]
        internal Agent agent;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentIncarnation maxincarnation;
        [QS.Fx.Base.Inspectable]
        internal uint maxclusterincarnation;
    }
}
