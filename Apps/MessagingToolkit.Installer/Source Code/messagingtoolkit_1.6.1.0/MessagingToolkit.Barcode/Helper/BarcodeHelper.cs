using System.Collections.Generic;
using System.Linq;
using System.IO;

#if !SILVERLIGHT

using System.Drawing;
using System.Drawing.Imaging;

#else

using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            object ret = null;
            if (dict.TryGetValue(key, out ret))
                return ret;
            return null;
        }

        public static object GetEncodeOptionType(Dictionary<EncodeOptions, object> dict, EncodeOptions key)
        {
            object ret = null;
            if (dict.TryGetValue(key, out ret))
                return ret;
            return null;
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

#if !SILVERLIGHT

        public static byte[] ImageToByteArray(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Gif);
            return ms.ToArray();
        }
#else
        public static byte[] ToByteArray(WriteableBitmap bmp)
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

#endif


#if !SILVERLIGHT

        public static Image ByteArrayToImage(byte[] ba)
        {
            MemoryStream ms = new MemoryStream(ba);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
#else
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
    }  
}
