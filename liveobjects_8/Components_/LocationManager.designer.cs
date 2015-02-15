/*

Copyright 2009, Jared Cantwell. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions 
   and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of 
   conditions and the following disclaimer in the documentation and/or other materials provided
  with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S) AND ALL OTHER CONTRIBUTORS 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE ABOVE 
COPYRIGHT HOLDER(S) OR ANY OTHER CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE. 
 
*/

#if RELEASE3

namespace Demo
{
#if XNA
    partial class LocationJumper
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
            this.latitude = new System.Windows.Forms.TextBox();
            this.longitude = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.latLongButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.x = new System.Windows.Forms.Label();
            this.y = new System.Windows.Forms.Label();
            this.z = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.altitude = new System.Windows.Forms.TextBox();
            this.geocodeLoc = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.locButton = new System.Windows.Forms.Button();
            this.notes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // latitude
            // 
            this.latitude.Location = new System.Drawing.Point(86, 9);
            this.latitude.Name = "latitude";
            this.latitude.Size = new System.Drawing.Size(100, 20);
            this.latitude.TabIndex = 0;
            // 
            // longitude
            // 
            this.longitude.Location = new System.Drawing.Point(86, 35);
            this.longitude.Name = "longitude";
            this.longitude.Size = new System.Drawing.Size(100, 20);
            this.longitude.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Latitude";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Longitude";
            // 
            // latLongButton
            // 
            this.latLongButton.Location = new System.Drawing.Point(71, 85);
            this.latLongButton.Name = "latLongButton";
            this.latLongButton.Size = new System.Drawing.Size(75, 23);
            this.latLongButton.TabIndex = 4;
            this.latLongButton.Text = "GO!";
            this.latLongButton.UseVisualStyleBackColor = true;
            this.latLongButton.Click += new System.EventHandler(this.latLongButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(192, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "X: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(191, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Y:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Z:";
            // 
            // x
            // 
            this.x.AutoSize = true;
            this.x.Location = new System.Drawing.Point(208, 39);
            this.x.Name = "x";
            this.x.Size = new System.Drawing.Size(0, 13);
            this.x.TabIndex = 8;
            // 
            // y
            // 
            this.y.AutoSize = true;
            this.y.Location = new System.Drawing.Point(207, 13);
            this.y.Name = "y";
            this.y.Size = new System.Drawing.Size(0, 13);
            this.y.TabIndex = 9;
            // 
            // z
            // 
            this.z.AutoSize = true;
            this.z.Location = new System.Drawing.Point(207, 62);
            this.z.Name = "z";
            this.z.Size = new System.Drawing.Size(0, 13);
            this.z.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Altitude";
            // 
            // altitude
            // 
            this.altitude.Location = new System.Drawing.Point(86, 59);
            this.altitude.Name = "altitude";
            this.altitude.Size = new System.Drawing.Size(100, 20);
            this.altitude.TabIndex = 12;
            // 
            // geocodeLoc
            // 
            this.geocodeLoc.Location = new System.Drawing.Point(21, 163);
            this.geocodeLoc.Name = "geocodeLoc";
            this.geocodeLoc.Size = new System.Drawing.Size(186, 20);
            this.geocodeLoc.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Location:";
            // 
            // locButton
            // 
            this.locButton.Location = new System.Drawing.Point(71, 189);
            this.locButton.Name = "locButton";
            this.locButton.Size = new System.Drawing.Size(75, 23);
            this.locButton.TabIndex = 15;
            this.locButton.Text = "GO!";
            this.locButton.UseVisualStyleBackColor = true;
            this.locButton.Click += new System.EventHandler(this.locButton_Click);
            // 
            // notes
            // 
            this.notes.Location = new System.Drawing.Point(21, 218);
            this.notes.Multiline = true;
            this.notes.Name = "notes";
            this.notes.Size = new System.Drawing.Size(186, 36);
            this.notes.TabIndex = 16;
            // 
            // LocationJumper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.notes);
            this.Controls.Add(this.locButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.geocodeLoc);
            this.Controls.Add(this.altitude);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.z);
            this.Controls.Add(this.y);
            this.Controls.Add(this.x);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.latLongButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.longitude);
            this.Controls.Add(this.latitude);
            this.Name = "LocationJumper";
            this.Size = new System.Drawing.Size(221, 275);
            this.Load += new System.EventHandler(this.LocationJumper_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox latitude;
        private System.Windows.Forms.TextBox longitude;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button latLongButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label x;
        private System.Windows.Forms.Label y;
        private System.Windows.Forms.Label z;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox altitude;
        private System.Windows.Forms.TextBox geocodeLoc;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button locButton;
        private System.Windows.Forms.TextBox notes;
    }

#endif
}

#endif