using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.UI;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;

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
using MessagingToolkit.Barcode.Aztec.Encoder;



namespace MessagingToolkit.Barcode
{
    /// <summary> 
    /// This is a factory class which finds the appropriate encoder subclass for the BarcodeFormat
    /// requested and encodes the barcode with the supplied contents.
    /// </summary>
    public sealed class BarcodeEncoder : IDisposable
    {
        #region Variables

        private IEncoder iOneDEncoder = new DefaultEncoder();
        private IEncoder iQRCodeEncoder = new DefaultEncoder();
        private IEncoder iDataMatrixEncoder = new DefaultEncoder();
        private IEncoder iPdf417Encoder = new DefaultEncoder();
        private IEncoder iAztecEncoder = new DefaultEncoder();

        private string content = string.Empty;
        private string encodedValue = "";
        private BarcodeFormat barcodeFormat = BarcodeFormat.Unknown;

        private Color foreColor = Colors.Black;
        private Color backColor = Colors.White;

        private int width = 300;
        private int height = 150;
        private bool includeLabel = false;
        private double encodingTime = 0;

        private ErrorCorrectionLevel errorCorrectionLevel = ErrorCorrectionLevel.L;
        private string characterSet = null;
        private int margin = -1;

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


        public IRandomAccessStream EncodedImageStream
        {
            get;
            set;
        }

        /// <summary>
        /// Bit matrix representation of the encoded barcode.
        /// </summary>
        public BitMatrix BitMatrix { get; private set; }

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
        public IDictionary<EncodeOptions, object> EncodingOptions
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


        #endregion

        /// <summary>
        /// Generates the specified format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format">The format.</param>
        /// <param name="content">The content.</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <param name="provider">The output provider.</param>
        /// <returns></returns>
        public IOutput Generate(BarcodeFormat format, string content, IDictionary<EncodeOptions, object> encodingOptions, IOutputProvider provider)
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
                    case BarcodeFormat.Aztec:
                        encoder = new AztecEncoder();
                        break;
                    default: throw new Exception("Unsupported encoding type specified.");
                }

                Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);
                if (!encodeOptions.ContainsKey(EncodeOptions.CharacterSet) && !string.IsNullOrEmpty(this.CharacterSet))
                    encodeOptions.Add(EncodeOptions.CharacterSet, this.CharacterSet);

                if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
                {
                    if (this.Margin >= 0)
                    {
                        encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                    }
                }

                if (this.barcodeFormat == BarcodeFormat.QRCode)
                {

                    if (!encodeOptions.ContainsKey(EncodeOptions.ErrorCorrection))
                        encodeOptions.Add(EncodeOptions.ErrorCorrection, this.ErrorCorrectionLevel);

                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);

                }
                else if (this.barcodeFormat == BarcodeFormat.DataMatrix)
                {
                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                }
                else if (this.barcodeFormat == BarcodeFormat.PDF417)
                {
                    if (!encodeOptions.ContainsKey(EncodeOptions.Pdf417Compaction))
                        encodeOptions.Add(EncodeOptions.Pdf417Compaction, this.Pdf417Compaction);

                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                }
                else if (this.barcodeFormat == BarcodeFormat.Aztec)
                {
                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                }
                else
                {
                    // One dimensional barcode
                    this.BitMatrix = encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
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
        public IOutput Generate(BarcodeFormat format, string content, IOutputProvider provider)
        {
            return Generate(format, content, null, provider);
        }

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
        public IAsyncOperation<IRandomAccessStream> Encode(BarcodeFormat format, string content)
        {
            this.content = content;
            this.barcodeFormat = format;
            return Task.Run(() => Encode()).AsAsyncOperation();
        }

        public IAsyncOperation<IRandomAccessStream> Encode(BarcodeFormat format, string content, IDictionary<EncodeOptions, object> encodingOptions)
        {
            try
            {
                this.content = content;
                if (encodingOptions != null)
                    this.EncodingOptions = encodingOptions;
                this.barcodeFormat = format;
                return Task.Run(() => Encode()).AsAsyncOperation();
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
        public IAsyncOperation<IRandomAccessStream> Encode(BarcodeFormat barcodeFormat)
        {
            this.barcodeFormat = barcodeFormat;
            return Task.Run(() => Encode()).AsAsyncOperation();
        }

        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.
        /// </summary>
        internal async Task<IRandomAccessStream> Encode()
        {
            iOneDEncoder = new DefaultEncoder();
            iQRCodeEncoder = new DefaultEncoder();
            iDataMatrixEncoder = new DefaultEncoder();
            iAztecEncoder = new DefaultEncoder();

            DateTime dtStartTime = DateTime.Now;

            // make sure there is something to encode
            if (string.IsNullOrEmpty(content))
                throw new Exception("Input data not allowed to be blank.");

            if (this.EncodedBarcodeFormat == BarcodeFormat.Unknown)
                throw new Exception("Symbology type not allowed to be unspecified.");

            this.encodedValue = string.Empty;

            switch (this.barcodeFormat)
            {
                case BarcodeFormat.QRCode:
                    iQRCodeEncoder = new QRCodeEncoder();
                    break;
                case BarcodeFormat.PDF417:
                    iPdf417Encoder = new Pdf417Encoder();
                    break;
                case BarcodeFormat.Aztec:
                    iAztecEncoder = new AztecEncoder();
                    break;
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
                default: throw new Exception("Unsupported encoding type specified.");
            }

            Dictionary<EncodeOptions, object> encodeOptions = new Dictionary<EncodeOptions, object>(this.EncodingOptions);
            if (!encodeOptions.ContainsKey(EncodeOptions.CharacterSet) && !string.IsNullOrEmpty(this.CharacterSet))
                encodeOptions.Add(EncodeOptions.CharacterSet, this.CharacterSet);
            if (!encodeOptions.ContainsKey(EncodeOptions.Margin))
            {
                if (this.Margin >= 0)
                {
                    encodeOptions.Add(EncodeOptions.Margin, this.Margin);
                }
            }

            if (iOneDEncoder.GetType() != typeof(DefaultEncoder))
            {
                this.BitMatrix = iOneDEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, this.EncodingOptions);
                this.EncodedImageStream = await MatrixToStreamHelper.ToStream(this.BitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix.ToString();
            }
            else if (iQRCodeEncoder.GetType() != typeof(DefaultEncoder))
            {

                if (!encodeOptions.ContainsKey(EncodeOptions.ErrorCorrection))
                    encodeOptions.Add(EncodeOptions.ErrorCorrection, this.ErrorCorrectionLevel);

                this.BitMatrix = iQRCodeEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                this.EncodedImageStream = await MatrixToStreamHelper.ToStream(this.BitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix.ToString();
            }
            else if (iDataMatrixEncoder.GetType() != typeof(DefaultEncoder))
            {
                DataMatrixNewEncoder dataMatrixEncoder = iDataMatrixEncoder as DataMatrixNewEncoder;
                this.BitMatrix = dataMatrixEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                this.EncodedImageStream = await MatrixToStreamHelper.ToStream(this.BitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix.ToString();
            }
            else if (iAztecEncoder.GetType() != typeof(DefaultEncoder))
            {
                AztecEncoder aztecEncoder = iAztecEncoder as AztecEncoder;
                this.BitMatrix = aztecEncoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                this.EncodedImageStream = await MatrixToStreamHelper.ToStream(this.BitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix.ToString();
            }
            else if (iPdf417Encoder.GetType() != typeof(DefaultEncoder))
            {
                Pdf417Encoder pdf417Encoder = iPdf417Encoder as Pdf417Encoder;

                if (!encodeOptions.ContainsKey(EncodeOptions.Pdf417Compaction))
                    encodeOptions.Add(EncodeOptions.Pdf417Compaction, this.Pdf417Compaction);

                this.BitMatrix = pdf417Encoder.Encode(this.content, this.barcodeFormat, this.Width, this.Height, encodeOptions);
                this.EncodedImageStream = await MatrixToStreamHelper.ToStream(this.BitMatrix, this.ForeColor, this.BackColor);
                this.encodedValue = this.BitMatrix.ToString();
            }

            this.encodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;
            return this.EncodedImageStream;
        }

        internal static bool CheckNumericOnly(string data)
        {
            // This function takes a string of data and breaks it into parts and trys to do Int64.TryParse
            // This will verify that only numeric data is contained in the string passed in.  The complexity below
            // was done to ensure that the minimum number of interations and checks could be performed.

            // 9223372036854775808 is the largest number a 64bit number(signed) can hold so ... make sure its less than that by one place
            int stringLengths = 18;

            string temp = data;
            string[] strings = new string[(data.Length / stringLengths) + ((data.Length % stringLengths == 0) ? 0 : 1)];

            int i = 0;
            while (i < strings.Length)
                if (temp.Length >= stringLengths)
                {
                    strings[i++] = temp.Substring(0, stringLengths);
                    temp = temp.Substring(stringLengths);
                }
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


        public void Dispose()
        {
        }
    }
}
