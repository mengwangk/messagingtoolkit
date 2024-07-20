using System;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;

namespace MessagingToolkit.Barcode.QRCode.Detector
{


    /// <summary>
    /// <p>Encapsulates an alignment pattern, which are the smaller square patterns found in
    /// all but the simplest QR Codes.</p>
    /// 
    /// Modified: April 27 2012
    /// </summary>
    internal sealed class AlignmentPattern
    {
        private readonly float estimatedModuleSize;
        private readonly float x;
        private readonly float y;
        private readonly byte[] bytesX;
        private readonly byte[] bytesY;


        internal AlignmentPattern(float posX, float posY, float estimatedModuleSize)
        {
            this.estimatedModuleSize = estimatedModuleSize;
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

        public float Y
        {
            get
            {
                return y;
            }

        }

        public sealed override bool Equals(Object other)
        {
            if (other is AlignmentPattern)
            {
                AlignmentPattern otherPoint = (AlignmentPattern)other;
                return x == otherPoint.x && y == otherPoint.y;
            }
            return false;
        }

        public static implicit operator ResultPoint(AlignmentPattern point)
        {
            if (point == null) return null;

            return new ResultPoint(point.X, point.Y);
        }

        public sealed override int GetHashCode()
        {
            return 31 * ((bytesX[0] << 24) + (bytesX[1] << 16) + (bytesX[2] << 8) + bytesX[3]) +
                   (bytesY[0] << 24) + (bytesY[1] << 16) + (bytesY[2] << 8) + bytesY[3];
        }

        /// <summary>
        /// <p>Determines if this alignment pattern "about equals" an alignment pattern at the stated
        /// position and size -- meaning, it is at nearly the same center with nearly the same size.</p>
        /// </summary>
        ///
        internal bool AboutEquals(float moduleSize, float i, float j)
        {
            if (Math.Abs(i - this.Y) <= moduleSize && Math.Abs(j - this.X) <= moduleSize)
            {
                float moduleSizeDiff = Math.Abs(moduleSize - estimatedModuleSize);
                return moduleSizeDiff <= 1.0f || moduleSizeDiff <= estimatedModuleSize;
            }
            return false;
        }

        /// <summary>
        /// Combines this object's current estimate of a finder pattern position and module size
        /// with a new estimate. It returns a new } containing an average of the two.
        /// </summary>
        ///
        internal AlignmentPattern CombineEstimate(float i, float j, float newModuleSize)
        {
            float combinedX = (this.X + j) / 2.0f;
            float combinedY = (this.Y + i) / 2.0f;
            float combinedModuleSize = (estimatedModuleSize + newModuleSize) / 2.0f;
            return new AlignmentPattern(combinedX, combinedY, combinedModuleSize);
        }
    }
}
