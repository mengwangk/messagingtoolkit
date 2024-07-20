using System;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{
    /// <summary>
    /// <p>Encapsulates data masks for the data bits in a QR code, per ISO 18004:2006 6.8. Implementations
    /// of this class can un-mask a raw BitMatrix. For simplicity, they will unmask the entire BitMatrix,
    /// including areas used for finder patterns, timing patterns, etc. These areas should be unused
    /// after the point they are unmasked anyway.</p>
    /// <p>Note that the diagram in section 6.8.1 is misleading since it indicates that i is column position
    /// and j is row position. In fact, as the text says, i is row position and j is column position.</p>
    /// 
    /// Modified: April 22 2012
    /// </summary>
    abstract internal class DataMask
    {

        /// <summary>
        /// See ISO 18004:2006 6.8.1
        /// </summary>
        private static readonly DataMask[] DATA_MASKS = 
            { new DataMask.DataMask000 (), new DataMask.DataMask001 (), new DataMask.DataMask010 (), new DataMask.DataMask011 (), new DataMask.DataMask100 (), new DataMask.DataMask101 (), new DataMask.DataMask110 (),
				new DataMask.DataMask111 (), };

        private DataMask()
        {
        }

        /// <summary>
        /// <p>Implementations of this method reverse the data masking process applied to a QR Code and
        /// make its bits ready to read.</p>
        /// </summary>
        ///
        /// <param name="bits">representation of QR Code bits</param>
        /// <param name="dimension">dimension of QR Code, represented by bits, being unmasked</param>
        internal void UnmaskBitMatrix(BitMatrix bits, int dimension)
        {
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    if (IsMasked(i, j))
                    {
                        bits.Flip(j, i);
                    }
                }
            }
        }

        abstract internal bool IsMasked(int i, int j);


        /// <param name="reference"></param>
        /// <returns>DataMask encapsulating the data mask pattern</returns>
        static internal DataMask ForReference(int reference)
        {
            if (reference < 0 || reference > 7)
            {
                throw new ArgumentException();
            }
            return DATA_MASKS[reference];
        }

        /// <summary>
        /// 000: mask bits for which (x + y) mod 2 == 0
        /// </summary>
        ///
        private sealed class DataMask000 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                return ((i + j) & 0x01) == 0;
            }
        }

        /// <summary>
        /// 001: mask bits for which x mod 2 == 0
        /// </summary>
        ///
        private sealed class DataMask001 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                return (i & 0x01) == 0;
            }
        }

        /// <summary>
        /// 010: mask bits for which y mod 3 == 0
        /// </summary>
        ///
        private sealed class DataMask010 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                return j % 3 == 0;
            }
        }

        /// <summary>
        /// 011: mask bits for which (x + y) mod 3 == 0
        /// </summary>
        ///
        private sealed class DataMask011 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                return (i + j) % 3 == 0;
            }
        }

        /// <summary>
        /// 100: mask bits for which (x/2 + y/3) mod 2 == 0
        /// </summary>
        ///
        private sealed class DataMask100 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                return ((((int)(((uint)i) >> 1)) + (j / 3)) & 0x01) == 0;
            }
        }

        /// <summary>
        /// 101: mask bits for which xy mod 2 + xy mod 3 == 0
        /// </summary>
        ///
        private sealed class DataMask101 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                int temp = i * j;
                return (temp & 0x01) + (temp % 3) == 0;
            }
        }

        /// <summary>
        /// 110: mask bits for which (xy mod 2 + xy mod 3) mod 2 == 0
        /// </summary>
        ///
        private sealed class DataMask110 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                int temp = i * j;
                return (((temp & 0x01) + (temp % 3)) & 0x01) == 0;
            }
        }

        /// <summary>
        /// 111: mask bits for which ((x+y)mod 2 + xy mod 3) mod 2 == 0
        /// </summary>
        ///
        private sealed class DataMask111 : DataMask
        {
            internal override bool IsMasked(int i, int j)
            {
                return ((((i + j) & 0x01) + ((i * j) % 3)) & 0x01) == 0;
            }
        }
    }
}
