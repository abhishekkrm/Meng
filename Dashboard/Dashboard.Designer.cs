namespace Dashboard
{
    partial class Dashboard
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
        private void InitializeBusInfoPanel()
        {
            this.busInfoPanel = new System.Windows.Forms.Panel();
            this.busNameValue = new System.Windows.Forms.Label();
            this.busIdValue = new System.Windows.Forms.Label();
            this.busTypeValue = new System.Windows.Forms.Label();
            this.busTypeLabel = new System.Windows.Forms.Label();
            this.busNameLabel = new System.Windows.Forms.Label();
            this.busIdLabel = new System.Windows.Forms.Label();
            this.busAreaNumberValue = new System.Windows.Forms.Label();
            this.busAreaNumberLabel = new System.Windows.Forms.Label();
            this.busVoltageValue = new System.Windows.Forms.Label();
            this.busVoltageLabel = new System.Windows.Forms.Label();
            this.busBaseKiloVoltageValue = new System.Windows.Forms.Label();
            this.baseKVLabel = new System.Windows.Forms.Label();
            this.busPhaseAngleValue = new System.Windows.Forms.Label();
            this.busPhaseAngleLabel = new System.Windows.Forms.Label();
            this.busInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // busInfoPanel
            // 
            this.busInfoPanel.BackColor = System.Drawing.Color.Black;
            this.busInfoPanel.Controls.Add(this.busPhaseAngleValue);
            this.busInfoPanel.Controls.Add(this.busPhaseAngleLabel);
            this.busInfoPanel.Controls.Add(this.busBaseKiloVoltageValue);
            this.busInfoPanel.Controls.Add(this.baseKVLabel);
            this.busInfoPanel.Controls.Add(this.busVoltageValue);
            this.busInfoPanel.Controls.Add(this.busVoltageLabel);
            this.busInfoPanel.Controls.Add(this.busAreaNumberValue);
            this.busInfoPanel.Controls.Add(this.busAreaNumberLabel);
            this.busInfoPanel.Controls.Add(this.busNameValue);
            this.busInfoPanel.Controls.Add(this.busIdValue);
            this.busInfoPanel.Controls.Add(this.busTypeValue);
            this.busInfoPanel.Controls.Add(this.busTypeLabel);
            this.busInfoPanel.Controls.Add(this.busNameLabel);
            this.busInfoPanel.Controls.Add(this.busIdLabel);
            this.busInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.busInfoPanel.Name = "busInfoPanel";
            //this.busInfoPanel.Size = new System.Drawing.Size(300 , 235);
            this.busInfoPanel.TabIndex = 0;
            this.busInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.busInfoPanel.Visible = false;
            this.busInfoPanel.AutoScroll = true;
            //this.busInfoPanel.Click += new System.EventHandler(this.Panel_Click);
            
            // Font and Color
            System.Drawing.Font textFont = new System.Drawing.Font("Arial", 9);
            System.Drawing.Color color = System.Drawing.Color.Green;

            // 
            // busNameValue
            //         
            this.busNameValue.AutoSize = true;
            this.busNameValue.Location = new System.Drawing.Point(165, 70);
            this.busNameValue.Name = "busNameValue";
            this.busNameValue.ForeColor = color;
            this.busNameValue.Size = new System.Drawing.Size(13, 13);
            this.busNameValue.Font = textFont;
            this.busNameValue.TabIndex = 5;
            this.busNameValue.Text = "0";
            // 
            // busIdValue
            // 
            this.busIdValue.AutoSize = true;
            this.busIdValue.Location = new System.Drawing.Point(165, 38);
            this.busIdValue.Name = "busIdValue";
            this.busIdValue.ForeColor = color;
            this.busIdValue.Font = textFont;
            this.busIdValue.Size = new System.Drawing.Size(13, 13);
            this.busIdValue.TabIndex = 4;
            this.busIdValue.Text = "0";
            // 
            // busTypeValue
            // 
            this.busTypeValue.AutoSize = true;
            this.busTypeValue.Location = new System.Drawing.Point(165, 100);
            this.busTypeValue.Name = "busTypeValue";
            this.busTypeValue.ForeColor = color;
            this.busTypeValue.Font = textFont;
            this.busTypeValue.Size = new System.Drawing.Size(13, 13);
            this.busTypeValue.TabIndex = 3;
            this.busTypeValue.Text = "0";
            // 
            // busTypeLabel
            // 
            this.busTypeLabel.AutoSize = true;
            this.busTypeLabel.Location = new System.Drawing.Point(49, 100);
            this.busTypeLabel.Name = "busTypeLabel";
            this.busTypeLabel.Font = textFont;
            this.busTypeLabel.ForeColor = color;
            this.busTypeLabel.Size = new System.Drawing.Size(55, 13);
            this.busTypeLabel.TabIndex = 2;
            this.busTypeLabel.Text = "Bus Type:";
            // 
            // busNameLabel
            // 
            this.busNameLabel.AutoSize = true;
            this.busNameLabel.Location = new System.Drawing.Point(49, 70);
            this.busNameLabel.Name = "busNameLabel";
            this.busNameLabel.Font = textFont;
            this.busNameLabel.ForeColor = color;
            this.busNameLabel.Size = new System.Drawing.Size(59, 13);
            this.busNameLabel.TabIndex = 1;
            this.busNameLabel.Text = "Bus Name:";
            // 
            // busIdLabel
            // 
            this.busIdLabel.AutoSize = true;
            this.busIdLabel.Location = new System.Drawing.Point(49, 38);
            this.busIdLabel.Name = "busIdLabel";
            this.busIdLabel.ForeColor = color;
            this.busIdLabel.Font = textFont;
            this.busIdLabel.Size = new System.Drawing.Size(40, 13);
            this.busIdLabel.TabIndex = 0;
            this.busIdLabel.Text = "Bus Id:";
            // 
            // busAreaNumberValue
            // 
            this.busAreaNumberValue.AutoSize = true;
            this.busAreaNumberValue.Location = new System.Drawing.Point(165, 129);
            this.busAreaNumberValue.Name = "busAreaNumberValue";
            this.busAreaNumberValue.Font = textFont;
            this.busAreaNumberValue.ForeColor = color;
            this.busAreaNumberValue.Size = new System.Drawing.Size(13, 13);
            this.busAreaNumberValue.TabIndex = 7;
            this.busAreaNumberValue.Text = "0";
            // 
            // busAreaNumberLabel
            // 
            this.busAreaNumberLabel.AutoSize = true;
            this.busAreaNumberLabel.Location = new System.Drawing.Point(49, 129);
            this.busAreaNumberLabel.Name = "busAreaNumberLabel";
            this.busAreaNumberLabel.Font = textFont;
            this.busAreaNumberLabel.ForeColor = color;
            this.busAreaNumberLabel.Size = new System.Drawing.Size(93, 13);
            this.busAreaNumberLabel.TabIndex = 6;
            this.busAreaNumberLabel.Text = "Bus Area Number:";
            // 
            // busVoltageValue
            // 
            this.busVoltageValue.AutoSize = true;
            this.busVoltageValue.Location = new System.Drawing.Point(165, 151);
            this.busVoltageValue.Name = "busVoltageValue";
            this.busVoltageValue.Font = textFont;
            this.busVoltageValue.ForeColor = color;
            this.busVoltageValue.Size = new System.Drawing.Size(13, 13);
            this.busVoltageValue.TabIndex = 9;
            this.busVoltageValue.Text = "0";
            // 
            // busVoltageLabel
            // 
            this.busVoltageLabel.AutoSize = true;
            this.busVoltageLabel.Location = new System.Drawing.Point(49, 151);
            this.busVoltageLabel.Name = "busVoltageLabel";
            this.busVoltageLabel.Font = textFont;
            this.busVoltageLabel.ForeColor = color;
            this.busVoltageLabel.Size = new System.Drawing.Size(70, 13);
            this.busVoltageLabel.TabIndex = 8;
            this.busVoltageLabel.Text = "Bus Voltage :";
            // 
            // busBaseKiloVoltageValue
            // 
            this.busBaseKiloVoltageValue.AutoSize = true;
            this.busBaseKiloVoltageValue.Location = new System.Drawing.Point(165, 177);
            this.busBaseKiloVoltageValue.Name = "busBaseKiloVoltageValue";
            this.busBaseKiloVoltageValue.Font = textFont;
            this.busBaseKiloVoltageValue.ForeColor = color;
            this.busBaseKiloVoltageValue.Size = new System.Drawing.Size(13, 13);
            this.busBaseKiloVoltageValue.TabIndex = 11;
            this.busBaseKiloVoltageValue.Text = "0";
            // 
            // baseKVLabel
            // 
            this.baseKVLabel.AutoSize = true;
            this.baseKVLabel.Location = new System.Drawing.Point(49, 177);
            this.baseKVLabel.Name = "baseKVLabel";
            this.baseKVLabel.Font = textFont;
            this.baseKVLabel.ForeColor = color;
            this.baseKVLabel.Size = new System.Drawing.Size(50, 13);
            this.baseKVLabel.TabIndex = 10;
            this.baseKVLabel.Text = "Base kV:";
            // 
            // busPhaseAngleValue
            // 
            this.busPhaseAngleValue.AutoSize = true;
            this.busPhaseAngleValue.Location = new System.Drawing.Point(165, 200);
            this.busPhaseAngleValue.Name = "busPhaseAngleValue";
            this.busPhaseAngleValue.Font = textFont;
            this.busPhaseAngleValue.ForeColor = color;
            this.busPhaseAngleValue.Size = new System.Drawing.Size(13, 13);
            this.busPhaseAngleValue.TabIndex = 13;
            this.busPhaseAngleValue.Text = "0";
            // 
            // busPhaseAngleLabel
            // 
            this.busPhaseAngleLabel.AutoSize = true;
            this.busPhaseAngleLabel.Location = new System.Drawing.Point(49, 200);
            this.busPhaseAngleLabel.Name = "busPhaseAngleLabel";
            this.busPhaseAngleLabel.Font = textFont;
            this.busPhaseAngleLabel.ForeColor = color;
            this.busPhaseAngleLabel.Size = new System.Drawing.Size(91, 13);
            this.busPhaseAngleLabel.TabIndex = 12;
            this.busPhaseAngleLabel.Text = "Bus Phase Angle:";
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 377);
            this.Controls.Add(this.busInfoPanel);
            this.Name = "Dashboard";
            this.Text = "Dashboard";
            this.busInfoPanel.ResumeLayout(false);
            this.busInfoPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel busInfoPanel;
        private System.Windows.Forms.Label busIdLabel;
        private System.Windows.Forms.Label busTypeLabel;
        private System.Windows.Forms.Label busNameLabel;
        private System.Windows.Forms.Label busNameValue;
        private System.Windows.Forms.Label busIdValue;
        private System.Windows.Forms.Label busTypeValue;
        private System.Windows.Forms.Label busPhaseAngleValue;
        private System.Windows.Forms.Label busPhaseAngleLabel;
        private System.Windows.Forms.Label busBaseKiloVoltageValue;
        private System.Windows.Forms.Label baseKVLabel;
        private System.Windows.Forms.Label busVoltageValue;
        private System.Windows.Forms.Label busVoltageLabel;
        private System.Windows.Forms.Label busAreaNumberValue;
        private System.Windows.Forms.Label busAreaNumberLabel;
    }
}

