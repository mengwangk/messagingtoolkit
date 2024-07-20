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
using System.Collections;
using System.Text;
using System.Collections.Generic;


namespace MessagingToolkit.MMS
{

    /// <summary>
    /// The MultimediaMessage class represents a Multimedia Message.
    /// </summary>
#if !NETFX_CORE
    [Serializable]
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = false)]
#else
    [System.Runtime.Serialization.DataContract]
#endif
    public class MultimediaMessage : MultimediaMessageConstants
    {


        /// <summary>
        /// Multipart related indication flag
        /// </summary>
        protected bool flagMultipartRelated = false;

        /// <summary>
        /// BCC available indication flag
        /// </summary>
        protected bool flagBccAvailable = false;

        /// <summary>
        /// CC available indication flag
        /// </summary>
        protected bool flagCcAvailable = false;

        /// <summary>
        /// Content type available indication flag
        /// </summary>
        protected bool flagContentTypeAvailable = false;

        /// <summary>
        /// Date available indication flag
        /// </summary>
        protected bool flagDateAvailable = false;

        /// <summary>
        /// Delivery report available indication flag
        /// </summary>
        protected bool flagDeliveryReportAvailable = false;

        /// <summary>
        /// Delivery time available indication flag
        /// </summary>
        protected bool flagDeliveryTimeAvailable = false;

        /// <summary>
        /// Absolute delivery time indication flag
        /// </summary>
        protected bool flagDeliveryTimeAbsolute = false;

        /// <summary>
        /// Expiry available indication flag
        /// </summary>
        protected bool flagExpiryAvailable = false;

        /// <summary>
        /// Expiry indication flag
        /// </summary>
        protected bool flagExpiryAbsolute = false;

        /// <summary>
        /// From indication flag
        /// </summary>
        protected bool flagFromAvailable = false;

        /// <summary>
        /// Message class indication flag
        /// </summary>
        protected bool flagMessageClassAvailable = false;

        /// <summary>
        /// Message id indication flag
        /// </summary>
        protected bool flagMessageIdAvailable = false;

        /// <summary>
        /// Message type indication flag
        /// </summary>
        protected bool flagMessageTypeAvailable = false;

        /// <summary>
        /// MMS version indication flag
        /// </summary>
        protected bool flagMMSVersionAvailable = false;
        /// <summary>
        /// Priority indication flag
        /// </summary>
        protected bool flagPriorityAvailable = false;
        /// <summary>
        /// Read reply indication flag
        /// </summary>
        protected bool flagReadReplyAvailable = false;
        /// <summary>
        /// Sender visibility indication flag
        /// </summary>
        protected bool flagSenderVisibilityAvailable = false;
        /// <summary>
        /// Status indication flag
        /// </summary>
        protected bool flagStatusAvailable = false;
        /// <summary>
        /// Subject indication flag
        /// </summary>
        protected bool flagSubjectAvailable = false;
        /// <summary>
        /// To indication flag
        /// </summary>
        protected bool flagToAvailable = false;
        /// <summary>
        /// 
        /// </summary>
        protected bool flagTransactionIdAvailable = false;


        /* Header Fields */

        /// <summary>
        /// Message type header
        /// </summary>
        protected byte hMessageType;
        /// <summary>
        /// Transaction id header
        /// </summary>
        protected string hTransactionId = "";
        /// <summary>
        /// Message id header
        /// </summary>
        protected string hMessageId = "";
        /// <summary>
        /// Version header
        /// </summary>
        protected byte hVersion = 0;
        /// <summary>
        /// To address header
        /// </summary>
        protected List<MultimediaMessageAddress> hTo = null;
        /// <summary>
        /// CC address header
        /// </summary>
        protected List<MultimediaMessageAddress> hCc = null;
        /// <summary>
        /// Bcc address header
        /// </summary>
        protected List<MultimediaMessageAddress> hBcc = null;
        /// <summary>
        /// Received date header
        /// </summary>
        protected DateTime hReceivedDate;
        /// <summary>
        /// From address 
        /// </summary>
        protected MultimediaMessageAddress hFrom = null;
        /// <summary>
        /// Subject header
        /// </summary>
        protected string hSubject = "";
        /// <summary>
        /// Message class header
        /// </summary>
        protected byte hMessageClass = 0;
        /// <summary>
        /// Expiry header
        /// </summary>
        protected DateTime hExpiry;
        /// <summary>
        /// Delivery report header
        /// </summary>
        protected bool hDeliveryReport = false;
        /// <summary>
        /// Read reply header
        /// </summary>
        protected bool hReadReply = false;
        /// <summary>
        /// Sender visibility header
        /// </summary>
        protected byte hSenderVisibility = 0;
        /// <summary>
        /// Delivery time header
        /// </summary>
        protected DateTime hDeliveryTime;
        /// <summary>
        /// Priority header
        /// </summary>
        protected byte hPriority;
        /// <summary>
        /// Content type header
        /// </summary>
        protected string hContentType = "";
        /// <summary>
        /// Status header
        /// </summary>
        protected byte hStatus;

        /// <summary>
        /// Multipart related type
        /// </summary>
        protected string multipartRelatedType = "";

        /// <summary>
        /// Presentation id
        /// </summary>
        protected string presentationId = "";

        /// <summary>
        /// Message content
        /// </summary>

        //protected SortedList tableOfContents = null;
        protected SortedDictionary<string, MultimediaMessageContent> tableOfContents = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MultimediaMessage()
        {
            //tableOfContents = SortedList.Synchronized(new SortedList());
            tableOfContents = new SortedDictionary<string, MultimediaMessageContent>();

            hTo = new List<MultimediaMessageAddress>(10);
            hCc = new List<MultimediaMessageAddress>(10);
            hBcc = new List<MultimediaMessageAddress>(10);
        }

        /// <summary> 
        /// Check if the presentation part is available.
        /// return true if availale.
        /// </summary>
        virtual public bool PresentationAvailable
        {
            get
            {
                return flagMultipartRelated;
            }

        }
        /// <summary> 
        /// Check if the message type field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool MessageTypeAvailable
        {
            get
            {
                return flagMessageTypeAvailable;
            }

        }
        /// <summary> 
        /// Check if the Delivery-Report field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool DeliveryReportAvailable
        {
            get
            {
                return flagDeliveryReportAvailable;
            }

        }
        /// <summary> 
        /// Check if the Sender-Visibility field is available. 
        /// return true if availale.
        /// </summary>
        virtual public bool SenderVisibilityAvailable
        {
            get
            {
                return flagSenderVisibilityAvailable;
            }

        }
        /// <summary> 
        /// Check if the Read-Reply field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool ReadReplyAvailable
        {
            get
            {
                return flagReadReplyAvailable;
            }

        }
        /// <summary> 
        /// Check if the Status field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool StatusAvailable
        {
            get
            {
                return flagStatusAvailable;
            }

        }
        /// <summary> 
        /// Check if the transaction ID field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool TransactionIdAvailable
        {
            get
            {
                return flagTransactionIdAvailable;
            }

        }
        /// <summary> 
        /// Check if the message ID field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool MessageIdAvailable
        {
            get
            {
                return flagMessageIdAvailable;
            }

        }
        /// <summary> 
        /// Check if the version field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool VersionAvailable
        {
            get
            {
                return flagMMSVersionAvailable;
            }

        }
        /// <summary> 
        /// Check if the date field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool DateAvailable
        {
            get
            {
                return flagDateAvailable;
            }

        }
        /// <summary> 
        /// Check if sender address field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool FromAvailable
        {
            get
            {
                return flagFromAvailable;
            }

        }
        /// <summary> 
        /// Check if the subject field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool SubjectAvailable
        {
            get
            {
                return flagSubjectAvailable;
            }

        }
        /// <summary> 
        /// Check if the message class field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool MessageClassAvailable
        {
            get
            {
                return flagMessageClassAvailable;
            }

        }
        /// <summary> 
        /// Check if the expiry date/time field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool ExpiryAvailable
        {
            get
            {
                return flagExpiryAvailable;
            }

        }
        /// <summary> 
        /// Check if the delivery date/time field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool DeliveryTimeAvailable
        {
            get
            {
                return flagDeliveryTimeAvailable;
            }

        }
        /// <summary> 
        /// Check if the priority date/time field is available.
        /// return true if availale.
        /// </summary>
        virtual public bool PriorityAvailable
        {
            get
            {
                return flagPriorityAvailable;
            }

        }
        /// <summary> 
        /// Check if the content type field is available. 
        /// return true if availale.
        /// </summary>
        virtual public bool ContentTypeAvailable
        {
            get
            {
                return flagContentTypeAvailable;
            }

        }
        /// <summary> 
        /// Check if there is at least one receiver is specified.
        /// return true if at least one receiver is specified.
        /// </summary>
        virtual public bool ToAvailable
        {
            get
            {
                return flagToAvailable;
            }

        }
        /// <summary>
        /// Check if there is at least one BCC receiver is specified.
        /// return true if at least one BCC receiver is specified.
        /// </summary>
        /// <value><c>true</c> if [BCC available]; otherwise, <c>false</c>.</value>
        virtual public bool BccAvailable
        {
            get
            {
                return flagBccAvailable;
            }

        }
        /// <summary>
        /// Check if there is at least one CC receiver is specified.
        /// return true if at least one CC receiver is specified.
        /// </summary>
        /// <value><c>true</c> if [cc available]; otherwise, <c>false</c>.</value>
        virtual public bool CcAvailable
        {
            get
            {
                return flagCcAvailable;
            }

        }
        /// <summary>
        /// Returns or sets the type of the message (Mandatory).
        /// Supported values are
        /// - MultimediaMessageConstants.MESSAGE_TYPE_M_SEND_REQ.
        /// - MultimediaMessageConstants.MESSAGE_TYPE_M_DELIVERY_IND.
        /// </summary>
        /// <value>The type of the message.</value>
        virtual public byte MessageType
        {
            get
            {
                return hMessageType;
            }

            set
            {
                hMessageType = value;
                flagMessageTypeAvailable = true;
            }
        }

        /// <summary>
        /// Retrieves the Message ID (Mandatory).
        /// Identifier of the message. From Send request, 
        /// connects delivery report to sent message in MS.
        /// </summary>
        /// <value>The message id.</value>
        virtual public string MessageId
        {
            get
            {
                return hMessageId;
            }
            set
            {
                hMessageId = value;
                flagMessageIdAvailable = true;
            }

        }

        /// <summary> 
        /// Retrieves the Message Status (Mandatory).
        /// The status of the message.
        /// </summary>
        virtual public byte MessageStatus
        {
            get
            {
                return hStatus;
            }

            set
            {
                hStatus = value;
                flagStatusAvailable = true;
            }

        }

        /// <summary> 
        /// Retrieves and Sets the transaction ID (Mandatory).
        /// It is a unique identifier for the message and it identifies the M-Send.req
        /// and the corresponding reply only.
        /// </summary>
        virtual public string TransactionId
        {
            get
            {
                return hTransactionId;
            }

            set
            {
                hTransactionId = value;
                flagTransactionIdAvailable = true;
            }

        }
        /// <summary>
        /// Retrieves the MMS version number as a String(Mandatory).
        /// return the version as a String. The only supported value are "1.0" and "1.1".
        /// </summary>
        /// <value>The version as string.</value>
        virtual public string VersionAsString
        {
            get
            {
                int ver_major = ((byte)(hVersion << 1)) >> 5;
                int ver_minor = ((byte)(hVersion << 4)) >> 4;
                string result = ver_major + "." + ver_minor;

                return result;
            }

        }

        /// <summary>
        /// Retrieves and sets the MMS version number (Mandatory).
        /// The only supported value are
        /// MultimediaMessageConstants.MMS_VERSION_10
        /// </summary>
        /// <value>The version.</value>
        virtual public byte Version
        {
            get
            {
                return hVersion;
            }

            set
            {
                hVersion = value;
                flagMMSVersionAvailable = true;
            }

        }

        /// <summary>
        /// Retrieve all the receivers of the Multimedia Message.
        /// </summary>
        /// <value>Receiver list</value>
        virtual public List<MultimediaMessageAddress> To
        {
            get
            {
                return hTo;
            }

        }

        /// <summary>
        /// Retrieve all the CC receivers of the Multimedia Message.
        /// </summary>
        /// <value>CC list</value>
        virtual public List<MultimediaMessageAddress> Cc
        {
            get
            {
                return hCc;
            }

        }

        /// <summary>
        /// Retrieve all the BCC receivers of the Multimedia Message.
        /// </summary>
        /// <value>BCC list</value>
        virtual public List<MultimediaMessageAddress> Bcc
        {
            get
            {
                return hBcc;
            }

        }

        /// <summary>
        /// Retrieves the arrival time of the message at the MMS Proxy-Relay (Optional).
        /// MMS Proxy-Relay will generate this field when not supplied by terminal.
        /// </summary>
        /// <value>Message arrival time.</value>
        virtual public DateTime Date
        {
            get
            {
                return hReceivedDate;
            }

            set
            {
                hReceivedDate = value;
                flagDateAvailable = true;
            }

        }

        /// <summary>
        ///  Retrieves the subject of the Multimedia Message (Optional).
        /// </summary>
        virtual public string Subject
        {
            get
            {
                return hSubject;
            }

            set
            {
                hSubject = new StringBuilder(value).ToString();
                flagSubjectAvailable = true;
            }

        }

        /// <summary> 
        /// Retrieves and sets the message class of the Multimedia Message (Optional).
        /// Supported values are
        /// MESSAGE_CLASS_PERSONAL, MESSAGE_CLASS_ADVERTISEMENT,
        /// MESSAGE_CLASS_INFORMATIONAL, MESSAGE_CLASS_AUTO
        /// </summary>
        virtual public byte MessageClass
        {
            get
            {
                return hMessageClass;
            }

            set
            {
                hMessageClass = value;
                flagMessageClassAvailable = true;
            }

        }

        /// <summary>
        /// Retrieves and sets the content type of the Multimedia Message (Mandatory).
        /// Supported values are CONTENT_TYPE_APPLICATION_MULTIPART_MIXED,
        /// CONTENT_TYPE_APPLICATION_MULTIPART_RELATED
        /// </summary>
        /// <value>The type of the content.</value>
        virtual public string ContentType
        {
            get
            {
                return hContentType;
            }

            set
            {
                hContentType = new StringBuilder(value).ToString();
                flagContentTypeAvailable = true;
            }

        }
        /// <summary>
        /// Retrieves the number of contents of of the Multimedia Message (Mandatory).
        /// </summary>
        virtual public int NumContents
        {
            get
            {
                return tableOfContents.Count;
            }

        }

        /// <summary> 
        /// Retrieves the content ID referring to the presentation part.
        /// Sets the content ID of the content containing the presentation part of the
        /// Multimedia Message (Mandatory when the content type of the Multimedia Message is 
        /// CONTENT_TYPE_APPLICATION_MULTIPART_RELATED).
        /// </summary>
        virtual public string PresentationId
        {
            get
            {
                if ((flagMultipartRelated == true) && (NumContents > 0))
                    return presentationId;
                else
                    return null;
            }

            set
            {
                presentationId = value;
            }

        }
        /// <summary>
        /// Retrieves the type of the presentation part.
        /// Sets the type of the presentation part.(Mandatory when the content type of 
        /// the Multimedia Message is CONTENT_TYPE_APPLICATION_MULTIPART_RELATED).
        /// </summary>
        /// <value>
        /// The type of the multipart related. he standard for interoperability supports
        /// only the  value: CONTENT_TYPE_APPLICATION_SMIL
        /// </value>
        virtual public string MultipartRelatedType
        {
            get
            {
                if (flagMultipartRelated)
                {
                    return multipartRelatedType;
                }
                else
                    return null;
            }

            set
            {
                flagMultipartRelated = true;
                multipartRelatedType = value;
            }

        }
        /// <summary>
        /// Retrieves the content referring to the presentation part.
        /// </summary>
        /// <value>The presentation content</value>
        virtual public MultimediaMessageContent Presentation
        {
            get
            {
                // Fixed Marc 06th 2016
                if ((flagMultipartRelated == true) && (NumContents > 0) && (tableOfContents.ContainsKey(presentationId)))
                    return (MultimediaMessageContent)tableOfContents[presentationId];
                else
                    return null;
            }

        }

        /// <summary>
        /// Retrieves the expiry date of the message (Optional).
        /// Sets the length of time the message will be stored in the MMS Proxy-Relay
        /// or time to delete the message (Optional). This field can have two format,
        /// either absolute or interval depending on how it is set with the method
        /// setExpiryAbsolute().
        /// </summary>
        /// <value>Message expiry date</value>
        virtual public DateTime Expiry
        {
            get
            {
                return hExpiry;
            }

            set
            {
                hExpiry = value;
                flagExpiryAvailable = true;
            }

        }
        /// <summary>
        /// Returns information about the format of the expiry date/time.
        /// Return true if the expiry date/time is absolute,
        /// false if it is intended as an interval.
        /// Sets the format of the expiry date/time. If true the expiry date is absolute, else is
        /// intended as an interval.
        /// </summary>
        /// <value><c>true</c> if [expiry absolute]; otherwise, <c>false</c>.</value>
        virtual public bool ExpiryAbsolute
        {
            get
            {
                return flagExpiryAbsolute;
            }

            set
            {
                flagExpiryAbsolute = value;
            }

        }
        /// <summary>
        /// Retrieves the delivery-report flag (Optional).
        /// Return  true if the user wants the delivery report.
        /// Specify whether the user wants a delivery report from each recipient (Optional).
        /// true if the user wants the delivery report.
        /// </summary>
        /// <value><c>true</c> if [delivery report]; otherwise, <c>false</c>.</value>
        virtual public bool DeliveryReport
        {
            get
            {
                return hDeliveryReport;
            }

            set
            {
                hDeliveryReport = value;
                flagDeliveryReportAvailable = true;
            }

        }

        /// <summary>
        /// Retrieves the sender-visibility flag (Optional).
        /// Return  0x80 if the user wants the sender visibility setting to Hide.
        /// 0x81 if the user wants the sender visibility setting to Show.
        /// Specify whether the user wants sender visibility (Optional).
        /// return  0x80 if the user wants the sender visibility setting to Hide.
        /// 0x81 if the user wants the sender visibility setting to Show.
        /// </summary>
        /// <value>The sender visibility.</value>
        virtual public byte SenderVisibility
        {
            get
            {
                return hSenderVisibility;
            }

            set
            {
                hSenderVisibility = value;
                flagSenderVisibilityAvailable = true;
            }

        }

        /// <summary>
        /// Retrieves the read reply flag (Optional).
        /// return  true if the user wants the read reply.
        /// </summary>
        /// <value><c>true</c> if [read reply]; otherwise, <c>false</c>.</value>
        virtual public bool ReadReply
        {
            get
            {
                return hReadReply;
            }

            set
            {
                hReadReply = value;
                flagReadReplyAvailable = true;
            }

        }

        /// <summary>
        /// Retrieves the delivery date/time of the message (Optional).
        /// Sets the date/time of the desired delivery(Optional).
        /// Indicates the earliest possible delivery of the message to the recipient.
        /// This field can have two format,
        /// either absolute or interval depending on how it is set with the method
        /// setDeliveryTimeAbsolute().
        /// </summary>
        /// <value>The delivery time.</value>
        virtual public DateTime DeliveryTime
        {
            get
            {
                return hDeliveryTime;
            }

            set
            {
                hDeliveryTime = value;
                flagDeliveryTimeAvailable = true;
            }

        }

        /// <summary>
        /// Returns information about the format of the delivery date/time.
        /// true if the delivery date/time is absolute,
        /// false if it is intended as an interval.
        /// Sets the format of the delivery date/time.
        /// true the delivery date/time is absolute, else is
        /// intended as an interval.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [delivery time absolute]; otherwise, <c>false</c>.
        /// </value>
        virtual public bool DeliveryTimeAbsolute
        {
            get
            {
                return flagDeliveryTimeAbsolute;
            }

            set
            {
                flagDeliveryTimeAbsolute = value;
            }

        }

        /// <summary>
        /// Retrieves or sets the priority of the message for the recipient (Optional).
        /// It can be one of the following the values:
        /// PRIORITY_LOW, PRIORITY_NORMAL, PRIORITY_HIGH
        /// </summary>
        /// <value>The priority.</value>
        virtual public byte Priority
        {
            get
            {
                return hPriority;
            }

            set
            {
                hPriority = value;
                flagPriorityAvailable = true;
            }

        }

        /// <summary>
        /// Decodes the address.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected MultimediaMessageAddress DecodeAddress(string value)
        {
            byte type = MultimediaMessageConstants.AddressTypeUnknown;
            string address = new StringBuilder(value).ToString();

            int sep = value.IndexOf((Char)47); // the character "/"
            if (sep != -1)
            {
                address = value.Substring(0, (sep) - (0));
                if (value.EndsWith("PLMN", StringComparison.OrdinalIgnoreCase))
                    type = MultimediaMessageConstants.AddressTypePlmn;
                else if (value.EndsWith("IPv4", StringComparison.OrdinalIgnoreCase))
                    type = MultimediaMessageConstants.AddressTypeIpv4;
                else if (value.EndsWith("IPv6", StringComparison.OrdinalIgnoreCase))
                    type = MultimediaMessageConstants.AddressTypeIpv6;
            }
            else
            {
                int at = address.IndexOf((Char)64); // the character "@"
                if (at != -1)
                    type = MultimediaMessageConstants.AddressTypeEmail;
            }
            MultimediaMessageAddress result = new MultimediaMessageAddress(address, type);
            return result;
        }

        /// <summary>
        /// Adds a new receiver of the Multimedia Message. The message can have
        /// more than one receiver but at least one.
        /// param value is the string representing the address of the receiver. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void AddToAddress(string value)
        {
            MultimediaMessageAddress addr = DecodeAddress(value);
            hTo.Add(addr);
            flagToAvailable = true;
        }

        /// <summary>
        /// Adds a new receiver in the CC (Carbon Copy) field of the Multimedia Message. The message can have
        /// more than one receiver in the CC field.
        /// param value is the string representing the address of the CC receiver. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void AddCcAddress(string value)
        {
            MultimediaMessageAddress addr = DecodeAddress(value);
            hCc.Add(addr);
            flagCcAvailable = true;
        }

        /// <summary>
        /// Adds a new receiver in the BCC (Blind Carbon Copy) field of the Multimedia Message. The message can have
        /// more than one receiver in the BCC field.
        /// </summary>
        /// <param name="value">value is the string representing the address of the BCC receiver. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4.</param>
        public virtual void AddBccAddress(string value)
        {
            MultimediaMessageAddress addr = DecodeAddress(value);
            hBcc.Add(addr);
            flagBccAvailable = true;
        }

        /// <summary>
        /// Clears all the receivers of the Multimedia Message.
        /// </summary>
        public virtual void ClearTo()
        {
            hTo.Clear();
            flagToAvailable = false;
        }

        /// <summary>
        /// Clears all the carbon copy receivers of the Multimedia Message.
        /// </summary>
        public virtual void ClearCc()
        {
            hCc.Clear();
            flagCcAvailable = false;
        }

        /// <summary>
        /// Clears all the blind carbon copy receivers of the Multimedia Message.
        /// </summary>
        public virtual void ClearBcc()
        {
            hBcc.Clear();
            flagBccAvailable = false;
        }

        /// <summary>
        /// Retrieves the address of the message sender (Mandatory).
        /// </summary>
        /// <returns></returns>
        public virtual MultimediaMessageAddress GetFrom()
        {
            return hFrom;
        }


        /// <summary>
        /// Sets the address of the message sender (Mandatory).
        /// </summary>
        /// <param name="value">value is the string representing the address of the sender. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4..</param>
        public virtual void SetFrom(string value)
        {
            hFrom = new MultimediaMessageAddress(DecodeAddress(value));
            flagFromAvailable = true;
        }

        /// <summary>
        /// Adds a content to the message.
        /// </summary>
        /// <param name="mmc">Message content</param>
        public virtual void AddContent(MultimediaMessageContent mmc)
        {
            tableOfContents[mmc.ContentId] = mmc;
        }

        /// <summary>
        /// Retrieves the content having the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Multimedia message content</returns>
        public virtual MultimediaMessageContent GetContent(string id)
        {
            return (MultimediaMessageContent)tableOfContents[id];
        }

        /// <summary>
        /// Retrieves the content at the position index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Multimedia message content</returns>
        public virtual MultimediaMessageContent GetContent(int index)
        {
            MultimediaMessageContent content = null;
            int j = 0;
            for (IEnumerator e = tableOfContents.Values.GetEnumerator(); (e.MoveNext() && j <= index); j++)
            {
                content = (MultimediaMessageContent)e.Current;
            }
            return content;
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy;
            System.Reflection.PropertyInfo[] infos = this.GetType().GetProperties(flags);

            StringBuilder sb = new StringBuilder();

            string typeName = this.GetType().Name;
            sb.AppendLine(typeName);       
            sb.AppendLine(string.Empty.PadRight(typeName.Length + 5, '='));

            foreach (var info in infos)
            {
                if (!info.PropertyType.Name.StartsWith("List"))
                {
                    object value = info.GetValue(this, null);
                    sb.AppendFormat("{0}: {1}{2}", info.Name, value != null ? value : string.Empty, Environment.NewLine);
                }
                else
                {
                    List<MultimediaMessageAddress> values = (List<MultimediaMessageAddress>)info.GetValue(this, null);
                    sb.AppendFormat("{0}: {1}", info.Name, Environment.NewLine);
                    foreach (var value in values)
                    {
                        sb.Append(value + Environment.NewLine);
                    }
                }
            }

            return sb.ToString();
        }
    }
}