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
    /// Device message.
    /// </summary>
    [DataContract]
    public sealed class Message : BaseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
            : base()
        {
            this.To = string.Empty;
        }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        [DataMember(Name = "to")]
        public string To { get; set; }

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
            str = String.Concat(str, "To = ", To, "\r\n");
            return str;
        }
    }
}
