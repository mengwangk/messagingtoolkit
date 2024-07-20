using System;

using ReaderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;
using DecoderResult = MessagingToolkit.Barcode.Common.DecoderResult;
using ErrorCorrection = MessagingToolkit.Barcode.Pdf417.Decoder.Ec.ErrorCorrection;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{

    /// <summary>
    /// The main class which implements PDF417 Code decoding -- as
    /// opposed to locating and extracting the PDF417 Code from an image.
    /// </summary>
    internal sealed class Decoder
    {
        private const int MAX_ERRORS = 3;
        private const int MAX_EC_CODEWORDS = 512;
        private readonly ErrorCorrection errorCorrection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoder"/> class.
        /// </summary>
        public Decoder()
        {
            errorCorrection = new ErrorCorrection();
        }

        /// <summary>
        ///   <p>Convenience method that can decode a PDF417 Code represented as a 2D array of booleans.
        /// "true" is taken to mean a black module.</p>
        /// </summary>
        /// <param name="image">booleans representing white/black PDF417 modules</param>
        /// <returns>
        /// text and bytes encoded within the PDF417 Code
        /// </returns>
        public DecoderResult Decode(bool[][] image)
        {
            int dimension = image.Length;
            BitMatrix bits = new BitMatrix(dimension);
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    if (image[j][i])
                    {
                        bits.Set(j, i);
                    }
                }
            }
            return Decode(bits);
        }

        /// <summary>
        /// <p>Decodes a PDF417 Code represented as a <see cref="BitMatrix"/>.
        /// A 1 or "true" is taken to mean a black module.</p>
        /// </summary>
        ///
        /// <param name="bits">booleans representing white/black PDF417 Code modules</param>
        /// <returns>text and bytes encoded within the PDF417 Code</returns>
        /// <exception cref="FormatException">if the PDF417 Code cannot be decoded</exception>
        public DecoderResult Decode(BitMatrix bits)
        {
            // Construct a parser to read the data codewords and error-correction level
            BitMatrixParser parser = new BitMatrixParser(bits);
            int[] codewords = parser.ReadCodewords();
            if (codewords.Length == 0)
            {
                throw FormatException.Instance;
            }

            int ecLevel = parser.ECLevel;
            int numECCodewords = 1 << (ecLevel + 1);
            int[] erasures = parser.Erasures;

            CorrectErrors(codewords, erasures, numECCodewords);
            VerifyCodewordCount(codewords, numECCodewords);

            // Decode the codewords
            return DecodedBitStreamParser.Decode(codewords, ecLevel.ToString(), erasures.Length);
        }

        /// <summary>
        /// Verify that all is OK with the codeword array.
        /// </summary>
        ///
        /// <param name="codewords"></param>
        /// <returns>an index to the first data codeword.</returns>
        private static void VerifyCodewordCount(int[] codewords, int numECCodewords)
        {
            if (codewords.Length < 4)
            {
                // Codeword array size should be at least 4 allowing for
                // Count CW, At least one Data CW, Error Correction CW, Error Correction CW
                throw FormatException.Instance;
            }
            // The first codeword, the Symbol Length Descriptor, shall always encode the total number of data
            // codewords in the symbol, including the Symbol Length Descriptor itself, data codewords and pad
            // codewords, but excluding the number of error correction codewords.
            int numberOfCodewords = codewords[0];
            if (numberOfCodewords > codewords.Length)
            {
                throw FormatException.Instance;
            }
            if (numberOfCodewords == 0)
            {
                // Reset to the length of the array - 8 (Allow for at least level 3 Error Correction (8 Error Codewords)
                if (numECCodewords < codewords.Length)
                {
                    codewords[0] = codewords.Length - numECCodewords;
                }
                else
                {
                    throw FormatException.Instance;
                }
            }
        }

        /// <summary>
        /// <p>Given data and error-correction codewords received, possibly corrupted by errors, attempts to
        /// correct the errors in-place.</p>
        /// </summary>
        ///
        /// <param name="codewords">data and error correction codewords</param>
        /// <param name="erasures">positions of any known erasures</param>
        /// <param name="numECCodewords">number of error correction codewards that were available in codewords</param>
        /// <exception cref="ChecksumException">if error correction fails</exception>
        private void CorrectErrors(int[] codewords, int[] erasures, int numECCodewords)
        {
            
            if (erasures.Length > numECCodewords / 2 + MAX_ERRORS || numECCodewords < 0 || numECCodewords > MAX_EC_CODEWORDS)
            {
                // Too many errors or EC Codewords is corrupted
                throw ChecksumException.Instance;
            }
            errorCorrection.Decode(codewords, numECCodewords, erasures);
            

            /***
            if ((erasures != null && erasures.Length > numECCodewords - 3) ||
                    numECCodewords < 0 || numECCodewords > MAX_EC_CODEWORDS)
            {
                // Too many errors or EC Codewords is corrupted
                throw FormatException.Instance;
            }

            int syncLen = codewords.Length - codewords[0];
            int numCorrections = rsDecoder.CorrectErrors(codewords, null, 0, codewords.Length, syncLen);

            if (numCorrections < 0)
            {
                // Errors detected but data could not be corrected
                throw FormatException.Instance;
            }
            //return numCorrections;
            */
        }

    }
}
