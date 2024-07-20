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
    /// There are two types of PDUs used for the handling of Read Reports. On the MM recipient side the M-Read-Rec.ind
    /// MUST be used and on the MM originating side the M-Read-Orig.ind MUST be used.
    /// </summary>
    /// 
    /// <code>
    /// M-Read-Rec PDU
    /// --------------
    /// 
    /// Field: X-Mms-Message-Type 
    /// Value: m-read-rec-ind Mandatory.
    /// Description: Identifies the PDU type
    /// 
    /// Field: X-Mms-MMS-Version 
    /// Value: MMS-version-value 
    /// Description: Mandatory. The MMS version number
    /// 
    /// Field: Message-ID 
    /// Value: Message-ID-value 
    /// Description: Mandatory. This is the reference that was originally assigned to the
    ///              MM by the MMS Proxy -Relay and included in the
    ///              corresponding M-Send.conf or M-Forward.conf PDU.
    ///              The ID enables an MMS Client to match read report
    ///              PDUs with previously sent or forwarded MMs.
    /// 
    /// Field: To 
    /// Value: To-value 
    /// Description: Mandatory. The address of the recipient of the Read Report, i.e. the
    ///              originator of the original multimedia message.
    ///
    /// Field: From 
    /// Value: From-value 
    /// Description: Mandatory. Address of the sender of the Read Report. The sending
    ///              client MUST send either its address or insert an address
    ///              token. In case of token, the MMS Proxy -Relay MUST
    ///              insert the correct address of the sender.
    ///
    /// Field: Date 
    /// Value: Date-value 
    /// Description: Optional. Time the message was handled by the recipient MMS
    ///              Client. Recipient MMS Proxy -Relay SHALL generate
    ///              this field when not supplied by the recipient MMS Client.
    ///
    /// Field: X-Mms-Read-Status 
    /// Value: Read-status-value 
    /// Description: Mandatory. The status of the message.
    /// 
    /// 
    /// M-Read-Orig.ind PDU
    /// -------------------
    /// 
    /// Field: X-Mms-Message-Type 
    /// Value: m-read-orig-ind 
    /// Description: Mandatory.Identifies the PDU type
    ///
    /// Field: X-Mms-MMS-Version 
    /// Value: MMS-version-value 
    /// Description: Mandatory.The MMS version number
    /// 
    /// Field: Message-ID 
    /// Value: Message-ID-value 
    /// Description: Mandatory. This is the reference that was originally assigned to the
    ///              MM by the MMS Proxy -Relay and included in the
    ///              corresponding M-Send.conf or M-Forward.conf PDU.
    ///              The ID enables an MMS Client to match read report
    ///              PDUs with previously sent or forwarded MMs.
    /// 
    /// Field: To 
    /// Value: To-value 
    /// Description: Mandatory. The address of the recipient of the Read Report, i.e. the
    ///              originator of the original multimedia message.
    ///
    /// Field: From 
    /// Value: From-value 
    /// Description: Mandatory. The address of the originator of the Read Report, i.e. the
    ///              recipient of the original multimedia message.
    ///              The insert-address-token MUST NOT be used as the
    ///              value of the field.
    ///              
    /// Field: Date 
    /// Value: Date-value 
    /// Description: Mandatory. Time the message was handled by the recipient MMS
    ///              Client.
    ///
    /// Field: X-Mms-Read-Status 
    /// Value: Read-status-value 
    /// Descripition: Mandatory. The status of the message.
    /// 
    /// </code>
    [Serializable]
    public class MmsReadReport: BaseWapMms
    {

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        [XmlAttribute]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        [XmlAttribute]
        public string To { get; set; }


        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>The message id.</value>
        [XmlAttribute]
        public string MessageId { get; set; }     

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
        public MmsConstants.MessageReadStatus MessageReadStatus { get; set; }
    }
}
