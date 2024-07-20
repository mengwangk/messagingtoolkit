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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using MessagingToolkit.Core.Helper;


namespace MessagingToolkit.Core.Mobile.Http.Feature
{
    /// <summary>
    /// HTTP feature.
    /// </summary>
    /// <typeparam name="T">Derived feature.</typeparam>
    internal abstract class BaseHttpFeature<T>
    {
        #region ============== Private Variable ===================================================

        /// <summary>
        /// The parameter unique identifier
        /// </summary>
        private const string ParameterGuid = "guid";


        #endregion =================================================================================


        #region ====================== Constructor ================================================

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseHttpFeature()
        {
            this.IPAddress = string.Empty;
            this.Port = -1;
            this.Uri = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
        }

        #endregion ================================================================================



        #region =========== Public Properties ====================================================

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>
        /// The IP address.
        /// </value>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }


        #endregion ================================================================================


        #region =========== Protected Methods ======================================================

        /// <summary>
        /// Builds the URL.
        /// </summary>
        /// <returns></returns>
        protected string BuildUrl()
        {
            UriBuilder uriBuilder = new UriBuilder("http", this.IPAddress, this.Port, this.Uri);
            return uriBuilder.ToString();
        }

        /// <summary>
        /// Appends the GUID to URL to make it unique.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        protected string AppendGuidtoUrl(string url)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[ParameterGuid] = GatewayHelper.GenerateUniqueIdentifier();        
            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();
            return url;
        }
        #endregion =================================================================================


        #region =========== Public Methods =========================================================


        #endregion =================================================================================
    }
}
