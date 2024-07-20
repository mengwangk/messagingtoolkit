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
    /// Defines the framing protcols.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "The Windows SDK indicates the values are not flags.")]
    public enum RasFramingProtocol
    {
        /// <summary>
        /// No framing protocol specified.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Point-to-point (PPP) protocol.
        /// </summary>
        Ppp = 0x1,

        /// <summary>
        /// Serial Line Internet Protocol (SLIP).
        /// </summary>
        Slip = 0x2,

        /// <summary>
        /// This member is no longer supported.
        /// </summary>
        [Obsolete("This member is no longer supported.", false)]
        Ras = 0x4
    }
}