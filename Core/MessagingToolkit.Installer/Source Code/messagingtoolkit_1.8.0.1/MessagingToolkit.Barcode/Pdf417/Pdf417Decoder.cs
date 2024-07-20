using System;
using System.Collections.Generic;

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Multi;
using MessagingToolkit.Barcode.Pdf417.Detector;
using MessagingToolkit.Barcode.Pdf417.Decoder;

namespace MessagingToolkit.Barcode.Pdf417
{
    /// <summary>
    /// This implementation can detect and decode PDF417 codes in an image.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class Pdf417Decoder : IDecoder, MultipleBarcodeDecoder
    {
        /// <summary>
        /// Locates and decodes a PDF417 code in an image.
        /// </summary>
        /// <returns> a String representing the content encoded by the PDF417 code </returns>
        /// <exception cref="NotFoundException"> if a PDF417 code cannot be found, </exception>
        /// <exception cref="FormatException"> if a PDF417 cannot be decoded </exception>
        public Result Decode(BinaryBitmap image)
        {
            return Decode(image, null);
        }

        public Result Decode(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
        {
            Result[] result = Decode(image, decodingOptions, false);
            if (result == null || result.Length == 0 || result[0] == null)
            {
                throw NotFoundException.Instance;
            }
            return result[0];
        }

        public Result[] DecodeMultiple(BinaryBitmap image)
        {
            return DecodeMultiple(image, null);
        }

        public Result[] DecodeMultiple(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
        {
            try
            {
                return Decode(image, decodingOptions, true);
            }
            catch (FormatException)
            {
                throw NotFoundException.Instance;
            }
            catch (ChecksumException)
            {
                throw NotFoundException.Instance;
            }
        }

        private static Result[] Decode(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions, bool multiple)
        {
            List<Result> results = new List<Result>();
            Pdf417DetectorResult detectorResult = MessagingToolkit.Barcode.Pdf417.Detector.Detector.Detect(image, decodingOptions, multiple);
            foreach (ResultPoint[] points in detectorResult.Points)
            {
                DecoderResult decoderResult = Pdf417ScanningDecoder.Decode(detectorResult.Bits, points[4], points[5], points[6], points[7], GetMinCodewordWidth(points), GetMaxCodewordWidth(points));
                if (decoderResult == null)
                {
                    throw NotFoundException.Instance;
                }
                Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.PDF417);
                result.PutMetadata(ResultMetadataType.ErrorCorrectionLevel, decoderResult.ECLevel);
                Pdf417ResultMetadata pdf417ResultMetadata = (Pdf417ResultMetadata)decoderResult.Other;
                if (pdf417ResultMetadata != null)
                {
                    result.PutMetadata(ResultMetadataType.PDF417ExtraMetadata, pdf417ResultMetadata);
                }
                results.Add(result);
            }
            return results.ToArray();
        }

        private static int GetMaxWidth(ResultPoint p1, ResultPoint p2)
        {
            if (p1 == null || p2 == null)
            {
                return 0;
            }
            return (int)Math.Abs(p1.X - p2.X);
        }

        private static int GetMinWidth(ResultPoint p1, ResultPoint p2)
        {
            if (p1 == null || p2 == null)
            {
                return int.MaxValue;
            }
            return (int)Math.Abs(p1.X - p2.X);
        }

        private static int GetMaxCodewordWidth(ResultPoint[] p)
        {
            return Math.Max(Math.Max(GetMaxWidth(p[0], p[4]), GetMaxWidth(p[6], p[2]) * Pdf417Common.MODULES_IN_CODEWORD / Pdf417Common.MODULES_IN_STOP_PATTERN), Math.Max(GetMaxWidth(p[1], p[5]), GetMaxWidth(p[7], p[3]) * Pdf417Common.MODULES_IN_CODEWORD / Pdf417Common.MODULES_IN_STOP_PATTERN));
        }

        private static int GetMinCodewordWidth(ResultPoint[] p)
        {
            return Math.Min(Math.Min(GetMinWidth(p[0], p[4]), GetMinWidth(p[6], p[2]) * Pdf417Common.MODULES_IN_CODEWORD / Pdf417Common.MODULES_IN_STOP_PATTERN), Math.Min(GetMinWidth(p[1], p[5]), GetMinWidth(p[7], p[3]) * Pdf417Common.MODULES_IN_CODEWORD / Pdf417Common.MODULES_IN_STOP_PATTERN));
        }

        public void Reset()
        {
            // nothing needs to be reset
        }
    }
}
