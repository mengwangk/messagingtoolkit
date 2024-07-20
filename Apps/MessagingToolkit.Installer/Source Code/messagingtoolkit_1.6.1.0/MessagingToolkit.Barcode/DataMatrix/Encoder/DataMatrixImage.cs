using System;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixImage
    {
        #region Fields

        int _rowPadBytes;

        #endregion

        #region Constructor
        internal DataMatrixImage(byte[] pxl, int width, int height, DataMatrixPackOrder pack)
        {
            this.BitsPerChannel = new int[4];
            this.ChannelStart = new int[4];
            if (pxl == null || width < 1 || height < 1)
            {
                throw new ArgumentException("Cannot create image of size null");
            }

            this.Pxl = pxl;
            this.Width = width;
            this.Height = height;
            this.PixelPacking = pack;
            this.BitsPerPixel = DataMatrixCommon.GetBitsPerPixel(pack);
            this.BytesPerPixel = this.BitsPerPixel / 8;
            this._rowPadBytes = 0;
            this.RowSizeBytes = this.Width * this.BytesPerPixel + this._rowPadBytes;
            this.ImageFlip = DataMatrixFlip.FlipNone;

            /* Leave channelStart[] and bitsPerChannel[] with zeros from calloc */
            this.ChannelCount = 0;

            switch (pack)
            {
                case DataMatrixPackOrder.PackCustom:
                    break;
                case DataMatrixPackOrder.Pack1bppK:
                    throw new ArgumentException("Cannot create image: not supported pack order!");
                case DataMatrixPackOrder.Pack8bppK:
                    SetChannel(0, 8);
                    break;
                case DataMatrixPackOrder.Pack16bppRGB:
                case DataMatrixPackOrder.Pack16bppBGR:
                case DataMatrixPackOrder.Pack16bppYCbCr:
                    SetChannel(0, 5);
                    SetChannel(5, 5);
                    SetChannel(10, 5);
                    break;
                case DataMatrixPackOrder.Pack24bppRGB:
                case DataMatrixPackOrder.Pack24bppBGR:
                case DataMatrixPackOrder.Pack24bppYCbCr:
                case DataMatrixPackOrder.Pack32bppRGBX:
                case DataMatrixPackOrder.Pack32bppBGRX:
                    SetChannel(0, 8);
                    SetChannel(8, 8);
                    SetChannel(16, 8);
                    break;
                case DataMatrixPackOrder.Pack16bppRGBX:
                case DataMatrixPackOrder.Pack16bppBGRX:
                    SetChannel(0, 5);
                    SetChannel(5, 5);
                    SetChannel(10, 5);
                    break;
                case DataMatrixPackOrder.Pack16bppXRGB:
                case DataMatrixPackOrder.Pack16bppXBGR:
                    SetChannel(1, 5);
                    SetChannel(6, 5);
                    SetChannel(11, 5);
                    break;
                case DataMatrixPackOrder.Pack32bppXRGB:
                case DataMatrixPackOrder.Pack32bppXBGR:
                    SetChannel(8, 8);
                    SetChannel(16, 8);
                    SetChannel(24, 8);
                    break;
                case DataMatrixPackOrder.Pack32bppCMYK:
                    SetChannel(0, 8);
                    SetChannel(8, 8);
                    SetChannel(16, 8);
                    SetChannel(24, 8);
                    break;
                default:
                    throw new ArgumentException("Cannot create image: Invalid Pack Order");
            }
        }
        #endregion

        #region Methods
        internal bool SetChannel(int channelStart, int bitsPerChannel)
        {
            if (this.ChannelCount >= 4) /* IMAGE_MAX_CHANNEL */
                return false;

            /* New channel extends beyond pixel data */

            this.BitsPerChannel[this.ChannelCount] = bitsPerChannel;
            this.ChannelStart[this.ChannelCount] = channelStart;
            (this.ChannelCount)++;

            return true;
        }

        internal int GetByteOffset(int x, int y)
        {
            if (this.ImageFlip == DataMatrixFlip.FlipX)
            {
                throw new ArgumentException("FlipX is not an option!");
            }

            if (!ContainsInt(0, x, y))
                return DataMatrixConstants.DataMatrixUndefined;

            if (this.ImageFlip == DataMatrixFlip.FlipY)
                return (y * this.RowSizeBytes + x * this.BytesPerPixel);

            return ((this.Height - y - 1) * this.RowSizeBytes + x * this.BytesPerPixel);
        }

        internal bool GetPixelValue(int x, int y, int channel, ref int value)
        {
            if (channel >= this.ChannelCount)
            {
                throw new ArgumentException("Channel greater than channel count!");
            }

            int offset = GetByteOffset(x, y);
            if (offset == DataMatrixConstants.DataMatrixUndefined)
            {
                return false;
            }

            switch (this.BitsPerChannel[channel])
            {
                case 1:
                    break;
                case 5:
                    break;
                case 8:
                    if (this.ChannelStart[channel] % 8 != 0 || this.BitsPerPixel % 8 != 0)
                    {
                        throw new Exception("Error getting pixel value");
                    }
                    value = this.Pxl[offset + channel];
                    break;
            }

            return true;
        }

        internal bool SetPixelValue(int x, int y, int channel, byte value)
        {
            if (channel >= this.ChannelCount)
            {
                throw new ArgumentException("Channel greater than channel count!");
            }

            int offset = GetByteOffset(x, y);
            if (offset == DataMatrixConstants.DataMatrixUndefined)
            {
                return false;
            }

            switch (this.BitsPerChannel[channel])
            {
                case 1:
                    break;
                case 5:
                    break;
                case 8:
                    if (this.ChannelStart[channel] % 8 != 0 || this.BitsPerPixel % 8 != 0)
                    {
                        throw new Exception("Error getting pixel value");
                    }
                    this.Pxl[offset + channel] = value;
                    break;
            }

            return true;
        }

        internal bool ContainsInt(int margin, int x, int y)
        {
            if (x - margin >= 0 && x + margin < this.Width &&
                  y - margin >= 0 && y + margin < this.Height)
                return true;

            return false;
        }

        internal bool ContainsFloat(double x, double y)
        {
            if (x >= 0.0 && x < this.Width && y >= 0.0 && y < this.Height)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Properties

        internal int Width { get; set; }

        internal int Height { get; set; }

        internal DataMatrixPackOrder PixelPacking { get; set; }

        internal int BitsPerPixel { get; set; }

        internal int BytesPerPixel { get; set; }

        internal int RowPadBytes
        {
            get { return _rowPadBytes; }
            set
            {
                _rowPadBytes = value;
                this.RowSizeBytes = this.Width * (this.BitsPerPixel / 8) + this._rowPadBytes;
            }
        }

        internal int RowSizeBytes { get; set; }

        internal DataMatrixFlip ImageFlip { get; set; }

        internal int ChannelCount { get; set; }

        internal int[] ChannelStart { get; set; }

        internal int[] BitsPerChannel { get; set; }

        internal byte[] Pxl { get; set; }

        #endregion
    }
}
