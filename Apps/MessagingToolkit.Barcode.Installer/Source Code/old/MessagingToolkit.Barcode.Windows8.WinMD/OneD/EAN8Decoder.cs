using System;
using System.Text;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// 	<para>Implements decoding of the EAN-8 format.</para>
    /// 	
    /// Modified: April 19 2012
    /// </summary>
    internal sealed class EAN8Decoder : UPCEANDecoder
    {
        private readonly int[] decodeMiddleCounters;

        public EAN8Decoder()
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

            for (int x = 0; x < 4 && rowOffset < end; x++)
            {
                int bestMatch = DecodeDigit(row, counters, rowOffset, LPatterns);
                result.Append((char)('0' + bestMatch));
                /* foreach */
                foreach (int counter in counters)
                {
                    rowOffset += counter;
                }
            }

            int[] middleRange = FindGuardPattern(row, rowOffset, true,MiddlePattern);
            rowOffset = middleRange[1];

            for (int x_0 = 0; x_0 < 4 && rowOffset < end; x_0++)
            {
                int bestMatch_1 = DecodeDigit(row, counters, rowOffset, LPatterns);
                result.Append((char)('0' + bestMatch_1));
                /* foreach */
                foreach (int counter_2 in counters)
                {
                    rowOffset += counter_2;
                }
            }

            return rowOffset;
        }

        internal override BarcodeFormat BarcodeFormat
        {
            get
            {
                return BarcodeFormat.EAN8;
            }
        }

    }
}