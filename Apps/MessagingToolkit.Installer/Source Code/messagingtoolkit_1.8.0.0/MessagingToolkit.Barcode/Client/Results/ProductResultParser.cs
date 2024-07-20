using System;
using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using Result = MessagingToolkit.Barcode.Result;
using UPCEReader = MessagingToolkit.Barcode.OneD.UPCEDecoder;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Parses strings of digits that represent a UPC code.
    /// Modified: May 18 2012
    /// </summary>
    public sealed class ProductResultParser : ResultParser
    {
        /// <summary>
        ///  Treat all UPC and EAN variants as UPCs, in the sense that they are all product barcodes.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override ParsedResult Parse(Result result)
        {
            BarcodeFormat format = result.BarcodeFormat;
            if (!(format == BarcodeFormat.UPCA || format == BarcodeFormat.UPCE || format == BarcodeFormat.EAN8 ||
                format == BarcodeFormat.EAN13))
            {
                return null;
            }
            String rawText = GetMassagedText(result);
            int length = rawText.Length;
            for (int x = 0; x < length; x++)
            {
                char c = rawText[x];
                if (c < '0' || c > '9')
                {
                    return null;
                }
            }
            // Not actually checking the checksum again here    

            String normalizedProductID;
            // Expand UPC-E for purposes of searching
            if (format == BarcodeFormat.UPCE)
            {
                normalizedProductID = MessagingToolkit.Barcode.OneD.UPCEDecoder.ConvertUPCEtoUPCA(rawText);
            }
            else
            {
                normalizedProductID = rawText;
            }

            return new ProductParsedResult(rawText, normalizedProductID);
        }

    }
}