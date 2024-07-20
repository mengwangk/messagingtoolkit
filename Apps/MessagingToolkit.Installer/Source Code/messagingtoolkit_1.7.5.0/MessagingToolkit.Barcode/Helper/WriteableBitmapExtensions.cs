
using System;
using System.Linq;

#if NETFX_CORE
using Windows.Foundation;
#endif

#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

#if WPF

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

#endif

#endif

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.OneD;

namespace MessagingToolkit.Barcode.Helper
{
    /// <summary>
    /// Collection of extension methods for the WriteableBitmap class.
    /// </summary>
    public
#if WPF 
    unsafe 
#endif
 static partial class WriteableBitmapExtensions
    {
        #region Fields

        internal const int SizeOfArgb = 4;

        #endregion

        #region Methods

        #region General

        private static int ConvertColor(Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            return col;
        }

        /// <summary>
        /// Fills the whole WriteableBitmap with a color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="color">The color used for filling.</param>
        public static void Clear(this WriteableBitmap bmp, Color color)
        {
            var col = ConvertColor(color);
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                var w = context.Width;
                var h = context.Height;
                var len = w * SizeOfArgb;

                // Fill first line
                for (var x = 0; x < w; x++)
                {
                    pixels[x] = col;
                }

                // Copy first line
                var blockHeight = 1;
                var y = 1;
                while (y < h)
                {
                    BitmapContext.BlockCopy(context, 0, context, y * len, blockHeight * len);
                    y += blockHeight;
                    blockHeight = Math.Min(2 * blockHeight, h - y);
                }
            }
        }

        /// <summary>
        /// Fills the whole WriteableBitmap with an empty color (0).
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        public static void Clear(this WriteableBitmap bmp)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Clear();
            }
        }

        /// <summary>
        /// Clones the specified WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <returns>A copy of the WriteableBitmap.</returns>
        public static WriteableBitmap Clone(this WriteableBitmap bmp)
        {
            using (var srcContext = bmp.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                var result = BitmapFactory.New(srcContext.Width, srcContext.Height);
                using (var destContext = result.GetBitmapContext())
                {
                    BitmapContext.BlockCopy(srcContext, 0, destContext, 0, srcContext.Length * SizeOfArgb);
                }
                return result;
            }
        }

        #endregion

        #region ForEach

        /// <summary>
        /// Applies the given function to all the pixels of the bitmap in 
        /// order to set their color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="func">The function to apply. With parameters x, y and a color as a result</param>
        public static void ForEach(this WriteableBitmap bmp, Func<int, int, Color> func)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                int w = context.Width;
                int h = context.Height;
                int index = 0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var color = func(x, y);
                        pixels[index++] = ConvertColor(color);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the given function to all the pixels of the bitmap in 
        /// order to set their color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="func">The function to apply. With parameters x, y, source color and a color as a result</param>
        public static void ForEach(this WriteableBitmap bmp, Func<int, int, Color, Color> func)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                var w = context.Width;
                var h = context.Height;
                var index = 0;

                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var c = pixels[index];

                        // Premultiplied Alpha!
                        var a = (byte)(c >> 24);
                        // Prevent division by zero
                        int ai = a;
                        if (ai == 0)
                        {
                            ai = 1;
                        }
                        // Scale inverse alpha to use cheap integer mul bit shift
                        ai = ((255 << 8) / ai);
                        var srcColor = Color.FromArgb(a,
                                                      (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                                                      (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                                                      (byte)((((c & 0xFF) * ai) >> 8)));

                        var color = func(x, y, srcColor);
                        pixels[index++] = ConvertColor(color);
                    }
                }
            }
        }

        #endregion

        #region Get Pixel / Brightness

        /// <summary>
        /// Gets the color of the pixel at the x, y coordinate as integer.  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at x, y.</returns>
        public static int GetPixeli(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext())
            {
                return context.Pixels[y * context.Width + x];
            }
        }

        /// <summary>
        /// Gets the color of the pixel at the x, y coordinate as a Color struct.  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at x, y as a Color struct.</returns>
        public static Color GetPixel(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var c = context.Pixels[y * context.Width + x];
                var a = (byte)(c >> 24);

                // Prevent division by zero
                int ai = a;
                if (ai == 0)
                {
                    ai = 1;
                }

                // Scale inverse alpha to use cheap integer mul bit shift
                ai = ((255 << 8) / ai);
                return Color.FromArgb(a,
                                     (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                                     (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                                     (byte)((((c & 0xFF) * ai) >> 8)));
            }
        }

        /// <summary>
        /// Gets the brightness / luminance of the pixel at the x, y coordinate as byte.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The brightness of the pixel at x, y.</returns>
        public static byte GetBrightness(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                // Extract color components
                var c = context.Pixels[y * context.Width + x];
                var r = (byte)(c >> 16);
                var g = (byte)(c >> 8);
                var b = (byte)(c);

                // Convert to gray with constant factors 0.2126, 0.7152, 0.0722
                return (byte)((r * 6966 + g * 23436 + b * 2366) >> 15);
            }
        }

        #endregion

        #region SetPixel

        #region Without alpha

        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }

        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }

        #endregion

        #region With alpha

        /// <summary>
        /// Sets the color of the pixel including the alpha value and using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte a, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }

        /// <summary>
        /// Sets the color of the pixel including the alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte a, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }

        #endregion

        #region With System.Windows.Media.Color

        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = ConvertColor(color);
            }
        }

        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = ConvertColor(color);
            }
        }

        /// <summary>
        /// Sets the color of the pixel using an extra alpha value and a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte a, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Add one to use mul and cheap bit shift for multiplicaltion
                var ai = a + 1;
                context.Pixels[index] = (a << 24)
                           | ((byte)((color.R * ai) >> 8) << 16)
                           | ((byte)((color.G * ai) >> 8) << 8)
                           | ((byte)((color.B * ai) >> 8));
            }
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
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte a, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Add one to use mul and cheap bit shift for multiplicaltion
                var ai = a + 1;
                context.Pixels[y * context.Width + x] = (a << 24)
                                             | ((byte)((color.R * ai) >> 8) << 16)
                                             | ((byte)((color.G * ai) >> 8) << 8)
                                             | ((byte)((color.B * ai) >> 8));
            }
        }

        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster).  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, int color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = color;
            }
        }

        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, int color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = color;
            }
        }


        /// <summary>
        /// Copies (blits) the pixels from the WriteableBitmap source to the destination WriteableBitmap (this).
        /// </summary>
        /// <param name="bmp">The destination WriteableBitmap.</param>
        /// <param name="destRect">The rectangle that defines the destination region.</param>
        /// <param name="source">The source WriteableBitmap.</param>
        /// <param name="sourceRect">The rectangle that will be copied from the source to the destination.</param>
        /// <param name="BlendMode">The blending mode <see cref="BlendMode"/>.</param>
        public static void Blit(this WriteableBitmap bmp, Rect destRect, WriteableBitmap source, Rect sourceRect, BlendMode BlendMode)
        {
            Blit(bmp, destRect, source, sourceRect, Colors.White, BlendMode);
        }

        /// <summary>
        /// Copies (blits) the pixels from the WriteableBitmap source to the destination WriteableBitmap (this).
        /// </summary>
        /// <param name="bmp">The destination WriteableBitmap.</param>
        /// <param name="destRect">The rectangle that defines the destination region.</param>
        /// <param name="source">The source WriteableBitmap.</param>
        /// <param name="sourceRect">The rectangle that will be copied from the source to the destination.</param>
        public static void Blit(this WriteableBitmap bmp, Rect destRect, WriteableBitmap source, Rect sourceRect)
        {
            Blit(bmp, destRect, source, sourceRect, Colors.White, BlendMode.Alpha);
        }

        /// <summary>
        /// Copies (blits) the pixels from the WriteableBitmap source to the destination WriteableBitmap (this).
        /// </summary>
        /// <param name="bmp">The destination WriteableBitmap.</param>
        /// <param name="destPosition">The destination position in the destination bitmap.</param>
        /// <param name="source">The source WriteableBitmap.</param>
        /// <param name="sourceRect">The rectangle that will be copied from the source to the destination.</param>
        /// <param name="color">If not Colors.White, will tint the source image. A partially transparent color and the image will be drawn partially transparent.</param>
        /// <param name="BlendMode">The blending mode <see cref="BlendMode"/>.</param>
        public static void Blit(this WriteableBitmap bmp, Point destPosition, WriteableBitmap source, Rect sourceRect, Color color, BlendMode BlendMode)
        {
            Rect destRect = new Rect(destPosition, new Size(sourceRect.Width, sourceRect.Height));
            Blit(bmp, destRect, source, sourceRect, color, BlendMode);
        }

        /// <summary>
        /// Copies (blits) the pixels from the WriteableBitmap source to the destination WriteableBitmap (this).
        /// </summary>
        /// <param name="bmp">The destination WriteableBitmap.</param>
        /// <param name="destRect">The rectangle that defines the destination region.</param>
        /// <param name="source">The source WriteableBitmap.</param>
        /// <param name="sourceRect">The rectangle that will be copied from the source to the destination.</param>
        /// <param name="color">If not Colors.White, will tint the source image. A partially transparent color and the image will be drawn partially transparent. If the BlendMode is ColorKeying, this color will be used as color key to mask all pixels with this value out.</param>
        /// <param name="BlendMode">The blending mode <see cref="BlendMode"/>.</param>
#if WINMD
      [Windows.Foundation.Metadata.DefaultOverload]
#endif
        public static void Blit(this WriteableBitmap bmp, Rect destRect, WriteableBitmap source, Rect sourceRect, Color color, BlendMode BlendMode)
        {
            if (color.A == 0)
            {
                return;
            }
            int dw = (int)destRect.Width;
            int dh = (int)destRect.Height;

            using (var srcContext = source.GetBitmapContext())
            {
                using (var destContext = bmp.GetBitmapContext())
                {
                    int sourceWidth = srcContext.Width;
                    int dpw = destContext.Width;
                    int dph = destContext.Height;
                    Rect intersect = new Rect(0, 0, dpw, dph);
                    intersect.Intersect(destRect);
                    if (intersect.IsEmpty)
                    {
                        return;
                    }

                    var sourcePixels = srcContext.Pixels;
                    var destPixels = destContext.Pixels;
                    int sourceLength = srcContext.Length;
                    int destLength = destContext.Length;
                    int sourceIdx = -1;
                    int px = (int)destRect.X;
                    int py = (int)destRect.Y;
                    int right = px + dw;
                    int bottom = py + dh;
                    int x;
                    int y;
                    int idx;
                    double ii;
                    double jj;
                    int sr = 0;
                    int sg = 0;
                    int sb = 0;
                    int dr, dg, db;
                    int sourcePixel;
                    int sa = 0;
                    int da;
                    int ca = color.A;
                    int cr = color.R;
                    int cg = color.G;
                    int cb = color.B;
                    bool tinted = color != Colors.White;
                    var sw = (int)sourceRect.Width;
                    var sdx = sourceRect.Width / destRect.Width;
                    var sdy = sourceRect.Height / destRect.Height;
                    int sourceStartX = (int)sourceRect.X;
                    int sourceStartY = (int)sourceRect.Y;
                    int lastii, lastjj;
                    lastii = -1;
                    lastjj = -1;
                    jj = sourceStartY;
                    y = py;
                    for (int j = 0; j < dh; j++)
                    {
                        if (y >= 0 && y < dph)
                        {
                            ii = sourceStartX;
                            idx = px + y * dpw;
                            x = px;
                            sourcePixel = sourcePixels[0];

                            // Scanline BlockCopy is much faster (3.5x) if no tinting and blending is needed,
                            // even for smaller sprites like the 32x32 particles. 
                            if (BlendMode == BlendMode.None && !tinted)
                            {
                                sourceIdx = (int)ii + (int)jj * sourceWidth;
                                var offset = x < 0 ? -x : 0;
                                var xx = x + offset;
                                var wx = sourceWidth - offset;
                                var len = xx + wx < dpw ? wx : dpw - xx;
                                if (len > sw) len = sw;
                                if (len > dw) len = dw;
                                BitmapContext.BlockCopy(srcContext, (sourceIdx + offset) * 4, destContext, (idx + offset) * 4, len * 4);
                            }

                            // Pixel by pixel copying
                            else
                            {
                                for (int i = 0; i < dw; i++)
                                {
                                    if (x >= 0 && x < dpw)
                                    {
                                        if ((int)ii != lastii || (int)jj != lastjj)
                                        {
                                            sourceIdx = (int)ii + (int)jj * sourceWidth;
                                            if (sourceIdx >= 0 && sourceIdx < sourceLength)
                                            {
                                                sourcePixel = sourcePixels[sourceIdx];
                                                sa = ((sourcePixel >> 24) & 0xff);
                                                sr = ((sourcePixel >> 16) & 0xff);
                                                sg = ((sourcePixel >> 8) & 0xff);
                                                sb = ((sourcePixel) & 0xff);
                                                if (tinted && sa != 0)
                                                {
                                                    sa = (((sa * ca) * 0x8081) >> 23);
                                                    sr = ((((((sr * cr) * 0x8081) >> 23) * ca) * 0x8081) >> 23);
                                                    sg = ((((((sg * cg) * 0x8081) >> 23) * ca) * 0x8081) >> 23);
                                                    sb = ((((((sb * cb) * 0x8081) >> 23) * ca) * 0x8081) >> 23);
                                                    sourcePixel = (sa << 24) | (sr << 16) | (sg << 8) | sb;
                                                }
                                            }
                                            else
                                            {
                                                sa = 0;
                                            }
                                        }
                                        if (BlendMode == BlendMode.None)
                                        {
                                            destPixels[idx] = sourcePixel;
                                        }
                                        else if (BlendMode == BlendMode.ColorKeying)
                                        {
                                            sr = ((sourcePixel >> 16) & 0xff);
                                            sg = ((sourcePixel >> 8) & 0xff);
                                            sb = ((sourcePixel) & 0xff);

                                            if (sr != color.R || sg != color.G || sb != color.B)
                                            {
                                                destPixels[idx] = sourcePixel;
                                            }

                                        }
                                        else if (BlendMode == BlendMode.Mask)
                                        {
                                            int destPixel = destPixels[idx];
                                            da = ((destPixel >> 24) & 0xff);
                                            dr = ((destPixel >> 16) & 0xff);
                                            dg = ((destPixel >> 8) & 0xff);
                                            db = ((destPixel) & 0xff);
                                            destPixel = ((((da * sa) * 0x8081) >> 23) << 24) |
                                                        ((((dr * sa) * 0x8081) >> 23) << 16) |
                                                        ((((dg * sa) * 0x8081) >> 23) << 8) |
                                                        ((((db * sa) * 0x8081) >> 23));
                                            destPixels[idx] = destPixel;
                                        }
                                        else if (sa > 0)
                                        {
                                            int destPixel = destPixels[idx];
                                            da = ((destPixel >> 24) & 0xff);
                                            if ((sa == 255 || da == 0) &&
                                               BlendMode != BlendMode.Additive
                                               && BlendMode != BlendMode.Subtractive
                                               && BlendMode != BlendMode.Multiply
                                               )
                                            {
                                                destPixels[idx] = sourcePixel;
                                            }
                                            else
                                            {
                                                dr = ((destPixel >> 16) & 0xff);
                                                dg = ((destPixel >> 8) & 0xff);
                                                db = ((destPixel) & 0xff);
                                                if (BlendMode == BlendMode.Alpha)
                                                {
                                                    destPixel = ((sa + (((da * (255 - sa)) * 0x8081) >> 23)) << 24) |
                                                       ((sr + (((dr * (255 - sa)) * 0x8081) >> 23)) << 16) |
                                                       ((sg + (((dg * (255 - sa)) * 0x8081) >> 23)) << 8) |
                                                       ((sb + (((db * (255 - sa)) * 0x8081) >> 23)));
                                                }
                                                else if (BlendMode == BlendMode.Additive)
                                                {
                                                    int a = (255 <= sa + da) ? 255 : (sa + da);
                                                    destPixel = (a << 24) |
                                                       (((a <= sr + dr) ? a : (sr + dr)) << 16) |
                                                       (((a <= sg + dg) ? a : (sg + dg)) << 8) |
                                                       (((a <= sb + db) ? a : (sb + db)));
                                                }
                                                else if (BlendMode == BlendMode.Subtractive)
                                                {
                                                    int a = da;
                                                    destPixel = (a << 24) |
                                                       (((sr >= dr) ? 0 : (sr - dr)) << 16) |
                                                       (((sg >= dg) ? 0 : (sg - dg)) << 8) |
                                                       (((sb >= db) ? 0 : (sb - db)));
                                                }
                                                else if (BlendMode == BlendMode.Multiply)
                                                {
                                                    // Faster than a division like (s * d) / 255 are 2 shifts and 2 adds
                                                    int ta = (sa * da) + 128;
                                                    int tr = (sr * dr) + 128;
                                                    int tg = (sg * dg) + 128;
                                                    int tb = (sb * db) + 128;

                                                    int ba = ((ta >> 8) + ta) >> 8;
                                                    int br = ((tr >> 8) + tr) >> 8;
                                                    int bg = ((tg >> 8) + tg) >> 8;
                                                    int bb = ((tb >> 8) + tb) >> 8;

                                                    destPixel = (ba << 24) |
                                                                ((ba <= br ? ba : br) << 16) |
                                                                ((ba <= bg ? ba : bg) << 8) |
                                                                ((ba <= bb ? ba : bb));
                                                }

                                                destPixels[idx] = destPixel;
                                            }
                                        }
                                    }
                                    x++;
                                    idx++;
                                    ii += sdx;
                                }
                            }
                        }
                        jj += sdy;
                        y++;
                    }
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}