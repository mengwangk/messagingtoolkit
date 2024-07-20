namespace MessagingToolkit.MMS.Demo
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
            this.tabEncoder = new System.Windows.Forms.TabPage();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtContentId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lstContent = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnAddContent = new System.Windows.Forms.Button();
            this.btnBrowseContent = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtContentFileName = new System.Windows.Forms.TextBox();
            this.cboContentType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSubject = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPresentationId = new System.Windows.Forms.TextBox();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTransactionId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabDecoder = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panelImages = new System.Windows.Forms.Panel();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDecode = new System.Windows.Forms.Button();
            this.btnBrowserMMS = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtMMSFileLocation = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabMain.SuspendLayout();
            this.tabEncoder.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabDecoder.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabEncoder);
            this.tabMain.Controls.Add(this.tabDecoder);
            this.tabMain.Location = new System.Drawing.Point(12, 12);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(570, 425);
            this.tabMain.TabIndex = 0;
            // 
            // tabEncoder
            // 
            this.tabEncoder.Controls.Add(this.btnReset);
            this.tabEncoder.Controls.Add(this.btnGenerate);
            this.tabEncoder.Controls.Add(this.groupBox2);
            this.tabEncoder.Controls.Add(this.groupBox1);
            this.tabEncoder.Location = new System.Drawing.Point(4, 22);
            this.tabEncoder.Name = "tabEncoder";
            this.tabEncoder.Padding = new System.Windows.Forms.Padding(3);
            this.tabEncoder.Size = new System.Drawing.Size(562, 399);
            this.tabEncoder.TabIndex = 0;
            this.tabEncoder.Text = "Encoder";
            this.tabEncoder.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(291, 359);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(131, 34);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(139, 359);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(131, 34);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtContentId);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.lstContent);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnAddContent);
            this.groupBox2.Controls.Add(this.btnBrowseContent);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtContentFileName);
            this.groupBox2.Controls.Add(this.cboContentType);
            this.groupBox2.Location = new System.Drawing.Point(7, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(536, 232);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Content";
            // 
            // txtContentId
            // 
            this.txtContentId.Location = new System.Drawing.Point(111, 46);
            this.txtContentId.Name = "txtContentId";
            this.txtContentId.Size = new System.Drawing.Size(167, 20);
            this.txtContentId.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Content Id";
            // 
            // lstContent
            // 
            this.lstContent.FormattingEnabled = true;
            this.lstContent.Location = new System.Drawing.Point(111, 125);
            this.lstContent.Name = "lstContent";
            this.lstContent.Size = new System.Drawing.Size(398, 95);
            this.lstContent.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 138);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Content";
            // 
            // btnAddContent
            // 
            this.btnAddContent.Location = new System.Drawing.Point(111, 98);
            this.btnAddContent.Name = "btnAddContent";
            this.btnAddContent.Size = new System.Drawing.Size(90, 23);
            this.btnAddContent.TabIndex = 4;
            this.btnAddContent.Text = "Add";
            this.btnAddContent.UseVisualStyleBackColor = true;
            this.btnAddContent.Click += new System.EventHandler(this.btnAddContent_Click);
            // 
            // btnBrowseContent
            // 
            this.btnBrowseContent.Location = new System.Drawing.Point(386, 73);
            this.btnBrowseContent.Name = "btnBrowseContent";
            this.btnBrowseContent.Size = new System.Drawing.Size(29, 19);
            this.btnBrowseContent.TabIndex = 10;
            this.btnBrowseContent.Text = "...";
            this.btnBrowseContent.UseVisualStyleBackColor = true;
            this.btnBrowseContent.Click += new System.EventHandler(this.btnBrowseContent_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "File Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Content Type";
            // 
            // txtContentFileName
            // 
            this.txtContentFileName.Location = new System.Drawing.Point(111, 72);
            this.txtContentFileName.Name = "txtContentFileName";
            this.txtContentFileName.Size = new System.Drawing.Size(269, 20);
            this.txtContentFileName.TabIndex = 7;
            // 
            // cboContentType
            // 
            this.cboContentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboContentType.FormattingEnabled = true;
            this.cboContentType.Location = new System.Drawing.Point(111, 19);
            this.cboContentType.Name = "cboContentType";
            this.cboContentType.Size = new System.Drawing.Size(167, 21);
            this.cboContentType.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblSubject);
            this.groupBox1.Controls.Add(this.txtSubject);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPresentationId);
            this.groupBox1.Controls.Add(this.txtFrom);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTransactionId);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(536, 106);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Content";
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Location = new System.Drawing.Point(281, 76);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(43, 13);
            this.lblSubject.TabIndex = 9;
            this.lblSubject.Text = "Subject";
            // 
            // txtSubject
            // 
            this.txtSubject.Location = new System.Drawing.Point(342, 73);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(153, 20);
            this.txtSubject.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(281, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "To";
            // 
            // txtPresentationId
            // 
            this.txtPresentationId.Location = new System.Drawing.Point(96, 43);
            this.txtPresentationId.Name = "txtPresentationId";
            this.txtPresentationId.Size = new System.Drawing.Size(153, 20);
            this.txtPresentationId.TabIndex = 6;
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(342, 19);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(153, 20);
            this.txtFrom.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "From ";
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(342, 47);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(153, 20);
            this.txtTo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Presentation Id";
            // 
            // txtTransactionId
            // 
            this.txtTransactionId.Location = new System.Drawing.Point(96, 18);
            this.txtTransactionId.Name = "txtTransactionId";
            this.txtTransactionId.Size = new System.Drawing.Size(153, 20);
            this.txtTransactionId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Transaction Id";
            // 
            // tabDecoder
            // 
            this.tabDecoder.Controls.Add(this.groupBox4);
            this.tabDecoder.Controls.Add(this.groupBox3);
            this.tabDecoder.Location = new System.Drawing.Point(4, 22);
            this.tabDecoder.Name = "tabDecoder";
            this.tabDecoder.Padding = new System.Windows.Forms.Padding(3);
            this.tabDecoder.Size = new System.Drawing.Size(562, 399);
            this.tabDecoder.TabIndex = 1;
            this.tabDecoder.Text = "Decoder";
            this.tabDecoder.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panelImages);
            this.groupBox4.Controls.Add(this.txtResults);
            this.groupBox4.Location = new System.Drawing.Point(9, 116);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(547, 277);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            // 
            // panelImages
            // 
            this.panelImages.AutoScroll = true;
            this.panelImages.Location = new System.Drawing.Point(9, 150);
            this.panelImages.Name = "panelImages";
            this.panelImages.Size = new System.Drawing.Size(532, 121);
            this.panelImages.TabIndex = 17;
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(9, 16);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(532, 128);
            this.txtResults.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDecode);
            this.groupBox3.Controls.Add(this.btnBrowserMMS);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtMMSFileLocation);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(550, 101);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MMS File";
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(101, 60);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(97, 35);
            this.btnDecode.TabIndex = 14;
            this.btnDecode.Text = "Decode";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // btnBrowserMMS
            // 
            this.btnBrowserMMS.Location = new System.Drawing.Point(387, 34);
            this.btnBrowserMMS.Name = "btnBrowserMMS";
            this.btnBrowserMMS.Size = new System.Drawing.Size(29, 19);
            this.btnBrowserMMS.TabIndex = 13;
            this.btnBrowserMMS.Text = "...";
            this.btnBrowserMMS.UseVisualStyleBackColor = true;
            this.btnBrowserMMS.Click += new System.EventHandler(this.btnBrowserMMS_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "File Name";
            // 
            // txtMMSFileLocation
            // 
            this.txtMMSFileLocation.Location = new System.Drawing.Point(101, 34);
            this.txtMMSFileLocation.Name = "txtMMSFileLocation";
            this.txtMMSFileLocation.Size = new System.Drawing.Size(269, 20);
            this.txtMMSFileLocation.TabIndex = 11;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 462);
            this.Controls.Add(this.tabMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessagingToolkit MMS Demo";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabMain.ResumeLayout(false);
            this.tabEncoder.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabDecoder.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabEncoder;
        private System.Windows.Forms.TabPage tabDecoder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTransactionId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPresentationId;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtContentFileName;
        private System.Windows.Forms.ComboBox cboContentType;
        private System.Windows.Forms.Button btnBrowseContent;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lstContent;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAddContent;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.Button btnBrowserMMS;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtMMSFileLocation;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtContentId;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panelImages;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

