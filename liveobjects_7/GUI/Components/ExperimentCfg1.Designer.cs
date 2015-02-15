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
    partial class ExperimentCfg1
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.experimentSelector1 = new QS.GUI.Components.ExperimentSelector(this.components);
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            typeof(QS._qss_e_.Experiments_.Experiment),
            typeof(QS._qss_e_.Experiments_.Experiment),
            typeof(QS._qss_e_.Experiments_.Experiment_000),
            typeof(QS._qss_e_.Experiments_.Experiment_000),
            typeof(QS._qss_e_.Experiments_.Experiment_001),
            typeof(QS._qss_e_.Experiments_.Experiment_001),
            typeof(QS._qss_e_.Experiments_.Experiment_006),
            typeof(QS._qss_e_.Experiments_.Experiment_006),
            typeof(QS._qss_e_.Experiments_.Experiment_008),
            typeof(QS._qss_e_.Experiments_.Experiment_008),
            typeof(QS._qss_e_.Experiments_.Experiment_100),
            typeof(QS._qss_e_.Experiments_.Experiment_100),
            typeof(QS._qss_e_.Experiments_.Experiment_103),
            typeof(QS._qss_e_.Experiments_.Experiment_103),
            typeof(QS._qss_e_.Experiments_.Experiment_200),
            typeof(QS._qss_e_.Experiments_.Experiment_200),
            typeof(QS._qss_e_.Experiments_.Experiment_201),
            typeof(QS._qss_e_.Experiments_.Experiment_201),
            typeof(QS._qss_e_.Experiments_.Experiment_202),
            typeof(QS._qss_e_.Experiments_.Experiment_202),
            typeof(QS._qss_e_.Experiments_.Experiment_203),
            typeof(QS._qss_e_.Experiments_.Experiment_203),
            typeof(QS._qss_e_.Experiments_.Experiment_204),
            typeof(QS._qss_e_.Experiments_.Experiment_204),
            typeof(QS._qss_e_.Experiments_.Experiment_250),
            typeof(QS._qss_e_.Experiments_.Experiment_250),
            typeof(QS._qss_e_.Experiments_.Experiment_251),
            typeof(QS._qss_e_.Experiments_.Experiment_251),
            typeof(QS._qss_e_.Experiments_.Experiment_252),
            typeof(QS._qss_e_.Experiments_.Experiment_252),
            typeof(QS._qss_e_.Experiments_.Experiment_253),
            typeof(QS._qss_e_.Experiments_.Experiment_253),
            typeof(QS._qss_e_.Experiments_.Experiment_254),
            typeof(QS._qss_e_.Experiments_.Experiment_254),
            typeof(QS._qss_e_.Experiments_.Experiment_255),
            typeof(QS._qss_e_.Experiments_.Experiment_256),
            typeof(QS._qss_e_.Experiments_.Experiment_257),
            typeof(QS._qss_e_.Experiments_.Experiment_259),
            typeof(QS._qss_e_.Experiments_.Experiment_260)});
            this.comboBox1.Location = new System.Drawing.Point(0, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(244, 21);
            this.comboBox1.Sorted = true;
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(244, 0);
            this.richTextBox1.Multiline = false;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(508, 21);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // experimentSelector1
            // 
            this.experimentSelector1.ComboBox = this.comboBox1;
            // 
            // ExperimentCfg1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.comboBox1);
            this.Name = "ExperimentCfg1";
            this.Size = new System.Drawing.Size(752, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private ExperimentSelector experimentSelector1;
    }
}
