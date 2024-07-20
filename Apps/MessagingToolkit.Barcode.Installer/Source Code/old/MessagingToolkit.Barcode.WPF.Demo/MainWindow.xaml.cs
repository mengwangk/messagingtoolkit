using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Pdf417.Encoder;

namespace MessagingToolkit.Barcode.WPF.Demo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BitmapSource bitmapSource = null;
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
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeApp();
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

        private void InitializeApp()
        {
            // Populate the character set
            CharacterSets.Keys.ToList<String>().ForEach(item => cboQRCodeCharacterSet.Items.Add(item));
            cboQRCodeCharacterSet.SelectedIndex = 0;

            // Populate error correction levels
            ErrorCorrectionLevels.Keys.ToList<String>().ForEach(item => cboQRCodeErrorCorrectionLevel.Items.Add(item));
            cboQRCodeErrorCorrectionLevel.SelectedIndex = 0;

            OneDBarcodeFormats.Keys.ToList<String>().ForEach(item => cboOneDBarcodeFormat.Items.Add(item));
            cboOneDBarcodeFormat.SelectedIndex = 0;

            OneDBarcodeAlignmentPositions.Keys.ToList<String>().ForEach(item => cboOneDBarcodeAlignment.Items.Add(item));
            cboOneDBarcodeAlignment.SelectedIndex = 0;

            OneDBarcodeLabelPositions.Keys.ToList<String>().ForEach(item => cboOneDBarcodeLabelLocation.Items.Add(item));
            cboOneDBarcodeLabelLocation.SelectedIndex = 0;

            QRCodeContentTypes.Keys.ToList<String>().ForEach(item => cboQRCodeContentType.Items.Add(item));

            CharacterSets.Keys.ToList<String>().ForEach(item => cboQRCodeMiscCharacterSet.Items.Add(item));
            cboQRCodeMiscCharacterSet.SelectedIndex = 0;

            ErrorCorrectionLevels.Keys.ToList<String>().ForEach(item => cboQRCodeMiscErrorCorrectionLevel.Items.Add(item));
            cboQRCodeMiscErrorCorrectionLevel.SelectedIndex = 0;

            Pdf417Compactions.Keys.ToList<String>().ForEach(item => cboPdf417Compaction.Items.Add(item));
            cboPdf417Compaction.SelectedIndex = 0;

        }

        private void btnEncodeQRCode_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQRCodeData.Text))
            {
                try
                {
                    barcodeEncoder.ClearAllOptions();
                    barcodeEncoder.CharacterSet = CharacterSets[cboQRCodeCharacterSet.Text];
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboQRCodeErrorCorrectionLevel.Text];

                    barcodeEncoder.ForeColor = ImageHelper.ToWinFormsColor(btnQRCodeForeColor.SelectedColor);
                    barcodeEncoder.BackColor = ImageHelper.ToWinFormsColor(btnQRCodeBackColor.SelectedColor);
                    barcodeEncoder.Margin = Convert.ToInt32(txtQRCodeQuietZone.Text);
                    barcodeEncoder.Width = Convert.ToInt32(txtQRCodeWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtQRCodeHeight.Text);

                    if (!string.IsNullOrEmpty(txtQRCodeLogoFileName.Text) && File.Exists(txtQRCodeLogoFileName.Text))
                    {
                        Image logo = Image.FromFile(@txtQRCodeLogoFileName.Text);
                        barcodeEncoder.AddOption(EncodeOptions.QRCodeLogo, logo);
                    }

                    Image image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtQRCodeData.Text);
                    BitmapSource bitmapSource = ImageHelper.ToBitmapSource(image);
                    picEncodedQRCode.Source = bitmapSource;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the QR Code content");
            }
        }

        private void btnSaveQRCode_Click(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource = picEncodedQRCode.Source;
            SaveImage(imageSource);          
        }

        private void cboQRCodeContentType_DropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboQRCodeContentType.Text))
            {
                Func<string> func = QRCodeContentTypes[cboQRCodeContentType.Text];
                txtQRCodeMiscData.Text = func();
            }
        }

        private void btnQRCodeMiscEncode_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQRCodeMiscData.Text))
            {
                try
                {
                    barcodeEncoder.ClearAllOptions();
                    barcodeEncoder.CharacterSet = CharacterSets[cboQRCodeMiscCharacterSet.Text];
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboQRCodeMiscErrorCorrectionLevel.Text];
                    barcodeEncoder.ForeColor = ImageHelper.ToWinFormsColor(btnQRCodeMiscForeColor.SelectedColor);
                    barcodeEncoder.BackColor = ImageHelper.ToWinFormsColor(btnQRCodeMiscBackColor.SelectedColor);
                    barcodeEncoder.Margin = Convert.ToInt32(txtQRCodeMiscQuietZone.Text);
                    barcodeEncoder.Width = Convert.ToInt32(txtQRCodeMiscWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtQRCodeMiscHeight.Text);

                    Image image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtQRCodeMiscData.Text);
                    BitmapSource bitmapSource = ImageHelper.ToBitmapSource(image);
                    picEncodedQRCodeMisc.Source = bitmapSource;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the QR Code content");
            }
        }

        private void btnQRCodeMiscSave_Click(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource = picEncodedQRCodeMisc.Source;
            SaveImage(imageSource);     
        }

        private void btnEncodeOneDBarcode_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOneDBarcodeData.Text))
            {
                try
                {
                    BarcodeFormat fmt = OneDBarcodeFormats[cboOneDBarcodeFormat.Text];
                    barcodeEncoder.ForeColor = ImageHelper.ToWinFormsColor(btnOneDBarcodeForeColor.SelectedColor);
                    barcodeEncoder.BackColor = ImageHelper.ToWinFormsColor(btnOneDBarcodeBackColor.SelectedColor);
                    barcodeEncoder.Width = Convert.ToInt32(txtOneDBarcodeWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtOneDBarcodeHeight.Text);
                    barcodeEncoder.IncludeLabel = chkOneDBarcodeGeneralLabel.IsChecked.Value;
                    barcodeEncoder.Alignment = OneDBarcodeAlignmentPositions[cboOneDBarcodeAlignment.Text];
                    barcodeEncoder.LabelPosition = OneDBarcodeLabelPositions[cboOneDBarcodeLabelLocation.Text];
                    barcodeEncoder.CustomLabel = txtOneDBarcodeCustomLabel.Text;

                    if (!string.IsNullOrEmpty(txtOneDBarcodeMargin.Text))
                    {
                        try
                        {
                            barcodeEncoder.Margin = Convert.ToInt32(txtOneDBarcodeMargin.Text);
                        }
                        catch
                        {
                        }
                    }

                    Image image = barcodeEncoder.Encode(fmt, txtOneDBarcodeData.Text);
                    BitmapSource bitmapSource = ImageHelper.ToBitmapSource(image);
                    picEncodedOneDBarcode.Source = bitmapSource;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the barcode content");
            }
        }

        private void btnSaveOneDBarcode_Click(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource = picEncodedOneDBarcode.Source;
            SaveImage(imageSource);    
        }

        private void btnEncodeDataMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDataMatrixData.Text))
            {
                try
                {
                    barcodeEncoder.ForeColor = ImageHelper.ToWinFormsColor(btnDataMatrixForeColor.SelectedColor);
                    barcodeEncoder.BackColor = ImageHelper.ToWinFormsColor(btnDataMatrixBackColor.SelectedColor);
                    barcodeEncoder.ModuleSize = Convert.ToInt32(txtDataMatrixModuleSize.Text);
                    barcodeEncoder.MarginSize = Convert.ToInt32(txtDataMatrixMarginSize.Text);

                    Image image = barcodeEncoder.Encode(BarcodeFormat.DataMatrix, txtDataMatrixData.Text);
                    BitmapSource bitmapSource = ImageHelper.ToBitmapSource(image);
                    picEncodedDataMatrix.Source = bitmapSource;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the data Matrix content");
            }
        }

        private void btnSaveDataMatrix_Click(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource = picEncodedDataMatrix.Source;
            SaveImage(imageSource);    
        }

        private void btnEncodePDF417_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPdf417Data.Text))
            {
                try
                {
                    barcodeEncoder.ForeColor = ImageHelper.ToWinFormsColor(btnPdf417ForeColor.SelectedColor);
                    barcodeEncoder.BackColor = ImageHelper.ToWinFormsColor(btnPdf417BackColor.SelectedColor);
                    barcodeEncoder.Width = Convert.ToInt32(txtPdf417Width.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtPdf417Height.Text);
                    barcodeEncoder.Pdf417Compaction = Pdf417Compactions[cboPdf417Compaction.Text];

                    Image image = barcodeEncoder.Encode(BarcodeFormat.PDF417, txtPdf417Data.Text);
                    BitmapSource bitmapSource = ImageHelper.ToBitmapSource(image);
                    picEncodedPdf417.Source = bitmapSource;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the Pdf417 content");
            }
        }

        private void btnSavePDF417_Click(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource = picEncodedPdf417.Source;
            SaveImage(imageSource);      
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            if (bitmapSource == null) return;
            DecodeBarcode(bitmapSource);
            TimeSpan ts = DateTime.Now.Subtract(dt);
            lblTime.Content = "Time taken: " + ts.Milliseconds.ToString() + "ms";
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "PNG Image (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                txtFile.Text = filename;

                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                bitmapSource = decoder.Frames[0];

                // Draw the Image
                picBoxDecode.Source = bitmapSource;
                picBoxDecode.Stretch = Stretch.None;
                picBoxDecode.Margin = new Thickness(20);
            }

        }


        private void DecodeBarcode(BitmapSource bmSource)
        {
            if (bmSource == null) return;

            Dictionary<DecodeOptions, object> decodingOptions = new Dictionary<DecodeOptions, object>();
            List<BarcodeFormat> possibleFormats = new List<BarcodeFormat>(10);

            if (chkDataMatrix.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.DataMatrix);
            }
            if (chkQRCode.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.QRCode);
            }
            if (chkPdf417.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.PDF417);
            }

            if (chkAztec.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.Aztec);
            }

            if (chkMaxiCode.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.MaxiCode);
            }

            if (chkUpcE.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.UPCE);
            }
            if (chkUpcA.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.UPCA);
            }
            if (chkCode128.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.Code128);
            }
            if (chkCode39.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.Code39);
            }
            if (chkItf.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.ITF14);
            }
            if (chkEan8.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.EAN8);
            }
            if (chkEan13.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.EAN13);
            }

            if (chkRss14.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.RSS14);
            }

            if (chkRssExpanded.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.RSSExpanded);
            }

            if (chkCodaBar.IsChecked.Value)
            {
                possibleFormats.Add(BarcodeFormat.Codabar);
            }

            if (chkTryHarder.IsChecked.Value)
            {
                decodingOptions.Add(DecodeOptions.TryHarder, true);
            }
            if (chkTryAllFormats.IsChecked.Value)
            {
                decodingOptions.Add(DecodeOptions.PossibleFormats, possibleFormats);
            }


            if (chkPureBarcode.IsChecked.Value)
            {
                decodingOptions.Add(DecodeOptions.PureBarcode, "");
            }


            try
            {
                Bitmap bitmap = ImageHelper.ToWinFormsBitmap(bmSource);
                if (!chkMultipleBarcodes.IsChecked.Value)
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
                MessageBox.Show(e.Message, "Decoder", MessageBoxButton.OK);
            }
        }

        private void chkMaxiCode_Checked(object sender, RoutedEventArgs e)
        {
            if (chkMaxiCode.IsChecked.Value && chkPureBarcode != null)
            {
                MessageBox.Show("Please make sure PureBarcode option is checked");
            }
        }

        private void SaveImage(ImageSource imageSource)
        {
            try
            {
                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = ""; // Default file name
                dlg.DefaultExt = ".png"; // Default file extension
                dlg.Filter = "PNG Image (.png)|*.png"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;

                    BitmapSource bmSource = (BitmapSource)imageSource;
                    FileStream stream = new FileStream(filename, FileMode.Create);
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Interlace = PngInterlaceOption.On;
                    encoder.Frames.Add(BitmapFrame.Create(bmSource));
                    encoder.Save(stream);
                    stream.Close();
                    MessageBox.Show("File is saved successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tabAbout_GotFocus(object sender, RoutedEventArgs e)
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
            lblAbout.Content = title;
            lblVersion.Content = version;
        }

        public static byte[] ImageToByteArray(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        private void btnBrowseQRCodeLogo_Click(object sender, RoutedEventArgs e)
        {
             // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "PNG Image (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                txtQRCodeLogoFileName.Text = filename;
            }
        }
    }
}
