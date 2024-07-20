using System;
using System.Text;
using System.Collections.Generic;

using BitArray = MessagingToolkit.Barcode.Common.BitArray;

namespace MessagingToolkit.Barcode.OneD
{
    internal sealed class UPCEANExtension2Support
    {

        public UPCEANExtension2Support()
        {
            this.decodeMiddleCounters = new int[4];
            this.decodeRowStringBuffer = new StringBuilder();
        }

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

            int checkParity = 0;

            for (int x = 0; x < 2 && rowOffset < end; x++)
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
                    checkParity |= 1 << (1 - x);
                }
                if (x != 1)
                {
                    // Read off separator if not last
                    rowOffset = row.GetNextSet(rowOffset);
                    rowOffset = row.GetNextUnset(rowOffset);
                }
            }

            if (resultString.Length != 2)
            {
                throw NotFoundException.Instance;
            }

            if (Int32.Parse(resultString.ToString()) % 4 != checkParity)
            {
                throw NotFoundException.Instance;
            }

            return rowOffset;
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
            if (raw.Length != 2)
            {
                return null;
            }
            Dictionary<ResultMetadataType, Object> result = new Dictionary<ResultMetadataType, Object>();
            result.Add(ResultMetadataType.IssueNumber, Int32.Parse(raw));
            return result;
        }

    }
}
