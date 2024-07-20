using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
using System.Drawing;
using System.Drawing.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.DataMatrix.Encoder;

namespace MessagingToolkit.Barcode.DataMatrix
{
    /// <summary>
    /// DataMatrix barcode encoder
    /// </summary>
    internal sealed class DataMatrixEncoder : DataMatrixImageEncoder, IEncoder
    {
        /// <summary>
        /// Encode a barcode using the default settings.
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height)
        {
            throw new NotImplementedException("Not implementation. This is the default encoder");
        }


        /// <summary>
        /// Encodes the specified contents.
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {
            throw new NotImplementedException("Not implementation. This is the default encoder");
        }
      
    }
}
