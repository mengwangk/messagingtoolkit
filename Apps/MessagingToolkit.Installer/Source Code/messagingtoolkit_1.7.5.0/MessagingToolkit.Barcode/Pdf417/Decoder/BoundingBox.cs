using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class BoundingBox
    {

        private BitMatrix image;
        private ResultPoint topLeft;
        private ResultPoint bottomLeft;
        private ResultPoint topRight;
        private ResultPoint bottomRight;
        private int minX;
        private int maxX;
        private int minY;
        private int maxY;

        internal BoundingBox(BitMatrix image, ResultPoint topLeft, ResultPoint bottomLeft, ResultPoint topRight, ResultPoint bottomRight)
        {
            if ((topLeft == null && topRight == null) ||
                (bottomLeft == null && bottomRight == null) ||
                (topLeft != null && bottomLeft == null) ||
                (topRight != null && bottomRight == null))
            {
                throw NotFoundException.Instance;
            }
            Init(image, topLeft, bottomLeft, topRight, bottomRight);
        }

        internal BoundingBox(BoundingBox boundingBox)
        {
            Init(boundingBox.image, boundingBox.topLeft, boundingBox.bottomLeft, boundingBox.topRight, boundingBox.bottomRight);
        }

        private void Init(BitMatrix image, ResultPoint topLeft, ResultPoint bottomLeft, ResultPoint topRight, ResultPoint bottomRight)
        {
            this.image = image;
            this.topLeft = topLeft;
            this.bottomLeft = bottomLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            CalculateMinMaxValues();
        }

        internal static BoundingBox Merge(BoundingBox leftBox, BoundingBox rightBox)
        {
            if (leftBox == null)
            {
                return rightBox;
            }
            if (rightBox == null)
            {
                return leftBox;
            }
            return new BoundingBox(leftBox.image, leftBox.topLeft, leftBox.bottomLeft, rightBox.topRight, rightBox.bottomRight);
        }

        internal BoundingBox AddMissingRows(int missingStartRows, int missingEndRows, bool isLeft)
        {
            ResultPoint newTopLeft = topLeft;
            ResultPoint newBottomLeft = bottomLeft;
            ResultPoint newTopRight = topRight;
            ResultPoint newBottomRight = bottomRight;

            if (missingStartRows > 0)
            {
                ResultPoint top = isLeft ? topLeft : topRight;
                int newMinY = (int)top.Y - missingStartRows;
                if (newMinY < 0)
                {
                    newMinY = 0;
                }
                // TODO use existing points to better interpolate the new x positions
                ResultPoint newTop = new ResultPoint(top.X, newMinY);
                if (isLeft)
                {
                    newTopLeft = newTop;
                }
                else
                {
                    newTopRight = newTop;
                }
            }

            if (missingEndRows > 0)
            {
                ResultPoint bottom = isLeft ? bottomLeft : bottomRight;
                int newMaxY = (int)bottom.Y + missingEndRows;
                if (newMaxY >= image.Height)
                {
                    newMaxY = image.Height - 1;
                }
                // TODO use existing points to better interpolate the new x positions
                ResultPoint newBottom = new ResultPoint(bottom.X, newMaxY);
                if (isLeft)
                {
                    newBottomLeft = newBottom;
                }
                else
                {
                    newBottomRight = newBottom;
                }
            }
            CalculateMinMaxValues();
            return new BoundingBox(image, newTopLeft, newBottomLeft, newTopRight, newBottomRight);
        }

        private void CalculateMinMaxValues()
        {
            if (topLeft == null)
            {
                topLeft = new ResultPoint(0, topRight.Y);
                bottomLeft = new ResultPoint(0, bottomRight.Y);
            }
            else if (topRight == null)
            {
                topRight = new ResultPoint(image.Width - 1, topLeft.Y);
                bottomRight = new ResultPoint(image.Width - 1, bottomLeft.Y);
            }

            minX = (int)Math.Min(topLeft.X, bottomLeft.X);
            maxX = (int)Math.Max(topRight.X, bottomRight.X);
            minY = (int)Math.Min(topLeft.Y, topRight.Y);
            maxY = (int)Math.Max(bottomLeft.Y, bottomRight.Y);
        }

        internal ResultPoint TopRight
        {
            set
            {
                this.topRight = value;
                CalculateMinMaxValues();
            }
            get
            {
                return topRight;
            }
        }

        internal ResultPoint BottomRight
        {
            set
            {
                this.bottomRight = value;
                CalculateMinMaxValues();
            }
            get
            {
                return bottomRight;
            }
        }

        internal int MinX
        {
            get
            {
                return minX;
            }
        }

        internal int MaxX
        {
            get
            {
                return maxX;
            }
        }

        internal int MinY
        {
            get
            {
                return minY;
            }
        }

        internal int MaxY
        {
            get
            {
                return maxY;
            }
        }

        internal ResultPoint TopLeft
        {
            get
            {
                return topLeft;
            }
        }


        internal ResultPoint BottomLeft
        {
            get
            {
                return bottomLeft;
            }
        }


    }
}
