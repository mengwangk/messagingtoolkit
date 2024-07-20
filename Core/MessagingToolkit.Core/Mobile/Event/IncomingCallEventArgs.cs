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
using System.Text;

namespace MessagingToolkit.Core.Mobile.Event
{
    /// <summary>
    /// Incoming call event arguments
    /// </summary>
    public class IncomingCallEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callInformation">Call information</param>
        public IncomingCallEventArgs(CallInformation callInformation)
        {
            this.CallInformation = callInformation;
        }

        /// <summary>
        /// Call information
        /// </summary>
        /// <value>Call information</value>
        public CallInformation CallInformation
        {
            get;
            internal set;
        }

        /// <summary>
        /// Current call information
        /// </summary>
        /// <value>Current call information</value>
        public CurrentCallInformation CurrentCallInformation
        {
            get;
            internal set;
        }
    }
}