using System.Collections.Generic;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// The base class for all objects which encode/generate a barcode image.
    /// </summary>
	public interface IEncoder
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
        BitMatrix Encode(string contents, BarcodeFormat format, int width, int height);

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
        BitMatrix Encode(string contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions);
	}
}