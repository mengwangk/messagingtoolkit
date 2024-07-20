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
    /// Generate EPS output.
    /// </summary>
    public sealed class EpsProvider : IOutputProvider
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
        /// Initializes a new instance of the <see cref="EpsProvider"/> class.
        /// </summary>
        public EpsProvider()
        {
#if NETFX_CORE || SILVERLIGHT
            Foreground = Colors.Black;
            Background = Colors.White;
#else
            Foreground = Color.Black;
            Background = Color.White;
#endif
        }


        public IOutput Generate(BitMatrix bitMatrix, BarcodeFormat format, string content)
        {
            Eps eps = new Eps();
            Create(eps, bitMatrix, format, content, null);
            return eps;
        }

        public IOutput Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, IDictionary<EncodeOptions, object> options)
        {
            Eps eps = new Eps();
            Create(eps, bitMatrix, format, content, options);
            return eps;
        }

        private void Create(Eps image, BitMatrix matrix, BarcodeFormat format, string content, IDictionary<EncodeOptions, object> options)
        {
            if (matrix == null)
                return;

            int width = matrix.Width;
            int height = matrix.Height;
            image.AddHeader(width, height);
            image.SetForeground(this.Foreground);
            image.AddDef(matrix);
            image.AddEnd();
        }
    }


    /// <summary>
    /// Represents a EPS image.
    /// </summary>
    public sealed class Eps:IOutput
    {
        private readonly StringBuilder content;

        private const int BlockSize = 4;

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
        /// Initializes a new instance of the <see cref="Eps" /> class.
        /// </summary>
        public Eps()
        {
            content = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Eps"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public Eps(string content)
        {
            this.content = new StringBuilder(content);
        }

        /// <summary>
        /// Gives the XML representation of the EPS image
        /// </summary>
        public override string ToString()
        {
            return content.ToString();
        }

        internal void AddHeader(double width, double height)
        {

            content.Append("%!PS-Adobe-3.0 EPSF-3.0\n");
            content.Append("%%BoundingBox: 0 0 "
                    + Math.Round(Math.Ceiling(width)) + " "
                    + Math.Round(Math.Ceiling(height)) + "\n");
            content.Append("%%HiResBoundingBox: 0 0 "
                    + width.ToString("0.####") + " "
                    + height.ToString("0.####") + "\n");
            content.Append("%%Creator: MessagingToolkit Barcode Library (http://platform.twit88.com/projects/mt-barcode)\n");
            content.Append("%%CreationDate: " + DateTime.Now.ToString("yyyy-MM-dd'T'hh:mm:ss") + "\n");
            content.Append("%%LanguageLevel: 1\n");
            content.Append("%%EndComments\n");
            content.Append("%%BeginProlog\n");
            content.Append("%%BeginProcSet: barcode-procset 1.1\n");
            content.Append("/rf {\n"); //rect fill: x y w h rf
            content.Append("newpath\n");
            content.Append("4 -2 roll moveto\n");
            content.Append("dup neg 0 exch rlineto\n");
            content.Append("exch 0 rlineto\n");
            content.Append("0 neg exch rlineto\n");
            content.Append("closepath fill\n");
            content.Append("} def\n");

            content.Append("/ct {\n"); //centered text: (text) middle-x y ct
            content.Append("moveto dup stringwidth\n");
            content.Append("2 div neg exch 2 div neg exch\n");
            content.Append("rmoveto show\n");
            content.Append("} def\n");

            content.Append("/rt {\n"); //right-aligned text: (text) x1 x2 y rt
            //Calc string width
            content.Append("4 -1 roll dup stringwidth pop\n");
            //Calc available width (x2-x1)
            content.Append("5 -2 roll 1 index sub\n");
            //Calc (text-width - avail-width) = diffx
            content.Append("3 -1 roll sub\n");
            //Calc x = (x1 + diffx)
            content.Append("add\n");
            //moveto and finally show
            content.Append("3 -1 roll moveto show\n");
            content.Append("} def\n");

            content.Append("/jt {\n"); //justified: (text) x1 x2 y jt
            //Calc string width
            content.Append("4 -1 roll dup stringwidth pop\n");
            //Calc available width (x2-x1)
            content.Append("5 -2 roll 1 index sub\n");
            //Calc (text-width - avail-width)
            content.Append("3 -1 roll sub\n");
            //Get string length
            content.Append("2 index length\n");
            //avail-width / (string-length - 1) = distributable-space
            content.Append("1 sub div\n");
            //setup moveto and ashow
            content.Append("0 4 -1 roll 4 -1 roll 5 -1 roll\n");
            content.Append("moveto ashow\n");
            content.Append("} def\n");

            content.Append("%%EndProcSet: barcode-procset 1.0\n");
            content.Append("%%EndProlog\n");
        }

        internal void AddEnd()
        {
            content.Append("%%EOF\n");
        }


        internal void AddDef(BitMatrix matrix)
        {
            content.Append("/bits [");
            for (int y = matrix.Height - 1; y >= 0; --y)
            {
                for (int x = 0; x < matrix.Width; ++x)
                {
                    content.Append(matrix.Get(x, y) ? "1 " : "0 ");
                }
            }
            content.Append("] def\n");

            content.Append("/width " + matrix.Width + " def\n");
            content.Append("/height " + matrix.Height + " def\n");

            content.Append(
                    "/y 0 def\n" +
                    1 + " " + 1 + " scale\n" +
                    "height {\n" +
                    "   /x 0 def\n" +
                    "   width {\n" +
                    "      bits y width mul x add get 1 eq {\n" +
                    "         newpath\n" +
                    "         x y moveto\n" +
                    "         0 1 rlineto\n" +
                    "         1 0 rlineto\n" +
                    "         0 -1 rlineto\n" +
                    "         closepath\n" +
                    "         fill\n" +
                    "      } if\n" +
                    "      /x x 1 add def\n" +
                    "   } repeat\n" +
                    "   /y y 1 add def\n" +
                    "} repeat\n");
        }

        internal void SetForeground(Color color)
        {
            content.Append( FormatColor(color.R) + " " + FormatColor(color.G) + " " + FormatColor(color.B)  + " setrgbcolor\n");
        }

        internal string FormatColor(byte c)
        {
            return (c / 255.0).ToString("0.###");
        }

    }
}
