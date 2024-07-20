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

using MessagingToolkit.Core.Service;

namespace MessagingToolkit.Core
{
    /// <summary>
    /// Provide support to cater for multiple gateways running
    /// at the same time
    /// </summary>
    public class MessageGatewayService : BaseGatewayService<MessageGatewayService>, IGatewayService
    {
        
        #region ====================== Constructor ================================================================
        
        /// <summary>
        /// Private constructor
        /// </summary>
        protected MessageGatewayService(): base()
        {
            // Use default router
            Router = new Router(this);

            // Use round robin load balancer
            LoadBalancer = new RoundRobinLoadBalancer(this);
            
        }

        #endregion ===============================================================================================



        #region ==================== Public Static Methods ========================================================

        /// <summary>
        /// Static factory to create a new gateway instance
        /// </summary>
        /// <returns>A new gateway instance</returns>
        public static MessageGatewayService NewInstance()
        {
            return new MessageGatewayService();
        }

        #endregion ===============================================================================================
    }
}
