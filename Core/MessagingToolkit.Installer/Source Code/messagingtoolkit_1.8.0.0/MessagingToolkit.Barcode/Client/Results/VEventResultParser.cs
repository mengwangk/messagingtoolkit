using System;
using System.Collections.Generic;
using System.Globalization;

using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Partially implements the iCalendar format's "VEVENT" format for specifying a
    /// calendar event. See RFC 2445. This supports SUMMARY, LOCATION, GEO, DTSTART and DTEND fields.
    /// 
    /// Modified: May 18 2012
    /// </summary>
    public sealed class VEventResultParser : ResultParser
    {
        override public ParsedResult Parse(Result result)
        {
            String rawText = result.Text;
            if (rawText == null)
            {
                return null;
            }
            int vEventStart = rawText.IndexOf("BEGIN:VEVENT");
            if (vEventStart < 0)
            {
                return null;
            }

            String summary = MatchSingleVCardPrefixedField("SUMMARY", rawText, true);
            String start = MatchSingleVCardPrefixedField("DTSTART", rawText, true);
            if (start == null)
            {
                return null;
            }
            String end = MatchSingleVCardPrefixedField("DTEND", rawText, true);
            String duration = MatchSingleVCardPrefixedField("DURATION", rawText, true);
            String location = MatchSingleVCardPrefixedField("LOCATION", rawText, true);
            String organizer = StripMailto(MatchSingleVCardPrefixedField("ORGANIZER", rawText, true));

            String[] attendees = matchVCardPrefixedField("ATTENDEE", rawText, true);
            if (attendees != null)
            {
                for (int i = 0; i < attendees.Length; i++)
                {
                    attendees[i] = StripMailto(attendees[i]);
                }
            }
            String description = MatchSingleVCardPrefixedField("DESCRIPTION", rawText, true);

            String geoString = MatchSingleVCardPrefixedField("GEO", rawText, true);
            double latitude;
            double longitude;
            if (geoString == null)
            {
                latitude = Double.NaN;
                longitude = Double.NaN;
            }
            else
            {
                int semicolon = geoString.IndexOf(';');
                if (!Double.TryParse(geoString.Substring(0, semicolon), NumberStyles.Float, CultureInfo.InvariantCulture, out latitude))
                    return null;
                if (!Double.TryParse(geoString.Substring(semicolon + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                    return null;
            }

            try
            {
                return new CalendarParsedResult(summary,
                                                start,
                                                end,
                                                duration,
                                                location,
                                                organizer,
                                                attendees,
                                                description,
                                                latitude,
                                                longitude);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static String MatchSingleVCardPrefixedField(String prefix,
                                                            String rawText,
                                                            bool trim)
        {
            var values = VCardResultParser.MatchSingleVCardPrefixedField(prefix, rawText, trim, false);
            return values == null || values.Count == 0 ? null : values[0];
        }

        private static String[] matchVCardPrefixedField(String prefix, String rawText, bool trim)
        {
            List<List<String>> values = VCardResultParser.MatchVCardPrefixedField(prefix, rawText, trim, false);
            if (values == null || values.Count == 0)
            {
                return null;
            }
            int size = values.Count;
            String[] result = new String[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = values[i][0];
            }
            return result;
        }

        private static String StripMailto(String s)
        {
            if (s != null && (s.StartsWith("mailto:") || s.StartsWith("MAILTO:")))
            {
                s = s.Substring(7);
            }
            return s;
        }
    }
}