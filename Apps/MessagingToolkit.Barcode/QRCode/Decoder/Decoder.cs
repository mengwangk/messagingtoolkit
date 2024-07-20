using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.ReedSolomon;

using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{

    /// <summary>
    ///   <p>The main class which implements QR Code decoding -- as opposed to locating and extracting
    /// the QR Code from an image.</p>
    /// 
    /// Modified: April 26 2012
    /// </summary>
    public sealed class Decoder
    {

        private readonly ReedSolomonDecoder rsDecoder;

        public Decoder()
        {
            rsDecoder = new ReedSolomonDecoder(GenericGF.QRCodeField256);
        }

        public DecoderResult Decode(bool[][] image)
        {
            return Decode(image, null);
        }

        /// <summary>
        /// <p>Convenience method that can decode a QR Code represented as a 2D array of booleans.
        /// "true" is taken to mean a black module.</p>
        /// </summary>
        ///
        /// <param name="image">booleans representing white/black QR Code modules</param>
        /// <returns>text and bytes encoded within the QR Code</returns>
        /// <exception cref="FormatException">if the QR Code cannot be decoded</exception>
        /// <exception cref="ChecksumException">if error correction fails</exception>
        public DecoderResult Decode(bool[][] image, Dictionary<DecodeOptions, object> decodingOptions)
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
            return Decode(bits, decodingOptions);
        }

        public DecoderResult Decode(BitMatrix bits)
        {
            return Decode(bits, null);
        }

        /// <summary>
        /// <p>Decodes a QR Code represented as a <see cref="null"/>. A 1 or "true" is taken to mean a black module.</p>
        /// </summary>
        ///
        /// <param name="bits">booleans representing white/black QR Code modules</param>
        /// <returns>text and bytes encoded within the QR Code</returns>
        /// <exception cref="FormatException">if the QR Code cannot be decoded</exception>
        /// <exception cref="ChecksumException">if error correction fails</exception>
        public DecoderResult Decode(BitMatrix bits, Dictionary<DecodeOptions, object> decodingOptions)
        {
            // Construct a parser and read version, error-correction level
            BitMatrixParser parser = new BitMatrixParser(bits);

            FormatException fe = null;
            ChecksumException ce = null;
            try
            {
                return Decode(parser, decodingOptions);
            }
            catch (FormatException e)
            {
                fe = e;
            }
            catch (ChecksumException e)
            {
                ce = e;
            }

            try
            {

                // Revert the bit matrix
                parser.Remask();

                // Will be attempting a mirrored reading of the version and format info.
                parser.SetMirror(true);

                // Preemptively read the version.
                parser.ReadVersion();

                // Preemptively read the format information.
                parser.ReadFormatInformation();

                // Since we're here, this means we have successfully detected some kind
                // of version and format information when mirrored. This is a good sign,
                // that the QR code may be mirrored, and we should try once more with a
                // mirrored content.
                //
                // Prepare for a mirrored reading.
                parser.Mirror();

                DecoderResult result = Decode(parser, decodingOptions);

                // Success! Notify the caller that the code was mirrored.
                result.Other = new QRCodeDecoderMetaData(true);
                return result;

            }
            catch (FormatException e)
            {
                // Throw the exception from the original reading
                if (fe != null)
                {
                    throw fe;
                }
                if (ce != null)
                {
                    throw ce;
                }
                throw e;

            }
            catch (ChecksumException e)
            {
                // Throw the exception from the original reading
                if (fe != null)
                {
                    throw fe;
                }
                if (ce != null)
                {
                    throw ce;
                }
                throw e;

            }
        }

        private DecoderResult Decode(BitMatrixParser parser, Dictionary<DecodeOptions, object> decodingOptions)
        {

            Version version = parser.ReadVersion();
            ErrorCorrectionLevel ecLevel = parser.ReadFormatInformation().GetErrorCorrectionLevel();

            // Read codewords
            byte[] codewords = parser.ReadCodewords();
            // Separate into data blocks
            DataBlock[] dataBlocks = DataBlock.GetDataBlocks(codewords, version, ecLevel);

            // Count total number of data bytes
            int totalBytes = 0;
            /* foreach */
            foreach (DataBlock dataBlock in dataBlocks)
            {
                totalBytes += dataBlock.NumDataCodewords;
            }
            byte[] resultBytes = new byte[totalBytes];
            int resultOffset = 0;

            /* foreach */
            // Error-correct and copy data blocks together into a stream of bytes
            foreach (DataBlock dataBlock_0 in dataBlocks)
            {
                byte[] codewordBytes = dataBlock_0.Codewords;
                int numDataCodewords = dataBlock_0.NumDataCodewords;
                CorrectErrors(codewordBytes, numDataCodewords);
                for (int i = 0; i < numDataCodewords; i++)
                {
                    resultBytes[resultOffset++] = codewordBytes[i];
                }
            }

            // Decode the contents of that stream of bytes
            return DecodedBitStreamParser.Decode(resultBytes, version, ecLevel, decodingOptions);
        }

        /// <summary>
        ///   <p>Given data and error-correction codewords received, possibly corrupted by errors, attempts to
        /// correct the errors in-place using Reed-Solomon error correction.</p>
        /// </summary>
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
            catch (ReedSolomonException)
            {
                throw ChecksumException.Instance;
            }
            // Copy back into array of bytes -- only need to worry about the bytes that were data
            // We don't care about errors in the error-correction codewords
            for (int i = 0; i < numDataCodewords; i++)
            {
                codewordBytes[i] = (byte)codewordsInts[i];
            }
        }

    }
}
