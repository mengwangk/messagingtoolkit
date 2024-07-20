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
    public sealed class FinderPattern : ResultPoint
    {

        private readonly float estimatedModuleSize;
        private int count;

        internal FinderPattern(float posX, float posY, float estimatedModuleSize)
            : this(posX, posY, estimatedModuleSize, 1)
        {
        }

        private FinderPattern(float posX, float posY, float estimatedModuleSize, int count)
            : base(posX, posY)
        {
            this.estimatedModuleSize = estimatedModuleSize;
            this.count = count;
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
