/*

Copyright (c) 2004-2009 Bo Peng. All rights reserved.

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
using System.Text;
using System.Collections.Generic;

namespace Quilt.HostDetector.NATCheck
{
    /// <summary>
    /// This enum specifies NATChecker message type.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// NATChecker message is binding request.
        /// </summary>
        BindingRequest = 0x0001,

        /// <summary>
        /// NATChecker message is binding request response.
        /// </summary>
        BindingResponse = 0x0101,

        /// <summary>
        /// NATChecker message is binding requesr error response.
        /// </summary>
        BindingErrorResponse = 0x0111,

        /// <summary>
        /// NATChecker message is "shared secret" request.
        /// </summary>
        SharedSecretRequest = 0x0002,

        /// <summary>
        /// NATChecker message is "shared secret" request response.
        /// </summary>
        SharedSecretResponse = 0x0102,

        /// <summary>
        /// NATChecker message is "shared secret" request error response.
        /// </summary>
        SharedSecretErrorResponse = 0x0112,
    }
}
