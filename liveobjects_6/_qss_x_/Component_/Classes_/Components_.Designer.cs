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

namespace QS._qss_x_.Component_.Classes_
{
    partial class Components_
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Components_));
            this._my_treeview = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // _my_treeview
            // 
            this._my_treeview.AllowDrop = true;
            this._my_treeview.Dock = System.Windows.Forms.DockStyle.Fill;
            this._my_treeview.FullRowSelect = true;
            this._my_treeview.ImageIndex = 0;
            this._my_treeview.ImageList = this.imageList1;
            this._my_treeview.Location = new System.Drawing.Point(0, 0);
            this._my_treeview.Name = "_my_treeview";
            this._my_treeview.SelectedImageIndex = 0;
            this._my_treeview.ShowNodeToolTips = true;
            this._my_treeview.Size = new System.Drawing.Size(367, 684);
            this._my_treeview.TabIndex = 0;
            this._my_treeview.DoubleClick += new System.EventHandler(this._my_treeview_DoubleClick);
            this._my_treeview.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this._my_treeview_ItemDrag);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "new.ico");
            this.imageList1.Images.SetKeyName(1, "props.ico");
            this.imageList1.Images.SetKeyName(2, "cfold16.ico");
            this.imageList1.Images.SetKeyName(3, "ofold16.ico");
            this.imageList1.Images.SetKeyName(4, "addrbook.ico");
            this.imageList1.Images.SetKeyName(5, "globe.ico");
            this.imageList1.Images.SetKeyName(6, "3.ico");
            this.imageList1.Images.SetKeyName(7, "WRENCH.ICO");
            this.imageList1.Images.SetKeyName(8, "mail.ico");
            this.imageList1.Images.SetKeyName(9, "table.ico");
            this.imageList1.Images.SetKeyName(10, "shell32_dll_Ico1084_ico_Ico1.ico");
            // 
            // Components_
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._my_treeview);
            this.Name = "Components_";
            this.Size = new System.Drawing.Size(367, 684);
            this.Load += new System.EventHandler(this.@__on_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _my_treeview;
        private System.Windows.Forms.ImageList imageList1;
    }
}
