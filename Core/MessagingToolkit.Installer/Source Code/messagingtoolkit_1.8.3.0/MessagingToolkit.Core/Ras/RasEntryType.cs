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
    /// Defines the entry types.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "This enum is not a flag.")]
    public enum RasEntryType
    {
        /// <summary>
        /// No entry type specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Phone line.
        /// </summary>
        Phone = 1,

        /// <summary>
        /// Virtual Private Network.
        /// </summary>
        Vpn = 2,

        /// <summary>
        /// Direct serial or parallel connection.
        /// <para>
        /// <b>Windows Vista or later:</b> This value is no longer supported.
        /// </para>
        /// </summary>
        Direct = 3,

        /// <summary>
        /// Connection Manager (CM) connection.
        /// <para>
        /// <b>Note:</b> This member is reserved for system use only.
        /// </para>
        /// </summary>
        Internet = 4,

#if (WINXP || WIN2K8 || WIN7)

        /// <summary>
        /// Broadband connection.
        /// <para>
        /// <b>Windows XP or later:</b> This value is supported.
        /// </para>
        /// </summary>
        Broadband = 5

#endif
    }
}