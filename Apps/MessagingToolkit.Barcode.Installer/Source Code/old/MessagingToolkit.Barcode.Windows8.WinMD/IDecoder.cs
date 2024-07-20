using System.Collections.Generic;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// Implementations of this interface can decode an image of a barcode in some format into
    /// the String it encodes. For example, QRCodeReader can
    /// decode a QR code. The decoder may optionally receive options from the caller which may help
    /// it decode more quickly or accurately.
    /// </summary>
	internal interface IDecoder
	{
        /// <summary>
        /// Decodes the specified image.
        /// </summary>
        /// <param name="binaryBitmap">The image.</param>
        /// <returns>
        /// The result data or null
        /// </returns>
		Result Decode(BinaryBitmap binaryBitmap);

        /// <summary>
        /// Locates and decodes a barcode in some format within an image. This method also accepts
        /// decoding options, each possibly associated to some data, which may help the implementation decode.
        /// </summary>
        /// <param name="binaryBitmap">image of barcode to decode</param>
        /// <param name="decodingOptions">The meaning of the data depends upon the decoding options type. The implementation may or may not do
        /// anything with these options.</param>
        /// <returns>
        /// The result data or null
        /// </returns>
        Result Decode(BinaryBitmap binaryBitmap, IDictionary<DecodeOptions, object> decodingOptions);

        /// <summary>
        /// Resets any internal state the implementation has after a decode, to prepare it
        /// for reuse.
        /// </summary>
        void Reset();
	}
}