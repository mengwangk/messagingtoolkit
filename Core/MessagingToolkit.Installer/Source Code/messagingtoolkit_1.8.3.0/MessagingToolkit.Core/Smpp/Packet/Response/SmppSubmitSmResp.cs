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

namespace MessagingToolkit.Core.Smpp.Packet.Response
{
	/// <summary>
	/// Defines the submit_sm response from the SMSC.
	/// </summary>
	public class SmppSubmitSmResp : PDU
	{
		private string _MessageId = string.Empty;
		private byte[] _ResponseAfterMsgId;
		
		/// <summary>
		/// The message ID(SMSC-assigned)of the submitted message.
		/// </summary>
		public string MessageId
		{
			get
			{
				return _MessageId;
			}
			
			set
			{
				_MessageId = (value == null) ? string.Empty : value;
			}
		}
		
		/// <summary>
		/// Accessor for the submit_multi to get at the response after the message ID.
		/// This is, in essence, set only after DecodeSmscResponse()in this base class
		/// is called.
		/// </summary>
		protected byte[] ResponseAfterMsgId
		{
			get
			{
				return _ResponseAfterMsgId;
			}
		}
		
		#region constructors
		
		/// <summary>
		/// Creates a submit_sm response Pdu.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppSubmitSmResp(byte[] incomingBytes): base(incomingBytes)
		{}
		
		/// <summary>
		/// Creates a submit_sm_resp
		/// </summary>
		public SmppSubmitSmResp(): base()
		{
		}
		
		#endregion constructors
		
		/// <summary>
		/// Decodes the submit response from the SMSC.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			DecodeNonTlv();
			//fill the TLV table if applicable
			TranslateTlvDataIntoTable(_ResponseAfterMsgId);
		}
		
		/// <summary>
		/// Decodes the non-TLV bytes.  Needed for the submit_multi_resp.
		/// </summary>
		protected void DecodeNonTlv()
		{
			//header
			byte[] remainder = BytesAfterHeader;
			MessageId = SmppStringUtil.GetCStringFromBody(ref remainder);
			_ResponseAfterMsgId = remainder;
		}
		
		/// <summary>
		/// Initializes this Pdu for sending purposes.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.SubmitSmResp;
		}
		
		///<summary>
		/// Gets the hex encoding(big-endian)of this Pdu.
		///</summary>
		///<return>The hex-encoded version of the Pdu</return>
		public override void ToMsbHexEncoding()
		{
			ArrayList pdu = GetPduHeader();
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(MessageId)));
			
			PacketBytes = EncodePduForTransmission(pdu);
		}
	}
}
