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

namespace MessagingToolkit.Core.Smpp.Packet.Request
{
	/// <summary>
	/// Provides some common attributes for data_sm, submit_sm, submit_multi,
	/// and replace_sm.
	/// </summary>
	public abstract class MessageLcd4 : MessageLcd6
	{
		/// <summary>
		/// The registered delivery type of the message.
		/// </summary>
		protected RegisteredDeliveryType _RegisteredDelivery = PDU.RegisteredDeliveryType.None;
		
		#region properties
		
		/// <summary>
		/// The registered delivery type of the message.
		/// </summary>
		public RegisteredDeliveryType RegisteredDelivery
		{
			get
			{
				return _RegisteredDelivery;
			}
			set
			{
				_RegisteredDelivery =  value;
			}
		}
		
		#endregion properties
		
		#region constructors
		
		/// <summary>
		/// Groups construction tasks for subclasses.  Sets source address TON to international, 
		/// source address NPI to ISDN, source address to "", and registered delivery type to 
		/// none.
		/// </summary>
		protected MessageLcd4(): base()
		{}
		
		/// <summary>
		/// Creates a new MessageLcd4 for incoming PDUs.
		/// </summary>
		/// <param name="incomingBytes">The incoming bytes to decode.</param>
		protected MessageLcd4(byte[] incomingBytes): base(incomingBytes)
		{}
		
		#endregion constructors
	}
}
