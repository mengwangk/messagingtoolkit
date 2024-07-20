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
	/// This class defines an ESME originated deliver_sm_resp.
	/// </summary>
	public class SmppDeliverSmResp : PDU
	{
		#region constructors
		
		/// <summary>
		/// Creates a deliver_sm_resp Pdu.  Sets command status and command ID.
		/// </summary>
		public SmppDeliverSmResp(): base()
		{}
		
		/// <summary>
		/// Creates a deliver_sm_resp Pdu.  Sets command status and command ID.
		/// </summary>
		/// <param name="incomingBytes">The incoming bytes from the ESME.</param>
		public SmppDeliverSmResp(byte[] incomingBytes): base(incomingBytes)
		{}
		
		#endregion constructors
		
		/// <summary>
		/// Initializes this Pdu.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.DeliverSmResp;
		}
		
		/// <summary>
		/// Creates the byte encoding for this Pdu.
		/// </summary>
		public override void ToMsbHexEncoding()
		{
			ArrayList pdu = GetPduHeader();
			//the message_id is unused and is always set to null
			pdu.Add((byte)0x00);
			PacketBytes = EncodePduForTransmission(pdu);
		}
		
		/// <summary>
		/// Decodes the deliver_sm response from the SMSC.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			byte[] remainder = BytesAfterHeader;

			//fill the TLV table if applicable
			TranslateTlvDataIntoTable(remainder, 1);
		}
	}
}
