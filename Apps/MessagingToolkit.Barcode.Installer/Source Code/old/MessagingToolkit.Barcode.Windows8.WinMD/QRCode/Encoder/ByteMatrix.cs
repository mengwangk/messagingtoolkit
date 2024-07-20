using System;
using System.Text;

namespace MessagingToolkit.Barcode.QRCode.Encoder
{
    /// <summary>
    /// The original code was a 2D array of ints, but since it only ever gets
    /// assigned -1, 0, and 1, I'm going to use less memory and go with bytes.
    /// </summary>
    internal sealed class ByteMatrix
    {
        private readonly byte[][] bytes;
        private readonly int width;
        private readonly int height;


        public ByteMatrix(int width, int height, byte[][] data)
        {
            this.width = width;
            this.height = height;
            this.bytes = data;
        }


        public ByteMatrix(int width, int height)
        {
            bytes = new byte[height][];
            for (int i = 0; i < height; i++)
            {
                bytes[i] = new byte[width];
            }
            this.width = width;
            this.height = height;
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public byte Get(int x, int y)
        {
            return bytes[y][x];
        }

        ///<summary>
        /// Return an internal representation as bytes, in row-major order. array[y][x] represents point (x,y)
        ///</summary>
        public byte[][] Array
        {
            get
            {
                return bytes;
            }
        }

        public void Set(int x, int y, byte val)
        {
            bytes[y][x] = val;
        }

        public void Set(int x, int y, int val)
        {
            bytes[y][x] = (byte)val;
        }

        public void Set(int x, int y, bool val)
        {
            bytes[y][x] = (byte)((val) ? 1 : 0);
        }

        public void Clear(byte val)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    bytes[y][x] = val;
                }
            }
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder(2 * width * height + 2);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    switch (bytes[y][x])
                    {
                        case 0:
                            result.Append(" 0");
                            break;
                        case 1:
                            result.Append(" 1");
                            break;
                        default:
                            result.Append("  ");
                            break;
                    }
                }
                result.Append('\n');
            }
            return result.ToString();
        }

    }
}
