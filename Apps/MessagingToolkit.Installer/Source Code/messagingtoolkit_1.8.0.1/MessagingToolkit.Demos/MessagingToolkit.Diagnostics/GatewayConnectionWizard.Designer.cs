namespace MessagingToolkit.Diagnostics
{
    partial class GatewayConnectionWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GatewayConnectionWizard));
            this.SuspendLayout();
            // 
            // wizardTop
            // 
            this.wizardTop.Size = new System.Drawing.Size(496, 64);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(43, 8);
            // 
            // back
            // 
            this.back.Location = new System.Drawing.Point(302, 8);
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(396, 8);
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // panelStep
            // 
            this.panelStep.BackColor = System.Drawing.SystemColors.Control;
            this.panelStep.Location = new System.Drawing.Point(0, 66);
            this.panelStep.Size = new System.Drawing.Size(496, 316);
            // 
            // GatewayConnectionWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 422);
            this.FirstStepName = "Step1";
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GatewayConnectionWizard";
            this.SideBarImage = null;
            this.SideBarLogo = null;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gateway Connection Wizard";
            this.LoadSteps += new System.EventHandler(this.GatewayConnectionWizard_LoadSteps);
            this.ResumeLayout(false);

        }

        #endregion
    }
}