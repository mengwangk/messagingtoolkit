using System;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.ReedSolomon;

namespace MessagingToolkit.Barcode.Aztec.Encoder
{
    /// <summary>
    /// Generates Aztec 2D barcodes.
    /// </summary>
    public static class Encoder
    {
        public const int DEFAULT_EC_PERCENT = 33; // default minimal percentage of error check words

        private static readonly int[] NB_BITS; // total bits per compact symbol for a given number of layers
        private static readonly int[] NB_BITS_COMPACT; // total bits per full symbol for a given number of layers

        static Encoder()
        {
            NB_BITS_COMPACT = new int[5];
            for (int i = 1; i < NB_BITS_COMPACT.Length; i++)
            {
                NB_BITS_COMPACT[i] = (88 + 16 * i) * i;
            }
            NB_BITS = new int[33];
            for (int i = 1; i < NB_BITS.Length; i++)
            {
                NB_BITS[i] = (112 + 16 * i) * i;
            }
        }

        private static readonly int[] WORD_SIZE = {
                                                   4, 6, 6, 8, 8, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                                                   12, 12, 12, 12, 12, 12, 12, 12, 12, 12
                                                };

        /// <summary>
        /// Encodes the given binary content as an Aztec symbol
        /// </summary>
        /// <param name="data">input data string</param>
        /// <returns>Aztec symbol matrix with metadata</returns>

        public static AztecCode Encode(byte[] data)
        {
            return Encode(data, DEFAULT_EC_PERCENT);
        }

        /// <summary>
        /// Encodes the given binary content as an Aztec symbol
        /// </summary>
        /// <param name="data">input data string</param>
        /// <param name="minECCPercent">minimal percentange of error check words (According to ISO/IEC 24778:2008,
        /// a minimum of 23% + 3 words is recommended)</param>
        /// <returns>Aztec symbol matrix with metadata</returns>
        public static AztecCode Encode(byte[] data, int minECCPercent)
        {
            // High-level encode
            BitArray bits = new HighLevelEncoder(data).Encode();

            // stuff bits and choose symbol size
            int eccBits = bits.Size * minECCPercent / 100 + 11;
            int totalSizeBits = bits.Size + eccBits;
            int layers;
            int wordSize = 0;
            int totalSymbolBits = 0;
            BitArray stuffedBits = null;
            for (layers = 1; layers < NB_BITS_COMPACT.Length; layers++)
            {
                if (NB_BITS_COMPACT[layers] >= totalSizeBits)
                {
                    if (wordSize != WORD_SIZE[layers])
                    {
                        wordSize = WORD_SIZE[layers];
                        stuffedBits = StuffBits(bits, wordSize);
                    }
                    totalSymbolBits = NB_BITS_COMPACT[layers];
                    if (stuffedBits.Size + eccBits <= NB_BITS_COMPACT[layers])
                    {
                        break;
                    }
                }
            }
            bool compact = true;
            if (layers == NB_BITS_COMPACT.Length)
            {
                compact = false;
                for (layers = 1; layers < NB_BITS.Length; layers++)
                {
                    if (NB_BITS[layers] >= totalSizeBits)
                    {
                        if (wordSize != WORD_SIZE[layers])
                        {
                            wordSize = WORD_SIZE[layers];
                            stuffedBits = StuffBits(bits, wordSize);
                        }
                        totalSymbolBits = NB_BITS[layers];
                        if (stuffedBits.Size + eccBits <= NB_BITS[layers])
                        {
                            break;
                        }
                    }
                }
            }
            if (layers == NB_BITS.Length)
            {
                throw new ArgumentException("Data too large for an Aztec code");
            }

            // pad the end
            int messageSizeInWords = (stuffedBits.Size + wordSize - 1) / wordSize;

            for (int i = messageSizeInWords * wordSize - stuffedBits.Size; i > 0; i--)
            {
                stuffedBits.AppendBit(true);
            }

            // generate check words
            var rs = new ReedSolomonEncoder(GetGF(wordSize));
            var totalSizeInFullWords = totalSymbolBits / wordSize;
            var messageWords = BitsToWords(stuffedBits, wordSize, totalSizeInFullWords);
            rs.Encode(messageWords, totalSizeInFullWords - messageSizeInWords);

            // convert to bit array and pad in the beginning
            var startPad = totalSymbolBits % wordSize;
            var messageBits = new BitArray();
            messageBits.AppendBits(0, startPad);
            foreach (var messageWord in messageWords)
            {
                messageBits.AppendBits(messageWord, wordSize);
            }

            // generate mode message
            BitArray modeMessage = GenerateModeMessage(compact, layers, messageSizeInWords);

            // allocate symbol
            var baseMatrixSize = compact ? 11 + layers * 4 : 14 + layers * 4; // not including alignment lines
            var alignmentMap = new int[baseMatrixSize];
            int matrixSize;
            if (compact)
            {
                // no alignment marks in compact mode, alignmentMap is a no-op
                matrixSize = baseMatrixSize;
                for (int i = 0; i < alignmentMap.Length; i++)
                {
                    alignmentMap[i] = i;
                }
            }
            else
            {
                matrixSize = baseMatrixSize + 1 + 2 * ((baseMatrixSize / 2 - 1) / 15);
                int origCenter = baseMatrixSize / 2;
                int center = matrixSize / 2;
                for (int i = 0; i < origCenter; i++)
                {
                    int newOffset = i + i / 15;
                    alignmentMap[origCenter - i - 1] = center - newOffset - 1;
                    alignmentMap[origCenter + i] = center + newOffset + 1;
                }
            }
            BitMatrix matrix = new BitMatrix(matrixSize);

            // draw mode and data bits
            for (int i = 0, rowOffset = 0; i < layers; i++)
            {
                int rowSize = compact ? (layers - i) * 4 + 9 : (layers - i) * 4 + 12;
                for (int j = 0; j < rowSize; j++)
                {
                    int columnOffset = j * 2;
                    for (int k = 0; k < 2; k++)
                    {
                        if (messageBits.Get(rowOffset + columnOffset + k))
                        {
                            matrix.Set(alignmentMap[i * 2 + k], alignmentMap[i * 2 + j]);
                        }
                        if (messageBits.Get(rowOffset + rowSize * 2 + columnOffset + k))
                        {
                            matrix.Set(alignmentMap[i * 2 + j], alignmentMap[baseMatrixSize - 1 - i * 2 - k]);
                        }
                        if (messageBits.Get(rowOffset + rowSize * 4 + columnOffset + k))
                        {
                            matrix.Set(alignmentMap[baseMatrixSize - 1 - i * 2 - k], alignmentMap[baseMatrixSize - 1 - i * 2 - j]);
                        }
                        if (messageBits.Get(rowOffset + rowSize * 6 + columnOffset + k))
                        {
                            matrix.Set(alignmentMap[baseMatrixSize - 1 - i * 2 - j], alignmentMap[i * 2 + k]);
                        }
                    }
                }
                rowOffset += rowSize * 8;
            }
            DrawModeMessage(matrix, compact, matrixSize, modeMessage);

            // draw alignment marks
            if (compact)
            {
                DrawBullsEye(matrix, matrixSize / 2, 5);
            }
            else
            {
                DrawBullsEye(matrix, matrixSize / 2, 7);
                for (int i = 0, j = 0; i < baseMatrixSize / 2 - 1; i += 15, j += 16)
                {
                    for (int k = (matrixSize / 2) & 1; k < matrixSize; k += 2)
                    {
                        matrix.Set(matrixSize / 2 - j, k);
                        matrix.Set(matrixSize / 2 + j, k);
                        matrix.Set(k, matrixSize / 2 - j);
                        matrix.Set(k, matrixSize / 2 + j);
                    }
                }
            }
            AztecCode aztec = new AztecCode();
            aztec.Compact = compact;
            aztec.Size = matrixSize;
            aztec.Layers = layers;
            aztec.CodeWords = messageSizeInWords;
            aztec.Matrix = matrix;
            return aztec;
        }

        private static void DrawBullsEye(BitMatrix matrix, int center, int size)
        {
            for (var i = 0; i < size; i += 2)
            {
                for (var j = center - i; j <= center + i; j++)
                {
                    matrix.Set(j, center - i);
                    matrix.Set(j, center + i);
                    matrix.Set(center - i, j);
                    matrix.Set(center + i, j);
                }
            }
            matrix.Set(center - size, center - size);
            matrix.Set(center - size + 1, center - size);
            matrix.Set(center - size, center - size + 1);
            matrix.Set(center + size, center - size);
            matrix.Set(center + size, center - size + 1);
            matrix.Set(center + size, center + size - 1);
        }

        internal static BitArray GenerateModeMessage(bool compact, int layers, int messageSizeInWords)
        {
            var modeMessage = new BitArray();
            if (compact)
            {
                modeMessage.AppendBits(layers - 1, 2);
                modeMessage.AppendBits(messageSizeInWords - 1, 6);
                modeMessage = GenerateCheckWords(modeMessage, 28, 4);
            }
            else
            {
                modeMessage.AppendBits(layers - 1, 5);
                modeMessage.AppendBits(messageSizeInWords - 1, 11);
                modeMessage = GenerateCheckWords(modeMessage, 40, 4);
            }
            return modeMessage;
        }

        private static void DrawModeMessage(BitMatrix matrix, bool compact, int matrixSize, BitArray modeMessage)
        {
            if (compact)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (modeMessage.Get(i))
                    {
                        matrix.Set(matrixSize / 2 - 3 + i, matrixSize / 2 - 5);
                    }
                    if (modeMessage.Get(i + 7))
                    {
                        matrix.Set(matrixSize / 2 + 5, matrixSize / 2 - 3 + i);
                    }
                    if (modeMessage.Get(20 - i))
                    {
                        matrix.Set(matrixSize / 2 - 3 + i, matrixSize / 2 + 5);
                    }
                    if (modeMessage.Get(27 - i))
                    {
                        matrix.Set(matrixSize / 2 - 5, matrixSize / 2 - 3 + i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (modeMessage.Get(i))
                    {
                        matrix.Set(matrixSize / 2 - 5 + i + i / 5, matrixSize / 2 - 7);
                    }
                    if (modeMessage.Get(i + 10))
                    {
                        matrix.Set(matrixSize / 2 + 7, matrixSize / 2 - 5 + i + i / 5);
                    }
                    if (modeMessage.Get(29 - i))
                    {
                        matrix.Set(matrixSize / 2 - 5 + i + i / 5, matrixSize / 2 + 7);
                    }
                    if (modeMessage.Get(39 - i))
                    {
                        matrix.Set(matrixSize / 2 - 7, matrixSize / 2 - 5 + i + i / 5);
                    }
                }
            }
        }

        private static BitArray GenerateCheckWords(BitArray stuffedBits, int totalSymbolBits, int wordSize)
        {
            var messageSizeInWords = (stuffedBits.Size + wordSize - 1) / wordSize;
            for (var i = messageSizeInWords * wordSize - stuffedBits.Size; i > 0; i--)
            {
                stuffedBits.AppendBit(true);
            }

            var rs = new ReedSolomonEncoder(GetGF(wordSize));
            var totalSizeInFullWords = totalSymbolBits / wordSize;
            var messageWords = BitsToWords(stuffedBits, wordSize, totalSizeInFullWords);
            rs.Encode(messageWords, totalSizeInFullWords - messageSizeInWords);

            var startPad = totalSymbolBits % wordSize;
            var messageBits = new BitArray();
            messageBits.AppendBits(0, startPad);
            foreach (var messageWord in messageWords)
            {
                messageBits.AppendBits(messageWord, wordSize);
            }
            return messageBits;
        }

        private static int[] BitsToWords(BitArray stuffedBits, int wordSize, int totalWords)
        {
            var message = new int[totalWords];
            int i;
            int n;
            for (i = 0, n = stuffedBits.Size / wordSize; i < n; i++)
            {
                int value = 0;
                for (int j = 0; j < wordSize; j++)
                {
                    value |= stuffedBits.Get(i * wordSize + j) ? (1 << wordSize - j - 1) : 0;
                }
                message[i] = value;
            }
            return message;
        }

        private static GenericGF GetGF(int wordSize)
        {
            switch (wordSize)
            {
                case 4:
                    return GenericGF.AztecParam;
                case 6:
                    return GenericGF.AztecData6;
                case 8:
                    return GenericGF.AztecData8;
                case 10:
                    return GenericGF.AztecData10;
                case 12:
                    return GenericGF.AztecData12;
                default:
                    return null;
            }
        }

        internal static BitArray StuffBits(BitArray bits, int wordSize)
        {
            var output = new BitArray();

            // 1. stuff the bits
            int n = bits.Size;
            int mask = (1 << wordSize) - 2;
            for (int i = 0; i < n; i += wordSize)
            {
                int word = 0;
                for (int j = 0; j < wordSize; j++)
                {
                    if (i + j >= n || bits.Get(i + j))
                    {
                        word |= 1 << (wordSize - 1 - j);
                    }
                }
                if ((word & mask) == mask)
                {
                    output.AppendBits(word & mask, wordSize);
                    i--;
                }
                else if ((word & mask) == 0)
                {
                    output.AppendBits(word | 1, wordSize);
                    i--;
                }
                else
                {
                    output.AppendBits(word, wordSize);
                }
            }

            // 2. pad last word to wordSize
            // This seems to be redundant?
            n = output.Size;
            int remainder = n % wordSize;
            if (remainder != 0)
            {
                int j = 1;
                for (int i = 0; i < remainder; i++)
                {
                    if (!output.Get(n - 1 - i))
                    {
                        j = 0;
                    }
                }
                for (int i = remainder; i < wordSize - 1; i++)
                {
                    output.AppendBit(true);
                }
                output.AppendBit(j == 0);
            }
            return output;
        }

    }
}