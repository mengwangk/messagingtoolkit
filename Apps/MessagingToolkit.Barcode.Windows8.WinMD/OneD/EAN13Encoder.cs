using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// This object renders an EAN13 code.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class EAN13Encoder : UPCEANEncoder
    {
        private const int CODE_WIDTH = 3 + // start guard
                 (7 * 6) + // left bars
                 5 + // middle guard
                 (7 * 6) + // right bars
                 3; // end guard

        public override BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {

            if (format != BarcodeFormat.EAN13)
            {
                throw new ArgumentException("Can only encode EAN_13, but got " + format);
            }

            return base.Encode(contents, format, width, height, encodingOptions);
        }

        public override bool[] Encode(String contents)
        {

            if (contents.Length != 13)
            {
                throw new ArgumentException("Requested contents should be 13 digits long, but got " + contents.Length);
            }
            
            try
            {
                if (!UPCEANDecoder.CheckStandardUPCEANChecksum(contents))
                {
                    throw new ArgumentException("Contents do not pass checksum");
                }
            }
            catch (FormatException)
            {
                throw new ArgumentException("Illegal contents");
            }

            int firstDigit = Int32.Parse(contents.Substring(0, (1) - (0)));
            int parities = EAN13Decoder.FirstDigitEncodings[firstDigit];
            bool[] result = new bool[CODE_WIDTH];
            int pos = 0;

            pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, MessagingToolkit.Barcode.OneD.UPCEANDecoder.StartEndPattern, true);

            // See {@link #EAN13Reader} for a description of how the first digit & left bars are encoded
            for (int i = 1; i <= 6; i++)
            {
                int digit = Int32.Parse(contents.Substring(i, (i + 1) - (i)));
                if ((parities >> (6 - i) & 1) == 1)
                {
                    digit += 10;
                }
                pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, MessagingToolkit.Barcode.OneD.UPCEANDecoder.LAndGPatterns[digit], false);
            }

            pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, MessagingToolkit.Barcode.OneD.UPCEANDecoder.MiddlePattern, false);

            for (int i = 7; i <= 12; i++)
            {
                int digit = Int32.Parse(contents.Substring(i, (i + 1) - (i)));
                pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, MessagingToolkit.Barcode.OneD.UPCEANDecoder.LPatterns[digit], true);
            }
            pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, MessagingToolkit.Barcode.OneD.UPCEANDecoder.StartEndPattern, true);

            return result;
        }

    }
}
