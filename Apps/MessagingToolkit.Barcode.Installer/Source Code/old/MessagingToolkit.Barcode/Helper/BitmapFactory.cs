using System;


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


namespace MessagingToolkit.Barcode.Helper
{
#if SILVERLIGHT || NETFX_CORE
    /// <summary>
    /// Cross-platform factory for WriteableBitmaps
    /// </summary>
    public static class BitmapFactory
    {
        /// <summary>
        /// Creates a new WriteableBitmap of the specified width and height
        /// </summary>
        /// <remarks>For WPF the default DPI is 96x96 and PixelFormat is Pbgra32</remarks>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <returns></returns>
        public static WriteableBitmap New(int pixelWidth, int pixelHeight)
        {
#if SILVERLIGHT && !WPF
         return new WriteableBitmap(pixelWidth, pixelHeight);
#elif WPF
         return new WriteableBitmap(pixelWidth, pixelHeight, 96.0, 96.0, PixelFormats.Pbgra32, null);
#elif NETFX_CORE
         return new WriteableBitmap(pixelWidth, pixelHeight);

#endif
        }

#if WPF
      /// <summary>
      /// Converts the input BitmapSource to the Pbgra32 format WriteableBitmap which is internally used by the WriteableBitmapEx.
      /// </summary>
      /// <param name="source">The source bitmap.</param>
      /// <returns></returns>
      public static WriteableBitmap ConvertToPbgra32Format(BitmapSource source)
      {
         // Convert to Pbgra32 if it's a different format
         if (source.Format == PixelFormats.Pbgra32)
         {
            return new WriteableBitmap(source);
         }

         var formatedBitmapSource = new FormatConvertedBitmap();
         formatedBitmapSource.BeginInit();
         formatedBitmapSource.Source = source;
         formatedBitmapSource.DestinationFormat = PixelFormats.Pbgra32;
         formatedBitmapSource.EndInit();
         return new WriteableBitmap(formatedBitmapSource);
      }
#endif
    }
#endif

}