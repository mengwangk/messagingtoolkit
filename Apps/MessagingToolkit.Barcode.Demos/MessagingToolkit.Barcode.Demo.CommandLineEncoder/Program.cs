using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Pdf417.Encoder;
using MessagingToolkit.Barcode.QRCode;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Client.Results;
using MessagingToolkit.Barcode.Multi;

namespace MessagingToolkit.Barcode.Demo.CommandLineEncoder
{
    class Program
    {
        private const BarcodeFormat DefaultBarcodeFormat = BarcodeFormat.QRCode;
        private const String DefaultImageFormat = "PNG";
        private const String DefaultOutputFile = "out";
        private const int DefaultWidth = 300;
        private const int DefaultHeight = 300;

        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            BarcodeFormat barcodeFormat = DefaultBarcodeFormat;
            String imageFormat = DefaultImageFormat;
            String outFileString = DefaultOutputFile;
            int width = DefaultWidth;
            int height = DefaultHeight;
            var clipboard = false;

            foreach (String arg in args)
            {
                if (arg.StartsWith("--barcode_format", StringComparison.OrdinalIgnoreCase))
                {
                    barcodeFormat = barcodeFormat = (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), arg.Split('=')[1].Trim());
                }
                else if (arg.StartsWith("--image_format", StringComparison.OrdinalIgnoreCase))
                {
                    imageFormat = arg.Split('=')[1];
                }
                else if (arg.StartsWith("--output", StringComparison.OrdinalIgnoreCase))
                {
                    outFileString = arg.Split('=')[1];
                }
                else if (arg.StartsWith("--width", StringComparison.OrdinalIgnoreCase))
                {
                    width = Convert.ToInt32(arg.Split('=')[1]);
                }
                else if (arg.StartsWith("--height", StringComparison.OrdinalIgnoreCase))
                {
                    height = Convert.ToInt32(arg.Split('=')[1]);
                }
                else if (arg.StartsWith("--copy_to_clipboard"))
                {
                    clipboard = true;
                }
            }

            if (DefaultOutputFile.Equals(outFileString, StringComparison.OrdinalIgnoreCase))
            {
                outFileString += '.' + imageFormat.ToLower();
            }

            String contents = null;
            foreach (String arg in args)
            {
                if (!arg.StartsWith("--"))
                {
                    contents = arg;
                    break;
                }
            }

            if (contents == null)
            {
                PrintUsage();
                return;
            }

            BarcodeEncoder barcodeEncoder = new BarcodeEncoder();
            barcodeEncoder.Width = width;
            barcodeEncoder.Height = height;
            Image image = barcodeEncoder.Encode(barcodeFormat, contents);
            ImageFormat saveImageFormat = ImageFormat.Png;
            switch (imageFormat.ToLower())
            {
                case "png":
                    saveImageFormat = ImageFormat.Png;
                    break;
                case "jpeg":
                case "jpg":
                    saveImageFormat = ImageFormat.Jpeg;
                    break;
                case "bmp":
                    saveImageFormat = ImageFormat.Bmp;
                    break;
                case "emf":
                    saveImageFormat = ImageFormat.Emf;
                    break;
                case "gif":
                    saveImageFormat = ImageFormat.Gif;
                    break;
                case "icon":
                    saveImageFormat = ImageFormat.Icon;
                    break;
                case "wmf":
                    saveImageFormat = ImageFormat.Wmf;
                    break;
                case "tiff":
                    saveImageFormat = ImageFormat.Tiff;
                    break;
            }
            if (clipboard)
            {
                System.Windows.Forms.Clipboard.SetImage(image);
            }
            else
            {
                image.Save(outFileString, saveImageFormat);
            }
        }


        private static void PrintUsage()
        {
            Console.WriteLine("Encodes barcode images using the library\n");
            Console.WriteLine("usage: CommandLineEncoder [ options ] content_to_encode");
            Console.WriteLine("  --barcode_format=format: Format to encode, from BarcodeFormat class. " +
                                   "Not all formats are supported. Defaults to QR_CODE.");
            Console.WriteLine("  --image_format=format: image output format, such as PNG, JPG, GIF. Defaults to PNG");
            Console.WriteLine("  --output=filename: File to write to. Defaults to out.png");
            Console.WriteLine("  --width=pixels: Image width. Defaults to 300");
            Console.WriteLine("  --height=pixels: Image height. Defaults to 300");
            Console.Out.WriteLine("  --copy_to_clipboard: Copy the image to the clipboard instead saving to a file");
        }



    }
}
