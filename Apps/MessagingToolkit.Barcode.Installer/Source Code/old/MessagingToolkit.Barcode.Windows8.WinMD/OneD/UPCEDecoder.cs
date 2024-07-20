using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{


    /// <summary>
    /// <p>Implements decoding of the UPC-E format.</p>
    /// <p/>
    /// <p><a href="http://www.barcodeisland.com/upce.phtml">This</a> is a great reference for
    /// UPC-E information.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class UPCEDecoder : UPCEANDecoder
    {

        /// <summary>
        /// The pattern that marks the middle, and end, of a UPC-E pattern.
        /// There is no "second half" to a UPC-E barcode.
        /// </summary>
        ///
        private static readonly int[] MiddleEndPattern = { 1, 1, 1, 1, 1, 1 };

        /// <summary>
        /// See <see cref="M:MessagingToolkit.Barcode.Oned.UPCEReader.L_AND_G_PATTERNS"/>; these values similarly represent patterns of
        /// even-odd parity encodings of digits that imply both the number system (0 or 1)
        /// used, and the check digit.
        /// </summary>
        ///
        private static readonly int[][] NumSysAndCheckDigitPatterns = {
				new int[] { 0x38, 0x34, 0x32, 0x31, 0x2C, 0x26, 0x23, 0x2A, 0x29,
						0x25 },
				new int[] { 0x07, 0x0B, 0x0D, 0x0E, 0x13, 0x19, 0x1C, 0x15, 0x16,
						0x1A } };

        private readonly int[] decodeMiddleCounters;

        public UPCEDecoder()
        {
            decodeMiddleCounters = new int[4];
        }

        protected internal override int DecodeMiddle(BitArray row, int[] startRange, StringBuilder result)
        {
            int[] counters = decodeMiddleCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;
            int end = row.GetSize();
            int rowOffset = startRange[1];

            int lgPatternFound = 0;

            for (int x = 0; x < 6 && rowOffset < end; x++)
            {
                int bestMatch = MessagingToolkit.Barcode.OneD.UPCEANDecoder.DecodeDigit(row, counters, rowOffset,
                        MessagingToolkit.Barcode.OneD.UPCEANDecoder.LAndGPatterns);
                result.Append((char)('0' + bestMatch % 10));
                for (int i = 0; i < counters.Length; i++)
                {
                    rowOffset += counters[i];
                }
                if (bestMatch >= 10)
                {
                    lgPatternFound |= 1 << (5 - x);
                }
            }

            DetermineNumSysAndCheckDigit(result, lgPatternFound);

            return rowOffset;
        }

        internal override int[] DecodeEnd(BitArray row, int endStart)
        {
            return MessagingToolkit.Barcode.OneD.UPCEANDecoder.FindGuardPattern(row, endStart, true, MiddleEndPattern);
        }

        internal override bool CheckChecksum(String s)
        {
            return base.CheckChecksum(ConvertUPCEtoUPCA(s));
        }

        private static void DetermineNumSysAndCheckDigit(StringBuilder resultString,
                int lgPatternFound)
        {

            for (int numSys = 0; numSys <= 1; numSys++)
            {
                for (int d = 0; d < 10; d++)
                {
                    if (lgPatternFound == NumSysAndCheckDigitPatterns[numSys][d])
                    {
#if !SILVERLIGHT
                        resultString.Insert(0, (char)('0' + numSys));
#else
                        resultString.Insert(0, new char[] { (char)('0' + numSys) });
#endif
                        resultString.Append((char)('0' + d));
                        return;
                    }
                }
            }
            throw NotFoundException.Instance;
        }

        internal override BarcodeFormat BarcodeFormat
        {
            get
            {
                return MessagingToolkit.Barcode.BarcodeFormat.UPCE;
            }
        }

        /// <summary>
        /// Expands a UPC-E value back into its full, equivalent UPC-A code value.
        /// </summary>
        ///
        /// <param name="upce">UPC-E code as string of digits</param>
        /// <returns>equivalent UPC-A code as string of digits</returns>
        public static String ConvertUPCEtoUPCA(String upce)
        {
            char[] upceChars = new char[6];
            upce.CopyTo(1, upceChars, 0, 7 - 1);
            StringBuilder result = new StringBuilder(12);
            result.Append(upce[0]);
            char lastChar = upceChars[5];
            switch ((int)lastChar)
            {
                case '0':
                case '1':
                case '2':
                    result.Append(upceChars, 0, 2);
                    result.Append(lastChar);
                    result.Append("0000");
                    result.Append(upceChars, 2, 3);
                    break;
                case '3':
                    result.Append(upceChars, 0, 3);
                    result.Append("00000");
                    result.Append(upceChars, 3, 2);
                    break;
                case '4':
                    result.Append(upceChars, 0, 4);
                    result.Append("00000");
                    result.Append(upceChars[4]);
                    break;
                default:
                    result.Append(upceChars, 0, 5);
                    result.Append("0000");
                    result.Append(lastChar);
                    break;
            }
            result.Append(upce[7]);
            return result.ToString();
        }

    }
}
