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

namespace QS.Fx.Network
{
    /// <summary>
    /// This interface is implemented by data feeds that can produce streams of pakcets on demand.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Get the next packet from the stream.
        /// </summary>
        /// <param name="maximumSize">The maximum number of bytes to return in the packet.</param>
        /// <param name="data">The list of contiguous memory segments that constitute the packet.</param>
        /// <param name="callback">The callback to be invoked when the packet has been physically transmitted.</param>
        /// <param name="context">The context object to be passed to the callback.</param>
        /// <param name="moreAvailable">Indicates whether there are more packets to send in this stream.</param>
        /// <returns>Tru if a packet was produced, false if the strream could not produce a packet. If the result is true, the stream
        /// will remain registered with the data sink. If the result is false, the stream will be unregistered and detached from the sink.</returns>
        bool Get(int maximumSize, 
            out QS.Fx.Network.Data data, out QS.Fx.Base.ContextCallback callback, out object context, out bool moreAvailable);
    };
}
