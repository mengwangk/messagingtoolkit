using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class Pdf417CodewordDecoder
    {
        private static readonly float[][] RATIOS_TABLE = BarcodeHelper.CreateRectangularFloatArray(Pdf417Common.SYMBOL_TABLE.Length, Pdf417Common.BARS_IN_MODULE);

        static Pdf417CodewordDecoder()
        {
            // Pre-computes the symbol ratio table.
            for (int i = 0; i < Pdf417Common.SYMBOL_TABLE.Length; i++)
            {
                int currentSymbol = Pdf417Common.SYMBOL_TABLE[i];
                int currentBit = currentSymbol & 0x1;
                for (int j = 0; j < Pdf417Common.BARS_IN_MODULE; j++)
                {
                    float size = 0.0f;
                    while ((currentSymbol & 0x1) == currentBit)
                    {
                        size += 1.0f;
                        currentSymbol >>= 1;
                    }
                    currentBit = currentSymbol & 0x1;
                    RATIOS_TABLE[i][Pdf417Common.BARS_IN_MODULE - j - 1] = size / Pdf417Common.MODULES_IN_CODEWORD;
                }
            }
        }

        private Pdf417CodewordDecoder()
        {
        }

        internal static int GetDecodedValue(int[] moduleBitCount)
        {
            int decodedValue = GetDecodedCodewordValue(SampleBitCounts(moduleBitCount));
            if (decodedValue != -1)
            {
                return decodedValue;
            }
            return GetClosestDecodedValue(moduleBitCount);
        }

        private static int[] SampleBitCounts(int[] moduleBitCount)
        {
            float bitCountSum = Pdf417Common.GetBitCountSum(moduleBitCount);
            int[] result = new int[Pdf417Common.BARS_IN_MODULE];
            int bitCountIndex = 0;
            int sumPreviousBits = 0;
            for (int i = 0; i < Pdf417Common.MODULES_IN_CODEWORD; i++)
            {
                float sampleIndex = bitCountSum / (2 * Pdf417Common.MODULES_IN_CODEWORD) + (i * bitCountSum) / Pdf417Common.MODULES_IN_CODEWORD;
                if (sumPreviousBits + moduleBitCount[bitCountIndex] <= sampleIndex)
                {
                    sumPreviousBits += moduleBitCount[bitCountIndex];
                    bitCountIndex++;
                }
                result[bitCountIndex]++;
            }
            return result;
        }

        private static int GetDecodedCodewordValue(int[] moduleBitCount)
        {
            int decodedValue = GetBitValue(moduleBitCount);
            return Pdf417Common.GetCodeword(decodedValue) == -1 ? -1 : decodedValue;
        }

        private static int GetBitValue(int[] moduleBitCount)
        {
            long result = 0;
            for (int i = 0; i < moduleBitCount.Length; i++)
            {
                for (int bit = 0; bit < moduleBitCount[i]; bit++)
                {
                    result = (result << 1) | (i % 2 == 0 ? 1 : 0);
                }
            }
            return (int)result;
        }

        private static int GetClosestDecodedValue(int[] moduleBitCount)
        {
            int bitCountSum = Pdf417Common.GetBitCountSum(moduleBitCount);
            float[] bitCountRatios = new float[Pdf417Common.BARS_IN_MODULE];
            for (int i = 0; i < bitCountRatios.Length; i++)
            {
                bitCountRatios[i] = moduleBitCount[i] / (float)bitCountSum;
            }
            float bestMatchError = float.MaxValue;
            int bestMatch = -1;
            for (int j = 0; j < RATIOS_TABLE.Length; j++)
            {
                float error = 0.0f;
                for (int k = 0; k < Pdf417Common.BARS_IN_MODULE; k++)
                {
                    float diff = RATIOS_TABLE[j][k] - bitCountRatios[k];
                    error += diff * diff;
                }
                if (error < bestMatchError)
                {
                    bestMatchError = error;
                    bestMatch = Pdf417Common.SYMBOL_TABLE[j];
                }
            }
            return bestMatch;
        }

    }
}
