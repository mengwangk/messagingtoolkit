using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Provider;
using MessagingToolkit.Barcode.Windows8.ClassLibraryDemo.Helper;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MessagingToolkit.Barcode.Windows8.ClassLibraryDemo
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : MessagingToolkit.Barcode.Windows8.ClassLibraryDemo.Common.LayoutAwarePage
    {
        private WriteableBitmap barcodeImage = null;

        private static Dictionary<string, BarcodeFormat> BarcodeTypes =
                       new Dictionary<string, BarcodeFormat>
                        {
                             {"QRCode", BarcodeFormat.QRCode},
                             {"Data Matrix", BarcodeFormat.DataMatrix},
                             {"PDF417", BarcodeFormat.PDF417},
                             {"Aztec", BarcodeFormat.Aztec},
                             {"CodaBar", BarcodeFormat.Codabar},
                             {"Code 128", BarcodeFormat.Code128},
                             {"Code 39", BarcodeFormat.Code39},
                             {"EAN-13", BarcodeFormat.EAN13},
                             {"EAN-8", BarcodeFormat.EAN8},
                             {"ITF-14", BarcodeFormat.ITF14},
                             {"UPC-A", BarcodeFormat.UPCA},
                             {"MSI Mod 10", BarcodeFormat.MSIMod10},
                        };

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

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Populate the character set
            foreach (object obj in CharacterSets.Keys)
            {
                cboCharacters.Items.Add(obj.ToString());
            }
            cboCharacters.SelectedIndex = 0;

            // Populate the barcode types
            foreach (object obj in BarcodeTypes.Keys)
            {
                cboBarcodeTypes.Items.Add(obj.ToString());
            }
            cboBarcodeTypes.SelectedIndex = 0;

            // Populate error correction levels
            foreach (object obj in ErrorCorrectionLevels.Keys)
            {
                cboErrorCorrectionLevel.Items.Add(obj.ToString());
            }
            cboErrorCorrectionLevel.SelectedIndex = 0;

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }


        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the barcode content
                string text = txtBarcodeContent.Text;

                // Get the barcode format that we want to generate
                BarcodeFormat format = BarcodeTypes[cboBarcodeTypes.SelectedValue as string];

                // Create a instance of barcode encoder
                BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

                // Set the character set
                barcodeEncoder.CharacterSet = CharacterSets[cboCharacters.SelectedValue as string];

                // Set error correction level
                barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboErrorCorrectionLevel.SelectedValue as string];

                // Set quiet zone
                barcodeEncoder.Margin = GetInt(txtQuietZone, 4);

                // Set width
                barcodeEncoder.Width = GetInt(txtWidth, 250);

                // Set height
                barcodeEncoder.Height = GetInt(txtHeight, 250);

                // Perform encoding
                this.barcodeImage = barcodeEncoder.Encode(format, text);

                // Display the generated barcode image
                imgEncodedBarcode.Source = this.barcodeImage;

            }
            catch (Exception ex)
            {
                var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private async void btnGenerateSVG_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcodeContent.Text))
            {
                try
                {
                    // Get the barcode content
                    string text = txtBarcodeContent.Text;

                    // Get the barcode format that we want to generate
                    BarcodeFormat format = BarcodeTypes[cboBarcodeTypes.SelectedValue as string];

                    // Create a instance of barcode encoder
                    BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

                    // Set the character set
                    barcodeEncoder.CharacterSet = CharacterSets[cboCharacters.SelectedValue as string];

                    // Set error correction level
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboErrorCorrectionLevel.SelectedValue as string];

                    // Set quiet zone
                    barcodeEncoder.Margin = GetInt(txtQuietZone, 4);

                    // Set width
                    barcodeEncoder.Width = GetInt(txtWidth, 250);

                    // Set height
                    barcodeEncoder.Height = GetInt(txtHeight, 250);

                    SvgProvider svgProvider = new SvgProvider();
                    Svg svg = barcodeEncoder.Generate(format, txtBarcodeContent.Text, svgProvider);


                    var fileName = "barcode.svg";
                    var folder = KnownFolders.PicturesLibrary;
                    var option = Windows.Storage.CreationCollisionOption.ReplaceExisting;

                    var file = await folder.CreateFileAsync(fileName, option);
                    await Windows.Storage.FileIO.WriteTextAsync(file, svg.Content);

                    var result = MessageBox.ShowAsync(string.Format("{0} saved to {1}", fileName, folder.Name), "Info", MessageBoxButton.OK);
                }

                catch (Exception ex)
                {
                    var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                var result = MessageBox.ShowAsync("No SVG image", "Info", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Convert text box value to integer. Return a default value on failure.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int GetInt(TextBox txt, int defaultValue)
        {
            try
            {
                return Convert.ToInt32(txt.Text);
            }
            catch
            {
                return defaultValue;
            }
        }

        private void radBarcode_Click(object sender, RoutedEventArgs e)
        {
            var radBtn = sender as RadioButton;

            if (radBtn != null)
            {
                int index = Convert.ToInt32(radBtn.Tag);
                flipView.SelectedIndex = index;
            }
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.barcodeImage != null)
                {
                    txtBarcodeType.Text = string.Empty;
                    txtResult.Text = string.Empty;

                    BarcodeDecoder barcodeDecoder = new BarcodeDecoder();

                    // You can pass in additional decoding options if you want as additional parameter
                    Result result = barcodeDecoder.Decode(this.barcodeImage);
                    if (result != null)
                    {
                        txtBarcodeType.Text = "" + result.BarcodeFormat;
                        txtResult.Text = result.Text;
                    }
                    else
                    {
                        txtResult.Text = "No barcode found";
                    }
                }
            }
            catch (Exception ex)
            {
                var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OK);
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (barcodeImage == null)
            {
                var result = MessageBox.ShowAsync("No image to save", "Info", MessageBoxButton.OK);
                return;
            }

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("PNG", new List<string>() { ".png" });
            savePicker.FileTypeChoices.Add("JPEG", new List<string>() { ".jpg", ".jpeg" });
            savePicker.FileTypeChoices.Add("GIF", new List<string>() { ".gif" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "barcode";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                // write to file
                await WriteableBitmapSaveExtensions.SaveToFile(this.barcodeImage, KnownFolders.PicturesLibrary, file.Name);

                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    var result = MessageBox.ShowAsync("File " + file.Name + " was saved.", "Info", MessageBoxButton.OK);
                }
                else
                {
                    var result = MessageBox.ShowAsync("File " + file.Name + " couldn't be saved.", "Info", MessageBoxButton.OK);
                }
            }
            else
            {
                var result = MessageBox.ShowAsync("Operation cancelled.", "Info", MessageBoxButton.OK);
            }

        }

        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".gif");
                StorageFile file = await openPicker.PickSingleFileAsync();
               
                if (file != null)
                {
                    // Application now has read/write access to the picked file
                    WriteableBitmap bitmap  = await WriteableBitmapLoadExtensions.LoadAsync(file);
                    txtDecodeImage.Text = file.Name;
                    imgDecodeImage.Source = bitmap;
                    this.barcodeImage = bitmap;
                }
            }
            catch (Exception ex)
            {
                var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private async void btnGenerateEPS_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcodeContent.Text))
            {
                try
                {
                    // Get the barcode content
                    string text = txtBarcodeContent.Text;

                    // Get the barcode format that we want to generate
                    BarcodeFormat format = BarcodeTypes[cboBarcodeTypes.SelectedValue as string];

                    // Create a instance of barcode encoder
                    BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

                    // Set the character set
                    barcodeEncoder.CharacterSet = CharacterSets[cboCharacters.SelectedValue as string];

                    // Set error correction level
                    barcodeEncoder.ErrorCorrectionLevel = ErrorCorrectionLevels[cboErrorCorrectionLevel.SelectedValue as string];

                    // Set quiet zone
                    barcodeEncoder.Margin = GetInt(txtQuietZone, 4);

                    // Set width
                    barcodeEncoder.Width = GetInt(txtWidth, 250);

                    // Set height
                    barcodeEncoder.Height = GetInt(txtHeight, 250);

                    EpsProvider epsProvider = new EpsProvider();
                    Eps svg = barcodeEncoder.Generate(format, txtBarcodeContent.Text, epsProvider);

                    var fileName = "barcode.eps";
                    var folder = KnownFolders.PicturesLibrary;
                    var option = Windows.Storage.CreationCollisionOption.ReplaceExisting;

                    var file = await folder.CreateFileAsync(fileName, option);
                    await Windows.Storage.FileIO.WriteTextAsync(file, svg.Content);

                    var result = MessageBox.ShowAsync(string.Format("{0} saved to {1}", fileName, folder.Name), "Info", MessageBoxButton.OK);
                }

                catch (Exception ex)
                {
                    var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                var result = MessageBox.ShowAsync("No EPS image", "Info", MessageBoxButton.OK);
            }
        }


    }
}
