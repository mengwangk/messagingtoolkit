using System;
using System.Collections.Generic;

using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;
using Collections = MessagingToolkit.Barcode.Common.Collections;
using Comparator = MessagingToolkit.Barcode.Common.Comparator;
using DetectorResult = MessagingToolkit.Barcode.Common.DetectorResult;
using GridSampler = MessagingToolkit.Barcode.Common.GridSampler;
using WhiteRectangleDetector = MessagingToolkit.Barcode.Common.Detector.WhiteRectangleDetector;
using MathUtils = MessagingToolkit.Barcode.Common.Detector.MathUtils;

namespace MessagingToolkit.Barcode.DataMatrix.detector
{
    /// <summary>
    /// <p>Encapsulates logic that can detect a Data Matrix Code in an image, even if the Data Matrix Code
    /// is rotated or skewed, or partially obscured.</p>
    /// 
    /// Modified: May 10 2012
    /// </summary>
    internal sealed class Detector
    {

        private readonly BitMatrix image;
        private readonly WhiteRectangleDetector rectangleDetector;

        public Detector(BitMatrix image)
        {
            this.image = image;
            rectangleDetector = new WhiteRectangleDetector(image);
        }

        /// <summary>
        /// <p>Detects a Data Matrix Code in an image.</p>
        /// </summary>
        ///
        /// <returns>/// <see cref="null"/>
        ///  encapsulating results of detecting a Data Matrix Code</returns>
        /// <exception cref="NotFoundException">if no Data Matrix Code can be found</exception>
        public DetectorResult Detect()
        {

            ResultPoint[] cornerPoints = rectangleDetector.Detect();
            ResultPoint pointA = cornerPoints[0];
            ResultPoint pointB = cornerPoints[1];
            ResultPoint pointC = cornerPoints[2];
            ResultPoint pointD = cornerPoints[3];

            // Point A and D are across the diagonal from one another,
            // as are B and C. Figure out which are the solid black lines
            // by counting transitions
            List<Detector.ResultPointsAndTransitions> transitions = new List<Detector.ResultPointsAndTransitions>(4);
            transitions.Add(TransitionsBetween(pointA, pointB));
            transitions.Add(TransitionsBetween(pointA, pointC));
            transitions.Add(TransitionsBetween(pointB, pointD));
            transitions.Add(TransitionsBetween(pointC, pointD));
            Collections.InsertionSort(transitions, new Detector.ResultPointsAndTransitionsComparator());

            // Sort by number of transitions. First two will be the two solid sides; last two
            // will be the two alternating black/white sides
            Detector.ResultPointsAndTransitions lSideOne = transitions[0];
            Detector.ResultPointsAndTransitions lSideTwo = transitions[1];

            // Figure out which point is their intersection by tallying up the number of times we see the
            // endpoints in the four endpoints. One will show up twice.
            Dictionary<ResultPoint, int> pointCount = new Dictionary<ResultPoint, int>();
            Increment(pointCount, lSideOne.GetFrom());
            Increment(pointCount, lSideOne.GetTo());
            Increment(pointCount, lSideTwo.GetFrom());
            Increment(pointCount, lSideTwo.GetTo());

            ResultPoint maybeTopLeft = null;
            ResultPoint bottomLeft = null;
            ResultPoint maybeBottomRight = null;
            IEnumerator<ResultPoint> points = pointCount.Keys.GetEnumerator();
            while (points.MoveNext())
            {
                ResultPoint point = (ResultPoint)points.Current;
                Int32 val = (Int32)pointCount[point];
                if (val == 2)
                {
                    bottomLeft = point; // this is definitely the bottom left, then -- end of two L sides
                }
                else
                {
                    // Otherwise it's either top left or bottom right -- just assign the two arbitrarily now
                    if (maybeTopLeft == null)
                    {
                        maybeTopLeft = point;
                    }
                    else
                    {
                        maybeBottomRight = point;
                    }
                }
            }

            if (maybeTopLeft == null || bottomLeft == null || maybeBottomRight == null)
            {
                throw NotFoundException.Instance;
            }

            // Bottom left is correct but top left and bottom right might be switched
            ResultPoint[] corners = { maybeTopLeft, bottomLeft, maybeBottomRight };
            // Use the dot product trick to sort them out
            ResultPoint.OrderBestPatterns(corners);

            // Now we know which is which:
            ResultPoint bottomRight = corners[0];
            bottomLeft = corners[1];
            ResultPoint topLeft = corners[2];

            // Which point didn't we find in relation to the "L" sides? that's the top right corner
            ResultPoint topRight;
            if (!pointCount.ContainsKey(pointA))
            {
                topRight = pointA;
            }
            else if (!pointCount.ContainsKey(pointB))
            {
                topRight = pointB;
            }
            else if (!pointCount.ContainsKey(pointC))
            {
                topRight = pointC;
            }
            else
            {
                topRight = pointD;
            }

            // Next determine the dimension by tracing along the top or right side and counting black/white
            // transitions. Since we start inside a black module, we should see a number of transitions
            // equal to 1 less than the code dimension. Well, actually 2 less, because we are going to
            // end on a black module:

            // The top right point is actually the corner of a module, which is one of the two black modules
            // adjacent to the white module at the top right. Tracing to that corner from either the top left
            // or bottom right should work here.

            int dimensionTop = TransitionsBetween(topLeft, topRight).GetTransitions();
            int dimensionRight = TransitionsBetween(bottomRight, topRight).GetTransitions();

            if ((dimensionTop & 0x01) == 1)
            {
                // it can't be odd, so, round... up?
                dimensionTop++;
            }
            dimensionTop += 2;

            if ((dimensionRight & 0x01) == 1)
            {
                // it can't be odd, so, round... up?
                dimensionRight++;
            }
            dimensionRight += 2;

            BitMatrix bits;
            ResultPoint correctedTopRight;

            // Rectanguar symbols are 6x16, 6x28, 10x24, 10x32, 14x32, or 14x44. If one dimension is more
            // than twice the other, it's certainly rectangular, but to cut a bit more slack we accept it as
            // rectangular if the bigger side is at least 7/4 times the other:
            if (4 * dimensionTop >= 7 * dimensionRight || 4 * dimensionRight >= 7 * dimensionTop)
            {
                // The matrix is rectangular

                correctedTopRight = CorrectTopRightRectangular(bottomLeft, bottomRight, topLeft, topRight, dimensionTop, dimensionRight);
                if (correctedTopRight == null)
                {
                    correctedTopRight = topRight;
                }

                dimensionTop = TransitionsBetween(topLeft, correctedTopRight).GetTransitions();
                dimensionRight = TransitionsBetween(bottomRight, correctedTopRight).GetTransitions();

                if ((dimensionTop & 0x01) == 1)
                {
                    // it can't be odd, so, round... up?
                    dimensionTop++;
                }

                if ((dimensionRight & 0x01) == 1)
                {
                    // it can't be odd, so, round... up?
                    dimensionRight++;
                }

                bits = SampleGrid(image, topLeft, bottomLeft, bottomRight, correctedTopRight, dimensionTop, dimensionRight);

            }
            else
            {
                // The matrix is square

                int dimension = Math.Min(dimensionRight, dimensionTop);
                // correct top right point to match the white module
                correctedTopRight = CorrectTopRight(bottomLeft, bottomRight, topLeft, topRight, dimension);
                if (correctedTopRight == null)
                {
                    correctedTopRight = topRight;
                }

                // Redetermine the dimension using the corrected top right point
                int dimensionCorrected = Math.Max(TransitionsBetween(topLeft, correctedTopRight).GetTransitions(), TransitionsBetween(bottomRight, correctedTopRight).GetTransitions());
                dimensionCorrected++;
                if ((dimensionCorrected & 0x01) == 1)
                {
                    dimensionCorrected++;
                }

                bits = SampleGrid(image, topLeft, bottomLeft, bottomRight, correctedTopRight, dimensionCorrected, dimensionCorrected);
            }

            return new DetectorResult(bits, new ResultPoint[] { topLeft, bottomLeft, bottomRight, correctedTopRight });
        }

        /// <summary>
        /// Calculates the position of the white top right module using the output of the rectangle detector
        /// for a rectangular matrix
        /// </summary>
        ///
        private ResultPoint CorrectTopRightRectangular(ResultPoint bottomLeft, ResultPoint bottomRight, ResultPoint topLeft, ResultPoint topRight, int dimensionTop, int dimensionRight)
        {

            float corr = Distance(bottomLeft, bottomRight) / (float)dimensionTop;
            int norm = Distance(topLeft, topRight);
            if (norm == 0)
                 return null;
            float cos = (topRight.X - topLeft.X) / norm;
            float sin = (topRight.Y - topLeft.Y) / norm;

            ResultPoint c1 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

            corr = Distance(bottomLeft, topLeft) / (float)dimensionRight;
            norm = Distance(bottomRight, topRight);
            if (norm == 0)
	             return null;
            cos = (topRight.X - bottomRight.X) / norm;
            sin = (topRight.Y - bottomRight.Y) / norm;

            ResultPoint c2 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

            if (!IsValid(c1))
            {
                if (IsValid(c2))
                {
                    return c2;
                }
                return null;
            }
            if (!IsValid(c2))
            {
                return c1;
            }

            int l1 = Math.Abs(dimensionTop - TransitionsBetween(topLeft, c1).GetTransitions()) + Math.Abs(dimensionRight - TransitionsBetween(bottomRight, c1).GetTransitions());
            int l2 = Math.Abs(dimensionTop - TransitionsBetween(topLeft, c2).GetTransitions()) + Math.Abs(dimensionRight - TransitionsBetween(bottomRight, c2).GetTransitions());

            if (l1 <= l2)
            {
                return c1;
            }

            return c2;
        }

        /// <summary>
        /// Calculates the position of the white top right module using the output of the rectangle detector
        /// for a square matrix
        /// </summary>
        ///
        private ResultPoint CorrectTopRight(ResultPoint bottomLeft, ResultPoint bottomRight, ResultPoint topLeft, ResultPoint topRight, int dimension)
        {

            float corr = Distance(bottomLeft, bottomRight) / (float)dimension;
            int norm = Distance(topLeft, topRight);
            if (norm == 0)
                return null;
            float cos = (topRight.X - topLeft.X) / norm;
            float sin = (topRight.Y - topLeft.Y) / norm;

            ResultPoint c1 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

            corr = Distance(bottomLeft, bottomRight) / (float)dimension;
            norm = Distance(bottomRight, topRight);
            if (norm == 0)
                return null;
            cos = (topRight.X - bottomRight.X) / norm;
            sin = (topRight.Y - bottomRight.Y) / norm;

            ResultPoint c2 = new ResultPoint(topRight.X + corr * cos, topRight.Y + corr * sin);

            if (!IsValid(c1))
            {
                if (IsValid(c2))
                {
                    return c2;
                }
                return null;
            }
            if (!IsValid(c2))
            {
                return c1;
            }

            int l1 = Math.Abs(TransitionsBetween(topLeft, c1).GetTransitions() - TransitionsBetween(bottomRight, c1).GetTransitions());
            int l2 = Math.Abs(TransitionsBetween(topLeft, c2).GetTransitions() - TransitionsBetween(bottomRight, c2).GetTransitions());

            return (l1 <= l2) ? c1 : c2;
        }

        private bool IsValid(ResultPoint p)
        {
            return p.X >= 0 && p.X < image.Width && p.Y > 0 && p.Y < image.Height;
        }

        // L2 distance
        private static int Distance(ResultPoint a, ResultPoint b)
        {
            return MathUtils.Round(ResultPoint.Distance(a, b));
        }

        /// <summary>
        /// Increments the Integer associated with a key by one.
        /// </summary>
        ///
        private static void Increment(Dictionary<ResultPoint, int> table, ResultPoint key)
        {
            if (table.ContainsKey(key))
            {
                int value = table[key];
                table[key] = value + 1;
            }
            else
            {
                table[key] = 1;
            }
        }

        private static BitMatrix SampleGrid(BitMatrix image_0, ResultPoint topLeft, ResultPoint bottomLeft, ResultPoint bottomRight, ResultPoint topRight, int dimensionX, int dimensionY)
        {

            GridSampler sampler = GridSampler.GetInstance();

            return sampler.SampleGrid(image_0, dimensionX, dimensionY, 0.5f, 0.5f, dimensionX - 0.5f, 0.5f, dimensionX - 0.5f, dimensionY - 0.5f, 0.5f, dimensionY - 0.5f, topLeft.X, topLeft.Y,
                    topRight.X, topRight.Y, bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y);
        }

        /// <summary>
        /// Counts the number of black/white transitions between two points, using something like Bresenham's algorithm.
        /// </summary>
        ///
        private Detector.ResultPointsAndTransitions TransitionsBetween(ResultPoint from, ResultPoint to)
        {
            // See QR Code Detector, sizeOfBlackWhiteBlackRun()
            int fromX = (int)from.X;
            int fromY = (int)from.Y;
            int toX = (int)to.X;
            int toY = (int)to.Y;
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
            int ystep = (fromY < toY) ? 1 : -1;
            int xstep = (fromX < toX) ? 1 : -1;
            int transitions = 0;
            bool inBlack = image.Get((steep) ? fromY : fromX, (steep) ? fromX : fromY);
            for (int x = fromX, y = fromY; x != toX; x += xstep)
            {
                bool isBlack = image.Get((steep) ? y : x, (steep) ? x : y);
                if (isBlack != inBlack)
                {
                    transitions++;
                    inBlack = isBlack;
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
            return new Detector.ResultPointsAndTransitions(from, to, transitions);
        }

        /// <summary>
        /// Simply encapsulates two points and a number of transitions between them.
        /// </summary>
        ///
        private sealed class ResultPointsAndTransitions
        {

            private readonly ResultPoint from;
            private readonly ResultPoint to;
            private readonly int transitions;

            public ResultPointsAndTransitions(ResultPoint from, ResultPoint to, int transitions)
            {
                this.from = from;
                this.to = to;
                this.transitions = transitions;
            }

            internal ResultPoint GetFrom()
            {
                return from;
            }

            internal ResultPoint GetTo()
            {
                return to;
            }

            public int GetTransitions()
            {
                return transitions;
            }

            public override String ToString()
            {
                return from + "/" + to + '/' + transitions;
            }
        }

        /// <summary>
        /// Orders ResultPointsAndTransitions by number of transitions, ascending.
        /// </summary>
#if !SILVERLIGHT && !NETFX_CORE
	[Serializable]
#endif
        private class ResultPointsAndTransitionsComparator : Comparator
        {
            public virtual int Compare(Object o1, Object o2)
            {
                return ((Detector.ResultPointsAndTransitions)o1).GetTransitions()
                        - ((Detector.ResultPointsAndTransitions)o2).GetTransitions();
            }
        }

    }
}
