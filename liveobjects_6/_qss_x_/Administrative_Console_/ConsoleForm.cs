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

namespace QS._qss_x_.Administrative_Console_
{
    public partial class ConsoleForm : Form
    {
        #region Constructor

        public ConsoleForm(Configuration configuration)
        {
            InitializeComponent();

            ((IConsole)console1).Initialize(configuration);

            this.Show();

            Rectangle rec = Screen.PrimaryScreen.WorkingArea;
            this.DesktopBounds = new Rectangle(
                (int)Math.Round(rec.Left + ((double)rec.Width * 0.5) / 2),
                (int)Math.Round(rec.Top + ((double)rec.Height * 0.5) / 2),
                (int)Math.Round(((double)rec.Width) * 0.5),
                (int)Math.Round(((double)rec.Height) * 0.5));

            // EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);

            // this.WindowState = FormWindowState.Minimized;
            // this.Hide();

            // EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);
        }

        #endregion

        #region Fields

        #endregion

        #region Win32

        // [DllImport("user32")]
        // private static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);
        // [DllImport("user32")]
        // private static extern bool EnableMenuItem(IntPtr hMenu, uint wIDEnableItem, uint wEnable);

        #endregion

        #region GUI Event Handlers

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            // EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);
        }

        private void ConsoleForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
            // EnableMenuItem(GetSystemMenu(this.Handle, false), 0xF060, 0x1);
        }

        #endregion

        #region Clicking

        #endregion

        #region Closing

        private void AdminConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((IConsole)console1).Cleanup();
        }

        #endregion
    }
}
