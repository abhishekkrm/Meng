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
    /// Callback invoked when sending is permitted by rate, flow and concurrency controllers. 
    /// Sending at this time prevents excessive buffering.
    /// </summary>
    /// <param name="unbufferedChannel">A channel to use for sending in this callback. Although the same channel will be provided in 
    /// each callback, using it outside the scope of this callback is not safe, and is likely to cause deadlocks and data corruption.</param>
    /// <param name="maximumNumberOfSendsPermittedAtThisTime">Number of send calls permitted at this time.</param>
    /// <param name="wouldLikeToKeepSending">Setting this parameter to true will keep this callback registered with the group, so that
    /// it would be invoked next time sending is permitted. Setting it to false will unregister the callback. After callback is unregistered, it must
    /// be explicitly re-registered via "ScheduleSend".</param>
    public delegate void OutgoingCallback(
        IChannel unbufferedChannel, uint maximumNumberOfSendsPermittedAtThisTime, out bool wouldLikeToKeepSending);
}
