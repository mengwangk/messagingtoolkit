namespace MessagingToolkit.BulkGateway
{
    partial class frmCodeViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCodeViewer));
            this.txtCode = new System.Windows.Forms.TextBox();
            this.btnCloseCodeViewer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(12, 3);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(643, 511);
            this.txtCode.TabIndex = 0;
            // 
            // btnCloseCodeViewer
            // 
            this.btnCloseCodeViewer.Location = new System.Drawing.Point(235, 532);
            this.btnCloseCodeViewer.Name = "btnCloseCodeViewer";
            this.btnCloseCodeViewer.Size = new System.Drawing.Size(143, 33);
            this.btnCloseCodeViewer.TabIndex = 1;
            this.btnCloseCodeViewer.Text = "Close";
            this.btnCloseCodeViewer.UseVisualStyleBackColor = true;
            this.btnCloseCodeViewer.Click += new System.EventHandler(this.btnCloseCodeViewer_Click);
            // 
            // frmCodeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 586);
            this.Controls.Add(this.btnCloseCodeViewer);
            this.Controls.Add(this.txtCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCodeViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Code Viewer";
            this.Load += new System.EventHandler(this.frmCodeViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnCloseCodeViewer;
    }
}