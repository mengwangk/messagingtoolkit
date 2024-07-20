namespace MessagingToolkit.Core.Utilities
{
    partial class frmSMPP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSMPP));
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.cboSystemMode = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSystemId = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.groupBox44 = new System.Windows.Forms.GroupBox();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboNpi = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cboTon = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtAddressRange = new System.Windows.Forms.TextBox();
            this.txtSystemType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtServerKeepAlive = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSleepAfterSocketFailure = new System.Windows.Forms.TextBox();
            this.cboInterfaceVersion = new System.Windows.Forms.ComboBox();
            this.tabSendReceiveSms = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkDeliveryReport = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtReceivedMessage = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnSendSms = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtRecipients = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cboDataCoding = new System.Windows.Forms.ComboBox();
            this.txtSourceAddress = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cboDestinationNpi = new System.Windows.Forms.ComboBox();
            this.cboSourceTon = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboDestinationTon = new System.Windows.Forms.ComboBox();
            this.cboSourceNpi = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.lblLicense = new System.Windows.Forms.Label();
            this.lblAbout = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkPayload = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBox44.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabSendReceiveSms.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(82, 19);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(204, 20);
            this.txtServer.TabIndex = 25;
            this.txtServer.Text = "localhost";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 22);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(38, 13);
            this.label26.TabIndex = 24;
            this.label26.Text = "Server";
            // 
            // cboSystemMode
            // 
            this.cboSystemMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSystemMode.FormattingEnabled = true;
            this.cboSystemMode.Location = new System.Drawing.Point(423, 42);
            this.cboSystemMode.Name = "cboSystemMode";
            this.cboSystemMode.Size = new System.Drawing.Size(148, 21);
            this.cboSystemMode.TabIndex = 23;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtSystemId);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.cboSystemMode);
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(648, 99);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(82, 66);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(204, 20);
            this.txtPassword.TabIndex = 32;
            this.txtPassword.Text = "password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "System Mode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "System ID";
            // 
            // txtSystemId
            // 
            this.txtSystemId.Location = new System.Drawing.Point(82, 42);
            this.txtSystemId.Name = "txtSystemId";
            this.txtSystemId.Size = new System.Drawing.Size(204, 20);
            this.txtSystemId.TabIndex = 29;
            this.txtSystemId.Text = "smppclient1";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(423, 19);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(148, 20);
            this.txtPort.TabIndex = 27;
            this.txtPort.Text = "2775";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(320, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Port";
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Controls.Add(this.tabSendReceiveSms);
            this.tabMain.Controls.Add(this.tabAbout);
            this.tabMain.Location = new System.Drawing.Point(1, 9);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(682, 455);
            this.tabMain.TabIndex = 27;
            this.tabMain.Click += new System.EventHandler(this.tabMain_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupBox44);
            this.tabSettings.Controls.Add(this.groupBox3);
            this.tabSettings.Controls.Add(this.groupBox2);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(674, 429);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox44
            // 
            this.groupBox44.Controls.Add(this.txtLogFile);
            this.groupBox44.Location = new System.Drawing.Point(7, 322);
            this.groupBox44.Name = "groupBox44";
            this.groupBox44.Size = new System.Drawing.Size(649, 44);
            this.groupBox44.TabIndex = 32;
            this.groupBox44.TabStop = false;
            this.groupBox44.Text = "Logging";
            // 
            // txtLogFile
            // 
            this.txtLogFile.Location = new System.Drawing.Point(59, 14);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.ReadOnly = true;
            this.txtLogFile.Size = new System.Drawing.Size(552, 20);
            this.txtLogFile.TabIndex = 14;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDisconnect);
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Location = new System.Drawing.Point(8, 366);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(647, 60);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(310, 18);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(98, 29);
            this.btnDisconnect.TabIndex = 37;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(194, 18);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(98, 29);
            this.btnConnect.TabIndex = 36;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cboNpi);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.cboTon);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.txtAddressRange);
            this.groupBox2.Controls.Add(this.txtSystemType);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtServerKeepAlive);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtSleepAfterSocketFailure);
            this.groupBox2.Controls.Add(this.cboInterfaceVersion);
            this.groupBox2.Location = new System.Drawing.Point(8, 111);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(648, 205);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Advanced Settings";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(368, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 69;
            this.label7.Text = "NPI";
            // 
            // cboNpi
            // 
            this.cboNpi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNpi.FormattingEnabled = true;
            this.cboNpi.Location = new System.Drawing.Point(462, 73);
            this.cboNpi.Name = "cboNpi";
            this.cboNpi.Size = new System.Drawing.Size(148, 21);
            this.cboNpi.TabIndex = 68;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(368, 50);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(30, 13);
            this.label14.TabIndex = 67;
            this.label14.Text = "TON";
            // 
            // cboTon
            // 
            this.cboTon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTon.FormattingEnabled = true;
            this.cboTon.Location = new System.Drawing.Point(462, 46);
            this.cboTon.Name = "cboTon";
            this.cboTon.Size = new System.Drawing.Size(148, 21);
            this.cboTon.TabIndex = 66;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(369, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 13);
            this.label16.TabIndex = 51;
            this.label16.Text = "Address Range";
            // 
            // txtAddressRange
            // 
            this.txtAddressRange.Location = new System.Drawing.Point(462, 19);
            this.txtAddressRange.Name = "txtAddressRange";
            this.txtAddressRange.Size = new System.Drawing.Size(148, 20);
            this.txtAddressRange.TabIndex = 50;
            // 
            // txtSystemType
            // 
            this.txtSystemType.Location = new System.Drawing.Point(144, 94);
            this.txtSystemType.Name = "txtSystemType";
            this.txtSystemType.Size = new System.Drawing.Size(75, 20);
            this.txtSystemType.TabIndex = 45;
            this.txtSystemType.Text = "SMPP";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "System  Type";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(226, 70);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 13);
            this.label13.TabIndex = 40;
            this.label13.Text = "seconds";
            // 
            // txtServerKeepAlive
            // 
            this.txtServerKeepAlive.Location = new System.Drawing.Point(144, 67);
            this.txtServerKeepAlive.Name = "txtServerKeepAlive";
            this.txtServerKeepAlive.Size = new System.Drawing.Size(75, 20);
            this.txtServerKeepAlive.TabIndex = 39;
            this.txtServerKeepAlive.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 74);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 38;
            this.label12.Text = "Server Keep Alive";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(225, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "seconds";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 13);
            this.label10.TabIndex = 36;
            this.label10.Text = "Sleep After Socket Failure";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Interface Version";
            // 
            // txtSleepAfterSocketFailure
            // 
            this.txtSleepAfterSocketFailure.Location = new System.Drawing.Point(144, 45);
            this.txtSleepAfterSocketFailure.Name = "txtSleepAfterSocketFailure";
            this.txtSleepAfterSocketFailure.Size = new System.Drawing.Size(75, 20);
            this.txtSleepAfterSocketFailure.TabIndex = 29;
            this.txtSleepAfterSocketFailure.Text = "10";
            // 
            // cboInterfaceVersion
            // 
            this.cboInterfaceVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInterfaceVersion.FormattingEnabled = true;
            this.cboInterfaceVersion.Location = new System.Drawing.Point(144, 19);
            this.cboInterfaceVersion.Name = "cboInterfaceVersion";
            this.cboInterfaceVersion.Size = new System.Drawing.Size(148, 21);
            this.cboInterfaceVersion.TabIndex = 23;
            // 
            // tabSendReceiveSms
            // 
            this.tabSendReceiveSms.Controls.Add(this.groupBox7);
            this.tabSendReceiveSms.Controls.Add(this.groupBox6);
            this.tabSendReceiveSms.Controls.Add(this.groupBox5);
            this.tabSendReceiveSms.Controls.Add(this.groupBox4);
            this.tabSendReceiveSms.Location = new System.Drawing.Point(4, 22);
            this.tabSendReceiveSms.Name = "tabSendReceiveSms";
            this.tabSendReceiveSms.Padding = new System.Windows.Forms.Padding(3);
            this.tabSendReceiveSms.Size = new System.Drawing.Size(674, 429);
            this.tabSendReceiveSms.TabIndex = 1;
            this.tabSendReceiveSms.Text = "Send & Receive SMS";
            this.tabSendReceiveSms.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkDeliveryReport);
            this.groupBox7.Location = new System.Drawing.Point(481, 108);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(183, 177);
            this.groupBox7.TabIndex = 61;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "SMS Send Options";
            // 
            // chkDeliveryReport
            // 
            this.chkDeliveryReport.Location = new System.Drawing.Point(6, 22);
            this.chkDeliveryReport.Name = "chkDeliveryReport";
            this.chkDeliveryReport.Size = new System.Drawing.Size(151, 24);
            this.chkDeliveryReport.TabIndex = 3;
            this.chkDeliveryReport.Text = "&Request delivery report";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtReceivedMessage);
            this.groupBox6.Location = new System.Drawing.Point(3, 291);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(661, 132);
            this.groupBox6.TabIndex = 72;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Received SMS";
            // 
            // txtReceivedMessage
            // 
            this.txtReceivedMessage.Location = new System.Drawing.Point(6, 19);
            this.txtReceivedMessage.Multiline = true;
            this.txtReceivedMessage.Name = "txtReceivedMessage";
            this.txtReceivedMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReceivedMessage.Size = new System.Drawing.Size(649, 107);
            this.txtReceivedMessage.TabIndex = 59;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkPayload);
            this.groupBox5.Controls.Add(this.btnSendSms);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.txtMessage);
            this.groupBox5.Controls.Add(this.txtRecipients);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Location = new System.Drawing.Point(3, 108);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(472, 177);
            this.groupBox5.TabIndex = 71;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Send SMS";
            // 
            // btnSendSms
            // 
            this.btnSendSms.Location = new System.Drawing.Point(216, 141);
            this.btnSendSms.Name = "btnSendSms";
            this.btnSendSms.Size = new System.Drawing.Size(137, 29);
            this.btnSendSms.TabIndex = 61;
            this.btnSendSms.Text = "Send";
            this.btnSendSms.UseVisualStyleBackColor = true;
            this.btnSendSms.Click += new System.EventHandler(this.btnSendSms_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(24, 48);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(50, 13);
            this.label21.TabIndex = 59;
            this.label21.Text = "Message";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(117, 45);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(338, 66);
            this.txtMessage.TabIndex = 58;
            // 
            // txtRecipients
            // 
            this.txtRecipients.Location = new System.Drawing.Point(117, 19);
            this.txtRecipients.Name = "txtRecipients";
            this.txtRecipients.Size = new System.Drawing.Size(338, 20);
            this.txtRecipients.TabIndex = 56;
            this.txtRecipients.Text = "337788665522";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 57;
            this.label15.Text = "Recipients";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.cboDataCoding);
            this.groupBox4.Controls.Add(this.txtSourceAddress);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.cboDestinationNpi);
            this.groupBox4.Controls.Add(this.cboSourceTon);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.cboDestinationTon);
            this.groupBox4.Controls.Add(this.cboSourceNpi);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Location = new System.Drawing.Point(5, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(661, 99);
            this.groupBox4.TabIndex = 70;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Configuration Settings";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(308, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 13);
            this.label9.TabIndex = 71;
            this.label9.Text = "Data Coding";
            // 
            // cboDataCoding
            // 
            this.cboDataCoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataCoding.FormattingEnabled = true;
            this.cboDataCoding.Location = new System.Drawing.Point(401, 72);
            this.cboDataCoding.Name = "cboDataCoding";
            this.cboDataCoding.Size = new System.Drawing.Size(148, 21);
            this.cboDataCoding.TabIndex = 70;
            // 
            // txtSourceAddress
            // 
            this.txtSourceAddress.Location = new System.Drawing.Point(115, 19);
            this.txtSourceAddress.Name = "txtSourceAddress";
            this.txtSourceAddress.Size = new System.Drawing.Size(148, 20);
            this.txtSourceAddress.TabIndex = 54;
            this.txtSourceAddress.Text = "4477665544";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(21, 76);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(81, 13);
            this.label20.TabIndex = 69;
            this.label20.Text = "Destination NPI";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(22, 22);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 13);
            this.label17.TabIndex = 55;
            this.label17.Text = "Source Address";
            // 
            // cboDestinationNpi
            // 
            this.cboDestinationNpi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDestinationNpi.FormattingEnabled = true;
            this.cboDestinationNpi.Location = new System.Drawing.Point(115, 72);
            this.cboDestinationNpi.Name = "cboDestinationNpi";
            this.cboDestinationNpi.Size = new System.Drawing.Size(148, 21);
            this.cboDestinationNpi.TabIndex = 68;
            // 
            // cboSourceTon
            // 
            this.cboSourceTon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceTon.FormattingEnabled = true;
            this.cboSourceTon.Location = new System.Drawing.Point(401, 18);
            this.cboSourceTon.Name = "cboSourceTon";
            this.cboSourceTon.Size = new System.Drawing.Size(148, 21);
            this.cboSourceTon.TabIndex = 62;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(21, 49);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(86, 13);
            this.label19.TabIndex = 67;
            this.label19.Text = "Destination TON";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(307, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 63;
            this.label5.Text = "Source TON";
            // 
            // cboDestinationTon
            // 
            this.cboDestinationTon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDestinationTon.FormattingEnabled = true;
            this.cboDestinationTon.Location = new System.Drawing.Point(115, 45);
            this.cboDestinationTon.Name = "cboDestinationTon";
            this.cboDestinationTon.Size = new System.Drawing.Size(148, 21);
            this.cboDestinationTon.TabIndex = 66;
            // 
            // cboSourceNpi
            // 
            this.cboSourceNpi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceNpi.FormattingEnabled = true;
            this.cboSourceNpi.Location = new System.Drawing.Point(401, 46);
            this.cboSourceNpi.Name = "cboSourceNpi";
            this.cboSourceNpi.Size = new System.Drawing.Size(148, 21);
            this.cboSourceNpi.TabIndex = 64;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(307, 49);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(62, 13);
            this.label18.TabIndex = 65;
            this.label18.Text = "Source NPI";
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.groupBox8);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(674, 429);
            this.tabAbout.TabIndex = 3;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.lblLicense);
            this.groupBox8.Controls.Add(this.lblAbout);
            this.groupBox8.Controls.Add(this.linkLabel1);
            this.groupBox8.Location = new System.Drawing.Point(3, 0);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(665, 426);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.Location = new System.Drawing.Point(271, 104);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(0, 20);
            this.lblLicense.TabIndex = 2;
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAbout
            // 
            this.lblAbout.AutoSize = true;
            this.lblAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbout.Location = new System.Drawing.Point(225, 139);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(60, 20);
            this.lblAbout.TabIndex = 1;
            this.lblAbout.Text = "label12";
            this.lblAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(252, 187);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(162, 20);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://www.twit88.com";
            // 
            // chkPayload
            // 
            this.chkPayload.Location = new System.Drawing.Point(114, 111);
            this.chkPayload.Name = "chkPayload";
            this.chkPayload.Size = new System.Drawing.Size(207, 24);
            this.chkPayload.TabIndex = 62;
            this.chkPayload.Text = "Payload ";
            // 
            // frmSMPP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 476);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSMPP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MessagingToolkit Demo - SMPP";
            this.Load += new System.EventHandler(this.frmSMPP_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSMPP_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.groupBox44.ResumeLayout(false);
            this.groupBox44.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabSendReceiveSms.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox cboSystemMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabSendReceiveSms;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSystemId;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtAddressRange;
        private System.Windows.Forms.TextBox txtSystemType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtServerKeepAlive;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSleepAfterSocketFailure;
        private System.Windows.Forms.ComboBox cboInterfaceVersion;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox44;
        private System.Windows.Forms.TextBox txtLogFile;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cboDestinationNpi;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cboDestinationTon;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cboSourceNpi;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboSourceTon;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtSourceAddress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboNpi;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cboTon;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtRecipients;
        private System.Windows.Forms.Label label15;
        internal System.Windows.Forms.GroupBox groupBox7;
        internal System.Windows.Forms.CheckBox chkDeliveryReport;
        private System.Windows.Forms.Button btnSendSms;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox txtReceivedMessage;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboDataCoding;
        internal System.Windows.Forms.CheckBox chkPayload;
    }
}