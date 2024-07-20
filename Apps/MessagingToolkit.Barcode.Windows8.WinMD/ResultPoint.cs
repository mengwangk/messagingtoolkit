using System;
using System.Text;

using MessagingToolkit.Barcode.Common.Detector;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// Encapsulates a point of interest in an image containing a barcode. Typically, this
    /// would be the location of a finder pattern or the corner of the barcode, for example.
    /// </summary>
    public sealed class ResultPoint
    {
        private float x;
        private float y;
        private readonly byte[] bytesX;
        private readonly byte[] bytesY;

        public float X
        {
            get
            {
                return x;
            }

        }

        public float Y
        {
            get
            {
                return y;
            }

        }


        public ResultPoint(float x, float y)
        {
            this.x = x;
            this.y = y;

            // calculate only once for GetHashCode
            bytesX = BitConverter.GetBytes(x);
            bytesY = BitConverter.GetBytes(y);
        }

        public sealed override bool Equals(object other)
        {
            if (other is ResultPoint)
            {
                ResultPoint otherPoint = (ResultPoint)other;
                return x == otherPoint.x && y == otherPoint.y;
            }
            return false;
        }

        public sealed override int GetHashCode()
        {
            return 31 * ((bytesX[0] << 24) + (bytesX[1] << 16) + (bytesX[2] << 8) + bytesX[3]) +
                   (bytesY[0] << 24) + (bytesY[1] << 16) + (bytesY[2] << 8) + bytesY[3];
        }

        public sealed override string ToString()
        {
            StringBuilder result = new StringBuilder(25);
            result.Append('(');
            result.Append(x);
            result.Append(',');
            result.Append(y);
            result.Append(')');
            return result.ToString();
        }

        /// <summary>
        /// Orders an array of three ResultPoints in an order [A,B,C] such that AB less than AC and
        /// BC less than AC and the angle between BC and BA is less than 180 degrees.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        internal static void OrderBestPatterns(ResultPoint[] patterns)
        {

            // Find distances between pattern centers
            float zeroOneDistance = Distance(patterns[0], patterns[1]);
            float oneTwoDistance = Distance(patterns[1], patterns[2]);
            float zeroTwoDistance = Distance(patterns[0], patterns[2]);

            ResultPoint pointA;
            ResultPoint pointB;
            ResultPoint pointC;

            // Assume one closest to other two is B; A and C will just be guesses at first
            if (oneTwoDistance >= zeroOneDistance && oneTwoDistance >= zeroTwoDistance)
            {
                pointB = patterns[0];
                pointA = patterns[1];
                pointC = patterns[2];
            }
            else if (zeroTwoDistance >= oneTwoDistance && zeroTwoDistance >= zeroOneDistance)
            {
                pointB = patterns[1];
                pointA = patterns[0];
                pointC = patterns[2];
            }
            else
            {
                pointB = patterns[2];
                pointA = patterns[0];
                pointC = patterns[1];
            }

            // Use cross product to figure out whether A and C are correct or flipped.
            // This asks whether BC x BA has a positive z component, which is the arrangement
            // we want for A, B, C. If it's negative, then we've got it flipped around and
            // should swap A and C.
            if (CrossProductZ(pointA, pointB, pointC) < 0.0f)
            {
                ResultPoint temp = pointA;
                pointA = pointC;
                pointC = temp;
            }

            patterns[0] = pointA;
            patterns[1] = pointB;
            patterns[2] = pointC;
        }


        /// <summary>
        /// Distances the specified pattern1.
        /// </summary>
        /// <param name="pattern1">The pattern1.</param>
        /// <param name="pattern2">The pattern2.</param>
        /// <returns>distance between two points</returns>
        public static float Distance(ResultPoint pattern1, ResultPoint pattern2)
        {
            return MathUtils.Distance(pattern1.x, pattern1.y, pattern2.x, pattern2.y);
        }

        /// <summary>
        /// Returns the z component of the cross product between vectors BC and BA.
        /// </summary>
        /// <param name="pointA">The point A.</param>
        /// <param name="pointB">The point B.</param>
        /// <param name="pointC">The point C.</param>
        /// <returns></returns>
        private static float CrossProductZ(ResultPoint pointA, ResultPoint pointB, ResultPoint pointC)
        {
            float bX = pointB.x;
            float bY = pointB.y;
            return ((pointC.x - bX) * (pointA.y - bY)) - ((pointC.y - bY) * (pointA.x - bX));
        }
    }
}