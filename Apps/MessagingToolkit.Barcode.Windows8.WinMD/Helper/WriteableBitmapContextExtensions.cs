﻿using System;

#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;

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

namespace MessagingToolkit.Barcode.Helper
{
#if SILVERLIGHT || NETFX_CORE
    /// <summary>
    /// Provides the WriteableBitmap context pixel data
    /// </summary>
    internal static partial class WriteableBitmapContextExtensions
    {
        /// <summary>
        /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
        /// </summary>
        /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static BitmapContext GetBitmapContext(this WriteableBitmap bmp)
        {
            return new BitmapContext(bmp);
        }

        /// <summary>
        /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
        /// </summary>
        /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="mode">The ReadWriteMode. If set to ReadOnly, the bitmap will not be invalidated on dispose of the context, else it will</param>
        /// <returns></returns>
        public static BitmapContext GetBitmapContext(this WriteableBitmap bmp, ReadWriteMode mode)
        {
            return new BitmapContext(bmp, mode);
        }
    }
#endif
}
