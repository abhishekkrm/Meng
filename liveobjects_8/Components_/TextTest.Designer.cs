/*

Copyright (c) 2009 Revant Kapoor (rk368@cornell.edu), Yilin Qin (yq33@cornell.edu), Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
    
    partial class TextTest
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
            this._textbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _textbox
            // 
            this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._textbox.Location = new System.Drawing.Point(0, 0);
            this._textbox.Multiline = true;
            this._textbox.Name = "_textbox";
            this._textbox.Size = new System.Drawing.Size(150, 150);
            this._textbox.TabIndex = 0;
            this._textbox.MouseUp += new System.Windows.Forms.MouseEventHandler(this._textbox_MouseUp);
            this._textbox.SizeChanged += new System.EventHandler(this._SizeChanged);
            this._textbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this._textbox_MouseDown);
            this._textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textbox_KeyPress);

            // 
            // TextTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._textbox);
            this.Name = "TextTest";            
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this._DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this._DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();


        }

        #endregion

        private System.Windows.Forms.TextBox _textbox;
    }
     
}
