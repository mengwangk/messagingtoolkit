using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{
    internal sealed class PlanarYUVLuminanceSource : BaseLuminanceSource
    {
        private static readonly int THUMBNAIL_SCALE_FACTOR = 2;

        private readonly byte[] yuvData;
        private readonly int dataWidth;
        private readonly int dataHeight;
        private readonly int left;
        private readonly int top;

        /// <summary>
        /// This object extends LuminanceSource around an array of YUV data returned from the camera driver,
        /// with the option to crop to a rectangle within the full data. This can be used to exclude
        /// superfluous pixels around the perimeter and speed up decoding.
        /// It works for any pixel format where the Y channel is planar and appears first, including
        /// YCbCr_420_SP and YCbCr_422_SP.
        /// </summary>
        public PlanarYUVLuminanceSource(byte[] yuvData,
                                        int dataWidth,
                                        int dataHeight,
                                        int left,
                                        int top,
                                        int width,
                                        int height,
                                        bool reverseHoriz)
            : base(width, height)
        {
            if (left + width > dataWidth || top + height > dataHeight)
            {
                throw new ArgumentException("Crop rectangle does not fit within image data.");
            }

            this.yuvData = yuvData;
            this.dataWidth = dataWidth;
            this.dataHeight = dataHeight;
            this.left = left;
            this.top = top;
            if (reverseHoriz)
            {
                ReverseHorizontal(width, height);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanarYUVLuminanceSource"/> class.
        /// </summary>
        /// <param name="luminances">The luminances.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private PlanarYUVLuminanceSource(byte[] luminances, int width, int height)
            : base(width, height)
        {
            yuvData = luminances;
            this.luminances = luminances;
            dataWidth = width;
            dataHeight = height;
            left = 0;
            top = 0;
        }

        override public byte[] GetRow(int y, byte[] row)
        {
            if (y < 0 || y >= Height)
            {
                throw new ArgumentException("Requested row is outside the image: " + y);
            }
            int width = Width;
            if (row == null || row.Length < width)
            {
                row = new byte[width];
            }
            int offset = (y + top) * dataWidth + left;
            Array.Copy(yuvData, offset, row, 0, width);
            return row;
        }

        override public byte[] Matrix
        {
            get
            {
                int width = Width;
                int height = Height;

                // If the caller asks for the entire underlying image, save the copy and give them the
                // original data. The docs specifically warn that result.length must be ignored.
                if (width == dataWidth && height == dataHeight)
                {
                    return yuvData;
                }

                int area = width * height;
                byte[] matrix = new byte[area];
                int inputOffset = top * dataWidth + left;

                // If the width matches the full width of the underlying data, perform a single copy.
                if (width == dataWidth)
                {
                    Array.Copy(yuvData, inputOffset, matrix, 0, area);
                    return matrix;
                }

                // Otherwise copy one cropped row at a time.
                byte[] yuv = yuvData;
                for (int y = 0; y < height; y++)
                {
                    int outputOffset = y * width;
                    Array.Copy(yuv, inputOffset, matrix, outputOffset, width);
                    inputOffset += dataWidth;
                }
                return matrix;
            }
        }

        override public bool CropSupported
        {
            get { return true; }
        }

        override public LuminanceSource Crop(int left, int top, int width, int height)
        {
            return new PlanarYUVLuminanceSource(yuvData,
                                                dataWidth,
                                                dataHeight,
                                                this.left + left,
                                                this.top + top,
                                                width,
                                                height,
                                                false);
        }

        public int[] RenderThumbnail()
        {
            int width = this.Width / THUMBNAIL_SCALE_FACTOR;
            int height = this.Height / THUMBNAIL_SCALE_FACTOR;
            int[] pixels = new int[width * height];
            byte[] yuv = yuvData;
            int inputOffset = top * dataWidth + left;

            for (int y = 0; y < height; y++)
            {
                int outputOffset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int grey = yuv[inputOffset + x * THUMBNAIL_SCALE_FACTOR] & 0xff;
                    pixels[outputOffset + x] = ((0x00FF0000 << 8) | (grey * 0x00010101));
                }
                inputOffset += dataWidth * THUMBNAIL_SCALE_FACTOR;
            }
            return pixels;
        }

        /// <summary>
        ///  Width of image from <seealso cref="RenderThumbnail"/>
        /// </summary>
        public int ThumbnailWidth
        {
            get
            {
                return this.Width / THUMBNAIL_SCALE_FACTOR;
            }
        }

        /// <summary>
        ///  Height of image from <seealso cref="RenderThumbnail"/>
        /// </summary>
        public int ThumbnailHeight
        {
            get
            {
                return this.Height / THUMBNAIL_SCALE_FACTOR;
            }
        }

        private void ReverseHorizontal(int width, int height)
        {
            byte[] yuvData = this.yuvData;
            for (int y = 0, rowStart = top * dataWidth + left; y < height; y++, rowStart += dataWidth)
            {
                int middle = rowStart + width / 2;
                for (int x1 = rowStart, x2 = rowStart + width - 1; x1 < middle; x1++, x2--)
                {
                    byte temp = yuvData[x1];
                    yuvData[x1] = yuvData[x2];
                    yuvData[x2] = temp;
                }
            }
        }

        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
        {
            return new PlanarYUVLuminanceSource(newLuminances, width, height);
        }
    }
}
