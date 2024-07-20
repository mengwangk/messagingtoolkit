using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;

using Xunit;

using MessagingToolkit.Barcode;

namespace MessagingToolkit.Barcode.Test
{
    /// <summary>
    /// Test class
    /// </summary>
    public class TestBarcodeDecoder
    {

        private BarcodeDecoder decoder;


        public TestBarcodeDecoder()
        {
            decoder = new BarcodeDecoder();
        }

        [Fact]
        public void TestPdf417Decode()
        {
            List<BarcodeFormat> formats = new List<BarcodeFormat>(1);
            formats.Add(BarcodeFormat.PDF417);

            Dictionary<DecodeOptions, object> decodeOptions = new Dictionary<DecodeOptions,object>(1);
            decodeOptions.Add(DecodeOptions.PossibleFormats, formats);
            decodeOptions.Add(DecodeOptions.TryHarder, false);
            decoder.SetOptions(decodeOptions);
            Result result = decoder.Decode(new Bitmap(Bitmap.FromFile(@"c:\temp\pppp.png")));
            Console.WriteLine(result.Text);
        }
    }
}
