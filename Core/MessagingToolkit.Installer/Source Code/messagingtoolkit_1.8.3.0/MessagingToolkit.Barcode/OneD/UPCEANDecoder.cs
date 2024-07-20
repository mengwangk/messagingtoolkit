using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    ///   <p>Encapsulates functionality and implementation that is common to UPC and EAN families
    /// of one-dimensional barcodes.</p>
    /// 
    /// Modified: April 19 2012
    /// </summary>
    public abstract class UPCEANDecoder : OneDDecoder
    {

        // These two values are critical for determining how permissive the decoding will be.
        // We've arrived at these values through a lot of trial and error. Setting them any higher
        // lets false positives creep in quickly.
        private const int MaxAvgVariance = (int)(PatternMatchResultScaleFactor * 0.48f);
        private const int MaxIndividualVariance = (int)(PatternMatchResultScaleFactor * 0.7f);

        /// <summary>
        /// Start/end guard pattern.
        /// </summary>
        ///
        static internal readonly int[] StartEndPattern = { 1, 1, 1, };

        /// <summary>
        /// Pattern marking the middle of a UPC/EAN pattern, separating the two halves.
        /// </summary>
        ///
        static internal readonly int[] MiddlePattern = { 1, 1, 1, 1, 1 };

        /// <summary>
        /// "Odd", or "L" patterns used to encode UPC/EAN digits.
        /// </summary>
        ///
        static internal readonly int[][] LPatterns = { new int[] { 3, 2, 1, 1 }, new int[] { 2, 2, 2, 1 }, new int[] { 2, 1, 2, 2 }, new int[] { 1, 4, 1, 1 }, new int[] { 1, 1, 3, 2 }, new int[] { 1, 2, 3, 1 }, new int[] { 1, 1, 1, 4 },
				new int[] { 1, 3, 1, 2 }, new int[] { 1, 2, 1, 3 }, new int[] { 3, 1, 1, 2 } };

        /// <summary>
        /// As above but also including the "even", or "G" patterns used to encode UPC/EAN digits.
        /// </summary>
        ///
        static internal readonly int[][] LAndGPatterns;

        private readonly StringBuilder decodeRowStringBuffer;
        private readonly UPCEANExtensionSupport extensionReader;
        private readonly EANManufacturerOrgSupport eanManSupport;

        protected internal UPCEANDecoder()
        {
            decodeRowStringBuffer = new StringBuilder(20);
            extensionReader = new UPCEANExtensionSupport();
            eanManSupport = new EANManufacturerOrgSupport();
        }

        static internal int[] FindStartGuardPattern(BitArray row)
        {
            bool foundStart = false;
            int[] startRange = null;
            int nextStart = 0;
            int[] counters = new int[StartEndPattern.Length];
            while (!foundStart)
            {
                for (int idx = 0; idx < StartEndPattern.Length; idx++)
                    counters[idx] = 0;
                startRange = FindGuardPattern(row, nextStart, false, StartEndPattern, counters);
                int start = startRange[0];
                nextStart = startRange[1];
                // Make sure there is a quiet zone at least as big as the start pattern before the barcode.
                // If this check would run off the left edge of the image, do not accept this barcode,
                // as it is very likely to be a false positive.
                int quietStart = start - (nextStart - start);
                if (quietStart >= 0)
                {
                    foundStart = row.IsRange(quietStart, start, false);
                }
            }
            return startRange;
        }

        public override Result DecodeRow(int rowNumber, BitArray row, Dictionary<DecodeOptions, object> decodingOptions)
        {
            return DecodeRow(rowNumber, row, FindStartGuardPattern(row), decodingOptions);
        }

        /// <summary>
        /// <p>Like <see cref="M:Com.Google.Zxing.Oned.UPCEANReader.DecodeRow(System.Int32,null,System.Collections.IDictionary)"/>, but
        /// allows caller to inform method about where the UPC/EAN start pattern is
        /// found. This allows this to be computed once and reused across many implementations.</p>
        /// </summary>
        ///
        public virtual Result DecodeRow(int rowNumber, BitArray row, int[] startGuardRange, Dictionary<DecodeOptions, object> decodingOptions)
        {

            ResultPointCallback resultPointCallback = (decodingOptions == null) ? null
                  : (ResultPointCallback)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.NeedResultPointCallback); ;

            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint((startGuardRange[0] + startGuardRange[1]) / 2.0f, rowNumber));
            }

            StringBuilder result = decodeRowStringBuffer;
            result.Length = 0;
            int endStart = DecodeMiddle(row, startGuardRange, result);

            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(endStart, rowNumber));
            }

            int[] endRange = DecodeEnd(row, endStart);

            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint((endRange[0] + endRange[1]) / 2.0f, rowNumber));
            }

            // Make sure there is a quiet zone at least as big as the end pattern after the barcode. The
            // spec might want more whitespace, but in practice this is the maximum we can count on.
            int end = endRange[1];
            int quietEnd = end + (end - endRange[0]);
            if (quietEnd >= row.GetSize() || !row.IsRange(end, quietEnd, false))
            {
                throw NotFoundException.Instance;
            }

            String resultString = result.ToString();
            if (!CheckChecksum(resultString))
            {
                throw ChecksumException.Instance;
            }

            float left = (float)(startGuardRange[1] + startGuardRange[0]) / 2.0f;
            float right = (float)(endRange[1] + endRange[0]) / 2.0f;
            BarcodeFormat format = this.BarcodeFormat;
            Result decodeResult = new Result(resultString, null, // no natural byte representation for these barcodes
                    new ResultPoint[] { new ResultPoint(left, (float)rowNumber), new ResultPoint(right, (float)rowNumber) }, format);

            try
            {
                Result extensionResult = extensionReader.DecodeRow(rowNumber, row, endRange[1]);
                decodeResult.PutMetadata(ResultMetadataType.UPCEANExtension, extensionResult.Text);
                decodeResult.PutAllMetadata(extensionResult.ResultMetadata);
                decodeResult.AddResultPoints(extensionResult.ResultPoints);
            }
            catch (BarcodeDecoderException)
            {
                // continue
            }

            if (format == BarcodeFormat.EAN13 || format == BarcodeFormat.UPCA)
            {
                String countryID = eanManSupport.LookupCountryIdentifier(resultString);
                if (countryID != null)
                {
                    decodeResult.PutMetadata(MessagingToolkit.Barcode.ResultMetadataType.PossibleCountry, countryID);
                }
            }

            return decodeResult;
        }


        internal virtual bool CheckChecksum(String s)
        {
            return CheckStandardUPCEANChecksum(s);
        }

        /// <summary>
        /// Computes the UPC/EAN checksum on a string of digits, and reports
        /// whether the checksum is correct or not.
        /// </summary>
        ///
        /// <param name="s">string of digits to check</param>
        /// <returns>true iff string of digits passes the UPC/EAN checksum algorithm</returns>
        /// <exception cref="FormatException">if the string does not contain only digits</exception>
        internal static bool CheckStandardUPCEANChecksum(String s)
        {
            int length = s.Length;
            if (length == 0)
            {
                return false;
            }

            int sum = 0;
            for (int i = length - 2; i >= 0; i -= 2)
            {
                int digit = (int)s[i] - (int)'0';
                if (digit < 0 || digit > 9)
                {
                    throw FormatException.Instance;
                }
                sum += digit;
            }
            sum *= 3;
            for (int i_0 = length - 1; i_0 >= 0; i_0 -= 2)
            {
                int digit_1 = (int)s[i_0] - (int)'0';
                if (digit_1 < 0 || digit_1 > 9)
                {
                    throw FormatException.Instance;
                }
                sum += digit_1;
            }
            return sum % 10 == 0;
        }

        internal virtual int[] DecodeEnd(BitArray row, int endStart)
        {
            return FindGuardPattern(row, endStart, false, StartEndPattern);
        }

        static internal int[] FindGuardPattern(BitArray row, int rowOffset, bool whiteFirst, int[] pattern)
        {
            return FindGuardPattern(row, rowOffset, whiteFirst, pattern, new int[pattern.Length]);
        }


        /// <param name="row">row of black/white values to search</param>
        /// <param name="rowOffset">position to start search</param>
        /// <param name="whiteFirst"></param>
        /// <param name="pattern"></param>
        /// <param name="counters">array of counters, as long as pattern, to re-use</param>
        /// <returns>start/end horizontal offset of guard pattern, as an array of two ints</returns>
        /// <exception cref="NotFoundException">if pattern is not found</exception>
        private static int[] FindGuardPattern(BitArray row, int rowOffset, bool whiteFirst, int[] pattern, int[] counters)
        {
            int patternLength = pattern.Length;
            int width = row.GetSize();
            bool isWhite = whiteFirst;
            rowOffset = (whiteFirst) ? row.GetNextUnset(rowOffset) : row.GetNextSet(rowOffset);
            int counterPosition = 0;
            int patternStart = rowOffset;
            for (int x = rowOffset; x < width; x++)
            {
                if (row.Get(x) ^ isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        if (PatternMatchVariance(counters, pattern, MaxIndividualVariance) < MaxAvgVariance)
                        {
                            return new int[] { patternStart, x };
                        }
                        patternStart += counters[0] + counters[1];
                        System.Array.Copy((Array)(counters), 2, (Array)(counters), 0, patternLength - 2);
                        counters[patternLength - 2] = 0;
                        counters[patternLength - 1] = 0;
                        counterPosition--;
                    }
                    else
                    {
                        counterPosition++;
                    }
                    counters[counterPosition] = 1;
                    isWhite = !isWhite;
                }
            }
            throw NotFoundException.Instance;
        }

        /// <summary>
        /// Attempts to decode a single UPC/EAN-encoded digit.
        /// </summary>
        ///
        /// <param name="row">row of black/white values to decode</param>
        /// <param name="counters">the counts of runs of observed black/white/black/... values</param>
        /// <param name="rowOffset">horizontal offset to start decoding from</param>
        /// <param name="patterns">be used</param>
        /// <returns>horizontal offset of first pixel beyond the decoded digit</returns>
        /// <exception cref="NotFoundException">if digit cannot be decoded</exception>
        static internal int DecodeDigit(BitArray row, int[] counters, int rowOffset, int[][] patterns)
        {
            RecordPattern(row, rowOffset, counters);
            int bestVariance = MaxAvgVariance; // worst variance we'll accept
            int bestMatch = -1;
            int max = patterns.Length;
            for (int i = 0; i < max; i++)
            {
                int[] pattern = patterns[i];
                int variance = PatternMatchVariance(counters, pattern, MaxIndividualVariance);
                if (variance < bestVariance)
                {
                    bestVariance = variance;
                    bestMatch = i;
                }
            }
            if (bestMatch >= 0)
            {
                return bestMatch;
            }
            else
            {
                throw NotFoundException.Instance;
            }
        }

        /// <summary>
        /// Get the format of this decoder.
        /// </summary>
        /// <returns>The 1D format.</returns>
        abstract internal BarcodeFormat BarcodeFormat { get; }

        /// <summary>
        /// Subclasses override this to decode the portion of a barcode between the start
        /// and end guard patterns.
        /// </summary>
        ///
        /// <param name="row">row of black/white values to search</param>
        /// <param name="startRange">start/end offset of start guard pattern</param>
        /// <param name="resultString">to append decoded chars to</param>
        /// <returns>horizontal offset of first pixel after the "middle" that was decoded</returns>
        /// <exception cref="NotFoundException">if decoding could not complete successfully</exception>
        protected abstract internal int DecodeMiddle(BitArray row, int[] startRange, StringBuilder resultString);

        static UPCEANDecoder()
        {
            LAndGPatterns = new int[20][];
            System.Array.Copy((Array)(LPatterns), 0, (Array)(LAndGPatterns), 0, 10);
            for (int i = 10; i < 20; i++)
            {
                int[] widths = LPatterns[i - 10];
                int[] reversedWidths = new int[widths.Length];
                for (int j = 0; j < widths.Length; j++)
                {
                    reversedWidths[j] = widths[widths.Length - j - 1];
                }
                LAndGPatterns[i] = reversedWidths;
            }
        }

    }
}
