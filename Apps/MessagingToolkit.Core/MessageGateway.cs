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
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core
{
    /// <summary>
    /// This is the entry point to instantiate the desired gateway.
    /// All access to the library must go through this library.
    /// </summary>
    /// <typeparam name="G">Gateway</typeparam>
    /// <typeparam name="C">Gateway configuration</typeparam>
    public class MessageGateway<G,C>
    {
        #region ====================== Constructor ================================================================
        
        /// <summary>
        /// Private constructor
        /// </summary>
        private MessageGateway() 
        {
        }

        #endregion ===============================================================================================

               

        #region ==================== Public Methods =============================================================

        /// <summary>
        /// Find and return the correct gateway instance based on the 
        /// configuration type
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <returns>The required gateway interface</returns>
        public G Find(C config)
        {  
            Type paramType = config.GetType();
            Assembly assembly = Assembly.GetAssembly(paramType);
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (type.GetInterface(typeof(IGatewayFactory).Name) != null)
                {
                    ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { paramType });
                    if (constructorInfo != null && constructorInfo.GetParameters()[0].ParameterType.Equals(paramType))
                    {
                        IGatewayFactory gatewayFactory = (IGatewayFactory)Activator.CreateInstance(type, new object[]{config});
                        return (G)gatewayFactory.Find();                       
                    }
                }
            }
            throw new GatewayException(Resources.GatewayFactoryNotFound);
        }

        #endregion ================================================================================================


        #region ==================== Public Static Methods ========================================================

        /// <summary>
        /// Static factory to create a new gateway instance
        /// </summary>
        /// <returns>A new gateway instance</returns>
        public static MessageGateway<G, C> NewInstance()
        {
            return new MessageGateway<G, C>();
        }

        #endregion ===============================================================================================
    }
}
