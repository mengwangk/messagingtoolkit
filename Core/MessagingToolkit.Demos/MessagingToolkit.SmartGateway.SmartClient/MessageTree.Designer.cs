namespace MessagingToolkit.SmartGateway.SmartClient
{
    partial class MessageTree
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Inbox");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Waiting to resend");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Outbox", new System.Windows.Forms.TreeNode[] {
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Sent items");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Could not be sent");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Deleted items", new System.Windows.Forms.TreeNode[] {
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Events");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Message Folders", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode3,
            treeNode4,
            treeNode6,
            treeNode7});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageTree));
            this.treeMessageFolder = new System.Windows.Forms.TreeView();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeMessageFolder
            // 
            this.treeMessageFolder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeMessageFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMessageFolder.ImageIndex = 0;
            this.treeMessageFolder.ImageList = this.ImageList;
            this.treeMessageFolder.Location = new System.Drawing.Point(0, 0);
            this.treeMessageFolder.Name = "treeMessageFolder";
            treeNode1.Name = "nodeInbox";
            treeNode1.Text = "Inbox";
            treeNode2.Name = "nodeResend";
            treeNode2.Text = "Waiting to resend";
            treeNode3.Name = "nodeOutbox";
            treeNode3.Text = "Outbox";
            treeNode4.Name = "nodeSentItems";
            treeNode4.Text = "Sent items";
            treeNode5.Name = "nodeCouldNotSend";
            treeNode5.Text = "Could not be sent";
            treeNode6.Name = "nodeDeletedItems";
            treeNode6.Text = "Deleted items";
            treeNode7.Name = "nodeEvents";
            treeNode7.Text = "Events";
            treeNode8.Name = "nodeMessageFolders";
            treeNode8.Text = "Message Folders";
            this.treeMessageFolder.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode8});
            this.treeMessageFolder.SelectedImageIndex = 0;
            this.treeMessageFolder.ShowRootLines = false;
            this.treeMessageFolder.Size = new System.Drawing.Size(227, 234);
            this.treeMessageFolder.TabIndex = 0;
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "Drafts.bmp");
            this.ImageList.Images.SetKeyName(1, "Outbox.bmp");
            this.ImageList.Images.SetKeyName(2, "Recycle.bmp");
            this.ImageList.Images.SetKeyName(3, "Send.bmp");
            // 
            // MessageTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeMessageFolder);
            this.Name = "MessageTree";
            this.Size = new System.Drawing.Size(227, 234);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeMessageFolder;
        private System.Windows.Forms.ImageList ImageList;
    }
}
