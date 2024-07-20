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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Statistics class
    /// </summary>
    public class Statistics: BaseClass<Statistics>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Statistics()
        {
            IncomingSms = 0;
            OutgoingSms = 0;
            IncomingMms = 0;
            OutgoingMms = 0;
            IncomingCall = 0;
            OutgoingCall = 0;
        }

        /// <summary>
        /// Count for incoming SMS
        /// </summary>
        /// <value>Incoming SMS count</value>
        public int IncomingSms
        {
            get;
            internal set;
        }

        /// <summary>
        /// Count for outgoing SMS
        /// </summary>
        /// <value>Outgoing SMS count</value>
        public int OutgoingSms
        {
            get;
            internal set;
        }

        /// <summary>
        /// Count for incoming MMS
        /// </summary>
        /// <value>Incoming MMS count</value>
        public int IncomingMms
        {
            get;
            internal set;
        }

        /// <summary>
        /// Count for outgoing MMS
        /// </summary>
        /// <value>Outgoing MMS count</value>
        public int OutgoingMms
        {
            get;
            internal set;
        }


        /// <summary>
        /// Count for incoming call
        /// </summary>
        /// <value>Incoming call</value>
        public int IncomingCall
        {
            get;
            internal set;
        }

        /// <summary>
        /// Count for outgoing call
        /// </summary>
        /// <value>Outgoing call</value>
        public int OutgoingCall
        {
            get;
            internal set;
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
            str = String.Concat(str, "IncomingSms = ", IncomingSms, "\r\n");
            str = String.Concat(str, "OutgoingSms = ", OutgoingSms, "\r\n");
            str = String.Concat(str, "IncomingMms = ", IncomingMms, "\r\n");
            str = String.Concat(str, "OutgoingMms = ", OutgoingMms, "\r\n");
            str = String.Concat(str, "IncomingCall = ", IncomingCall, "\r\n");
            str = String.Concat(str, "OutgoingCall = ", OutgoingCall, "\r\n");
            return str;
        }
    }
}
