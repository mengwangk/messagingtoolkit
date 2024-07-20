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
    /// SMS received
    /// </summary>
    internal class SmsReceived : BaseSms
    {
        #region ========================= Protected Members   =====================

        /// <summary>
        /// Timezone
        /// </summary>
        protected string timezone;

        #endregion ================================================================

        #region ========================= Public Constructor  =====================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public SmsReceived(string pduCode)
        {
            messageType = MessageTypeIndicator.MtiSmsDeliver;
            Decode(pduCode);
        }

        #endregion ================================================================

        #region ========================= Public Properties  ======================

        /// <summary>
        /// Source address length
        /// </summary>
        /// <value>Address length</value>
        public byte SourceAddressLength
        {
            get;
            set;
        }
        /// <summary>
        /// Source address type
        /// </summary>
        /// <value>Address type</value>
        public byte SourceAddressType
        {
            get;
            set;
        }

        /// <summary>
        /// Source address value
        /// </summary>
        /// <value>Address value</value>
        public string SourceAddress
        {
            get;
            set;
        }


        /// <summary>
        /// Service center timestamp
        /// </summary>
        /// <value>Timestamp</value>
        public DateTime ServiceCenterTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Timezone
        /// </summary>
        /// <value>Timezone</value>
        public string Timezone
        {
            get
            {
                return timezone;
            }
            set
            {
                timezone = value;
            }
        }

        #endregion ================================================================


        #region ========================= Public Method ===========================

        /// <summary>
        /// Decode the received SMS
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public virtual void Decode(string pduCode)
        {
            serviceCenterNumberLength = PduUtils.GetByte(ref pduCode);
            serviceCenterNumberType = PduUtils.GetByte(ref pduCode);
            ServiceCenterNumber = PduUtils.GetAddress((PduUtils.GetString(ref pduCode, (serviceCenterNumberLength - 1) * 2)));
            firstOctet = PduUtils.GetByte(ref pduCode);

            SourceAddressLength = PduUtils.GetByte(ref pduCode);
            SourceAddressType = PduUtils.GetByte(ref pduCode);
            SourceAddressLength += Convert.ToByte(SourceAddressLength % 2);
            SourceAddress = PduUtils.GetAddress((PduUtils.GetString(ref pduCode, SourceAddressLength)));

            protocolIdentifier = PduUtils.GetByte(ref pduCode);
            DataCodingScheme = (MessageDataCodingScheme)Enum.Parse(typeof(MessageDataCodingScheme), Convert.ToString(PduUtils.GetByte(ref pduCode)));
            string scts = PduUtils.GetString(ref pduCode, 14);
            ServiceCenterTimestamp = PduUtils.GetDate(ref scts, ref timezone);
            ContentLength = PduUtils.GetByte(ref pduCode);
            Content = PduUtils.GetString(ref pduCode, ContentLength * 2);
        }

        #endregion ================================================================
    }

}
