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

namespace MessagingToolkit.Core.Mobile.Http.Event
{
    /// <summary>
    /// Message received event arguments.
    /// </summary>
    public class NewMessageReceivedEventArgs : EventArgs
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message information</param>
        public NewMessageReceivedEventArgs(DeviceMessage message)
        {
            this.Message = message;
        }


        /// <summary>
        /// Message information property
        /// </summary>
        /// <value>Message information</value>
        public DeviceMessage Message
        {
            get;
            internal set;
        }
    }
}
