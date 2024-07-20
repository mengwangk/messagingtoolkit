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

using MessagingToolkit.Core.Mobile.Http.Event;

namespace MessagingToolkit.Core.Mobile.Http
{
    #region ================= Internal static class ============================================================

    /// <summary>
    /// Service interface definition.
    /// </summary>
    internal static class ServiceInterfaceDefinition
    {
        public static string DateFormat = @"yyyy-MM-dd'T'HH:mm:ss'GMT'zzz";
    }

    #endregion =================================================================================================


    #region ================= Public enum class ================================================================

    /// <summary>
    /// Message memory location
    /// </summary>
    public enum MessageDeliveryStatus
    {
        /// <summary>
        /// Sent
        /// </summary>
        [StringValue("Sent")]
        Sent,
        /// <summary>
        /// Delivered
        /// </summary>
        [StringValue("Delivered")]
        Delivered,
        /// <summary>
        /// Queued
        /// </summary>
        [StringValue("Queued")]
        Queued,
        /// <summary>
        /// Failed
        /// </summary>
        [StringValue("Failed")]
        Failed
    }

    #endregion =================================================================================================


    #region ================= Internal enum class ==============================================================

    /// <summary>
    /// Message read status.
    /// </summary>
    internal enum MessageReadStatus
    {
        /// <summary>
        /// Unread status.
        /// </summary>
        Unread = 0,
        /// <summary>
        /// Read status.
        /// </summary>
        Read = 1
    }

    #endregion =================================================================================================


    #region ================= Public Delegates =================================================================

    /// <summary>
    /// Message received event handler
    /// </summary>
    public delegate void NewMessageReceivedEventHandler(object sender, NewMessageReceivedEventArgs e);


    #endregion =================================================================================================
}
