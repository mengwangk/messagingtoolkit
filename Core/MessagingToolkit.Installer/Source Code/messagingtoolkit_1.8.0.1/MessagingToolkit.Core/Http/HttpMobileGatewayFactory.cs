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

using MessagingToolkit.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Http
{
     /// <summary>
    /// Mobile HTTP gateway factory to create the HTTP gateway.
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    public class HttpMobileGatewayFactory: BaseGatewayFactory<HttpMobileGatewayFactory>, IGatewayFactory
    {
        #region ================== Private Variable ================================================================

        /// <summary>
        /// Mobile gateway configuration
        /// </summary>
        private HttpMobileGatewayConfiguration config;


        #endregion ================================================================================================

        #region ================== Constructor ====================================================================

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public HttpMobileGatewayFactory(HttpMobileGatewayConfiguration config)
        {
            this.config = config;
        }

        #endregion ===================================================================================================



        #region ================== Public Methods ====================================================================
        
        /// <summary>
        /// Instantiate a new SMPP gateway
        /// </summary>
        /// <returns></returns>
        public IGateway Find()
        {
            IHttpMobileGateway mobileGateway = null;
            mobileGateway = new HttpMobileGateway(config);
            return mobileGateway;
        }

        #endregion ===================================================================================================


        #region ================== Public Static Method ====================================================================

        /// <summary>
        /// Default HTTP mobile gateway
        /// </summary>
        /// <returns>Default gateway</returns>
        public static IHttpMobileGateway Default
        {
            get
            {
                return new DefaultHttpMobileGateway();
            }
        }
        #endregion ===================================================================================================

    
    }
}
