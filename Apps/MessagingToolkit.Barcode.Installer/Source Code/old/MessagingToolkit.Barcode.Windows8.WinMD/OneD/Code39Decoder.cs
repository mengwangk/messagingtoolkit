using MessagingToolkit.Barcode.Common;
using System;
using System.Text;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// <p>Decodes Code 39 barcodes. This does not support "Full ASCII Code 39" yet.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class Code39Decoder : OneDDecoder
    {

        internal const String AlphabetString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. *$/+%";
        private static readonly char[] Alphabet = AlphabetString.ToCharArray();

        /// <summary>
        /// These represent the encodings of characters, as patterns of wide and narrow bars.
        /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow,
        /// with 1s representing "wide" and 0s representing narrow.
        /// </summary>
        ///
        static internal readonly int[] CharacterEncodings = { 0x034, 0x121, 0x061, 0x160, 0x031, 0x130, 0x070, 0x025, 0x124, 0x064, // 0-9
				0x109, 0x049, 0x148, 0x019, 0x118, 0x058, 0x00D, 0x10C, 0x04C, 0x01C, // A-J
				0x103, 0x043, 0x142, 0x013, 0x112, 0x052, 0x007, 0x106, 0x046, 0x016, // K-T
				0x181, 0x0C1, 0x1C0, 0x091, 0x190, 0x0D0, 0x085, 0x184, 0x0C4, 0x094, // U-*
				0x0A8, 0x0A2, 0x08A, 0x02A // $-%
		};

        private static readonly int AsteriskEncoding = CharacterEncodings[39];

        private readonly bool usingCheckDigit;
        private readonly bool extendedMode;
        private readonly StringBuilder decodeRowResult;
        private readonly int[] counters;

        /// <summary>
        /// Creates a reader that assumes all encoded data is data, and does not treat the final
        /// character as a check digit. It will not decoded "extended Code 39" sequences.
        /// </summary>
        ///
        public Code39Decoder()
            : this(false)
        {
        }

        /// <summary>
        /// Creates a reader that can be configured to check the last character as a check digit.
        /// It will not decoded "extended Code 39" sequences.
        /// </summary>
        ///
        /// <param name="usingCheckDigit"></param>
        public Code39Decoder(bool usingCheckDigit)
            : this(usingCheckDigit, false)
        {

        }

        /// <summary>
        /// Creates a reader that can be configured to check the last character as a check digit,
        /// or optionally attempt to decode "extended Code 39" sequences that are used to encode
        /// the full ASCII character set.
        /// </summary>
        ///
        /// <param name="usingCheckDigit"></param>
        /// <param name="extendedMode"></param>
        public Code39Decoder(bool usingCheckDigit, bool extendedMode)
        {
            this.usingCheckDigit = usingCheckDigit;
            this.extendedMode = extendedMode;
            this.decodeRowResult = new StringBuilder(20);
            this.counters = new int[9];
        }

        public override Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions)
        {
            int[] theCounters = counters;
            for (var index = 0; index < theCounters.Length; index++)
                theCounters[index] = 0;
            StringBuilder result = decodeRowResult;
            result.Length = 0;

            int[] start = FindAsteriskPattern(row, theCounters);
            // Read off white space    
            int nextStart = row.GetNextSet(start[1]);
            int end = row.GetSize();

            char decodedChar;
            int lastStart;
            do
            {
                RecordPattern(row, nextStart, theCounters);
                int pattern = ToNarrowWidePattern(theCounters);
                if (pattern < 0)
                {
                    throw NotFoundException.Instance;
                }
                decodedChar = PatternToChar(pattern);
                result.Append(decodedChar);
                lastStart = nextStart;
                /* foreach */
                foreach (int counter in theCounters)
                {
                    nextStart += counter;
                }
                // Read off white space
                nextStart = row.GetNextSet(nextStart);
            } while (decodedChar != '*');
            result.Length = result.Length - 1; // remove asterisk

            // Look for whitespace after pattern:
            int lastPatternSize = 0;
            /* foreach */
            foreach (int counter in theCounters)
            {
                lastPatternSize += counter;
            }
            int whiteSpaceAfterEnd = nextStart - lastStart - lastPatternSize;
            // If 50% of last pattern size, following last pattern, is not whitespace, fail
            // (but if it's whitespace to the very end of the image, that's OK)
            if (nextStart != end && (whiteSpaceAfterEnd >> 1) < lastPatternSize)
            {
                throw NotFoundException.Instance;
            }

            if (usingCheckDigit)
            {
                int max = result.Length - 1;
                int total = 0;
                for (int i = 0; i < max; i++)
                {
                    total += AlphabetString.IndexOf(decodeRowResult[i]);
                }
                if (result[max] != Alphabet[total % 43])
                {
                    throw ChecksumException.Instance;
                }
                result.Length = max;
            }

            if (result.Length == 0)
            {
                // false positive
                throw NotFoundException.Instance;
            }

            String resultString;
            if (extendedMode)
            {
                resultString = DecodeExtended(result);
            }
            else
            {
                resultString = result.ToString();
            }

            float left = (float)(start[1] + start[0]) / 2.0f;
            float right = (float)(nextStart + lastStart) / 2.0f;

            var resultPointCallback = decodingOptions == null || !decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback)
                                         ? null
                                         : (ResultPointCallback)decodingOptions[DecodeOptions.NeedResultPointCallback];
            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(left, rowNumber));
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(right, rowNumber));
            }
            return new Result(resultString, null, new[] { new ResultPoint(left, rowNumber), new ResultPoint(right, rowNumber) },
                BarcodeFormat.Code39);

        }

        private static int[] FindAsteriskPattern(BitArray row, int[] counters)
        {
            int width = row.GetSize();
            int rowOffset = row.GetNextSet(0);

            int counterPosition = 0;
            int patternStart = rowOffset;
            bool isWhite = false;
            int patternLength = counters.Length;

            for (int i = rowOffset; i < width; i++)
            {
                if (row.Get(i) ^ isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        // Look for whitespace before start pattern, >= 50% of width of start pattern
                        if (ToNarrowWidePattern(counters) == AsteriskEncoding && row.IsRange(Math.Max(0, patternStart - ((i - patternStart) >> 1)), patternStart, false))
                        {
                            return new int[] { patternStart, i };
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

        // For efficiency, returns -1 on failure. Not throwing here saved as many as 700 exceptions
        // per image when using some of our blackbox images.
        private static int ToNarrowWidePattern(int[] counters)
        {
            int numCounters = counters.Length;
            int maxNarrowCounter = 0;
            int wideCounters;
            do
            {
                int minCounter = Int32.MaxValue;
                /* foreach */
                foreach (int counter in counters)
                {
                    if (counter < minCounter && counter > maxNarrowCounter)
                    {
                        minCounter = counter;
                    }
                }
                maxNarrowCounter = minCounter;
                wideCounters = 0;
                int totalWideCountersWidth = 0;
                int pattern = 0;
                for (int i = 0; i < numCounters; i++)
                {
                    int counter = counters[i];
                    if (counter > maxNarrowCounter)
                    {
                        pattern |= 1 << (numCounters - 1 - i);
                        wideCounters++;
                        totalWideCountersWidth += counter;
                    }
                }
                if (wideCounters == 3)
                {
                    // Found 3 wide counters, but are they close enough in width?
                    // We can perform a cheap, conservative check to see if any individual
                    // counter is more than 1.5 times the average:
                    for (int i = 0; i < numCounters && wideCounters > 0; i++)
                    {
                        int counter = counters[i];
                        if (counter > maxNarrowCounter)
                        {
                            wideCounters--;
                            // totalWideCountersWidth = 3 * average, so this checks if counter >= 3/2 * average
                            if ((counter << 1) >= totalWideCountersWidth)
                            {
                                return -1;
                            }
                        }
                    }
                    return pattern;
                }
            } while (wideCounters > 3);
            return -1;
        }

        private static char PatternToChar(int pattern)
        {
            for (int i = 0; i < CharacterEncodings.Length; i++)
            {
                if (CharacterEncodings[i] == pattern)
                {
                    return Alphabet[i];
                }
            }
            throw NotFoundException.Instance;
        }

        private static String DecodeExtended(StringBuilder encoded)
        {
            int length = encoded.Length;
            StringBuilder decoded = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                char c = encoded[i];
                if (c == '+' || c == '$' || c == '%' || c == '/')
                {
                    char next = encoded[i + 1];
                    char decodedChar = '\0';
                    switch ((int)c)
                    {
                        case '+':
                            // +A to +Z map to a to z
                            if (next >= 'A' && next <= 'Z')
                            {
                                decodedChar = (char)(next + 32);
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                        case '$':
                            // $A to $Z map to control codes SH to SB
                            if (next >= 'A' && next <= 'Z')
                            {
                                decodedChar = (char)(next - 64);
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                        case '%':
                            // %A to %E map to control codes ESC to US
                            if (next >= 'A' && next <= 'E')
                            {
                                decodedChar = (char)(next - 38);
                            }
                            else if (next >= 'F' && next <= 'W')
                            {
                                decodedChar = (char)(next - 11);
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                        case '/':
                            // /A to /O map to ! to , and /Z maps to :
                            if (next >= 'A' && next <= 'O')
                            {
                                decodedChar = (char)(next - 32);
                            }
                            else if (next == 'Z')
                            {
                                decodedChar = ':';
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                    }
                    decoded.Append(decodedChar);
                    // bump up i again since we read two characters
                    i++;
                }
                else
                {
                    decoded.Append(c);
                }
            }
            return decoded.ToString();
        }

    }
}
