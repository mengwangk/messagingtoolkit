using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.QRCode.Decoder;

namespace MessagingToolkit.Barcode.WindowsPhone.App
{
    public partial class Encoder : PhoneApplicationPage
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
                           {"Standard 2 of 5", BarcodeFormat.Standard2of5},
                           {"Telepen", BarcodeFormat.Telepen},
                           {"UPC 2 Digit Ext.", BarcodeFormat.UPCSupplemental2Digit},
                           {"UPC 5 Digit Ext.", BarcodeFormat.UPCSupplemental5Digit},
                           {"UPC-A", BarcodeFormat.UPCA},
                           {"UPC-E", BarcodeFormat.UPCE},
                           {"Plessey", BarcodeFormat.ModifiedPlessey},
                       };


        private BarcodeEncoder barcodeEncoder = new BarcodeEncoder();

        public Encoder()
        {
            InitializeComponent();
        }

        private void lnkBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = txtBarcodeData.Text;
                if (string.IsNullOrEmpty(data))
                {
                    MessageBox.Show("Please specify barcode content", "Encoder", MessageBoxButton.OK);
                    return;
                }

                // Get barcode format
                BarcodeFormat fmt = BarcodeFormat.QRCode;
				if (lstBarcodeType.SelectedItem == null)
                {
                    MessageBox.Show("Please specify barcode type", "Encoder", MessageBoxButton.OK);
                    return;
                }
                ListBoxItem selected = lstBarcodeType.SelectedItem as ListBoxItem;
                string selectedText = selected.Content as string; 
                if (BarcodeFormats.ContainsKey(selectedText))
                    fmt = BarcodeFormats[selectedText];
                WriteableBitmap image = barcodeEncoder.Encode(fmt, data);
                picEncodedBarcode.Source = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Encoder", MessageBoxButton.OK);
            }
        }
    }
}


