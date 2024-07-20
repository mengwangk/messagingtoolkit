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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Pdu;


namespace MessagingToolkit.Core.Mobile.PduLibrary
{
	/// <summary>
	/// Factory class that is used to encode or decode various kinds of messages 
	/// </summary>
	internal class PduFactory
	{

		#region ========================= Constructor ============================

		/// <summary>
		/// Private constructor
		/// </summary>
		private PduFactory()
		{
		}

		#endregion ================================================================            
		

		#region ========================= Public Method ===========================

	   
		/// <summary>
		/// Encode the SMS message
		/// </summary>
		/// <param name="sms">SMS object</param>
		/// <returns>List of PDU string</returns>
		internal string[] Encode(Sms sms)
		{

			// If no data coding scheme is defined
			if (sms.DataCodingScheme == MessageDataCodingScheme.Undefined)
			{
				sms.DataCodingScheme = PduUtils.GetDataCodingScheme(sms.Content);
			}

			bool isEms = false;

			if (sms.DataCodingScheme == MessageDataCodingScheme.DefaultAlphabet)
			{
				if (sms.Content.Length > 160)
				{
					isEms = true;
				}
			}
			else if (sms.DataCodingScheme == MessageDataCodingScheme.Ucs2)
			{
				if (sms.Content.Length > 70)
				{
					isEms = true;
				}
			}

			if (!isEms) 
			{
				string pdu = sms.GetSmsPduCode();
				string[] pduCodes = new string[] { pdu };
				return pduCodes;
			}
			else
			{
				return sms.GetEmsPduCode();
			}               
		}

		/// <summary>
		/// Determines whether [is alpha numeric address] [the specified address].
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns>
		/// 	<c>true</c> if [is alpha numeric address] [the specified address]; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsAlphaNumericAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return true;
			}

			if (address.StartsWith("+"))
			{
				return false;
			}

			// now we need to loop through the string, examining each character
			for (int i = 0; i < address.Length; i++)
			{
				// if this character isn't a letter and it isn't a number then return false
				// because it means this isn't a valid alpha numeric string
				if (!(char.IsNumber(address[i])))
					return true;

			}
			return false;
		}

		/// <summary>
		/// Generate the PDU for the SMS message
		/// </summary>
		/// <param name="message">Message instance</param>
		/// <returns>A list of PDU codes</returns>
		public string[] Generate(Sms message)
		{
			if (message.DataCodingScheme == MessageDataCodingScheme.Undefined)
			{
				message.DataCodingScheme = PduUtils.GetDataCodingScheme(message.Content);
			}

			SmsSubmitPdu pdu;
			if (message.StatusReportRequest == MessageStatusReportRequest.SmsReportRequest)
			{
				pdu = MessagingToolkit.Pdu.PduFactory.NewSmsSubmitPdu(MessagingToolkit.Pdu.PduUtils.TpSrrReport | MessagingToolkit.Pdu.PduUtils.TpVpfInteger);
			}
			else
			{
				pdu = MessagingToolkit.Pdu.PduFactory.NewSmsSubmitPdu();
			}
			if (!string.IsNullOrEmpty(message.ServiceCenterNumber) && message.ServiceCenterNumber != Sms.DefaultSmscAddress && !IsAlphaNumericAddress(message.ServiceCenterNumber))
			{               
				pdu.SmscAddress = message.ServiceCenterNumber;
				string smscNumberForLengthCheck = pdu.SmscAddress;
				if (pdu.SmscAddress.StartsWith("+"))
				{
					smscNumberForLengthCheck = pdu.SmscAddress.Substring(1);
				}
				pdu.SmscInfoLength = 1 + (smscNumberForLengthCheck.Length / 2) + ((smscNumberForLengthCheck.Length % 2 == 1) ? 1 : 0);
				pdu.SmscAddressType = MessagingToolkit.Pdu.PduUtils.GetAddressTypeFor(message.ServiceCenterNumber);
			}
			else
			{
				pdu.SmscAddress = string.Empty;
			}

			pdu.Address = message.DestinationAddress;
			string userData = message.Content;

			switch (message.DataCodingScheme) 
			{
				case MessageDataCodingScheme.DefaultAlphabet:
				case MessageDataCodingScheme.Class0Ud7Bits:
				case MessageDataCodingScheme.Class1Ud7Bits:
				case MessageDataCodingScheme.Class2Ud7Bits:
				case MessageDataCodingScheme.Class3Ud7Bits:
				case MessageDataCodingScheme.SevenBits:
				case MessageDataCodingScheme.Undefined:
				case MessageDataCodingScheme.Custom:
					pdu.DataCodingScheme = MessagingToolkit.Pdu.PduUtils.DcsEncoding7Bit;
					break;
				case MessageDataCodingScheme.EightBits:
				case MessageDataCodingScheme.Class0Ud8Bits:
				case MessageDataCodingScheme.Class1Ud8Bits:
				case MessageDataCodingScheme.Class2Ud8Bits:
				case MessageDataCodingScheme.Class3Ud8Bits:
					pdu.DataCodingScheme = MessagingToolkit.Pdu.PduUtils.DcsEncoding8Bit;
					break;
				case MessageDataCodingScheme.Ucs2:
					pdu.DataCodingScheme = MessagingToolkit.Pdu.PduUtils.DcsEncodingUcs2;
					break;                
			}

			// Set the message class
			switch (message.DataCodingScheme)
			{
				case MessageDataCodingScheme.Class0Ud7Bits:
				case MessageDataCodingScheme.Class0Ud8Bits:
					pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassMe;
					break;
				case MessageDataCodingScheme.Class1Ud7Bits:
				case MessageDataCodingScheme.Class1Ud8Bits:
					pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassSim;
					break;
				case MessageDataCodingScheme.Class2Ud7Bits:
				case MessageDataCodingScheme.Class2Ud8Bits:
					pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassTe;
					break;
				case MessageDataCodingScheme.Class3Ud7Bits:
				case MessageDataCodingScheme.Class3Ud8Bits:
					pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassFlash;
					break;
			}

			if (message.DcsMessageClass == MessageClasses.Flash)
			{
				pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassFlash;
			}
			else if (message.DcsMessageClass == MessageClasses.Me)
			{
				pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassMe;
			}
			else if (message.DcsMessageClass == MessageClasses.Sim)
			{
				pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassSim;
			}
			else if (message.DcsMessageClass == MessageClasses.Te)
			{
				pdu.DataCodingScheme = pdu.DataCodingScheme | MessagingToolkit.Pdu.PduUtils.DcsMessageClassTe;
			}

			if (message.ValidityPeriod != MessageValidPeriod.Custom)
				pdu.ValidityPeriod = (int)Enum.Parse(typeof(MessageValidPeriod), message.ValidityPeriod.ToString());
			else
				pdu.ValidityPeriod = message.CustomValidityPeriod / 60.0; // Convert to hours
 
			pdu.ProtocolIdentifier = 0;


			if (!string.IsNullOrEmpty(message.ReplyPath))
			{
				pdu.AddReplyPath(message.ReplyPath, MessagingToolkit.Pdu.PduUtils.GetAddressTypeFor(message.ReplyPath));
			}

			if (message.DestinationPort > 0 || message.SourcePort > 0)
			{
				pdu.AddInformationElement(MessagingToolkit.Pdu.Ie.InformationElementFactory.GeneratePortInfo(message.DestinationPort, message.SourcePort));
			}
			
			if (message.DataCodingScheme == MessageDataCodingScheme.EightBits)
			{
				if (message.GetType() == typeof(Sms))
				{
					if (message.Binary && message.DataBytes != null)
					{
						pdu.SetDataBytes(message.DataBytes);
					}
					else
					{
						if (PduUtils.GetDataCodingScheme(userData) == MessageDataCodingScheme.Ucs2)
						{
							pdu.SetDataBytes(Encoding.GetEncoding("UTF-16").GetBytes(userData));
						}
						else
						{
							pdu.SetDataBytes(Encoding.ASCII.GetBytes(userData));
						}
					}
				}
				else
				{
					pdu.SetDataBytes(message.GetPdu());
				}
			}
			else
			{                
				pdu.DecodedText = userData;
			}

			if (message.LongMessageOption == MessageSplitOption.Concatenate)
			{
				int refNo = new Random().Next();
				refNo %= 65536;
				pdu.MessageReference = refNo;
				message.ReferenceNo.Add((byte)refNo);
				PduGenerator pduGenerator = new PduGenerator();
				List<string> pduList = pduGenerator.GeneratePduList(pdu, refNo);
				return pduList.ToArray();
			}
			else if (message.LongMessageOption == MessageSplitOption.Truncate)
			{
				/*
				if (message.DataCodingScheme == MessageDataCodingScheme.Ucs2)
				{
					if (message.Content.Length > 70)
						pdu.DecodedText = message.Content.Substring(0, 70);
				}
				else
				{
					if (message.Content.Length > 160)
						pdu.DecodedText = message.Content.Substring(0, 160);
				}
				*/

				int refNo = new Random().Next();
				refNo %= 65536;
				pdu.MessageReference = refNo;
				message.ReferenceNo.Add((byte)refNo);
				PduGenerator pduGenerator = new PduGenerator();
				string pduString = pduGenerator.GeneratePduString(pdu, refNo, 1);
				List<string> pduList = new List<string>();
				pduList.Add(pduString);
				return pduList.ToArray();
			}
			else if (message.LongMessageOption == MessageSplitOption.SimpleSplit)
			{              
				List<string> pduList = new List<string>();
				PduGenerator pduGenerator = new PduGenerator();
				for (int i = 1; i <= pdu.MpMaxNo; i++)
				{
					int refNo = new Random().Next();
					refNo %= 65536;
					pdu.MessageReference = refNo;
					message.ReferenceNo.Add((byte)refNo);
					string pduString = pduGenerator.GeneratePduString(pdu, refNo, i);
					pduList.Add(pduString);
				}
				return pduList.ToArray();
			}

			return new string[] { };

			/*
			int refNo;
			string pdu;

			string encodedText = message.GetPdu();
			bool isBig = PduUtils.IsBig(encodedText);

			if (isBig && message.LongMessageOption == MessageSplitOption.Truncate) {
				if (message.DataCodingScheme == MessageDataCodingScheme.Ucs2)
				{
					message.Content = message.Content.Substring(0, 70);
				}
				else
				{
					message.Content = message.Content.Substring(0, 160);
				}
			   encodedText = message.GetPdu();
			   isBig = PduUtils.IsBig(encodedText);
			}            
			
			int noOfParts = PduUtils.GetNoOfParts(encodedText);

			if (!isBig)
			{
				refNo = new Random().Next();
				refNo %= 65536;
				message.ReferenceNo.Add(refNo);
				pdu = PduUtils.GetPdu(message, encodedText, noOfParts, 0, refNo);
				return new string[] { pdu };
			}
			else
			{               
				refNo = new Random().Next();               
				string[] pduList = new string[noOfParts];
				refNo %= 65536;
				message.ReferenceNo.Add(refNo);  
			  
				for (int partNo = 1; partNo <= noOfParts; partNo++)
				{
					if (message.LongMessageOption == MessageSplitOption.SimpleSplit && partNo > 1)
					{
						refNo = (refNo + 1) % 65536;
						message.ReferenceNo.Add(refNo);  
					}
					pdu = PduUtils.GetPdu(message, encodedText, noOfParts, partNo, refNo);
					pduList[partNo - 1] = pdu;
								   
				}
				return pduList;               
			}
			*/ 
		   

		}

		/// <summary>
		/// Decode the PDU code into SMS or EMS
		/// </summary>
		/// <param name="pduCode">PDU code string</param>
		/// <returns>The message information</returns>
		public MessageInformation Decode(string pduCode)
		{                      
			MessageInformation messageInformation = new MessageInformation();
			messageInformation.RawMessage = pduCode;

			// Sanity check
			if (string.IsNullOrEmpty(pduCode)) return messageInformation;

			// Some preprocessing
			pduCode = pduCode.Replace("\r", string.Empty);
			pduCode = pduCode.Replace("\n", string.Empty);
			pduCode = pduCode.Replace("\t", string.Empty);
		 
			try
			{               
				PduParser pduParser = new PduParser();
				PduFactory pduFactory = new PduFactory();
				PduGenerator pduGenerator = new PduGenerator();
				Pdu.Pdu pdu;
				pdu = pduParser.ParsePdu(pduCode);
				if (pdu.Binary)
				{
					pdu.SetDataBytes(pdu.UserDataAsBytes);
				}
							 
				// Check if it is a MMS notification, MMS delivery report or other WAP MMS messages
				if (pdu.DestinationPort == MmsConstants.MmsNotificationDestinationPort &&
					!string.IsNullOrEmpty(pdu.DecodedText) && pdu.MpMaxNo == 1)
				{
					MessageInformation msgInfo = PduDecoder.DecodeWapMms(pdu.UserDataAsBytes);
					if (msgInfo != null)
					{
						messageInformation = msgInfo;
						messageInformation.RawMessage = pduCode;
					}
				}
				messageInformation.Content = pdu.DecodedText;
				messageInformation.CurrentPiece = pdu.MpSeqNo;
				messageInformation.TotalPiece = pdu.MpMaxNo;
				messageInformation.ReferenceNo = pdu.MpRefNo;
				messageInformation.SourcePort = pdu.SourcePort;
				messageInformation.DestinationPort = pdu.DestinationPort;
				messageInformation.TotalPieceReceived = 1;
				messageInformation.ServiceCentreAddress = pdu.SmscAddress;
				if (StringEnum.IsStringDefined(typeof(NumberType), Convert.ToString(pdu.SmscAddressType)))
					messageInformation.ServiceCentreAddressType = (NumberType)StringEnum.Parse(typeof(NumberType), Convert.ToString(pdu.SmscAddressType));
							   
				if (!string.IsNullOrEmpty(pdu.DecodedText))
					messageInformation.DataBytes.AddRange(pdu.UserDataAsBytes);
										   
				if (pdu is SmsDeliveryPdu)
				{
					SmsDeliveryPdu smsDeliveryPdu = (SmsDeliveryPdu)pdu;
					messageInformation.PhoneNumber = smsDeliveryPdu.Address;
					if (smsDeliveryPdu.AddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber; 
					messageInformation.ReceivedDate = smsDeliveryPdu.Timestamp;
					messageInformation.Timezone = smsDeliveryPdu.Timezone;
					messageInformation.MessageType = MessageTypeIndicator.MtiSmsDeliver;                   
				}
				else if (pdu is SmsSubmitPdu)
				{
					SmsSubmitPdu smsSubmitPdu = (SmsSubmitPdu)pdu;
					messageInformation.PhoneNumber = smsSubmitPdu.Address;
					if (smsSubmitPdu.AddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;
					messageInformation.MessageType = MessageTypeIndicator.MtiSmsSubmit;
					messageInformation.Timezone = smsSubmitPdu.Timezone;
					messageInformation.ValidityTimestamp = smsSubmitPdu.ValidityTimestamp;
					messageInformation.ReferenceNo = smsSubmitPdu.MessageReference;
				}
				else if (pdu is SmsStatusReportPdu)
				{
					SmsStatusReportPdu smsStatusReportPdu = (SmsStatusReportPdu)pdu;
					messageInformation.PhoneNumber = smsStatusReportPdu.Address;
					if (smsStatusReportPdu.AddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;
					messageInformation.ReceivedDate = smsStatusReportPdu.DischargeTime;
					messageInformation.DestinationReceivedDate = smsStatusReportPdu.Timestamp;
					if (Enum.IsDefined(typeof(MessageStatusReportStatus), Convert.ToInt32(smsStatusReportPdu.Status.ToString(),16)))
					{                        
						messageInformation.DeliveryStatus = (MessageStatusReportStatus)Enum.Parse(typeof(MessageStatusReportStatus),
									Convert.ToInt32(smsStatusReportPdu.Status.ToString(),16).ToString());
					}
					else
					{
						messageInformation.DeliveryStatus = MessageStatusReportStatus.UnknownStatus;
					}
					messageInformation.Timezone = smsStatusReportPdu.Timezone;
					messageInformation.MessageType = MessageTypeIndicator.MtiSmsStatusReport;
					messageInformation.ReferenceNo = smsStatusReportPdu.MessageReference;
				}
				 
			  
			
				/*
				if (pduCode.StartsWith("00"))
				{
					// If starts with "00", default service center address to 12345
					pduCode = "04912143F5" + pduCode.Substring(2, pduCode.Length - 2);
				}   
			
			   
				BaseSms message = null;
				MessageTypeIndicator type = PduUtils.GetSmsType(pduCode);
				messageInformation.MessageType = type;
				switch (type)
				{
					case MessageTypeIndicator.MtiEmsReceived:
						EmsReceived emsReceived = new EmsReceived(pduCode);
						messageInformation.PhoneNumber = emsReceived.SourceAddress;

						if (emsReceived.SourceAddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;

						messageInformation.ReceivedDate = emsReceived.ServiceCenterTimestamp;
						messageInformation.Timezone = emsReceived.Timezone;
						
						if ((emsReceived.InfoElements != null))
						{
							string data = emsReceived.InfoElements[0].Data;
							if (data.Length >= 6)
							{
								messageInformation.ReferenceNo = Convert.ToInt32(data.Substring(0, 4), 16);
								messageInformation.TotalPiece = Convert.ToInt32(data.Substring(4, 2), 16);
								if (messageInformation.TotalPiece == 1)
									messageInformation.CurrentPiece = 1;
								else
								{
									if (data.Length >= 8)
										messageInformation.CurrentPiece = Convert.ToInt32(data.Substring(6, 2), 16);
								}
							}
							else
							{
								messageInformation.TotalPiece = 1;
								messageInformation.CurrentPiece = 1;
							}
						}
						message = emsReceived;

						break;

					case MessageTypeIndicator.MtiSmsDeliver:
						SmsReceived smsReceived = new SmsReceived(pduCode);
						messageInformation.PhoneNumber = smsReceived.SourceAddress;
						if (smsReceived.SourceAddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;
						messageInformation.ReceivedDate = smsReceived.ServiceCenterTimestamp;
						messageInformation.Timezone = smsReceived.Timezone;                     
						message = smsReceived;
						break;

					case MessageTypeIndicator.MtiEmsSubmit:
						EmsSubmit emsSubmit = new EmsSubmit(pduCode);
						messageInformation.PhoneNumber = emsSubmit.DestinationAddress;
						if (emsSubmit.DestinationAddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;
						if ((emsSubmit.InfoElements != null))
						{
							string data = emsSubmit.InfoElements[0].Data;
							if (data.Length >= 6)
							{
								messageInformation.ReferenceNo = Convert.ToInt32(data.Substring(0, 4), 16);
								messageInformation.TotalPiece = Convert.ToInt32(data.Substring(4, 2), 16);
								if (messageInformation.TotalPiece == 1)
									messageInformation.CurrentPiece = 1;
								else
									if (data.Length >= 8)
										messageInformation.CurrentPiece = Convert.ToInt32(data.Substring(6, 2), 16);
							}
							else
							{
								messageInformation.TotalPiece = 1;
								messageInformation.CurrentPiece = 1;
							}
						}
						message = emsSubmit;
						break;

					case MessageTypeIndicator.MtiSmsSubmit:
						SmsSubmit smsSubmit = new SmsSubmit(pduCode);
						messageInformation.PhoneNumber = smsSubmit.DestinationAddress;
						if (smsSubmit.DestinationAddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;
						message = smsSubmit;
						break;

					case MessageTypeIndicator.MtiSmsStatusReport:
						SmsStatusReport smsStatusReport = new SmsStatusReport(pduCode);
						messageInformation.PhoneNumber = smsStatusReport.SourceAddress;
						if (smsStatusReport.SourceAddressType == 0x91) messageInformation.PhoneNumber = "+" + messageInformation.PhoneNumber;
						messageInformation.ReceivedDate = smsStatusReport.ServiceCenterTimestamp;
						messageInformation.DestinationReceivedDate = smsStatusReport.DestinationReceivedDate;
						messageInformation.DeliveryStatus = smsStatusReport.Status;
						messageInformation.Timezone = smsStatusReport.Timezone;
						message = smsStatusReport;
						break;

					default:

						break;
				}

				if (message.DataCodingScheme == MessageDataCodingScheme.Ucs2)
				{
					messageInformation.Content = PduUtils.DecodeUcs2(message.Content);
				}               
				else if (message.DataCodingScheme == MessageDataCodingScheme.EightBits || 
					message.DataCodingScheme == MessageDataCodingScheme.Class0Ud8Bits ||
					message.DataCodingScheme == MessageDataCodingScheme.Class1Ud8Bits ||
					message.DataCodingScheme == MessageDataCodingScheme.Class2Ud8Bits ||
					message.DataCodingScheme == MessageDataCodingScheme.Class3Ud8Bits 
					)
				{
					messageInformation.Content = PduUtils.Decode8bit(message.Content, message.ContentLength);
				}
				else 
				 // if (message.DataCodingScheme == MessageDataCodingScheme.DefaultAlphabet ||
				  //  message.DataCodingScheme == MessageDataCodingScheme.SevenBits ||
				  //  message.DataCodingScheme == MessageDataCodingScheme.Class0Ud7Bits ||
				   //  message.DataCodingScheme == MessageDataCodingScheme.Class1Ud7Bits ||
				   //  message.DataCodingScheme == MessageDataCodingScheme.Class2Ud7Bits ||
				   //  message.DataCodingScheme == MessageDataCodingScheme.Class3Ud7Bits
				   // )
				{
					if (type == MessageTypeIndicator.MtiSmsDeliver || type == MessageTypeIndicator.MtiSmsStatusReport ||
						type == MessageTypeIndicator.MtiSmsSubmit)
					{
						// add a parameter                       
						messageInformation.Content = PduUtils.Decode7Bit(message.Content, message.ContentLength);
					}

					if (type == MessageTypeIndicator.MtiEmsReceived || type == MessageTypeIndicator.MtiEmsSubmit)
					{
						if (type == MessageTypeIndicator.MtiEmsReceived)
						{
							EmsReceived emsReceived = (EmsReceived)message;
							messageInformation.Content = PduUtils.Decode7Bit(message.Content, message.ContentLength - 8 * (1 + emsReceived.UserDataHeaderLength) / 7);
						}
						else
						{
							EmsSubmit emsSubmit = (EmsSubmit)message;
							messageInformation.Content = PduUtils.Decode7Bit(message.Content, message.ContentLength - 8 * (1 + emsSubmit.UserDataHeaderLength) / 7);
						}
					}
				}     
				*/ 
			}
			catch (Exception ex)
			{
				messageInformation.Content = string.Format(Resources.DecodingException, pduCode);
				throw;
			}

			return messageInformation;
		}


		/******* Commented - not in used now ********************
		/// <summary>
		/// </summary>
		/// <param name="pdu"></param>
		/// <returns></returns>
		public MessageInformation Decode(string pdu)
		{
			// Sanity check
			if (string.IsNullOrEmpty(pdu)) return new MessageInformation();

			// Some preprocessing
			pdu = pdu.Replace("\r", string.Empty);
			pdu = pdu.Replace("\n", string.Empty);
			pdu = pdu.Replace("\t", string.Empty);

			if (IsStatusReportMessage(pdu))
			{
				return DecodeStatusReport(pdu);
			}
			else
			{
				return DecodeInboundMessage(pdu);
			}

			return new MessageInformation();
		}       
		*/

		#endregion ================================================================


		#region ============== Factory method   ===================================

		/// <summary>
		/// Static factory to create the message factory
		/// </summary>
		/// <returns>A new instance of the message factory</returns>
		public static PduFactory NewInstance()
		{
			return new PduFactory();
		}

		#endregion ================================================================
	}


}


