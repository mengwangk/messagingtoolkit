using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// MSI Mod 10 encoder.
    /// </summary>
    public sealed class MsiEncoder : OneDEncoder
    {
        private static readonly int[] StartWidths = new[] { 2, 1 };
        private static readonly int[] EndWidths = new[] { 1, 2, 1 };
        private static readonly int[][] NumberWidths = new[]
                                                        {
                                                           new[] { 1, 2, 1, 2, 1, 2, 1, 2 },
                                                           new[] { 1, 2, 1, 2, 1, 2, 2, 1 },
                                                           new[] { 1, 2, 1, 2, 2, 1, 1, 2 },
                                                           new[] { 1, 2, 1, 2, 2, 1, 2, 1 },
                                                           new[] { 1, 2, 2, 1, 1, 2, 1, 2 },
                                                           new[] { 1, 2, 2, 1, 1, 2, 2, 1 },
                                                           new[] { 1, 2, 2, 1, 2, 1, 1, 2 },
                                                           new[] { 1, 2, 2, 1, 2, 1, 2, 1 },
                                                           new[] { 2, 1, 1, 2, 1, 2, 1, 2 },
                                                           new[] { 2, 1, 1, 2, 1, 2, 2, 1 }
                                                        };

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
        public override BitMatrix Encode(String contents,
                                BarcodeFormat format,
                                int width,
                                int height,
                                Dictionary<EncodeOptions, object> encodingOptions)
        {
            if (format != BarcodeFormat.MSIMod10)
            {
                throw new ArgumentException("Can only encode MSI, but got " + format);
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
                int indexInString = MsiDecoder.AlphabetString.IndexOf(contents[i]);
                if (indexInString < 0)
                    throw new ArgumentException("Requested contents contains a not encodable character: '" + contents[i] + "'");
            }

            var codeWidth = 3 + length * 12 + 4;
            var result = new bool[codeWidth];
            var pos = AppendPattern(result, 0, StartWidths, true);
            for (var i = 0; i < length; i++)
            {
                var indexInString = MsiDecoder.AlphabetString.IndexOf(contents[i]);
                var widths = NumberWidths[indexInString];
                pos += AppendPattern(result, pos, widths, true);
            }
            AppendPattern(result, pos, EndWidths, true);
            return result;
        }
    }
}
