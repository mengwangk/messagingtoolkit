using MessagingToolkit.Barcode.Aztec;
using MessagingToolkit.Barcode.DataMatrix;
using MessagingToolkit.Barcode.OneD;
using MessagingToolkit.Barcode.Pdf417;
using MessagingToolkit.Barcode.QRCode;
using MessagingToolkit.Barcode.MaxiCode;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.Common;

using System;
using System.Collections.Generic;

#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// BarcodeDecoder is a convenience class and the main entry point into the library for most uses.
    /// By default it attempts to decode all barcode formats that the library supports. Optionally, you
    /// can provide a hints object to request different behavior, for example only decoding QR codes.
    /// </summary>
    public sealed class BarcodeDecoder : IBarcodeDecoder
    {
        private Dictionary<DecodeOptions, object> decodingOptions;
        private List<IDecoder> decoders = new List<IDecoder>();


#if !SILVERLIGHT
        private static readonly Func<Bitmap, LuminanceSource> defaultCreateLuminanceSource =
           (bitmap) => new RGBLuminanceSource(bitmap, bitmap.Width, bitmap.Height);
#else
      private static readonly Func<WriteableBitmap, LuminanceSource> defaultCreateLuminanceSource =
         (bitmap) => new RGBLuminanceSource(bitmap, bitmap.PixelWidth, bitmap.PixelHeight);
#endif
        private static readonly Func<LuminanceSource, Binarizer> defaultCreateBinarizer =
           (luminanceSource) => new HybridBinarizer(luminanceSource);

#if !SILVERLIGHT
        public Func<Bitmap, LuminanceSource> CreateLuminanceSourceFunc;
#else
      public readonly Func<WriteableBitmap, LuminanceSource> CreateLuminanceSourceFunc;
#endif
        public readonly Func<LuminanceSource, Binarizer> CreateBinarizerFunc;



#if !SILVERLIGHT
        /// <summary>
        /// Optional: Gets or sets the function to create a luminance source object for a bitmap.
        /// If null then RGBLuminanceSource is used
        /// </summary>
        /// <value>
        /// The function to create a luminance source object.
        /// </value>
        public Func<Bitmap, LuminanceSource> CreateLuminanceSource
#else
      /// <summary>
      /// Optional: Gets or sets the function to create a luminance source object for a bitmap.
      /// If null then RGBLuminanceSource is used
      /// </summary>
      /// <value>
      /// The function to create a luminance source object.
      /// </value>
      public Func<WriteableBitmap, LuminanceSource> CreateLuminanceSource
#endif
        {
            get
            {
                return CreateLuminanceSourceFunc ?? defaultCreateLuminanceSource;
            }
        }

        /// <summary>
        /// Optional: Gets or sets the function to create a binarizer object for a luminance source.
        /// If null then HybridBinarizer is used
        /// </summary>
        /// <value>
        /// The function to create a binarizer object.
        /// </value>
        public Func<LuminanceSource, Binarizer> CreateBinarizer
        {
            get
            {
                return CreateBinarizerFunc ?? defaultCreateBinarizer;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeDecoder"/> class.
        /// </summary>
        public BarcodeDecoder()
            : base()
        {

        }

        /// <summary>
        /// This version of decode honors the intent of Decoder.decode(BinaryBitmap) in that it
        /// passes null as a options to the decoders. However, that makes it inefficient to call repeatedly.
        /// Use setDecodingOptions() followed by decodeWithState() for continuous scan applications.
        /// </summary>
        /// <param name="image">The pixel data to decode</param>
        /// <returns>
        /// The contents of the image
        /// </returns>
        public Result Decode(BinaryBitmap image)
        {
            SetOptions(null);
            return DecodeInternal(image);
        }

#if !SILVERLIGHT
        public Result Decode(Bitmap image)
#else
        public Result Decode(WriteableBitmap image)
#endif
        {
            if (image != null)
            {
                LuminanceSource luminanceSource = CreateLuminanceSource(image);
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
        public Result Decode(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
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
#if !SILVERLIGHT
        public Result Decode(Bitmap image, Dictionary<DecodeOptions, object> decodingOptions)
#else
        public Result Decode(WriteableBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
#endif
        {
            if (image != null)
            {
                LuminanceSource luminanceSource = CreateLuminanceSource(image);
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
        /// Decode an image using the state set up by calling SetOptions() previously. Continuous scan
        /// clients will get a <b>large</b> speed increase by using this instead of decode().
        /// </summary>
        /// <param name="image">The pixel data to decode</param>
        /// <returns>
        /// The contents of the image
        /// </returns>
        public Result DecodeWithState(BinaryBitmap image)
        {
            // Make sure to set up the default state so we don't crash
            if (decoders.Count == 0)
            {
                SetOptions(null);
            }
            return DecodeInternal(image);
        }

        /// <summary>
        /// This method adds state to the BarcodeDecoder. By setting the hints once, subsequent calls
        /// to decodeWithState(image) can reuse the same set of decoders without reallocating memory. This
        /// is important for performance in continuous scan clients.
        /// </summary>
        /// <param name="decodingOptions">The decoding options.</param>
        public void SetOptions(Dictionary<DecodeOptions, object> decodingOptions)
        {
            this.decodingOptions = decodingOptions;

            bool tryHarder = decodingOptions != null
                    && decodingOptions.ContainsKey(MessagingToolkit.Barcode.DecodeOptions.TryHarder);
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
                        || formats.Contains(BarcodeFormat.RSSExpanded);
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
                if (!tryHarder)
                {
                    decoders.Add(new BarcodeOneDDecoder(decodingOptions));
                }

                if (BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.MultipleBarcode) == null)
                {
                    decoders.Add(new QRCodeDecoder());
                }
                
                decoders.Add(new DataMatrixDecoder());
                decoders.Add(new AztecDecoder());
                decoders.Add(new Pdf417Decoder());
                decoders.Add(new MaxiCodeDecoder());

                if (tryHarder)
                {
                    decoders.Add(new BarcodeOneDDecoder(decodingOptions));
                }
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


        private Result DecodeInternal(BinaryBitmap image)
        {
            foreach (IDecoder decoder in decoders)
            {
                try
                {
                    return decoder.Decode(image, decodingOptions);
                }
                catch (BarcodeDecoderException re)
                {
                    //System.Console.WriteLine(re.Message);
                    // continue
                }
            }
            throw NotFoundException.Instance;
        }

    }
}
