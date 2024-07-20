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

namespace MessagingToolkit.Core.Mobile.Event
{
    /// <summary>
    /// Message sending 
    /// </summary>
    public class MessageErrorEventArgs: EventArgs
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message instance</param>
        /// <param name="ex">Exception</param>
        public MessageErrorEventArgs(IMessage message, Exception ex)
        {
            this.Message = message;
            this.Error = ex;
        }

        /// <summary>
        /// Message property. You can check the type of the message and then
        /// cast it to the actual type
        /// </summary>
        /// <value></value>
        public IMessage Message
        {
            get;
            internal set;
        }

        /// <summary>
        /// Exception
        /// </summary>
        /// <value>exception</value>
        public Exception Error
        {
            get;
            internal set;
        }

    }
}

