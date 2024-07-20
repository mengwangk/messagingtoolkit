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
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Message status information.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class MessageStatusInformation: BaseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStatusInformation" /> class.
        /// </summary>
        public MessageStatusInformation()
            : base()
        {
            this.To = string.Empty;
            this.AnswerTo = string.Empty;
        }

        /// <summary>
        /// Gets or sets the answer to.
        /// </summary>
        /// <value>
        /// The answer to.
        /// </value>
        [DataMember(Name = "answerTo")]
        [XmlAttribute]
        public string AnswerTo { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        [DataMember(Name = "to")]
        [XmlAttribute]
        public string To { get; set; }


        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        [DataMember(Name = "read")]
        [XmlAttribute]
        public bool IsRead { get; set; }


        /// <summary>
        /// Messaging sending status, which can be "Sent", "Delivered", "Queued" or "Failed".
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [XmlAttribute]
        public string Status { get; set; }


        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        [XmlAttribute]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "TimeStampString = ", TimeStampString, "\r\n");
            str = String.Concat(str, "TimeStamp = ", TimeStamp, "\r\n");
            str = String.Concat(str, "Id = ", Id, "\r\n");
            str = String.Concat(str, "Message = ", Message, "\r\n");
            str = String.Concat(str, "PhoneNumber = ", PhoneNumber, "\r\n");
            str = String.Concat(str, "AnswerTo = ", AnswerTo, "\r\n");
            str = String.Concat(str, "To = ", To, "\r\n");
            str = String.Concat(str, "IsRead = ", IsRead, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            str = String.Concat(str, "ErrorDescription = ", ErrorDescription, "\r\n");
            return str;
        }
    }
}
