using MessagingToolkit.Barcode.Common;
using System;
using System.Text;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// <p>Decodes Code 93 barcodes.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public sealed class Code93Decoder : OneDDecoder
    {


        // Note that 'abcd' are dummy characters in place of control characters.
        private const String AlphabetString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%abcd*";
        private static readonly char[] Alphabet = AlphabetString.ToCharArray();

        /// <summary>
        /// These represent the encodings of characters, as patterns of wide and narrow bars.
        /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow.
        /// </summary>
        ///
        private static readonly int[] CharacterEncodings = { 0x114, 0x148, 0x144, 0x142, 0x128, 0x124, 0x122, 0x150, 0x112, 0x10A, // 0-9
				0x1A8, 0x1A4, 0x1A2, 0x194, 0x192, 0x18A, 0x168, 0x164, 0x162, 0x134, // A-J
				0x11A, 0x158, 0x14C, 0x146, 0x12C, 0x116, 0x1B4, 0x1B2, 0x1AC, 0x1A6, // K-T
				0x196, 0x19A, 0x16C, 0x166, 0x136, 0x13A, // U-Z
				0x12E, 0x1D4, 0x1D2, 0x1CA, 0x16E, 0x176, 0x1AE, // - - %
				0x126, 0x1DA, 0x1D6, 0x132, 0x15E, // Control chars? $-*
		};
        private static readonly int AsteriskEncoding = CharacterEncodings[47];
        private readonly StringBuilder decodeRowResult;
        private readonly int[] counters;

        public Code93Decoder()
        {
            this.decodeRowResult = new StringBuilder(20);
            this.counters = new int[6];
        }

        public override Result DecodeRow(int rowNumber, BitArray row, Dictionary<DecodeOptions, object> decodingOptions)
        {

            int[] start = FindAsteriskPattern(row);

            // Read off white space    
            int nextStart = row.GetNextSet(start[1]);
            int end = row.GetSize();

            int[] theCounters = this.counters;
            for (var index = 0; index < theCounters.Length; index++)
                theCounters[index] = 0;
            StringBuilder result = decodeRowResult;
            result.Length = 0;

            char decodedChar;
            int lastStart;
            do
            {
                MessagingToolkit.Barcode.OneD.OneDDecoder.RecordPattern(row, nextStart, theCounters);
                int pattern = ToPattern(theCounters);
                if (pattern < 0)
                {
                    throw NotFoundException.Instance; ;
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
            result.Remove(result.Length - 1, 1); // remove asterisk

            // Should be at least one more black module
            if (nextStart == end || !row.Get(nextStart))
            {
                throw NotFoundException.Instance; ;
            }

            if (result.Length < 2)
            {
                // false positive -- need at least 2 checksum digits
                throw NotFoundException.Instance; ;
            }

            CheckChecksums(result);
            // Remove checksum digits
            result.Length = result.Length - 2;

            String resultString = DecodeExtended(result);

            float left = (start[1] + start[0]) / 2.0f;
            float right = (nextStart + lastStart) / 2.0f;

            var resultPointCallback = decodingOptions == null || !decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback)
                                         ? null
                                         : (ResultPointCallback)decodingOptions[DecodeOptions.NeedResultPointCallback];
            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(left, rowNumber));
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(right, rowNumber));
            }

            return new Result(resultString, null, new ResultPoint[] { new ResultPoint(left, rowNumber), new ResultPoint(right, rowNumber) }, BarcodeFormat.Code93);

        }

        private int[] FindAsteriskPattern(BitArray row)
        {
            int width = row.GetSize();
            int rowOffset = row.GetNextSet(0);

            for (var index = 0; index < counters.Length; index++)
                counters[index] = 0;
            int[] theCounters = counters;

            int patternStart = rowOffset;
            bool isWhite = false;
            int patternLength = theCounters.Length;
            int counterPosition = 0;

            for (int i = rowOffset; i < width; i++)
            {
                if (row.Get(i) ^ isWhite)
                {
                    theCounters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        if (ToPattern(theCounters) == AsteriskEncoding)
                        {
                            return new int[] { patternStart, i };
                        }
                        patternStart += theCounters[0] + theCounters[1];
                        System.Array.Copy((Array)(theCounters), 2, (Array)(theCounters), 0, patternLength - 2);
                        theCounters[patternLength - 2] = 0;
                        theCounters[patternLength - 1] = 0;
                        counterPosition--;
                    }
                    else
                    {
                        counterPosition++;
                    }
                    theCounters[counterPosition] = 1;
                    isWhite = !isWhite;
                }
            }
            throw NotFoundException.Instance;
        }

        private static int ToPattern(int[] counters)
        {
            int max = counters.Length;
            int sum = 0;
            /* foreach */
            foreach (int counter in counters)
            {
                sum += counter;
            }
            int pattern = 0;
            for (int i = 0; i < max; i++)
            {
                int scaledShifted = (counters[i] << MessagingToolkit.Barcode.OneD.OneDDecoder.IntegerMathShift) * 9 / sum;
                int scaledUnshifted = scaledShifted >> MessagingToolkit.Barcode.OneD.OneDDecoder.IntegerMathShift;
                if ((scaledShifted & 0xFF) > 0x7F)
                {
                    scaledUnshifted++;
                }
                if (scaledUnshifted < 1 || scaledUnshifted > 4)
                {
                    return -1;
                }
                if ((i & 0x01) == 0)
                {
                    for (int j = 0; j < scaledUnshifted; j++)
                    {
                        pattern = (pattern << 1) | 0x01;
                    }
                }
                else
                {
                    pattern <<= scaledUnshifted;
                }
            }
            return pattern;
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
                if (c >= 'a' && c <= 'd')
                {
                    if (i >= length - 1)
                    {
                        throw FormatException.Instance;
                    }
                    char next = encoded[i + 1];
                    char decodedChar = '\0';
                    switch ((int)c)
                    {
                        case 'd':
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
                        case 'a':
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
                        case 'b':
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
                        case 'c':
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

        private static void CheckChecksums(StringBuilder result)
        {
            int length = result.Length;
            CheckOneChecksum(result, length - 2, 20);
            CheckOneChecksum(result, length - 1, 15);
        }

        private static void CheckOneChecksum(StringBuilder result, int checkPosition, int weightMax)
        {
            int weight = 1;
            int total = 0;
            for (int i = checkPosition - 1; i >= 0; i--)
            {
                total += weight * AlphabetString.IndexOf(result[i]);
                if (++weight > weightMax)
                {
                    weight = 1;
                }
            }
            if (result[checkPosition] != Alphabet[total % 47])
            {
                throw ChecksumException.Instance;
            }
        }

    }
}
