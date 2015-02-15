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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.AppMulticast, "AppMulticast", "Broadcasts messages to all active listeners.")]
    public sealed class AppMulticast<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public AppMulticast(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("group", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IGroup<
            QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass>> group,
            [QS.Fx.Reflection.Parameter("debugging", QS.Fx.Reflection.ParameterClass.Value)] bool _debugging)
        {
            this._mycontext = _mycontext;
            this.inbox = new Queue<MessageClass>();
            this.outbox = new Queue<MessageClass>();

            this.appChannelEndpoint = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            this.groupEndpoint = this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass>,
                QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass>>(this);

            this.myidentifier = new QS.Fx.Base.Identifier(Guid.NewGuid());
            this.myname = new QS.Fx.Base.Name("AppMulticast");
            this.myIncarnation = new QS.Fx.Base.Incarnation(1U);

            this.members = new List<QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation,
            QS.Fx.Base.Name>>();

            this._debugging = _debugging;
            if (this._debugging)
            {
                this._form = new System.Windows.Forms.Form();
                this._form.Text = "AppMulticastTest";
                this._textbox = new System.Windows.Forms.RichTextBox();
                this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
                this._textbox.ReadOnly = true;
                this._form.Controls.Add(this._textbox);
                this._form.Show();
            }

            if (group != null)
            {
                this.groupProxy = group.Dereference(this._mycontext);
                this.groupEndpoint.OnConnected += new QS.Fx.Base.Callback(groupEndpoint_OnConnected);
                this.groupEndpoint.OnDisconnect += new QS.Fx.Base.Callback(groupEndpoint_OnDisconnect);
                this.connectionToGroup = this.groupEndpoint.Connect(this.groupProxy.Group);
            }
            else
            {
                throw new ArgumentException("Appmulticast: Group argument is null.");
            }

            //Check that this is right.
            this.appChannelEndpoint.OnConnect +=new QS.Fx.Base.Callback(appChannelEndpoint_OnConnect);
            this.appChannelEndpoint.OnConnected += new QS.Fx.Base.Callback(appChannelEndpoint_OnConnected);
            this.appChannelEndpoint.OnDisconnect += new QS.Fx.Base.Callback(appChannelEndpoint_OnDisconnect);
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        private Queue<MessageClass> inbox;
        private Queue<MessageClass> outbox;

        private QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass> groupProxy;

        [QS.Fx.Base.Inspectable("endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> appChannelEndpoint;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation,
            QS.Fx.Base.Name, MessageClass>,
            QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation,
            QS.Fx.Base.Name, MessageClass>> groupEndpoint;

        private QS.Fx.Endpoint.IConnection connectionToGroup;

        private IEnumerable<QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation,
            QS.Fx.Base.Name>> members;
        
        private QS.Fx.Base.Identifier myidentifier;
        private QS.Fx.Base.Name myname;
        private QS.Fx.Base.Incarnation myIncarnation;

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

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            lock (this.outbox)
            {
                if (this.groupEndpoint.IsConnected)
                {
                    this.printDebug("Multicasting message.");
                    multicastMessage(_message);
                }
                else
                {
                    this.printDebug("Adding message to outbox.");
                    this.outbox.Enqueue(_message);
                }
            }
        }

        #endregion

        #region IGroupClient<Identifier,Incarnation,Name,ISerializable> Members

        void QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass>.Membership(QS.Fx.Value.Classes.IMembership<QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership)
        {
            updateMembers(_membership.Members);
        }

        void QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, MessageClass>.Message(QS.Fx.Base.Identifier identifier, MessageClass message)
        {
            lock (this.inbox)
            {
                if (appChannelEndpoint.IsConnected)
                {
                    this.printDebug("Received message. Sending message to application.");
                    this.appChannelEndpoint.Interface.Receive(message);
                }
                else
                {
                    this.printDebug("Adding message to inbox.");
                    this.inbox.Enqueue(message);
                }
            }
        }

        #endregion

        private void groupEndpoint_OnConnected()
        {
            this.printDebug("Group endpoint connected.");
            this.groupEndpoint.Interface.Register(this.myidentifier, this.myIncarnation, this.myname);
            this.dumpOutbox();
        }

        private void groupEndpoint_OnDisconnect()
        {
            this.printDebug("Group endpoint disconnect.");
        }

        private void appChannelEndpoint_OnConnect()
        {
            this.printDebug("Channel endpoint connect.");
        }

        private void appChannelEndpoint_OnConnected()
        {
            this.printDebug("Channel endpoint connected.");
            this.dumpInbox();
        }

        private void appChannelEndpoint_OnDisconnect()
        {
            this.printDebug("Channel endpoint disconnect.");
            if(this.groupEndpoint.IsConnected)
            {
                this.printDebug("Unregistering from group.");
                this.groupEndpoint.Interface.Unregister(this.myidentifier, this.myIncarnation);
            }
        }

        private void updateMembers(IEnumerable<QS.Fx.Value.Classes.IMember<
            QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> members)
        {
            lock (this.members)
            {
                this.printDebug("Updating group members.");
                this.members = members.ToList<QS.Fx.Value.Classes.IMember<
                QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>();
            }
        }

        private void multicastMessage(MessageClass msg)
        {
            lock (this.members)
            {
                foreach (QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> m in this.members)
                {
                    if (this.myidentifier.CompareTo(m.Identifier) != 0)
                    {
                        this.printDebug("Attempting send to " + m.Identifier.String);
                        try
                        {
                            this.groupEndpoint.Interface.Message(m.Identifier, msg);
                        }
                        catch (Exception e)
                        {
                            this.printDebug("Could not send to " + m.Identifier.String);
                        }
                    }
                }
            }
        }

        private void dumpOutbox()
        {
            lock (this.outbox)
            {
                this.printDebug("Dumping outbox.");
                while (this.outbox.Count > 0)
                {
                    try
                    {
                        MessageClass m = outbox.Dequeue();
                        this.multicastMessage(m);
                    }
                    catch (InvalidOperationException e)
                    {
                        break;
                    }
                }
            }
        }

        private void dumpInbox()
        {
            lock (this.inbox)
            {
                this.printDebug("Dumping inbox.");
                while (this.inbox.Count > 0)
                {
                    try
                    {
                        MessageClass m = inbox.Dequeue();
                        this.appChannelEndpoint.Interface.Receive(m);
                    }
                    catch (InvalidOperationException e)
                    {
                        break;
                    }
                }
            }
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
