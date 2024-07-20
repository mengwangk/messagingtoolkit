using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.Detector;
using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Helper;

using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.QRCode.Detector
{
    /// <summary>
    /// <p>Encapsulates logic that can detect a QR Code in an image, even if the QR Code
    /// is rotated or skewed, or partially obscured.</p>
    /// 
    /// Modified: April 27 2012
    /// </summary>
    public class Detector
    {
        private readonly BitMatrix image;
        private ResultPointCallback resultPointCallback;

        public Detector(BitMatrix image)
        {
            this.image = image;
        }

        protected internal BitMatrix Image
        {
            get
            {
                return image;
            }
        }

        protected internal ResultPointCallback ResultPointCallback
        {
            get
            {
                return resultPointCallback;
            }
        }

        /// <summary>
        /// <p>Detects a QR Code in an image, simply.</p>
        /// </summary>
        /// <returns>
        ///  encapsulating results of detecting a QR Code</returns>
        /// <exception cref="NotFoundException">if no QR Code can be found</exception>
        public DetectorResult Detect()
        {
            return Detect(null);
        }

        /// <summary>
        /// <p>Detects a QR Code in an image, simply.</p>
        /// </summary>
        /// <param name="decodingOptions">optional hints to detector</param>
        /// <returns>encapsulating results of detecting a QR Code</returns>
        public DetectorResult Detect(Dictionary<DecodeOptions, object> decodingOptions)
        {

            resultPointCallback = (decodingOptions == null) ? null
                  : (ResultPointCallback)BarcodeHelper.GetDecodeOptionType(decodingOptions, MessagingToolkit.Barcode.DecodeOptions.NeedResultPointCallback);

            FinderPatternFinder finder = new FinderPatternFinder(image, resultPointCallback);
            FinderPatternInfo info = finder.Find(decodingOptions);

            return ProcessFinderPatternInfo(info);
        }

        protected internal DetectorResult ProcessFinderPatternInfo(FinderPatternInfo info)
        {

            FinderPattern topLeft = info.TopLeft;
            FinderPattern topRight = info.TopRight;
            FinderPattern bottomLeft = info.BottomLeft;

            float moduleSize = CalculateModuleSize(topLeft, topRight, bottomLeft);
            if (moduleSize < 1.0f)
            {
                throw NotFoundException.Instance;
            }
            int dimension = ComputeDimension(topLeft, topRight, bottomLeft, moduleSize);
            MessagingToolkit.Barcode.QRCode.Decoder.Version provisionalVersion = MessagingToolkit.Barcode.QRCode.Decoder.Version.GetProvisionalVersionForDimension(dimension);
            int modulesBetweenFPCenters = provisionalVersion.DimensionForVersion - 7;

            AlignmentPattern alignmentPattern = null;
            // Anything above version 1 has an alignment pattern
            if (provisionalVersion.AlignmentPatternCenters.Length > 0)
            {

                // Guess where a "bottom right" finder pattern would have been
                float bottomRightX = topRight.X - topLeft.X + bottomLeft.X;
                float bottomRightY = topRight.Y - topLeft.Y + bottomLeft.Y;

                // Estimate that alignment pattern is closer by 3 modules
                // from "bottom right" to known top left location
                float correctionToTopLeft = 1.0f - 3.0f / (float)modulesBetweenFPCenters;
                int estAlignmentX = (int)(topLeft.X + correctionToTopLeft * (bottomRightX - topLeft.X));
                int estAlignmentY = (int)(topLeft.Y + correctionToTopLeft * (bottomRightY - topLeft.Y));

                // Kind of arbitrary -- expand search radius before giving up
                for (int i = 4; i <= 16; i <<= 1)
                {
                    try
                    {
                        alignmentPattern = FindAlignmentInRegion(moduleSize, estAlignmentX, estAlignmentY, (float)i);
                        break;
                    }
                    catch (NotFoundException)
                    {
                        // try next round
                    }
                }
                // If we didn't find alignment pattern... well try anyway without it
            }

            PerspectiveTransform transform = CreateTransform(topLeft, topRight, bottomLeft, alignmentPattern, dimension);

            BitMatrix bits = SampleGrid(image, transform, dimension);

            ResultPoint[] points;
            if (alignmentPattern == null)
            {
                points = new ResultPoint[] { bottomLeft, topLeft, topRight };
            }
            else
            {
                points = new ResultPoint[] { bottomLeft, topLeft, topRight, alignmentPattern };
            }
            return new DetectorResult(bits, points);
        }

        public static PerspectiveTransform CreateTransform(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, ResultPoint alignmentPattern, int dimension)
        {
            float dimMinusThree = (float)dimension - 3.5f;
            float bottomRightX;
            float bottomRightY;
            float sourceBottomRightX;
            float sourceBottomRightY;
            if (alignmentPattern != null)
            {
                bottomRightX = alignmentPattern.X;
                bottomRightY = alignmentPattern.Y;
                sourceBottomRightX = dimMinusThree - 3.0f;
                sourceBottomRightY = sourceBottomRightX;
            }
            else
            {
                // Don't have an alignment pattern, just make up the bottom-right point
                bottomRightX = (topRight.X - topLeft.X) + bottomLeft.X;
                bottomRightY = (topRight.Y - topLeft.Y) + bottomLeft.Y;
                sourceBottomRightX = dimMinusThree;
                sourceBottomRightY = dimMinusThree;
            }

            return MessagingToolkit.Barcode.Common.PerspectiveTransform.QuadrilateralToQuadrilateral(3.5f, 3.5f, dimMinusThree, 3.5f, sourceBottomRightX, sourceBottomRightY, 3.5f, dimMinusThree, topLeft.X, topLeft.Y,
                    topRight.X, topRight.Y, bottomRightX, bottomRightY, bottomLeft.X, bottomLeft.Y);
        }

        private static BitMatrix SampleGrid(BitMatrix image, PerspectiveTransform transform, int dimension)
        {

            GridSampler sampler = MessagingToolkit.Barcode.Common.GridSampler.GetInstance();
            return sampler.SampleGrid(image, dimension, dimension, transform);
        }

        /// <summary>
        /// <p>Computes the dimension (number of modules on a size) of the QR Code based on the position
        /// of the finder patterns and estimated module size.</p>
        /// </summary>
        ///
        protected static internal int ComputeDimension(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, float moduleSize)
        {
            int tltrCentersDimension = MathUtils.Round(ResultPoint.Distance(topLeft, topRight) / moduleSize);
            int tlblCentersDimension = MathUtils.Round(ResultPoint.Distance(topLeft, bottomLeft) / moduleSize);
            int dimension = ((tltrCentersDimension + tlblCentersDimension) >> 1) + 7;
            switch (dimension & 0x03)
            { // mod 4
                case 0:
                    dimension++;
                    break;
                // 1? do nothing
                case 2:
                    dimension--;
                    break;
                case 3:
                    throw NotFoundException.Instance;
            }
            return dimension;
        }

        /// <summary>
        /// <p>Computes an average estimated module size based on estimated derived from the positions
        /// of the three finder patterns.</p>
        /// </summary>
        ///
        protected internal float CalculateModuleSize(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft)
        {
            // Take the average
            return (CalculateModuleSizeOneWay(topLeft, topRight) + CalculateModuleSizeOneWay(topLeft, bottomLeft)) / 2.0f;
        }

        /// <summary>
        /// <p>Estimates module size based on two finder patterns -- it uses<see cref="M:Com.Google.Zxing.Qrcode.Detector.Detector.SizeOfBlackWhiteBlackRunBothWays(System.Int32,System.Int32,System.Int32,System.Int32)"/> to figure the
        /// width of each, measuring along the axis between their centers.</p>
        /// </summary>
        ///
        private float CalculateModuleSizeOneWay(ResultPoint pattern, ResultPoint otherPattern)
        {
            float moduleSizeEst1 = SizeOfBlackWhiteBlackRunBothWays((int)pattern.X, (int)pattern.Y, (int)otherPattern.X, (int)otherPattern.Y);
            float moduleSizeEst2 = SizeOfBlackWhiteBlackRunBothWays((int)otherPattern.X, (int)otherPattern.Y, (int)pattern.X, (int)pattern.Y);
            if (Single.IsNaN(moduleSizeEst1))
            {
                return moduleSizeEst2 / 7.0f;
            }
            if (Single.IsNaN(moduleSizeEst2))
            {
                return moduleSizeEst1 / 7.0f;
            }
            // Average them, and divide by 7 since we've counted the width of 3 black modules,
            // and 1 white and 1 black module on either side. Ergo, divide sum by 14.
            return (moduleSizeEst1 + moduleSizeEst2) / 14.0f;
        }

        /// <summary>
        /// See <see cref="M:Com.Google.Zxing.Qrcode.Detector.Detector.SizeOfBlackWhiteBlackRun(System.Int32,System.Int32,System.Int32,System.Int32)"/>; computes the total width of
        /// a finder pattern by looking for a black-white-black run from the center in the direction
        /// of another point (another finder pattern center), and in the opposite direction too.</p>
        /// </summary>
        ///
        private float SizeOfBlackWhiteBlackRunBothWays(int fromX, int fromY, int toX, int toY)
        {

            float result = SizeOfBlackWhiteBlackRun(fromX, fromY, toX, toY);

            // Now count other way -- don't run off image though of course
            float scale = 1.0f;
            int otherToX = fromX - (toX - fromX);
            if (otherToX < 0)
            {
                scale = (float)fromX / (float)(fromX - otherToX);
                otherToX = 0;
            }
            else if (otherToX >= image.GetWidth())
            {
                scale = (float)(image.GetWidth() - 1 - fromX) / (float)(otherToX - fromX);
                otherToX = image.GetWidth() - 1;
            }
            int otherToY = (int)(fromY - (toY - fromY) * scale);

            scale = 1.0f;
            if (otherToY < 0)
            {
                scale = (float)fromY / (float)(fromY - otherToY);
                otherToY = 0;
            }
            else if (otherToY >= image.GetHeight())
            {
                scale = (float)(image.GetHeight() - 1 - fromY) / (float)(otherToY - fromY);
                otherToY = image.GetHeight() - 1;
            }
            otherToX = (int)(fromX + (otherToX - fromX) * scale);

            result += SizeOfBlackWhiteBlackRun(fromX, fromY, otherToX, otherToY);

            // Middle pixel is double-counted this way; subtract 1
            return result - 1.0f;
        }

        /// <summary>
        /// <p>This method traces a line from a point in the image, in the direction towards another point.
        /// It begins in a black region, and keeps going until it finds white, then black, then white again.
        /// It reports the distance from the start to this point.</p>
        /// <p>This is used when figuring out how wide a finder pattern is, when the finder pattern
        /// may be skewed or rotated.</p>
        /// </summary>
        ///
        private float SizeOfBlackWhiteBlackRun(int fromX, int fromY, int toX, int toY)
        {
            // Mild variant of Bresenham's algorithm;
            // see http://en.wikipedia.org/wiki/Bresenham's_line_algorithm
            bool steep = Math.Abs(toY - fromY) > Math.Abs(toX - fromX);
            if (steep)
            {
                int temp = fromX;
                fromX = fromY;
                fromY = temp;
                temp = toX;
                toX = toY;
                toY = temp;
            }

            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);
            int error = -dx >> 1;
            int xstep = (fromX < toX) ? 1 : -1;
            int ystep = (fromY < toY) ? 1 : -1;

            // In black pixels, looking for white, first or second time.
            int state = 0;
            // Loop up until x == toX, but not beyond
            int xLimit = toX + xstep;
            for (int x = fromX, y = fromY; x != xLimit; x += xstep)
            {
                int realX = (steep) ? y : x;
                int realY = (steep) ? x : y;

                // Does current pixel mean we have moved white to black or vice versa?
                // Scanning black in state 0,2 and white in state 1, so if we find the wrong
                // color, advance to next state or end if we are in state 2 already
                if ((state == 1) == image.Get(realX, realY))
                {
                    if (state == 2)
                    {
                        return MathUtils.Distance(x, y, fromX, fromY);
                    }
                    state++;
                }

                error += dy;
                if (error > 0)
                {
                    if (y == toY)
                    {
                        break;
                    }
                    y += ystep;
                    error -= dx;
                }
            }
            // Found black-white-black; give the benefit of the doubt that the next pixel outside the image
            // is "white" so this last point at (toX+xStep,toY) is the right ending. This is really a
            // small approximation; (toX+xStep,toY+yStep) might be really correct. Ignore this.
            if (state == 2)
            {
                return MathUtils.Distance(toX + xstep, toY, fromX, fromY);
            }
            // else we didn't find even black-white-black; no estimate is really possible
            return System.Single.NaN;
        }

        /// <summary>
        /// <p>Attempts to locate an alignment pattern in a limited region of the image, which is
        /// guessed to contain it. This method uses <see cref="T:Com.Google.Zxing.Qrcode.Detector.AlignmentPattern"/>.</p>
        /// </summary>
        ///
        /// <param name="overallEstModuleSize">estimated module size so far</param>
        /// <param name="estAlignmentX">x coordinate of center of area probably containing alignment pattern</param>
        /// <param name="estAlignmentY">y coordinate of above</param>
        /// <param name="allowanceFactor">number of pixels in all directions to search from the center</param>
        /// <returns>/// <see cref="T:Com.Google.Zxing.Qrcode.Detector.AlignmentPattern"/>
        ///  if found, or null otherwise</returns>
        /// <exception cref="NotFoundException">if an unexpected error occurs during detection</exception>
        protected internal AlignmentPattern FindAlignmentInRegion(float overallEstModuleSize, int estAlignmentX, int estAlignmentY, float allowanceFactor)
        {
            // Look for an alignment pattern (3 modules in size) around where it
            // should be
            int allowance = (int)(allowanceFactor * overallEstModuleSize);
            int alignmentAreaLeftX = Math.Max(0, estAlignmentX - allowance);
            int alignmentAreaRightX = Math.Min(image.GetWidth() - 1, estAlignmentX + allowance);
            if (alignmentAreaRightX - alignmentAreaLeftX < overallEstModuleSize * 3)
            {
                throw NotFoundException.Instance;
            }

            int alignmentAreaTopY = Math.Max(0, estAlignmentY - allowance);
            int alignmentAreaBottomY = Math.Min(image.GetHeight() - 1, estAlignmentY + allowance);
            if (alignmentAreaBottomY - alignmentAreaTopY < overallEstModuleSize * 3)
            {
                throw NotFoundException.Instance;
            }

            AlignmentPatternFinder alignmentFinder = new AlignmentPatternFinder(image, alignmentAreaLeftX, alignmentAreaTopY, alignmentAreaRightX - alignmentAreaLeftX, alignmentAreaBottomY
                    - alignmentAreaTopY, overallEstModuleSize, resultPointCallback);
            return alignmentFinder.Find();
        }
    }
}
