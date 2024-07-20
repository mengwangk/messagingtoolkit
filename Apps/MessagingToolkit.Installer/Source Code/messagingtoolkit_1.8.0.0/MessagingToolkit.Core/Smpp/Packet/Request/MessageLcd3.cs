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
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.Utility;

namespace MessagingToolkit.Core.Smpp.Packet.Request
{
	/// <summary>
	/// Provides some common attributes for data_sm, submit_sm, and submit_multi.
	/// </summary>
	public abstract class MessageLcd3 : MessageLcd4
	{
		#region private fields

		private string _ServiceType = string.Empty;
		private byte _EsmClass = 0;
		private DataCodingType _DataCoding = PDU.DataCodingType.SMSCDefault;
		
		#endregion private fields
		
		#region constants
		
		private const int SAR_MIN = 1;
		private const int SAR_MAX = 255;
		
		#endregion constants
		
		#region mandatory parameters
		
		/// <summary>
		/// The service type of the message.  Null values are treated as empty strings.
		/// </summary>
		public string ServiceType
		{
			get
			{				
				return _ServiceType;
			}
			
			set
			{
				if(value != null)
				{
					if(value.Length <= SERVICE_TYPE_LENGTH)
					{
						_ServiceType = value;
					}
					else
					{
						throw new ArgumentOutOfRangeException("Service Type must be " + 
							SERVICE_TYPE_LENGTH + " 5 characters or less.");
					}
				}
				else
				{
					_ServiceType = string.Empty;
				}
			}
		}
		
		/// <summary>
		/// Indicates Message Mode and Message Type.  See the SMSC version 3.4 specification 
		/// for details on setting this.
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
		/// Defines the encoding scheme of the short message user data.
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
		
		#endregion mandatory parameters
		
		#region optional params
		
		/// <summary>
		/// The application port number associated with the source address of the message.
		/// This parameter should be present for WAP applications.
		/// </summary>
		public UInt16 SourcePort
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)PDU.OptionalParamCodes.SourcePort);
			}
			set
			{
				if(value < UInt16.MaxValue)
				{
					SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.SourcePort, value);
				}
				else
				{
					throw new ArgumentException("source_port value too large.");
				}
			}
		}
		
		/// <summary>
		/// The subcomponent in the destination device which created the user data.
		/// </summary>
		public AddressSubunitType SourceAddressSubunit
		{
			get
			{
				return (AddressSubunitType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.SourceAddrSubunit)[0];
			}
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.SourceAddrSubunit, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// The application port number associated with the destination address of the
		/// message.  This parameter should be present for WAP applications.
		/// </summary>
		public UInt16 DestinationPort
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)PDU.OptionalParamCodes.DestinationPort);
			}
			set
			{
				if(value < UInt16.MaxValue)
				{
					SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.DestinationPort, value);
				}
				else
				{
					throw new ArgumentException("destination_port value too large.");
				}
			}
		}
		
		/// <summary>
		/// The reference number for a particular concatenated short message.  Both
		/// SarTotalSegments and SarSegmentSeqnum need to be set in conjunction with this
		/// property.  In addition, this must be the same for each segment.
		/// </summary>
		public UInt16 SarMsgRefNumber
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)PDU.OptionalParamCodes.SarMsgRefNum);
			}
			set
			{
				if(value < UInt16.MaxValue)
				{
					SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.SarMsgRefNum, value);
				}
				else
				{
					throw new ArgumentException("sar_msg_ref_number value too large.");
				}
			}
		}
		
		/// <summary>
		/// Indicates the total number of short messages within the concatenated short
		/// message.  Both SarMsgRefNumber and SarSegmentSeqNum need to be set in
		/// conjunction with this property.  In addition, this must be the same for each
		/// segment.
		/// </summary>
		public byte SarTotalSegments
		{
			get
			{
				return GetOptionalParamBytes(
					(ushort)OptionalParamCodes.SarTotalSegments)[0];
			}
			
			set
			{
				if(value >= SAR_MIN && value <= SAR_MAX)
				{
					SetOptionalParamBytes(
						(ushort)PDU.OptionalParamCodes.SarTotalSegments, new Byte[] {(byte)value});
				}
				else
				{
					throw new ArgumentException(
						"sar_total_segments must be >= " + SAR_MIN + " and <= " + SAR_MAX);
				}
			}
		}
		
		/// <summary>
		/// The sequence number of a particular short message within the concatenated
		/// short message.  Both SarMsgRefNumber and SarTotalSegments need to be set in
		/// conjunction with this property.
		/// </summary>
		public byte SarSegmentSeqnum
		{
			get
			{
				return GetOptionalParamBytes(
					(ushort)OptionalParamCodes.SarSegmentSeqNum)[0];
			}
			
			set
			{
				if(value >= SAR_MIN && value <= SAR_MAX)
				{
					SetOptionalParamBytes(
						(ushort)PDU.OptionalParamCodes.SarSegmentSeqNum, new Byte[] {(byte)value});
				}
				else
				{
					throw new ArgumentException(
						"sar_segment_seqnum must be >= " + SAR_MIN + " and <= " + SAR_MAX);
				}
			}
		}
		
		/// <summary>
		/// Defines the type of payload.
		/// </summary>
		public PayloadTypeType PayloadType
		{
			get
			{
				return (PayloadTypeType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.PayloadType)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.PayloadType, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Contains the extended short message user data.  Up to 64K octets can be
		/// transmitted.  The limit for this is network/SMSC dependent.
		/// </summary>
		public object MessagePayload
		{
			get
			{
				return GetOptionalParamBytes(
					(ushort)OptionalParamCodes.MessagePayload);
			}
			
			set
			{
				PduUtil.SetMessagePayload(this, value);
			}
		}
		
		/// <summary>
		/// The privacy level of the message.
		/// </summary>
		public PrivacyType PrivacyIndicator
		{
			get
			{
				return (PrivacyType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.PrivacyIndicator)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.PrivacyIndicator, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// ESME assigned message reference number.
		/// </summary>
		public UInt16 UserMessageReference
		{
			get
			{				
				return GetHostOrderUInt16FromTlv((ushort)PDU.OptionalParamCodes.UserMessageReference);
			}
			set
			{
				if(value < UInt16.MaxValue)
				{
					SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.UserMessageReference, value);
				}
				else
				{
					throw new ArgumentException("user_message_reference too large.");
				}
			}
		}
		
		/// <summary>
		/// Allows an indication to be provided to an MS that there are
		/// messages waiting for the subscriber on systems on the PLMN. The indication
		/// can be an icon on the MS screen or other MMI indication.
		/// See section 5.3.2.13 for details on how to set this.
		/// </summary>
		public byte MsMsgWaitFacilities
		{
			get
			{
				return GetOptionalParamBytes(
					(ushort)OptionalParamCodes.MsMsgWaitFacilities)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.MsMsgWaitFacilities, new Byte[] {value});
			}
		}
		
		/// <summary>
		/// Provides a MS with validity information associated with the received
		/// short message.
		/// </summary>
		public MsValidityType MsValidity
		{
			get
			{
				return (MsValidityType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.MsValidity)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.MsValidity, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Provides a TDMA MS station with alert tone information associated with the
		/// received short message.
		/// </summary>
		public UInt16 SmsSignal
		{
			get
			{
				return GetHostOrderUInt16FromTlv((ushort)PDU.OptionalParamCodes.SmsSignal);
			}
			
			set
			{
				if(value < UInt16.MaxValue)
				{
					SetHostOrderValueIntoTlv((UInt16)PDU.OptionalParamCodes.SmsSignal, value);
				}
				else
				{
					throw new ArgumentException("sms_signal value too large.");
				}
			}
		}
		
		/// <summary>
		/// The subcomponent in the destination device for which the user data is intended.
		/// </summary>
		public AddressSubunitType DestinationAddrSubunit
		{
			get
			{
				return (AddressSubunitType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.DestAddrSubunit)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.DestAddrSubunit, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Tells a mobile station to alert the user when the short message arrives at the MS.
		///
		/// Note: there is no value part associated with this parameter.
		/// Any value you pass in will be discarded.
		/// </summary>
		public byte AlertOnMsgDelivery
		{
			get
			{
				try
				{
					byte[] bytes = GetOptionalParamBytes(
						(ushort)OptionalParamCodes.AlertOnMessageDelivery);
					return 1;
				}
				catch(ApplicationException)
				{
					return 0;
				}
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.AlertOnMessageDelivery, new Byte[0]);
			}
		}
		
		/// <summary>
		/// The language of the short message.
		/// </summary>
		public LanguageType LanguageIndicator
		{
			get
			{
				return (LanguageType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.LanguageIndicator)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.LanguageIndicator, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Associates a display time with the short message on the MS.
		/// </summary>
		public DisplayTimeType DisplayTime
		{
			get
			{
				return (DisplayTimeType)GetOptionalParamBytes(
					(ushort)OptionalParamCodes.DisplayTime)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.DisplayTime, new Byte[] {(byte)value});
			}
		}
		
		/// <summary>
		/// Associates a call back number with the message.  See section 5.3.2.36 of the
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
		/// Controls the presentation indication and screening of the CallBackNumber at the
		/// mobile station.  You must also use the callback_num parameter with this.
		/// See section 5.3.2.37 of the SMPP spec for details in how to set this.
		/// </summary>
		public byte CallbackNumPresInd
		{
			get
			{
				return GetOptionalParamBytes(
					(ushort)OptionalParamCodes.CallbackNumPresInd)[0];
			}
			
			set
			{
				SetOptionalParamBytes(
					(ushort)PDU.OptionalParamCodes.CallbackNumPresInd, new Byte[] {value});
			}
		}
		
		/// <summary>
		/// Alphanumeric display tag for call back number.  This must be less than or
		/// equal to 65 characters in length.
		/// </summary>
		public string CallbackNumAtag
		{
			get
			{
				return GetOptionalParamString((ushort)OptionalParamCodes.CallbackNumATag);
			}
			
			set
			{
				const int CALLBACK_ATAG_MAX = 65;
				
				if(value.Length <= CALLBACK_ATAG_MAX)
				{
					SetOptionalParamString(
						(ushort)PDU.OptionalParamCodes.CallbackNumATag, value);
				}
				else
				{
					throw new ArgumentException("Callback number atag must be <= " + CALLBACK_ATAG_MAX + ".");
				}
			}
		}
		
		/// <summary>
		/// Specifies a subaddress associated with the originator of the message.
		/// See section 5.3.2.15 of the SMPP spec for details on
		/// setting this parameter.
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
		/// Specifies a subaddress associated with the receiver of the message.
		/// See section 5.3.2.15 of the SMPP spec for details on
		/// setting this parameter.
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
		
		#endregion optional params
		
		#region constructors
		
		/// <summary>
		/// Groups construction tasks for subclasses.  Sets source address TON to international, 
		/// source address NPI to ISDN, source address to "", registered delivery type to none, 
		/// ESM class to 0, and data coding to SMSC default.
		/// </summary>
		protected MessageLcd3(): base()
		{}
		
		/// <summary>
		/// Creates a new MessageLcd3 for incoming PDUs.
		/// </summary>
		/// <param name="incomingBytes">The incoming bytes to decode.</param>
		protected MessageLcd3(byte[] incomingBytes): base(incomingBytes)
		{}
		
		#endregion constructors
	}
}
