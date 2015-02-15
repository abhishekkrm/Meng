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
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace QS.Interop
{
    public static unsafe class Win32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateIoCompletionPort(
            IntPtr handle, IntPtr existingPort, IntPtr completionKey, uint numberOfConcurrentThreads);

        [DllImport("kernel32.dll")]
        public static extern bool PostQueuedCompletionStatus(
            IntPtr completionPort, uint numberOfBytes, IntPtr completionKey, NativeOverlapped *pOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool GetQueuedCompletionStatus(
            IntPtr completionPort, out uint numberOfBytes, out IntPtr completionKey, 
            out NativeOverlapped *pOverlapped, uint milliseconds);  

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public static readonly IntPtr InvalidHandle = new IntPtr(-1);
    }
}
