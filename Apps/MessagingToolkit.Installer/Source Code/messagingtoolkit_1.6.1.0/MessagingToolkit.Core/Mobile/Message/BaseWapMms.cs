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
    public abstract class BaseWapMms: MessageInformation
    {
        /// <summary>
        /// Message type. 
        /// </summary>
        /// <value>Message type</value>
        [XmlAttribute]
        public MmsConstants.WapMmsType MessageType { get; set; }

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
        /// <value>The type of the content.</value>
        //public string ContentType { get; set; }

    }
}
