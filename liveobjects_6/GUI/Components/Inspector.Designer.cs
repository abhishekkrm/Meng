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
    partial class Inspector
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectSelector = new QS.GUI.Components.ObjectSelector2();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.selectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshResursivelyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectVisualizer = new QS.GUI.Components.ObjectVisualizer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectSelector);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.objectVisualizer);
            this.splitContainer1.Size = new System.Drawing.Size(651, 497);
            this.splitContainer1.SplitterDistance = 321;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.Text = "splitContainer1";
            this.splitContainer1.Resize += new System.EventHandler(this.splitContainer1_Resize);
            // 
            // objectSelector
            // 
            this.objectSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectSelector.Location = new System.Drawing.Point(0, 24);
            this.objectSelector.Name = "objectSelector";
            this.objectSelector.Size = new System.Drawing.Size(321, 473);
            this.objectSelector.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(321, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // selectionToolStripMenuItem
            // 
            this.selectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshResursivelyToolStripMenuItem,
            this.expandAllToolStripMenuItem,
            this.refreshSelectedToolStripMenuItem,
            this.unrollToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            this.selectionToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.selectionToolStripMenuItem.Text = "Selection";
            // 
            // refreshResursivelyToolStripMenuItem
            // 
            this.refreshResursivelyToolStripMenuItem.Name = "refreshResursivelyToolStripMenuItem";
            this.refreshResursivelyToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.refreshResursivelyToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.refreshResursivelyToolStripMenuItem.Text = "Refresh Resursively";
            this.refreshResursivelyToolStripMenuItem.Click += new System.EventHandler(this.refreshResursivelyToolStripMenuItem_Click);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // refreshSelectedToolStripMenuItem
            // 
            this.refreshSelectedToolStripMenuItem.Enabled = false;
            this.refreshSelectedToolStripMenuItem.Name = "refreshSelectedToolStripMenuItem";
            this.refreshSelectedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.refreshSelectedToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.refreshSelectedToolStripMenuItem.Text = "Refresh Selected";
            // 
            // unrollToolStripMenuItem
            // 
            this.unrollToolStripMenuItem.Name = "unrollToolStripMenuItem";
            this.unrollToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.unrollToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.unrollToolStripMenuItem.Text = "Unroll";
            this.unrollToolStripMenuItem.Click += new System.EventHandler(this.unrollToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // objectVisualizer
            // 
            this.objectVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectVisualizer.Location = new System.Drawing.Point(0, 0);
            this.objectVisualizer.Name = "objectVisualizer";
            this.objectVisualizer.Size = new System.Drawing.Size(326, 497);
            this.objectVisualizer.TabIndex = 0;
            // 
            // Inspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Inspector";
            this.Size = new System.Drawing.Size(651, 497);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ObjectVisualizer objectVisualizer;
        private ObjectSelector2 objectSelector;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshResursivelyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unrollToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}
