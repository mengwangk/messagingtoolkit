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

using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Sms submit
    /// </summary>
    internal class SmsSubmit : BaseSms
    {
        #region ========================= Constructor =============================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public SmsSubmit(string pduCode)
        {
            messageType = MessageTypeIndicator.MtiSmsSubmit;
            Decode(pduCode);
        }

        #endregion ================================================================

        #region ========================= Public Properties =======================

        /// <summary>
        /// Destination address length
        /// </summary>
        /// <value>Address length</value>
        public byte DestinationAddressLength
        {
            get;
            set;
        }

        /// <summary>
        /// Destination address type
        /// </summary>
        /// <value>Address type</value>
        public byte DestinationAddressType
        {
            get;
            set;
        }

        /// <summary>
        /// Destination address
        /// </summary>
        /// <value>Destination address</value>
        public string DestinationAddress
        {
            get;
            set;
        }
              

        #endregion ================================================================


        #region ========================= Public Method ===========================

        /// <summary>
        /// Decode the SMS
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public virtual void Decode(string pduCode)
        {
            serviceCenterNumberLength = PduUtils.GetByte(ref pduCode);
            serviceCenterNumberType = PduUtils.GetByte(ref pduCode);
            ServiceCenterNumber = PduUtils.GetAddress((PduUtils.GetString(ref pduCode, (serviceCenterNumberLength - 1) * 2)));
            firstOctet = PduUtils.GetByte(ref pduCode);

            MessageReference = PduUtils.GetByte(ref pduCode);

            DestinationAddressLength = PduUtils.GetByte(ref pduCode);
            DestinationAddressType = PduUtils.GetByte(ref pduCode);
            DestinationAddressLength += Convert.ToByte(DestinationAddressLength % 2);
            DestinationAddress = PduUtils.GetAddress((PduUtils.GetString(ref pduCode, DestinationAddressLength)));

            protocolIdentifier = PduUtils.GetByte(ref pduCode);
            int dcs = PduUtils.GetByte(ref pduCode);
            if (dcs >= 16) dcs -= 16;
            DataCodingScheme = (MessageDataCodingScheme)Enum.Parse(typeof(MessageDataCodingScheme), Convert.ToString(dcs));
            ValidityPeriod = (MessageValidPeriod)Enum.Parse(typeof(MessageValidPeriod), PduUtils.GetByte(ref pduCode).ToString()); 
            ContentLength = PduUtils.GetByte(ref pduCode);
            Content = PduUtils.GetString(ref pduCode, ContentLength * 2);
        }

        #endregion ================================================================

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
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
            str = String.Concat(str, "DestinationAddressLength = ", DestinationAddressLength, "\r\n");
            str = String.Concat(str, "DestinationAddressType = ", DestinationAddressType, "\r\n");
            str = String.Concat(str, "DestinationAddress = ", DestinationAddress, "\r\n");
            return str;
        }
    }


}
