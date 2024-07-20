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
    /// Current call information
    /// </summary>
    public class CurrentCallInformation: IIndicationObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="direction">Call type/direction</param>
        /// <param name="state">Call state</param>
        /// <param name="mode">Call mode</param>
        /// <param name="isMultiParty">Is multiparty call</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="numberType">Number type</param>
        /// <param name="contactName">Contact name</param>
        public CurrentCallInformation(int index, CallType direction, CallState state, CallMode mode, bool isMultiParty, string phoneNumber, NumberType numberType, string contactName)
        {
            this.Index = index;
            this.Direction = direction;
            this.State = state;
            this.Mode = mode;
            this.IsMultiParty = isMultiParty;
            this.Number = phoneNumber;
            this.NumberType = numberType;
            this.ContactName = contactName;
        }

        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Direction
        /// </summary>
        public CallType Direction { get; private set; }

        /// <summary>
        /// Call state
        /// </summary>
        public CallState State { get; private set; }

        /// <summary>
        /// Call mode
        /// </summary>
        public CallMode Mode { get; private set; }

        /// <summary>
        /// Is multiparty call
        /// </summary>
        public bool IsMultiParty { get; private set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string Number { get; private set; }
        
        /// <summary>
        /// Number type
        /// </summary>
        public NumberType NumberType { get; private set; }

        /// <summary>
        /// Contact name in phone book
        /// </summary>
        public string ContactName { get; private set; }


        /// <summary>
        /// Gets or sets the gateway id.
        /// </summary>
        /// <value>The gateway id.</value>
        public string GatewayId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "Index = ", Index.ToString(), "\r\n");
            str = String.Concat(str, "Direction = ", Direction, "\r\n");
            str = String.Concat(str, "State = ", State, "\r\n");
            str = String.Concat(str, "Mode = ", Mode, "\r\n");
            str = String.Concat(str, "IsMultiParty = ", IsMultiParty, "\r\n");
            str = String.Concat(str, "Number = ", Number, "\r\n");
            str = String.Concat(str, "NumberType = ", NumberType, "\r\n");
            str = String.Concat(str, "ContactName = ", ContactName, "\r\n");
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            return str;
        }
    }
}
