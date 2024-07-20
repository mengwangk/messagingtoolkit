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
    /// Defines the Link Control Protocol (LCP) authentication protocol types.
    /// </summary>
    public enum RasLcpAuthenticationType
    {
        /// <summary>
        /// No authentication protocol used.
        /// </summary>
        None = 0,

        /// <summary>
        /// Password Authentication Protocol.
        /// </summary>
        Pap = 0xC023,

        /// <summary>
        /// Shiva Password Authentication Protocol.
        /// </summary>
        Spap = 0xC027,

        /// <summary>
        /// Challenge Handshake Authentication Protocol.
        /// </summary>
        Chap = 0xC223,

        /// <summary>
        /// Extensible Authentication Protocol.
        /// </summary>
        Eap = 0xC227
    }
}