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

using MessagingToolkit.Core.Mobile.Message;

namespace MessagingToolkit.Core.Mobile.Event
{
    /// <summary>
    /// USSD response received event arguments
    /// </summary>
    public class UssdReceivedEventArgs: EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ussdResponse">The ussd response.</param>
        public UssdReceivedEventArgs(UssdResponse ussdResponse)
        {
            this.UssdResponse = ussdResponse;
        }

        /// <summary>
        /// Gets or sets the ussd response.
        /// </summary>
        /// <value>The ussd response.</value>
        public UssdResponse UssdResponse
        {
            get;
            internal set;
        }
    }
}
