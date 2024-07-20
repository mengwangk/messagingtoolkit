using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.IO;
using System.Drawing;

namespace MessagingToolkit.Barcode.Service.Demo
{
    public partial class ScanService : ServiceBase
    {
        public ScanService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            fileWatcher.Path = ConfigurationManager.AppSettings["WatchPath"];
        }

        protected override void OnStop()
        {
        }

        private void fileWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {

        }

        /// <summary>
        /// Event occurs when the contents of a File or Directory is changed
        /// </summary>
        private void fileWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            //code here for newly changed file or directory
        }

        /// <summary>
        /// Event occurs when the a File or Directory is created
        /// </summary>
        private void fileWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            string fullPath = e.FullPath;
            string fileNameNoExt = Path.GetFileNameWithoutExtension(fullPath);
            string dir = Path.GetDirectoryName(fullPath);
            string ext = Path.GetExtension(fullPath);

            if (ext.Equals(".txt")) return;

            // Create a file to write to. 
            using (StreamWriter sw = File.CreateText(Path.Combine(dir, fileNameNoExt) + ".txt"))
            {
                try
                {
                    //code here for newly created file or directory
                    BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
                    Bitmap bitmap = new Bitmap(fullPath);

                    Dictionary<DecodeOptions, object> decodingOptions = new Dictionary<DecodeOptions, object>();
                    List<BarcodeFormat> possibleFormats = new List<BarcodeFormat>(10);

                    possibleFormats.Add(BarcodeFormat.DataMatrix);
                    possibleFormats.Add(BarcodeFormat.QRCode);
                    possibleFormats.Add(BarcodeFormat.PDF417);
                    possibleFormats.Add(BarcodeFormat.Aztec);
                    possibleFormats.Add(BarcodeFormat.UPCE);
                    possibleFormats.Add(BarcodeFormat.UPCA);
                    possibleFormats.Add(BarcodeFormat.Code128);
                    possibleFormats.Add(BarcodeFormat.Code39);
                    possibleFormats.Add(BarcodeFormat.ITF14);
                    possibleFormats.Add(BarcodeFormat.EAN8);
                    possibleFormats.Add(BarcodeFormat.EAN13);
                    possibleFormats.Add(BarcodeFormat.RSS14);
                    possibleFormats.Add(BarcodeFormat.RSSExpanded);
                    possibleFormats.Add(BarcodeFormat.Codabar);
                    possibleFormats.Add(BarcodeFormat.MaxiCode);
                    possibleFormats.Add(BarcodeFormat.MSIMod10);

                    decodingOptions.Add(DecodeOptions.TryHarder, true);
                    decodingOptions.Add(DecodeOptions.PossibleFormats, possibleFormats);
                    Result decodedResult = barcodeDecoder.Decode(bitmap, decodingOptions);
                    if (decodedResult != null)
                    {
                        sw.WriteLine(decodedResult.Text);
                    }
                }
                catch (Exception ex)
                {
                    sw.WriteLine(ex.Message);
                    sw.WriteLine(ex.StackTrace);
                    sw.WriteLine(ex.ToString());
                }
            }	          
        }


        /// <summary>
        /// Event occurs when the a File or Directory is renamed
        /// </summary>
        private void fileWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            //code here for newly renamed file or directory
        }

        /// <summary>
        /// Starts the service. Use for debugging purpose
        /// </summary>
        public void StartService()
        {
            OnStart(null);
        }
    }
}
