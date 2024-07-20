#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif
namespace MessagingToolkit.Barcode
{
     /// <summary>
    /// The base class for all objects which encode/generate a barcode image.
    /// </summary>
    public interface IBarcodeEncoder
    {
        /// <summary>
        /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
        /// </summary>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <returns>Image representing the barcode.</returns>
        
#if !SILVERLIGHT
        Image Encode(BarcodeFormat format, string content);
#else
        WriteableBitmap Encode(BarcodeFormat format, string content);
#endif

    }
        
}
