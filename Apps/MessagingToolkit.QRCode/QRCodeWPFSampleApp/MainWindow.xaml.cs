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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using MessagingToolkit.QRCode.Helper;

namespace QRCodeWPFSampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Dictionary<string, string> CharacterSets =
                       new Dictionary<string, string>
                        {
                             { "Default UTF-8", "UTF-8"},
                             { "ISO-8859-1", "ISO-8859-1"},
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

        private static Dictionary<string, QRCodeEncoder.ENCODE_MODE> Encodings =
                      new Dictionary<string, QRCodeEncoder.ENCODE_MODE>
                        {
                             { "Byte", QRCodeEncoder.ENCODE_MODE.BYTE},
                             { "Alphanumeric", QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC},
                             { "Numeric", QRCodeEncoder.ENCODE_MODE.NUMERIC},
                        };

        private static Dictionary<string, QRCodeEncoder.ERROR_CORRECTION> CorrectionLevels =
                   new Dictionary<string, QRCodeEncoder.ERROR_CORRECTION>
                        {
                             { "H", QRCodeEncoder.ERROR_CORRECTION.H},
                             { "M", QRCodeEncoder.ERROR_CORRECTION.M},
                             { "L", QRCodeEncoder.ERROR_CORRECTION.L},
                             { "Q", QRCodeEncoder.ERROR_CORRECTION.Q},
                        };


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            string data = txtEncodeData.Text;
            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeEncodeMode = Encodings[cboEncoding.Text];
            encoder.QRCodeErrorCorrect = CorrectionLevels[cboCorrectionLevel.Text];
            encoder.CharacterSet = CharacterSets[cboCharacterSet.Text];
            try
            {
                int scale = Convert.ToInt16(txtSize.Text);
                encoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid size!");
                return;
            }
            try
            {
                int version = Convert.ToInt16(cboVersion.Text);
                encoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid version !");
            }

            Image image = encoder.Encode(data);
            BitmapSource bitmapSource = ImageHelper.ToBitmapSource(image);
            picEncode.Source = bitmapSource;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate the encoding
            Encodings.Keys.ToList<String>().ForEach(item => cboEncoding.Items.Add(item));
            cboEncoding.SelectedIndex = 0;

            // Populate the character set
            CharacterSets.Keys.ToList<String>().ForEach(item=>cboCharacterSet.Items.Add(item));
            cboCharacterSet.SelectedIndex = 0;

            // Populate the error correction levels
            CorrectionLevels.Keys.ToList<String>().ForEach(item => cboCorrectionLevel.Items.Add(item));
            cboCorrectionLevel.SelectedIndex = 0;

            for (int i = 0; i <= 40; i++)
            {
                cboVersion.Items.Add(i);
            }
            cboVersion.SelectedIndex = 0;

            txtSize.Text = "4";
        }

        private void btnForeColor_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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

                    BitmapSource bmSource = (BitmapSource)picEncode.Source;
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
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
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

                // Open a Stream and decode a PNG image
                Stream imageStreamSource = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                // Draw the Image
                picDecode.Source = bitmapSource;
                picDecode.Stretch = Stretch.None;
                picDecode.Margin = new Thickness(20);


            }
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (picDecode.Source == null) return;
                QRCodeDecoder decoder = new QRCodeDecoder();
                Bitmap bm = ImageHelper.ToWinFormsBitmap((BitmapSource)picDecode.Source);
                String decodedString = decoder.Decode(new QRCodeBitmapImage(bm));
                txtDecodedData.Text = decodedString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }               
    }
}
