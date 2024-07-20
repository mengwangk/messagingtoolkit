﻿using MessagingToolkit.Barcode.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// This object renders a CODE128 code.
    /// 
    /// Modified: April 18 2012
    /// </summary>
    internal sealed class Code128Encoder : OneDEncoder
    {
        private const int CODE_START_B = 104;
        private const int CODE_START_C = 105;
        private const int CODE_CODE_B = 100;
        private const int CODE_CODE_C = 99;
        private const int CODE_STOP = 106;

        // Dummy characters used to specify control characters in input
        private const char ESCAPE_FNC_1 = '\u00f1';
        private const char ESCAPE_FNC_2 = '\u00f2';
        private const char ESCAPE_FNC_3 = '\u00f3';
        private const char ESCAPE_FNC_4 = '\u00f4';

        private const int CODE_FNC_1 = 102; // Code A, Code B, Code C
        private const int CODE_FNC_2 = 97; // Code A, Code B
        private const int CODE_FNC_3 = 96; // Code A, Code B
        private const int CODE_FNC_4_B = 100; // Code B


        public override BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {
            if (format != BarcodeFormat.Code128)
            {
                throw new ArgumentException("Can only Encode CODE_128, but got " + format);
            }
            return base.Encode(contents, format, width, height, encodingOptions);
        }

        public override bool[] Encode(String contents)
        {
            int length = contents.Length;
            // Check length
            if (length < 1 || length > 80)
            {
                throw new ArgumentException("Contents length should be between 1 and 80 characters, but got " + length);
            }
            // Check content
            for (int i = 0; i < length; i++)
            {
                char c = contents[i];
                if (c < ' ' || c > '~')
                {
                    switch ((int)c)
                    {
                        case ESCAPE_FNC_1:
                        case ESCAPE_FNC_2:
                        case ESCAPE_FNC_3:
                        case ESCAPE_FNC_4:
                            break;
                        default:
                            throw new ArgumentException("Bad character in input: " + c);
                    }
                }
            }

            ICollection<int[]> patterns = new List<int[]>(); // temporary storage for patterns
            int checkSum = 0;
            int checkWeight = 1;
            int codeSet = 0; // selected code (CODE_CODE_B or CODE_CODE_C)
            int position = 0; // position in contents

            while (position < length)
            {
                //Select code to use
                int requiredDigitCount = (codeSet == CODE_CODE_C) ? 2 : 4;
                int newCodeSet;
                if (IsDigits(contents, position, requiredDigitCount))
                {
                    newCodeSet = CODE_CODE_C;
                }
                else
                {
                    newCodeSet = CODE_CODE_B;
                }

                //Get the pattern index
                int patternIndex;
                if (newCodeSet == codeSet)
                {
                    // Encode the current character
                    if (codeSet == CODE_CODE_B)
                    {
                        patternIndex = contents[position] - ' ';
                        position += 1;
                    }
                    else
                    { // CODE_CODE_C
                        switch ((int)contents[position])
                        {
                            case ESCAPE_FNC_1:
                                patternIndex = CODE_FNC_1;
                                position++;
                                break;
                            case ESCAPE_FNC_2:
                                patternIndex = CODE_FNC_2;
                                position++;
                                break;
                            case ESCAPE_FNC_3:
                                patternIndex = CODE_FNC_3;
                                position++;
                                break;
                            case ESCAPE_FNC_4:
                                patternIndex = CODE_FNC_4_B; // FIXME if this ever outputs Code A
                                position++;
                                break;
                            default:
                                patternIndex = Int32.Parse(contents.Substring(position, (position + 2) - (position)));
                                position += 2;
                                break;
                        }
                    }
                }
                else
                {
                    // Should we change the current code?
                    // Do we have a code set?
                    if (codeSet == 0)
                    {
                        // No, we don't have a code set
                        if (newCodeSet == CODE_CODE_B)
                        {
                            patternIndex = CODE_START_B;
                        }
                        else
                        {
                            // CODE_CODE_C
                            patternIndex = CODE_START_C;
                        }
                    }
                    else
                    {
                        // Yes, we have a code set
                        patternIndex = newCodeSet;
                    }
                    codeSet = newCodeSet;
                }

                // Get the pattern
                patterns.Add(MessagingToolkit.Barcode.OneD.Code128Decoder.CODE_PATTERNS[patternIndex]);

                // Compute checksum
                checkSum += patternIndex * checkWeight;
                if (position != 0)
                {
                    checkWeight++;
                }
            }

            // Compute and append checksum
            checkSum %= 103;
            patterns.Add(MessagingToolkit.Barcode.OneD.Code128Decoder.CODE_PATTERNS[checkSum]);

            // Append stop code
            patterns.Add(MessagingToolkit.Barcode.OneD.Code128Decoder.CODE_PATTERNS[CODE_STOP]);

            // Compute code width
            int codeWidth = 0;
            /* foreach */
            foreach (int[] pattern in patterns)
            {
                /* foreach */
                foreach (int width in pattern)
                {
                    codeWidth += width;
                }
            }

            // Compute result
            bool[] result = new bool[codeWidth];
            int pos = 0;
            /* foreach */
            foreach (int[] pat in patterns)
            {
                pos += MessagingToolkit.Barcode.OneD.UPCEANEncoder.AppendPattern(result, pos, pat, true);
            }

            return result;
        }

        private static bool IsDigits(String val, int start, int length)
        {
            int end = start + length;
            int last = val.Length;
            for (int i = start; i < end && i < last; i++)
            {
                char c = val[i];
                if (c < '0' || c > '9')
                {
                    if (c != ESCAPE_FNC_1)
                    {
                        return false;
                    }
                    end++; // ignore FNC_1
                }
            }
            return end <= last; // end > last if we've run out of string
        }
    }
}
