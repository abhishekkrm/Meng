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
using System.ServiceModel;
using System.Threading;

namespace QS._qss_x_.Administrative_Console_
{
    public partial class Connection : UserControl, IConnection, IDisposable
    {
        #region Constructor

        public Connection(string name, string address)
        {
            InitializeComponent();

            this.name = name;
            this.address = address;
            this.Enabled = false;

            toolStripButton1.ToolTipText = "Refresh \"" + name + "\" (" + address + ")";
        }

        public Connection(string name, QS._qss_x_.Service_Old_.IService service)
        {
            InitializeComponent();

            this.name = name;
            this.address = null;
            this.Enabled = false;

            this.service = service;
            this.islocal = true;

            toolStripButton1.ToolTipText = "Refresh \"" + name + "\" (" + address + ")";
        }

        #endregion

        #region Fields

        private string name, address;
        private bool islocal, connected;
        private ChannelFactory<Service_Old_.IService> servicefactory;
        private Service_Old_.IService service;
        private int lastmessage;
        private Thread thread;
        private bool closing;
        private AutoResetEvent recheck = new AutoResetEvent(false);
        private AutoResetEvent messagesadded = new AutoResetEvent(false);
        private NamespaceNode selectednode;

        #endregion

        #region IConnection Members

        string IConnection.Name
        {
            get { return name; }
        }

        string IConnection.Address
        {
            get { return address; }
        }

        bool IConnection.Connected
        {
            get { return connected; }
        }

        IAsyncResult IConnection.BeginConnect(AsyncCallback callback, object cookie)
        {
            lock (this)
            {
                if (!connected)
                {
                    if (!islocal)
                    {
                        servicefactory = new ChannelFactory<Service_Old_.IService>(new WSHttpBinding(), new EndpointAddress("http://" + address));
                        service = servicefactory.CreateChannel();
                    }

                    connected = true;
                    this.Enabled = true;

                    Initialization();

                    closing = false;
                    recheck.Reset();
                    thread = new Thread(new ThreadStart(this.ThreadCallback));
                    thread.Start();
                }
            }

            QS._qss_c_.Base3_.AsyncResult<object> result = new QS._qss_c_.Base3_.AsyncResult<object>(callback, cookie, null);
            result.Completed(true, true, null);
            return result;
        }

        void IConnection.EndConnect(IAsyncResult request)
        {
        }

        IAsyncResult IConnection.BeginDisconnect(AsyncCallback callback, object cookie)
        {
            lock (this)
            {
                if (connected)
                {
                    closing = true;
                    recheck.Set();
                    thread.Join();
                    thread = null;

                    Cleanup();

                    this.Enabled = false;
                    connected = false;

                    if (!islocal)
                    {
                        service = null;
                        servicefactory.Close();
                    }
                }
            }

            QS._qss_c_.Base3_.AsyncResult<object> result = new QS._qss_c_.Base3_.AsyncResult<object>(callback, cookie, null);
            result.Completed(true, true, null);
            return result;
        }

        void IConnection.EndDisconnect(IAsyncResult request)
        {
        }

        #endregion

        #region Clicking

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                if (listView1.SelectedItems.Count == 1)
                    richTextBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
                else
                    richTextBox1.Clear();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            recheck.Set();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NamespaceNode node = treeView1.SelectedNode as NamespaceNode;
            if (node != selectednode)
            {
                if (selectednode != null)
                    selectednode._Deselect();
                selectednode = node;

                if (node != null)
                {
                    node._Refresh(true, true);
                    node._Select();
                }
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            NamespaceNode node = treeView1.SelectedNode as NamespaceNode;
            if (node != null)
            {
                node._Refresh(true, true);
            }            
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                TreeNode _node = treeView1.GetNodeAt(p);
                if (_node != null)
                {
                    ((NamespaceNode) _node)._Menu(p);
                }
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return name;
        }

        #endregion

        #region ThreadCallback

        private void ThreadCallback()
        {
            while (!closing)
            {
                lock (this)
                {
                    int nmessages = service.NumberOfLogMessages;
                    while (lastmessage < nmessages)
                    {
                        int from = lastmessage + 1;
                        int to = Math.Min(lastmessage + 100, nmessages);

                        string[] messages = service.DownloadLogMessages(from, to);
                        lastmessage += messages.Length;

                        listView1.BeginInvoke(new QS.Fx.Base.ContextCallback<int, string[]>(this.MessageCallback), new object[] { from, messages });
                        messagesadded.WaitOne();
                    }
                }

                recheck.WaitOne();
            }
        }

        #endregion

        #region MessageCallback

        private void MessageCallback(int from, string[] messages)
        {
            listView1.BeginUpdate();
            for (int ind = 0; ind < messages.Length; ind++)
                listView1.Items.Add(new ListViewItem(new string[] { (from + ind).ToString(), messages[ind] }));
            listView1.EndUpdate();
            messagesadded.Set();
        }

        #endregion

        #region Initialization

        private void Initialization()
        {
            listView1.Items.Clear();
            richTextBox1.Clear();
            lastmessage = 0;

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(new NamespaceNode(service, service.NamespaceRoot));
            treeView1.EndUpdate();
        }

        #endregion

        #region Cleanup

        private void Cleanup()
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Accessors

        public ToolStrip ToolStrip
        {
            get { return toolStrip1; }
        }

        #endregion
    }
}
