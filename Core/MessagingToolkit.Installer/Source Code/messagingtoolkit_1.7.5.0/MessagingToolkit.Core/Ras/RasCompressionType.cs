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
    /// Defines the remote access service (RAS) compression algorithms.
    /// </summary>
    public enum RasCompressionType
    {
        /// <summary>
        /// No compression in use.
        /// </summary>
        None = 0x0,
        
        /// <summary>
        /// STAC option 4.
        /// </summary>
        Stac = 0x5,

        /// <summary>
        /// Microsoft Point-to-Point Compression (MPPC) protocol.
        /// </summary>
        Mppc = 0x6
    }
}