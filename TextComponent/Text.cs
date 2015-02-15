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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;

using Isis;

namespace TextComponent
{
    [QS.Fx.Reflection.ComponentClass("1`1", "Log", "A simple plain graph with a graphical user interface.")]
    public partial class Text : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        private Random random = new Random();

        delegate void stringArg(string who, string val);

        public Text(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channel)
            : base(_mycontext)
        {
            InitializeComponent();
            
            this._channelendpoint_text = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);

            if (_channel != null)
            {
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText> _channelproxy =
                    _channel.Dereference(_mycontext);

                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                            _otherendpoint =
                                _channelproxy.Channel;

                this._channelconnection_text = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint_text).Connect(_otherendpoint);
            }
            
            /***** Isis-2 *****/
            IsisSystem.Start();
            myGroup = Isis.Group.Lookup("LDO");
            if (myGroup == null)
                myGroup = new Group("LDO");
            myGroup.Handlers[UPDATE] += (stringArg)delegate(string name, string val)
            {
                lock (this)
                {
                    string _new_text = val;
                    if (!_new_text.Equals(this._text))
                    {
                        this._text = _new_text;
                        //_Refresh();
                    }

                }
            };
            
            myGroup.Join();
            _Refresh();
        }

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channelendpoint_text;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection_text;

        private string _text = string.Empty;
        private bool _editing;
        private DateTime _lastchanged;
        private System.Threading.Timer _timer;

        Group myGroup;
        int UPDATE = 1;
        TcpClient client;
        Stream s;
        StreamReader sr = null;
        string ip = "127.0.0.1";
        BindingSource bsA = new BindingSource(); 

        private const int EditingTimeoutInMilliseconds1 = 1000;
        private const int EditingTimeoutInMilliseconds2 = 100;
        private KeyPressEventArgs e;

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(
            QS.Fx.Value.Classes.IText _checkpoint)
        {
            lock (this)
            {
                string _new_text = (_checkpoint != null) ? _checkpoint.Text : null;
                if (_new_text == null)
                {
                    _new_text = string.Empty;
                    client = new TcpClient(ip, 9090);
                    s = client.GetStream();
                    sr = new StreamReader(s);
                }
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                    
                }
                //_Refresh();
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(
            QS.Fx.Value.Classes.IText _message)
        {
            /*lock (this)
            {
                string _new_text = _message.Text;
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                    _Refresh();
                }
            }*/
        }

        QS.Fx.Value.Classes.IText
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            return new QS.Fx.Value.UnicodeText(this._text);
        }

        private void _Refresh()
        {
            lock (this)
            {
                if (!_editing)
                {
                    if (InvokeRequired)
                        BeginInvoke(new QS.Fx.Base.Callback(this._Refresh));
                    else
                    {
                        
                        if (sr != null)
                        {
                            this._text = sr.ReadLine();
                            //_channelendpoint_text.Interface.Send(new QS.Fx.Value.UnicodeText(this._text));

                            /******** Isis-2 ******/
                            myGroup.Send(UPDATE, "Test", this._text);
                            /**********************/                            
                        }
                        String[] parts, values;
                        parts = this._text.Split(',');
                        bsA.Clear();
                        foreach (string point in parts)
                        {
                            values = point.Split(' ');
                            if (values.Length < 2)
                            {
                                continue;
                            }
                            if(Convert.ToDouble(values[1]) < 0.6)
                                bsA.Add(new TextLog(values[0], values[1], "Offline"));
                            else
                                bsA.Add(new TextLog(values[0], values[1], "Online"));
                        }

                        grid.DataSource = bsA;

                        foreach (DataGridViewRow row in grid.Rows)
                        {
                            if (row.Cells.Count < 3)
                            {
                                continue;
                            }
                            row.Cells[2].Style.ForeColor = Color.White;
                            //MessageBox.Show(row.Cells[2]+ "");
                            if (row.Cells[2].Value.Equals("Online"))
                                row.Cells[2].Style.BackColor = Color.Green;
                            else
                                row.Cells[2].Style.BackColor = Color.Red;

                        }


                        _textbox_KeyPress(this, e);
                    }
                }
            }
        }

        private void _textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (this)
            {
                if (_editing)
                    _lastchanged = DateTime.Now;
                else
                {
                    _editing = true;
                    _lastchanged = DateTime.Now;
                    _timer = new System.Threading.Timer(new System.Threading.TimerCallback(this._TimerCallback), null,
                        EditingTimeoutInMilliseconds1, System.Threading.Timeout.Infinite);
                }
            }
        }

        private void _TimerCallback(object o)
        {
            lock (this)
            {
                if (_editing)
                {
                    int _remaining_milliseconds =
                        EditingTimeoutInMilliseconds1 - ((int)Math.Floor((DateTime.Now - _lastchanged).TotalMilliseconds));
                    if (_remaining_milliseconds > EditingTimeoutInMilliseconds2)
                        _timer = new System.Threading.Timer(new System.Threading.TimerCallback(this._TimerCallback), null,
                            _remaining_milliseconds, System.Threading.Timeout.Infinite);
                    else
                    {
                        if (InvokeRequired)
                            BeginInvoke(new System.Threading.TimerCallback(this._TimerCallback), new object[] { null });
                        else
                        {
                            _editing = false;
                            string _text = this._text;
                            this.Log(_text);
                        }
                    }
                }
                _Refresh();
            }
        }
    }

    public class TextLog
    {
        public TextLog(String time, string value, string status)
        {
            Time = time;
            Value = value;
            Status = status;
        }
        string _time;
        public string Time { get { return _time; } set { _time = value; } }
        string _value;
        public string Value { get { return _value; } set { _value = value; } }
        string _status;
        public string Status { get { return _status; } set { _status = value; } }
    }
}
