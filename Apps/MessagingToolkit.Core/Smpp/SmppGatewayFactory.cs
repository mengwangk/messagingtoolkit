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

namespace MessagingToolkit.Core.Smpp
{
    /// <summary>
    /// SMPP gateway factory to create the SMPP gateway
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    public class SmppGatewayFactory: BaseGatewayFactory<SmppGatewayFactory>,
                                          IGatewayFactory
    {
        #region ================== Private Variable ================================================================

        /// <summary>
        /// SMPP gateway configuration
        /// </summary>
        private SmppGatewayConfiguration config;


        #endregion ================================================================================================

        #region ================== Constructor ====================================================================

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public SmppGatewayFactory(SmppGatewayConfiguration config)
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
            ISmppGateway smppGateway = null;
            smppGateway = new SmppGateway(config);
            return smppGateway;
        }

        #endregion ===================================================================================================


        #region ================== Public Static Method ====================================================================

        /// <summary>
        /// Default SMPP gateway
        /// </summary>
        /// <returns>Default gateway</returns>
        public static ISmppGateway Default
        {
            get
            {
                return new DefaultSmppGateway();
            }
        }
        #endregion ===================================================================================================

    
    }
}
