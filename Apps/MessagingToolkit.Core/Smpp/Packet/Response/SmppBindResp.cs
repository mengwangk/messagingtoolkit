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
using System.Text;
using System.Collections;
using MessagingToolkit.Core.Smpp;
using MessagingToolkit.Core.Smpp.Utility;

namespace MessagingToolkit.Core.Smpp.Packet.Response
{
	/// <summary>
	/// Class to define an SMSC bind response.
	/// </summary>
	public class SmppBindResp : PDU
	{
		private string _SystemId = string.Empty;
		
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
		/// The SMPP version supported by SMSC.
		/// </summary>
		public string ScInterfaceVersion
		{
			get
			{
				return GetOptionalParamString((ushort)
					PDU.OptionalParamCodes.ScInterfaceVersion);
			}
			
			set
			{
				string version = value;
				if(value == null)
				{
					version = string.Empty;
				}
				SetOptionalParamString(
					(ushort)PDU.OptionalParamCodes.ScInterfaceVersion, version);
			}
		}
		
		#region constructors
		
		/// <summary>
		/// Creates a bind response Pdu.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppBindResp(byte[] incomingBytes): base(incomingBytes)
		{}
		
		/// <summary>
		/// Creates a bind_resp.
		/// </summary>
		public SmppBindResp(): base()
		{}
		
		#endregion constructors
		
		/// <summary>
		/// Decodes the bind response from the SMSC.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			byte[] remainder = BytesAfterHeader;
			SystemId = SmppStringUtil.GetCStringFromBody(ref remainder);
			//fill the TLV table if applicable
			TranslateTlvDataIntoTable(remainder);
		}
		
		/// <summary>
		/// Initializes this Pdu for sending purposes.  Note that this sets the bind type to 
		/// transceiver so you will want to change this if you are not dealing with a 
		/// transceiver.  This also sets system type to an empty string.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.BindTransceiverResp;
		}
		
		///<summary>
		/// Gets the hex encoding(big-endian)of this Pdu.
		///</summary>
		///<return>The hex-encoded version of the Pdu</return>
		public override void ToMsbHexEncoding()
		{
			ArrayList pdu = GetPduHeader();
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(SystemId)));
			
			PacketBytes = EncodePduForTransmission(pdu);
		}
	}
}
