using System;

#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.OneD;

namespace MessagingToolkit.Barcode.Helper
{

    public sealed class MatrixToImageHelper
    {

        private const int Black = -16777216;
        private const int White = -1;

        private MatrixToImageHelper()
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Renders a bit matrix as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        public static Image ToBitmap(BitMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap image = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, y, (matrix.Get(x, y)) ? Color.FromArgb(Black) : Color.FromArgb(White));
                }
            }
            return image;
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

#else

        /// <summary>
        /// Renders a <see cref="null"/> as an image, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        public static WriteableBitmap ToBitmap(BitMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            WriteableBitmap image = new WriteableBitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = (matrix.Get(x, y)) ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
                    SetPixel(image, x, y, c);

                }
            }
            return image;

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
            int width = matrix.Width;
            int height = matrix.Height;
            WriteableBitmap image = new WriteableBitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = (matrix.Get(x, y)) ? foreColor : backColor;
                    SetPixel(image, x, y, c);

                }
            }
            return image;
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
            // Add one to use mul and cheap bit shift for multiplicaltion
            var a = color.A + 1;
            bmp.Pixels[y * bmp.PixelWidth + x] = (color.A << 24)
                                                | ((byte)((color.R * a) >> 8) << 16)
                                                | ((byte)((color.G * a) >> 8) << 8)
                                                | ((byte)((color.B * a) >> 8));
        }



#endif

        /// <summary>
        /// Renders a bit matrix as SVG 
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        public static string ToSVG(BitMatrix matrix)
        {
            return string.Empty;
        }

        // @Todo - renders as EPS and PDF

    }
}
