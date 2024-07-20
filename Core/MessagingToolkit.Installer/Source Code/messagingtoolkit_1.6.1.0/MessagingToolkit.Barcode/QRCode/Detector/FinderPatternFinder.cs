using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.QRCode.Detector
{

    /// <summary>
    /// <p>This class attempts to find finder patterns in a QR Code. Finder patterns are the square
    /// markers at three corners of a QR Code.</p>
    /// <p>This class is thread-safe but not reentrant. Each thread must allocate its own object.
    /// 
    /// Modified: April 27 2012
    /// </summary>
    public class FinderPatternFinder
    {
        // @Conversion 
        //private const int CENTER_QUORUM = 2;
        private const int CENTER_QUORUM = 6;

        protected internal const int MIN_SKIP = 3; // 1 pixel/module times 3 modules/center
        protected internal const int MAX_MODULES = 57; // support up to version 10 for mobile clients
        private const int INTEGER_MATH_SHIFT = 8;

        private readonly BitMatrix image;
        private readonly List<object> possibleCenters;
        private bool hasSkipped;
        private readonly int[] crossCheckStateCount;
        private readonly ResultPointCallback resultPointCallback;

        /// <summary>
        /// <p>Creates a finder that will search the image for three finder patterns.</p>
        /// </summary>
        ///
        /// <param name="image">image to search</param>
        public FinderPatternFinder(BitMatrix image)
            : this(image, null)
        {
        }

        public FinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
        {
            this.image = image;
            this.possibleCenters = new List<object>();
            this.crossCheckStateCount = new int[5];
            this.resultPointCallback = resultPointCallback;
        }


        protected internal BitMatrix GetImage()
        {
            return image;
        }

        protected internal List<object> GetPossibleCenters()
        {
            return possibleCenters;
        }


        internal FinderPatternInfo Find(Dictionary<DecodeOptions, object> decodingOptions)
        {
            bool tryHarder = decodingOptions != null
                    && decodingOptions.ContainsKey(MessagingToolkit.Barcode.DecodeOptions.TryHarder);
            int maxI = image.GetHeight();
            int maxJ = image.GetWidth();

            // We are looking for black/white/black/white/black modules in
            // 1:1:3:1:1 ratio; this tracks the number of such modules seen so far

            // Let's assume that the maximum version QR Code we support takes up 1/4 the height of the
            // image, and then account for the center being 3 modules in size. This gives the smallest
            // number of pixels the center could be, so skip this often. When trying harder, look for all
            // QR versions regardless of how dense they are.
            int iSkip = (3 * maxI) / (4 * MAX_MODULES);
            if (iSkip < MIN_SKIP || tryHarder)
            {
                iSkip = MIN_SKIP;
            }

            bool done = false;
            int[] stateCount = new int[5];
            for (int i = iSkip - 1; i < maxI && !done; i += iSkip)
            {
                // Get a row of black/white values
                stateCount[0] = 0;
                stateCount[1] = 0;
                stateCount[2] = 0;
                stateCount[3] = 0;
                stateCount[4] = 0;
                int currentState = 0;
                for (int j = 0; j < maxJ; j++)
                {
                    if (image.Get(j, i))
                    {
                        // Black pixel
                        if ((currentState & 1) == 1)
                        { // Counting white pixels
                            currentState++;
                        }
                        stateCount[currentState]++;
                    }
                    else
                    { // White pixel
                        if ((currentState & 1) == 0)
                        { // Counting black pixels
                            if (currentState == 4)
                            { // A winner?
                                if (FoundPatternCross(stateCount))
                                { // Yes
                                    bool confirmed = HandlePossibleCenter(stateCount, i, j);
                                    if (confirmed)
                                    {
                                        // Start examining every other line. Checking each line turned out to be too
                                        // expensive and didn't improve performance.
                                        iSkip = 2;
                                        if (hasSkipped)
                                        {
                                            done = HaveMultiplyConfirmedCenters();
                                        }
                                        else
                                        {
                                            int rowSkip = FindRowSkip();
                                            if (rowSkip > stateCount[2])
                                            {
                                                // Skip rows between row of lower confirmed center
                                                // and top of presumed third confirmed center
                                                // but back up a bit to get a full chance of detecting
                                                // it, entire width of center of finder pattern

                                                // Skip by rowSkip, but back off by stateCount[2] (size of last center
                                                // of pattern we saw) to be conservative, and also back off by iSkip which
                                                // is about to be re-added
                                                i += rowSkip - stateCount[2] - iSkip;
                                                j = maxJ - 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        stateCount[0] = stateCount[2];
                                        stateCount[1] = stateCount[3];
                                        stateCount[2] = stateCount[4];
                                        stateCount[3] = 1;
                                        stateCount[4] = 0;
                                        currentState = 3;
                                        continue;
                                    }
                                    // Clear state to start looking again
                                    currentState = 0;
                                    stateCount[0] = 0;
                                    stateCount[1] = 0;
                                    stateCount[2] = 0;
                                    stateCount[3] = 0;
                                    stateCount[4] = 0;
                                }
                                else
                                { // No, shift counts back by two
                                    stateCount[0] = stateCount[2];
                                    stateCount[1] = stateCount[3];
                                    stateCount[2] = stateCount[4];
                                    stateCount[3] = 1;
                                    stateCount[4] = 0;
                                    currentState = 3;
                                }
                            }
                            else
                            {
                                stateCount[++currentState]++;
                            }
                        }
                        else
                        { // Counting white pixels
                            stateCount[currentState]++;
                        }
                    }
                }
                if (FoundPatternCross(stateCount))
                {
                    bool confirmed_0 = HandlePossibleCenter(stateCount, i, maxJ);
                    if (confirmed_0)
                    {
                        iSkip = stateCount[0];
                        if (hasSkipped)
                        {
                            // Found a third one
                            done = HaveMultiplyConfirmedCenters();
                        }
                    }
                }
            }

            FinderPattern[] patternInfo = SelectBestPatterns();
            ResultPoint.OrderBestPatterns(patternInfo);

            return new FinderPatternInfo(patternInfo);
        }

        /// <summary>
        /// Given a count of black/white/black/white/black pixels just seen and an end position,
        /// figures the location of the center of this run.
        /// </summary>
        ///
        private static float CenterFromEnd(int[] stateCount, int end)
        {
            return (float)(end - stateCount[4] - stateCount[3]) - stateCount[2] / 2.0f;
        }


        /// <param name="stateCount">count of black/white/black/white/black pixels just read</param>
        /// <returns>true iff the proportions of the counts is close enough to the 1/1/3/1/1 ratios
        /// used by finder patterns to be considered a match</returns>
        protected static internal bool FoundPatternCross(int[] stateCount)
        {
            int totalModuleSize = 0;
            for (int i = 0; i < 5; i++)
            {
                int count = stateCount[i];
                if (count == 0)
                {
                    return false;
                }
                totalModuleSize += count;
            }
            if (totalModuleSize < 7)
            {
                return false;
            }
            int moduleSize = (totalModuleSize << INTEGER_MATH_SHIFT) / 7;
            int maxVariance = moduleSize / 2;
            // Allow less than 50% variance from 1-1-3-1-1 proportions
            return Math.Abs(moduleSize - (stateCount[0] << INTEGER_MATH_SHIFT)) < maxVariance && Math.Abs(moduleSize - (stateCount[1] << INTEGER_MATH_SHIFT)) < maxVariance
                    && Math.Abs(3 * moduleSize - (stateCount[2] << INTEGER_MATH_SHIFT)) < 3 * maxVariance && Math.Abs(moduleSize - (stateCount[3] << INTEGER_MATH_SHIFT)) < maxVariance
                    && Math.Abs(moduleSize - (stateCount[4] << INTEGER_MATH_SHIFT)) < maxVariance;
        }

        private int[] GetCrossCheckStateCount()
        {
            crossCheckStateCount[0] = 0;
            crossCheckStateCount[1] = 0;
            crossCheckStateCount[2] = 0;
            crossCheckStateCount[3] = 0;
            crossCheckStateCount[4] = 0;
            return crossCheckStateCount;
        }

        /// <summary>
        /// <p>After a horizontal scan finds a potential finder pattern, this method
        /// "cross-checks" by scanning down vertically through the center of the possible
        /// finder pattern to see if the same proportion is detected.</p>
        /// </summary>
        ///
        /// <param name="startI">row where a finder pattern was detected</param>
        /// <param name="centerJ">center of the section that appears to cross a finder pattern</param>
        /// <param name="maxCount"></param>
        /// <returns>vertical center of finder pattern, or 
        /// <see cref="null"/>
        ///  if not found</returns>
        private float CrossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
        {
            BitMatrix image = this.image;

            int maxI = image.GetHeight();
            int[] stateCount = GetCrossCheckStateCount();

            // Start counting up from center
            int i = startI;
            while (i >= 0 && image.Get(centerJ, i))
            {
                stateCount[2]++;
                i--;
            }
            if (i < 0)
            {
                return System.Single.NaN;
            }
            while (i >= 0 && !image.Get(centerJ, i) && stateCount[1] <= maxCount)
            {
                stateCount[1]++;
                i--;
            }
            // If already too many modules in this state or ran off the edge:
            if (i < 0 || stateCount[1] > maxCount)
            {
                return System.Single.NaN;
            }
            while (i >= 0 && image.Get(centerJ, i) && stateCount[0] <= maxCount)
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
            while (i < maxI && image.Get(centerJ, i))
            {
                stateCount[2]++;
                i++;
            }
            if (i == maxI)
            {
                return System.Single.NaN;
            }
            while (i < maxI && !image.Get(centerJ, i) && stateCount[3] < maxCount)
            {
                stateCount[3]++;
                i++;
            }
            if (i == maxI || stateCount[3] >= maxCount)
            {
                return System.Single.NaN;
            }
            while (i < maxI && image.Get(centerJ, i) && stateCount[4] < maxCount)
            {
                stateCount[4]++;
                i++;
            }
            if (stateCount[4] >= maxCount)
            {
                return System.Single.NaN;
            }

            // If we found a finder-pattern-like section, but its size is more than 40% different than
            // the original, assume it's a false positive
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
            if (5 * Math.Abs(stateCountTotal - originalStateCountTotal) >= 2 * originalStateCountTotal)
            {
                return System.Single.NaN;
            }

            return (FoundPatternCross(stateCount)) ? CenterFromEnd(stateCount, i) : System.Single.NaN;
        }

        /// <summary>
        /// <p>Like <see cref="M:Com.Google.Zxing.Qrcode.Detector.FinderPatternFinder.CrossCheckVertical(System.Int32,System.Int32,System.Int32,System.Int32)"/>, and in fact is basically identical,
        /// except it reads horizontally instead of vertically. This is used to cross-cross
        /// check a vertical cross check and locate the real center of the alignment pattern.</p>
        /// </summary>
        private float CrossCheckHorizontal(int startJ, int centerI, int maxCount, int originalStateCountTotal)
        {
            BitMatrix image_0 = this.image;

            int maxJ = image_0.GetWidth();
            int[] stateCount = GetCrossCheckStateCount();

            int j = startJ;
            while (j >= 0 && image_0.Get(j, centerI))
            {
                stateCount[2]++;
                j--;
            }
            if (j < 0)
            {
                return System.Single.NaN;
            }
            while (j >= 0 && !image_0.Get(j, centerI) && stateCount[1] <= maxCount)
            {
                stateCount[1]++;
                j--;
            }
            if (j < 0 || stateCount[1] > maxCount)
            {
                return System.Single.NaN;
            }
            while (j >= 0 && image_0.Get(j, centerI) && stateCount[0] <= maxCount)
            {
                stateCount[0]++;
                j--;
            }
            if (stateCount[0] > maxCount)
            {
                return System.Single.NaN;
            }

            j = startJ + 1;
            while (j < maxJ && image_0.Get(j, centerI))
            {
                stateCount[2]++;
                j++;
            }
            if (j == maxJ)
            {
                return System.Single.NaN;
            }
            while (j < maxJ && !image_0.Get(j, centerI) && stateCount[3] < maxCount)
            {
                stateCount[3]++;
                j++;
            }
            if (j == maxJ || stateCount[3] >= maxCount)
            {
                return System.Single.NaN;
            }
            while (j < maxJ && image_0.Get(j, centerI) && stateCount[4] < maxCount)
            {
                stateCount[4]++;
                j++;
            }
            if (stateCount[4] >= maxCount)
            {
                return System.Single.NaN;
            }

            // If we found a finder-pattern-like section, but its size is significantly different than
            // the original, assume it's a false positive
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
            if (5 * Math.Abs(stateCountTotal - originalStateCountTotal) >= originalStateCountTotal)
            {
                return System.Single.NaN;
            }

            return (FoundPatternCross(stateCount)) ? CenterFromEnd(stateCount, j) : System.Single.NaN;
        }

        /// <summary>
        /// This is called when a horizontal scan finds a possible alignment pattern. It will
        /// cross check with a vertical scan, and if successful, will, ah, cross-cross-check
        /// with another horizontal scan. This is needed primarily to locate the real horizontal
        /// center of the pattern in cases of extreme skew.
        /// If that succeeds the finder pattern location is added to a list that tracks
        /// the number of times each location has been nearly-matched as a finder pattern.
        /// Each additional find is more evidence that the location is in fact a finder
        /// pattern center
        /// </summary>
        /// <param name="stateCount">reading state module counts from horizontal scan</param>
        /// <param name="i">row where finder pattern may be found</param>
        /// <param name="j">end of possible finder pattern in row</param>
        /// <returns>
        /// true if a finder pattern candidate was found this time
        /// </returns>
        protected internal bool HandlePossibleCenter(int[] stateCount, int i, int j)
        {
            int stateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
            float centerJ = CenterFromEnd(stateCount, j);
            float centerI = CrossCheckVertical(i, (int)centerJ, stateCount[2], stateCountTotal);
            if (!Single.IsNaN(centerI))
            {
                // Re-cross check
                centerJ = CrossCheckHorizontal((int)centerJ, (int)centerI, stateCount[2], stateCountTotal);
                if (!Single.IsNaN(centerJ))
                {
                    float estimatedModuleSize = (float)stateCountTotal / 7.0f;
                    bool found = false;
                    for (int index = 0; index < possibleCenters.Count; index++)
                    {
                        FinderPattern center = (FinderPattern)possibleCenters[index];
                        // Look for about the same center and module size:
                        if (center.AboutEquals(estimatedModuleSize, centerI, centerJ))
                        {
                            possibleCenters[index] = center.CombineEstimate(centerI, centerJ, estimatedModuleSize);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        FinderPattern point = new FinderPattern(centerJ, centerI, estimatedModuleSize);
                        possibleCenters.Add(point);
                        if (resultPointCallback != null)
                        {
                            resultPointCallback.FoundPossibleResultPoint(point);
                        }
                    }
                    return true;
                }
            }
            return false;
        }


        /// <returns>number of rows we could safely skip during scanning, based on the first
        /// two finder patterns that have been located. In some cases their position will
        /// allow us to infer that the third pattern must lie below a certain point farther
        /// down in the image.</returns>
        private int FindRowSkip()
        {
            int max = possibleCenters.Count;
            if (max <= 1)
            {
                return 0;
            }
            FinderPattern firstConfirmedCenter = null;
            /* foreach */
            foreach (FinderPattern center in possibleCenters)
            {
                if (center.Count >= CENTER_QUORUM)
                {
                    if (firstConfirmedCenter == null)
                    {
                        firstConfirmedCenter = center;
                    }
                    else
                    {
                        // We have two confirmed centers
                        // How far down can we skip before resuming looking for the next
                        // pattern? In the worst case, only the difference between the
                        // difference in the x / y coordinates of the two centers.
                        // This is the case where you find top left last.
                        hasSkipped = true;
                        return (int)(Math.Abs(firstConfirmedCenter.X - center.X) - Math.Abs(firstConfirmedCenter.Y - center.Y)) / 2;
                    }
                }
            }
            return 0;
        }


        /// <returns>true iff we have found at least 3 finder patterns that have been detected
        /// at least 
        /// <see cref="F:Com.Google.Zxing.Qrcode.Detector.FinderPatternFinder.CENTER_QUORUM"/>
        ///  times each, and, the estimated module size of the
        /// candidates is "pretty similar"</returns>
        private bool HaveMultiplyConfirmedCenters()
        {
            int confirmedCount = 0;
            float totalModuleSize = 0.0f;
            int max = possibleCenters.Count;
            /* foreach */
            foreach (FinderPattern pattern in possibleCenters)
            {
                if (pattern.Count >= CENTER_QUORUM)
                {
                    confirmedCount++;
                    totalModuleSize += pattern.EstimatedModuleSize;
                }
            }
            if (confirmedCount < 3)
            {
                return false;
            }
            // OK, we have at least 3 confirmed centers, but, it's possible that one is a "false positive"
            // and that we need to keep looking. We detect this by asking if the estimated module sizes
            // vary too much. We arbitrarily say that when the total deviation from average exceeds
            // 5% of the total module size estimates, it's too much.
            float average = totalModuleSize / (float)max;
            float totalDeviation = 0.0f;
            /* foreach */
            foreach (FinderPattern pat in possibleCenters)
            {
                totalDeviation += Math.Abs(pat.EstimatedModuleSize - average);
            }
            return totalDeviation <= 0.05f * totalModuleSize;
        }


        /// <returns>the 3 best 
        /// <see cref="T:Com.Google.Zxing.Qrcode.Detector.FinderPattern"/>
        /// s from our list of candidates. The "best" are
        /// those that have been detected at least 
        /// <see cref="F:Com.Google.Zxing.Qrcode.Detector.FinderPatternFinder.CENTER_QUORUM"/>
        ///  times, and whose module
        /// size differs from the average among those patterns the least</returns>
        /// <exception cref="NotFoundException">if 3 such finder patterns do not exist</exception>
        private FinderPattern[] SelectBestPatterns()
        {

            int startSize = possibleCenters.Count;
            if (startSize < 3)
            {
                // Couldn't find enough finder patterns
                throw NotFoundException.Instance;
            }

            // Filter outlier possibilities whose module size is too different
            if (startSize > 3)
            {
                // But we can only afford to do so if we have at least 4 possibilities to choose from
                float totalModuleSize = 0.0f;
                float square = 0.0f;
                /* foreach */
                foreach (FinderPattern center in possibleCenters)
                {
                    float size = center.EstimatedModuleSize;
                    totalModuleSize += size;
                    square += size * size;
                }
                float average = totalModuleSize / (float)startSize;
                float stdDev = (float)Math.Sqrt(square / startSize - average * average);

                MessagingToolkit.Barcode.Common.Collections.InsertionSort(possibleCenters,
                      new FinderPatternFinder.FurthestFromAverageComparator(average));

                float limit = Math.Max(0.2f * average, stdDev);

                for (int i = 0; i < possibleCenters.Count && possibleCenters.Count > 3; i++)
                {
                    FinderPattern pattern = (FinderPattern)possibleCenters[i];
                    if (Math.Abs(pattern.EstimatedModuleSize - average) > limit)
                    {
                        possibleCenters.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (possibleCenters.Count > 3)
            {
                // Throw away all but those first size candidate points we found.

                float totalModuleSize = 0.0f;
                /* foreach */
                foreach (FinderPattern possibleCenter in possibleCenters)
                {
                    totalModuleSize += possibleCenter.EstimatedModuleSize;
                }

                float average = totalModuleSize / (float)possibleCenters.Count;
                MessagingToolkit.Barcode.Common.Collections.InsertionSort(possibleCenters,
                                new FinderPatternFinder.CenterComparator(
                                  average));

                possibleCenters.RemoveRange(3, possibleCenters.Count - 3);
            }

            return new FinderPattern[] {
					(FinderPattern) possibleCenters[0],
					(FinderPattern) possibleCenters[1],
					(FinderPattern) possibleCenters[2] };
        }

        /// <summary>
        /// <p>Orders by furthest from average</p>
        /// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
        private class FurthestFromAverageComparator : Comparator
        {
            private readonly float average;

            public FurthestFromAverageComparator(float f)
            {
                average = f;
            }

            public virtual int Compare(Object center1, Object center2)
            {
                float dA = Math.Abs(((FinderPattern)center2)
                                    .EstimatedModuleSize - average);
                float dB = Math.Abs(((FinderPattern)center1)
                                    .EstimatedModuleSize - average);
                return (dA < dB) ? -1 : (dA == dB) ? 0 : 1;
            }

        }

        /// <summary>
        /// <p>Orders by <see cref="M:Com.Google.Zxing.Qrcode.Detector.FinderPattern.GetCount"/>, descending.</p>
        /// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
        private class CenterComparator : Comparator
        {
            private readonly float average;

            public CenterComparator(float f)
            {
                average = f;
            }

            public virtual int Compare(Object center1, Object center2)
            {
                if (((FinderPattern)center2).Count == ((FinderPattern)center1).Count)
                {
                    float dA = Math.Abs(((FinderPattern)center2)
                                            .EstimatedModuleSize - average);
                    float dB = Math.Abs(((FinderPattern)center1)
                                            .EstimatedModuleSize - average);
                    return (dA < dB) ? 1 : (dA == dB) ? 0 : -1;
                }
                else
                {
                    return ((FinderPattern)center2).Count
                            - ((FinderPattern)center1).Count;
                }
            }
        }
    }
}
