using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Foundation.Metadata;

using MessagingToolkit.Barcode.Aztec;
using MessagingToolkit.Barcode.DataMatrix;
using MessagingToolkit.Barcode.OneD;
using MessagingToolkit.Barcode.Pdf417;
using MessagingToolkit.Barcode.QRCode;
using MessagingToolkit.Barcode.MaxiCode;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Multi;
using MessagingToolkit.Barcode.Multi.QRCode;

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// BarcodeDecoder is a convenience class and the main entry point into the library for most uses.
    /// By default it attempts to decode all barcode formats that the library supports. Optionally, you
    /// can provide a hints object to request different behavior, for example only decoding QR codes.
    /// </summary>
    public sealed class BarcodeDecoder
    {
        private IDictionary<DecodeOptions, object> decodingOptions;
        private List<IDecoder> decoders = new List<IDecoder>();
        private LuminanceSource luminanceSource;

        private static readonly Func<WriteableBitmap, LuminanceSource> defaultCreateLuminanceSource = (bitmap) => new BitmapLuminanceSource(bitmap);
        private readonly Func<WriteableBitmap, LuminanceSource> createLuminanceSource;
        private readonly Func<LuminanceSource, Binarizer> createBinarizer;

        private static readonly Func<LuminanceSource, Binarizer> defaultCreateBinarizer = (luminanceSource) => new HybridBinarizer(luminanceSource);

        private readonly Func<byte[], int, int, BitmapFormat, LuminanceSource> createRGBLuminanceSource;
        private static readonly Func<byte[], int, int, BitmapFormat, LuminanceSource> defaultCreateRGBLuminanceSource =
                                                    (rawBytes, width, height, format) => new RGBLuminanceSource(rawBytes, width, height, format);

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeDecoder"/> class.
        /// </summary>
        public BarcodeDecoder()
            : this(null, null)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image should be automatically rotated.
        /// Rotation is supported for 90, 180 and 270 degrees
        /// </summary>
        /// <value>
        ///   <c>true</c> if image should be rotated; otherwise, <c>false</c>.
        /// </value>
        protected bool AutoRotate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the image should be automatically inverted
        /// if no result is found in the original image.
        /// ATTENTION: Please be carefully because it slows down the decoding process if it is used
        /// </summary>
        /// <value>
        ///   <c>true</c> if image should be inverted; otherwise, <c>false</c>.
        /// </value>
        protected bool TryInverted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeDecoder"/> class.
        /// </summary>
        /// <param name="createLuminanceSource">The create luminance source.</param>
        /// <param name="createBinarizer">The create binarizer.</param>
        internal BarcodeDecoder(Func<WriteableBitmap, LuminanceSource> createLuminanceSource, Func<LuminanceSource, Binarizer> createBinarizer)
            : this(createLuminanceSource, createBinarizer, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeDecoder"/> class.
        /// </summary>
        /// <param name="createLuminanceSource">The create luminance source.</param>
        /// <param name="createBinarizer">The create binarizer.</param>
        internal BarcodeDecoder(Func<WriteableBitmap, LuminanceSource> createLuminanceSource,
                Func<LuminanceSource, Binarizer> createBinarizer,
                Func<byte[], int, int, BitmapFormat, LuminanceSource> createRGBLuminanceSource
           )
        {
            this.createLuminanceSource = createLuminanceSource ?? defaultCreateLuminanceSource;
            this.createBinarizer = createBinarizer ?? defaultCreateBinarizer;
            this.createRGBLuminanceSource = createRGBLuminanceSource ?? defaultCreateRGBLuminanceSource;
            this.decodingOptions = new Dictionary<DecodeOptions, object>(1);
        }



        /// <summary>
        /// Optional: Gets or sets the function to create a luminance source object for a bitmap.
        /// If null then RGBLuminanceSource is used
        /// </summary>
        /// <value>
        /// The function to create a luminance source object.
        /// </value>
        internal Func<WriteableBitmap, LuminanceSource> CreateLuminanceSource
        {
            get
            {
                return createLuminanceSource ?? defaultCreateLuminanceSource;
            }
        }

        /// <summary>
        /// Optional: Gets or sets the function to create a binarizer object for a luminance source.
        /// If null then HybridBinarizer is used
        /// </summary>
        /// <value>
        /// The function to create a binarizer object.
        /// </value>
        internal Func<LuminanceSource, Binarizer> CreateBinarizer
        {
            get
            {
                return createBinarizer ?? defaultCreateBinarizer;
            }
        }

        /// <summary>
        /// This version of decode honors the intent of Decoder.decode(BinaryBitmap) in that it
        /// passes null as a options to the decoders. However, that makes it inefficient to call repeatedly.
        /// Use SetDecodingOptions() followed by DecodeWithState() for continuous scan applications.
        /// </summary>
        /// <param name="binaryBitmap">The pixel data to decode</param>
        /// <returns>
        /// The contents of the image
        /// </returns>
        internal Result Decode(BinaryBitmap binaryBitmap)
        {
            SetOptions(null);
            return DecodeInternal(binaryBitmap);
        }

        public Result Decode(WriteableBitmap image)
        {
            if (image != null)
            {
                this.luminanceSource = CreateLuminanceSource(image);
                Binarizer binarizer = CreateBinarizer(luminanceSource);
                BinaryBitmap bitmap = new BinaryBitmap(binarizer);
                return Decode(bitmap);
            }
            else
            {
                throw NotFoundException.Instance;
            }

        }

        /// <summary>
        /// Decode an image using the options provided. Does not honor existing state.
        /// </summary>
        /// <param name="image">The pixel data to decode</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        /// The contents of the image
        /// </returns>
        internal Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            SetOptions(decodingOptions);
            return DecodeInternal(image);
        }

        /// <summary>
        /// Decode an image using the options provided. Does not honor existing state.
        /// </summary>
        /// <param name="image">The pixel data to decode</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        /// The contents of the image
        /// </returns>
        public Result Decode(WriteableBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            if (image != null)
            {
                this.luminanceSource = CreateLuminanceSource(image);
                Binarizer binarizer = CreateBinarizer(luminanceSource);
                BinaryBitmap bitmap = new BinaryBitmap(binarizer);
                return Decode(bitmap, decodingOptions);
            }
            else
            {
                throw NotFoundException.Instance;
            }

        }

        /// <summary>
        /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="rawRGB">The image as RGB24 array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The format.</param>
        /// <returns>
        /// the result data or null
        /// </returns>
        public Result Decode([System.Runtime.InteropServices.WindowsRuntime.ReadOnlyArray]byte[] rawRGB, int width, int height, BitmapFormat format)
        {
            if (rawRGB == null)
                throw new ArgumentNullException("rawRGB");

            this.luminanceSource = createRGBLuminanceSource(rawRGB, width, height, format);
            Binarizer binarizer = CreateBinarizer(luminanceSource);
            BinaryBitmap bitmap = new BinaryBitmap(binarizer);
            return Decode(bitmap);
        }

        /// <summary>
        /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="rawRGB">The image as RGB24 array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The format.</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        /// the result data or null
        /// </returns>
        public Result Decode([System.Runtime.InteropServices.WindowsRuntime.ReadOnlyArray]byte[] rawRGB, int width, int height, BitmapFormat format, IDictionary<DecodeOptions, object> decodingOptions)
        {
            if (rawRGB == null)
                throw new ArgumentNullException("rawRGB");

            this.luminanceSource = createRGBLuminanceSource(rawRGB, width, height, format);
            Binarizer binarizer = CreateBinarizer(luminanceSource);
            BinaryBitmap bitmap = new BinaryBitmap(binarizer);
            return Decode(bitmap, decodingOptions);
        }


        /// <summary>
        /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="IRandomAccessStream">Stream representing the image file.</param>
        /// <returns>
        /// the result data or null
        /// </returns>
        [DefaultOverloadAttribute]
        public IAsyncOperation<Result> Decode(IRandomAccessStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            return Task.Run(() => DoDecode(stream, null)).AsAsyncOperation();
        }


        /// <summary>
        /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="IRandomAccessStream">Stream representing the image file.</param>
        /// <param name="decodingOptions">
        /// The meaning of the data depends upon the decoding options type. The implementation may or may not do anything with these options.
        /// </param>
        /// <returns>
        /// the result data or null
        /// </returns>
        [DefaultOverloadAttribute]
        public IAsyncOperation<Result> Decode(IRandomAccessStream stream, IDictionary<DecodeOptions, object> decodingOptions)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            return Task.Run(() => DoDecode(stream, decodingOptions)).AsAsyncOperation();
        }


        internal async Task<Result> DoDecode(IRandomAccessStream stream, IDictionary<DecodeOptions, object> decodingOptions)
        {
            stream.Seek(0);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            if (decoder != null)
            {
                PixelDataProvider pixelDataProvider = await decoder.GetPixelDataAsync();
                byte[] rawPixels = pixelDataProvider.DetachPixelData();
                byte[] pixels = null;
                BitmapFormat format = BitmapFormat.RGBA32;

                switch (decoder.BitmapPixelFormat)
                {
                    case BitmapPixelFormat.Rgba16:
                        // Allocate a typed array with the raw pixel data
                        var pixelBuffer = new byte[rawPixels.Length];
                        rawPixels.CopyTo(pixelBuffer, 0);

                        pixels = new byte[pixelBuffer.Length];
                        pixelBuffer.CopyTo(pixels, 0);
                        if (decoder.BitmapAlphaMode == BitmapAlphaMode.Straight)
                            format = MessagingToolkit.Barcode.BitmapFormat.RGBA32;
                        else
                            format = MessagingToolkit.Barcode.BitmapFormat.RGB32;
                        break;
                    case BitmapPixelFormat.Rgba8:
                        // For 8 bit pixel formats, just use the returned pixel array.
                        pixels = rawPixels;
                        if (decoder.BitmapAlphaMode == BitmapAlphaMode.Straight)
                            format = MessagingToolkit.Barcode.BitmapFormat.RGBA32;
                        else
                            format = MessagingToolkit.Barcode.BitmapFormat.RGB32;
                        break;

                    case BitmapPixelFormat.Bgra8:
                        // For 8 bit pixel formats, just use the returned pixel array.
                        pixels = rawPixels;
                        if (decoder.BitmapAlphaMode == BitmapAlphaMode.Straight)
                            format = MessagingToolkit.Barcode.BitmapFormat.BGRA32;
                        else
                            format = MessagingToolkit.Barcode.BitmapFormat.BGR32;
                        break;
                }
                if (decodingOptions != null)
                    return Decode(pixels, (int)decoder.PixelWidth, (int)decoder.PixelHeight, format, decodingOptions);
                else
                    return Decode(pixels, (int)decoder.PixelWidth, (int)decoder.PixelHeight, format);
            }

            return null;
        }

        /// <summary>
        /// Decode an image using the state set up by calling SetOptions() previously. Continuous scan
        /// clients will get a <b>large</b> speed increase by using this instead of decode().
        /// </summary>
        /// <param name="image">The pixel data to decode</param>
        /// <returns>
        /// The contents of the image
        /// </returns>
        internal Result DecodeWithState(LuminanceSource luminanceSource, BinaryBitmap image)
        {
            // Make sure to set up the default state so we don't crash
            if (decoders.Count == 0)
            {
                SetOptions(null);
            }
            this.luminanceSource = luminanceSource;
            return DecodeInternal(image);
        }

        /// <summary>
        /// This method adds state to the BarcodeDecoder. By setting the hints once, subsequent calls
        /// to decodeWithState(image) can reuse the same set of decoders without reallocating memory. This
        /// is important for performance in continuous scan clients.
        /// </summary>
        /// <param name="decodingOptions">The decoding options.</param>
        public void SetOptions(IDictionary<DecodeOptions, object> decodingOptions)
        {
            this.decodingOptions = decodingOptions;

            bool tryHarder = decodingOptions != null && decodingOptions.ContainsKey(MessagingToolkit.Barcode.DecodeOptions.TryHarder);
            bool hasAutoRotateOption = decodingOptions != null && decodingOptions.ContainsKey(MessagingToolkit.Barcode.DecodeOptions.AutoRotate);

            if (hasAutoRotateOption)
            {
                this.AutoRotate = (bool)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.AutoRotate);
            }
            else
            {
                this.AutoRotate = false;
            }
            List<BarcodeFormat> formats = (decodingOptions == null) ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);
            decoders.Clear();
            if (formats != null)
            {
                bool addOneDDecoder = formats.Contains(BarcodeFormat.UPCA)
                        || formats.Contains(BarcodeFormat.UPCE)
                        || formats.Contains(BarcodeFormat.EAN13)
                        || formats.Contains(BarcodeFormat.EAN8)
                        || formats.Contains(BarcodeFormat.Codabar)
                        || formats.Contains(BarcodeFormat.Code39)
                        || formats.Contains(BarcodeFormat.Code93)
                        || formats.Contains(BarcodeFormat.Code128)
                        || formats.Contains(BarcodeFormat.ITF14)
                        || formats.Contains(BarcodeFormat.RSS14)
                        || formats.Contains(BarcodeFormat.RSSExpanded) 
                        || formats.Contains(BarcodeFormat.MSIMod10);
                // Put 1D decoders upfront in "normal" mode
                if (addOneDDecoder && !tryHarder)
                {
                    decoders.Add(new BarcodeOneDDecoder(decodingOptions));
                }
                if (formats.Contains(MessagingToolkit.Barcode.BarcodeFormat.QRCode))
                {
                    if (BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.MultipleBarcode) == null)
                        decoders.Add(new QRCodeDecoder());
                }
                if (formats.Contains(MessagingToolkit.Barcode.BarcodeFormat.DataMatrix))
                {
                    decoders.Add(new DataMatrixDecoder());
                }
                if (formats.Contains(MessagingToolkit.Barcode.BarcodeFormat.Aztec))
                {
                    decoders.Add(new AztecDecoder());
                }
                if (formats.Contains(MessagingToolkit.Barcode.BarcodeFormat.PDF417))
                {
                    decoders.Add(new Pdf417Decoder());
                }

                if (formats.Contains(MessagingToolkit.Barcode.BarcodeFormat.MaxiCode))
                {
                    decoders.Add(new MaxiCodeDecoder());
                }
                // At end in "try harder" mode
                if (addOneDDecoder && tryHarder)
                {
                    decoders.Add(new BarcodeOneDDecoder(decodingOptions));
                }
            }
            if ((decoders.Count == 0))
            {
                if (BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.MultipleBarcode) == null)
                {
                    decoders.Add(new QRCodeDecoder());
                }

                decoders.Add(new DataMatrixDecoder());
                decoders.Add(new AztecDecoder());
                decoders.Add(new Pdf417Decoder());
                decoders.Add(new MaxiCodeDecoder());

                //if (!tryHarder)
                //{
                decoders.Add(new BarcodeOneDDecoder(decodingOptions));
                //}

                //if (tryHarder)
                //{
                //decoders.Add(new BarcodeOneDDecoder(decodingOptions));
                //}
            }
        }

        public void Reset()
        {
            int size = decoders.Count;
            foreach (IDecoder decoder in decoders)
            {
                decoder.Reset();
            }
        }


        private Result DecodeInternal(BinaryBitmap binaryBitmap)
        {
            int rotationCount = 0;
            int rotationMaxCount = this.AutoRotate ? 4 : 1;

            for (; rotationCount < rotationMaxCount; rotationCount++)
            {
                foreach (IDecoder decoder in decoders)
                {
                    try
                    {
                        Result result = decoder.Decode(binaryBitmap, decodingOptions);
                        if (result != null)
                        {
                            if (result.ResultMetadata == null)
                            {
                                result.PutMetadata(ResultMetadataType.Orientation, rotationCount * 90);
                            }
                            else if (!result.ResultMetadata.ContainsKey(ResultMetadataType.Orientation))
                            {
                                result.ResultMetadata[ResultMetadataType.Orientation] = rotationCount * 90;
                            }
                            else
                            {
                                // perhaps the core decoder rotates the image already (can happen if TryHarder is specified)
                                result.ResultMetadata[ResultMetadataType.Orientation] = ((int)(result.ResultMetadata[ResultMetadataType.Orientation]) + rotationCount * 90) % 360;
                            }
                        }
                        return result;
                    }
                    catch
                    {
                        // Continue
                    }
                }

                if (luminanceSource == null || !luminanceSource.RotateSupported || !AutoRotate)
                    break;

                binaryBitmap = new BinaryBitmap(CreateBinarizer(luminanceSource.RotateCounterClockwise()));
            }

            throw NotFoundException.Instance;
        }

        /// <summary>
        /// Decodes multiple barcodes
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="decodingOptions">The decoding options.</param>
        /// <returns></returns>
        public Result[] DecodeMultiple(WriteableBitmap image)
        {
            return DecodeMultiple(image, null);
        }

        /// <summary>
        /// Decodes multiple barcodes
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="decodingOptions">The decoding options.</param>
        /// <returns></returns>
        public Result[] DecodeMultiple(WriteableBitmap image, IDictionary<DecodeOptions, object> decodingOptions)

        {
            LuminanceSource luminanceSource = CreateLuminanceSource(image);
            Binarizer binarizer = CreateBinarizer(luminanceSource);
            BinaryBitmap bitmap = new BinaryBitmap(binarizer);
            return DecodeMultiple(bitmap, decodingOptions);
        }

        /// <summary>
        /// Decode multiple barcode.
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns></returns>
        internal Result[] DecodeMultiple(BinaryBitmap image)
        {
            return DecodeMultiple(image, null);
        }

        /// <summary>
        /// Decode multiple barcodes
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="decodingOptions">Decoding options</param>
        /// <returns></returns>
        internal Result[] DecodeMultiple(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            if (decodingOptions == null) decodingOptions = new Dictionary<DecodeOptions, object>(1);
            decodingOptions.Add(DecodeOptions.MultipleBarcode, true);
            List<BarcodeFormat> formats = (decodingOptions == null) ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);
            if (formats == null) formats = new List<BarcodeFormat>();
            BarcodeDecoderException savedException = null;
            List<Result> theResults = new List<Result>(1);
            try
            {
                // Look for multiple barcodes
                MultipleBarcodeDecoder multiDecoder = new GenericMultipleBarcodeDecoder(this);
                Result[] results = multiDecoder.DecodeMultiple(image, decodingOptions);
                if (results != null) theResults.AddRange(results);
            }
            catch (BarcodeDecoderException re)
            {
                savedException = re;
            }

            if (formats.Count == 0 || formats.Contains(BarcodeFormat.QRCode))
            {
                try
                {
                    // Look for multiple barcodes
                    QRCodeMultiDecoder qrcodeMultiDecoder = new QRCodeMultiDecoder();
                    Result[] results2 = qrcodeMultiDecoder.DecodeMultiple(image);
                    if (results2 != null) theResults.AddRange(results2);

                }
                catch (BarcodeDecoderException re)
                {
                    savedException = re;
                }
            }
            return theResults.ToArray();
        }

        /// <summary>
        /// Decodes multiple barcodes in the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="rawRGB">The image as RGB24 array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The format.</param>
        /// <returns>
        /// The result data or a empty list
        /// </returns>
        public Result[] DecodeMultiple([System.Runtime.InteropServices.WindowsRuntime.ReadOnlyArray]byte[] rawRGB, int width, int height, BitmapFormat format)
        {
            if (rawRGB == null)
                throw new ArgumentNullException("rawRGB");

            this.luminanceSource = createRGBLuminanceSource(rawRGB, width, height, format);
            Binarizer binarizer = CreateBinarizer(luminanceSource);
            BinaryBitmap bitmap = new BinaryBitmap(binarizer);
            return DecodeMultiple(bitmap);
        }

        /// <summary>
        /// Decodes multiple barcodes in the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="rawRGB">The image as RGB24 array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The format.</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        /// The result data or a empty list
        /// </returns>
        public Result[] DecodeMultiple([System.Runtime.InteropServices.WindowsRuntime.ReadOnlyArray]byte[] rawRGB, int width, int height, BitmapFormat format, IDictionary<DecodeOptions, object> decodingOptions)
        {
            if (rawRGB == null)
                throw new ArgumentNullException("rawRGB");

            this.luminanceSource = createRGBLuminanceSource(rawRGB, width, height, format);
            Binarizer binarizer = CreateBinarizer(luminanceSource);
            BinaryBitmap bitmap = new BinaryBitmap(binarizer);
            return DecodeMultiple(bitmap, decodingOptions);
        }
    }
}
