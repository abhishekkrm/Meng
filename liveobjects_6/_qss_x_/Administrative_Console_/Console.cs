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

namespace QS._qss_x_.Administrative_Console_
{
    public partial class Console : UserControl, IConsole
    {
        #region Constructor

        public Console()
        {
            InitializeComponent();
        }

        #endregion

        #region IConsole Members

        void IConsole.Initialize(Configuration configuration)
        {
            if (configuration != null)
            {
                foreach (Configuration.Connection _connection in configuration.Connections)
                {
                    Connection connection = new Connection(_connection.Name, _connection.Address);
                    listView1.Items.Add(
                        new _ConnectionRef(_connection.Name, _connection.Address, connection));
                    connection.Dock = DockStyle.Fill;
                    connection.Visible = false;
                    mainpanel.Controls.Add(connection);
                    connection.Visible = false;
                }
            }
        }

        void IConsole.AddLocal(string name, QS._qss_x_.Service_Old_.IService service)
        {
            Connection connection = new Connection(name, service);
            listView1.Items.Add(
                new _ConnectionRef(name, "hosted locally", connection));
            connection.Dock = DockStyle.Fill;
            connection.Visible = false;
            mainpanel.Controls.Add(connection);
            connection.Visible = false;
        }

        void IConsole.AddRemote(string name, string address)
        {
            Connection connection = new Connection(name, address);
            listView1.Items.Add(
                new _ConnectionRef(name, address, connection));
            connection.Dock = DockStyle.Fill;
            connection.Visible = false;
            mainpanel.Controls.Add(connection);
            connection.Visible = false;
        }

        void IConsole.Cleanup()
        {
            foreach (_ConnectionRef connectionref in listView1.Items)
            {
                IConnection connection = connectionref.connection;
                if (connection.Connected)
                    connection.BeginDisconnect(new AsyncCallback(this.DisconnectCallback), connection);
            }
        }

        #endregion

        #region Fields

        private _ConnectionRef selectedconnectionref;

        #endregion

        #region Clicking

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                _ConnectionRef connectionref = listView1.SelectedItems[0] as _ConnectionRef;
                if (connectionref != null)
                    ((IConnection) connectionref.connection).BeginConnect(
                        new AsyncCallback(this.ConnectCallback), connectionref.connection);
                _UpdateSelected();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                _ConnectionRef connectionref = listView1.SelectedItems[0] as _ConnectionRef;
                if (connectionref != null)
                    ((IConnection) connectionref.connection).BeginDisconnect(
                        new AsyncCallback(this.DisconnectCallback), connectionref.connection);
                _UpdateSelected();
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _UpdateSelected();
        }

        #endregion

        #region ConnectCallback

        private void ConnectCallback(IAsyncResult result)
        {
            ((IConnection) result.AsyncState).EndConnect(result);
            _UpdateSelected();
        }

        #endregion

        #region DisconnectCallback

        private void DisconnectCallback(IAsyncResult result)
        {
            ((IConnection) result.AsyncState).EndDisconnect(result);
            _UpdateSelected();
        }

        #endregion

        #region _UpdateSelected

        private void _UpdateSelected()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                _ConnectionRef connectionref = listView1.SelectedItems[0] as _ConnectionRef;

                if (selectedconnectionref != connectionref && selectedconnectionref != null)
                {
                    ToolStripManager.RevertMerge(toolStrip1, selectedconnectionref.connection.ToolStrip);
                    selectedconnectionref.connection.Visible = false;
                }

                if (connectionref != null)
                {
                    bool connected = ((IConnection)connectionref.connection).Connected;
                    toolStripButton1.Enabled = !connected;
                    toolStripButton2.Enabled = connected;
                    connectionref.connection.Visible = true;
                    toolStripStatusLabel1.Text = connected ? "connected" : "disconnected";
                    toolStripStatusLabel2.Text = ((IConnection)connectionref.connection).Address;

                    if (connectionref != selectedconnectionref)
                        ToolStripManager.Merge(connectionref.connection.ToolStrip, toolStrip1);
                }
                else
                {
                    toolStripButton1.Enabled = false;
                    toolStripButton2.Enabled = false;
                    toolStripStatusLabel1.Text = string.Empty;
                    toolStripStatusLabel2.Text = string.Empty;
                }

                selectedconnectionref = connectionref;
            }
        }

        #endregion

        #region _ConnectionRef

        private class _ConnectionRef : ListViewItem
        {
            public _ConnectionRef(string name, string address, Connection connection) : base(name)
            {
                this.name = name;
                this.address = address;
                this.connection = connection;

#if !MONO
                this.ToolTipText = address;
#endif
            }

            public string name, address;
            public Connection connection;
        }

        #endregion
    }
}
