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


#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

#endif

#endif

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Barcode decoder interface
    /// </summary>
    public interface IBarcodeDecoder : IDecoder
    {
        /// <summary>
        /// Decodes the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// The result data or null
        /// </returns>
#if !SILVERLIGHT && !NETFX_CORE

        Result Decode(Bitmap image);
#else

        Result Decode(WriteableBitmap image);
#endif
        /// <summary>
        /// Locates and decodes a barcode in some format within an image. This method also accepts
        /// decoding options, each possibly associated to some data, which may help the implementation decode.
        /// </summary>
        /// <param name="image">image of barcode to decode</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        /// The result data or null
        /// </returns>
#if !SILVERLIGHT  && !NETFX_CORE
        Result Decode(Bitmap image, Dictionary<DecodeOptions, object> decodingOptions);
#else
        Result Decode(WriteableBitmap image, Dictionary<DecodeOptions, object> decodingOptions);
#endif

         /// <summary>
        /// Decodes the specified barcode bitmap which is given by a generic byte array with the order RGB24.
        /// </summary>
        /// <param name="rawRGB">The image as RGB24 array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="format">The format.</param>
        /// <returns>
        /// The result data or null
        /// </returns>
        Result Decode(byte[] rawRGB, int width, int height, RGBLuminanceSource.BitmapFormat format);


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
        /// The result data or null
        /// </returns>
        Result Decode(byte[] rawRGB, int width, int height, RGBLuminanceSource.BitmapFormat format, Dictionary<DecodeOptions, object> decodingOptions);

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
        Result[] DecodeMultiple(byte[] rawRGB, int width, int height, RGBLuminanceSource.BitmapFormat format);
       

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
        /// The result data  or a empty list
        /// </returns>
        Result[] DecodeMultiple(byte[] rawRGB, int width, int height, RGBLuminanceSource.BitmapFormat format, Dictionary<DecodeOptions, object> decodingOptions);


        /// <summary>
        /// Decodes the specified image.
        /// </summary>
        /// <param name="binaryBitmap">The image.</param>
        /// <returns>
        ///  Ghe result data  or a empty list
        /// </returns>
        Result[] DecodeMultiple(BinaryBitmap binaryBitmap);

        /// <summary>
        /// Locates and decodes a barcode in some format within an image. This method also accepts
        /// decoding options, each possibly associated to some data, which may help the implementation decode.
        /// </summary>
        /// <param name="binaryBitmap">image of barcode to decode</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        ///  The result data  or a empty list
        /// </returns>
        Result[] DecodeMultiple(BinaryBitmap binaryBitmap, Dictionary<DecodeOptions, object> decodingOptions);

    }
}
