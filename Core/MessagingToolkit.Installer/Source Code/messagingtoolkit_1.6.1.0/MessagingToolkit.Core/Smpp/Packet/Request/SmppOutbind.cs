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
using System.Text;
using MessagingToolkit.Core.Smpp.Utility;

namespace MessagingToolkit.Core.Smpp.Packet.Request
{
	/// <summary>
	/// Defines an outbind response(really a request TO us)from the SMSC.
	/// </summary>
	public class SmppOutbind : PDU
	{
		private string _SystemId = string.Empty;
		private string _Password = string.Empty;
		
		/// <summary>
		/// The ID of the SMSC.
		/// </summary>
		public string SystemId
		{
			get
			{
				return _SystemId;
			}
			
			set
			{
				_SystemId = (value == null) ? string.Empty : value;
			}
		}
		
		/// <summary>
		/// Password that the ESME can use for authentication.
		/// </summary>
		public string Password
		{
			get
			{
				return _Password;
			}
			
			set
			{
				_Password = (value == null) ? string.Empty : value;
			}
		}
		
		#region constructors
		
		/// <summary>
		/// Creates an outbind response Pdu.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppOutbind(byte[] incomingBytes): base(incomingBytes)
		{}
		
		/// <summary>
		/// Creates an outbind response Pdu.
		/// </summary>
		public SmppOutbind(): base()
		{}
		
		#endregion constructors
		
		/// <summary>
		/// Decodes the outbind response from the SMSC.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			byte[] remainder = BytesAfterHeader;
			SystemId = SmppStringUtil.GetCStringFromBody(ref remainder);
			Password = SmppStringUtil.GetCStringFromBody(ref remainder);
			//fill the TLV table if applicable
			TranslateTlvDataIntoTable(remainder);
		}
		
		/// <summary>
		/// Initializes this Pdu.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.Outbind;
		}
		
		///<summary>
		/// Gets the hex encoding(big-endian)of this Pdu.
		///</summary>
		///<return>The hex-encoded version of the Pdu</return>
		public override void ToMsbHexEncoding()
		{
			ArrayList pdu = GetPduHeader();
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(SystemId)));
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(Password)));
			PacketBytes = EncodePduForTransmission(pdu);
		}
	}
}
