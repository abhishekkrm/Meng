/*

Copyright (c) 2004-2009 Colin Barth. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Threading;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.AppFileBacking, "AppFileBacking", "Provides file backing of checkpoints/messages received.")]
    public sealed class AppFileBacking<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public AppFileBacking(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("filepath", QS.Fx.Reflection.ParameterClass.Value)] string filepath,
            [QS.Fx.Reflection.Parameter("timer interval", QS.Fx.Reflection.ParameterClass.Value)] int interval,
            [QS.Fx.Reflection.Parameter("debugging", QS.Fx.Reflection.ParameterClass.Value)] bool _debugging)
        {
            this._mycontext = _mycontext;
            this.oldFileExt = ".old";
            this.file = null;
            if (filepath == null || filepath.Equals(""))
            {
                throw new ArgumentException("AppFileBacking: No file path specified.");
            }
            this.filepath = filepath;
            if (interval <= 0)
            {
                throw new ArgumentException("AppFileBacking: Checkpointing interval must be greater than 0.");
            }
            this.interval = (double)interval/1000.0;
            this._debugging = _debugging;

            this.msgQueue = new Queue<MessageClass>();

            if (this._debugging)
            {
                this._form = new System.Windows.Forms.Form();
                this._form.Text = "AppFilebackingTest";
                this._textbox = new System.Windows.Forms.RichTextBox();
                this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
                this._textbox.ReadOnly = true;
                this._form.Controls.Add(this._textbox);
                this._form.Show();
            }

            this.appChannelEndpoint = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            this.appChannelEndpoint.OnConnected += new QS.Fx.Base.Callback(appChannelEndpoint_OnConnected);
            this.appChannelEndpoint.OnConnect += new QS.Fx.Base.Callback(appChannelEndpoint_OnConnect);
            this.appChannelEndpoint.OnDisconnect += new QS.Fx.Base.Callback(appChannelEndpoint_OnDisconnect);

            //this.getCheckPointTimer = new System.Timers.Timer(this.interval);
            //this.getCheckPointTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.enqueueStartNewFile);
            //this.getCheckPointTimer.AutoReset = true;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private string filepath;
        private double interval;
        private string oldFileExt;

        [QS.Fx.Base.Inspectable("endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> appChannelEndpoint;

        private QS.Fx.Filesystem.IFile file;

        private Queue<MessageClass> msgQueue;

        private int writes = 0;
        private bool startingNewFile = false;
        private Object writeCountLock = new Object();

       // private System.Timers.Timer getCheckPointTimer;

        private QS.Fx.Clock.IAlarm cpAlarm;

        private bool _debugging;
        private System.Windows.Forms.Form _form;
        private System.Windows.Forms.RichTextBox _textbox;

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this.appChannelEndpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            lock (msgQueue)
            {
                this.printDebug("Received message.");
                msgQueue.Enqueue(_message);
                if (this.appChannelEndpoint.IsConnected && this.file != null && !startingNewFile)
                enqueueWriteMessage(null);
            }
        }

        #endregion

        private void enqueueScheduleAlarm(object o)
        {
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.scheduleAlarm), null));
        }

        private void scheduleAlarm(object o)
        {
            try
            {
                this.cpAlarm = this._mycontext.Platform.AlarmClock.Schedule
                (
                    interval,
                    new QS.Fx.Clock.AlarmCallback(alarmDelegate),
                    null
                );
            }
            catch (Exception e)
            {
                printDebug("stuff."); //Do stuff.
            }
        }

        private void alarmDelegate(QS.Fx.Clock.IAlarm alarm)
        {
            if ((cpAlarm != null) && !cpAlarm.Cancelled && ReferenceEquals(cpAlarm, alarm))
                this.startNewFile(null);
        }

        private void enqueueWriteMessage(object o)
        {
            this.printDebug("Enqueuing write messages.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.writeMessages), o));
        }

        private void writeMessages(object o)
        {
            lock (msgQueue)
            {
                if (msgQueue.Count > 0)
                {
                    byte[] dataArray = msgQueue2Bytes();
                    this.printDebug("Attempting to write " + dataArray.Length + " bytes of message.");
                    {
                        try
                        {  
                            this.file.BeginWrite(file.Length, new ArraySegment<byte>(dataArray), new AsyncCallback(this.writeMessageCallBack), dataArray);
                            lock (writeCountLock)
                            {
                                writes++;
                            }
                        }
                        catch (Exception e)
                        {
                            printDebug("AppFilebacking: " + e.Message);
                        }
                    }
                }
            }
        }

        private void writeMessageCallBack(IAsyncResult r)
        {
            if (r.IsCompleted)
            {
                int numBytes = this.file.EndWrite(r);
                this.printDebug("Wrote " + numBytes + " bytes of message.");
                lock (writeCountLock)
                {
                    writes--;
                }
            }
            else
            {
                this.printDebug("Did not complete writing message.");
                lock (writeCountLock)
                {
                    writes--;
                }
                throw new Exception("AppFileBacking: Message write did not complete.");
            }
        }

        private void enqueueInitializeFileBacking(object o)
        {
//            lock (this)
//            {
                this.printDebug("Enqueuing initialization.");
                this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.initializeFileBacking), o));
//            }
        }

        private void initializeFileBacking(object o)
        {
            this.printDebug("Initializing file backing.");
            if (this.file.Length == 0)
            {
                this.printDebug("Found empty file.");
                this.writeCheckpoint(null);
                this.writeMessages(null);
            }
            else
            {
                this.printDebug("File contains data.");
                this.readFile(null);
                this.writeMessages(null);
            }
            //Setting up the timer that will handle storing a checkpoint to file after every specified interval.
            scheduleAlarm(null);
        }

        private void enqueueGetFile(object o)
        {
            this.printDebug("Enqueuing get file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.getFile), null));
        }

        private void getFile(object o)
        {
            this.printDebug("Getting file " + this.filepath);
            this.file = this._mycontext.Platform.Filesystem.Root.OpenFile(this.filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, QS.Fx.Filesystem.FileFlags.None);
        }

        private void enqueueStartNewFile(object o, System.Timers.ElapsedEventArgs args)
        {
            this.printDebug("Enqueuing start new file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(startNewFile), o));
        }

        private void enqueueStartNewFile2(object o)
        {
            this.printDebug("Enqueuing start new file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(startNewFile), o));
        }

        private void startNewFile(object o)
        {
            this.printDebug("Starting new file.");
            CheckpointClass cp = this.appChannelEndpoint.Interface.Checkpoint();
            lock (msgQueue)
            {
                startingNewFile = true;
                lock (writeCountLock)
                {
                    if (writes > 0)
                    {
                        this.cpAlarm.Reschedule();
                        return;
                    }
                }
                try
                {
                    if (file != null)
                        closeFile(null);
                    this.renameFile(o);
                    this.getFile(o);
                    this.writeCheckpoint(cp);
                    this.deleteOldFile(o);
                    msgQueue.Clear();
                }
                catch (Exception e)
                {
                    this.printDebug("Error starting new file: " + e.Message);
                }
                finally
                {
                    startingNewFile = false;
                    this.cpAlarm.Reschedule();
                }
            }
        }

        private void enqueueCloseFile(object o)
        {
            this.printDebug("Enqueue close file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(closeFile), o));
        }

        private void closeFile(object o)
        {
            this.printDebug("Closing file.");
            this.file.Dispose();
            this._mycontext.Platform.Filesystem.Root.Dispose();
            this.file = null;
        }

        private void enqueueRenameFile(object o)
        {
            this.printDebug("Enqueue rename file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.renameFile), o));
        }

        private void enqueueDeleteOldFile(object o)
        {
            this.printDebug("Enqueuing delete old file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.deleteOldFile), o));
        }

        private void renameFile(object o)
        {
            this.printDebug("Renaming file.");
            this._mycontext.Platform.Filesystem.Root.RenameFile(this.filepath, this.filepath + this.oldFileExt);
        }

        private void deleteOldFile(object o)
        {
            this.printDebug("Deleting old file.");
            this._mycontext.Platform.Filesystem.Root.DeleteFile(this.filepath + this.oldFileExt);
        }

        private void enqueueWriteCheckpoint(object o)
        {
            this.printDebug("Enqueuing write checkpoint.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.writeCheckpoint), null));
        }

        private void writeCheckpoint(object o)
        {
            this.printDebug("Writing checkpoint.");
            CheckpointClass cc = (CheckpointClass)o;
            byte[] bytes = serializable2Bytes(cc);
            this.printDebug("Attempting to write " + bytes.Length + "bytes of checkpoint.");
            this.file.BeginWrite(file.Length, new ArraySegment<byte>(bytes), new AsyncCallback(this.writeCheckpointCallback), null);
        }

        private void writeCheckpointCallback(IAsyncResult r)
        {
            if (r.IsCompleted)
            {

                int numBytes = file.EndWrite(r);
                this.printDebug("Wrote " + numBytes + " bytes of checkpoint.");
                // closeFile();
            }
            else
            {
                this.printDebug("Did not complete writing checkpoint.");
                file.EndWrite(r);
                throw new Exception("AppFileBacking: Did not complete writing checkpoint.");
            }
        }

        private void enqueueReadFile(object o)
        {
            this.printDebug("Enqueueing read file.");
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this.readFile), null));
        }

        private void readFile(object o)
        {
            this.printDebug("Reading file.");
            if (this.file.Length > 0)
            {
                this.printDebug("Attemptng to read " + this.file.Length + " bytes of file.");
                long position = 0;
                ArraySegment<byte> checkpointLength = new ArraySegment<byte>(new byte[10]);
                this.file.BeginRead(position, checkpointLength, new AsyncCallback(readFileCallback), o);
                position += checkpointLength.Count;
                Int32 checkpointHeaderLength = BitConverter.ToInt32(checkpointLength.Array, 0);
                Int32 checkpointDataLength = BitConverter.ToInt32(checkpointLength.Array, 4);
                ushort checkpointClassID = (ushort)BitConverter.ToInt16(checkpointLength.Array, 8);
                CheckpointClass cp;
                if (checkpointHeaderLength == 0 && checkpointDataLength == 0)
                {
                    cp = null;
                }
                else
                {
                    ArraySegment<byte> checkpointHeader = new ArraySegment<byte>(new byte[checkpointHeaderLength]);
                    this.file.BeginRead(position, checkpointHeader, new AsyncCallback(readFileCallback), o);
                    QS.Fx.Base.ConsumableBlock cpHeaderBlock = new QS.Fx.Base.ConsumableBlock(checkpointHeader);
                    position += checkpointHeaderLength;
                    ArraySegment<byte> checkpointData = new ArraySegment<byte>(new byte[checkpointDataLength]);
                    this.file.BeginRead(position, checkpointData, new AsyncCallback(readFileCallback), o);
                    QS.Fx.Base.ConsumableBlock cpDataBlock = new QS.Fx.Base.ConsumableBlock(checkpointData);
                    position += checkpointDataLength;
                    cp = (CheckpointClass)QS.Fx.Serialization.Serializer.Internal.CreateObject(checkpointClassID);
                    cp.DeserializeFrom(ref cpHeaderBlock, ref cpDataBlock);
                }
                this.appChannelEndpoint.Interface.Initialize(cp);
                while (position < this.file.Length)
                {
                    ArraySegment<byte> msgLength = new ArraySegment<byte>(new byte[10]);
                    this.file.BeginRead(position, msgLength, new AsyncCallback(readFileCallback), o);
                    position += msgLength.Count;
                    Int32 msgHeaderLength = BitConverter.ToInt32(msgLength.Array, 0);
                    Int32 msgDataLength = BitConverter.ToInt32(msgLength.Array, 4);
                    ushort msgClassID = (ushort)BitConverter.ToInt16(msgLength.Array, 8);
                    MessageClass msg;
                    if (msgHeaderLength == 0 && msgDataLength == 0)
                    {
                        msg = null;
                    }
                    else
                    {
                        ArraySegment<byte> msgHeader = new ArraySegment<byte>(new byte[msgHeaderLength]);
                        this.file.BeginRead(position, msgHeader, new AsyncCallback(readFileCallback), o);
                        QS.Fx.Base.ConsumableBlock msgHeaderBlock = new QS.Fx.Base.ConsumableBlock(msgHeader);
                        position += msgHeaderLength;
                        ArraySegment<byte> msgData = new ArraySegment<byte>(new byte[msgDataLength]);
                        this.file.BeginRead(position, msgData, new AsyncCallback(readFileCallback), o);
                        QS.Fx.Base.ConsumableBlock msgDataBlock = new QS.Fx.Base.ConsumableBlock(msgData);
                        position += msgDataLength;
                        try
                        {
                            msg = (MessageClass)QS.Fx.Serialization.Serializer.Internal.CreateObject(msgClassID);
                            msg.DeserializeFrom(ref msgHeaderBlock, ref msgDataBlock);
                        }
                        catch (Exception e)
                        {
                            this.printDebug("Error deserializing message: " + e.Message);
                            msg = null;
                        }
                    }
                    this.appChannelEndpoint.Interface.Receive(msg);
                }
            }
        }

        private void readFileCallback(IAsyncResult r)
        {
            if (r.IsCompleted)
            {
                int numBytes = file.EndRead(r);
                if (numBytes <= 0)
                    this.printDebug("What happened.");
                this.printDebug("Read " + numBytes + " bytes of file.");
            }
            else
            {
                this.printDebug("Did not complete reading file.");
                throw new Exception("AppFileBacking: Did not complete reading file.");
            }

        }

        private byte[] serializable2Bytes(QS.Fx.Serialization.ISerializable s)
        {
            if (s == null)
            {
                List<byte> nullBytes = new List<byte>();
                int nullHeaderLength = 0;
                int nullBodyLength = 0;
                ushort nullClassID = 0;
                //Prepend 2 bytes denoting the class id of the message object.
                nullBytes.InsertRange(0, BitConverter.GetBytes(nullClassID));
                //Prepend bytes denoting the size of the Checkpoint data in bytes.
                nullBytes.InsertRange(0, BitConverter.GetBytes(nullHeaderLength));
                //Prepend bytes denoting the size of the Checkpoint headaer in bytes.
                nullBytes.InsertRange(0, BitConverter.GetBytes(nullBodyLength));
                return nullBytes.ToArray();
            }
            QS.Fx.Base.ConsumableBlock sHeader = new QS.Fx.Base.ConsumableBlock((uint)s.SerializableInfo.HeaderSize);
            IList<QS.Fx.Base.Block> sDataList = new List<QS.Fx.Base.Block>();

            s.SerializeTo(ref sHeader, ref sDataList);
            List<byte> allSBytes = new List<byte>();
            //Getting all the checkpoint bytes into one list.
            foreach (QS.Fx.Base.Block b in sDataList)
            {
                allSBytes.AddRange(b.buffer);
            }
            Int32 totalSHeaderBytes = sHeader.Array.Length;
            Int32 totalSDataBytes = allSBytes.Count;
            allSBytes.InsertRange(0, sHeader.Array);
            //Prepend 2 bytes denoting the class id of the message object.
            allSBytes.InsertRange(0, BitConverter.GetBytes(s.SerializableInfo.ClassID));
            //Prepend bytes denoting the size of the Checkpoint data in bytes.
            allSBytes.InsertRange(0, BitConverter.GetBytes(totalSDataBytes));
            //Prepend bytes denoting the size of the Checkpoint header in bytes.
            allSBytes.InsertRange(0, BitConverter.GetBytes(totalSHeaderBytes));
            return allSBytes.ToArray();
        }

        private byte[] msgQueue2Bytes()
        {
            lock (msgQueue)
            {
                List<byte> allBytes = new List<byte>();
                while (msgQueue.Count > 0)
                {
                    MessageClass s = msgQueue.Dequeue();
                    allBytes.AddRange(serializable2Bytes(s));
                }
                return allBytes.ToArray();
            }
        }

        private CheckpointClass bytes2Checkpoint(byte[] bytes)
        {
            return null;
        }

        private MessageClass bytes2Message(byte[] bytes)
        {
            return null;
        }

        private void writeMsgQueue()
        {
        }

        private void appChannelEndpoint_OnConnect()
        {
            this.printDebug("Channel Endpoint Connect.");
        }

        private void appChannelEndpoint_OnConnected()
        {
            this.printDebug("Channel Endpoint Connected.");
            enqueueGetFile(null);
            enqueueInitializeFileBacking(null);
        }

        private void appChannelEndpoint_OnDisconnect()
        {
            this.printDebug("Channel Endpoint Disconnect.");
            this.cpAlarm.Cancel();
            //this.getCheckPointTimer.Stop();
            if (this.file != null)
                enqueueWriteMessage(null);
            this.enqueueCloseFile(null);
        }

        private void printDebug(object msg)
        {
            if (this._debugging)
            {
                lock (this)
                {
                    string smsg = (string)msg;
                    if (_form.InvokeRequired)
                        _form.BeginInvoke(new QS.Fx.Base.ContextCallback(this.printDebug), new object[] { msg });
                    else
                        this._textbox.AppendText(smsg + "\r\n");
                }
            }
        }
    }
}
