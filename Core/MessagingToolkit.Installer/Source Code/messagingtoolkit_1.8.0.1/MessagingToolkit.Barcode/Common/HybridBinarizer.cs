using System;
using Binarizer = MessagingToolkit.Barcode.Binarizer;
using LuminanceSource = MessagingToolkit.Barcode.LuminanceSource;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// This class implements a local thresholding algorithm, which while slower than the
    /// GlobalHistogramBinarizer, is fairly efficient for what it does. It is designed for
    /// high frequency images of barcodes with black data on white backgrounds. For this application,
    /// it does a much better job than a global blackpoint with severe shadows and gradients.
    /// However it tends to produce artifacts on lower frequency images and is therefore not
    /// a good general purpose binarizer for uses outside ZXing.
    /// This class extends GlobalHistogramBinarizer, using the older histogram approach for 1D readers,
    /// and the newer local approach for 2D readers. 1D decoding using a per-row histogram is already
    /// inherently local, and only fails for horizontal gradients. We can revisit that problem later,
    /// but for now it was not a win to use local blocks for 1D.
    /// This Binarizer is the default for the unit tests and the recommended class for library users.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public class HybridBinarizer : GlobalHistogramBinarizer
    {
        // This class uses 5x5 blocks to compute local luminance, where each block is 8x8 pixels.
        // So this is the smallest dimension in each axis we can accept.
        private const int BLOCK_SIZE_POWER = 3;
        private const int BLOCK_SIZE = 1 << BLOCK_SIZE_POWER; // ...0100...00
        private const int BLOCK_SIZE_MASK = BLOCK_SIZE - 1; // ...0011...11
        private const int MINIMUM_DIMENSION = BLOCK_SIZE * 5;
        private const int MIN_DYNAMIC_RANGE = 24;

        private BitMatrix matrix;

        public HybridBinarizer(LuminanceSource source)
            : base(source)
        {
        }

        /// <summary>
        /// Calculates the final BitMatrix once for all requests. This could be called once from the
        /// constructor instead, but there are some advantages to doing it lazily, such as making
        /// profiling easier, and not doing heavy lifting when callers don't expect it.
        /// </summary>
        ///
        public override BitMatrix BlackMatrix
        {
            get
            {
                if (matrix != null)
                {
                    return matrix;
                }
                LuminanceSource source = this.LuminanceSource;
                int width = source.Width;
                int height = source.Height;
                if (width >= MINIMUM_DIMENSION && height >= MINIMUM_DIMENSION)
                {
                    byte[] luminances = source.Matrix;
                    int subWidth = width >> BLOCK_SIZE_POWER;
                    if ((width & BLOCK_SIZE_MASK) != 0)
                    {
                        subWidth++;
                    }
                    int subHeight = height >> BLOCK_SIZE_POWER;
                    if ((height & BLOCK_SIZE_MASK) != 0)
                    {
                        subHeight++;
                    }
                    int[][] blackPoints = CalculateBlackPoints(luminances, subWidth, subHeight, width, height);

                    BitMatrix newMatrix = new BitMatrix(width, height);
                    CalculateThresholdForBlock(luminances, subWidth, subHeight, width, height, blackPoints, newMatrix);
                    matrix = newMatrix;
                }
                else
                {
                    // If the image is too small, fall back to the global histogram approach.
                    matrix = base.BlackMatrix;
                }
                return matrix;
            }
        }

        public override Binarizer CreateBinarizer(LuminanceSource source)
        {
            return new HybridBinarizer(source);
        }

        /// <summary>
        /// For each block in the image, calculate the average black point using a 5x5 grid
        /// of the blocks around it. Also handles the corner cases (fractional blocks are computed based
        /// on the last pixels in the row/column which are also used in the previous block).
        /// </summary>
        ///
        private static void CalculateThresholdForBlock(byte[] luminances, int subWidth, int subHeight, int width, int height, int[][] blackPoints, BitMatrix matrix_0)
        {
            for (int y = 0; y < subHeight; y++)
            {
                int yoffset = y << BLOCK_SIZE_POWER;
                int maxYOffset = height - BLOCK_SIZE;
                if (yoffset > maxYOffset)
                {
                    yoffset = maxYOffset;
                }
                for (int x = 0; x < subWidth; x++)
                {
                    int xoffset = x << BLOCK_SIZE_POWER;
                    int maxXOffset = width - BLOCK_SIZE;
                    if (xoffset > maxXOffset)
                    {
                        xoffset = maxXOffset;
                    }
                    int left = Cap(x, 2, subWidth - 3);
                    int top = Cap(y, 2, subHeight - 3);
                    int sum = 0;
                    for (int z = -2; z <= 2; z++)
                    {
                        int[] blackRow = blackPoints[top + z];
                        sum += blackRow[left - 2] + blackRow[left - 1] + blackRow[left] + blackRow[left + 1] + blackRow[left + 2];
                    }
                    int average = sum / 25;
                    ThresholdBlock(luminances, xoffset, yoffset, average, width, matrix_0);
                }
            }
        }

        private static int Cap(int val, int min, int max)
        {
            return (val < min) ? min : (val > max) ? max : val;
        }

        /// <summary>
        /// Applies a single threshold to a block of pixels.
        /// </summary>
        ///
        private static void ThresholdBlock(byte[] luminances, int xoffset, int yoffset, int threshold, int stride, BitMatrix matrix_0)
        {
            for (int y = 0, offset = yoffset * stride + xoffset; y < BLOCK_SIZE; y++, offset += stride)
            {
                for (int x = 0; x < BLOCK_SIZE; x++)
                {
                    // Comparison needs to be <= so that black == 0 pixels are black even if the threshold is 0.
                    if ((luminances[offset + x] & 0xFF) <= threshold)
                    {
                        matrix_0.Set(xoffset + x, yoffset + y);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates a single black point for each block of pixels and saves it away.
        /// See the following thread for a discussion of this algorithm:
        /// http://groups.google.com/group/zxing/browse_thread/thread/d06efa2c35a7ddc0
        /// </summary>
        ///
        private static int[][] CalculateBlackPoints(byte[] luminances, int subWidth, int subHeight, int width, int height)
        {
            int[][] blackPoints = new int[subHeight][];
            for (int i = 0; i < subHeight; i++)
            {
                blackPoints[i] = new int[subWidth];
            }
            for (int y = 0; y < subHeight; y++)
            {
                int yoffset = y << BLOCK_SIZE_POWER;
                int maxYOffset = height - BLOCK_SIZE;
                if (yoffset > maxYOffset)
                {
                    yoffset = maxYOffset;
                }
                for (int x = 0; x < subWidth; x++)
                {
                    int xoffset = x << BLOCK_SIZE_POWER;
                    int maxXOffset = width - BLOCK_SIZE;
                    if (xoffset > maxXOffset)
                    {
                        xoffset = maxXOffset;
                    }
                    int sum = 0;
                    int min = 0xFF;
                    int max = 0;
                    for (int yy = 0, offset = yoffset * width + xoffset; yy < BLOCK_SIZE; yy++, offset += width)
                    {
                        for (int xx = 0; xx < BLOCK_SIZE; xx++)
                        {
                            int pixel = luminances[offset + xx] & 0xFF;
                            sum += pixel;
                            // still looking for good contrast
                            if (pixel < min)
                            {
                                min = pixel;
                            }
                            if (pixel > max)
                            {
                                max = pixel;
                            }
                        }
                        // short-circuit min/max tests once dynamic range is met
                        if (max - min > MIN_DYNAMIC_RANGE)
                        {
                            // finish the rest of the rows quickly
                            for (yy++, offset += width; yy < BLOCK_SIZE; yy++, offset += width)
                            {
                                for (int xx_0 = 0; xx_0 < BLOCK_SIZE; xx_0++)
                                {
                                    sum += luminances[offset + xx_0] & 0xFF;
                                }
                            }
                        }
                    }

                    // The default estimate is the average of the values in the block.
                    int average = sum >> (BLOCK_SIZE_POWER * 2);
                    if (max - min <= MIN_DYNAMIC_RANGE)
                    {
                        // If variation within the block is low, assume this is a block with only light or only
                        // dark pixels. In that case we do not want to use the average, as it would divide this
                        // low contrast area into black and white pixels, essentially creating data out of noise.
                        //
                        // The default assumption is that the block is light/background. Since no estimate for
                        // the level of dark pixels exists locally, use half the min for the block.
                        average = min >> 1;

                        if (y > 0 && x > 0)
                        {
                            // Correct the "white background" assumption for blocks that have neighbors by comparing
                            // the pixels in this block to the previously calculated black points. This is based on
                            // the fact that dark barcode symbology is always surrounded by some amount of light
                            // background for which reasonable black point estimates were made. The bp estimated at
                            // the boundaries is used for the interior.

                            // The (min < bp) is arbitrary but works better than other heuristics that were tried.
                            int averageNeighborBlackPoint = (blackPoints[y - 1][x] + (2 * blackPoints[y][x - 1]) + blackPoints[y - 1][x - 1]) >> 2;
                            if (min < averageNeighborBlackPoint)
                            {
                                average = averageNeighborBlackPoint;
                            }
                        }
                    }
                    blackPoints[y][x] = average;
                }
            }
            return blackPoints;
        }
    }
}