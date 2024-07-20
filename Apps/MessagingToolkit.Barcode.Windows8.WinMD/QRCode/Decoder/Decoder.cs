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
    internal sealed class Decoder
    {

       private readonly ReedSolomonDecoder rsDecoder;
	
		public Decoder() {
			rsDecoder = new ReedSolomonDecoder(GenericGF.QRCodeField256);
		}
	
		public DecoderResult Decode(bool[][] image) {
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
		public DecoderResult Decode(bool[][] image, IDictionary<DecodeOptions, object> decodingOptions) {
			int dimension = image.Length;
			BitMatrix bits = new BitMatrix(dimension);
			for (int i = 0; i < dimension; i++) {
				for (int j = 0; j < dimension; j++) {
					if (image[i][j]) {
						bits.Set(j, i);
					}
				}
			}
			return Decode(bits, decodingOptions);
		}
	
		public DecoderResult Decode(BitMatrix bits) {
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
        public DecoderResult Decode(BitMatrix bits, IDictionary<DecodeOptions, object> decodingOptions)
        {
	
			// Construct a parser and read version, error-correction level
			BitMatrixParser parser = new BitMatrixParser(bits);
			Version version = parser.ReadVersion();
			ErrorCorrectionLevel ecLevel = parser.ReadFormatInformation().GetErrorCorrectionLevel();
	
			// Read codewords
			byte[] codewords = parser.ReadCodewords();
			// Separate into data blocks
			DataBlock[] dataBlocks = DataBlock.GetDataBlocks(codewords, version, ecLevel);
	
			// Count total number of data bytes
			int totalBytes = 0;
			/* foreach */
			foreach (DataBlock dataBlock  in  dataBlocks) {
				totalBytes += dataBlock.NumDataCodewords;
			}
			byte[] resultBytes = new byte[totalBytes];
			int resultOffset = 0;
	
			/* foreach */
			// Error-correct and copy data blocks together into a stream of bytes
			foreach (DataBlock dataBlock_0  in  dataBlocks) {
				byte[] codewordBytes = dataBlock_0.Codewords;
				int numDataCodewords = dataBlock_0.NumDataCodewords;
				CorrectErrors(codewordBytes, numDataCodewords);
				for (int i = 0; i < numDataCodewords; i++) {
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
		private void CorrectErrors(byte[] codewordBytes, int numDataCodewords) {
			int numCodewords = codewordBytes.Length;
			// First read into an array of ints
			int[] codewordsInts = new int[numCodewords];
			for (int i = 0; i < numCodewords; i++) {
				codewordsInts[i] = codewordBytes[i] & 0xFF;
			}
			int numECCodewords = codewordBytes.Length - numDataCodewords;
			try {
				rsDecoder.Decode(codewordsInts, numECCodewords);
			} catch (ReedSolomonException rse) {
                throw ChecksumException.Instance;
			}
			// Copy back into array of bytes -- only need to worry about the bytes that were data
			// We don't care about errors in the error-correction codewords
			for (int i_0 = 0; i_0 < numDataCodewords; i_0++) {
				codewordBytes[i_0] = (byte) codewordsInts[i_0];
			}
		}

    }
}
