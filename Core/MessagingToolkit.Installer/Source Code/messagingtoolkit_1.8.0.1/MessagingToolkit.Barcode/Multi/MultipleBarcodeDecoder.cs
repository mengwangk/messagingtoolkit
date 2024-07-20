using System;
using System.Collections;
using System.Collections.Generic;

using Result = MessagingToolkit.Barcode.Result;
using BinaryBitmap = MessagingToolkit.Barcode.BinaryBitmap;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;

namespace MessagingToolkit.Barcode.Multi
{

    /// <summary>
    /// Implementation of this interface attempt to read several barcodes from one image.
    /// </summary>
    /// <seealso cref="MessagingToolkit.Barcode.IDecoder">
    /// </seealso>
    public interface MultipleBarcodeDecoder
    {
        Result[] DecodeMultiple(BinaryBitmap image);

        Result[] DecodeMultiple(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions);
    }
}