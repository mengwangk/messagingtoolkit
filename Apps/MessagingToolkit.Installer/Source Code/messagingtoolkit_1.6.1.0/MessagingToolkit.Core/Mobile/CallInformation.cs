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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Call information
    /// </summary>
    public class CallInformation: IIndicationObject
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">Phone number</param>
        /// <param name="numberType">Number type</param>
        /// <param name="timestamp">Timestamp</param>
        public CallInformation(string number, NumberType numberType, DateTime timestamp)
        {
            this.Number = number;
            this.NumberType = numberType;
            this.Timestamp = timestamp;
            this.GatewayId = string.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">Phone number</param>
        /// <param name="numberType">Number type</param>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="gatewayId">The gateway id.</param>
        public CallInformation(string number, NumberType numberType, DateTime timestamp, string gatewayId)
        {
            this.Number = number;
            this.NumberType = numberType;
            this.Timestamp = timestamp;
            this.GatewayId = gatewayId;
        }

        /// <summary>
        /// Phone Number
        /// </summary>
        /// <value>Phone number</value>
        public string Number
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets or sets the type of the number.
        /// </summary>
        /// <value>The type of the number.</value>
        public NumberType NumberType
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the gateway id.
        /// </summary>
        /// <value>The gateway id.</value>
        public string GatewayId
        {
            get;
            set;
        }
    }
}
