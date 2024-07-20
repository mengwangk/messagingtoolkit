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

    /// <summary>
    /// Contains the result of a NetBEUI Framer (NBF) projection operation. This class cannot be inherited.
    /// </summary>
    /// <remarks>This object is created from an <see cref="RasProjectionType.Nbf"/> projection operation on a connection.</remarks>
    [Serializable]
    public sealed class RasNbfInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasNbfInfo"/> class.
        /// </summary>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="netBiosErrorCode">The NetBIOS error code (if any) that occurred.</param>
        /// <param name="netBiosErrorMessage">The NetBIOS error message for the error code that occurred.</param>
        /// <param name="workstationName">The local workstation name.</param>
        /// <param name="lana">The NetBIOS network adapter identifier on which the remote access connection was established.</param>
        internal RasNbfInfo(int errorCode, int netBiosErrorCode, string netBiosErrorMessage, string workstationName, byte lana)
        {
            this.ErrorCode = errorCode;
            this.NetBiosErrorCode = netBiosErrorCode;
            this.NetBiosErrorMessage = netBiosErrorMessage;
            this.WorkstationName = workstationName;
            this.Lana = lana;
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
        /// Gets the NetBIOS error code (if any) that occurred.
        /// </summary>
        public int NetBiosErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the NetBIOS error message for the error code that occurred.
        /// </summary>
        public string NetBiosErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the local workstation name.
        /// </summary>
        public string WorkstationName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the NetBIOS network adapter identifier on which the remote access connection was established.
        /// </summary>
        /// <remarks>This member contains <see cref="Byte.MaxValue"/> if a connection was not established.</remarks>
        public byte Lana
        {
            get;
            private set;
        }

        #endregion
    }
}