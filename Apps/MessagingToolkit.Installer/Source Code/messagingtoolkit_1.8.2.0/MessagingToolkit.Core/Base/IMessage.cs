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

using MessagingToolkit.Core;

namespace MessagingToolkit.Core.Base
{   
    /// <summary>
    /// Message interface
    /// </summary>
    public interface IMessage
    {
        
        /// <summary>
        /// Gateway id
        /// </summary>
        string GatewayId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the queue priority.
        /// </summary>
        /// <value>The queue priority.</value>
        MessageQueuePriority QueuePriority
        {
            get;
            set;
        }

        /// <summary>
        /// This is a optional property for you to set so that you can
        /// uniquely identify the message especially message is sent
        /// asynchronously.
        /// If you are sending message to queue, this value will be set 
        /// automatically for you if it is not set.
        /// </summary>
        /// <value></value>
        string Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scheduled delivery date.
        /// </summary>
        /// <value>The scheduled delivery date.</value>
        DateTime ScheduledDeliveryDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if the message is persisted. default to false
        /// </summary>
        /// <value><c>true</c> if persisted; otherwise, <c>false</c>.</value>
        bool Persisted
        {
            get;
            set;
        }
    }
}
