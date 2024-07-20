#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
#else

#if SILVERLIGHT

using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

#endif

namespace MessagingToolkit.Barcode
{
    public partial class BitmapLuminanceSource : BaseLuminanceSource
    {

#if SILVERLIGHT || NETFX_CORE
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected BitmapLuminanceSource(int width, int height)
            : base(width, height)
        {
        }

        public BitmapLuminanceSource(WriteableBitmap writeableBitmap)
            : base(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight)
        {
            var height = writeableBitmap.PixelHeight;
            var width = writeableBitmap.PixelWidth;
            var stride = width * 4;
            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            luminances = new byte[width * height];
            Color c;

#if NETFX_CORE
            var stream = writeableBitmap.PixelBuffer.AsStream();
            //if (stream.Position != 0 && stream.CanSeek) 
            //    stream.Seek(0, System.IO.SeekOrigin.Begin);
            var data = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(writeableBitmap.PixelBuffer, 0, (int)writeableBitmap.PixelBuffer.Length);
            for (int y = 0; y < height; y++)
            {
                int offset = y * stride;
                for (int x = 0, xl = 0; x < stride; x += 4, xl++)
                {
                    c = Color.FromArgb(
                       data[x + offset],
                       data[x + offset + 1],
                       data[x + offset + 2],
                       data[x + offset + 3]);
                    luminances[y * width + xl] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
                }
            }
        }
#else

            var pixels = writeableBitmap.Pixels;
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int srcPixel = pixels[x + offset];
                    c = Color.FromArgb((byte)((srcPixel >> 0x18) & 0xff),
                          (byte)((srcPixel >> 0x10) & 0xff),
                          (byte)((srcPixel >> 8) & 0xff),
                          (byte)(srcPixel & 0xff));
                    luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
                }
            }
        }
#endif
        /// <summary>
        /// Should create a new luminance source with the right class type.
        /// The method is used in methods crop and rotate.
        /// </summary>
        /// <param name="newLuminances">The new luminances.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
        {
            return new BitmapLuminanceSource(width, height) { luminances = newLuminances };
        }
#endif
    }
}