using System;

namespace MessagingToolkit.Barcode.Common
{
    /// <summary>
    /// This Binarizer implementation uses the old ZXing global histogram approach. It is suitable
    /// for low-end mobile devices which don't have enough CPU or memory to use a local thresholding
    /// algorithm. However, because it picks a global black point, it cannot handle difficult shadows
    /// and gradients.
    /// Faster mobile devices and all desktop applications should probably use HybridBinarizer instead.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public class GlobalHistogramBinarizer : Binarizer
    {
        private const int LUMINANCE_BITS = 5;
        private const int LUMINANCE_SHIFT = 8 - LUMINANCE_BITS;
        private const int LUMINANCE_BUCKETS = 1 << LUMINANCE_BITS;
        private static readonly byte[] EMPTY = new byte[0];

        private byte[] luminances;
        private readonly int[] buckets;

        public GlobalHistogramBinarizer(LuminanceSource source)
            : base(source)
        {
            luminances = EMPTY;
            buckets = new int[LUMINANCE_BUCKETS];
        }

        // Applies simple sharpening to the row data to improve performance of the 1D Readers.
        public override BitArray GetBlackRow(int y, BitArray row)
        {
            LuminanceSource source = this.LuminanceSource;
            int width = source.Width;
            if (row == null || row.GetSize() < width)
            {
                row = new BitArray(width);
            }
            else
            {
                row.Clear();
            }

            InitArrays(width);
            byte[] localLuminances = source.GetRow(y, luminances);
            int[] localBuckets = buckets;
            for (int x = 0; x < width; x++)
            {
                int pixel = localLuminances[x] & 0xff;
                localBuckets[pixel >> LUMINANCE_SHIFT]++;
            }
            int blackPoint = EstimateBlackPoint(localBuckets);

            int left = localLuminances[0] & 0xff;
            int center = localLuminances[1] & 0xff;
            for (int x_0 = 1; x_0 < width - 1; x_0++)
            {
                int right = localLuminances[x_0 + 1] & 0xff;
                // A simple -1 4 -1 box filter with a weight of 2.
                int luminance = ((center << 2) - left - right) >> 1;
                if (luminance < blackPoint)
                {
                    row.Set(x_0);
                }
                left = center;
                center = right;
            }
            return row;
        }

        // Does not sharpen the data, as this call is intended to only be used by 2D Readers.
        public override BitMatrix BlackMatrix
        {
            get
            {
                LuminanceSource source = this.LuminanceSource;
                int width = source.Width;
                int height = source.Height;
                BitMatrix matrix = new BitMatrix(width, height);

                // Quickly calculates the histogram by sampling four rows from the image. This proved to be
                // more robust on the blackbox tests than sampling a diagonal as we used to do.
                InitArrays(width);
                int[] localBuckets = buckets;
                for (int y = 1; y < 5; y++)
                {
                    int row = height * y / 5;
                    byte[] localLuminances = source.GetRow(row, luminances);
                    int right = (width << 2) / 5;
                    for (int x = width / 5; x < right; x++)
                    {
                        int pixel = localLuminances[x] & 0xff;
                        localBuckets[pixel >> LUMINANCE_SHIFT]++;
                    }
                }
                int blackPoint = EstimateBlackPoint(localBuckets);

                // We delay reading the entire image luminance until the black point estimation succeeds.
                // Although we end up reading four rows twice, it is consistent with our motto of
                // "fail quickly" which is necessary for continuous scanning.
                byte[] localLuminances_0 = source.Matrix;
                for (int y_1 = 0; y_1 < height; y_1++)
                {
                    int offset = y_1 * width;
                    for (int x_2 = 0; x_2 < width; x_2++)
                    {
                        int pixel_3 = localLuminances_0[offset + x_2] & 0xff;
                        if (pixel_3 < blackPoint)
                        {
                            matrix.Set(x_2, y_1);
                        }
                    }
                }

                return matrix;
            }
        }

        public override Binarizer CreateBinarizer(LuminanceSource source)
        {
            return new GlobalHistogramBinarizer(source);
        }

        private void InitArrays(int luminanceSize)
        {
            if (luminances.Length < luminanceSize)
            {
                luminances = new byte[luminanceSize];
            }
            for (int x = 0; x < LUMINANCE_BUCKETS; x++)
            {
                buckets[x] = 0;
            }
        }

        private static int EstimateBlackPoint(int[] buckets)
        {
            // Find the tallest peak in the histogram.
            int numBuckets = buckets.Length;
            int maxBucketCount = 0;
            int firstPeak = 0;
            int firstPeakSize = 0;
            for (int x = 0; x < numBuckets; x++)
            {
                if (buckets[x] > firstPeakSize)
                {
                    firstPeak = x;
                    firstPeakSize = buckets[x];
                }
                if (buckets[x] > maxBucketCount)
                {
                    maxBucketCount = buckets[x];
                }
            }

            // Find the second-tallest peak which is somewhat far from the tallest peak.
            int secondPeak = 0;
            int secondPeakScore = 0;
            for (int x_1 = 0; x_1 < numBuckets; x_1++)
            {
                int distanceToBiggest = x_1 - firstPeak;
                // Encourage more distant second peaks by multiplying by square of distance.
                int score = buckets[x_1] * distanceToBiggest * distanceToBiggest;
                if (score > secondPeakScore)
                {
                    secondPeak = x_1;
                    secondPeakScore = score;
                }
            }

            // Make sure firstPeak corresponds to the black peak.
            if (firstPeak > secondPeak)
            {
                int temp = firstPeak;
                firstPeak = secondPeak;
                secondPeak = temp;
            }

            // If there is too little contrast in the image to pick a meaningful black point, throw rather
            // than waste time trying to decode the image, and risk false positives.
            if (secondPeak - firstPeak <= numBuckets >> 4)
            {
                throw NotFoundException.Instance;
            }

            // Find a valley between them that is low and closer to the white peak.
            int bestValley = secondPeak - 1;
            int bestValleyScore = -1;
            for (int x_2 = secondPeak - 1; x_2 > firstPeak; x_2--)
            {
                int fromFirst = x_2 - firstPeak;
                int score_3 = fromFirst * fromFirst * (secondPeak - x_2) * (maxBucketCount - buckets[x_2]);
                if (score_3 > bestValleyScore)
                {
                    bestValley = x_2;
                    bestValleyScore = score_3;
                }
            }

            return bestValley << LUMINANCE_SHIFT;
        }

    }
}
