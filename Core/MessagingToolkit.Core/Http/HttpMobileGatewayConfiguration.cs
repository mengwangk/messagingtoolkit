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
using System.Xml.Serialization;
using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Http
{
    /// <summary>
    /// Configuration for the HTTP mobile gateway.
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    [Serializable]
    public class HttpMobileGatewayConfiguration: BaseConfiguration, IConfiguration
    {
        
        #region =============== Constructor ==================================================================

        /// <summary>
        /// Private constructor
        /// </summary>
        private HttpMobileGatewayConfiguration()
            : base()
        {
            // Set default values
            this.IPAddress = string.Empty;
          
        }


        #endregion ============================================================================================



        #region ============== Public Properties ==============================================================


        /// <summary>
        /// IP address of the mobile phone.
        /// </summary>
        /// <value>IP address.</value>
        [XmlAttribute]
        public string IPAddress
        {
            get;
            set;
        }


        /// <summary>
        /// IP address of the mobile phone.
        /// </summary>
        /// <value>IP address.</value>
        [XmlAttribute]
        public string URIPattern
        {
            get;
            set;
        }

    
        #endregion ============================================================================================

        #region ============== Factory method   ===============================================================

        /// <summary>
        /// Static factory to create the mobile HTTP gateway configuration
        /// </summary>
        /// <returns>A new instance of mobile HTTP configuration</returns>
        public static HttpMobileGatewayConfiguration NewInstance()
        {
            return new HttpMobileGatewayConfiguration();
        }

        #endregion ===========================================================================================

    }
}
