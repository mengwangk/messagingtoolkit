using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Aztec;
using MessagingToolkit.Barcode.Aztec.Detector;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Aztec.Decoder;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.Aztec
{
    /// <summary>
    /// This implementation can detect and decode Aztec codes in an image.
    /// 
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public sealed class AztecDecoder : IDecoder
    {

		/// <summary>
		/// Locates and decodes a Data Matrix code in an image.
		/// </summary>
		///
		/// <returns>a String representing the content encoded by the Data Matrix code</returns>
		/// <exception cref="NotFoundException">if a Data Matrix code cannot be found</exception>
		/// <exception cref="FormatException">if a Data Matrix code cannot be decoded</exception>
		/// <exception cref="com.google.zxing.ChecksumException">if error correction fails</exception>
		public Result Decode(BinaryBitmap image) {
			return Decode(image, null);
		}
	
		public Result Decode(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions) {

            AztecDetectorResult detectorResult = new MessagingToolkit.Barcode.Aztec.Detector.Detector(image.BlackMatrix).Detect();
			ResultPoint[] points = detectorResult.Points;
	
			if (decodingOptions != null) {
                     ResultPointCallback rpcb = (decodingOptions == null) ? null
                        : (ResultPointCallback)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.NeedResultPointCallback);

				if (rpcb != null) {
					/* foreach */
					foreach (ResultPoint point  in  points) {
						rpcb.FoundPossibleResultPoint(point);
					}
				}
			}

            DecoderResult decoderResult = new MessagingToolkit.Barcode.Aztec.Decoder.Decoder().Decode(detectorResult);
	
			Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.Aztec);
	
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

    }
}
