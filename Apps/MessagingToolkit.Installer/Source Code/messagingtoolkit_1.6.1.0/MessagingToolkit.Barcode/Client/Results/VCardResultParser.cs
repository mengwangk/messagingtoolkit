using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Parses contact information formatted according to the VCard (2.1) format. This is not a complete
    /// implementation but should parse information as commonly encoded in 2D barcodes.
    /// 
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class VCardResultParser : ResultParser
    {
        private static readonly Regex BEGIN_VCARD = new Regex("BEGIN:VCARD", RegexOptions.IgnoreCase);
        private static readonly Regex VCARD_LIKE_DATE = new Regex("\\d{4}-?\\d{2}-?\\d{2}");
        private static readonly Regex CR_LF_SPACE_TAB = new Regex("\r\n[ \t]");
        private static readonly Regex NEWLINE_ESCAPE = new Regex("\\\\[nN]");
        private static readonly Regex VCARD_ESCAPES = new Regex("\\\\([,;\\\\])");
        private static readonly Regex EQUALS = new Regex("=");
        private static readonly Regex SEMICOLON = new Regex(";");
        private static readonly Regex UNESCAPED_SEMICOLONS = new Regex("(?<!\\\\);+");

        override public ParsedResult Parse(Result result)
        {
            // Although we should insist on the raw text ending with "END:VCARD", there's no reason
            // to throw out everything else we parsed just because this was omitted. In fact, Eclair
            // is doing just that, and we can't parse its contacts without this leniency.
            String rawText = result.Text;
            var m = BEGIN_VCARD.Match(rawText);
            if (!m.Success || m.Index != 0)
            {
                return null;
            }
            List<List<String>> names = MatchVCardPrefixedField("FN", rawText, true, false);
            if (names == null)
            {
                // If no display names found, look for regular name fields and format them
                names = MatchVCardPrefixedField("N", rawText, true, false);
                FormatNames(names);
            }
            List<List<String>> phoneNumbers = MatchVCardPrefixedField("TEL", rawText, true, false);
            List<List<String>> emails = MatchVCardPrefixedField("EMAIL", rawText, true, false);
            List<String> note = MatchSingleVCardPrefixedField("NOTE", rawText, false, false);
            List<List<String>> addresses = MatchVCardPrefixedField("ADR", rawText, true, true);
            List<String> org = MatchSingleVCardPrefixedField("ORG", rawText, true, false);
            List<String> birthday = MatchSingleVCardPrefixedField("BDAY", rawText, true, false);
            if (birthday != null && !IsLikeVCardDate(birthday[0]))
            {
                birthday = null;
            }
            List<String> title = MatchSingleVCardPrefixedField("TITLE", rawText, true, false);
            List<String> url = MatchSingleVCardPrefixedField("URL", rawText, true, false);
            List<String> instantMessenger = MatchSingleVCardPrefixedField("IMPP", rawText, true, false);
            return new AddressBookParsedResult(ToPrimaryValues(names),
                                               null,
                                               ToPrimaryValues(phoneNumbers),
                                               ToTypes(phoneNumbers),
                                               ToPrimaryValues(emails),
                                               ToTypes(emails),
                                               ToPrimaryValue(instantMessenger),
                                               ToPrimaryValue(note),
                                               ToPrimaryValues(addresses),
                                               ToTypes(addresses),
                                               ToPrimaryValue(org),
                                               ToPrimaryValue(birthday),
                                               ToPrimaryValue(title),
                                               ToPrimaryValue(url));
        }

        public static List<List<String>> MatchVCardPrefixedField(String prefix,
                                                                  String rawText,
                                                                  bool trim,
                                                                  bool parseFieldDivider)
        {
            List<List<String>> matches = null;
            int i = 0;
            int max = rawText.Length;

            while (i < max)
            {
                // At start or after newline, match prefix, followed by optional metadata 
                // (led by ;) ultimately ending in colon
                var matcher = new Regex("(?:^|\n)" + prefix + "(?:;([^:]*))?:", RegexOptions.IgnoreCase);

                if (i > 0)
                {
                    i--; // Find from i-1 not i since looking at the preceding character
                }
                var match = matcher.Match(rawText, i);
                if (!match.Success)
                {
                    break;
                }
                i = match.Index + match.Length;

                String metadataString = match.Groups[1].Value; // group 1 = metadata substring
                List<String> metadata = null;
                bool quotedPrintable = false;
                String quotedPrintableCharset = null;
                if (metadataString != null)
                {
                    foreach (String metadatum in SEMICOLON.Split(metadataString))
                    {
                        if (metadata == null)
                        {
                            metadata = new List<String>(1);
                        }
                        metadata.Add(metadatum);
                        String[] metadatumTokens = EQUALS.Split(metadatum, 2);
                        if (metadatumTokens.Length > 1)
                        {
                            String key = metadatumTokens[0];
                            String value = metadatumTokens[1];
                            if (String.Compare("ENCODING", key, StringComparison.OrdinalIgnoreCase) == 0 &&
                               String.Compare("QUOTED-PRINTABLE", value, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                quotedPrintable = true;
                            }
                            else if (String.Compare("CHARSET", key, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                quotedPrintableCharset = value;
                            }
                        }
                    }
                }

                int matchStart = i; // Found the start of a match here

                while ((i = rawText.IndexOf('\n', i)) >= 0)
                { // Really, end in \r\n
                    if (i < rawText.Length - 1 &&           // But if followed by tab or space,
                        (rawText[i + 1] == ' ' ||        // this is only a continuation
                         rawText[i + 1] == '\t'))
                    {
                        i += 2; // Skip \n and continutation whitespace
                    }
                    else if (quotedPrintable &&             // If preceded by = in quoted printable
                             (rawText[i - 1] == '=' || // this is a continuation
                              rawText[i - 2] == '='))
                    {
                        i++; // Skip \n
                    }
                    else
                    {
                        break;
                    }
                }

                if (i < 0)
                {
                    // No terminating end character? uh, done. Set i such that loop terminates and break
                    i = max;
                }
                else if (i > matchStart)
                {
                    // found a match
                    if (matches == null)
                    {
                        matches = new List<List<String>>(1); // lazy init
                    }
                    if (rawText[i - 1] == '\r')
                    {
                        i--; // Back up over \r, which really should be there
                    }
                    String element = rawText.Substring(matchStart, i - matchStart);
                    if (trim)
                    {
                        element = element.Trim();
                    }
                    if (quotedPrintable)
                    {
                        element = DecodeQuotedPrintable(element, quotedPrintableCharset);
                        if (parseFieldDivider)
                        {
                            element = UNESCAPED_SEMICOLONS.Replace(element, "\n").Trim();
                        }
                    }
                    else
                    {
                        if (parseFieldDivider)
                        {
                            element = UNESCAPED_SEMICOLONS.Replace(element, "\n").Trim();
                        }
                        element = CR_LF_SPACE_TAB.Replace(element, "");
                        element = NEWLINE_ESCAPE.Replace(element, "\n");
                        element = VCARD_ESCAPES.Replace(element, "$1");
                    }
                    if (metadata == null)
                    {
                        var matched = new List<String>(1);
                        matched.Add(element);
                        matches.Add(matched);
                    }
                    else
                    {
                        metadata.Insert(0, element);
                        matches.Add(metadata);
                    }
                    i++;
                }
                else
                {
                    i++;
                }

            }

            return matches;
        }

        private static String DecodeQuotedPrintable(String value, String charset)
        {
            int length = value.Length;
            var result = new StringBuilder(length);
            var fragmentBuffer = new MemoryStream();
            for (int i = 0; i < length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '\r':
                    case '\n':
                        break;
                    case '=':
                        if (i < length - 2)
                        {
                            char nextChar = value[i + 1];
                            if (nextChar == '\r' || nextChar == '\n')
                            {
                                // Ignore, it's just a continuation symbol
                            }
                            else
                            {
                                char nextNextChar = value[i + 2];
                                int firstDigit = ParseHexDigit(nextChar);
                                int secondDigit = ParseHexDigit(nextNextChar);
                                if (firstDigit >= 0 && secondDigit >= 0)
                                {
                                    fragmentBuffer.WriteByte((byte)((firstDigit << 4) | secondDigit));
                                } // else ignore it, assume it was incorrectly encoded
                                i += 2;
                            }
                        }
                        break;
                    default:
                        MaybeAppendFragment(fragmentBuffer, charset, result);
                        result.Append(c);
                        break;
                }
            }
            MaybeAppendFragment(fragmentBuffer, charset, result);
            return result.ToString();
        }

        private static void MaybeAppendFragment(MemoryStream fragmentBuffer,
                                                String charset,
                                                StringBuilder result)
        {
            if (fragmentBuffer.Length > 0)
            {
                byte[] fragmentBytes = fragmentBuffer.ToArray();
                String fragment;
                if (charset == null)
                {
#if !SILVERLIGHT
                    fragment = Encoding.Default.GetString(fragmentBytes);
#else
                    fragment = Encoding.UTF8.GetString(fragmentBytes, 0, fragmentBytes.Length);
#endif
                }
                else
                {
                    try
                    {
                        fragment = Encoding.GetEncoding(charset).GetString(fragmentBytes, 0, fragmentBytes.Length);
                    }
                    catch (Exception)
                    {
                        // Yikes, well try anyway:
                        // @Conversion
#if !SILVERLIGHT
                    fragment = Encoding.Default.GetString(fragmentBytes);
#else
                        fragment = Encoding.UTF8.GetString(fragmentBytes, 0, fragmentBytes.Length);
#endif
                    }
                }
                fragmentBuffer.Seek(0, SeekOrigin.Begin);
                fragmentBuffer.SetLength(0);
                result.Append(fragment);
            }
        }

        internal static List<String> MatchSingleVCardPrefixedField(String prefix,
                                                      String rawText,
                                                      bool trim,
                                                      bool parseFieldDivider)
        {
            List<List<String>> values = MatchVCardPrefixedField(prefix, rawText, trim, parseFieldDivider);
            return values == null || values.Count == 0 ? null : values[0];
        }

        private static String ToPrimaryValue(List<String> list)
        {
            return list == null || list.Count == 0 ? null : list[0];
        }

        private static String[] ToPrimaryValues(ICollection<List<String>> lists)
        {
            if (lists == null || lists.Count == 0)
            {
                return null;
            }
            List<String> result = new List<String>(lists.Count);
            foreach (var list in lists)
            {
                result.Add(list[0]);
            }
            return ToStringArray(result);
        }

        private static String[] ToTypes(ICollection<List<String>> lists)
        {
            if (lists == null || lists.Count == 0)
            {
                return null;
            }
            List<String> result = new List<String>(lists.Count);
            foreach (var list in lists)
            {
                String type = null;
                for (int i = 1; i < list.Count; i++)
                {
                    String metadatum = list[i];
                    int equals = metadatum.IndexOf('=');
                    if (equals < 0)
                    {
                        // take the whole thing as a usable label
                        type = metadatum;
                        break;
                    }
                    if (String.Compare("TYPE", metadatum.Substring(0, equals), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        type = metadatum.Substring(equals + 1);
                        break;
                    }
                }
                result.Add(type);
            }
            return ToStringArray(result);
        }

        private static bool IsLikeVCardDate(String value)
        {
            return value == null || VCARD_LIKE_DATE.Match(value).Success;
        }

        /**
         * Formats name fields of the form "Public;John;Q.;Reverend;III" into a form like
         * "Reverend John Q. Public III".
         *
         * @param names name values to format, in place
         */
        private static void FormatNames(IEnumerable<List<String>> names)
        {
            if (names != null)
            {
                foreach (var list in names)
                {
                    String name = list[0];
                    String[] components = new String[5];
                    int start = 0;
                    int end;
                    int componentIndex = 0;
                    while (componentIndex < components.Length - 1 && (end = name.IndexOf(';', start)) > 0)
                    {
                        components[componentIndex] = name.Substring(start, end - start);


                        componentIndex++;
                        start = end + 1;
                    }
                    components[componentIndex] = name.Substring(start);
                    StringBuilder newName = new StringBuilder(100);
                    MaybeAppendComponent(components, 3, newName);
                    MaybeAppendComponent(components, 1, newName);
                    MaybeAppendComponent(components, 2, newName);
                    MaybeAppendComponent(components, 0, newName);
                    MaybeAppendComponent(components, 4, newName);
                    list.Insert(0, newName.ToString().Trim());
                }
            }
        }

        private static void MaybeAppendComponent(String[] components, int i, StringBuilder newName)
        {
            if (components[i] != null)
            {
                newName.Append(' ');
                newName.Append(components[i]);
            }
        }
    }
}