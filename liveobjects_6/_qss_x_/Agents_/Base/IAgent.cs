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
    public interface IAgent : IDisposable
    {
        QS._qss_x_.Base1_.AgentID ID
        {
            get;
        }

        QS._qss_x_.Base1_.AgentIncarnation Incarnation
        {
            get;
        }

        void Initialize(QS.Fx.Logging.ILogger logger, IAgent bottomagent);
        void Reconfigure(IConfiguration configuration);
        void Receive(uint configuration_incarnation, uint sender_member_index, QS.Fx.Serialization.ISerializable message);
        
        void StartSession(QS._qss_x_.Base1_.SessionID sessionid, string sessionname, IProtocol protocol);
        void StopSession(QS._qss_x_.Base1_.SessionID sessionid);

        void Register(QS._qss_x_.Base1_.SessionID sessionid, string sessionname, IAgent upperagent);


    }
}
