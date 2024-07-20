using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using BitSource = MessagingToolkit.Barcode.Common.BitSource;
using DecoderResult = MessagingToolkit.Barcode.Common.DecoderResult;
using StringHelper = MessagingToolkit.Barcode.Common.StringHelper;

namespace MessagingToolkit.Barcode.DataMatrix.Decoders
{
    /// <summary>
    /// <p>Data Matrix Codes can encode text as bits in one of several modes, and can use multiple modes
    /// in one Data Matrix Code. This class decodes the bits back into text.</p>
    /// <p>See ISO 16022:2006, 5.2.1 - 5.2.9.2</p>
    /// 
    /// Modified: May 10 2012
    /// </summary>
    internal sealed class DecodedBitStreamParser
    {

        public enum Mode
        {
            PAD_ENCODE, // Not really a mode
            ASCII_ENCODE, C40_ENCODE, TEXT_ENCODE, ANSIX12_ENCODE, EDIFACT_ENCODE, BASE256_ENCODE
        }

        /// <summary>
        /// See ISO 16022:2006, Annex C Table C.1
        /// The C40 Basic Character Set (///'s used for placeholders for the shift values)
        /// </summary>
        ///
        private static readonly char[] C40_BASIC_SET_CHARS = { '*', '*', '*', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
				'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        private static readonly char[] C40_SHIFT2_SET_CHARS = { '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_' };

        /// <summary>
        /// See ISO 16022:2006, Annex C Table C.2
        /// The Text Basic Character Set (///'s used for placeholders for the shift values)
        /// </summary>
        ///
        private static readonly char[] TEXT_BASIC_SET_CHARS = { '*', '*', '*', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
				'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static readonly char[] TEXT_SHIFT3_SET_CHARS = { '\'', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '{',
				'|', '}', '~', (char) 127 };

        private DecodedBitStreamParser()
        { }

        static internal DecoderResult Decode(byte[] bytes)
        {
            BitSource bits = new BitSource(bytes);
            StringBuilder result = new StringBuilder(100);
            StringBuilder resultTrailer = new StringBuilder(0);
            List<byte[]> byteSegments = new List<byte[]>(1);
            DecodedBitStreamParser.Mode mode = Mode.ASCII_ENCODE;
            do
            {
                if (mode == Mode.ASCII_ENCODE)
                {
                    mode = DecodeAsciiSegment(bits, result, resultTrailer);
                }
                else
                {
                    switch (mode)
                    {
                        case Mode.C40_ENCODE:
                            DecodeC40Segment(bits, result);
                            break;
                        case Mode.TEXT_ENCODE:
                            DecodeTextSegment(bits, result);
                            break;
                        case Mode.ANSIX12_ENCODE:
                            DecodeAnsiX12Segment(bits, result);
                            break;
                        case Mode.EDIFACT_ENCODE:
                            DecodeEdifactSegment(bits, result);
                            break;
                        case Mode.BASE256_ENCODE:
                            DecodeBase256Segment(bits, result, byteSegments);
                            break;
                        default:
                            throw FormatException.Instance;
                    }
                    mode = Mode.ASCII_ENCODE;
                }
            } while (mode != Mode.PAD_ENCODE && bits.Available() > 0);
            if (resultTrailer.Length > 0)
            {
                result.Append(resultTrailer.ToString());
            }
            return new DecoderResult(bytes, result.ToString(), ((byteSegments.Count == 0)) ? null : byteSegments, null);
        }

        /// <summary>
        /// See ISO 16022:2006, 5.2.3 and Annex C, Table C.2
        /// </summary>
        private static DecodedBitStreamParser.Mode DecodeAsciiSegment(BitSource bits, StringBuilder result, StringBuilder resultTrailer)
        {
            bool upperShift = false;
            do
            {
                int oneByte = bits.ReadBits(8);
                if (oneByte == 0)
                {
                    throw FormatException.Instance;
                }
                else if (oneByte <= 128)
                { // ASCII data (ASCII value + 1)
                    if (upperShift)
                    {
                        oneByte += 128;
                        //upperShift = false;
                    }
                    result.Append((char)(oneByte - 1));
                    return Mode.ASCII_ENCODE;
                }
                else if (oneByte == 129)
                { // Pad
                    return Mode.PAD_ENCODE;
                }
                else if (oneByte <= 229)
                { // 2-digit data 00-99 (Numeric Value + 130)
                    int val = oneByte - 130;
                    if (val < 10)
                    { // padd with '0' for single digit values
                        result.Append('0');
                    }
                    result.Append(val);
                }
                else if (oneByte == 230)
                { // Latch to C40 encodation
                    return Mode.C40_ENCODE;
                }
                else if (oneByte == 231)
                { // Latch to Base 256 encodation
                    return Mode.BASE256_ENCODE;
                }
                else if (oneByte == 232)
                {
                    // FNC1
                    result.Append((char)29); // translate as ASCII 29
                }
                else if (oneByte == 233 || oneByte == 234)
                {
                    // Structured Append, Reader Programming
                    // Ignore these symbols for now
                    //throw ReaderException.getInstance();
                }
                else if (oneByte == 235)
                { // Upper Shift (shift to Extended ASCII)
                    upperShift = true;
                }
                else if (oneByte == 236)
                { // 05 Macro
                    result.Append("[)>\u001E05\u001D");
                    resultTrailer.Insert(0, "\u001E\u0004");
                }
                else if (oneByte == 237)
                { // 06 Macro
                    result.Append("[)>\u001E06\u001D");
                    resultTrailer.Insert(0, "\u001E\u0004");
                }
                else if (oneByte == 238)
                { // Latch to ANSI X12 encodation
                    return Mode.ANSIX12_ENCODE;
                }
                else if (oneByte == 239)
                { // Latch to Text encodation
                    return Mode.TEXT_ENCODE;
                }
                else if (oneByte == 240)
                { // Latch to EDIFACT encodation
                    return Mode.EDIFACT_ENCODE;
                }
                else if (oneByte == 241)
                { // ECI Character
                    // TODO(bbrown): I think we need to support ECI
                    //throw ReaderException.getInstance();
                    // Ignore this symbol for now
                }
                else if (oneByte >= 242)
                { // Not to be used in ASCII encodation
                    // ... but work around encoders that end with 254, latch back to ASCII
                    if (oneByte != 254 || bits.Available() != 0)
                    {
                        throw FormatException.Instance;
                    }
                }
            } while (bits.Available() > 0);
            return Mode.ASCII_ENCODE;
        }

        /// <summary>
        /// See ISO 16022:2006, 5.2.5 and Annex C, Table C.1
        /// </summary>
        ///
        private static void DecodeC40Segment(BitSource bits, StringBuilder result)
        {
            // Three C40 values are encoded in a 16-bit value as
            // (1600 * C1) + (40 * C2) + C3 + 1
            // TODO(bbrown): The Upper Shift with C40 doesn't work in the 4 value scenario all the time
            bool upperShift = false;

            int[] cValues = new int[3];
            int shift = 0;

            do
            {
                // If there is only one byte left then it will be encoded as ASCII
                if (bits.Available() == 8)
                {
                    return;
                }
                int firstByte = bits.ReadBits(8);
                if (firstByte == 254)
                { // Unlatch codeword
                    return;
                }

                ParseTwoBytes(firstByte, bits.ReadBits(8), cValues);

                for (int i = 0; i < 3; i++)
                {
                    int cValue = cValues[i];
                    switch (shift)
                    {
                        case 0:
                            if (cValue < 3)
                            {
                                shift = cValue + 1;
                            }
                            else if (cValue < C40_BASIC_SET_CHARS.Length)
                            {
                                char c40char = C40_BASIC_SET_CHARS[cValue];
                                if (upperShift)
                                {
                                    result.Append((char)(c40char + 128));
                                    upperShift = false;
                                }
                                else
                                {
                                    result.Append(c40char);
                                }
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                        case 1:
                            if (upperShift)
                            {
                                result.Append((char)(cValue + 128));
                                upperShift = false;
                            }
                            else
                            {
                                result.Append((char)cValue);
                            }
                            shift = 0;
                            break;
                        case 2:
                            if (cValue < C40_SHIFT2_SET_CHARS.Length)
                            {
                                char c40char_0 = C40_SHIFT2_SET_CHARS[cValue];
                                if (upperShift)
                                {
                                    result.Append((char)(c40char_0 + 128));
                                    upperShift = false;
                                }
                                else
                                {
                                    result.Append(c40char_0);
                                }
                            }
                            else if (cValue == 27)
                            { // FNC1
                                result.Append((char)29); // translate as ASCII 29
                            }
                            else if (cValue == 30)
                            { // Upper Shift
                                upperShift = true;
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            shift = 0;
                            break;
                        case 3:
                            if (upperShift)
                            {
                                result.Append((char)(cValue + 224));
                                upperShift = false;
                            }
                            else
                            {
                                result.Append((char)(cValue + 96));
                            }
                            shift = 0;
                            break;
                        default:
                            throw FormatException.Instance;
                    }
                }
            } while (bits.Available() > 0);
        }

        /// <summary>
        /// See ISO 16022:2006, 5.2.6 and Annex C, Table C.2
        /// </summary>
        ///
        private static void DecodeTextSegment(BitSource bits, StringBuilder result)
        {
            // Three Text values are encoded in a 16-bit value as
            // (1600 * C1) + (40 * C2) + C3 + 1
            // TODO(bbrown): The Upper Shift with Text doesn't work in the 4 value scenario all the time
            bool upperShift = false;

            int[] cValues = new int[3];
            int shift = 0;
            do
            {
                // If there is only one byte left then it will be encoded as ASCII
                if (bits.Available() == 8)
                {
                    return;
                }
                int firstByte = bits.ReadBits(8);
                if (firstByte == 254)
                { // Unlatch codeword
                    return;
                }

                ParseTwoBytes(firstByte, bits.ReadBits(8), cValues);

                for (int i = 0; i < 3; i++)
                {
                    int cValue = cValues[i];
                    switch (shift)
                    {
                        case 0:
                            if (cValue < 3)
                            {
                                shift = cValue + 1;
                            }
                            else if (cValue < TEXT_BASIC_SET_CHARS.Length)
                            {
                                char textChar = TEXT_BASIC_SET_CHARS[cValue];
                                if (upperShift)
                                {
                                    result.Append((char)(textChar + 128));
                                    upperShift = false;
                                }
                                else
                                {
                                    result.Append(textChar);
                                }
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                        case 1:
                            if (upperShift)
                            {
                                result.Append((char)(cValue + 128));
                                upperShift = false;
                            }
                            else
                            {
                                result.Append((char)cValue);
                            }
                            shift = 0;
                            break;
                        case 2:
                            // Shift 2 for Text is the same encoding as C40
                            if (cValue < C40_SHIFT2_SET_CHARS.Length)
                            {
                                char c40char = C40_SHIFT2_SET_CHARS[cValue];
                                if (upperShift)
                                {
                                    result.Append((char)(c40char + 128));
                                    upperShift = false;
                                }
                                else
                                {
                                    result.Append(c40char);
                                }
                            }
                            else if (cValue == 27)
                            { // FNC1
                                result.Append((char)29); // translate as ASCII 29
                            }
                            else if (cValue == 30)
                            { // Upper Shift
                                upperShift = true;
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            shift = 0;
                            break;
                        case 3:
                            if (cValue < TEXT_SHIFT3_SET_CHARS.Length)
                            {
                                char textChar_0 = TEXT_SHIFT3_SET_CHARS[cValue];
                                if (upperShift)
                                {
                                    result.Append((char)(textChar_0 + 128));
                                    upperShift = false;
                                }
                                else
                                {
                                    result.Append(textChar_0);
                                }
                                shift = 0;
                            }
                            else
                            {
                                throw FormatException.Instance;
                            }
                            break;
                        default:
                            throw FormatException.Instance;
                    }
                }
            } while (bits.Available() > 0);
        }

        /// <summary>
        /// See ISO 16022:2006, 5.2.7
        /// </summary>
        ///
        private static void DecodeAnsiX12Segment(BitSource bits, StringBuilder result)
        {
            // Three ANSI X12 values are encoded in a 16-bit value as
            // (1600 * C1) + (40 * C2) + C3 + 1

            int[] cValues = new int[3];
            do
            {
                // If there is only one byte left then it will be encoded as ASCII
                if (bits.Available() == 8)
                {
                    return;
                }
                int firstByte = bits.ReadBits(8);
                if (firstByte == 254)
                { // Unlatch codeword
                    return;
                }

                ParseTwoBytes(firstByte, bits.ReadBits(8), cValues);

                for (int i = 0; i < 3; i++)
                {
                    int cValue = cValues[i];
                    if (cValue == 0)
                    { // X12 segment terminator <CR>
                        result.Append('\r');
                    }
                    else if (cValue == 1)
                    { // X12 segment separator *
                        result.Append('*');
                    }
                    else if (cValue == 2)
                    { // X12 sub-element separator >
                        result.Append('>');
                    }
                    else if (cValue == 3)
                    { // space
                        result.Append(' ');
                    }
                    else if (cValue < 14)
                    { // 0 - 9
                        result.Append((char)(cValue + 44));
                    }
                    else if (cValue < 40)
                    { // A - Z
                        result.Append((char)(cValue + 51));
                    }
                    else
                    {
                        throw FormatException.Instance;
                    }
                }
            } while (bits.Available() > 0);
        }

        private static void ParseTwoBytes(int firstByte, int secondByte, int[] result)
        {
            int fullBitValue = (firstByte << 8) + secondByte - 1;
            int temp = fullBitValue / 1600;
            result[0] = temp;
            fullBitValue -= temp * 1600;
            temp = fullBitValue / 40;
            result[1] = temp;
            result[2] = fullBitValue - temp * 40;
        }

        /// <summary>
        /// See ISO 16022:2006, 5.2.8 and Annex C Table C.3
        /// </summary>
        ///
        private static void DecodeEdifactSegment(BitSource bits, StringBuilder result)
        {
            do
            {
                // If there is only two or less bytes left then it will be encoded as ASCII
                if (bits.Available() <= 16)
                {
                    return;
                }

                for (int i = 0; i < 4; i++)
                {
                    int edifactValue = bits.ReadBits(6);

                    // Check for the unlatch character
                    if (edifactValue == 0x1F)
                    { 
                        // 011111
                        // Read rest of byte, which should be 0, and stop
                        int bitsLeft = 8 - bits.BitOffset;
                        if (bitsLeft != 8)
                        {
                            bits.ReadBits(bitsLeft);
                        }
                        return;
                    }

                    if ((edifactValue & 0x20) == 0)
                    {  // no 1 in the leading (6th) bit
                        edifactValue |= 0x40;  // Add a leading 01 to the 6 bit binary value
                    }
                    result.Append((char)edifactValue);
                }
            } while (bits.Available() > 0);
        }

        /// <summary>
        /// See ISO 16022:2006, 5.2.9 and Annex B, B.2
        /// </summary>
        ///
        private static void DecodeBase256Segment(BitSource bits, StringBuilder result, ICollection<byte[]> byteSegments)
        {
            // Figure out how long the Base 256 Segment is.
            int codewordPosition = 1 + bits.ByteOffset; // position is 1-indexed
            int d1 = Unrandomize255State(bits.ReadBits(8), codewordPosition++);
            int count;
            if (d1 == 0)
            { // Read the remainder of the symbol
                count = bits.Available() / 8;
            }
            else if (d1 < 250)
            {
                count = d1;
            }
            else
            {
                count = 250 * (d1 - 249) + Unrandomize255State(bits.ReadBits(8), codewordPosition++);
            }

            // We're seeing NegativeArraySizeException errors from users.
            if (count < 0)
            {
                throw FormatException.Instance;
            }

            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                // Have seen this particular error in the wild, such as at
                // http://www.bcgen.com/demo/IDAutomationStreamingDataMatrix.aspx?MODE=3&D=Fred&PFMT=3&PT=F&X=0.3&O=0&LM=0.2
                if (bits.Available() < 8)
                {
                    throw FormatException.Instance;
                }
                bytes[i] = (byte)Unrandomize255State(bits.ReadBits(8), codewordPosition++);
            }
            byteSegments.Add(bytes);
            try
            {
#if !SILVERLIGHT
    result.Append(Encoding.GetEncoding("ISO-8859-1").GetString(bytes));           
#else
                result.Append(Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, bytes.Length));
#endif

            }
            catch (IOException uee)
            {
                throw new InvalidOperationException("Platform does not support required encoding: " + uee);
            }
        }

        /// <summary>
        /// See ISO 16022:2006, Annex B, B.2
        /// </summary>
        ///
        private static int Unrandomize255State(int randomizedBase256Codeword, int base256CodewordPosition)
        {
            int pseudoRandomNumber = ((149 * base256CodewordPosition) % 255) + 1;
            int tempVariable = randomizedBase256Codeword - pseudoRandomNumber;
            return (tempVariable >= 0) ? tempVariable : tempVariable + 256;
        }

    }
}
