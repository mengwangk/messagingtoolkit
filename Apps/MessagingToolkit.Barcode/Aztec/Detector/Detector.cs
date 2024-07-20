using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Aztec;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.Detector;
using MessagingToolkit.Barcode.Common.ReedSolomon;

namespace MessagingToolkit.Barcode.Aztec.Detector
{
    /// <summary>
    /// Encapsulates logic that can detect an Aztec Code in an image, even if the Aztec Code
    /// is rotated or skewed, or partially obscured.
    /// 
    /// Modified: May 25 2012
    /// </summary>
    public sealed class Detector
    {
        private readonly BitMatrix image;

        private bool compact;
        private int nbLayers;
        private int nbDataBlocks;
        private int nbCenterLayers;
        private int shift;

        public Detector(BitMatrix image)
        {
            this.image = image;
        }

        /// <summary>
        ///   Detects an Aztec Code in an image.
        /// </summary>
        /// <returns>
        /// encapsulating results of detecting an Aztec Code
        /// </returns>
        /// <exception cref="NotFoundException">if no Aztec Code can be found</exception>
        public AztecDetectorResult Detect()
        {

            // 1. Get the center of the aztec matrix
            Detector.Point pCenter = GetMatrixCenter();

            // 2. Get the center points of the four diagonal points just outside the bull's eye
            //  [topRight, bottomRight, bottomLeft, topLeft]
            ResultPoint[] bullsEyeCorners = GetBullsEyeCorners(pCenter);

            // 3. Get the size of the matrix and other parameters from the bull's eye
            ExtractParameters(bullsEyeCorners);

            // 4. Sample the grid
            BitMatrix bits = SampleGrid(image,
                      bullsEyeCorners[shift % 4], bullsEyeCorners[(shift + 1) % 4],
                      bullsEyeCorners[(shift + 2) % 4], bullsEyeCorners[(shift + 3) % 4]);

            // 5. Get the corners of the matrix.
            ResultPoint[] corners = GetMatrixCornerPoints(bullsEyeCorners);

            return new AztecDetectorResult(bits, corners, compact, nbDataBlocks, nbLayers);
        }

        /// <summary>
        /// Extracts the number of data layers and data blocks from the layer around the bull's eye 
        /// </summary>
        ///
        /// <param name="bullsEyeCorners">the array of bull's eye corners</param>
        /// <exception cref="NotFoundException">in case of too many errors or invalid parameters</exception>
        private void ExtractParameters(ResultPoint[] bullsEyeCorners)
        {
            if (!IsValid(bullsEyeCorners[0]) || !IsValid(bullsEyeCorners[1]) ||
                !IsValid(bullsEyeCorners[2]) || !IsValid(bullsEyeCorners[3]))
            {
                throw NotFoundException.Instance;
            }

            int twoCenterLayers = 2 * nbCenterLayers;

            // Get the bits around the bull's eye
            bool[] resab = SampleLine(bullsEyeCorners[0], bullsEyeCorners[1], twoCenterLayers + 1);
            bool[] resbc = SampleLine(bullsEyeCorners[1], bullsEyeCorners[2], twoCenterLayers + 1);
            bool[] rescd = SampleLine(bullsEyeCorners[2], bullsEyeCorners[3], twoCenterLayers + 1);
            bool[] resda = SampleLine(bullsEyeCorners[3], bullsEyeCorners[0], twoCenterLayers + 1);

            // Determine the orientation of the matrix
            if (resab[0] && resab[twoCenterLayers])
            {
                shift = 0;
            }
            else if (resbc[0] && resbc[twoCenterLayers])
            {
                shift = 1;
            }
            else if (rescd[0] && rescd[twoCenterLayers])
            {
                shift = 2;
            }
            else if (resda[0] && resda[twoCenterLayers])
            {
                shift = 3;
            }
            else
            {
                throw NotFoundException.Instance;
            }

            //d      a
            //
            //c      b

            // Flatten the bits in a single array
            bool[] parameterData;
            bool[] shiftedParameterData;
            if (compact)
            {
                shiftedParameterData = new bool[28];
                for (int i = 0; i < 7; i++)
                {
                    shiftedParameterData[i] = resab[2 + i];
                    shiftedParameterData[i + 7] = resbc[2 + i];
                    shiftedParameterData[i + 14] = rescd[2 + i];
                    shiftedParameterData[i + 21] = resda[2 + i];
                }

                parameterData = new bool[28];
                for (int i_0 = 0; i_0 < 28; i_0++)
                {
                    parameterData[i_0] = shiftedParameterData[(i_0 + shift * 7) % 28];
                }
            }
            else
            {
                shiftedParameterData = new bool[40];
                for (int i_1 = 0; i_1 < 11; i_1++)
                {
                    if (i_1 < 5)
                    {
                        shiftedParameterData[i_1] = resab[2 + i_1];
                        shiftedParameterData[i_1 + 10] = resbc[2 + i_1];
                        shiftedParameterData[i_1 + 20] = rescd[2 + i_1];
                        shiftedParameterData[i_1 + 30] = resda[2 + i_1];
                    }
                    if (i_1 > 5)
                    {
                        shiftedParameterData[i_1 - 1] = resab[2 + i_1];
                        shiftedParameterData[i_1 + 9] = resbc[2 + i_1];
                        shiftedParameterData[i_1 + 19] = rescd[2 + i_1];
                        shiftedParameterData[i_1 + 29] = resda[2 + i_1];
                    }
                }

                parameterData = new bool[40];
                for (int i_2 = 0; i_2 < 40; i_2++)
                {
                    parameterData[i_2] = shiftedParameterData[(i_2 + shift * 10) % 40];
                }
            }

            // corrects the error using RS algorithm
            CorrectParameterData(parameterData, compact);

            // gets the parameters from the bit array
            GetParameters(parameterData);
        }

        /// <summary>
        /// Gets the Aztec code corners from the bull's eye corners and the parameters
        /// </summary>
        ///
        /// <param name="bullsEyeCorners">the array of bull's eye corners</param>
        /// <returns>the array of aztec code corners</returns>
        /// <exception cref="NotFoundException">if the corner points do not fit in the image</exception>
        private ResultPoint[] GetMatrixCornerPoints(ResultPoint[] bullsEyeCorners)
        {
            return ExpandSquare(bullsEyeCorners, 2 * nbCenterLayers, GetDimension());
        }

        /// <summary>
        /// Corrects the parameter bits using Reed-Solomon algorithm
        /// </summary>
        /// <param name="parameterData">parameter bits</param>
        /// <param name="compact_0">true if this is a compact Aztec code</param>
        /// <exception cref="NotFoundException">if the array contains too many errors</exception>
        private static void CorrectParameterData(bool[] parameterData, bool compact_0)
        {

            int numCodewords;
            int numDataCodewords;

            if (compact_0)
            {
                numCodewords = 7;
                numDataCodewords = 2;
            }
            else
            {
                numCodewords = 10;
                numDataCodewords = 4;
            }

            int numECCodewords = numCodewords - numDataCodewords;
            int[] parameterWords = new int[numCodewords];

            int codewordSize = 4;
            for (int i = 0; i < numCodewords; i++)
            {
                int flag = 1;
                for (int j = 1; j <= codewordSize; j++)
                {
                    if (parameterData[codewordSize * i + codewordSize - j])
                    {
                        parameterWords[i] += flag;
                    }
                    flag <<= 1;
                }
            }

            try
            {
                ReedSolomonDecoder rsDecoder = new ReedSolomonDecoder(MessagingToolkit.Barcode.Common.ReedSolomon.GenericGF.AztecParam);
                rsDecoder.Decode(parameterWords, numECCodewords);
            }
            catch (ReedSolomonException ignored)
            {
                throw NotFoundException.Instance;
            }

            for (int i = 0; i < numDataCodewords; i++)
            {
                int flag = 1;
                for (int j = 1; j <= codewordSize; j++)
                {
                    parameterData[i * codewordSize + codewordSize - j] = (parameterWords[i] & flag) == flag;
                    flag <<= 1;
                }
            }
        }

        /// <summary>
        /// Finds the corners of a bull-eye centered on the passed point
        /// This returns the centers of the diagonal points just outside the bull's eye
        /// Returns [topRight, bottomRight, bottomLeft, topLeft]
        /// </summary>
        /// <param name="pCenter">Center point</param>
        /// <returns>The corners of the bull-eye</returns>
        /// <exception cref="NotFoundException">If no valid bull-eye can be found</exception>
        private ResultPoint[] GetBullsEyeCorners(Detector.Point pCenter)
        {

            Detector.Point pina = pCenter;
            Detector.Point pinb = pCenter;
            Detector.Point pinc = pCenter;
            Detector.Point pind = pCenter;

            bool color = true;

            for (nbCenterLayers = 1; nbCenterLayers < 9; nbCenterLayers++)
            {
                Detector.Point pouta = GetFirstDifferent(pina, color, 1, -1);
                Detector.Point poutb = GetFirstDifferent(pinb, color, 1, 1);
                Detector.Point poutc = GetFirstDifferent(pinc, color, -1, 1);
                Detector.Point poutd = GetFirstDifferent(pind, color, -1, -1);

                //d      a
                //
                //c      b

                if (nbCenterLayers > 2)
                {
                    float q = Distance(poutd, pouta) * nbCenterLayers / (Distance(pind, pina) * (nbCenterLayers + 2));
                    if (q < 0.75d || q > 1.25d || !IsWhiteOrBlackRectangle(pouta, poutb, poutc, poutd))
                    {
                        break;
                    }
                }

                pina = pouta;
                pinb = poutb;
                pinc = poutc;
                pind = poutd;

                color = !color;
            }

            if (nbCenterLayers != 5 && nbCenterLayers != 7)
            {
                throw NotFoundException.Instance;
            }

            compact = nbCenterLayers == 5;

            // Expand the square by .5 pixel in each direction so that we're on the border
            // between the white square and the black square
            ResultPoint pinax = new ResultPoint(pina.X + 0.5f, pina.Y - 0.5f);
            ResultPoint pinbx = new ResultPoint(pinb.X + 0.5f, pinb.Y + 0.5f);
            ResultPoint pincx = new ResultPoint(pinc.X - 0.5f, pinc.Y + 0.5f);
            ResultPoint pindx = new ResultPoint(pind.X - 0.5f, pind.Y - 0.5f);

            // Expand the square so that its corners are the centers of the points
            // just outside the bull's eye.
            return ExpandSquare(new ResultPoint[] { pinax, pinbx, pincx, pindx },
                                 2 * nbCenterLayers - 3,
                                 2 * nbCenterLayers);
        }

        /// <summary>
        /// Finds a candidate center point of an Aztec code from an image
        /// </summary>
        /// <returns>
        /// the center point
        /// </returns>
        private Detector.Point GetMatrixCenter()
        {

            ResultPoint pointA;
            ResultPoint pointB;
            ResultPoint pointC;
            ResultPoint pointD;

            //Get a white rectangle that can be the border of the matrix in center bull's eye or
            try
            {

                ResultPoint[] cornerPoints = new WhiteRectangleDetector(image).Detect();
                pointA = cornerPoints[0];
                pointB = cornerPoints[1];
                pointC = cornerPoints[2];
                pointD = cornerPoints[3];

            }
            catch (NotFoundException e)
            {

                // This exception can be in case the initial rectangle is white
                // In that case, surely in the bull's eye, we try to expand the rectangle.
                int cx = image.GetWidth() / 2;
                int cy = image.GetHeight() / 2;
                pointA = GetFirstDifferent(new Detector.Point(cx + 7, cy - 7), false, 1, -1).ToResultPoint();
                pointB = GetFirstDifferent(new Detector.Point(cx + 7, cy + 7), false, 1, 1).ToResultPoint();
                pointC = GetFirstDifferent(new Detector.Point(cx - 7, cy + 7), false, -1, 1).ToResultPoint();
                pointD = GetFirstDifferent(new Detector.Point(cx - 7, cy - 7), false, -1, -1).ToResultPoint();

            }

            //Compute the center of the rectangle
            int cx_0 = MathUtils.Round((pointA.X + pointD.X + pointB.X + pointC.X) / 4.0f);
            int cy_1 = MathUtils.Round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4.0f);

            // Redetermine the white rectangle starting from previously computed center.
            // This will ensure that we end up with a white rectangle in center bull's eye
            // in order to compute a more accurate center.
            try
            {
                ResultPoint[] cornerPoints_2 = new WhiteRectangleDetector(image, 15, cx_0, cy_1).Detect();
                pointA = cornerPoints_2[0];
                pointB = cornerPoints_2[1];
                pointC = cornerPoints_2[2];
                pointD = cornerPoints_2[3];
            }
            catch (NotFoundException e_3)
            {

                // This exception can be in case the initial rectangle is white
                // In that case we try to expand the rectangle.
                pointA = GetFirstDifferent(new Detector.Point(cx_0 + 7, cy_1 - 7), false, 1, -1).ToResultPoint();
                pointB = GetFirstDifferent(new Detector.Point(cx_0 + 7, cy_1 + 7), false, 1, 1).ToResultPoint();
                pointC = GetFirstDifferent(new Detector.Point(cx_0 - 7, cy_1 + 7), false, -1, 1).ToResultPoint();
                pointD = GetFirstDifferent(new Detector.Point(cx_0 - 7, cy_1 - 7), false, -1, -1).ToResultPoint();

            }

            // Recompute the center of the rectangle
            cx_0 = MathUtils.Round((pointA.X + pointD.X + pointB.X + pointC.X) / 4.0f);
            cy_1 = MathUtils.Round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4.0f);

            return new Detector.Point(cx_0, cy_1);
        }

        /// <summary>
        /// Creates a BitMatrix by sampling the provided image.
        /// topLeft, topRight, bottomRight, and bottomLeft are the centers of the squares on the
        /// diagonal just outside the bull's eye.
        /// </summary>
        /// <param name="image_0">The image_0.</param>
        /// <param name="topLeft">The top left.</param>
        /// <param name="topRight">The bottom left.</param>
        /// <param name="bottomRight">The bottom right.</param>
        /// <param name="topRight">The top right.</param>
        /// <returns></returns>
        private BitMatrix SampleGrid(BitMatrix image_0, ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomRight, ResultPoint bottomLeft)
        {

            GridSampler sampler = GridSampler.GetInstance();
            int dimension = GetDimension();

            float low = dimension / 2.0f - nbCenterLayers;
            float high = dimension / 2.0f + nbCenterLayers;

            return sampler.SampleGrid(image,
                dimension,
                dimension,
                low, low,   // topleft
                high, low,  // topright
                 high, high, // bottomright
                 low, high,  // bottomleft
                 topLeft.X, topLeft.Y,
                 topRight.X, topRight.Y,
                 bottomRight.X, bottomRight.Y,
                 bottomLeft.X, bottomLeft.Y);
        }

        /// <summary>
        /// Sets number of layers and number of data blocks from parameter bits
        /// </summary>
        /// <param name="parameterData">The parameter data.</param>
        private void GetParameters(bool[] parameterData)
        {

            int nbBitsForNbLayers;
            int nbBitsForNbDatablocks;

            if (compact)
            {
                nbBitsForNbLayers = 2;
                nbBitsForNbDatablocks = 6;
            }
            else
            {
                nbBitsForNbLayers = 5;
                nbBitsForNbDatablocks = 11;
            }

            for (int i = 0; i < nbBitsForNbLayers; i++)
            {
                nbLayers <<= 1;
                if (parameterData[i])
                {
                    nbLayers++;
                }
            }

            for (int i_0 = nbBitsForNbLayers; i_0 < nbBitsForNbLayers + nbBitsForNbDatablocks; i_0++)
            {
                nbDataBlocks <<= 1;
                if (parameterData[i_0])
                {
                    nbDataBlocks++;
                }
            }

            nbLayers++;
            nbDataBlocks++;

        }

        /// <summary>
        /// Samples a line
        /// </summary>
        ///
        /// <param name="p1">first point</param>
        /// <param name="p2">second point</param>
        /// <param name="size">number of bits</param>
        /// <returns>the array of bits</returns>
        private bool[] SampleLine(ResultPoint p1, ResultPoint p2, int size)
        {

            bool[] res = new bool[size];
            float d = Distance(p1, p2);
            float moduleSize = d / (size - 1);
            float dx = moduleSize * (p2.X - p1.X) / d;
            float dy = moduleSize * (p2.Y - p1.Y) / d;

            float px = p1.X;
            float py = p1.Y;

            for (int i = 0; i < size; i++)
            {
                res[i] = image.Get(MathUtils.Round(px), MathUtils.Round(py));
                px += dx;
                py += dy;
            }

            return res;
        }


        /// <returns>
        /// true if the border of the rectangle passed in parameter is compound of white points only
        /// or black points only</returns>
        private bool IsWhiteOrBlackRectangle(Detector.Point p1, Detector.Point p2, Detector.Point p3, Detector.Point p4)
        {

            int corr = 3;

            p1 = new Detector.Point(p1.X - corr, p1.Y + corr);
            p2 = new Detector.Point(p2.X - corr, p2.Y - corr);
            p3 = new Detector.Point(p3.X + corr, p3.Y - corr);
            p4 = new Detector.Point(p4.X + corr, p4.Y + corr);

            int cInit = GetColor(p4, p1);

            if (cInit == 0)
            {
                return false;
            }

            int c = GetColor(p1, p2);

            if (c != cInit)
            {
                return false;
            }

            c = GetColor(p2, p3);

            if (c != cInit)
            {
                return false;
            }

            c = GetColor(p3, p4);

            return c == cInit;

        }

        /// <summary>
        /// Gets the color of a segment
        /// </summary>
        /// <returns>1 if segment more than 90% black, -1 if segment is more than 90% white, 0 else</returns>
        private int GetColor(Detector.Point p1, Detector.Point p2)
        {
            float d = Distance(p1, p2);
            float dx = (p2.X - p1.X) / d;
            float dy = (p2.Y - p1.Y) / d;
            int error = 0;

            float px = p1.X;
            float py = p1.Y;

            bool colorModel = image.Get(p1.X, p1.Y);

            for (int i = 0; i < d; i++)
            {
                px += dx;
                py += dy;
                if (image.Get(MathUtils.Round(px), MathUtils.Round(py)) != colorModel)
                {
                    error++;
                }
            }

            float errRatio = error / d;

            if (errRatio > 0.1f && errRatio < 0.9f)
            {
                return 0;
            }

            return (errRatio <= 0.1f) == colorModel ? 1 : -1;
        }

        /// <summary>
        /// Gets the coordinate of the first point with a different color in the given direction
        /// </summary>
        private Detector.Point GetFirstDifferent(Detector.Point init, bool color, int dx, int dy)
        {
            int x = init.X + dx;
            int y = init.Y + dy;

            while (IsValid(x, y) && image.Get(x, y) == color)
            {
                x += dx;
                y += dy;
            }

            x -= dx;
            y -= dy;

            while (IsValid(x, y) && image.Get(x, y) == color)
            {
                x += dx;
            }
            x -= dx;

            while (IsValid(x, y) && image.Get(x, y) == color)
            {
                y += dy;
            }
            y -= dy;

            return new Detector.Point(x, y);
        }

        /// <summary>
        /// Expand the square represented by the corner points by pushing out equally in all directions
        /// </summary>
        /// <param name="cornerPoints">the corners of the square, which has the bull's eye at its center</param>
        /// <param name="oldSide">the original length of the side of the square in the target bit matrix</param>
        /// <param name="newSide">the new length of the size of the square in the target bit matrix</param>
        /// <returns>the corners of the expanded square</returns>
        private ResultPoint[] ExpandSquare(ResultPoint[] cornerPoints, float oldSide, float newSide)
        {
            float ratio = newSide / (2 * oldSide);
            float dx = cornerPoints[0].X - cornerPoints[2].X;
            float dy = cornerPoints[0].Y - cornerPoints[2].Y;
            float centerx = (cornerPoints[0].X + cornerPoints[2].X) / 2.0f;
            float centery = (cornerPoints[0].Y + cornerPoints[2].Y) / 2.0f;

            ResultPoint result0 = new ResultPoint(centerx + ratio * dx, centery + ratio * dy);
            ResultPoint result2 = new ResultPoint(centerx - ratio * dx, centery - ratio * dy);

            dx = cornerPoints[1].X - cornerPoints[3].X;
            dy = cornerPoints[1].Y - cornerPoints[3].Y;
            centerx = (cornerPoints[1].X + cornerPoints[3].X) / 2.0f;
            centery = (cornerPoints[1].Y + cornerPoints[3].Y) / 2.0f;
            ResultPoint result1 = new ResultPoint(centerx + ratio * dx, centery + ratio * dy);
            ResultPoint result3 = new ResultPoint(centerx - ratio * dx, centery - ratio * dy);

            return new ResultPoint[] { result0, result1, result2, result3 };
        }


        private bool IsValid(int x, int y)
        {
            return x >= 0 && x < image.Width && y > 0 && y < image.Height;
        }

        private bool IsValid(ResultPoint point)
        {
            int x = MathUtils.Round(point.X);
            int y = MathUtils.Round(point.Y);
            return IsValid(x, y);
        }

        private static float Distance(Point a, Point b)
        {
            return MathUtils.Distance(a.X, a.Y, b.X, b.Y);
        }

        private static float Distance(ResultPoint a, ResultPoint b)
        {
            return MathUtils.Distance(a.X, a.Y, b.X, b.Y);
        }

        private int GetDimension()
        {
            if (compact)
            {
                return 4 * nbLayers + 11;
            }
            if (nbLayers <= 4)
            {
                return 4 * nbLayers + 15;
            }
            return 4 * nbLayers + 2 * ((nbLayers - 4) / 8 + 1) + 15;
        }

        internal sealed class Point
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public ResultPoint ToResultPoint()
            {
                return new ResultPoint(X, Y);
            }

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public override string ToString()
            {
                return "<" + X + ' ' + Y + '>';
            }
        }

    }
}
