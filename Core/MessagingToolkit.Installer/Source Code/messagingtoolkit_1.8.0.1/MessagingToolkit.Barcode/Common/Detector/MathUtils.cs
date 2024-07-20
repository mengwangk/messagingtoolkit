using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Common.Detector
{
    /// <summary>
    /// Added May 25 2012
    /// </summary>
    public sealed class MathUtils
    {
        private MathUtils()
        {
        }

        /**
         * Ends up being a bit faster than {@link Math#round(float)}. This merely rounds its
         * argument to the nearest int, where x.5 rounds up to x+1.
         */
        public static int Round(float d)
        {
            return (int)(d + 0.5f);
        }

        public static float Distance(float aX, float aY, float bX, float bY)
        {
            float xDiff = aX - bX;
            float yDiff = aY - bY;
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public static float Distance(int aX, int aY, int bX, int bY)
        {
            int xDiff = aX - bX;
            int yDiff = aY - bY;
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

    }
}
