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
    /// Message information
    /// </summary>
    [Serializable]
    public class MessageInformation
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageInformation"/> class.
        /// </summary>
        public MessageInformation()
        {
            TotalPiece = 0;
            CurrentPiece = 0;
            ReferenceNo = 0;
            TotalPieceReceived = 1;
            Indexes = new List<int>(1);
            Status = 0;
            DataBytes = new List<byte>();
        }

        /// <summary>
        /// Phone number
        /// </summary>
        /// <value>Phone number</value>
        [XmlAttribute]
        public string PhoneNumber
        {
            get;
            set;
        }     


        /// <summary>
        /// Received date
        /// </summary>
        /// <value>Received date</value>
        [XmlAttribute]
        public DateTime ReceivedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Timezone
        /// </summary>
        /// <value>Timezone</value>
        [XmlAttribute]
        public string Timezone
        {
            get;
            set;
        }

        /// <summary>
        /// Message text
        /// </summary>
        /// <value>Message text</value>
        [XmlAttribute]
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Message type. See <see cref="MessageTypeIndicator"/>
        /// </summary>
        /// <value>Message type</value>
        [XmlAttribute]
        public MessageTypeIndicator MessageType
        {
            get;
            set;
        }


        /// <summary>
        /// Total number of messages for multipart message
        /// </summary>
        /// <value>Total messages</value>
        [XmlAttribute]
        public int TotalPiece
        {
            get;
            set;
        }

        /// <summary>
        /// Current message
        /// </summary>
        /// <value>Current message</value>
        [XmlAttribute]
        public int CurrentPiece
        {
            get;
            set;
        }

        /// <summary>
        /// Message status report status. See <see cref="MessageStatusReportStatus"/>
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public MessageStatusReportStatus DeliveryStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Destination received date
        /// </summary>
        /// <value>Received date</value>
        [XmlAttribute]
        public DateTime DestinationReceivedDate
        {
            get;
            set;
        }

        /// <summary>
        /// Validity timestamp
        /// </summary>
        /// <value>Validity timestamp</value>
        [XmlAttribute]
        public DateTime ValidityTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Message index in the gateway
        /// </summary>
        /// <value>Message index</value>
        [XmlAttribute]
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Message type. See <see cref="MessageType"/>
        /// </summary>
        /// <value>Message type</value>  
        [XmlAttribute]
        public MessageStatusType MessageStatusType
        {
            get;
            set;
        }

        /// <summary>
        /// Message reference number
        /// </summary>
        /// <value>Message reference number</value>
        [XmlAttribute]
        public int ReferenceNo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source port.
        /// </summary>
        /// <value>The source port.</value>
        [XmlAttribute]
        public int SourcePort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the destination port.
        /// </summary>
        /// <value>The destination port.</value>
        [XmlAttribute]
        public int DestinationPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gateway id.
        /// </summary>
        /// <value>The gateway id.</value>
        [XmlAttribute]
        public string GatewayId
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the total piece received.
        /// </summary>
        /// <value>The total piece received.</value>
        [XmlAttribute]
        public int TotalPieceReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [XmlAttribute]
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// The incoming message in PDU format
        /// </summary>
        /// <value>Undecoded message in PDU format</value>
        [XmlAttribute]
        public string RawMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        [XmlAttribute]
        public List<int> Indexes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user data bytes.
        /// </summary>
        /// <value>The user data bytes.</value>
        [XmlAttribute]
        public List<byte> DataBytes { get; set; }

        
        /// <summary>
        /// Gets or sets the service centre address.
        /// </summary>
        /// <value>
        /// The service centre address.
        /// </value>
        [XmlAttribute]
        public string ServiceCentreAddress { get; set; }
        

        /// <summary>
        /// Gets or sets the type of the service centre address.
        /// </summary>
        /// <value>
        /// The type of the service centre address.
        /// </value>
        [XmlAttribute]
        public NumberType ServiceCentreAddressType { get; set; }



        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
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
            return str;
        }
    }
}
