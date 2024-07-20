using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessagingToolkit.Barcode.Common
{
    /// <summary>
    ///   <p>Represents a 2D matrix of bits. In function arguments below, and throughout the common
    /// module, x is the column position, and y is the row position. The ordering is always x, y.
    /// The origin is at the top-left.</p>
    ///   <p>Internally the bits are represented in a 1-D array of 32-bit ints. However, each row begins
    /// with a new int. This is done intentionally so that we can copy out a row into a BitArray very
    /// efficiently.</p>
    ///   <p>The ordering of bits is row-major. Within each int, the least significant bits are used first,
    /// meaning they represent lower x values. This is compatible with BitArray's implementation.</p>
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class BitMatrix
    {
        private readonly int width;
        private readonly int height;
        private readonly int rowSize;
        private readonly int[] bits;

        // A helper to construct a square matrix.
        public BitMatrix(int dimension)
            : this(dimension, dimension)
        {
        }

        public BitMatrix(int width, int height)
        {
            if (width < 1 || height < 1)
            {
                throw new ArgumentException("Both dimensions must be greater than 0");
            }
            this.width = width;
            this.height = height;
            this.rowSize = (width + 31) >> 5;
            bits = new int[rowSize * height];
        }

        /// <summary>
        /// <p>Gets the requested bit, where true means black.</p>
        /// </summary>
        /// <param name="x">The horizontal component (i.e. which column)</param>
        /// <param name="y">The vertical component (i.e. which row)</param>
        /// <returns>value of given bit in matrix</returns>
        public bool Get(int x, int y)
        {
            int offset = y * rowSize + (x >> 5);
            return (((int)(((uint)bits[offset]) >> (x & 0x1f))) & 1) != 0;
        }

        /// <summary>
        /// <p>Gets the requested bit, where true means black.</p>
        /// </summary>
        ///
        /// <param name="x">The horizontal component (i.e. which column)</param>
        /// <param name="y">The vertical component (i.e. which row)</param>
        /// <returns>value of given bit in matrix</returns>
        public bool GetValue(int x, int y)
        {
            return Get(x, y);
        }

        /// <summary>
        /// <p>Sets the given bit to true.</p>
        /// </summary>
        ///
        /// <param name="x">The horizontal component (i.e. which column)</param>
        /// <param name="y">The vertical component (i.e. which row)</param>
        public void Set(int x, int y)
        {
            int offset = y * rowSize + (x >> 5);
            bits[offset] |= 1 << (x & 0x1f);
        }

        /// <summary>
        ///   <p>Sets the given bit to true.</p>
        /// </summary>
        /// <param name="x">The horizontal component (i.e. which column)</param>
        /// <param name="y">The vertical component (i.e. which row)</param>
        public void SetValue(int x, int y)
        {
            Set(x, y);
        }

        /// <summary>
        /// <p>Flips the given bit.</p>
        /// </summary>
        ///
        /// <param name="x">The horizontal component (i.e. which column)</param>
        /// <param name="y">The vertical component (i.e. which row)</param>
        public void Flip(int x, int y)
        {
            int offset = y * rowSize + (x >> 5);
            bits[offset] ^= 1 << (x & 0x1f);
        }

        /// <summary>
        /// Clears all bits (sets to false).
        /// </summary>
        ///
        public void Clear()
        {
            int max = bits.Length;
            for (int i = 0; i < max; i++)
            {
                bits[i] = 0;
            }
        }

        /// <summary>
        /// <p>Sets a square region of the bit matrix to true.</p>
        /// </summary>
        ///
        /// <param name="left">The horizontal position to begin at (inclusive)</param>
        /// <param name="top">The vertical position to begin at (inclusive)</param>
        /// <param name="width_0">The width of the region</param>
        /// <param name="height_1">The height of the region</param>
        public void SetRegion(int left, int top, int width_0, int height_1)
        {
            if (top < 0 || left < 0)
            {
                throw new ArgumentException("Left and top must be nonnegative");
            }
            if (height_1 < 1 || width_0 < 1)
            {
                throw new ArgumentException("Height and width must be at least 1");
            }
            int right = left + width_0;
            int bottom = top + height_1;
            if (bottom > this.height || right > this.width)
            {
                throw new ArgumentException("The region must fit inside the matrix");
            }
            for (int y = top; y < bottom; y++)
            {
                int offset = y * rowSize;
                for (int x = left; x < right; x++)
                {
                    bits[offset + (x >> 5)] |= 1 << (x & 0x1f);
                }
            }
        }

        /// <summary>
        /// A fast method to retrieve one row of data from the matrix as a BitArray.
        /// </summary>
        ///
        /// <param name="y">The row to retrieve</param>
        /// <param name="row">An optional caller-allocated BitArray, will be allocated if null or too small</param>
        /// <returns>The resulting BitArray - this reference should always be used even when passing
        /// your own row</returns>
        public BitArray GetRow(int y, BitArray row)
        {
            if (row == null || row.GetSize() < width)
            {
                row = new BitArray(width);
            }
            int offset = y * rowSize;
            for (int x = 0; x < rowSize; x++)
            {
                row.SetBulk(x << 5, bits[offset + x]);
            }
            return row;
        }


        /// <param name="y">row to set</param>
        /// <param name="row">to copy from</param>
        public void SetRow(int y, BitArray row)
        {
            System.Array.Copy((Array)(row.GetBitArray()), 0, (Array)(bits), y * rowSize, rowSize);
        }

        /// <summary>
        /// This is useful in detecting the enclosing rectangle of a 'pure' barcode.
        /// </summary>
        ///
        /// <returns>{left,top,width,height} enclosing rectangle of all 1 bits, or null if it is all white</returns>
        public int[] GetEnclosingRectangle()
        {
            int left = width;
            int top = height;
            int right = -1;
            int bottom = -1;

            for (int y = 0; y < height; y++)
            {
                for (int x32 = 0; x32 < rowSize; x32++)
                {
                    int theBits = bits[y * rowSize + x32];
                    if (theBits != 0)
                    {
                        if (y < top)
                        {
                            top = y;
                        }
                        if (y > bottom)
                        {
                            bottom = y;
                        }
                        if (x32 * 32 < left)
                        {
                            int bit = 0;
                            while ((theBits << (31 - bit)) == 0)
                            {
                                bit++;
                            }
                            if ((x32 * 32 + bit) < left)
                            {
                                left = x32 * 32 + bit;
                            }
                        }
                        if (x32 * 32 + 31 > right)
                        {
                            int bit_0 = 31;
                            while (((int)(((uint)theBits) >> bit_0)) == 0)
                            {
                                bit_0--;
                            }
                            if ((x32 * 32 + bit_0) > right)
                            {
                                right = x32 * 32 + bit_0;
                            }
                        }
                    }
                }
            }

            int width_1 = right - left;
            int height_2 = bottom - top;

            if (width_1 < 0 || height_2 < 0)
            {
                return null;
            }

            return new int[] { left, top, width_1, height_2 };
        }

        /// <summary>
        /// This is useful in detecting a corner of a 'pure' barcode.
        /// </summary>
        ///
        /// <returns>{x,y} coordinate of top-left-most 1 bit, or null if it is all white</returns>
        public int[] GetTopLeftOnBit()
        {
            int bitsOffset = 0;
            while (bitsOffset < bits.Length && bits[bitsOffset] == 0)
            {
                bitsOffset++;
            }
            if (bitsOffset == bits.Length)
            {
                return null;
            }
            int y = bitsOffset / rowSize;
            int x = (bitsOffset % rowSize) << 5;

            int theBits = bits[bitsOffset];
            int bit = 0;
            while ((theBits << (31 - bit)) == 0)
            {
                bit++;
            }
            x += bit;
            return new int[] { x, y };
        }

        public int[] GetBottomRightOnBit()
        {
            int bitsOffset = bits.Length - 1;
            while (bitsOffset >= 0 && bits[bitsOffset] == 0)
            {
                bitsOffset--;
            }
            if (bitsOffset < 0)
            {
                return null;
            }

            int y = bitsOffset / rowSize;
            int x = (bitsOffset % rowSize) << 5;

            int theBits = bits[bitsOffset];
            int bit = 31;
            while (((int)(((uint)theBits) >> bit)) == 0)
            {
                bit--;
            }
            x += bit;

            return new int[] { x, y };
        }


        /// <returns>The width of the matrix</returns>
        public int GetWidth()
        {
            return width;
        }


        /// <returns>The height of the matrix</returns>
        public int GetHeight()
        {
            return height;
        }


        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public override bool Equals(Object o)
        {
            if (!(o is BitMatrix))
            {
                return false;
            }
            BitMatrix other = (BitMatrix)o;
            if (width != other.width || height != other.height || rowSize != other.rowSize || bits.Length != other.bits.Length)
            {
                return false;
            }
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] != other.bits[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = width;
            hash = 31 * hash + width;
            hash = 31 * hash + height;
            hash = 31 * hash + rowSize;
            /* foreach */
            foreach (int bit in bits)
            {
                hash = 31 * hash + bit;
            }
            return hash;
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder(height * (width + 1));
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result.Append((Get(x, y)) ? "X " : "  ");
                }
                result.Append('\n');
            }
            return result.ToString();
        }
	
    }
}
