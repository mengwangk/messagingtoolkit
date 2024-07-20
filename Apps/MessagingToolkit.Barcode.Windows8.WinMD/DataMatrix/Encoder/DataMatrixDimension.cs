using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    // <summary>
    /// This class provides information on the dimensions of a barcode. It makes a
    /// distinction between the dimensions with and without quiet zone.
    /// </summary>
    internal class DataMatrixDimension
    {

        private double width;
        private double height;

        private double widthPlusQuiet;
        private double heightPlusQuiet;
        private double xOffset;
        private double yOffset;

        /// <summary>
        /// Creates a new BarcodeDimension object. No quiet-zone is respected.
        /// </summary>
        /// <param name="w">width of the barcode in millimeters (mm).</param>
        /// <param name="h">height of the barcode in millimeters (mm).</param>
        public DataMatrixDimension(double w, double h)
        {
            this.width = w;
            this.height = h;
            this.widthPlusQuiet = this.width;
            this.heightPlusQuiet = this.height;
            this.xOffset = 0.0;
            this.yOffset = 0.0;
        }

        /// <summary>
        /// Creates a new BarcodeDimension object. </summary>
        /// <param name="w"> width of the raw barcode (without quiet-zone) in millimeters (mm). </param>
        /// <param name="h"> height of the raw barcode (without quiet-zone) in millimeters (mm). </param>
        /// <param name="wpq"> width of the barcode (quiet-zone included) in millimeters (mm). </param>
        /// <param name="hpq"> height of the barcode (quiet-zone included) in millimeters (mm). </param>
        /// <param name="xoffset"> x-offset if the upper-left corner of the barcode within 
        /// the extended barcode area. </param>
        /// <param name="yoffset"> y-offset if the upper-left corner of the barcode within 
        /// the extended barcode area. </param>
        public DataMatrixDimension(double w, double h, double wpq, double hpq, double xoffset, double yoffset)
        {
            this.width = w;
            this.height = h;
            this.widthPlusQuiet = wpq;
            this.heightPlusQuiet = hpq;
            this.xOffset = xoffset;
            this.yOffset = yoffset;
        }


        /// <summary>
        /// Returns the height of the barcode (ignores quiet-zone). </summary>
        /// <returns> height in millimeters (mm) </returns>
        public virtual double Height
        {
            get
            {
                return height;
            }
        }

        public virtual double GetHeight(int orientation)
        {
            orientation = NormalizeOrientation(orientation);
            if (orientation % 180 != 0)
            {
                return Width;
            }
            else
            {
                return Height;
            }
        }

        /// <summary>
        /// Returns the height of the barcode (quiet-zone included). </summary>
        /// <returns> height in millimeters (mm) </returns>
        public virtual double HeightPlusQuiet
        {
            get
            {
                return heightPlusQuiet;
            }
        }

        public virtual double GetHeightPlusQuiet(int orientation)
        {
            orientation = NormalizeOrientation(orientation);
            if (orientation % 180 != 0)
            {
                return WidthPlusQuiet;
            }
            else
            {
                return HeightPlusQuiet;
            }
        }

        /// <summary>
        /// Returns the width of the barcode (ignores quiet-zone). </summary>
        /// <returns> width in millimeters (mm) </returns>
        public virtual double Width
        {
            get
            {
                return width;
            }
        }

        public static int NormalizeOrientation(int orientation)
        {
            switch (orientation)
            {
                case 0:
                    return 0;
                case 90:
                case -270:
                    return 90;
                case 180:
                case -180:
                    return 180;
                case 270:
                case -90:
                    return 270;
                default:
                    throw new System.ArgumentException("Orientation must be 0, 90, 180, 270, -90, -180 or -270");
            }
        }

        public virtual double GetWidth(int orientation)
        {
            orientation = NormalizeOrientation(orientation);
            if (orientation % 180 != 0)
            {
                return Height;
            }
            else
            {
                return Width;
            }
        }

        /// <summary>
        /// Returns the width of the barcode (quiet-zone included). </summary>
        /// <returns> width in millimeters (mm) </returns>
        public virtual double WidthPlusQuiet
        {
            get
            {
                return widthPlusQuiet;
            }
        }

        public virtual double GetWidthPlusQuiet(int orientation)
        {
            orientation = NormalizeOrientation(orientation);
            if (orientation % 180 != 0)
            {
                return HeightPlusQuiet;
            }
            else
            {
                return WidthPlusQuiet;
            }
        }

        /// <summary>
        /// Returns the x-offset of the upper-left corner of the barcode within the 
        /// extended barcode area. </summary>
        /// <returns> double x-offset in millimeters (mm) </returns>
        public virtual double XOffset
        {
            get
            {
                return xOffset;
            }
        }

        /// <summary>
        /// Returns the y-offset of the upper-left corner of the barcode within the 
        /// extended barcode area. </summary>
        /// <returns> double y-offset in millimeters (mm) </returns>
        public virtual double YOffset
        {
            get
            {
                return yOffset;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("[width=");
            sb.Append(Width);
            sb.Append("(");
            sb.Append(WidthPlusQuiet);
            sb.Append("),height=");
            sb.Append(Height);
            sb.Append("(");
            sb.Append(HeightPlusQuiet);
            sb.Append(")]");
            return sb.ToString();
        }

    }
}
