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
using System.Text;
using System.Windows.Forms;
using Isis;
using System.IO;
using System.Net.Sockets;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Text, "Text", "A simple plain text note with a graphical user interface.")]
    public sealed partial class Text
        : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        #region Constructor

        int UPDATE = 1;
        delegate void stringArg(string who, string val);

        void receive_data(string who, string val)
        {
            //lock (this)
            {
                string _new_text = val;
                //System.Windows.Forms.MessageBox.Show("Received: " + _new_text);
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                    _Refresh();
                }
            }
        }

        Group myGroup;

        public Text(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> 
                _channel) 
            : base(_mycontext)
        {
            //System.Windows.Forms.MessageBox.Show("From text constructor!");

            /***** Isis-2 *****/
            //IsisSystem.Start();
            myGroup = Isis.Group.Lookup("LDO");
            if(myGroup == null)
                myGroup = new Group("LDO");
            myGroup.Handlers[UPDATE] += (stringArg)delegate(string name, string val)
            {
                lock (this)
                {
                   string _new_text = val;
                   //System.Windows.Forms.MessageBox.Show("Received: " + _new_text);
                   if (!_new_text.Equals(this._text))
                   {
                        this._text = _new_text;
                        _Refresh();
                   }
                }
            };
            myGroup.Join();
            //MessageBox.Show(IsisSystem.GetMyAddress().ToStringVerboseFormat());
            /*************/

            InitializeComponent();

            this._channelendpoint = _mycontext.DualInterface<
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

                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint) this._channelendpoint).Connect(_otherendpoint);
                
            }
            _Refresh();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> 
                _channelendpoint;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        private string _text = string.Empty;
        private bool _editing;
        private DateTime _lastchanged;
        private System.Threading.Timer _timer, _dragtimer;

        #endregion

        #region Constants

        private const int EditingTimeoutInMilliseconds1 = 1000;
        private const int EditingTimeoutInMilliseconds2 = 100;
        private KeyPressEventArgs e;

        #endregion

        #region ICheckpointedCommunicationChannelClient<QS.Fx.Channel.Message.UnicodeText, QS.Fx.Channel.Message.UnicodeText> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(
            QS.Fx.Value.Classes.IText _checkpoint)
        {
            lock (this)
            {
                string _new_text = (_checkpoint != null) ? _checkpoint.Text : null;
                //System.Windows.Forms.MessageBox.Show("Initialzed: " + _new_text);
                if (_new_text == null)
                    _new_text = string.Empty;
                if (!_new_text.Equals(this._text))
                {
                    this._text = _new_text;
                    _Refresh();
                }
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(
            QS.Fx.Value.Classes.IText _message)
        {
            /*lock (this)
            {
                string _new_text = _message.Text;
                System.Windows.Forms.MessageBox.Show("Received: " + _new_text);
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

        #endregion

        #region _Refresh

        private void _Refresh()
        {
            lock (this)
            {
                //System.Windows.Forms.MessageBox.Show("Refresh");
                if (!_editing)
                {
                    if (InvokeRequired)
                        BeginInvoke(new QS.Fx.Base.Callback(this._Refresh));
                    else
                    {
                        //start here
                        String line;
                        String[] parts;
                        String[] pparts;

                        // Simulate adding new data points
                        //int numberOfPointsAddedMax = 5;
                        int x, y;

                        TcpClient client = new TcpClient("127.0.0.1", 9090);
                        Stream s = client.GetStream();
                        StreamReader sr = new StreamReader(s);
                        line = sr.ReadLine();
                        sr.Close();

                        parts = line.Split(',');

                        foreach (String point in parts)
                        {
                            this._textbox.Text = point + "\n" + this._textbox.Text;
                            _textbox_KeyPress(this, e);
                        }
                    }
                }
            }
        }

        #endregion

        #region _DragEnter

        private void _DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion

        #region _DragDrop

        private void _DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
                {
                    string _text = (string) e.Data.GetData(DataFormats.UnicodeText);
                    lock (this)
                    {
                        _channelendpoint.Interface.Send(new QS.Fx.Value.UnicodeText(_text));
                        
                    }
                }
                else
                    throw new Exception("The drag and drop operation cannot continue because none of the data formats was recognized.");
            }
            catch (Exception _exc)
            {
                (new QS._qss_x_.Base1_.ExceptionForm(_exc)).ShowDialog();
            }
        }

        #endregion

        #region _textbox_KeyPress

        private void _textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            lock (this)
            {
                if (_editing)
                    _lastchanged = DateTime.Now;
                else
                {
                    _editing = true;
                    this._RefreshReadOnly();
                    _lastchanged = DateTime.Now;
                    _timer = new System.Threading.Timer(new System.Threading.TimerCallback(this._TimerCallback), null,
                        EditingTimeoutInMilliseconds1, System.Threading.Timeout.Infinite);
                }
            }
        }
        
        #endregion

        #region _TimerCallback

        private void _TimerCallback(object o)
        {
            lock (this)
            {
                if (_editing)
                {
                    int _remaining_milliseconds = 
                        EditingTimeoutInMilliseconds1 - ((int) Math.Floor((DateTime.Now - _lastchanged).TotalMilliseconds));
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
                            this._RefreshReadOnly();
                            string _text = _textbox.Text;
                            //_channelendpoint.Interface.Send(new QS.Fx.Value.UnicodeText(_text));
                            //System.Windows.Forms.MessageBox.Show("Sending:" + _text);
                            /******** Isis-2 ******/
                            myGroup.Send(UPDATE, "Test", _text);
                            /**********************/
                            this.Log(_text);
                        }
                    }
                }
            }
        }

        #endregion

        #region _RefreshBorder

        private void _RefreshReadOnly()
        {
            if (InvokeRequired)
                BeginInvoke(new QS.Fx.Base.Callback(this._RefreshReadOnly));
            else
            {
                lock (this)
                {
                    this.BorderStyle = this._editing ? BorderStyle.Fixed3D : BorderStyle.None;
                    _textbox.ReadOnly = !this._editing;
                }
            }
        }

        #endregion

        #region _textbox_MouseDown

        private void _textbox_MouseDown(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (Control.MouseButtons == MouseButtons.Right)
                {
                    DataObject _dataobject = new DataObject();
                    _dataobject.SetData(DataFormats.UnicodeText, this._text);
                    this.DoDragDrop(_dataobject, DragDropEffects.Copy);
                }
/*
                _dragtimer = new System.Threading.Timer(
                    new System.Threading.TimerCallback(this._DragTimerCallback), null, 500, System.Threading.Timeout.Infinite);
*/ 
            }
        }

        #endregion

        #region _textbox_MouseUp

        private void _textbox_MouseUp(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (_dragtimer != null)
                    _dragtimer.Dispose();
                _dragtimer = null;
            }
        }

        #endregion

        #region _DragTimerCallback

        private void _DragTimerCallback(object o)
        {
            lock (this)
            {
                if (_dragtimer != null)
                {
                    _dragtimer = null;
                    DataObject _dataobject = new DataObject();
                    _dataobject.SetData(DataFormats.UnicodeText, _textbox.Text);
                    this.DoDragDrop(_dataobject, DragDropEffects.Copy);
                }
            }
        }

        #endregion

        #region _SizeChanged

        private void _SizeChanged(object sender, EventArgs e)
        {
            float _text_size = 25.0f * (((float) this._textbox.Width) / 200.0f);
            _text_size = Math.Min(72.0f, Math.Max(5.0f, _text_size));
            this._textbox.Font = new Font(
                FontFamily.GenericSansSerif, _text_size, FontStyle.Regular);
        }

        #endregion
    }
}
