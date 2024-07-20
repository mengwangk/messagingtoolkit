namespace MessagingToolkit.BulkGateway
{
    partial class frmGatewayDetails
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGatewayDetails));
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtManufacturer = new System.Windows.Forms.TextBox();
            this.txtFirmware = new System.Windows.Forms.TextBox();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.txtImsi = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtOutgoingMessage = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.txtGatewayStatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBarBatteryLevel = new System.Windows.Forms.ProgressBar();
            this.progressBarSignalQuality = new System.Windows.Forms.ProgressBar();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.ssApplicationStatus = new System.Windows.Forms.StatusStrip();
            this.toolStripGateway = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnApplyPrefixes = new System.Windows.Forms.Button();
            this.txtPrefixes = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.ssApplicationStatus.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(110, 216);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(142, 31);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(201, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(141, 31);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtManufacturer);
            this.groupBox3.Controls.Add(this.txtFirmware);
            this.groupBox3.Controls.Add(this.txtSerialNo);
            this.groupBox3.Controls.Add(this.txtImsi);
            this.groupBox3.Controls.Add(this.txtModel);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(12, 11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(285, 199);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Device Information";
            // 
            // txtManufacturer
            // 
            this.txtManufacturer.Enabled = false;
            this.txtManufacturer.Location = new System.Drawing.Point(98, 52);
            this.txtManufacturer.Name = "txtManufacturer";
            this.txtManufacturer.Size = new System.Drawing.Size(171, 20);
            this.txtManufacturer.TabIndex = 10;
            // 
            // txtFirmware
            // 
            this.txtFirmware.Enabled = false;
            this.txtFirmware.Location = new System.Drawing.Point(98, 76);
            this.txtFirmware.Multiline = true;
            this.txtFirmware.Name = "txtFirmware";
            this.txtFirmware.Size = new System.Drawing.Size(171, 53);
            this.txtFirmware.TabIndex = 9;
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Enabled = false;
            this.txtSerialNo.Location = new System.Drawing.Point(98, 135);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(171, 20);
            this.txtSerialNo.TabIndex = 8;
            // 
            // txtImsi
            // 
            this.txtImsi.Enabled = false;
            this.txtImsi.Location = new System.Drawing.Point(98, 159);
            this.txtImsi.Name = "txtImsi";
            this.txtImsi.Size = new System.Drawing.Size(171, 20);
            this.txtImsi.TabIndex = 7;
            // 
            // txtModel
            // 
            this.txtModel.Enabled = false;
            this.txtModel.Location = new System.Drawing.Point(98, 26);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(171, 20);
            this.txtModel.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 162);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "IMSI";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 138);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Serial No";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 79);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Firmware";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Manufacturer";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Model";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtOutgoingMessage);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Location = new System.Drawing.Point(303, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(243, 65);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Gateway Statistics";
            // 
            // txtOutgoingMessage
            // 
            this.txtOutgoingMessage.Enabled = false;
            this.txtOutgoingMessage.Location = new System.Drawing.Point(121, 30);
            this.txtOutgoingMessage.Name = "txtOutgoingMessage";
            this.txtOutgoingMessage.Size = new System.Drawing.Size(102, 20);
            this.txtOutgoingMessage.TabIndex = 8;
            this.txtOutgoingMessage.Text = "0";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 33);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(96, 13);
            this.label23.TabIndex = 3;
            this.label23.Text = "Outgoing Message";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.txtGatewayStatus);
            this.groupBox10.Controls.Add(this.label1);
            this.groupBox10.Controls.Add(this.progressBarBatteryLevel);
            this.groupBox10.Controls.Add(this.progressBarSignalQuality);
            this.groupBox10.Controls.Add(this.label28);
            this.groupBox10.Controls.Add(this.label29);
            this.groupBox10.Location = new System.Drawing.Point(303, 90);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(243, 120);
            this.groupBox10.TabIndex = 4;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Gateway Status";
            // 
            // txtGatewayStatus
            // 
            this.txtGatewayStatus.Enabled = false;
            this.txtGatewayStatus.Location = new System.Drawing.Point(100, 88);
            this.txtGatewayStatus.Name = "txtGatewayStatus";
            this.txtGatewayStatus.Size = new System.Drawing.Size(102, 20);
            this.txtGatewayStatus.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Status";
            // 
            // progressBarBatteryLevel
            // 
            this.progressBarBatteryLevel.Location = new System.Drawing.Point(100, 57);
            this.progressBarBatteryLevel.Name = "progressBarBatteryLevel";
            this.progressBarBatteryLevel.Size = new System.Drawing.Size(139, 21);
            this.progressBarBatteryLevel.TabIndex = 13;
            // 
            // progressBarSignalQuality
            // 
            this.progressBarSignalQuality.Location = new System.Drawing.Point(100, 30);
            this.progressBarSignalQuality.Name = "progressBarSignalQuality";
            this.progressBarSignalQuality.Size = new System.Drawing.Size(139, 21);
            this.progressBarSignalQuality.TabIndex = 12;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 60);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(69, 13);
            this.label28.TabIndex = 3;
            this.label28.Text = "Battery Level";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(6, 29);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(71, 13);
            this.label29.TabIndex = 2;
            this.label29.Text = "Signal Quality";
            // 
            // ssApplicationStatus
            // 
            this.ssApplicationStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripGateway});
            this.ssApplicationStatus.Location = new System.Drawing.Point(0, 375);
            this.ssApplicationStatus.Name = "ssApplicationStatus";
            this.ssApplicationStatus.Size = new System.Drawing.Size(575, 22);
            this.ssApplicationStatus.TabIndex = 5;
            this.ssApplicationStatus.Text = "statusStrip1";
            // 
            // toolStripGateway
            // 
            this.toolStripGateway.Name = "toolStripGateway";
            this.toolStripGateway.Size = new System.Drawing.Size(0, 17);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.groupBox10);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(564, 253);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnApplyPrefixes);
            this.groupBox2.Controls.Add(this.txtPrefixes);
            this.groupBox2.Location = new System.Drawing.Point(7, 263);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(563, 51);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Prefixes (Comma separated)";
            // 
            // btnApplyPrefixes
            // 
            this.btnApplyPrefixes.Location = new System.Drawing.Point(325, 15);
            this.btnApplyPrefixes.Name = "btnApplyPrefixes";
            this.btnApplyPrefixes.Size = new System.Drawing.Size(139, 26);
            this.btnApplyPrefixes.TabIndex = 8;
            this.btnApplyPrefixes.Text = "Apply";
            this.btnApplyPrefixes.UseVisualStyleBackColor = true;
            this.btnApplyPrefixes.Click += new System.EventHandler(this.btnApplyPrefixes_Click);
            // 
            // txtPrefixes
            // 
            this.txtPrefixes.Location = new System.Drawing.Point(11, 19);
            this.txtPrefixes.Name = "txtPrefixes";
            this.txtPrefixes.Size = new System.Drawing.Size(308, 20);
            this.txtPrefixes.TabIndex = 7;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnClose);
            this.groupBox4.Location = new System.Drawing.Point(6, 320);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(564, 52);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            // 
            // frmGatewayDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 397);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ssApplicationStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGatewayDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gateway Details";
            this.Load += new System.EventHandler(this.frmGatewayDetails_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGatewayDetails_FormClosed);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.ssApplicationStatus.ResumeLayout(false);
            this.ssApplicationStatus.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtManufacturer;
        private System.Windows.Forms.TextBox txtFirmware;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.TextBox txtImsi;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtOutgoingMessage;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.ProgressBar progressBarBatteryLevel;
        private System.Windows.Forms.ProgressBar progressBarSignalQuality;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.StatusStrip ssApplicationStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripGateway;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGatewayStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnApplyPrefixes;
        private System.Windows.Forms.TextBox txtPrefixes;
    }
}