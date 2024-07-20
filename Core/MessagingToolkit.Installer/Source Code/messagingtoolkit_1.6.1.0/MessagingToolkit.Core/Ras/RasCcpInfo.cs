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
    /// Contains the results of a Compression Control Protocol (CCP) projection operation. This class cannot be inherited.
    /// </summary>
    /// <remarks>This object is created from a <see cref="RasProjectionType.Ccp"/> projection operation on a connection.</remarks>
    [Serializable]
    public sealed class RasCcpInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasCcpInfo"/> class.
        /// </summary>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="compressionAlgorithm">The compression algorithm in use by the client.</param>
        /// <param name="options">The compression options on the client.</param>
        /// <param name="serverCompressionAlgorithm">The compression algorithm in use by the server.</param>
        /// <param name="serverOptions">The compression options on the server.</param>
        internal RasCcpInfo(int errorCode, RasCompressionType compressionAlgorithm, RasCompressionOptions options, RasCompressionType serverCompressionAlgorithm, RasCompressionOptions serverOptions)
        {
            this.ErrorCode = errorCode;
            this.CompressionAlgorithm = compressionAlgorithm;
            this.Options = options;
            this.ServerCompressionAlgorithm = serverCompressionAlgorithm;
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
        /// Gets the compression algorithm in use by the client.
        /// </summary>
        public RasCompressionType CompressionAlgorithm
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the compression options on the client.
        /// </summary>
        public RasCompressionOptions Options
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the compression algorithm in use by the server.
        /// </summary>
        public RasCompressionType ServerCompressionAlgorithm
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the compression options on the server.
        /// </summary>
        public RasCompressionOptions ServerOptions
        {
            get;
            private set;
        }

        #endregion
    }
}