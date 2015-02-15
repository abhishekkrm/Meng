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

namespace QS._qss_x_.Agents_.Base
{
    public sealed class Session : QS.Fx.Inspection.Inspectable
    {
        internal Session(QS._qss_x_.Base1_.SessionID id, string name, IAgent upperagent, IAgent bottomagent, IProtocol protocol)
        {
            this.id = id;
            this.name = name;
            this.upperagent = upperagent;
            this.bottomagent = bottomagent;
            this.protocol = protocol;
        }

        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.SessionID id;
        [QS.Fx.Base.Inspectable]
        internal string name;
        [QS.Fx.Base.Inspectable]
        internal IAgent upperagent, bottomagent;
        [QS.Fx.Base.Inspectable]
        internal IProtocol protocol;
        // [QS.TMS.Inspection.Inspectable]
        // internal IProtocolBinding binding;
        [QS.Fx.Base.Inspectable]
        internal IProtocolControl controlobject;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentID parentid;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentIncarnation parentincarnation;
        [QS.Fx.Base.Inspectable]
        internal bool parentupdated;
    }
}
