using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// Common string-related functions.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class StringHelper
    {

#if (WINDOWS_PHONE || SILVERLIGHT || NETFX_CORE)
        private static String PLATFORM_DEFAULT_ENCODING = "UTF-8";
#else
        private static String PLATFORM_DEFAULT_ENCODING = Encoding.Default.WebName;
#endif

        public const String SHIFT_JIS = "SHIFT-JIS";
        public const String GB2312 = "GB2312";

        private const String EUC_JP = "EUC-JP";
        private const String UTF8 = "UTF-8";
        private const String ISO88591 = "ISO-8859-1";
        private const String GBK = "GB2312";

        private static readonly bool ASSUME_SHIFT_JIS = SHIFT_JIS.Equals(PLATFORM_DEFAULT_ENCODING, StringComparison.OrdinalIgnoreCase)
                || EUC_JP.Equals(PLATFORM_DEFAULT_ENCODING, StringComparison.OrdinalIgnoreCase);


        private StringHelper()
        {
        }


        /// <summary>
        /// Guesses the encoding.
        /// </summary>
        /// <param name="bytes">bytes encoding a string, whose encoding should be guessed</param>
        /// <param name="decodingOptions">decode hints if applicable</param>
        /// <returns>
        /// name of guessed encoding; at the moment will only guess one of:
        /// default encoding if none of these can possibly be correct
        /// </returns>
        public static String GuessEncoding(byte[] bytes, Dictionary<DecodeOptions, object> decodingOptions)
        {

            if (decodingOptions != null)
            {
                String characterSet = (String)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.CharacterSet);
                if (characterSet != null)
                {
                    return characterSet;
                }
            }


            // For now, merely tries to distinguish ISO-8859-1, UTF-8 and Shift_JIS,
            // which should be by far the most common encodings.
            int length = bytes.Length;
            bool canBeISO88591 = true;
            bool canBeShiftJIS = true;
            bool canBeUTF8 = true;
            bool canBeGBK = true;
            int utf8BytesLeft = 0;
            //int utf8LowChars = 0;
            int utf2BytesChars = 0;
            int utf3BytesChars = 0;
            int utf4BytesChars = 0;
            int sjisBytesLeft = 0;
            //int sjisLowChars = 0;
            int sjisKatakanaChars = 0;
            //int sjisDoubleBytesChars = 0;
            int sjisCurKatakanaWordLength = 0;
            int sjisCurDoubleBytesWordLength = 0;
            int sjisMaxKatakanaWordLength = 0;
            int sjisMaxDoubleBytesWordLength = 0;
            //int isoLowChars = 0;
            //int isoHighChars = 0;
            int isoHighOther = 0;

            bool utf8bom = bytes.Length > 3 && bytes[0] == (byte)0xEF && bytes[1] == (byte)0xBB && bytes[2] == (byte)0xBF;

            for (int i = 0; i < length && (canBeISO88591 || canBeShiftJIS || canBeUTF8 || canBeGBK); i++)
            {

                int val = bytes[i] & 0xFF;

                // UTF-8 stuff
                if (canBeUTF8)
                {
                    if (utf8BytesLeft > 0)
                    {
                        if ((val & 0x80) == 0)
                        {
                            canBeUTF8 = false;
                        }
                        else
                        {
                            utf8BytesLeft--;
                        }
                    }
                    else if ((val & 0x80) != 0)
                    {
                        if ((val & 0x40) == 0)
                        {
                            canBeUTF8 = false;
                        }
                        else
                        {
                            utf8BytesLeft++;
                            if ((val & 0x20) == 0)
                            {
                                utf2BytesChars++;
                            }
                            else
                            {
                                utf8BytesLeft++;
                                if ((val & 0x10) == 0)
                                {
                                    utf3BytesChars++;
                                }
                                else
                                {
                                    utf8BytesLeft++;
                                    if ((val & 0x08) == 0)
                                    {
                                        utf4BytesChars++;
                                    }
                                    else
                                    {
                                        canBeUTF8 = false;
                                    }
                                }
                            }
                        }
                    } //else {
                    //utf8LowChars++;
                    //}
                }

                // ISO-8859-1 stuff
                if (canBeISO88591)
                {
                    if (val > 0x7F && val < 0xA0)
                    {
                        canBeISO88591 = false;
                    }
                    else if (val > 0x9F)
                    {
                        if (val < 0xC0 || val == 0xD7 || val == 0xF7)
                        {
                            isoHighOther++;
                        } //else {
                        //isoHighChars++;
                        //}
                    } //else {
                    //isoLowChars++;
                    //}
                }

                if (canBeGBK)
                {
                    // GBK stuff --
                    int idx = 0;
                    for (idx = 0; idx < length; idx++)
                    {
                        int val2;
                        val = bytes[idx];
                        if (val < 128)
                            continue;
                        if (idx + 1 >= length)
                            break;
                        val2 = bytes[idx + 1];
                        if (((val >= 0xA1 && val <= 0xA9) && (val2 >= 0xA1 && val2 <= 0xFE)) ||
                        ((val >= 0xB0 && val <= 0xF7) && (val2 >= 0xA1 && val2 <=
                        0xFE)) || ((val >= 0x81 && val <= 0xA0) && (val2 >= 0x40 && val2 <= 0xFE
                        && val2 != 0x7F)) ||
                        ((val >= 0xAA && val <= 0xFE) && (val2 >= 0x40 && val2 <= 0xA0
                        && val2 != 0x7F)) ||
                        ((val >= 0xA8 && val <= 0xA9) && (val2 >= 0x40 && val2 <= 0xA0
                        && val2 != 0x7F)) ||
                        ((val >= 0xAA && val <= 0xAF) && (val2 >= 0xA1 && val2 <=
                        0xFE)) ||
                        ((val >= 0xF8 && val <= 0xFE) && (val2 >= 0xA1 && val2 <=
                        0xFE)) ||
                        ((val >= 0xA1 && val <= 0xA7) && (val2 >= 0x40 && val2 <= 0xA0
                        && val2 != 0x7F)))
                        {
                            idx++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (idx != length)
                    {
                        canBeGBK = false;
                    }
                    // GBK stuff end
                }

                // Shift_JIS stuff
                if (canBeShiftJIS)
                {
                    if (sjisBytesLeft > 0)
                    {
                        if (val < 0x40 || val == 0x7F || val > 0xFC)
                        {
                            canBeShiftJIS = false;
                        }
                        else
                        {
                            sjisBytesLeft--;
                        }
                    }
                    else if (val == 0x80 || val == 0xA0 || val > 0xEF)
                    {
                        canBeShiftJIS = false;
                    }
                    else if (val > 0xA0 && val < 0xE0)
                    {
                        sjisKatakanaChars++;
                        sjisCurDoubleBytesWordLength = 0;
                        sjisCurKatakanaWordLength++;
                        if (sjisCurKatakanaWordLength > sjisMaxKatakanaWordLength)
                        {
                            sjisMaxKatakanaWordLength = sjisCurKatakanaWordLength;
                        }
                    }
                    else if (val > 0x7F)
                    {
                        sjisBytesLeft++;
                        //sjisDoubleBytesChars++;
                        sjisCurKatakanaWordLength = 0;
                        sjisCurDoubleBytesWordLength++;
                        if (sjisCurDoubleBytesWordLength > sjisMaxDoubleBytesWordLength)
                        {
                            sjisMaxDoubleBytesWordLength = sjisCurDoubleBytesWordLength;
                        }
                    }
                    else
                    {
                        //sjisLowChars++;
                        sjisCurKatakanaWordLength = 0;
                        sjisCurDoubleBytesWordLength = 0;
                    }
                }
            }

            if (canBeUTF8 && utf8BytesLeft > 0)
            {
                canBeUTF8 = false;
            }
            if (canBeShiftJIS && sjisBytesLeft > 0)
            {
                canBeShiftJIS = false;
            }

            if (canBeGBK)
            {
#if !SILVERLIGHT
                return GBK;
#else
                    return PLATFORM_DEFAULT_ENCODING;
#endif
            }

            // Easy -- if there is BOM or at least 1 valid not-single byte character (and no evidence it can't be UTF-8), done
            if (canBeUTF8 && (utf8bom || utf2BytesChars + utf3BytesChars + utf4BytesChars > 0))
            {
                return UTF8;
            }
            // Easy -- if assuming Shift_JIS or at least 3 valid consecutive not-ascii characters (and no evidence it can't be), done
            if (canBeShiftJIS && (ASSUME_SHIFT_JIS || sjisMaxKatakanaWordLength >= 3 || sjisMaxDoubleBytesWordLength >= 3))
            {
#if !SILVERLIGHT
                return SHIFT_JIS;
#else
                return PLATFORM_DEFAULT_ENCODING;
#endif
            }
            // Distinguishing Shift_JIS and ISO-8859-1 can be a little tough for short words. The crude heuristic is:
            // - If we saw
            //   - only two consecutive katakana chars in the whole text, or
            //   - at least 10% of bytes that could be "upper" not-alphanumeric Latin1,
            // - then we conclude Shift_JIS, else ISO-8859-1
            if (canBeISO88591 && canBeShiftJIS)
            {
#if !SILVERLIGHT
                return ((sjisMaxKatakanaWordLength == 2 && sjisKatakanaChars == 2) || isoHighOther * 10 >= length) ? SHIFT_JIS : ISO88591;
#else
                return PLATFORM_DEFAULT_ENCODING;
#endif
            }

            // Otherwise, try in order ISO-8859-1, Shift JIS, UTF-8 and fall back to default platform encoding
            if (canBeISO88591)
            {
#if !SILVERLIGHT
                return ISO88591;
#else
                return PLATFORM_DEFAULT_ENCODING;
#endif
            }
            if (canBeShiftJIS)
            {
#if !SILVERLIGHT
                return SHIFT_JIS;
#else
                return PLATFORM_DEFAULT_ENCODING;
#endif
            }
            if (canBeUTF8)
            {
                return UTF8;
            }

            // Otherwise, we take a wild guess with platform encoding
            return PLATFORM_DEFAULT_ENCODING;

        }

    }
}
