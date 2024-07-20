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
    /// Base class for messages.
    /// </summary>
    [DataContract]
    public abstract class BaseMessage
    {
        private string timeStampString;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessage" /> class.
        /// </summary>
        public BaseMessage()
        {
            this.Id = string.Empty;
            this.Message = string.Empty;
            this.PhoneNumber = string.Empty;
            this.TimeStamp = DateTime.MinValue;
            this.TimeStampString = string.Empty;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [DataMember(Name = "date")]
        [XmlAttribute]
        public string TimeStampString
        {

            get
            {
                return timeStampString;
            }
            set
            {
                this.timeStampString = value;

                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        this.TimeStamp = DateTime.ParseExact(value, ServiceInterfaceDefinition.DateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        // Ignore any error
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [XmlAttribute]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember(Name = "id")]
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember(Name = "message")]
        [XmlAttribute]
        public string Message { get; set; }

    

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        [DataMember(Name = "number")]
        [XmlAttribute]
        public string PhoneNumber { get; set; }

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
            return str;
        }
    }
}
