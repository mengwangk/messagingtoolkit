using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.QRCode.Helper
{
    /// <summary>
    /// Common string-related functions.
    /// </summary>
    public sealed class StringHelper
    {

        private static readonly String PlatformDefaultEncoding = "ISO-8859-1";

        public const String ShiftJis = "SHIFT-JIS";
        public const String GB2312 = "GB2312";

        private const String EucJP = "EUC-JP";
        private const String Utf8 = "UTF-8";
        private const String ISO88591 = "ISO-8859-1";

        private static readonly bool AssumeShiftJis = ShiftJis.Equals(PlatformDefaultEncoding, StringComparison.InvariantCultureIgnoreCase)
                || EucJP.Equals(PlatformDefaultEncoding, StringComparison.InvariantCultureIgnoreCase);


        private StringHelper()
        {
        }


        /// <summary>
        /// Guesses the encoding.
        /// </summary>
        /// <param name="bytes">bytes encoding a string, whose encoding should be guessed</param>
        /// <param name="hints">decode hints if applicable</param>
        /// <returns>
        /// name of guessed encoding; at the moment will only guess one of:
        /// <see cref="F:MessagingToolkit.Barcode.Common.StringHelper.ShiftJis"/>
        /// ,
        /// <see cref="F:MessagingToolkit.Barcode.Common.StringHelper.Utf8"/>
        /// ,
        /// <see cref="F:MessagingToolkit.Barcode.Common.StringHelper.ISO88591"/>
        /// , or the platform
        /// default encoding if none of these can possibly be correct
        /// </returns>
        public static String GuessEncoding(byte[] bytes)
        {
            // Does it start with the UTF-8 byte order mark? then guess it's UTF-8
            if (bytes.Length > 3 && bytes[0] == (byte)0xEF
                    && bytes[1] == (byte)0xBB && bytes[2] == (byte)0xBF)
            {
                return Utf8;
            }
            // For now, merely tries to distinguish ISO-8859-1, UTF-8 and Shift_JIS,
            // which should be by far the most common encodings. ISO-8859-1
            // should not have bytes in the 0x80 - 0x9F range, while Shift_JIS
            // uses this as a first byte of a two-byte character. If we see this
            // followed by a valid second byte in Shift_JIS, assume it is Shift_JIS.
            // If we see something else in that second byte, we'll make the risky guess
            // that it's UTF-8.
            int length = bytes.Length;
            bool canBeISO88591 = true;
            bool canBeShiftJIS = true;
            bool canBeUTF8 = true;
            int utf8BytesLeft = 0;
            int maybeDoubleByteCount = 0;
            int maybeSingleByteKatakanaCount = 0;
            bool sawLatin1Supplement = false;
            bool sawUTF8Start = false;
            bool lastWasPossibleDoubleByteStart = false;

            for (int i = 0; i < length
                    && (canBeISO88591 || canBeShiftJIS || canBeUTF8); i++)
            {

                int val = bytes[i] & 0xFF;

                // UTF-8 stuff
                if (val >= 0x80 && val <= 0xBF)
                {
                    if (utf8BytesLeft > 0)
                    {
                        utf8BytesLeft--;
                    }
                }
                else
                {
                    if (utf8BytesLeft > 0)
                    {
                        canBeUTF8 = false;
                    }
                    if (val >= 0xC0 && val <= 0xFD)
                    {
                        sawUTF8Start = true;
                        int valueCopy = val;
                        while ((valueCopy & 0x40) != 0)
                        {
                            utf8BytesLeft++;
                            valueCopy <<= 1;
                        }
                    }
                }

                // ISO-8859-1 stuff

                if ((val == 0xC2 || val == 0xC3) && i < length - 1)
                {
                    // This is really a poor hack. The slightly more exotic characters people might want to put in
                    // a QR Code, by which I mean the Latin-1 supplement characters (e.g. u-umlaut) have encodings
                    // that start with 0xC2 followed by [0xA0,0xBF], or start with 0xC3 followed by [0x80,0xBF].
                    int nextValue = bytes[i + 1] & 0xFF;
                    if (nextValue <= 0xBF
                            && ((val == 0xC2 && nextValue >= 0xA0) || (val == 0xC3 && nextValue >= 0x80)))
                    {
                        sawLatin1Supplement = true;
                    }
                }
                if (val >= 0x7F && val <= 0x9F)
                {
                    canBeISO88591 = false;
                }

                // Shift_JIS stuff

                if (val >= 0xA1 && val <= 0xDF)
                {
                    // count the number of characters that might be a Shift_JIS single-byte Katakana character
                    if (!lastWasPossibleDoubleByteStart)
                    {
                        maybeSingleByteKatakanaCount++;
                    }
                }
                if (!lastWasPossibleDoubleByteStart
                        && ((val >= 0xF0 && val <= 0xFF) || val == 0x80 || val == 0xA0))
                {
                    canBeShiftJIS = false;
                }
                if ((val >= 0x81 && val <= 0x9F)
                        || (val >= 0xE0 && val <= 0xEF))
                {
                    // These start double-byte characters in Shift_JIS. Let's see if it's followed by a valid
                    // second byte.
                    if (lastWasPossibleDoubleByteStart)
                    {
                        // If we just checked this and the last byte for being a valid double-byte
                        // char, don't check starting on this byte. If this and the last byte
                        // formed a valid pair, then this shouldn't be checked to see if it starts
                        // a double byte pair of course.
                        lastWasPossibleDoubleByteStart = false;
                    }
                    else
                    {
                        // ... otherwise do check to see if this plus the next byte form a valid
                        // double byte pair encoding a character.
                        lastWasPossibleDoubleByteStart = true;
                        if (i >= bytes.Length - 1)
                        {
                            canBeShiftJIS = false;
                        }
                        else
                        {
                            int nextValue_0 = bytes[i + 1] & 0xFF;
                            if (nextValue_0 < 0x40 || nextValue_0 > 0xFC)
                            {
                                canBeShiftJIS = false;
                            }
                            else
                            {
                                maybeDoubleByteCount++;
                            }
                            // There is some conflicting information out there about which bytes can follow which in
                            // double-byte Shift_JIS characters. The rule above seems to be the one that matches practice.
                        }
                    }
                }
                else
                {
                    lastWasPossibleDoubleByteStart = false;
                }
            }
            if (utf8BytesLeft > 0)
            {
                canBeUTF8 = false;
            }

            // Easy -- if assuming Shift_JIS and no evidence it can't be, done
            if (canBeShiftJIS && AssumeShiftJis)
            {
                return ShiftJis;
            }
            if (canBeUTF8 && sawUTF8Start)
            {
                return Utf8;
            }
            // Distinguishing Shift_JIS and ISO-8859-1 can be a little tough. The crude heuristic is:
            // - If we saw
            //   - at least 3 bytes that starts a double-byte value (bytes that are rare in ISO-8859-1), or
            //   - over 5% of bytes could be single-byte Katakana (also rare in ISO-8859-1),
            // - and, saw no sequences that are invalid in Shift_JIS, then we conclude Shift_JIS
            if (canBeShiftJIS
                    && (maybeDoubleByteCount >= 3 || 20 * maybeSingleByteKatakanaCount > length))
            {
                return ShiftJis;
            }
            // Otherwise, we default to ISO-8859-1 unless we know it can't be
            if (!sawLatin1Supplement && canBeISO88591)
            {
                return ISO88591;
            }
            // Otherwise, we take a wild guess with platform encoding
            return PlatformDefaultEncoding;
        }

    }
}
