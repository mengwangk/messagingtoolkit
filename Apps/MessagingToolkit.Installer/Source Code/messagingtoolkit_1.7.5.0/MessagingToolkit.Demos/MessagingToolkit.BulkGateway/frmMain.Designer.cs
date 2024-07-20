namespace MessagingToolkit.BulkGateway
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabSubMain = new System.Windows.Forms.TabControl();
            this.tabGatewaySettings = new System.Windows.Forms.TabPage();
            this.grpBoxGatewaySettings = new System.Windows.Forms.GroupBox();
            this.lnkBrowse = new System.Windows.Forms.LinkLabel();
            this.txtPersistenceFolder = new System.Windows.Forms.TextBox();
            this.chkPersistenceQueue = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radPhone = new System.Windows.Forms.RadioButton();
            this.radSim = new System.Windows.Forms.RadioButton();
            this.label20 = new System.Windows.Forms.Label();
            this.txtNumberPrefix = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtGatewayId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCancelAddGateway = new System.Windows.Forms.Button();
            this.btnAddGateway = new System.Windows.Forms.Button();
            this.updSendWaitInterval = new System.Windows.Forms.NumericUpDown();
            this.updSendRetries = new System.Windows.Forms.NumericUpDown();
            this.txtModelConfig = new System.Windows.Forms.TextBox();
            this.txtPin = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cboParity = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboStopBits = new System.Windows.Forms.ComboBox();
            this.cboDataBits = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBaudRate = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabServiceSettings = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.lnkLabelSampleRouter = new System.Windows.Forms.LinkLabel();
            this.lnkLabelSampleBalancer = new System.Windows.Forms.LinkLabel();
            this.label16 = new System.Windows.Forms.Label();
            this.cboLoadBalancer = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnApplyServiceSettings = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.cboRouter = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLoadFromFile = new System.Windows.Forms.Button();
            this.btnSaveToFile = new System.Windows.Forms.Button();
            this.chkMonitorGatewayStatus = new System.Windows.Forms.CheckBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstGateways = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabMessaging = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkPauseMessageSending = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnAddPhoneBookEntry = new System.Windows.Forms.Button();
            this.txtPhoneBookEntry = new System.Windows.Forms.TextBox();
            this.btnSavePhoneBook = new System.Windows.Forms.Button();
            this.btnImportPhoneBook = new System.Windows.Forms.Button();
            this.btnRemoveAllPhoneBook = new System.Windows.Forms.Button();
            this.btnRemovePhoneBookEntry = new System.Windows.Forms.Button();
            this.lstPhoneBook = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.cboQueuePriority = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cboValidityPeriod = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.cboLongMessage = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.txtSmsc = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.txtWapPushUrl = new System.Windows.Forms.TextBox();
            this.btnStopSending = new System.Windows.Forms.Button();
            this.btnStartSending = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cboMessageEncoding = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.chkWapPush = new System.Windows.Forms.CheckBox();
            this.chkAlertMessage = new System.Windows.Forms.CheckBox();
            this.tabTroubleshooting = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnClearLoggingInformation = new System.Windows.Forms.Button();
            this.btnRefreshLoggingInformation = new System.Windows.Forms.Button();
            this.txtLoggingInformation = new System.Windows.Forms.TextBox();
            this.grpBoxLoggingInformation = new System.Windows.Forms.GroupBox();
            this.cboLoggingLevel = new System.Windows.Forms.ComboBox();
            this.btnApplyLoggingLevel = new System.Windows.Forms.Button();
            this.label32 = new System.Windows.Forms.Label();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.lblLicense = new System.Windows.Forms.Label();
            this.lblAbout = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.ssApplicationStatus = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripGateway = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabMain.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabSubMain.SuspendLayout();
            this.tabGatewaySettings.SuspendLayout();
            this.grpBoxGatewaySettings.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updSendWaitInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updSendRetries)).BeginInit();
            this.tabServiceSettings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabMessaging.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabTroubleshooting.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grpBoxLoggingInformation.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.ssApplicationStatus.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Controls.Add(this.tabMessaging);
            this.tabMain.Controls.Add(this.tabTroubleshooting);
            this.tabMain.Controls.Add(this.tabAbout);
            this.tabMain.Location = new System.Drawing.Point(0, 27);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(633, 458);
            this.tabMain.TabIndex = 0;
            this.tabMain.Click += new System.EventHandler(this.tabMain_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabSubMain);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(625, 432);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabSubMain
            // 
            this.tabSubMain.Controls.Add(this.tabGatewaySettings);
            this.tabSubMain.Controls.Add(this.tabServiceSettings);
            this.tabSubMain.Location = new System.Drawing.Point(6, 180);
            this.tabSubMain.Name = "tabSubMain";
            this.tabSubMain.SelectedIndex = 0;
            this.tabSubMain.Size = new System.Drawing.Size(614, 246);
            this.tabSubMain.TabIndex = 41;
            // 
            // tabGatewaySettings
            // 
            this.tabGatewaySettings.Controls.Add(this.grpBoxGatewaySettings);
            this.tabGatewaySettings.Location = new System.Drawing.Point(4, 22);
            this.tabGatewaySettings.Name = "tabGatewaySettings";
            this.tabGatewaySettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabGatewaySettings.Size = new System.Drawing.Size(606, 220);
            this.tabGatewaySettings.TabIndex = 0;
            this.tabGatewaySettings.Text = "Gateway Settings";
            this.tabGatewaySettings.UseVisualStyleBackColor = true;
            // 
            // grpBoxGatewaySettings
            // 
            this.grpBoxGatewaySettings.Controls.Add(this.lnkBrowse);
            this.grpBoxGatewaySettings.Controls.Add(this.txtPersistenceFolder);
            this.grpBoxGatewaySettings.Controls.Add(this.chkPersistenceQueue);
            this.grpBoxGatewaySettings.Controls.Add(this.groupBox6);
            this.grpBoxGatewaySettings.Controls.Add(this.label20);
            this.grpBoxGatewaySettings.Controls.Add(this.txtNumberPrefix);
            this.grpBoxGatewaySettings.Controls.Add(this.label19);
            this.grpBoxGatewaySettings.Controls.Add(this.btnReset);
            this.grpBoxGatewaySettings.Controls.Add(this.txtGatewayId);
            this.grpBoxGatewaySettings.Controls.Add(this.label7);
            this.grpBoxGatewaySettings.Controls.Add(this.btnCancelAddGateway);
            this.grpBoxGatewaySettings.Controls.Add(this.btnAddGateway);
            this.grpBoxGatewaySettings.Controls.Add(this.updSendWaitInterval);
            this.grpBoxGatewaySettings.Controls.Add(this.updSendRetries);
            this.grpBoxGatewaySettings.Controls.Add(this.txtModelConfig);
            this.grpBoxGatewaySettings.Controls.Add(this.txtPin);
            this.grpBoxGatewaySettings.Controls.Add(this.label17);
            this.grpBoxGatewaySettings.Controls.Add(this.label15);
            this.grpBoxGatewaySettings.Controls.Add(this.label14);
            this.grpBoxGatewaySettings.Controls.Add(this.label12);
            this.grpBoxGatewaySettings.Controls.Add(this.cboParity);
            this.grpBoxGatewaySettings.Controls.Add(this.label4);
            this.grpBoxGatewaySettings.Controls.Add(this.label5);
            this.grpBoxGatewaySettings.Controls.Add(this.cboStopBits);
            this.grpBoxGatewaySettings.Controls.Add(this.cboDataBits);
            this.grpBoxGatewaySettings.Controls.Add(this.label3);
            this.grpBoxGatewaySettings.Controls.Add(this.cboBaudRate);
            this.grpBoxGatewaySettings.Controls.Add(this.label2);
            this.grpBoxGatewaySettings.Controls.Add(this.cboPort);
            this.grpBoxGatewaySettings.Controls.Add(this.label1);
            this.grpBoxGatewaySettings.Enabled = false;
            this.grpBoxGatewaySettings.Location = new System.Drawing.Point(0, 6);
            this.grpBoxGatewaySettings.Name = "grpBoxGatewaySettings";
            this.grpBoxGatewaySettings.Size = new System.Drawing.Size(596, 218);
            this.grpBoxGatewaySettings.TabIndex = 2;
            this.grpBoxGatewaySettings.TabStop = false;
            // 
            // lnkBrowse
            // 
            this.lnkBrowse.AutoSize = true;
            this.lnkBrowse.Enabled = false;
            this.lnkBrowse.Location = new System.Drawing.Point(460, 164);
            this.lnkBrowse.Name = "lnkBrowse";
            this.lnkBrowse.Size = new System.Drawing.Size(42, 13);
            this.lnkBrowse.TabIndex = 47;
            this.lnkBrowse.TabStop = true;
            this.lnkBrowse.Text = "Browse";
            this.lnkBrowse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBrowse_LinkClicked);
            // 
            // txtPersistenceFolder
            // 
            this.txtPersistenceFolder.Enabled = false;
            this.txtPersistenceFolder.Location = new System.Drawing.Point(190, 161);
            this.txtPersistenceFolder.Name = "txtPersistenceFolder";
            this.txtPersistenceFolder.Size = new System.Drawing.Size(268, 20);
            this.txtPersistenceFolder.TabIndex = 46;
            // 
            // chkPersistenceQueue
            // 
            this.chkPersistenceQueue.AutoSize = true;
            this.chkPersistenceQueue.Location = new System.Drawing.Point(73, 163);
            this.chkPersistenceQueue.Name = "chkPersistenceQueue";
            this.chkPersistenceQueue.Size = new System.Drawing.Size(103, 17);
            this.chkPersistenceQueue.TabIndex = 45;
            this.chkPersistenceQueue.Text = "Persist Message";
            this.chkPersistenceQueue.UseVisualStyleBackColor = true;
            this.chkPersistenceQueue.CheckedChanged += new System.EventHandler(this.chkPersistenceQueue_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radPhone);
            this.groupBox6.Controls.Add(this.radSim);
            this.groupBox6.Location = new System.Drawing.Point(469, 43);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(121, 90);
            this.groupBox6.TabIndex = 44;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Memory";
            // 
            // radPhone
            // 
            this.radPhone.AutoSize = true;
            this.radPhone.Location = new System.Drawing.Point(22, 53);
            this.radPhone.Name = "radPhone";
            this.radPhone.Size = new System.Drawing.Size(56, 17);
            this.radPhone.TabIndex = 1;
            this.radPhone.Text = "Phone";
            this.radPhone.UseVisualStyleBackColor = true;
            // 
            // radSim
            // 
            this.radSim.AutoSize = true;
            this.radSim.Checked = true;
            this.radSim.Location = new System.Drawing.Point(22, 22);
            this.radSim.Name = "radSim";
            this.radSim.Size = new System.Drawing.Size(44, 17);
            this.radSim.TabIndex = 0;
            this.radSim.TabStop = true;
            this.radSim.Text = "SIM";
            this.radSim.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(460, 145);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(97, 13);
            this.label20.TabIndex = 43;
            this.label20.Text = "(comma separated)";
            // 
            // txtNumberPrefix
            // 
            this.txtNumberPrefix.Location = new System.Drawing.Point(340, 139);
            this.txtNumberPrefix.Name = "txtNumberPrefix";
            this.txtNumberPrefix.Size = new System.Drawing.Size(114, 20);
            this.txtNumberPrefix.TabIndex = 31;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(239, 142);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(33, 13);
            this.label19.TabIndex = 41;
            this.label19.Text = "Prefix";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(244, 185);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(90, 26);
            this.btnReset.TabIndex = 33;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtGatewayId
            // 
            this.txtGatewayId.Location = new System.Drawing.Point(124, 16);
            this.txtGatewayId.Name = "txtGatewayId";
            this.txtGatewayId.Size = new System.Drawing.Size(233, 20);
            this.txtGatewayId.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "Gateway Id *";
            // 
            // btnCancelAddGateway
            // 
            this.btnCancelAddGateway.Location = new System.Drawing.Point(340, 185);
            this.btnCancelAddGateway.Name = "btnCancelAddGateway";
            this.btnCancelAddGateway.Size = new System.Drawing.Size(90, 26);
            this.btnCancelAddGateway.TabIndex = 34;
            this.btnCancelAddGateway.Text = "Cancel";
            this.btnCancelAddGateway.UseVisualStyleBackColor = true;
            this.btnCancelAddGateway.Click += new System.EventHandler(this.btnCancelAddGateway_Click);
            // 
            // btnAddGateway
            // 
            this.btnAddGateway.Location = new System.Drawing.Point(145, 185);
            this.btnAddGateway.Name = "btnAddGateway";
            this.btnAddGateway.Size = new System.Drawing.Size(90, 26);
            this.btnAddGateway.TabIndex = 32;
            this.btnAddGateway.Text = "Add Gateway";
            this.btnAddGateway.UseVisualStyleBackColor = true;
            this.btnAddGateway.Click += new System.EventHandler(this.btnAddGateway_Click);
            // 
            // updSendWaitInterval
            // 
            this.updSendWaitInterval.Location = new System.Drawing.Point(340, 116);
            this.updSendWaitInterval.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.updSendWaitInterval.Name = "updSendWaitInterval";
            this.updSendWaitInterval.Size = new System.Drawing.Size(114, 20);
            this.updSendWaitInterval.TabIndex = 30;
            // 
            // updSendRetries
            // 
            this.updSendRetries.Location = new System.Drawing.Point(340, 93);
            this.updSendRetries.Name = "updSendRetries";
            this.updSendRetries.Size = new System.Drawing.Size(114, 20);
            this.updSendRetries.TabIndex = 29;
            // 
            // txtModelConfig
            // 
            this.txtModelConfig.Location = new System.Drawing.Point(340, 70);
            this.txtModelConfig.Name = "txtModelConfig";
            this.txtModelConfig.Size = new System.Drawing.Size(114, 20);
            this.txtModelConfig.TabIndex = 28;
            // 
            // txtPin
            // 
            this.txtPin.Location = new System.Drawing.Point(340, 47);
            this.txtPin.Name = "txtPin";
            this.txtPin.Size = new System.Drawing.Size(114, 20);
            this.txtPin.TabIndex = 26;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(239, 120);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(95, 13);
            this.label17.TabIndex = 24;
            this.label17.Text = "Send Wait Interval";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(239, 98);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(68, 13);
            this.label15.TabIndex = 23;
            this.label15.Text = "Send Retries";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(239, 73);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 13);
            this.label14.TabIndex = 22;
            this.label14.Text = "Model";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(239, 50);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(25, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "PIN";
            // 
            // cboParity
            // 
            this.cboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParity.FormattingEnabled = true;
            this.cboParity.Location = new System.Drawing.Point(73, 115);
            this.cboParity.Name = "cboParity";
            this.cboParity.Size = new System.Drawing.Size(117, 21);
            this.cboParity.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Parity";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Stop Bits";
            // 
            // cboStopBits
            // 
            this.cboStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStopBits.FormattingEnabled = true;
            this.cboStopBits.Location = new System.Drawing.Point(73, 138);
            this.cboStopBits.Name = "cboStopBits";
            this.cboStopBits.Size = new System.Drawing.Size(117, 21);
            this.cboStopBits.TabIndex = 11;
            // 
            // cboDataBits
            // 
            this.cboDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataBits.FormattingEnabled = true;
            this.cboDataBits.Location = new System.Drawing.Point(73, 93);
            this.cboDataBits.Name = "cboDataBits";
            this.cboDataBits.Size = new System.Drawing.Size(117, 21);
            this.cboDataBits.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Data Bits";
            // 
            // cboBaudRate
            // 
            this.cboBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBaudRate.FormattingEnabled = true;
            this.cboBaudRate.Location = new System.Drawing.Point(73, 69);
            this.cboBaudRate.Name = "cboBaudRate";
            this.cboBaudRate.Size = new System.Drawing.Size(117, 21);
            this.cboBaudRate.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Baud Rate";
            // 
            // cboPort
            // 
            this.cboPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Location = new System.Drawing.Point(73, 46);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(117, 21);
            this.cboPort.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port";
            // 
            // tabServiceSettings
            // 
            this.tabServiceSettings.Controls.Add(this.groupBox2);
            this.tabServiceSettings.Location = new System.Drawing.Point(4, 22);
            this.tabServiceSettings.Name = "tabServiceSettings";
            this.tabServiceSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabServiceSettings.Size = new System.Drawing.Size(606, 220);
            this.tabServiceSettings.TabIndex = 1;
            this.tabServiceSettings.Text = "Service Settings";
            this.tabServiceSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.lnkLabelSampleRouter);
            this.groupBox2.Controls.Add(this.lnkLabelSampleBalancer);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.cboLoadBalancer);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.btnApplyServiceSettings);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.cboRouter);
            this.groupBox2.Location = new System.Drawing.Point(5, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(595, 211);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 182);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(213, 13);
            this.label18.TabIndex = 38;
            this.label18.Text = "You can write your own balancer and router";
            // 
            // lnkLabelSampleRouter
            // 
            this.lnkLabelSampleRouter.AutoSize = true;
            this.lnkLabelSampleRouter.Location = new System.Drawing.Point(447, 182);
            this.lnkLabelSampleRouter.Name = "lnkLabelSampleRouter";
            this.lnkLabelSampleRouter.Size = new System.Drawing.Size(142, 13);
            this.lnkLabelSampleRouter.TabIndex = 37;
            this.lnkLabelSampleRouter.TabStop = true;
            this.lnkLabelSampleRouter.Text = "Sample Router Source Code";
            this.lnkLabelSampleRouter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLabelSampleRouter_LinkClicked);
            // 
            // lnkLabelSampleBalancer
            // 
            this.lnkLabelSampleBalancer.AutoSize = true;
            this.lnkLabelSampleBalancer.Location = new System.Drawing.Point(259, 182);
            this.lnkLabelSampleBalancer.Name = "lnkLabelSampleBalancer";
            this.lnkLabelSampleBalancer.Size = new System.Drawing.Size(152, 13);
            this.lnkLabelSampleBalancer.TabIndex = 36;
            this.lnkLabelSampleBalancer.TabStop = true;
            this.lnkLabelSampleBalancer.Text = "Sample Balancer Source Code";
            this.lnkLabelSampleBalancer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLabelSampleBalancer_LinkClicked);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(284, 71);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(314, 26);
            this.label16.TabIndex = 35;
            this.label16.Text = "Router determines the gateway to be used to send the message, \r\ne.g. by the prefi" +
    "x of the destionation number.";
            // 
            // cboLoadBalancer
            // 
            this.cboLoadBalancer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoadBalancer.FormattingEnabled = true;
            this.cboLoadBalancer.Location = new System.Drawing.Point(115, 19);
            this.cboLoadBalancer.Name = "cboLoadBalancer";
            this.cboLoadBalancer.Size = new System.Drawing.Size(162, 21);
            this.cboLoadBalancer.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(283, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(262, 26);
            this.label11.TabIndex = 34;
            this.label11.Text = "Load balancer distributes the messages among all the \r\nconfigured gateways";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Load Balancer";
            // 
            // btnApplyServiceSettings
            // 
            this.btnApplyServiceSettings.Location = new System.Drawing.Point(115, 121);
            this.btnApplyServiceSettings.Name = "btnApplyServiceSettings";
            this.btnApplyServiceSettings.Size = new System.Drawing.Size(111, 26);
            this.btnApplyServiceSettings.TabIndex = 33;
            this.btnApplyServiceSettings.Text = "Apply";
            this.btnApplyServiceSettings.UseVisualStyleBackColor = true;
            this.btnApplyServiceSettings.Click += new System.EventHandler(this.btnApplyServiceSettings_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(26, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Router";
            // 
            // cboRouter
            // 
            this.cboRouter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRouter.FormattingEnabled = true;
            this.cboRouter.Location = new System.Drawing.Point(115, 76);
            this.cboRouter.Name = "cboRouter";
            this.cboRouter.Size = new System.Drawing.Size(162, 21);
            this.cboRouter.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnLoadFromFile);
            this.groupBox1.Controls.Add(this.btnSaveToFile);
            this.groupBox1.Controls.Add(this.chkMonitorGatewayStatus);
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.lstGateways);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(614, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gateways";
            // 
            // btnLoadFromFile
            // 
            this.btnLoadFromFile.Location = new System.Drawing.Point(436, 136);
            this.btnLoadFromFile.Name = "btnLoadFromFile";
            this.btnLoadFromFile.Size = new System.Drawing.Size(114, 26);
            this.btnLoadFromFile.TabIndex = 5;
            this.btnLoadFromFile.Text = "Load from File";
            this.btnLoadFromFile.UseVisualStyleBackColor = true;
            this.btnLoadFromFile.Click += new System.EventHandler(this.btnLoadFromFile_Click);
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.Location = new System.Drawing.Point(316, 136);
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(114, 26);
            this.btnSaveToFile.TabIndex = 3;
            this.btnSaveToFile.Text = "Save to File";
            this.btnSaveToFile.UseVisualStyleBackColor = true;
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // chkMonitorGatewayStatus
            // 
            this.chkMonitorGatewayStatus.AutoSize = true;
            this.chkMonitorGatewayStatus.Location = new System.Drawing.Point(9, 115);
            this.chkMonitorGatewayStatus.Name = "chkMonitorGatewayStatus";
            this.chkMonitorGatewayStatus.Size = new System.Drawing.Size(139, 17);
            this.chkMonitorGatewayStatus.TabIndex = 3;
            this.chkMonitorGatewayStatus.Text = "Monitor Gateway Status";
            this.chkMonitorGatewayStatus.UseVisualStyleBackColor = true;
            this.chkMonitorGatewayStatus.CheckedChanged += new System.EventHandler(this.chkMonitorGatewayStatus_CheckedChanged);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(220, 136);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 26);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(124, 136);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 26);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstGateways
            // 
            this.lstGateways.LargeImageList = this.imageList;
            this.lstGateways.Location = new System.Drawing.Point(9, 19);
            this.lstGateways.Name = "lstGateways";
            this.lstGateways.Size = new System.Drawing.Size(591, 91);
            this.lstGateways.TabIndex = 0;
            this.lstGateways.UseCompatibleStateImageBehavior = false;
            this.lstGateways.ItemActivate += new System.EventHandler(this.lstGateways_ItemActivate);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "modem.png");
            this.imageList.Images.SetKeyName(1, "phone.png");
            // 
            // tabMessaging
            // 
            this.tabMessaging.Controls.Add(this.groupBox7);
            this.tabMessaging.Controls.Add(this.groupBox5);
            this.tabMessaging.Controls.Add(this.groupBox4);
            this.tabMessaging.Location = new System.Drawing.Point(4, 22);
            this.tabMessaging.Name = "tabMessaging";
            this.tabMessaging.Size = new System.Drawing.Size(625, 432);
            this.tabMessaging.TabIndex = 2;
            this.tabMessaging.Text = "Messaging";
            this.tabMessaging.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkPauseMessageSending);
            this.groupBox7.Location = new System.Drawing.Point(371, 350);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(247, 64);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Message Control";
            // 
            // chkPauseMessageSending
            // 
            this.chkPauseMessageSending.AutoSize = true;
            this.chkPauseMessageSending.Location = new System.Drawing.Point(10, 25);
            this.chkPauseMessageSending.Name = "chkPauseMessageSending";
            this.chkPauseMessageSending.Size = new System.Drawing.Size(141, 17);
            this.chkPauseMessageSending.TabIndex = 0;
            this.chkPauseMessageSending.Text = "Pause message sending";
            this.chkPauseMessageSending.UseVisualStyleBackColor = true;
            this.chkPauseMessageSending.CheckedChanged += new System.EventHandler(this.chkPauseMessageSending_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnAddPhoneBookEntry);
            this.groupBox5.Controls.Add(this.txtPhoneBookEntry);
            this.groupBox5.Controls.Add(this.btnSavePhoneBook);
            this.groupBox5.Controls.Add(this.btnImportPhoneBook);
            this.groupBox5.Controls.Add(this.btnRemoveAllPhoneBook);
            this.groupBox5.Controls.Add(this.btnRemovePhoneBookEntry);
            this.groupBox5.Controls.Add(this.lstPhoneBook);
            this.groupBox5.Location = new System.Drawing.Point(371, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(249, 341);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Phone Book";
            // 
            // btnAddPhoneBookEntry
            // 
            this.btnAddPhoneBookEntry.Location = new System.Drawing.Point(145, 15);
            this.btnAddPhoneBookEntry.Name = "btnAddPhoneBookEntry";
            this.btnAddPhoneBookEntry.Size = new System.Drawing.Size(90, 26);
            this.btnAddPhoneBookEntry.TabIndex = 18;
            this.btnAddPhoneBookEntry.Text = "&Add";
            this.btnAddPhoneBookEntry.UseVisualStyleBackColor = true;
            this.btnAddPhoneBookEntry.Click += new System.EventHandler(this.btnAddPhoneBookEntry_Click);
            // 
            // txtPhoneBookEntry
            // 
            this.txtPhoneBookEntry.Location = new System.Drawing.Point(10, 19);
            this.txtPhoneBookEntry.Name = "txtPhoneBookEntry";
            this.txtPhoneBookEntry.Size = new System.Drawing.Size(129, 20);
            this.txtPhoneBookEntry.TabIndex = 17;
            this.txtPhoneBookEntry.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPhoneBookEntry_KeyUp);
            // 
            // btnSavePhoneBook
            // 
            this.btnSavePhoneBook.Location = new System.Drawing.Point(132, 306);
            this.btnSavePhoneBook.Name = "btnSavePhoneBook";
            this.btnSavePhoneBook.Size = new System.Drawing.Size(90, 26);
            this.btnSavePhoneBook.TabIndex = 16;
            this.btnSavePhoneBook.Text = "Save";
            this.btnSavePhoneBook.UseVisualStyleBackColor = true;
            this.btnSavePhoneBook.Click += new System.EventHandler(this.btnSavePhoneBook_Click);
            // 
            // btnImportPhoneBook
            // 
            this.btnImportPhoneBook.Location = new System.Drawing.Point(27, 306);
            this.btnImportPhoneBook.Name = "btnImportPhoneBook";
            this.btnImportPhoneBook.Size = new System.Drawing.Size(90, 26);
            this.btnImportPhoneBook.TabIndex = 15;
            this.btnImportPhoneBook.Text = "Import";
            this.btnImportPhoneBook.UseVisualStyleBackColor = true;
            this.btnImportPhoneBook.Click += new System.EventHandler(this.btnImportPhoneBook_Click);
            // 
            // btnRemoveAllPhoneBook
            // 
            this.btnRemoveAllPhoneBook.Location = new System.Drawing.Point(132, 274);
            this.btnRemoveAllPhoneBook.Name = "btnRemoveAllPhoneBook";
            this.btnRemoveAllPhoneBook.Size = new System.Drawing.Size(90, 26);
            this.btnRemoveAllPhoneBook.TabIndex = 14;
            this.btnRemoveAllPhoneBook.Text = "Remove All";
            this.btnRemoveAllPhoneBook.UseVisualStyleBackColor = true;
            this.btnRemoveAllPhoneBook.Click += new System.EventHandler(this.btnRemoveAllPhoneBook_Click);
            // 
            // btnRemovePhoneBookEntry
            // 
            this.btnRemovePhoneBookEntry.Location = new System.Drawing.Point(27, 274);
            this.btnRemovePhoneBookEntry.Name = "btnRemovePhoneBookEntry";
            this.btnRemovePhoneBookEntry.Size = new System.Drawing.Size(90, 26);
            this.btnRemovePhoneBookEntry.TabIndex = 13;
            this.btnRemovePhoneBookEntry.Text = "Remove";
            this.btnRemovePhoneBookEntry.UseVisualStyleBackColor = true;
            this.btnRemovePhoneBookEntry.Click += new System.EventHandler(this.btnRemovePhoneBookEntry_Click);
            // 
            // lstPhoneBook
            // 
            this.lstPhoneBook.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstPhoneBook.FormattingEnabled = true;
            this.lstPhoneBook.ItemHeight = 16;
            this.lstPhoneBook.Location = new System.Drawing.Point(10, 56);
            this.lstPhoneBook.Name = "lstPhoneBook";
            this.lstPhoneBook.Size = new System.Drawing.Size(229, 212);
            this.lstPhoneBook.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label21);
            this.groupBox4.Controls.Add(this.cboQueuePriority);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.cboValidityPeriod);
            this.groupBox4.Controls.Add(this.label35);
            this.groupBox4.Controls.Add(this.cboLongMessage);
            this.groupBox4.Controls.Add(this.label34);
            this.groupBox4.Controls.Add(this.txtSmsc);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label37);
            this.groupBox4.Controls.Add(this.txtWapPushUrl);
            this.groupBox4.Controls.Add(this.btnStopSending);
            this.groupBox4.Controls.Add(this.btnStartSending);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.cboMessageEncoding);
            this.groupBox4.Controls.Add(this.label33);
            this.groupBox4.Controls.Add(this.txtMessage);
            this.groupBox4.Controls.Add(this.chkWapPush);
            this.groupBox4.Controls.Add(this.chkAlertMessage);
            this.groupBox4.Location = new System.Drawing.Point(8, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(357, 411);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Message Details";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(15, 190);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(38, 13);
            this.label21.TabIndex = 44;
            this.label21.Text = "Priority";
            // 
            // cboQueuePriority
            // 
            this.cboQueuePriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboQueuePriority.FormattingEnabled = true;
            this.cboQueuePriority.Location = new System.Drawing.Point(96, 187);
            this.cboQueuePriority.Name = "cboQueuePriority";
            this.cboQueuePriority.Size = new System.Drawing.Size(179, 21);
            this.cboQueuePriority.TabIndex = 43;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(203, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(123, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "(Leave empty if not sure)";
            // 
            // cboValidityPeriod
            // 
            this.cboValidityPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValidityPeriod.FormattingEnabled = true;
            this.cboValidityPeriod.Location = new System.Drawing.Point(96, 160);
            this.cboValidityPeriod.Name = "cboValidityPeriod";
            this.cboValidityPeriod.Size = new System.Drawing.Size(179, 21);
            this.cboValidityPeriod.TabIndex = 41;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(15, 163);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(73, 13);
            this.label35.TabIndex = 40;
            this.label35.Text = "Validity Period";
            // 
            // cboLongMessage
            // 
            this.cboLongMessage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLongMessage.FormattingEnabled = true;
            this.cboLongMessage.Location = new System.Drawing.Point(96, 133);
            this.cboLongMessage.Name = "cboLongMessage";
            this.cboLongMessage.Size = new System.Drawing.Size(179, 21);
            this.cboLongMessage.TabIndex = 39;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(15, 136);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(77, 13);
            this.label34.TabIndex = 38;
            this.label34.Text = "Long Message";
            // 
            // txtSmsc
            // 
            this.txtSmsc.Location = new System.Drawing.Point(96, 107);
            this.txtSmsc.Name = "txtSmsc";
            this.txtSmsc.Size = new System.Drawing.Size(101, 20);
            this.txtSmsc.TabIndex = 29;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 110);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 13);
            this.label13.TabIndex = 28;
            this.label13.Text = "SMSC";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(15, 56);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(56, 13);
            this.label37.TabIndex = 14;
            this.label37.Text = "Push URL";
            // 
            // txtWapPushUrl
            // 
            this.txtWapPushUrl.Enabled = false;
            this.txtWapPushUrl.Location = new System.Drawing.Point(96, 53);
            this.txtWapPushUrl.Name = "txtWapPushUrl";
            this.txtWapPushUrl.Size = new System.Drawing.Size(255, 20);
            this.txtWapPushUrl.TabIndex = 15;
            // 
            // btnStopSending
            // 
            this.btnStopSending.Location = new System.Drawing.Point(168, 372);
            this.btnStopSending.Name = "btnStopSending";
            this.btnStopSending.Size = new System.Drawing.Size(90, 26);
            this.btnStopSending.TabIndex = 13;
            this.btnStopSending.Text = "Stop Sending";
            this.btnStopSending.UseVisualStyleBackColor = true;
            this.btnStopSending.Click += new System.EventHandler(this.btnStopSending_Click);
            // 
            // btnStartSending
            // 
            this.btnStartSending.Location = new System.Drawing.Point(62, 372);
            this.btnStartSending.Name = "btnStartSending";
            this.btnStartSending.Size = new System.Drawing.Size(90, 26);
            this.btnStartSending.TabIndex = 12;
            this.btnStartSending.Text = "Start Sending";
            this.btnStartSending.UseVisualStyleBackColor = true;
            this.btnStartSending.Click += new System.EventHandler(this.btnStartSending_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 214);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Message";
            // 
            // cboMessageEncoding
            // 
            this.cboMessageEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageEncoding.FormattingEnabled = true;
            this.cboMessageEncoding.Location = new System.Drawing.Point(96, 80);
            this.cboMessageEncoding.Name = "cboMessageEncoding";
            this.cboMessageEncoding.Size = new System.Drawing.Size(179, 21);
            this.cboMessageEncoding.TabIndex = 10;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(15, 83);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(52, 13);
            this.label33.TabIndex = 9;
            this.label33.Text = "Encoding";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(13, 233);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessage.Size = new System.Drawing.Size(338, 133);
            this.txtMessage.TabIndex = 2;
            // 
            // chkWapPush
            // 
            this.chkWapPush.AutoSize = true;
            this.chkWapPush.Location = new System.Drawing.Point(140, 19);
            this.chkWapPush.Name = "chkWapPush";
            this.chkWapPush.Size = new System.Drawing.Size(78, 17);
            this.chkWapPush.TabIndex = 1;
            this.chkWapPush.Text = "WAP Push";
            this.chkWapPush.UseVisualStyleBackColor = true;
            this.chkWapPush.CheckedChanged += new System.EventHandler(this.chkWapPush_CheckedChanged);
            // 
            // chkAlertMessage
            // 
            this.chkAlertMessage.AutoSize = true;
            this.chkAlertMessage.Location = new System.Drawing.Point(13, 19);
            this.chkAlertMessage.Name = "chkAlertMessage";
            this.chkAlertMessage.Size = new System.Drawing.Size(93, 17);
            this.chkAlertMessage.TabIndex = 0;
            this.chkAlertMessage.Text = "Alert Message";
            this.chkAlertMessage.UseVisualStyleBackColor = true;
            // 
            // tabTroubleshooting
            // 
            this.tabTroubleshooting.Controls.Add(this.groupBox3);
            this.tabTroubleshooting.Controls.Add(this.grpBoxLoggingInformation);
            this.tabTroubleshooting.Location = new System.Drawing.Point(4, 22);
            this.tabTroubleshooting.Name = "tabTroubleshooting";
            this.tabTroubleshooting.Size = new System.Drawing.Size(625, 432);
            this.tabTroubleshooting.TabIndex = 3;
            this.tabTroubleshooting.Text = "Troubleshooting";
            this.tabTroubleshooting.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnClearLoggingInformation);
            this.groupBox3.Controls.Add(this.btnRefreshLoggingInformation);
            this.groupBox3.Controls.Add(this.txtLoggingInformation);
            this.groupBox3.Location = new System.Drawing.Point(8, 73);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(612, 344);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Logging Information";
            // 
            // btnClearLoggingInformation
            // 
            this.btnClearLoggingInformation.Location = new System.Drawing.Point(276, 307);
            this.btnClearLoggingInformation.Name = "btnClearLoggingInformation";
            this.btnClearLoggingInformation.Size = new System.Drawing.Size(102, 31);
            this.btnClearLoggingInformation.TabIndex = 15;
            this.btnClearLoggingInformation.Text = "Clear";
            this.btnClearLoggingInformation.UseVisualStyleBackColor = true;
            this.btnClearLoggingInformation.Click += new System.EventHandler(this.btnClearLoggingInformation_Click);
            // 
            // btnRefreshLoggingInformation
            // 
            this.btnRefreshLoggingInformation.Location = new System.Drawing.Point(168, 307);
            this.btnRefreshLoggingInformation.Name = "btnRefreshLoggingInformation";
            this.btnRefreshLoggingInformation.Size = new System.Drawing.Size(102, 31);
            this.btnRefreshLoggingInformation.TabIndex = 14;
            this.btnRefreshLoggingInformation.Text = "Refresh";
            this.btnRefreshLoggingInformation.UseVisualStyleBackColor = true;
            this.btnRefreshLoggingInformation.Click += new System.EventHandler(this.btnRefreshLoggingInformation_Click);
            // 
            // txtLoggingInformation
            // 
            this.txtLoggingInformation.Location = new System.Drawing.Point(6, 19);
            this.txtLoggingInformation.Multiline = true;
            this.txtLoggingInformation.Name = "txtLoggingInformation";
            this.txtLoggingInformation.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLoggingInformation.Size = new System.Drawing.Size(600, 282);
            this.txtLoggingInformation.TabIndex = 6;
            // 
            // grpBoxLoggingInformation
            // 
            this.grpBoxLoggingInformation.Controls.Add(this.cboLoggingLevel);
            this.grpBoxLoggingInformation.Controls.Add(this.btnApplyLoggingLevel);
            this.grpBoxLoggingInformation.Controls.Add(this.label32);
            this.grpBoxLoggingInformation.Location = new System.Drawing.Point(8, 3);
            this.grpBoxLoggingInformation.Name = "grpBoxLoggingInformation";
            this.grpBoxLoggingInformation.Size = new System.Drawing.Size(612, 64);
            this.grpBoxLoggingInformation.TabIndex = 4;
            this.grpBoxLoggingInformation.TabStop = false;
            this.grpBoxLoggingInformation.Text = "Gateway Logging";
            // 
            // cboLoggingLevel
            // 
            this.cboLoggingLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoggingLevel.FormattingEnabled = true;
            this.cboLoggingLevel.Location = new System.Drawing.Point(116, 26);
            this.cboLoggingLevel.Name = "cboLoggingLevel";
            this.cboLoggingLevel.Size = new System.Drawing.Size(121, 21);
            this.cboLoggingLevel.TabIndex = 12;
            // 
            // btnApplyLoggingLevel
            // 
            this.btnApplyLoggingLevel.Location = new System.Drawing.Point(256, 22);
            this.btnApplyLoggingLevel.Name = "btnApplyLoggingLevel";
            this.btnApplyLoggingLevel.Size = new System.Drawing.Size(102, 27);
            this.btnApplyLoggingLevel.TabIndex = 13;
            this.btnApplyLoggingLevel.Text = "Apply";
            this.btnApplyLoggingLevel.UseVisualStyleBackColor = true;
            this.btnApplyLoggingLevel.Click += new System.EventHandler(this.btnApplyLoggingLevel_Click);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 29);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(54, 13);
            this.label32.TabIndex = 2;
            this.label32.Text = "Log Level";
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.lblLicense);
            this.tabAbout.Controls.Add(this.lblAbout);
            this.tabAbout.Controls.Add(this.linkLabel1);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(625, 432);
            this.tabAbout.TabIndex = 1;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // lblLicense
            // 
            this.lblLicense.AutoSize = true;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLicense.Location = new System.Drawing.Point(210, 140);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(0, 20);
            this.lblLicense.TabIndex = 4;
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAbout
            // 
            this.lblAbout.AutoSize = true;
            this.lblAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbout.Location = new System.Drawing.Point(168, 160);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(60, 20);
            this.lblAbout.TabIndex = 3;
            this.lblAbout.Text = "label12";
            this.lblAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(195, 206);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(162, 20);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://www.twit88.com";
            // 
            // ssApplicationStatus
            // 
            this.ssApplicationStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus,
            this.toolStripMessage,
            this.toolStripGateway});
            this.ssApplicationStatus.Location = new System.Drawing.Point(0, 488);
            this.ssApplicationStatus.Name = "ssApplicationStatus";
            this.ssApplicationStatus.Size = new System.Drawing.Size(634, 22);
            this.ssApplicationStatus.TabIndex = 1;
            this.ssApplicationStatus.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatus.Text = "Offline";
            // 
            // toolStripMessage
            // 
            this.toolStripMessage.Name = "toolStripMessage";
            this.toolStripMessage.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripGateway
            // 
            this.toolStripGateway.Name = "toolStripGateway";
            this.toolStripGateway.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(634, 24);
            this.menuStripMain.TabIndex = 2;
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 510);
            this.Controls.Add(this.ssApplicationStatus);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessagingToolkit - Bulk Gateway";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabMain.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabSubMain.ResumeLayout(false);
            this.tabGatewaySettings.ResumeLayout(false);
            this.grpBoxGatewaySettings.ResumeLayout(false);
            this.grpBoxGatewaySettings.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updSendWaitInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updSendRetries)).EndInit();
            this.tabServiceSettings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabMessaging.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabTroubleshooting.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpBoxLoggingInformation.ResumeLayout(false);
            this.grpBoxLoggingInformation.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.ssApplicationStatus.ResumeLayout(false);
            this.ssApplicationStatus.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.StatusStrip ssApplicationStatus;
        private System.Windows.Forms.TabPage tabMessaging;
        private System.Windows.Forms.TabPage tabTroubleshooting;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lstGateways;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.GroupBox grpBoxLoggingInformation;
        private System.Windows.Forms.ComboBox cboLoggingLevel;
        private System.Windows.Forms.Button btnApplyLoggingLevel;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtLoggingInformation;
        private System.Windows.Forms.Button btnClearLoggingInformation;
        private System.Windows.Forms.Button btnRefreshLoggingInformation;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkWapPush;
        private System.Windows.Forms.CheckBox chkAlertMessage;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboMessageEncoding;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Button btnStopSending;
        private System.Windows.Forms.Button btnStartSending;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox txtWapPushUrl;
        private System.Windows.Forms.Button btnSavePhoneBook;
        private System.Windows.Forms.Button btnImportPhoneBook;
        private System.Windows.Forms.Button btnRemoveAllPhoneBook;
        private System.Windows.Forms.Button btnRemovePhoneBookEntry;
        private System.Windows.Forms.ListBox lstPhoneBook;
        private System.Windows.Forms.Button btnAddPhoneBookEntry;
        private System.Windows.Forms.TextBox txtPhoneBookEntry;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.TextBox txtSmsc;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cboValidityPeriod;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.ComboBox cboLongMessage;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox chkMonitorGatewayStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMessage;
        private System.Windows.Forms.ToolStripStatusLabel toolStripGateway;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnLoadFromFile;
        private System.Windows.Forms.Button btnSaveToFile;
        private System.Windows.Forms.TabControl tabSubMain;
        private System.Windows.Forms.TabPage tabGatewaySettings;
        private System.Windows.Forms.GroupBox grpBoxGatewaySettings;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox txtGatewayId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCancelAddGateway;
        private System.Windows.Forms.Button btnAddGateway;
        private System.Windows.Forms.NumericUpDown updSendWaitInterval;
        private System.Windows.Forms.NumericUpDown updSendRetries;
        private System.Windows.Forms.TextBox txtModelConfig;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboParity;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboStopBits;
        private System.Windows.Forms.ComboBox cboDataBits;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboBaudRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabServiceSettings;
        private System.Windows.Forms.ComboBox cboLoadBalancer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnApplyServiceSettings;
        private System.Windows.Forms.ComboBox cboRouter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.LinkLabel lnkLabelSampleRouter;
        private System.Windows.Forms.LinkLabel lnkLabelSampleBalancer;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtNumberPrefix;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radPhone;
        private System.Windows.Forms.RadioButton radSim;
        private System.Windows.Forms.TextBox txtPersistenceFolder;
        private System.Windows.Forms.CheckBox chkPersistenceQueue;
        private System.Windows.Forms.LinkLabel lnkBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox cboQueuePriority;
        private System.Windows.Forms.CheckBox chkPauseMessageSending;

    }
}

