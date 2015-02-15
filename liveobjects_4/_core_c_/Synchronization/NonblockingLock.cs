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

// #define RELEASE_BACKWARDS
// #define YIELD_AFTER_UNLOCK

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._core_c_.Synchronization
{
    public sealed class NonblockingLock
    {
        private int prelock;

        public static void Lock(params NonblockingLock[] locks)
        {
            int locked = 0;
            while (locked < locks.Length)
            {
                if (Interlocked.CompareExchange(ref locks[locked].prelock, 1, 0) == 1)
                {
                    while (locked > 0)
                        locks[--locked].prelock = 0;
                    Thread.Sleep(0);
                }
                else
                    locked++;
            }

            for (locked = 0; locked < locks.Length; locked++)
                Monitor.Enter(locks[locked]);

#if RELEASE_BACKWARDS
            for (locked = locks.Length - 1; locked >= 0; locked--)
                locks[locked].prelock = 0;
#else
            for (locked = 0; locked < locks.Length; locked++)
                locks[locked].prelock = 0;
#endif

        }

        public static void Unlock(params NonblockingLock[] locks)
        {
            for (int locked = 0; locked < locks.Length; locked++)
                Monitor.Exit(locks[locked]);

#if YIELD_AFTER_UNLOCK
            Thread.Sleep(0);
#endif
        }
    }
}
