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
using MessagingToolkit.Core.Smpp.Packet.Response;

namespace MessagingToolkit.Core.Smpp.EventObjects 
{

	/// <summary>
	/// Class that defines a cancel_sm_resp event.
	/// </summary>
	public class CancelEventArgs : SmppEventArgs 
	{
		private SmppCancelSmResp _response;

		/// <summary>
		/// Allows access to the underlying Pdu.
		/// </summary>
		public SmppCancelSmResp CancelPdu
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Sets up the CancelEventArgs.
		/// </summary>
		/// <param name="response">The SmppCancelSmResp.</param>
		internal CancelEventArgs(SmppCancelSmResp response): base(response)
		{
			_response = response;
		}
	}
}
