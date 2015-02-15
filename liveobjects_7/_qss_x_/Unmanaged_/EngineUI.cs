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

namespace QS._qss_x_.Unmanaged_
{
    public partial class EngineUI : Form, IEngineUI
    {
        #region Constructor

        public EngineUI()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private EventHandler onStart, onStop, onClosing;
        private string application, status;
        private bool canStart, canStop;

        #endregion

        #region Constants

        private const string titleprefix = "QuickSilver for Unmanaged Applications : ";

        #endregion

        #region IEngineUI Members

        QS.Fx.Logging.IConsole IEngineUI.Console
        {
            get { return newLogWindow1; }
        }

        QS.GUI.Components.IInspector IEngineUI.Inspector
        {
            get { return inspector1; }
        }

        string IEngineUI.Application
        {
            get { return application;  }
            
            set
            {
                application = value;

                try
                {
                    this.Text = titleprefix + ((application != null) ? application : string.Empty);
                }
                catch (Exception)
                {
                    this.BeginInvoke
                    (
                        new QS._qss_c_.Base3_.NoArgumentCallback
                        (
                            delegate()
                            {
                                this.Text = titleprefix + ((application != null) ? application : string.Empty);
                            }
                        )
                    );
                }
            }
        }

        string IEngineUI.Status
        {
            get { return status; }
            
            set 
            {
                status = value;
                
                try
                {
                    toolStripStatusLabel1.Text = status;
                }
                catch (Exception)
                {
                    statusStrip1.BeginInvoke
                    (
                        new QS._qss_c_.Base3_.NoArgumentCallback
                        (
                            delegate() 
                            {
                                toolStripStatusLabel1.Text = ((status != null) ? status : string.Empty); 
                            }
                        )
                    );
                }
            }
        }

        bool IEngineUI.CanStart
        {
            get { return canStart; }
            
            set
            {
                canStart = value;

                try
                {
                    toolStripButton1.Enabled = canStart;
                }
                catch (Exception)
                {
                    toolStrip1.BeginInvoke
                    (
                        new QS._qss_c_.Base3_.NoArgumentCallback
                        (
                            delegate()
                            {
                                toolStripButton1.Enabled = canStart;
                            }
                        )
                    );
                }
            }
        }

        bool IEngineUI.CanStop
        {
            get { return canStop; }

            set
            {
                canStop = value;

                try
                {
                    toolStripButton2.Enabled = canStop;
                }
                catch (Exception)
                {
                    toolStrip1.BeginInvoke
                    (
                        new QS._qss_c_.Base3_.NoArgumentCallback
                        (
                            delegate()
                            {
                                toolStripButton2.Enabled = canStop;
                            }
                        )
                    );
                }
            }
        }

        event EventHandler IEngineUI.OnStart
        {
            add { onStart += value; }
            remove { onStart -= value; }
        }

        event EventHandler IEngineUI.OnStop
        {
            add { onStop += value; }
            remove { onStop -= value; }
        }

        event EventHandler IEngineUI.OnClosing
        {
            add { onClosing += value; }
            remove { onClosing -= value; }
        }

        #endregion

        #region User Interface Event Handlers

        private void EngineUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (onClosing != null)
                onClosing(this, null);

            e.Cancel = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (onStart != null)
                onStart(this, null);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (onStop != null)
                onStop(this, null);
        }

        #endregion
    }
}
