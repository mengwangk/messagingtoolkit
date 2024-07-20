using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Data;

using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Pdf417.Encoder;


namespace MessagingToolkit.Barcode.Silverlight.Demo
{
    public partial class MainPage : UserControl
    {
        private BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
        private BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

        private WriteableBitmap barcodeImage;

        private static Dictionary<string, string> CharacterSets =
                         new Dictionary<string, string>
                        {
                             { "UTF-8", "UTF-8"},
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
                           {"Codabar", BarcodeFormat.Codabar},
                           {"Code 128", BarcodeFormat.Code128},
                           {"Code 39", BarcodeFormat.Code39},
                           {"EAN-13", BarcodeFormat.EAN13},
                           {"EAN-8", BarcodeFormat.EAN8},
                           {"ITF-14", BarcodeFormat.ITF14},
                           {"UPC-A", BarcodeFormat.UPCA},
                           {"MSI Mod 10", BarcodeFormat.MSIMod10},
                           {"Plessey", BarcodeFormat.ModifiedPlessey},
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




        private static Dictionary<string, Compaction> Pdf417Compactions =
               new Dictionary<string, Compaction>
                       {
                           {"Auto", Compaction.Auto},
                           {"Byte", Compaction.Byte},
                           {"Numeric", Compaction.Numeric},
                           {"Text", Compaction.Text},
                          
                       };

        public MainPage()
        {
            InitializeComponent();
        }

        private void btnEncodeQRCode_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQRCode.Text))
            {
                try
                {

                    barcodeEncoder.CharacterSet = CharacterSets[cboQRCodeCharacterSet.SelectedValue as string];
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboQRCodeErrorCorrection.SelectedValue as string];

                    barcodeEncoder.ForeColor = colorQRCodeForeColor.SelectedColor;
                    barcodeEncoder.BackColor = colorQRCodeBackColor.SelectedColor;
                    barcodeEncoder.Margin = Convert.ToInt32(txtQRCodeQuietZone.Text);
                    barcodeEncoder.Width = Convert.ToInt32(txtQRCodeWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtQRCodeHeight.Text);
                    WriteableBitmap image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtQRCode.Text);
                    picQRCode.Source = image;
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

        private void btnEncodePdf417_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPdf417.Text))
            {
                try
                {
                    Dictionary<EncodeOptions, object> encodingOptions = new Dictionary<EncodeOptions, object>();
                    barcodeEncoder.Width = Convert.ToInt32(txtPdf417Width.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtPdf417Height.Text);
                    barcodeEncoder.Pdf417Compaction = Pdf417Compactions[cboPdf417Compaction.SelectedValue as string];

                    int quietZone = 4;  // Default to 4
                    try
                    {
                        quietZone = Convert.ToInt32(txtPdf417QuietZone.Text);
                        encodingOptions.Add(EncodeOptions.Margin, quietZone);
                    }
                    catch (Exception)
                    {
                    }

                    WriteableBitmap image = barcodeEncoder.Encode(BarcodeFormat.PDF417, txtPdf417.Text, encodingOptions);
                    picPdf417.Source = image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the PDF417 content");
            }
        }



        private void btnEncodeOneD_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOneD.Text))
            {
                try
                {
                    BarcodeFormat fmt = OneDBarcodeFormats[cboOneDFormat.SelectedValue as string];
                    barcodeEncoder.Width = Convert.ToInt32(txtOneDWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtOneDHeight.Text);
                    WriteableBitmap image = barcodeEncoder.Encode(fmt, txtOneD.Text);
                    picOneD.Source = image;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CharacterSets.Keys.ToList<String>().ForEach(item => cboQRCodeCharacterSet.Items.Add(item));
            cboQRCodeCharacterSet.SelectedIndex = 0;

            // Populate error correction levels
            ErrorCorrectionLevels.Keys.ToList<String>().ForEach(item => cboQRCodeErrorCorrection.Items.Add(item));
            cboQRCodeErrorCorrection.SelectedIndex = 0;

            OneDBarcodeFormats.Keys.ToList<String>().ForEach(item => cboOneDFormat.Items.Add(item));
            cboOneDFormat.SelectedIndex = 0;

            Pdf417Compactions.Keys.ToList<String>().ForEach(item => cboPdf417Compaction.Items.Add(item));
            cboPdf417Compaction.SelectedIndex = 0;
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (barcodeImage != null)
                {

                    WriteableBitmap image = new WriteableBitmap(barcodeImage);
                    Dictionary<DecodeOptions, object> decodingOptions = new Dictionary<DecodeOptions, object>();

                    /**
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
                    */

                    Result decodedResult = barcodeDecoder.Decode(image, decodingOptions);
                    if (decodedResult != null)
                    {
                        txtDecodeResults.Text = decodedResult.Text;
                    }
                    else
                    {
                        txtDecodeResults.Text = "No barcode found";
                    }

                }
                else
                {
                    MessageBox.Show("Nothing to decode. Please specify a valid image", "Decoder", MessageBoxButton.OK);
                }

            }
            catch (Exception ex)
            {
                txtDecodeResults.Text = ex.Message;
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "PNG (*.png)|*.png";
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                try
                {
                    barcodeImage = new WriteableBitmap(0, 0);
                    using (var stream = dlg.File.OpenRead())
                    {
                        barcodeImage.SetSource(stream);
                    }
                    picDecode.Source = barcodeImage;
                }
                catch (Exception exc)
                {
                    txtDecodeResults.Text = exc.Message;
                }
            }
        }

        private void btnEncodeDataMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDataMatrix.Text))
            {
                try
                {
                    Dictionary<EncodeOptions, object> encodingOptions = new Dictionary<EncodeOptions, object>();

                    barcodeEncoder.Width = Convert.ToInt32(txtDataMatrixWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtDataMatrixHeight.Text);

                    int quietZone = 4;  // Default to 4
                    try
                    {
                        quietZone = Convert.ToInt32(txtDataMatrixQuietZone.Text);
                        encodingOptions.Add(EncodeOptions.Margin, quietZone);
                    }
                    catch (Exception ex)
                    {
                    }

                    WriteableBitmap image = barcodeEncoder.Encode(BarcodeFormat.DataMatrix, txtDataMatrix.Text, encodingOptions);
                    picDataMatrix.Source = image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the PDF417 content");
            }
        }

        private void btnEncodeAztec_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAztec.Text))
            {
                try
                {
                    barcodeEncoder.Width = Convert.ToInt32(txtAztecWidth.Text);
                    barcodeEncoder.Height = Convert.ToInt32(txtAztecHeight.Text);
                    picAztec.Source = barcodeEncoder.Encode(BarcodeFormat.Aztec, txtAztec.Text);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please specify the Aztec content");
            }
        }
    }
}
