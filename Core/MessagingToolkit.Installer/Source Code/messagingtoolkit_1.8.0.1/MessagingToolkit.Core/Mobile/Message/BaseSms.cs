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
using System.Globalization;
using System.Collections;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Base class for all messages
    /// </summary>
    [global::System.Serializable]
    public abstract class BaseSms : BaseMessage, IMessage
    {
        #region ========================= Public Constants ==============================================

        /// <summary>
        /// Default SMSC address
        /// </summary>
        public const string DefaultSmscAddress = "00";

        #endregion ======================================================================================

        #region ========================= Protected Variables ===========================================

        /// <summary>
        /// Service center number length
        /// </summary>
        protected byte serviceCenterNumberLength;

        /// <summary>
        /// Service center number type
        /// </summary>
        protected byte serviceCenterNumberType;

        /// <summary>
        /// First octet
        /// </summary>
        protected byte firstOctet;

        /// <summary>
        /// Protocol identifier. Default to 0 for SMS submit
        /// </summary>
        protected byte protocolIdentifier = 0;

        /// <summary>
        /// Message type indicator
        /// </summary>
        protected MessageTypeIndicator messageType;

        #endregion ========================================================================================

        #region ========================= Protected Constructor============================================

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseSms()
            : base()
        {
            // Default to 1 day
            ValidityPeriod = MessageValidPeriod.OneDay;

            // Default to no status report
            StatusReportRequest = MessageStatusReportRequest.NoSmsReportRequest;

            // Set to none
            DcsMessageClass = MessageClasses.None;

            // Default to false
            Binary = false;

            // Default to 60 minutes
            CustomValidityPeriod = 60;

        }

        #endregion =========================================================================================

        #region ========================= Public Properties ================================================

        /// <summary>
        /// Service center number
        /// </summary>
        /// <value>Service center number</value>
        public string ServiceCenterNumber
        {
            get;
            set;
        }


        /// <summary>
        /// Message data coding scheme. See <see cref="DataCodingScheme"/>
        /// </summary>
        /// <value></value>
        public MessageDataCodingScheme DataCodingScheme
        {
            get;
            set;
        }

        /// <summary>
        /// Message content
        /// </summary>
        /// <value>Content</value>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Message content length
        /// </summary>
        /// <value>Content length</value>
        public int ContentLength
        {
            get;
            protected set;
        }


        /// <summary>
        /// Status report request
        /// </summary>
        /// <value>Message status report. See <see cref="MessageStatusReportRequest"/></value>
        public MessageStatusReportRequest StatusReportRequest
        {
            get;
            set;
        }


        /// <summary>
        /// Message reference number
        /// </summary>
        /// <value>Message reference number</value>
        protected int MessageReference
        {
            get;
            set;
        }

        /// <summary>
        /// Message validity period
        /// </summary>
        /// <value>Message validity period</value>
        public MessageValidPeriod ValidityPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// <para>Gets or sets the custom validity period in minutes. 
        /// Default to 60 minutes</para>
        /// <para>
        /// Set this value if you set 
        /// <code>
        /// Sms.ValidityPeriod = MessageValidPeriod.Custom
        /// </code>
        /// </para>
        /// </summary>
        /// <value>
        /// The custom validity period.
        /// </value>
        public int CustomValidityPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DCS message class.
        /// </summary>
        /// <value>The DCS message class.</value>
        public MessageClasses DcsMessageClass
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate if the message is binary. Only apply for 8 bits encoding.
        /// If it is set to tru, the the <see cref="DataBytes"/> is used instead of
        /// <see cref="Content"/>
        /// </summary>
        /// <value><c>true</c> if binary; otherwise, <c>false</c>.</value>
        public bool Binary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data bytes.
        /// </summary>
        /// <value>The data bytes.</value>
        public byte[] DataBytes
        {
            get;
            set;
        }

        #endregion ======================================================================================



        #region ============== Internal Method  ==========================================================

        /// <summary>
        /// Encode the SMS content
        /// </summary>
        /// <returns>Encoded SMS content</returns>
        internal virtual byte[] GetPdu()
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(this.Content);
            //return MessagingToolkit.Pdu.PduUtils.StringToUnencodedSeptets(this.Content);        

            /*
            if (string.IsNullOrEmpty(this.Content)) return string.Empty;
            string encodedText = string.Empty;
            switch (this.DataCodingScheme)
            {
                case MessageDataCodingScheme.DefaultAlphabet:
                case MessageDataCodingScheme.Undefined:
                case MessageDataCodingScheme.SevenBits:
                    encodedText = PduUtils.TextToPdu(this.Content);
                    break;
                case MessageDataCodingScheme.EightBits:
                case MessageDataCodingScheme.Class1Ud8Bits:
                    for (int i = 0; i < this.Content.Length; i++)
                    {
                        char c = this.Content[i];
                        encodedText += ((((int)c).ToString("X").Length < 2) ? "0" + ((int)c).ToString("X") : ((int)c).ToString("X"));
                    }
                    break;
                case MessageDataCodingScheme.Ucs2:
                    for (int i = 0; i < this.Content.Length; i++)
                    {
                        char c = this.Content[i];
                        int high = (int)(c / 256);
                        int low = c % 256;
                        encodedText += ((high.ToString("X").Length < 2) ? "0" + high.ToString("X") : high.ToString("X"));
                        encodedText += ((low.ToString("X").Length < 2) ? "0" + low.ToString("X") : low.ToString("X"));
                    }
                    break;
                case MessageDataCodingScheme.Custom:
                    int dcs = (int)Enum.Parse(typeof(MessageDataCodingScheme), this.DataCodingScheme.ToString());
                    if ((dcs & 0x04) == 0)
                    {
                        encodedText = PduUtils.TextToPdu(this.Content);
                    }
                    else
                    {
                        for (int i = 0; i < this.Content.Length; i++)
                        {
                            char c = this.Content[i];
                            encodedText += ((((int)c).ToString("X").Length < 2) ? "0" + ((int)c).ToString("X") : ((int)c).ToString("X"));
                        }
                    }
                    break;
            }
            return encodedText;
            */

        }

        #endregion =========================================================================================


        #region ============== Protected  Method  ==========================================================




        #endregion =========================================================================================

    }
}
