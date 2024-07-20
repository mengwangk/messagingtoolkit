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

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core
{
    /// <summary>
    /// Gateway exception.
    /// E.g. Gateway is not found, connection problem, etc.
    /// </summary>
    public class GatewayException: BaseException<GatewayException>
    {        
        /// <summary>
        /// Initializes a new object of GatewayException class.
        /// </summary>
        public GatewayException() {         
        }

        /// <summary>
        /// Initializes a new object of GatewayException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GatewayException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new object of GatewayException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public GatewayException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new object of GatewayException class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.</param>
        protected GatewayException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
