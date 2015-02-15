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

namespace QS.GUI.Fx.Simulations
{
    public partial class SimulationStatusStrip : UserControl, IDisposable
    {
        public SimulationStatusStrip()
        {
            InitializeComponent();
        }

        private QS._qss_x_.Simulations_.ISimulation simulation;
        private bool done;
        private ManualResetEvent check = new ManualResetEvent(false);
        private Thread thread;

        #region Accessors

        public QS._qss_x_.Simulations_.ISimulation Simulation 
        {
            get { return simulation; }

            set 
            {
                lock (this)
                {
                    this.simulation = value;
                    if (simulation != null)
                    {
                        if (thread == null)
                        {
                            thread = new Thread(new ThreadStart(this.MonitoringLoop));
                            thread.Start();
                        }
                    }
                    else
                        _StopMonitoring();
                }
            }           
        }

        #endregion

        #region MonitoringLoop

        private void MonitoringLoop()
        {
            while (true)
            {
                check.WaitOne(200, false);

                if (!done)
                {
                    try
                    {
                        this.BeginInvoke(new QS.Fx.Base.Callback(
                            delegate()
                            {
                                try
                                {
                                    toolStripStatusLabel1.Text = 
                                        (simulation.Running ? (simulation.Finished ? "Finished" : "Running") : (simulation.Finished ? "Complete" : "Stopped"));
                                    toolStripStatusLabel2.Text = "Time: " + simulation.SimulationTime + "s";
                                    toolStripStatusLabel3.Text = "Handled: " + simulation.NumberOfEventsHandled.ToString();
                                    toolStripStatusLabel4.Text = "Pending: " + simulation.NumberOfEventsPending.ToString();
                                }
                                catch (System.Exception)
                                {
                                }
                            }));
                    }
                    catch (System.Exception)
                    {
                    }
                }
                else
                    break;
            }
        }

        #endregion

        #region Control

        public void _StopMonitoring()
        {
            lock (this)
            {
                if (thread != null)
                {
                    Monitor.Exit(this);
                    try
                    {
                        done = true;
                        check.Set();
                        if (thread.Join(1000))
                            thread = null;
                    }
                    finally
                    {
                        Monitor.Enter(this);
                    }

                    if (thread != null)
                        thread.Abort();
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _StopMonitoring();
            base.Dispose();            
        }

        #endregion
    }
}
