using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Helper;

#if !SILVERLIGHT
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

using System;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// RGB luminance source
    /// 
    /// Modified: April 23 2012
    /// </summary>
    public class RGBLuminanceSource : LuminanceSource
    {
        private byte[] luminances;
        private bool isRotated = false;
        private bool isRegionSelect = false;

#if SILVERLIGHT
        private System.Windows.Rect region;
#else
        private Rectangle region;
#endif


#if SILVERLIGHT
        private WriteableBitmap bmp;
#else
     private Bitmap bmp;
#endif


        override public int Height
        {
            get
            {
                if (!isRotated)
                    return height;
                else
                    return width;
            }

        }
        override public int Width
        {
            get
            {
                if (!isRotated)
                    return width;
                else
                    return height;
            }
        }

        public RGBLuminanceSource(byte[] d, int w, int h)
            : base(w, h)
        {
            this.width = w;
            this.height = h;
            int width = w;
            int height = h;
            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
            luminances = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int r = d[offset * 3 + x * 3];
                    int g = d[offset * 3 + x * 3 + 1];
                    int b = d[offset * 3 + x * 3 + 2];
                    if (r == g && g == b)
                    {
                        // Image is already greyscale, so pick any channel.
                        luminances[offset + x] = (byte)r;
                    }
                    else
                    {
                        // Calculate luminance cheaply, favoring green.
                        luminances[offset + x] = (byte)((r + g + g + b) >> 2);
                    }
                }
            }
        }
        public RGBLuminanceSource(byte[] d, int w, int H, bool is8Bit)
            : base(w, H)
        {
            width = w;
            height = H;
            luminances = new byte[w * H];
            Buffer.BlockCopy(d, 0, luminances, 0, w * H);
        }

        public RGBLuminanceSource(byte[] d, int W, int H, bool is8Bit,
#if SILVERLIGHT
 System.Windows.Rect region
#else
        Rectangle region
#endif
)
            : base(W, H)
        {
#if SILVERLIGHT
            this.width = (int)region.Width;
            this.height = (int)region.Height;
#else
            this.width = region.Width;
            this.height = region.Height;
#endif
            this.region = region;
            isRegionSelect = true;
            //luminances = Red.Imaging.Filters.CropArea(d, W, H, Region);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RGBLuminanceSource"/> class.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        public RGBLuminanceSource(
#if SILVERLIGHT
WriteableBitmap d,
#else
        Bitmap d, 
#endif
 int w, int h)
            : base(w, h)
        {

#if SILVERLIGHT

            var height = d.PixelHeight;
            var width = d.PixelWidth;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            luminances = new byte[width * height];
            System.Windows.Media.Color c;
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int srcPixel = d.Pixels[x + offset];
                    c = System.Windows.Media.Color.FromArgb((byte)((srcPixel >> 0x18) & 0xff),
                          (byte)((srcPixel >> 0x10) & 0xff),
                          (byte)((srcPixel >> 8) & 0xff),
                          (byte)(srcPixel & 0xff));
                    luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
                }
            }
#else
            int width = this.width = w;
            int height = this.height = h;
            this.bmp = d;

            // In order to measure pure decoding speed, we convert the entire image to a greyscale array
            // up front, which is the same as the Y channel of the YUVLuminanceSource in the real app.
            luminances = new byte[width * height];

            // The underlying raster of image consists of bytes with the luminance values
            var data = d.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, d.PixelFormat);
            try
            {
                var stride = Math.Abs(data.Stride);
                var pixelWidth = stride / width;

                if (pixelWidth == 2 || pixelWidth > 4)
                {
                    // old slow way for unsupported bit depth
                    Color c;
                    for (int y = 0; y < height; y++)
                    {
                        int offset = y * width;
                        for (int x = 0; x < width; x++)
                        {
                            c = d.GetPixel(x, y);
                            luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
                        }
                    }
                }
                else
                {
                    var strideStep = data.Stride;
                    var buffer = new byte[stride];
                    var ptrInBitmap = data.Scan0;

                    // prepare palette for 1 and 8 bit indexed bitmaps
                    var luminancePalette = new byte[d.Palette.Entries.Length];
                    for (var index = 0; index < d.Palette.Entries.Length; index++)
                    {
                        var color = d.Palette.Entries[index];
                        luminancePalette[index] = (byte)(0.3 * color.R +
                                                          0.59 * color.G +
                                                          0.11 * color.B + 0.01);
                    }

                    for (int y = 0; y < height; y++)
                    {
                        // copy a scanline not the whole bitmap because of memory usage
                        Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40
                        ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                        ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                        var offset = y * width;
                        switch (pixelWidth)
                        {
                            case 0:
                                for (int x = 0; x * 8 < width; x++)
                                {
                                    for (int subX = 0; subX < 8 && 8 * x + subX < width; subX++)
                                    {
                                        var index = (buffer[x] >> (7 - subX)) & 1;
                                        luminances[offset + 8 * x + subX] = luminancePalette[index];
                                    }
                                }
                                break;
                            case 1:
                                for (int x = 0; x < width; x++)
                                {
                                    luminances[offset + x] = luminancePalette[buffer[x]];
                                }
                                break;
                            case 3:
                            case 4:
                                for (int x = 0; x < width; x++)
                                {
                                    var luminance = (byte)(0.3 * buffer[x * pixelWidth] +
                                                            0.59 * buffer[x * pixelWidth + 1] +
                                                            0.11 * buffer[x * pixelWidth + 2] + 0.01);
                                    luminances[offset + x] = luminance;
                                }
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                }
            }
            finally
            {
                d.UnlockBits(data);
            }
#endif

        }


        override public byte[] GetRow(int y, byte[] row)
        {
            if (isRotated == false)
            {
                int width = Width;
                if (row == null || row.Length < width)
                {
                    row = new byte[width];
                }
                for (int i = 0; i < width; i++)
                    row[i] = luminances[y * width + i];
                return row;
            }
            else
            {
                int width = this.width;
                int height = this.height;
                if (row == null || row.Length < height)
                {
                    row = new byte[height];
                }
                for (int i = 0; i < height; i++)
                    row[i] = luminances[i * width + y];
                return row;
            }
        }
        public override byte[] Matrix
        {
            get { return luminances; }
        }

        public override LuminanceSource Crop(int left, int top, int width, int height)
        {
#if SILVERLIGHT
            const int SizeOfArgb = 4;

            if (this.bmp != null)
            {
                var srcWidth = bmp.PixelWidth;
                var srcHeight = bmp.PixelHeight;

                // If the rectangle is completly out of the bitmap
                if (left > srcWidth || top > srcHeight)
                {
                    return base.Crop(left, top, width, height);
                }

                // Clamp to boundaries
                if (left < 0) left = 0;
                if (left + width > srcWidth) width = srcWidth - left;
                if (top < 0) top = 0;
                if (top + height > srcHeight) height = srcHeight - top;

                // Copy the pixels line by line using fast BlockCopy
                var result = new WriteableBitmap(width, height);
                for (var line = 0; line < height; line++)
                {
                    var srcOff = ((top + line) * srcWidth + left) * SizeOfArgb;
                    var dstOff = line * width * SizeOfArgb;
                    Buffer.BlockCopy(bmp.Pixels, srcOff, result.Pixels, dstOff, width * SizeOfArgb);
                }
                return new RGBLuminanceSource(result, result.PixelWidth, result.PixelHeight);
            }
            else
            {
                return base.Crop(left, top, width, height);
            }
#else
            if (this.bmp != null)
            {
                // create the destination (cropped) bitmap
                Bitmap bmpCropped = new Bitmap(width, height);

                // create the graphics object to draw with
                Graphics g = Graphics.FromImage(bmpCropped);

                Rectangle rectDestination = new Rectangle(0, 0, bmpCropped.Width, bmpCropped.Height);
                Rectangle rectCropArea = new Rectangle(left, top, width, height);

                // draw the rectCropArea of the original image to the rectDestination of bmpCropped
                g.DrawImage(bmp, rectDestination, rectCropArea, GraphicsUnit.Pixel);

                // release system resources
                g.Dispose();

                return new RGBLuminanceSource(bmpCropped, bmpCropped.Width, bmpCropped.Height);
            }
            else
            {
                return base.Crop(left, top, width, height);
            }
#endif
        }

        /// <summary>
        /// Returns a new object with rotated image data. Only callable if RotateSupported is true.
        /// </summary>
        /// <returns>
        /// A rotated version of this object.
        /// </returns>
        public override LuminanceSource RotateCounterClockwise()
        {
            byte[] rotatedLuminances = new byte[width * height];
            int newWidth = height;
            int newHeight = width;
            for (int yold = 0; yold < height; yold++)
            {
                for (int xold = 0; xold < width; xold++)
                {
                    int ynew = xold;
                    int xnew = newWidth - yold - 1;
                    rotatedLuminances[ynew * newWidth + xnew] = luminances[yold * width + xold];
                }
            }
            luminances = rotatedLuminances;
            height = newHeight;
            width = newWidth;
            isRotated = true;
            return this;
        }

        public override bool RotateSupported
        {
            get
            {
                return true;
            }

        }
    }
}
