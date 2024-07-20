using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Modified: July 07 2012
    /// </summary>
    internal sealed class CalendarParsedResult : ParsedResult
    {
        private static readonly Regex RFC2445_DURATION = new Regex("P(?:(\\d+)W)?(?:(\\d+)D)?(?:T(?:(\\d+)H)?(?:(\\d+)M)?(?:(\\d+)S)?)?"
#if !(SILVERLIGHT || NETFX_CORE)
, RegexOptions.Compiled);
#else
	 );
#endif

        private static readonly long[] RFC2445_DURATION_FIELD_UNITS = {
	                                                                        7*24*60*60*1000L, // 1 week
	                                                                        24*60*60*1000L, // 1 day
	                                                                        60*60*1000L, // 1 hour
	                                                                        60*1000L, // 1 minute
	                                                                        1000L, // 1 second
	                                                                     };

        private static readonly Regex DATE_TIME = new Regex("[0-9]{8}(T[0-9]{6}Z?)?"
#if !(SILVERLIGHT || NETFX_CORE)
, RegexOptions.Compiled);
#else
);
#endif

        private const string DATE_FORMAT = "yyyyMMdd";

        private const string DATE_TIME_FORMAT = "yyyyMMdd'T'HHmmss";

        private readonly String summary;
        private readonly DateTime start;
        private readonly bool startAllDay;
        private readonly DateTime? end;
        private readonly bool endAllDay;
        private readonly String location;
        private readonly String organizer;
        private readonly String[] attendees;
        private readonly String description;
        private readonly double latitude;
        private readonly double longitude;


        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarParsedResult" /> class.
        /// </summary>
        /// <param name="summary">The summary.</param>
        /// <param name="startString">The start string.</param>
        /// <param name="endString">The end string.</param>
        /// <param name="durationString">The duration string.</param>
        /// <param name="location">The location.</param>
        /// <param name="organizer">The organizer.</param>
        /// <param name="attendees">The attendees.</param>
        /// <param name="description">The description.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <exception cref="ArgumentException"></exception>
        public CalendarParsedResult(String summary,
                                    String startString,
                                    String endString,
                                    String durationString,
                                    String location,
                                    String organizer,
                                    String[] attendees,
                                    String description,
                                    double latitude,
                                    double longitude)
            : base(ParsedResultType.Calendar)
        {
            this.summary = summary;
            try
            {
                this.start = ParseDate(startString);
            }
            catch (Exception pe)
            {
                throw new ArgumentException(pe.ToString());
            }

            if (endString == null)
            {
                long durationMS = ParseDurationMS(durationString);
                end = durationMS < 0L ? null : (DateTime?)start + new TimeSpan(0, 0, 0, 0, (int)durationMS);
            }
            else
            {
                try
                {
                    this.end = ParseDate(endString);
                }
                catch (Exception pe)
                {
                    throw new ArgumentException(pe.ToString());
                }
            }

            this.startAllDay = startString.Length == 8;
            this.endAllDay = endString != null && endString.Length == 8;
            this.location = location;
            this.organizer = organizer;
            this.attendees = attendees;
            this.description = description;
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public String Summary
        {
            get { return summary; }
        }

        /// <summary>
        /// Gets the start.
        /// </summary>
        public DateTime Start
        {
            get { return start; }
        }

        /// <summary>
        /// Determines whether [is start all day].
        /// </summary>
        /// <returns>if start time was specified as a whole day</returns>
        public bool IsStartAllDay()
        {
            return startAllDay;
        }

        /// <summary>
        /// May return null if the event has no duration.
        /// </summary>
        public DateTime? End
        {
            get { return end; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is end all day.
        /// </summary>
        /// <value>true if end time was specified as a whole day</value>
        public bool IsEndAllDay
        {
            get { return endAllDay; }
        }

        public String Location
        {
            get { return location; }
        }

        public String Organizer
        {
            get { return organizer; }
        }

        public String[] Attendees
        {
            get { return attendees; }
        }

        public String Description
        {
            get { return description; }
        }

        public double Latitude
        {
            get { return latitude; }
        }

        public double Longitude
        {
            get { return longitude; }
        }

        public override String DisplayResult
        {
            get
            {
                var result = new StringBuilder(100);
                MaybeAppend(summary, result);
                MaybeAppend(Format(startAllDay, start), result);
                MaybeAppend(Format(endAllDay, end), result);
                MaybeAppend(location, result);
                MaybeAppend(organizer, result);
                MaybeAppend(attendees, result);
                MaybeAppend(description, result);
                return result.ToString();
            }
        }

        /// <summary>
        /// Parses a string as a date. RFC 2445 allows the start and end fields to be of type DATE (e.g. 20081021)
        /// or DATE-TIME (e.g. 20081021T123000 for local time, or 20081021T123000Z for UTC).
        /// </summary>
        /// <param name="when">The string to parse</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if not a date formatted string</exception>
        private static DateTime ParseDate(String when)
        {
            if (!DATE_TIME.Match(when).Success)
            {
                throw new ArgumentException(String.Format("no date Format: {0}", when));
            }
            if (when.Length == 8)
            {
                // Show only year/month/day
                return DateTime.ParseExact(when, DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            else
            {
                // The when string can be local time, or UTC if it ends with a Z
                DateTime date;
                if (when.Length == 16 && when[15] == 'Z')
                {
                    date = DateTime.ParseExact(when.Substring(0, 15), DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                    date = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local);
                }
                else
                {
                    date = DateTime.ParseExact(when, DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
                }
                return date;
            }
        }

        private static String Format(bool allDay, DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            if (allDay)
                return date.Value.ToString("D", CultureInfo.CurrentCulture);
            return date.Value.ToString("F", CultureInfo.CurrentCulture);
        }

        private static long ParseDurationMS(String durationString)
        {
            if (durationString == null)
            {
                return -1L;
            }
            var m = RFC2445_DURATION.Match(durationString);
            if (!m.Success)
            {
                return -1L;
            }
            long durationMS = 0L;
            for (int i = 0; i < RFC2445_DURATION_FIELD_UNITS.Length; i++)
            {
                String fieldValue = m.Groups[i + 1].Value;
                if (!String.IsNullOrEmpty(fieldValue))
                {
                    durationMS += RFC2445_DURATION_FIELD_UNITS[i] * Int32.Parse(fieldValue);
                }
            }
            return durationMS;
        }
    }
}