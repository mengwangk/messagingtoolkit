using System;
using System.Collections.Generic;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Pdf417
{
    /// <summary>
    /// This implementation can detect and decode PDF417 codes in an image.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class Pdf417Decoder : IDecoder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Pdf417Decoder()
        {
            this.decoder = new MessagingToolkit.Barcode.Pdf417.Decoder.Decoder();
        }

        private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

        private readonly MessagingToolkit.Barcode.Pdf417.Decoder.Decoder decoder;

        /// <summary>
        /// Locates and decodes a PDF417 code in an image.
        /// </summary>
        ///
        /// <returns>a String representing the content encoded by the PDF417 code</returns>
        /// <exception cref="NotFoundException">if a PDF417 code cannot be found,</exception>
        /// <exception cref="FormatException">if a PDF417 cannot be decoded</exception>
        public Result Decode(BinaryBitmap image)
        {
            return Decode(image, null);
        }

        public Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            DecoderResult decoderResult;
            ResultPoint[] points;
            if (decodingOptions != null && decodingOptions.ContainsKey(MessagingToolkit.Barcode.DecodeOptions.PureBarcode))
            {
                BitMatrix bits = ExtractPureBits(image.BlackMatrix);
                decoderResult = decoder.Decode(bits);
                points = NO_POINTS;
            }
            else
            {
                DetectorResult detectorResult = new MessagingToolkit.Barcode.Pdf417.Detector.Detector(image).Detect(decodingOptions);
                //decoderResult = decoder.Decode(detectorResult.Bits);

                
                try
                {
                    decoderResult = decoder.Decode(detectorResult.Bits);
                }
                catch
                {
                    // Added Oct 6 2012 ---- New detection algorithm
                    MessagingToolkit.Barcode.Pdf417.Detector.Detector2 detector2 = new Detector.Detector2(image.BlackMatrix);
                    decoderResult = detector2.Detect();
                }
                points = detectorResult.Points;
            }
            return new Result(decoderResult.Text, decoderResult.RawBytes, points, BarcodeFormat.PDF417);
        }

        public void Reset()
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

            int moduleSize = ModuleSize(leftTopBlack, image);

            int top = leftTopBlack[1];
            int bottom = rightBottomBlack[1];
            int left = FindPatternStart(leftTopBlack[0], top, image);
            int right = FindPatternEnd(leftTopBlack[0], top, image);

            int matrixWidth = (right - left + 1) / moduleSize;
            int matrixHeight = (bottom - top + 1) / moduleSize;
            if (matrixWidth <= 0 || matrixHeight <= 0)
            {
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
            for (int y = 0; y < matrixHeight; y++)
            {
                int iOffset = top + y * moduleSize;
                for (int x = 0; x < matrixWidth; x++)
                {
                    if (image.Get(left + x * moduleSize, iOffset))
                    {
                        bits.Set(x, y);
                    }
                }
            }
            return bits;
        }

        private static int ModuleSize(int[] leftTopBlack, BitMatrix image)
        {
            int x = leftTopBlack[0];
            int y = leftTopBlack[1];
            int width = image.GetWidth();
            while (x < width && image.Get(x, y))
            {
                x++;
            }
            if (x == width)
            {
                throw NotFoundException.Instance;
            }

            int moduleSize = (int)(((uint)(x - leftTopBlack[0])) >> 3); // We've crossed left first bar, which is 8x
            if (moduleSize == 0)
            {
                throw NotFoundException.Instance;
            }

            return moduleSize;
        }

        private static int FindPatternStart(int x, int y, BitMatrix image)
        {
            int width = image.GetWidth();
            int start = x;
            // start should be on black
            int transitions = 0;
            bool black = true;
            while (start < width - 1 && transitions < 8)
            {
                start++;
                bool newBlack = image.Get(start, y);
                if (black != newBlack)
                {
                    transitions++;
                }
                black = newBlack;
            }
            if (start == width - 1)
            {
                throw NotFoundException.Instance;
            }
            return start;
        }

        private static int FindPatternEnd(int x, int y, BitMatrix image)
        {
            int width = image.GetWidth();
            int end = width - 1;
            // end should be on black
            while (end > x && !image.Get(end, y))
            {
                end--;
            }
            int transitions = 0;
            bool black = true;
            while (end > x && transitions < 9)
            {
                end--;
                bool newBlack = image.Get(end, y);
                if (black != newBlack)
                {
                    transitions++;
                }
                black = newBlack;
            }
            if (end == x)
            {
                throw NotFoundException.Instance;
            }
            return end;
        }
    }
}
