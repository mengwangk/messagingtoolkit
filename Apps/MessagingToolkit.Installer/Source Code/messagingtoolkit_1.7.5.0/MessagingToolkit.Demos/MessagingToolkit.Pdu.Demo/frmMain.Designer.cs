namespace MessagingToolkit.Pdu.Demo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPduEncoder = new System.Windows.Forms.TabPage();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.btnGetPduCode = new System.Windows.Forms.Button();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.txtPduCode = new System.Windows.Forms.TextBox();
            this.groupBox31 = new System.Windows.Forms.GroupBox();
            this.txtUserData = new System.Windows.Forms.TextBox();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.cboMessageClass = new System.Windows.Forms.ComboBox();
            this.label91 = new System.Windows.Forms.Label();
            this.txtReplyPath = new System.Windows.Forms.TextBox();
            this.label84 = new System.Windows.Forms.Label();
            this.nupdPduMessageReferenceNo = new System.Windows.Forms.NumericUpDown();
            this.nupdPduValidityPeriod = new System.Windows.Forms.NumericUpDown();
            this.nupdPduDestinationPort = new System.Windows.Forms.NumericUpDown();
            this.nupdPduSourcePort = new System.Windows.Forms.NumericUpDown();
            this.cboPduEncoding = new System.Windows.Forms.ComboBox();
            this.chkPduFlashMessage = new System.Windows.Forms.CheckBox();
            this.chkPduStatusReport = new System.Windows.Forms.CheckBox();
            this.label61 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.groupBox29 = new System.Windows.Forms.GroupBox();
            this.txtPduDestinationNumber = new System.Windows.Forms.TextBox();
            this.txtPduServiceCentreAddress = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.tabPduDecoder = new System.Windows.Forms.TabPage();
            this.btnDecodePdu = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.txtDecodedPdu = new System.Windows.Forms.TextBox();
            this.txtPdu = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tabWap = new System.Windows.Forms.TabPage();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblAbout = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.dtpWapPushExpiryDate = new System.Windows.Forms.DateTimePicker();
            this.btnGetWapPushPDU = new System.Windows.Forms.Button();
            this.chkWapPushExpiry = new System.Windows.Forms.CheckBox();
            this.dtpWapPushCreated = new System.Windows.Forms.DateTimePicker();
            this.chkWapPushCreated = new System.Windows.Forms.CheckBox();
            this.cboWapPushSignal = new System.Windows.Forms.ComboBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.txtWapPushMessage = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtWapPushUrl = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtWapPushPhoneNumber = new System.Windows.Forms.TextBox();
            this.txtWapPushPDU = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabMain.SuspendLayout();
            this.tabPduEncoder.SuspendLayout();
            this.groupBox22.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.groupBox31.SuspendLayout();
            this.groupBox30.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduMessageReferenceNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduValidityPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduDestinationPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduSourcePort)).BeginInit();
            this.groupBox29.SuspendLayout();
            this.tabPduDecoder.SuspendLayout();
            this.tabWap.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPduEncoder);
            this.tabMain.Controls.Add(this.tabPduDecoder);
            this.tabMain.Controls.Add(this.tabWap);
            this.tabMain.Controls.Add(this.tabAbout);
            this.tabMain.Location = new System.Drawing.Point(0, 2);
            this.tabMain.Multiline = true;
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(578, 477);
            this.tabMain.TabIndex = 0;
            this.tabMain.Click += new System.EventHandler(this.tabMain_Click);
            // 
            // tabPduEncoder
            // 
            this.tabPduEncoder.Controls.Add(this.groupBox22);
            this.tabPduEncoder.Location = new System.Drawing.Point(4, 22);
            this.tabPduEncoder.Name = "tabPduEncoder";
            this.tabPduEncoder.Size = new System.Drawing.Size(570, 451);
            this.tabPduEncoder.TabIndex = 12;
            this.tabPduEncoder.Text = "PDU Encoder";
            this.tabPduEncoder.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.btnGetPduCode);
            this.groupBox22.Controls.Add(this.groupBox32);
            this.groupBox22.Controls.Add(this.groupBox31);
            this.groupBox22.Controls.Add(this.groupBox30);
            this.groupBox22.Controls.Add(this.groupBox29);
            this.groupBox22.Location = new System.Drawing.Point(3, 0);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(531, 413);
            this.groupBox22.TabIndex = 2;
            this.groupBox22.TabStop = false;
            // 
            // btnGetPduCode
            // 
            this.btnGetPduCode.Location = new System.Drawing.Point(169, 364);
            this.btnGetPduCode.Name = "btnGetPduCode";
            this.btnGetPduCode.Size = new System.Drawing.Size(187, 32);
            this.btnGetPduCode.TabIndex = 6;
            this.btnGetPduCode.Text = "Get PDU Code";
            this.btnGetPduCode.UseVisualStyleBackColor = true;
            this.btnGetPduCode.Click += new System.EventHandler(this.btnGetPduCode_Click);
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.txtPduCode);
            this.groupBox32.Location = new System.Drawing.Point(260, 148);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(272, 210);
            this.groupBox32.TabIndex = 5;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "PDU Code";
            // 
            // txtPduCode
            // 
            this.txtPduCode.Location = new System.Drawing.Point(6, 22);
            this.txtPduCode.Multiline = true;
            this.txtPduCode.Name = "txtPduCode";
            this.txtPduCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPduCode.Size = new System.Drawing.Size(260, 182);
            this.txtPduCode.TabIndex = 7;
            // 
            // groupBox31
            // 
            this.groupBox31.Controls.Add(this.txtUserData);
            this.groupBox31.Location = new System.Drawing.Point(260, 19);
            this.groupBox31.Name = "groupBox31";
            this.groupBox31.Size = new System.Drawing.Size(272, 123);
            this.groupBox31.TabIndex = 4;
            this.groupBox31.TabStop = false;
            this.groupBox31.Text = "User Data";
            // 
            // txtUserData
            // 
            this.txtUserData.Location = new System.Drawing.Point(6, 19);
            this.txtUserData.Multiline = true;
            this.txtUserData.Name = "txtUserData";
            this.txtUserData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtUserData.Size = new System.Drawing.Size(260, 86);
            this.txtUserData.TabIndex = 6;
            // 
            // groupBox30
            // 
            this.groupBox30.Controls.Add(this.cboMessageClass);
            this.groupBox30.Controls.Add(this.label91);
            this.groupBox30.Controls.Add(this.txtReplyPath);
            this.groupBox30.Controls.Add(this.label84);
            this.groupBox30.Controls.Add(this.nupdPduMessageReferenceNo);
            this.groupBox30.Controls.Add(this.nupdPduValidityPeriod);
            this.groupBox30.Controls.Add(this.nupdPduDestinationPort);
            this.groupBox30.Controls.Add(this.nupdPduSourcePort);
            this.groupBox30.Controls.Add(this.cboPduEncoding);
            this.groupBox30.Controls.Add(this.chkPduFlashMessage);
            this.groupBox30.Controls.Add(this.chkPduStatusReport);
            this.groupBox30.Controls.Add(this.label61);
            this.groupBox30.Controls.Add(this.label60);
            this.groupBox30.Controls.Add(this.label59);
            this.groupBox30.Controls.Add(this.label57);
            this.groupBox30.Controls.Add(this.label58);
            this.groupBox30.Location = new System.Drawing.Point(6, 113);
            this.groupBox30.Name = "groupBox30";
            this.groupBox30.Size = new System.Drawing.Size(248, 245);
            this.groupBox30.TabIndex = 3;
            this.groupBox30.TabStop = false;
            this.groupBox30.Text = "Options";
            // 
            // cboMessageClass
            // 
            this.cboMessageClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageClass.FormattingEnabled = true;
            this.cboMessageClass.Location = new System.Drawing.Point(126, 180);
            this.cboMessageClass.Name = "cboMessageClass";
            this.cboMessageClass.Size = new System.Drawing.Size(117, 21);
            this.cboMessageClass.TabIndex = 19;
            // 
            // label91
            // 
            this.label91.AutoSize = true;
            this.label91.Location = new System.Drawing.Point(6, 183);
            this.label91.Name = "label91";
            this.label91.Size = new System.Drawing.Size(78, 13);
            this.label91.TabIndex = 18;
            this.label91.Text = "Message Class";
            // 
            // txtReplyPath
            // 
            this.txtReplyPath.Location = new System.Drawing.Point(125, 156);
            this.txtReplyPath.Name = "txtReplyPath";
            this.txtReplyPath.Size = new System.Drawing.Size(117, 20);
            this.txtReplyPath.TabIndex = 17;
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(6, 159);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(59, 13);
            this.label84.TabIndex = 16;
            this.label84.Text = "Reply Path";
            // 
            // nupdPduMessageReferenceNo
            // 
            this.nupdPduMessageReferenceNo.Location = new System.Drawing.Point(126, 78);
            this.nupdPduMessageReferenceNo.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nupdPduMessageReferenceNo.Name = "nupdPduMessageReferenceNo";
            this.nupdPduMessageReferenceNo.Size = new System.Drawing.Size(116, 20);
            this.nupdPduMessageReferenceNo.TabIndex = 15;
            // 
            // nupdPduValidityPeriod
            // 
            this.nupdPduValidityPeriod.DecimalPlaces = 2;
            this.nupdPduValidityPeriod.Location = new System.Drawing.Point(126, 51);
            this.nupdPduValidityPeriod.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nupdPduValidityPeriod.Name = "nupdPduValidityPeriod";
            this.nupdPduValidityPeriod.Size = new System.Drawing.Size(116, 20);
            this.nupdPduValidityPeriod.TabIndex = 14;
            this.nupdPduValidityPeriod.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nupdPduDestinationPort
            // 
            this.nupdPduDestinationPort.Location = new System.Drawing.Point(126, 129);
            this.nupdPduDestinationPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nupdPduDestinationPort.Name = "nupdPduDestinationPort";
            this.nupdPduDestinationPort.Size = new System.Drawing.Size(117, 20);
            this.nupdPduDestinationPort.TabIndex = 13;
            // 
            // nupdPduSourcePort
            // 
            this.nupdPduSourcePort.Location = new System.Drawing.Point(126, 103);
            this.nupdPduSourcePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nupdPduSourcePort.Name = "nupdPduSourcePort";
            this.nupdPduSourcePort.Size = new System.Drawing.Size(116, 20);
            this.nupdPduSourcePort.TabIndex = 12;
            // 
            // cboPduEncoding
            // 
            this.cboPduEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPduEncoding.FormattingEnabled = true;
            this.cboPduEncoding.Location = new System.Drawing.Point(126, 22);
            this.cboPduEncoding.Name = "cboPduEncoding";
            this.cboPduEncoding.Size = new System.Drawing.Size(117, 21);
            this.cboPduEncoding.TabIndex = 9;
            // 
            // chkPduFlashMessage
            // 
            this.chkPduFlashMessage.AutoSize = true;
            this.chkPduFlashMessage.Location = new System.Drawing.Point(125, 213);
            this.chkPduFlashMessage.Name = "chkPduFlashMessage";
            this.chkPduFlashMessage.Size = new System.Drawing.Size(97, 17);
            this.chkPduFlashMessage.TabIndex = 6;
            this.chkPduFlashMessage.Text = "Flash Message";
            this.chkPduFlashMessage.UseVisualStyleBackColor = true;
            // 
            // chkPduStatusReport
            // 
            this.chkPduStatusReport.AutoSize = true;
            this.chkPduStatusReport.Location = new System.Drawing.Point(9, 213);
            this.chkPduStatusReport.Name = "chkPduStatusReport";
            this.chkPduStatusReport.Size = new System.Drawing.Size(91, 17);
            this.chkPduStatusReport.TabIndex = 5;
            this.chkPduStatusReport.Text = "Status Report";
            this.chkPduStatusReport.UseVisualStyleBackColor = true;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(6, 133);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(82, 13);
            this.label61.TabIndex = 4;
            this.label61.Text = "Destination Port";
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(6, 107);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(63, 13);
            this.label60.TabIndex = 3;
            this.label60.Text = "Source Port";
            // 
            // label59
            // 
            this.label59.AutoSize = true;
            this.label59.Location = new System.Drawing.Point(6, 80);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(120, 13);
            this.label59.TabIndex = 2;
            this.label59.Text = "Message Reference No";
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(6, 54);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(105, 13);
            this.label57.TabIndex = 1;
            this.label57.Text = "Validity Period (Hour)";
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(6, 25);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(52, 13);
            this.label58.TabIndex = 0;
            this.label58.Text = "Encoding";
            // 
            // groupBox29
            // 
            this.groupBox29.Controls.Add(this.txtPduDestinationNumber);
            this.groupBox29.Controls.Add(this.txtPduServiceCentreAddress);
            this.groupBox29.Controls.Add(this.label56);
            this.groupBox29.Controls.Add(this.label55);
            this.groupBox29.Location = new System.Drawing.Point(6, 19);
            this.groupBox29.Name = "groupBox29";
            this.groupBox29.Size = new System.Drawing.Size(248, 88);
            this.groupBox29.TabIndex = 2;
            this.groupBox29.TabStop = false;
            this.groupBox29.Text = "Number Information";
            // 
            // txtPduDestinationNumber
            // 
            this.txtPduDestinationNumber.Location = new System.Drawing.Point(126, 57);
            this.txtPduDestinationNumber.Name = "txtPduDestinationNumber";
            this.txtPduDestinationNumber.Size = new System.Drawing.Size(111, 20);
            this.txtPduDestinationNumber.TabIndex = 3;
            // 
            // txtPduServiceCentreAddress
            // 
            this.txtPduServiceCentreAddress.Location = new System.Drawing.Point(126, 23);
            this.txtPduServiceCentreAddress.Name = "txtPduServiceCentreAddress";
            this.txtPduServiceCentreAddress.Size = new System.Drawing.Size(111, 20);
            this.txtPduServiceCentreAddress.TabIndex = 2;
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(3, 57);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(100, 13);
            this.label56.TabIndex = 1;
            this.label56.Text = "Destination Number";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(3, 26);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(117, 13);
            this.label55.TabIndex = 0;
            this.label55.Text = "Service Centre Number";
            // 
            // tabPduDecoder
            // 
            this.tabPduDecoder.Controls.Add(this.btnDecodePdu);
            this.tabPduDecoder.Controls.Add(this.label20);
            this.tabPduDecoder.Controls.Add(this.txtDecodedPdu);
            this.tabPduDecoder.Controls.Add(this.txtPdu);
            this.tabPduDecoder.Controls.Add(this.label19);
            this.tabPduDecoder.Location = new System.Drawing.Point(4, 22);
            this.tabPduDecoder.Name = "tabPduDecoder";
            this.tabPduDecoder.Size = new System.Drawing.Size(570, 451);
            this.tabPduDecoder.TabIndex = 13;
            this.tabPduDecoder.Text = "PDU Decoder";
            this.tabPduDecoder.UseVisualStyleBackColor = true;
            // 
            // btnDecodePdu
            // 
            this.btnDecodePdu.Location = new System.Drawing.Point(202, 333);
            this.btnDecodePdu.Name = "btnDecodePdu";
            this.btnDecodePdu.Size = new System.Drawing.Size(134, 35);
            this.btnDecodePdu.TabIndex = 4;
            this.btnDecodePdu.Text = "Decode";
            this.btnDecodePdu.UseVisualStyleBackColor = true;
            this.btnDecodePdu.Click += new System.EventHandler(this.btnDecodePdu_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 209);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(77, 13);
            this.label20.TabIndex = 3;
            this.label20.Text = "Decoded PDU";
            // 
            // txtDecodedPdu
            // 
            this.txtDecodedPdu.AcceptsReturn = true;
            this.txtDecodedPdu.Location = new System.Drawing.Point(82, 116);
            this.txtDecodedPdu.Multiline = true;
            this.txtDecodedPdu.Name = "txtDecodedPdu";
            this.txtDecodedPdu.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDecodedPdu.Size = new System.Drawing.Size(434, 199);
            this.txtDecodedPdu.TabIndex = 2;
            // 
            // txtPdu
            // 
            this.txtPdu.Location = new System.Drawing.Point(82, 16);
            this.txtPdu.Multiline = true;
            this.txtPdu.Name = "txtPdu";
            this.txtPdu.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPdu.Size = new System.Drawing.Size(434, 80);
            this.txtPdu.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(26, 46);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(30, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "PDU";
            // 
            // tabWap
            // 
            this.tabWap.Controls.Add(this.groupBox16);
            this.tabWap.Location = new System.Drawing.Point(4, 22);
            this.tabWap.Name = "tabWap";
            this.tabWap.Size = new System.Drawing.Size(570, 451);
            this.tabWap.TabIndex = 14;
            this.tabWap.Text = "WAP Message";
            this.tabWap.UseVisualStyleBackColor = true;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.groupBox6);
            this.tabAbout.Location = new System.Drawing.Point(4, 22);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(570, 451);
            this.tabAbout.TabIndex = 2;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblAbout);
            this.groupBox6.Controls.Add(this.linkLabel1);
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(525, 401);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            // 
            // lblAbout
            // 
            this.lblAbout.AutoSize = true;
            this.lblAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbout.Location = new System.Drawing.Point(156, 148);
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
            this.linkLabel1.Location = new System.Drawing.Point(156, 195);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(162, 20);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://www.twit88.com";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.label1);
            this.groupBox16.Controls.Add(this.txtWapPushPDU);
            this.groupBox16.Controls.Add(this.dtpWapPushExpiryDate);
            this.groupBox16.Controls.Add(this.btnGetWapPushPDU);
            this.groupBox16.Controls.Add(this.chkWapPushExpiry);
            this.groupBox16.Controls.Add(this.dtpWapPushCreated);
            this.groupBox16.Controls.Add(this.chkWapPushCreated);
            this.groupBox16.Controls.Add(this.cboWapPushSignal);
            this.groupBox16.Controls.Add(this.label39);
            this.groupBox16.Controls.Add(this.label38);
            this.groupBox16.Controls.Add(this.txtWapPushMessage);
            this.groupBox16.Controls.Add(this.label37);
            this.groupBox16.Controls.Add(this.txtWapPushUrl);
            this.groupBox16.Controls.Add(this.label36);
            this.groupBox16.Controls.Add(this.txtWapPushPhoneNumber);
            this.groupBox16.Location = new System.Drawing.Point(8, 3);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(536, 433);
            this.groupBox16.TabIndex = 1;
            this.groupBox16.TabStop = false;
            // 
            // dtpWapPushExpiryDate
            // 
            this.dtpWapPushExpiryDate.Enabled = false;
            this.dtpWapPushExpiryDate.Location = new System.Drawing.Point(165, 230);
            this.dtpWapPushExpiryDate.Name = "dtpWapPushExpiryDate";
            this.dtpWapPushExpiryDate.Size = new System.Drawing.Size(155, 20);
            this.dtpWapPushExpiryDate.TabIndex = 16;
            // 
            // btnGetWapPushPDU
            // 
            this.btnGetWapPushPDU.Location = new System.Drawing.Point(96, 386);
            this.btnGetWapPushPDU.Name = "btnGetWapPushPDU";
            this.btnGetWapPushPDU.Size = new System.Drawing.Size(155, 41);
            this.btnGetWapPushPDU.TabIndex = 15;
            this.btnGetWapPushPDU.Text = "Get PDU Code";
            this.btnGetWapPushPDU.UseVisualStyleBackColor = true;
            this.btnGetWapPushPDU.Click += new System.EventHandler(this.btnGetWapPushPDU_Click);
            // 
            // chkWapPushExpiry
            // 
            this.chkWapPushExpiry.AutoSize = true;
            this.chkWapPushExpiry.Location = new System.Drawing.Point(96, 233);
            this.chkWapPushExpiry.Name = "chkWapPushExpiry";
            this.chkWapPushExpiry.Size = new System.Drawing.Size(54, 17);
            this.chkWapPushExpiry.TabIndex = 12;
            this.chkWapPushExpiry.Text = "Expiry";
            this.chkWapPushExpiry.UseVisualStyleBackColor = true;
            // 
            // dtpWapPushCreated
            // 
            this.dtpWapPushCreated.Enabled = false;
            this.dtpWapPushCreated.Location = new System.Drawing.Point(165, 198);
            this.dtpWapPushCreated.Name = "dtpWapPushCreated";
            this.dtpWapPushCreated.Size = new System.Drawing.Size(155, 20);
            this.dtpWapPushCreated.TabIndex = 11;
            // 
            // chkWapPushCreated
            // 
            this.chkWapPushCreated.AutoSize = true;
            this.chkWapPushCreated.Location = new System.Drawing.Point(96, 201);
            this.chkWapPushCreated.Name = "chkWapPushCreated";
            this.chkWapPushCreated.Size = new System.Drawing.Size(63, 17);
            this.chkWapPushCreated.TabIndex = 10;
            this.chkWapPushCreated.Text = "Created";
            this.chkWapPushCreated.UseVisualStyleBackColor = true;
            // 
            // cboWapPushSignal
            // 
            this.cboWapPushSignal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWapPushSignal.FormattingEnabled = true;
            this.cboWapPushSignal.Location = new System.Drawing.Point(96, 157);
            this.cboWapPushSignal.Name = "cboWapPushSignal";
            this.cboWapPushSignal.Size = new System.Drawing.Size(121, 21);
            this.cboWapPushSignal.TabIndex = 9;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(9, 160);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(36, 13);
            this.label39.TabIndex = 8;
            this.label39.Text = "Signal";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(9, 92);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(50, 13);
            this.label38.TabIndex = 6;
            this.label38.Text = "Message";
            // 
            // txtWapPushMessage
            // 
            this.txtWapPushMessage.Location = new System.Drawing.Point(96, 89);
            this.txtWapPushMessage.Multiline = true;
            this.txtWapPushMessage.Name = "txtWapPushMessage";
            this.txtWapPushMessage.Size = new System.Drawing.Size(376, 62);
            this.txtWapPushMessage.TabIndex = 7;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(9, 66);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(29, 13);
            this.label37.TabIndex = 4;
            this.label37.Text = "URL";
            // 
            // txtWapPushUrl
            // 
            this.txtWapPushUrl.Location = new System.Drawing.Point(96, 63);
            this.txtWapPushUrl.Name = "txtWapPushUrl";
            this.txtWapPushUrl.Size = new System.Drawing.Size(434, 20);
            this.txtWapPushUrl.TabIndex = 5;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(9, 31);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(78, 13);
            this.label36.TabIndex = 2;
            this.label36.Text = "Phone Number";
            // 
            // txtWapPushPhoneNumber
            // 
            this.txtWapPushPhoneNumber.Location = new System.Drawing.Point(96, 28);
            this.txtWapPushPhoneNumber.Name = "txtWapPushPhoneNumber";
            this.txtWapPushPhoneNumber.Size = new System.Drawing.Size(229, 20);
            this.txtWapPushPhoneNumber.TabIndex = 3;
            // 
            // txtWapPushPDU
            // 
            this.txtWapPushPDU.Location = new System.Drawing.Point(96, 274);
            this.txtWapPushPDU.Multiline = true;
            this.txtWapPushPDU.Name = "txtWapPushPDU";
            this.txtWapPushPDU.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtWapPushPDU.Size = new System.Drawing.Size(301, 106);
            this.txtWapPushPDU.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 307);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "PDU Code";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 483);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessagingToolkit - PDU Demo";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabMain.ResumeLayout(false);
            this.tabPduEncoder.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            this.groupBox31.ResumeLayout(false);
            this.groupBox31.PerformLayout();
            this.groupBox30.ResumeLayout(false);
            this.groupBox30.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduMessageReferenceNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduValidityPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduDestinationPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupdPduSourcePort)).EndInit();
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            this.tabPduDecoder.ResumeLayout(false);
            this.tabPduDecoder.PerformLayout();
            this.tabWap.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.TabPage tabPduEncoder;
        private System.Windows.Forms.TabPage tabPduDecoder;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.TextBox txtDecodedPdu;
        private System.Windows.Forms.TextBox txtPdu;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnDecodePdu;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.GroupBox groupBox29;
        private System.Windows.Forms.GroupBox groupBox32;
        private System.Windows.Forms.GroupBox groupBox31;
        private System.Windows.Forms.GroupBox groupBox30;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.CheckBox chkPduFlashMessage;
        private System.Windows.Forms.CheckBox chkPduStatusReport;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.TextBox txtPduCode;
        private System.Windows.Forms.TextBox txtUserData;
        private System.Windows.Forms.TextBox txtPduDestinationNumber;
        private System.Windows.Forms.TextBox txtPduServiceCentreAddress;
        private System.Windows.Forms.Button btnGetPduCode;
        private System.Windows.Forms.ComboBox cboPduEncoding;
        private System.Windows.Forms.NumericUpDown nupdPduDestinationPort;
        private System.Windows.Forms.NumericUpDown nupdPduSourcePort;
        private System.Windows.Forms.NumericUpDown nupdPduValidityPeriod;
        private System.Windows.Forms.NumericUpDown nupdPduMessageReferenceNo;
        private System.Windows.Forms.TextBox txtReplyPath;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.ComboBox cboMessageClass;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.TabPage tabWap;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.DateTimePicker dtpWapPushExpiryDate;
        private System.Windows.Forms.Button btnGetWapPushPDU;
        private System.Windows.Forms.CheckBox chkWapPushExpiry;
        private System.Windows.Forms.DateTimePicker dtpWapPushCreated;
        private System.Windows.Forms.CheckBox chkWapPushCreated;
        private System.Windows.Forms.ComboBox cboWapPushSignal;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox txtWapPushMessage;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox txtWapPushUrl;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox txtWapPushPhoneNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWapPushPDU;
    }
}

