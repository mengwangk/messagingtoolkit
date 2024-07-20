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
    /// Contains the result of a Serial Line Internet Protocol (SLIP) projection operation. This class cannot be inherited.
    /// </summary>
    /// <remarks>This object is created from a <see cref="RasProjectionType.Slip"/> projection operation on a connection.</remarks>
    [Serializable]
    public sealed class RasSlipInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasSlipInfo"/> class.
        /// </summary>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="ipAddress">The client IP address on the connection.</param>
        internal RasSlipInfo(int errorCode, IPAddress ipAddress)
        {
            this.ErrorCode = errorCode;
            this.IPAddress = ipAddress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code (if any) that occurred.
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the client IP address on the connection.
        /// </summary>
        public IPAddress IPAddress
        {
            get;
            private set;
        }

        #endregion
    }
}