using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// <p>Abstract class representing the result of decoding a barcode, as more than
    /// a String -- as some type of structured data. This might be a subclass which represents
    /// a URL, or an e-mail address. <see cref="M:MessagingToolkit.Barcode.Client.Result.ResultParser.ParseResult(MessagingToolkit.Barcode.Client.Result.Result)"/> will turn a raw
    /// decoded string into the most appropriate type of structured representation.</p>
    /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
    /// on exception-based mechanisms during parsing.</p>
    /// 
    /// Modified: May 10 2012
    /// </summary>
    internal abstract class ResultParser
    {

        private static readonly ResultParser[] PARSERS = { 
                new BookmarkDoCoMoResultParser(), 
                new AddressBookDoCoMoResultParser(), 
                new EmailDoCoMoResultParser(), 
                new AddressBookAUResultParser(),
				new VCardResultParser(), 
                new BizcardResultParser(), 
                new VEventResultParser(), 
                new EmailAddressResultParser(), 
                new SMTPResultParser(), 
                new TelResultParser(), 
                new SMSMMSResultParser(),
				new SmsToMmsToResultParser(), 
                new GeoResultParser(), 
                new WifiResultParser(), 
                new URLTOResultParser(), 
                new URIResultParser(), 
                new ISBNResultParser(), 
                new ProductResultParser(),
				new ExpandedProductResultParser(), 
         };

#if !SILVERLIGHT && !NETFX_CORE
        private static readonly Regex DIGITS = new Regex("\\d*", RegexOptions.Compiled);
        private static readonly Regex ALPHANUM = new Regex("[a-zA-Z0-9]*", RegexOptions.Compiled);
        private static readonly Regex AMPERSAND = new Regex("&", RegexOptions.Compiled);
        private static readonly Regex EQUALS = new Regex("=", RegexOptions.Compiled);
#else
        private static readonly Regex DIGITS = new Regex("\\d*");
        private static readonly Regex ALPHANUM = new Regex("[a-zA-Z0-9]*");
        private static readonly Regex AMPERSAND = new Regex("&");
        private static readonly Regex EQUALS = new Regex("=");
      
#endif
        private static readonly String BYTE_ORDER_MARK = "\ufeff";
        /// <summary>
        /// Attempts to parse the raw <see cref="null"/>'s contents as a particular type
        /// of information (email, URL, etc.) and return a <see cref="T:Com.Google.Zxing.Client.Result.ParsedResult"/> encapsulating
        /// the result of parsing.
        /// </summary>
        ///
        public abstract ParsedResult Parse(Result theResult);

        protected static internal String GetMassagedText(Result result)
        {
            String text = result.Text;
            if (text.StartsWith(BYTE_ORDER_MARK))
            {
                text = text.Substring(1);
            }
            return text;
        }

        public static ParsedResult ParseResult(Result theResult)
        {
            /* foreach */
            foreach (ResultParser parser in PARSERS)
            {
                ParsedResult result = parser.Parse(theResult);
                if (result != null)
                {
                    return result;
                }
            }
            return new TextParsedResult(theResult.Text, null);
        }

        protected static internal void MaybeAppend(String val, StringBuilder result)
        {
            if (val != null)
            {
                result.Append('\n');
                result.Append(val);
            }
        }

        protected static internal void MaybeAppend(String[] val, StringBuilder result)
        {
            if (val != null)
            {
                /* foreach */
                foreach (String s in val)
                {
                    result.Append('\n');
                    result.Append(s);
                }
            }
        }

        protected static internal String[] MaybeWrap(String value_ren)
        {
            return (value_ren == null) ? null : new String[] { value_ren };
        }

        protected static internal String UnescapeBackslash(String escaped)
        {
            int backslash = escaped.IndexOf('\\');
            if (backslash < 0)
            {
                return escaped;
            }
            int max = escaped.Length;
            StringBuilder unescaped = new StringBuilder(max - 1);
            unescaped.Append(escaped.ToCharArray(), 0, backslash);
            bool nextIsEscaped = false;
            for (int i = backslash; i < max; i++)
            {
                char c = escaped[i];
                if (nextIsEscaped || c != '\\')
                {
                    unescaped.Append(c);
                    nextIsEscaped = false;
                }
                else
                {
                    nextIsEscaped = true;
                }
            }
            return unescaped.ToString();
        }

        protected static internal int ParseHexDigit(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            if (c >= 'a' && c <= 'f')
            {
                return 10 + (c - 'a');
            }
            if (c >= 'A' && c <= 'F')
            {
                return 10 + (c - 'A');
            }
            return -1;
        }

        protected static internal bool IsStringOfDigits(String val, int length)
        {
            return val != null && length == val.Length && DIGITS.Match(val).Success;
        }

        protected static internal bool IsSubstringOfDigits(String val, int offset, int length)
        {
            if (val == null)
            {
                return false;
            }
            int max = offset + length;
            return val.Length >= max && DIGITS.Match(val.Substring(offset, length)).Success;
        }

        protected static internal bool IsSubstringOfAlphaNumeric(String val, int offset, int length)
        {
            if (val == null)
            {
                return false;
            }
            int max = offset + length;
            return val.Length >= max && ALPHANUM.Match(val.Substring(offset, length)).Success;
        }

        static internal IDictionary<String, String> ParseNameValuePairs(String uri)
        {
            int paramStart = uri.IndexOf('?');
            if (paramStart < 0)
            {
                return null;
            }
            var result = new Dictionary<String, String>(3);
            foreach (var keyValue in AMPERSAND.Split(uri.Substring(paramStart + 1)))
            {
                AppendKeyValue(keyValue, result);
            }
            return result;
        }

        private static void AppendKeyValue(String keyValue, IDictionary<String, String> result)
        {
            String[] keyValueTokens = EQUALS.Split(keyValue, 2);
            if (keyValueTokens.Length == 2)
            {
                String key = keyValueTokens[0];
                String val = keyValueTokens[1];
                try
                {
                    // @Conversion - to further check
                    //val = System.Uri.UnescapeDataString(val);
                    val = UrlDecode(val);
                    result.Add(key, val);
                }
                catch
                {
                    // continue; invalid data such as an escape like %0t
                }
            }
        }


        protected static String UrlDecode(String escaped)
        {
            // No we can't use java.net.URLDecoder here. JavaME doesn't have it.
            if (escaped == null)
            {
                return null;
            }
            char[] escapedArray = escaped.ToCharArray();

            int first = FindFirstEscape(escapedArray);
            if (first < 0)
            {
                return escaped;
            }

            int max = escapedArray.Length;
            // final length is at most 2 less than original due to at least 1 unescaping
            var unescaped = new System.Text.StringBuilder(max - 2);
            // Can append everything up to first escape character
            unescaped.Append(escapedArray, 0, first);

            for (int i = first; i < max; i++)
            {
                char c = escapedArray[i];
                if (c == '+')
                {
                    // + is translated directly into a space
                    unescaped.Append(' ');
                }
                else if (c == '%')
                {
                    // Are there even two more chars? if not we will just copy the escaped sequence and be done
                    if (i >= max - 2)
                    {
                        unescaped.Append('%'); // append that % and move on
                    }
                    else
                    {
                        int firstDigitValue = ParseHexDigit(escapedArray[++i]);
                        int secondDigitValue = ParseHexDigit(escapedArray[++i]);
                        if (firstDigitValue < 0 || secondDigitValue < 0)
                        {
                            // bad digit, just move on
                            unescaped.Append('%');
                            unescaped.Append(escapedArray[i - 1]);
                            unescaped.Append(escapedArray[i]);
                        }
                        unescaped.Append((char)((firstDigitValue << 4) + secondDigitValue));
                    }
                }
                else
                {
                    unescaped.Append(c);
                }
            }
            return unescaped.ToString();
        }

        private static int FindFirstEscape(char[] escapedArray)
        {
            int max = escapedArray.Length;
            for (int i = 0; i < max; i++)
            {
                char c = escapedArray[i];
                if (c == '+' || c == '%')
                {
                    return i;
                }
            }
            return -1;
        }


        static internal String[] MatchPrefixedField(String prefix, String rawText, char endChar, bool trim)
        {
            List<String> matches = null;
            int i = 0;
            int max = rawText.Length;
            while (i < max)
            {

                i = rawText.IndexOf(prefix, i);
                if (i < 0)
                {
                    break;
                }
                i += prefix.Length; // Skip past this prefix we found to start
                int start = i; // Found the start of a match here
                bool more = true;
                while (more)
                {
                    i = rawText.IndexOf(endChar, i);
                    if (i < 0)
                    {
                        // No terminating end character? uh, done. Set i such that loop terminates and break
                        i = rawText.Length;
                        more = false;
                    }
                    else if (rawText[i - 1] == '\\')
                    {
                        // semicolon was escaped so continue
                        i++;
                    }
                    else
                    {
                        // found a match
                        if (matches == null)
                        {
                            matches = new List<String>(3); // lazy init
                        }
                        String element = UnescapeBackslash(rawText.Substring(start, (i) - (start)));
                        if (trim)
                        {
                            element = element.Trim();
                        }
                        if (element.Length > 0)
                        {
                            matches.Add(element);
                        }
                        i++;
                        more = false;
                    }
                }
            }
            if (matches == null || (matches.Count == 0))
            {
                return null;
            }
            return ToStringArray(matches);
        }

        static internal String MatchSinglePrefixedField(String prefix, String rawText, char endChar, bool trim)
        {
            String[] matches = MatchPrefixedField(prefix, rawText, endChar, trim);
            return (matches == null) ? null : matches[0];
        }

        static internal String[] ToStringArray(List<string> strings)
        {
            int size = strings.Count;
            String[] result = new String[size];
            for (int j = 0; j < size; j++)
            {
                result[j] = strings[j];
            }
            return result;
        }

    }
}
