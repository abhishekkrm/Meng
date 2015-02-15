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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Isis;

namespace QS.Fx.Object
{
    public partial class DebuggerForm : Form
    {
        #region Constructor

        public DebuggerForm(string _objectname, string _typename, QS.Fx.Object.Classes.IObject _object, QS.Fx.Object.IRuntimeContext _context, bool _inspection)
        {
            InitializeComponent();
            
            this._inspection = _inspection;
            this._object = _object;
            this._context = _context;

            this.Text = "Debugging object \"" + _objectname + "\"" + ((_typename != null) ? (" of type \"" + _typename + "\"") : "") + ".";
            this.toolStripStatusLabel1.Text = "RUNNING";
            
            if (this._inspection)
            {
                this._inspector = new QS.GUI.Components.Inspector();
                this._inspector.Dock = DockStyle.Fill;
                this.panel1.Controls.Add(this._inspector);
                this._inspector.Visible = false;
                ((QS.GUI.Components.IInspector)this._inspector).Add(_object);
            }
            
            this._mainpanel = new System.Windows.Forms.Panel();
            this._mainpanel.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(this._mainpanel);
            
            this._logwindow = new QS.GUI.NewLogWindow(this._context.Console);
            this._logwindow.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(this._logwindow);
            this._logwindow.Visible = true;
            this._context.Console = this._logwindow;
            
            if (!this._inspection)
            {
                this.toolStripButton1.Visible = false;
                this.toolStripButton2.Visible = false;
                this.toolStripButton3.Visible = false;
                this.toolStripButton1.Enabled = false;
                this.toolStripButton2.Enabled = false;
                this.toolStripButton3.Enabled = false;
                this.toolStrip1.Enabled = false;
                this.toolStrip1.Visible = false;
                this._inspector = null;
            }

            if (this._object is QS.Fx.Object.Classes.IUI)
            {
                this._mycontext = new QS._qss_x_.Object_.Context_(this._context.Platform, QS._qss_x_.Object_.Context_.ErrorHandling_.Halt,
                    QS.Fx.Object.Runtime.SynchronizationOption, QS.Fx.Object.Runtime.SynchronizationOption);
                this._uiendpoint = this._mycontext.ImportedUI(this._mainpanel);
                this._uiendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
                this._uiendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);
                this._uiconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._uiendpoint).Connect(((QS.Fx.Object.Classes.IUI)this._object).UI);
                this._logwindow.Visible = false;
                this.toolStripButton4.Enabled = true;
                this.toolStripButton5.Enabled = false;
                this._mainpanel.Visible = true;
                this._mainpanel.BringToFront();
            }
            else
            {
                this._logwindow.Visible = true;
                this.toolStripButton4.Enabled = false;
                this.toolStripButton5.Enabled = false;
                this._mainpanel.Visible = false;
                this._logwindow.BringToFront();
            }

            //IsisSystem.Start();
            
        }

        #endregion

        #region Fields

        private QS.Fx.Object.Classes.IObject _object;
        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Endpoint.Internal.IImportedUI _uiendpoint;
        private QS.Fx.Endpoint.IConnection _uiconnection;

        private QS.Fx.Object.IRuntimeContext _context;
        private QS.GUI.Components.Inspector _inspector;
        private QS.GUI.NewLogWindow _logwindow;
        private bool _inspection;
        private System.Windows.Forms.Panel _mainpanel;

        #endregion

        #region Closing

        private void DebuggerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        #endregion

        #region Clicking

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this._context.Start();
            this.toolStripButton1.Enabled = false;
            this.toolStripButton2.Enabled = true;
            this.toolStripStatusLabel1.Text = "RUNNING";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this._context.Stop();
            this.toolStripButton1.Enabled = true;
            this.toolStripButton2.Enabled = false;
            this.toolStripStatusLabel1.Text = "STOPPED";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this._inspection)
            {
                this._mainpanel.Visible = false;
                this._inspector.Visible = true;
                this._logwindow.Visible = false;
                this.toolStripButton3.Enabled = false;
                this.toolStripButton4.Enabled = true;
                if (this._object is QS.Fx.Object.Classes.IUI)
                    this.toolStripButton5.Enabled = true;
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this._mainpanel.Visible = false;
            this._inspector.Visible = false;
            this._logwindow.Visible = true;
            if (this._inspection)
                this.toolStripButton3.Enabled = true;
            this.toolStripButton4.Enabled = false;
            if (this._object is QS.Fx.Object.Classes.IUI)
                this.toolStripButton5.Enabled = true;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            this._mainpanel.Visible = true;
            this._inspector.Visible = false;
            this._logwindow.Visible = false;
            if (this._inspection)
                this.toolStripButton3.Enabled = true;
            this.toolStripButton4.Enabled = true;
            this.toolStripButton5.Enabled = false;
        }

        #endregion

        #region _UIConnectCallback

        private void _UIConnectCallback()
        {
            this._uiendpoint.UI.Dock = DockStyle.Fill;
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
        }

        #endregion
    }
}
