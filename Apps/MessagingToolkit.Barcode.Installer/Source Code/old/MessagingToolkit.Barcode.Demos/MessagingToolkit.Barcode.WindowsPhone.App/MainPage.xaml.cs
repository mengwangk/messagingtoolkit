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
using Microsoft.Phone.Controls;

namespace MessagingToolkit.Barcode.WindowsPhone.App
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Encoder.xaml", UriKind.Relative));
        }

        private void btnDecode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Decoder.xaml", UriKind.Relative));
        }
    }
}