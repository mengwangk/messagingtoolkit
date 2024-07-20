using System.Collections.Generic;

#if !SILVERLIGHT

using System.Drawing;

#else

using System.Windows.Media;
using System.Windows.Media.Imaging;

#endif

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Barcode decoder interface
    /// </summary>
    public interface IBarcodeDecoder: IDecoder
    {
        /// <summary>
        /// Decodes the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// String which the barcode encodes
        /// </returns>
#if !SILVERLIGHT 

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
        /// String which the barcode encodes
        /// </returns>
#if !SILVERLIGHT 
        Result Decode(Bitmap image, Dictionary<DecodeOptions, object> decodingOptions);
#else
        Result Decode(WriteableBitmap image, Dictionary<DecodeOptions, object> decodingOptions);
#endif
    }
}
