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

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Mobile HTTP gateway factory to create the HTTP gateway.
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    public class HttpGatewayFactory : BaseGatewayFactory<HttpGatewayFactory>, IGatewayFactory
    {
        #region ================== Private Variable ================================================================

        /// <summary>
        /// Mobile gateway configuration
        /// </summary>
        private HttpGatewayConfiguration config;


        #endregion ================================================================================================

        #region ================== Constructor ====================================================================

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public HttpGatewayFactory(HttpGatewayConfiguration config)
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
            IHttpGateway httpGateway = new HttpGateway(config);

            // Return the instance upon successful connection
            if (httpGateway.Connect())
                return httpGateway;

            // Else throw the error encountered
            throw httpGateway.LastError;
        }

        #endregion ===================================================================================================


        #region ================== Public Static Method ====================================================================

        /// <summary>
        /// Default HTTP mobile gateway
        /// </summary>
        /// <returns>Default gateway</returns>
        public static IHttpGateway Default
        {
            get
            {
                return new DefaultHttpGateway();
            }
        }


        /// <summary>
        /// Check if the gateway is valid.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <returns>true is valid, otherwise returns false.</returns>
        public static bool IsDefaultOrNull(IHttpGateway gateway)
        {
            return (gateway == null || (gateway is DefaultHttpGateway));
        }
        #endregion ===================================================================================================


    }
}
