namespace MessagingToolkit.Barcode.Service.Demo
{
    partial class ScanService
    {

        private System.IO.FileSystemWatcher fileWatcher;

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
            this.fileWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).BeginInit();
            // 
            // fileWatcher
            // 
            this.fileWatcher.EnableRaisingEvents = true;
            this.fileWatcher.IncludeSubdirectories = true;
            this.fileWatcher.NotifyFilter = ((System.IO.NotifyFilters)((((((((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName) 
            | System.IO.NotifyFilters.Attributes) 
            | System.IO.NotifyFilters.Size) 
            | System.IO.NotifyFilters.LastWrite) 
            | System.IO.NotifyFilters.LastAccess) 
            | System.IO.NotifyFilters.CreationTime) 
            | System.IO.NotifyFilters.Security)));
            this.fileWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileWatcher_Changed);
            this.fileWatcher.Created += new System.IO.FileSystemEventHandler(this.fileWatcher_Created);
            this.fileWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileWatcher_Deleted);
            this.fileWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileWatcher_Renamed);
            // 
            // ScanService
            // 
            this.ServiceName = "ScanService";
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).EndInit();

        }

        #endregion

      
    }
}
