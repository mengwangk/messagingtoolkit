namespace MessagingToolkit.Barcode.Demo
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
            this.tabPageEncoder = new System.Windows.Forms.TabPage();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEncode = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.cboBarcodeType = new System.Windows.Forms.ComboBox();
            this.panelEncoder = new System.Windows.Forms.Panel();
            this.picEncode = new System.Windows.Forms.PictureBox();
            this.txtBarcodeData = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPageDecoder = new System.Windows.Forms.TabPage();
            this.btnDecode = new System.Windows.Forms.Button();
            this.panelDecode = new System.Windows.Forms.Panel();
            this.picBoxDecode = new System.Windows.Forms.PictureBox();
            this.GB_OPTIONS = new System.Windows.Forms.GroupBox();
            this.chkTryAllFormats = new System.Windows.Forms.CheckBox();
            this.chkTryHarder = new System.Windows.Forms.CheckBox();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.grpBox2DBarcode = new System.Windows.Forms.GroupBox();
            this.chkAll2D = new System.Windows.Forms.CheckBox();
            this.chkPdf417 = new System.Windows.Forms.CheckBox();
            this.chkDataMatrix = new System.Windows.Forms.CheckBox();
            this.chkQRCode = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAll1D = new System.Windows.Forms.CheckBox();
            this.chkItf = new System.Windows.Forms.CheckBox();
            this.chkCode128 = new System.Windows.Forms.CheckBox();
            this.chkCode39 = new System.Windows.Forms.CheckBox();
            this.chkEan13 = new System.Windows.Forms.CheckBox();
            this.chkEan8 = new System.Windows.Forms.CheckBox();
            this.chkUpcA = new System.Windows.Forms.CheckBox();
            this.chkUpcE = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBarcodeResult = new System.Windows.Forms.TextBox();
            this.txtBarcodeDirection = new System.Windows.Forms.TextBox();
            this.txtBarcodeType = new System.Windows.Forms.TextBox();
            this.tabMain.SuspendLayout();
            this.tabPageEncoder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            this.panelEncoder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEncode)).BeginInit();
            this.tabPageDecoder.SuspendLayout();
            this.panelDecode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDecode)).BeginInit();
            this.GB_OPTIONS.SuspendLayout();
            this.grpBox2DBarcode.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPageEncoder);
            this.tabMain.Controls.Add(this.tabPageDecoder);
            this.tabMain.Location = new System.Drawing.Point(1, 21);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(815, 538);
            this.tabMain.TabIndex = 15;
            // 
            // tabPageEncoder
            // 
            this.tabPageEncoder.Controls.Add(this.btnSave);
            this.tabPageEncoder.Controls.Add(this.btnEncode);
            this.tabPageEncoder.Controls.Add(this.label7);
            this.tabPageEncoder.Controls.Add(this.label6);
            this.tabPageEncoder.Controls.Add(this.numHeight);
            this.tabPageEncoder.Controls.Add(this.numWidth);
            this.tabPageEncoder.Controls.Add(this.label5);
            this.tabPageEncoder.Controls.Add(this.cboBarcodeType);
            this.tabPageEncoder.Controls.Add(this.panelEncoder);
            this.tabPageEncoder.Controls.Add(this.txtBarcodeData);
            this.tabPageEncoder.Controls.Add(this.label4);
            this.tabPageEncoder.ForeColor = System.Drawing.Color.White;
            this.tabPageEncoder.Location = new System.Drawing.Point(4, 22);
            this.tabPageEncoder.Name = "tabPageEncoder";
            this.tabPageEncoder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEncoder.Size = new System.Drawing.Size(807, 512);
            this.tabPageEncoder.TabIndex = 1;
            this.tabPageEncoder.Text = "Encoder";
            this.tabPageEncoder.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.ForeColor = System.Drawing.Color.Black;
            this.btnSave.Location = new System.Drawing.Point(118, 443);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 53);
            this.btnSave.TabIndex = 27;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEncode
            // 
            this.btnEncode.ForeColor = System.Drawing.Color.Black;
            this.btnEncode.Location = new System.Drawing.Point(7, 443);
            this.btnEncode.Name = "btnEncode";
            this.btnEncode.Size = new System.Drawing.Size(105, 53);
            this.btnEncode.TabIndex = 26;
            this.btnEncode.Text = "Encode";
            this.btnEncode.UseVisualStyleBackColor = true;
            this.btnEncode.Click += new System.EventHandler(this.btnEncode_Click);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(548, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 23);
            this.label7.TabIndex = 25;
            this.label7.Text = "Height";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(397, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 23);
            this.label6.TabIndex = 24;
            this.label6.Text = "Width";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numHeight
            // 
            this.numHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numHeight.Location = new System.Drawing.Point(626, 9);
            this.numHeight.Maximum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.numHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(66, 26);
            this.numHeight.TabIndex = 23;
            this.numHeight.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // numWidth
            // 
            this.numWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numWidth.Location = new System.Drawing.Point(465, 9);
            this.numWidth.Maximum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.numWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(66, 26);
            this.numWidth.TabIndex = 22;
            this.numWidth.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(6, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(137, 23);
            this.label5.TabIndex = 21;
            this.label5.Text = "Barcode Data";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboBarcodeType
            // 
            this.cboBarcodeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBarcodeType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboBarcodeType.FormattingEnabled = true;
            this.cboBarcodeType.Location = new System.Drawing.Point(149, 9);
            this.cboBarcodeType.Name = "cboBarcodeType";
            this.cboBarcodeType.Size = new System.Drawing.Size(234, 28);
            this.cboBarcodeType.TabIndex = 20;
            // 
            // panelEncoder
            // 
            this.panelEncoder.AutoScroll = true;
            this.panelEncoder.BackColor = System.Drawing.Color.White;
            this.panelEncoder.Controls.Add(this.picEncode);
            this.panelEncoder.Location = new System.Drawing.Point(9, 79);
            this.panelEncoder.Name = "panelEncoder";
            this.panelEncoder.Size = new System.Drawing.Size(791, 358);
            this.panelEncoder.TabIndex = 18;
            // 
            // picEncode
            // 
            this.picEncode.BackColor = System.Drawing.Color.White;
            this.picEncode.Location = new System.Drawing.Point(3, 0);
            this.picEncode.Name = "picEncode";
            this.picEncode.Size = new System.Drawing.Size(128, 78);
            this.picEncode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picEncode.TabIndex = 0;
            this.picEncode.TabStop = false;
            // 
            // txtBarcodeData
            // 
            this.txtBarcodeData.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBarcodeData.Location = new System.Drawing.Point(149, 42);
            this.txtBarcodeData.Name = "txtBarcodeData";
            this.txtBarcodeData.Size = new System.Drawing.Size(651, 26);
            this.txtBarcodeData.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(6, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 23);
            this.label4.TabIndex = 17;
            this.label4.Text = "Barcode Type";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageDecoder
            // 
            this.tabPageDecoder.Controls.Add(this.btnDecode);
            this.tabPageDecoder.Controls.Add(this.panelDecode);
            this.tabPageDecoder.Controls.Add(this.GB_OPTIONS);
            this.tabPageDecoder.Controls.Add(this.btnBrowseFile);
            this.tabPageDecoder.Controls.Add(this.txtFile);
            this.tabPageDecoder.Controls.Add(this.lblTime);
            this.tabPageDecoder.Controls.Add(this.grpBox2DBarcode);
            this.tabPageDecoder.Controls.Add(this.label3);
            this.tabPageDecoder.Controls.Add(this.groupBox1);
            this.tabPageDecoder.Controls.Add(this.label2);
            this.tabPageDecoder.Controls.Add(this.label1);
            this.tabPageDecoder.Controls.Add(this.txtBarcodeResult);
            this.tabPageDecoder.Controls.Add(this.txtBarcodeDirection);
            this.tabPageDecoder.Controls.Add(this.txtBarcodeType);
            this.tabPageDecoder.Location = new System.Drawing.Point(4, 22);
            this.tabPageDecoder.Name = "tabPageDecoder";
            this.tabPageDecoder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDecoder.Size = new System.Drawing.Size(807, 512);
            this.tabPageDecoder.TabIndex = 0;
            this.tabPageDecoder.Text = "Decoder";
            this.tabPageDecoder.UseVisualStyleBackColor = true;
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(7, 428);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(105, 53);
            this.btnDecode.TabIndex = 14;
            this.btnDecode.Text = "Decode";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // panelDecode
            // 
            this.panelDecode.AutoScroll = true;
            this.panelDecode.BackColor = System.Drawing.Color.White;
            this.panelDecode.Controls.Add(this.picBoxDecode);
            this.panelDecode.Location = new System.Drawing.Point(6, 35);
            this.panelDecode.Name = "panelDecode";
            this.panelDecode.Size = new System.Drawing.Size(490, 374);
            this.panelDecode.TabIndex = 13;
            // 
            // picBoxDecode
            // 
            this.picBoxDecode.BackColor = System.Drawing.Color.White;
            this.picBoxDecode.Location = new System.Drawing.Point(3, 0);
            this.picBoxDecode.Name = "picBoxDecode";
            this.picBoxDecode.Size = new System.Drawing.Size(128, 78);
            this.picBoxDecode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBoxDecode.TabIndex = 0;
            this.picBoxDecode.TabStop = false;
            // 
            // GB_OPTIONS
            // 
            this.GB_OPTIONS.BackColor = System.Drawing.Color.White;
            this.GB_OPTIONS.Controls.Add(this.chkTryAllFormats);
            this.GB_OPTIONS.Controls.Add(this.chkTryHarder);
            this.GB_OPTIONS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GB_OPTIONS.Location = new System.Drawing.Point(502, 322);
            this.GB_OPTIONS.Name = "GB_OPTIONS";
            this.GB_OPTIONS.Size = new System.Drawing.Size(295, 87);
            this.GB_OPTIONS.TabIndex = 4;
            this.GB_OPTIONS.TabStop = false;
            this.GB_OPTIONS.Text = "OPTIONS";
            // 
            // chkTryAllFormats
            // 
            this.chkTryAllFormats.AutoSize = true;
            this.chkTryAllFormats.Checked = true;
            this.chkTryAllFormats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTryAllFormats.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTryAllFormats.Location = new System.Drawing.Point(18, 53);
            this.chkTryAllFormats.Name = "chkTryAllFormats";
            this.chkTryAllFormats.Size = new System.Drawing.Size(125, 17);
            this.chkTryAllFormats.TabIndex = 1;
            this.chkTryAllFormats.Tag = "DATAMATRIX";
            this.chkTryAllFormats.Text = "TRY ALL FORMATS";
            this.chkTryAllFormats.UseVisualStyleBackColor = true;
            // 
            // chkTryHarder
            // 
            this.chkTryHarder.AutoSize = true;
            this.chkTryHarder.Checked = true;
            this.chkTryHarder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTryHarder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTryHarder.Location = new System.Drawing.Point(18, 30);
            this.chkTryHarder.Name = "chkTryHarder";
            this.chkTryHarder.Size = new System.Drawing.Size(97, 17);
            this.chkTryHarder.TabIndex = 0;
            this.chkTryHarder.Tag = "";
            this.chkTryHarder.Text = "TRY HARDER";
            this.chkTryHarder.UseVisualStyleBackColor = true;
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Location = new System.Drawing.Point(470, 6);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(26, 23);
            this.btnBrowseFile.TabIndex = 1;
            this.btnBrowseFile.Text = "...";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(6, 9);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(458, 20);
            this.txtFile.TabIndex = 2;
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(523, 467);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(270, 23);
            this.lblTime.TabIndex = 12;
            this.lblTime.Text = "Time: 0000 ms";
            // 
            // grpBox2DBarcode
            // 
            this.grpBox2DBarcode.BackColor = System.Drawing.Color.White;
            this.grpBox2DBarcode.Controls.Add(this.chkAll2D);
            this.grpBox2DBarcode.Controls.Add(this.chkPdf417);
            this.grpBox2DBarcode.Controls.Add(this.chkDataMatrix);
            this.grpBox2DBarcode.Controls.Add(this.chkQRCode);
            this.grpBox2DBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBox2DBarcode.Location = new System.Drawing.Point(502, 9);
            this.grpBox2DBarcode.Name = "grpBox2DBarcode";
            this.grpBox2DBarcode.Size = new System.Drawing.Size(295, 104);
            this.grpBox2DBarcode.TabIndex = 3;
            this.grpBox2DBarcode.TabStop = false;
            this.grpBox2DBarcode.Text = "2D Barcode";
            // 
            // chkAll2D
            // 
            this.chkAll2D.AutoSize = true;
            this.chkAll2D.Checked = true;
            this.chkAll2D.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAll2D.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAll2D.Location = new System.Drawing.Point(171, 0);
            this.chkAll2D.Name = "chkAll2D";
            this.chkAll2D.Size = new System.Drawing.Size(62, 17);
            this.chkAll2D.TabIndex = 3;
            this.chkAll2D.Tag = "ALL";
            this.chkAll2D.Text = "ALL 2D";
            this.chkAll2D.UseVisualStyleBackColor = true;
            this.chkAll2D.CheckedChanged += new System.EventHandler(this.chkAll2D_CheckedChanged);
            // 
            // chkPdf417
            // 
            this.chkPdf417.AutoSize = true;
            this.chkPdf417.Checked = true;
            this.chkPdf417.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPdf417.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPdf417.Location = new System.Drawing.Point(18, 76);
            this.chkPdf417.Name = "chkPdf417";
            this.chkPdf417.Size = new System.Drawing.Size(65, 17);
            this.chkPdf417.TabIndex = 2;
            this.chkPdf417.Tag = "PDF417";
            this.chkPdf417.Text = "PDF417";
            this.chkPdf417.UseVisualStyleBackColor = true;
            // 
            // chkDataMatrix
            // 
            this.chkDataMatrix.AutoSize = true;
            this.chkDataMatrix.Checked = true;
            this.chkDataMatrix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDataMatrix.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDataMatrix.Location = new System.Drawing.Point(18, 53);
            this.chkDataMatrix.Name = "chkDataMatrix";
            this.chkDataMatrix.Size = new System.Drawing.Size(77, 17);
            this.chkDataMatrix.TabIndex = 1;
            this.chkDataMatrix.Tag = "DATAMATRIX";
            this.chkDataMatrix.Text = "Data Marix";
            this.chkDataMatrix.UseVisualStyleBackColor = true;
            // 
            // chkQRCode
            // 
            this.chkQRCode.AutoSize = true;
            this.chkQRCode.Checked = true;
            this.chkQRCode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQRCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkQRCode.Location = new System.Drawing.Point(18, 30);
            this.chkQRCode.Name = "chkQRCode";
            this.chkQRCode.Size = new System.Drawing.Size(75, 17);
            this.chkQRCode.TabIndex = 0;
            this.chkQRCode.Tag = "QR_CODE";
            this.chkQRCode.Text = "QR CODE";
            this.chkQRCode.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(120, 464);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "Barcode Direction";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.chkAll1D);
            this.groupBox1.Controls.Add(this.chkItf);
            this.groupBox1.Controls.Add(this.chkCode128);
            this.groupBox1.Controls.Add(this.chkCode39);
            this.groupBox1.Controls.Add(this.chkEan13);
            this.groupBox1.Controls.Add(this.chkEan8);
            this.groupBox1.Controls.Add(this.chkUpcA);
            this.groupBox1.Controls.Add(this.chkUpcE);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(502, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 197);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1D Barcode";
            // 
            // chkAll1D
            // 
            this.chkAll1D.AutoSize = true;
            this.chkAll1D.Checked = true;
            this.chkAll1D.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAll1D.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAll1D.Location = new System.Drawing.Point(171, 0);
            this.chkAll1D.Name = "chkAll1D";
            this.chkAll1D.Size = new System.Drawing.Size(104, 17);
            this.chkAll1D.TabIndex = 4;
            this.chkAll1D.Tag = "ALL";
            this.chkAll1D.Text = "ALL 1D Brcodes";
            this.chkAll1D.UseVisualStyleBackColor = true;
            this.chkAll1D.CheckedChanged += new System.EventHandler(this.chkAll1D_CheckedChanged);
            // 
            // chkItf
            // 
            this.chkItf.AutoSize = true;
            this.chkItf.Checked = true;
            this.chkItf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkItf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkItf.Location = new System.Drawing.Point(18, 168);
            this.chkItf.Name = "chkItf";
            this.chkItf.Size = new System.Drawing.Size(109, 17);
            this.chkItf.TabIndex = 6;
            this.chkItf.Tag = "ITF";
            this.chkItf.Text = "Interleaved 2 of 5";
            this.chkItf.UseVisualStyleBackColor = true;
            // 
            // chkCode128
            // 
            this.chkCode128.AutoSize = true;
            this.chkCode128.Checked = true;
            this.chkCode128.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCode128.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCode128.Location = new System.Drawing.Point(18, 145);
            this.chkCode128.Name = "chkCode128";
            this.chkCode128.Size = new System.Drawing.Size(77, 17);
            this.chkCode128.TabIndex = 5;
            this.chkCode128.Tag = "CODE_128";
            this.chkCode128.Text = "CODE 128";
            this.chkCode128.UseVisualStyleBackColor = true;
            // 
            // chkCode39
            // 
            this.chkCode39.AutoSize = true;
            this.chkCode39.Checked = true;
            this.chkCode39.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCode39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCode39.Location = new System.Drawing.Point(18, 122);
            this.chkCode39.Name = "chkCode39";
            this.chkCode39.Size = new System.Drawing.Size(71, 17);
            this.chkCode39.TabIndex = 4;
            this.chkCode39.Tag = "CODE_39";
            this.chkCode39.Text = "CODE 39";
            this.chkCode39.UseVisualStyleBackColor = true;
            // 
            // chkEan13
            // 
            this.chkEan13.AutoSize = true;
            this.chkEan13.Checked = true;
            this.chkEan13.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEan13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEan13.Location = new System.Drawing.Point(18, 99);
            this.chkEan13.Name = "chkEan13";
            this.chkEan13.Size = new System.Drawing.Size(66, 17);
            this.chkEan13.TabIndex = 3;
            this.chkEan13.Tag = "EAN_13";
            this.chkEan13.Text = "EAN  13";
            this.chkEan13.UseVisualStyleBackColor = true;
            // 
            // chkEan8
            // 
            this.chkEan8.AutoSize = true;
            this.chkEan8.Checked = true;
            this.chkEan8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEan8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEan8.Location = new System.Drawing.Point(18, 76);
            this.chkEan8.Name = "chkEan8";
            this.chkEan8.Size = new System.Drawing.Size(57, 17);
            this.chkEan8.TabIndex = 2;
            this.chkEan8.Tag = "EAN_8";
            this.chkEan8.Text = "EAN 8";
            this.chkEan8.UseVisualStyleBackColor = true;
            // 
            // chkUpcA
            // 
            this.chkUpcA.AutoSize = true;
            this.chkUpcA.Checked = true;
            this.chkUpcA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpcA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUpcA.Location = new System.Drawing.Point(18, 53);
            this.chkUpcA.Name = "chkUpcA";
            this.chkUpcA.Size = new System.Drawing.Size(58, 17);
            this.chkUpcA.TabIndex = 1;
            this.chkUpcA.Tag = "UPC_A";
            this.chkUpcA.Text = "UPC A";
            this.chkUpcA.UseVisualStyleBackColor = true;
            // 
            // chkUpcE
            // 
            this.chkUpcE.AutoSize = true;
            this.chkUpcE.Checked = true;
            this.chkUpcE.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpcE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUpcE.Location = new System.Drawing.Point(18, 30);
            this.chkUpcE.Name = "chkUpcE";
            this.chkUpcE.Size = new System.Drawing.Size(58, 17);
            this.chkUpcE.TabIndex = 0;
            this.chkUpcE.Tag = "UPC_E";
            this.chkUpcE.Text = "UPC E";
            this.chkUpcE.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(120, 438);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 23);
            this.label2.TabIndex = 10;
            this.label2.Text = "Barcode Result";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(120, 415);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "Barcode Type";
            // 
            // txtBarcodeResult
            // 
            this.txtBarcodeResult.Location = new System.Drawing.Point(263, 441);
            this.txtBarcodeResult.Name = "txtBarcodeResult";
            this.txtBarcodeResult.Size = new System.Drawing.Size(534, 20);
            this.txtBarcodeResult.TabIndex = 6;
            // 
            // txtBarcodeDirection
            // 
            this.txtBarcodeDirection.Location = new System.Drawing.Point(263, 466);
            this.txtBarcodeDirection.Name = "txtBarcodeDirection";
            this.txtBarcodeDirection.Size = new System.Drawing.Size(254, 20);
            this.txtBarcodeDirection.TabIndex = 8;
            // 
            // txtBarcodeType
            // 
            this.txtBarcodeType.Location = new System.Drawing.Point(263, 415);
            this.txtBarcodeType.Name = "txtBarcodeType";
            this.txtBarcodeType.Size = new System.Drawing.Size(534, 20);
            this.txtBarcodeType.TabIndex = 7;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 569);
            this.Controls.Add(this.tabMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessagingToolkit Barcode - Demo";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabMain.ResumeLayout(false);
            this.tabPageEncoder.ResumeLayout(false);
            this.tabPageEncoder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            this.panelEncoder.ResumeLayout(false);
            this.panelEncoder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEncode)).EndInit();
            this.tabPageDecoder.ResumeLayout(false);
            this.tabPageDecoder.PerformLayout();
            this.panelDecode.ResumeLayout(false);
            this.panelDecode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxDecode)).EndInit();
            this.GB_OPTIONS.ResumeLayout(false);
            this.GB_OPTIONS.PerformLayout();
            this.grpBox2DBarcode.ResumeLayout(false);
            this.grpBox2DBarcode.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPageDecoder;
        private System.Windows.Forms.Panel panelDecode;
        private System.Windows.Forms.PictureBox picBoxDecode;
        private System.Windows.Forms.GroupBox GB_OPTIONS;
        private System.Windows.Forms.CheckBox chkTryAllFormats;
        private System.Windows.Forms.CheckBox chkTryHarder;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.GroupBox grpBox2DBarcode;
        private System.Windows.Forms.CheckBox chkAll2D;
        private System.Windows.Forms.CheckBox chkPdf417;
        private System.Windows.Forms.CheckBox chkDataMatrix;
        private System.Windows.Forms.CheckBox chkQRCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkAll1D;
        private System.Windows.Forms.CheckBox chkItf;
        private System.Windows.Forms.CheckBox chkCode128;
        private System.Windows.Forms.CheckBox chkCode39;
        private System.Windows.Forms.CheckBox chkEan13;
        private System.Windows.Forms.CheckBox chkEan8;
        private System.Windows.Forms.CheckBox chkUpcA;
        private System.Windows.Forms.CheckBox chkUpcE;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBarcodeResult;
        private System.Windows.Forms.TextBox txtBarcodeDirection;
        private System.Windows.Forms.TextBox txtBarcodeType;
        private System.Windows.Forms.TabPage tabPageEncoder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboBarcodeType;
        private System.Windows.Forms.Panel panelEncoder;
        private System.Windows.Forms.PictureBox picEncode;
        private System.Windows.Forms.TextBox txtBarcodeData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnEncode;
    }
}

