﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


namespace MessagingToolkit.Barcode.WindowsPhone8.App
{
    public partial class Decoder : PhoneApplicationPage
    {
        private static Dictionary<string, string> ImageList =
                new Dictionary<string, string>
                       {
                           {"Sample QR Code", "/Assets/Samples/sample-qrcode.png"},
                           {"Sample Data Matrix", "/Assets/Samples/sample-datamatrix.png"},
                           {"Sample PDF417", "/Assets/Samples/sample-pdf417.png"},
                           {"Sample Aztec", "/Assets/Samples/sample-aztec.png"},
                           {"Sample Code 128", "/Assets/Samples/sample-code128.png"},
                           {"Sample EAN 13", "/Assets/Samples/sample-ean13.png"},
                           {"Sample UPCA", "/Assets/Samples/sample-upca.png"},
                          
                       };


        private BarcodeDecoder barcodeDecoder = new BarcodeDecoder();


        public Decoder()
        {
            InitializeComponent();
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (picBarcode.Source != null)
                {
                    ListBoxItem selected = lstSamples.SelectedItem as ListBoxItem;
                    string selectedText = selected.Content as string;
                    BitmapImage bmp = null;
                    if (ImageList.ContainsKey(selectedText))
                    {
                        string uri = ImageList[selectedText];
                        bmp = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
                        bmp.CreateOptions = BitmapCreateOptions.None;
                    }

                    WriteableBitmap image = new WriteableBitmap(bmp);
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
                        txtResult.Text = decodedResult.Text;
                    }
                }
                else
                {
                    MessageBox.Show("Nothing to decode. Please specify a valid image", "Decoder", MessageBoxButton.OK);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Decoder", MessageBoxButton.OK);
            }
        }

        private void lnkBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void lstSamples_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = lstSamples.SelectedItem as ListBoxItem;
            string selectedText = selected.Content as string;
            if (ImageList.ContainsKey(selectedText))
            {
                string uri = ImageList[selectedText];
                BitmapImage image = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
                image.CreateOptions = BitmapCreateOptions.None;
                picBarcode.Source = image;
            }
        }
    }
}