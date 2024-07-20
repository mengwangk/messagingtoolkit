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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Decode multiple barcodes in a image source.
    /// </summary>
    public sealed class BarcodeMultiDecoder : IDisposable
    {
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
        /// Initializes a new instance of the <see cref="BarcodeMultiDecoder"/> class.
        /// </summary>
        public BarcodeMultiDecoder()
        {
        }

        /// <summary>
        /// Decodes multiple barcodes
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="barcodeFormat">The barcode format.</param>
        /// <returns></returns>
#if !SILVERLIGHT
        public Result[] Decode(Bitmap image)
#else
        public Result[] Decode(WriteableBitmap image)
#endif
        {
            return Decode(image, null);
        }

        /// <summary>
        /// Decodes multiple barcodes
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="decodingOptions">The decoding options.</param>
        /// <returns></returns>
#if !SILVERLIGHT
        public Result[] Decode(Bitmap image, Dictionary<DecodeOptions, object> decodingOptions)
#else
        public Result[] Decode(WriteableBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
#endif
       
        {
            if (decodingOptions == null) decodingOptions = new Dictionary<DecodeOptions, object>(1);
            decodingOptions.Add(DecodeOptions.MultipleBarcode, true);
            List<BarcodeFormat> formats = (decodingOptions == null) ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);
            if (formats == null) formats = new List<BarcodeFormat>();

            IBarcodeDecoder barcodeDecoder = new BarcodeDecoder();
            LuminanceSource luminanceSource = CreateLuminanceSource(image);
            Binarizer binarizer = CreateBinarizer(luminanceSource);
            BinaryBitmap bitmap = new BinaryBitmap(binarizer);
            BarcodeDecoderException savedException = null;
            List<Result> theResults = new List<Result>(1);
            try
            {
                // Look for multiple barcodes
                MultipleBarcodeDecoder multiDecoder = new GenericMultipleBarcodeDecoder(barcodeDecoder);
                Result[] results1 = multiDecoder.DecodeMultiple(bitmap, decodingOptions);
                if (results1 != null) theResults.AddRange(results1);
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
                    Result[] results2 = qrcodeMultiDecoder.DecodeMultiple(bitmap);
                    if (results2 != null) theResults.AddRange(results2);

                }
                catch (BarcodeDecoderException re)
                {
                    savedException = re;
                }
            }
            return theResults.ToArray();
        }



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
