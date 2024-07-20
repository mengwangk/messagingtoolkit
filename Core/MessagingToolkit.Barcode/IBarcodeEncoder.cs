using System;
using System.Collections.Generic;

#if PORTABLE

using MessagingToolkit.Barcode.Common;

#else

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
        
#if PORTABLE
        BitMatrix Encode(BarcodeFormat format, string content);
        BitMatrix Encode(BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions);
#else

#if !SILVERLIGHT && !NETFX_CORE
        Image Encode(BarcodeFormat format, string content);
        Image Encode(BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions);

#else
        WriteableBitmap Encode(BarcodeFormat format, string content);
        WriteableBitmap Encode(BarcodeFormat format, string content,Dictionary<EncodeOptions, object> encodingOptions);
#endif

#endif

    }
        
}
