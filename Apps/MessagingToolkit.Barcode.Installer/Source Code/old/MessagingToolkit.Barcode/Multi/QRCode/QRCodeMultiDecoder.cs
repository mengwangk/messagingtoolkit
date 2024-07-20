using System.Collections;
using System.Collections.Generic;
using System;

using DecoderResult = MessagingToolkit.Barcode.Common.DecoderResult;
using DetectorResult = MessagingToolkit.Barcode.Common.DetectorResult;
using MultiDetector = MessagingToolkit.Barcode.Multi.QRCode.Detector.MultiDetector;
using MessagingToolkit.Barcode.Multi.QRCode.Detector;
using MessagingToolkit.Barcode.QRCode;

namespace MessagingToolkit.Barcode.Multi.QRCode
{

    /// <summary>
    /// This implementation can detect and decode multiple QR Codes in an image.
    /// 
    /// Modfied: April 21 2012
    /// </summary>
    public sealed class QRCodeMultiDecoder : QRCodeDecoder, MultipleBarcodeDecoder
    {

        private static readonly Result[] EMPTY_RESULT_ARRAY = new Result[0];

        public Result[] DecodeMultiple(BinaryBitmap image)
        {
            return DecodeMultiple(image, null);
        }

        public Result[] DecodeMultiple(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
        {
            List<Result> results = new List<Result>();
            DetectorResult[] detectorResults = new MultiDetector(image.BlackMatrix).DetectMulti(decodingOptions);
            /* foreach */
            foreach (DetectorResult detectorResult in detectorResults)
            {
                try
                {
                    DecoderResult decoderResult = GetDecoder().Decode(detectorResult.Bits, decodingOptions);
                    ResultPoint[] points = detectorResult.Points;
                    Result result = new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.QRCode);
                    IList<byte[]> byteSegments = decoderResult.ByteSegments;
                    if (byteSegments != null)
                    {
                        result.PutMetadata(ResultMetadataType.ByteSegments, byteSegments);
                    }
                    String ecLevel = decoderResult.ECLevel;
                    if (ecLevel != null)
                    {
                        result.PutMetadata(ResultMetadataType.ErrorCorrectionLevel, ecLevel);
                    }
                    results.Add(result);
                }
                catch (BarcodeDecoderException)
                {
                    // ignore and continue 
                }
            }
            if ((results.Count == 0))
            {
                return EMPTY_RESULT_ARRAY;
            }
            else
            {
                Result[] resultArray = new Result[results.Count];
                for (int i = 0; i < results.Count; i++)
                {
                    resultArray[i] = (Result)results[i];
                }
                return resultArray;
            }
        }

    }
}
