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

namespace MessagingToolkit.Core.Service
{
    /// <summary>
    /// Load balancing base class. Just choose the first available gateway
    /// for message sending. You can extend it to implement custom
    /// load balancing.
    /// </summary>
    public class LoadBalancer
    {
        /// <summary>
        /// Gateway service
        /// </summary>
        protected MessageGatewayService service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Gateway service</param>
        public LoadBalancer(MessageGatewayService service)
        {
            this.service = service;
        }
        /// <summary>
        /// Just return the first available candidate
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="candidates">Available candidates</param>
        /// <returns></returns>
        public virtual IGateway Balance(IMessage message, List<IGateway> candidates) 
        {
            return candidates.First();        
        }

        /// <summary>
        /// Derive the gateway without processing the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="candidates">The candidates.</param>
        /// <returns></returns>
        public virtual IGateway Current(IMessage message, List<IGateway> candidates)
        {
            return candidates.First();
        }
    }
}
