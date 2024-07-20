using System;
using System.Collections.Generic;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;
using ResultPointCallback = MessagingToolkit.Barcode.ResultPointCallback;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;

namespace MessagingToolkit.Barcode.QRCode.Detector
{

    /// <summary>
    /// <p>This class attempts to find alignment patterns in a QR Code. Alignment patterns look like finder
    /// patterns but are smaller and appear at regular intervals throughout the image.</p>
    /// <p>At the moment this only looks for the bottom-right alignment pattern.</p>
    /// <p>This is mostly a simplified copy of FinderPatternFinder. It is copied,
    /// pasted and stripped down here for maximum performance but does unfortunately duplicate
    /// some code.</p>
    /// <p>This class is thread-safe but not reentrant. Each thread must allocate its own object.</p>
    /// 
    /// Modified: April 27 2012
    /// </summary>
    internal sealed class AlignmentPatternFinder
    {
        private readonly BitMatrix image;
        private List<object> possibleCenters;
        private readonly int startX;
        private readonly int startY;
        private readonly int width;
        private readonly int height;
        private readonly float moduleSize;
        private readonly int[] crossCheckStateCount;
        private readonly ResultPointCallback resultPointCallback;

        /// <summary>
        ///   <p>Creates a finder that will look in a portion of the whole image.</p>
        /// </summary>
        /// <param name="image">image to search</param>
        /// <param name="startX">left column from which to start searching</param>
        /// <param name="startY">top row from which to start searching</param>
        /// <param name="width">width of region to search</param>
        /// <param name="height">height of region to search</param>
        /// <param name="moduleSize">estimated module size so far</param>
        /// <param name="resultPointCallback">The result point callback_6.</param>
        internal AlignmentPatternFinder(BitMatrix image, int startX, int startY, int width, int height, float moduleSize, ResultPointCallback resultPointCallback)
        {
            this.image = image;
            this.possibleCenters = new List<object>(5);
            this.startX = startX;
            this.startY = startY;
            this.width = width;
            this.height = height;
            this.moduleSize = moduleSize;
            this.crossCheckStateCount = new int[3];
            this.resultPointCallback = resultPointCallback;
        }

        /// <summary>
        ///   <p>This method attempts to find the bottom-right alignment pattern in the image. It is a bit messy since
        /// it's pretty performance-critical and so is written to be fast foremost.</p>
        /// </summary>
        internal AlignmentPattern Find()
        {
            int startX_0 = this.startX;
            int height_1 = this.height;
            int maxJ = startX_0 + width;
            int middleI = startY + (height_1 >> 1);
            // We are looking for black/white/black modules in 1:1:1 ratio;
            // this tracks the number of black/white/black modules seen so far
            int[] stateCount = new int[3];
            for (int iGen = 0; iGen < height_1; iGen++)
            {
                // Search from middle outwards
                int i = middleI + (((iGen & 0x01) == 0) ? (iGen + 1) >> 1 : -((iGen + 1) >> 1));
                stateCount[0] = 0;
                stateCount[1] = 0;
                stateCount[2] = 0;
                int j = startX_0;
                // Burn off leading white pixels before anything else; if we start in the middle of
                // a white run, it doesn't make sense to count its length, since we don't know if the
                // white run continued to the left of the start point
                while (j < maxJ && !image.Get(j, i))
                {
                    j++;
                }
                int currentState = 0;
                while (j < maxJ)
                {
                    if (image.Get(j, i))
                    {
                        // Black pixel
                        if (currentState == 1)
                        { // Counting black pixels
                            stateCount[currentState]++;
                        }
                        else
                        { // Counting white pixels
                            if (currentState == 2)
                            { // A winner?
                                if (FoundPatternCross(stateCount))
                                { // Yes
                                    AlignmentPattern confirmed = HandlePossibleCenter(stateCount, i, j);
                                    if (confirmed != null)
                                    {
                                        return confirmed;
                                    }
                                }
                                stateCount[0] = stateCount[2];
                                stateCount[1] = 1;
                                stateCount[2] = 0;
                                currentState = 1;
                            }
                            else
                            {
                                stateCount[++currentState]++;
                            }
                        }
                    }
                    else
                    { // White pixel
                        if (currentState == 1)
                        { // Counting black pixels
                            currentState++;
                        }
                        stateCount[currentState]++;
                    }
                    j++;
                }
                if (FoundPatternCross(stateCount))
                {
                    AlignmentPattern confirmed_2 = HandlePossibleCenter(stateCount, i, maxJ);
                    if (confirmed_2 != null)
                    {
                        return confirmed_2;
                    }
                }

            }

            // Hmm, nothing we saw was observed and confirmed twice. If we had
            // any guess at all, return it.
            if (!(possibleCenters.Count == 0))
            {
                return (AlignmentPattern)possibleCenters[0];
            }

            throw NotFoundException.Instance;
        }

        /// <summary>
        /// Given a count of black/white/black pixels just seen and an end position,
        /// figures the location of the center of this black/white/black run.
        /// </summary>
        ///
        private static float CenterFromEnd(int[] stateCount, int end)
        {
            return (float)(end - stateCount[2]) - stateCount[1] / 2.0f;
        }


        /// <param name="stateCount">count of black/white/black pixels just read</param>
        /// <returns>true iff the proportions of the counts is close enough to the 1/1/1 ratios
        /// used by alignment patterns to be considered a match</returns>
        private bool FoundPatternCross(int[] stateCount)
        {
            float moduleSize_0 = this.moduleSize;
            float maxVariance = moduleSize_0 / 2.0f;
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(moduleSize_0 - stateCount[i]) >= maxVariance)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// <p>After a horizontal scan finds a potential alignment pattern, this method
        /// "cross-checks" by scanning down vertically through the center of the possible
        /// alignment pattern to see if the same proportion is detected.</p>
        /// </summary>
        ///
        /// <param name="startI">row where an alignment pattern was detected</param>
        /// <param name="centerJ">center of the section that appears to cross an alignment pattern</param>
        /// <param name="maxCount"></param>
        /// <returns>vertical center of alignment pattern, or 
        /// <see cref="null"/>
        ///  if not found</returns>
        private float CrossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
        {
            BitMatrix image_0 = this.image;

            int maxI = image_0.GetHeight();
            int[] stateCount = crossCheckStateCount;
            stateCount[0] = 0;
            stateCount[1] = 0;
            stateCount[2] = 0;

            // Start counting up from center
            int i = startI;
            while (i >= 0 && image_0.Get(centerJ, i) && stateCount[1] <= maxCount)
            {
                stateCount[1]++;
                i--;
            }
            // If already too many modules in this state or ran off the edge:
            if (i < 0 || stateCount[1] > maxCount)
            {
                return System.Single.NaN;
            }
            while (i >= 0 && !image_0.Get(centerJ, i) && stateCount[0] <= maxCount)
            {
                stateCount[0]++;
                i--;
            }
            if (stateCount[0] > maxCount)
            {
                return System.Single.NaN;
            }

            // Now also count down from center
            i = startI + 1;
            while (i < maxI && image_0.Get(centerJ, i) && stateCount[1] <= maxCount)
            {
                stateCount[1]++;
                i++;
            }
            if (i == maxI || stateCount[1] > maxCount)
            {
                return System.Single.NaN;
            }
            while (i < maxI && !image_0.Get(centerJ, i) && stateCount[2] <= maxCount)
            {
                stateCount[2]++;
                i++;
            }
            if (stateCount[2] > maxCount)
            {
                return System.Single.NaN;
            }

            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2];
            if (5 * Math.Abs(stateCountTotal - originalStateCountTotal) >= 2 * originalStateCountTotal)
            {
                return System.Single.NaN;
            }

            return (FoundPatternCross(stateCount)) ? CenterFromEnd(stateCount, i) : System.Single.NaN;
        }

        /// <summary>
        ///   <p>This is called when a horizontal scan finds a possible alignment pattern. It will
        /// cross check with a vertical scan, and if successful, will see if this pattern had been
        /// found on a previous horizontal scan. If so, we consider it confirmed and conclude we have
        /// found the alignment pattern.</p>
        /// </summary>
        /// <param name="stateCount">reading state module counts from horizontal scan</param>
        /// <param name="i">row where alignment pattern may be found</param>
        /// <param name="j">end of possible alignment pattern in row</param>
        /// <returns>
        /// if we have found the same pattern twice, or null if not
        /// </returns>
        private AlignmentPattern HandlePossibleCenter(int[] stateCount, int i, int j)
        {
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2];
            float centerJ = CenterFromEnd(stateCount, j);
            float centerI = CrossCheckVertical(i, (int)centerJ, 2 * stateCount[1], stateCountTotal);
            if (!Single.IsNaN(centerI))
            {
                float estimatedModuleSize = (float)(stateCount[0] + stateCount[1] + stateCount[2]) / 3.0f;
                /* foreach */
                foreach (AlignmentPattern center in possibleCenters)
                {
                    // Look for about the same center and module size:
                    if (center.AboutEquals(estimatedModuleSize, centerI, centerJ))
                    {
                        return center.CombineEstimate(centerI, centerJ, estimatedModuleSize);
                    }
                }
                // Hadn't found this before; save it
                AlignmentPattern point = new AlignmentPattern(centerJ, centerI, estimatedModuleSize);
                possibleCenters.Add(point);
                if (resultPointCallback != null)
                {
                    resultPointCallback.FoundPossibleResultPoint(point);
                }
            }
            return null;
        }
    }
}
