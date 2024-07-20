using System;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Modified: May 10 2012
    /// </summary>
    public sealed class BookmarkDoCoMoResultParser : AbstractDoCoMoResultParser
    {

        public override ParsedResult Parse(Result result)
        {
            String rawText = result.Text;
            if (!rawText.StartsWith("MEBKM:"))
            {
                return null;
            }
            String title = MatchSingleDoCoMoPrefixedField("TITLE:", rawText, true);
            String[] rawUri = MatchDoCoMoPrefixedField("URL:", rawText, true);
            if (rawUri == null)
            {
                return null;
            }
            String uri = rawUri[0];
            return (URIResultParser.IsBasicallyValidURI(uri)) ? new URIParsedResult(uri, title) : null;
        }

    }
}