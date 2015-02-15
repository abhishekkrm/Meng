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
using System.Runtime.InteropServices;

namespace QS._qss_d_.ServiceController_
{
    public partial class ServiceController : Form
    {
        [DllImport("user32")]
        private static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);
        [DllImport("user32")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint wIDEnableItem, uint wEnable);

        private const double ScalingRatio = 0.5;

        public ServiceController(QS._core_c_.Base.IOutputReader serviceLog, object inspectedObject,
            QS._qss_d_.Service_2_.Service service)
        {
            this.serviceLog = serviceLog;
            this.inspectedObject = inspectedObject;
            this.service = service;

            InitializeComponent();

            checkBox1.Checked = service.ProcessesStartedManuallyByTheUser;

            this.Show();
            // this.WindowState = FormWindowState.Maximized;

            Rectangle rec = Screen.PrimaryScreen.WorkingArea;
            this.DesktopBounds = new Rectangle(
                (int) Math.Round(rec.Left + ((double) rec.Width * (1 - ScalingRatio)) / 2),
                (int) Math.Round(rec.Top + ((double) rec.Height * (1 - ScalingRatio)) / 2),
                (int) Math.Round(((double) rec.Width) * ScalingRatio),
                (int)Math.Round(((double) rec.Height) * ScalingRatio));

            EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);

            tabControl1.SelectedIndex = 0;
            ActivateLogConsole();

            ((QS.GUI.Components.IInspector)inspector1).Add(inspectedObject);

            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);
        }

        private QS._core_c_.Base.IOutputReader serviceLog;
        private object inspectedObject;
        private QS._qss_d_.Service_2_.Service service;

        #region Toolbar Icon and Size Changes

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            // this.WindowState = FormWindowState.Maximized;
            EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);
        }

        private void ServiceController_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState ==  FormWindowState.Minimized)
                this.Hide();
            EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);
        }

        #endregion

        #region Tab Selection

        private void ActivateLogConsole()
        {
            serviceLog.Console = null;
            logWindow1.Clear();
            serviceLog.Console = logWindow1;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        ActivateLogConsole();
                        // inspector1.ObjectSelector.SelectedObject = null;
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBox1.Text);
            int pid = Convert.ToInt32(textBox2.Text);
            service.RegisterProcess(id, pid);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            service.ProcessesStartedManuallyByTheUser = checkBox1.Checked;
        }
    }
}
