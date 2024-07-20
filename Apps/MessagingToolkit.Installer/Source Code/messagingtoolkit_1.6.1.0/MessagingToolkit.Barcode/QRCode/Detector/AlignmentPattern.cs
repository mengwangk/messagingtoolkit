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
    public sealed class AlignmentPattern : ResultPoint
    {

        private readonly float estimatedModuleSize;

        internal AlignmentPattern(float posX, float posY, float estimatedModuleSize_0)
            : base(posX, posY)
        {
            this.estimatedModuleSize = estimatedModuleSize_0;
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
