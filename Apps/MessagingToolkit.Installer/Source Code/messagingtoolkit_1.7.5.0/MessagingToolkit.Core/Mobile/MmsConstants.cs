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

using MessagingToolkit.MMS;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// MMS constants
    /// </summary>
    public class MmsConstants: MultimediaMessageConstants
    {
        /// <summary>
        /// Suffix for PLMN address
        /// </summary>
        public const string PLMNSuffix = "/TYPE=PLMN";

        /// <summary>
        /// Suffix for IPv4 address
        /// </summary>
        public const string IPv4Suffix = "/TYPE=IPv4";

        /// <summary>
        /// Suffix for IPv6 address
        /// </summary>
        public const string IPv6Suffix = "/TYPE=IPv6";


        /// <summary>
        /// Destination port for MMS notifcation
        /// </summary>
        internal const int MmsNotificationDestinationPort = 2948;

        /// <summary>
        /// MMS mime type
        /// </summary>
        //internal const string ContentTypeWapMmsMessage = "application/vnd.wap.mms-message";


        /// <summary>
        /// WAP MMS type
        /// </summary>
        public enum WapMmsType
        {        
            /// <summary>
            /// m-send-req 0x80 (decimal 128)
            /// </summary>
            MSendReq = 0x80,

            /// <summary>
            /// m-send-conf 0x81 (decimal 129)
            /// </summary>
            MSendConf = 0x81,

            /// <summary>
            /// m-notificationind 0x82 (decimal 130)
            /// </summary>
            MNotificationInd = 0x82,
        
            /// <summary>
            /// m-notifyresp-ind 0x83 (decimal 131)
            /// </summary>
            MNotifyRespInd = 0x83,

            /// <summary>
            /// m-retrieve-conf 0x84 (decimal 132)
            /// </summary>
            MRetrieveConf = 0x84,

            /// <summary>
            ///  m-acknowledgeind 0x85 (decimal 133)
            /// </summary>
            MAcknowledgeInd = 0x85,

            /// <summary>
            /// m-delivery-ind 0x86 (decimal 134)
            /// </summary>
            MDeliveryInd = 0x86,

            /// <summary>
            /// m-read-rec-ind 0x87 (decimal 135)
            /// </summary>
            MReadRecInd = 0x87,

            /// <summary>
            /// m-read-orig-ind 0x88 (decimal 136)
            /// </summary>
            MReadOrigInd = 0x88,

            /// <summary>
            /// m-forward-req 0x89 (decimal 137)
            /// </summary>
            MForwardReq = 0x89,

            /// <summary>
            /// m-forward-conf 0x8A (decimal 138)
            /// </summary>
            MForwardConf = 0x8A
        }

        /// <summary>
        /// Message id
        /// </summary>
        internal const int MmsMessageId = 0x8B;

        /// <summary>
        /// MMS type
        /// </summary>
        internal const int MmsType = 0x8C;

        /// <summary>
        /// MMS version
        /// </summary>
        internal const int MmsVersion = 0x8D;

        /// <summary>
        /// To field
        /// </summary>
        internal const int MmsTo = 0x97;

        /// <summary>
        /// Transaction id
        /// </summary>
        internal const int MmsTransactionId = 0x98;

        /// <summary>
        /// Message class
        /// </summary>
        internal const int MmsMessageClass = 0x8A;

        /// <summary>
        /// MMS date
        /// </summary>
        internal const int MmsDate = 0x85;

        /// <summary>
        /// MMS status
        /// </summary>
        internal const int MmsStatus = 0x95;

        /// <summary>
        /// MMS content location
        /// </summary>
        internal const int MmsContentLocation = 0x83;

        /// <summary>
        /// MMS expiry date field
        /// </summary>
        internal const int MmsExpiry = 0x88;

        /// <summary>
        /// MMS from field
        /// </summary>
        internal const int MmsFrom = 0x89;

        /// <summary>
        /// MMS subject field
        /// </summary>
        internal const int MmsSubject = 0x96;
        
        /// <summary>
        /// MMS message size
        /// </summary>
        internal const int MmsMessageSize = 0x8E;

        /// <summary>
        /// MMS read status
        /// </summary>
        internal const int MmsReadStatus = 0x9B;

        /// <summary>
        /// X-MMS-Status field value
        /// </summary>
        public enum MessageStatus
        {
            /// <summary>
            /// Expired status
            /// </summary>
            Expired = 128,
            /// <summary>
            /// Retrieved status
            /// </summary>
            Retrieved = 129,
            /// <summary>
            /// Rejected
            /// </summary>
            Rejected = 130,
            /// <summary>
            /// Deferred status
            /// </summary>
            Deferred = 131,
            /// <summary>
            /// Unrecognised status
            /// </summary>
            Unrecognised = 132,
            /// <summary>
            /// Indeterminate status
            /// </summary>
            Indeterminate = 133,
            /// <summary>
            /// Forwarded status
            /// </summary>
            Forwarded = 134
        }

        /// <summary>
        /// Message read status
        /// </summary>
        public enum MessageReadStatus
        {
            /// <summary>
            /// Read 0x80 (decimal 128)
            /// </summary>
            Read = 0x80,
            /// <summary>
            /// Deleted without being read 0x81 (decimal 129
            /// </summary>
            DeletedWithoutBeingRead = 0x81
        }

        /// <summary>
        /// WAP MMS message class
        /// </summary>
        public enum MessageClass
        {
            /// <summary>
            ///  Personal 0x80 (decimal 128)
            /// </summary>
            Personal = 0x80,
            /// <summary>
            /// Advertisement 0x81 (decimal 129)
            /// </summary>
            Advertisement = 0x81,
            /// <summary>
            /// Informational 0x82 (decimal 130)
            /// </summary>
            Informational = 0x82,
            /// <summary>
            /// Auto 0x83 (decimal 131)
            /// </summary>
            Auto = 0x83

        }

        /// <summary>
        /// Delivery report request
        /// </summary>
        public enum DeliveryReport
        {
            /// <summary>
            /// Yes 0x80 (decimal 128)
            /// </summary>
            Yes = 0x80,
            /// <summary>
            /// No 0x81 (decimal 129)
            /// </summary>
            No = 0x81
        }
        
    }
}
