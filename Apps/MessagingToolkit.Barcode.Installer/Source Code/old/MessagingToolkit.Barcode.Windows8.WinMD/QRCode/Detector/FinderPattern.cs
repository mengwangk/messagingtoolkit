using MessagingToolkit.Barcode.Common.Detector;
using System;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;

namespace MessagingToolkit.Barcode.QRCode.Detector
{
    /// <summary>
    /// <p>Encapsulates a finder pattern, which are the three square patterns found in
    /// the corners of QR Codes. It also encapsulates a count of similar finder patterns,
    /// as a convenience to the finder's bookkeeping.</p>
    /// 
    /// Modified: April 27 2012
    /// </summary>
    internal sealed class FinderPattern
    {
        private readonly float estimatedModuleSize;
        private int count;
        private readonly float x;
        private readonly float y;
        private readonly byte[] bytesX;
        private readonly byte[] bytesY;

        internal FinderPattern(float posX, float posY, float estimatedModuleSize)
            : this(posX, posY, estimatedModuleSize, 1)
        {
        }

        private FinderPattern(float posX, float posY, float estimatedModuleSize, int count)
        {
            this.estimatedModuleSize = estimatedModuleSize;
            this.count = count;
            this.x = posX;
            this.y = posY;
            // calculate only once for GetHashCode
            bytesX = BitConverter.GetBytes(x);
            bytesY = BitConverter.GetBytes(y);
        }

        public float X
        {
            get
            {
                return x;
            }

        }

        public sealed override bool Equals(System.Object other)
        {
            if (other is ResultPoint)
            {
                FinderPattern otherPoint = (FinderPattern)other;
                return x == otherPoint.x && y == otherPoint.y;
            }
            return false;
        }

        public sealed override int GetHashCode()
        {
            return 31 * ((bytesX[0] << 24) + (bytesX[1] << 16) + (bytesX[2] << 8) + bytesX[3]) +
                   (bytesY[0] << 24) + (bytesY[1] << 16) + (bytesY[2] << 8) + bytesY[3];
        }

        public float Y
        {
            get
            {
                return y;
            }

        }

        public float EstimatedModuleSize
        {
            get
            {
                return estimatedModuleSize;
            }
        }

        internal int Count
        {
            get
            {
                return count;
            }
        }

        internal void IncrementCount()
        {
            this.count++;
        }

        /// <summary>
        /// <p>Determines if this finder pattern "about equals" a finder pattern at the stated
        /// position and size -- meaning, it is at nearly the same center with nearly the same size.</p>
        /// </summary>
        internal bool AboutEquals(float moduleSize, float i, float j)
        {
            if (Math.Abs(i - this.Y) <= moduleSize && Math.Abs(j - this.X) <= moduleSize)
            {
                float moduleSizeDiff = Math.Abs(moduleSize - estimatedModuleSize);
                return moduleSizeDiff <= 1.0f || moduleSizeDiff <= estimatedModuleSize;
            }
            return false;
        }

        public static implicit operator ResultPoint(FinderPattern point)
        {
            return new ResultPoint(point.X, point.Y);
        }

        /// <summary>
        /// Orders an array of three ResultPoints in an order [A,B,C] such that AB less than AC and
        /// BC less than AC and the angle between BC and BA is less than 180 degrees.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        internal static void OrderBestPatterns(FinderPattern[] patterns)
        {

            // Find distances between pattern centers
            float zeroOneDistance = Distance(patterns[0], patterns[1]);
            float oneTwoDistance = Distance(patterns[1], patterns[2]);
            float zeroTwoDistance = Distance(patterns[0], patterns[2]);

            FinderPattern pointA;
            FinderPattern pointB;
            FinderPattern pointC;

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
                FinderPattern temp = pointA;
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
        public static float Distance(FinderPattern pattern1, FinderPattern pattern2)
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
        private static float CrossProductZ(FinderPattern pointA, FinderPattern pointB, FinderPattern pointC)
        {
            float bX = pointB.x;
            float bY = pointB.y;
            return ((pointC.x - bX) * (pointA.y - bY)) - ((pointC.y - bY) * (pointA.x - bX));
        }

        /// <summary>
        /// Combines this object's current estimate of a finder pattern position and module size
        /// with a new estimate. It returns a new } containing a weighted average
        /// based on count.
        /// </summary>
        internal FinderPattern CombineEstimate(float i, float j, float newModuleSize)
        {
            int combinedCount = count + 1;
            float combinedX = (count * this.X + j) / combinedCount;
            float combinedY = (count * this.Y + i) / combinedCount;
            float combinedModuleSize = (count * estimatedModuleSize + newModuleSize) / combinedCount;
            return new FinderPattern(combinedX, combinedY, combinedModuleSize, combinedCount);
        }

    }
}
