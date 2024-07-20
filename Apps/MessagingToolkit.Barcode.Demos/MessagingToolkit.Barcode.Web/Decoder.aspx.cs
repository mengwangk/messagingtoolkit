using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.QRCode.Decoder;

namespace MessagingToolkit.Barcode.Web
{
    public partial class Decoder : System.Web.UI.Page
    {
        private BarcodeDecoder barcodeDecoder = new BarcodeDecoder();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtDecodedResult.Text = string.Empty;
        }

        protected void btnDecode_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUploadImage.PostedFile != null)
                {
                    string virtualTempFileName = "~/" + GenerateRandomFileName(Server.MapPath("~/"));
                    string physicalTempFileName = Server.MapPath(virtualTempFileName);
                    fileUploadImage.PostedFile.SaveAs(physicalTempFileName);

                    // Perform decoding
                    System.Drawing.Bitmap image = new System.Drawing.Bitmap(physicalTempFileName);

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

                    decodingOptions.Add(DecodeOptions.TryHarder, true);
                    decodingOptions.Add(DecodeOptions.PossibleFormats, possibleFormats);
                    Result decodedResult = barcodeDecoder.Decode(image, decodingOptions);
                    if (decodedResult != null)
                    {
                        txtDecodedResult.Text = decodedResult.Text;
                        txtBarcodeType.Text = decodedResult.BarcodeFormat.ToString();
                    }
                    txtDecodedResult.Visible = true;
                    lblMsg.Visible = false;
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Nothing to Decode. Please specify a valid file";
                }
               
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                txtDecodedResult.Text = string.Empty;
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
            } while (File.Exists(folderPath + name));
            return name;
        }

    }
}