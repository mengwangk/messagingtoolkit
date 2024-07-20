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
using System.IO;

using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.MMS;

namespace MessagingToolkit.Core.Mobile.PduLibrary
{
    /// <summary>
    /// PDU decoder
    /// </summary>
    internal static class PduDecoder
    {
        /// <summary>
        /// Interprete the WAP MMS message
        /// </summary>
        private static Dictionary<MmsConstants.WapMmsType, Func<byte[], MessageInformation>> MessageInterpreter =
            new Dictionary<MmsConstants.WapMmsType, Func<byte[], MessageInformation>> 
            {
                {MmsConstants.WapMmsType.MDeliveryInd, DecodeMmsDeliveryNotification},
                {MmsConstants.WapMmsType.MReadRecInd, DecodeMmsReadReport},
                {MmsConstants.WapMmsType.MReadOrigInd, DecodeMmsReadReport},
                {MmsConstants.WapMmsType.MNotificationInd, DecodeMmsNotification}

            };

        /// <summary>
        /// Decodes WAP MMS
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns>Message information</returns>
        public static MessageInformation DecodeWapMms(byte[] userData)
        {
            try
            {

                //byte[] userData = pdu.UserDataAsBytes;

                int index = 0;
                string transactionId = Convert.ToString(userData[index++]);

                // Position 1 is the PDU type
                // Position 2 is the header length
                int headerLength = userData[++index];

                // Read content type            
                //string contentType = ReadTextString(userData, ref index);

                // Skip AF86 - x-wap-application:mms.ua
                index = index + (headerLength + 1);      // 1 is the header length field


                // Get the message type
                int messageType = 0;
                while (index < userData.Length)
                {
                    switch (userData[index++])
                    {
                        case MmsConstants.MmsType:
                            messageType = userData[index];
                            index++;
                            break;
                    }
                    if (messageType > 0) break;
                }

                if (messageType == 0) return null;

                object obj = Enum.Parse(typeof(MmsConstants.WapMmsType), Convert.ToString(messageType));
                if (obj == null || Convert.ToInt32(obj) == 0)
                {
                    // no supported message type found
                    return null;
                }

                MmsConstants.WapMmsType wapMmsType = (MmsConstants.WapMmsType)obj;
                Func<byte[], MessageInformation> func = null;
                if (MessageInterpreter.TryGetValue(wapMmsType, out func))
                {
                    // Able to get the interpreter function
                    return func(userData);
                }

            }
            catch (Exception ex)
            {
                Logger.LogThis(string.Format("Unable to decode WAP MMS. {0}", ex.Message), LogLevel.Error);
            }
            return null;
        }

        /// <summary>
        /// Decodes the MMS notification.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        private static MessageInformation DecodeMmsNotification(byte[] userData)
        {
            MmsNotification notification = new MmsNotification();
            int index = 0;
            // Position 0 is the transaction id
            notification.TransactionId = Convert.ToString(userData[index++]);

            // Position 1 is the PDU type
            // Position 2 is the header length
            notification.HeaderLength = userData[++index];

            // Read content type            
            //notification.ContentType = ReadTextString(userData, ref index);

            // Skip AF86 - x-wap-application:mms.ua
            index = index + (notification.HeaderLength + 1);      // 1 is the header length field

            // Start reading the content
            while (index < userData.Length)
            {
                switch (userData[index++])
                {
                    case MmsConstants.MmsType:
                        notification.MessageType = (MmsConstants.WapMmsType)Enum.Parse(typeof(MmsConstants.WapMmsType), Convert.ToString(userData[index]));
                        index++;
                        break;
                    case MmsConstants.MmsVersion:
                        notification.Version = ReadVersion(userData[index]);
                        index++;
                        break;
                    case MmsConstants.MmsContentLocation:
                        notification.ContentLocation = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsSubject:
                        notification.Subject = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsFrom:
                        int valueLength = userData[index++];
                        int addressPresentToken = UnsignedByte(userData[index++]);
                        if (addressPresentToken == 0x80)
                        {
                            // Address-present-token
                            notification.From = ReadTextString(userData, ref index);
                        }
                        break;
                    case MmsConstants.MmsTransactionId:
                        notification.TransactionId = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsExpiry:
                        valueLength = ReadValueLength(userData, ref index);
                        int tokenType = UnsignedByte(userData[index++]);
                        long expiry = 0;
                        if (tokenType == 128)
                        {
                            // Absolute-token
                            int length = userData[index++];
                            expiry = ReadMultipleByteInt(userData, length, ref index) * 1000;
                            notification.ExpiryAbsolute = true;
                        }

                        if (tokenType == 129)
                        {
                            // Relative-token
                            notification.ExpiryAbsolute = false;
                            // Read the Delta-seconds-value
                            if (valueLength > 3)
                            {
                                // Long Integer
                                int length = userData[index++];
                                expiry = ReadMultipleByteInt(userData, length, ref index) * 1000;
                            }
                            else
                            {
                                // Short Integhet (1 OCTECT)
                                int b = userData[index] & 0x7F;
                                expiry = b * 1000;
                                index++;
                            }
                        }
                        notification.Expiry = new DateTime((expiry * TimeSpan.TicksPerMillisecond) + MmsConstants.Ticks1970);
                        break;
                    case MmsConstants.MmsMessageClass:
                        notification.MessageClass = (MmsConstants.MessageClass)Enum.Parse(typeof(MmsConstants.MessageClass), Convert.ToString(userData[index]));
                        index++;
                        break;
                    case MmsConstants.MmsMessageSize:
                        index++;
                        break;
                    default:
                        index++;
                        break;
                }
            }
            return notification;
        }

        /// <summary>
        /// Decodes the MMS read report.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        private static MessageInformation DecodeMmsReadReport(byte[] userData)
        {
            MmsReadReport notification = new MmsReadReport();
            int index = 0;
            // Position 0 is the transaction id
            notification.TransactionId = Convert.ToString(userData[index++]);

            // Position 1 is the PDU type
            // Position 2 is the header length
            notification.HeaderLength = userData[++index];

            // Read content type            
            //notification.ContentType = ReadTextString(userData, ref index);

            // Skip AF86 - x-wap-application:mms.ua
            index = index + notification.HeaderLength + 1;      // 1 is the header length field


            // Start reading the content
            while (index < userData.Length)
            {
                switch (userData[index++])
                {
                    case MmsConstants.MmsType:
                        notification.MessageType = (MmsConstants.WapMmsType)Enum.Parse(typeof(MmsConstants.WapMmsType), Convert.ToString(userData[index]));
                        index++;
                        break;
                    case MmsConstants.MmsVersion:
                        notification.Version = ReadVersion(userData[index]);
                        index++;
                        break;
                    case MmsConstants.MmsTo:
                        notification.To = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsFrom:
                        int valueLength = userData[index++];
                        int addressPresentToken = UnsignedByte(userData[index++]);
                        if (addressPresentToken == 0x80)
                        {
                            // Address-present-token
                            notification.From = ReadTextString(userData, ref index);
                        }
                        break;
                    case MmsConstants.MmsMessageId:
                        notification.MessageId = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsTransactionId:
                        notification.TransactionId = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsDate:
                        int length = userData[index++];
                        long msecs = ReadMultipleByteInt(userData, length, ref index) * 1000;
                        notification.MessageDate = new DateTime((msecs * TimeSpan.TicksPerMillisecond) + MmsConstants.Ticks1970);
                        break;
                    case MmsConstants.MmsReadStatus:
                        notification.MessageReadStatus = (MmsConstants.MessageReadStatus)Enum.Parse(typeof(MmsConstants.MessageReadStatus), Convert.ToString(userData[index]));
                        index++;
                        break;
                    default:
                        index++;
                        break;
                }
            }
            return notification;
        }

        /// <summary>
        /// Decodes the MMS delivery notification.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        private static MessageInformation DecodeMmsDeliveryNotification(byte[] userData)
        {
            MmsDeliveryNotification notification = new MmsDeliveryNotification();
            int index = 0;
            // Position 0 is the transaction id
            notification.TransactionId = Convert.ToString(userData[index++]);

            // Position 1 is the PDU type
            // Position 2 is the header length
            notification.HeaderLength = userData[++index];

            // Read content type            
            //notification.ContentType = ReadTextString(userData, ref index);

            // Skip AF86 - x-wap-application:mms.ua
            index = index + notification.HeaderLength + 1;      // 1 is the header length field


            // Start reading the content
            while (index < userData.Length)
            {
                switch (userData[index++])
                {
                    case MmsConstants.MmsType:
                        notification.MessageType = (MmsConstants.WapMmsType)Enum.Parse(typeof(MmsConstants.WapMmsType), Convert.ToString(userData[index]));
                        index++;
                        break;
                    case MmsConstants.MmsVersion:
                        notification.Version = ReadVersion(userData[index]);
                        index++;
                        break;
                    case MmsConstants.MmsTo:
                        notification.To = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsMessageId:
                        notification.MessageId = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsTransactionId:
                        notification.TransactionId = ReadTextString(userData, ref index);
                        break;
                    case MmsConstants.MmsDate:
                        int length = userData[index++];
                        long msecs = ReadMultipleByteInt(userData, length, ref index) * 1000;
                        notification.MessageDate = new DateTime((msecs * TimeSpan.TicksPerMillisecond) + MmsConstants.Ticks1970);
                        break;
                    case MmsConstants.MmsStatus:
                        notification.MessageStatus = (MmsConstants.MessageStatus)Enum.Parse(typeof(MmsConstants.MessageStatus), Convert.ToString(userData[index]));
                        index++;
                        break;
                    default:
                        index++;
                        break;
                }
            }
            return notification;
        }

        /// <summary>
        /// Reads the text string.
        /// </summary>
        /// <returns>The text string</returns>
        private static string ReadTextString(byte[] userData, ref int index)
        {
            string value = string.Empty;

            if (userData[index] == 0x22)
            {
                // in this case it's a "Quoted-string"
                index++;
            }

            List<byte> byteList = new List<byte>(10);

            while (userData[index] > 0)
            {
                //value = value + (char) m_In[m_i++];
                byteList.Add(userData[index++]);
                if (index >= userData.Length) break;
            }
            byte[] byteArray = byteList.ToArray();


            Encoding enc = Encoding.UTF8;
            value = enc.GetString(byteArray);

            index++;

            return value;
        }

        /// <summary>
        /// Reads the multiple byte int.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="length">The length.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static long ReadMultipleByteInt(byte[] userData, int length, ref int index)
        {
            long value = 0L;
            int start = index;
            int end = index + length - 1;

            for (int ii = end, weight = 1; ii >= start; ii--, weight *= 256)
            {
                int bv = UnsignedByte(userData[ii]);
                value = value + bv * weight;
            }

            index = end + 1;

            return value;
        }

        /// <summary>
        /// Unsigneds the byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static int UnsignedByte(byte value)
        {
            if (value < 0)
            {
                return (value + 256);
            }
            else
                return value;
        }


        /// <summary>
        /// MMS-version-value = Short-integer
        /// The three most significant bits of the Short-integer are interpreted to encode a major version number in the range 1-7, and the
        /// four least significant bits contain a minor version number in the range 0-14. If there is only a major version number, this is
        /// encoded by placing the value 15 in the four least significant bits [WAPWSP].
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>The version number</returns>
        private static string ReadVersion(byte version)
        {
            if (version == MmsConstants.MmsVersion10)
            {
                return "1.0";
            }
            else if (version == MmsConstants.MmsVersion11)
            {
                return "1.1";
            }
            else if (version == MmsConstants.MmsVersion12)
            {
                return "1.2";
            }
            return Convert.ToString(version);
        }

        /// <summary>
        /// Reads the length of the value.
        /// </summary>
        /// <returns></returns>
        private static int ReadValueLength(byte[] userData, ref int index)
        {
            int length = 0;
            int temp = userData[index++];

            if (temp < 31)
            {
                length = temp;
            }
            else if (temp == 31)
            {
                length = ReadUintVar(userData, ref index);
            }

            return length;
        }


        /// <summary>
        /// Reads the uint var.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static int ReadUintVar(byte[] userData, ref int index)
        {
            int value = 0;
            int bv = UnsignedByte(userData[index]);

            if (bv < 0x80)
            {
                value = bv;
                index++;
            }
            else
            {
                // In this case the format is "Variable Length Unsigned Integer"
                bool flag = true;
                short count = 0, inc = 0;

                // Count the number of byte needed for the number
                while (flag)
                {
                    flag = (userData[index + count] & 0x80) == 0x80;
                    count++;
                }

                inc = count;
                count--;

                int weight = 1;
                while (count >= 0)
                {
                    bv = DecodeByte(userData[index + count]) * weight;
                    weight *= 128;
                    value = value + bv;
                    count--;
                }
                index += inc;
            }
            return value;
        }

        /// <summary>
        /// Decodes the byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static byte DecodeByte(byte value)
        {
            return ((byte)(value & 0x7F));
        }
    }
}
