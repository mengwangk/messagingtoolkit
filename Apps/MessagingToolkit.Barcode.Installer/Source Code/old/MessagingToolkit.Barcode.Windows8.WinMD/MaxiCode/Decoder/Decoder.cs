using System;
using System.Text;
using System.Collections.Generic;

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.ReedSolomon;

namespace MessagingToolkit.Barcode.MaxiCode.Decoder
{

    /// <summary>
    /// <p>The main class which implements MaxiCode decoding -- as opposed to locating and extracting
    /// the MaxiCode from an image.</p>
    /// </summary>
    internal sealed class Decoder
    {

        private const int ALL = 0;
        private const int EVEN = 1;
        private const int ODD = 2;

        private readonly ReedSolomonDecoder rsDecoder;

        public Decoder()
        {
            rsDecoder = new ReedSolomonDecoder(GenericGF.MaxicodeField64);
        }

        public DecoderResult Decode(BitMatrix bits)
        {
            return Decode(bits, null);
        }

        public DecoderResult Decode(BitMatrix bits, IDictionary<DecodeOptions, object> decodingOptions)
        {
            BitMatrixParser parser = new BitMatrixParser(bits);
            byte[] codewords = parser.ReadCodeWords();

            CorrectErrors(codewords, 0, 10, 10, ALL);
            int mode = codewords[0] & 0x0F;
            byte[] datawords;
            switch (mode)
            {
                case 2:
                case 3:
                case 4:
                    CorrectErrors(codewords, 20, 84, 40, EVEN);
                    CorrectErrors(codewords, 20, 84, 40, ODD);
                    datawords = new byte[94];
                    break;
                case 5:
                    CorrectErrors(codewords, 20, 68, 56, EVEN);
                    CorrectErrors(codewords, 20, 68, 56, ODD);
                    datawords = new byte[78];
                    break;
                default:
                    throw FormatException.Instance;
            }

            Array.Copy(codewords, 0, datawords, 0, 10);
            Array.Copy(codewords, 20, datawords, 10, datawords.Length - 10);

            return DecodedBitStreamParser.decode(datawords, mode);
        }

        private void CorrectErrors(byte[] codewordBytes, int start, int dataCodewords, int ecCodewords, int mode)
        {
            int codewords = dataCodewords + ecCodewords;

            // in EVEN or ODD mode only half the codewords
            int divisor = (mode == ALL) ? 1 : 2;

            // First read into an array of ints
            int[] codewordsInts = new int[codewords / divisor];
            for (int i = 0; i < codewords; i++)
            {
                if ((mode == ALL) || (i % 2 == (mode - 1)))
                {
                    codewordsInts[i / divisor] = codewordBytes[i + start] & 0xFF;
                }
            }
            try
            {
                rsDecoder.Decode(codewordsInts, ecCodewords / divisor);
            }
            catch (ReedSolomonException rse)
            {
                throw ChecksumException.Instance;
            }
            // Copy back into array of bytes -- only need to worry about the bytes that were data
            // We don't care about errors in the error-correction codewords
            for (int i = 0; i < dataCodewords; i++)
            {
                if ((mode == ALL) || (i % 2 == (mode - 1)))
                {
                    codewordBytes[i + start] = (byte)codewordsInts[i / divisor];
                }
            }
        }

    }
}
