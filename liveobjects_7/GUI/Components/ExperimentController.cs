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

namespace QS.GUI.Components
{
    public partial class ExperimentController : UserControl
    {
        public ExperimentController(
            QS._qss_e_.Components_.IExperimentController experimentController, Control experimentControllerGUI)
        {
            InitializeComponent();

            if (experimentControllerGUI != null)
                controllerPanel.Controls.Add(experimentControllerGUI);
            else
                controllerPanel.Hide();

            ((IObjectSelector)objectSelector).SelectionChanged += new EventHandler(
                delegate(object sender, EventArgs e)
                {
                    ((IObjectVizualizer)objectVisualizer).VisualizedObject = ((IObjectSelector)objectSelector).SelectedObject;
                });

            this.experimentController = experimentController;
            ((IObjectSelector)objectSelector).Add(experimentController);
        }

        private QS._qss_e_.Components_.IExperimentController experimentController;

        #region Internal Processing

        public void Run()
        {
            IExperimentConfigurator experimentConfigurator = (IExperimentConfigurator)experimentCfg11;
            QS._core_c_.Components.AttributeSet arguments = experimentConfigurator.Arguments;
            experimentController.Run(experimentConfigurator.Class, arguments);
            button1.Enabled = false;
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.Run();
        }

        private void refreshRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IObjectSelector)objectSelector).RefreshRecursively();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IObjectSelector)objectSelector).ExpandAll();
        }

        private void refreshSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ((IObjectSelector)objectSelector).Refres();
        }

        private void unrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IObjectSelector)objectSelector).Unroll();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IObjectSelector)objectSelector).Refresh();
        }

        private void splitContainer1_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.SplitterDistance = (int)Math.Floor(splitContainer1.Height * 0.5);
            }
            catch (Exception)
            {
            }
        }
    }
}
