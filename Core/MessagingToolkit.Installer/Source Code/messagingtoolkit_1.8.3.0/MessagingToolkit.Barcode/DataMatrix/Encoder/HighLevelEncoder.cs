using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{

    /// <summary>
    /// DataMatrix ECC 200 data encoder following the algorithm described in ISO/IEC 16022:200(E) in
    /// annex S.
    /// </summary>
    public sealed class HighLevelEncoder
    {
        /// <summary>
        /// Padding character </summary>
        internal static char PAD = (char)129;
        /// <summary>
        /// mode latch to C40 encodation mode </summary>
        internal static char LATCH_TO_C40 = (char)230;
        /// <summary>
        /// mode latch to Base 256 encodation mode </summary>
        internal static char LATCH_TO_BASE256 = (char)231;
        /// <summary>
        /// FNC1 Codeword </summary>
        //internal static char FNC1 = (char)232;
        /// <summary>
        /// Structured Append Codeword </summary>
        //internal static char STRUCTURED_APPEND = (char)233;
        /// <summary>
        /// Reader Programming </summary>
        //internal static char READER_PROGRAMMING = (char)234;
        /// <summary>
        /// Upper Shift </summary>
        internal static char UPPER_SHIFT = (char)235;
        /// <summary>
        /// 05 Macro </summary>
        internal static char MACRO_05 = (char)236;
        /// <summary>
        /// 06 Macro </summary>
        internal static char MACRO_06 = (char)237;
        /// <summary>
        /// mode latch to ANSI X.12 encodation mode </summary>
        internal static char LATCH_TO_ANSIX12 = (char)238;
        /// <summary>
        /// mode latch to Text encodation mode </summary>
        internal static char LATCH_TO_TEXT = (char)239;
        /// <summary>
        /// mode latch to EDIFACT encodation mode </summary>
        internal static char LATCH_TO_EDIFACT = (char)240;
        /// <summary>
        /// ECI character (Extended Channel Interpretation) </summary>
        internal static char ECI = (char)241;

        /// <summary>
        /// Unlatch from C40 encodation </summary>
        internal static char C40_UNLATCH = (char)254;
        /// <summary>
        /// Unlatch from X12 encodation </summary>
        internal static char X12_UNLATCH = (char)254;

        /// <summary>
        /// 05 Macro header </summary>
        internal static string MACRO_05_HEADER = "[)>\u001E05\u001D";
        /// <summary>
        /// 06 Macro header </summary>
        internal static string MACRO_06_HEADER = "[)>\u001E06\u001D";
        /// <summary>
        /// Macro trailer </summary>
        internal static string MACRO_TRAILER = "\u001E\u0004";


        internal const int ASCII_ENCODATION = 0;
        internal const int C40_ENCODATION = 1;
        internal const int TEXT_ENCODATION = 2;
        internal const int X12_ENCODATION = 3;
        internal const int EDIFACT_ENCODATION = 4;
        internal const int BASE256_ENCODATION = 5;

        private HighLevelEncoder()
        {
        }

        /// <summary>
        /// Converts the message to a byte array using the default encoding (cp437) as defined by the
        /// specification
        /// </summary>
        /// <param name="msg"> the message </param>
        /// <returns> the byte array of the message </returns>
        public static byte[] GetBytesForMessage(string msg)
        {
            const string charset = "CP437"; // See 4.4.3 and annex B of ISO/IEC 15438:2001(E)
            try
            {
                return Encoding.GetEncoding(charset).GetBytes(msg);
            }
            catch (ArgumentException e)
            {
                throw new NotSupportedException("Incompatible environment. The '" + charset + "' charset is not available!");
            }
        }

        private static char Randomize253State(char ch, int codewordPosition)
        {
            int pseudoRandom = ((149 * codewordPosition) % 253) + 1;
            int tempVariable = ch + pseudoRandom;
            return tempVariable <= 254 ? (char)tempVariable : (char)(tempVariable - 254);
        }

        private static char Randomize255State(char ch, int codewordPosition)
        {
            int pseudoRandom = ((149 * codewordPosition) % 255) + 1;
            int tempVariable = ch + pseudoRandom;
            if (tempVariable <= 255)
            {
                return (char)tempVariable;
            }
            else
            {
                return (char)(tempVariable - 256);
            }
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E).
        /// </summary>
        /// <param name="msg"> the message </param>
        /// <returns> the encoded message (the char values range from 0 to 255) </returns>
        public static string EncodeHighLevel(string msg)
        {
            return EncodeHighLevel(msg, SymbolShapeHint.ForceNone, null, null, null);
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E).
        /// </summary>
        /// <param name="msg">     the message </param>
        /// <param name="shape">   requested shape. May be SymbolShapeHint.ForceNone,
        ///                SymbolShapeHint.ForceSquare} or SymbolShapeHint.ForceRectangle}. </param>
        /// <param name="minSize"> the minimum symbol size constraint or null for no constraint </param>
        /// <param name="maxSize"> the maximum symbol size constraint or null for no constraint </param>
        /// <returns> the encoded message (the char values range from 0 to 255) </returns>
        public static string EncodeHighLevel(string msg, SymbolShapeHint shape, Dimension minSize, Dimension maxSize, Dictionary<EncodeOptions, object> encodingOptions)
        {
            //the codewords 0..255 are encoded as Unicode characters
            Encoder[] encoders = { new ASCIIEncoder(), new C40Encoder(), new TextEncoder(), new X12Encoder(), new EdifactEncoder(), new Base256Encoder() };

            EncoderContext context = new EncoderContext(msg);
            context.SymbolShape = shape;
            context.SetSizeConstraints(minSize, maxSize);

            if (msg.StartsWith(MACRO_05_HEADER) && msg.EndsWith(MACRO_TRAILER))
            {
                context.WriteCodeword(MACRO_05);
                context.SkipAtEnd = 2;
                context.pos += MACRO_05_HEADER.Length;
            }
            else if (msg.StartsWith(MACRO_06_HEADER) && msg.EndsWith(MACRO_TRAILER))
            {
                context.WriteCodeword(MACRO_06);
                context.SkipAtEnd = 2;
                context.pos += MACRO_06_HEADER.Length;
            }

            int encodingMode = ASCII_ENCODATION; //Default mode
            while (context.HasMoreCharacters())
            {
                encoders[encodingMode].Encode(context);
                if (context.NewEncoding >= 0)
                {
                    encodingMode = context.NewEncoding;
                    context.ResetEncoderSignal();
                }
            }
            int len = context.Codewords.Length;
            context.UpdateSymbolInfo();
            int capacity = context.SymbolInfo.DataCapacity;
            if (len < capacity)
            {
                if (encodingMode != ASCII_ENCODATION && encodingMode != BASE256_ENCODATION)
                {
                    context.WriteCodeword('\u00fe'); //Unlatch (254)
                }
            }
            //Padding
            StringBuilder codewords = context.Codewords;
            if (codewords.Length < capacity)
            {
                codewords.Append(PAD);
            }
            while (codewords.Length < capacity)
            {
                codewords.Append(Randomize253State(PAD, codewords.Length + 1));
            }

            return context.Codewords.ToString();
        }

        internal static int LookAheadTest(string msg, int startpos, int currentMode)
        {
            if (startpos >= msg.Length)
            {
                return currentMode;
            }
            float[] charCounts;
            //step J
            if (currentMode == ASCII_ENCODATION)
            {
                charCounts = new float[] { 0, 1, 1, 1, 1, 1.25f };
            }
            else
            {
                charCounts = new float[] { 1, 2, 2, 2, 2, 2.25f };
                charCounts[currentMode] = 0;
            }

            int charsProcessed = 0;
            while (true)
            {
                //step K
                if ((startpos + charsProcessed) == msg.Length)
                {
                    int min = int.MaxValue;
                    sbyte[] mins = new sbyte[6];
                    int[] intCharCounts = new int[6];
                    min = FindMinimums(charCounts, intCharCounts, min, mins);
                    int minCount = GetMinimumCount(mins);

                    if (intCharCounts[ASCII_ENCODATION] == min)
                    {
                        return ASCII_ENCODATION;
                    }
                    if (minCount == 1 && mins[BASE256_ENCODATION] > 0)
                    {
                        return BASE256_ENCODATION;
                    }
                    if (minCount == 1 && mins[EDIFACT_ENCODATION] > 0)
                    {
                        return EDIFACT_ENCODATION;
                    }
                    if (minCount == 1 && mins[TEXT_ENCODATION] > 0)
                    {
                        return TEXT_ENCODATION;
                    }
                    if (minCount == 1 && mins[X12_ENCODATION] > 0)
                    {
                        return X12_ENCODATION;
                    }
                    return C40_ENCODATION;
                }

                char c = msg[startpos + charsProcessed];
                charsProcessed++;

                //step L
                if (IsDigit(c))
                {
                    charCounts[ASCII_ENCODATION] += 0.5f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[ASCII_ENCODATION] = (int)Math.Ceiling(charCounts[ASCII_ENCODATION]);
                    charCounts[ASCII_ENCODATION] += 2;
                }
                else
                {
                    charCounts[ASCII_ENCODATION] = (int)Math.Ceiling(charCounts[ASCII_ENCODATION]);
                    charCounts[ASCII_ENCODATION]++;
                }

                //step M
                if (IsNativeC40(c))
                {
                    charCounts[C40_ENCODATION] += 2.0f / 3.0f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[C40_ENCODATION] += 8.0f / 3.0f;
                }
                else
                {
                    charCounts[C40_ENCODATION] += 4.0f / 3.0f;
                }

                //step N
                if (IsNativeText(c))
                {
                    charCounts[TEXT_ENCODATION] += 2.0f / 3.0f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[TEXT_ENCODATION] += 8.0f / 3.0f;
                }
                else
                {
                    charCounts[TEXT_ENCODATION] += 4.0f / 3.0f;
                }

                //step O
                if (IsNativeX12(c))
                {
                    charCounts[X12_ENCODATION] += 2.0f / 3.0f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[X12_ENCODATION] += 13.0f / 3.0f;
                }
                else
                {
                    charCounts[X12_ENCODATION] += 10.0f / 3.0f;
                }

                //step P
                if (IsNativeEDIFACT(c))
                {
                    charCounts[EDIFACT_ENCODATION] += 3.0f / 4.0f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[EDIFACT_ENCODATION] += 17.0f / 4.0f;
                }
                else
                {
                    charCounts[EDIFACT_ENCODATION] += 13.0f / 4.0f;
                }

                // step Q
                if (IsSpecialB256(c))
                {
                    charCounts[BASE256_ENCODATION] += 4;
                }
                else
                {
                    charCounts[BASE256_ENCODATION]++;
                }

                //step R
                if (charsProcessed >= 4)
                {
                    int[] intCharCounts = new int[6];
                    sbyte[] mins = new sbyte[6];
                    FindMinimums(charCounts, intCharCounts, int.MaxValue, mins);
                    int minCount = GetMinimumCount(mins);

                    if (intCharCounts[ASCII_ENCODATION] < intCharCounts[BASE256_ENCODATION] && intCharCounts[ASCII_ENCODATION] < intCharCounts[C40_ENCODATION] && intCharCounts[ASCII_ENCODATION] < intCharCounts[TEXT_ENCODATION] && intCharCounts[ASCII_ENCODATION] < intCharCounts[X12_ENCODATION] && intCharCounts[ASCII_ENCODATION] < intCharCounts[EDIFACT_ENCODATION])
                    {
                        return ASCII_ENCODATION;
                    }
                    if (intCharCounts[BASE256_ENCODATION] < intCharCounts[ASCII_ENCODATION] || (mins[C40_ENCODATION] + mins[TEXT_ENCODATION] + mins[X12_ENCODATION] + mins[EDIFACT_ENCODATION]) == 0)
                    {
                        return BASE256_ENCODATION;
                    }
                    if (minCount == 1 && mins[EDIFACT_ENCODATION] > 0)
                    {
                        return EDIFACT_ENCODATION;
                    }
                    if (minCount == 1 && mins[TEXT_ENCODATION] > 0)
                    {
                        return TEXT_ENCODATION;
                    }
                    if (minCount == 1 && mins[X12_ENCODATION] > 0)
                    {
                        return X12_ENCODATION;
                    }
                    if (intCharCounts[C40_ENCODATION] + 1 < intCharCounts[ASCII_ENCODATION] && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[BASE256_ENCODATION] && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[EDIFACT_ENCODATION] && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[TEXT_ENCODATION])
                    {
                        if (intCharCounts[C40_ENCODATION] < intCharCounts[X12_ENCODATION])
                        {
                            return C40_ENCODATION;
                        }
                        if (intCharCounts[C40_ENCODATION] == intCharCounts[X12_ENCODATION])
                        {
                            int p = startpos + charsProcessed + 1;
                            while (p < msg.Length)
                            {
                                char tc = msg[p];
                                if (IsX12TermSep(tc))
                                {
                                    return X12_ENCODATION;
                                }
                                if (!IsNativeX12(tc))
                                {
                                    break;
                                }
                                p++;
                            }
                            return C40_ENCODATION;
                        }
                    }
                }
            }
        }

        private static int FindMinimums(float[] charCounts, int[] intCharCounts, int min, sbyte[] mins)
        {
            for (int i = 0; i < mins.Length; i++)
                mins[i] = 0;
            for (int i = 0; i < 6; i++)
            {
                intCharCounts[i] = (int)Math.Ceiling(charCounts[i]);
                int current = intCharCounts[i];
                if (min > current)
                {
                    min = current;
                    for (int j = 0; j < mins.Length; j++)
                        mins[j] = 0;
                }
                if (min == current)
                {
                    mins[i]++;

                }
            }
            return min;
        }

        private static int GetMinimumCount(sbyte[] mins)
        {
            int minCount = 0;
            for (int i = 0; i < 6; i++)
            {
                minCount += mins[i];
            }
            return minCount;
        }

        internal static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        internal static bool IsExtendedASCII(char ch)
        {
            return ch >= 128 && ch <= 255;
        }

        private static bool IsASCII7(char ch)
        {
            return (ch >= 0 && ch <= 127);
        }

        private static bool IsNativeC40(char ch)
        {
            return (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z');
        }

        private static bool IsNativeText(char ch)
        {
            return (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z');
        }

        private static bool IsNativeX12(char ch)
        {
            return IsX12TermSep(ch) || (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z');
        }

        private static bool IsX12TermSep(char ch)
        {
            return (ch == '\r') || (ch == '*') || (ch == '>'); //CR
        }

        private static bool IsNativeEDIFACT(char ch)
        {
            return ch >= ' ' && ch <= '^';
        }

        private static bool IsSpecialB256(char ch)
        {
            return false; // TODO NOT IMPLEMENTED YET!!!
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using numeric compaction.
        /// </summary>
        /// <param name="msg">the message </param>
        /// <param name="startpos">the start position within the message </param>
        /// <returns> the requested character count </returns>
        public static int DetermineConsecutiveDigitCount(string msg, int startpos)
        {
            int count = 0;
            int len = msg.Length;
            int idx = startpos;
            if (idx < len)
            {
                char ch = msg[idx];
                while (IsDigit(ch) && idx < len)
                {
                    count++;
                    idx++;
                    if (idx < len)
                    {
                        ch = msg[idx];
                    }
                }
            }
            return count;
        }

        internal static void IllegalCharacter(char c)
        {
            string hex = (Convert.ToInt32(c)).ToString("X");
            hex = "0000".Substring(0, 4 - hex.Length) + hex;
            throw new ArgumentException("Illegal character: " + c + " (0x" + hex + ')');
        }

    }
}
