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
    /// Defines the Link Control Protocol (LCP) authentication data types.
    /// </summary>
    public enum RasLcpAuthenticationDataType
    {
        /// <summary>
        /// No authentication data used.
        /// </summary>
        None = 0,

        /// <summary>
        /// MD5 Challenge Handshake Authentication Protocol.
        /// </summary>
        MD5Chap = 0x05,

        /// <summary>
        /// Challenge Handshake Authentication Protocol.
        /// </summary>
        MSChap = 0x80,

        /// <summary>
        /// Challenge Handshake Authentication Protocol version 2.
        /// </summary>
        MSChap2 = 0x81
    }
}