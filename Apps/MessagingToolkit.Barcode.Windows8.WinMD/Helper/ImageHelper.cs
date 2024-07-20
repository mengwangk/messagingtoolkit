using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Windows.Foundation;
#endif

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

namespace MessagingToolkit.Barcode.Helper
{

#if !SILVERLIGHT && !NETFX_CORE

    public static class ImageHelper
    {

        /// <summary>
        /// Converts an image to black and white
        /// </summary>
        /// <param name="Image">Image to change</param>
        /// <returns>A bitmap object of the black and white image</returns>
        public static Bitmap ConvertBlackAndWhite(Bitmap Image)
        {
            System.Drawing.Bitmap TempBitmap = Image;

            System.Drawing.Bitmap NewBitmap = new System.Drawing.Bitmap(TempBitmap.Width, TempBitmap.Height);
            System.Drawing.Graphics NewGraphics = System.Drawing.Graphics.FromImage(NewBitmap);
            float[][] FloatColorMatrix ={
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

            System.Drawing.Imaging.ColorMatrix NewColorMatrix = new System.Drawing.Imaging.ColorMatrix(FloatColorMatrix);
            System.Drawing.Imaging.ImageAttributes Attributes = new System.Drawing.Imaging.ImageAttributes();
            Attributes.SetColorMatrix(NewColorMatrix);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), 0, 0, TempBitmap.Width, TempBitmap.Height, System.Drawing.GraphicsUnit.Pixel, Attributes);
            Attributes.Dispose();
            NewGraphics.Dispose();
            return NewBitmap;
        }

        /// <summary>
        /// method for resizing an image
        /// </summary>
        /// <param name="img">the image to resize</param>
        /// <param name="percentage">Percentage of change (i.e for 105% of the original provide 105)</param>
        /// <returns></returns>
        public static Image Resize(Image img, int percentage)
        {
            //get the height and width of the image
            int originalW = img.Width;
            int originalH = img.Height;

            //get the new size based on the percentage change
            int resizedW = (int)(originalW * percentage);
            int resizedH = (int)(originalH * percentage);

            //create a new Bitmap the size of the new image
            Bitmap bmp = new Bitmap(resizedW, resizedH);
            //create a new graphic from the Bitmap
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //draw the newly resized image
            graphic.DrawImage(img, 0, 0, resizedW, resizedH);
            //dispose and free up the resources
            graphic.Dispose();
            //return the image
            return (Image)bmp;
        }
    }
#else


    #region Enum

    /// <summary>
    /// The blending mode.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// Alpha blendiing uses the alpha channel to combine the source and destination. 
        /// </summary>
        Alpha,

        /// <summary>
        /// Additive blending adds the colors of the source and the destination.
        /// </summary>
        Additive,

        /// <summary>
        /// Subtractive blending subtracts the source color from the destination.
        /// </summary>
        Subtractive,

        /// <summary>
        /// Uses the source color as a mask.
        /// </summary>
        Mask,

        /// <summary>
        /// Multiplies the source color with the destination color.
        /// </summary>
        Multiply,

        /// <summary>
        /// Ignores the specified Color
        /// </summary>
        ColorKeying,

        /// <summary>
        /// No blending just copies the pixels from the source.
        /// </summary>
        None
    }

    #endregion

#endif

}
