using System;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary> 
    /// Parses the "URLTO" result format, which is of the form "URLTO:[title]:[url]".
    /// This seems to be used sometimes, but I am not able to find documentation
    /// on its origin or official format?
    /// 
    /// Modified: May 18 2012
    /// </summary>
    sealed class URLTOResultParser : ResultParser
    {
        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            if (!rawText.StartsWith("urlto:") && !rawText.StartsWith("URLTO:"))
            {
                return null;
            }
            int titleEnd = rawText.IndexOf(':', 6);
            if (titleEnd < 0)
            {
                return null;
            }
            String title = (titleEnd <= 6) ? null : rawText.Substring(6, (titleEnd) - (6));
            String uri = rawText.Substring(titleEnd + 1);
            return new URIParsedResult(uri, title);
        }
    }
}