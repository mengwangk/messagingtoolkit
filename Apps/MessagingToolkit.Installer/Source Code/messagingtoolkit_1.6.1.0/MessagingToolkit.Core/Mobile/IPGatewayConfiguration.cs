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

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// IP based gateway configuration
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    public class IPGatewayConfiguration: BaseGatewayConfiguration, IConfiguration
    {
        #region =============== Constructor ==================================================================

        /// <summary>
        /// Initializes a new instance of the <see cref="IPGatewayConfiguration"/> class.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="port">The port.</param>
       public IPGatewayConfiguration(string ipAddress, int port): base()
       {
           this.IPAddress = ipAddress;
           this.Port = port;
       }

       #endregion ============================================================================================


       #region ============== Public Properties ==============================================================

       /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>The IP address.</value>
        public string IPAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get;
            private set;
        }

        #endregion ============================================================================================



        #region ============== Factory method   ===============================================================
        /// <summary>
        /// Static factory to create the gateway configuration
        /// </summary>
        /// <returns>A new instance of gateway configuration</returns>
        public static IPGatewayConfiguration NewInstance(string ipAddress, int port)
        {
            return new IPGatewayConfiguration(ipAddress, port);
        }

        #endregion ===========================================================================================

    }
}
