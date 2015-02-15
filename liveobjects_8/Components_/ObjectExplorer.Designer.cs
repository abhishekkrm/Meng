/* Copyright (c) 2004-2009, 
 * Revant Kapoor (rk368@cornell.edu),
 * Yilin Qin (yq33@cornell.edu),
 * Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
    documentation and/or other materials provided with the distribution.
3. Neither the name of the Cornell University nor the names of its contributors may be used to endorse or promote products derived from 
    this software without specific prior written permission.

This software is provided by Krzysztof Ostrowski ''as is'' and any express or implied warranties, including, but not limited to, the implied
warranties of merchantability and fitness for a particular purpose are disclaimed. in no event shall krzysztof ostrowski be liable for any direct, 
indirect, incidental, special, exemplary, or consequential damages (including, but not limited to, procurement of substitute goods or services;
loss of use, data, or profits; or business interruption) however caused and on any theory of liability, whether in contract, strict liability, or tort
(including negligence or otherwise) arising in any way out of the use of this software, even if advised of the possibility of such damage. */

namespace QS._qss_x_.ObjectExplorer_
{
    partial class ObjectExplorer
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(199, 453);
            this.treeView1.TabIndex = 1;
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.OntreeView1DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnTreeView1DragEnter);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeView1NodeMouseClick);
            // 
            // treeView2
            // 
            this.treeView2.AllowDrop = true;
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.LabelEdit = true;
            this.treeView2.Location = new System.Drawing.Point(199, 0);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(487, 453);
            this.treeView2.TabIndex = 2;
            this.treeView2.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeView2NodeMouseDoubleClick);
            this.treeView2.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnTreeView2AfterLabelEdit);
            this.treeView2.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeView2NodeMouseClick);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(199, 0);
            this.splitter1.MinExtra = 5;
            this.splitter1.MinSize = 5;
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 453);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // ObjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.treeView1);
            this.Name = "ObjectExplorer";
            this.Size = new System.Drawing.Size(686, 453);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.DoubleClick += new System.EventHandler(this.OnControl1DoubleClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Splitter splitter1;



        //internal System.Windows.Forms.Splitter Splitter1;
        //internal System.Windows.Forms.TreeView treeOne;
        //private System.Windows.Forms.TextBox textBox1;
            //private System.Windows.Forms.Splitter splitter1;
            
    }
}
