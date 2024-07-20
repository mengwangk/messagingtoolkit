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
    /// SMS status report
    /// </summary>
    internal class SmsStatusReport : SmsReceived
    {
        #region ========================= Constructor =============================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public SmsStatusReport(string pduCode): base(pduCode)
        {
            messageType = MessageTypeIndicator.MtiSmsStatusReport;
            Decode(pduCode);
        }

        #endregion ================================================================

        #region ========================= Public Properties =======================

        /// <summary>
        /// Destination received date
        /// </summary>
        /// <value></value>
        public DateTime DestinationReceivedDate
        {
            get;
            set;
        }
        /// <summary>
        /// Status for message status report. See <see cref="MessageStatusReportStatus"/>
        /// </summary>
        /// <value>Message status report status</value>
        public MessageStatusReportStatus Status
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
        public override void Decode(string pduCode)
        {
            serviceCenterNumberLength = PduUtils.GetByte(ref pduCode);
            serviceCenterNumberType = PduUtils.GetByte(ref pduCode);
            ServiceCenterNumber = PduUtils.GetAddress(PduUtils.GetString(ref pduCode, (serviceCenterNumberLength - 1) * 2));

            firstOctet = PduUtils.GetByte(ref pduCode);

            MessageReference = PduUtils.GetByte(ref pduCode);

            SourceAddressLength = PduUtils.GetByte(ref pduCode);
            SourceAddressType = PduUtils.GetByte(ref pduCode);
            SourceAddressLength += Convert.ToByte(SourceAddressLength % 2);
            SourceAddress = PduUtils.GetAddress(PduUtils.GetString(ref pduCode, SourceAddressLength));
            string scts = PduUtils.GetString(ref pduCode, 14);
            ServiceCenterTimestamp = PduUtils.GetDate(ref scts, ref timezone);
            scts = PduUtils.GetString(ref pduCode, 14);
            DestinationReceivedDate = PduUtils.GetDate(ref scts, ref timezone);

            string deliveryStatus = Convert.ToString(PduUtils.GetByte(ref pduCode));
            Status = (MessageStatusReportStatus)Enum.Parse(typeof(MessageStatusReportStatus), deliveryStatus.ToString());

            // Status report do not have content so I set it a zero length string
            Content = string.Empty;
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
            str = String.Concat(str, "SourceAddressLength = ", SourceAddressLength, "\r\n");
            str = String.Concat(str, "SourceAddressType = ", SourceAddressType, "\r\n");
            str = String.Concat(str, "SourceAddress = ", SourceAddress, "\r\n");
            str = String.Concat(str, "ServiceCenterTimestamp = ", ServiceCenterTimestamp, "\r\n");
            str = String.Concat(str, "Timezone = ", Timezone, "\r\n");
            str = String.Concat(str, "DestinationReceivedDate = ", DestinationReceivedDate, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            return str;
        }
    }

}
