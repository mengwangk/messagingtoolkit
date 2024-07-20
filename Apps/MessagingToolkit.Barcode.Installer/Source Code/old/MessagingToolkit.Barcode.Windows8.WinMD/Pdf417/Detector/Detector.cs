using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.Detector;

using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.Pdf417.Detector
{
    /// <summary>
    /// <p>Encapsulates logic that can detect a PDF417 Code in an image, even if the
    /// PDF417 Code is rotated or skewed, or partially obscured.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class Detector
    {
        private const int INTEGER_MATH_SHIFT = 8;
        private const int PATTERN_MATCH_RESULT_SCALE_FACTOR = 1 << INTEGER_MATH_SHIFT;
        private const int MAX_AVG_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.42f);
        private const int MAX_INDIVIDUAL_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.8f);
        private const int SKEW_THRESHOLD = 3;

        // B S B S B S B S Bar/Space pattern
        // 11111111 0 1 0 1 0 1 000
        private static readonly int[] START_PATTERN = { 8, 1, 1, 1, 1, 1, 1, 3 };

        // 11111111 0 1 0 1 0 1 000
        private static readonly int[] START_PATTERN_REVERSE = { 3, 1, 1, 1, 1, 1, 1, 8 };

        // 1111111 0 1 000 1 0 1 00 1
        private static readonly int[] STOP_PATTERN = { 7, 1, 1, 3, 1, 1, 1, 2, 1 };

        // B S B S B S B S B Bar/Space pattern
        // 1111111 0 1 000 1 0 1 00 1
        private static readonly int[] STOP_PATTERN_REVERSE = { 1, 2, 1, 1, 1, 3, 1, 1, 7 };

        private readonly BinaryBitmap image;

        public Detector(BinaryBitmap image)
        {
            this.image = image;
        }

        /// <summary>
        /// <p>Detects a PDF417 Code in an image, simply.</p>
        /// </summary>
        ///
        /// <returns>
        ///  encapsulating results of detecting a PDF417 Code</returns>
        /// <exception cref="NotFoundException">if no QR Code can be found</exception>
        public DetectorResult Detect()
        {
            return Detect(null);
        }

        /// <summary>
        /// <p>Detects a PDF417 Code in an image. Only checks 0 and 180 degree rotations.</p>
        /// </summary>
        ///
        /// <param name="decodingOptions">optional hints to detector</param>
        /// <returns>/// <see cref="null"/>
        ///  encapsulating results of detecting a PDF417 Code</returns>
        /// <exception cref="NotFoundException">if no PDF417 Code can be found</exception>
        public DetectorResult Detect(IDictionary<DecodeOptions, object> decodingOptions)
        {
            // Fetch the 1 bit matrix once up front.
            BitMatrix matrix = image.BlackMatrix;

            bool tryHarder = decodingOptions != null && decodingOptions.ContainsKey(DecodeOptions.TryHarder);
            var resultPointCallback = decodingOptions == null || !decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback)
                ? null : (ResultPointCallback)decodingOptions[DecodeOptions.NeedResultPointCallback];

            // Try to find the vertices assuming the image is upright.
            ResultPoint[] vertices = FindVertices(matrix, tryHarder);
            if (vertices == null)
            {
                // Maybe the image is rotated 180 degrees?
                vertices = FindVertices180(matrix, tryHarder);
                if (vertices != null)
                {
                    CorrectCodeWordVertices(vertices, true);
                }
            }
            else
            {
                CorrectCodeWordVertices(vertices, false);
            }

            if (vertices == null)
            {
                throw NotFoundException.Instance;
            }

            float moduleWidth = ComputeModuleWidth(vertices);
            if (moduleWidth < 1.0f)
            {
                throw NotFoundException.Instance;
            }

            int dimension = ComputeDimension(vertices[4], vertices[6], vertices[5], vertices[7], moduleWidth);
            if (dimension < 1)
            {
                throw NotFoundException.Instance;
            }

            int ydimension = ComputeYDimension(vertices[4], vertices[6],
                    vertices[5], vertices[7], moduleWidth);
            ydimension = ydimension > dimension ? ydimension : dimension;

            // Deskew and sample image.
            BitMatrix bits = SampleGrid(matrix, vertices[4], vertices[5], vertices[6], vertices[7], dimension, ydimension);
            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(vertices[5]);
                resultPointCallback.FoundPossibleResultPoint(vertices[4]);
                resultPointCallback.FoundPossibleResultPoint(vertices[6]);
                resultPointCallback.FoundPossibleResultPoint(vertices[7]);
            }
            return new DetectorResult(bits, new[] { vertices[5], vertices[4], vertices[6], vertices[7] });
        }

        /// <summary>
        /// Locate the vertices and the codewords area of a black blob using the Start
        /// and Stop patterns as locators.
        /// </summary>
        ///
        /// <param name="matrix">the scanned barcode image.</param>
        /// <returns>an array containing the vertices:
        /// vertices[0] x, y top left barcode
        /// vertices[1] x, y bottom left barcode
        /// vertices[2] x, y top right barcode
        /// vertices[3] x, y bottom right barcode
        /// vertices[4] x, y top left codeword area
        /// vertices[5] x, y bottom left codeword area
        /// vertices[6] x, y top right codeword area
        /// vertices[7] x, y bottom right codeword area</returns>
        private static ResultPoint[] FindVertices(BitMatrix matrix, bool tryHarder)
        {
            int height = matrix.GetHeight();
            int width = matrix.GetWidth();

            ResultPoint[] result = new ResultPoint[8];
            bool found = false;

            int[] counters = new int[START_PATTERN.Length];

            int rowStep = Math.Max(1, height >> ((tryHarder) ? 9 : 7));

            // Top Left
            for (int i = 0; i < height; i += rowStep)
            {
                int[] loc = FindGuardPattern(matrix, 0, i, width, false, START_PATTERN, counters);
                if (loc != null)
                {
                    result[0] = new ResultPoint(loc[0], i);
                    result[4] = new ResultPoint(loc[1], i);
                    found = true;
                    break;
                }
            }
            // Bottom left
            if (found)
            { // Found the Top Left vertex
                found = false;
                for (int i = height - 1; i > 0; i -= rowStep)
                {
                    int[] loc = FindGuardPattern(matrix, 0, i, width, false, START_PATTERN, counters);
                    if (loc != null)
                    {
                        result[1] = new ResultPoint(loc[0], i);
                        result[5] = new ResultPoint(loc[1], i);
                        found = true;
                        break;
                    }
                }
            }

            counters = new int[STOP_PATTERN.Length];

            // Top right
            if (found)
            { // Found the Bottom Left vertex
                found = false;
                for (int i = 0; i < height; i += rowStep)
                {
                    int[] loc = FindGuardPattern(matrix, 0, i, width, false, STOP_PATTERN, counters);
                    if (loc != null)
                    {
                        result[2] = new ResultPoint(loc[1], i);
                        result[6] = new ResultPoint(loc[0], i);
                        found = true;
                        break;
                    }
                }
            }
            // Bottom right
            if (found)
            { // Found the Top right vertex
                found = false;
                for (int i = height - 1; i > 0; i -= rowStep)
                {
                    int[] loc = FindGuardPattern(matrix, 0, i, width, false, STOP_PATTERN, counters);
                    if (loc != null)
                    {
                        result[3] = new ResultPoint(loc[1], i);
                        result[7] = new ResultPoint(loc[0], i);
                        found = true;
                        break;
                    }
                }
            }
            return (found) ? result : null;
        }

        /// <summary>
        /// Locate the vertices and the codewords area of a black blob using the Start
        /// and Stop patterns as locators. This assumes that the image is rotated 180
        /// degrees and if it locates the start and stop patterns at it will re-map
        /// the vertices for a 0 degree rotation.
        /// TODO: Change assumption about barcode location.
        /// </summary>
        ///
        /// <param name="matrix">the scanned barcode image.</param>
        /// <returns>an array containing the vertices:
        /// vertices[0] x, y top left barcode
        /// vertices[1] x, y bottom left barcode
        /// vertices[2] x, y top right barcode
        /// vertices[3] x, y bottom right barcode
        /// vertices[4] x, y top left codeword area
        /// vertices[5] x, y bottom left codeword area
        /// vertices[6] x, y top right codeword area
        /// vertices[7] x, y bottom right codeword area</returns>
        private static ResultPoint[] FindVertices180(BitMatrix matrix, bool tryHarder)
        {
            int height = matrix.GetHeight();
            int width = matrix.GetWidth();
            int halfWidth = width >> 1;

            ResultPoint[] result = new ResultPoint[8];
            bool found = false;

            int[] counters = new int[START_PATTERN_REVERSE.Length];

            int rowStep = Math.Max(1, height >> ((tryHarder) ? 9 : 7));

            // Top Left
            for (int i = height - 1; i > 0; i -= rowStep)
            {
                int[] loc = FindGuardPattern(matrix, halfWidth, i, halfWidth, true, START_PATTERN_REVERSE, counters);
                if (loc != null)
                {
                    result[0] = new ResultPoint(loc[1], i);
                    result[4] = new ResultPoint(loc[0], i);
                    found = true;
                    break;
                }
            }
            // Bottom Left
            if (found)
            { // Found the Top Left vertex
                found = false;
                for (int i_0 = 0; i_0 < height; i_0 += rowStep)
                {
                    int[] loc_1 = FindGuardPattern(matrix, halfWidth, i_0, halfWidth, true, START_PATTERN_REVERSE, counters);
                    if (loc_1 != null)
                    {
                        result[1] = new ResultPoint(loc_1[1], i_0);
                        result[5] = new ResultPoint(loc_1[0], i_0);
                        found = true;
                        break;
                    }
                }
            }

            counters = new int[STOP_PATTERN_REVERSE.Length];

            // Top Right
            if (found)
            { // Found the Bottom Left vertex
                found = false;
                for (int i_2 = height - 1; i_2 > 0; i_2 -= rowStep)
                {
                    int[] loc_3 = FindGuardPattern(matrix, 0, i_2, halfWidth, false, STOP_PATTERN_REVERSE, counters);
                    if (loc_3 != null)
                    {
                        result[2] = new ResultPoint(loc_3[0], i_2);
                        result[6] = new ResultPoint(loc_3[1], i_2);
                        found = true;
                        break;
                    }
                }
            }
            // Bottom Right
            if (found)
            { // Found the Top Right vertex
                found = false;
                for (int i_4 = 0; i_4 < height; i_4 += rowStep)
                {
                    int[] loc_5 = FindGuardPattern(matrix, 0, i_4, halfWidth, false, STOP_PATTERN_REVERSE, counters);
                    if (loc_5 != null)
                    {
                        result[3] = new ResultPoint(loc_5[0], i_4);
                        result[7] = new ResultPoint(loc_5[1], i_4);
                        found = true;
                        break;
                    }
                }
            }
            return (found) ? result : null;
        }

        /// <summary>
        /// Because we scan horizontally to detect the start and stop patterns, the vertical component of
        /// the codeword coordinates will be slightly wrong if there is any skew or rotation in the image.
        /// This method moves those points back onto the edges of the theoretically perfect bounding
        /// quadrilateral if needed.
        /// </summary>
        ///
        /// <param name="vertices">The eight vertices located by findVertices().</param>
        private static void CorrectCodeWordVertices(ResultPoint[] vertices, bool upsideDown)
        {

            float v0x = vertices[0].X;
            float v0y = vertices[0].Y;
            float v2x = vertices[2].X;
            float v2y = vertices[2].Y;
            float v4x = vertices[4].X;
            float v4y = vertices[4].Y;
            float v6x = vertices[6].X;
            float v6y = vertices[6].Y;

            float skew = v4y - v6y;
            if (upsideDown)
            {
                skew = -skew;
            }
            if (skew > SKEW_THRESHOLD)
            {
                // Fix v4
                float deltax = v6x - v0x;
                float deltay = v6y - v0y;
                float delta2 = deltax * deltax + deltay * deltay;
                float correction = (v4x - v0x) * deltax / delta2;
                vertices[4] = new ResultPoint(v0x + correction * deltax, v0y + correction * deltay);
            }
            else if (-skew > SKEW_THRESHOLD)
            {
                // Fix v6
                float deltax_0 = v2x - v4x;
                float deltay_1 = v2y - v4y;
                float delta2_2 = deltax_0 * deltax_0 + deltay_1 * deltay_1;
                float correction_3 = (v2x - v6x) * deltax_0 / delta2_2;
                vertices[6] = new ResultPoint(v2x - correction_3 * deltax_0, v2y - correction_3 * deltay_1);
            }

            float v1x = vertices[1].X;
            float v1y = vertices[1].Y;
            float v3x = vertices[3].X;
            float v3y = vertices[3].Y;
            float v5x = vertices[5].X;
            float v5y = vertices[5].Y;
            float v7x = vertices[7].X;
            float v7y = vertices[7].Y;

            skew = v7y - v5y;
            if (upsideDown)
            {
                skew = -skew;
            }
            if (skew > SKEW_THRESHOLD)
            {
                // Fix v5
                float deltax_4 = v7x - v1x;
                float deltay_5 = v7y - v1y;
                float delta2_6 = deltax_4 * deltax_4 + deltay_5 * deltay_5;
                float correction_7 = (v5x - v1x) * deltax_4 / delta2_6;
                vertices[5] = new ResultPoint(v1x + correction_7 * deltax_4, v1y + correction_7 * deltay_5);
            }
            else if (-skew > SKEW_THRESHOLD)
            {
                // Fix v7
                float deltax_8 = v3x - v5x;
                float deltay_9 = v3y - v5y;
                float delta2_10 = deltax_8 * deltax_8 + deltay_9 * deltay_9;
                float correction_11 = (v3x - v7x) * deltax_8 / delta2_10;
                vertices[7] = new ResultPoint(v3x - correction_11 * deltax_8, v3y - correction_11 * deltay_9);
            }
        }

        /// <summary>
        /// <p>Estimates module size (pixels in a module) based on the Start and End
        /// finder patterns.</p>
        /// </summary>
        ///
        /// <param name="vertices">vertices[1] x, y bottom left barcode vertices[2] x, y top right barcode vertices[3] x, y bottom right barcode vertices[4] x, y top left codeword area vertices[5] x, y bottom left codeword area vertices[6] x, y top right codeword area vertices[7] x, y bottom right codeword area</param>
        /// <returns>the module size.</returns>
        private static float ComputeModuleWidth(ResultPoint[] vertices)
        {
            float pixels1 = ResultPoint.Distance(vertices[0], vertices[4]);
            float pixels2 = ResultPoint.Distance(vertices[1], vertices[5]);
            float moduleWidth1 = (pixels1 + pixels2) / (17 * 2.0f);
            float pixels3 = ResultPoint.Distance(vertices[6], vertices[2]);
            float pixels4 = ResultPoint.Distance(vertices[7], vertices[3]);
            float moduleWidth2 = (pixels3 + pixels4) / (18 * 2.0f);
            return (moduleWidth1 + moduleWidth2) / 2.0f;
        }

        /// <summary>
        /// Computes the dimension (number of modules in a row) of the PDF417 Code
        /// based on vertices of the codeword area and estimated module size.
        /// </summary>
        ///
        /// <param name="topLeft">of codeword area</param>
        /// <param name="topRight">of codeword area</param>
        /// <param name="bottomLeft">of codeword area</param>
        /// <param name="bottomRight">of codeword are</param>
        /// <param name="moduleWidth">estimated module size</param>
        /// <returns>the number of modules in a row.</returns>
        private static int ComputeDimension(ResultPoint topLeft,
                            ResultPoint topRight, ResultPoint bottomLeft, ResultPoint bottomRight, float moduleWidth)
        {
            int topRowDimension = MathUtils.Round(ResultPoint.Distance(topLeft, topRight) / moduleWidth);
            int bottomRowDimension = MathUtils.Round(ResultPoint.Distance(bottomLeft, bottomRight) / moduleWidth);
            return ((((topRowDimension + bottomRowDimension) >> 1) + 8) / 17) * 17;
        }


        /// <summary>
        /// Computes the y dimension (number of modules in a column) of the PDF417 Code
        /// based on vertices of the codeword area and estimated module size.
        /// </summary>
        /// <param name="topLeft">The top left.</param>
        /// <param name="topRight">The top right.</param>
        /// <param name="bottomLeft">The bottom left.</param>
        /// <param name="bottomRight">The bottom right.</param>
        /// <param name="moduleWidth">Width of the module.</param>
        /// <returns></returns>
        private static int ComputeYDimension(ResultPoint topLeft,
                                            ResultPoint topRight,
                                            ResultPoint bottomLeft,
                                            ResultPoint bottomRight,
                                            float moduleWidth)
        {
            int leftColumnDimension = MathUtils.Round(ResultPoint.Distance(topLeft, bottomLeft) / moduleWidth);
            int rightColumnDimension = MathUtils.Round(ResultPoint.Distance(topRight, bottomRight) / moduleWidth);
            return (leftColumnDimension + rightColumnDimension) >> 1;
        }


        private static BitMatrix SampleGrid(BitMatrix matrix,
                                ResultPoint topLeft,
                                ResultPoint bottomLeft,
                                ResultPoint topRight,
                                ResultPoint bottomRight,
                                int xdimension,
                                int ydimension)
        {

            // Note that unlike the QR Code sampler, we didn't find the center of modules, but the
            // very corners. So there is no 0.5f here; 0.0f is right.
            GridSampler sampler = GridSampler.GetInstance();

            return sampler.SampleGrid(matrix, xdimension, ydimension, 0.0f, // p1ToX
                    0.0f, // p1ToY
                    xdimension, // p2ToX
                    0.0f, // p2ToY
                    xdimension, // p3ToX
                    ydimension, // p3ToY
                    0.0f, // p4ToX
                    ydimension, // p4ToY
                    topLeft.X, // p1FromX
                    topLeft.Y, // p1FromY
                    topRight.X, // p2FromX
                    topRight.Y, // p2FromY
                    bottomRight.X, // p3FromX
                    bottomRight.Y, // p3FromY
                    bottomLeft.X, // p4FromX
                    bottomLeft.Y); // p4FromY
        }

        /// <summary>
        /// Finds the guard pattern.
        /// </summary>
        /// <param name="matrix">row of black/white values to search</param>
        /// <param name="column">x position to start search</param>
        /// <param name="row">y position to start search</param>
        /// <param name="width">the number of pixels to search on this row</param>
        /// <param name="whiteFirst">if set to <c>true</c> [white first].</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="counters">array of counters, as long as pattern, to re-use</param>
        /// <returns>
        /// start/end horizontal offset of guard pattern, as an array of two ints.
        /// </returns>
        private static int[] FindGuardPattern(BitMatrix matrix, int column, int row, int width, bool whiteFirst, int[] pattern, int[] counters)
        {
            for (int i = 0; i < counters.Length; i++)
            {
                counters[i] = 0;
            }

            int patternLength = pattern.Length;
            bool isWhite = whiteFirst;

            int counterPosition = 0;
            int patternStart = column;
            for (int x = column; x < column + width; x++)
            {
                bool pixel = matrix.Get(x, row);
                if (pixel ^ isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        if (PatternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE) < MAX_AVG_VARIANCE)
                        {
                            return new int[] { patternStart, x };
                        }
                        patternStart += counters[0] + counters[1];
                        System.Array.Copy((Array)(counters), 2, (Array)(counters), 0, patternLength - 2);
                        counters[patternLength - 2] = 0;
                        counters[patternLength - 1] = 0;
                        counterPosition--;
                    }
                    else
                    {
                        counterPosition++;
                    }
                    counters[counterPosition] = 1;
                    isWhite = !isWhite;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines how closely a set of observed counts of runs of black/white
        /// values matches a given target pattern. This is reported as the ratio of
        /// the total variance from the expected pattern proportions across all
        /// pattern elements, to the length of the pattern.
        /// </summary>
        ///
        /// <param name="counters">observed counters</param>
        /// <param name="pattern">expected pattern</param>
        /// <param name="maxIndividualVariance">The most any counter can differ before we give up</param>
        /// <returns>ratio of total variance between counters and pattern compared to
        /// total pattern size, where the ratio has been multiplied by 256.
        /// So, 0 means no variance (perfect match); 256 means the total
        /// variance between counters and patterns equals the pattern length,
        /// higher values mean even more variance</returns>
        private static int PatternMatchVariance(int[] counters, int[] pattern, int maxIndividualVariance)
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
                // If we don't even have one pixel per unit of bar width, assume this
                // is too small to reliably match, so fail:
                return Int32.MaxValue;
            }
            // We're going to fake floating-point math in integers. We just need to use more bits.
            // Scale up patternLength so that intermediate values below like scaledCounter will have
            // more "significant digits".
            int unitBarWidth = (total << INTEGER_MATH_SHIFT) / patternLength;
            maxIndividualVariance = (maxIndividualVariance * unitBarWidth) >> 8;

            int totalVariance = 0;
            for (int x = 0; x < numCounters; x++)
            {
                int counter = counters[x] << INTEGER_MATH_SHIFT;
                int scaledPattern = pattern[x] * unitBarWidth;
                int variance = (counter > scaledPattern) ? counter - scaledPattern : scaledPattern - counter;
                if (variance > maxIndividualVariance)
                {
                    return Int32.MaxValue;
                }
                totalVariance += variance;
            }
            return totalVariance / total;
        }

    }
}
