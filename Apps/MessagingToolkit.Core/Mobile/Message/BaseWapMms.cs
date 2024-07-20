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
    /// Base class for all WAP MMS message
    /// </summary>
    [Serializable]
    public abstract class BaseWapMms : MessageInformation
    {
        /// <summary>
        /// Message type. 
        /// </summary>
        /// <value>Message type</value>
        [XmlAttribute]
        public new MmsConstants.WapMmsType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute]
        public string Version { get; set; }


        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        /// <value>The transaction id.</value>
        [XmlAttribute]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the length of the header.
        /// </summary>
        /// <value>The length of the header.</value>
        [XmlAttribute]
        public byte HeaderLength { get; set; }


        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <value>The type of the content.</value>
        //public string ContentType { get; set; }

        public override string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "PhoneNumber = ", PhoneNumber, "\r\n");
            str = String.Concat(str, "ReceivedDate = ", ReceivedDate, "\r\n");
            str = String.Concat(str, "Timezone = ", Timezone, "\r\n");
            str = String.Concat(str, "Content = ", Content, "\r\n");
            str = String.Concat(str, "MessageType = ", MessageType, "\r\n");
            str = String.Concat(str, "TotalPiece = ", TotalPiece, "\r\n");
            str = String.Concat(str, "CurrentPiece = ", CurrentPiece, "\r\n");
            str = String.Concat(str, "DeliveryStatus = ", DeliveryStatus, "\r\n");
            str = String.Concat(str, "DestinationReceivedDate = ", DestinationReceivedDate, "\r\n");
            str = String.Concat(str, "ValidityTimestamp = ", ValidityTimestamp, "\r\n");
            str = String.Concat(str, "Index = ", Index, "\r\n");
            str = String.Concat(str, "MessageStatusType = ", MessageStatusType, "\r\n");
            str = String.Concat(str, "ReferenceNo = ", ReferenceNo, "\r\n");
            str = String.Concat(str, "SourcePort = ", SourcePort, "\r\n");
            str = String.Concat(str, "DestinationPort = ", DestinationPort, "\r\n");
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            str = String.Concat(str, "TotalPieceReceived = ", TotalPieceReceived, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            str = String.Concat(str, "RawMessage = ", RawMessage, "\r\n");
            str = String.Concat(str, "Indexes = ", Indexes, "\r\n");
            str = String.Concat(str, "DataBytes = ", DataBytes, "\r\n");
            str = String.Concat(str, "ServiceCentreAddress = ", ServiceCentreAddress, "\r\n");
            str = String.Concat(str, "ServiceCentreAddressType = ", ServiceCentreAddressType, "\r\n");
            str = String.Concat(str, "MessageType = ", MessageType, "\r\n");
            str = String.Concat(str, "Version = ", Version, "\r\n");
            str = String.Concat(str, "TransactionId = ", TransactionId, "\r\n");
            str = String.Concat(str, "HeaderLength = ", HeaderLength, "\r\n");
            return str;
        }

    }
}
