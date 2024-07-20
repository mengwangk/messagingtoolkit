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
using MessagingToolkit.MMS;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Base class for MMS
    /// </summary>
    [global::System.Serializable]
    public abstract class BaseMms : MultimediaMessage, IMessage
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMms"/> class.
        /// </summary>
        public BaseMms()
        {
            // Default to current time
            this.ScheduledDeliveryDate = DateTime.Now;

            // Default to false
            this.Persisted = false;
        }

        /// <summary>
        /// Id of gateway used to send the message
        /// </summary>
        /// <value>Gateway id</value>
        public string GatewayId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <value>The last error.</value>
        public Exception LastError
        {
            get;
            protected set;
        }

        /// <summary>
        /// Message queue priority
        /// </summary>
        /// <value>Message priority. See <see cref="MessageQueuePriority"/></value>
        public MessageQueuePriority QueuePriority
        {
            get;
            set;
        }

        /// <summary>
        /// This is a optional property for you to set so that you can
        /// uniquely identify the message especially message is sent
        /// asynchronously.
        /// If you are sending message to queue, this value will be set 
        /// automatically for you if it is not set.
        /// </summary>
        /// <value></value>
        public string Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scheduled delivery date.
        /// </summary>
        /// <value>The scheduled delivery date.</value>
        public DateTime ScheduledDeliveryDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if the message is persisted. default to false
        /// </summary>
        /// <value><c>true</c> if persisted; otherwise, <c>false</c>.</value>
        public bool Persisted
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "PresentationAvailable = ", PresentationAvailable, "\r\n");
            str = String.Concat(str, "MessageTypeAvailable = ", MessageTypeAvailable, "\r\n");
            str = String.Concat(str, "DeliveryReportAvailable = ", DeliveryReportAvailable, "\r\n");
            str = String.Concat(str, "SenderVisibilityAvailable = ", SenderVisibilityAvailable, "\r\n");
            str = String.Concat(str, "ReadReplyAvailable = ", ReadReplyAvailable, "\r\n");
            str = String.Concat(str, "StatusAvailable = ", StatusAvailable, "\r\n");
            str = String.Concat(str, "TransactionIdAvailable = ", TransactionIdAvailable, "\r\n");
            str = String.Concat(str, "MessageIdAvailable = ", MessageIdAvailable, "\r\n");
            str = String.Concat(str, "VersionAvailable = ", VersionAvailable, "\r\n");
            str = String.Concat(str, "DateAvailable = ", DateAvailable, "\r\n");
            str = String.Concat(str, "FromAvailable = ", FromAvailable, "\r\n");
            str = String.Concat(str, "SubjectAvailable = ", SubjectAvailable, "\r\n");
            str = String.Concat(str, "MessageClassAvailable = ", MessageClassAvailable, "\r\n");
            str = String.Concat(str, "ExpiryAvailable = ", ExpiryAvailable, "\r\n");
            str = String.Concat(str, "DeliveryTimeAvailable = ", DeliveryTimeAvailable, "\r\n");
            str = String.Concat(str, "PriorityAvailable = ", PriorityAvailable, "\r\n");
            str = String.Concat(str, "ContentTypeAvailable = ", ContentTypeAvailable, "\r\n");
            str = String.Concat(str, "ToAvailable = ", ToAvailable, "\r\n");
            str = String.Concat(str, "BccAvailable = ", BccAvailable, "\r\n");
            str = String.Concat(str, "CcAvailable = ", CcAvailable, "\r\n");
            str = String.Concat(str, "MessageType = ", MessageType, "\r\n");
            str = String.Concat(str, "MessageId = ", MessageId, "\r\n");
            str = String.Concat(str, "MessageStatus = ", MessageStatus, "\r\n");
            str = String.Concat(str, "TransactionId = ", TransactionId, "\r\n");
            str = String.Concat(str, "VersionAsString = ", VersionAsString, "\r\n");
            str = String.Concat(str, "Version = ", Version, "\r\n");
            str = String.Concat(str, "To = ", To, "\r\n");
            str = String.Concat(str, "Cc = ", Cc, "\r\n");
            str = String.Concat(str, "Bcc = ", Bcc, "\r\n");
            str = String.Concat(str, "Date = ", Date, "\r\n");
            str = String.Concat(str, "Subject = ", Subject, "\r\n");
            str = String.Concat(str, "MessageClass = ", MessageClass, "\r\n");
            str = String.Concat(str, "ContentType = ", ContentType, "\r\n");
            str = String.Concat(str, "NumContents = ", NumContents, "\r\n");
            str = String.Concat(str, "PresentationId = ", PresentationId, "\r\n");
            str = String.Concat(str, "MultipartRelatedType = ", MultipartRelatedType, "\r\n");
            str = String.Concat(str, "Presentation = ", Presentation, "\r\n");
            str = String.Concat(str, "Expiry = ", Expiry, "\r\n");
            str = String.Concat(str, "ExpiryAbsolute = ", ExpiryAbsolute, "\r\n");
            str = String.Concat(str, "DeliveryReport = ", DeliveryReport, "\r\n");
            str = String.Concat(str, "SenderVisibility = ", SenderVisibility, "\r\n");
            str = String.Concat(str, "ReadReply = ", ReadReply, "\r\n");
            str = String.Concat(str, "DeliveryTime = ", DeliveryTime, "\r\n");
            str = String.Concat(str, "DeliveryTimeAbsolute = ", DeliveryTimeAbsolute, "\r\n");
            str = String.Concat(str, "Priority = ", Priority, "\r\n");
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            str = String.Concat(str, "LastError = ", LastError, "\r\n");
            str = String.Concat(str, "QueuePriority = ", QueuePriority, "\r\n");
            str = String.Concat(str, "Identifier = ", Identifier, "\r\n");
            str = String.Concat(str, "ScheduledDeliveryDate = ", ScheduledDeliveryDate, "\r\n");
            str = String.Concat(str, "Persisted = ", Persisted, "\r\n");
            return str;
        }
    }
}
