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

#define DEBUG_EnableLogging

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace QS._qss_x_.Persistence_
{
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public class Persistent<SerializationInterface, QueryInterface, UnderlyingClass, OperationClass> 
        : IPersistent<QueryInterface, UnderlyingClass, OperationClass>
        where UnderlyingClass : QueryInterface, SerializationInterface, new()
        where OperationClass : IOperation<UnderlyingClass>, SerializationInterface, new()
    {
        public Persistent(QS.Fx.Filesystem.IFolder folder, QS.Fx.Clock.IAlarmClock alarmClock, 
            QS._core_x_.Serialization.ISerializer<SerializationInterface> serializer, QS.Fx.Logging.ILogger logger)
        {
            this.folder = folder;
            this.serializer = serializer;
            this.logger = logger;
            this.alarmClock = alarmClock;

            batchingAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.BatchingAlarmCallback);
            writingCallback = new AsyncCallback(this.WritingCallback);

            _LoadPersistentState();
        }

        private const double BatchingTimeout = 0.01;
        private double batchingTimeout = BatchingTimeout;

        private bool ready, batching, writing;
        private IList<QS.Fx.Base.Callback> readyCallbacks = new List<QS.Fx.Base.Callback>();
        private IList<QS.Fx.Base.ContextCallback<OperationClass>> changeCallbacks = new List<QS.Fx.Base.ContextCallback<OperationClass>>();
        private UnderlyingClass state;
        private QS._core_x_.Serialization.ISerializer<SerializationInterface> serializer;
        private QS.Fx.Filesystem.IFolder folder;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Filesystem.IFile oldfile, newfile;
        private int newnum;
        private string oldname, tempname, newname;
        private byte[] olddata;
        private int writecount, writeoffset;
        private Queue<OperationContext> pending = new Queue<OperationContext>();
        private Queue<OperationContext> inwriting = new Queue<OperationContext>();
        // private Queue<OperationContext> completed = new Queue<OperationContext>();
        private QS.Fx.Clock.IAlarm batchingAlarm;
        private QS.Fx.Clock.AlarmCallback batchingAlarmCallback;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private AsyncCallback writingCallback;

        private const string CommonFileNamePrefix = "Log_";
        private const string DataFileNameExtension = ".dat";
        private const string TempFileNameExtension = ".tmp";

        #region _LoadPersistentState

        private void _LoadPersistentState()
        {
            string lastname = null;
            int lastnum = 0;
            foreach (string filename in folder.Files)
            {
                int num = -1;
                if (filename.StartsWith(CommonFileNamePrefix))
                {
                    string mid;
                    bool good = false;

                    if (filename.EndsWith(DataFileNameExtension))
                    {
                        mid = filename.Substring(CommonFileNamePrefix.Length,
                            filename.Length - CommonFileNamePrefix.Length - DataFileNameExtension.Length);
                        good = true;
                    }
                    else if (filename.EndsWith(TempFileNameExtension))
                    {
                        mid = filename.Substring(CommonFileNamePrefix.Length,
                            filename.Length - CommonFileNamePrefix.Length - TempFileNameExtension.Length);
                    }
                    else
                        mid = null;

                    if (mid != null)
                    {
                        try
                        {
                            num = Convert.ToInt32(mid);

                            if (good && (lastname == null || num > lastnum))
                            {
                                lastname = filename;
                                lastnum = num;
                            }
                            else
                                num = -1;
                        }
                        catch (Exception)
                        {
                            num = -1;
                        }
                    }
                }

                if (num < 0)
                {
#if DEBUG_EnableLogging
                    logger.Log(this, "Deleting file \"" + filename + "\".");
#endif

                    folder.DeleteFile(filename);
                }
            }

            newnum = lastnum + 1;
            tempname = CommonFileNamePrefix + newnum.ToString("000000000000") + TempFileNameExtension;
            newname = CommonFileNamePrefix + newnum.ToString("000000000000") + DataFileNameExtension;

            if (lastname != null)
            {
#if DEBUG_EnableLogging
                logger.Log(this, "Loading state from \"" + lastname + "\".");
#endif

                oldname = lastname;

                oldfile = folder.OpenFile(oldname, FileMode.Open, FileAccess.Read, FileShare.Read, QS.Fx.Filesystem.FileFlags.None);
                olddata = new byte[oldfile.Length];
                oldfile.BeginRead(0, new ArraySegment<byte>(olddata), new AsyncCallback(this.LoadingCallback), null);
            }
            else
            {
#if DEBUG_EnableLogging
                logger.Log(this, "Starting with empty state.");
#endif

                state = new UnderlyingClass();

                lock (this)
                {
                    _SaveState();
                }
            }
        }

        #endregion

        #region Class OperationContext

        private class OperationContext
        {
            public OperationContext(OperationClass operation, QS.Fx.Base.ContextCallback<OperationClass> callback)
            {
                this.operation = operation;
                this.callback = callback;
            }

            public OperationClass operation;
            public QS.Fx.Base.ContextCallback<OperationClass> callback;
        }

        #endregion

        #region LoadingCallback

        private void LoadingCallback(IAsyncResult result)
        {
            lock (this)
            {
                int nread;
                try
                {
                    nread = oldfile.EndRead(result);

#if DEBUG_EnableLogging
                    logger.Log(this, "Successfully loaded " + nread.ToString() + " bytes from old file \"" + oldfile.Name + "\".");
#endif
                }
                catch (Exception exc)
                {
#if DEBUG_EnableLogging
                    logger.Log(this, "Could not load file \"" + oldfile.Name + "\".\n" + exc.ToString());
#endif

                    nread = 0;
                }

                oldfile.Dispose();
                oldfile = null;

                if (nread > 0)
                {
                    int nconsumed;
                    try
                    {
                        state = serializer.Deserialize<UnderlyingClass>(new ArraySegment<byte>(olddata), out nconsumed);
                    }
                    catch (Exception exc)
                    {
#if DEBUG_EnableLogging
                        logger.Log(this, "Could not deserialize the loaded old state.\n" + exc.ToString());
#endif

                        // TODO: Handle this somehow..........................
                        throw;
                    }

                    int offset = nconsumed;
                    while (offset < olddata.Length)
                    {
                        try
                        {
                            OperationClass operation = serializer.Deserialize<OperationClass>(
                                new ArraySegment<byte>(olddata, offset, olddata.Length - offset), out nconsumed);
                            offset += nconsumed;

                            operation.Execute(state);
                        }
                        catch (Exception exc)
                        {
#if DEBUG_EnableLogging
                            logger.Log(this, "Could not load operation.\n" + exc.ToString());
#endif
                        }
                    }
                }

                olddata = null;

#if DEBUG_EnableLogging
                logger.Log(this, "Loading complete");
#endif

                _SaveState();
            }
        }

        #endregion

        #region _SaveState

        private void _SaveState()
        {
#if DEBUG_EnableLogging
            logger.Log(this, "Saving state to \"" + newname + "\".");
#endif

            newfile = folder.OpenFile(tempname, FileMode.CreateNew, FileAccess.Write, FileShare.None, QS.Fx.Filesystem.FileFlags.WriteThrough);

            IEnumerable<QS.Fx.Base.Block> flattened_state = serializer.Serialize<UnderlyingClass>(state);
            int totalbytes = 0;
            foreach (QS.Fx.Base.Block segment in flattened_state)
                totalbytes += (int) segment.size;
            byte[] flattened_bytes = new byte[totalbytes];
            int bytesflattened = 0;
            foreach (QS.Fx.Base.Block segment in flattened_state)
            {
                if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                    Buffer.BlockCopy(segment.buffer, (int) segment.offset, flattened_bytes, bytesflattened, (int) segment.size);
                else
                    throw new Exception("Unmanaged memory not supported here.");

                bytesflattened += (int) segment.size;
            }

            newfile.BeginWrite(writeoffset, new ArraySegment<byte>(flattened_bytes), new AsyncCallback(this.SavingCallback), null);
            writeoffset += totalbytes;
            writecount = 1;
        }

        #endregion

        #region SavingCallback

        private void SavingCallback(IAsyncResult result)
        {
            IList<QS.Fx.Base.Callback> callbacks = null;

            lock (this)
            {
                try
                {
                    newfile.EndWrite(result);
                }
                catch (Exception exc)
                {
#if DEBUG_EnableLogging
                    logger.Log(this, "Could not write to \"" + tempname + "\".\n" + exc.ToString());
#endif

                    // TODO: Handle this failure somehow............
                }

                writecount--;

                if (writecount == 0)
                {
                    newfile.Dispose();
                    folder.RenameFile(tempname, newname);
                    if (oldname != null)
                    {
#if DEBUG_EnableLogging
                        logger.Log(this, "Deleting file \"" + oldname + "\".");
#endif

                        folder.DeleteFile(oldname);
                    }
                    tempname = oldname = null;

                    newfile = folder.OpenFile(newname, FileMode.Open, FileAccess.Write, FileShare.Read, QS.Fx.Filesystem.FileFlags.WriteThrough);

#if DEBUG_EnableLogging
                    logger.Log(this, "State successfully saved to \"" + newname + "\".");
#endif

                    ready = true;
                    if (pending.Count > 0 && !batching)
                    {
                        batching = true;
                        batchingAlarm = alarmClock.Schedule(batchingTimeout, batchingAlarmCallback, null);
                    }

                    callbacks = readyCallbacks;
                    readyCallbacks = null;
                }
            }

            if (callbacks != null)
            {
                foreach (QS.Fx.Base.Callback callback in callbacks)
                    callback();
            }
        }

        #endregion

        #region WritingCallback

        private void WritingCallback(IAsyncResult result)
        {
            lock (this)
            {
                try
                {
                    newfile.EndWrite(result);
                }
                catch (Exception exc)
                {
#if DEBUG_EnableLogging
                    logger.Log(this, "Could not write to \"" + newname + "\".\n" + exc.ToString());
#endif

                    // TODO: Handle this failure somehow............
                }

                writing = false;

                while (inwriting.Count > 0)
                {
                    OperationContext context = inwriting.Dequeue();
                    context.operation.Execute(state);

                    if (context.callback != null)
                        context.callback(context.operation);

                    foreach (QS.Fx.Base.ContextCallback<OperationClass> callback in changeCallbacks)
                        callback(context.operation);
                }
            }
        }

        #endregion

        #region BatchingAlarmCallback

        private void BatchingAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (pending.Count > 0)
                {
                    if (writing || !ready)
                    {
                        alarm.Reschedule();
                        logger.Log(this, "Internal error: batchingAlarmCallback has fired with messages pending while reading or writing.");
                    }
                    else
                    {
                        writing = true;
                        batching = false;

                        if (inwriting.Count > 0)
                        {
                            // TODO: Handle this........
                            logger.Log(this, "Internal error: not writing, but requests are on the queye; dropping the offending requests");
                            inwriting.Clear();
                        }

                        Queue<OperationContext> temp = inwriting;
                        inwriting = pending;
                        pending = temp;

                        int totalnumbytes = 0;
                        List<QS.Fx.Base.Block> segments = new List<QS.Fx.Base.Block>();
                        foreach (OperationContext context in inwriting)
                        {
                            lock (context)
                            {
                                foreach (QS.Fx.Base.Block segment in serializer.Serialize<OperationClass>(context.operation))
                                {
                                    totalnumbytes += (int) segment.size;
                                    segments.Add(segment);
                                }
                            }
                        }

                        byte[] flattenedbytes = new byte[totalnumbytes];
                        int flatteningoffset = 0;
                        foreach (QS.Fx.Base.Block segment in segments)
                        {
                            if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                                Buffer.BlockCopy(segment.buffer, (int) segment.offset, flattenedbytes, flatteningoffset, (int) segment.size);
                            else
                                throw new Exception("Unmanaged memory nto supported here.");

                            flatteningoffset += (int) segment.size;
                        }

                        newfile.BeginWrite(writeoffset, new ArraySegment<byte>(flattenedbytes), writingCallback, null);
                        writeoffset += totalnumbytes;
                    }
                }
                else
                {
                    batching = false;
                    logger.Log(this, "Internal error: batchingAlarmCallback fired, but there are no pending messages.");
                }
            }
        }

        #endregion

        #region IPersistent<QueryInterface, UnderlyingClass, OperationClass> Members

        QueryInterface IPersistent<QueryInterface, UnderlyingClass, OperationClass>.State
        {
            get { return state; }
        }

        void IPersistent<QueryInterface, UnderlyingClass, OperationClass>.Submit(
            OperationClass operation, QS.Fx.Base.ContextCallback<OperationClass> callback)
        {
            OperationContext context = new OperationContext(operation, callback);
            lock (this)
            {
                pending.Enqueue(context);

                if (ready && !batching)
                {
                    batching = true;
                    batchingAlarm = alarmClock.Schedule(batchingTimeout, batchingAlarmCallback, null);
                }
            }
        }

        bool IPersistent<QueryInterface, UnderlyingClass, OperationClass>.Ready
        {
            get { return ready; }
        }

        event QS.Fx.Base.Callback IPersistent<QueryInterface, UnderlyingClass, OperationClass>.OnReady
        {
            add
            {
                bool callnow = false;
                lock (this)
                {
                    if (ready)
                        callnow = true;
                    else
                    {
                        if (readyCallbacks.Contains(value))
                            throw new Exception("Already registered.");
                        else
                            readyCallbacks.Add(value);
                    }
                }
                if (callnow)
                    value();
            }

            remove
            {
                lock (this)
                {
                    if (!readyCallbacks.Remove(value))
                        throw new Exception("Not registered.");
                }
            }
        }

        event QS.Fx.Base.ContextCallback<OperationClass> IPersistent<QueryInterface, UnderlyingClass, OperationClass>.OnChange
        {
            add
            { 
                lock (this)
                {
                    if (changeCallbacks.Contains(value))
                        throw new Exception("Already registered.");
                    else
                        changeCallbacks.Add(value);
                }
            }

            remove 
            {
                lock (this)
                {
                    if (!changeCallbacks.Remove(value))
                        throw new Exception("Not registered.");
                }
            }
        }

        #endregion
    }
}
