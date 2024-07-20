using System;
using System.Collections.Generic;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.MaxiCode
{
	/// <summary>
	/// This implementation can detect and decode a MaxiCode in an image.
	/// </summary>
	internal sealed class MaxiCodeDecoder : IDecoder {
	
		private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
		private const int MATRIX_WIDTH = 30;
		private const int MATRIX_HEIGHT = 33;
	
		private readonly MessagingToolkit.Barcode.MaxiCode.Decoder.Decoder decoder = new MessagingToolkit.Barcode.MaxiCode.Decoder.Decoder();
	
		internal MessagingToolkit.Barcode.MaxiCode.Decoder.Decoder GetDecoder() {
			return decoder;
		}
	
		/// <summary>
		/// Locates and decodes a MaxiCode in an image.
		/// </summary>
		/// <returns>a String representing the content encoded by the MaxiCode</returns>
		public Result Decode(BinaryBitmap image) 
        {
			return Decode(image, null);
		}
	
		public Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)  {
			DecoderResult decoderResult;
			if (decodingOptions != null && decodingOptions.ContainsKey(DecodeOptions.PureBarcode))
            {
				BitMatrix bits = ExtractPureBits(image.BlackMatrix);
				decoderResult = decoder.Decode(bits, decodingOptions);
			} else {
               throw NotFoundException.Instance;
			}
	
			ResultPoint[] points = NO_POINTS;
			Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.MaxiCode);
	
			String ecLevel = decoderResult.ECLevel;
			if (ecLevel != null) {
				result.PutMetadata(ResultMetadataType.ErrorCorrectionLevel, ecLevel);
			}
			return result;
		}
	
		public void Reset() {
			// do nothing
		}
	
		/// <summary>
		/// This method detects a code in a "pure" image -- that is, pure monochrome image
		/// which contains only an unrotated, unskewed, image of a code, with some white border
		/// around it. This is a specialized method that works exceptionally fast in this special
		/// case.
		/// </summary>
		private static BitMatrix ExtractPureBits(BitMatrix image) 
        {
	
			int[] enclosingRectangle = image.GetEnclosingRectangle();
			if (enclosingRectangle == null) {
                throw NotFoundException.Instance;
			}
	
			int left = enclosingRectangle[0];
			int top = enclosingRectangle[1];
			int width = enclosingRectangle[2];
			int height = enclosingRectangle[3];
	
			// Now just read off the bits
			BitMatrix bits = new BitMatrix(MATRIX_WIDTH, MATRIX_HEIGHT);
			for (int y = 0; y < MATRIX_HEIGHT; y++) {
				int iy = top + (y * height + height / 2) / MATRIX_HEIGHT;
				for (int x = 0; x < MATRIX_WIDTH; x++) {
					int ix = left + (x * width + width / 2 + (y & 0x01) * width / 2) / MATRIX_WIDTH;
					if (image.Get(ix, iy)) {
						bits.Set(x, y);
					}
				}
			}
			return bits;
		}
	
	}
}
