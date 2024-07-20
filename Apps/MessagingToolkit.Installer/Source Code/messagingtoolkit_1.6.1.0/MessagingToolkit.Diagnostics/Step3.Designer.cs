namespace MessagingToolkit.Diagnostics
{
    partial class Step3
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
            this.btnDiagnose = new System.Windows.Forms.Button();
            this.txtDiagnoseResults = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Description
            // 
            this.Description.Size = new System.Drawing.Size(467, 37);
            this.Description.Text = "Verify Gateway Capabilities";
            // 
            // btnDiagnose
            // 
            this.btnDiagnose.Location = new System.Drawing.Point(114, 95);
            this.btnDiagnose.Name = "btnDiagnose";
            this.btnDiagnose.Size = new System.Drawing.Size(128, 23);
            this.btnDiagnose.TabIndex = 1;
            this.btnDiagnose.Text = "Start Diagnosing";
            this.btnDiagnose.UseVisualStyleBackColor = true;
            this.btnDiagnose.Click += new System.EventHandler(this.btnDiagnose_Click);
            // 
            // txtDiagnoseResults
            // 
            this.txtDiagnoseResults.Location = new System.Drawing.Point(12, 143);
            this.txtDiagnoseResults.Multiline = true;
            this.txtDiagnoseResults.Name = "txtDiagnoseResults";
            this.txtDiagnoseResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDiagnoseResults.Size = new System.Drawing.Size(301, 241);
            this.txtDiagnoseResults.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "Press the button if you want to start diagnosing. \r\nYou can skip this step.";
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Location = new System.Drawing.Point(42, 101);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkVerbose.TabIndex = 4;
            this.chkVerbose.Text = "Verbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // Step3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkVerbose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDiagnoseResults);
            this.Controls.Add(this.btnDiagnose);
            this.IsFinished = true;
            this.Name = "Step3";
            this.NextStep = "FINISHED";
            this.PreviousStep = "Step2";
            this.Size = new System.Drawing.Size(483, 410);
            this.StepDescription = "Verify Gateway Capabilities";
            this.Controls.SetChildIndex(this.btnDiagnose, 0);
            this.Controls.SetChildIndex(this.txtDiagnoseResults, 0);
            this.Controls.SetChildIndex(this.Description, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.chkVerbose, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDiagnose;
        private System.Windows.Forms.TextBox txtDiagnoseResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkVerbose;
    }
}
