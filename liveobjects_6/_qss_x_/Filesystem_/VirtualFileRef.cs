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
    public sealed class VirtualFileRef : FilesystemObject, IVirtualFileRef
    {
        public VirtualFileRef(IVirtualFile file, System.IO.FileAccess access, System.IO.FileShare share, QS.Fx.Filesystem.FileFlags flags)
            : base(file.Name)
        {
            this.file = file;
            this.access = access;
            this.share = share;
            this.flags = flags;
        }

        private IVirtualFile file;
        private System.IO.FileAccess access;
        private System.IO.FileShare share;
        private QS.Fx.Filesystem.FileFlags flags;

        #region IFilesystemObject Members

        QS.Fx.Filesystem.FilesystemObjectType QS.Fx.Filesystem.IFilesystemObject.Type
        {
            get { return QS.Fx.Filesystem.FilesystemObjectType.File; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            file.DisposeRef(this);
        }

        #endregion

        #region IFile Members

        long QS.Fx.Filesystem.IFile.Length
        {
            get { return file.Length; }
        }

        IAsyncResult QS.Fx.Filesystem.IFile.BeginRead(long position, ArraySegment<byte> buffer, AsyncCallback callback, System.Object state)
        {
            if ((access & System.IO.FileAccess.Read) == System.IO.FileAccess.Read)
                return file.BeginRead(position, buffer, callback, state);
            else
                throw new Exception("Access denied, file \"" + name + "\" was not opened for reading.");
        }

        int QS.Fx.Filesystem.IFile.EndRead(IAsyncResult asyncResult)
        {
            return file.EndRead(asyncResult);
        }

        IAsyncResult QS.Fx.Filesystem.IFile.BeginWrite(long position, ArraySegment<byte> data, AsyncCallback callback, object state)
        {
            if ((access & System.IO.FileAccess.Write) == System.IO.FileAccess.Write)
                return file.BeginWrite(position, data, callback, state);
            else
                throw new Exception("Access denied, file \"" + name + "\" was not opened for writing.");
        }

        int QS.Fx.Filesystem.IFile.EndWrite(IAsyncResult asyncResult)
        {
            return file.EndWrite(asyncResult);
        }

        void QS.Fx.Filesystem.IFile.Read(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, System.Object state)
        {
            if ((access & System.IO.FileAccess.Read) == System.IO.FileAccess.Read)
                file.Read(position, buffer, callback, state);
            else
                throw new Exception("Access denied, file \"" + name + "\" was not opened for reading.");
        }

        void QS.Fx.Filesystem.IFile.Write(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, System.Object state)
        {
            if ((access & System.IO.FileAccess.Write) == System.IO.FileAccess.Write)
                file.Write(position, buffer, callback, state);
            else
                throw new Exception("Access denied, file \"" + name + "\" was not opened for writing.");
        }

        #endregion

        #region IVirtualFileRef Members

        System.IO.FileAccess IVirtualFileRef.Access
        {
            get { return access; }
        }

        System.IO.FileShare IVirtualFileRef.Share
        {
            get { return share; }
        }

        void IVirtualFileRef.Reset()
        {
            lock (this)
            {
                file = null;
            }
        }

        #endregion
    }
}
