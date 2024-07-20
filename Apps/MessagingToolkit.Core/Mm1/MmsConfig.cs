//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Xml.Serialization;

namespace MessagingToolkit.Core.Mm1
{
    /// <summary>
    /// MMS configuration
    /// </summary>
    [Serializable]
    internal class MmsConfig
    {

        /// <summary>
        /// Default to 1 minute
        /// </summary>
        private const int DefaultHttpSocketTimeout = 60 * 1000;

        /// <summary>
        /// Default user agent
        /// </summary>
        private const string DefaultUserAgent = "mstoolkit/1.8";
        //private const string DefaultUserAgent = "Android-Mms/2.0";

        /// <summary>
        /// Default UA profile tag name
        /// </summary>
        private const string DefaultUaProfTagName = "x-wap-profile";

        /// <summary>
        /// Calling line id header
        /// </summary>
        public const string HeaderCallingLineId = "x-up-calling-line-id";

        /// <summary>
        /// MSISDN header
        /// </summary>
        public const string HeaderMsisdn = "X-MDN";

        /// <summary>
        /// Carrier magic header
        /// </summary>
        public const string HeaderCarrierMagic = "x-carrier-magic";

        /// <summary>
        /// Default carrier magic
        /// </summary>
        private const string DefaultCarrierMagic = "http://magic.google.com";

        /// <summary>
        /// Default UA profile URL
        /// </summary>
        private const string DefaultUaProfUrl = "http://www.google.com/oha/rdf/ua-profile-kila.xml";

        /// <summary>
        /// Default value for support for HTTP charset header
        /// </summary>
        private const bool DefaultSupportHttpCharsetHeader = false;

        /// <summary>
        /// Default HTTP parameters
        /// </summary>
        private const string DefaultHttpParams = "x-up-calling-line-id: ##LINE1##|x-carrier-magic: http://magic.google.com";

        /// <summary>
        /// Default constructor
        /// </summary>
        public MmsConfig()
        {
            this.HttpSocketTimeout = DefaultHttpSocketTimeout;
            this.UserAgent = DefaultUserAgent;
            this.UaProfileTagName = DefaultUaProfTagName;
            this.UaProfileUrl = DefaultUaProfUrl;
            this.SupportHttpCharsetHeader = DefaultSupportHttpCharsetHeader;
            this.HttpParams = DefaultHttpParams;
            this.CallingLineId = string.Empty;
            this.CarrierMagic = DefaultCarrierMagic;
            this.ApnUser = string.Empty;
            this.ApnPassword = string.Empty;
        }


        /// <summary>
        /// MMS HTTP socket timeout in milliseconds (int type)
        /// </summary>
        [XmlAttribute]
        public int HttpSocketTimeout { get; set; }

        /// <summary>
        /// MMS user agent
        /// </summary>
        [XmlAttribute]
        public String UserAgent { get; set; }

        /// <summary>
        /// UA profile tag name
        /// </summary>
        [XmlAttribute]
        public String UaProfileTagName { get; set; }


        /// <summary>
        /// UA profile URL
        /// </summary>
        [XmlAttribute]
        public String UaProfileUrl { get; set; }


        /// <summary>
        /// Flag indicating whether charset should be added to Content-Type header
        /// </summary>
        [XmlAttribute]
        public bool SupportHttpCharsetHeader { get; set; }


        /// <summary>
        /// Additional HTTP parameters
        /// </summary>
        [XmlAttribute]
        public string HttpParams { get; set; }


        /// <summary>
        /// Calling line id
        /// </summary>
        [XmlAttribute]
        public string CallingLineId { get; set; }


        /// <summary>
        /// Carrier magic
        /// </summary>
        [XmlAttribute]
        public string CarrierMagic { get; set; }

        /// <summary>
        /// APN user name
        /// </summary>
        [XmlAttribute]
        public string ApnUser { get; set; }


        /// <summary>
        /// APN user password
        /// </summary>
        [XmlAttribute]
        public string ApnPassword { get; set; }

    }
}
