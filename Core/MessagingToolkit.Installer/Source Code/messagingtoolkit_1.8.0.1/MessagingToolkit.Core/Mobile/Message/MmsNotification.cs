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
    /// A MMS notification is an indication on your mobile phone, that tells you that a new MMS message for you has arrived at the MMSC (Multimedia Message Service Centre)
    /// </summary>
    ///  
    /// <code>
    /// Depending on your phone's settings, it will automatically download the MMS message from the MMSC, if there is a GPRS or UMTS connection configured.
    /// The MMS notification message is encoded according to the MMS encapsulation protocol.
    /// 
    /// There are eight types of PDU's described in this protocol:
    ///
    /// M-Send.req
    /// M-Send.conf
    /// GET.req
    /// M-Retrieve.conf
    /// M-Notification.ind
    /// M-NotifyResp.ind
    /// M-Delivery.ind
    /// M-Acknowledge.req
    ///     
    /// For MMS notification messages the 'M-Notification.ind' PDU is used. This PDU contains the following fields:
    ///     Name	                 Content	                Description
    ///     X-Mms-Message-Type	    m-notification-ind	        specify the pdu type
    ///     X-Mms-Transaction-ID	Transaction-id-value	    Unique ID to identify the transaction
    ///     X-Mms-MMS-Version	    MMS-version-value	        MMS Version: 1.0
    ///     From	                From-value	                Address of the sender, if left blank, this address will be added by the MMSC
    ///     Subject	                Subject-value	            Optional: subject of the MMS message
    ///     X-Mms-Message-Class	    Message-class-value	        Message Class: private, informational, advertising or auto
    ///     X-Mms-Message-Size	    Message-size-value	        Size of the MMS message in bytes
    ///     X-Mms-Expiry	        Expiry-value	            Expiration date of the MMS message on the MMSC
    ///     X-Mms-Content-Location	Content-location-value	    The location of the MMS message (for instance http://mmsc.com/mmsc/133/145a.mms)
    ///     
    /// 
    ///     Sample of a MMS notification message in binary encoding:
    ///
    ///     0x8C	X-Mms-Message-Type
    ///     0x82	'm-notification-ind'
    ///     0x98	X-Mms-Transaction-ID
    ///     0x34	'4'
    ///     0x35	'5'
    ///     0x41	'A'
    ///     0x36	'6'
    ///     0x37	'7'
    ///     0x32	'2'
    ///     0x33	'3'
    ///     0x37	'7'
    ///     0x00	Terminating Zero
    ///     0x8D	X-Mms-MMS-Version
    ///     0x90	'1.0'
    ///     0x89	From
    ///     0x0F	FieldSize
    ///     0x80	Field is present ( 0x81 when no From address is specified )
    ///     0x2B	'+'
    ///     0x33	'3'
    ///     0x31	'1'
    ///     0x36	'6'
    ///     0x33	'3'
    ///     0x38	'8'
    ///     0x37	'7'
    ///     0x34	'4'
    ///     0x30	'0'
    ///     0x31	'1'
    ///     0x36	'6'
    ///     0x30	'0'
    ///     0x00	Terminating Zero
    ///     0x96	Subject
    ///     0x4D	'M'
    ///     0x4D	'M'
    ///     0x53	'S'
    ///     0x00	Terminating Zero
    ///     0x8A	X-Mms-Message-Class
    ///     0x80	'Personal'
    ///     0x8E	X-Mms-Message-Size
    ///     0x04	4 bytes
    ///     0x00	
    ///     0x00	
    ///     0x1E
    ///     0xD3	7891 Bytes
    ///     0x88	X-Mms-Expiry
    ///     0x06	Field Size
    ///     0x80	Absolute Date Format ( 0x81 = Relative Date Format )
    ///     0x04	Size of Time field
    ///     0x45	
    ///     0xA7
    ///     0xC3
    ///     0xB7	1168622519 Seconds from 1-1-1970
    ///     0x83	X-Mms-Content-Location
    ///     0x68	'h'
    ///     0x74	't'
    ///     0x74	't'
    ///     0x70	'p'
    ///     0x3A	':'
    ///     0x6D	'/'
    ///     0x6D	'/'
    ///     ...
    ///     0x00	Terminating Zero
    ///       
    ///
    /// MMS Notifications provide the MMS Client with information (e.g. message class and expiry time) about a MM located
    /// at the recipient MMS Proxy -Relay and waiting for retrieval. The purpose of the notification is to allow the client to
    /// automatically fetch a MM from the location indicated in the notification.
    /// The transaction identifier is created by the MMS Proxy -Relay and is unique up to the following M-NotifyResp.ind only.
    ///
    /// Note: If the MMS Notification is resent at a later point in time - prior to receiving a corresponding M-NotifyResp.ind
    /// - then this MMS Notification must be identical to the original MMS Notification.
    /// If the MMS Client requests deferred retrieval with M-NotifyResp.ind, the MMS Proxy -Relay MAY create a new
    /// transaction identifier.
    /// 
    /// Table 3 contains the field names, the field values and descriptions of the header fields of the M-Notification.ind PDU.
    ///
    /// Field: X-Mms-Message-Type 
    /// Value: Message-type-value = m-notification-ind
    /// Description: Mandatory. Specifies the PDU type.
    /// 
    /// Field: X-Mms-Transaction-ID 
    /// Value: Transaction-id-value Mandatory.
    /// Description: This transaction ID identifies the M-Notification.ind and
    ///              the corresponding M-NotifyResp.ind
    ///
    /// Field: X-Mms-MMS-Version 
    /// Value: MMS-version-value 
    /// Description: Mandatory. The MMS version number
    /// 
    /// Field: From 
    /// Value: From-value Optional.
    /// Description: Address of the last MMS Client that handled the MM,
    ///             i.e. that sent or forwarded the MM. If hiding the address
    ///             of the sender from the recipient is requested by the
    ///             originating MMS Client and supported and accepted by
    ///             the MMS Proxy -Relay, the MMS Proxy -Relay MUST
    ///             NOT add this field to the M-Notification.ind PDU.
    ///             The insert-address-token MUST NOT be used as the
    ///             value of the field.
    ///
    /// Field: Subject 
    /// Value: Subject-value 
    /// Description: Optional. Subject of the message.
    ///
    /// Field: X-Mms-Delivery-Report
    /// Valule: Delivery-report-value 
    /// Description: Optional. Specifies whether the user wants a delivery report from
    ///              each recipient. The absence of the field does not indicate
    ///              any default value.
    ///
    /// Field: X-Mms-Message-Class 
    /// Value: Message-class-value 
    /// Description: Mandatory. Class of the message.
    ///             The MMS Proxy -Relay M UST provide the Personal
    ///             message class if the original submission did not include
    ///             the X-Mms-Message-Class field.
    ///
    /// Field: X-Mms-Message-Size 
    /// Value: Message-size-value 
    /// Description: Mandatory.
    ///              Full size of the associated M-Retrieve.conf PDU in
    ///              octets. The value of this header field could be based on
    ///              approximate calculation, therefore it SHOULD NOT be
    ///              used as a reason to reject the MM.
    ///
    /// Field: X-Mms-Expiry 
    /// Value: Expiry-value 
    /// Description: Mandatory.
    ///              Length of time the message will be available. The field
    ///              has only one format, relative.
    ///
    /// Field: X-Mms-Reply-Charging
    /// Value: Reply-charging-value 
    /// Description: Optional.
    ///              If this field is present its value is set to "accepted" or
    ///              "accepted text only" and the MMS-version-value of the
    ///              M-Notification.ind PDU is higher than 1.0, this header
    ///              field will indicate that a reply to this particular MM is
    ///              free of charge for the recipient.
    ///              If the Reply-Charging service is offered and the request
    ///              for reply-charging has been accepted by the MMS
    ///              service provider the value of this header field SHALL be
    ///              set to “accepted” or “accepted text only”.
    ///
    /// Field: X-Mms-Reply-Charging-Deadline
    /// Value: Reply-chargingdeadline-value
    /// Description: Optional.
    ///              This header field SHALL NOT be present if the X-Mms-Reply-Charging header field is not present.
    ///              It SHALL only be interpreted if the value of X-Mms-Reply-Charging header field is set to ”accepted” or
    ///              “accepted text only”. It specifies the latest time the
    ///              recipient has to submit the Reply-MM. After this time
    ///              the originator of the Original-MM will not pay for the
    ///              Reply-MM any more.
    ///
    /// Field: X-Mms-Reply-Charging-Size
    /// Value: Reply-charging-size-value
    /// Description: Optional.
    ///              This header field SHALL NOT be present if the X-Mms-Reply-Charging header field is not present. It specifies
    ///              the maximum size (number of octets) for the Reply-MM.
    ///
    /// Field: X-Mms-Reply-Charging-ID
    /// Value: Reply-charging-ID value
    /// Description: Optional. This header field SHALL only be present in PDUs thatnotify a recipient about a Reply-MM.
    ///              The value of this header field SHALL be the same as the
    ///              Message-ID of the Original-MM that is replied to.
    /// 
    /// Field: X-Mms-Content-Location
    /// Value:  Content-location-value 
    /// Description: Mandatory. This field defines the location of the MM to be retrieved.
    ///
    /// The M-Notification.ind PDU does not contain a message body.
    /// The standard URI format according to [RFC2396] SHALL be set as the Content-location-value, for example:
    /// http://mmsc/message-id
    /// </code>
    [Serializable]
    public class MmsNotification: BaseWapMms
    {
        /// <summary>
        /// Gets or sets the content location.
        /// </summary>
        /// <value>The content location.</value>
        [XmlAttribute]
        public string ContentLocation { get; set; }

        /// <summary>
        /// Gets or sets the message class.
        /// </summary>
        /// <value>The message class.</value>
        [XmlAttribute]
        public MmsConstants.MessageClass MessageClass { get; set; }
        
        /// <summary>
        /// Gets or sets the expiry.
        /// </summary>
        /// <value>The expiry.</value>
        [XmlAttribute]
        public DateTime Expiry { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlAttribute]
        public string Subject { get; set; }


        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        [XmlAttribute]
        public string From { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [expiry absolute].
        /// </summary>
        /// <value><c>true</c> if expiry absolute; otherwise, <c>false</c>.</value>
        [XmlAttribute]
        public bool ExpiryAbsolute { get; set; }


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
            str = String.Concat(str, "ContentLocation = ", ContentLocation, "\r\n");
            str = String.Concat(str, "MessageClass = ", MessageClass, "\r\n");
            str = String.Concat(str, "Expiry = ", Expiry, "\r\n");
            str = String.Concat(str, "Subject = ", Subject, "\r\n");
            str = String.Concat(str, "From = ", From, "\r\n");
            str = String.Concat(str, "ExpiryAbsolute = ", ExpiryAbsolute, "\r\n");
            return str;
        }
    }
}
