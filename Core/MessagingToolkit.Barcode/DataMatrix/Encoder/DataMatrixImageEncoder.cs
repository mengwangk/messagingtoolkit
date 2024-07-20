using System;
using System.Text;
using System.Collections.Generic;

#if !PORTABLE

using System.Windows;

#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

#if WPF

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


#else

using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

#endif

#endif

#endif

using System.Runtime.InteropServices;
using System.Globalization;

using MessagingToolkit.Barcode.Helper;
using System.IO;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{

    internal class DataMatrixImageEncoder
    {
        public static readonly int DefaultDotSize = 5;
        public static readonly int DefaultMargin = 10;
#if !SILVERLIGHT  && !NETFX_CORE
        public static readonly Color DefaultBackColor = Color.White;
        public static readonly Color DefaultForeColor = Color.Black;
#else
#if NETFX_CORE
        public static readonly Color DefaultBackColor = Colors.White;
        public static readonly Color DefaultForeColor = Colors.Black;
#else
        public static readonly System.Windows.Media.Color DefaultBackColor = Colors.White;
        public static readonly System.Windows.Media.Color DefaultForeColor = Colors.Black;
#endif
#endif


#if !SILVERLIGHT && !NETFX_CORE
        public Bitmap EncodeImageMosaic(string val)
#else
        public WriteableBitmap EncodeImageMosaic(string val)
#endif
        {
            return EncodeImageMosaic(val, DefaultDotSize);
        }

#if !SILVERLIGHT && !NETFX_CORE
        public Bitmap EncodeImageMosaic(string val, int dotSize)
#else
        public WriteableBitmap EncodeImageMosaic(string val, int dotSize)
#endif
        {
            return EncodeImageMosaic(val, dotSize, DefaultMargin);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public Bitmap EncodeImageMosaic(string val, int dotSize, int margin)
#else
        public WriteableBitmap EncodeImageMosaic(string val, int dotSize, int margin)
#endif
        {
            DataMatrixImageEncoderOptions options = new DataMatrixImageEncoderOptions { MarginSize = margin, ModuleSize = dotSize };
            return EncodeImageMosaic(val, options);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public Bitmap EncodeImageMosaic(string val, DataMatrixImageEncoderOptions options)
#else
        public WriteableBitmap EncodeImageMosaic(string val, DataMatrixImageEncoderOptions options)
#endif
        {
            return EncodeImage(val, options, true);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        private Bitmap EncodeImage(string val, DataMatrixImageEncoderOptions options, bool isMosaic)
#else
        private WriteableBitmap EncodeImage(string val, DataMatrixImageEncoderOptions options, bool isMosaic)
#endif
        {
            DataMatrixEncode encode = new DataMatrixEncode
                                    {
                                        ModuleSize = options.ModuleSize,
                                        MarginSize = options.MarginSize,
                                        SizeIdxRequest = options.SizeIdx,
                                        Width = options.Width,
                                        Height = options.Height,
                                        QuietZone = options.QuietZone,
                                    };


            byte[] valAsByteArray = GetRawDataAndSetEncoding(val, options, encode);
            if (isMosaic)
            {
                encode.EncodeDataMosaic(valAsByteArray);
            }
            else
            {
                encode.EncodeDataMatrix(options.ForeColor, options.BackColor, valAsByteArray);
            }
            return CopyDataToBitmap(encode.Image.Pxl, encode.Image.Width, encode.Image.Height);


        }

        private static byte[] GetRawDataAndSetEncoding(string code, DataMatrixImageEncoderOptions options, DataMatrixEncode encode)
        {
            //byte[] result = Encoding.ASCII.GetBytes(code);
            byte[] result = Encoding.GetEncoding(options.CharacterSet).GetBytes(code);
            encode.Scheme = options.Scheme;
            if (options.Scheme == DataMatrixScheme.SchemeAsciiGS1)
            {
                List<byte> prefixedRawData = new List<byte>(new[] { (byte)232 });
                prefixedRawData.AddRange(result);
                result = prefixedRawData.ToArray();
                encode.Scheme = DataMatrixScheme.SchemeAscii;
            }
            return result;
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public Bitmap EncodeImage(string val)
#else
        public WriteableBitmap EncodeImage(string val)
#endif
        {
            return EncodeImage(val, DefaultDotSize, DefaultMargin);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public Bitmap EncodeImage(string val, int dotSize)
#else
        public WriteableBitmap EncodeImage(string val, int dotSize)
#endif
        {
            return EncodeImage(val, dotSize, DefaultMargin);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public Bitmap EncodeImage(string val, int dotSize, int margin)
#else
        public WriteableBitmap EncodeImage(string val, int dotSize, int margin)
#endif
        {
            DataMatrixImageEncoderOptions options = new DataMatrixImageEncoderOptions { MarginSize = margin, ModuleSize = dotSize };
            return EncodeImage(val, options);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public Bitmap EncodeImage(string val, DataMatrixImageEncoderOptions options)
#else
        public WriteableBitmap EncodeImage(string val, DataMatrixImageEncoderOptions options)
#endif
        {
            return EncodeImage(val, options, false);
        }

        public string EncodeSvgImage(string val)
        {
            return EncodeSvgImage(val, DefaultDotSize, DefaultMargin, DefaultForeColor, DefaultBackColor);
        }

        public string EncodeSvgImage(string val, int dotSize)
        {
            return EncodeSvgImage(val, dotSize, DefaultMargin, DefaultForeColor, DefaultBackColor);
        }

        public string EncodeSvgImage(string val, int dotSize, int margin)
        {
            return EncodeSvgImage(val, dotSize, margin, DefaultForeColor, DefaultBackColor);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public string EncodeSvgImage(string val, int dotSize, int margin, System.Drawing.Color foreColor, System.Drawing.Color backColor)
#else
#if NETFX_CORE
        public string EncodeSvgImage(string val, int dotSize, int margin, Color foreColor, Color backColor)

#else
        public string EncodeSvgImage(string val, int dotSize, int margin, System.Windows.Media.Color foreColor, System.Windows.Media.Color backColor)
#endif
#endif
        {
            DataMatrixImageEncoderOptions options = new DataMatrixImageEncoderOptions
                                                  {
                                                      ModuleSize = dotSize,
                                                      MarginSize = margin,
                                                      ForeColor = foreColor,
                                                      BackColor = backColor
                                                  };
            return EncodeSvgImage(val, options);
        }

        public bool[,] EncodeRawData(string val)
        {
            return EncodeRawData(val, new DataMatrixImageEncoderOptions());
        }

        public bool[,] EncodeRawData(string val, DataMatrixImageEncoderOptions options)
        {
            DataMatrixEncode encode = new DataMatrixEncode
                                    {
                                        ModuleSize = 1,
                                        MarginSize = 0,
                                        Width = options.Width,
                                        Height = options.Height,
                                        QuietZone = options.QuietZone,
                                        SizeIdxRequest = options.SizeIdx,
                                        Scheme = options.Scheme
                                    };

            byte[] valAsByteArray = GetRawDataAndSetEncoding(val, options, encode);

            encode.EncodeDataMatrixRaw(valAsByteArray);

            return encode.RawData;
        }

        public string EncodeSvgImage(string val, DataMatrixImageEncoderOptions options)
        {
            DataMatrixEncode encode = new DataMatrixEncode
                                    {
                                        ModuleSize = options.ModuleSize,
                                        MarginSize = options.MarginSize,
                                        SizeIdxRequest = options.SizeIdx,
                                        Width = options.Width,
                                        Height = options.Height,
                                        QuietZone = options.QuietZone,
                                        Scheme = options.Scheme
                                    };

            byte[] valAsByteArray = GetRawDataAndSetEncoding(val, options, encode);

            encode.EncodeDataMatrix(options.ForeColor, options.BackColor, valAsByteArray);

            return EncodeSvgFile(encode, "", options.ModuleSize, options.MarginSize, options.ForeColor, options.BackColor);
        }

#if !SILVERLIGHT  && !NETFX_CORE
        internal static Bitmap CopyDataToBitmap(byte[] data, int width, int height)
        {
            data = InsertPaddingBytes(data, width, height, 24);

            Bitmap bmp = new Bitmap(width, height);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
#elif WPF

        private static int ConvertColor(System.Windows.Media.Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            return col;
        }

        internal static WriteableBitmap CopyDataToBitmap(byte[] data, int width, int height)
        {
                data = InsertPaddingBytes(data, width, height, 24);
                Bitmap bmp = new Bitmap(width, height);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
                bmp.UnlockBits(bmpData);
                return new WriteableBitmap(BarcodeHelper.ToBitmapSource(bmp));
        }
       

#else


        /// <summary>
        /// Copies the data to bitmap image.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        internal static WriteableBitmap CopyDataToBitmap(byte[] data, int width, int height)
        {
            data = InsertPaddingBytes(data, width, height, 24);
            WriteableBitmap bmp = new WriteableBitmap(width, height);

            /*
            using (var stream = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsStream(bmp.PixelBuffer))
            {
                var bytes = new byte[4 * data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    var greyness = data[i];
                    bytes[4 * i] = greyness;
                    bytes[4 * i + 1] = greyness;
                    bytes[4 * i + 2] = greyness;
                    bytes[4 * i + 3] = 0xff;
                }
                stream.Write(bytes, 0, (int)bmp.PixelBuffer.Length);
            }
            */

            /*
           
            int pointer = 0;
            int index = 0;
            byte alpha = 255;
            while (pointer <= data.Length)
            {
#if NETFX_CORE
                uint limit = bmp.PixelBuffer.Length;
#else
                int limit = bmp.Pixels.Length;
#endif
                if (index <limit)
                {
                    using (var context = bmp.GetBitmapContext())
                    {
                        int color = (alpha << 24) | (data[pointer++]) << 16 | (data[pointer++] << 8) | data[pointer++];
                        context.Pixels[index++] = color; ;
                    }
                }
                else break;
            }
            bmp.Invalidate();
          */
            return bmp;
            

        }


        internal static int ConvertColor(Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            return col;

        }
#endif


        private static byte[] InsertPaddingBytes(byte[] data, int width, int height, int bitsPerPixel)
        {
            int paddedWidth = 4 * ((width * bitsPerPixel + 31) / 32);
            int padding = paddedWidth - 3 * width;
            if (padding == 0)
            {
                return data;
            }
            byte[] newData = new byte[paddedWidth * height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newData[i * paddedWidth + 3 * j] = data[3 * (i * width + j)];
                    newData[i * paddedWidth + 3 * j + 1] = data[3 * (i * width + j) + 1];
                    newData[i * paddedWidth + 3 * j + 2] = data[3 * (i * width + j) + 2];
                }
                for (int k = 0; k < padding; k++)
                {
                    newData[i * paddedWidth + 3 * k] = 255;
                    newData[i * paddedWidth + 3 * k + 1] = 255;
                    newData[i * paddedWidth + 3 * k + 2] = 255;
                }
            }
            return newData;
        }

        private static NumberFormatInfo _dotFormatProvider;

#if !SILVERLIGHT  && !NETFX_CORE
        internal string EncodeSvgFile(DataMatrixEncode enc, string format, int moduleSize, int margin, System.Drawing.Color foreColor, System.Drawing.Color backColor)
#else
#if NETFX_CORE
        internal string EncodeSvgFile(DataMatrixEncode enc, string format, int moduleSize, int margin, Color foreColor, Color backColor)

#else
        internal string EncodeSvgFile(DataMatrixEncode enc, string format, int moduleSize, int margin, System.Windows.Media.Color foreColor, System.Windows.Media.Color backColor)
#endif
#endif
        {
            bool defineOnly = false;
            string idString = null;
            string style;
            string outputString = "";

            if (_dotFormatProvider == null)
            {
                _dotFormatProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            }

            if (format == "svg:")
            {
                defineOnly = true;
                idString = format.Substring(4);
            }

            if (string.IsNullOrEmpty(idString))
            {
                idString = "dmtx_0001";
            }

            int width = 2 * enc.MarginSize + (enc.Region.SymbolCols * enc.ModuleSize);
            int height = 2 * enc.MarginSize + (enc.Region.SymbolRows * enc.ModuleSize);

            int symbolCols = DataMatrixCommon.GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolCols, enc.Region.SizeIdx);
            int symbolRows = DataMatrixCommon.GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolRows, enc.Region.SizeIdx);

            /* Print SVG Header */
            if (!defineOnly)
            {
                outputString += string.Format(
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n" +
                    "<!-- Created with MessagingToolkit.Barcode.DataMatrix.Pdf417Encoder (http://datamatrixnet.sourceforge.Pdf417Encoder/) -->\n" +
          "<svg\n" +
             "xmlns:svg=\"http://www.w3.org/2000/svg\"\n" +
             "xmlns=\"http://www.w3.org/2000/svg\"\n" +
             "xmlns:xlink=\"http://www.w3.org/1999/xlink\"\n" +
             "version=\"1.0\"\n" +
             "width=\"{0}\"\n" +
             "height=\"{1}\"\n" +
             "id=\"svg2\">\n" +
            "<defs>\n" +
            "<symbol id=\"{2}\">\n" +
                 "    <desc>Layout:{0}x%{1} Symbol:{3}x{4} data Matrix</desc>\n", width, height, idString, symbolCols, symbolRows);
            }

#if !SILVERLIGHT  && !NETFX_CORE
            if (backColor != System.Drawing.Color.White)
#else
            if (backColor != Colors.White)
#endif
            {
                style = string.Format("style=\"fill:#{0}{1}{2};fill-opacity:{3};stroke:none\" ",
                              backColor.R.ToString("X2"), backColor.G.ToString("X2"), backColor.B.ToString("X2"), ((double)backColor.A / (double)byte.MaxValue).ToString("0.##", _dotFormatProvider));
                outputString += string.Format("    <rect width=\"{0}\" height=\"{1}\" x=\"0\" y=\"0\" {2}/>\n",
                      width, height, style);
            }

            /* Write Data Matrix ON modules */
            for (int row = 0; row < enc.Region.SymbolRows; row++)
            {
                int rowInv = enc.Region.SymbolRows - row - 1;
                for (int col = 0; col < enc.Region.SymbolCols; col++)
                {
                    int module = enc.Message.SymbolModuleStatus(enc.Region.SizeIdx, row, col);
                    style = string.Format("style=\"fill:#{0}{1}{2};fill-opacity:{3};stroke:none\" ",
                          foreColor.R.ToString("X2"), foreColor.G.ToString("X2"), foreColor.B.ToString("X2"), ((double)foreColor.A / (double)byte.MaxValue).ToString("0.##", _dotFormatProvider));

                    if ((module & DataMatrixConstants.DataMatrixModuleOn) != 0)
                    {
                        outputString += string.Format("    <rect width=\"{0}\" height=\"{1}\" x=\"{2}\" y=\"{3}\" {4}/>\n",
                              moduleSize, moduleSize,
                              col * moduleSize + margin,
                              rowInv * moduleSize + margin, style);
                    }
                }
            }

            outputString += "  </symbol>\n";

            /* Close SVG document */
            if (!defineOnly)
            {
                outputString += string.Format("</defs>\n" +
            "<use xlink:href=\"#{0}\" x='0' y='0' style=\"fill:#000000;fill-opacity:1;stroke:none\" />\n" +
          "\n</svg>\n", idString);
            }

            return outputString;
        }
    }
}
