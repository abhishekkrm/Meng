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

namespace QS._qss_x_.Filesystem_
{
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public sealed class PhysicalFile : FilesystemObject, QS.Fx.Filesystem.IFile
    {
        public PhysicalFile(QS._core_c_.Core.IFile physical_file, string physical_path, string name) : base(name)
        {
            this.physical_path = physical_path;
            this.physical_file = physical_file;
        }

        private string physical_path;
        private QS._core_c_.Core.IFile physical_file;

        #region IFilesystemObject Members

        QS.Fx.Filesystem.FilesystemObjectType QS.Fx.Filesystem.IFilesystemObject.Type
        {
            get { return QS.Fx.Filesystem.FilesystemObjectType.File; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            physical_file.Dispose();
        }

        #endregion

        #region IFile Members

        long QS.Fx.Filesystem.IFile.Length
        {
            get { return physical_file.Length; }
        }

        IAsyncResult QS.Fx.Filesystem.IFile.BeginRead(long position, ArraySegment<byte> buffer, AsyncCallback callback, System.Object state)
        {
            return physical_file.BeginRead(position, buffer, callback, state);
        }

        int QS.Fx.Filesystem.IFile.EndRead(IAsyncResult asyncResult)
        {
            return physical_file.EndRead(asyncResult);
        }

        IAsyncResult QS.Fx.Filesystem.IFile.BeginWrite(long position, ArraySegment<byte> data, AsyncCallback callback, object state)
        {
            return physical_file.BeginWrite(position, data, callback, state);
        }

        int QS.Fx.Filesystem.IFile.EndWrite(IAsyncResult asyncResult)
        {
            return physical_file.EndWrite(asyncResult);
        }

        void QS.Fx.Filesystem.IFile.Read(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, System.Object state)
        {
            physical_file.Read(position, buffer, callback, state);
        }

        void QS.Fx.Filesystem.IFile.Write(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, System.Object state)
        {
            physical_file.Write(position, buffer, callback, state);
        }

        #endregion
    }
}
