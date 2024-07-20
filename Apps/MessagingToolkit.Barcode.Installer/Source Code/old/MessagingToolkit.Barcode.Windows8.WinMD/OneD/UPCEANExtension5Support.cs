using System;
using System.Text;
using System.Collections.Generic;

using BitArray = MessagingToolkit.Barcode.Common.BitArray;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// Modified: May 26 2012
    /// </summary>
    internal sealed class UPCEANExtension5Support 
    {
        public UPCEANExtension5Support()
        {
            this.decodeMiddleCounters = new int[4];
            this.decodeRowStringBuffer = new StringBuilder();
        }

        private static readonly int[] CHECK_DIGIT_ENCODINGS = { 0x18, 0x14, 0x12, 0x11, 0x0C, 0x06, 0x03, 0x0A, 0x09, 0x05 };

        private readonly int[] decodeMiddleCounters;
        private readonly StringBuilder decodeRowStringBuffer;

        internal Result DecodeRow(int rowNumber, BitArray row, int[] extensionStartRange)
        {

            StringBuilder result = decodeRowStringBuffer;
            result.Length = 0;
            int end = DecodeMiddle(row, extensionStartRange, result);

            String resultString = result.ToString();
            Dictionary<ResultMetadataType, Object> extensionData = ParseExtensionString(resultString);

            Result extensionResult = new Result(resultString, null, new ResultPoint[] { 
                new ResultPoint((extensionStartRange[0] + extensionStartRange[1]) / 2.0f, (float) rowNumber),
					new ResultPoint((float) end, (float) rowNumber), }, BarcodeFormat.UPCEANExtension);
            if (extensionData != null)
            {
                extensionResult.PutAllMetadata(extensionData);
            }
            return extensionResult;
        }

        internal int DecodeMiddle(BitArray row, int[] startRange, StringBuilder resultString)
        {
            int[] counters = decodeMiddleCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;
            int end = row.GetSize();
            int rowOffset = startRange[1];

            int lgPatternFound = 0;

            for (int x = 0; x < 5 && rowOffset < end; x++)
            {
                int bestMatch = UPCEANDecoder.DecodeDigit(row, counters, rowOffset, UPCEANDecoder.LAndGPatterns);
                resultString.Append((char)('0' + bestMatch % 10));
                /* foreach */
                foreach (int counter in counters)
                {
                    rowOffset += counter;
                }
                if (bestMatch >= 10)
                {
                    lgPatternFound |= 1 << (4 - x);
                }
                if (x != 4)
                {
                    // Read off separator if not last
                    rowOffset = row.GetNextSet(rowOffset);
                    rowOffset = row.GetNextUnset(rowOffset);
                }
            }

            if (resultString.Length != 5)
            {
                throw NotFoundException.Instance;
            }

            int checkDigit = DetermineCheckDigit(lgPatternFound);
            if (ExtensionChecksum(resultString.ToString()) != checkDigit)
            {
                 throw NotFoundException.Instance;
            }

            return rowOffset;
        }

        private static int ExtensionChecksum(String s)
        {
            int length = s.Length;
            int sum = 0;
            for (int i = length - 2; i >= 0; i -= 2)
            {
                sum += (int)s[i] - (int)'0';
            }
            sum *= 3;
            for (int i_0 = length - 1; i_0 >= 0; i_0 -= 2)
            {
                sum += (int)s[i_0] - (int)'0';
            }
            sum *= 3;
            return sum % 10;
        }

        private static int DetermineCheckDigit(int lgPatternFound)
        {
            for (int d = 0; d < 10; d++)
            {
                if (lgPatternFound == CHECK_DIGIT_ENCODINGS[d])
                {
                    return d;
                }
            }
            throw NotFoundException.Instance;
        }


        /// <param name="raw">raw content of extension</param>
        /// <returns>formatted interpretation of raw content as a 
        /// <see cref="T:System.Collections.IDictionary"/>
        ///  mapping
        /// one 
        /// <see cref="null"/>
        ///  to appropriate value, or 
        /// }
        ///  if not known</returns>
        private static Dictionary<ResultMetadataType, Object> ParseExtensionString(String raw)
        {
            if (raw.Length != 5)
            {
                return null;
            }
            Object val = ParseExtension5String(raw);
            if (val == null)
            {
                return null;
            }
            Dictionary<ResultMetadataType, Object> result = new Dictionary<ResultMetadataType, Object>();
            result.Add(ResultMetadataType.SuggestedPrice, val);
            return result;
        }

        private static String ParseExtension5String(String raw)
        {
            String currency;
            switch ((int)raw[0])
            {
                case '0':
                    currency = "£";
                    break;
                case '5':
                    currency = "$";
                    break;
                case '9':
                    // Reference: http://www.jollytech.com
                    if ("90000".Equals(raw))
                    {
                        // No suggested retail price
                        return null;
                    }
                    if ("99991".Equals(raw))
                    {
                        // Complementary
                        return "0.00";
                    }
                    if ("99990".Equals(raw))
                    {
                        return "Used";
                    }
                    // Otherwise... unknown currency?
                    currency = "";
                    break;
                default:
                    currency = "";
                    break;
            }
            int rawAmount = Int32.Parse(raw.Substring(1));
            String unitsString = (rawAmount / 100).ToString();
            int hundredths = rawAmount % 100;
            String hundredthsString = (hundredths < 10) ? "0" + hundredths : hundredths.ToString();
            return currency + unitsString + '.' + hundredthsString;
        }
    }
}
