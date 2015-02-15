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
using System.IO;

namespace QS._qss_x_.Persistence_
{
/*
    [Base.SynchronizationClass(Base.SynchronizationOption.Safe)]
    public class Persisted<SerializeInterface, PersistClass, UpdateClass> : IPersisted<PersistClass, UpdateClass> 
        where PersistClass : class, SerializeInterface, new()
    {
        public Persisted(Filesystem.IFolder folder, string filename, Serialization.ISerializer<SerializeInterface> serializer)
        {
            this.serializer = serializer;
            if (folder.Exists(filename, Filesystem.FilesystemObjectType.File))
            {
                file = folder.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                byte[] bytes = new byte[file.Length];
                file.BeginRead(0, new ArraySegment<byte>(bytes), new AsyncCallback(this.ReadCallback), bytes);
            }
            else
            {
                state = new PersistClass();
                ready = true;
            }
        }

        private bool ready;
        private Filesystem.IFile file;
        private Serialization.ISerializer<SerializeInterface> serializer;
        private PersistClass state;
        private Queue<EventHandler> ready_handlers;

        #region ReadCallback

        private void ReadCallback(IAsyncResult asyncResult)
        {
            Queue<EventHandler> my_handlers;

            lock (this)
            {
                byte[] bytes = (byte[])asyncResult.AsyncState;
                file.EndRead(asyncResult);
                file.Dispose();
                file = null;

                state = (PersistClass)serializer.Deserialize(new ArraySegment<byte>(bytes));
                ready = true;

                my_handlers = ready_handlers;
                ready_handlers = null;
            }

            if (my_handlers != null)
            {
                while (my_handlers.Count > 0)
                    (my_handlers.Dequeue())(this, null);
            }
        }

        #endregion

        #region IPersisted<PersistClass,UpdateClass> Members

        PersistClass IPersisted<PersistClass, UpdateClass>.Data
        {
            get { return state; }
        }

        bool IPersisted<PersistClass, UpdateClass>.Ready
        {
            get { return ready; }
        }

        event EventHandler IPersisted<PersistClass, UpdateClass>.OnReady
        {
            add
            {
                bool call_now = ready;
                if (!call_now)
                {
                    lock (this)
                    {
                        if (ready)
                            call_now = true;
                        else
                        {
                            if (ready_handlers == null)
                                ready_handlers = new Queue<EventHandler>();
                            ready_handlers.Enqueue(value);
                        }
                    }
                }

                if (call_now)
                    value(this, null);
            }

            remove { throw new NotImplementedException(); }
        }

        void IPersisted<PersistClass, UpdateClass>.Update(UpdateClass update)
        {

            // ..................................................................................................................................................................

        }

        IAsyncResult IPersisted<PersistClass, UpdateClass>.BeginUpdate(UpdateClass update, AsyncCallback callback, object context)
        {

            // ..................................................................................................................................................................

            return null;
        }

        void IPersisted<PersistClass, UpdateClass>.EndUpdate(IAsyncResult result)
        {

            // ..................................................................................................................................................................

        }

        #endregion
    }
*/
}
