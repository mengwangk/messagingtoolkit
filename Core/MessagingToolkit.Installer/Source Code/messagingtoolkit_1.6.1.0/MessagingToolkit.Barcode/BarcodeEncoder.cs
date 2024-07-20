using System;
using System.Collections.Generic;

#if !SILVERLIGHT

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

using System.IO;


using MessagingToolkit.Barcode.OneD;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.QRCode;
using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.DataMatrix;
using MessagingToolkit.Barcode.DataMatrix.Encoder;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.Pdf417;
using MessagingToolkit.Barcode.Pdf417.Encoder;



namespace MessagingToolkit.Barcode
{
    /// <summary> 
    /// This is a factory class which finds the appropriate encoder subclass for the BarcodeFormat
    /// requested and encodes the barcode with the supplied contents.
    /// </summary>
    public sealed class BarcodeEncoder : IDisposable, IBarcodeEncoder
    {
        #region Variables

#if !SILVERLIGHT
        private IBarcode iOneDEncoder = new OneD.Blank();
#else
        private IEncoder iOneDEncoder = new DefaultEncoder();
#endif

        private IEncoder iQRCodeEncoder = new DefaultEncoder();
        private IEncoder iDataMatrixEncoder = new DefaultEncoder();
        private IEncoder iPdf417Encoder = new DefaultEncoder();

        private string content = string.Empty;
        private string encodedValue = "";
        private string countryAssigningManufacturerCode = "N/A";
        private BarcodeFormat barcodeFormat = BarcodeFormat.Unknown;

#if !SILVERLIGHT
        private Image encodedImage = null;
        private Color foreColor = Color.Black;
        private Color backColor = Color.White;
        private ImageFormat imageFormat = ImageFormat.Jpeg;
        private System.Drawing.Font labelFont = new System.Drawing.Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
#else

        private WriteableBitmap encodedImage = null;
        private Color foreColor = Colors.Black;
        private Color backColor = Colors.White;

#endif

        private int width = 300;
        private int height = 150;
        private bool includeLabel = false;
        private double encodingTime = 0;
        private DataMatrixScheme dataMatrixScheme = DataMatrixScheme.SchemeBase256;
        private DataMatrixSymbolSize dataMatrixSymbolSize = DataMatrixSymbolSize.SymbolSquareAuto;

        private AlignmentPositions alignment = AlignmentPositions.Center;
        private LabelPositions labelPosition = LabelPositions.BottomCenter;

        private ErrorCorrectionLevel errorCorrectionLevel = ErrorCorrectionLevel.L;
        private string characterSet = MessagingToolkit.Barcode.QRCode.Encoder.Encoder.DefaultByteModeEncoding;
        private int margin = -1;
        private int moduleSize = 5;
        private int marginSize = 10;

        private Compaction compaction = Compaction.Auto;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor.  Does not populate the raw data.  MUST be done via the Content property before encoding.
        /// </summary>
        public BarcodeEncoder()
        {
            this.ExtraOptions = new Dictionary<EncodeOptions, object>(1);
        }

        /// <summary>
        /// Constructor. Populates the raw data. No whitespace will be added before or after the barcode.
        /// </summary>
        /// <param name="data">String to be encoded.</param>
        public BarcodeEncoder(string data)
            : this()
        {
            this.content = data;
        }

        public BarcodeEncoder(string data, BarcodeFormat barcodeFormat)
            : this()
        {
            this.content = data;
            this.barcodeFormat = barcodeFormat;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the raw data to encode.
        /// </summary>
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        /// <summary>
        /// Gets the encoded value.
        /// </summary>
        private string EncodedValue
        {
            get
            {
                return encodedValue;
            }
        }

        /// <summary>
        /// Gets the Country that assigned the Manufacturer Code.
        /// </summary>
        public string CountryAssigningManufacturerCode
        {
            get
            {
                return countryAssigningManufacturerCode;
            }
        }

        /// <summary>
        /// Gets or sets the Encoded Type (ex. UPC-A, EAN-13 ... etc)
        /// </summary>
        public BarcodeFormat EncodedBarcodeFormat
        {
            set
            {
                barcodeFormat = value;
            }
            get
            {
                return barcodeFormat;
            }
        }

        /// <summary>
        /// Gets the Image of the generated barcode.
        /// </summary>
#if !SILVERLIGHT
        public Image EncodedImage
#else
        public WriteableBitmap EncodedImage
#endif
        {
            get
            {
                return encodedImage;
            }
        }

        /// <summary>
        /// Gets or sets the color of the bars. (Default is black)
        /// </summary>
        public Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
            set
            {
                this.foreColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color. (Default is white)
        /// </summary>
        public Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                this.backColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the extra options for encoding
        /// </summary>
        /// <value>
        /// The extra encoding options
        /// </value>
        public Dictionary<EncodeOptions, object> ExtraOptions
        {
            get;
            internal set;
        }

        /// <summary>
        /// Adds additional encoding  option.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOption(EncodeOptions key, object value)
        {
            if (!ExtraOptions.ContainsKey(key))
            {
                ExtraOptions.Add(key, value);
            }
            else
            {
                ExtraOptions[key] = value;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the label font. (Default is Microsoft Sans Serif, 10pt, Bold)
        /// </summary>
        public System.Drawing.Font LabelFont
        {
            get
            {
                return this.labelFont;
            }
            set
            {
                this.labelFont = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the location of the label in relation to the barcode. (BOTTOMCENTER is default)
        /// </summary>
        public LabelPositions LabelPosition
        {
            get
            {
                return labelPosition;
            }
            set
            {
                labelPosition = value;
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the degree in which to rotate/flip the image.(No action is default)
        /// </summary>
        public RotateFlipType RotateFlipType
        {
            get
            {
                return rotateFlipType;
            }
            set
            {
                rotateFlipType = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the width of the image to be drawn. (Default is 300 pixels)
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the image to be drawn. (Default is 150 pixels)
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        /// Gets or sets whether a label should be drawn below the image.
        /// </summary>
        public bool IncludeLabel
        {
            set
            {
                this.includeLabel = value;
            }
            get
            {
                return this.includeLabel;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time in milliseconds that it took to encode and draw the barcode.
        /// </summary>
        public double EncodingTime
        {
            get
            {
                return encodingTime;
            }
            set
            {
                encodingTime = value;
            }
        }

        /// <summary>
        /// Gets the XML representation of the Barcode data and image.
        /// </summary>
        /**
        public string XML
        {
            get
            {
                return xml;
            }
        }
        */

#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the image format to use when encoding and returning images. (Jpeg is default)
        /// </summary>
        public ImageFormat ImageFormat
        {
            get
            {
                return imageFormat;
            }
            set
            {
                imageFormat = value;
            }
        }

        /// <summary>
        /// Gets the list of errors encountered.
        /// </summary>
        public List<string> Errors
        {
            get
            {
                return this.iOneDEncoder.Errors;
            }
        }
#endif


        /// <summary>
        /// Gets or sets the alignment of the barcode inside the image. (Not for Postnet or ITF-14)
        /// </summary>
        public AlignmentPositions Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                alignment = value;
            }
        }


        /// <summary>
        /// Gets or sets the error correction level.
        /// </summary>
        /// <value>
        /// The error correction level.
        /// </value>
        public ErrorCorrectionLevel ErrorCorrectionLevel
        {
            get
            {
                return this.errorCorrectionLevel;
            }
            set
            {
                this.errorCorrectionLevel = value;
            }
        }


        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public int Margin
        {
            get
            {
                return this.margin;
            }
            set
            {
                this.margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the character set.
        /// </summary>
        /// <value>
        /// The character set.
        /// </value>
        public string CharacterSet
        {
            get
            {
                return this.characterSet;
            }
            set
            {
                this.characterSet = value;
            }
        }

        /// <summary>
        /// Gets or sets the module size of a Data Matrix barcode
        /// </summary>
        /// <value>
        /// The module size
        /// </value>
        public int ModuleSize
        {
            get
            {
                return this.moduleSize;
            }
            set
            {
                this.moduleSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the margin size of a Data Matrix barcode
        /// </summary>
        /// <value>
        /// The margin size
        /// </value>
        public int MarginSize
        {
            get
            {
                return this.marginSize;
            }
            set
            {
                this.marginSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the data matrix scheme.
        /// </summary>
        /// <value>
        /// The data matrix scheme.
        /// </value>
        public DataMatrixScheme DataMatrixScheme
        {
            get
            {
                return this.dataMatrixScheme;
            }
            set
            {
                this.dataMatrixScheme = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the data matrix symbol.
        /// </summary>
        /// <value>
        /// The size of the data matrix symbol.
        /// </value>
        public DataMatrixSymbolSize DataMatrixSymbolSize
        {
            get
            {
                return this.dataMatrixSymbolSize;
            }
            set
            {
                this.dataMatrixSymbolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the PDF417 compaction.
        /// </summary>
        /// <value>
        /// The PDF417 compaction.
        /// </value>
        public Compaction Pdf417Compaction
        {
            get
            {
                return this.compaction;
            }
            set
            {
                this.compaction = value;
            }
        }

        /// <summary>
        /// Gets the assembly version information.
        /// </summary>
        public static System.Version Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        #endregion

        #region Functions


        #region General Encode



        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public Image Encode(BarcodeFormat format, string content)
#else
        public WriteableBitmap Encode(BarcodeFormat format, string content)
#endif
        {
            this.content = content;
            return Encode(format);
        }


        /***
        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height)
        {

            return Encode(contents, format, width, height, null);
        }

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {

            IBarcodeEncoder encoder;
            if (format == MessagingToolkit.Barcode.BarcodeFormat.EAN8)
            {
                encoder = new EAN8Encoder();
            }
            else if (format == MessagingToolkit.Barcode.BarcodeFormat.EAN13)
            {
                encoder = new EAN13Encoder();
            }
            else if (format == MessagingToolkit.Barcode.BarcodeFormat.UPCA)
            {
                encoder = new UPCAEncoder();
            }
            else if (format == MessagingToolkit.Barcode.BarcodeFormat.QRCode)
            {
                encoder = new QRCodeEncoder();
            }
            else if (format == MessagingToolkit.Barcode.BarcodeFormat.Code39)
            {
                encoder = new Code39Encoder();
            }
            else if (format == MessagingToolkit.Barcode.BarcodeFormat.Code128)
            {
                encoder = new Code128Encoder();
            }
            else if (format == MessagingToolkit.Barcode.BarcodeFormat.ITF14)
            {
                encoder = new ITFEncoder();
            }
            else
            {
                throw new ArgumentException(
                        "No encoder available for format " + format);
            }
            return encoder.Encode(contents, format, width, height, encodingOptions);
        }
       
        ***/

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="barcodeFormat">Type of encoding to use.</param>
#if !SILVERLIGHT
        internal Image Encode(BarcodeFormat barcodeFormat)
#else
        internal WriteableBitmap Encode(BarcodeFormat barcodeFormat)
#endif
        {
            this.barcodeFormat = barcodeFormat;
            return Encode();
        }

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.
        /// </summary>
#if !SILVERLIGHT
        internal Image Encode()
#else
        internal WriteableBitmap Encode()
#endif
        {
#if !SILVERLIGHT
            iOneDEncoder = new OneD.Blank();
            iOneDEncoder.Errors.Clear();
#else
            iOneDEncoder = new DefaultEncoder();
#endif

            iQRCodeEncoder = new DefaultEncoder();
            iDataMatrixEncoder = new DefaultEncoder();

            DateTime dtStartTime = DateTime.Now;

            // make sure there is something to encode
            if (content.Trim() == string.Empty)
                throw new Exception("Input data not allowed to be blank.");

            if (this.EncodedBarcodeFormat == BarcodeFormat.Unknown)
                throw new Exception("Symbology type not allowed to be unspecified.");

            this.encodedValue = string.Empty;
            this.countryAssigningManufacturerCode = "N/A";

            switch (this.barcodeFormat)
            {
                case BarcodeFormat.QRCode:
                    iQRCodeEncoder = new QRCodeEncoder();
                    break;
                case BarcodeFormat.DataMatrix:
                    iDataMatrixEncoder = new DataMatrixEncoder();
                    break;
                case BarcodeFormat.PDF417:
                    iPdf417Encoder = new Pdf417Encoder();
                    break;


#if !SILVERLIGHT
                case BarcodeFormat.USD8:
                case BarcodeFormat.Code11:
                    iOneDEncoder = new Code11(content);
                    break;
                case BarcodeFormat.EAN8:
                    iOneDEncoder = new EAN8(content);
                    break;
                case BarcodeFormat.PostNet:
                    iOneDEncoder = new Postnet(content);
                    break;
                case BarcodeFormat.ISBN:
                case BarcodeFormat.Bookland:
                    iOneDEncoder = new ISBN(content);
                    break;
                case BarcodeFormat.JAN13:
                    iOneDEncoder = new JAN13(content);
                    break;
                case BarcodeFormat.UPCSupplemental2Digit:
                    iOneDEncoder = new UPCSupplement2(content);
                    break;
                case BarcodeFormat.Codabar:
                    iOneDEncoder = new Codabar(content);
                    break;
                case BarcodeFormat.Code39Extended:
                    iOneDEncoder = new Code39(content, true);
                    break;
                case BarcodeFormat.LOGMARS:
                case BarcodeFormat.Code39:
                    iOneDEncoder = new Code39(content);
                    break;
                case BarcodeFormat.Interleaved2of5:
                    iOneDEncoder = new Interleaved2of5(content);
                    break;
                case BarcodeFormat.Industrial2of5:
                case BarcodeFormat.Standard2of5:
                    iOneDEncoder = new Standard2of5(content);
                    break;
                case BarcodeFormat.UCC13:
                case BarcodeFormat.EAN13:
                    iOneDEncoder = new EAN13(content);
                    break;
                case BarcodeFormat.UCC12:
                case BarcodeFormat.UPCA:
                    iOneDEncoder = new UPCA(content);
                    break;
                case BarcodeFormat.MSIMod10:
                case BarcodeFormat.MSI2Mod10:
                case BarcodeFormat.MSIMod11:
                case BarcodeFormat.MSIMod11Mod10:
                case BarcodeFormat.ModifiedPlessey:
                    iOneDEncoder = new MSI(content, barcodeFormat);
                    break;
                case BarcodeFormat.UPCSupplemental5Digit:
                    iOneDEncoder = new UPCSupplement5(content);
                    break;
                case BarcodeFormat.UPCE:
                    iOneDEncoder = new UPCE(content);
                    break;
                case BarcodeFormat.ITF14:
                    iOneDEncoder = new ITF14(content);
                    break;
                case BarcodeFormat.Code93:
                    iOneDEncoder = new Code93(content);
                    break;
                case BarcodeFormat.Telepen:
                    iOneDEncoder = new Telepen(content);
                    break;
                case BarcodeFormat.FIM:
                    iOneDEncoder = new FIM(content);
                    break;
                case BarcodeFormat.Code128:
                    iOneDEncoder = new Code128(content);
                    break;
                case BarcodeFormat.Code128A:
                    iOneDEncoder = new Code128(content, Code128.Types.A);
                    break;
                case BarcodeFormat.Code128B:
                    iOneDEncoder = new Code128(content, Code128.Types.B);
                    break;
                case BarcodeFormat.Code128C:
                    iOneDEncoder = new Code128(content, Code128.Types.C);
                    break;
#else
                case BarcodeFormat.EAN8:
                    iOneDEncoder = new EAN8Encoder();
                    break;
                case BarcodeFormat.Codabar:
                    iOneDEncoder = new CodaBarEncoder();
                    break;
                case BarcodeFormat.Code39Extended:
                    iOneDEncoder = new Code39Encoder();
                    break;
                case BarcodeFormat.LOGMARS:
                case BarcodeFormat.Code39:
                    iOneDEncoder = new Code39Encoder();
                    break;
                case BarcodeFormat.UCC13:
                case BarcodeFormat.EAN13:
                    iOneDEncoder = new EAN13Encoder();
                    break;
                case BarcodeFormat.UCC12:
                case BarcodeFormat.UPCA:
                    iOneDEncoder = new UPCAEncoder();
                    break;
                case BarcodeFormat.ITF14:
                    iOneDEncoder = new ITFEncoder();
                    break;
                case BarcodeFormat.Code128:
                    iOneDEncoder = new Code128Encoder();
                    break;
                case BarcodeFormat.Code128A:
                    iOneDEncoder = new Code128Encoder();
                    break;
                case BarcodeFormat.Code128B:
                    iOneDEncoder = new Code128Encoder();
                    break;
                case BarcodeFormat.Code128C:
                    iOneDEncoder = new Code128Encoder();
                    break;
#endif
                default: throw new Exception("Unsupported encoding type specified.");
            }

#if !SILVERLIGHT
            if (iOneDEncoder.GetType() != typeof(OneD.Blank))
            {
                this.encodedValue = iOneDEncoder.EncodedValue;
                this.content = iOneDEncoder.RawData;
                encodedImage = (Image)GenerateImage();
            }
#else
            if (iOneDEncoder.GetType() != typeof(DefaultEncoder))
            {
                BitMatrix bitMatrix = iOneDEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, this.ExtraOptions);
                encodedImage = MatrixToImageHelper.ToBitmap(bitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = bitMatrix.ToString();
            }
#endif
            else if (iQRCodeEncoder.GetType() != typeof(DefaultEncoder))
            {
                Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.ExtraOptions);
                if (!encodeOptions.ContainsKey(EncodeOptions.CharacterSet))
                    encodeOptions.Add(EncodeOptions.CharacterSet, this.CharacterSet);
                if (!encodeOptions.ContainsKey(EncodeOptions.ErrorCorrection))
                    encodeOptions.Add(EncodeOptions.ErrorCorrection, this.ErrorCorrectionLevel);

                if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                {
                    if (this.Margin > 0)
                    {
                        encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                    }
                }
            

                BitMatrix bitMatrix = iQRCodeEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                encodedImage = MatrixToImageHelper.ToBitmap(bitMatrix, this.ForeColor, this.BackColor);

                // Overlay logo on the image if any
                object data = BarcodeHelper.GetEncodeOptionType(encodeOptions, EncodeOptions.QRCodeLogo);
                if (data != null)
                {
#if !SILVERLIGHT
                    byte[] imageData = (byte[])data;
                    Image logo = BarcodeHelper.ByteArrayToImage(imageData);

                    int left = (encodedImage.Width / 2) - (logo.Width / 2);
                    int top = (encodedImage.Height / 2) - (logo.Height / 2);

                    Graphics g = Graphics.FromImage(encodedImage);
                    g.DrawImage(logo, new Point(left, top));
#else
                    byte[] imageData = (byte[])data;
                    WriteableBitmap logo = BarcodeHelper.ByteArrayToImage(imageData);

                    int left = (encodedImage.PixelWidth / 2) - (logo.PixelWidth / 2);
                    int top = (encodedImage.PixelHeight / 2) - (logo.PixelHeight / 2);
                    WriteableBitmapExtensions.Blit(encodedImage, new Rect(new Point(left, top), new Size(logo.PixelWidth, logo.PixelHeight)), logo, new Rect(new Point(0, 0), new Size(logo.PixelWidth, logo.PixelHeight)));
#endif
                }
                this.encodedValue = bitMatrix.ToString();
            }
            else if (iDataMatrixEncoder.GetType() != typeof(DefaultEncoder))
            {
                DataMatrixEncoder dataMatrixEncoder = iDataMatrixEncoder as DataMatrixEncoder;
                DataMatrixImageEncoderOptions options = new DataMatrixImageEncoderOptions();
                options.BackColor = this.BackColor;
                options.ForeColor = this.ForeColor;
                options.MarginSize = this.MarginSize;
                options.ModuleSize = this.ModuleSize;
                options.Scheme = this.DataMatrixScheme;
                options.SizeIdx = this.DataMatrixSymbolSize;
                options.CharacterSet = this.CharacterSet;
                this.encodedImage = dataMatrixEncoder.EncodeImage(this.content, options);
            }
            else if (iPdf417Encoder.GetType() != typeof(DefaultEncoder))
            {
                Pdf417Encoder pdf417Encoder = iPdf417Encoder as Pdf417Encoder;
                Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.ExtraOptions);
                
                if (!encodeOptions.ContainsKey(EncodeOptions.Pdf417Compaction))
                    encodeOptions.Add(EncodeOptions.Pdf417Compaction, this.Pdf417Compaction);
                
                BitMatrix bitMatrix = pdf417Encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                encodedImage = MatrixToImageHelper.ToBitmap(bitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = bitMatrix.ToString();
            }

#if !SILVERLIGHT
            this.EncodedImage.RotateFlip(this.RotateFlipType);
            this.encodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;
            return EncodedImage;
#else
            this.encodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;
            return EncodedImage;
#endif
        }

        #endregion

        #region Image Functions


#if !SILVERLIGHT
        /// <summary>
        /// Gets a bitmap representation of the encoded data.
        /// </summary>
        /// <returns>Bitmap of encoded value.</returns>
        private Bitmap GenerateImage()
        {
            if (encodedValue == "") throw new Exception("Must be encoded first.");
            Bitmap b = null;

            DateTime dtStartTime = DateTime.Now;

            switch (this.barcodeFormat)
            {
                case BarcodeFormat.ITF14:
                    {
                        b = new Bitmap(Width, Height);

                        int bearerwidth = (int)((b.Width) / 12.05);
                        int iquietzone = Convert.ToInt32(b.Width * 0.05);
                        if (this.margin >= 0) iquietzone = this.margin;
                        int iBarWidth = (b.Width - (bearerwidth * 2) - (iquietzone * 2)) / encodedValue.Length;
                        int shiftAdjustment = ((b.Width - (bearerwidth * 2) - (iquietzone * 2)) % encodedValue.Length) / 2;

                        if (iBarWidth <= 0 || iquietzone <= 0)
                            throw new Exception("Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");

                        //draw image
                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(b))
                        {
                            // fill background
                            g.Clear(BackColor);

                            // lines are fBarWidth wide so draw the appropriate color line vertically
                            using (Pen pen = new Pen(ForeColor, iBarWidth))
                            {
                                pen.Alignment = PenAlignment.Right;

                                while (pos < encodedValue.Length)
                                {
                                    //draw the appropriate color line vertically
                                    if (encodedValue[pos] == '1')
                                        g.DrawLine(pen, new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height));

                                    pos++;
                                }

                                // bearer bars
                                pen.Width = (float)b.Height / 8;
                                pen.Color = ForeColor;
                                pen.Alignment = PenAlignment.Center;
                                g.DrawLine(pen, new Point(0, 0), new Point(b.Width, 0));//top
                                g.DrawLine(pen, new Point(0, b.Height), new Point(b.Width, b.Height));//bottom
                                g.DrawLine(pen, new Point(0, 0), new Point(0, b.Height));//left
                                g.DrawLine(pen, new Point(b.Width, 0), new Point(b.Width, b.Height));//right
                            }
                        }

                        if (IncludeLabel)
                            LabelITF14((Image)b);

                        break;
                    }//case
                default:
                    {
                        b = new Bitmap(Width, Height);
                        //int iBarWidth = Width / encodedValue.Length;
                        //if (iBarWidth == 0) iBarWidth = 1;

                        int shiftAdjustment = 0;
                        int iBarWidthModifier = 1;

                        int iquietzone = Convert.ToInt32(b.Width * 0.05);
                        if (this.margin >= 0) iquietzone = this.margin;
                        int iBarWidth = (b.Width - (iquietzone * 2)) / encodedValue.Length;
                        if (iBarWidth == 0) iBarWidth = 1;

                        if (this.barcodeFormat == BarcodeFormat.PostNet)
                            iBarWidthModifier = 2;

                        //set alignment
                        switch (Alignment)
                        {
                            case AlignmentPositions.Center:
                                shiftAdjustment = ((Width - (iquietzone * 2)) % encodedValue.Length) / 2;
                                break;
                            case AlignmentPositions.Left: shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.Right: shiftAdjustment = ((Width - (iquietzone * 2)) % encodedValue.Length);
                                break;
                            default: shiftAdjustment = ((Width - (iquietzone * 2)) % encodedValue.Length) / 2;
                                break;
                        }//switch

                        if (iBarWidth <= 0)
                            throw new Exception("Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                        //draw image
                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(b))
                        {
                            //clears the image and colors the entire background
                            g.Clear(BackColor);

                            //lines are fBarWidth wide so draw the appropriate color line vertically
                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < encodedValue.Length)
                                    {
                                        if (this.barcodeFormat == BarcodeFormat.PostNet)
                                        {
                                            //draw half bars in postnet
                                            if (encodedValue[pos] != '1')
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + 1, Height), new Point(pos * iBarWidth + shiftAdjustment + 1, Height / 2));

                                            //draw spaces between bars in postnet
                                            g.DrawLine(backpen, new Point(pos * (iBarWidth * iBarWidthModifier) + shiftAdjustment + iBarWidth + 1, 0), new Point(pos * (iBarWidth * iBarWidthModifier) + shiftAdjustment + iBarWidth + 1, Height));
                                        }//if

                                        if (encodedValue[pos] == '1')
                                            //g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + (int)(iBarWidth * 0.5), 0), new Point(pos * iBarWidth + shiftAdjustment + (int)(iBarWidth * 0.5), Height));
                                            g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + iquietzone, iquietzone), new Point(pos * iBarWidth + shiftAdjustment + iquietzone, Height - iquietzone));

                                        pos++;
                                    }//while
                                }//using
                            }//using
                        }//using
                        if (IncludeLabel)
                        {
                            //if (this.EncodedType != TYPE.UPCA)
                            LabelGeneric((Image)b, iquietzone);
                            //else
                            //    Label_UPCA((Image)b);
                        }//if

                        break;
                    }//case
            }//switch

            encodedImage = (Image)b;

            this.encodingTime += ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            return b;
        }

#endif

#if !SILVERLIGHT

        /// <summary>
        /// Gets the bytes that represent the image.
        /// </summary>
        /// <param name="savetype">File type to put the data in before returning the bytes.</param>
        /// <returns>Bytes representing the encoded image.</returns>
        public byte[] GetImageData(SaveOptions savetype)
        {
            byte[] imageData = null;

            try
            {
                if (encodedImage != null)
                {
                    //Save the image to a memory stream so that we can get a byte array!      
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SaveImage(ms, savetype);
                        imageData = ms.ToArray();
                        ms.Flush();
                        ms.Close();
                    }//using
                }//if
            }//try
            catch (Exception ex)
            {
                throw new Exception("Could not retrieve image data. " + ex.Message);
            }//catch  
            return imageData;
        }

        /// <summary>
        /// Saves an encoded image to a specified file and type.
        /// </summary>
        /// <param name="Filename">Filename to save to.</param>
        /// <param name="FileType">Format to use.</param>
        public void SaveImage(string Filename, SaveOptions FileType)
        {
            try
            {
                if (encodedImage != null)
                {
                    System.Drawing.Imaging.ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveOptions.Bmp: imageformat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case SaveOptions.Gif: imageformat = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case SaveOptions.Jpg: imageformat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case SaveOptions.Png: imageformat = System.Drawing.Imaging.ImageFormat.Png; break;
                        case SaveOptions.Tiff: imageformat = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }//switch
                    ((Bitmap)encodedImage).Save(Filename, imageformat);
                }//if
            }//try
            catch (Exception ex)
            {
                throw new Exception("Could not save image.\n\n=======================\n\n" + ex.Message);
            }//catch
        }//SaveImage(string, SaveTypes)

        /// <summary>
        /// Saves an encoded image to a specified stream.
        /// </summary>
        /// <param name="stream">Stream to write image to.</param>
        /// <param name="FileType">Format to use.</param>
        public void SaveImage(Stream stream, SaveOptions FileType)
        {
            try
            {
                if (encodedImage != null)
                {
                    System.Drawing.Imaging.ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveOptions.Bmp: imageformat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case SaveOptions.Gif: imageformat = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case SaveOptions.Jpg: imageformat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case SaveOptions.Png: imageformat = System.Drawing.Imaging.ImageFormat.Png; break;
                        case SaveOptions.Tiff: imageformat = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }//switch
                    ((Bitmap)encodedImage).Save(stream, imageformat);
                }//if
            }//try
            catch (Exception ex)
            {
                throw new Exception("Could not save image.\n\n=======================\n\n" + ex.Message);
            }//catch
        }



        /// <summary>
        /// Returns the size of the EncodedImage in real world coordinates (millimeters or inches).
        /// </summary>
        /// <param name="width">Width of the image in the specified coordinates.</param>
        /// <param name="height">Height of the image in the specified coordinates.</param>
        /// <param name="metric">Millimeters if true, otherwise Inches.</param>
        /// <returns></returns>
        public void GetSizeOfImage(ref double width, ref double height, bool metric)
        {
            width = 0;
            height = 0;
            if (this.EncodedImage != null && this.EncodedImage.Width > 0 && this.EncodedImage.Height > 0)
            {
                double MillimetersPerInch = 25.4;
                using (Graphics g = Graphics.FromImage(this.EncodedImage))
                {
                    width = this.EncodedImage.Width / g.DpiX;
                    height = this.EncodedImage.Height / g.DpiY;

                    if (metric)
                    {
                        width *= MillimetersPerInch;
                        height *= MillimetersPerInch;
                    }//if
                }//using
            }//if
        }
#endif

        #endregion

        #region Label Generation

#if !SILVERLIGHT
        private Image LabelITF14(Image img)
        {
            try
            {
                System.Drawing.Font font = this.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    //color a white box at the bottom of the barcode to hold the string of data
                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, img.Height - (font.Height - 2), img.Width, font.Height));

                    //draw datastring under the barcode image
                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    g.DrawString(this.Content, font, new SolidBrush(this.ForeColor), (float)(img.Width / 2), img.Height - font.Height + 1, f);

                    Pen pen = new Pen(ForeColor, (float)img.Height / 16);
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawLine(pen, new Point(0, img.Height - font.Height - 2), new Point(img.Width, img.Height - font.Height - 2));//bottom

                    g.Save();
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }//catch
        }

        private Image LabelGeneric(Image img, int iquietzone)
        {
            try
            {
                System.Drawing.Font font = this.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Near;
                    f.LineAlignment = StringAlignment.Near;
                    int LabelX = 0;
                    int LabelY = 0;

                    switch (LabelPosition)
                    {
                        case LabelPositions.BottomCenter:
                            LabelX = img.Width / 2;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Center;
                            break;
                        case LabelPositions.BottomLeft:
                            LabelX = 0;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Near;
                            break;
                        case LabelPositions.BottomRight:
                            LabelX = img.Width;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Far;
                            break;
                        case LabelPositions.TopCenter:
                            LabelX = img.Width / 2;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Center;
                            break;
                        case LabelPositions.TopLeft:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Near;
                            break;
                        case LabelPositions.TopRight:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Far;
                            break;
                    }//switch

                    //color a background color box at the bottom of the barcode to hold the string of data
                    g.FillRectangle(new SolidBrush(this.BackColor), new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height));

                    //draw datastring under the barcode image
                    g.DrawString(this.Content, font, new SolidBrush(this.ForeColor), new RectangleF((float)0, (float)LabelY, (float)(img.Width - iquietzone), (float)font.Height), f);

                    g.Save();
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }//catch
        }//Label_Generic

        /// <summary>
        /// Draws Label for UPC-A barcodes (NOT COMPLETE)
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private Image LabelUPCA(Image img)
        {
            try
            {
                int iBarWidth = Width / encodedValue.Length;
                int shiftAdjustment = 0;

                //set alignment
                switch (Alignment)
                {
                    case AlignmentPositions.Center: shiftAdjustment = (Width % encodedValue.Length) / 2;
                        break;
                    case AlignmentPositions.Left: shiftAdjustment = 0;
                        break;
                    case AlignmentPositions.Right: shiftAdjustment = (Width % encodedValue.Length);
                        break;
                    default: shiftAdjustment = (Width % encodedValue.Length) / 2;
                        break;
                }//switch

                System.Drawing.Font font = new System.Drawing.Font("OCR A Extended", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0))); ;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    //draw datastring under the barcode image
                    RectangleF rect = new RectangleF((iBarWidth * 3) + shiftAdjustment, this.Height - (int)(this.Height * 0.1), (iBarWidth * 43), (int)(this.Height * 0.1));
                    g.FillRectangle(new SolidBrush(Color.Yellow), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(this.Content.Substring(1, 5), font, new SolidBrush(this.ForeColor), rect.X, rect.Y);

                    g.Save();
                }//using
                return img;
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }//catch
        }//Label_UPCA
#endif

        #endregion

        #endregion

        #region Misc
        internal static bool CheckNumericOnly(string data)
        {
            //This function takes a string of data and breaks it into parts and trys to do Int64.TryParse
            //This will verify that only numeric data is contained in the string passed in.  The complexity below
            //was done to ensure that the minimum number of interations and checks could be performed.

            //9223372036854775808 is the largest number a 64bit number(signed) can hold so ... make sure its less than that by one place
            int StringLengths = 18;

            string temp = data;
            string[] strings = new string[(data.Length / StringLengths) + ((data.Length % StringLengths == 0) ? 0 : 1)];

            int i = 0;
            while (i < strings.Length)
                if (temp.Length >= StringLengths)
                {
                    strings[i++] = temp.Substring(0, StringLengths);
                    temp = temp.Substring(StringLengths);
                }//if
                else
                    strings[i++] = temp.Substring(0);

            foreach (string s in strings)
            {
                long value = 0;
                if (!Int64.TryParse(s, out value))
                    return false;
            }

            return true;
        }

        /*
        private string GetXML()
        {
            if (EncodedValue == "")
                throw new Exception("Could not retrieve XML due to the barcode not being encoded first.  Please call Encode first.");
            else
            {
                try
                {
                    using (BarcodeXML xml = new BarcodeXML())
                    {
                        BarcodeXML.BarcodeRow row = xml.Barcode.NewBarcodeRow();
                        row.Type = EncodedBarcodeFormat.ToString();
                        row.RawData = Content;
                        row.EncodedValue = EncodedValue;
                        row.EncodingTime = EncodingTime;
                        row.IncludeLabel = IncludeLabel;
                        row.Forecolor = ColorTranslator.ToHtml(ForeColor);
                        row.Backcolor = ColorTranslator.ToHtml(BackColor);
                        row.CountryAssigningManufacturingCode = CountryAssigningManufacturerCode;
                        row.ImageWidth = Width;
                        row.ImageHeight = Height;
                        row.RotateFlipType = this.RotateFlipType;
                        row.LabelPosition = this.LabelPosition;
                        row.LabelFont = this.LabelFont.ToString();
                        row.ImageFormat = this.ImageFormat.ToString();
                        row.Alignment = this.Alignment;

                        //get image in base 64
                        using (MemoryStream ms = new MemoryStream())
                        {
                            EncodedImage.Save(ms, ImageFormat);
                            row.Image = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None);
                        }//using

                        xml.Barcode.AddBarcodeRow(row);

                        StringWriter sw = new StringWriter();
                        xml.WriteXml(sw, XmlWriteMode.WriteSchema);
                        return sw.ToString();
                    }//using
                }//try
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }//catch
            }//else
        }
        */

        /*
        public static Image GetImageFromXML(BarcodeXML internalXML)
        {
            try
            {
                //converting the base64 string to byte array
                Byte[] imageContent = new Byte[internalXML.Barcode[0].Image.Length];

                //loading it to memory stream and then to image object
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(internalXML.Barcode[0].Image)))
                {
                    return Image.FromStream(ms);
                }//using
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }//catch
        }
        */
        #endregion

        #region Static Methods
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
                return b.Encode(format, data);
            }
        }
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="xml">XML representation of the data and the image of the barcode.</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data, ref string xml)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, ref string xml)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
#if !SILVERLIGHT
                Image i = b.Encode(format, data);
#else
                WriteableBitmap i = b.Encode(format, data);
#endif
                return i;
            }
        }
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
                b.IncludeLabel = includeLabel;
                return b.Encode(format, data);
            }
        }
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="width">Width of the resulting barcode.(pixels)</param>
        /// <param name="height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel, int width, int height)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, int width, int height)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
                b.IncludeLabel = includeLabel;
                b.Width = width;
                b.Height = height;
                return b.Encode(format, data);
            }
        }
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="DrawColor">Foreground color</param>
        /// <param name="BackColor">Background color</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
                b.IncludeLabel = includeLabel;
                b.ForeColor = foreColor;
                b.BackColor = backColor;
                return b.Encode(format, data);
            }
        }
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="drawColor">Foreground color</param>
        /// <param name="backColor">Background color</param>
        /// <param name="width">Width of the resulting barcode.(pixels)</param>
        /// <param name="height">Height of the resulting barcode.(pixels)</param>
        /// <returns>Image representing the barcode.</returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor, int width, int height)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor, int width, int height)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
                b.IncludeLabel = includeLabel;
                b.Width = width;
                b.Height = height;
                b.ForeColor = foreColor;
                b.BackColor = backColor;
                return b.Encode(format, data);
            }
        }
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="data">Raw data to encode.</param>
        /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <param name="foreColor">Color of the fore.</param>
        /// <param name="backColor">Color of the back.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// Image representing the barcode.
        /// </returns>
#if !SILVERLIGHT
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor, int width, int height, ref string xml)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor, int width, int height, ref string xml)
#endif
        {
            using (BarcodeEncoder b = new BarcodeEncoder())
            {
                b.IncludeLabel = includeLabel;
                b.Width = width;
                b.Height = height;
                b.ForeColor = foreColor;
                b.BackColor = backColor;
#if !SILVERLIGHT
                Image i = b.Encode(format, data);
#else
                WriteableBitmap i = b.Encode(format, data);
#endif
                //XML = b.XML;
                return i;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
