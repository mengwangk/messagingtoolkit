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
using System.Reflection;

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Mobile gateway factory
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    public class MobileGatewayFactory : BaseGatewayFactory<MobileGatewayFactory>,
                                          IGatewayFactory
    {
        #region ================== Private Variable ================================================================

        /// <summary>
        /// Mobile gateway configuration
        /// </summary>
        private MobileGatewayConfiguration config;


        #endregion ================================================================================================

        #region ================== Constructor ====================================================================

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public MobileGatewayFactory(MobileGatewayConfiguration config)
        {
            this.config = config;
        }

        #endregion ===================================================================================================



        #region ================== Public Methods ====================================================================

        /// <summary>
        /// Instantiate a new mobile gateway
        /// </summary>
        /// <returns></returns>
        public IGateway Find()
        {
            IMobileGateway mobileGateway = null;
            if (!string.IsNullOrEmpty(config.Model))
            {
                // Try to see if there is custom gateway based on the model
                Type paramType = config.GetType();
                Assembly assembly = Assembly.GetAssembly(paramType);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.GetInterface(typeof(IMobileGateway).Name) != null)
                    {
                        string typeName = type.Name.ToLower().Replace("gateway", string.Empty);
                        string modelName = config.Model.ToLower();

                        if (typeName.Equals(modelName, StringComparison.OrdinalIgnoreCase))
                        {
                            mobileGateway = (IMobileGateway)Activator.CreateInstance(type, new object[] { config });
                            break;
                        }
                    }
                }
            }

            // If cannot find, then default to this one
            if (mobileGateway == null)
            {
                mobileGateway = new MobileGateway(config);
            }

            // Connect to the gateway
            if (mobileGateway.Connect())
            {
                // If successful, just return it
                return mobileGateway;
            }

            // Else throw the error encountered
            throw mobileGateway.LastError;
        }

        #endregion ===================================================================================================


        #region ================== Public Static Method ====================================================================

        /// <summary>
        /// Default mobile gateway
        /// </summary>
        /// <returns>Default gateway</returns>
        public static IMobileGateway Default
        {
            get
            {
                return new DefaultMobileGateway();
            }
        }


        /// <summary>
        /// Check if the gateway is valid.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <returns>true is valid, otherwise returns false.</returns>
        public static bool IsDefaultOrNull(IMobileGateway gateway)
        {
            return (gateway == null || (gateway is DefaultMobileGateway));
        }
        #endregion ===================================================================================================

    }
}
