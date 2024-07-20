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
using MessagingToolkit.Core.Smpp.Packet;

namespace MessagingToolkit.Core.Smpp.Utility
{
	/// <summary>
	/// Holds common functionality for requests.
	/// </summary>
	public class PduUtil
	{			
		#region constants
		
		private const int SUBADDRESS_MIN = 2;
		private const int SUBADDRESS_MAX = 23;
		
		#endregion constants
		/// <summary>
		/// Do not instantiate
		/// </summary>
		private PduUtil()
		{
		}
		
		/// <summary>
		/// Inserts the short message into the PDU ArrayList.
		/// </summary>
		/// <param name="pdu">The PDU to put the short message into.</param>
		/// <param name="ShortMessage">The short message to insert.</param>
		/// <returns>The length of the short message.</returns>
		public static byte InsertShortMessage(ArrayList pdu, object ShortMessage)
		{
			byte[] msg;
			
			if(ShortMessage == null)
			{
				msg = new Byte[]{0x00};
			}
			else if(ShortMessage is string)
			{
				msg =  Encoding.ASCII.GetBytes((string)ShortMessage);
			}
			else if(ShortMessage is byte[])
			{
				msg =(byte[])ShortMessage;
			}
			else
			{
				throw new ArgumentException("Short Message must be a string or byte array.");
			}
			//			if(msg.Length >= MessageLcd2.SHORT_MESSAGE_LIMIT)
			//				throw new ArgumentException(
			//					"Short message cannot be longer than " +
			//					MessageLcd2.SHORT_MESSAGE_LIMIT + " octets.");
			
			byte SmLength = (byte)msg.Length;
			pdu.Add(SmLength);
			pdu.AddRange(msg);
			
			return SmLength;
		}


        /// <summary>
        /// Inserts the short message into the PDU ArrayList.
        /// </summary>
        /// <param name="pdu">The PDU to put the short message into.</param>
        /// <param name="ShortMessage">The short message to insert.</param>
        /// <param name="dataCoding">The data coding.</param>
        /// <returns>The length of the short message.</returns>
        public static byte InsertShortMessage(ArrayList pdu, object ShortMessage, PDU.DataCodingType dataCoding)
        {
            byte[] msg;

            if (ShortMessage == null)
            {
                msg = new Byte[] { 0x00 };
            }
            else if (ShortMessage is string)
            {
                if (dataCoding == PDU.DataCodingType.Ucs2 || dataCoding == PDU.DataCodingType.UnicodeFlashSms)
                {
                    msg = Encoding.GetEncoding("UTF-16BE").GetBytes((string)ShortMessage);
                }
                else
                {
                    msg = Encoding.ASCII.GetBytes((string)ShortMessage);
                }
            }
            else if (ShortMessage is byte[])
            {
                msg = (byte[])ShortMessage;
            }
            else
            {
                throw new ArgumentException("Short Message must be a string or byte array.");
            }
            //			if(msg.Length >= MessageLcd2.SHORT_MESSAGE_LIMIT)
            //				throw new ArgumentException(
            //					"Short message cannot be longer than " +
            //					MessageLcd2.SHORT_MESSAGE_LIMIT + " octets.");

            byte SmLength = (byte)msg.Length;
            pdu.Add(SmLength);
            pdu.AddRange(msg);

            return SmLength;
        }
		
		/// <summary>
		/// Takes the given PDU and inserts a receipted message ID into the TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetReceiptedMessageId(PDU pdu, string val)
		{
			const int MAX_RECEIPTED_ID_LEN = 65;
			if(val == null)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.ReceiptedMessageId, string.Empty);
			}
			else if(val.Length <= MAX_RECEIPTED_ID_LEN)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.ReceiptedMessageId, val);
			}
			else
			{
				throw new ArgumentException(
					"receipted_message_id must have length 1-" + MAX_RECEIPTED_ID_LEN);
			}
		}
		
		/// <summary>
		/// Takes the given PDU and inserts a network error code into the TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetNetworkErrorCode(PDU pdu, string val)
		{
			const int ERR_CODE_LEN = 3;
			if(val == null || val.Length != ERR_CODE_LEN)
			{
				throw new ArgumentException("network_error_code must have length " + ERR_CODE_LEN);
			}
			else
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.NetworkErrorCode, val);
			}
		}
		
		/// <summary>
		/// Takes the given PDU and inserts ITS session info into the TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetItsSessionInfo(PDU pdu, string val)
		{
			const int MAX_ITS = 16;
			
			if(val == null)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.ItsSessionInfo, string.Empty);
			}
			else if(val.Length == MAX_ITS)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.ItsSessionInfo, val);
			}
			else
			{
				throw new ArgumentException("its_session_info must have length " + MAX_ITS);
			}
		}
		
		/// <summary>
		/// Takes the given PDU and inserts a destination subaddress into the TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetDestSubaddress(PDU pdu, string val)
		{
			if(val.Length >= SUBADDRESS_MIN && val.Length <= SUBADDRESS_MAX)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.DestSubAddress, val);
			}
			else
			{
				throw new ArgumentException(
					"Destination subaddress must be between " + SUBADDRESS_MIN + 
					" and " + SUBADDRESS_MAX + " characters.");
			}
		}
		
		/// <summary>
		/// Takes the given PDU and inserts a source subaddress into the TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetSourceSubaddress(PDU pdu, string val)
		{
			if(val.Length >= SUBADDRESS_MIN && val.Length <= SUBADDRESS_MAX)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.SourceSubAddress, val);
			}
			else
			{
				throw new ArgumentException(
					"Source subaddress must be between " + SUBADDRESS_MIN + 
					" and " + SUBADDRESS_MAX + " characters.");
			}
		}
		
		/// <summary>
		/// Takes the given PDU and inserts a callback number into the TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetCallbackNum(PDU pdu, string val)
		{
			const int CALLBACK_NUM_MIN = 4;
			const int CALLBACK_NUM_MAX = 19;
			if(val.Length >= CALLBACK_NUM_MIN && val.Length <= CALLBACK_NUM_MAX)
			{
				pdu.SetOptionalParamString(
                    (ushort)PDU.OptionalParamCodes.CallbackNum, val);
			}
			else
			{
				throw new ArgumentException(
					"callback_num size must be between " + CALLBACK_NUM_MIN + 
					" and " + CALLBACK_NUM_MAX + " characters.");
			}
		}
		
		/// <summary>
		/// Takes the given PDU and inserts a message payload into its TLV table.
		/// </summary>
		/// <param name="pdu">The PDU to operate on.</param>
		/// <param name="val">The value to insert.</param>
        public static void SetMessagePayload(PDU pdu, object val)
		{
			if(val != null)
				{
					byte[] encodedValue;
					if(val is string)
					{
						encodedValue = Encoding.ASCII.GetBytes((string)val);
					}
					else if(val is byte[])
					{
						encodedValue =(byte[])val;
					}
					else
					{
						throw new ArgumentException("Message Payload must be a string or byte array.");
					}
					
					const int MAX_PAYLOAD_LENGTH = 64000;
					if(encodedValue.Length < MAX_PAYLOAD_LENGTH)
					{
						pdu.SetOptionalParamBytes(
                            (ushort)PDU.OptionalParamCodes.MessagePayload, encodedValue);
					}
					else
					{
						throw new ArgumentException(
							"Message Payload must be " + MAX_PAYLOAD_LENGTH + " characters or less in size.");
					}
				}
				else
				{
					pdu.SetOptionalParamBytes(
                        (ushort)PDU.OptionalParamCodes.MessagePayload, new byte[] { 0 });
				}
		}



        /// <summary>
        /// Takes the given PDU and inserts a message payload into its TLV table.
        /// </summary>
        /// <param name="pdu">The PDU to operate on.</param>
        /// <param name="val">The value to insert.</param>
        /// <param name="dataCoding">The data coding.</param>
        public static void SetMessagePayload(PDU pdu, object val, PDU.DataCodingType dataCoding)
        {
            if (val != null)
            {
                byte[] encodedValue;
                if (val is string)
                {
                    if (dataCoding == PDU.DataCodingType.Ucs2 || dataCoding == PDU.DataCodingType.UnicodeFlashSms)
                    {
                        encodedValue = Encoding.GetEncoding("UTF-16BE").GetBytes((string)val);                       
                    }
                    else
                    {
                        encodedValue = Encoding.ASCII.GetBytes((string)val);
                    }
                }
                else if (val is byte[])
                {
                    encodedValue = (byte[])val;
                }
                else
                {
                    throw new ArgumentException("Message Payload must be a string or byte array.");
                }

                const int MAX_PAYLOAD_LENGTH = 64000;
                if (encodedValue.Length < MAX_PAYLOAD_LENGTH)
                {
                    pdu.SetOptionalParamBytes(
                        (ushort)PDU.OptionalParamCodes.MessagePayload, encodedValue);
                }
                else
                {
                    throw new ArgumentException(
                        "Message Payload must be " + MAX_PAYLOAD_LENGTH + " characters or less in size.");
                }
            }
            else
            {
                pdu.SetOptionalParamBytes(
                    (ushort)PDU.OptionalParamCodes.MessagePayload, new byte[] { 0 });
            }
        }
	}
}
