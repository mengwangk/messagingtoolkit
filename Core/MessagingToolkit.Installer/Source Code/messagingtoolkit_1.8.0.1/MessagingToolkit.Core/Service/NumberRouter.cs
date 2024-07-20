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
using MessagingToolkit.Core.Mobile.Message;

namespace MessagingToolkit.Core.Service
{
	/// <summary>
	/// Number routing based on prefix
	/// </summary>
	public class NumberRouter: Router
	{
		private Dictionary<string, string> assignments;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="service">Message gateway service</param>
		public NumberRouter(MessageGatewayService service)
			: base(service)
		{
			this.assignments = new Dictionary<string, string>();
		}

		/// <summary>
		/// Assign a prefix to a gateway
		/// </summary>
		/// <param name="prefix">Number prefix</param>
		/// <param name="gateway">Gateway</param>
		public void Assign(string prefix, IGateway gateway)
		{
			this.assignments.Add(prefix, gateway.Id);
		}

		/// <summary>
		/// Removes the specified gateway.
		/// </summary>
		/// <param name="gateway">The gateway.</param>
		public void Remove(IGateway gateway)
		{
			List<string> keys = assignments.Keys.ToList<string>();
			foreach (string key in keys)
			{
				string gatewayId = assignments[key];
				if (gateway.Id.Equals(gatewayId, StringComparison.OrdinalIgnoreCase))
				{
					assignments.Remove(key);
				}
			}
		}

	 

		/// <summary>
		/// Custom routing based on number prefix
		/// </summary>
		/// <param name="message">Message</param>
		public override void CustomRouting(IMessage message)
		{
			Sms sms = (Sms)message;
			string phoneNumber = sms.DestinationAddress;            
			foreach (string prefix in this.assignments.Keys) 
			{
				if (phoneNumber.StartsWith(prefix)) 
				{
					// Add gateway to allowed list
					string gatewayId = string.Empty;
					if (this.assignments.TryGetValue(prefix, out gatewayId )) 
					{
						IGateway gateway;
						if (Service.Find(gatewayId, out gateway)) 
						{
							Allowed.Add(gateway);
						}                       
					}                    
				}                
			}		  
		}
	}
}
