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

namespace QS._qss_e_.Experiments_
{
    public partial class AppController : Form
    {
        public static void Show(string name, QS._qss_e_.Runtime_.IControlledApp controlledApp)
        {
            (new System.Threading.Thread(new System.Threading.ThreadStart(
                delegate
                {
                    (new AppController(name, controlledApp)).ShowDialog();
                }))).Start();
        }

        private AppController(string name, QS._qss_e_.Runtime_.IControlledApp controlledApp)
        {
            InitializeComponent();

            ((QS.GUI.Components.IInspector)inspector1).Add(controlledApp);

            this.Text = name;
            this.controlledApp = controlledApp;

            UpdateStatus();            
        }

        private QS._qss_e_.Runtime_.IControlledApp controlledApp;

        private void UpdateStatus()
        {
            bool running = controlledApp.Running;
            toolStripStatusLabel1.Text = running ? "RUNNING" : "SUSPENDED";
            toolStripButton1.Enabled = !running;
            toolStripButton2.Enabled = running;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                controlledApp.Start();
            }
            catch (Exception)
            {
            }

            UpdateStatus();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                controlledApp.Stop();
            }
            catch (Exception)
            {
            }

            UpdateStatus();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            (new QS.GUI.Components.PythonConsole(inspector1.ObjectSelector)).Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            QS.GUI.Components.Box.Show(inspector1.ObjectSelector.SelectedObject);
        }
    }
}
