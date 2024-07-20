
using System;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 18 2010
    /// </summary>
    public sealed class GeoParsedResult : ParsedResult
    {
        private readonly double latitude;
        private readonly double longitude;
        private readonly double altitude;
        private readonly String query;

        internal GeoParsedResult(double latitude, double longitude, double altitude, String query)
            : base(ParsedResultType.Geo)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
            this.query = query;
        }

        public String GetGeoURI()
        {
            StringBuilder result = new StringBuilder();
            result.Append("geo:");
            result.Append(latitude);
            result.Append(',');
            result.Append(longitude);
            if (altitude > 0)
            {
                result.Append(',');
                result.Append(altitude);
            }
            if (query != null)
            {
                result.Append('?');
                result.Append(query);
            }
            return result.ToString();
        }


        /// <summary>
        /// Gets the latitude.
        /// </summary>
        /// <returns>
        /// latitude in degrees
        /// </returns>
        public double GetLatitude()
        {
            return latitude;
        }


        /// <summary>
        /// Gets the longitude.
        /// </summary>
        /// <returns>
        /// longitude in degrees
        /// </returns>
        public double GetLongitude()
        {
            return longitude;
        }


        /// <summary>
        /// Gets the altitude.
        /// </summary>
        /// <returns>
        /// altitude in meters. If not specified, in the geo URI, returns 0.0
        /// </returns>
        public double GetAltitude()
        {
            return altitude;
        }


        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <returns>
        /// query string associated with geo URI or null if none exists
        /// </returns>
        public String GetQuery()
        {
            return query;
        }

        public override String DisplayResult
        {
            get
            {
                StringBuilder result = new StringBuilder(20);
                result.Append(latitude);
                result.Append(", ");
                result.Append(longitude);
                if (altitude > 0.0d)
                {
                    result.Append(", ");
                    result.Append(altitude);
                    result.Append('m');
                }
                if (query != null)
                {
                    result.Append(" (");
                    result.Append(query);
                    result.Append(')');
                }
                return result.ToString();
            }
        }
	
    }
}