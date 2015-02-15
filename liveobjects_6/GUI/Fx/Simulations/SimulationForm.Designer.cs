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

namespace QS.GUI.Fx.Simulations
{
    partial class SimulationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulationForm));
            this.simulationToolStrip1 = new QS.GUI.Fx.Simulations.SimulationToolStrip();
            this.simulationStatusStrip1 = new QS.GUI.Fx.Simulations.SimulationStatusStrip();
            this.inspector1 = new QS.GUI.Components.Inspector();
            this.SuspendLayout();
            // 
            // simulationToolStrip1
            // 
            this.simulationToolStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.simulationToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.simulationToolStrip1.Name = "simulationToolStrip1";
            this.simulationToolStrip1.Simulation = null;
            this.simulationToolStrip1.Size = new System.Drawing.Size(954, 25);
            this.simulationToolStrip1.TabIndex = 0;
            // 
            // simulationStatusStrip1
            // 
            this.simulationStatusStrip1.AutoSize = true;
            this.simulationStatusStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.simulationStatusStrip1.Location = new System.Drawing.Point(0, 638);
            this.simulationStatusStrip1.Name = "simulationStatusStrip1";
            this.simulationStatusStrip1.Simulation = null;
            this.simulationStatusStrip1.Size = new System.Drawing.Size(954, 24);
            this.simulationStatusStrip1.TabIndex = 1;
            // 
            // inspector1
            // 
            this.inspector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inspector1.Location = new System.Drawing.Point(0, 25);
            this.inspector1.Name = "inspector1";
            this.inspector1.Size = new System.Drawing.Size(954, 613);
            this.inspector1.TabIndex = 2;
            // 
            // SimulationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 662);
            this.Controls.Add(this.inspector1);
            this.Controls.Add(this.simulationStatusStrip1);
            this.Controls.Add(this.simulationToolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SimulationForm";
            this.Text = "Simulation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimulationForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SimulationToolStrip simulationToolStrip1;
        private SimulationStatusStrip simulationStatusStrip1;
        private QS.GUI.Components.Inspector inspector1;
    }
}
