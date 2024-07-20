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
using System.Collections;
using MessagingToolkit.Core.Smpp.Packet;
	
namespace MessagingToolkit.Core.Smpp.Packet.Response
{
	/// <summary>
	/// Defines the response Pdu from an enquire_link.
	/// </summary>
	public class SmppEnquireLinkResp : PDU
	{
		#region constructors
		
		/// <summary>
		/// Creates an enquire_link Pdu.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppEnquireLinkResp(byte[] incomingBytes): base(incomingBytes)
		{}
		
		/// <summary>
		/// Creates an enquire_link Pdu.
		/// </summary>
		public SmppEnquireLinkResp(): base()
		{}
		
		#endregion constructors
		
		/// <summary>
		/// Decodes the enquire_link response from the SMSC.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			TranslateTlvDataIntoTable(BytesAfterHeader);
		}
		
		/// <summary>
		/// Initializes this Pdu for sending purposes.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.EnquireLinkResp;
		}
		
		///<summary>
		/// Gets the hex encoding(big-endian)of this Pdu.
		///</summary>
		///<return>The hex-encoded version of the Pdu</return>
		public override void ToMsbHexEncoding()
		{
			ArrayList pdu = GetPduHeader();
			
			PacketBytes = EncodePduForTransmission(pdu);
		}
	}
}
