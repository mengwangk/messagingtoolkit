using System;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{
	
	/// <summary> 
    /// Parses a "tel:" URI result, which specifies a phone number.
    /// 
    /// Modified: May 18 2012
	/// </summary>
	sealed class TelResultParser:ResultParser
	{

        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            if (!rawText.StartsWith("tel:") && !rawText.StartsWith("TEL:"))
            {
                return null;
            }
            // Normalize "TEL:" to "tel:"
            String telURI = (rawText.StartsWith("TEL:")) ? "tel:" + rawText.Substring(4) : rawText;
            // Drop tel, query portion
            int queryStart = rawText.IndexOf('?', 4);
            String number = (queryStart < 0) ? rawText.Substring(4) : rawText.Substring(4, (queryStart) - (4));
            return new TelParsedResult(number, telURI, null);
        }
	}
}