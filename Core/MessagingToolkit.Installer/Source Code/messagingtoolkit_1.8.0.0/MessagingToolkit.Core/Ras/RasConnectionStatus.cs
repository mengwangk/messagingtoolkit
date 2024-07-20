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
    using System.Diagnostics;
    using System.Net;

    /// <summary>
    /// Represents the current status of a remote access connection. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("PhoneNumber = {PhoneNumber}, ConnectionState = {ConnectionState}")]
    public sealed class RasConnectionStatus
    {
        #region Fields

        private RasConnectionState _connectionState;
        private int _errorCode;
        private string _errorMessage;
        private RasDevice _device;
        private string _phoneNumber;
#if (WIN7)
        private IPAddress _localEndPoint;
        private IPAddress _remoteEndPoint;
        private RasConnectionSubState _connectionSubState;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasConnectionStatus"/> class.
        /// </summary>
        internal RasConnectionStatus()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current connection state.
        /// </summary>
        public RasConnectionState ConnectionState
        {
            get { return this._connectionState; }
            internal set { this._connectionState = value; }
        }

        /// <summary>
        /// Gets the error code (if any) that occurred which caused a failed connection attempt.
        /// </summary>
        public int ErrorCode
        {
            get { return this._errorCode; }
            internal set { this._errorCode = value; }
        }

        /// <summary>
        /// Gets the error message for the <see cref="ErrorCode"/> that occurred.
        /// </summary>
        public string ErrorMessage
        {
            get { return this._errorMessage; }
            internal set { this._errorMessage = value; }
        }

        /// <summary>
        /// Gets the device through which the connection has been established.
        /// </summary>
        public RasDevice Device
        {
            get { return this._device; }
            internal set { this._device = value; }
        }

        /// <summary>
        /// Gets the phone number dialed for this specific connection.
        /// </summary>
        public string PhoneNumber
        {
            get { return this._phoneNumber; }
            internal set { this._phoneNumber = value; }
        }

#if (WIN7)

        /// <summary>
        /// Gets the local client endpoint information of a virtual private network (VPN) tunnel.
        /// </summary>
        public IPAddress LocalEndPoint
        {
            get { return this._localEndPoint; }
            internal set { this._localEndPoint = value; }
        }

        /// <summary>
        /// Gets the remote server endpoint information of a virtual private network (VPN) tunnel.
        /// </summary>
        public IPAddress RemoteEndPoint
        {
            get { return this._remoteEndPoint; }
            internal set { this._remoteEndPoint = value; }
        }

        /// <summary>
        /// Gets the state information of an Internet Key Exchange version 2 (IKEv2) virtual private network (VPN) tunnel.
        /// </summary>
        public RasConnectionSubState ConnectionSubState
        {
            get { return this._connectionSubState; }
            internal set { this._connectionSubState = value; }
        }

#endif

        #endregion
    }
}