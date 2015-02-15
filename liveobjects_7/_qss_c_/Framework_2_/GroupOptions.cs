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
    /// Options controlling the flow of messages and restrictions on callbacks.
    /// </summary>
    [Flags]
    public enum GroupOptions
    {
        /// <summary>
        /// No options set. Currently this setting is unsupported: the FastSendCallback option is mandatory.
        /// </summary>
        None                                            =      0,
        /// <summary>
        /// This is currently a default and mandatory option. When this option is set, sending callbacks will be invoked in the context
        /// of the core QSM thread. 
        /// Such callbacks are faster and incur less overhead, but the application must not spend too much time
        /// in the callback and must be careful about the use of synchronization, ideally avoiding it, so as not to introduce deadlocks.
        /// </summary>
        FastSendCallback                      =      1,
        /// <summary>
        /// When this option is set, receive callbacks are invoked in the context of the QSM thread.  
        /// Such callbacks are faster and incur less overhead, but the application must not spend too much time
        /// in the callback and must be careful about the use of synchronization, ideally avoiding it, so as not to introduce deadlocks.
        /// By default, this option is off, and receive callbacks are executed in the context of a separate thread.
        /// </summary>
        FastReceiveCallback                 =      2,        
        /// <summary>
        /// When this option is set, completion callbacks are invoked in the context of the QSM thread.
        /// Such callbacks are faster and incur less overhead, but the application must not spend too much time
        /// in the callback and must be careful about the use of synchronization, ideally avoiding it, so as not to introduce deadlocks.
        /// By default, this option is off, and completion callbacks are executed in the context of a separate thread.
        /// </summary>
        FastCompletionCallback            =      4,
        /// <summary>
        /// Turns on fast processing for send, receive and completion callbacks.
        /// </summary>
        FastCallbacks                             =      FastSendCallback | FastReceiveCallback | FastCompletionCallback,
        /// <summary>
        /// Enables hybrid dissemination mode, where data is initially multicast to a per-group multicast address, to reduce
        /// the network overhead in case the group consists of a larger number of regions.
        /// </summary>
        Hybrid = 8,
        /// <summary>
        /// The default setting, fast send callbacks, and safe (slow) receive and completion callbacks.
        /// </summary>
        Defaults                                       =      FastSendCallback
    }
}
