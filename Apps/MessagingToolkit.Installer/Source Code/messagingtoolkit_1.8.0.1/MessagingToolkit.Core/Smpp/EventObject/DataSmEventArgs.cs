//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright � TWIT88.COM.  All rights reserved.
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
	/// Class that defines a data_sm event.  
	/// </summary>
	public class DataSmEventArgs : SmppEventArgs 
	{
		private SmppDataSm _response;

		/// <summary>
		/// Allows access to the underlying Pdu.
		/// </summary>
		public SmppDataSm DataSmPdu
		{
			get
			{
				return _response;
			}
		}
		
		/// <summary>
		/// Creates a DataSmEventArgs.
		/// </summary>
		/// <param name="packet">The PDU that was received.</param>
		internal DataSmEventArgs(SmppDataSm packet): base(packet)
		{
			_response = packet;
		}
	}
}
