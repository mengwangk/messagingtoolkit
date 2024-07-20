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
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.Packet.Request;

namespace MessagingToolkit.Core.Smpp.EventObjects 
{

	/// <summary>
	/// Class that defines the bind event.  Includes all the available
	/// mandatory and optional parameters in a bind_response.
	/// </summary>
	public class AlertEventArgs : SmppEventArgs 
	{
		private SmppAlertNotification _response;

		/// <summary>
		/// Allows access to the underlying Pdu.
		/// </summary>
		public SmppAlertNotification AlertPdu
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Sets up the AlertEventArgs.
		/// </summary>
		/// <param name="response">The SmppAlertNotification.</param>
		internal AlertEventArgs(SmppAlertNotification response): base(response)
		{
			_response = response;
		}
	}
}
