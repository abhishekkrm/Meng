/*

Copyright (c) 2004-2009 Colin Barth. All rights reserved.

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
namespace liveobjects_8.Components_
{
    partial class ChannelTester
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.outMsg = new System.Windows.Forms.TextBox();
            this.inMsg = new System.Windows.Forms.TextBox();
            this.outCPLabel = new System.Windows.Forms.Label();
            this.outCP = new System.Windows.Forms.TextBox();
            this.inCP = new System.Windows.Forms.TextBox();
            this.inCPLabel = new System.Windows.Forms.Label();
            this.errors = new System.Windows.Forms.TextBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Outgoing";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(325, 227);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Incoming";
            // 
            // outMsg
            // 
            this.outMsg.Location = new System.Drawing.Point(3, 250);
            this.outMsg.Multiline = true;
            this.outMsg.Name = "outMsg";
            this.outMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outMsg.Size = new System.Drawing.Size(322, 230);
            this.outMsg.TabIndex = 2;
            this.outMsg.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // inMsg
            // 
            this.inMsg.Location = new System.Drawing.Point(329, 250);
            this.inMsg.Multiline = true;
            this.inMsg.Name = "inMsg";
            this.inMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.inMsg.Size = new System.Drawing.Size(325, 230);
            this.inMsg.TabIndex = 3;
            // 
            // outCPLabel
            // 
            this.outCPLabel.AutoSize = true;
            this.outCPLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outCPLabel.Location = new System.Drawing.Point(-4, 123);
            this.outCPLabel.Name = "outCPLabel";
            this.outCPLabel.Size = new System.Drawing.Size(186, 20);
            this.outCPLabel.TabIndex = 4;
            this.outCPLabel.Text = "Outgoing Checkpoints";
            this.outCPLabel.Click += new System.EventHandler(this.label3_Click);
            // 
            // outCP
            // 
            this.outCP.Location = new System.Drawing.Point(0, 146);
            this.outCP.Multiline = true;
            this.outCP.Name = "outCP";
            this.outCP.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outCP.Size = new System.Drawing.Size(325, 78);
            this.outCP.TabIndex = 5;
            // 
            // inCP
            // 
            this.inCP.Location = new System.Drawing.Point(329, 146);
            this.inCP.Multiline = true;
            this.inCP.Name = "inCP";
            this.inCP.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.inCP.Size = new System.Drawing.Size(325, 78);
            this.inCP.TabIndex = 6;
            // 
            // inCPLabel
            // 
            this.inCPLabel.AutoSize = true;
            this.inCPLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inCPLabel.Location = new System.Drawing.Point(325, 123);
            this.inCPLabel.Name = "inCPLabel";
            this.inCPLabel.Size = new System.Drawing.Size(186, 20);
            this.inCPLabel.TabIndex = 7;
            this.inCPLabel.Text = "Incoming Checkpoints";
            // 
            // errors
            // 
            this.errors.Location = new System.Drawing.Point(0, 20);
            this.errors.Multiline = true;
            this.errors.Name = "errors";
            this.errors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errors.Size = new System.Drawing.Size(655, 100);
            this.errors.TabIndex = 8;
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorLabel.Location = new System.Drawing.Point(-4, 0);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(58, 20);
            this.errorLabel.TabIndex = 9;
            this.errorLabel.Text = "Errors";
            // 
            // ChannelTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.errors);
            this.Controls.Add(this.inCPLabel);
            this.Controls.Add(this.inCP);
            this.Controls.Add(this.outCP);
            this.Controls.Add(this.outCPLabel);
            this.Controls.Add(this.inMsg);
            this.Controls.Add(this.outMsg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ChannelTester";
            this.Size = new System.Drawing.Size(679, 560);
            this.Load += new System.EventHandler(this.ChannelTesterUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox outMsg;
        private System.Windows.Forms.TextBox inMsg;
        private System.Windows.Forms.Label outCPLabel;
        private System.Windows.Forms.TextBox outCP;
        private System.Windows.Forms.TextBox inCP;
        private System.Windows.Forms.Label inCPLabel;
        private System.Windows.Forms.TextBox errors;
        private System.Windows.Forms.Label errorLabel;
    }
}
