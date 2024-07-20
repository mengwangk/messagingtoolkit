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

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Received message.
    /// </summary>
    [global::System.Serializable]
    [DataContract]
    public sealed class DeviceMessage : BaseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceMessage"/> class.
        /// </summary>
        public DeviceMessage()
            : base()
        {
            this.IsRead = false;
            this.MessageType = string.Empty;
            this.Receiver = string.Empty;
            this.Sender = string.Empty;
            this.ThreadId = -1;
        }


        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        [DataMember(Name = "messageType")]
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <value>
        /// The receiver.
        /// </value>
        [DataMember(Name = "receiver")]
        public string Receiver { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        [DataMember(Name = "sender")]
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the thread identifier.
        /// </summary>
        /// <value>
        /// The thread identifier.
        /// </value>
        [DataMember(Name = "threadID")]
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "read")]
        public Boolean IsRead { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "TimeStampString = ", TimeStampString, "\r\n");
            str = String.Concat(str, "TimeStamp = ", TimeStamp, "\r\n");
            str = String.Concat(str, "Id = ", Id, "\r\n");
            str = String.Concat(str, "Message = ", Message, "\r\n");
            str = String.Concat(str, "PhoneNumber = ", PhoneNumber, "\r\n");
            str = String.Concat(str, "MessageType = ", MessageType, "\r\n");
            str = String.Concat(str, "Receiver = ", Receiver, "\r\n");
            str = String.Concat(str, "Sender = ", Sender, "\r\n");
            str = String.Concat(str, "ThreadId = ", ThreadId, "\r\n");
            str = String.Concat(str, "IsRead = ", IsRead, "\r\n");
            return str;
        }
    }
}
