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

namespace QS._qss_c_.Framework_2_
{
    /// <summary>
    /// A disposable local client reference to a group. Multiple such references can be opened by the application independently.
    /// </summary>
    public interface IGroup : IDisposable
    {
        /// <summary>
        /// Unique group identifier.
        /// </summary>
        QS._qss_c_.Base3_.GroupID ID
        {
            get;
        }

        /// <summary>
        /// Invoked to dispatch a message received either from the network or via a software loopback.
        /// </summary>
        event IncomingCallback OnReceive;

        /// <summary>
        /// A channel that supports buffering, safe to use in any context.
        /// </summary>
        IChannel BufferedChannel
        {
            get;
        }

        /// <summary>
        /// Schedules sending to occur at a time when it is permitted by rate, flow and concurrency controllers, 
        /// so that it can be done without excessive buffering.
        /// </summary>
        /// <param name="callback">Callback to invoke when sending is permitted.</param>
        void ScheduleSend(OutgoingCallback callback);
    }
}
