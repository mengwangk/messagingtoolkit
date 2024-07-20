using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Parses a "geo:" URI result, which specifies a location on the surface of
    /// the Earth as well as an optional altitude above the surface. See
    /// <a href="http://tools.ietf.org/html/draft-mayrhofer-geo-uri-00">
    /// http://tools.ietf.org/html/draft-mayrhofer-geo-uri-00</a>.
    /// 
    /// Modified: May 18 2010
    /// </summary>
    internal sealed class GeoResultParser : ResultParser
    {

        private static Regex GEO_URL_PATTERN = new Regex("geo:([\\-0-9.]+),([\\-0-9.]+)(?:,([\\-0-9.]+))?(?:\\?(.*))?", RegexOptions.IgnoreCase);

        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            var matcher = GEO_URL_PATTERN.Match(rawText);
            if (!matcher.Success)
            {
                return null;
            }



            String query = matcher.Groups[4].Value;

            double latitude;
            double longitude;
            double altitude;
            try
            {
                latitude = Double.Parse(matcher.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
                if (latitude > 90.0d || latitude < -90.0d)
                {
                    return null;
                }
                longitude = Double.Parse(matcher.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
                if (longitude > 180.0d || longitude < -180.0d)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(matcher.Groups[3].Value))
                {
                    altitude = 0.0d;
                }
                else
                {
                    altitude = Double.Parse(matcher.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
                    if (altitude < 0.0d)
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return new GeoParsedResult(latitude, longitude, altitude, query);
        }
    }
}
