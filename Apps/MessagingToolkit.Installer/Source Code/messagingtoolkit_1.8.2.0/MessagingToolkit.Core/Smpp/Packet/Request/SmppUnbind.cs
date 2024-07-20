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
using MessagingToolkit.Core.Smpp.Utility;

namespace MessagingToolkit.Core.Smpp.Packet.Request
{
	/// <summary>
	/// Class to issue an unbind request to the SMSC.
	/// </summary>
	public class SmppUnbind : PDU
	{	
		#region constructors
		
		/// <summary>
		/// Creates an unbind PDU.
		/// </summary>
		public SmppUnbind(): base()
		{}
		
		/// <summary>
		/// Creates an unbind PDU.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppUnbind(byte[] incomingBytes): base(incomingBytes)
		{}
		
		#endregion constructors
		
		/// <summary>
		/// Initializes this Pdu.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.Unbind;
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
		
		/// <summary>
		/// This decodes the query_sm Pdu.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			byte[] remainder = BytesAfterHeader;
			
			TranslateTlvDataIntoTable(remainder);
		}
	}
}
