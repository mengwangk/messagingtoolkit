using MessagingToolkit.Barcode.Common;

using System;
using System.Text;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// <para>Decodes Codabar barcodes.</para>
    /// 
    /// Modified: April 12 2012
    /// </summary>
    internal sealed class CodaBarDecoder : OneDDecoder
    {

        // These values are critical for determining how permissive the decoding
        // will be. All stripe sizes must be within the window these define, as
        // compared to the average stripe size.
        private const int MAX_ACCEPTABLE = (int)(PatternMatchResultScaleFactor * 2.0f);
        private const int PADDING = (int)(PatternMatchResultScaleFactor * 1.5f);

        private const String ALPHABET_STRING = "0123456789-$:/.+ABCD";
        static internal readonly char[] ALPHABET = ALPHABET_STRING.ToCharArray();

        /// <summary>
        /// These represent the encodings of characters, as patterns of wide and narrow bars. The 7 least-significant bits of
        /// each int correspond to the pattern of wide and narrow, with 1s representing "wide" and 0s representing narrow.
        /// </summary>
        ///
        static internal readonly int[] CHARACTER_ENCODINGS = { 0x003, 0x006, 0x009, 0x060, 0x012, 0x042, 0x021, 0x024, 0x030, 0x048, // 0-9
				0x00c, 0x018, 0x045, 0x051, 0x054, 0x015, 0x01A, 0x029, 0x00B, 0x00E, // -$:/.+ABCD
		};

        // minimal number of characters that should be present (inclusing start and stop characters)
        // under normal circumstances this should be set to 3, but can be set higher
        // as a last-ditch attempt to reduce false positives.
        private const int MIN_CHARACTER_LENGTH = 3;

        // official start and end patterns
        private static readonly char[] STARTEND_ENCODING = { 'A', 'B', 'C', 'D' };
        // some codabar generator allow the codabar string to be closed by every
        // character. This will cause lots of false positives!

        // some industries use a checksum standard but this is not part of the original codabar standard
        // for more information see : http://www.mecsw.com/specs/codabar.html

        // Keep some instance variables to avoid reallocations
        private readonly StringBuilder decodeRowResult;
        private int[] counters;
        private int counterLength;

        public CodaBarDecoder()
        {
            decodeRowResult = new StringBuilder(20);
            counters = new int[80];
            counterLength = 0;
        }

        public override Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions)
        {

            for (var index = 0; index < counters.Length; index++)
	             counters[index] = 0;

            SetCounters(row);
            int startOffset = FindStartPattern();
            int nextStart = startOffset;

            decodeRowResult.Length = 0;
            do
            {
                int charOffset = ToNarrowWidePattern(nextStart);
                if (charOffset == -1)
                {
                    throw NotFoundException.Instance;
                }
                // Hack: We store the position in the alphabet table into a
                // StringBuilder, so that we can access the decoded patterns in
                // validatePattern. We'll translate to the actual characters later.
                decodeRowResult.Append((char)charOffset);
                nextStart += 8;
                // Stop as soon as we see the end character.
                if (decodeRowResult.Length > 1 && ArrayContains(STARTEND_ENCODING, ALPHABET[charOffset]))
                {
                    break;
                }
            } while (nextStart < counterLength); // no fixed end pattern so keep on reading while data is available

            // Look for whitespace after pattern:
            int trailingWhitespace = counters[nextStart - 1];
            int lastPatternSize = 0;
            for (int i = -8; i < -1; i++)
            {
                lastPatternSize += counters[nextStart + i];
            }

            // We need to see whitespace equal to 50% of the last pattern size,
            // otherwise this is probably a false positive. The exception is if we are
            // at the end of the row. (I.e. the barcode barely fits.)
            if (nextStart < counterLength && trailingWhitespace < lastPatternSize / 2)
            {
                throw NotFoundException.Instance;
            }

            ValidatePattern(startOffset);

            // Translate character table offsets to actual characters.
            for (int i_0 = 0; i_0 < decodeRowResult.Length; i_0++)
            {
                decodeRowResult[i_0] = ALPHABET[decodeRowResult[i_0]];
            }
            // Ensure a valid start and end character
            char startchar = decodeRowResult[0];
            if (!ArrayContains(STARTEND_ENCODING, startchar))
            {
                throw NotFoundException.Instance;
            }
            char endchar = decodeRowResult[decodeRowResult.Length - 1];
            if (!ArrayContains(STARTEND_ENCODING, endchar))
            {
                throw NotFoundException.Instance;
            }

            // remove stop/start characters character and check if a long enough string is contained
            if (decodeRowResult.Length <= MIN_CHARACTER_LENGTH)
            {
                // Almost surely a false positive ( start + stop + at least 1 character)
                throw NotFoundException.Instance;
            }

            decodeRowResult.Remove(decodeRowResult.Length - 1, 1);
            decodeRowResult.Remove(0, 1);

            int runningCount = 0;
            for (int i = 0; i < startOffset; i++)
            {
                runningCount += counters[i];
            }
            float left = runningCount;
            for (int i_2 = startOffset; i_2 < nextStart - 1; i_2++)
            {
                runningCount += counters[i_2];
            }
            float right = runningCount;

            ResultPointCallback resultPointCallback = decodingOptions == null || !decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback)
                                          ? null
                                          : (ResultPointCallback)decodingOptions[DecodeOptions.NeedResultPointCallback];
            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(left, rowNumber));
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(right, rowNumber));
            }
            return new Result(
                decodeRowResult.ToString(),
                null,
                new[] { new ResultPoint(left, rowNumber), new ResultPoint(right, rowNumber) },
                BarcodeFormat.Codabar);
        }

        internal void ValidatePattern(int start)
        {
            // First, sum up the total size of our four categories of stripe sizes;
            int[] sizes = { 0, 0, 0, 0 };
            int[] counts = { 0, 0, 0, 0 };
            int end = decodeRowResult.Length - 1;

            // We break out of this loop in the middle, in order to handle
            // inter-character spaces properly.
            int pos = start;
            for (int i = 0; true; i++)
            {
                int pattern = CHARACTER_ENCODINGS[decodeRowResult[i]];
                for (int j = 6; j >= 0; j--)
                {
                    // Even j = bars, while odd j = spaces. Categories 2 and 3 are for
                    // long stripes, while 0 and 1 are for short stripes.
                    int category = (j & 1) + (pattern & 1) * 2;
                    sizes[category] += counters[pos + j];
                    counts[category]++;
                    pattern >>= 1;
                }
                if (i >= end)
                {
                    break;
                }
                // We ignore the inter-character space - it could be of any size.
                pos += 8;
            }

            // Calculate our allowable size thresholds using fixed-point math.
            int[] maxes = new int[4];
            int[] mins = new int[4];
            // Define the threshold of acceptability to be the midpoint between the
            // average small stripe and the average large stripe. No stripe lengths
            // should be on the "wrong" side of that line.
            for (int i_0 = 0; i_0 < 2; i_0++)
            {
                mins[i_0] = 0; // Accept arbitrarily small "short" stripes.
                mins[i_0 + 2] = ((sizes[i_0] << IntegerMathShift) / counts[i_0] + (sizes[i_0 + 2] << IntegerMathShift) / counts[i_0 + 2]) >> 1;
                maxes[i_0] = mins[i_0 + 2];
                maxes[i_0 + 2] = (sizes[i_0 + 2] * MAX_ACCEPTABLE + PADDING) / counts[i_0 + 2];
            }

            // Now verify that all of the stripes are within the thresholds.
            pos = start;
            for (int i_1 = 0; true; i_1++)
            {
                int pattern_2 = CHARACTER_ENCODINGS[decodeRowResult[i_1]];
                for (int j_3 = 6; j_3 >= 0; j_3--)
                {
                    // Even j = bars, while odd j = spaces. Categories 2 and 3 are for
                    // long stripes, while 0 and 1 are for short stripes.
                    int category_4 = (j_3 & 1) + (pattern_2 & 1) * 2;
                    int size = counters[pos + j_3] << IntegerMathShift;
                    if (size < mins[category_4] || size > maxes[category_4])
                    {
                        throw NotFoundException.Instance;
                    }
                    pattern_2 >>= 1;
                }
                if (i_1 >= end)
                {
                    break;
                }
                pos += 8;
            }
        }

        /// <summary>
        /// Records the size of all runs of white and black pixels, starting with white.
        /// This is just like recordPattern, except it records all the counters, and
        /// uses our builtin "counters" member for storage.
        /// </summary>
        ///
        /// <param name="row">row to count from</param>
        private void SetCounters(BitArray row)
        {
            counterLength = 0;
            // Start from the first white bit.
            int i = row.GetNextUnset(0);
            int end = row.GetSize();
            if (i >= end)
            {
                throw NotFoundException.Instance;
            }
            bool isWhite = true;
            int count = 0;
            for (; i < end; i++)
            {
                if (row.Get(i) ^ isWhite)
                { // that is, exactly one is true
                    count++;
                }
                else
                {
                    CounterAppend(count);
                    count = 1;
                    isWhite = !isWhite;
                }
            }
            CounterAppend(count);
        }

        private void CounterAppend(int e)
        {
            counters[counterLength] = e;
            counterLength++;
            if (counterLength >= counters.Length)
            {
                int[] temp = new int[counterLength * 2];
                System.Array.Copy((Array)(counters), 0, (Array)(temp), 0, counterLength);
                counters = temp;
            }
        }

        private int FindStartPattern()
        {
            for (int i = 1; i < counterLength; i += 2)
            {
                int charOffset = ToNarrowWidePattern(i);
                if (charOffset != -1 && ArrayContains(STARTEND_ENCODING, ALPHABET[charOffset]))
                {
                    // Look for whitespace before start pattern, >= 50% of width of start pattern
                    // We make an exception if the whitespace is the first element.
                    int patternSize = 0;
                    for (int j = i; j < i + 7; j++)
                    {
                        patternSize += counters[j];
                    }
                    if (i == 1 || counters[i - 1] >= patternSize / 2)
                    {
                        return i;
                    }
                }
            }
            throw NotFoundException.Instance;
        }

        static internal bool ArrayContains(char[] array, char key)
        {
            if (array != null)
            {
                /* foreach */
                foreach (char c in array)
                {
                    if (c == key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Assumes that counters[position] is a bar.
        private int ToNarrowWidePattern(int position)
        {
            int end = position + 7;
            if (end >= counterLength)
            {
                return -1;
            }

            int[] theCounters = counters;

            int maxBar = 0;
            int minBar = Int32.MaxValue;
            for (int j = position; j < end; j += 2)
            {
                int currentCounter = theCounters[j];
                if (currentCounter < minBar)
                {
                    minBar = currentCounter;
                }
                if (currentCounter > maxBar)
                {
                    maxBar = currentCounter;
                }
            }
            int thresholdBar = (minBar + maxBar) / 2;

            int maxSpace = 0;
            int minSpace = Int32.MaxValue;
            for (int j = position + 1; j < end; j += 2)
            {
                int currentCounter = theCounters[j];
                if (currentCounter < minSpace)
                {
                    minSpace = currentCounter;
                }
                if (currentCounter > maxSpace)
                {
                    maxSpace = currentCounter;
                }
            }
            int thresholdSpace = (minSpace + maxSpace) / 2;

            int bitmask = 1 << 7;
            int pattern = 0;
            for (int i = 0; i < 7; i++)
            {
                int threshold = (i & 1) == 0 ? thresholdBar : thresholdSpace;
                bitmask >>= 1;
                if (theCounters[position + i] > threshold)
                {
                    pattern |= bitmask;
                }
            }

            for (int i = 0; i < CHARACTER_ENCODINGS.Length; i++)
            {
                if (CHARACTER_ENCODINGS[i] == pattern)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
