using System;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Parses strings of digits that represent a ISBN.
    /// 
    /// Modified: May 18 2012
    /// </summary>
    internal class ISBNResultParser : ResultParser
	{
        /// <summary>
        /// See <a href="http://www.bisg.org/isbn-13/for.dummies.html">ISBN-13 For Dummies</a>
        /// 
        /// Modified: May 18 2010
        /// </summary>
        public override ParsedResult Parse(Result result)
        {
            BarcodeFormat format = result.BarcodeFormat;
            if (format != BarcodeFormat.EAN13)
            {
                return null;
            }
            String rawText = GetMassagedText(result);
            int length = rawText.Length;
            if (length != 13)
            {
                return null;
            }
            if (!rawText.StartsWith("978") && !rawText.StartsWith("979"))
            {
                return null;
            }

            return new ISBNParsedResult(rawText);
        }
	}
}