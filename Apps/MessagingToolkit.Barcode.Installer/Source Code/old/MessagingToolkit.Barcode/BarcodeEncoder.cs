using System;
using System.Collections.Generic;

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;


#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

#endif

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
using MessagingToolkit.Barcode.Provider;



namespace MessagingToolkit.Barcode
{
    /// <summary> 
    /// This is a factory class which finds the appropriate encoder subclass for the BarcodeFormat
    /// requested and encodes the barcode with the supplied contents.
    /// </summary>
    public sealed class BarcodeEncoder : IDisposable, IBarcodeEncoder
    {
        #region Variables

#if (!SILVERLIGHT && !NETFX_CORE)
        private IBarcode iOneDEncoder = new OneD.Blank();
#else

#if WPF
        private IBarcode iOneDEncoder = new OneD.Blank();
#else
        private IEncoder iOneDEncoder = new DefaultEncoder();
#endif

#endif

        private IEncoder iQRCodeEncoder = new DefaultEncoder();
        private IEncoder iDataMatrixEncoder = new DefaultEncoder();
        private IEncoder iPdf417Encoder = new DefaultEncoder();

        private string content = string.Empty;
        private string encodedValue = "";
        private string countryAssigningManufacturerCode = "N/A";
        private BarcodeFormat barcodeFormat = BarcodeFormat.Unknown;

#if !SILVERLIGHT && !NETFX_CORE
        private Image encodedImage = null;
        private Color foreColor = Color.Black;
        private Color backColor = Color.White;
        private ImageFormat imageFormat = ImageFormat.Jpeg;
        private System.Drawing.Font labelFont = new System.Drawing.Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
#else

#if WPF
        private ImageFormat imageFormat = ImageFormat.Jpeg;
        private System.Drawing.Font labelFont = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
        private RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
#endif

#if !NETFX_CORE

        private WriteableBitmap encodedImage = null;
        private System.Windows.Media.Color foreColor = Colors.Black;
        private System.Windows.Media.Color backColor = Colors.White;
#else

        private WriteableBitmap encodedImage = null;
        private Color foreColor = Colors.Black;
        private Color backColor = Colors.White;

#endif

#endif

        private int width = 300;
        private int height = 150;
        private bool includeLabel = false;
        private string customeLabel = string.Empty;
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
            this.EncodingOptions = new Dictionary<EncodeOptions, object>(1);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeEncoder" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="barcodeFormat">The barcode format.</param>
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
#if !SILVERLIGHT && !NETFX_CORE
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
        /// Bit matrix representation of the encoded barcode.
        /// </summary>
        public BitMatrix BitMatrix { get; private set; }


        /// <summary>
        /// Gets or sets the color of the bars. (Default is black)
        /// </summary>
#if !SILVERLIGHT
        public Color ForeColor
#else
        public System.Windows.Media.Color ForeColor
#endif
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
#if !SILVERLIGHT
        public Color BackColor
#else
        public System.Windows.Media.Color BackColor
#endif
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
        public Dictionary<EncodeOptions, object> EncodingOptions
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
            if (!EncodingOptions.ContainsKey(key))
            {
                EncodingOptions.Add(key, value);
            }
            else
            {
                EncodingOptions[key] = value;
            }
        }

        /// <summary>
        /// Removes the encoding option.
        /// </summary>
        /// <param name="key">The key.</param>
        public void RemoveOption(EncodeOptions key)
        {
            if (!EncodingOptions.ContainsKey(key))
            {
                EncodingOptions.Remove(key);
            }
        }

        /// <summary>
        /// Removes all encoding option.
        /// </summary>
        public void ClearAllOptions()
        {
            EncodingOptions.Clear();
        }


#if (!SILVERLIGHT && !NETFX_CORE) || WPF
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

#if (!SILVERLIGHT && !NETFX_CORE) || WPF

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
        /// Gets or sets the custom label.
        /// </summary>
        /// <value>
        /// The custom label.
        /// </value>
        public string CustomLabel
        {
            get
            {
                return this.customeLabel;
            }
            set
            {
                this.customeLabel = value;
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

#if (!SILVERLIGHT && !NETFX_CORE) || WPF

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
        public System.Version Version
        {
            get
            {
                string version = string.Empty;
                var assemblyName = this.GetType().AssemblyQualifiedName;
                var versionExpression = new System.Text.RegularExpressions.Regex("Version=(?<version>[0-9.]*)");
                var m = versionExpression.Match(assemblyName);
                if (m.Success)
                {
                    version = m.Groups["version"].Value;
                }
                return new System.Version(version);
            }

        }

        #endregion

        #region Functions


        #region General Encode


        /// <summary>
        /// Generates the specified format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <param name="provider">The output provider.</param>
        /// <returns></returns>
        public T Generate<T>(BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions, IOutputProvider<T> provider)
        {
            try
            {
                this.content = content;
                this.barcodeFormat = format;
                if (encodingOptions != null)
                    this.EncodingOptions = encodingOptions;
                IEncoder encoder = new DefaultEncoder();
                DateTime dtStartTime = DateTime.Now;

                // make sure there is something to encode
                if (string.IsNullOrEmpty(content))
                    throw new Exception("Input data not allowed to be blank.");

                if (this.EncodedBarcodeFormat == BarcodeFormat.Unknown)
                    throw new Exception("Barcode type is not specified.");

                this.encodedValue = string.Empty;
                this.countryAssigningManufacturerCode = "N/A";

                switch (this.barcodeFormat)
                {
                    case BarcodeFormat.QRCode:
                        encoder = new QRCodeEncoder();
                        break;
                    case BarcodeFormat.PDF417:
                        encoder = new Pdf417Encoder();
                        break;
                    case BarcodeFormat.DataMatrix:
                        encoder = new DataMatrixNewEncoder();
                        break;
                    case BarcodeFormat.EAN8:
                        encoder = new EAN8Encoder();
                        break;
                    case BarcodeFormat.Codabar:
                        encoder = new CodaBarEncoder();
                        break;
                    case BarcodeFormat.Code39Extended:
                        encoder = new Code39Encoder();
                        break;
                    case BarcodeFormat.LOGMARS:
                    case BarcodeFormat.Code39:
                        encoder = new Code39Encoder();
                        break;
                    case BarcodeFormat.UCC13:
                    case BarcodeFormat.EAN13:
                        encoder = new EAN13Encoder();
                        break;
                    case BarcodeFormat.UCC12:
                    case BarcodeFormat.UPCA:
                        encoder = new UPCAEncoder();
                        break;
                    case BarcodeFormat.ITF14:
                        encoder = new ITFEncoder();
                        break;
                    case BarcodeFormat.Code128:
                        encoder = new Code128Encoder();
                        break;
                    case BarcodeFormat.Code128A:
                        encoder = new Code128Encoder();
                        break;
                    case BarcodeFormat.Code128B:
                        encoder = new Code128Encoder();
                        break;
                    case BarcodeFormat.Code128C:
                        encoder = new Code128Encoder();
                        break;
                    default: throw new Exception("Unsupported encoding type specified.");
                }

                if (this.barcodeFormat == BarcodeFormat.QRCode)
                {
                    Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);
                    if (!encodeOptions.ContainsKey(EncodeOptions.CharacterSet))
                        encodeOptions.Add(EncodeOptions.CharacterSet, this.CharacterSet);
                    if (!encodeOptions.ContainsKey(EncodeOptions.ErrorCorrection))
                        encodeOptions.Add(EncodeOptions.ErrorCorrection, this.ErrorCorrectionLevel);

                    if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                    {
                        if (this.Margin >= 0)
                        {
                            encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                        }
                    }
                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);

                }
                else if (this.barcodeFormat == BarcodeFormat.DataMatrix)
                {
                    if (!this.EncodingOptions.ContainsKey(EncodeOptions.Margin))
                    {
                        if (this.Margin >= 0)
                        {
                            this.EncodingOptions.Add(EncodeOptions.Margin, this.Margin);
                        }
                    }
                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, this.EncodingOptions);
                }
                else if (this.barcodeFormat == BarcodeFormat.PDF417)
                {
                    Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);

                    if (!encodeOptions.ContainsKey(EncodeOptions.Pdf417Compaction))
                        encodeOptions.Add(EncodeOptions.Pdf417Compaction, this.Pdf417Compaction);

                    if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                    {
                        if (this.Margin >= 0)
                        {
                            encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                        }
                    }

                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                }
                else
                {
                    // One dimensional barcode
                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, this.EncodingOptions);
                }
                this.encodedValue = this.BitMatrix.ToString();
                this.encodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;
                return provider.Generate(this.BitMatrix, this.barcodeFormat, content, this.EncodingOptions);
            }
            finally
            {
                this.content = string.Empty;
                this.ClearAllOptions();
            }
        }

        /// <summary>
        /// Generates the specified format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="provider">The output provider.</param>
        /// <returns></returns>
        public T Generate<T>(BarcodeFormat format, string content, IOutputProvider<T> provider)
        {
            return Generate<T>(format, content, null, provider);
        }

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
#if (!SILVERLIGHT && !NETFX_CORE)
        public Image Encode(BarcodeFormat format, string content)
#else
        public WriteableBitmap Encode(BarcodeFormat format, string content)
#endif
        {
            this.content = content;
            return Encode(format);
        }

#if (!SILVERLIGHT && !NETFX_CORE)
        public Image Encode(BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions)
#else
        public WriteableBitmap Encode(BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions)
#endif
        {
            try
            {
                this.content = content;
                if (encodingOptions != null)
                    this.EncodingOptions = encodingOptions;
                return Encode(format);
            }
            finally
            {
                this.content = string.Empty;
                this.ClearAllOptions();
            }
        }

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="barcodeFormat">Type of encoding to use.</param>
#if (!SILVERLIGHT && !NETFX_CORE)
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
#if (!SILVERLIGHT && !NETFX_CORE)
        internal Image Encode()
#else
        internal WriteableBitmap Encode()
#endif
        {
#if (!SILVERLIGHT && !NETFX_CORE)
            iOneDEncoder = new OneD.Blank();
            iOneDEncoder.Errors.Clear();
#else

#if WPF
            iOneDEncoder = new OneD.Blank();
            iOneDEncoder.Errors.Clear();
#else
            iOneDEncoder = new DefaultEncoder();
#endif

#endif
            iQRCodeEncoder = new DefaultEncoder();
            iDataMatrixEncoder = new DefaultEncoder();

            DateTime dtStartTime = DateTime.Now;

            // make sure there is something to encode
            if (string.IsNullOrEmpty(content))
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
                case BarcodeFormat.PDF417:
                    iPdf417Encoder = new Pdf417Encoder();
                    break;


#if (!SILVERLIGHT && !NETFX_CORE) || WPF
                case BarcodeFormat.DataMatrix:
                    iDataMatrixEncoder = new DataMatrixEncoder();
                    //iDataMatrixEncoder = new DataMatrixNewEncoder();
                    break;
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
                case BarcodeFormat.DataMatrix:
                    iDataMatrixEncoder = new DataMatrixNewEncoder();
                    break;
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
                case BarcodeFormat.MSIMod10:
                    iOneDEncoder = new MsiEncoder();
                    break;
                case BarcodeFormat.ModifiedPlessey:
                    iOneDEncoder = new PlesseyEncoder();
                    break;
#endif
                default: throw new Exception("Unsupported encoding type specified.");
            }

#if (!SILVERLIGHT && !NETFX_CORE) || WPF
            if (iOneDEncoder.GetType() != typeof(OneD.Blank))
            {
                this.encodedValue = iOneDEncoder.EncodedValue;
                this.content = iOneDEncoder.RawData;
#if !WPF
                encodedImage = (Image)GenerateImage();
#else
                encodedImage = new WriteableBitmap(BarcodeHelper.ToBitmapSource(GenerateImage()));
#endif
            }
#else
            if (iOneDEncoder.GetType() != typeof(DefaultEncoder))
            {
                this.BitMatrix = iOneDEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, this.EncodingOptions);
                encodedImage = MatrixToImageHelper.ToBitmap(this.BitMatrix , this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix .ToString();
            }
#endif
            else if (iQRCodeEncoder.GetType() != typeof(DefaultEncoder))
            {
                Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);
                if (!encodeOptions.ContainsKey(EncodeOptions.CharacterSet))
                    encodeOptions.Add(EncodeOptions.CharacterSet, this.CharacterSet);
                if (!encodeOptions.ContainsKey(EncodeOptions.ErrorCorrection))
                    encodeOptions.Add(EncodeOptions.ErrorCorrection, this.ErrorCorrectionLevel);

                if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                {
                    if (this.Margin >= 0)
                    {
                        encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                    }
                }
                this.BitMatrix = iQRCodeEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                encodedImage = MatrixToImageHelper.ToBitmap(this.BitMatrix, this.ForeColor, this.BackColor);

                // Overlay logo on the image if any
                object data = BarcodeHelper.GetEncodeOptionType(encodeOptions, EncodeOptions.QRCodeLogo);
                if (data != null)
                {
                    try
                    {
#if (!SILVERLIGHT && !NETFX_CORE)
                        Image logo = (Image)data;

                        int left = (encodedImage.Width / 2) - (logo.Width / 2);
                        int top = (encodedImage.Height / 2) - (logo.Height / 2);

                        Graphics g = Graphics.FromImage(encodedImage);
                        g.DrawImage(logo, new Point(left, top));
#else
                        WriteableBitmap logo = (WriteableBitmap)data;

                        int left = (encodedImage.PixelWidth / 2) - (logo.PixelWidth / 2);
                        int top = (encodedImage.PixelHeight / 2) - (logo.PixelHeight / 2);

#if WPF

                        WriteableBitmapExtensions.Blit(encodedImage, new Rect(left, top, logo.PixelWidth, logo.PixelHeight), BitmapFactory.ConvertToPbgra32Format(logo), new Rect(0, 0, logo.PixelWidth, logo.PixelHeight));
#else

                        WriteableBitmapExtensions.Blit(encodedImage, new Rect(left, top, logo.PixelWidth, logo.PixelHeight), logo, new Rect(0, 0, logo.PixelWidth, logo.PixelHeight));
#endif

#endif
                    }
                    catch (Exception ex)
                    {

                        //Console.Write(ex.Message);
                    }
                }
                this.encodedValue = this.BitMatrix.ToString();
            }
            else if (iDataMatrixEncoder.GetType() != typeof(DefaultEncoder))
            {
                if (iDataMatrixEncoder.GetType() == typeof(DataMatrixEncoder))
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
                else
                {
                    DataMatrixNewEncoder dataMatrixEncoder = iDataMatrixEncoder as DataMatrixNewEncoder;
                    Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);
                    if (!encodeOptions.ContainsKey(EncodeOptions.CharacterSet))
                        encodeOptions.Add(EncodeOptions.CharacterSet, this.CharacterSet);

                    if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                    {
                        if (this.Margin >= 0)
                        {
                            encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                        }
                    }

                    this.BitMatrix = dataMatrixEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                    encodedImage = MatrixToImageHelper.ToBitmap(this.BitMatrix, this.ForeColor, this.BackColor);
                    this.encodedValue = this.BitMatrix.ToString();
                }
            }
            else if (iPdf417Encoder.GetType() != typeof(DefaultEncoder))
            {
                Pdf417Encoder pdf417Encoder = iPdf417Encoder as Pdf417Encoder;
                Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);

                if (!encodeOptions.ContainsKey(EncodeOptions.Pdf417Compaction))
                    encodeOptions.Add(EncodeOptions.Pdf417Compaction, this.Pdf417Compaction);

                if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                {
                    if (this.Margin >= 0)
                    {
                        encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                    }
                }

                this.BitMatrix = pdf417Encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                encodedImage = MatrixToImageHelper.ToBitmap(this.BitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix.ToString();
            }

#if (!SILVERLIGHT && !NETFX_CORE)
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


#if (!SILVERLIGHT && !NETFX_CORE) || WPF
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
#if !WPF
                            // fill background
                            g.Clear(BackColor);
#else
                            // fill background
                            g.Clear(BarcodeHelper.ToWinFormsColor(BackColor));
#endif


#if !WPF
                            // lines are fBarWidth wide so draw the appropriate color line vertically
                            using (System.Drawing.Pen pen = new System.Drawing.Pen(ForeColor, iBarWidth))
#else
                            using (System.Drawing.Pen pen = new System.Drawing.Pen(BarcodeHelper.ToWinFormsColor(ForeColor), iBarWidth))
#endif
                            {
                                pen.Alignment = PenAlignment.Right;

                                while (pos < encodedValue.Length)
                                {
                                    //draw the appropriate color line vertically
                                    if (encodedValue[pos] == '1')
                                        g.DrawLine(pen, new System.Drawing.Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new System.Drawing.Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height));

                                    pos++;
                                }

                                // bearer bars
                                pen.Width = (float)b.Height / 8;
#if !WPF
                                pen.Color = ForeColor;
#else
                                pen.Color = BarcodeHelper.ToWinFormsColor(ForeColor);
#endif
                                pen.Alignment = PenAlignment.Center;
                                g.DrawLine(pen, new System.Drawing.Point(0, 0), new System.Drawing.Point(b.Width, 0));//top
                                g.DrawLine(pen, new System.Drawing.Point(0, b.Height), new System.Drawing.Point(b.Width, b.Height));//bottom
                                g.DrawLine(pen, new System.Drawing.Point(0, 0), new System.Drawing.Point(0, b.Height));//left
                                g.DrawLine(pen, new System.Drawing.Point(b.Width, 0), new System.Drawing.Point(b.Width, b.Height));//right
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

                        int iQuietZone = Convert.ToInt32(b.Width * 0.05);
                        if (this.margin >= 0) iQuietZone = this.margin;
                        int iBarWidth = (b.Width - (iQuietZone * 2)) / encodedValue.Length;
                        if (iBarWidth == 0) iBarWidth = 1;

                        if (this.barcodeFormat == BarcodeFormat.PostNet)
                            iBarWidthModifier = 2;

                        //set alignment
                        switch (Alignment)
                        {
                            case AlignmentPositions.Center:
                                shiftAdjustment = ((Width - (iQuietZone * 2)) % encodedValue.Length) / 2;
                                break;
                            case AlignmentPositions.Left: shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.Right: shiftAdjustment = ((Width - (iQuietZone * 2)) % encodedValue.Length);
                                break;
                            default: shiftAdjustment = ((Width - (iQuietZone * 2)) % encodedValue.Length) / 2;
                                break;
                        }//switch

                        if (iBarWidth <= 0)
                            throw new Exception("Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                        //draw image
                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(b))
                        {
#if !WPF
                            //clears the image and colors the entire background
                            g.Clear(BackColor);

                            //lines are fBarWidth wide so draw the appropriate color line vertically
                            using (System.Drawing.Pen backpen = new System.Drawing.Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (System.Drawing.Pen pen = new System.Drawing.Pen(ForeColor, iBarWidth / iBarWidthModifier))
#else
                            g.Clear(BarcodeHelper.ToWinFormsColor(BackColor));
                            using (System.Drawing.Pen backpen = new System.Drawing.Pen(BarcodeHelper.ToWinFormsColor(BackColor), iBarWidth / iBarWidthModifier))
                            {
                                using (System.Drawing.Pen pen = new System.Drawing.Pen(BarcodeHelper.ToWinFormsColor(ForeColor), iBarWidth / iBarWidthModifier))
#endif
                                {
                                    while (pos < encodedValue.Length)
                                    {
                                        if (this.barcodeFormat == BarcodeFormat.PostNet)
                                        {
                                            //draw half bars in postnet
                                            if (encodedValue[pos] != '1')
                                                g.DrawLine(pen, new System.Drawing.Point(pos * iBarWidth + shiftAdjustment + 1, Height), new System.Drawing.Point(pos * iBarWidth + shiftAdjustment + 1, Height / 2));

                                            //draw spaces between bars in postnet
                                            g.DrawLine(backpen, new System.Drawing.Point(pos * (iBarWidth * iBarWidthModifier) + shiftAdjustment + iBarWidth + 1, 0), new System.Drawing.Point(pos * (iBarWidth * iBarWidthModifier) + shiftAdjustment + iBarWidth + 1, Height));
                                        }//if

                                        if (encodedValue[pos] == '1')
                                            //g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + (int)(iBarWidth * 0.5), 0), new Point(pos * iBarWidth + shiftAdjustment + (int)(iBarWidth * 0.5), Height));
                                            g.DrawLine(pen, new System.Drawing.Point(pos * iBarWidth + shiftAdjustment + iQuietZone, iQuietZone), new System.Drawing.Point(pos * iBarWidth + shiftAdjustment + iQuietZone, Height - iQuietZone));

                                        pos++;
                                    }//while
                                }//using
                            }//using
                        }//using
                        if (IncludeLabel)
                        {
                            //if (this.EncodedType != TYPE.UPCA)
                            LabelGeneric((Image)b, iQuietZone);
                            //else
                            //    Label_UPCA((Image)b);
                        }//if

                        break;
                    }//case
            }//switch
#if !WPF
            encodedImage = (Image)b;
#else
            encodedImage = new WriteableBitmap(BarcodeHelper.ToBitmapSource(b));
#endif

            this.encodingTime += ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            return b;
        }

#endif

#if (!SILVERLIGHT && !NETFX_CORE) || WPF

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
        /// <param name="fileName">Filename to save to.</param>
        /// <param name="fileType">Format to use.</param>
        public void SaveImage(string fileName, SaveOptions fileType)
        {
            try
            {
                if (encodedImage != null)
                {
                    System.Drawing.Imaging.ImageFormat imageformat;
                    switch (fileType)
                    {
                        case SaveOptions.Bmp: imageformat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case SaveOptions.Gif: imageformat = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case SaveOptions.Jpg: imageformat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case SaveOptions.Png: imageformat = System.Drawing.Imaging.ImageFormat.Png; break;
                        case SaveOptions.Tiff: imageformat = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }//switch
#if !WPF
                    ((Bitmap)encodedImage).Save(fileName, imageformat);
#else
                    (BarcodeHelper.ToWinFormsBitmap(encodedImage)).Save(fileName, imageformat);
#endif
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
#if !WPF
                    ((Bitmap)encodedImage).Save(stream, imageformat);
#else
                    (BarcodeHelper.ToWinFormsBitmap(encodedImage)).Save(stream, imageformat);
#endif
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
#if !WPF
                using (Graphics g = Graphics.FromImage(this.EncodedImage))
#else
                using (Graphics g = Graphics.FromImage(BarcodeHelper.ToWinFormsBitmap(this.EncodedImage)))

#endif
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

#if (!SILVERLIGHT && !NETFX_CORE) || WPF
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
#if !WPF
                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, img.Height - (font.Height - 2), img.Width, font.Height));
#else
                    g.FillRectangle(new SolidBrush(BarcodeHelper.ToWinFormsColor(this.BackColor)), new Rectangle(0, img.Height - (font.Height - 2), img.Width, font.Height));

#endif
                    //draw datastring under the barcode image
                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
#if !WPF
                    if (string.IsNullOrEmpty(this.CustomLabel))
                        g.DrawString(this.Content, font, new SolidBrush(this.ForeColor), (float)(img.Width / 2), img.Height - font.Height + 1, f);
                    else
                        g.DrawString(this.CustomLabel, font, new SolidBrush(this.ForeColor), (float)(img.Width / 2), img.Height - font.Height + 1, f);
#else
                    if (string.IsNullOrEmpty(this.CustomLabel))
                        g.DrawString(this.Content, font, new SolidBrush(BarcodeHelper.ToWinFormsColor(this.ForeColor)), (float)(img.Width / 2), img.Height - font.Height + 1, f);
                    else
                        g.DrawString(this.CustomLabel, font, new SolidBrush(BarcodeHelper.ToWinFormsColor(this.ForeColor)), (float)(img.Width / 2), img.Height - font.Height + 1, f);

#endif

#if !WPF
                    System.Drawing.Pen pen = new System.Drawing.Pen(ForeColor, (float)img.Height / 16);
#else
                    System.Drawing.Pen pen = new System.Drawing.Pen(BarcodeHelper.ToWinFormsColor(ForeColor), (float)img.Height / 16);
#endif
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawLine(pen, new System.Drawing.Point(0, img.Height - font.Height - 2), new System.Drawing.Point(img.Width, img.Height - font.Height - 2));//bottom

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
#if !WPF
                    g.FillRectangle(new SolidBrush(this.BackColor), new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height));

                    // draw datastring under the barcode image
                    if (string.IsNullOrEmpty(this.CustomLabel))
                        g.DrawString(this.Content, font, new SolidBrush(this.ForeColor), new RectangleF((float)0, (float)LabelY, (float)(img.Width - iquietzone), (float)font.Height), f);
                    else
                        g.DrawString(this.CustomLabel, font, new SolidBrush(this.ForeColor), new RectangleF((float)0, (float)LabelY, (float)(img.Width - iquietzone), (float)font.Height), f);

#else
                    g.FillRectangle(new SolidBrush(BarcodeHelper.ToWinFormsColor(this.BackColor)), new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height));

                    if (string.IsNullOrEmpty(this.CustomLabel))
                        //draw datastring under the barcode image
                        g.DrawString(this.Content, font, new SolidBrush(BarcodeHelper.ToWinFormsColor(this.ForeColor)), new RectangleF((float)0, (float)LabelY, (float)(img.Width - iquietzone), (float)font.Height), f);
                    else
                        g.DrawString(this.CustomLabel, font, new SolidBrush(BarcodeHelper.ToWinFormsColor(this.ForeColor)), new RectangleF((float)0, (float)LabelY, (float)(img.Width - iquietzone), (float)font.Height), f);
#endif
                    g.Save();
                } //using
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
                    g.FillRectangle(new SolidBrush(System.Drawing.Color.Yellow), rect.X, rect.Y, rect.Width, rect.Height);
#if !WPF
                    g.DrawString(this.Content.Substring(1, 5), font, new SolidBrush(this.ForeColor), rect.X, rect.Y);
#else
                    g.DrawString(this.Content.Substring(1, 5), font, new SolidBrush(BarcodeHelper.ToWinFormsColor(this.ForeColor)), rect.X, rect.Y);
#endif

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
            int stringLengths = 18;

            string temp = data;
            string[] strings = new string[(data.Length / stringLengths) + ((data.Length % stringLengths == 0) ? 0 : 1)];

            int i = 0;
            while (i < strings.Length)
                if (temp.Length >= stringLengths)
                {
                    strings[i++] = temp.Substring(0, stringLengths);
                    temp = temp.Substring(stringLengths);
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
#if (!SILVERLIGHT && !NETFX_CORE)
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
        /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
        /// <returns>Image representing the barcode.</returns>
#if (!SILVERLIGHT && !NETFX_CORE)
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
#if (!SILVERLIGHT && !NETFX_CORE)
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
#if (!SILVERLIGHT && !NETFX_CORE)
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor)
#else
#if NETFX_CORE
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, Windows.UI.Color foreColor, Windows.UI.Color backColor)
#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, System.Windows.Media.Color foreColor, System.Windows.Media.Color backColor)
#endif
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
#if (!SILVERLIGHT && !NETFX_CORE)
        public static Image DoEncode(BarcodeFormat format, string data, bool includeLabel, Color foreColor, Color backColor, int width, int height)
#else
#if NETFX_CORE
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, Windows.UI.Color foreColor, Windows.UI.Color backColor, int width, int height)

#else
        public static WriteableBitmap DoEncode(BarcodeFormat format, string data, bool includeLabel, System.Windows.Media.Color foreColor, System.Windows.Media.Color backColor, int width, int height)
#endif

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
