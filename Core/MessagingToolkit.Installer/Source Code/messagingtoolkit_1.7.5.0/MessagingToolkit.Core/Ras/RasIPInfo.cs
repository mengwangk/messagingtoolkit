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
namespace MessagingToolkit.Core.Ras
{
    using System;
    using System.Net;

    /// <summary>
    /// Contains the result of an IP projection operation. This class cannot be inherited.
    /// </summary>
    /// <remarks>This object is created from an <see cref="RasProjectionType.IP"/> projection operation on a connection.</remarks>
    [Serializable]
    public sealed class RasIPInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasIPInfo"/> class.
        /// </summary>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="ipAddress">The client IP address.</param>
        /// <param name="serverIPAddress">The server IP address.</param>
        /// <param name="options">The IPCP options for the local computer.</param>
        /// <param name="serverOptions">The IPCP options for the remote computer.</param>
        internal RasIPInfo(int errorCode, IPAddress ipAddress, IPAddress serverIPAddress, RasIPOptions options, RasIPOptions serverOptions)
        {
            this.ErrorCode = errorCode;
            this.IPAddress = ipAddress;
            this.ServerIPAddress = serverIPAddress;
            this.Options = options;
            this.ServerOptions = serverOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code (if any) that occurred.
        /// </summary>
        /// <remarks>This member indicates the actual fatal error (if any) that occurred during the control protocol negotiation, the error that prevented the projection from completing successfully.</remarks>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the client IP address.
        /// </summary>
        public IPAddress IPAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the server IP address.
        /// </summary>
        public IPAddress ServerIPAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the IPCP options for the local computer.
        /// </summary>
        public RasIPOptions Options
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the IPCP options for the remote computer.
        /// </summary>
        public RasIPOptions ServerOptions
        {
            get;
            private set;
        }

        #endregion
    }
}