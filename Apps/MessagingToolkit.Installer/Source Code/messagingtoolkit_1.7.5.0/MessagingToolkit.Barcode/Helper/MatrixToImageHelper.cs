using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

#else

using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

#endif

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.OneD;

namespace MessagingToolkit.Barcode.Helper
{

    /// <summary>
    /// Convert BitMatrix to image
    /// </summary>
    public sealed class MatrixToImageHelper
    {

        private MatrixToImageHelper()
        {
        }

#if !SILVERLIGHT && !NETFX_CORE

        public static Image RenderBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap image = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, y, (matrix.Get(x, y)) ? foreColor : backColor);
                }
            }
            return image;
        }
        /// <summary>
        /// Renders a bit matrix as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        public static Image ToBitmap(BitMatrix matrix)
        {
            return RenderBitmap(matrix, Color.Black, Color.White);
        }

        /// <summary>
        /// Renders a bit matrix as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="foreColor">Color of the fore.</param>
        /// <param name="backColor">Color of the back.</param>
        /// <returns></returns>
        public static Image ToBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            return RenderBitmap(matrix, foreColor, backColor);
        }

#else

#if !WPF && !NETFX_CORE

        public static WriteableBitmap RenderBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            WriteableBitmap image = new WriteableBitmap(width, height);
            using (image.GetBitmapContext())
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color c = (matrix.Get(x, y)) ? foreColor : backColor;
                        SetPixel(image, x, y, c);

                    }
                }
            }
            return image;
        }
        /// <summary>
        /// Renders a <see cref="null"/> as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        public static WriteableBitmap ToBitmap(BitMatrix matrix)
        {
            return RenderBitmap(matrix, Colors.Black, Colors.White);

        }

        /// <summary>
        /// Renders a <see cref="null"/> as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="foreColor">Color of the fore.</param>
        /// <param name="backColor">Color of the back.</param>
        /// <returns></returns>
        public static WriteableBitmap ToBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            return RenderBitmap(matrix, foreColor, backColor);
        }


        /// <summary>
        /// Sets the color of the pixel using an extra alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(WriteableBitmap bmp, int x, int y, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplication
            var a = color.A + 1;
            bmp.Pixels[y * bmp.PixelWidth + x] = (color.A << 24)
                                                | ((byte)((color.R * a) >> 8) << 16)
                                                | ((byte)((color.G * a) >> 8) << 8)
                                                | ((byte)((color.B * a) >> 8));
        }

#else


#if NETFX_CORE

        public static WriteableBitmap RenderBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            var foreground = new byte[] { foreColor.B, foreColor.G, foreColor.R, foreColor.A };
            var background = new byte[] { backColor.B, backColor.G, backColor.R, backColor.A };
            int width = matrix.Width;
            int height = matrix.Height;
            var bmp = new WriteableBitmap(width, height);
            var length = width * height;
            int emptyArea = 0;
            // Copy data back
            using (Stream stream = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.AsStream(bmp.PixelBuffer))
            {
                for (int y = 0; y < height - emptyArea; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var color = matrix.Get(x, y) ? foreground : background;
                        stream.Write(color, 0, 4);
                    }
                }
            }
            bmp.Invalidate();
            return bmp;
        }

#else
        /// <summary>
        /// Sets the color of the pixel using an extra alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(WriteableBitmap bmp, int x, int y, Color color)
        {
            // Add one to use mul and cheap bit shift for multiplication
            var a = color.A + 1;
            WriteableBitmapExtensions.SetPixeli(bmp, y * bmp.PixelWidth + x, color);
        }

        private static WriteableBitmap RenderBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            WriteableBitmap image = BitmapFactory.New(width, height);
            using (image.GetBitmapContext())    // Refer to http://writeablebitmapex.codeplex.com/discussions/360149
            {

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color c = (matrix.Get(x, y)) ? foreColor : backColor;
                        SetPixel(image, x, y, c);

                    }
                }
            }
            return image;
        }
#endif

        /// <summary>
        /// Renders a <see cref="BitMatrix"/> as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        public static WriteableBitmap ToBitmap(BitMatrix matrix)
        {
            return RenderBitmap(matrix, Colors.Black, Colors.White);
        }

        /// <summary>
        /// Renders a <see cref="BitMatrix"/> as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="foreColor">Color of the fore.</param>
        /// <param name="backColor">Color of the back.</param>
        /// <returns></returns>
        public static WriteableBitmap ToBitmap(BitMatrix matrix, Color foreColor, Color backColor)
        {
            return RenderBitmap(matrix, foreColor, backColor);
        }

#endif


#endif


    }
}
