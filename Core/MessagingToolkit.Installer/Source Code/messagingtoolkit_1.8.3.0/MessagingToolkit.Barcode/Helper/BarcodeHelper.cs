using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

#if !PORTABLE
using System.Windows;
#endif

#if !PORTABLE
#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;
using System.Drawing.Imaging;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

#else

using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

#endif

#endif

namespace MessagingToolkit.Barcode.Helper
{

    /// <summary>
    /// Contains conversion support elements such as classes, interfaces and static methods.
    /// </summary>
    internal sealed class BarcodeHelper
    {

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2 << ~bits);
        }

        public static object GetDecodeOptionType(Dictionary<DecodeOptions, object> dict, DecodeOptions key)
        {
            try
            {
                if (dict == null) return null;

                object ret = null;
                if (dict.TryGetValue(key, out ret))
                    return ret;
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static object GetEncodeOptionType(Dictionary<EncodeOptions, object> dict, EncodeOptions key)
        {
            try
            {
                if (dict == null) return null;

                object ret = null;
                if (dict.TryGetValue(key, out ret))
                    return ret;
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static float[][] CreateRectangularFloatArray(int size1, int size2)
        {
            float[][] array = new float[size1][];
            for (int array1 = 0; array1 < size1; array1++)
            {
                array[array1] = new float[size2];
            }
            return array;
        }

        /// <summary>
        /// Fills the specified array.
        /// (can't use extension method because of .Net 2.0 support)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The value.</param>
        public static void Fill<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static string ToBinaryString(int x)
        {
            char[] bits = new char[32];
            int i = 0;

            while (x != 0)
            {
                bits[i++] = (x & 1) == 1 ? '1' : '0';
                x >>= 1;
            }

            Array.Reverse(bits, 0, i);
            return new string(bits);
        }

        /// <summary>
        /// Joins the list of values using the passed in separator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="separator">The separator.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static string Join<T>(string separator, T[] values)
        {
            return string.Join(",", values.Select(x => x.ToString()).ToArray());
        }

#if !PORTABLE

#if !SILVERLIGHT && !NETFX_CORE

        public static byte[] ImageToByteArray(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Gif);
            return ms.ToArray();
        }
#else
#if !WPF && !NETFX_CORE
        public static byte[] ImageToByteArray(WriteableBitmap bmp)
        {
            // Init buffer
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] p = bmp.Pixels;
            int len = p.Length;
            byte[] result = new byte[4 * w * h];

            // Copy pixels to buffer
            for (int i = 0, j = 0; i < len; i++, j += 4)
            {
                int color = p[i];
                result[j + 0] = (byte)(color >> 24); // A
                result[j + 1] = (byte)(color >> 16); // R
                result[j + 2] = (byte)(color >> 8);  // G
                result[j + 3] = (byte)(color);       // B
            }

            return result;
        }
#else

#if !NETFX_CORE

        public unsafe static byte[] ImageToByteArray(WriteableBitmap bmp)
        {
            // Init buffer
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            BitmapContext bitmapContext = new BitmapContext(bmp);
            int* p = bitmapContext.Pixels;
            int len = bitmapContext.Length;
            byte[] result = new byte[4 * w * h];

            // Copy pixels to buffer
            for (int i = 0, j = 0; i < len; i++, j += 4)
            {
                int color = p[i];
                result[j + 0] = (byte)(color >> 24); // A
                result[j + 1] = (byte)(color >> 16); // R
                result[j + 2] = (byte)(color >> 8);  // G
                result[j + 3] = (byte)(color);       // B
            }

            return result;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        public static System.Drawing.Color ToWinFormsColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Drawing.Bitmap ToWinFormsBitmap(BitmapSource bitmapsource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(stream);

                using (var tempBitmap = new System.Drawing.Bitmap(stream))
                {
                    // According to MSDN, one "must keep the stream open for the lifetime of the Bitmap."
                    // So we return a copy of the new bitmap, allowing us to dispose both the bitmap and the stream.
                    return new System.Drawing.Bitmap(tempBitmap);
                }
            }
        }

#endif

#endif

#endif

#if !WPF
#if !SILVERLIGHT && !NETFX_CORE

        public static Image ByteArrayToImage(byte[] ba)
        {
            MemoryStream ms = new MemoryStream(ba);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
#else

#if !NETFX_CORE

        public static WriteableBitmap ByteArrayToImage(byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData, 0, imageData.Length))
            {
                BitmapImage im = new BitmapImage();
                im.SetSource(ms);
                return new WriteableBitmap(im);
            }
        }
#endif

#endif
#else
        public static WriteableBitmap ByteArrayToImage(byte[] imageData)
        {
            WriteableBitmap bitmap = BitmapFactory.New(250,250);
            return WriteableBitmapExtensions.FromByteArray(bitmap, imageData, imageData.Length);
        }
#endif

#endif
    }
}
