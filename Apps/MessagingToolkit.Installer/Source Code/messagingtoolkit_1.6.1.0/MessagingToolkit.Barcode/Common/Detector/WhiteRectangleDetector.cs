using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Common.Detector
{
    /// <summary>
    /// <p>
    /// Detects a candidate barcode-like rectangular region within an image. It
    /// starts around the center of the image, increases the size of the candidate
    /// region until it finds a white rectangular region. By keeping track of the
    /// last black points it encountered, it determines the corners of the barcode.
    /// </p>
    /// 
    /// Modified: May 25 2012
    /// </summary>
    public sealed class WhiteRectangleDetector
    {

        private const int InitSize = 30;
        private const int Corr = 1;

        private readonly BitMatrix image;
        private readonly int height;
        private readonly int width;
        private readonly int leftInit;
        private readonly int rightInit;
        private readonly int downInit;
        private readonly int upInit;


        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteRectangleDetector"/> class.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <exception cref="NotFoundException">if image is too small</exception>
        public WhiteRectangleDetector(BitMatrix img)
        {
            this.image = img;
            height = img.Height;
            width = img.Width;
            leftInit = (width - InitSize) >> 1;
            rightInit = (width + InitSize) >> 1;
            upInit = (height - InitSize) >> 1;
            downInit = (height + InitSize) >> 1;
            if (upInit < 0 || leftInit < 0 || downInit >= height
                    || rightInit >= width)
            {
                throw NotFoundException.Instance;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteRectangleDetector"/> class.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <param name="initSize">Size of the init.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <exception cref="NotFoundException">if image is too small</exception>
        public WhiteRectangleDetector(BitMatrix img, int initSize, int x, int y)
        {
            this.image = img;
            height = img.Height;
            width = img.Width;
            int halfsize = initSize >> 1;
            leftInit = x - halfsize;
            rightInit = x + halfsize;
            upInit = y - halfsize;
            downInit = y + halfsize;
            if (upInit < 0 || leftInit < 0 || downInit >= height
                    || rightInit >= width)
            {
                throw NotFoundException.Instance;
            }
        }

        /// <summary>
        ///   <p>
        /// Detects a candidate barcode-like rectangular region within an image. It
        /// starts around the center of the image, increases the size of the candidate
        /// region until it finds a white rectangular region.
        ///   </p>
        /// </summary>
        /// <returns>
        /// /// <see cref="null"/>
        /// describing the corners of the rectangular
        /// region. The first and last points are opposed on the diagonal, as
        /// are the second and third. The first point will be the topmost
        /// point and the last, the bottommost. The second point will be
        /// leftmost and the third, the rightmost
        /// </returns>
        /// <exception cref="NotFoundException">if no Data Matrix Code can be found</exception>
        public ResultPoint[] Detect()
        {

            int left = leftInit;
            int right = rightInit;
            int up = upInit;
            int down = downInit;
            bool sizeExceeded = false;
            bool aBlackPointFoundOnBorder = true;
            bool atLeastOneBlackPointFoundOnBorder = false;

            while (aBlackPointFoundOnBorder)
            {

                aBlackPointFoundOnBorder = false;

                // .....
                // .   |
                // .....
                bool rightBorderNotWhite = true;
                while (rightBorderNotWhite && right < width)
                {
                    rightBorderNotWhite = ContainsBlackPoint(up, down, right, false);
                    if (rightBorderNotWhite)
                    {
                        right++;
                        aBlackPointFoundOnBorder = true;
                    }
                }

                if (right >= width)
                {
                    sizeExceeded = true;
                    break;
                }

                // .....
                // .   .
                // .___.
                bool bottomBorderNotWhite = true;
                while (bottomBorderNotWhite && down < height)
                {
                    bottomBorderNotWhite = ContainsBlackPoint(left, right, down,
                            true);
                    if (bottomBorderNotWhite)
                    {
                        down++;
                        aBlackPointFoundOnBorder = true;
                    }
                }

                if (down >= height)
                {
                    sizeExceeded = true;
                    break;
                }

                // .....
                // |   .
                // .....
                bool leftBorderNotWhite = true;
                while (leftBorderNotWhite && left >= 0)
                {
                    leftBorderNotWhite = ContainsBlackPoint(up, down, left, false);
                    if (leftBorderNotWhite)
                    {
                        left--;
                        aBlackPointFoundOnBorder = true;
                    }
                }

                if (left < 0)
                {
                    sizeExceeded = true;
                    break;
                }

                // .___.
                // .   .
                // .....
                bool topBorderNotWhite = true;
                while (topBorderNotWhite && up >= 0)
                {
                    topBorderNotWhite = ContainsBlackPoint(left, right, up, true);
                    if (topBorderNotWhite)
                    {
                        up--;
                        aBlackPointFoundOnBorder = true;
                    }
                }

                if (up < 0)
                {
                    sizeExceeded = true;
                    break;
                }

                if (aBlackPointFoundOnBorder)
                {
                    atLeastOneBlackPointFoundOnBorder = true;
                }

            }

            if (!sizeExceeded && atLeastOneBlackPointFoundOnBorder)
            {

                int maxSize = right - left;

                ResultPoint z = null;
                for (int i = 1; i < maxSize; i++)
                {
                    z = GetBlackPointOnSegment(left, down - i, left + i, down);
                    if (z != null)
                    {
                        break;
                    }
                }

                if (z == null)
                {
                    throw NotFoundException.Instance;
                }

                ResultPoint t = null;
                //go down right
                for (int i_0 = 1; i_0 < maxSize; i_0++)
                {
                    t = GetBlackPointOnSegment(left, up + i_0, left + i_0, up);
                    if (t != null)
                    {
                        break;
                    }
                }

                if (t == null)
                {
                    throw NotFoundException.Instance;
                }

                ResultPoint x = null;
                //go down left
                for (int i_1 = 1; i_1 < maxSize; i_1++)
                {
                    x = GetBlackPointOnSegment(right, up + i_1, right - i_1, up);
                    if (x != null)
                    {
                        break;
                    }
                }

                if (x == null)
                {
                    throw NotFoundException.Instance;
                }

                ResultPoint y = null;
                //go up left
                for (int i_2 = 1; i_2 < maxSize; i_2++)
                {
                    y = GetBlackPointOnSegment(right, down - i_2, right - i_2, down);
                    if (y != null)
                    {
                        break;
                    }
                }

                if (y == null)
                {
                    throw NotFoundException.Instance;
                }

                return CenterEdges(y, z, x, t);

            }
            else
            {
                throw NotFoundException.Instance;
            }
        }

        private ResultPoint GetBlackPointOnSegment(float aX, float aY, float bX,
                float bY)
        {
            int dist = MathUtils.Round(MathUtils.Distance(aX, aY, bX, bY));
            float xStep = (bX - aX) / dist;
            float yStep = (bY - aY) / dist;

            for (int i = 0; i < dist; i++)
            {
                int x = MathUtils.Round(aX + i * xStep);
                int y = MathUtils.Round(aY + i * yStep);
                if (image.Get(x, y))
                {
                    return new ResultPoint(x, y);
                }
            }
            return null;
        }

      
        /// <summary>
        /// recenters the points of a constant distance towards the center
        /// </summary>
        ///
        /// <param name="y">bottom most point</param>
        /// <param name="z">left most point</param>
        /// <param name="x">right most point</param>
        /// <param name="t">top most point</param>
        /// <returns>/// <see cref="null"/>
        /// [] describing the corners of the rectangular
        /// region. The first and last points are opposed on the diagonal, as
        /// are the second and third. The first point will be the topmost
        /// point and the last, the bottommost. The second point will be
        /// leftmost and the third, the rightmost</returns>
        private ResultPoint[] CenterEdges(ResultPoint y, ResultPoint z,
                ResultPoint x, ResultPoint t)
        {

            //
            //       t            t
            //  z                      x
            //        x    OR    z
            //   y                    y
            //

            float yi = y.X;
            float yj = y.Y;
            float zi = z.X;
            float zj = z.Y;
            float xi = x.X;
            float xj = x.Y;
            float ti = t.X;
            float tj = t.Y;

            if (yi < width / 2)
            {
                return new ResultPoint[] { new ResultPoint(ti - Corr, tj + Corr),
						new ResultPoint(zi + Corr, zj + Corr),
						new ResultPoint(xi - Corr, xj - Corr),
						new ResultPoint(yi + Corr, yj - Corr) };
            }
            else
            {
                return new ResultPoint[] { new ResultPoint(ti + Corr, tj + Corr),
						new ResultPoint(zi + Corr, zj - Corr),
						new ResultPoint(xi - Corr, xj + Corr),
						new ResultPoint(yi - Corr, yj - Corr) };
            }
        }

        /// <summary>
        /// Determines whether a segment contains a black point
        /// </summary>
        ///
        /// <param name="a">min value of the scanned coordinate</param>
        /// <param name="b">max value of the scanned coordinate</param>
        /// <param name="fixed">value of fixed coordinate</param>
        /// <param name="horizontal">set to true if scan must be horizontal, false if vertical</param>
        /// <returns>true if a black point has been found, else false.</returns>
        private bool ContainsBlackPoint(int a, int b, int fix,
                bool horizontal)
        {

            if (horizontal)
            {
                for (int x = a; x <= b; x++)
                {
                    if (image.Get(x, fix))
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int y = a; y <= b; y++)
                {
                    if (image.Get(fix, y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
