using System;

using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;
using DecoderResult = MessagingToolkit.Barcode.Common.DecoderResult;
using GenericGF = MessagingToolkit.Barcode.Common.ReedSolomon.GenericGF;
using ReedSolomonDecoder = MessagingToolkit.Barcode.Common.ReedSolomon.ReedSolomonDecoder;
using ReedSolomonException = MessagingToolkit.Barcode.Common.ReedSolomon.ReedSolomonException;

namespace MessagingToolkit.Barcode.DataMatrix.Decoders
{
    /// <summary>
    /// <p>The main class which implements Data Matrix Code decoding -- as opposed to locating and extracting
    /// the Data Matrix Code from an image.</p>
    /// 
    /// Modified: May 10 2012
    /// </summary>
    internal sealed class Decoder
    {
        private readonly ReedSolomonDecoder rsDecoder;

        public Decoder()
        {
            rsDecoder = new ReedSolomonDecoder(GenericGF.DataMatrixField256);
        }

        /// <summary>
        /// <p>Convenience method that can decode a Data Matrix Code represented as a 2D array of booleans.
        /// "true" is taken to mean a black module.</p>
        /// </summary>
        ///
        /// <param name="image">booleans representing white/black Data Matrix Code modules</param>
        /// <returns>text and bytes encoded within the Data Matrix Code</returns>
        /// <exception cref="FormatException">if the Data Matrix Code cannot be decoded</exception>
        /// <exception cref="ChecksumException">if error correction fails</exception>
        public DecoderResult Decode(bool[][] image)
        {
            int dimension = image.Length;
            BitMatrix bits = new BitMatrix(dimension);
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    if (image[i][j])
                    {
                        bits.Set(j, i);
                    }
                }
            }
            return Decode(bits);
        }

        /// <summary>
        /// <p>Decodes a Data Matrix Code represented as a A 1 or "true" is taken
        /// to mean a black module.</p>
        /// </summary>
        ///
        /// <param name="bits">booleans representing white/black Data Matrix Code modules</param>
        /// <returns>text and bytes encoded within the Data Matrix Code</returns>
        /// <exception cref="FormatException">if the Data Matrix Code cannot be decoded</exception>
        /// <exception cref="ChecksumException">if error correction fails</exception>
        public DecoderResult Decode(BitMatrix bits)
        {

            // Construct a parser and read version, error-correction level
            BitMatrixParser parser = new BitMatrixParser(bits);
            Version version = parser.GetVersion();

            // Read codewords
            byte[] codewords = parser.ReadCodewords();
            // Separate into data blocks
            DataBlock[] dataBlocks = DataBlock.GetDataBlocks(codewords, version);

            int dataBlocksCount = dataBlocks.Length;

            // Count total number of data bytes
            int totalBytes = 0;
            for (int i = 0; i < dataBlocksCount; i++)
            {
                totalBytes += dataBlocks[i].GetNumDataCodewords();
            }
            byte[] resultBytes = new byte[totalBytes];

            // Error-correct and copy data blocks together into a stream of bytes
            for (int j = 0; j < dataBlocksCount; j++)
            {
                DataBlock dataBlock = dataBlocks[j];
                byte[] codewordBytes = dataBlock.GetCodewords();
                int numDataCodewords = dataBlock.GetNumDataCodewords();
                CorrectErrors(codewordBytes, numDataCodewords);
                for (int i = 0; i < numDataCodewords; i++)
                {
                    // De-interlace data blocks.
                    resultBytes[i * dataBlocksCount + j] = codewordBytes[i];
                }
            }


            // Decode the contents of that stream of bytes
            return DecodedBitStreamParser.Decode(resultBytes);
        }

        /// <summary>
        /// <p>Given data and error-correction codewords received, possibly corrupted by errors, attempts to
        /// correct the errors in-place using Reed-Solomon error correction.</p>
        /// </summary>
        ///
        /// <param name="codewordBytes">data and error correction codewords</param>
        /// <param name="numDataCodewords">number of codewords that are data bytes</param>
        /// <exception cref="ChecksumException">if error correction fails</exception>
        private void CorrectErrors(byte[] codewordBytes, int numDataCodewords)
        {
            int numCodewords = codewordBytes.Length;
            // First read into an array of ints
            int[] codewordsInts = new int[numCodewords];
            for (int i = 0; i < numCodewords; i++)
            {
                codewordsInts[i] = codewordBytes[i] & 0xFF;
            }
            int numECCodewords = codewordBytes.Length - numDataCodewords;
            try
            {
                rsDecoder.Decode(codewordsInts, numECCodewords);
            }
            catch (ReedSolomonException rse)
            {
                throw ChecksumException.Instance;
            }
            // Copy back into array of bytes -- only need to worry about the bytes that were data
            // We don't care about errors in the error-correction codewords
            for (int i_0 = 0; i_0 < numDataCodewords; i_0++)
            {
                codewordBytes[i_0] = (byte)codewordsInts[i_0];
            }
        }

    }
}
