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
using System.Xml.Serialization;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// A MMS Delivery Report MUST be sent from the MMS Proxy -Relay to the originator MMS Client or the forwarding
    /// MMS Client when a delivery report has been requested and the recipient MMS Client has not explicitly requested for
    /// denial of the report. As for example, the recipient can request for denial of the Delivery Report by using the X-Mms-
    /// Report-Allowed field of M-Acknowledge.ind or M-NotifyResp.ind PDU. There will be a separate delivery report from
    /// each recipient. There is no response PDU to the delivery report.
    ///</summary>
    /// <code>
    /// Field: X-Mms-Message-Type  
    /// Value: Message-type-value = mdelivery-ind  
    /// Description: Mandatory. Specifies the PDU type.
    /// 
    /// Field: X-Mms-MMS-Version   
    /// Value:MMS-version-value                   
    /// Description: Mandatory. The MMS version number. 
    /// 
    /// Field: Message-ID 
    /// Value: Message-ID-value 
    /// Description: Mandatory. This is the reference that was originally assigned to the MM by the MMS Proxy -Relay and included in the corresponding M-Send.conf or M-Forward.conf PDU.
    ///             The ID enables an MMS Client to match delivery reports with previously sent or forwarded MMs.
    /// 
    /// Field: To 
    /// Value: To-value 
    /// Description: Mandatory. Needed for reporting in case of point-to-multipoint message.
    ///
    /// Field: Date 
    /// Value: Date-value 
    /// Description: Mandatory.Date and time the message was handled (fetched, expired, etc.) by the recipient or MMS Proxy -Relay.
    ///
    /// Field: X-Mms-Status 
    /// Value: Status-value 
    /// Description: Mandatory.The status of the message.
    /// 
    /// </code>
    [Serializable]
    public class MmsDeliveryNotification: BaseWapMms
    {

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>The message id.</value>
        [XmlAttribute]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        [XmlAttribute]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the message date.
        /// </summary>
        /// <value>The message date.</value>
        [XmlAttribute]
        public DateTime MessageDate { get; set; }


        /// <summary>
        /// Gets or sets the message status.
        /// </summary>
        /// <value>The message status.</value>
        [XmlAttribute]
        public MmsConstants.MessageStatus MessageStatus { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "MessageType = ", MessageType, "\r\n");
            str = String.Concat(str, "Version = ", Version, "\r\n");
            str = String.Concat(str, "TransactionId = ", TransactionId, "\r\n");
            str = String.Concat(str, "HeaderLength = ", HeaderLength, "\r\n");
            str = String.Concat(str, "MessageId = ", MessageId, "\r\n");
            str = String.Concat(str, "To = ", To, "\r\n");
            str = String.Concat(str, "MessageDate = ", MessageDate, "\r\n");
            str = String.Concat(str, "MessageStatus = ", MessageStatus, "\r\n");
            return str;
        }
    }
}
