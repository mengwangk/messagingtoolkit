
using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// Encapsulates functionality and implementation that is common to all families
    /// of one-dimensional barcodes.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal abstract class OneDDecoder : IDecoder
    {

        protected internal const int IntegerMathShift = 8;
        protected internal const int PatternMatchResultScaleFactor = 1 << IntegerMathShift;

        public virtual Result Decode(BinaryBitmap image)
        {
            return Decode(image, null);
        }

        // Note that we don't try rotation without the try harder flag, even if rotation was supported.
        public virtual Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            try
            {
                return DoDecode(image, decodingOptions);
            }
            catch (NotFoundException nfe)
            {
                bool tryHarder = decodingOptions != null
                        && decodingOptions.ContainsKey(DecodeOptions.TryHarder);
                if (tryHarder && image.RotateSupported)
                {
                    BinaryBitmap rotatedImage = image.RotateCounterClockwise();
                    Result result = DoDecode(rotatedImage, decodingOptions);
                    // Record that we found it rotated 90 degrees CCW / 270 degrees CW
                    IDictionary<ResultMetadataType, object> metadata = result.ResultMetadata;
                    int orientation = 270;
                    if (metadata != null
                            && metadata.ContainsKey(MessagingToolkit.Barcode.ResultMetadataType.Orientation))
                    {
                        // But if we found it reversed in doDecode(), add in that result here:
                        orientation = (orientation + ((Int32)metadata[MessagingToolkit.Barcode.ResultMetadataType.Orientation])) % 360;
                    }
                    result.PutMetadata(MessagingToolkit.Barcode.ResultMetadataType.Orientation, ((int)(orientation)));
                    // Update result points
                    ResultPoint[] points = result.ResultPoints;
                    int height = rotatedImage.Height;
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = new ResultPoint(height - points[i].Y - 1,
                                points[i].X);
                    }
                    return result;
                }
                else
                {
                    throw nfe;
                }
            }
        }

        public virtual void Reset()
        {
            // do nothing
        }

        /// <summary>
        /// We're going to examine rows from the middle outward, searching alternately above and below the
        /// middle, and farther out each time. rowStep is the number of rows between each successive
        /// attempt above and below the middle. So we'd scan row middle, then middle - rowStep, then
        /// middle + rowStep, then middle - (2/// rowStep), etc.
        /// rowStep is bigger as the image is taller, but is always at least 1. We've somewhat arbitrarily
        /// decided that moving up and down by about 1/16 of the image is pretty good; we try more of the
        /// image if "trying harder".
        /// </summary>
        ///
        /// <param name="image">The image to decode</param>
        /// <param name="hints">Any hints that were requested</param>
        /// <returns>The contents of the decoded barcode</returns>
        /// <exception cref="NotFoundException">Any spontaneous errors which occur</exception>
        private Result DoDecode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            int width = image.Width;
            int height = image.Height;
            BitArray row = new BitArray(width);

            int middle = height >> 1;
            bool tryHarder = decodingOptions != null
                    && decodingOptions.ContainsKey(DecodeOptions.TryHarder);
            int rowStep = Math.Max(1, height >> ((tryHarder) ? 8 : 5));
            int maxLines;
            if (tryHarder)
            {
                maxLines = height; // Look at the whole image, not just the center
            }
            else
            {
                maxLines = 15; // 15 rows spaced 1/32 apart is roughly the middle half of the image
            }

            for (int x = 0; x < maxLines; x++)
            {

                // Scanning from the middle out. Determine which row we're looking at next:
                int rowStepsAboveOrBelow = (x + 1) >> 1;
                bool isAbove = (x & 0x01) == 0; // i.e. is x even?
                int rowNumber = middle + rowStep
                        * ((isAbove) ? rowStepsAboveOrBelow : -rowStepsAboveOrBelow);
                if (rowNumber < 0 || rowNumber >= height)
                {
                    // Oops, if we run off the top or bottom, stop
                    break;
                }

                // Estimate black point for this row and load it:
                try
                {
                    row = image.GetBlackRow(rowNumber, row);
                }
                catch (NotFoundException)
                {
                    continue;
                }

                // While we have the image data in a BitArray, it's fairly cheap to reverse it in place to
                // handle decoding upside down barcodes.
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    if (attempt == 1)
                    { // trying again?
                        row.Reverse(); // reverse the row and continue
                        // This means we will only ever draw result points *once* in the life of this method
                        // since we want to avoid drawing the wrong points after flipping the row, and,
                        // don't want to clutter with noise from every single row scan -- just the scans
                        // that start on the center line.
                        if (decodingOptions != null
                                && decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback))
                        {
                            Dictionary<DecodeOptions, object> newDecodingOptions = new Dictionary<DecodeOptions, object>(); // Can't use clone() in J2ME
                            IEnumerator<DecodeOptions> decodeEnum =  decodingOptions.Keys.GetEnumerator();
                            while (decodeEnum.MoveNext())
                            {
                                DecodeOptions key = decodeEnum.Current;
                                if (key != DecodeOptions.NeedResultPointCallback)
                                {
                                    newDecodingOptions.Add(key, decodingOptions[key]);
                                }
                            }
                            decodingOptions = newDecodingOptions;
                        }
                    }
                    try
                    {
                        // Look for a barcode
                        Result result = DecodeRow(rowNumber, row, decodingOptions);
                        // We found our barcode
                        if (attempt == 1)
                        {
                            // But it was upside down, so note that
                            result.PutMetadata(MessagingToolkit.Barcode.ResultMetadataType.Orientation,
                                    ((int)(180)));
                            // And remember to flip the result points horizontally.
                            ResultPoint[] points = result.ResultPoints;
                            points[0] = new ResultPoint(width - points[0].X
                                    - 1, points[0].Y);
                            points[1] = new ResultPoint(width - points[1].X
                                    - 1, points[1].Y);
                        }
                        return result;
                    }
                    catch (BarcodeDecoderException)
                    {
                        // continue -- just couldn't decode this row
                    }
                }
            }

            throw NotFoundException.Instance;
        }

        /// <summary>
        /// Records the size of successive runs of white and black pixels in a row, starting at a given point.
        /// The values are recorded in the given array, and the number of runs recorded is equal to the size
        /// of the array. If the row starts on a white pixel at the given start point, then the first count
        /// recorded is the run of white pixels starting from that point; likewise it is the count of a run
        /// of black pixels if the row begin on a black pixels at that point.
        /// </summary>
        ///
        /// <param name="row">row to count from</param>
        /// <param name="start">offset into row to start at</param>
        /// <param name="counters">array into which to record counts</param>
        /// <exception cref="NotFoundException">if counters cannot be filled entirely from row before running outof pixels</exception>
        protected static internal void RecordPattern(BitArray row, int start, int[] counters)
        {
            int numCounters = counters.Length;
            for (int idx = 0; idx < numCounters; idx++)
            {
                counters[idx] = 0;
            }
            int end = row.GetSize();
            if (start >= end)
            {
                throw NotFoundException.Instance;
            }
            bool isWhite = !row.Get(start);
            int counterPosition = 0;
            int i = start;
            while (i < end)
            {
                if (row.Get(i) ^ isWhite)
                { // that is, exactly one is true
                    counters[counterPosition]++;
                }
                else
                {
                    counterPosition++;
                    if (counterPosition == numCounters)
                    {
                        break;
                    }
                    else
                    {
                        counters[counterPosition] = 1;
                        isWhite = !isWhite;
                    }
                }
                i++;
            }
            // If we read fully the last section of pixels and filled up our counters -- or filled
            // the last counter but ran off the side of the image, OK. Otherwise, a problem.
            if (!(counterPosition == numCounters || (counterPosition == numCounters - 1 && i == end)))
            {
                throw NotFoundException.Instance;
            }
        }

        protected static internal void RecordPatternInReverse(BitArray row, int start,
                int[] counters)
        {
            // This could be more efficient I guess
            int numTransitionsLeft = counters.Length;
            bool last = row.Get(start);
            while (start > 0 && numTransitionsLeft >= 0)
            {
                if (row.Get(--start) != last)
                {
                    numTransitionsLeft--;
                    last = !last;
                }
            }
            if (numTransitionsLeft >= 0)
            {
                throw NotFoundException.Instance;
            }
            RecordPattern(row, start + 1, counters);
        }

        /// <summary>
        /// Determines how closely a set of observed counts of runs of black/white values matches a given
        /// target pattern. This is reported as the ratio of the total variance from the expected pattern
        /// proportions across all pattern elements, to the length of the pattern.
        /// </summary>
        ///
        /// <param name="counters">observed counters</param>
        /// <param name="pattern">expected pattern</param>
        /// <param name="maxIndividualVariance">The most any counter can differ before we give up</param>
        /// <returns>ratio of total variance between counters and pattern compared to total pattern size,
        /// where the ratio has been multiplied by 256. So, 0 means no variance (perfect match); 256 means
        /// the total variance between counters and patterns equals the pattern length, higher values mean
        /// even more variance</returns>
        protected static internal int PatternMatchVariance(int[] counters, int[] pattern,
                int maxIndividualVariance)
        {
            int numCounters = counters.Length;
            int total = 0;
            int patternLength = 0;
            for (int i = 0; i < numCounters; i++)
            {
                total += counters[i];
                patternLength += pattern[i];
            }
            if (total < patternLength)
            {
                // If we don't even have one pixel per unit of bar width, assume this is too small
                // to reliably match, so fail:
                return Int32.MaxValue;
            }
            // We're going to fake floating-point math in integers. We just need to use more bits.
            // Scale up patternLength so that intermediate values below like scaledCounter will have
            // more "significant digits"
            int unitBarWidth = (total << IntegerMathShift) / patternLength;
            maxIndividualVariance = (maxIndividualVariance * unitBarWidth) >> IntegerMathShift;

            int totalVariance = 0;
            for (int x = 0; x < numCounters; x++)
            {
                int counter = counters[x] << IntegerMathShift;
                int scaledPattern = pattern[x] * unitBarWidth;
                int variance = (counter > scaledPattern) ? counter - scaledPattern
                        : scaledPattern - counter;
                if (variance > maxIndividualVariance)
                {
                    return Int32.MaxValue;
                }
                totalVariance += variance;
            }
            return totalVariance / total;
        }

        /// <summary>
        /// <p>Attempts to decode a one-dimensional barcode format given a single row of
        /// an image.</p>
        /// </summary>
        ///
        /// <param name="rowNumber">row number from top of the row</param>
        /// <param name="row">the black/white pixel data of the row</param>
        /// <param name="hints">decode hints</param>
        /// <returns>/// <see cref="null"/>
        ///  containing encoded string and start/end of barcode</returns>
        /// <exception cref="NotFoundException">if an error occurs or barcode cannot be found</exception>
        public abstract Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions);

    }
}
