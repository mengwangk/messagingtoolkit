namespace MessagingToolkit.Diagnostics
{
    partial class Step1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Step1));
            this.label1 = new System.Windows.Forms.Label();
            this.linkGuide = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Description
            // 
            this.Description.Size = new System.Drawing.Size(484, 48);
            this.Description.Text = "Connection Setup Verification";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 156);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // linkGuide
            // 
            this.linkGuide.AutoSize = true;
            this.linkGuide.Location = new System.Drawing.Point(9, 240);
            this.linkGuide.Name = "linkGuide";
            this.linkGuide.Size = new System.Drawing.Size(262, 13);
            this.linkGuide.TabIndex = 2;
            this.linkGuide.TabStop = true;
            this.linkGuide.Text = "Use the .NET SMS Library to Retrieve Phone Settings";
            this.linkGuide.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGuide_LinkClicked);
            // 
            // Step1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkGuide);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "Step1";
            this.NextStep = "Step2";
            this.Size = new System.Drawing.Size(500, 399);
            this.StepDescription = "Connection Setup Verification";
            this.Controls.SetChildIndex(this.linkGuide, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.Description, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkGuide;
    }
}
