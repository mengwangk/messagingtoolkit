using MessagingToolkit.Barcode.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{
    /// <summary>
    ///   <p>QR Codes can encode text as bits in one of several modes, and can use multiple modes
    /// in one QR Code. This class decodes the bits back into text.</p>
    ///   <p>See ISO 18004:2006, 6.4.3 - 6.4.7</p>
    ///   
    /// Modified: April 22 2012
    /// </summary>
    internal sealed class DecodedBitStreamParser
    {
        /// <summary>
        /// See ISO 18004:2006, 6.4.4 Table 5
        /// </summary>
        private static readonly char[] AlphaNumericChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
				'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':' };

        private const int Gb2312Subset = 1;

        private DecodedBitStreamParser()
        {
        }


        static internal DecoderResult Decode(byte[] bytes, Version version, ErrorCorrectionLevel ecLevel, IDictionary<DecodeOptions, object> decodingOptions)
        {
            BitSource bits = new BitSource(bytes);
            StringBuilder result = new StringBuilder(50);
            List<byte[]> byteSegments = new List<byte[]>(1);
            try
            {
                CharacterSetECI currentCharacterSetECI = null;
                bool fc1InEffect = false;
                Mode mode;
                do
                {
                    // While still another segment to read...
                    if (bits.Available() < 4)
                    {
                        // OK, assume we're done. Really, a TERMINATOR mode should have been recorded here
                        mode = Mode.Terminator;
                    }
                    else
                    {
                        mode = Mode.ForBits(bits.ReadBits(4)); // mode is encoded by 4 bits
                    }
                    if (mode != Mode.Terminator)
                    {
                        if (mode == Mode.Fnc1FirstPosition || mode == Mode.Fnc1SecondPosition)
                        {
                            // We do little with FNC1 except alter the parsed result a bit according to the spec
                            fc1InEffect = true;
                        }
                        else if (mode == Mode.StructuredAppend)
                        {
                            if (bits.Available() < 16)
                            {
                                throw FormatException.Instance;
                            }
                            // not really supported; all we do is ignore it
                            // Read next 8 bits (symbol sequence #) and 8 bits (parity data), then continue
                            bits.ReadBits(16);
                        }
                        else if (mode == Mode.Eci)
                        {
                            // Count doesn't apply to ECI
                            int value = ParseECIValue(bits);
                            currentCharacterSetECI = CharacterSetECI.GetCharacterSetECIByValue(value);
                            if (currentCharacterSetECI == null)
                            {
                                throw FormatException.Instance;
                            }
                        }
                        else
                        {
                            // First handle Hanzi mode which does not start with character count
                            if (mode == Mode.Hanzi)
                            {
                                //chinese mode contains a sub set indicator right after mode indicator
                                int subset = bits.ReadBits(4);
                                int countHanzi = bits.ReadBits(mode.GetCharacterCountBits(version));
                                if (subset == Gb2312Subset)
                                {
                                    DecodeHanziSegment(bits, result, countHanzi);
                                }
                            }
                            else
                            {
                                // "Normal" QR code modes:
                                // How many characters will follow, encoded in this mode?
                                int count = bits.ReadBits(mode.GetCharacterCountBits(version));
                                if (mode == Mode.Numeric)
                                {
                                    DecodeNumericSegment(bits, result, count);
                                }
                                else if (mode == Mode.Alphanumeric)
                                {
                                    DecodeAlphanumericSegment(bits, result, count, fc1InEffect);
                                }
                                else if (mode == Mode.Byte)
                                {
                                    DecodeByteSegment(bits, result, count, currentCharacterSetECI, byteSegments, decodingOptions);
                                }
                                else if (mode == Mode.Kanji)
                                {
                                    DecodeKanjiSegment(bits, result, count);
                                }
                                else
                                {
                                    throw FormatException.Instance;
                                }
                            }
                        }
                    }
                } while (mode != Mode.Terminator);
            }
            catch (ArgumentException iae)
            {
                // from readBits() calls
                throw FormatException.Instance;
            }

            return new DecoderResult(bytes, result.ToString(), ((byteSegments.Count == 0)) ? null : byteSegments, (ecLevel == null) ? null : ecLevel.ToString());
        }

        /// <summary>
        /// See specification GBT 18284-2000
        /// </summary>
        private static void DecodeHanziSegment(BitSource bits, StringBuilder result, int count)
        {
            // Don't crash trying to read more bits than we have available.
            if (count * 13 > bits.Available())
            {
                throw FormatException.Instance;
            }

            // Each character will require 2 bytes. Read the characters as 2-byte pairs
            // and decode as GB2312 afterwards
            byte[] buffer = new byte[2 * count];
            int offset = 0;
            while (count > 0)
            {
                // Each 13 bits encodes a 2-byte character
                int twoBytes = bits.ReadBits(13);
                int assembledTwoBytes = ((twoBytes / 0x060) << 8) | (twoBytes % 0x060);
                if (assembledTwoBytes < 0x003BF)
                {
                    // In the 0xA1A1 to 0xAAFE range
                    assembledTwoBytes += 0x0A1A1;
                }
                else
                {
                    // In the 0xB0A1 to 0xFAFE range
                    assembledTwoBytes += 0x0A6A1;
                }
                buffer[offset] = (byte)((assembledTwoBytes >> 8) & 0xFF);
                buffer[offset + 1] = (byte)(assembledTwoBytes & 0xFF);
                offset += 2;
                count--;
            }

            try
            {

                result.Append(Encoding.GetEncoding(StringHelper.GB2312).GetString(buffer, 0, buffer.Length));
            }
#if (WINDOWS_PHONE|| SILVERLIGHT || NETFX_CORE)
            catch (ArgumentException)
            {
                try
                {
                    // Silverlight only supports a limited number of character sets, trying fallback to UTF-8
                    result.Append(Encoding.GetEncoding("UTF-8").GetString(buffer, 0, buffer.Length));
                }
                catch (Exception)
                {
                    throw FormatException.Instance;
                }
            }
#endif
            catch (Exception)
            {
                throw FormatException.Instance;
            }
        }

        private static void DecodeKanjiSegment(BitSource bits, StringBuilder result, int count)
        {
            // Don't crash trying to read more bits than we have available.
            if (count * 13 > bits.Available())
            {
                throw FormatException.Instance;
            }

            // Each character will require 2 bytes. Read the characters as 2-byte pairs
            // and decode as Shift_JIS afterwards
            byte[] buffer = new byte[2 * count];
            int offset = 0;
            while (count > 0)
            {
                // Each 13 bits encodes a 2-byte character
                int twoBytes = bits.ReadBits(13);
                int assembledTwoBytes = ((twoBytes / 0x0C0) << 8) | (twoBytes % 0x0C0);
                if (assembledTwoBytes < 0x01F00)
                {
                    // In the 0x8140 to 0x9FFC range
                    assembledTwoBytes += 0x08140;
                }
                else
                {
                    // In the 0xE040 to 0xEBBF range
                    assembledTwoBytes += 0x0C140;
                }
                buffer[offset] = (byte)(assembledTwoBytes >> 8);
                buffer[offset + 1] = (byte)assembledTwoBytes;
                offset += 2;
                count--;
            }
            // Shift_JIS may not be supported in some environments:
            try
            {
                result.Append(Encoding.GetEncoding(StringHelper.SHIFT_JIS).GetString(buffer, 0, buffer.Length));
            }
#if (WINDOWS_PHONE|| SILVERLIGHT || NETFX_CORE)
            catch (ArgumentException)
            {
                try
                {
                    // Silverlight only supports a limited number of character sets, trying fallback to UTF-8
                    result.Append(Encoding.GetEncoding("UTF-8").GetString(buffer, 0, buffer.Length));
                }
                catch (Exception)
                {
                    throw FormatException.Instance;
                }
            }
#endif
            catch (Exception)
            {
                throw FormatException.Instance;
            }
        }

        private static void DecodeByteSegment(BitSource bits, StringBuilder result, int count, CharacterSetECI currentCharacterSetECI, List<byte[]> byteSegments, IDictionary<DecodeOptions, object> decodingOptions)
        {
            // Don't crash trying to read more bits than we have available.
            if (count << 3 > bits.Available())
            {
                throw FormatException.Instance;
            }

            byte[] readBytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                readBytes[i] = (byte)bits.ReadBits(8);
            }
            String encoding;
            if (currentCharacterSetECI == null)
            {
                // The spec isn't clear on this mode; see
                // section 6.4.5: t does not say which encoding to assuming
                // upon decoding. I have seen ISO-8859-1 used as well as
                // Shift_JIS -- without anything like an ECI designator to
                // give a hint.
                encoding = MessagingToolkit.Barcode.Common.StringHelper.GuessEncoding(readBytes, decodingOptions);
            }
            else
            {
                encoding = currentCharacterSetECI.EncodingName;
            }
            try
            {
                result.Append(Encoding.GetEncoding(encoding).GetString(readBytes, 0, readBytes.Length));
            }
#if (WINDOWS_PHONE|| SILVERLIGHT || NETFX_CORE)
            catch (ArgumentException)
            {
                try
                {
                    // Silverlight only supports a limited number of character sets, trying fallback to UTF-8
                    result.Append(Encoding.GetEncoding("UTF-8").GetString(readBytes, 0, readBytes.Length));
                }
                catch (Exception)
                {
                    throw FormatException.Instance;
                }
            }
#endif
            catch (Exception)
            {
                throw FormatException.Instance;
            }
            byteSegments.Add(readBytes);
        }

        private static char ToAlphaNumericChar(int val)
        {
            if (val >= AlphaNumericChars.Length)
            {
                throw FormatException.Instance;
            }
            return AlphaNumericChars[val];
        }

        private static void DecodeAlphanumericSegment(BitSource bits, StringBuilder result, int count, bool fc1InEffect)
        {
            // Read two characters at a time
            int start = result.Length;
            while (count > 1)
            {
                if (bits.Available() < 11)
                {
                    throw FormatException.Instance;
                }
                int nextTwoCharsBits = bits.ReadBits(11);
                result.Append(ToAlphaNumericChar(nextTwoCharsBits / 45));
                result.Append(ToAlphaNumericChar(nextTwoCharsBits % 45));
                count -= 2;
            }
            if (count == 1)
            {
                // special case: one character left
                if (bits.Available() < 6)
                {
                    throw FormatException.Instance;
                }
                result.Append(ToAlphaNumericChar(bits.ReadBits(6)));
            }
            // See section 6.4.8.1, 6.4.8.2
            if (fc1InEffect)
            {
                // We need to massage the result a bit if in an FNC1 mode:
                for (int i = start; i < result.Length; i++)
                {
                    if (result[i] == '%')
                    {
                        if (i < result.Length - 1 && result[i + 1] == '%')
                        {
                            // %% is rendered as %
                            result.Remove(i + 1, 1);
                        }
                        else
                        {
                            // In alpha mode, % should be converted to FNC1 separator 0x1D
                            result[i] = (char)0x1D;
                        }
                    }
                }
            }
        }

        private static void DecodeNumericSegment(BitSource bits, StringBuilder result, int count)
        {
            // Read three digits at a time
            while (count >= 3)
            {
                // Each 10 bits encodes three digits
                if (bits.Available() < 10)
                {
                    throw FormatException.Instance;
                }
                int threeDigitsBits = bits.ReadBits(10);
                if (threeDigitsBits >= 1000)
                {
                    throw FormatException.Instance;
                }
                result.Append(ToAlphaNumericChar(threeDigitsBits / 100));
                result.Append(ToAlphaNumericChar((threeDigitsBits / 10) % 10));
                result.Append(ToAlphaNumericChar(threeDigitsBits % 10));
                count -= 3;
            }
            if (count == 2)
            {
                // Two digits left over to read, encoded in 7 bits
                if (bits.Available() < 7)
                {
                    throw FormatException.Instance;
                }
                int twoDigitsBits = bits.ReadBits(7);
                if (twoDigitsBits >= 100)
                {
                    throw FormatException.Instance;
                }
                result.Append(ToAlphaNumericChar(twoDigitsBits / 10));
                result.Append(ToAlphaNumericChar(twoDigitsBits % 10));
            }
            else if (count == 1)
            {
                // One digit left over to read
                if (bits.Available() < 4)
                {
                    throw FormatException.Instance;
                }
                int digitBits = bits.ReadBits(4);
                if (digitBits >= 10)
                {
                    throw FormatException.Instance;
                }
                result.Append(ToAlphaNumericChar(digitBits));
            }
        }

        private static int ParseECIValue(BitSource bits)
        {
            int firstByte = bits.ReadBits(8);
            if ((firstByte & 0x80) == 0)
            {
                // just one byte
                return firstByte & 0x7F;
            }
            if ((firstByte & 0xC0) == 0x80)
            {
                // two bytes
                int secondByte = bits.ReadBits(8);
                return ((firstByte & 0x3F) << 8) | secondByte;
            }
            if ((firstByte & 0xE0) == 0xC0)
            {
                // three bytes
                int secondThirdBytes = bits.ReadBits(16);
                return ((firstByte & 0x1F) << 16) | secondThirdBytes;
            }
            throw FormatException.Instance;
        }
    }
}
