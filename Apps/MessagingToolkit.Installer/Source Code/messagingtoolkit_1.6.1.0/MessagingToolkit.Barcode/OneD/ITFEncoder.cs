using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// This object renders a ITF code.
    /// 
    /// Modifed: April 30 2012
    /// </summary>
    public sealed class ITFEncoder : OneDEncoder
    {
        private static readonly int[] START_PATTERN = { 1, 1, 1, 1 };
        private static readonly int[] END_PATTERN = { 3, 1, 1 };

        public override BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {
            if (format != BarcodeFormat.ITF14)
            {
                throw new ArgumentException("Can only encode ITF, but got " + format);
            }

            return base.Encode(contents, format, width, height, encodingOptions);
        }

        public override bool[] Encode(String contents)
        {
            int length = contents.Length;
            if (length % 2 != 0)
            {
                throw new ArgumentException("The length of the input should be even");
            }
            if (length > 80)
            {
                throw new ArgumentException("Requested contents should be less than 80 digits long, but got " + length);
            }
            bool[] result = new bool[9 + 9 * length];
            int pos = AppendPattern(result, 0, START_PATTERN, true);
            for (int i = 0; i < length; i += 2)
            {
                // @TODO - conversion made here
                int one = Convert.ToInt32(Convert.ToString(contents[i]), 10);
                int two = Convert.ToInt32(Convert.ToString(contents[i + 1]), 10);
                int[] encoding = new int[18];
                for (int j = 0; j < 5; j++)
                {
                    encoding[(j << 1)] = MessagingToolkit.Barcode.OneD.ITFDecoder.PATTERNS[one][j];
                    encoding[(j << 1) + 1] = MessagingToolkit.Barcode.OneD.ITFDecoder.PATTERNS[two][j];
                }
                pos += OneDEncoder.AppendPattern(result, pos, encoding, true);
            }
            AppendPattern(result, pos, END_PATTERN, true);

            return result;
        }

    }
}
