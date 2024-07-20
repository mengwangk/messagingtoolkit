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
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Service
{
    /// <summary>
    /// Routing class to decide which gateway to send message.
    /// More than 1 gateway may be used to send message.
    /// Custom routing rule can be defined by extending this class.
    /// </summary>
    public class Router
    {
        private List<IGateway> candidates;
        private List<IGateway> allowed;
        private MessageGatewayService service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Message gateway service</param>
        public Router(MessageGatewayService service)
        {
            this.candidates = new List<IGateway>();
            this.allowed = new List<IGateway>();
            this.service = service;
        }


        /// <summary>
        /// Message gateway service
        /// </summary>
        /// <value>Service</value>
        public MessageGatewayService Service
        {
            get
            {
                return this.service;
            }
        }


        /// <summary>
        /// Candidates for routing
        /// </summary>
        /// <value>List of gateways</value>
        public List<IGateway> Candidates
        {
            get
            {
                return this.candidates;
            }
        }

        /// <summary>
        /// Allowed gateways
        /// </summary>
        /// <value>List of gateways</value>
        public List<IGateway> Allowed
        {
            get
            {
                return this.allowed;
            }
        }

        /// <summary>
        /// Perform early-stage routing, pick gateways that meet minimal requirements
        /// to send message (for example are set to handle outbound messages).
        /// </summary>
        /// <param name="message"></param>
        protected void Preroute(IMessage message)
        {
            foreach (IGateway gateway in service.Gateways) 
            {
                // Gateway must be started
                if (gateway.Status != GatewayStatus.Started) continue;
                
                // At this moment, only mobile gateway is supported
                IMobileGateway mobileGateway = (IMobileGateway)gateway;
                if ( (mobileGateway.Attributes & GatewayAttribute.Send) == GatewayAttribute.Send)
                {
                    if (string.IsNullOrEmpty(message.GatewayId))
                    {
                        candidates.Add(gateway);
                    }
                    else
                    {
                        if (message.GatewayId.Equals(gateway.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            candidates.Add(gateway);
                        }
                    }
                }
            }         
        }


        /// <summary>
        /// Heart of routing and load balancing mechanism
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IGateway Route(IMessage message)
        {
            IGateway gtw = null;
            BeginRouting();
            Preroute(message);
            // Perform custom routing
            CustomRouting(message);
            // check if there are any gateways designated to send?
            if (Allowed.Count() > 0) 
                gtw = Service.LoadBalancer.Balance(message, Allowed);
            else
            {
                Sms sms = (Sms)message;
                throw new GatewayException(string.Format(Resources.NoRouteFoundException, sms.GatewayId, sms.DestinationAddress,sms.Content));
            }
            // finish
            FinishRouting();
            return gtw;          
        }

        /// <summary>
        /// Get the current route
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IGateway GetRoute(IMessage message)
        {
            IGateway gtw = null;
            BeginRouting();
            Preroute(message);
            // Perform custom routing
            CustomRouting(message);
            // check if there are any gateways designated to send?
            if (Allowed.Count() > 0)
                gtw = Service.LoadBalancer.Current(message, Allowed);
            else
            {
                Sms sms = (Sms)message;
                throw new GatewayException(string.Format(Resources.NoRouteFoundException, sms.GatewayId, sms.DestinationAddress, sms.Content));
            }
            // finish
            FinishRouting();
            return gtw;

        }

        /// <summary>
        /// Custom routing
        /// </summary>
        /// <param name="message"></param>
        public virtual void CustomRouting(IMessage message)
        {
            Allowed.AddRange(Candidates.AsEnumerable());
        }

        /// <summary>
        /// Prepare for routing
        /// </summary>
        protected void BeginRouting()
        {
            Candidates.Clear();
            Allowed.Clear();
        }


        /// <summary>
        /// Clean up after routing
        /// </summary>
        protected void FinishRouting()
        {
            Candidates.Clear();
            Allowed.Clear();
        }
    }
}
