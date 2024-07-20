using System;


#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

#if WPF

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

#endif

#endif

namespace MessagingToolkit.Barcode.Common
{
    /// <summary>
    /// Base class for Color, serves as wrapper class for <see cref="T:System.Drawing.Color"/>
    /// to allow extension.
    /// </summary>
    internal class BaseColor
    {
        public static readonly BaseColor WHITE = new BaseColor(255, 255, 255);
        public static readonly BaseColor LIGHT_GRAY = new BaseColor(192, 192, 192);
        public static readonly BaseColor GRAY = new BaseColor(128, 128, 128);
        public static readonly BaseColor DARK_GRAY = new BaseColor(64, 64, 64);
        public static readonly BaseColor BLACK = new BaseColor(0, 0, 0);
        public static readonly BaseColor RED = new BaseColor(255, 0, 0);
        public static readonly BaseColor PINK = new BaseColor(255, 175, 175);
        public static readonly BaseColor ORANGE = new BaseColor(255, 200, 0);
        public static readonly BaseColor YELLOW = new BaseColor(255, 255, 0);
        public static readonly BaseColor GREEN = new BaseColor(0, 255, 0);
        public static readonly BaseColor MAGENTA = new BaseColor(255, 0, 255);
        public static readonly BaseColor CYAN = new BaseColor(0, 255, 255);
        public static readonly BaseColor BLUE = new BaseColor(0, 0, 255);

        private const double FACTOR = 0.7;
        private Color color;

        /// <summary>
        /// Constuctor for Color object.
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 255.</param>
        public BaseColor(int red, int green, int blue)
        {
#if !SILVERLIGHT && !NETFX_CORE
            color = Color.FromArgb(red, green, blue);
#else
            color = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
#endif
        }

        /// <summary>
        /// Constuctor for Color object.
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 255.</param>
        /// <param name="alpha">The transparency component value for the new Color structure. Valid values are 0 through 255.</param>
        public BaseColor(int red, int green, int blue, int alpha)
        {
#if !SILVERLIGHT && !NETFX_CORE

            color = Color.FromArgb(alpha, red, green, blue);
#else
            color = Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
#endif
        }

        /// <summary>
        /// Constructor for Color object
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 1.</param>
        public BaseColor(float red, float green, float blue)
        {
#if !SILVERLIGHT && !NETFX_CORE

            color = Color.FromArgb((int)(red * 255 + .5), (int)(green * 255 + .5), (int)(blue * 255 + .5));
#else
            color = Color.FromArgb(255, (byte)(red * 255 + .5), (byte)(green * 255 + .5), (byte)(blue * 255 + .5));
#endif
        }

        /// <summary>
        /// Constructor for Color object
        /// </summary>
        /// <param name="red">The red component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="green">The green component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="blue">The blue component value for the new Color structure. Valid values are 0 through 1.</param>
        /// <param name="alpha">The transparency component value for the new Color structure. Valid values are 0 through 1.</param>
        public BaseColor(float red, float green, float blue, float alpha)
        {
#if !SILVERLIGHT && !NETFX_CORE
            color = Color.FromArgb((int)(alpha * 255 + .5), (int)(red * 255 + .5), (int)(green * 255 + .5), (int)(blue * 255 + .5));
#else
            color = Color.FromArgb((byte)(alpha * 255 + .5), (byte)(red * 255 + .5), (byte)(green * 255 + .5), (byte)(blue * 255 + .5));
#endif
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public BaseColor(int argb)
        {
            color = Color.FromArgb(argb);
        }
#endif

        /// <summary>
        /// Constructor for Color object
        /// </summary>
        /// <param name="color">a Color object</param>
        /// <overloads>
        /// Has three overloads.
        /// </overloads>
        public BaseColor(Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Gets the red component value of this <see cref="T:System.Drawing.Color"/> structure.
        /// </summary>
        /// <value>The red component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
        public int R
        {
            get
            {
                return color.R;
            }
        }

        /// <summary>
        /// Gets the green component value of this <see cref="T:System.Drawing.Color"/> structure.
        /// </summary>
        /// <value>The green component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
        public int G
        {
            get
            {
                return color.G;
            }
        }

        /// <summary>
        /// Gets the blue component value of this <see cref="T:System.Drawing.Color"/> structure.
        /// </summary>
        /// <value>The blue component value of this <see cref="T:System.Drawing.Color"/> structure.</value>
        public int B
        {
            get
            {
                return color.B;
            }
        }

        public BaseColor Brighter()
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            int i = (int)(1.0 / (1.0 - FACTOR));
            if (r == 0 && g == 0 && b == 0)
                return new BaseColor(i, i, i);

            if (r > 0 && r < i) r = i;
            if (g > 0 && g < i) g = i;
            if (b > 0 && b < i) b = i;

            return new BaseColor(Math.Min((int)(r / FACTOR), 255),
                    Math.Min((int)(g / FACTOR), 255),
                    Math.Min((int)(b / FACTOR), 255));
        }

        public BaseColor Darker()
        {
            return new BaseColor(Math.Max((int)(color.R * FACTOR), 0),
                    Math.Max((int)(color.G * FACTOR), 0),
                    Math.Max((int)(color.B * FACTOR), 0));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BaseColor))
                return false;
            return color.Equals(((BaseColor)obj).color);
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }

#if !SILVERLIGHT  && !NETFX_CORE
        public int ToArgb()
        {
            return color.ToArgb();
        }
#endif

        public override string ToString()
        {
            return color.ToString();
        }
    }
}
