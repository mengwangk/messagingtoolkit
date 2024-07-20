using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// Simply encapsulates a width and height.
    /// </summary>
    public sealed class Dimension
    {

        private readonly int width;
        private readonly int height;

        public Dimension(int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException();
            }
            this.width = width;
            this.height = height;
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public override bool Equals(object other)
        {
            if (other is Dimension)
            {
                Dimension d = (Dimension)other;
                return width == d.width && height == d.height;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return width * 32713 + height;
        }

        public override string ToString()
        {
            return width + "x" + height;
        }

    }

}
