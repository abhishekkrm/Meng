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
    /// A channel that can be used for sending data to the group. Whether it performs buffering underneath, and how promptly it transmits
    /// data, depends on how the channel was created. The default channel buffers messages and sends asynchronously. Channels passed as
    /// an argument to the outgoing callback do not perform buffering and work faster, but using them in a context other than an outgoing 
    /// callback is not safe and can lead to deadlocks or data corruption.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Send a message without confirmation.
        /// </summary>
        /// <param name="message">Message to send.</param>
        void Send(QS.Fx.Serialization.ISerializable message);

        /// <summary>
        /// Send a message with confirmation.
        /// </summary>
        /// <param name="message">Message to end.</param>
        /// <param name="callback">Callback to invoke when message is acknowledged by all recipients.</param>
        /// <param name="context">Context object to pass to the callback.</param>
        void Send(QS.Fx.Serialization.ISerializable message, CompletionCallback callback, object context);        
    }
}
