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

using MessagingToolkit.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Message to be send to the device throught HTTP POST method.
    /// </summary>
    [Serializable]
    public class PostMessage : MessagingToolkit.Core.Base.BaseMessage, IMessage
    {
        internal static string ParamTo = "to";
        internal static string ParamMessage = "message";

        internal static string ParamDeliveryReport = "deliveryreport";
        internal static string ParamScAddress = "scaddress";
        internal static string ParamSlot = "slot";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PostMessage"/> class.
        /// </summary>
        public PostMessage()
        {
            this.To = string.Empty;
            this.Message = string.Empty;
            this.MessageIdFromDevice = string.Empty;
            this.DeliveryReport = 1;    // Default to receive delivery report
            this.ScAddress = string.Empty;
            this.Slot = string.Empty;
        }

        /// <summary>
        /// Gets or sets recipient contact name or phone number.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the message to be sent.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the delivery report indicator.
        /// <value>
        /// Delivery report indicator.
        /// </value>
        /// </summary>
        public int DeliveryReport { get; set; }

        /// <summary>
        /// Gets or sets the service center address
        /// <value>
        /// Service center address.
        /// </value>
        /// </summary>
        public string ScAddress { get; set; }

        /// <summary>
        /// Gets or sets the SIM slot.
        /// <value>
        /// SIM slot.
        /// </value>
        /// </summary>
        public string Slot { get; set; }

        /// <summary>
        /// Gets unique message identifier from device.
        /// </summary>
        /// <value>
        /// The unique message identifier from device.
        /// </value>
        public string MessageIdFromDevice { get; internal set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            str = String.Concat(str, "QueuePriority = ", QueuePriority, "\r\n");
            str = String.Concat(str, "Identifier = ", Identifier, "\r\n");
            str = String.Concat(str, "ScheduledDeliveryDate = ", ScheduledDeliveryDate, "\r\n");
            str = String.Concat(str, "Persisted = ", Persisted, "\r\n");
            str = String.Concat(str, "To = ", To, "\r\n");
            str = String.Concat(str, "Message = ", Message, "\r\n");
            str = String.Concat(str, "MessageIdFromDevice = ", MessageIdFromDevice, "\r\n");
            str = String.Concat(str, "DeliveryReport = ", DeliveryReport, "\r\n");
            str = String.Concat(str, "ScAddress = ", ScAddress, "\r\n");
            str = String.Concat(str, "Slot = ", Slot, "\r\n");
            return str;
        }

        #region ============== Factory method   ============================================================

        /// <summary>
        /// News the instance.
        /// </summary>
        /// <returns></returns>
        public static PostMessage NewInstance()
        {
            PostMessage message = new PostMessage();
            return message;
        }

        #endregion ========================================================================================

    }
}
