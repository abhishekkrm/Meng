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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Filesystem_
{
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public sealed class VirtualFile : FilesystemObject, QS.Fx.Filesystem.IFile, IVirtualFile
    {
        public VirtualFile(string name, QS.Fx.Logging.ILogger logger, IVirtualDisk disk) : base(name)
        {
            this.logger = logger;
            this.disk = disk;
            completionCallback = new QS.Fx.Base.ContextCallback(this.CompletionCallback);
        }

        private const int blocksize = 1000;

        private ICollection<IVirtualFileRef> refs = new System.Collections.ObjectModel.Collection<IVirtualFileRef>();
        private QS.Fx.Logging.ILogger logger;
        private IVirtualDisk disk;
        private int length;
        private byte[] filedata;
        private int nreading, nwriting, ndisablereading, ndisablewriting;

        private QS.Fx.Base.ContextCallback completionCallback;

        public override string ToString()
        {
            return "File(\"" + name + "\")";
        }

        #region Class OperationType

        private enum OperationType
        {
            Read, Write
        }

        #endregion

        #region Class Operation

        private class Operation : IAsyncResult
        {
            public Operation(OperationType type, long position, ArraySegment<byte> buffer, AsyncCallback asyncCallback, 
                QS.Fx.Base.IOCompletionCallback completionCallback, object state)
            {
                this.type = type;
                this.position = position;
                this.buffer = buffer;
                this.asyncCallback = asyncCallback;
                this.completionCallback = completionCallback;
                this.state = state;
            }

            public OperationType type;
            public long position;
            public ArraySegment<byte> buffer;
            public AsyncCallback asyncCallback;
            public QS.Fx.Base.IOCompletionCallback completionCallback;
            public object state;
            public bool completed, succeeded;
            public Exception error;

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get { return state; }
            }

            System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { throw new NotSupportedException(); }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return completed; }
            }

            #endregion
        }

        #endregion

        #region IFile Members

        long QS.Fx.Filesystem.IFile.Length
        {
            get { return length; }
        }

        void QS.Fx.Filesystem.IFile.Read(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, object state)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Reading " + buffer.Count.ToString() + " bytes from \"" + name + "\" at position " + position.ToString() + "."); 
#endif

            disk.Schedule(this, (int) position, buffer.Count, completionCallback, new Operation(OperationType.Read, position, buffer, null, callback, state));
        }

        void QS.Fx.Filesystem.IFile.Write(long position, ArraySegment<byte> buffer, QS.Fx.Base.IOCompletionCallback callback, object state)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Writing " + buffer.Count.ToString() + " bytes to \"" + name + "\" at position " + position.ToString() + ".");
#endif

            disk.Schedule(this, (int) position, buffer.Count, completionCallback, new Operation(OperationType.Write, position, buffer, null, callback, state));
        }

        IAsyncResult QS.Fx.Filesystem.IFile.BeginRead(long position, ArraySegment<byte> buffer, AsyncCallback callback, object state)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Reading " + buffer.Count.ToString() + " bytes from \"" + name + "\" at position " + position.ToString() + ".");
#endif

            Operation operation = new Operation(OperationType.Read, position, buffer, callback, null, state);
            disk.Schedule(this, (int) position, buffer.Count, completionCallback, operation);
            return operation;
        }

        int QS.Fx.Filesystem.IFile.EndRead(IAsyncResult asyncResult)
        {
            Operation operation = (Operation)asyncResult;
            if (operation.succeeded)
                return operation.buffer.Count;
            else
                throw operation.error;
        }

        IAsyncResult QS.Fx.Filesystem.IFile.BeginWrite(long position, ArraySegment<byte> buffer, AsyncCallback callback, object state)
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Writing " + buffer.Count.ToString() + " bytes to \"" + name + "\" at position " + position.ToString() + ".");
#endif

            Operation operation = new Operation(OperationType.Write, position, buffer, callback, null, state);
            disk.Schedule(this, (int) position, buffer.Count, completionCallback, operation);
            return operation;
        }

        int QS.Fx.Filesystem.IFile.EndWrite(IAsyncResult asyncResult)
        {
            Operation operation = (Operation)asyncResult;
            if (operation.succeeded)
                return operation.buffer.Count;
            else
                throw operation.error;
        }

        #endregion

        #region CompletionCallback

        private void CompletionCallback(object context)
        {
            Operation operation = (Operation) context;

            lock (this)
            {
#if DEBUG_LogGenerously
                logger.Log(this, "Completed operation \"" + operation.type.ToString() + "\" of " + operation.buffer.Count.ToString() + " bytes for \"" +
                    name + "\" at position " + operation.position.ToString() + ".");
#endif

                int required_length = (int) operation.position + operation.buffer.Count;

                switch (operation.type)
                {
                    case OperationType.Read:
                        {
                            if (required_length <= length)
                            {
                                Buffer.BlockCopy(filedata, (int) operation.position, operation.buffer.Array, operation.buffer.Offset, operation.buffer.Count);
                                operation.succeeded = true;
                            }
                            else
                            {
                                operation.succeeded = false;
                                operation.error = new Exception("Attempting to read beyond the end of file \"" + name + "\".");
                            }
                        }
                        break;

                    case OperationType.Write:
                        {
                            if (required_length > length)
                            {
                                if (filedata == null)
                                {
                                    filedata = new byte[(int)Math.Ceiling(((double)required_length) / ((double)blocksize)) * blocksize];
                                    length = required_length;
                                }
                                else
                                {
                                    if (filedata.Length < required_length)
                                    {
                                        byte[] new_filedata = new byte[(int)Math.Ceiling(((double)required_length) / ((double)blocksize)) * blocksize];
                                        Buffer.BlockCopy(filedata, 0, new_filedata, 0, length);
                                        filedata = new_filedata;
                                    }

                                    length = required_length;
                                }
                            }

                            Buffer.BlockCopy(operation.buffer.Array, operation.buffer.Offset, filedata, (int) operation.position, operation.buffer.Count);
                            operation.succeeded = true;
                        }
                        break;
                }

                operation.completed = true;
            }

            if (operation.completionCallback != null)
                operation.completionCallback(operation.succeeded, (uint) (operation.succeeded ? operation.buffer.Count : 0), operation.error, operation.state);

            if (operation.asyncCallback != null)
                operation.asyncCallback(operation);
        }

        #endregion

        #region IFilesystemObject Members

        QS.Fx.Filesystem.FilesystemObjectType QS.Fx.Filesystem.IFilesystemObject.Type
        {
            get { return QS.Fx.Filesystem.FilesystemObjectType.File; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region IVirtualFile Members

        bool IVirtualFile.IsOpened
        {
            get { return refs.Count > 0; }
        }

        void IVirtualFile.Rename(string newname)
        {
            this.name = newname;
        }

        IVirtualFileRef IVirtualFile.OpenRef(System.IO.FileAccess access, System.IO.FileShare share, QS.Fx.Filesystem.FileFlags flags)
        {
            if (((share & System.IO.FileShare.Delete) == System.IO.FileShare.Delete) || ((share & System.IO.FileShare.Inheritable) == System.IO.FileShare.Inheritable))
                throw new NotImplementedException();

            bool requested_reading = ((access & System.IO.FileAccess.Read) == System.IO.FileAccess.Read);
            bool requested_writing = ((access & System.IO.FileAccess.Write) == System.IO.FileAccess.Write);

            bool requested_disable_reading = ((share & System.IO.FileShare.Read) != System.IO.FileShare.Read);
            bool requested_disable_writing = ((share & System.IO.FileShare.Write) != System.IO.FileShare.Write);

            VirtualFileRef newref;
            lock (this)
            {
                if (requested_reading && ndisablereading > 0)
                    throw new Exception("Cannot open file \"" + name + "\" for reading because it is already open, and it is not shared for reading.");

                if (requested_writing && ndisablewriting > 0)
                    throw new Exception("Cannot open file \"" + name + "\" for reading because it is already open, and it is not shared for reading.");

                if (requested_disable_reading && nreading > 0)
                    throw new Exception("Cannot open file \"" + name + "\" without sharing for reading because it is already opened for reading.");

                if (requested_disable_writing && nwriting > 0)
                    throw new Exception("Cannot open file \"" + name + "\" without sharing for reading because it is already opened for reading.");

                if (requested_reading)
                    nreading++;
                
                if (requested_writing)
                    nwriting++;

                if (requested_disable_reading)
                    ndisablereading++;

                if (requested_disable_writing)
                    ndisablewriting++;

                newref = new VirtualFileRef(this, access, share, flags);
                refs.Add(newref);
            }
            return newref;
        }

        void IVirtualFile.DisposeRef(IVirtualFileRef fileref)
        {
            bool requested_reading = ((fileref.Access & System.IO.FileAccess.Read) == System.IO.FileAccess.Read);
            bool requested_writing = ((fileref.Access & System.IO.FileAccess.Write) == System.IO.FileAccess.Write);

            bool requested_disable_reading = ((fileref.Share & System.IO.FileShare.Read) != System.IO.FileShare.Read);
            bool requested_disable_writing = ((fileref.Share & System.IO.FileShare.Write) != System.IO.FileShare.Write);

            lock (this)
            {
                refs.Remove(fileref);

                if (requested_reading)
                    nreading--;

                if (requested_writing)
                    nwriting--;

                if (requested_disable_reading)
                    ndisablereading--;

                if (requested_disable_writing)
                    ndisablewriting--;
            }
        }

        void IVirtualFile.Truncate()
        {
#if DEBUG_LogGenerously
            logger.Log(this, "Truncating file \"" + name + "\".");
#endif

            lock (this)
            {
                filedata = null;
                length = 0;
            }
        }

        void IVirtualFile.Reset()
        {
            lock (this)
            {
                foreach (IVirtualFileRef fileref in refs)
                    fileref.Reset();

                refs.Clear();

                nreading = nwriting = ndisablereading = ndisablewriting = 0;
            }
        }

        #endregion
    }
}
