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
    /// Round robin load balancer 
    /// </summary>
    public sealed class RoundRobinLoadBalancer: LoadBalancer
    {
        private int currentGateway;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Message gateway service</param>
        public RoundRobinLoadBalancer(MessageGatewayService service): base(service)
        {
            this.currentGateway = 0;            
        }

        /// <summary>
        /// Balance the load
        /// </summary>
        /// <param name="message"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        public override IGateway Balance(MessagingToolkit.Core.Base.IMessage message, List<IGateway> candidates)
        {
            if (this.currentGateway >= candidates.Count()) this.currentGateway = 0;
            return (candidates.ElementAt(this.currentGateway++));
        }
    }
}
