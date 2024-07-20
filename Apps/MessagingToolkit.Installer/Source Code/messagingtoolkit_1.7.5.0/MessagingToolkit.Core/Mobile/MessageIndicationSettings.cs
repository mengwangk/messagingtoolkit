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
    /// Message indication settings
    /// </summary>
    internal sealed class MessageIndicationSettings
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="deliver">The deliver.</param>
        /// <param name="cbm">The CBM.</param>
        /// <param name="statusReport">The status report.</param>
        /// <param name="buffer">The buffer.</param>
        public MessageIndicationSettings(MessageIndicationMode mode, ReceivedMessageIndication deliver,
                                        CellBroadcastMessageIndication cbm, StatusReportMessageIndication statusReport,
                                        MessageBufferIndication buffer)
        {
            this.Mode = mode;
            this.ReceivedMessage = deliver;
            this.CellBroadcastMessage = cbm;
            this.StatusReport = statusReport;
            this.MessageBuffer = buffer;
        }


        /// <summary>
        /// Message indication mode
        /// </summary>
        /// <value>Indication mode</value>
        public MessageIndicationMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// SMS deliver indication
        /// </summary>
        /// <value>SMS deliver indication</value>
        public ReceivedMessageIndication ReceivedMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Cell broadcast message indication
        /// </summary>
        /// <value>CBM indication mode</value>
        public CellBroadcastMessageIndication CellBroadcastMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Status report indication
        /// </summary>
        /// <value>Status report indication</value>
        public StatusReportMessageIndication StatusReport
        {
            get;
            set;
        }

        /// <summary>
        /// Message buffer mode
        /// </summary>
        /// <value>Message buffer mode</value>
        public MessageBufferIndication MessageBuffer
        {
            get;
            set;
        }     
    }
}
