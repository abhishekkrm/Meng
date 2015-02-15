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

namespace QS.Fx.Filesystem
{
    /// <summary>
    /// This interface represents a file in a virtualized filesystem.
    /// </summary>
    public interface IFile : IFilesystemObject
    {
        /// <summary>
        /// The length of the file in bytes.
        /// </summary>
        long Length
        {
            get;
        }

        /// <summary>
        /// Read a segment of memory at the given position asynchronously.
        /// </summary>
        /// <param name="position">The position to read at.</param>
        /// <param name="buffer">The memory buffer that the data should be read into.</param>
        /// <param name="callback">The callback to invoke when the transmission completes or if it fails.</param>
        /// <param name="state">The context object to be passed to the callback.</param>
        void Read(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, System.Object state);

        /// <summary>
        /// Write a segment of memory at the given position asynchronously.
        /// </summary>
        /// <param name="position">The position to write at.</param>
        /// <param name="buffer">The memory buffer that the data should be written from.</param>
        /// <param name="callback">The callback to invoke when the transmission completes or if it fails.</param>
        /// <param name="state">The context object to be passed to the callback.</param>
        void Write(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, System.Object state);

        /// <summary>
        /// Read a segment of memory at the given position asynchronously.
        /// </summary>
        /// <param name="position">The position to read at.</param>
        /// <param name="buffer">The memory buffer that the data should be read into.</param>
        /// <param name="callback">The callback to invoke when the transmission completes or if it fails.</param>
        /// <param name="state">The context object to be passed to the callback.</param>
        /// <returns>The reference to the asynchronous read request.</returns>
        IAsyncResult BeginRead(long position, ArraySegment<byte> buffer, AsyncCallback callback, System.Object state);

        /// <summary>
        /// Complete the asynchronous read request, release resources and retrieve the result.
        /// </summary>
        /// <param name="asyncResult">The asynchronous read request.</param>
        /// <returns>The number of bytes read. An exception may be thrown if the operation failed.</returns>
        int EndRead(IAsyncResult asyncResult);

        /// <summary>
        /// Write a segment of memory at the given position asynchronously.
        /// </summary>
        /// <param name="position">The position to write at.</param>
        /// <param name="data">The memory buffer that the data should be written from.</param>
        /// <param name="callback">The callback to invoke when the transmission completes or if it fails.</param>
        /// <param name="state">The context object to be passed to the callback.</param>
        /// <returns>The reference to the asynchronous write request.</returns>
        IAsyncResult BeginWrite(long position, ArraySegment<byte> data, AsyncCallback callback, object state);

        /// <summary>
        /// Complete the asynchronous write request, release resources and retrieve the result.
        /// </summary>
        /// <param name="asyncResult">The asynchronous write request.</param>
        /// <returns>The number of bytes written. An exception may be thrown if the operation failed.</returns>
        int EndWrite(IAsyncResult asyncResult);
    }
}
