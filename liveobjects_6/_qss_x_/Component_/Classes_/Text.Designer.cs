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
    public partial class Text
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
            this._textbox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // _textbox
            // 
            this._textbox.BackColor = System.Drawing.SystemColors.Window;
            this._textbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._textbox.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._textbox.Location = new System.Drawing.Point(0, 0);
            this._textbox.Name = "_textbox";
            this._textbox.ReadOnly = true;
            this._textbox.Size = new System.Drawing.Size(150, 150);
            this._textbox.TabIndex = 0;
            this._textbox.Text = "";
            this._textbox.MouseUp += new System.Windows.Forms.MouseEventHandler(this._textbox_MouseUp);
            this._textbox.SizeChanged += new System.EventHandler(this._SizeChanged);
            this._textbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this._textbox_MouseDown);
            this._textbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textbox_KeyPress);
            // 
            // Text
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._textbox);
            this.Name = "Text";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this._DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this._DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _textbox;
    }
}
