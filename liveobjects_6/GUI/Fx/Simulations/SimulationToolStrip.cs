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

namespace QS.GUI.Fx.Simulations
{
    public partial class SimulationToolStrip : UserControl
    {
        public SimulationToolStrip()
        {
            InitializeComponent();
        }

        private QS._qss_x_.Simulations_.ISimulation simulation;

        #region Accessors

        public QS._qss_x_.Simulations_.ISimulation Simulation
        {
            get { return simulation; }

            set 
            { 
                lock (this)
                {
                    simulation = value; 
                }
            }
        }

        #endregion

        #region Clicking

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (simulation != null)
                {
                    if (simulation.Finished)
                    {
                        toolStripButton1.Enabled = false;
                        toolStripButton2.Enabled = false;
                        toolStripButton3.Enabled = false;
                    }
                    else
                    {
                        toolStripButton1.Enabled = false;
                        toolStripButton2.Enabled = false;
                        toolStripButton3.Enabled = true;

                        simulation.Start();
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (simulation != null)
                {
                    if (simulation.Finished)
                    {
                        toolStripButton1.Enabled = false;
                        toolStripButton2.Enabled = false;
                        toolStripButton3.Enabled = false;
                    }
                    else
                    {
                        if (!simulation.Running)
                            simulation.Step();
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (simulation != null)
                {
                    if (simulation.Finished)
                    {
                        toolStripButton1.Enabled = false;
                        toolStripButton2.Enabled = false;
                        toolStripButton3.Enabled = false;
                    }
                    else
                    {
                        toolStripButton1.Enabled = true;
                        toolStripButton2.Enabled = true;
                        toolStripButton3.Enabled = false;

                        simulation.Stop();
                    }
                }
            }
        }

        #endregion
    }
}
