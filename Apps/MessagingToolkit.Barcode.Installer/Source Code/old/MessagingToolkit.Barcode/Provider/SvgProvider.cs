using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Windows.UI;
#elif SILVERLIGHT
using System.Windows.Media;
#else
using System.Drawing;
#endif

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Provider
{
    /// <summary>
    /// SVG provider.
    /// </summary>
    public sealed class SvgProvider : IOutputProvider<Svg>
    {
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        /// <value>The foreground color.</value>
        public Color Foreground { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>The background color.</value>
        public Color Background { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="SvgProvider"/> class.
        /// </summary>
        public SvgProvider()
        {
#if NETFX_CORE || SILVERLIGHT
            Foreground = Colors.Black;
            Background = Colors.White;
#else
            Foreground = Color.Black;
            Background = Color.White;
#endif
        }


        private void Create(Svg image, BitMatrix matrix, BarcodeFormat format, string content, Dictionary<EncodeOptions, object> options)
        {
            const int quietZone = 5;

            if (matrix == null)
                return;

            int width = matrix.Width;
            int height = matrix.Height;
            image.AddHeader();
            image.AddTag(0, 0, 2 * quietZone + width, 2 * quietZone + height, Background, Foreground);
            AppendDarkCell(image, matrix, quietZone, quietZone);
            image.AddEnd();
        }

        private static void AppendDarkCell(Svg image, BitMatrix matrix, int offsetX, int offSetY)
        {
            if (matrix == null)
                return;

            int width = matrix.Width;
            int height = matrix.Height;
            var processed = new BitMatrix(width, height);
            bool currentIsBlack = false;
            int startPosX = 0;
            int startPosY = 0;
            for (int x = 0; x < width; x++)
            {
                int endPosX;
                for (int y = 0; y < height; y++)
                {
                    if (processed.Get(x, y))
                        continue;

                    processed.Set(x, y);

                    if (matrix.Get(x, y))
                    {
                        if (!currentIsBlack)
                        {
                            startPosX = x;
                            startPosY = y;
                            currentIsBlack = true;
                        }
                    }
                    else
                    {
                        if (currentIsBlack)
                        {
                            FindMaximumRectangle(matrix, processed, startPosX, startPosY, y, out endPosX);
                            image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, y - startPosY);
                            currentIsBlack = false;
                        }
                    }
                }
                if (currentIsBlack)
                {
                    FindMaximumRectangle(matrix, processed, startPosX, startPosY, height, out endPosX);
                    image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, height - startPosY);
                    currentIsBlack = false;
                }
            }
        }

        private static void FindMaximumRectangle(BitMatrix matrix, BitMatrix processed, int startPosX, int startPosY, int endPosY, out int endPosX)
        {
            endPosX = startPosX + 1;

            for (int x = startPosX + 1; x < matrix.Width; x++)
            {
                for (int y = startPosY; y < endPosY; y++)
                {
                    if (!matrix.Get(x, y))
                    {
                        return;
                    }
                }
                endPosX = x;
                for (int y = startPosY; y < endPosY; y++)
                {
                    processed.Set(x, y);
                }
            }
        }

        /// <summary>
        /// Encodes the specified format.
        /// </summary>
        /// <param name="bitMatrix">The bit matrix.</param>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The SVG image
        /// </returns>
        public Svg Generate(BitMatrix bitMatrix, BarcodeFormat format, string content)
        {
            return Generate(bitMatrix, format, content, null);

        }

        /// <summary>
        /// Encodes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <param name="bitMatrix">The bit matrix.</param>
        /// <returns>The SVG image</returns>
        public Svg Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, Dictionary<EncodeOptions, object> options)
        {
            var result = new Svg();
            Create(result, bitMatrix, format, content, options);
            return result;
        }
    }


    /// <summary>
    /// Represents a barcode as a Svg image.
    /// </summary>
    public sealed class Svg
    {
        private readonly StringBuilder content;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public String Content
        {
            get { return content.ToString(); }
            set { content.Length = 0; if (value != null) content.Append(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Svg" /> class.
        /// </summary>
        public Svg()
        {
            this.content = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Svg"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public Svg(string content)
        {
            this.content = new StringBuilder(content);
        }

        /// <summary>
        /// Gives the XML representation of the SVG image
        /// </summary>
        public override string ToString()
        {
            return this.content.ToString();
        }

        internal void AddHeader()
        {
            content.Append("<?xml version=\"1.0\" standalone=\"no\"?>");
            content.Append(@"<!-- Created with MessagingToolkit Barcode Library (http://platform.twit88.com/projects/mt-barcode) -->");
            content.Append("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
        }

        internal void AddEnd()
        {
            content.Append("</svg>");
        }

        internal void AddTag(int displaysizeX, int displaysizeY, int viewboxSizeX, int viewboxSizeY, Color background, Color fill)
        {

            if (displaysizeX <= 0 || displaysizeY <= 0)
                content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6}>",
                    viewboxSizeX,
                    viewboxSizeY,
                    GetColorRgb(background),
                    ConvertAlpha(background),
                    GetColorRgb(fill),
                    ConvertAlpha(fill),
                    GetBackgroundStyle(background)
                    ));
            else
                content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6} width=\"{7}\" height=\"{8}\">",
                    viewboxSizeX,
                    viewboxSizeY,
                    GetColorRgb(background),
                    ConvertAlpha(background),
                    GetColorRgb(fill),
                    ConvertAlpha(fill),
                    GetBackgroundStyle(background),
                    displaysizeX,
                    displaysizeY));
        }

        internal void AddRec(int posX, int posY, int width, int height)
        {
            content.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\"/>", posX, posY, width, height);
        }

        internal static double ConvertAlpha(Color alpha)
        {
            return Math.Round((((double)alpha.A) / (double)255), 2);
        }

        internal static string GetBackgroundStyle(Color color)
        {
            double alpha = ConvertAlpha(color);
            return string.Format("style=\"background-color:rgb({0},{1},{2});background-color:rgba({3});\"",
                color.R, color.G, color.B, alpha);
        }

        internal static string GetColorRgb(Color color)
        {
            return color.R + "," + color.G + "," + color.B;
        }

        internal static string GetColorRgba(Color color)
        {
            double alpha = ConvertAlpha(color);
            return color.R + "," + color.G + "," + color.B + "," + alpha;
        }
    }
}
