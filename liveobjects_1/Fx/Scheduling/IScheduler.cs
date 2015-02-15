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

namespace QS.Fx.Scheduling
{
    /// <summary>
    /// This is a scheduler service that can be used to transfer control to the QuickSilver threads.
    /// </summary>
    public interface IScheduler
    {
        void Schedule(QS.Fx.Base.IEvent e);

        /// <summary>
        /// Schedule code for execution asynchronously.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <param name="context">The parameter to be passed to the callback</param>
        /// <returns>The structure that represents the request to the scheduler.</returns>
        IAsyncResult BeginExecute(AsyncCallback callback, object context);

        /// <summary>
        /// Complete the asynchronous execution (to be invoked from within the callback).
        /// </summary>
        /// <param name="result">The structure that represents the request returned by BeginExecute.</param>
        void EndExecute(IAsyncResult result);

        /// <summary>
        /// Schedule code for execution.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <param name="context">The context object to pass to the callback.</param>
        void Execute(AsyncCallback callback, object context);
    }
}
