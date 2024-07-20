﻿//===============================================================================
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

namespace MessagingToolkit.Core.Http.Model
{
    /// <summary>
    /// GET message request returning a list of messages.
    /// </summary>
    [DataContract]
    internal sealed class GetMessageResponse: RequestBase
    {
        /// <summary>
        /// Gets or sets the list of messages
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [DataMember(Name = "messages")]
        public List<Message> Messages { get; set; }

    }
}