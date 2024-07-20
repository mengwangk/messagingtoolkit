using MessagingToolkit.Barcode.Common;

using System.Collections.Generic;
using System;

namespace MessagingToolkit.Barcode.QRCode
{
    /// <summary>
    /// This implementation can detect and decode QR Codes in an image.
    /// 
    /// Modified: April 28 2012
    /// </summary>
    internal class QRCodeDecoder : IDecoder
    {
        public QRCodeDecoder()
        {
            this.decoder = new MessagingToolkit.Barcode.QRCode.Decoder.Decoder();
        }

        private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

        private readonly MessagingToolkit.Barcode.QRCode.Decoder.Decoder decoder;

        protected internal MessagingToolkit.Barcode.QRCode.Decoder.Decoder GetDecoder()
        {
            return decoder;
        }

        /// <summary>
        /// Locates and decodes a QR code in an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// a String representing the content encoded by the QR code
        /// </returns>
        public virtual Result Decode(BinaryBitmap image)
        {
            return Decode(image, null);
        }

        public virtual Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            DecoderResult decoderResult;
            ResultPoint[] points;
            if (decodingOptions != null && decodingOptions.ContainsKey(DecodeOptions.PureBarcode))
            {
                BitMatrix bits = ExtractPureBits(image.BlackMatrix);
                decoderResult = decoder.Decode(bits, decodingOptions);
                points = NO_POINTS;
            }
            else
            {
                DetectorResult detectorResult = new MessagingToolkit.Barcode.QRCode.Detector.Detector(image.BlackMatrix).Detect(decodingOptions);
                decoderResult = decoder.Decode(detectorResult.Bits, decodingOptions);
                points = detectorResult.Points;
            }

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
            return result;
        }

        public virtual void Reset()
        {
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

            int[] leftTopBlack = image.GetTopLeftOnBit();
            int[] rightBottomBlack = image.GetBottomRightOnBit();
            if (leftTopBlack == null || rightBottomBlack == null)
            {
                throw NotFoundException.Instance;
            }

            float moduleSize = ModuleSize(leftTopBlack, image);

            int top = leftTopBlack[1];
            int bottom = rightBottomBlack[1];
            int left = leftTopBlack[0];
            int right = rightBottomBlack[0];

            if (bottom - top != right - left)
            {
                // Special case, where bottom-right module wasn't black so we found something else in the last row
                // Assume it's a square, so use height as the width
                right = left + (bottom - top);
            }

            int matrixWidth = (int)Math.Round((right - left + 1) / moduleSize);
            int matrixHeight = (int)Math.Round((bottom - top + 1) / moduleSize);
            if (matrixWidth <= 0 || matrixHeight <= 0)
            {
                throw NotFoundException.Instance;
            }
            if (matrixHeight != matrixWidth)
            {
                // Only possibly decode square regions
                throw NotFoundException.Instance;
            }

            // Push in the "border" by half the module width so that we start
            // sampling in the middle of the module. Just in case the image is a
            // little off, this will help recover.
            int nudge = (int)(moduleSize / 2.0f);
            top += nudge;
            left += nudge;

            // Now just read off the bits
            BitMatrix bits = new BitMatrix(matrixWidth, matrixHeight);
            for (int y = 0; y < matrixHeight; y++)
            {
                int iOffset = top + (int)(y * moduleSize);
                for (int x = 0; x < matrixWidth; x++)
                {
                    if (image.Get(left + (int)(x * moduleSize), iOffset))
                    {
                        bits.Set(x, y);
                    }
                }
            }
            return bits;
        }

        private static float ModuleSize(int[] leftTopBlack, BitMatrix image)
        {
            int height = image.GetHeight();
            int width = image.GetWidth();
            int x = leftTopBlack[0];
            int y = leftTopBlack[1];
            bool inBlack = true;
            int transitions = 0;
            while (x < width && y < height)
            {
                if (inBlack != image.Get(x, y))
                {
                    if (++transitions == 5)
                    {
                        break;
                    }
                    inBlack = !inBlack;
                }
                x++;
                y++;
            }
            if (x == width || y == height)
            {
                throw NotFoundException.Instance;
            }
            return (x - leftTopBlack[0]) / 7.0f;
        }
    }
}
