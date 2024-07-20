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
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet;
using System.Collections;

namespace MessagingToolkit.Core.Smpp.Packet.Request
{
	/// <summary>
	/// This class defines a deliver_sm that is SMSC generated.  This does
	/// NOT handle anything other than strings in the short message.
	/// </summary>
	public class SmppDeliverSm : PDU
	{
		#region private fields
		
		private string _ServiceType = string.Empty;
		private TonType _SourceAddressTon = TonType.International;
		private NpiType _SourceAddressNpi = NpiType.ISDN;
		private string _SourceAddress = string.Empty;
		private TonType _DestinationAddressTon = TonType.International;
		private NpiType _DestinationAddressNpi = NpiType.ISDN;
		private string _DestinationAddress = string.Empty;
		private byte _EsmClass = 0;
		private SmppVersionType _ProtocolId = SmppVersionType.Version3_4;
		private PriorityType _PriorityFlag = PriorityType.Lowest;
		private RegisteredDeliveryType _RegisteredDelivery = RegisteredDeliveryType.None;
		private DataCodingType _DataCoding = DataCodingType.SMSCDefault;
		private byte _SmLength = 0;
		private string _ShortMessage = null;
		
		#endregion private fields
		
		/// <summary>
		/// Used to indicate the SMS Application service associated with the message.
		/// If this is unknown, null is returned.
		/// </summary>
		public string ServiceType
		{
			get
			{
				return _ServiceType;
			}
			
			set
			{
				_ServiceType = (value == null) ? string.Empty : value;
			}
		}
		
		/// <summary>
		/// Type of Number for source address.
		/// </summary>
		public TonType SourceAddressTon
		{
			get
			{
				return _SourceAddressTon;
			}
			
			set
			{
				_SourceAddressTon = value;
			}
		}
		
		/// <summary>
		/// Numbering Plan Indicator for source address.
		/// </summary>
		public NpiType SourceAddressNpi
		{
			get
			{
				return _SourceAddressNpi;
			}
			
			set
			{
				_SourceAddressNpi = value;
			}
		}
		
		/// <summary>
		/// Address of origination entity.
		/// </summary>
		public string SourceAddress
		{
			get
			{
				return _SourceAddress;
			}
			
			set
			{
				_SourceAddress = (value == null) ? string.Empty : value;
			}
		}
		
		/// <summary>
		/// Type of number of destination entity.
		/// </summary>
		public TonType DestinationAddressTon
		{
			get
			{
				return _DestinationAddressTon;
			}
			
			set
			{
				_DestinationAddressTon = value;
			}
		}
		
		/// <summary>
		/// Numbering Plan Indicator of destination entity.
		/// </summary>
		public NpiType DestinationAddressNpi
		{
			get
			{
				return _DestinationAddressNpi;
			}
			
			set
			{
				_DestinationAddressNpi = value;
			}
		}
		
		/// <summary>
		/// Destination address of entity.
		/// </summary>
		public string DestinationAddress
		{
			get
			{
				return _DestinationAddress;
			}
			
			set
			{
				_DestinationAddress = (value == null) ? string.Empty : value;
			}
		}
		
		/// <summary>
		/// Indicates Message Mode and Message Type.  See the SMSC
		/// version 3.4 specification for details on this.
		/// </summary>
		public byte EsmClass
		{
			get
			{
				return _EsmClass;
			}
			
			set
			{
				_EsmClass = value;
			}
		}
		
		/// <summary>
		/// Protocol Identifier; network specific.
		/// </summary>
		public SmppVersionType ProtocolId
		{
			get
			{
				return _ProtocolId;
			}
			
			set
			{
				_ProtocolId = value;
			}
		}
		
		/// <summary>
		/// Designates the priority level of the message.
		/// </summary>
		public PriorityType PriorityFlag
		{
			get
			{
				return _PriorityFlag;
			}
			
			set
			{
				_PriorityFlag = value;
			}
		}
		
		/// <summary>
		/// Use this to indicate if you want delivery confirmation.
		/// </summary>
		public RegisteredDeliveryType RegisteredDelivery
		{
			get
			{
				return _RegisteredDelivery;
			}
			
			set
			{
				_RegisteredDelivery = value;
			}
		}
		
		/// <summary>
		/// Indicates the encoding scheme of the short message.
		/// </summary>
		public DataCodingType DataCoding
		{
			get
			{
				return _DataCoding;
			}
			
			set
			{
				_DataCoding = value;
			}
		}
		
		/// <summary>
		/// Short message length in octets(bytes for x86).
		/// </summary>
		public byte SmLength
		{
			get
			{
				return _SmLength;
			}
		}
		
		/// <summary>
		/// The short message for this Pdu.  This holds up to 160 characters.
		/// If the message is longer, the MessagePayload property will be used.
		/// If this is the case, the short message length will be zero.  Note
		/// that both the ShortMessage and MessagePayload cannot be used
		/// simultaneously.
		/// </summary>
		public string ShortMessage
		{
			get
			{
				return _ShortMessage;
			}
			
			set
			{
				_ShortMessage = value;
			}
		}
		
		#region optional parameters
		
		/// <summary>
		/// The message reference number assigned by the ESME.
		/// </summary>
		public UInt16 UserMessageReference
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)OptionalParamCodes.UserMessageReference);
			}
			
			set
			{
				SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.UserMessageReference, value);
			}
		}
		
		/// <summary>
		/// The port number associated with the source address of the message.  This
		/// parameter will be present for WAP applications.
		/// </summary>
		public UInt16 SourcePort
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)OptionalParamCodes.SourcePort);
			}
			
			set
			{
				SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.SourcePort, value);
			}
		}
		
		/// <summary>
		/// The port number associated with the destination address of the message.  This
		/// parameter will be present for WAP applications.
		/// </summary>
		public UInt16 DestinationPort
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)OptionalParamCodes.DestinationPort);
			}
			
			set
			{
				SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.DestinationPort, value);
			}
		}
		
		/// <summary>
		/// The reference number for a particular concatenated short message.
		/// </summary>
		public UInt16 SarMsgRefNumber
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)OptionalParamCodes.SarMsgRefNum);
			}
			
			set
			{
				SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.SarMsgRefNum, value);
			}
		}
		
		/// <summary>
		/// Total number of short message fragments within the concatenated short message.
		/// </summary>
		public byte SarTotalSegments
		{
			get
			{
				return GetOptionalParamBytes((ushort)OptionalParamCodes.SarTotalSegments)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.SarTotalSegments, new Byte[] {value});
			}
		}
		
		/// <summary>
		/// The sequence number of a particular short message fragment within the 
		/// concatenated short message.
		/// </summary>
		public byte SarSegmentSeqnum
		{
			get
			{
				return GetOptionalParamBytes((ushort)OptionalParamCodes.SarSegmentSeqNum)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.SarSegmentSeqNum, new Byte[] {value});
			}
		}
		
		/// <summary>
		/// A user response code. The actual response codes are SMS application specific.
		/// </summary>
		public byte UserResponseCode
		{
			get
			{
				return GetOptionalParamBytes((ushort)OptionalParamCodes.UserResponseCode)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.UserResponseCode, new Byte[] {value});
			}
		}
		
		/// <summary>
		/// Indicates a level of privacy associated with the message.
		/// </summary>
		public PrivacyType PrivacyIndicator
		{
			get
			{
				return(PrivacyType)
					GetOptionalParamBytes((ushort)OptionalParamCodes.PrivacyIndicator)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.PrivacyIndicator, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Defines the type of payload(e.g. WDP, WCMP, etc.)
		/// </summary>
		public PayloadTypeType PayloadType
		{
			get
			{
				return(PayloadTypeType)
					GetOptionalParamBytes((ushort)OptionalParamCodes.PayloadType)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.PayloadType, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// This can hold up to 64K octets of short message data.
		/// The actual limit is network/SMSC dependent.
		/// </summary>
		public string MessagePayload
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.MessagePayload);
			}
			
			set
			{
				PduUtil.SetMessagePayload(this, value);
			}
		}
		
		/// <summary>
		/// Associates a callback number with a message.  See section 5.3.2.36 of the
		/// SMPP spec for details.  This must be between 4 and 19 characters in length.
		/// </summary>
		public string CallbackNum
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.CallbackNum);
			}
			
			set
			{
				PduUtil.SetCallbackNum(this, value);
			}
		}
		
		/// <summary>
		/// Specifies a source subaddress associated with the originating entity.
		/// See section 5.3.2.15 of the SMPP spec for details on setting this parameter.
		/// </summary>
		public string SourceSubaddress
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.SourceSubAddress);
			}
			
			set
			{
				PduUtil.SetSourceSubaddress(this, value);
			}
		}
		
		/// <summary>
		/// Specifies a source subaddress associated with the receiving entity.
		/// See section 5.3.2.15 of the SMPP spec for details on setting this parameter.
		/// </summary>
		public string DestinationSubaddress
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.DestSubAddress);
			}
			
			set
			{
				PduUtil.SetDestSubaddress(this, value);
			}
		}
		
		/// <summary>
		/// The language of the short message.
		/// </summary>
		public LanguageType LanguageIndicator
		{
			get
			{
				return(LanguageType)
					GetOptionalParamBytes((ushort)OptionalParamCodes.LanguageIndicator)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.LanguageIndicator, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// From the SMPP spec:
		/// The its_session_info parameter is a required parameter for the CDMA Interactive
		/// Teleservice as defined by the Korean PCS carriers [KORITS]. It contains control
		/// information for the interactive session between an MS and an ESME.
		///
		/// See section 5.3.2.43 of the SMPP spec for how to set this.
		/// </summary>
		public string ItsSessionInfo
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.ItsSessionInfo);
			}
			
			set
			{
				PduUtil.SetItsSessionInfo(this, value);
			}
		}
		
		/// <summary>
		/// Network Error Code.  May be present for Intermediate Notifications
		/// and SMSC Delivery Receipts.  See SMPP spec 5.3.2.31 for details.
		/// </summary>
		public string NetworkErrorCode
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.NetworkErrorCode);
			}
			
			set
			{
				PduUtil.SetNetworkErrorCode(this, value);
			}
		}
		
		/// <summary>
		/// Indicates to the ESME the final message state for an SMSC Delivery Receipt.
		/// </summary>
		public MessageStateType MessageState
		{
			get
			{
				return(MessageStateType)
					GetOptionalParamBytes((ushort)OptionalParamCodes.MessageState)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.MessageState, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Indicates the ID of the message being receipted in an SMSC Delivery Receipt.
		/// </summary>
		public string ReceiptedMessageId
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.ReceiptedMessageId);
			}
			
			set
			{
				PduUtil.SetReceiptedMessageId(this, value);
			}
		}
		
		#endregion optional parameters
		
		#region constructors
		
		/// <summary>
		/// Creates a deliver_sm Pdu.
		/// </summary>
		/// <param name="incomingBytes">The bytes received from an ESME.</param>
		public SmppDeliverSm(byte[] incomingBytes): base(incomingBytes)
		{}
		
		/// <summary>
		/// Creates a deliver_sm Pdu.
		/// </summary>
		public SmppDeliverSm(): base()
		{}
		
		#endregion constructors
		
		/// <summary>
		/// This decodes the deliver_sm Pdu.  The Pdu has basically the same format as
		/// the submit_sm Pdu, but in this case it is a response.
		/// </summary>
		protected override void DecodeSmscResponse()
		{
			byte[] remainder = BytesAfterHeader;
			ServiceType = SmppStringUtil.GetCStringFromBody(ref remainder);
			SourceAddressTon = (TonType)remainder[0];
			SourceAddressNpi = (NpiType)remainder[1];
			SourceAddress = SmppStringUtil.GetCStringFromBody(ref remainder, 2);
			DestinationAddressTon = (TonType)remainder[0];
			DestinationAddressNpi = (NpiType)remainder[1];
			DestinationAddress = SmppStringUtil.GetCStringFromBody(ref remainder, 2);
			EsmClass = remainder[0];
			ProtocolId = (SmppVersionType)remainder[1];
			PriorityFlag = (PriorityType)remainder[2];
			//schedule_delivery_time and validity_period are null, so don't bother
			//reading them
			RegisteredDelivery = (RegisteredDeliveryType)remainder[5];
			//replace_if_present is always null, so don't bother reading it
			DataCoding = (DataCodingType)remainder[7];
			//sm_default_msg_id is always null, so don't bother reading it
			_SmLength = remainder[9];
			ShortMessage = SmppStringUtil.GetStringFromBody(ref remainder, 10, 10 + _SmLength, DataCoding);
			TranslateTlvDataIntoTable(remainder);
		}
		
		/// <summary>
		/// Initializes this Pdu.
		/// </summary>
		protected override void InitPdu()
		{
			base.InitPdu();
			CommandStatus = 0;
			CommandID = CommandIdType.DeliverSm;
		}
		
		///<summary>
		/// Gets the hex encoding(big-endian)of this Pdu.
		///</summary>
		///<return>The hex-encoded version of the Pdu</return>
		public override void ToMsbHexEncoding()
		{
			ArrayList pdu = GetPduHeader();
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(ServiceType)));
			pdu.Add((byte)SourceAddressTon);
			pdu.Add((byte)SourceAddressNpi);
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(SourceAddress)));
			pdu.Add((byte)DestinationAddressTon);
			pdu.Add((byte)DestinationAddressNpi);
			pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(DestinationAddress)));
			pdu.Add(EsmClass);
			pdu.Add((byte)ProtocolId);
			pdu.Add((byte)PriorityFlag);
			//schedule_delivery_time and validity_period are null, so set them to zero
			pdu.Add((byte)0);
			pdu.Add((byte)0);
			pdu.Add((byte)RegisteredDelivery);
			//replace_if_present is always null, so set it to zero
			pdu.Add((byte)0);
			pdu.Add((byte)DataCoding);
			//sm_default_msg_id is always null, so set it to zero
			pdu.Add((byte)0);
			_SmLength = PduUtil.InsertShortMessage(pdu, ShortMessage, DataCoding);
			
			PacketBytes = EncodePduForTransmission(pdu);
		}
	}
}
