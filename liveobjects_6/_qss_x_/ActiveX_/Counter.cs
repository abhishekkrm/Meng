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
using System.Reflection;
using System.Threading;
using Microsoft.Win32;

namespace QS._qss_x_.ActiveX_
{
    [Guid("D9BB6DA5-2C48-4dbe-B179-DED0F9F5234B")]
    [ProgId("QS.Fx.ActiveX.Counter")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public partial class Counter : UserControl, IDisposable
    {
        #region Constructor

        public Counter()
        {
            InitializeComponent();

            try
            {
                thread = new Thread(new ThreadStart(this.ThreadCallback));
                finished = false;
                finishedevent = new ManualResetEvent(false);
                thread.Start();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                finished = true;
                finishedevent.Set();
                if (!thread.Join(TimeSpan.FromMilliseconds(100)))
                    thread.Abort();

                base.Dispose();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Fields

        private bool finished;
        private ManualResetEvent finishedevent;
        private Thread thread;
        private int num;
        private double frequency;

        #endregion

        #region ThreadCallback

        private delegate void FrequencyCallback(double _frequency);

        private void ThreadCallback()
        {
            try
            {
                while (!finished)
                {
                    textBox1.Text = (++num).ToString();

                    double _frequency = frequency;
                    if ((_frequency <= 0) || (_frequency >= 100))
                        _frequency = 1;
                    frequency = _frequency;

                    bool ok = false;
                    for (int countdown = 3; !ok && countdown > 0; countdown--)
                    {                        
                        try
                        {
                            if (textBox3.InvokeRequired)
                                textBox3.BeginInvoke(
                                    new FrequencyCallback(delegate(double x) { textBox3.Text = x.ToString(); }),
                                    new object[] { _frequency });
                            else
                                textBox3.Text = _frequency.ToString();
                            ok = true;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    finishedevent.WaitOne(TimeSpan.FromSeconds(1 / _frequency), false);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Accessors

        public string Frequency
        {
            get { return frequency.ToString(); }
            set 
            {
                if (value != null)
                {
                    try
                    {
                        double _frequency = Convert.ToDouble(value);
                        if (_frequency > 0 && _frequency < 100)
                            frequency = _frequency;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        #endregion

        #region RegisterClass

        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            QS._qss_x_.ActiveX_.Registration.Register(key);
        }

        #endregion

        #region UnregisterClass

        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            QS._qss_x_.ActiveX_.Registration.Unregister(key);
        }

        #endregion

        #region User Interface Events

        private void button1_Click(object sender, EventArgs e)
        {
            this.Frequency = textBox2.Text;
        }

        #endregion
    }
}
