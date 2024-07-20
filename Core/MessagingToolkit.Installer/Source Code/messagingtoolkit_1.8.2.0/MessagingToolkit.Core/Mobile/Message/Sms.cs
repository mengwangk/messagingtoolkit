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
using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Short Message Service (SMS) class
    /// </summary>
    [global::System.Serializable]
    public class Sms : BaseSms
    {
        #region ========================= Protected Members   =====================

        /// <summary>
        /// User data header indicator
        /// </summary>
        protected byte userDataHeaderIndicator;


        #endregion ================================================================


        #region ========================= Constructor =============================

        /// <summary>
        /// Constructor
        /// </summary>
        protected Sms():base()
        {
            // Default to PDU
            Protocol = MessageProtocol.PduMode;

            // Default to -1
            SourcePort = -1;

            // Default to -1
            DestinationPort = -1;

            // Create a list of size 1
            ReferenceNo = new List<int>(1);

            // Create a list of size 1
            Indexes = new List<int>(1);

            // Default to false
            SaveSentMessage = false;

            // Default to false
            RawMessage = false;

            // Default to false
            Flash = false;

            // Default to concatenate
            LongMessageOption = MessageSplitOption.Concatenate;

            // Set to blank
            ReplyPath = string.Empty;
        }

        #endregion ================================================================

        #region ========================= Protected Method  =======================

        /// <summary>
        /// Get the first octet
        /// </summary>
        /// <returns>The first octet</returns>
        protected string FirstOctet()
        {
            return PduUtils.ByteToHex(
                            Convert.ToByte(
                            (int)Enum.Parse(typeof(MessageTypeIndicator), MessageTypeIndicator.MtiSmsSubmit.ToString())
                            +
                            (int)Enum.Parse(typeof(MessageValidityPeriodFormat), MessageValidityPeriodFormat.RelativeFormat.ToString())
                            +
                            (int)Enum.Parse(typeof(MessageStatusReportRequest), StatusReportRequest.ToString()) + userDataHeaderIndicator));
        }             

        #endregion ================================================================


        #region ========================= Public Properties =======================

        /// <summary>
        /// Destination mobile number
        /// </summary>
        /// <value>Destination mobile number</value>
        public string DestinationAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Message sending protocol. Default to PDU
        /// </summary>
        /// <value>Messaging sending protocol. See <see cref="MessageProtocol"/></value>
        public MessageProtocol Protocol
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate if this is a flash SMS
        /// </summary>
        /// <value>true if it is a flash message</value>
        public bool Flash
        {
            get
            {
                if (DcsMessageClass == MessageClasses.Flash) 
                    return true;
                return false;
            }
            set
            {
                if (value) 
                    DcsMessageClass = MessageClasses.Flash;
                else 
                    DcsMessageClass = MessageClasses.None;
            }
        }

        /// <summary>
        /// Source port
        /// </summary>
        /// <value>source port</value>
        public int SourcePort
        {
            get;
            set;
        }

        /// <summary>
        /// Destination port
        /// </summary>
        /// <value>Destination port</value>
        public int DestinationPort
        {
            get;
            set;
        }

        /// <summary>
        /// List of references number for this messages.
        /// If it is a EMS, then it consists of more than 1 reference number
        /// </summary>
        /// <value>List of reference numbers</value>
        public List<int> ReferenceNo
        {
            get;
            set;
        }

        /// <summary>
        /// Message indexes in the message storage
        /// </summary>
        /// <value>Message indexes</value>
        public List<int> Indexes
        {
            get;
            set;
        }

        /// <summary>
        /// Store sent message
        /// </summary>
        /// <value>true if do not want to store, false otherwise</value>
        public bool SaveSentMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is a raw mesage
        /// </summary>
        /// <value><c>true</c> if [raw message]; otherwise, <c>false</c>.</value>
        public bool RawMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Message split option for long message. Default is to
        /// concatenate the message
        /// See <see cref="MessageSplitOption"/>
        /// </summary>
        /// <value>Long message split option</value>
        public MessageSplitOption LongMessageOption
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the reply path.
        /// </summary>
        /// <value>The reply path.</value>
        public string ReplyPath
        {
            get;
            set;
        }

        #endregion ================================================================

        #region ========================= Public Method ===========================
             
        /// <summary>
        /// Encode SMS
        /// </summary>
        /// <returns>SMS PDU code</returns>
        internal string GetSmsPduCode()
        {
            string pduCode = string.Empty;
            string encodedContent = PduUtils.EncodeContent(this);
            ContentLength = PduUtils.GetContentLength(this);

            // Check User Data Length
            if (DataCodingScheme == MessageDataCodingScheme.DefaultAlphabet)
            {
                if (encodedContent.Length > 280) throw new GatewayException(Resources.MessageTooLong);
            }
            if (DataCodingScheme == MessageDataCodingScheme.Ucs2)
            {
                if (encodedContent.Length > 280) throw new GatewayException(Resources.MessageTooLong);
            }
            if (DataCodingScheme == MessageDataCodingScheme.SevenBits)
            {
                if (encodedContent.Length > 280) throw new GatewayException(Resources.MessageTooLong);
            }
            // Make PDUCode
            pduCode = PduUtils.EncodeServiceCenterAddress(ServiceCenterNumber);
            pduCode += FirstOctet();
            pduCode += MessageReference.ToString("X2");
            pduCode += PduUtils.EncodeDestinationAddress(DestinationAddress);
            pduCode += protocolIdentifier.ToString("X2");

            pduCode += PduUtils.GetEnumHexValue(typeof(MessageDataCodingScheme), DataCodingScheme.ToString());
            pduCode += PduUtils.GetEnumHexValue(typeof(MessageValidPeriod), ValidityPeriod.ToString());

            pduCode += ContentLength.ToString("X2");
            pduCode += encodedContent;
            return pduCode;
        }
         


        /// <summary>
        /// Encode EMS
        /// </summary>
        /// <returns>EMS PDU code</returns>
        internal string[] GetEmsPduCode()
        {
            int totalMessages = 0;
            string encodedContent = PduUtils.EncodeContent(this);
            ContentLength = PduUtils.GetContentLength(this);

            switch (DataCodingScheme)
            {
                case MessageDataCodingScheme.Ucs2:
                    totalMessages = Convert.ToInt32((encodedContent.Length / 4)) / 66 + Convert.ToInt32(((encodedContent.Length / 4 % 66) == 0));
                    break;
                case MessageDataCodingScheme.EightBits:
                case MessageDataCodingScheme.Class1Ud8Bits:
                    totalMessages = Convert.ToInt32((encodedContent.Length / 4)) / 66 + Convert.ToInt32(((encodedContent.Length / 4 % 66) == 0));
                    break;
                case MessageDataCodingScheme.DefaultAlphabet:
                    totalMessages = (encodedContent.Length / 266) - Convert.ToInt32(((encodedContent.Length % 266) == 0));
                    break;
                case MessageDataCodingScheme.SevenBits:
                    totalMessages = (encodedContent.Length / 266) - Convert.ToInt32(((encodedContent.Length % 266) == 0));
                    break;
            }

            string[] result = new string[totalMessages + 1];
            string tmpTpUd = string.Empty;
            int i = 0;
            userDataHeaderIndicator = Convert.ToByte(Math.Pow(2, 6));

            int reference = Convert.ToInt32(new Random().Next(1) * 65536);

            // 16bit Reference Number 'See 3GPP Document
            for (i = 0; i <= totalMessages; i++)
            {
                tmpTpUd = string.Empty;
                int length = 0;
                int index = 0;
                switch (DataCodingScheme)
                {
                    case MessageDataCodingScheme.Ucs2:
                        length = 66 * 4;
                        index = i * 66 * 4;
                        if ((index + length) > encodedContent.Length)
                        {
                            length = encodedContent.Length - index;
                        }   
                        if (index < encodedContent.Length)
                            tmpTpUd = encodedContent.Substring(index, length);
                        break;
                    // When TP_UDL is odd, the max length of an Unicode string in PDU code is 66 Charactor.See [3GPP TS 23.040 V6.5.0 (2004-09] 9.2.3.24.1
                    case MessageDataCodingScheme.DefaultAlphabet:
                        length = 133 * 2;
                        index = i * 133 * 2;
                        if ((index + length) > encodedContent.Length)
                        {
                            length = encodedContent.Length - index;    
                        }              
                        if (index < encodedContent.Length)
                            tmpTpUd = encodedContent.Substring(index, length);
                        break;
                }
                // skip if empty
                if (string.IsNullOrEmpty(tmpTpUd)) continue;

                result[i] = PduUtils.EncodeServiceCenterAddress(ServiceCenterNumber);
                result[i] += FirstOctet();
                result[i] += MessageReference.ToString("X2");
                // Next segement TP_MR must be increased
                // tpMr += 1
                result[i] += PduUtils.EncodeDestinationAddress(DestinationAddress);
                result[i] += protocolIdentifier.ToString("X2");
                result[i] += PduUtils.GetEnumHexValue(typeof(MessageDataCodingScheme), DataCodingScheme.ToString());
                result[i] += PduUtils.GetEnumHexValue(typeof(MessageValidPeriod), ValidityPeriod.ToString());
                if (DataCodingScheme == MessageDataCodingScheme.Ucs2)
                {
                    ContentLength = Convert.ToInt32(tmpTpUd.Length / 2 + 6 + 1);

                }
                if (DataCodingScheme == MessageDataCodingScheme.DefaultAlphabet)
                {
                    ContentLength = Convert.ToInt32((tmpTpUd.Length + 7 * 2) * 4 / 7);
                }
                // still problem here:
                // when the charcter is several times of 7 of the last message, tp_udl will not correct!
                // to correct this problem i write some code below. that's may not perfect solution.
                // if  (i == totalMessages && ((tmpTpUd.Length % 14) = 0) {
                //  tp_udl -= 1
                // }
                                  
                result[i] += ContentLength.ToString("X2");
                result[i] += "060804";
                // TP_UDHL and some of Concatenated message
                result[i] += reference.ToString("X4");
                result[i] += (totalMessages + 1).ToString("X2");
                result[i] += (i + 1).ToString("X2");
                result[i] += tmpTpUd;
            }
            return result;
        }        
       

        #endregion =========================================================================================

        
        #region ============== Factory method   ============================================================

        /// <summary>
        /// Static factory to create the SMS object
        /// </summary>
        /// <returns>A new instance of the SMS object</returns>
        public static Sms NewInstance()
        {
            Sms sms =  new Sms();
            sms.DataCodingScheme = MessageDataCodingScheme.Undefined;
            return sms;
        }

        #endregion ========================================================================================

        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "ServiceCenterNumber = ", ServiceCenterNumber, "\r\n");
            str = String.Concat(str, "DataCodingScheme = ", DataCodingScheme, "\r\n");
            str = String.Concat(str, "Content = ", Content, "\r\n");
            str = String.Concat(str, "ContentLength = ", ContentLength, "\r\n");
            str = String.Concat(str, "StatusReportRequest = ", StatusReportRequest, "\r\n");
            str = String.Concat(str, "ValidityPeriod = ", ValidityPeriod, "\r\n");
            str = String.Concat(str, "CustomValidityPeriod = ", CustomValidityPeriod, "\r\n");
            str = String.Concat(str, "DcsMessageClass = ", DcsMessageClass, "\r\n");
            str = String.Concat(str, "Binary = ", Binary, "\r\n");
            str = String.Concat(str, "DataBytes = ", DataBytes, "\r\n");
            str = String.Concat(str, "DestinationAddress = ", DestinationAddress, "\r\n");
            str = String.Concat(str, "Protocol = ", Protocol, "\r\n");
            str = String.Concat(str, "Flash = ", Flash, "\r\n");
            str = String.Concat(str, "SourcePort = ", SourcePort, "\r\n");
            str = String.Concat(str, "DestinationPort = ", DestinationPort, "\r\n");
            str = String.Concat(str, "ReferenceNo = ", ReferenceNo, "\r\n");
            str = String.Concat(str, "Indexes = ", Indexes, "\r\n");
            str = String.Concat(str, "SaveSentMessage = ", SaveSentMessage, "\r\n");
            str = String.Concat(str, "RawMessage = ", RawMessage, "\r\n");
            str = String.Concat(str, "LongMessageOption = ", LongMessageOption, "\r\n");
            str = String.Concat(str, "ReplyPath = ", ReplyPath, "\r\n");
            return str;
        }


    }

}


