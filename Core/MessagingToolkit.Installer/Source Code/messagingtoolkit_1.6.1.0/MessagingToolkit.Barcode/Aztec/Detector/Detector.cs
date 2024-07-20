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
    /// <p>Encapsulates logic that can detect an Aztec Code in an image, even if the Aztec Code
    /// is rotated or skewed, or partially obscured.</p>
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
        ///   <p>Detects an Aztec Code in an image.</p>
        /// </summary>
        /// <returns>
        /// encapsulating results of detecting an Aztec Code
        /// </returns>
        /// <exception cref="NotFoundException">if no Aztec Code can be found</exception>
        public AztecDetectorResult Detect()
        {

            // 1. Get the center of the aztec matrix
            Detector.Point pCenter = GetMatrixCenter();

            // 2. Get the corners of the center bull's eye
            Detector.Point[] bullEyeCornerPoints = GetBullEyeCornerPoints(pCenter);

            // 3. Get the size of the matrix from the bull's eye
            ExtractParameters(bullEyeCornerPoints);

            // 4. Get the corners of the matrix
            ResultPoint[] corners = GetMatrixCornerPoints(bullEyeCornerPoints);

            // 5. Sample the grid
            BitMatrix bits = SampleGrid(image, corners[shift % 4], corners[(shift + 3) % 4], corners[(shift + 2) % 4], corners[(shift + 1) % 4]);

            return new AztecDetectorResult(bits, corners, compact, nbDataBlocks, nbLayers);
        }

        /// <summary>
        /// <p> Extracts the number of data layers and data blocks from the layer around the bull's eye </p>
        /// </summary>
        ///
        /// <param name="bullEyeCornerPoints">the array of bull's eye corners</param>
        /// <exception cref="NotFoundException">in case of too many errors or invalid parameters</exception>
        private void ExtractParameters(Detector.Point[] bullEyeCornerPoints)
        {

            // Get the bits around the bull's eye
            bool[] resab = SampleLine(bullEyeCornerPoints[0], bullEyeCornerPoints[1], 2 * nbCenterLayers + 1);
            bool[] resbc = SampleLine(bullEyeCornerPoints[1], bullEyeCornerPoints[2], 2 * nbCenterLayers + 1);
            bool[] rescd = SampleLine(bullEyeCornerPoints[2], bullEyeCornerPoints[3], 2 * nbCenterLayers + 1);
            bool[] resda = SampleLine(bullEyeCornerPoints[3], bullEyeCornerPoints[0], 2 * nbCenterLayers + 1);

            // Determine the orientation of the matrix
            if (resab[0] && resab[2 * nbCenterLayers])
            {
                shift = 0;
            }
            else if (resbc[0] && resbc[2 * nbCenterLayers])
            {
                shift = 1;
            }
            else if (rescd[0] && rescd[2 * nbCenterLayers])
            {
                shift = 2;
            }
            else if (resda[0] && resda[2 * nbCenterLayers])
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
                        shiftedParameterData[i_1 + 10 - 1] = resbc[2 + i_1];
                        shiftedParameterData[i_1 + 20 - 1] = rescd[2 + i_1];
                        shiftedParameterData[i_1 + 30 - 1] = resda[2 + i_1];
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
        /// <p>Gets the Aztec code corners from the bull's eye corners and the parameters </p>
        /// </summary>
        ///
        /// <param name="bullEyeCornerPoints">the array of bull's eye corners</param>
        /// <returns>the array of aztec code corners</returns>
        /// <exception cref="NotFoundException">if the corner points do not fit in the image</exception>
        private ResultPoint[] GetMatrixCornerPoints(Detector.Point[] bullEyeCornerPoints)
        {

            float ratio = (2 * nbLayers + ((nbLayers > 4) ? 1 : 0) + (nbLayers - 4) / 8) / (2.0f * nbCenterLayers);

            int dx = bullEyeCornerPoints[0].x - bullEyeCornerPoints[2].x;
            dx += (dx > 0) ? 1 : -1;
            int dy = bullEyeCornerPoints[0].y - bullEyeCornerPoints[2].y;
            dy += (dy > 0) ? 1 : -1;

            int targetcx = MathUtils.Round(bullEyeCornerPoints[2].x - ratio * dx);
            int targetcy = MathUtils.Round(bullEyeCornerPoints[2].y - ratio * dy);

            int targetax = MathUtils.Round(bullEyeCornerPoints[0].x + ratio * dx);
            int targetay = MathUtils.Round(bullEyeCornerPoints[0].y + ratio * dy);

            dx = bullEyeCornerPoints[1].x - bullEyeCornerPoints[3].x;
            dx += (dx > 0) ? 1 : -1;
            dy = bullEyeCornerPoints[1].y - bullEyeCornerPoints[3].y;
            dy += (dy > 0) ? 1 : -1;

            int targetdx = MathUtils.Round(bullEyeCornerPoints[3].x - ratio * dx);
            int targetdy = MathUtils.Round(bullEyeCornerPoints[3].y - ratio * dy);
            int targetbx = MathUtils.Round(bullEyeCornerPoints[1].x + ratio * dx);
            int targetby = MathUtils.Round(bullEyeCornerPoints[1].y + ratio * dy);

            if (!IsValid(targetax, targetay) || !IsValid(targetbx, targetby) || !IsValid(targetcx, targetcy) || !IsValid(targetdx, targetdy))
            {
                throw NotFoundException.Instance;
            }

            return new ResultPoint[] { new ResultPoint(targetax, targetay), new ResultPoint(targetbx, targetby), new ResultPoint(targetcx, targetcy), new ResultPoint(targetdx, targetdy) };
        }

        /// <summary>
        /// <p> Corrects the parameter bits using Reed-Solomon algorithm </p>
        /// </summary>
        ///
        /// <param name="parameterData">paremeter bits</param>
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
            catch (ReedSolomonException rse)
            {
                throw NotFoundException.Instance;
            }

            for (int i_1 = 0; i_1 < numDataCodewords; i_1++)
            {
                int flag_2 = 1;
                for (int j_3 = 1; j_3 <= codewordSize; j_3++)
                {
                    parameterData[i_1 * codewordSize + codewordSize - j_3] = (parameterWords[i_1] & flag_2) == flag_2;
                    flag_2 <<= 1;
                }
            }
        }

        /// <summary>
        /// <p> Finds the corners of a bull-eye centered on the passed point </p>
        /// </summary>
        ///
        /// <param name="pCenter">Center point</param>
        /// <returns>The corners of the bull-eye</returns>
        /// <exception cref="NotFoundException">If no valid bull-eye can be found</exception>
        private Detector.Point[] GetBullEyeCornerPoints(Detector.Point pCenter)
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

            float ratio = 0.75f * 2 / (2 * nbCenterLayers - 3);

            int dx = pina.x - pinc.x;
            int dy = pina.y - pinc.y;
            int targetcx = MathUtils.Round(pinc.x - ratio * dx);
            int targetcy = MathUtils.Round(pinc.y - ratio * dy);
            int targetax = MathUtils.Round(pina.x + ratio * dx);
            int targetay = MathUtils.Round(pina.y + ratio * dy);

            dx = pinb.x - pind.x;
            dy = pinb.y - pind.y;

            int targetdx = MathUtils.Round(pind.x - ratio * dx);
            int targetdy = MathUtils.Round(pind.y - ratio * dy);
            int targetbx = MathUtils.Round(pinb.x + ratio * dx);
            int targetby = MathUtils.Round(pinb.y + ratio * dy);

            if (!IsValid(targetax, targetay) || !IsValid(targetbx, targetby) || !IsValid(targetcx, targetcy) || !IsValid(targetdx, targetdy))
            {
                throw NotFoundException.Instance;
            }

            Detector.Point pa = new Detector.Point(targetax, targetay);
            Detector.Point pb = new Detector.Point(targetbx, targetby);
            Detector.Point pc = new Detector.Point(targetcx, targetcy);
            Detector.Point pd = new Detector.Point(targetdx, targetdy);

            return new Detector.Point[] { pa, pb, pc, pd };
        }

        /// <summary>
        /// Finds a candidate center point of an Aztec code from an image
        /// </summary>
        ///
        /// <returns>the center point</returns>
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
                pointA = GetFirstDifferent(new Detector.Point(cx + 15 / 2, cy - 15 / 2), false, 1, -1).ToResultPoint();
                pointB = GetFirstDifferent(new Detector.Point(cx + 15 / 2, cy + 15 / 2), false, 1, 1).ToResultPoint();
                pointC = GetFirstDifferent(new Detector.Point(cx - 15 / 2, cy + 15 / 2), false, -1, 1).ToResultPoint();
                pointD = GetFirstDifferent(new Detector.Point(cx - 15 / 2, cy - 15 / 2), false, -1, -1).ToResultPoint();

            }

            //Compute the center of the rectangle
            int cx_0 = MathUtils.Round((pointA.X + pointD.X + pointB.X + pointC.X) / 4);
            int cy_1 = MathUtils.Round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4);

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
                pointA = GetFirstDifferent(new Detector.Point(cx_0 + 15 / 2, cy_1 - 15 / 2), false, 1, -1).ToResultPoint();
                pointB = GetFirstDifferent(new Detector.Point(cx_0 + 15 / 2, cy_1 + 15 / 2), false, 1, 1).ToResultPoint();
                pointC = GetFirstDifferent(new Detector.Point(cx_0 - 15 / 2, cy_1 + 15 / 2), false, -1, 1).ToResultPoint();
                pointD = GetFirstDifferent(new Detector.Point(cx_0 - 15 / 2, cy_1 - 15 / 2), false, -1, -1).ToResultPoint();

            }

            // Recompute the center of the rectangle
            cx_0 = MathUtils.Round((pointA.X + pointD.X + pointB.X + pointC.X) / 4);
            cy_1 = MathUtils.Round((pointA.Y + pointD.Y + pointB.Y + pointC.Y) / 4);

            return new Detector.Point(cx_0, cy_1);
        }

        /// <summary>
        /// Samples an Aztec matrix from an image
        /// </summary>
        ///
        private BitMatrix SampleGrid(BitMatrix image_0, ResultPoint topLeft, ResultPoint bottomLeft, ResultPoint bottomRight, ResultPoint topRight)
        {

            int dimension;
            if (compact)
            {
                dimension = 4 * nbLayers + 11;
            }
            else
            {
                if (nbLayers <= 4)
                {
                    dimension = 4 * nbLayers + 15;
                }
                else
                {
                    dimension = 4 * nbLayers + 2 * ((nbLayers - 4) / 8 + 1) + 15;
                }
            }

            GridSampler sampler = GridSampler.GetInstance();

            return sampler.SampleGrid(image_0, dimension, dimension, 0.5f, 0.5f, dimension - 0.5f, 0.5f, dimension - 0.5f, dimension - 0.5f, 0.5f, dimension - 0.5f, topLeft.X, topLeft.Y,
                    topRight.X, topRight.Y, bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y);
        }

        /// <summary>
        /// Sets number of layers and number of datablocks from parameter bits
        /// </summary>
        ///
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
                    nbLayers += 1;
                }
            }

            for (int i_0 = nbBitsForNbLayers; i_0 < nbBitsForNbLayers + nbBitsForNbDatablocks; i_0++)
            {
                nbDataBlocks <<= 1;
                if (parameterData[i_0])
                {
                    nbDataBlocks += 1;
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
        private bool[] SampleLine(Detector.Point p1, Detector.Point p2, int size)
        {

            bool[] res = new bool[size];
            float d = Distance(p1, p2);
            float moduleSize = d / (size - 1);
            float dx = moduleSize * (p2.x - p1.x) / d;
            float dy = moduleSize * (p2.y - p1.y) / d;

            float px = p1.x;
            float py = p1.y;

            for (int i = 0; i < size; i++)
            {
                res[i] = image.Get(MathUtils.Round(px), MathUtils.Round(py));
                px += dx;
                py += dy;
            }

            return res;
        }


        /// <returns>true if the border of the rectangle passed in parameter is compound of white points only
        /// or black points only</returns>
        private bool IsWhiteOrBlackRectangle(Detector.Point p1, Detector.Point p2, Detector.Point p3, Detector.Point p4)
        {

            int corr = 3;

            p1 = new Detector.Point(p1.x - corr, p1.y + corr);
            p2 = new Detector.Point(p2.x - corr, p2.y - corr);
            p3 = new Detector.Point(p3.x + corr, p3.y - corr);
            p4 = new Detector.Point(p4.x + corr, p4.y + corr);

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
        ///
        /// <returns>1 if segment more than 90% black, -1 if segment is more than 90% white, 0 else</returns>
        private int GetColor(Detector.Point p1, Detector.Point p2)
        {
            float d = Distance(p1, p2);
            float dx = (p2.x - p1.x) / d;
            float dy = (p2.y - p1.y) / d;
            int error = 0;

            float px = p1.x;
            float py = p1.y;

            bool colorModel = image.Get(p1.x, p1.y);

            for (int i = 0; i < d; i++)
            {
                px += dx;
                py += dy;
                if (image.Get(MathUtils.Round(px), MathUtils.Round(py)) != colorModel)
                {
                    error++;
                }
            }

            float errRatio = (float)error / d;

            if (errRatio > 0.1d && errRatio < 0.9d)
            {
                return 0;
            }

            if (errRatio <= 0.1d)
            {
                return (colorModel) ? 1 : -1;
            }
            else
            {
                return (colorModel) ? -1 : 1;
            }
        }

        /// <summary>
        /// Gets the coordinate of the first point with a different color in the given direction
        /// </summary>
        ///
        private Detector.Point GetFirstDifferent(Detector.Point init, bool color, int dx, int dy)
        {
            int x = init.x + dx;
            int y = init.y + dy;

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

        private sealed class Point
        {
            public readonly int x;
            public readonly int y;

            public ResultPoint ToResultPoint()
            {
                return new ResultPoint(x, y);
            }

            public Point(int x_0, int y_1)
            {
                this.x = x_0;
                this.y = y_1;
            }
        }

        private bool IsValid(int x_0, int y_1)
        {
            return x_0 >= 0 && x_0 < image.GetWidth() && y_1 > 0 && y_1 < image.GetHeight();
        }

        // L2 distance
        private static float Distance(Detector.Point a, Detector.Point b)
        {
            return MathUtils.Distance(a.x, a.y, b.x, b.y);
        }

    }
}
