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

#if (WIN2K8 || WIN7)

    /// <summary>
    /// Contains the result of an IPv6 projection operation. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// <para>    
    /// This object is created from an <see cref="RasProjectionType.IPv6"/> projection operation on a connection.
    /// </para>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows Vista and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class RasIPv6Info
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasIPv6Info"/> class.
        /// </summary>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="localInterfaceIdentifier">The local 64-bit IPv6 interface identifier.</param>
        /// <param name="peerInterfaceIdentifier">The remote 64-bit IPv6 interface identifier.</param>
        /// <param name="localCompressionProtocol">The local compression protocol.</param>
        /// <param name="peerCompressionProtocol">The remote compression protocol.</param>
        internal RasIPv6Info(int errorCode, long localInterfaceIdentifier, long peerInterfaceIdentifier, short localCompressionProtocol, short peerCompressionProtocol)
        {
            this.ErrorCode = errorCode;
            this.LocalInterfaceIdentifier = localInterfaceIdentifier;
            this.PeerInterfaceIdentifier = peerInterfaceIdentifier;
            this.LocalCompressionProtocol = localCompressionProtocol;
            this.PeerCompressionProtocol = peerCompressionProtocol;
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
        /// Gets the local 64-bit IPv6 interface identifier.
        /// </summary>
        public long LocalInterfaceIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the remote 64-bit IPv6 interface identifier.
        /// </summary>
        public long PeerInterfaceIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the local compression protocol.
        /// </summary>
        /// <remarks>Reserved for future use.</remarks>
        public short LocalCompressionProtocol
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the remote compression protocol.
        /// </summary>
        /// <remarks>Reserved for future use.</remarks>
        public short PeerCompressionProtocol
        {
            get;
            private set;
        }

        #endregion
    }

#endif
}