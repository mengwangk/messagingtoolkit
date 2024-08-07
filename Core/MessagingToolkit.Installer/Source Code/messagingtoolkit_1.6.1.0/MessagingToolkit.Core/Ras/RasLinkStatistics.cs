﻿//===============================================================================
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
    /// Represents connection link statistics for a remote access connection. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class RasLinkStatistics
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasLinkStatistics"/> class.
        /// </summary>
        /// <param name="bytesTransmitted">The number of bytes transmitted.</param>
        /// <param name="bytesReceived">The number of bytes received.</param>
        /// <param name="framesTransmitted">The number of frames transmitted.</param>
        /// <param name="framesReceived">The number of frames received.</param>
        /// <param name="crcError">The number of cyclic redundancy check (CRC) errors that have occurred.</param>
        /// <param name="timeoutError">The number of timeout errors that have occurred.</param>
        /// <param name="alignmentError">The number of alignment errors that have occurred.</param>
        /// <param name="hardwareOverrunError">The number of hardware overrun errors that have occurred.</param>
        /// <param name="framingError">The number of framing errors that have occurred.</param>
        /// <param name="bufferOverrunError">The number of buffer overrun errors that have occurred.</param>
        /// <param name="compressionRatioIn">The compression ratio for data received on this connection or link.</param>
        /// <param name="compressionRatioOut">The compression ratio for data transmitted on this connection or link.</param>
        /// <param name="linkSpeed">The speed of the link, in bits per second.</param>
        /// <param name="connectionDuration">The length of time that the connection has been connected.</param>
        internal RasLinkStatistics(long bytesTransmitted, long bytesReceived, long framesTransmitted, long framesReceived, long crcError, long timeoutError, long alignmentError, long hardwareOverrunError, long framingError, long bufferOverrunError, long compressionRatioIn, long compressionRatioOut, long linkSpeed, TimeSpan connectionDuration)
        {
            this.BytesTransmitted = bytesTransmitted;
            this.BytesReceived = bytesReceived;
            this.FramesTransmitted = framesTransmitted;
            this.FramesReceived = framesReceived;
            this.CrcError = crcError;
            this.TimeoutError = timeoutError;
            this.AlignmentError = alignmentError;
            this.HardwareOverrunError = hardwareOverrunError;
            this.FramingError = framingError;
            this.BufferOverrunError = bufferOverrunError;
            this.CompressionRatioIn = compressionRatioIn;
            this.CompressionRatioOut = compressionRatioOut;
            this.LinkSpeed = linkSpeed;
            this.ConnectionDuration = connectionDuration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of bytes transmitted.
        /// </summary>
        public long BytesTransmitted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        public long BytesReceived
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of frames transmitted.
        /// </summary>
        public long FramesTransmitted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of frames received.
        /// </summary>
        public long FramesReceived
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of cyclic redundancy check (CRC) errors that have occurred.
        /// </summary>
        public long CrcError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of timeout errors that have occurred.
        /// </summary>
        public long TimeoutError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of alignment errors that have occurred.
        /// </summary>
        public long AlignmentError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of hardware overrun errors that have occurred.
        /// </summary>
        public long HardwareOverrunError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of framing errors that have occurred.
        /// </summary>
        public long FramingError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of buffer overrun errors that have occurred.
        /// </summary>
        public long BufferOverrunError
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the compression ratio for data received on this connection or link.
        /// </summary>
        /// <remarks>This member is valid only for a single link connection, or a single link in a multilink connection.</remarks>
        public long CompressionRatioIn
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the compression ratio for data transmitted on this connection or link.
        /// </summary>
        /// <remarks>This member is valid only for a single link connection, or a single link in a multilink connection.</remarks>
        public long CompressionRatioOut
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the speed of the link, in bits per second.
        /// </summary>
        public long LinkSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length of time that the connection has been connected.
        /// </summary>
        public TimeSpan ConnectionDuration
        {
            get;
            private set;
        }

        #endregion
    }
}