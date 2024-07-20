using System;
using System.Collections;
using System.Collections.Generic;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using DecodeOptions = MessagingToolkit.Barcode.DecodeOptions;
using BinaryBitmap = MessagingToolkit.Barcode.BinaryBitmap;
using IBarcodeDecoder = MessagingToolkit.Barcode.IDecoder;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using Result = MessagingToolkit.Barcode.Result;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;
using ResultMetadataType = MessagingToolkit.Barcode.ResultMetadataType;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;
using DecoderResult = MessagingToolkit.Barcode.Common.DecoderResult;
using DetectorResult = MessagingToolkit.Barcode.Common.DetectorResult;
using Decoder = MessagingToolkit.Barcode.DataMatrix.Decoders.Decoder;
using Detector = MessagingToolkit.Barcode.DataMatrix.detector.Detector;

namespace MessagingToolkit.Barcode.DataMatrix
{
	
	/// <summary>
	/// This implementation can detect and decode Data Matrix codes in an image.
    /// 
    /// Modified: May 10 2012
	/// </summary>
	internal sealed class DataMatrixDecoder : IDecoder {

        public DataMatrixDecoder()
        {
			this.decoder = new Decoder();
		}
	
		private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
	
		private readonly Decoder decoder;
	
		/// <summary>
		/// Locates and decodes a Data Matrix code in an image.
		/// </summary>
		///
		/// <returns>a String representing the content encoded by the Data Matrix code</returns>
		/// <exception cref="NotFoundException">if a Data Matrix code cannot be found</exception>
		/// <exception cref="FormatException">if a Data Matrix code cannot be decoded</exception>
		/// <exception cref="ChecksumException">if error correction fails</exception>
		public Result Decode(BinaryBitmap image) {
			return Decode(image, null);
		}
	
		public Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
			DecoderResult decoderResult;
			ResultPoint[] points;

			 if (decodingOptions != null && decodingOptions.ContainsKey(DecodeOptions.PureBarcode))
            {
                BitMatrix bits = ExtractPureBits(image.BlackMatrix);
                decoderResult = decoder.Decode(bits);
                points = NO_POINTS;
            }
            else
            {
                DetectorResult detectorResult = new Detector(image.BlackMatrix)
                        .Detect();
                decoderResult = decoder.Decode(detectorResult.Bits);
                points = detectorResult.Points;
            }
            Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.DataMatrix);
			List<byte[]> byteSegments = decoderResult.ByteSegments;
		
            if (byteSegments != null) {
				result.PutMetadata(ResultMetadataType.ByteSegments, byteSegments);
			}
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
		private static BitMatrix ExtractPureBits(BitMatrix image) {
	
			int[] leftTopBlack = image.GetTopLeftOnBit();
			int[] rightBottomBlack = image.GetBottomRightOnBit();
			if (leftTopBlack == null || rightBottomBlack == null) {
                throw NotFoundException.Instance;
			}
	
			int moduleSize = ModuleSize(leftTopBlack, image);
	
			int top = leftTopBlack[1];
			int bottom = rightBottomBlack[1];
			int left = leftTopBlack[0];
			int right = rightBottomBlack[0];
	
			int matrixWidth = (right - left + 1) / moduleSize;
			int matrixHeight = (bottom - top + 1) / moduleSize;
			if (matrixWidth <= 0 || matrixHeight <= 0) {
                throw NotFoundException.Instance;
			}
	
			// Push in the "border" by half the module width so that we start
			// sampling in the middle of the module. Just in case the image is a
			// little off, this will help recover.
			int nudge = moduleSize >> 1;
			top += nudge;
			left += nudge;
	
			// Now just read off the bits
			BitMatrix bits = new BitMatrix(matrixWidth, matrixHeight);
			for (int y = 0; y < matrixHeight; y++) {
				int iOffset = top + y * moduleSize;
				for (int x = 0; x < matrixWidth; x++) {
					if (image.Get(left + x * moduleSize, iOffset)) {
						bits.Set(x, y);
					}
				}
			}
			return bits;
		}
	
		private static int ModuleSize(int[] leftTopBlack, BitMatrix image) {
			int width = image.Width;
			int x = leftTopBlack[0];
			int y = leftTopBlack[1];
			while (x < width && image.Get(x, y)) {
				x++;
			}
			if (x == width) {
                throw NotFoundException.Instance;
			}
	
			int moduleSize = x - leftTopBlack[0];
			if (moduleSize == 0) {
                throw NotFoundException.Instance;
			}
			return moduleSize;
		}
	
	}}
