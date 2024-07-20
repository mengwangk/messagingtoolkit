using System;
using System.Collections;
using System.IO;
using System.Text;

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Aztec;
using MessagingToolkit.Barcode.Common.ReedSolomon;

namespace MessagingToolkit.Barcode.Aztec.Decoder
{

    /// <summary>
    /// The main class which implements Aztec Code decoding -- as opposed to locating and extracting
    /// the Aztec Code from an image.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public sealed class Decoder
    {
        public enum Table
        {
            UPPER, LOWER, MIXED, DIGIT, PUNCT, BINARY
        }

        private static readonly int[] NB_BITS_COMPACT = { 0, 104, 240, 408, 608 };

        private static readonly int[] NB_BITS = { 0, 128, 288, 480, 704, 960, 1248, 1568, 1920, 2304, 2720, 3168, 3648, 4160, 4704, 5280, 5888, 6528, 7200, 7904, 8640, 9408, 10208, 11040, 11904, 12800,
				13728, 14688, 15680, 16704, 17760, 18848, 19968 };

        private static readonly int[] NB_DATABLOCK_COMPACT = { 0, 17, 40, 51, 76 };

        private static readonly int[] NB_DATABLOCK = { 0, 21, 48, 60, 88, 120, 156, 196, 240, 230, 272, 316, 364, 416, 470, 528, 588, 652, 720, 790, 864, 940, 1020, 920, 992, 1066, 1144, 1224, 1306, 1392,
				1480, 1570, 1664 };

        private static readonly String[] UPPER_TABLE = { "CTRL_PS", " ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
				"CTRL_LL", "CTRL_ML", "CTRL_DL", "CTRL_BS" };

        private static readonly String[] LOWER_TABLE = { "CTRL_PS", " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
				"CTRL_US", "CTRL_ML", "CTRL_DL", "CTRL_BS" };

        private static readonly String[] MIXED_TABLE = { "CTRL_PS", " ", @"\1", @"\2", @"\3", @"\4", @"\5", @"\6", @"\7", "\b", "\t", "\n", @"\13", "\f", "\r", @"\33", @"\34", @"\35", @"\36", @"\37", "@", "\\", "^",
				"_", "`", "|", "~", @"\177", "CTRL_LL", "CTRL_UL", "CTRL_PL", "CTRL_BS" };

        private static readonly String[] PUNCT_TABLE = { "", "\r", "\r\n", ". ", ", ", ": ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "[",
				"]", "{", "}", "CTRL_UL" };

        private static readonly String[] DIGIT_TABLE = { "CTRL_PS", " ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ",", ".", "CTRL_UL", "CTRL_US" };

        private int numCodewords;
        private int codewordSize;
        private AztecDetectorResult ddata;
        private int invertedBitCount;

        public DecoderResult Decode(AztecDetectorResult detectorResult)
        {
            ddata = detectorResult;
            BitMatrix matrix = detectorResult.Bits;

            if (!ddata.IsCompact())
            {
                matrix = RemoveDashedLines(ddata.Bits);
            }

            bool[] rawbits = ExtractBits(matrix);

            bool[] correctedBits = CorrectBits(rawbits);

            String result = GetEncodedData(correctedBits);

            return new DecoderResult(null, result, null, null);
        }

        /// <summary>
        /// Gets the string encoded in the aztec code bits
        /// </summary>
        /// <param name="correctedBits">The corrected bits.</param>
        /// <returns>
        /// the decoded string
        /// </returns>
        /// <exception cref="FormatException">if the input is not valid</exception>
        private String GetEncodedData(bool[] correctedBits)
        {

            int endIndex = codewordSize * ddata.GetNbDatablocks() - invertedBitCount;
            if (endIndex > correctedBits.Length)
            {
                throw FormatException.Instance;
            }

            Decoder.Table lastTable = MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.UPPER;
            Decoder.Table table = MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.UPPER;
            int startIndex = 0;
            StringBuilder result = new StringBuilder(20);
            bool end = false;
            bool shift = false;
            bool switchShift = false;
            bool binaryShift = false;

            while (!end)
            {

                if (shift)
                {
                    // the table is for the next character only
                    switchShift = true;
                }
                else
                {
                    // save the current table in case next one is a shift
                    lastTable = table;
                }

                int code;
                if (binaryShift)
                {
                    if (endIndex - startIndex < 5)
                    {
                        break;
                    }

                    int length = ReadCode(correctedBits, startIndex, 5);
                    startIndex += 5;
                    if (length == 0)
                    {
                        if (endIndex - startIndex < 11)
                        {
                            break;
                        }

                        length = ReadCode(correctedBits, startIndex, 11) + 31;
                        startIndex += 11;
                    }
                    for (int charCount = 0; charCount < length; charCount++)
                    {
                        if (endIndex - startIndex < 8)
                        {
                            end = true;
                            break;
                        }

                        code = ReadCode(correctedBits, startIndex, 8);
                        result.Append((char)code);
                        startIndex += 8;
                    }
                    binaryShift = false;
                }
                else
                {
                    if (table == MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.BINARY)
                    {
                        if (endIndex - startIndex < 8)
                        {
                            break;
                        }
                        code = ReadCode(correctedBits, startIndex, 8);
                        startIndex += 8;

                        result.Append((char)code);

                    }
                    else
                    {
                        int size = 5;

                        if (table == MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.DIGIT)
                        {
                            size = 4;
                        }

                        if (endIndex - startIndex < size)
                        {
                            break;
                        }

                        code = ReadCode(correctedBits, startIndex, size);
                        startIndex += size;

                        String str = GetCharacter(table, code);
                        if (str.StartsWith("CTRL_"))
                        {
                            // Table changes
                            table = GetTable(str[5]);

                            if (str[6] == 'S')
                            {
                                shift = true;
                                if (str[5] == 'B')
                                {
                                    binaryShift = true;
                                }
                            }
                        }
                        else
                        {
                            result.Append(str);
                        }

                    }
                }

                if (switchShift)
                {
                    table = lastTable;
                    shift = false;
                    switchShift = false;
                }

            }
            return result.ToString();
        }

        /// <summary>
        /// gets the table corresponding to the char passed
        /// </summary>
        ///
        private static Decoder.Table GetTable(char t)
        {
            switch ((int)t)
            {
                case 'L':
                    return MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.LOWER;
                case 'P':
                    return MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.PUNCT;
                case 'M':
                    return MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.MIXED;
                case 'D':
                    return MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.DIGIT;
                case 'B':
                    return MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.BINARY;
                case 'U':
                default:
                    return MessagingToolkit.Barcode.Aztec.Decoder.Decoder.Table.UPPER;
            }
        }

        /// <summary>
        /// Gets the character (or string) corresponding to the passed code in the given table
        /// </summary>
        ///
        /// <param name="table">the table used</param>
        /// <param name="code">the code of the character</param>
        private static String GetCharacter(Decoder.Table table, int code)
        {
            switch (table)
            {
                case Table.UPPER:
                    return UPPER_TABLE[code];
                case Table.LOWER:
                    return LOWER_TABLE[code];
                case Table.MIXED:
                    return MIXED_TABLE[code];
                case Table.PUNCT:
                    return PUNCT_TABLE[code];
                case Table.DIGIT:
                    return DIGIT_TABLE[code];
                default:
                    return "";
            }
        }

        /// <summary>
        /// <p>Performs RS error correction on an array of bits.</p>
        /// </summary>
        ///
        /// <returns>the corrected array</returns>
        /// <exception cref="FormatException">if the input contains too many errors</exception>
        private bool[] CorrectBits(bool[] rawbits)
        {
            GenericGF gf;

            if (ddata.GetNbLayers() <= 2)
            {
                codewordSize = 6;
                gf = MessagingToolkit.Barcode.Common.ReedSolomon.GenericGF.AztecData6;
            }
            else if (ddata.GetNbLayers() <= 8)
            {
                codewordSize = 8;
                gf = MessagingToolkit.Barcode.Common.ReedSolomon.GenericGF.AztecData8;
            }
            else if (ddata.GetNbLayers() <= 22)
            {
                codewordSize = 10;
                gf = MessagingToolkit.Barcode.Common.ReedSolomon.GenericGF.AztecData10;
            }
            else
            {
                codewordSize = 12;
                gf = MessagingToolkit.Barcode.Common.ReedSolomon.GenericGF.AztecData12;
            }

            int numDataCodewords = ddata.GetNbDatablocks();
            int numECCodewords;
            int offset;

            if (ddata.IsCompact())
            {
                offset = NB_BITS_COMPACT[ddata.GetNbLayers()] - numCodewords * codewordSize;
                numECCodewords = NB_DATABLOCK_COMPACT[ddata.GetNbLayers()] - numDataCodewords;
            }
            else
            {
                offset = NB_BITS[ddata.GetNbLayers()] - numCodewords * codewordSize;
                numECCodewords = NB_DATABLOCK[ddata.GetNbLayers()] - numDataCodewords;
            }

            int[] dataWords = new int[numCodewords];
            for (int i = 0; i < numCodewords; i++)
            {
                int flag = 1;
                for (int j = 1; j <= codewordSize; j++)
                {
                    if (rawbits[codewordSize * i + codewordSize - j + offset])
                    {
                        dataWords[i] += flag;
                    }
                    flag <<= 1;
                }

                //if (dataWords[i] >= flag) {
                //  flag++;
                //}
            }

            try
            {
                ReedSolomonDecoder rsDecoder = new ReedSolomonDecoder(gf);
                rsDecoder.Decode(dataWords, numECCodewords);
            }
            catch (ReedSolomonException rse)
            {
                throw FormatException.Instance;
            }

            offset = 0;
            invertedBitCount = 0;

            bool[] correctedBits = new bool[numDataCodewords * codewordSize];
            for (int i_0 = 0; i_0 < numDataCodewords; i_0++)
            {

                bool seriesColor = false;
                int seriesCount = 0;
                int flag_1 = 1 << (codewordSize - 1);

                for (int j_2 = 0; j_2 < codewordSize; j_2++)
                {

                    bool color = (dataWords[i_0] & flag_1) == flag_1;

                    if (seriesCount == codewordSize - 1)
                    {

                        if (color == seriesColor)
                        {
                            //bit must be inverted
                            throw FormatException.Instance;
                        }

                        seriesColor = false;
                        seriesCount = 0;
                        offset++;
                        invertedBitCount++;
                    }
                    else
                    {

                        if (seriesColor == color)
                        {
                            seriesCount++;
                        }
                        else
                        {
                            seriesCount = 1;
                            seriesColor = color;
                        }

                        correctedBits[i_0 * codewordSize + j_2 - offset] = color;

                    }

                    flag_1 = (int)(((uint)flag_1) >> 1);
                }
            }

            return correctedBits;
        }

        /// <summary>
        /// Gets the array of bits from an Aztec Code matrix
        /// </summary>
        ///
        /// <returns>the array of bits</returns>
        /// <exception cref="FormatException">if the matrix is not a valid aztec code</exception>
        private bool[] ExtractBits(BitMatrix matrix)
        {

            bool[] rawbits;
            if (ddata.IsCompact())
            {
                if (ddata.GetNbLayers() > NB_BITS_COMPACT.Length)
                {
                    throw FormatException.Instance;
                }
                rawbits = new bool[NB_BITS_COMPACT[ddata.GetNbLayers()]];
                numCodewords = NB_DATABLOCK_COMPACT[ddata.GetNbLayers()];
            }
            else
            {
                if (ddata.GetNbLayers() > NB_BITS.Length)
                {
                    throw FormatException.Instance;
                }
                rawbits = new bool[NB_BITS[ddata.GetNbLayers()]];
                numCodewords = NB_DATABLOCK[ddata.GetNbLayers()];
            }

            int layer = ddata.GetNbLayers();
            int size = matrix.GetHeight();
            int rawbitsOffset = 0;
            int matrixOffset = 0;

            while (layer != 0)
            {

                int flip = 0;
                for (int i = 0; i < 2 * size - 4; i++)
                {
                    rawbits[rawbitsOffset + i] = matrix.Get(matrixOffset + flip, matrixOffset + i / 2);
                    rawbits[rawbitsOffset + 2 * size - 4 + i] = matrix.Get(matrixOffset + i / 2, matrixOffset + size - 1 - flip);
                    flip = (flip + 1) % 2;
                }

                flip = 0;
                for (int i_0 = 2 * size + 1; i_0 > 5; i_0--)
                {
                    rawbits[rawbitsOffset + 4 * size - 8 + (2 * size - i_0) + 1] = matrix.Get(matrixOffset + size - 1 - flip, matrixOffset + i_0 / 2 - 1);
                    rawbits[rawbitsOffset + 6 * size - 12 + (2 * size - i_0) + 1] = matrix.Get(matrixOffset + i_0 / 2 - 1, matrixOffset + flip);
                    flip = (flip + 1) % 2;
                }

                matrixOffset += 2;
                rawbitsOffset += 8 * size - 16;
                layer--;
                size -= 4;
            }

            return rawbits;
        }

        /// <summary>
        /// Transforms an Aztec code matrix by removing the control dashed lines
        /// </summary>
        ///
        private static BitMatrix RemoveDashedLines(BitMatrix matrix)
        {
            int nbDashed = 1 + 2 * ((matrix.GetWidth() - 1) / 2 / 16);
            BitMatrix newMatrix = new BitMatrix(matrix.GetWidth() - nbDashed, matrix.GetHeight() - nbDashed);

            int nx = 0;

            for (int x = 0; x < matrix.GetWidth(); x++)
            {

                if ((matrix.GetWidth() / 2 - x) % 16 == 0)
                {
                    continue;
                }

                int ny = 0;
                for (int y = 0; y < matrix.GetHeight(); y++)
                {

                    if ((matrix.GetWidth() / 2 - y) % 16 == 0)
                    {
                        continue;
                    }

                    if (matrix.Get(x, y))
                    {
                        newMatrix.Set(nx, ny);
                    }
                    ny++;
                }
                nx++;
            }

            return newMatrix;
        }

        /// <summary>
        /// Reads a code of given length and at given index in an array of bits
        /// </summary>
        ///
        private static int ReadCode(bool[] rawbits, int startIndex, int length)
        {
            int res = 0;

            for (int i = startIndex; i < startIndex + length; i++)
            {
                res <<= 1;
                if (rawbits[i])
                {
                    res++;
                }
            }

            return res;
        }
    }
}
