using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.Core.Http.Model
{
    /// <summary>
    /// Base class for all REST request message.
    /// </summary>
    [DataContract]
    internal abstract class RequestBase
    {
        /// <summary>
        /// Gets or sets the request method.
        /// </summary>
        /// <value>
        /// The HTTP request method - GET, POST, DELETE, PUT.
        /// </value>
        [DataMember(Name = "requestMethod")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// Gets or sets the description. Normally contains the
        /// error message if <seealso cref="IsSuccessful"/> is false.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request operation is successful.
        /// </summary>
        /// <value>
        /// <c>true</c> if the request is successful; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "isSuccessful")]
        public bool IsSuccessful { get; set; }
    }
}
