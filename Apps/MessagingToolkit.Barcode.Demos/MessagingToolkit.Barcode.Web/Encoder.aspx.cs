using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.QRCode.Decoder;

namespace MessagingToolkit.Barcode.Web
{
    public partial class Encoder : System.Web.UI.Page
    {
        private static Dictionary<string, BarcodeFormat> BarcodeFormats =
                    new Dictionary<string, BarcodeFormat>
                       {
                           {"QR Code", BarcodeFormat.QRCode},
                           {"Data Matrix", BarcodeFormat.DataMatrix},
                           {"PDF417", BarcodeFormat.PDF417},
                           {"Aztec", BarcodeFormat.Aztec},
                           {"Bookland/ISBN", BarcodeFormat.Bookland},
                           {"Codabar", BarcodeFormat.Codabar},
                           {"Code 11", BarcodeFormat.Code11},
                           {"Code 128", BarcodeFormat.Code128},
                           {"Code 128-A", BarcodeFormat.Code128A},
                           {"Code 128-B", BarcodeFormat.Code128B},
                           {"Code 128-C", BarcodeFormat.Code128C},
                           {"Code 39", BarcodeFormat.Code39},
                           {"Code 39 Extended", BarcodeFormat.Code39Extended},
                           {"Code 93", BarcodeFormat.Code93},
                           {"EAN-13", BarcodeFormat.EAN13},
                           {"EAN-8", BarcodeFormat.EAN8},
                           {"FIM", BarcodeFormat.FIM},
                           {"Interleaved 2 of 5", BarcodeFormat.Interleaved2of5},
                           {"ITF-14", BarcodeFormat.ITF14},
                           {"LOGMARS", BarcodeFormat.LOGMARS},
                           {"MSI 2 Mod 10", BarcodeFormat.MSI2Mod10},
                           {"MSI Mod 10", BarcodeFormat.MSIMod10},
                           {"MSI Mod 11", BarcodeFormat.MSIMod11},
                           {"MSI Mod 11 Mod 10", BarcodeFormat.MSIMod11Mod10},
                           {"PostNet", BarcodeFormat.PostNet},
                           {"Plessey", BarcodeFormat.ModifiedPlessey},
                           {"Standard 2 of 5", BarcodeFormat.Standard2of5},
                           {"Telepen", BarcodeFormat.Telepen},
                           {"UPC 2 Digit Ext.", BarcodeFormat.UPCSupplemental2Digit},
                           {"UPC 5 Digit Ext.", BarcodeFormat.UPCSupplemental5Digit},
                           {"UPC-A", BarcodeFormat.UPCA},
                           {"UPC-E", BarcodeFormat.UPCE}
                       };


       
        private BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnEncode_Click(object sender, EventArgs e)
        {
            
            try
            {
                string data = txtBarcodeData.Text;
                if (string.IsNullOrEmpty(data))
                {
                    picEncodedBarcode.Visible = false;
                    lblMsg.Visible = true;
                    lblMsg.Text = "Please specify barcode content";
                    return;
                }

                // Get barcode format
                BarcodeFormat fmt = BarcodeFormat.QRCode;
                if (BarcodeFormats.ContainsKey(cboBarcodeType.Text))
                    fmt = BarcodeFormats[cboBarcodeType.Text];
                
                string tempFileName = string.Empty;
                System.Drawing.Image image = barcodeEncoder.Encode(fmt, data);
                tempFileName = "~/" + GenerateRandomFileName(Server.MapPath("~/"));
                barcodeEncoder.SaveImage(Server.MapPath(tempFileName), SaveOptions.Png);
                picEncodedBarcode.ImageUrl = tempFileName;
                picEncodedBarcode.Visible = true;
                lblMsg.Visible = false;
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                picEncodedBarcode.Visible = false;
                lblMsg.Text = ex.Message;
            }
        }

        private string GenerateRandomFileName(string folderPath)
        {
            string chars = "2346789ABCDEFGHJKLMNPQRTUVWXYZabcdefghjkmnpqrtuvwxyz";
            Random rnd = new Random();
            string name;
            do
            {
                name = string.Empty;
                while (name.Length < 5)
                {
                    name += chars.Substring(rnd.Next(chars.Length), 1);
                }
                name += ".png";
            } while (File.Exists(folderPath + name));
            return name;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtBarcodeData.Text = "Demo for barcode library";
            cboBarcodeType.SelectedIndex = 0;
        }
    }
}