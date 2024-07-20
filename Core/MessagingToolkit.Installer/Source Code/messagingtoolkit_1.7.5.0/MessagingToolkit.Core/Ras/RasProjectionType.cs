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
    /// Defines the projection types.
    /// </summary>
    /// <remarks>The projection types defined here are used on a projection operation on an active connection.</remarks>
    public enum RasProjectionType
    {
        /// <summary>
        /// Authentication Message Block (AMB) protocol.
        /// </summary>
        Amb,

        /// <summary>
        /// NetBEUI Framer (NBF) protocol.
        /// </summary>
        Nbf,

        /// <summary>
        /// Internetwork Packet Exchange (IPX) control protocol.
        /// </summary>
        Ipx,

        /// <summary>
        /// Internet Protocol (IP) control protocol.
        /// </summary>
        IP,

        /// <summary>
        /// Compression Control Protocol (CCP).
        /// </summary>
        Ccp,

        /// <summary>
        /// Link Control Protocol (LCP).
        /// </summary>
        Lcp,

        /// <summary>
        /// Serial Line Internet Protocol (SLIP).
        /// <para>
        /// <b>Windows Vista or later:</b> This value is no longer supported.
        /// </para>
        /// </summary>
        Slip,

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Internet Protocol Version 6 (IPv6) control protocol.
        /// <para>
        /// <b>Windows Vista or later:</b> This value is supported.
        /// </para>
        /// </summary>
        IPv6,

#endif

#if (WIN7)

        /// <summary>
        /// Point-to-Point protocol (PPP).
        /// <para>
        /// <b>Windows 7 or later:</b> This value is supported.
        /// </para>
        /// </summary>
        Ppp,

        /// <summary>
        /// Internet Key Exchange (IKEv2) protocol.
        /// <para>
        /// <b>Windows 7 or later:</b> This value is supported.
        /// </para>
        /// </summary>
        IkeV2

#endif
    }
}