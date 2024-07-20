using System;
using System.Collections.Generic;
using IBarcodeDecoder = MessagingToolkit.Barcode.IDecoder;
using Result = MessagingToolkit.Barcode.Result;
using BinaryBitmap = MessagingToolkit.Barcode.BinaryBitmap;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;

namespace MessagingToolkit.Barcode.Multi
{

    /// <summary>
    /// <p>Attempts to locate multiple barcodes in an image by repeatedly decoding portion of the image.
    /// After one barcode is found, the areas left, above, right and below the barcode's<see cref="null"/>s are scanned, recursively.</p>
    /// <p>A caller may want to also employ <see cref="T:Com.Google.Zxing.Multi.ByQuadrantReader"/> when attempting to find multiple
    /// 2D barcodes, like QR Codes, in an image, where the presence of multiple barcodes might prevent
    /// detecting any one of them.</p>
    /// <p>That is, instead of passing a <see cref="null"/> a caller might pass}.</p>
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal class GenericMultipleBarcodeDecoder : MultipleBarcodeDecoder
    {

        private const int MIN_DIMENSION_TO_RECUR = 100;
        private const int MAX_DEPTH = 4;

        private readonly BarcodeDecoder decoder;

        public GenericMultipleBarcodeDecoder(BarcodeDecoder decoder)
        {
            this.decoder = decoder;
        }

        public Result[] DecodeMultiple(BinaryBitmap image)
        {
            return DecodeMultiple(image, null);
        }

        public Result[] DecodeMultiple(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            IList<Result> results = new List<Result>();
            DoDecodeMultiple(image, decodingOptions, results, 0, 0, 0);
            if ((results.Count == 0))
            {
                throw NotFoundException.Instance;
            }
            int numResults = results.Count;
            Result[] resultArray = new Result[numResults];
            for (int i = 0; i < numResults; i++)
            {
                resultArray[i] = results[i];
            }
            return resultArray;
        }

        private void DoDecodeMultiple(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions,
            IList<Result> results, int xOffset, int yOffset, int currentDepth)
        {
            if (currentDepth > MAX_DEPTH)
            {
                return;
            }

            Result result;
            try
            {
                result = decoder.Decode(image, decodingOptions);
            }
            catch (BarcodeDecoderException)
            {
                return;
            }
            bool alreadyFound = false;
            /* foreach */
            foreach (Result existingResult in results)
            {
                if (existingResult.Text.Equals(result.Text))
                {
                    alreadyFound = true;
                    break;
                }
            }
            if (!alreadyFound)
            {
                results.Add(TranslateResultPoints(result, xOffset, yOffset));
            }

            ResultPoint[] resultPoints = result.ResultPoints;
            if (resultPoints == null || resultPoints.Length == 0)
            {
                return;
            }
            int width = image.Width;
            int height = image.Height;
            float minX = width;
            float minY = height;
            float maxX = 0.0f;
            float maxY = 0.0f;
            /* foreach */
            foreach (ResultPoint point in resultPoints)
            {
                float x = point.X;
                float y = point.Y;
                if (x < minX)
                {
                    minX = x;
                }
                if (y < minY)
                {
                    minY = y;
                }
                if (x > maxX)
                {
                    maxX = x;
                }
                if (y > maxY)
                {
                    maxY = y;
                }
            }

            // Decode left of barcode
            if (minX > MIN_DIMENSION_TO_RECUR)
            {
                DoDecodeMultiple(image.Crop(0, 0, (int)minX, height), decodingOptions, results, xOffset, yOffset, currentDepth + 1);
            }
            // Decode above barcode
            if (minY > MIN_DIMENSION_TO_RECUR)
            {
                DoDecodeMultiple(image.Crop(0, 0, width, (int)minY), decodingOptions, results, xOffset, yOffset, currentDepth + 1);
            }
            // Decode right of barcode
            if (maxX < width - MIN_DIMENSION_TO_RECUR)
            {
                DoDecodeMultiple(image.Crop((int)maxX, 0, width - (int)maxX, height), decodingOptions, results, xOffset + (int)maxX, yOffset, currentDepth + 1);
            }
            // Decode below barcode
            if (maxY < height - MIN_DIMENSION_TO_RECUR)
            {
                DoDecodeMultiple(image.Crop(0, (int)maxY, width, height - (int)maxY), decodingOptions, results, xOffset, yOffset + (int)maxY, currentDepth + 1);
            }
        }

        private static Result TranslateResultPoints(Result result, int xOffset, int yOffset)
        {
            ResultPoint[] oldResultPoints = result.ResultPoints;
            if (oldResultPoints == null)
            {
                return result;
            }
            ResultPoint[] newResultPoints = new ResultPoint[oldResultPoints.Length];
            for (int i = 0; i < oldResultPoints.Length; i++)
            {
                ResultPoint oldPoint = oldResultPoints[i];
                newResultPoints[i] = new ResultPoint(oldPoint.X + xOffset, oldPoint.Y + yOffset);
            }
            return new Result(result.Text, result.RawBytes, newResultPoints, result.BarcodeFormat);
        }

    }
}
