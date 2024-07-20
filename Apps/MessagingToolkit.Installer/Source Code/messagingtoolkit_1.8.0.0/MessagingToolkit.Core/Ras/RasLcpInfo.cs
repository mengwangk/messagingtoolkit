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
    /// Contains the result of a Link Control Protocol (LCP) multilink projection operation. This class cannot be inherited.
    /// </summary>
    /// <remarks>This object is created from an <see cref="RasProjectionType.Lcp"/> projection operation on a connection.</remarks>
    [Serializable]
    public sealed class RasLcpInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasLcpInfo"/> class.
        /// </summary>
        /// <param name="bundled"><b>true</b> if the connection is composed of multiple links, otherwise <b>false</b>.</param>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="authenticationProtocol">The authentication protocol used to authenticate the client.</param>
        /// <param name="authenticationData">The authentication data about the authentication protocol used by the client.</param>
        /// <param name="eapTypeId">The type id of the Extensible Authentication Protocol (EAP) used to authenticate the local computer.</param>
        /// <param name="serverAuthenticatonProtocol">The authentication protocol used to authenticate the server.</param>
        /// <param name="serverAuthenticationData">The authentication data about the authentication protocol used by the server.</param>
        /// <param name="serverEapTypeId">The type id of the Extensible Authentication Protocol (EAP) used to authenticate the remote computer.</param>
        /// <param name="multilink"><b>true</b> if the connection supports multilink, otherwise <b>false</b>.</param>
        /// <param name="terminateReason">The reason the client terminated the connection.</param>
        /// <param name="serverTerminateReason">The reason the server terminated the connection.</param>
        /// <param name="replyMessage">The message (if any) from the authentication protocol success/failure packet.</param>
        /// <param name="options">The additional options for the local computer.</param>
        /// <param name="serverOptions">The additional options for the remote computer.</param>
        internal RasLcpInfo(bool bundled, int errorCode, RasLcpAuthenticationType authenticationProtocol, RasLcpAuthenticationDataType authenticationData, int eapTypeId, RasLcpAuthenticationType serverAuthenticatonProtocol, RasLcpAuthenticationDataType serverAuthenticationData, int serverEapTypeId, bool multilink, int terminateReason, int serverTerminateReason, string replyMessage, RasLcpOptions options, RasLcpOptions serverOptions)
        {
            this.Bundled = bundled;
            this.ErrorCode = errorCode;
            this.AuthenticationProtocol = authenticationProtocol;
            this.AuthenticationData = authenticationData;
            this.EapTypeId = eapTypeId;
            this.ServerAuthenticationProtocol = serverAuthenticatonProtocol;
            this.ServerAuthenticationData = serverAuthenticationData;
            this.ServerEapTypeId = serverEapTypeId;
            this.Multilink = multilink;
            this.TerminateReason = terminateReason;
            this.ServerTerminateReason = serverTerminateReason;
            this.ReplyMessage = replyMessage;
            this.Options = options;
            this.ServerOptions = serverOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the connection is composed of multiple links.
        /// </summary>
        public bool Bundled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error code (if any) that occurred.
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the authentication protocol used to authenticate the client.
        /// </summary>
        public RasLcpAuthenticationType AuthenticationProtocol
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the authentication data about the authentication protocol used by the client.
        /// </summary>
        public RasLcpAuthenticationDataType AuthenticationData
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type id of the Extensible Authentication Protocol (EAP) used to authenticate the local computer.
        /// </summary>
        /// <remarks>This member is valid only if <see cref="RasLcpInfo.AuthenticationProtocol"/> is <see cref="RasLcpAuthenticationType.Eap"/>.</remarks>
        public int EapTypeId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the authentication protocol used to authenticate the server.
        /// </summary>
        public RasLcpAuthenticationType ServerAuthenticationProtocol
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the authentication data about the authentication protocol used by the server.
        /// </summary>
        public RasLcpAuthenticationDataType ServerAuthenticationData
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type id of the Extensible Authentication Protocol (EAP) used to authenticate the remote computer.
        /// </summary>
        /// <remarks>This member is valid only if <see cref="RasLcpInfo.ServerAuthenticationProtocol"/> is <see cref="RasLcpAuthenticationType.Eap"/>.</remarks>
        public int ServerEapTypeId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the connection supports multilink.
        /// </summary>
        public bool Multilink
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the reason the client terminated the connection.
        /// </summary>
        /// <remarks>This member always has a return value of zero.</remarks>
        public int TerminateReason
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the reason the server terminated the connection.
        /// </summary>
        /// <remarks>This member always has a return value of zero.</remarks>
        public int ServerTerminateReason
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message (if any) from the authentication protocol success/failure packet.
        /// </summary>
        public string ReplyMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the additional options for the local computer.
        /// </summary>
        public RasLcpOptions Options
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the additional options for the remote computer.
        /// </summary>
        public RasLcpOptions ServerOptions
        {
            get;
            private set;
        }

        #endregion
    }
}