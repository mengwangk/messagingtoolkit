using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.IO;

using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Pdf417.Encoder;
using MessagingToolkit.Barcode.QRCode;

namespace MessagingToolkit.Barcode.Demo
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class frmMain : Form
    {
        private Bitmap bitmap = null;
        private BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
        private BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

        private static Dictionary<string, string> CharacterSets =
                         new Dictionary<string, string>
                        {
                             { "Default ISO-8859-1", "ISO-8859-1"},
                             { "UTF-8", "UTF-8"},
                             { "SHIFT-JIS", "SHIFT-JIS"},
                             { "CP437", "CP437"},
                             { "ISO-8859-2", "ISO-8859-2"},
                             { "ISO-8859-3", "ISO-8859-3"},
                             { "ISO-8859-4", "ISO-8859-4"},
                             { "ISO-8859-5", "ISO-8859-5"},
                             { "ISO-8859-6", "ISO-8859-6"},
                             { "ISO-8859-7", "ISO-8859-7"},
                             { "ISO-8859-8", "ISO-8859-8"},
                             { "ISO-8859-9", "ISO-8859-9"},
                             { "ISO-8859-10", "ISO-8859-10"},
                             { "ISO-8859-11", "ISO-8859-11"},
                             { "ISO-8859-13", "ISO-8859-13"},
                             { "ISO-8859-14", "ISO-8859-14"},
                             { "ISO-8859-15", "ISO-8859-15"},
                             { "ISO-8859-16", "ISO-8859-16"},
                        };

        private static Dictionary<string, ErrorCorrectionLevel> ErrorCorrectionLevels =
                        new Dictionary<string, ErrorCorrectionLevel>
                        {
                             { "Low", ErrorCorrectionLevel.L},
                             { "Medium Low", ErrorCorrectionLevel.M},
                             { "Medium High", ErrorCorrectionLevel.Q},
                             { "High", ErrorCorrectionLevel.H},
                        };

        private static Dictionary<string, BarcodeFormat> OneDBarcodeFormats =
                       new Dictionary<string, BarcodeFormat>
                       {
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
                           {"Standard 2 of 5", BarcodeFormat.Standard2of5},
                           {"Telepen", BarcodeFormat.Telepen},
                           {"UPC 2 Digit Ext.", BarcodeFormat.UPCSupplemental2Digit},
                           {"UPC 5 Digit Ext.", BarcodeFormat.UPCSupplemental5Digit},
                           {"UPC-A", BarcodeFormat.UPCA},
                           {"UPC-E", BarcodeFormat.UPCE}
                       };

        private static Dictionary<string, AlignmentPositions> OneDBarcodeAlignmentPositions =
                   new Dictionary<string, AlignmentPositions>
                       {
                           {"Center", AlignmentPositions.Center},
                           {"Left", AlignmentPositions.Left},
                           {"Right", AlignmentPositions.Right},
                       };


        private static Dictionary<string, LabelPositions> OneDBarcodeLabelPositions =
                 new Dictionary<string, LabelPositions>
                       {
                           {"BottomCenter", LabelPositions.BottomCenter},
                           {"BottomLeft", LabelPositions.BottomLeft},
                           {"BottomRight", LabelPositions.BottomRight},
                           {"TopCenter", LabelPositions.TopCenter},
                           {"TopLeft", LabelPositions.TopLeft},
                           {"TopRight", LabelPositions.TopRight},
                       };

        private static Dictionary<string, Func<string>> QRCodeContentTypes =
                new Dictionary<string, Func<string>>
                       {
                           {"Calendar Event", ShowCalendar},
                           {"Contact Information", ShowContactInfo},
                           {"Email Address", ShowEmailAddress},
                           {"Geo Location", ShowGeoLocation},
                           {"Phone Number", ShowPhoneNumber},
                           {"SMS", ShowSMS},
                           {"URL", ShowURL},
                           {"Wifi Network", ShowWifiNetwork},
                       };

        private static Dictionary<string, Compaction> Pdf417Compactions =
               new Dictionary<string, Compaction>
                       {
                           {"Auto", Compaction.Auto},
                           {"Byte", Compaction.Byte},
                           {"Numeric", Compaction.Numeric},
                           {"Text", Compaction.Text},
                          
                       };
        /// <summary>
        /// Initializes a new instance of the <see cref="frmMain"/> class.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            InitializeApp();
        }

        private void InitializeApp()
        {
            // Populate the character set
            cboQRCodeCharacterSet.Items.AddRange(CharacterSets.Keys.ToArray());
            cboQRCodeCharacterSet.SelectedIndex = 0;

            // Populate error correction levels
            cboQRCodeErrorCorrectionLevel.Items.AddRange(ErrorCorrectionLevels.Keys.ToArray());
            cboQRCodeErrorCorrectionLevel.SelectedIndex = 0;

            cboOneDBarcodeFormat.Items.AddRange(OneDBarcodeFormats.Keys.ToArray());
            cboOneDBarcodeFormat.SelectedIndex = 0;

            cboOneDBarcodeAlignment.Items.AddRange(OneDBarcodeAlignmentPositions.Keys.ToArray());
            cboOneDBarcodeAlignment.SelectedIndex = 0;

            cboOneDBarcodeLabelLocation.Items.AddRange(OneDBarcodeLabelPositions.Keys.ToArray());
            cboOneDBarcodeLabelLocation.SelectedIndex = 0;

            cboQRCodeContentType.Items.AddRange(QRCodeContentTypes.Keys.ToArray());

            // Populate the character set
            cboQRCodeMiscCharacterSet.Items.AddRange(CharacterSets.Keys.ToArray());
            cboQRCodeMiscCharacterSet.SelectedIndex = 0;

            // Populate error correction levels
            cboQRCodeMiscErrorCorrectionLevel.Items.AddRange(ErrorCorrectionLevels.Keys.ToArray());
            cboQRCodeMiscErrorCorrectionLevel.SelectedIndex = 0;

            cboPdf417Compaction.Items.AddRange(Pdf417Compactions.Keys.ToArray());
            cboPdf417Compaction.SelectedIndex = 0;

        }

        private void btnEncodeQRCode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQRCodeData.Text))
            {
                try
                {
                    barcodeEncoder.CharacterSet = CharacterSets[cboQRCodeCharacterSet.Text];
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboQRCodeErrorCorrectionLevel.Text];
                    barcodeEncoder.ForeColor = btnQRCodeForeColor.BackColor;
                    barcodeEncoder.BackColor = btnQRCodeBackColor.BackColor;
                    barcodeEncoder.Margin = (int)numQRCodeQuietZone.Value;
                    barcodeEncoder.Width = (int)numQRCodeWidth.Value;
                    barcodeEncoder.Height = (int)numQRCodeHeight.Value;

                    Dictionary<EncodeOptions, object> encodingOptions = new Dictionary<EncodeOptions, object>(1);
                    if (!string.IsNullOrEmpty(txtQRCodeLogoFileName.Text) && File.Exists(txtQRCodeLogoFileName.Text))
                    {
                        Image logo = Image.FromFile(@txtQRCodeLogoFileName.Text);
                        encodingOptions.Add(EncodeOptions.QRCodeLogo, logo);
                    }

                    // If there is no encoding options, use
                    //picEncodedQRCode.Image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtQRCodeData.Text);
                    picEncodedQRCode.Image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtQRCodeData.Text, encodingOptions);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please specify the QR Code content", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSaveQRCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSaveQRCode_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)picEncodedQRCode.Image;
            SaveImage(bm);
        }


        private void btnQRCodeForeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnQRCodeForeColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnQRCodeForeColor.BackColor = colorDialog1.Color;
            }
        }



        private void btnQRCodeBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnQRCodeBackColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnQRCodeBackColor.BackColor = colorDialog1.Color;
            }
        }





        private void btnEncodeDataMatrix_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDataMatrixData.Text))
            {
                try
                {
                    barcodeEncoder.ForeColor = btnDataMatrixForeColor.BackColor;
                    barcodeEncoder.BackColor = btnDataMatrixBackColor.BackColor;
                    barcodeEncoder.ModuleSize = (int)numDataMatrixModuleSize.Value;
                    barcodeEncoder.MarginSize = (int)numDataMatrixMarginSize.Value;
                    picEncodedDataMatrix.Image = barcodeEncoder.Encode(BarcodeFormat.DataMatrix, txtDataMatrixData.Text);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please specify the data Matrix content", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSaveDataMatrix_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)picEncodedDataMatrix.Image;
            SaveImage(bm);

        }


        private void SaveImage(Bitmap bm)
        {
            try
            {
                if (bm != null)
                {
                    SaveFileDialog sdlg = new SaveFileDialog();
                    sdlg.Filter = "Png files (*.png)|*.png|All files (*.*)|*.*";
                    if (sdlg.ShowDialog() == DialogResult.OK)
                    {
                        // You can save to any formats you want, as long as it is supported by .NET
                        bm.Save(sdlg.FileName, ImageFormat.Png);
                        MessageBox.Show("Image is saved successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDataMatrixForeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnDataMatrixForeColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnDataMatrixForeColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnDataMatrixBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnDataMatrixBackColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnDataMatrixBackColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnPdf417Encode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPdf417Data.Text))
            {
                try
                {
                    barcodeEncoder.ForeColor = btnPdf417ForeColor.BackColor;
                    barcodeEncoder.BackColor = btnPdf417BackColor.BackColor;
                    barcodeEncoder.Width = (int)numPdf417Width.Value;
                    barcodeEncoder.Height = (int)numPdf417Height.Value;
                    barcodeEncoder.Pdf417Compaction = Pdf417Compactions[cboPdf417Compaction.Text];

                    // You can also set the Compact mode - true or false
                    //barcodeEncoder.AddOption(EncodeOptions.Pdf417Compact, true);

                    // You can set the dimension
                    //barcodeEncoder.AddOption(EncodeOptions.Pdf417Dimensions, new Dimensions(0,10,0,20);

                    picEncodedPdf417.Image = barcodeEncoder.Encode(BarcodeFormat.PDF417, txtPdf417Data.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please specify the Pdf417 content", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPdf417Save_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)picEncodedPdf417.Image;
            SaveImage(bm);
        }

        private void btnEncodeOneDBarcode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOneDBarcodeData.Text))
            {
                try
                {
                    BarcodeFormat fmt = OneDBarcodeFormats[cboOneDBarcodeFormat.Text];
                    barcodeEncoder.ForeColor = btnOneDBarcodeForeColor.BackColor;
                    barcodeEncoder.BackColor = btnOneDBarcodeBackColor.BackColor;
                    barcodeEncoder.Width = (int)numOneDBarcodeWidth.Value;
                    barcodeEncoder.Height = (int)numOneDBarcodeHeight.Value;
                    barcodeEncoder.IncludeLabel = chkOneDBarcodeGeneralLabel.Checked;
                    barcodeEncoder.Alignment = OneDBarcodeAlignmentPositions[cboOneDBarcodeAlignment.Text];
                    barcodeEncoder.LabelPosition = OneDBarcodeLabelPositions[cboOneDBarcodeLabelLocation.Text];
                    barcodeEncoder.CustomLabel = txtOneDBarcodeCustomText.Text;

                    if (!string.IsNullOrEmpty(txtOneDBarcodeQuietZone.Text))
                    {
                        try
                        {
                            barcodeEncoder.Margin = Convert.ToInt32(txtOneDBarcodeQuietZone.Text);
                        }
                        catch
                        {
                        }
                    }
                    picEncodedOneDBarcode.Image = barcodeEncoder.Encode(fmt, txtOneDBarcodeData.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please specify the barcode content", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveOneDBarcode_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)picEncodedOneDBarcode.Image;
            SaveImage(bm);
        }

        private void btnOneDBarcodeForeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnOneDBarcodeForeColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnOneDBarcodeForeColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnOneDBarcodeBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnOneDBarcodeBackColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnOneDBarcodeBackColor.BackColor = colorDialog1.Color;
            }
        }


        private void btnQRCodeMiscEncode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQRCodeMiscData.Text))
            {
                try
                {
                    barcodeEncoder.CharacterSet = CharacterSets[cboQRCodeMiscCharacterSet.Text];
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboQRCodeMiscErrorCorrectionLevel.Text];
                    barcodeEncoder.ForeColor = btnQRCodeMiscForeColor.BackColor;
                    barcodeEncoder.BackColor = btnQRCodeMiscBackColor.BackColor;
                    barcodeEncoder.Margin = (int)numQRCodeMiscQuietZone.Value;
                    barcodeEncoder.Width = (int)numQRCodeMiscWidth.Value;
                    barcodeEncoder.Height = (int)numQRCodeMiscHeight.Value;
                    picEncodedQRCodeMisc.Image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtQRCodeMiscData.Text);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please specify the QR Code content", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboQRCodeContentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Func<string> func = QRCodeContentTypes[cboQRCodeContentType.Text];
            txtQRCodeMiscData.Text = func();

        }

        private static string ShowCalendar()
        {
            return "BEGIN:VEVENT" + "\n" +
                    "SUMMARY:<summary>" + "\n" +
                    "DTSTART:20110915T000000Z" + "\n" +
                    "DTEND:20110916T000000Z" + "\n" +
                    "END:VEVENT";
        }

        private static string ShowContactInfo()
        {

            return "MECARD:\n" +
                    "N:<my name>;\n" +
                    "ORG:<my company>;\n" +
                    "TEL:<my telephone no>;\n" +
                    "URL:<my url>;\n" +
                    "EMAIL:<my email>;\n" +
                    "ADR:<my address>;\n" +
                    "NOTE:<my note>;;\n";
        }

        private static string ShowEmailAddress()
        {

            return "MAILTO:<my email>";
        }

        private static string ShowGeoLocation()
        {

            return "geo:40.71872,-73.98905,100";
        }

        private static string ShowPhoneNumber()
        {

            return "TEL:<phone number>";
        }

        private static string ShowSMS()
        {
            return "SMSTO:<phone number>";
        }

        private static string ShowURL()
        {
            return "HTTP://";
        }

        private static string ShowWifiNetwork()
        {
            return "WIFI:T:WPA;S:<my network>;P:<my password>;;";
        }

        private void btnQRCodeMiscForeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnQRCodeMiscForeColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnQRCodeMiscForeColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnQRCodeMiscBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnQRCodeMiscBackColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnQRCodeMiscBackColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnPdf417ForeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnPdf417ForeColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnPdf417ForeColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnPdf417BackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnPdf417BackColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnPdf417BackColor.BackColor = colorDialog1.Color;
            }
        }



        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                bitmap = new Bitmap(fd.FileName);
                picBoxDecode.Image = (Image)bitmap;
                txtFile.Text = fd.FileName;
                txtBarcodeContent.Text = string.Empty;
                txtBarcodeType.Text = string.Empty;
            }
        }

        private void DecodeBarcode(Bitmap image)
        {
            Dictionary<DecodeOptions, object> decodingOptions = new Dictionary<DecodeOptions, object>();
            List<BarcodeFormat> possibleFormats = new List<BarcodeFormat>(10);

            if (chkDataMatrix.Checked)
            {
                possibleFormats.Add(BarcodeFormat.DataMatrix);
            }
            if (chkQRCode.Checked)
            {
                possibleFormats.Add(BarcodeFormat.QRCode);
            }
            if (chkPdf417.Checked)
            {
                possibleFormats.Add(BarcodeFormat.PDF417);
            }

            if (chkAztec.Checked)
            {
                possibleFormats.Add(BarcodeFormat.Aztec);
            }

            if (chkMaxiCode.Checked)
            {
                possibleFormats.Add(BarcodeFormat.MaxiCode);
            }

            if (chkUpcE.Checked)
            {
                possibleFormats.Add(BarcodeFormat.UPCE);
            }
            if (chkUpcA.Checked)
            {
                possibleFormats.Add(BarcodeFormat.UPCA);
            }
            if (chkCode128.Checked)
            {
                possibleFormats.Add(BarcodeFormat.Code128);
            }
            if (chkCode39.Checked)
            {
                possibleFormats.Add(BarcodeFormat.Code39);
            }
            if (chkItf.Checked)
            {
                possibleFormats.Add(BarcodeFormat.ITF14);
            }
            if (chkEan8.Checked)
            {
                possibleFormats.Add(BarcodeFormat.EAN8);
            }
            if (chkEan13.Checked)
            {
                possibleFormats.Add(BarcodeFormat.EAN13);
            }

            if (chkRss14.Checked)
            {
                possibleFormats.Add(BarcodeFormat.RSS14);
            }

            if (chkRssExpanded.Checked)
            {
                possibleFormats.Add(BarcodeFormat.RSSExpanded);
            }

            if (chkCodaBar.Checked)
            {
                possibleFormats.Add(BarcodeFormat.Codabar);
            }

            if (chkTryHarder.Checked)
            {
                decodingOptions.Add(DecodeOptions.TryHarder, true);
            }
            if (chkTryAllFormats.Checked)
            {
                decodingOptions.Add(DecodeOptions.PossibleFormats, possibleFormats);
            }


            if (chkPureBarcode.Checked)
            {
                decodingOptions.Add(DecodeOptions.PureBarcode, "");
            }


            try
            {
                if (!chkMultipleBarcodes.Checked)
                {
                    Result decodedResult = barcodeDecoder.Decode(bitmap, decodingOptions);
                    if (decodedResult != null)
                    {
                        txtBarcodeContent.Text = decodedResult.Text;
                        txtBarcodeType.Text = decodedResult.BarcodeFormat.ToString();
                    }
                    else
                    {
                        txtBarcodeContent.Text = "No barcode found";
                    }
                }
                else
                {
                    Result[] results = barcodeDecoder.DecodeMultiple(bitmap, decodingOptions);

                    txtBarcodeType.Text = string.Empty;
                    string content = string.Empty;
                    foreach (Result result in results)
                    {
                        content += "Barcode Type: " + result.BarcodeFormat + Environment.NewLine;
                        content += "Content: " + result.Text + Environment.NewLine + Environment.NewLine;
                    }
                    txtBarcodeContent.Text = content;
                }
            }
            catch (BarcodeDecoderException e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            if (bitmap == null) return;
            DecodeBarcode(bitmap);
            TimeSpan ts = DateTime.Now.Subtract(dt);
            lblTime.Text = "Time taken: " + ts.Milliseconds.ToString() + "ms";
        }

        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabAbout)
            {
                Assembly assembly = Assembly.GetAssembly(this.GetType());
                string name = assembly.GetName().Name;
                string version = assembly.GetName().Version.ToString();
                string title = string.Empty;
                string description = string.Empty;
                object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length == 1)
                {
                    title = ((AssemblyTitleAttribute)attributes[0]).Title;
                }
                attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 1)
                {
                    description = ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
                lblAbout.Text = title + "\n" + version;
            }
        }

        private void btnQRCodeMiscSave_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)picEncodedQRCodeMisc.Image;
            SaveImage(bm);
        }

        private void chkMaxiCode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMaxiCode.Checked)
            {
                MessageBox.Show("Please make sure PureBarcode option is checked", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static byte[] ImageToByteArray(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        private void btnBrowseQRCodeLogo_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Png files (*.png)|*.png|All files (*.*)|*.*";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtQRCodeLogoFileName.Text = fd.FileName;
            }
        }

    }
}
