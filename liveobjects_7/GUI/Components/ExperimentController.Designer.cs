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

namespace QS.GUI.Components
{
    partial class ExperimentController
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.controllerPanel = new System.Windows.Forms.Panel();
            this.experimentPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.experimentCfg11 = new QS.GUI.Components.ExperimentCfg1();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectSelector = new QS.GUI.Components.ObjectSelector2();
            this.objectVisualizer = new QS.GUI.Components.ObjectVisualizer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.selectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshRecursivelyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.experimentPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.statusStrip1.Location = new System.Drawing.Point(0, 549);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(569, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Text = "Ready.";
            // 
            // controllerPanel
            // 
            this.controllerPanel.AutoSize = true;
            this.controllerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controllerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.controllerPanel.Location = new System.Drawing.Point(0, 24);
            this.controllerPanel.Name = "controllerPanel";
            this.controllerPanel.Size = new System.Drawing.Size(569, 0);
            this.controllerPanel.TabIndex = 2;
            // 
            // experimentPanel
            // 
            this.experimentPanel.Controls.Add(this.panel2);
            this.experimentPanel.Controls.Add(this.panel1);
            this.experimentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.experimentPanel.Location = new System.Drawing.Point(0, 24);
            this.experimentPanel.Name = "experimentPanel";
            this.experimentPanel.Size = new System.Drawing.Size(569, 22);
            this.experimentPanel.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.experimentCfg11);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(22, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(547, 22);
            this.panel2.TabIndex = 2;
            // 
            // experimentCfg11
            // 
            this.experimentCfg11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.experimentCfg11.Location = new System.Drawing.Point(0, 0);
            this.experimentCfg11.Name = "experimentCfg11";
            this.experimentCfg11.Size = new System.Drawing.Size(547, 22);
            this.experimentCfg11.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(22, 22);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 46);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectSelector);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.objectVisualizer);
            this.splitContainer1.Size = new System.Drawing.Size(569, 503);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 4;
            this.splitContainer1.Text = "splitContainer1";
            this.splitContainer1.Resize += new System.EventHandler(this.splitContainer1_Resize);
            // 
            // objectSelector
            // 
            this.objectSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectSelector.Location = new System.Drawing.Point(0, 0);
            this.objectSelector.Name = "objectSelector";
            this.objectSelector.Size = new System.Drawing.Size(569, 232);
            this.objectSelector.TabIndex = 0;
            // 
            // objectVisualizer
            // 
            this.objectVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectVisualizer.Location = new System.Drawing.Point(0, 0);
            this.objectVisualizer.Name = "objectVisualizer";
            this.objectVisualizer.Size = new System.Drawing.Size(569, 267);
            this.objectVisualizer.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(569, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // selectionToolStripMenuItem
            // 
            this.selectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshRecursivelyToolStripMenuItem,
            this.expandAllToolStripMenuItem,
            this.refreshSelectedToolStripMenuItem,
            this.unrollToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            this.selectionToolStripMenuItem.Text = "Selection";
            // 
            // refreshRecursivelyToolStripMenuItem
            // 
            this.refreshRecursivelyToolStripMenuItem.Name = "refreshRecursivelyToolStripMenuItem";
            this.refreshRecursivelyToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.refreshRecursivelyToolStripMenuItem.Text = "Refresh Recursively";
            this.refreshRecursivelyToolStripMenuItem.Click += new System.EventHandler(this.refreshRecursivelyToolStripMenuItem_Click);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // refreshSelectedToolStripMenuItem
            // 
            this.refreshSelectedToolStripMenuItem.Enabled = false;
            this.refreshSelectedToolStripMenuItem.Name = "refreshSelectedToolStripMenuItem";
            this.refreshSelectedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.refreshSelectedToolStripMenuItem.Text = "Refresh Selected";
            this.refreshSelectedToolStripMenuItem.Click += new System.EventHandler(this.refreshSelectedToolStripMenuItem_Click);
            // 
            // unrollToolStripMenuItem
            // 
            this.unrollToolStripMenuItem.Name = "unrollToolStripMenuItem";
            this.unrollToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.unrollToolStripMenuItem.Text = "Unroll";
            this.unrollToolStripMenuItem.Click += new System.EventHandler(this.unrollToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.Image = QS.GUI.Properties.Resources.PLAYP16;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 22);
            this.button1.TabIndex = 0;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ExperimentController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.experimentPanel);
            this.Controls.Add(this.controllerPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ExperimentController";
            this.Size = new System.Drawing.Size(569, 571);
            this.statusStrip1.ResumeLayout(false);
            this.experimentPanel.ResumeLayout(false);
            this.experimentPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel controllerPanel;
        private System.Windows.Forms.Panel experimentPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private ExperimentCfg1 experimentCfg11;
        private ObjectSelector2 objectSelector;
        private ObjectVisualizer objectVisualizer;
        // private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unrollToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshRecursivelyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshSelectedToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}
