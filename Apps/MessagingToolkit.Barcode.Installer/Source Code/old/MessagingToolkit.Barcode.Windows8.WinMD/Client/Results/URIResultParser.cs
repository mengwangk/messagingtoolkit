using System;
using System.Text.RegularExpressions;

using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Tries to parse results that are a URI of some kind.
    /// 
    /// Modified: July 07 2012
    /// </summary>
    sealed class URIResultParser : ResultParser
    {
        private const String ALPHANUM_PART = "[a-zA-Z0-9\\-]";
        private static readonly Regex URL_WITH_PROTOCOL_PATTERN = new Regex("[a-zA-Z0-9]{2,}:"
#if !SILVERLIGHT && !NETFX_CORE
, RegexOptions.Compiled);
#else
);
#endif
        private static readonly Regex URL_WITHOUT_PROTOCOL_PATTERN = new Regex(
             "(" + ALPHANUM_PART + "+\\.)+" + ALPHANUM_PART + "{2,}" + // host name elements
             "(:\\d{1,5})?" + // maybe port
             "(/|\\?|$)" // query, path or nothing
#if !SILVERLIGHT && !NETFX_CORE
                , RegexOptions.Compiled);
#else
);
#endif

        override public ParsedResult Parse(Result result)
        {
            String rawText = result.Text;
            // We specifically handle the odd "URL" scheme here for simplicity and add "URI" for fun
            // Assume anything starting this way really means to be a URI
            if (rawText.StartsWith("URL:") || rawText.StartsWith("URI:"))
            {
                return new URIParsedResult(rawText.Substring(4).Trim(), null);
            }
            rawText = rawText.Trim();
            return IsBasicallyValidURI(rawText) ? new URIParsedResult(rawText, null) : null;
        }

        internal static bool IsBasicallyValidURI(String uri)
        {
            var m = URL_WITH_PROTOCOL_PATTERN.Match(uri);
            if (m.Success && m.Index == 0)
            { // match at start only
                return true;
            }
            m = URL_WITHOUT_PROTOCOL_PATTERN.Match(uri);
            return m.Success && m.Index == 0;
        }
    }
}