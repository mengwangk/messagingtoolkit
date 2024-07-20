
using System;

namespace MessagingToolkit.Barcode.QRCode.Detector
{

    /// <summary>
    /// <p>Encapsulates information about finder patterns in an image, including the location of
    /// the three finder patterns, and their estimated module size.</p>
    /// </summary>
    ///
    public sealed class FinderPatternInfo
    {
        private FinderPattern bottomLeft;
        private FinderPattern topLeft;
        private FinderPattern topRight;


        public FinderPattern BottomLeft
        {
            get
            {
                return bottomLeft;
            }

        }
        public FinderPattern TopLeft
        {
            get
            {
                return topLeft;
            }

        }
        public FinderPattern TopRight
        {
            get
            {
                return topRight;
            }

        }

        public FinderPatternInfo(FinderPattern[] patternCenters)
        {
            this.bottomLeft = patternCenters[0];
            this.topLeft = patternCenters[1];
            this.topRight = patternCenters[2];
        }
    }
}
