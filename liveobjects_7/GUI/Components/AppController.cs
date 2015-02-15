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

namespace QS.GUI.Components
{
    public partial class AppController : Form, QS._qss_e_.Runtime_.IAppController
    {
        public static void Show(QS._qss_e_.Runtime_.IControlledApp controlledApp, string name)
        {
            new Thread(new ThreadStart(delegate { new AppController(controlledApp, name); }));
        }

        private AppController(QS._qss_e_.Runtime_.IControlledApp controlledApp, string name)
        {
            InitializeComponent();

            this.Text = name;
            this.controlledApp = controlledApp;
            if (controlledApp != null)
                toolStripStatusLabel1.Text = controlledApp.Running ? "RUNNING" : "STOPPED";
        }

        private QS._qss_e_.Runtime_.IControlledApp controlledApp;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (controlledApp != null)
            {
                controlledApp.Start();
                toolStripButton1.Enabled = false;
                toolStripButton2.Enabled = true;
                toolStripStatusLabel1.Text = controlledApp.Running ? "RUNNING" : "STOPPED";
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (controlledApp != null)
            {
                controlledApp.Stop();
                toolStripButton1.Enabled = true;
                toolStripButton2.Enabled = false;
                toolStripStatusLabel1.Text = controlledApp.Running ? "RUNNING" : "STOPPED";
            }
        }

        #region IAppController Members

        QS._qss_e_.Runtime_.IControlledApp QS._qss_e_.Runtime_.IAppController.ControlledApp
        {
            get { return controlledApp; }
            set 
            { 
                controlledApp = value;
                if (controlledApp != null)
                    toolStripStatusLabel1.Text = controlledApp.Running ? "RUNNING" : "STOPPED";
            }
        }

        #endregion
    }
}
