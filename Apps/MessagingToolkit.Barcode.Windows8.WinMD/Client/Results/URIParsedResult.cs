using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class URIParsedResult : ParsedResult
    {
        private static readonly Regex USER_IN_HOST = new Regex(":/*([^/@]+)@[^/]+"
#if !SILVERLIGHT && !NETFX_CORE
, RegexOptions.Compiled);
#else
);
#endif

        private readonly String uri;
        private readonly String title;

        public String URI
        {
            get
            {
                return uri;
            }
        }

        public String Title
        {
            get
            {
                return title;
            }
        }

        /// <returns> true if the URI contains suspicious patterns that may suggest it intends to
        /// mislead the user about its true nature. At the moment this looks for the presence
        /// of user/password syntax in the host/authority portion of a URI which may be used
        /// in attempts to make the URI's host appear to be other than it is. Example:
        /// http://yourbank.com@phisher.com  This URI connects to phisher.com but may appear
        /// to connect to yourbank.com at first glance.
        /// </returns>
        public bool PossiblyMaliciousURI
        {
            get
            {
                return USER_IN_HOST.Match(uri).Success;
            }
        }

        override public String DisplayResult
        {
            get
            {
                var result = new System.Text.StringBuilder(30);
                MaybeAppend(title, result);
                MaybeAppend(uri, result);
                return result.ToString();
            }
        }

        public URIParsedResult(String uri, String title)
            : base(ParsedResultType.Uri)
        {
            this.uri = MassageURI(uri);
            this.title = title;
        }

        /// <summary> Transforms a string that represents a URI into something more proper, by adding or canonicalizing
        /// the protocol.
        /// </summary>
        private static String MassageURI(String uri)
        {
            int protocolEnd = uri.IndexOf(':');
            if (protocolEnd < 0)
            {
                // No protocol, assume http
                uri = "http://" + uri;
            }
            else if (isColonFollowedByPortNumber(uri, protocolEnd))
            {
                // Found a colon, but it looks like it is after the host, so the protocol is still missing
                uri = "http://" + uri;
            }
            return uri;
        }

        private static bool isColonFollowedByPortNumber(String uri, int protocolEnd)
        {
            int nextSlash = uri.IndexOf('/', protocolEnd + 1);
            if (nextSlash < 0)
            {
                nextSlash = uri.Length;
            }
            if (nextSlash <= protocolEnd + 1)
            {
                return false;
            }
            for (int x = protocolEnd + 1; x < nextSlash; x++)
            {
                if (uri[x] < '0' || uri[x] > '9')
                {
                    return false;
                }
            }
            return true;
        }
    }
}