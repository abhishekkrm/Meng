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

namespace QS.Fx.Base
{
    [Flags]
    public enum SynchronizationOption : int
    {
        // the lack of a given attribute means the default behavior is used
        None = 0,

        // calls go through a non-blocking queue
        // inherited
        // overridden by synchronous
        Asynchronous = 1,

        // calls are direct
        // inherited
        // overrides asynchronous
        Synchronous = 2,

        // calls are scheduled on non-quicksilver threads        
        // non-inherited
        // overridden by singlethreaded
        Multithreaded  = 4,

        // calls can only ever be scheduled on the main quicksilver thread
        // inherited
        // overrides multithreaded
        Singlethreaded = 8,

        // calls are schedule concurrently, directly on scheduler queues, so they can excute concurrently
        // inherited
        // overridden by serialized, replicated, and aggregated
        Concurrent = 16,

        // calls are serialized through a local context queue, so they excute atomically with respect to one-another 
        // inherited
        // overrides  concurrent
        Serialized = 32,

        // calls can be sprayed among a set of automatically spawned replicas
        // inherited
        // overridden by aggregated
        Replicated = 64,

        // calls must be executed agains a single collapsed copy
        // inherited
        // overrides replicated
        Aggregated = 128
    }
}
