using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

#if !NET35 && !WINDOWS_PHONE
using System.Numerics;
#else
using MessagingToolkit.Barcode.Common.Maths;
#endif

namespace MessagingToolkit.Barcode.Pdf417.Encoder
{

    /// <summary>
    /// PDF417 high-level encoder following the algorithm described in ISO/IEC 15438:2001(E) in
    /// annex P.
    /// </summary>
    internal sealed class Pdf417HighLevelEncoder
    {

        /// <summary>
        /// code for Text compaction
        /// </summary>
        ///
        private const int TEXT_COMPACTION = 0;

        /// <summary>
        /// code for Byte compaction
        /// </summary>
        ///
        private const int BYTE_COMPACTION = 1;

        /// <summary>
        /// code for Numeric compaction
        /// </summary>
        ///
        private const int NUMERIC_COMPACTION = 2;

        /// <summary>
        /// Text compaction submode Alpha
        /// </summary>
        ///
        private const int SUBMODE_ALPHA = 0;

        /// <summary>
        /// Text compaction submode Lower
        /// </summary>
        ///
        private const int SUBMODE_LOWER = 1;

        /// <summary>
        /// Text compaction submode Mixed
        /// </summary>
        ///
        private const int SUBMODE_MIXED = 2;

        /// <summary>
        /// Text compaction submode Punctuation
        /// </summary>
        ///
        private const int SUBMODE_PUNCTUATION = 3;

        /// <summary>
        /// mode latch to Text Compaction mode
        /// </summary>
        ///
        private const int LATCH_TO_TEXT = 900;

        /// <summary>
        /// mode latch to Byte Compaction mode (number of characters NOT a multiple of 6)
        /// </summary>
        ///
        private const int LATCH_TO_BYTE_PADDED = 901;

        /// <summary>
        /// mode latch to Numeric Compaction mode
        /// </summary>
        ///
        private const int LATCH_TO_NUMERIC = 902;

        /// <summary>
        /// mode shift to Byte Compaction mode
        /// </summary>
        ///
        private const int SHIFT_TO_BYTE = 913;

        /// <summary>
        /// mode latch to Byte Compaction mode (number of characters a multiple of 6)
        /// </summary>
        ///
        private const int LATCH_TO_BYTE = 924;

        /// <summary>
        /// Raw code table for text compaction Mixed sub-mode
        /// </summary>
        ///
        private static readonly sbyte[] TEXT_MIXED_RAW = { 48, 49, 50, 51, 52, 53, 54,
				55, 56, 57, 38, 13, 9, 44, 58, 35, 45, 46, 36, 47, 43, 37, 42, 61,
				94, 0, 32, 0, 0, 0 };

        /// <summary>
        /// Raw code table for text compaction: Punctuation sub-mode
        /// </summary>
        ///
        private static readonly sbyte[] TEXT_PUNCTUATION_RAW = { 59, 60, 62, 64, 91,
				92, 93, 95, 96, 126, 33, 13, 9, 44, 58, 10, 45, 46, 36, 47, 34,
				124, 42, 40, 41, 63, 123, 125, 39, 0 };

        private static readonly sbyte[] MIXED = new sbyte[128];
        private static readonly sbyte[] PUNCTUATION = new sbyte[128];

        private Pdf417HighLevelEncoder()
        {
        }

        /// <summary>
        /// Converts the message to a byte array using the default encoding (cp437) as defined by the
        /// specification
        /// </summary>
        ///
        /// <param name="msg">the message</param>
        /// <returns>the byte array of the message</returns>
        private static byte[] GetBytesForMessage(String msg)
        {
            return Encoding.GetEncoding("CP437").GetBytes(msg);
        }

        /// <summary>
        /// Performs high-level encoding of a PDF417 message using the algorithm described in annex P
        /// of ISO/IEC 15438:2001(E). If byte compaction has been selected, then only byte compaction
        /// is used.
        /// </summary>
        ///
        /// <param name="msg">the message</param>
        /// <returns>the encoded message (the char values range from 0 to 928)</returns>
        static internal String EncodeHighLevel(String msg, Compaction compaction)
        {
            byte[] bytes = null; //Fill later and only if needed

            //the codewords 0..928 are encoded as Unicode characters
            StringBuilder sb = new StringBuilder(msg.Length);

            int len = msg.Length;
            int p = 0;
            //int encodingMode = TEXT_COMPACTION; //Default mode, see 4.4.2.1
            int textSubMode = SUBMODE_ALPHA;

            // User selected encoding mode
            if (compaction == Compaction.Text)
            {
                EncodeText(msg, p, len, sb, textSubMode);

            }
            else if (compaction == Compaction.Byte)
            {
                //encodingMode = BYTE_COMPACTION;
                bytes = GetBytesForMessage(msg);
                EncodeBinary(bytes, p, bytes.Length, BYTE_COMPACTION, sb);

            }
            else if (compaction == Compaction.Numeric)
            {
                //encodingMode = NUMERIC_COMPACTION;
                sb.Append((char)LATCH_TO_NUMERIC);
                EncodeNumeric(msg, p, len, sb);

            }
            else
            {
                int encodingMode = TEXT_COMPACTION; //Default mode, see 4.4.2.1
                while (p < len)
                {
                    int n = DetermineConsecutiveDigitCount(msg, p);
                    if (n >= 13)
                    {
                        sb.Append((char)LATCH_TO_NUMERIC);
                        encodingMode = NUMERIC_COMPACTION;
                        textSubMode = SUBMODE_ALPHA; //Reset after latch
                        EncodeNumeric(msg, p, n, sb);
                        p += n;
                    }
                    else
                    {
                        int t = DetermineConsecutiveTextCount(msg, p);
                        if (t >= 5 || n == len)
                        {
                            if (encodingMode != TEXT_COMPACTION)
                            {
                                sb.Append((char)LATCH_TO_TEXT);
                                encodingMode = TEXT_COMPACTION;
                                textSubMode = SUBMODE_ALPHA; //start with submode alpha after latch
                            }
                            textSubMode = EncodeText(msg, p, t, sb, textSubMode);
                            p += t;
                        }
                        else
                        {
                            if (bytes == null)
                            {
                                bytes = GetBytesForMessage(msg);
                            }
                            int b = DetermineConsecutiveBinaryCount(msg, bytes, p);
                            if (b == 0)
                            {
                                b = 1;
                            }
                            if (b == 1 && encodingMode == TEXT_COMPACTION)
                            {
                                //Switch for one byte (instead of latch)
                                EncodeBinary(bytes, p, 1, TEXT_COMPACTION, sb);
                            }
                            else
                            {
                                //Mode latch performed by encodeBinary()
                                EncodeBinary(bytes, p, b, encodingMode, sb);
                                encodingMode = BYTE_COMPACTION;
                                textSubMode = SUBMODE_ALPHA; //Reset after latch
                            }
                            p += b;
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Encode parts of the message using Text Compaction as described in ISO/IEC 15438:2001(E),
        /// chapter 4.4.2.
        /// </summary>
        ///
        /// <param name="msg">the message</param>
        /// <param name="startpos">the start position within the message</param>
        /// <param name="count">the number of characters to encode</param>
        /// <param name="sb">receives the encoded codewords</param>
        /// <param name="initialSubmode">should normally be SUBMODE_ALPHA</param>
        /// <returns>the text submode in which this method ends</returns>
        private static int EncodeText(String msg, int startpos, int count,
                StringBuilder sb, int initialSubmode)
        {
            StringBuilder tmp = new StringBuilder(count);
            int submode = initialSubmode;
            int idx = 0;
            while (true)
            {
                char ch = msg[startpos + idx];
                switch (submode)
                {
                    case SUBMODE_ALPHA:
                        if (IsAlphaUpper(ch))
                        {
                            if (ch == ' ')
                            {
                                tmp.Append((char)26); //space
                            }
                            else
                            {
                                tmp.Append((char)(ch - 65));
                            }
                        }
                        else
                        {
                            if (IsAlphaLower(ch))
                            {
                                submode = SUBMODE_LOWER;
                                tmp.Append((char)27); //ll
                                continue;
                            }
                            else if (IsMixed(ch))
                            {
                                submode = SUBMODE_MIXED;
                                tmp.Append((char)28); //ml
                                continue;
                            }
                            else
                            {
                                tmp.Append((char)29); //ps
                                tmp.Append((char)PUNCTUATION[ch]);
                                break;
                            }
                        }
                        break;
                    case SUBMODE_LOWER:
                        if (IsAlphaLower(ch))
                        {
                            if (ch == ' ')
                            {
                                tmp.Append((char)26); //space
                            }
                            else
                            {
                                tmp.Append((char)(ch - 97));
                            }
                        }
                        else
                        {
                            if (IsAlphaUpper(ch))
                            {
                                tmp.Append((char)27); //as
                                tmp.Append((char)(ch - 65));
                                //space cannot happen here, it is also in "Lower"
                                break;
                            }
                            else if (IsMixed(ch))
                            {
                                submode = SUBMODE_MIXED;
                                tmp.Append((char)28); //ml
                                continue;
                            }
                            else
                            {
                                tmp.Append((char)29); //ps
                                tmp.Append((char)PUNCTUATION[ch]);
                                break;
                            }
                        }
                        break;
                    case SUBMODE_MIXED:
                        if (IsMixed(ch))
                        {
                            tmp.Append((char)MIXED[ch]);
                        }
                        else
                        {
                            if (IsAlphaUpper(ch))
                            {
                                submode = SUBMODE_ALPHA;
                                tmp.Append((char)28); //al
                                continue;
                            }
                            else if (IsAlphaLower(ch))
                            {
                                submode = SUBMODE_LOWER;
                                tmp.Append((char)27); //ll
                                continue;
                            }
                            else
                            {
                                if (startpos + idx + 1 < count)
                                {
                                    char next = msg[startpos + idx + 1];
                                    if (IsPunctuation(next))
                                    {
                                        submode = SUBMODE_PUNCTUATION;
                                        tmp.Append((char)25); //pl
                                        continue;
                                    }
                                }
                                tmp.Append((char)29); //ps
                                tmp.Append((char)PUNCTUATION[ch]);
                            }
                        }
                        break;
                    default: //SUBMODE_PUNCTUATION
                        if (IsPunctuation(ch))
                        {
                            tmp.Append((char)PUNCTUATION[ch]);
                        }
                        else
                        {
                            submode = SUBMODE_ALPHA;
                            tmp.Append((char)29); //al
                            continue;
                        }
                        break;
                }
                idx++;
                if (idx >= count)
                {
                    break;
                }
            }
            char h = (char)(0);
            int len = tmp.Length;
            for (int i = 0; i < len; i++)
            {
                bool odd = (i % 2) != 0;
                if (odd)
                {
                    h = (char)((h * 30) + tmp[i]);
                    sb.Append(h);
                }
                else
                {
                    h = tmp[i];
                }
            }
            if ((len % 2) != 0)
            {
                sb.Append((char)((h * 30) + 29)); //ps
            }
            return submode;
        }

        /// <summary>
        /// Encode parts of the message using Byte Compaction as described in ISO/IEC 15438:2001(E),
        /// chapter 4.4.3. The Unicode characters will be converted to binary using the cp437
        /// codepage.
        /// </summary>
        ///
        /// <param name="bytes">the message converted to a byte array</param>
        /// <param name="startpos">the start position within the message</param>
        /// <param name="count">the number of bytes to encode</param>
        /// <param name="startmode">the mode from which this method starts</param>
        /// <param name="sb">receives the encoded codewords</param>
        private static void EncodeBinary(byte[] bytes, int startpos, int count,
                int startmode, StringBuilder sb)
        {
            if (count == 1 && startmode == TEXT_COMPACTION)
            {
                sb.Append((char)SHIFT_TO_BYTE);
            }

            int idx = startpos;
            // Encode sixpacks
            if (count >= 6)
            {
                sb.Append((char)LATCH_TO_BYTE);
                char[] chars = new char[5];
                while ((startpos + count - idx) >= 6)
                {
                    long t = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        t <<= 8;
                        t += bytes[idx + i] & 0xff;
                    }
                    for (int i_0 = 0; i_0 < 5; i_0++)
                    {
                        chars[i_0] = (char)(t % 900);
                        t /= 900;
                    }
                    for (int i_1 = chars.Length - 1; i_1 >= 0; i_1--)
                    {
                        sb.Append(chars[i_1]);
                    }
                    idx += 6;
                }
            }
            //Encode rest (remaining n<5 bytes if any)
            if (idx < startpos + count)
            {
                sb.Append((char)LATCH_TO_BYTE_PADDED);
            }
            for (int i_2 = idx; i_2 < startpos + count; i_2++)
            {
                int ch = bytes[i_2] & 0xff;
                sb.Append((char)ch);
            }
        }

        private static void EncodeNumeric(String msg, int startpos, int count,
                StringBuilder sb)
        {
            /*
            int idx = 0;
            StringBuilder tmp = new StringBuilder(count / 3 + 1);
            Int64 num900 = 900;
            Int64 num0 = 0;             
            while (idx < count - 1)
            {
                tmp.Length = 0;
                int len = Math.Min(44, count - idx);
                String part = '1' + msg.Substring(startpos + idx, (startpos + idx
                                    + len) - (startpos + idx));
                Int64 bigint = Int64.Parse(part);
                do
                {
                    Int64 c = bigint % num900;
                    tmp.Append((char)System.Convert.ToInt64(c));
                    bigint /= num900;
                } while (!bigint.Equals(num0));

                //Reverse temporary string
                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    sb.Append(tmp[i]);
                }
                idx += len;
            }
            */

#if !NET35 && !WINDOWS_PHONE
            int idx = 0;
            StringBuilder tmp = new StringBuilder(count / 3 + 1);
            BigInteger num900 = new BigInteger(900);
            BigInteger num0 = new BigInteger(0);
            while (idx < count - 1)
            {
                tmp.Length = 0;
                int len = Math.Min(44, count - idx);
                String part = '1' + msg.Substring(startpos + idx, len);
#if SILVERLIGHT && !WPF 
            BigInteger bigint = BigIntegerExtensions.Parse(part);
#else
                BigInteger bigint = BigInteger.Parse(part);
#endif
                do
                {
                    BigInteger c = bigint % num900;
                    tmp.Append((char)c);
                    bigint = BigInteger.Divide(bigint, num900);
                } while (!bigint.Equals(num0));

                //Reverse temporary string
                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    sb.Append(tmp[i]);
                }
                idx += len;
            }
#else
         int idx = 0;
         StringBuilder tmp = new StringBuilder(count / 3 + 1);
         BigInteger num900 = new BigInteger(900);
         BigInteger num0 = new BigInteger(0);
         while (idx < count - 1)
         {
            tmp.Length = 0;
            int len = Math.Min(44, count - idx);
            String part = '1' + msg.Substring(startpos + idx, len);
            BigInteger bigint = BigInteger.Parse(part);
            do
            {
               BigInteger c = BigInteger.Modulo(bigint, num900);
               tmp.Append((char)c.GetHashCode());
               bigint = BigInteger.Division(bigint, num900);
            } while (!bigint.Equals(num0));

            //Reverse temporary string
            for (int i = tmp.Length - 1; i >= 0; i--)
            {
               sb.Append(tmp[i]);
            }
            idx += len;
         }
#endif
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsAlphaUpper(char ch)
        {
            return ch == ' ' || (ch >= 'A' && ch <= 'Z');
        }

        private static bool IsAlphaLower(char ch)
        {
            return ch == ' ' || (ch >= 'a' && ch <= 'z');
        }

        private static bool IsMixed(char ch)
        {
            return MIXED[ch] != -1;
        }

        private static bool IsPunctuation(char ch)
        {
            return PUNCTUATION[ch] != -1;
        }

        private static bool IsText(char ch)
        {
            return ch == '\t' || ch == '\n' || ch == '\r'
                    || (ch >= 32 && ch <= 126);
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using numeric compaction.
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="startpos">the start position within the message</param>
        /// <returns>
        /// the requested character count
        /// </returns>
        private static int DetermineConsecutiveDigitCount(String msg, int startpos)
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

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using text compaction.
        /// </summary>
        ///
        /// <param name="msg">the message</param>
        /// <param name="startpos">the start position within the message</param>
        /// <returns>the requested character count</returns>
        private static int DetermineConsecutiveTextCount(String msg,
                int startpos)
        {
            int len = msg.Length;
            int idx = startpos;
            while (idx < len)
            {
                char ch = msg[idx];
                int numericCount = 0;
                while (numericCount < 13 && IsDigit(ch) && idx < len)
                {
                    numericCount++;
                    idx++;
                    if (idx < len)
                    {
                        ch = msg[idx];
                    }
                }
                if (numericCount >= 13)
                {
                    return idx - startpos - numericCount;
                }
                if (numericCount > 0)
                {
                    //Heuristic: All text-encodable chars or digits are binary encodable
                    continue;
                }
                ch = msg[idx];

                //Check if character is encodable
                if (!IsText(ch))
                {
                    break;
                }
                idx++;
            }
            return idx - startpos;
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using binary compaction.
        /// </summary>
        ///
        /// <param name="msg">the message</param>
        /// <param name="bytes">the message converted to a byte array</param>
        /// <param name="startpos">the start position within the message</param>
        /// <returns>the requested character count</returns>
        private static int DetermineConsecutiveBinaryCount(String msg,
                byte[] bytes, int startpos)
        {
            int len = msg.Length;
            int idx = startpos;
            while (idx < len)
            {
                char ch = msg[idx];
                int numericCount = 0;

                while (numericCount < 13 && IsDigit(ch))
                {
                    numericCount++;
                    //textCount++;
                    int i = idx + numericCount;
                    if (i >= len)
                    {
                        break;
                    }
                    ch = msg[i];
                }
                if (numericCount >= 13)
                {
                    return idx - startpos;
                }
                int textCount = 0;
                while (textCount < 5 && IsText(ch))
                {
                    textCount++;
                    int i_0 = idx + textCount;
                    if (i_0 >= len)
                    {
                        break;
                    }
                    ch = msg[i_0];
                }
                if (textCount >= 5)
                {
                    return idx - startpos;
                }
                ch = msg[idx];

                //Check if character is encodable
                //Sun returns a ASCII 63 (?) for a character that cannot be mapped. Let's hope all
                //other VMs do the same
                if (bytes[idx] == 63 && ch != '?')
                {
                    throw new BarcodeEncoderException("Non-encodable character detected: "
                            + ch + " (Unicode: " + (int)ch + ')');
                }
                idx++;
            }
            return idx - startpos;
        }

        static Pdf417HighLevelEncoder()
        {
            for (int i = 0; i < MIXED.Length; i++)
            {
                MIXED[i] = -1;
            }
            for (sbyte i = 0; i < TEXT_MIXED_RAW.Length; i++)
            {
                sbyte b = TEXT_MIXED_RAW[i];
                if (b > 0)
                {
                    MIXED[b] = i;
                }
            }
            for (int i = 0; i < PUNCTUATION.Length; i++)
            {
                PUNCTUATION[i] = -1;
            }
            for (sbyte i = 0; i < TEXT_PUNCTUATION_RAW.Length; i++)
            {
                sbyte b = TEXT_PUNCTUATION_RAW[i];
                if (b > 0)
                {
                    PUNCTUATION[b] = i;
                }
            }
        }

    }
}
