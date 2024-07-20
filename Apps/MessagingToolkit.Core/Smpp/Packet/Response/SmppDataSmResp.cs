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
using MessagingToolkit.Core.Smpp.Packet;

namespace MessagingToolkit.Core.Smpp.Packet.Response
{
	/// <summary>
	/// Response Pdu for the data_sm command.
	/// </summary>
	public class SmppDataSmResp : SmppSubmitSmResp
	{
		/// <summary>
		/// Enumerates the delivery failure types.
		/// </summary>
		public enum DeliveryFailureType : byte
		{
			/// <summary>
			/// DestinationUnavailable
			/// </summary>
			DestinationUnavailable = 0x00,
			/// <summary>
			/// DestinationAddressInvalid
			/// </summary>
			DestinationAddressInvalid = 0x01,
			/// <summary>
			/// PermanentNetworkError
			/// </summary>
			PermanentNetworkError = 0x02,
			/// <summary>
			/// TemporaryNetworkError
			/// </summary>
			TemporaryNetworkError = 0x03
		}
		
		#region optional parameters
		
		/// <summary>
		/// Indicates the reason for delivery failure.
		/// </summary>
		public DeliveryFailureType DeliveryFailureReason
		{
			get
			{
				return(DeliveryFailureType)
					GetOptionalParamBytes((ushort)
					PDU.OptionalParamCodes.DeliveryFailureReason)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(UInt16)PDU.OptionalParamCodes.DeliveryFailureReason,
					BitConverter.GetBytes(
					UnsignedNumConverter.SwapByteOrdering((byte)value)));
			}
		}
		
		/// <summary>
		/// Error code specific to a wireless network.  See SMPP spec section
		/// 5.3.2.31 for details.
		/// </summary>
		public string NetworkErrorCode
		{
			get
			{
				return GetOptionalParamString((ushort)
					PDU.OptionalParamCodes.NetworkErrorCode);
			}
			
			set
			{
				PduUtil.SetNetworkErrorCode(this, value);
			}
		}
		
		/// <summary>
		/// Text(ASCII)giving additional info on the meaning of the response.
		/// </summary>
		public string AdditionalStatusInfoText
		{
			get
			{
				return GetOptionalParamString((ushort)
					PDU.OptionalParamCodes.AdditionalStatusInfoText);
			}
			
			set
			{
				const int MAX_STATUS_LEN = 264;
				if(value == null)
				{
					SetOptionalParamString(
						(ushort)PDU.OptionalParamCodes.AdditionalStatusInfoText, string.Empty);
				}
				else if(value.Length <= MAX_STATUS_LEN)
				{
					SetOptionalParamString(
						(ushort)PDU.OptionalParamCodes.AdditionalStatusInfoText, value);
				}
				else
				{
					throw new ArgumentException(
						"additional_status_info_text must have length <= " + MAX_STATUS_LEN);
				}
			}
		}
		
		/// <summary>
		/// Indicates whether the Delivery Pending Flag was set.
		/// </summary>
		public DpfResultType DpfResult
		{
			get
			{
				return(DpfResultType)
					GetOptionalParamBytes((ushort)
					PDU.OptionalParamCodes.DpfResult)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(UInt16)PDU.OptionalParamCodes.DpfResult,
					BitConverter.GetBytes(
					UnsignedNumConverter.SwapByteOrdering((byte)value)));
			}
		}
		
		#endregion optional parameters
		
		#region constructors
		
		/// <summary>
		/// Creates a data_sm_resp Pdu.
		/// </summary>
		public SmppDataSmResp(): base()
		{}
		
		/// <summary>
		/// Creates a data_sm_resp Pdu.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppDataSmResp(byte[] incomingBytes): base(incomingBytes)
		{}
		
		#endregion constructors
		
		/// <summary>
		/// Initializes this Pdu for sending purposes.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.DataSmResp;
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
