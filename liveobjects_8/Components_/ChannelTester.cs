/*

Copyright (c) 2009 Colin Barth. All rights reserved.

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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.ChannelTester, "Channel Tester", "A component used to test channel message ordering.")]
    public sealed partial class ChannelTester : UserControl, QS.Fx.Object.Classes.IUI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        public ChannelTester(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel
            <QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> _channel,
            [QS.Fx.Reflection.Parameter("id", QS.Fx.Reflection.ParameterClass.Value)] string id,
            [QS.Fx.Reflection.Parameter("connect delay", QS.Fx.Reflection.ParameterClass.Value)] int delay,
            [QS.Fx.Reflection.Parameter("message delay", QS.Fx.Reflection.ParameterClass.Value)] int interval,
            [QS.Fx.Reflection.Parameter("message count", QS.Fx.Reflection.ParameterClass.Value)] int count,
            [QS.Fx.Reflection.Parameter("disconnect interval", QS.Fx.Reflection.ParameterClass.Value)] int dInterval,
            [QS.Fx.Reflection.Parameter("disconnect time", QS.Fx.Reflection.ParameterClass.Value)] int dTime
            )
        {
            this.mycontext = _mycontext;

            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);

            this.uiendpoint = _mycontext.ExportedUI(this);

            if (id != null)
                this.id = id;
            else
                this.id = "NULL";
            msgCount = count;
            msgsSent = 0;
            if (interval > 0)
                msgInterval = interval;
            else
                msgInterval = 1;
            if (dInterval > 0)
            {
                disconnecting = true;
                disInterval = dInterval;
                this.dTime = dTime;
            }
            msgDelay = delay;
            this.lastReceived = "";
            this.lastID = 0;
            this.disCount = 0;
            InitializeComponent();

            if (_channel != null)
            {
                _channelproxy = _channel.Dereference(_mycontext);

                this._channelendpoint.OnConnected += new QS.Fx.Base.Callback(_channelendpoint_OnConnected);

                this._channelconnection = this._channelendpoint.Connect(_channelproxy.Channel);

                /*this.mycontext.Platform.AlarmClock.Schedule(delay, new QS.Fx.Clock.AlarmCallback(
                    delegate(QS.Fx.Clock.IAlarm alarm)
                    {
                        this.mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(performTest)));
                    }),
                    null);
                */
            }
            else
            {
                throw new Exception("ChannelTester: The channel reference is null.");
            }
        }

        #region Fields

        private string id;
        private int msgCount;
        private int msgInterval;
        private int msgDelay;
        private int msgsSent;
        private int disInterval;
        private bool disconnecting;
        private int dTime;
        private int disCount;

        private QS.Fx.Object.IContext mycontext;

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channelendpoint;

        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText> _channelproxy;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedUI uiendpoint;

        private string lastReceived;
        private int lastID;

        #endregion

        #region IUI Members

        public QS.Fx.Endpoint.Classes.IExportedUI UI
        {
            get { return uiendpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<QS.Fx.Channel.Message.UnicodeText, QS.Fx.Channel.Message.UnicodeText> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(
            QS.Fx.Value.Classes.IText _checkpoint)
        {
            lock (this)
            {
                updateInCheckpointText(_checkpoint.Text);
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(
            QS.Fx.Value.Classes.IText _message)
        {
            lock (this)
            {
                string text = _message.Text;
                this.lastReceived = text;
                int id = extractID(text);
                int expectedId = lastID + 1;
                if (id != expectedId)
                {
                    updateErrorText("ERROR: expecting message ID of " + expectedId + " but received " + id);
                }
                this.lastID = id;
                updateIncomingText(text);
            }
        }

        QS.Fx.Value.Classes.IText
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            lock (this)
            {
                string cp = "Last Received: " + lastReceived + " Last Sent: " + id + "." + msgsSent.ToString();
                updateOutCheckpointText(cp);
                return new QS.Fx.Value.UnicodeText(cp);
            }
        }

        #endregion

        private int extractID(string msg)
        {
            string sid = msg.Split(new char[] { ':' })[1];
            try
            {
                int id = Int32.Parse(sid);
                return id;
            }
            catch (Exception e)
            {
                updateErrorText("Error parsing message id: " + e.Message);
                return -1;
            }
        }

        private void sendMessage(string msg)
        {
            lock(this)
            {
                this._channelendpoint.Interface.Send(new QS.Fx.Value.UnicodeText(msg));
                updateOutgoingText(msg);
            }
        }

        private void updateOutgoingText(object msg)
        {
            
            lock(this)
            {
                string smsg = (string)msg;
                if (InvokeRequired)
                    BeginInvoke(new QS.Fx.Base.ContextCallback(this.updateOutgoingText), new object[]{msg});
                else
                    this.outMsg.Text += smsg + "\r\n";
            }
        }

        private void updateIncomingText(object msg)
        {         
            lock (this)
            {
                string smsg = (string)msg;
                if (InvokeRequired)
                    BeginInvoke(new QS.Fx.Base.ContextCallback(this.updateIncomingText), new object[] { msg });
                else
                    this.inMsg.Text += smsg + "\r\n";
            }
        }

        private void updateOutCheckpointText(object msg)
        {
            lock (this)
            {
                string smsg = (string)msg;
                if (InvokeRequired)
                    BeginInvoke(new QS.Fx.Base.ContextCallback(this.updateOutCheckpointText), new object[] { msg });
                else
                    this.outCP.Text += smsg + "\r\n";
            }
        }

        private void updateInCheckpointText(object msg)
        {
            lock (this)
            {
                string smsg = (string)msg;
                if (InvokeRequired)
                    BeginInvoke(new QS.Fx.Base.ContextCallback(this.updateInCheckpointText), new object[] { msg });
                else
                    this.inCP.Text += smsg + "\r\n";
            }
        }

        private void updateErrorText(object msg)
        {
            lock (this)
            {
                string smsg = (string)msg;
                if (InvokeRequired)
                    BeginInvoke(new QS.Fx.Base.ContextCallback(this.updateErrorText), new object[] { msg });
                else
                    this.errors.Text += smsg + "\r\n";
            }
        }

        private void performTest(object o, System.Timers.ElapsedEventArgs args)
        {
            lock (this)
            {
                if (!this._channelendpoint.IsConnected)
                {
                    this._channelconnection = this._channelendpoint.Connect(_channelproxy.Channel);
                    //System.Timers.Timer timer = new System.Timers.Timer(this.msgInterval);
                    //timer.Elapsed += new System.Timers.ElapsedEventHandler(this.performTest);
                    //timer.AutoReset = false;
                    //timer.Start();
                    return;
                }
                if (disconnecting)
                {
                    if (disCount == disInterval)
                    {
                        disCount = 0;
                        this._channelendpoint.Disconnect();
                        System.Timers.Timer timer = new System.Timers.Timer(this.dTime);
                        timer.Elapsed += new System.Timers.ElapsedEventHandler(this.performTest);
                        timer.AutoReset = false;
                        timer.Start();
                        return;
                    }
                    disCount++;
                }
                int msgID = msgsSent + 1;
                string msg = this.id + ": " + msgID.ToString();
                sendMessage(msg);
                msgsSent++;
                if (msgsSent < msgCount)
                {
                    /*this.mycontext.Platform.AlarmClock.Schedule(msgInterval, new QS.Fx.Clock.AlarmCallback(
                        delegate(QS.Fx.Clock.IAlarm alarm)
                        {
                            this.mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(performTest)));
                        }),
                        null);
                     */
                    if (this.msgInterval > 0)
                    {
                        System.Timers.Timer timer = new System.Timers.Timer(this.msgInterval);
                        timer.Elapsed += new System.Timers.ElapsedEventHandler(this.performTest);
                        timer.AutoReset = false;
                        timer.Start();
                    }
                    else
                    {
                        this.performTest(null, null);
                    }
                }
            }
        }

        void _channelendpoint_OnConnected()
        {
            lock (this)
            {
                if (this.msgCount > 0)
                {
                    if (this.msgDelay > 0)
                    {
                        System.Timers.Timer timer = new System.Timers.Timer(this.msgDelay);
                        timer.Elapsed += new System.Timers.ElapsedEventHandler(this.performTest);
                        timer.AutoReset = false;
                        timer.Start();
                    }
                    else
                    {
                        this.performTest(null, null);
                    }
                }
            }
        }

        private void ChannelTesterUI_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
