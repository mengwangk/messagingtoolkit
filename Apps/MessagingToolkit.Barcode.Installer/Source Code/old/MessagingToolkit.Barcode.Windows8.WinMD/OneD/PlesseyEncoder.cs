using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// Renders a Plessey code.
    /// </summary>
    internal sealed class PlesseyEncoder : OneDEncoder
    {
        private const String AlphabetString = "0123456789ABCDEF";
        private static readonly int[] StartWidths = new[] { 14, 11, 14, 11, 5, 20, 14, 11 };
        private static readonly int[] TerminationWidths = new[] { 25 };
        private static readonly int[] EndWidths = new[] { 20, 5, 20, 5, 14, 11, 14, 11 };
        private static readonly int[][] NumberWidths = new[]
                                                        {
                                                           new[] { 5, 20, 5, 20, 5, 20, 5, 20 },     // 0
                                                           new[] { 14, 11, 5, 20, 5, 20, 5, 20 },    // 1
                                                           new[] { 5, 20, 14, 11, 5, 20, 5, 20 },    // 2
                                                           new[] { 14, 11, 14, 11, 5, 20, 5, 20 },   // 3
                                                           new[] { 5, 20, 5, 20, 14, 11, 5, 20 },    // 4
                                                           new[] { 14, 11, 5, 20, 14, 11, 5, 20 },   // 5
                                                           new[] { 5, 20, 14, 11, 14, 11, 5, 20 },   // 6
                                                           new[] { 14, 11, 14, 11, 14, 11, 5, 20 },  // 7
                                                           new[] { 5, 20, 5, 20, 5, 20, 14, 11 },    // 8
                                                           new[] { 14, 11, 5, 20, 5, 20, 14, 11 },   // 9
                                                           new[] { 5, 20, 14, 11, 5, 20, 14, 11 },   // A / 10
                                                           new[] { 14, 11, 14, 11, 5, 20, 14, 11 },  // B / 11
                                                           new[] { 5, 20, 5, 20, 14, 11, 14, 11 },   // C / 12
                                                           new[] { 14, 11, 5, 20, 14, 11, 14, 11 },  // D / 13
                                                           new[] { 5, 20, 14, 11, 14, 11, 14, 11 },  // E / 14
                                                           new[] { 14, 11, 14, 11, 14, 11, 14, 11 }, // F / 15
                                                        };

        private static readonly byte[] crcGrid = new byte[] { 1, 1, 1, 1, 0, 1, 0, 0, 1 };
        private static readonly int[] crc0Widths = new[] { 5, 20 };
        private static readonly int[] crc1Widths = new[] { 14, 11 };

        /// <summary>
        /// Encode the contents following specified format.
        /// {@code width} and {@code height} are required size. This method may return bigger size
        /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
        /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
        /// or {@code height}, {@code IllegalArgumentException} is thrown.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="encodingOptions"></param>
        /// <returns></returns>
        public override BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {
            if (format != BarcodeFormat.ModifiedPlessey)
            {
                throw new ArgumentException("Can only encode Plessey, but got " + format);
            }
            return base.Encode(contents, format, width, height, encodingOptions);
        }

        /// <summary>
        /// Encode the contents to byte array expression of one-dimensional barcode.
        /// Start code and end code should be included in result, and side margins should not be included.
        /// <returns>a {@code boolean[]} of horizontal pixels (false = white, true = black)</returns>
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        override public bool[] Encode(String contents)
        {
            var length = contents.Length;
            for (var i = 0; i < length; i++)
            {
                int indexInString = AlphabetString.IndexOf(contents[i]);
                if (indexInString < 0)
                    throw new ArgumentException("Requested contents contains a not encodable character: '" + contents[i] + "'");
            }

            // quiet zone + start pattern + data + crc + termination bar + end pattern + quiet zone
            var codeWidth = 100 + 100 + length * 100 + 25 * 8 + 25 + 100 + 100;
            var result = new bool[codeWidth];
            var crcBuffer = new byte[4 * length + 8];
            var crcBufferPos = 0;
            var pos = 100;
            // start pattern
            pos = AppendPattern(result, pos, StartWidths, true);
            // data
            for (var i = 0; i < length; i++)
            {
                var indexInString = AlphabetString.IndexOf(contents[i]);
                var widths = NumberWidths[indexInString];
                pos += AppendPattern(result, pos, widths, true);
                // remember the position number for crc calculation
                crcBuffer[crcBufferPos++] = (byte)(indexInString & 1);
                crcBuffer[crcBufferPos++] = (byte)((indexInString >> 1) & 1);
                crcBuffer[crcBufferPos++] = (byte)((indexInString >> 2) & 1);
                crcBuffer[crcBufferPos++] = (byte)((indexInString >> 3) & 1);
            }

            // CRC calculation
            for (var i = 0; i < (4 * length); i++)
            {
                if (crcBuffer[i] != 0)
                {
                    for (var j = 0; j < 9; j++)
                    {
                        crcBuffer[i + j] ^= crcGrid[j];
                    }
                }
            }
            // append CRC pattern
            for (var i = 0; i < 8; i++)
            {
                switch (crcBuffer[length * 4 + i])
                {
                    case 0:
                        pos += AppendPattern(result, pos, crc0Widths, true);
                        break;
                    case 1:
                        pos += AppendPattern(result, pos, crc1Widths, true);
                        break;
                }
            }
            // termination bar

            pos += AppendPattern(result, pos, TerminationWidths, true);
            // end pattern
            AppendPattern(result, pos, EndWidths, false);
            return result;
        }
    }
}
