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
using System.Threading;

namespace QS.GUI
{
    public partial class NewLogWindow : UserControl, QS.Fx.Logging.IConsole, QS.Fx.Logging.ILogger
    {
        #region Constructor

        public NewLogWindow()
        {
            InitializeComponent();
            this._callback_1 = new System.Threading.TimerCallback(this._Update_1);
            this._callback_2 = new QS.Fx.Base.Callback(this._Update_2);
        }

        public NewLogWindow(QS.Fx.Logging.IConsole ul) : this()
        {
            this.underlyingConsole = ul;
        }

        #endregion

        #region Fields

        private int _lastno, _schedule, _operating;
        private _MyItem _lastitem;
        private System.Threading.Timer _timer;
        private System.Threading.TimerCallback _callback_1;
        private QS.Fx.Base.Callback _callback_2;
        private Stack<_MyItem> _todo = new Stack<_MyItem>();
        private QS.Fx.Logging.IConsole underlyingConsole;

        #endregion

        #region Class _MyItem

        private class _MyItem : ListViewItem
        {
            public _MyItem(int _no, string _message) : base(new string[] { ((_no > 0) ? _no.ToString() : string.Empty), _message })
            {
                this._no = _no;
                this._message = _message;
            }

            public int _no;
            public string _message;
            public _MyItem _next;
        }

        #endregion

        #region IConsole Members

        public void Log(string _s)
        {
            if (this.underlyingConsole != null)
                this.underlyingConsole.Log(_s);

            _MyItem _myitem = new _MyItem(Interlocked.Increment(ref this._lastno), _s);
            _MyItem _lastitem;
            do
            {
                _lastitem = this._lastitem;
                _myitem._next = _lastitem;
            }
            while (Interlocked.CompareExchange<_MyItem>(ref this._lastitem, _myitem, _lastitem) != _lastitem);
            this._schedule = 1;
            if ((this._operating == 0) && (Interlocked.CompareExchange(ref this._operating, 1, 0) == 0))
            {
                this._timer = new System.Threading.Timer(this._callback_1, null, 1, System.Threading.Timeout.Infinite);
            }
        }
        
        #endregion

        #region _Update_1

        private void _Update_1(object _o)
        {
            try
            {
                if (listView1.InvokeRequired)
                    listView1.BeginInvoke(this._callback_2);
                else
                    this._Update_3();
            }
            catch (Exception _exc)
            {
                System.Windows.Forms.MessageBox.Show(_exc.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region _Update_2

        private void _Update_2()
        {
            try
            {
                this._Update_3();
            }
            catch (Exception _exc)
            {
                System.Windows.Forms.MessageBox.Show(_exc.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region _Update_3

        private void _Update_3()
        {
            lock (this)
            {
                do
                {
                    while (this._schedule != 0)
                    {
                        this._schedule = 0;
                        _MyItem _myitem = Interlocked.Exchange<_MyItem>(ref this._lastitem, null);
                        if (_myitem != null)
                        {
                            do
                            {
                                _MyItem _next = _myitem._next;
                                _myitem._next = null;
                                _todo.Push(_myitem);
                                _myitem = _next;
                            }
                            while (_myitem != null);
                        }
                    }
                    this._operating = 0;
                }
                while ((this._schedule != 0) && (Interlocked.CompareExchange(ref this._operating, 1, 0) == 0));
                
                listView1.BeginUpdate();
                while (_todo.Count > 0)
                    listView1.Items.Add(_todo.Pop());
                listView1.EndUpdate();
            }
        }

        #endregion

        #region listView1_SelectedIndexChanged

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ListView.SelectedListViewItemCollection selecteditems = listView1.SelectedItems;
                if (selecteditems.Count > 0)
                {
                    _MyItem selecteditem = (_MyItem) selecteditems[0];
                    richTextBox1.Text = selecteditem._message;
                }
                else
                    richTextBox1.Clear();
            }
        }

        #endregion

        #region ILogger Members

        public void Clear()
        {
            lock (this)
            {
                try
                {
                    listView1.BeginUpdate();
                    listView1.Items.Clear();
                    listView1.EndUpdate();
                }
                catch (Exception)
                {
                    try
                    {
                        listView1.BeginInvoke
                        (
                            new QS.Fx.Base.Callback(
                                delegate()
                                {
                                    lock (this)
                                    {
                                        listView1.BeginUpdate();
                                        listView1.Items.Clear();
                                        listView1.EndUpdate();
                                    }
                                }
                            ), 
                            null
                        );
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.Assert(false, exc.ToString());
                    }
                }
            }            
        }

        public void Log(object source, string message)
        {
            ((QS.Fx.Logging.IConsole) this).Log(source.GetType() + " : " + message);
        }

        #endregion
    }
}
