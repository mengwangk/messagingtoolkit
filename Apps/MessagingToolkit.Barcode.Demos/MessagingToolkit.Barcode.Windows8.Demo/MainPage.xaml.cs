using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


using MessagingToolkit.Barcode.QRCode.Decoder;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MessagingToolkit.Barcode.Windows8.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        


        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            var radBtn = sender as RadioButton;

            if (radBtn != null)
            {
                int index = Convert.ToInt32(radBtn.Tag);

                myflipVw.SelectedIndex = index;
            }
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string text = txtQRCode.Text;
                BarcodeEncoder barcodeEncoder = new BarcodeEncoder();
                WriteableBitmap bitmap = barcodeEncoder.Encode(BarcodeFormat.QRCode, text);
                imgQRCode.Source = bitmap;
               
            }
            catch (Exception ex)
            {
                var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OKCancel);
            }
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".png");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.SettingsIdentifier = "picker1";
            filePicker.CommitButtonText = "Open File to Process";

            var files = await filePicker.PickMultipleFilesAsync();
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {

        }

        async private void LoadImage()
        {
            try
            {
                IReadOnlyList<Windows.Storage.StorageFile> resultsLibrary =
                    await Windows.Storage.KnownFolders.PicturesLibrary.GetFilesAsync();
                Windows.Storage.Streams.IRandomAccessStream imageStream =
                    await resultsLibrary[0].OpenAsync(Windows.Storage.FileAccessMode.Read);
                Windows.UI.Xaml.Media.Imaging.BitmapImage imageBitmap =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                imageBitmap.SetSource(imageStream);
                imgDecodeImage.Source = imageBitmap;
            }
            catch (Exception ex)
            {
                var result = MessageBox.ShowAsync(ex.Message, "Error", MessageBoxButton.OKCancel);

            }
        }

        private async void btnSaveQRCode_Click(object sender, RoutedEventArgs e)
        {
            var fileSavePicker = new FileSavePicker();

            fileSavePicker.FileTypeChoices.Add("PNG Image", new List<string> { ".jpg" });
            fileSavePicker.DefaultFileExtension = ".png";
            fileSavePicker.SettingsIdentifier = "picker1";
            var fileToSave = await fileSavePicker.PickSaveFileAsync();

        }
    }
}
