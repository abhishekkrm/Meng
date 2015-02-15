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

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CCFactory, "Communication Channel Factory", "Factory to generate checkpointed communication channels that link to a AppFileBacking object.")]
    public sealed class CCFactory<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass> :
        QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>,
        QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public CCFactory(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> channel,
            [QS.Fx.Reflection.Parameter("debugging", QS.Fx.Reflection.ParameterClass.Value)] bool _debugging)
        {
            this._mycontext = _mycontext;
            this.factoryEndpoint = this._mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
            this.channelEndpoint = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>(this);

            this._debugging = _debugging;
            if (this._debugging)
            {
                this._form = new System.Windows.Forms.Form();
                this._form.Text = "CC Factory";
                this._textbox = new System.Windows.Forms.RichTextBox();
                this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
                this._textbox.ReadOnly = true;
                this._form.Controls.Add(this._textbox);
                this._form.Show();
            }

            this.channelEndpoint.OnConnect += new QS.Fx.Base.Callback(onConnectedCallback);
            if (channel == null)
                throw new ArgumentException("CCFactory: channel argument cannot be null.");
            this.channelProxy = channel.Dereference(this._mycontext);
            this.connections = new List<CCConnection>();
            this.messageQueue = new Queue<MessageClass>();
            this.messageCache = new Queue<KeyValuePair<int, MessageClass>>();
            this.lastSequenceNum = 0;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> factoryEndpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> channelEndpoint;
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass> channelProxy;
        private List<CCConnection> connections;
        private Queue<MessageClass> messageQueue;
        private Queue<KeyValuePair<int, MessageClass>> messageCache;
        private int lastSequenceNum;

        private bool _debugging;
        private System.Windows.Forms.Form _form;
        private System.Windows.Forms.RichTextBox _textbox;

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this.factoryEndpoint; }
        }

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Create()
        {
            lock (this)
            {
                CCConnection ccc = new CCConnection(this._mycontext, this, this._debugging);
                this.printDebug("Creating new proxy.");
                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                    (
                        ccc,
                        "channel",
                        new QS.Fx.Attributes.Attributes
                        (
                            new QS.Fx.Attributes.IAttribute[]
                        {
                            new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "Checkpointed Channel")
                        }
                        ),
                        QS._qss_x_.Reflection_.Library.ObjectClassOf
                        (
                            typeof(QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>)
                        )
                    );
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Receive(MessageClass _message)
        {
            lock (this)
            {
                this.lastSequenceNum++;
                this.messageCache.Enqueue(new KeyValuePair<int, MessageClass>(this.lastSequenceNum, _message));
                this.printDebug("Receiving message. Setting sequence number to " + this.lastSequenceNum);
                foreach (CCConnection ccc in connections)
                {
                    ccc.receiveMessage(this.lastSequenceNum, _message);
                }
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
        {
            lock (this)
            {
                this.printDebug("Initializing factory. Zeroing sequence number. Clearing cache.");
                this.lastSequenceNum = 0;
                this.messageCache.Clear();
                foreach (CCConnection ccc in connections)
                {
                    ccc.initializeChannel(this.lastSequenceNum, _checkpoint);
                }
            }
        }

        CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Checkpoint()
        {
            this.printDebug("Receiving checkpoint from connections.");
            if (this.channelEndpoint.IsConnected)
            {
                CheckpointClass cp;
                int sequNum;
                if (getCheckpoint(out sequNum, out cp))
                {
                    this.printDebug("Obtained checkpoint with sequence number " + sequNum);
                    return cp;
                }
                this.printDebug("Could not retrieve checkpoint.");
                throw new Exception("CCFactory: Could not retrieve checkpoint.");
            }
            else
            {
                return null;
            }
        }

        private void SendMessage(MessageClass msg)
        {
            this.printDebug("Attempting to send message down channel");
            lock (this)
            {
                if (_debugging)
                    _textbox.AppendText("Sending " + QS.Fx.Printing.Printable.ToString(msg) + "\n");
                if (this.channelEndpoint.IsConnected)
                {
                    this.printDebug("Sending message down channel");
                    this.channelEndpoint.Interface.Send(msg);
                }
                else
                {
                    this.printDebug("Channel endpoint not connected.");
                    this.messageQueue.Enqueue(msg);
                }
            }
        }

        private bool getCheckpoint(out int seqNum, out CheckpointClass cp)
        {
            lock (this)
            {
                foreach (CCConnection ccc in connections)
                {
                    if (ccc.getCheckpoint(out seqNum, out cp))
                        return true;
                }
                cp = null;
                seqNum = 0;
                return false;
            }
        }

        #endregion

        private void removeConnection(CCConnection ccc)
        {
            lock (this)
            {
                this.printDebug("Removing connection.");
                this.connections.Remove(ccc);
                if (this.connections.Count == 0)
                {
                    this.channelEndpoint.Disconnect();
                    this.lastSequenceNum = 0;
                }
            }
        }

        private void connectionConnected(CCConnection ccc)
        {
            lock (this)
            {
                this.printDebug("Connecting to connection.");
                CheckpointClass cp;
                int sequenceNum;
                if (this.getCheckpoint(out sequenceNum, out cp))
                {
                    ccc.initializeChannel(sequenceNum, cp);
                    foreach(KeyValuePair<int, MessageClass> msg in this.messageCache)
                    {
                        if (msg.Key > sequenceNum)
                        {
                            ccc.receiveMessage(msg.Key, msg.Value);
                        }
                    }
                }
                this.connections.Add(ccc);
                if (connections.Count == 1)
                {
                    this.printDebug("First proxy. Connecting to channel.");
                    this.channelEndpoint.Connect(this.channelProxy.Channel);
                }
            }
        }

        private void onConnectedCallback()
        {
            this.printDebug("Connected to channel.");
            lock (this)
            {
                this.printDebug("Sending queued messages down channel.");
                while (messageQueue.Count > 0)
                {
                    try
                    {
                        MessageClass m = messageQueue.Dequeue();
                        this.channelEndpoint.Interface.Send(m);
                    }
                    catch (InvalidOperationException e)
                    {
                        break;
                    }
                }
            }
        }

        private void printDebug(String s)
        {
            if (this._debugging)
            {
                this._textbox.AppendText(s + "\n");
            }
        }

        private sealed class CCConnection :
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        {

            public CCConnection(QS.Fx.Object.IContext _mycontext, CCFactory<MessageClass, CheckpointClass> ccf, bool _debugging)
            {
                this.mycontext = _mycontext;
                this.ccf = ccf;
                this.appChannelEndpoint = this.mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);
                this.appChannelEndpoint.OnConnect +=new QS.Fx.Base.Callback(onChannelConnect);
                this.appChannelEndpoint.OnConnected += new QS.Fx.Base.Callback(onChannelConnected);
                this.appChannelEndpoint.OnDisconnect += new QS.Fx.Base.Callback(onChannelDisconnect);

                this._debugging = _debugging;
                if (this._debugging)
                {
                    this._form = new System.Windows.Forms.Form();
                    this._form.Text = "CC Factory";
                    this._textbox = new System.Windows.Forms.RichTextBox();
                    this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
                    this._textbox.ReadOnly = true;
                    this._form.Controls.Add(this._textbox);
                    this._form.Show();
                }

                this.lastSequenceNum = 0;
                //this.appMsgQueue = new Queue<MessageClass>();
            }

            #region fields
            
            private QS.Fx.Object.IContext mycontext;
            private CCFactory<MessageClass, CheckpointClass> ccf;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> appChannelEndpoint;
            //private Queue<MessageClass> appMsgQueue;
            private int lastSequenceNum;

            private bool _debugging;
            private System.Windows.Forms.Form _form;
            private System.Windows.Forms.RichTextBox _textbox;
            
            #endregion

            #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

            QS.Fx.Endpoint.Classes.IDualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, 
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> 
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
            {
                get { return this.appChannelEndpoint; }
            }

            #endregion

            #region ICheckpointedCommunicationChannel<ISerializable,ISerializable> Members

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
            {
                this.ccf.SendMessage(_message);
            }

            #endregion

            public void receiveMessage(int sequenceNum, MessageClass m)
            {
                this.printDebug("Proxy received message with sequence number " + sequenceNum);
                if (this.appChannelEndpoint.IsConnected)
                {
                    if (sequenceNum == (this.lastSequenceNum + 1))
                    {
                        this.appChannelEndpoint.Interface.Receive(m);
                        this.lastSequenceNum = sequenceNum;
                    }
                    else
                    {
                        this.printDebug("Sequence number " + sequenceNum + " is too far ahead. Expecting " + this.lastSequenceNum + "+" + 1);
                        throw new Exception("CCConnection: Received sequence number " + sequenceNum + " but expected " + (this.lastSequenceNum + 1));
                    }
                }
               /* else
                {
                    this.appMsgQueue.Enqueue(m);
                }*/
            }

            public void initializeChannel(int sequenceNum, CheckpointClass c)
            {
                this.printDebug("Initializing CCChannel with checkpoint at sequence number " + sequenceNum);
                this.appChannelEndpoint.Interface.Initialize(c);
                this.lastSequenceNum = sequenceNum;
            }

            public bool getCheckpoint(out int sequenceNum, out CheckpointClass cp)
            {
                this.printDebug("Attempting to get checkpoint.");
                if (this.appChannelEndpoint.IsConnected)
                {
                    cp = this.appChannelEndpoint.Interface.Checkpoint();
                    sequenceNum = this.lastSequenceNum;
                    this.printDebug("Got checkpoint with sequence number " + sequenceNum);
                    return true;
                }
                this.printDebug("Could not get checkpoint.");
                cp = null;
                sequenceNum = 0;
                return false;
            }


            private void onChannelConnect()
            {
                this.printDebug("Proxy connecting.");
            }

            private void onChannelConnected()
            {
                this.printDebug("Proxy connected.");
                this.ccf.connectionConnected(this);
            }

            private void onChannelDisconnect()
            {
                this.printDebug("Proxy disconnect.");
                this.ccf.removeConnection(this);
            }

            private void printDebug(String s)
            {
                if (this._debugging)
                {
                    this._textbox.AppendText(s + "\n");
                }
            }
        }
    }
}
