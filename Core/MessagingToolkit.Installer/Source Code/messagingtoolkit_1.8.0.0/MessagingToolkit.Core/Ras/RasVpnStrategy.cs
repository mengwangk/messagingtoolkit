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
    /// Defines the VPN strategies.
    /// </summary>
    public enum RasVpnStrategy
    {
        /// <summary>
        /// Dials PPTP first. If PPTP fails, L2TP is attempted.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Dial PPTP only.
        /// </summary>
        PptpOnly = 1,

        /// <summary>
        /// Always dial PPTP first.
        /// </summary>
        PptpFirst = 2,

        /// <summary>
        /// Dial L2TP only.
        /// </summary>
        L2tpOnly = 3,

        /// <summary>
        /// Always dial L2TP first.
        /// </summary>
        L2tpFirst = 4,

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Dial SSTP only.
        /// <para>
        /// <b>Windows Vista or later:</b> This value is supported.
        /// </para>
        /// </summary>
        SstpOnly = 5,

        /// <summary>
        /// Always dial SSTP first.
        /// <para>
        /// <b>Windows Vista or later:</b> This value is supported.
        /// </para>
        /// </summary>
        SstpFirst = 6,

#endif

#if (WIN7)

        /// <summary>
        /// Dial IKEv2 only.
        /// <para>
        /// <b>Windows 7 or later:</b> This value is supported.
        /// </para>
        /// </summary>
        IkeV2Only = 7,

        /// <summary>
        /// Dial IKEv2 first.
        /// <para>
        /// <b>Windows 7 or later:</b> This value is supported.
        /// </para>
        /// </summary>
        IkeV2First = 8

#endif
    }
}